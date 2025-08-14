using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using NAudio.Wave;
using System.IO;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using System.Windows.Forms;
using EPSSEditor.Vorbis;
using static System.Net.Mime.MediaTypeNames;
using NAudio.CoreAudioApi;
using System.ComponentModel;
using System.Security.AccessControl;
using System.Runtime.Remoting.Messaging;

namespace EPSSEditor
{

    public class SpiSound : IDisposable, ICloneable, IEquatable<SpiSound>, IComparable<SpiSound>, INotifyPropertyChanged
    {
        public byte midiChannel; // [1-16]
        public byte midiNote; // [0-127] This is the midi note where the sound is played at pitch 0. center note

        public byte startNote; // [0-127] 128:not defined
        public byte endNote; // [0-127] 128:not defined

        public byte programNumber; // [0-127]  128:not defined
        
        private sbyte _transpose; // [-64, 63] 0 = no transpose, -64 = -5 octaves, 63 = +5 octaves (check EPSSSpi_soundInfo for details)

        [Browsable(false)]
        public sbyte transpose
        {
            get => _transpose;
            set
            {
                if (_transpose != value)
                {
                    _transpose = value;
                    OnPropertyChanged((nameof(transpose)));
                }
            }
        }



        public byte vvfe;
        public UInt16 s_gr_frek;

        public Guid soundId;
        public string _name;
        public string _extName;

        public byte loopMode; // EPSS, 1 -> single shot, 2 -> loop
        public UInt32 start;
        public UInt32 end;

        // These are mirrored with Sound, i.e. *NOT* converted depending on frequency changes, bit changes etc.
        public UInt32 loopStart;
        public UInt32 loopEnd;

        public UInt16 extVolume;
        public UInt16 subTone;

        private MemoryStream _ms = null;
        private UInt32 _msLoopStart; // Values present when sound is converted
        private UInt32 _msLoopEnd;
        private BlockAlignReductionStream _blockAlignedStream = null;
        private CachedSound _cachedAudio;

        private EPSSEditorData _parent;

        public SpiSound()
        {
            startNote = endNote = programNumber = 128;
            //midiNoteMapped = 84;
            transpose = 0;

        }

        public SpiSound(Sound sound)
        {
            startNote = endNote = programNumber = 128;
            //midiNoteMapped = 84;
            soundId = sound.id();
            SetNameFromSound(sound);
            transpose = 0;

            end = (UInt32)sound.parameters().sizeAfterConversion(sound);

            loopMode = (byte)(sound.loop ? 2 : 1);
            if (sound.loop)
            {
                loopMode = 2;
                loopStart = (UInt32)sound.loopStart; //Initially from original Sound based on Sound freq, channels, bits
                loopEnd = (UInt32)sound.loopEnd;
            }
            else
            {
                loopMode = 1;
                loopStart = 0;
                loopEnd = 0;
            }

            extVolume = 100;
            midiNote = 84;
        }


        public SpiSound(Sound sound, SfzSplitInfo sfz) // Used when importing from SPI
        {
            //midiNoteMapped = 84;
            soundId = sound.id();
            SetNameFromSound(sound);

            midiChannel = (byte)(sfz.Midich + 1);
            startNote = (byte)sfz.NoteStart;
            endNote = (byte)sfz.NoteEnd;
            int center = sfz.Low - sfz.NoteStart;
            midiNote = (byte)(84 - center);
            transpose = (sbyte)sfz.Transpose;
            vvfe = (byte)sfz.Vvfe;
            s_gr_frek = sfz.S_gr_frek;

            loopMode = (byte)sfz.Loopmode;
            start = sfz.Start;
            end = sfz.End;
            loopStart = sfz.LoopStart;
            loopEnd = sfz.LoopEnd;

            extVolume = sfz.ExtVolume;
            subTone = sfz.SubTone;

            programNumber = (byte)sfz.ProgramChange;
        }

