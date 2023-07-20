using NAudio.Wave;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EPSSEditor
{

    // Not possible as it has lots of privates. Has to fork from NAudio, change so it will be extendable and then extend it here.
    public class WaveLoopFileWriter : WaveFileWriter 
    {      
        private long smplChunkSizePos;
        private long smplChunkSize;
        private bool loop;
        private long loopStart;
        private long loopEnd;

        public WaveLoopFileWriter(Stream outStream, WaveFormat format, bool loop, long loopStart, long loopEnd)
    : base(outStream, format)
        {
            this.loop = loop;
            this.loopStart = loopStart; 
            this.loopEnd = loopEnd;
        }


        public WaveLoopFileWriter(string filename, WaveFormat format, bool loop, long loopStart, long loopEnd)
    : this(new FileStream(filename, FileMode.Create, FileAccess.Write, FileShare.Read), format, loop, loopStart, loopEnd)
        { }


        public static void CreateWaveLoopFile(string filename, IWaveProvider sourceProvider, bool loop, long loopStart, long loopEnd)
        {
            using (var writer = new WaveLoopFileWriter(filename, sourceProvider.WaveFormat, loop, loopStart, loopEnd))
            {
                var buffer = new byte[sourceProvider.WaveFormat.AverageBytesPerSecond * 4];
                while (true)
                {
                    int bytesRead = sourceProvider.Read(buffer, 0, buffer.Length);
                    if (bytesRead == 0)
                    {
                        // end of source provider
                        break;
                    }
                    // Write will throw exception if WAV file becomes too large
                    writer.Write(buffer, 0, bytesRead);
                }
                writer.WriteAdditionalDataChunks(writer.Writer);
            }
        }


        private void WriteSmplChunk(BinaryWriter writer)
        {
            if (HasSmplChunk())
            {
                writer.Write(Encoding.UTF8.GetBytes("smpl"));
                smplChunkSizePos = writer.BaseStream.Position;
                writer.Write((int)0); // chunkSize
                writer.Write((int)0); // dwManufacturer
                writer.Write((int)0); // dwProduct
                writer.Write((int)39947); // dwSamplePeriod 1/25033 in ns
                writer.Write((int)84); // dwMIDIUnittyNote
                writer.Write((int)0); // dwMIDIPitchFraction
                writer.Write((int)0); // dwSMPTEFormat
                writer.Write((int)0); // dwSMPTEOffset
                writer.Write((int)1); // cSampleLoops
                writer.Write((int)0); // cbSamplerData, can be used for additional mfr specific info


                writer.Write((int)0); // dwIdentifier
                writer.Write((int)0); // dwType: 0 loop forward, 1 alternating, 2 loop backward, 3-31 res, 32 sampler specific
                writer.Write((int)loopStart); // dwStart
                writer.Write((int)loopEnd); // dwEnd
                writer.Write((int)0); // dwFraction
                writer.Write((int)0); // dwPlayCount
                smplChunkSize = writer.BaseStream.Position - smplChunkSizePos - 4;
            }
        }

        private void UpdateSmplChunk(BinaryWriter writer)
        {
            if (HasSmplChunk())
            {
                writer.Seek((int)smplChunkSizePos, SeekOrigin.Begin);
                writer.Write((long)smplChunkSize);
            }
        }


        private bool HasSmplChunk()
        {
            return loop;
        }


        protected override void UpdateHeader(BinaryWriter writer)
        {
            base.UpdateHeader(writer);
            UpdateSmplChunk(writer);
        }


        public void WriteAdditionalDataChunks(BinaryWriter writer)
        {
            WriteSmplChunk(writer);
        }
    }
}
