using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.IO;
using NAudio.Wave;
using NAudio.MediaFoundation;
using System.Security.Policy;
using System.Security.Cryptography;
using System.Windows.Forms.VisualStyles;

/*
 * Example https://stackoverflow.com/questions/14464/bit-fields-in-c-sharp

public struct rcSpan2
{
    internal uint data;

    //public uint smin : 13; 
    public uint smin
    {
        get { return data & 0x1FFF; }
        set { data = (data & ~0x1FFFu) | (value & 0x1FFF); }
    }

    //public uint smax : 13; 
    public uint smax
    {
        get { return (data >> 13) & 0x1FFF; }
        set { data = (data & ~(0x1FFFu << 13)) | (value & 0x1FFF) << 13; }
    }

    //public uint area : 6; 
    public uint area
    {
        get { return (data >> 26) & 0x3F; }
        set { data = (data & ~(0x3F << 26)) | (value & 0x3F) << 26; }
    }
}
*/


// http://www.developerfusion.com/article/84519/mastering-structs-in-c/

//https://stackoverflow.com/questions/8704161/c-sharp-array-within-a-struct

/*
 * }
class Block {
char[] version;
int  field1;
int  field2;
RECORD[] records;
char[] filler1;
}

class MyReader
{
BinaryReader Reader;

Block ReadBlock()
{
    Block block=new Block();
    block.version=Reader.ReadChars(4);
    block.field1=Reader.ReadInt32();
    block.field2=Reader.ReadInt32();
    block.records=new Record[15];
    for(int i=0;i<block.records.Length;i++)
        block.records[i]=ReadRecord();
    block.filler1=Reader.ReadChars(24);
    return block;
}

Record ReadRecord()
{
    ...
}

public MyReader(BinaryReader reader)
{
    Reader=reader;
}
}*/

namespace EPSSEditor
{

    public abstract class EPSSBase
    {
        public int prePadding = 0;
        public abstract int length();

        public abstract int write(ref BinaryWriter writer);
        public abstract int Read(ref BinaryReader reader, EPSSSpi spi, ref string errorMessage);
        public virtual bool isValidBlock() { return true; }
    }


    public abstract class EPSSSpi
    {
        public EPSSSpi_main main;
        public EPSSSpi_extended ext;
        public EPSSSpi_splitInfo split;

        public EPSSSpi_sounds sounds;
        public EPSSSpi_extSounds extSounds;

        public EPSSSpi_samples samples;

        public abstract void initialize();


        public virtual int save(Uri dest)
        {
            int result = 0;

            try
            {
                FileStream fs = new FileStream(dest.LocalPath, FileMode.Create, FileAccess.ReadWrite);
                BinaryWriter writer = new BinaryWriter(fs);

                if (main.isValidBlock())
                {

                    result = main.write(ref writer);

                    result = ext.write(ref writer);
                }
                else
                {
                    throw (new Exception("Old version of SPI not supported!"));
                }

                result = split.write(ref writer);

                result = sounds.write(ref writer);

                result = extSounds.write(ref writer);

                result = samples.write(ref writer);

                writer.Close();
                //                fs.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception during save: {0}", ex.ToString());
                result = 1;
            }
            return result;
        }

        
        public virtual int Load(Uri src, ref string errorMessage)
        {
            int result = 0;

            try
            {
                FileStream fs = new FileStream(src.LocalPath, FileMode.Open, FileAccess.Read);
                BinaryReader reader = new BinaryReader(fs);
                bool isG0 = false;

                result = main.Read(ref reader, this, ref errorMessage);
                if (result == 0)
                {
                    byte low = main.i_fileID.versionLow;
                    byte high= main.i_fileID.versionHigh;
                    isG0 = (low == 0);
                    if (isG0)
                    {
                        string fileName = Path.GetFileNameWithoutExtension(src.LocalPath);
                        ext.InitForG0(fileName);
                    }
                    else
                    {
                        result = ext.Read(ref reader, this, ref errorMessage);
                    }
                }
                if (result == 0) result = split.Read(ref reader, this, ref errorMessage);
                if (result == 0) result = sounds.Read(ref reader, this, ref errorMessage);
                if (result == 0)
                {
                    if (isG0)
                    {
                        extSounds.InitForG0(this);
                    }
                    else
                    {
                        result = extSounds.Read(ref reader, this, ref errorMessage);
                    }
                }
                if (result == 0) result = samples.Read(ref reader, this, ref errorMessage);
            
                reader.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception during load: {0}", ex.ToString());
                result = 1;
            }
            return result;
        }
    }