        public SpiSound(EPSSSpi_soundInfo soundInfo, EPSSSpi_extSoundInfo extSoundInfo, EPSSSpi_sample soundData)
        {
            _name = extSoundInfo.s_sampname;
            _extName = extSoundInfo.s_extname;
            transpose = soundInfo.s_loopmode.Toneoffset;
            // TODO
        }



        public virtual object Clone()
        {
            return this.MemberwiseClone();
        }

        public void Dispose()
        {
            if (_blockAlignedStream != null)
            {
                _blockAlignedStream.Close();
                _blockAlignedStream.Dispose();
                _blockAlignedStream = null;
            }
            if (_ms != null)
            {
                _ms.Close();
                _ms.Dispose();
            }
        }

        public string name() { return _name; }
        public string extName() { return _extName; }

        public void setParent(EPSSEditorData data)
        {
            _parent = data;
        }


        public string description(EPSSEditorData data)
        {
            if (_parent == null)
            {
                return "";
            }
            StringBuilder s = new StringBuilder();
            s.Append(midiChannel);
            s.Append("    ");
            s.Append(midiNote);
            s.Append("    ");
            int i = data.GetSoundNumberFromGuid(soundId);

            s.Append(i);
            s.Append("    ");
            s.Append(_name);
            return s.ToString();

        }

        public long preLength(EPSSEditorData data)
        {
            if (_parent == null)
            {
                return 0;
            }
            Sound sound = data.GetSoundFromSoundId(soundId);

            if (sound != null)
            {
                return sound.parameters().sizeAfterConversion(sound);
            }
            return 0;
        }

        /* TODO?
        private void getNormalizeValues(ref Sound sound, ref float volume, ref float max)
        {
            volume = 1.0f;
            max = 1.0f;
            if (sound.parameters().normalize.normalize)
            {
                volume = (float)(sound.parameters().normalize.normalizePercentage / 100.0);

                max = 0;
                using (var reader = new AudioFileReader(sound.path))
                {
                    // find the max peak
                    float[] buffer = new float[reader.WaveFormat.SampleRate];
                    int read;
                    do
                    {
                        read = reader.Read(buffer, 0, buffer.Length);
                        for (int n = 0; n < read; n++)
                        {
                            var abs = Math.Abs(buffer[n]);
                            if (abs > max) max = abs;
                        }
                    } while (read > 0);
                }
            
            }
        }
        */


