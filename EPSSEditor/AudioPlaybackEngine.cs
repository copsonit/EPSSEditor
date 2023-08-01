using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using NAudio.CoreAudioApi;
using NAudio.Wave;
using NAudio.Wave.SampleProviders;
using EPSSEditor.Vorbis;

namespace EPSSEditor
{
    public class AudioPlaybackEngine : IDisposable
    {
        private readonly WasapiOut outputDevice;
        private readonly MixingSampleProvider mixer;

        public AudioPlaybackEngine(int sampleRate = 44100, int channelCount = 2)
        {
            outputDevice = new WasapiOut(AudioClientShareMode.Shared, true, 20);
            mixer = new MixingSampleProvider(WaveFormat.CreateIeeeFloatWaveFormat(sampleRate, channelCount));
        }


        public void Start()
        {
            mixer.ReadFully = true;
            outputDevice.Init(mixer);
            outputDevice.Play();
        }


        public void PlaySound(CachedSound sound)
        {
            ISampleProvider addedProvider = new CachedSoundSampleProvider(sound, sound.vvfeOffset);
            AddMixerInput(addedProvider);
            sound.IsPlaying = true;
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


    public class CachedSoundSampleProvider : ISampleProvider
    {
        private readonly CachedSound cachedSound;
        private long position = 0;

        public CachedSoundSampleProvider(CachedSound cachedSound, long vvfeOffset)
        {
            this.cachedSound = cachedSound;
            position = vvfeOffset;
            //Console.WriteLine("vvfeOffset: {0}", vvfeOffset);
           
        }



        public int Read(float[] buffer, int offset, int count)
        {
            int loopEnd = (int)cachedSound.loopEnd;
            int loopStart = (int)cachedSound.loopStart;
            int sourceSampleEnd = cachedSound.loop ? loopEnd : cachedSound.AudioData.Length;
            var availableSamples = sourceSampleEnd - position;

            var samplesToCopy = Math.Min(availableSamples, count);
            //Console.WriteLine("AudioData:{0}, position:{1} count:{2}, available: {3}, offset: {4} samplesToCopy: {5} pitch:{6} loopStart {7} loopEnd {8} orgLoopStart {9} orgLoopEnd {10}", cachedSound.AudioData.Length, position, count.ToString(), availableSamples.ToString(), offset, samplesToCopy, pitch, loopStart, loopEnd, cachedSound.loopStart, cachedSound.loopEnd);

            double pitch = cachedSound.pitch;
            double readPos = position;
            int destOffset = offset;

            //try
            //{
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

                if (!cachedSound.IsPlaying)
                {
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


                while ((destOffset - offset) < samplesToCopy)
                {

                    if (destOffset >= buffer.Length)
                    {
                        Console.WriteLine("DestOffset overflow!");
                    }
                    if ((int)readPos >= cachedSound.AudioData.Length)
                    {
                        Console.WriteLine("readPos overflow!");
                    }

                    buffer[destOffset++] = cachedSound.AudioData[(int)readPos];
                    readPos += pitch;

                    if (readPos >= sourceSampleEnd)
                    {
                        //Console.WriteLine($"End found {readPos} {availableSamples}");
                        //Array.Clear(buffer, offset, count - offset);
                        //samplesToCopy = 0;
                        //break;
                        readPos -= pitch;
                        break;
                    }

                }
                position = (int)readPos;


            /*
            Array.Copy(cachedSound.AudioData, position, buffer, offset, samplesToCopy);

            position += samplesToCopy;
            */

            if (cachedSound.loop && samplesToCopy < count)
            {
                position = loopStart;
                readPos = position;
                destOffset = offset + (int)samplesToCopy;

                 while ((destOffset - offset) < count)
                {
                    if ((int)readPos >= cachedSound.AudioData.Length)
                    {
                        Console.WriteLine("Loop readPos overflow!");
                    }
                    if (destOffset >= buffer.Length)
                    {
                        Console.WriteLine("Loop DestOffset overflow!");
                    }
                    buffer[destOffset++] = cachedSound.AudioData[(int)readPos];
                    readPos += pitch;

                    if (readPos >= sourceSampleEnd)
                    {
                        position = loopStart;
                        readPos = position;
                    }
                }
                position = (int)readPos;
                samplesToCopy = count;
            }

            //Console.WriteLine("SamplesToCopy: {0}", samplesToCopy.ToString());
            return (int)samplesToCopy;
            //}
            //catch (Exception ex)
            //{
              //  Console.WriteLine($"loopStart: {loopStart}, loopEnd: {loopEnd}, availableSamples: {availableSamples}, count: {count}, samplesToCopy: {samplesToCopy}, sourceSampEnd: {sourceSampleEnd}, position: {position}, readPos:{readPos}, cachedSound.Length: {cachedSound.AudioData.Length}, destOffset:{destOffset}, buffer length:{buffer.Length}");
              // Console.WriteLine("Read buffer Exception:", ex.ToString());
            // }
            // return 0;
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
        public UInt32 loopStart = 0;
        public UInt32 loopEnd = 0;
        public double pitch = 1.0;
        public int vvfeOffset = 0;
        public CachedSound(MemoryStream ms, bool loop, UInt32 loopStart, UInt32 loopEnd, float pan)
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
            int newFreq = 44100;

            if (Math.Abs(pan) > 0.01)
            {
                var panner = new PanningSampleProvider(rsAsSampleProvider);
                panner.PanStrategy = new SquareRootPanStrategy();
                panner.Pan = pan;
                resampler = new WdlResamplingSampleProvider(panner, newFreq);
            }
            else
            {
                resampler = new WdlResamplingSampleProvider(rsAsSampleProvider, newFreq);
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
                double lengthChangeFact = (double)newFreq / (double)rsAsSampleProvider.WaveFormat.SampleRate;
                this.loopStart = (UInt32)((double)loopStart * lengthChangeFact);
                this.loopEnd = (UInt32)((double)loopEnd * lengthChangeFact);
            }

            
        }

        public CachedSound(string audioFileName, bool lp, UInt32 ls, UInt32 le)
        {
            int newSampleRate = 44100;
            loop = lp;
            loopType = 0; // Only forward supported in EPSS

            string ext = Path.GetExtension(audioFileName).ToLower();

             long length;

            if (ext == ".ogg")
            {               
                using (VorbisWaveReader reader = new VorbisWaveReader(audioFileName))
                {
                    length = reader.Length;
                    InitCachedSound(reader, newSampleRate, length, ls, le);
                }               
            }
            else
            {
                using (AudioFileReader reader = new AudioFileReader(audioFileName))
                {
                    length = reader.Length;
                    InitCachedSound(reader, newSampleRate, length, ls, le);
                }
            }





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


        private void InitCachedSound(ISampleProvider audioFileReader, int newSampleRate, long length, UInt32 ls, UInt32 le)
        {
            loopStart = ls;
            loopEnd = le;
            double loopFactor = (double)newSampleRate / (double)audioFileReader.WaveFormat.SampleRate;
            loopStart = (UInt32)((double)ls * loopFactor);
            loopEnd = (UInt32)((double)le * loopFactor);

            var resampler = new WdlResamplingSampleProvider(audioFileReader, newSampleRate);
            WaveFormat = resampler.WaveFormat;
            var wholeFile = new List<float>((int)(length / 4));
            var readBuffer = new float[resampler.WaveFormat.SampleRate * resampler.WaveFormat.Channels];
            int samplesRead;
            while ((samplesRead = resampler.Read(readBuffer, 0, readBuffer.Length)) > 0)
            {
                wholeFile.AddRange(readBuffer.Take(samplesRead));
            }
            AudioData = wholeFile.ToArray();
        }
    }
}