    public class EPSSSpiG0G1 : EPSSSpi
    {
        public EPSSSpiG0G1() { }


        public override void initialize()
        {
            main = new EPSSSpi_main();
            ext = new EPSSSpi_extended();
            split = new EPSSSpi_splitInfo();
            sounds = new EPSSSpi_sounds();
            extSounds = new EPSSSpi_extSounds();
            samples = new EPSSSpi_samples();
        }
    }


    public class EPSSSpiGen2 : EPSSSpi
    {
        public EPSSSpiGen2() { }


        public override void initialize()
        {
            main = new EPSSSpi_main();
            ext = new EPSSSpi_extendedGen2();
            split = new EPSSSpi_splitInfoGen2();
            sounds = new EPSSSpi_sounds();
            extSounds = new EPSSSpi_extSounds();
            samples = new EPSSSpi_samples();
        }
    }




    public class EPSSSpi_main : EPSSBase
    {
        public EPSSSpi_i_no_of_MIDICh i_no_of_MIDIch; // 00
        public EPSSSpi_i_no_of_sounds i_no_of_sounds; // 02
        public UInt32 i_filelen;  // 04
        public UInt16 i_patch_offset;  // 08
        public UInt16 i_sinfo_offset; // 0A
        public UInt16 i_sdata_offset; // 0C
        public EPSSSpi_fileId i_fileID; // 0E 

        
        public override int write(ref BinaryWriter writer)
        {
            int result = 0;

            try
            {
                writer.WriteBigEndian(i_no_of_MIDIch.data);
                writer.WriteBigEndian(i_no_of_sounds.data);

                writer.WriteBigEndian(i_filelen);
                writer.WriteBigEndian(i_patch_offset);
                writer.WriteBigEndian(i_sinfo_offset);
                writer.WriteBigEndian(i_sdata_offset);
                writer.WriteBigEndian(i_fileID.data);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception during EPSSSpi_main.write: {0}", ex.ToString());
                result = 1;
            }

            return result;
        }


        public override int Read(ref BinaryReader reader, EPSSSpi spi, ref string errorMessage)
        {
            int result = 0;
            try
            {
                i_no_of_MIDIch.data = reader.ReadBigEndianUInt16();
                i_no_of_sounds.data = reader.ReadBigEndianUInt16();

                i_filelen = reader.ReadBigEndianUInt32();
                i_patch_offset = reader.ReadBigEndianUInt16();
                i_sinfo_offset = reader.ReadBigEndianUInt16();
                i_sdata_offset = reader.ReadBigEndianUInt16();
                i_fileID.data = reader.ReadBigEndianUInt16();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception during EPSSSpi_main.Read: {0}", ex.ToString());
                errorMessage = ex.ToString();
                result = 1;
            }

            return result;
        }

        public override int length()
        {
            return 16;
        }


        public override bool isValidBlock()
        {

            // What we currently support.
            if (i_fileID.versionHigh == 1 ||
                (i_fileID.versionLow == 2 && i_fileID.versionHigh == 0))
            {
                return true;
            }
            return false;
        }
    }



    public struct EPSSSpi_i_no_of_MIDICh   // [1-16]
    {
        internal UInt16 data;

        // public Byte no_of_MIDICh : 4
        public Byte no_of_MIDICh
        {
            get { return (byte)((data & 0xf) + 1); }
            set { data = (UInt16)((data & ~0xf) | ((value - 1) & 0xf)); }
        }
    }

    public struct EPSSSpi_i_no_of_sounds
    {
        internal UInt16 data;

        // public Byte no_of_sounds : 8
        public Byte no_of_sounds
        {
            get { return (byte)(data & 0xff); }
            set { data = (UInt16)((data & ~0xff) | (value & 0xff)); }
        }
    }


    public struct EPSSSpi_fileId
    {
        internal UInt16 data;

        // public Byte versionLow : 8
        public Byte versionLow  // 0x0 - Generation 0, 0x1 - Generation 1, 0x2 - Generation 2 (with program change)
        { 
            get { return (byte)(data & 0xff); }
            set { data = (UInt16)((data & ~0xff) | (value & 0xff)); }
        }


