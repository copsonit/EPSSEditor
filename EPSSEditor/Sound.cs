using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using NAudio.Wave;
using System.Xml.Serialization;

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
        public long length;

        public Guid _id;
        public ConversionParameters _parameters;

        public int channels;
        public int bitsPerSample;
        public int samplesPerSecond; // i.e. Hz

        public byte loKey;
        public byte hiKey;
        public byte keyCenter;


        public Sound() { }

        public Sound(string p) {
            path = p;
            description = null;
            length = new System.IO.FileInfo(path).Length;
            _id = Guid.NewGuid();
            
            _parameters = new ConversionParameters();
            _parameters.normalize = new ConversionNormalize();

            FileStream wav = File.OpenRead(path);
            wav.Seek(0, SeekOrigin.Begin);

            WaveStream ws = new WaveFileReader(wav);
            WaveFormat fmt = ws.WaveFormat;

            channels = fmt.Channels;
            bitsPerSample = fmt.BitsPerSample;
            samplesPerSecond = fmt.SampleRate;

            loKey = hiKey = keyCenter = 128;
        }


        public Sound(byte[] soundData, string outPath)
        {
            int sampleRate = 25033;
            int bits = 8;
            int channels = 1;
            var ms = new MemoryStream(soundData);
            var s = new RawSourceWaveStream(ms, new WaveFormat(sampleRate, bits, channels));
            WaveFileWriter.CreateWaveFile(outPath, s);
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

        public override string ToString()
        {
            return base.ToString() + $" ({path}, {description}, {length}, {_id}, {channels}, {bitsPerSample}, {samplesPerSecond}, {loKey}, {hiKey}, {keyCenter})";
        }
    }
}
