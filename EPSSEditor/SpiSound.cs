using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using NAudio.Wave;
using System.IO;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace EPSSEditor
{
  
    public class SpiSound : IDisposable
    {
        public byte midiChannel; // [1-16]
        public byte midiNote;

        public byte startNote;
        public byte endNote;
        public byte midiNoteMapped;
        public byte programNumber;
        public sbyte transpose;
        public byte vvfe;
        public UInt16 s_gr_frek;

        public Guid soundId;
        public string _name;
        public string _extName;

        public byte loopMode;
        public UInt32 start;
        public UInt32 end;
        public UInt32 loopStart;

        public UInt16 extVolume;
        public UInt16 subTone;


        private MemoryStream _ms = null;
        private BlockAlignReductionStream _blockAlignedStream = null;
        private CachedSound _cachedAudio;

        public SpiSound() {
            startNote = endNote = programNumber = 128;
            midiNoteMapped = 84;
            transpose = 0;
            
        }

        public SpiSound(ref Sound sound)
        {
            startNote = endNote = programNumber = 128;
            midiNoteMapped = 84;
            soundId = sound.id();
            SetNameFromSound(sound);
            transpose = 0;
        }
        
        
        public SpiSound(ref Sound sound, SfzSplitInfo sfz) // Used when importing from SPI
        {
            midiNoteMapped = 84;
            soundId = sound.id();
            SetNameFromSound(sound);
 
            midiChannel = (byte)(sfz.Midich + 1);
            startNote = (byte)sfz.NoteStart;
            endNote = (byte)sfz.NoteEnd;
            int center = sfz.NoteStart + 84 - sfz.Low;
            midiNote = (byte)(84 - (center - sfz.NoteStart));
            transpose = (sbyte)sfz.Transpose;
            vvfe = (byte)sfz.Vvfe;
            s_gr_frek = sfz.S_gr_frek;

            loopMode = (byte)sfz.Loopmode;
            start = sfz.Start;
            end = sfz.End;
            loopStart = sfz.LoopStart;

            extVolume = sfz.ExtVolume;
            subTone = sfz.SubTone;
        }

        public SpiSound(EPSSSpi_soundInfo soundInfo, EPSSSpi_extSoundInfo extSoundInfo, EPSSSpi_sample soundData)
        {
            _name = extSoundInfo.s_sampname;
            _extName = extSoundInfo.s_extname;
            transpose = soundInfo.s_loopmode.toneoffset;
            // TODO
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


        public string description(ref EPSSEditorData data)
        {
            StringBuilder s = new StringBuilder();
            s.Append(midiChannel);
            s.Append("    ");
            s.Append(midiNote);
            s.Append("    ");
            int i = data.getSoundNumberFromGuid(soundId);

            s.Append(i);
            s.Append("    ");
            s.Append(_name);
            return s.ToString();
          
        }

        public long preLength(ref EPSSEditorData data)
        {
            Sound sound = data.getSoundFromSoundId(soundId);

            if (sound != null)
            {
                return sound.parameters().sizeAfterConversion(ref sound);
            }
            return 0;
        }
       
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


        public bool convertSound(EPSSEditorData data, string outFile, int newFreq, int bits, int channels)
        {
            bool result = true;

            try
            {
                Sound sound = data.getSoundFromSoundId(soundId);
                if (sound != null)
                {
                    float volume = 1.0f;
                    float max = 1.0f;
                    getNormalizeValues(ref sound, ref volume, ref max);

                    string volTempPath = System.IO.Path.GetTempFileName();

                    using (var reader = new AudioFileReader(sound.path))
                    {
                        // rewind and amplify
                        reader.Position = 0;
                        //                    reader.Volume = 1.0f / max;
                       reader.Volume = volume / max;

    

                        var outFormat = new WaveFormat(newFreq, reader.WaveFormat.Channels);
                        // TODO use the pitch info to pitch it.
                        // https://gitee.com/weivyuan/NAudio/tree/master/Docs  SmbPitchShiftingSampleProvider
                        // Need to use the varispeed converter!
                        using (var resampler = new MediaFoundationResampler(reader, outFormat))
                        {
                            resampler.ResamplerQuality = 60;
                            WaveFileWriter.CreateWaveFile(volTempPath, resampler);
                        }

                    }

                    using (var reader = new WaveFileReader(volTempPath))
                    {

                        var newFormat = new WaveFormat(newFreq, bits, channels);
                        using (var conversionStream = new WaveFormatConversionStream(newFormat, reader))
                        {
                            WaveFileWriter.CreateWaveFile(outFile, conversionStream);
                        }
                    }

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
                        using (WaveFileWriter writer = new WaveFileWriter(outFile, waveFormat))
                        {
                            writer.Write(spl.ToArray(), 0, (int)len);
                        }
                    }

                    System.IO.File.Delete(volTempPath);
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                result = false;
            }



            return result;
        }


        public MemoryStream getWaveStream(EPSSEditorData data, int newFreq, int bits, int channels)
        {
            if (_ms == null)
            {
                string outFile = data.convertSoundFileName();

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


        public CachedSound cachedSound(MemoryStream ms, int newFreq, int bits, int channels)
        {
            if (_cachedAudio == null)
            {
                _cachedAudio = new CachedSound(ms, newFreq, bits, channels);
            }
            return _cachedAudio;
        }


        public string transposeString()
        {
            if (transpose == 0) return "0";
            else if (transpose > 0) return "+" + transpose.ToString();
            else return transpose.ToString();
        }


        public byte transposedNote(byte original)
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
    }
}