        // public Byte versionHigh : 8
        public Byte versionHigh  // 0x0 - Generation 0, 0x1 - Generation 1, 0x2 - Precalculated 12.5,

        {
            get { return (byte)((data >> 8) & 0xff); }
            set { data = (UInt16)((data & ~(0xff << 8)) | ((value & 0xff) << 8)); }
            
        }
    }


    public class EPSSSpi_extended : EPSSBase
    {
        public UInt16 i_sx_offset; //10
        public UInt16 i_crtime; // 12 
        public UInt16 i_crdate; // 14
        public UInt16 i_chtime; // 16
        public UInt16 i_chdate; // 18
        public string i_pname; // 1A-21
        public UInt16 i_mainlen; //22
        public UInt16 i_splitlen; // 24
        public UInt16 i_xsinflen; // 26
        public UInt16 i_sinflen; // 28
        public string i_patchinfo; // 30 . Up to 32 bytes.

        public void InitForG0(string filename)
        {
            i_sx_offset = 0;
            i_crtime = 0; // TODO
            i_crdate = 0; // TODO
            i_chtime = 0; // TODO
            i_chdate = 0; // TODO
            i_pname = filename;
            i_mainlen = 16; // Check!
            i_splitlen = 2;
            i_xsinflen = 0;
            i_sinflen = 16;
            i_patchinfo = filename + " conv from v0";
        }

        public virtual void writeExpansionBytes(ref BinaryWriter writer)
        {
            writer.Write("".ToFixedByteStream(6));
        }

        public virtual void writeAdditionalBytes(ref BinaryWriter writer)
        {
            // None
        }


        public override int write(ref BinaryWriter writer)
        {
            int result = 0;

            try
            {
                writer.WriteBigEndian(i_sx_offset);
                writer.WriteBigEndian(i_crtime);
                writer.WriteBigEndian(i_crdate);
                writer.WriteBigEndian(i_chtime);
                writer.WriteBigEndian(i_chdate);

                writer.Write(i_pname.ToFixedByteStream(8));
 
                writer.WriteBigEndian(i_mainlen);
                writer.WriteBigEndian(i_splitlen);
                writer.WriteBigEndian(i_xsinflen);
                writer.WriteBigEndian(i_sinflen);

                writeExpansionBytes(ref writer);
                
                writer.Write(i_patchinfo.ToFixedByteStream(32));

                writeAdditionalBytes(ref writer);
     
            }

            catch (Exception ex)
            {
                Console.WriteLine("Exception during EPSSSpi_extended.write: {0}", ex.ToString());
                result = 1;
            }

            return result;
        }

        public override int Read(ref BinaryReader reader, EPSSSpi spi, ref string errorMessage)
        {
            int result = 0;

            try
            {
                
                if (spi.main.i_fileID.versionLow == 2)
                {
                    throw (new Exception("Generation 2 patchfile NYI."));
                }


                i_sx_offset = reader.ReadBigEndianUInt16();
                i_crtime = reader.ReadBigEndianUInt16();
                i_crdate = reader.ReadBigEndianUInt16();
                i_chtime = reader.ReadBigEndianUInt16();
                i_chdate = reader.ReadBigEndianUInt16();

                byte[] bs = reader.ReadBytes(8);
                i_pname = bs.FromFixedByteStream().Trim();

                i_mainlen = reader.ReadBigEndianUInt16();
                i_splitlen = reader.ReadBigEndianUInt16();
                i_xsinflen = reader.ReadBigEndianUInt16();
                i_sinflen = reader.ReadBigEndianUInt16();

                byte[] expansions = reader.ReadBytes(6);

                bs = reader.ReadBytes(32);
                i_patchinfo = bs.FromFixedByteStream().Trim();

                // No additional bytes
            }

            catch (Exception ex)
            {
                Console.WriteLine("Exception during EPSSSpi_extended.Rrite: {0}", ex.ToString());
                errorMessage = ex.ToString();
                result = 1;
            }

            return result;
        }

        public override int length()
        {
            return 80 - 16; // !! Note that documentation start this offset at 16. This should be the length of this block only!
        }

    }

    public class EPSSSpi_extendedGen2 : EPSSSpi_extended
    {
        public UInt32 i_pchg_offset = 0; // 2A Offset to program change. 0 if not used.

