using System;
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
        private readonly DirectSoundOut outputDevice;
        private readonly MixingSampleProvider mixer;

        public AudioPlaybackEngine(IntPtr winHandle, int sampleRate = 44100, int channelCount = 2)
        {
            //outputDevice = new WaveOutEvent();

            //outputDevice = new WaveOut();
            //outputDevice.DesiredLatency = 60;

            outputDevice = new DirectSoundOut(40);

            //outputDevice.DesiredLatency = 50;

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
            ISampleProvider addedProvider = new CachedSoundSampleProvider(sound, sound.vvfeOffset);
            AddMixerInput(addedProvider);
            sound.IsPlaying = true;
//            return addedProvider;
        }

        public void StopSound(CachedSound sound)
        {
            sound.IsPlaying = false;
            //mixer.RemoveMixerInput(provider);
            //mixer.RemoveAllMixerInputs();
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
        private long position = 0;
        private bool isEnding = false;

        public CachedSoundSampleProvider(CachedSound cachedSound, long vvfeOffset)
        {
            this.cachedSound = cachedSound;
            position = vvfeOffset;
            //Console.WriteLine("vvfeOffset: {0}", vvfeOffset);
           
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

            if (!cachedSound.IsPlaying) {
                return 0;
            }
            /*
                isEnding = true;
                for (int i = 0; i < count; i++)
                {
                    buffer[i] = 0;
                }
                return count; //xfade down to 0 to avoid clicks.
            }
            if (isEnding)
            {
                isEnding = false;
                return 0;
            }
            */

            int loopEnd = cachedSound.loopEnd;
            int loopStart = cachedSound.loopStart;
            int sourceSampleEnd = cachedSound.loop ? loopEnd : cachedSound.AudioData.Length; 
            var availableSamples = sourceSampleEnd - position;


            var samplesToCopy = Math.Min(availableSamples, count);
            //Console.WriteLine("AudioData:{0}, position:{1} count:{2}, available: {3}, offset: {4} samplesToCopy: {5} loopStart {6} loopEnd {7}", cachedSound.AudioData.Length, position, count.ToString(), availableSamples.ToString(), offset, samplesToCopy, loopStart, loopEnd);


            double pitch = cachedSound.pitch;
            double readPos = position;
            int destOffset = offset;
            while ((destOffset - offset) < samplesToCopy)
            {
                /*
                if (destOffset >= buffer.Length)
                {
                    Console.WriteLine("DestOffset overflow!");
                }
                if ((int)readPos >= cachedSound.AudioData.Length)
                {
                    Console.WriteLine("readPos overflow!");
                }
                */
                buffer[destOffset++] = cachedSound.AudioData[(int)readPos];
                readPos += pitch;

                if (readPos >= sourceSampleEnd)
                {
                    //Console.WriteLine($"End found {readPos} {availableSamples}");
                    //Array.Clear(buffer, offset, count - offset);
                    //samplesToCopy = 0;
                    //break;
                    break;
                }

            }
            position = (int)readPos;


            /*
            Array.Copy(cachedSound.AudioData, position, buffer, offset, samplesToCopy);

            position += samplesToCopy;
            */


            // TODO fix when we have pitch.
            
            
            if (cachedSound.loop && samplesToCopy < count)
            {
                position = loopStart;
                readPos = position;
                destOffset = offset + (int)samplesToCopy ;

                while ((destOffset - offset) < samplesToCopy)
                {
                    buffer[destOffset++] = cachedSound.AudioData[(int)readPos];
                    readPos += pitch;

                    if (readPos >= sourceSampleEnd)
                    {
                        break;
                    }
                }
                position = (int)readPos;
                samplesToCopy = count;
            }
            

            //Console.WriteLine("SamplesToCopy: {0}", samplesToCopy.ToString());
            return (int)samplesToCopy;
        }

        public WaveFormat WaveFormat { get { return cachedSound.WaveFormat; } }
    }

    public class CachedSound
    {
        public float[] AudioData { get; private set; }
        public WaveFormat WaveFormat { get; private set; }
        
        public bool IsPlaying { get; set; }
        public bool loop = false;
        public int loopType = 0;
        public int loopStart = 0;
        public int loopEnd = 0;
        public double pitch = 1.0;
        public int vvfeOffset = 0;
        public CachedSound(MemoryStream ms, bool loop, int loopStart, int orgSampleCount, float pan)
        {
            this.loop = loop;
            loopType = 0; // only forward supported in EPSS
            //this.loopStart = loopStart;
            //this.loopEnd = loopEnd;

            //var rs = new RawSourceWaveStream(ms, new WaveFormat(newFreq, bits, channels));
            var rs = new WaveFileReader(ms);


            //uint orgLoopStart = loopStart;
            //uint orgLoopEnd = loopEnd;
            //long newSampleDataLen = rs.Length;
            var rsAsSampleProvider = rs.ToSampleProvider();

            ISampleProvider resampler;
            bool usePanning = false;

            if (usePanning)
            {
                var panner = new PanningSampleProvider(rsAsSampleProvider);
                panner.PanStrategy = new SquareRootPanStrategy();
                panner.Pan = pan;
                resampler = new WdlResamplingSampleProvider(panner, 44100);
            }
            else
            {
                resampler = new WdlResamplingSampleProvider(rsAsSampleProvider, 44100);
            }

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

            if (loop)
            {
                this.loopEnd = AudioData.Length;
                this.loopStart = (int)(((double)loopStart / (double)orgSampleCount) * (double)AudioData.Length);
            }

            
        }

        public CachedSound(string audioFileName, bool loop, int loopStart, int loopEnd)
        {
            this.loop = loop;
            loopType = 0; // Only forward supported in EPSS
            this.loopStart = loopStart;
            this.loopEnd = loopEnd;

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