        public bool convertSound(EPSSEditorData data, string outFile, int newFreq, int bits, int channels)
        {
            bool result = true;
            UInt32 newLs = loopStart;
            UInt32 newLe = loopEnd;

            try
            {
                Sound sound = data.GetSoundFromSoundId(soundId);
                if (sound != null)
                {
                    //float volume = 1.0f;
                    //float max = 1.0f;
                    //getNormalizeValues(ref sound, ref volume, ref max);
                    bool loop = loopMode == 2;
                    double lengthChangeFact;

                    string volTempPath = System.IO.Path.GetTempFileName();
                    volTempPath = Path.ChangeExtension(volTempPath, ".wav");

                    string ext = Path.GetExtension(sound.path).ToLower();
                    if (ext == ".ogg")
                    {
                        using (VorbisWaveReader reader = new VorbisWaveReader(sound.path))
                        {
                            // rewind and amplify
                            reader.Position = 0;
                            //                    reader.Volume = 1.0f / max;
                            //reader.Volume = volume / max;
                            //reader.Volume = (float)(extVolume) / 100.0f;

                            var outFormat = new WaveFormat(newFreq, reader.WaveFormat.Channels);
                            // TODO use the pitch info to pitch it.
                            // https://gitee.com/weivyuan/NAudio/tree/master/Docs  SmbPitchShiftingSampleProvider
                            // Need to use the varispeed converter!
                            using (var resampler = new MediaFoundationResampler(reader, outFormat))
                            {
                                resampler.ResamplerQuality = 60; // Highest quality

                                lengthChangeFact = (double)newFreq / (double)reader.WaveFormat.SampleRate;
                                //lengthFactor *= (double)(16.0 / (double)reader.WaveFormat.BitsPerSample);
                                newLs = (UInt32)((double)newLs * lengthChangeFact);
                                newLe = (UInt32)((double)newLe * lengthChangeFact);

                                WaveLoopFileWriter.CreateWaveLoopFile(volTempPath, resampler, loop, newLs, newLe);
                            }
                        }
                    }
                    else
                    {
                        using (var reader = new AudioFileReader(sound.path))
                        {
                            // rewind and amplify
                            reader.Position = 0;
                            //                    reader.Volume = 1.0f / max;
                            //reader.Volume = volume / max;
                            reader.Volume = (float)(extVolume) / 100.0f;

                            var outFormat = new WaveFormat(newFreq, reader.WaveFormat.Channels);
                            // TODO use the pitch info to pitch it.
                            // https://gitee.com/weivyuan/NAudio/tree/master/Docs  SmbPitchShiftingSampleProvider
                            // Need to use the varispeed converter!
                            using (var resampler = new MediaFoundationResampler(reader, outFormat))
                            {
                                resampler.ResamplerQuality = 60; // Highest quality

                                lengthChangeFact = (double)newFreq / (double)reader.WaveFormat.SampleRate;
                                //lengthFactor *= (double)(16.0 / (double)reader.WaveFormat.BitsPerSample);
                                newLs = (UInt32)((double)newLs * lengthChangeFact);
                                newLe = (UInt32)((double)newLe * lengthChangeFact);

                                WaveLoopFileWriter.CreateWaveLoopFile(volTempPath, resampler, loop, newLs, newLe);
                            }
                            Console.WriteLine($"Temporary file:{volTempPath}");
                        }
                    }




                    using (var reader = new WaveFileReader(volTempPath))
                    {

                        var newFormat = new WaveFormat(newFreq, bits, channels);
                        using (var conversionStream = new WaveFormatConversionStream(newFormat, reader))
                        {
                            // no loop recalc should be needed as frequences are correct and loop defines samples, not bytes
                            WaveLoopFileWriter.CreateWaveLoopFile(outFile, conversionStream, loop, newLs, newLe);
                        }
                        Console.WriteLine($"Outfile: {outFile}");
                    }

                    _msLoopStart = newLs;
                    _msLoopEnd = newLe;

                    /*
                    bool destroyMore = false;

                    if (destroyMore)
                    {
                        List<byte> spl = new List<byte>();
                        long len = 0;
                        using (var wav = File.OpenRead(outFile))
                        {
                            wav.Seek(0, SeekOrigin.Begin);
                            using (var fr = new WaveFileReader(wav))
                            {
                                len = fr.Length;

                                byte[] buffer = new byte[len];
                                int rd = fr.Read(buffer, 0, (int)len);

                                foreach (byte b in buffer)
                                {
                                    byte newB = b;
                                    spl.Add(newB);
                                }
                            }
                         }

                        WaveFormat waveFormat = new WaveFormat(newFreq, bits, channels);
                        using (WaveLoopFileWriter writer = new WaveLoopFileWriter(outFile, waveFormat, sound.loop, sound.loopStart, sound.loopEnd))
                        {
                            writer.Write(spl.ToArray(), 0, (int)len);
                        }
                    }
                    */

                    System.IO.File.Delete(volTempPath);
                }
                else
                {
                    MessageBox.Show("Fatal error, not matching sound found!");
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine("convertSound exception: {0}", ex.ToString());
                result = false;
            }



            return result;
        }


        public MemoryStream getWaveStream(EPSSEditorData data, int newFreq, int bits, int channels)
        {
            if (_ms == null)
            {
                string outFile = data.ConvertSoundFileName();

                if (convertSound(data, outFile, newFreq, bits, channels))
                {
                    _ms = new MemoryStream();
                    using (FileStream file = new FileStream(outFile, FileMode.Open, FileAccess.Read))
                    {

                        byte[] bytes = new byte[file.Length];
                        file.Read(bytes, 0, (int)file.Length);
                        _ms.Write(bytes, 0, (int)file.Length);
                    }

                }
            }
            return _ms;
        }


        public UInt32 MsLoopStart() { return _msLoopStart; }


        public UInt32 MsLoopEnd() { return _msLoopEnd; }


        /*
        public WaveStream waveStream()
        {
            if (_ms != null)
            {
                if (_blockAlignedStream != null)
                {
                    _blockAlignedStream.Close();
                    _blockAlignedStream.Dispose();
                    _blockAlignedStream = null;
                }
                _blockAlignedStream = new BlockAlignReductionStream(
                                        WaveFormatConversionStream.CreatePcmStream(
                                        new WaveFileReader(_ms)));
              
                 
                return _blockAlignedStream;
            }
            return null;
        }
        */


        public CachedSound cachedSound()
        {
            return _cachedAudio;
        }


        public CachedSound cachedSound(MemoryStream ms, bool loop, UInt32 ls, UInt32 le, float pan)
        {
            if (_cachedAudio == null)
            {
                _cachedAudio = new CachedSound(ms, loop, ls, le, pan);
            }
            return _cachedAudio;
        }


        public int CenterNote()
        {
            //return startNote + 84 - midiNote - transpose + 24;
            return Math.Min(127, Math.Max(0, midiNote - transpose));
        }

        public string TransposeString()
        {
            if (transpose == 0) return "0";
            else if (transpose > 0) return "+" + transpose.ToString();
            else return transpose.ToString();
        }


        public byte TransposedNote(byte original)
        {
            int newNote = (int)original + transpose; // normal pitch + transpose
            if (newNote > 128) newNote = 128;
            else if (newNote < 0) newNote = 0;
            return (byte)newNote;
        }

        public void SetNameFromSound(Sound sound)
        {
            _name = sound.name();
            _extName = sound.name() + " MSWav"; // TODO, find more info in sound??         

        }

        public string VvfeString()
        {
            switch (vvfe)
            {
                case 0x3b: return "x1";
                case 0x3c: return "x2";
                case 0x3d: return "x4";
                case 0x3e: return "x8";
                case 0x3f: return "x16";
                case 0: return "x32";
                case 1: return "x64";
                case 2: return "x128";
                default: return "---";
            }
        }

        public override bool Equals(object obj)
        {
            if (obj == null) return false;
            SpiSound objAsSnd = obj as SpiSound;
            if (objAsSnd == null) return false;
            else return Equals(objAsSnd);
        }

        public bool Equals(SpiSound other)
        {
            if (other == null) return false;
            return (this.midiChannel.Equals(other.midiChannel) &&
                this.midiNote.Equals(other.midiNote) &&
                this.startNote.Equals(other.startNote) &&
                this.endNote.Equals(other.endNote) &&
                this.programNumber.Equals(other.programNumber) &&
                this.soundId.Equals(other.soundId));
        }

        public int CompareTo(SpiSound compareSnd)
        {
            // A null value means that this object is greater.
            if (compareSnd == null)
                return 1;
            else
            {
                if (this.programNumber == compareSnd.programNumber)
                {
                    if (this.midiChannel == compareSnd.midiChannel)
                    {
                        if (this.startNote == compareSnd.startNote)
                        {
                            if (this.endNote == compareSnd.endNote)
                            {
                                return _name.CompareTo(compareSnd._name);
                            }
                            else return this.endNote.CompareTo(compareSnd.endNote);
                        }
                        else return this.startNote.CompareTo(compareSnd.startNote);
                    }
                    else return this.midiChannel.CompareTo(compareSnd.midiChannel);
                }
                else return this.programNumber.CompareTo(compareSnd.programNumber);
            }
        }


        [DisplayName("MIDI"), Width(35)]
        public string MidiChannel
        {
            get
            {
                if (midiChannel <= 16)
                {
                    return midiChannel.ToString();
                }
                return "-";
            }
        }

        [DisplayName("Note"), Width(55)]
        public string MidiNote
        {
            get {
                if (startNote < 128 && endNote < 128)
                {
                    return startNote.ToString() + "-" + endNote.ToString();
                }
                return midiNote.ToString();
            }
        }

        [DisplayName("Program"), Width(55)]
        public string Program
        {
            get {
                if (programNumber < 128)
                {
                    return (programNumber + 1).ToString();
                }
                return "-";
            }
        }


        [DisplayName("Sound"), Width(100)]
        public string SoundName
        {
            get { return _name; }
            //set { _name = value;  }
        }

        [DisplayName("#"), Width(40)]
        public string SoundNumber
        {
            get {
                if (_parent == null)
                {
                    return "-";
                }

                return _parent.GetSoundNumberFromGuid(soundId).ToString();
                //return soundId.ToString(); // TODO needs global data to be able to map to number
                //item.SubItems.Add(nr.ToString());
            }

        }


        [DisplayName("Size"), Width(55)]
        public string Size
        {
            get {

                return Ext.ToPrettySize(preLength(_parent), 2).ToString();
            }
        }

        [DisplayName("Transpose"), Width(55)]
        public string Transpose
        {
            get
            {
                return TransposeString();
            }
        }

        [DisplayName("Vvfe"), Width(35)]
        public string Vvfe
        {
            get
            {
                return VvfeString();
            }
        }

        private EPSSSpi_s_gr_frek S_gr_frek()
        {
            return new EPSSSpi_s_gr_frek
            {
                data = s_gr_frek
            };
        }



        [DisplayName("Drum"), Width(20)]
        public string Drum
        {
            get
            {
                return S_gr_frek().Drum.ToString();
            }
        }

        [DisplayName("Velocity"), Width(20)]
        public string Velocity
        {
            get
            {
                return S_gr_frek().Velocity.ToString();
            }
        }

        [DisplayName("SoundType"), Width(20)]
        public string SoundType
        {
            get
            {
                return S_gr_frek().SoundType.ToString();
            }
        }
        [DisplayName("Mode"), Width(20)]
        public string Mode
        {
            get
            {
                return S_gr_frek().Mode.ToString();
            }
        }
        [DisplayName("Aftertouch"), Width(20)]
        public string Aftertouch
        {
            get
            {
                return S_gr_frek().Aftertouch.ToString();
            }
        }

        [DisplayName("StereoType"), Width(20)]
        public string StereoType
        {
            get
            {
                return S_gr_frek().StereoType.ToString();
            }
        }
        [DisplayName("StereoPan"), Width(20)]
        public string StereoPan
        {
            get
            {
                return S_gr_frek().StereoPan.ToString();
            }
        }
        [DisplayName("OrgFreq"), Width(20)]
        public string OrgFreq
        {
            get
            {
                return S_gr_frek().OrgFreq.ToString();
            }
        }

        [DisplayName("Start"), Width(65)]
        public string Start
        {
            get
            {
                return start.ToString("X8");
            }
        }

        private Sound Sound()
        {
            if (_parent == null)
            {
                return null;
            }

            return _parent.GetSoundFromSoundId(soundId);
        }

        [DisplayName("Loop"), Width(65)]
        public string Loop
        {
            get
            {
                Sound s = Sound();
                if (s == null) return "";
                ConversionParameters p = s.parameters();

                uint ls = loopStart;
                if (p != null)
                { 
                    if (p.freq != null)
                    {
                        ls = (uint)(p.freq.compressionFactor() * loopStart);
                    }
                }

                return ls.ToString("X8");
            }
       
        }

        [DisplayName("End"), Width(65)]
        public string End
        { get { return end.ToString("X8"); } }

        [DisplayName("LoopMode"), Width(20)]
        public string LoopMode
        { get { return loopMode.ToString(); } }

        [DisplayName("Volume"), Width(40)]
        public string Volume
        { get { return extVolume.ToString(); } }

        [DisplayName("SubTone"), Width(40)]
        public string SubTone
        { get { return subTone.ToString(); } }

        
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        

    }
}