        public UInt32 i_sinfo_offset_g2; // $50
        public UInt32 i_sdata_offset_g2; // $54

        public override void writeExpansionBytes(ref BinaryWriter writer)
        {
            writer.WriteBigEndian(i_pchg_offset);
            writer.Write("".ToFixedByteStream(2));
        }

        public override void writeAdditionalBytes(ref BinaryWriter writer)
        {
            writer.WriteBigEndian(i_sinfo_offset_g2);
            writer.WriteBigEndian(i_sdata_offset_g2);
        }

        public override int length()
        {
            return base.length() + 8;
        }
    }



        public class EPSSSpi_splitInfo : EPSSBase
    {

        public EPSSSpi_midiChannelSplit[] channels;

        
        public override int write(ref BinaryWriter writer)
        {
            int result = 0;

            try
            {
                // Pad out with any necessary padding bytes.
                if (prePadding != 0) writer.Write("".ToFixedByteStream(prePadding));

                foreach (EPSSSpi_midiChannelSplit channel in channels) 
                {
                    channel.write(ref writer);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception during EPSSSpi_splitInfo.write: {0}", ex.ToString());
                result = 1;
            }

            return result;
        }

        public override int Read(ref BinaryReader reader, EPSSSpi spi, ref string errorMessage)
        {
            int result = 0;

            try
            {
                reader.BaseStream.Seek(spi.main.i_patch_offset, SeekOrigin.Begin);
                byte maxSoundNo = spi.main.i_no_of_sounds.no_of_sounds;
                List<EPSSSpi_midiChannelSplit> channelList = new List<EPSSSpi_midiChannelSplit>();
                for (int midiChannel = 1; midiChannel <= spi.main.i_no_of_MIDIch.no_of_MIDICh; midiChannel++)
                {
                    EPSSSpi_midiChannelSplit channel = new EPSSSpi_midiChannelSplit();
                    channel.data = new EPSSSpi_soundAndPitch[128];

                    for (int tone = 0; tone < 128; tone++)
                    {
                        EPSSSpi_soundAndPitch snp = new EPSSSpi_soundAndPitch(0);
                        UInt16 data = reader.ReadBigEndianUInt16();
                        snp.data = data;
                        //byte pitch = reader.ReadByte();
                        //byte sound = reader.ReadByte();
                        if (snp.sound > maxSoundNo)
                        {
                            throw (new Exception("Corrupt split table. Sound number exceeeds max sounds in spi."));
                        }
                        //snp.sound = sound;
                        //snp.pitch = pitch;
                        channel.data[tone] = snp;
                    }
                    channelList.Add(channel);
                }
                channels = channelList.ToArray();
            }

            catch (Exception ex)
            {
                Console.WriteLine("Exception during EPSSSpi_splitInfo.Read: {0}", ex.ToString());
                errorMessage = ex.ToString();
                result = 1;
            }

            return result;
        }

        public override int length()
        {
            int l = 0;
            foreach (EPSSSpi_midiChannelSplit channel in channels)
            {
                l += channel.length();
            }
            return l;
        }
    }


    public class EPSSSpi_splitInfoGen2 : EPSSSpi_splitInfo
    {
        public EPSSSpi_programChangeSplit[] programs;


        public override int write(ref BinaryWriter writer)
        {
            int result = base.write(ref writer);
            if (result == 0)
            {
                try
                {
                    int i = 0;
                    foreach (EPSSSpi_programChangeSplit program in programs)
                    {
                        program.write(ref writer);
                        i++;
                    }
                    if (i != 128)
                    {
                        throw (new ArgumentException("Too few program changes!"));
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Exception during EPSSSpi_splitInfoGen3.write: {0}", ex.ToString());
                    result = 1;
                }
            }

            return result;
        }

        public override int length()
        {
            int l = base.length();

            foreach (var program in programs)
            {
                l += program.length();
            }

            return l;
        }
    }


    public class EPSSSpi_programChangeSplit
    {
        public EPSSSpi_soundAndPitchGen2[] data;


