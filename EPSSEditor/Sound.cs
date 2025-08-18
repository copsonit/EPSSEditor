using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using NAudio.Wave;
using System.Xml.Serialization;
using System.Text.RegularExpressions;
using System.Runtime.CompilerServices;
using EPSSEditor.Vorbis;

namespace EPSSEditor
{
    public class Sound : ICloneable
    {
        public string path;
        public string description;
        //public long length; // File length, not samples!

        public Guid _id;
        public ConversionParameters _parameters;

        public int channels;
        public int bitsPerSample;
        public int samplesPerSecond; // i.e. Hz
        public long sampleDataLength; // Length in bytes of the raw sample
        public long sampleCount;

        public byte loKey;
        public byte hiKey;
        public byte keyCenter;

        public bool loop;
        public int loopStart; // in samples
        public int loopEnd; // in samples
        public int loopType;

        private CachedSound _cachedAudio = null;


        public Sound() { }


        public Sound(byte[] soundData, string outPath, bool loop, long loopStart, long loopEnd)
        {
            this.loop = loop;
            this.loopStart = (int)loopStart;
            this.loopEnd = (int)loopEnd;
            int sampleRate = 25033;
            int bits = 8;
            int channels = 1;
            using (var ms = new MemoryStream(soundData))
            {
                using (var s = new RawSourceWaveStream(ms, new WaveFormat(sampleRate, bits, channels)))
                {
                    WaveLoopFileWriter.CreateWaveLoopFile(outPath, s, loop, loopStart, loopEnd);
                    if (!InitSound(outPath, out string errorMessage))
                    {
                        Console.WriteLine($"Sound not initialized: {errorMessage}");
                    }
                }
            }
        }


        public virtual object Clone()
        {
            return this.MemberwiseClone();
        }

        public bool InitSound(string p, out string errorMessage)
        {
            bool result = false;
            errorMessage = "Unknown error";
            loop = false;

            using (FileStream sampleStream = File.OpenRead(p))
            {
                sampleStream.Seek(0, SeekOrigin.Begin);
                string ext = Path.GetExtension(p).ToLower();
                if (ext == ".ogg")
                {
                    using (var reader = new VorbisWaveReader(p))
                    {
                        WaveFormat fmt = reader.WaveFormat;
                        channels = fmt.Channels;
                        bitsPerSample = fmt.BitsPerSample;
                        samplesPerSecond = fmt.SampleRate;
                        sampleDataLength = reader.Length;
                        sampleCount = sampleDataLength / (bitsPerSample / 2);
                        result = true;
                    }

                }
                else if (ext == ".wav")
                {

                    using (var reader = new WaveFileReader(sampleStream))
                    {
                        WaveFormat fmt = reader.WaveFormat;
                        channels = fmt.Channels;
                        bitsPerSample = fmt.BitsPerSample;
                        samplesPerSecond = fmt.SampleRate;
                        sampleDataLength = reader.Length;
                        sampleCount = reader.SampleCount; // Does not take channels into account!

                        var smp = reader.ExtraChunks.FirstOrDefault(ec => ec.IdentifierAsString == "smpl");
                        if (smp != null)
                        {
                            var chunkData = reader.GetChunkData(smp);
                            // https://sites.google.com/site/musicgapi/technical-documents/wav-file-format#smpl
                            var midiNote = BitConverter.ToInt32(chunkData, 12);
                            keyCenter = (byte)midiNote;
                            loKey = (byte)Math.Max(0, keyCenter - 24); // EPSS only supports two octaves up and two down
                            hiKey = (byte)Math.Min(128, keyCenter + 24);

                            var numberOfLoops = BitConverter.ToInt32(chunkData, 28);
                            Console.WriteLine($"MIDI {midiNote}, {numberOfLoops} loops");
                            int offset = 36;
                            //for (int n = 0; n < numberOfLoops; n++)
                            //{
                            var cuePointId = BitConverter.ToInt32(chunkData, offset);
                            var type = BitConverter.ToInt32(chunkData, offset + 4); // 0 = loop forward, 1 = alternating loop, 2 = reverse

                            var start = BitConverter.ToInt32(chunkData, offset + 8);
                            var end = BitConverter.ToInt32(chunkData, offset + 12);
                            var fraction = BitConverter.ToInt32(chunkData, offset + 16);
                            var playCount = BitConverter.ToInt32(chunkData, offset + 20);

                            Console.WriteLine($"Sample {cuePointId} Start {start} End {end} Type {type} Fraction {fraction} PlayCount {playCount} SampleDataLength {sampleDataLength}");
                            offset += 24;

                            loop = true;
                            loopStart = start;
                            loopEnd = end;
                            loopType = type;

                            //break; // only read one loop
                            //}
                        }
                        result = true;
                    }
                }
            }

            if (!result)
            {
                try
                {
                    using (var reader = new AudioFileReader(p))
                    {
                        WaveFormat fmt = reader.WaveFormat;
                        channels = fmt.Channels;
                        bitsPerSample = fmt.BitsPerSample;
                        samplesPerSecond = fmt.SampleRate;
                        sampleDataLength = reader.Length;
                        sampleCount = sampleDataLength / (bitsPerSample / 2);
                        //sampleCount = reader.SampleCount; // Does not take channels into account!
                        result = true;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                    errorMessage = ex.Message;
                }
            }
              
            if (result)
            {
                path = p;
                description = Path.GetFileNameWithoutExtension(path);
                //length = new System.IO.FileInfo(path).Length;
                _id = Guid.NewGuid();

                _parameters = new ConversionParameters();
                _parameters.normalize = new ConversionNormalize();

                loKey = hiKey = keyCenter = 128;
                //loop = false;
            }
            return result;
        }

        public Guid id() { return _id; }


        public ConversionParameters parameters() { return _parameters; }


        public void updateConversionParameters(int toBit, int toFreq)
        {
            _parameters.updateConversions(this, toBit, toFreq);
        }


        public void updateNormalize(bool normalize, int normalizePercentage)
        {
            _parameters.updateNormalize(normalize, normalizePercentage);
        }


        public string name()
        {
            //if (description == null) return Path.GetFileNameWithoutExtension(path);
            return description;
        }


        public bool Rename(string newName, out string errorString)
        {
            errorString = "";
            if (newName != name())
            {
                string newFullPath = Path.GetDirectoryName(path);
                newFullPath += "\\" + newName + Path.GetExtension(path);
                if (!File.Exists(newFullPath))
                {
                    try
                    {
                        File.Move(path, newFullPath);
                        path = newFullPath;
                        description = Path.GetFileNameWithoutExtension(path);
                        return true;
                    }
                    catch (Exception e)
                    {
                        errorString = e.Message;
                        return false;
                    }
                }
                else
                {
                    errorString = "File already exists."; 
                }
            }
            return false;
        }


        public CachedSound cachedSound()
        {
            if (_cachedAudio == null)
            {
                _cachedAudio = new CachedSound(path, loop, (UInt32)loopStart, (UInt32)loopEnd);
            }
            return _cachedAudio;
        }


        public string IdToString
        {
            get {
                return _id.ToString();
            }
        }


        public string ListDisplayName
        {
            get
            {
                return description;
            }
        }

        public override string ToString()
        {
            return base.ToString() + $" ({path}, {description}, {sampleDataLength}, {_id}, {channels}, {bitsPerSample}, {samplesPerSecond}, {loKey}, {hiKey}, {keyCenter})";
        }
       
    }
}
