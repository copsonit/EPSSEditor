using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NAudio.Wave;
using NAudio.Wave.SampleProviders;

namespace EPSSEditor
{
    public class AudioPlaybackEngine : IDisposable
    {
        private readonly IWavePlayer outputDevice;
        private readonly MixingSampleProvider mixer;

        public AudioPlaybackEngine(int sampleRate = 44100, int channelCount = 2)
        {
            outputDevice = new WaveOutEvent();
            mixer = new MixingSampleProvider(WaveFormat.CreateIeeeFloatWaveFormat(sampleRate, channelCount));
        }


        public void Start()
        {

            mixer.ReadFully = true;
            outputDevice.Init(mixer);
            outputDevice.Play();
        }


        public void PlaySound(string fileName)
        {
            var input = new AudioFileReader(fileName);
            AddMixerInput(new AutoDisposeFileReader(input));
        }


        public void PlaySound(CachedSound sound)
        {
            AddMixerInput(new CachedSoundSampleProvider(sound));
        }


        private void AddMixerInput(ISampleProvider input)
        {
            mixer.AddMixerInput(ConvertToRightChannelCount(input));
        }

        private ISampleProvider ConvertToRightChannelCount(ISampleProvider input)
        {
            if (input.WaveFormat.Channels == mixer.WaveFormat.Channels)
            {
                return input;
            }
            if (input.WaveFormat.Channels == 1 && mixer.WaveFormat.Channels == 2)
            {
                return new MonoToStereoSampleProvider(input);
            }
            throw new NotImplementedException("Not yet implemented this channel count conversion");
        }

        public void Stop()
        {
            outputDevice.Stop();
        }

        public void Dispose()
        {
            outputDevice.Dispose();
        }
    }


    public class AutoDisposeFileReader : ISampleProvider
    {
        private readonly AudioFileReader reader;
        private bool isDisposed;
        public AutoDisposeFileReader(AudioFileReader reader)
        {
            this.reader = reader;
            this.WaveFormat = reader.WaveFormat;
        }

        public int Read(float[] buffer, int offset, int count)
        {
            if (isDisposed)
                return 0;
            int read = reader.Read(buffer, offset, count);
            if (read == 0)
            {
                reader.Dispose();
                isDisposed = true;
            }
            return read;
        }

        public WaveFormat WaveFormat { get; private set; }
    }


    public class CachedSoundSampleProvider : ISampleProvider
    {
        private readonly CachedSound cachedSound;
        private long position;

        public CachedSoundSampleProvider(CachedSound cachedSound)
        {
            this.cachedSound = cachedSound;
        }

        public int Read(float[] buffer, int offset, int count)
        {
            var availableSamples = cachedSound.AudioData.Length - position;
            var samplesToCopy = Math.Min(availableSamples, count);
            Array.Copy(cachedSound.AudioData, position, buffer, offset, samplesToCopy);
            position += samplesToCopy;
            return (int)samplesToCopy;
        }

        public WaveFormat WaveFormat { get { return cachedSound.WaveFormat; } }
    }

    public class CachedSound
    {
        public float[] AudioData { get; private set; }
        public WaveFormat WaveFormat { get; private set; }
        public CachedSound(string audioFileName)
        {
            using (var audioFileReader = new AudioFileReader(audioFileName))
            {
                
                var resampler = new WdlResamplingSampleProvider(audioFileReader, 44100);
                WaveFormat = resampler.WaveFormat;
                var wholeFile = new List<float>((int)(audioFileReader.Length / 4));
                var readBuffer = new float[resampler.WaveFormat.SampleRate * resampler.WaveFormat.Channels];
                int samplesRead;
                while ((samplesRead = resampler.Read(readBuffer, 0, readBuffer.Length)) > 0)
                {
                    wholeFile.AddRange(readBuffer.Take(samplesRead));
                }
                AudioData = wholeFile.ToArray();
                

                /*
                var resampler = new MediaFoundationResampler(audioFileReader, 44100);
                WaveFormat = resampler.WaveFormat;
                var wholeFile = new List<byte>((int)(audioFileReader.Length / 4));
                var readBufferByte = new byte[resampler.WaveFormat.SampleRate * resampler.WaveFormat.Channels];
                int samplesRead;
                while ((samplesRead = resampler.Read(readBufferByte, 0, readBufferByte.Length)) > 0)
                {
                    wholeFile.AddRange(readBufferByte.Take(samplesRead));
                }
                AudioData = wholeFile.ToArray();
                */

                /*
                WaveFormat = audioFileReader.WaveFormat;
                var wholeFile = new List<float>((int)(audioFileReader.Length / 4));
                var readBuffer = new float[audioFileReader.WaveFormat.SampleRate * audioFileReader.WaveFormat.Channels];
                int samplesRead;
                while ((samplesRead = audioFileReader.Read(readBuffer, 0, readBuffer.Length)) > 0)
                {
                    wholeFile.AddRange(readBuffer.Take(samplesRead));
                }
                AudioData = wholeFile.ToArray();
                //WaveFormat = new WaveFormat(); // Default 44100, 16, 2
                */
            }
        }
    }


}