        public int write(ref BinaryWriter writer)
        {
            int result = 0;

            try
            {
                int i = 0;
                foreach (EPSSSpi_soundAndPitchGen2 sp in data)
                {
                    writer.WriteBigEndian(sp.data);
                    i++;
                }
                if (i != 128)
                {
                    throw (new ArgumentException("Too few split points in a program change split!"));
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception during EPSSSpi_splitInfoGen3.write: {0}", ex.ToString());
                result = 1;
            }

            return result;
        }

        public int length()
        {
            return 128 * 4;
        }
    }


    public struct EPSSSpi_soundAndPitchGen2
    {
        internal UInt32 data;

        // public UInt16 sound : 16
        public UInt16 sound
        {
            get { return (UInt16)(data & 0xffff); }
            set { data = (UInt32)((data & ~0xffff) | (value & (UInt32)0xffff)); }
        }

        // public UInt16 pitch : 7
        public UInt16 pitch
        {
            get { return (UInt16)((data >> 16) & 0x7f); }
            set { data = (UInt32)((data & ~(0x7f << 16)) | ((value & (UInt32)0x7f) << 16)); }
        }

        // public Byte noSound : 0: use pitch, 1 no sound
        public Byte noSound
        {
            get { return (byte)((data >> 31) & (UInt32)0x1); }
            set { data = (UInt32)((data & ~(0x1 << 31)) | ((value & (UInt32)0x1) << 31)); }
        }
    }


    public class EPSSSpi_midiChannelSplit
    {
        public EPSSSpi_soundAndPitch[] data;

        public int write(ref BinaryWriter writer)
        {
            int result = 0;

            try
            {

/*                for (int i = 0; i < 128; i++)
                {
                    writer.WriteBigEndian(data[i].data);
                }
                */
                foreach (EPSSSpi_soundAndPitch sp in data)
                {
                    writer.WriteBigEndian(sp.data);
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception during EPSSSpi_splitInfo.write: {0}", ex.ToString());
                result = 1;
            }

            return result;
        }


        public int length()
        {
            return 128 * 2;
        }

    }


    public struct EPSSSpi_soundAndPitch
    {
        internal UInt16 data;

        public EPSSSpi_soundAndPitch(UInt16 value) { data = value;  }

        // public Byte sound : 8 bit
        public Byte sound
        {
            get { return (byte)(data & 0xff); }
            set { data = (UInt16)((data & ~0xff) | (value & 0xff)); }
        }

        // public Byte pitch : 7 bit
        public Byte pitch
        {
            get { return (byte)((data >> 8) & 0x7f); }
            set { data = (UInt16)((data & ~(0x7f << 8)) | ((value & 0x7f) << 8));  }
        }

        // public Byte noSound : 0: use pitch, 1 no sound
        public Byte noSound
        {
            get { return (byte)((data >> 15) & 0x1); }
            set { data = (UInt16)((data & ~(0x1 << 15)) | ((value & 0x1) << 15)); }
        }

    }



    public class EPSSSpi_sounds : EPSSBase
    {
        public EPSSSpi_soundInfo[] sounds;

        public override int write(ref BinaryWriter writer)
        {
            int result = 0;

            try
            {
                int j = 0;
                long k = writer.BaseStream.Position;
                foreach (EPSSSpi_soundInfo sound in sounds)
                {
                    sound.write(ref writer);
                    j++;
                }
                long diff = writer.BaseStream.Position - k;
                //System.Windows.Forms.MessageBox.Show(j.ToString() + " " + diff.ToString() + " " + (diff/16).ToString());
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception during EPSSSpi_sounds.write: {0}", ex.ToString());
                result = 1;
            }

            return result;
        }

        public override int Read(ref BinaryReader reader, EPSSSpi spi, ref string errorMessage)
        {
            int result = 0;

            try
            {
                reader.BaseStream.Seek(spi.main.i_sinfo_offset, SeekOrigin.Begin);
                byte maxSoundNo = spi.main.i_no_of_sounds.no_of_sounds;

                List<EPSSSpi_soundInfo> soundList = new List<EPSSSpi_soundInfo>();
                for (int i =0; i < maxSoundNo; i++)
                {
                    EPSSSpi_soundInfo s = new EPSSSpi_soundInfo();
                    s.Read(ref reader, ref errorMessage);
                    soundList.Add(s);
                }
                sounds = soundList.ToArray();
            }

            catch (Exception ex)
            {
                Console.WriteLine("Exception during EPSSSpi_splitInfo.Read: {0}", ex.ToString());
                errorMessage = ex.ToString();
                result = 1;
            }

            return result;
        }

