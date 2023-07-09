﻿using System;
using System.Collections.Generic;
using System.IO;
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

            //outputDevice.PlaybackStopped += this.PlaybackLoopCallback;
           
        }


        public void Start()
        {

            mixer.ReadFully = true;
            outputDevice.Init(mixer);
            outputDevice.Play();
        }


        /*public void PlaySound(string fileName)
        {
            var input = new AudioFileReader(fileName);
            AddMixerInput(new AutoDisposeFileReader(input));
        }
        */


        public void PlaySound(CachedSound sound)
        {
            ISampleProvider addedProvider = new CachedSoundSampleProvider(sound);
            AddMixerInput(addedProvider);
            sound.IsPlaying = true;
//            return addedProvider;
        }

        public void StopSound(CachedSound sound)
        {
            sound.IsPlaying = false;
            //mixer.RemoveMixerInput(provider);
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
            mixer.RemoveAllMixerInputs();
            outputDevice.Stop();
        }

        public void Dispose()
        {
            outputDevice.Dispose();
        }
    }

    /*
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
    */


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

            /*
            int read = 0;
            while (read < count)
            {
                //int bytesRead = _reader.Read(buffer, offset + read, count - read);
                var availableSamples = cachedSound.AudioData.Length - position;
                var samplesToCopy = Math.Min(availableSamples, count);
                Array.Copy(cachedSound.AudioData, position, buffer, offset, samplesToCopy);

                if (bytesRead == 0)
                {
                    if (_reader.Position == 0 || !_loop)
                    {
                        break;
                    }
                    _reader.Position = 0;
                }
                read += bytesRead;
            }

            if (read < count)
            {
                Dispose();
            }
            return read;
            */
            //            Console.WriteLine(count.ToString());
            if (!cachedSound.IsPlaying) return 0;


            var availableSamples = cachedSound.AudioData.Length - position;
            var samplesToCopy = Math.Min(availableSamples, count);
            Console.WriteLine("AudioData:{0}, position:{1} count:{2}, available: {3}, offset: {4}", cachedSound.AudioData.Length, position, count.ToString(), availableSamples.ToString(), offset);

            //int bufferSize = 500;
            //samplesToCopy = Math.Min(bufferSize, samplesToCopy);

            Array.Copy(cachedSound.AudioData, position, buffer, offset, samplesToCopy);
            position += samplesToCopy;
            if (cachedSound.loop && samplesToCopy < count)
            {
                //cachedSound.IsPlaying = false;
                position = 0;
                long buffPos = offset + samplesToCopy;
                for (int i = 0; i < (cachedSound.AudioData.Length - 1); i++)
                {
                    buffer[buffPos] = cachedSound.AudioData[position++];
                    samplesToCopy++;
                    if (samplesToCopy > (count - 1)) break;
                }
            }

            Console.WriteLine("SamplesToCopy: {0}", samplesToCopy.ToString());
            return (int)samplesToCopy;
        }

        public WaveFormat WaveFormat { get { return cachedSound.WaveFormat; } }
    }

    public class CachedSound
    {
        public float[] AudioData { get; private set; }
        public WaveFormat WaveFormat { get; private set; }
        public bool IsPlaying { get; set; }
        public bool loop { get; set;  }

        public CachedSound(MemoryStream ms, int newFreq, int bits, int channels, bool loop)
        {
            this.loop = loop;

            var rs = new RawSourceWaveStream(ms, new WaveFormat(newFreq, bits, channels));
            var rsAsSampleProvider = rs.ToSampleProvider();

            var resampler = new WdlResamplingSampleProvider(rsAsSampleProvider, 44100);
            WaveFormat = resampler.WaveFormat;
            long l = ms.Length;
            var wholeFile = new List<float>((int)l);
            var readBuffer = new float[resampler.WaveFormat.SampleRate * resampler.WaveFormat.Channels];
            int samplesRead;
            while ((samplesRead = resampler.Read(readBuffer, 0, readBuffer.Length)) > 0)
            {
                wholeFile.AddRange(readBuffer.Take(samplesRead));
            }
            AudioData = wholeFile.ToArray();
        }

        public CachedSound(string audioFileName)
        {
            using (var audioFileReader = new AudioFileReader(audioFileName))
            {
                loop = false;
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
