using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using NAudio.Wave;
using System.IO;

namespace EPSSEditor
{
  
    public class SpiSound
    {
        public byte midiChannel;
        public byte midiNote;
        public int soundNumber;

        public byte startNote;
        public byte endNote;
        public byte midiNoteMapped;
        public byte programNumber;


        public Guid soundId;
        public string _name;
        public string _extName;

        private MemoryStream ms = null;

        public SpiSound() {
            startNote = endNote = programNumber = 128;
            midiNoteMapped = 84;
            
        }

        public SpiSound(ref Sound sound)
        {
            startNote = endNote = programNumber = 128;
            midiNoteMapped = 84;
            soundId = sound.id();
            _name = sound.name();
            _extName = sound.name() + " MSWav"; // TODO, find more info in sound??         
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


        public bool convertSound(ref EPSSEditorData data, string outFile, int newFreq, int bits, int channels)
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
                            WaveFileReader fr = new WaveFileReader(wav);
                            len = fr.Length;

                            byte[] buffer = new byte[len];
                            int rd = fr.Read(buffer, 0, (int)len);

                            foreach (byte b in buffer)
                            {
                                byte newB = b;
                                spl.Add(newB);
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


        public MemoryStream getWaveStream(ref EPSSEditorData data, int newFreq, int bits, int channels)
        {
            if (ms == null)
            {
                string outFile = data.convertSoundFileName();

                if (convertSound(ref data, outFile, newFreq, bits, channels))
                {
                    ms = new MemoryStream();
                    using (FileStream file = new FileStream(outFile, FileMode.Open, FileAccess.Read))
                    {

                        byte[] bytes = new byte[file.Length];
                        file.Read(bytes, 0, (int)file.Length);
                        ms.Write(bytes, 0, (int)file.Length);
                    }
                }

               
            }
            return ms;
        }


    }
}