        public override int length()
        {
            int l = 0;
            foreach (EPSSSpi_soundInfo sound in sounds)
            {
                l += sound.length();
            }
            return l;
        }
    }


    public class EPSSSpi_soundInfo
    {
        public UInt32 s_sampstart; // 00
        public UInt32 s_sampend; // 04
        public UInt32 s_loopstart; // 08
        public EPSSSpi_loopmode s_loopmode; // 0C
        public EPSSSpi_s_gr_frek s_gr_freq; // 0E

        public int length() { return 16; }

        public int write(ref BinaryWriter writer)
        {
            int result = 0;

            try
            {
                writer.WriteBigEndian(s_sampstart);
                writer.WriteBigEndian(s_sampend);
                writer.WriteBigEndian(s_loopstart);
                writer.WriteBigEndian(s_loopmode.data);
                writer.WriteBigEndian(s_gr_freq.data);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception during EPSSSpi_soundInfo.write: {0}", ex.ToString());
                result = 1;
            }

            return result;
        }


        public int Read(ref BinaryReader reader, ref string errorMessage)
        {
            int result = 0;

            try
            {
                s_sampstart = reader.ReadBigEndianUInt32();
                s_sampend = reader.ReadBigEndianUInt32();
                s_loopstart = reader.ReadBigEndianUInt32();
                UInt16 data = reader.ReadBigEndianUInt16();
                s_loopmode.data = data;
                data = reader.ReadBigEndianUInt16();
                s_gr_freq.data = data;
            }

            catch (Exception ex)
            {
                Console.WriteLine("Exception during EPSSSpi_sountInfo.Read: {0}", ex.ToString());
                errorMessage = ex.ToString();
                result = 1;
            }

            return result;
        }
    }


    public class EPSSSpi_extSounds : EPSSBase
    {
        public EPSSSpi_extSoundInfo[] sounds;


        public void InitForG0(EPSSSpi spi)
        {
            byte maxSoundNo = spi.main.i_no_of_sounds.no_of_sounds;
            List<EPSSSpi_extSoundInfo> soundList = new List<EPSSSpi_extSoundInfo>();
            for (int i = 0; i < maxSoundNo; i++)
            {
                EPSSSpi_extSoundInfo s = new EPSSSpi_extSoundInfo();
                string name = "Sample" + i.ToString();
                s.InitForG0(name);
                soundList.Add(s);
            }
            sounds = soundList.ToArray();
        }
        
        public override int write(ref BinaryWriter writer)
        {
            int result = 0;

            try
            {
                int j = 0;
                foreach (EPSSSpi_extSoundInfo sound in sounds)
                {
                    sound.write(ref writer);
                    j++;
                }

               // System.Windows.Forms.MessageBox.Show(j.ToString());
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception during EPSSSpi_extSounds.write: {0}", ex.ToString());
                result = 1;
            }

            return result;
        }

        public override int Read(ref BinaryReader reader, EPSSSpi spi, ref string errorMessage)
        {
            int result = 0;

            try
            {
                reader.BaseStream.Seek(spi.ext.i_sx_offset, SeekOrigin.Begin);
                byte maxSoundNo = spi.main.i_no_of_sounds.no_of_sounds;

                List<EPSSSpi_extSoundInfo> soundList = new List<EPSSSpi_extSoundInfo>();
                for (int i = 0; i < maxSoundNo; i++)
                {
                    EPSSSpi_extSoundInfo s = new EPSSSpi_extSoundInfo();
                    s.Read(ref reader, ref errorMessage);
                    soundList.Add(s);
                }
                sounds = soundList.ToArray();
            }

            catch (Exception ex)
            {
                Console.WriteLine("Exception during EPSSSpi_splitInfo.Read: {0}", ex.ToString());
                errorMessage =  ex.ToString();
                result = 1;
            }

            return result;
        }

        public override int length()
        {
            int l = 0;
            foreach (EPSSSpi_extSoundInfo sound in sounds)
            {
                l += sound.length();
            }
            return l;
        }
    }


    public class EPSSSpi_extSoundInfo
    {
        public string s_sampname; // 0
        public string s_extname; // 08
        public UInt16 s_extvolume; // percent
        public UInt16 s_subtone; // only valid when sound if subtone

        // reserved after this. Fill with zeroes.

