﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using NAudio.Wave;
using System.Xml.Serialization;
using System.Text.RegularExpressions;
using System.Runtime.CompilerServices;

namespace EPSSEditor
{
    public class SoundLoadError
    {
        public SoundLoadError() {}
    }

    public class SoundLoadNoError : SoundLoadError
    {
        public SoundLoadNoError() { }
    }


 
    public class Sound
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

        public Sound(string p) {
            InitSound(p);
        }


        public Sound(byte[] soundData, string outPath)
        {
            int sampleRate = 25033;
            int bits = 8;
            int channels = 1;
            var ms = new MemoryStream(soundData);
            var s = new RawSourceWaveStream(ms, new WaveFormat(sampleRate, bits, channels));


            WaveFileWriter.CreateWaveFile(outPath, s);

            InitSound(outPath);
        }


        public void InitSound(string p)
        {
            path = p;
            description = null;
            //length = new System.IO.FileInfo(path).Length;
            _id = Guid.NewGuid();

            _parameters = new ConversionParameters();
            _parameters.normalize = new ConversionNormalize();

            loKey = hiKey = keyCenter = 128;
            loop = false;

            FileStream wav = File.OpenRead(path);
            wav.Seek(0, SeekOrigin.Begin);


            if (Path.GetExtension(p).ToLower() == ".wav")
            {
                using (var reader = new WaveFileReader(wav))
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
                        loKey = (byte)Math.Max(0, keyCenter - 36);
                        hiKey = (byte)Math.Min(128, keyCenter + 36);

                        var numberOfLoops = BitConverter.ToInt32(chunkData, 28);
                        Console.WriteLine($"MIDI {midiNote}, {numberOfLoops} loops");
                        int offset = 36;
                        for (int n = 0; n < numberOfLoops; n++)
                        {
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

                            break; // only read one loop
                        }
                    }
                }
            }

            //WaveStream ws = new WaveFileReader(wav);
            //WaveFormat fmt = ws.WaveFormat;




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


        public SoundLoadError loadSound(string path)
        {
            return new SoundLoadNoError();
        }

        public string name()
        {
            if (description == null) return Path.GetFileNameWithoutExtension(path);
            return description;
        }

        public bool Rename(string newName)
        {
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
                        return true;
                    }
                    catch (Exception e)
                    {
                        return false;
                    }
                }
            }
            return false;
        }


        public CachedSound cachedSound()
        {
            if (_cachedAudio == null)
            {
                _cachedAudio = new CachedSound(path, loop, loopStart, loopEnd);
            }
            return _cachedAudio;
        }


        public override string ToString()
        {
            return base.ToString() + $" ({path}, {description}, {sampleDataLength}, {_id}, {channels}, {bitsPerSample}, {samplesPerSecond}, {loKey}, {hiKey}, {keyCenter})";
        }
    }
}