        public void InitForG0(string name)
        {
            s_sampname = name;
            s_extname = name;
            s_extvolume = 100; //??
            s_subtone = 0;
        }

        public int length() { return 64; }

        public int write(ref BinaryWriter writer)
        {
            int result = 0;

            try
            {
                writer.Write(s_sampname.ToFixedByteStream(8));
                writer.Write(s_extname.ToFixedByteStream(16));
                writer.WriteBigEndian(s_extvolume);
                writer.WriteBigEndian(s_subtone);
                writer.Write("".ToFixedByteStream(length()-8-16-2-2));
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception during EPSSSpi_extSoundInfo.write: {0}", ex.ToString());
                result = 1;
            }

            return result;
        }

        public int Read(ref BinaryReader reader, ref string errorMessage)
        {
            int result = 0;

            try
            {
                byte[] bs = reader.ReadBytes(8);
                s_sampname = bs.FromFixedByteStream();

                bs = reader.ReadBytes(16);
                s_extname = bs.FromFixedByteStream();

                s_extvolume = reader.ReadBigEndianUInt16();
                s_subtone = reader.ReadBigEndianUInt16();

                int numBytesLeft = length() - 8 - 16 - 2 - 2;
                bs = reader.ReadBytes(numBytesLeft);
            }

            catch (Exception ex)
            {
                Console.WriteLine("Exception during EPSSSpi_splitInfo.Read: {0}", ex.ToString());
                errorMessage = ex.ToString();
                result = 1;
            }

            return result;
        }
    }


    public struct EPSSSpi_loopmode
    {
        internal UInt16 data;

        // public Byte loopmode : 2
        public byte loopmode // 0x00 - reserved, 0x01 - One shot, 0x02 - Loop on, 0x03 - reserved
        {
            get { return (byte)(data & 0x3); }
            set { data = (UInt16)((data & ~0x3) | (value & 0x3)); }
        }

        // public Byte vvfe : 6
        public byte vvfe
        {
            get { return (byte)((data >> 2) & 0x3f); }
            set { data = (UInt16)((data & ~(0x3f << 2)) | ((value & 0x3f) << 2)); }
        }

        // public Byte toneoffset : 1
        public sbyte toneoffset
        {
            get { 
                byte b = (byte)((data >> 8) & 0xff);
                if (b < 128)
                {
                    return (sbyte)b;
                }
                else
                {
                    return (sbyte)((int)b - 256); // 255->-1,254->-2
                }
            }
            set { data = (UInt16)((data & ~(0xff << 8)) | ((value & 0xff) << 8)); }
        }

    }

    public struct EPSSSpi_s_gr_frek
    {
        internal UInt16 data;

        // public Byte orgFreq : 2
        public Byte orgFreq // 0x00 - 6250 Hz, 0x01 - 12517 Hz, 0x10 - 25033 Hz, 0x11 - 50066 Hz (Not used, only informational)
        {
            get { return (byte)(data & 0x3); }
            set { data = (UInt16)((data & ~0x3) | (value & 0x3)); }
        }


        // public Byte stereoPan : 2
        public Byte stereoPan // 0x00 - Default panning, 0x01 - Undefined, 0x10 - Left, 0x11 - Right
        {
            get { return (byte)((data >> 2) & 0x3); }
            set { data = (UInt16)((data & ~(0x3 << 2)) | ((value & 0x3) << 2)); }
        }


        // public Byte stereoType : 2
        public Byte stereoType // 0x00 - No effect, 0x01 - Reserved, 0x10 - Reserved, 0x11 - Reserved
        {
            get { return (byte)((data >> 4) & 0x3); }
            set { data = (UInt16)((data & ~(0x3 << 4)) | ((value & 0x3) << 4)); }
        }

        // public Byte aftertouch : 1
        public Byte aftertouch // 0x0 - Off , 0x1 - On
        {
            get { return (byte)((data >> 6) & 0x1); }
            set { data = (UInt16)((data & ~(0x1 << 6)) | ((value & 0x1) << 6)); }
        }

        // public Byte mode : 1
        public Byte mode // 0x0 - Stereo (not used) , 0x1 - Mono
        {
            get { return (byte)((data >> 7) & 0x1); }
            set { data = (UInt16)((data & ~(0x1 << 7)) | ((value & 0x1) << 7)); }
        }


        // public Byte reserved : 4
        public Byte reserved // Not used
        {
            get { return (byte)((data >> 8) & 0xf); }
            set { data = (UInt16)((data & ~(0xf << 8)) | ((value & 0xf) << 8)); }
        }


        // public Byte soundType: 2
        public Byte soundType // 0x00 - physical sound, 0x01 - virtual sound or subtone, s_subtone is the number of the sound which contains
            // the base sample for this sound, 0x02 - reserved, 0x03 - reserved
        {
            get { return (byte)((data >> 12) & 0x3); }
            set { data = (UInt16)((data & ~(0x3 << 12)) | ((value & 0xd) << 12)); }
        }


        // public Byte velocity : 1
        public Byte velocity // 0 - MIDI Velocity = VVFE, MIDI PolyPressure = Volume, 1 - MIDI Velocity = Volume, MIDI PolyPressure=VVFE
        {
            get { return (byte)((data >> 14) & 0x1); }
            set { data = (UInt16)((data & ~(0x1 << 14)) | ((value & 0x1) << 14)); }

        }

        // public Byte drum : 1
        public Byte drum // 0 - Normal sound, 1 - Drumsound (no freq calculation made)
        {
            get { return (byte)((data >> 15) & 0x1); }
            set { data = (UInt16)((data & ~(0x1 << 15)) | ((value & 0x1) << 15)); }
        }
    }


    public class EPSSSpi_samples : EPSSBase
    {
        public EPSSSpi_sample[] samples;


        public override int write(ref BinaryWriter writer)
        {
            int result = 0;

            try
            {
                foreach (EPSSSpi_sample smp in samples)
                {
                    smp.write(ref writer);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception during EPSSSpi_samples.write: {0}", ex.ToString());
                result = 1;
            }

            return result;
        }

        public override int Read(ref BinaryReader reader, EPSSSpi spi, ref string errorMessage)
        {
            int result = 0;

            try
            {                
                reader.BaseStream.Seek(spi.main.i_sdata_offset, SeekOrigin.Begin);
                byte maxSoundNo = spi.main.i_no_of_sounds.no_of_sounds;
                List<EPSSSpi_sample> sampleList = new List<EPSSSpi_sample>();
                for (int i=0; i < maxSoundNo; i++)
                {
                    EPSSSpi_soundInfo sndInfo = spi.sounds.sounds[i];
                    EPSSSpi_sample sample = new EPSSSpi_sample();
                    sample.Read(ref reader, sndInfo.s_sampstart, sndInfo.s_sampend - sndInfo.s_sampstart, ref errorMessage);
                    sampleList.Add(sample);
    
                }
                samples = sampleList.ToArray();

            }

            catch (Exception ex)
            {
                Console.WriteLine("Exception during EPSSSpi_samples.Read: {0}", ex.ToString());
                errorMessage = ex.ToString();
                result = 1;
            }

            return result;
        }

        public override int length()
        {
            int l = 0;
            foreach (EPSSSpi_sample smp in samples)
            {
                l += smp.length();
            }
            return l;
        }
    }


    public class EPSSSpi_sample
    {

        public byte[] data;

        public int length() { return data.Length; }

        public int write(ref BinaryWriter writer)
        {
            int result = 0;

            try
            {
                writer.Write(data);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception during EPSSSpi_sample.write: {0}", ex.ToString());
                result = 1;
            }

            return result;
        }


        public void loadSpl(Uri path)
        {
            try
            {
                long len = new FileInfo(path.LocalPath).Length;
                FileStream fs = new FileStream(path.LocalPath, FileMode.Open, FileAccess.Read);
                BinaryReader reader = new BinaryReader(fs);
                data = reader.ReadBytes((int)len);
                reader.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception during EPSSSpi_sample.loadSpl: {0}", ex.ToString());

            }
        }


        public int Read(ref BinaryReader reader, UInt32 start, UInt32 length, ref string errorMessage)
        {
            int result = 0;

            try
            {
                reader.BaseStream.Seek(start, SeekOrigin.Begin);
                data = reader.ReadBytes((int)length);
            }

            catch (Exception ex)
            {
                Console.WriteLine("Exception during EPSSSpi_sample.Read: {0}", ex.ToString());
                errorMessage = ex.ToString();
                result = 1;
            }

            return result;
        }
    }
   
}
