using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.IO;
using NAudio.Wave;
using NAudio.MediaFoundation;

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
        public abstract int length();

        public abstract int write(ref BinaryWriter writer); 

        public virtual bool isValidBlock() { return true; }
    }



    public class EPSSSpi
    {
        public EPSSSpi_main main;
        public EPSSSpi_extended ext;
        public EPSSSpi_splitInfo split;

        public EPSSSpi_sounds sounds;
        public EPSSSpi_extSounds extSounds;

        public EPSSSpi_samples samples;



        public int save(Uri dest)
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
                Console.WriteLine("Exception during save: ", ex.ToString());
                result = 1;
            }
            return result;
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
                Console.WriteLine("Exception during EPSSSpi_main.write: ", ex.ToString());
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

            if (i_fileID.versionHigh == 1)
            {
                return true;
            }
            return false;
        }
    }



    public struct EPSSSpi_i_no_of_MIDICh
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
        public Byte versionLow // 0x0 - Generation 0, 0x1 - Generation 1, 0x2 - Precalculated 12.5, 0x3 - reserved
        {
            get { return (byte)(data & 0xff); }
            set { data = (UInt16)((data & ~0xff) | (value & 0xff)); }
        }


        // public Byte versionHigh : 8
        public Byte versionHigh
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

                writer.Write("".ToFixedByteStream(6));

                writer.Write(i_patchinfo.ToFixedByteStream(32));
            }

            catch (Exception ex)
            {
                Console.WriteLine("Exception during EPSSSpi_extended.write: ", ex.ToString());
                result = 1;
            }

            return result;
        }

        public override int length()
        {
            return 64;
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
                foreach(EPSSSpi_midiChannelSplit channel in channels) 
                {
                    channel.write(ref writer);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception during EPSSSpi_splitInfo.write: ", ex.ToString());
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
                Console.WriteLine("Exception during EPSSSpi_splitInfo.write: ", ex.ToString());
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

        // public Byte sound : 8
        public Byte sound
        {
            get { return (byte)(data & 0xff); }
            set { data = (UInt16)((data & ~0xff) | (value & 0xff)); }
        }

        // public Byte pitch : 7
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
                Console.WriteLine("Exception during EPSSSpi_sounds.write: ", ex.ToString());
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
                Console.WriteLine("Exception during EPSSSpi_soundInfo.write: ", ex.ToString());
                result = 1;
            }

            return result;
        }
    }


    public class EPSSSpi_extSounds : EPSSBase
    {
        public EPSSSpi_extSoundInfo[] sounds;

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
                Console.WriteLine("Exception during EPSSSpi_extSounds.write: ", ex.ToString());
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
                Console.WriteLine("Exception during EPSSSpi_extSoundInfo.write: ", ex.ToString());
                result = 1;
            }

            return result;
        }
    }


    public struct EPSSSpi_loopmode
    {
        internal UInt16 data;

        // public Byte loopmode : 2
        public Byte loopmode // 0x00 - reserved, 0x01 - One shop, 0x02 - Loop on, 0x03 - reserved
        {
            get { return (byte)(data & 0x3); }
            set { data = (UInt16)((data & ~0x3) | (value & 0x3)); }
        }

        // public Byte vvfe : 6
        public Byte vvfe
        {
            get { return (byte)((data >> 6) & 0x3f); }
            set { data = (UInt16)((data & ~(0x3f << 6)) | ((value & 0x3f) << 6)); }
        }

        // public Byte toneoffset : 1
        public byte toneoffset
        {
            get { return (byte)((data >> 8) & 0xff); }
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
                Console.WriteLine("Exception during EPSSSpi_samples.write: ", ex.ToString());
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
                Console.WriteLine("Exception during EPSSSpi_sample.write: ", ex.ToString());
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
                Console.WriteLine("Exception during EPSSSpi_sample.loadSpl: ", ex.ToString());

            }
        }
    }





    public class SpiCreator
    {
        public int noOfMidiCh = 16; // always create 16 channel maps from now on
        public int noOfSounds;
        
        public void initialize(ref EPSSSpi spi)
        {
            spi.main = new EPSSSpi_main();
            spi.ext = new EPSSSpi_extended();
            spi.split = new EPSSSpi_splitInfo();
            spi.sounds = new EPSSSpi_sounds();
            spi.extSounds = new EPSSSpi_extSounds();
            spi.samples = new EPSSSpi_samples();
        }


        public void fillInMain(ref EPSSSpi spi)
        {
            spi.main.i_no_of_MIDIch.no_of_MIDICh = (byte)noOfMidiCh;

            spi.main.i_no_of_sounds.no_of_sounds = (byte)(noOfSounds);

            spi.main.i_patch_offset = (UInt16)(spi.main.length() + spi.ext.length());

            spi.main.i_fileID.versionLow = 1;
            spi.main.i_fileID.versionHigh = 1;
        }


        public void fillInExt(ref EPSSSpi spi, string name, string info)
        {
            DateTime dr = DateTime.Now;
            spi.ext.i_crtime = dr.ToDosDateTime();
            spi.ext.i_crdate = dr.ToDosDateTime();
            spi.ext.i_chtime = dr.ToDosDateTime();
            spi.ext.i_chdate = dr.ToDosDateTime();
            spi.ext.i_pname = name;
            spi.ext.i_mainlen = (UInt16)(spi.main.length() + spi.ext.length());
            spi.ext.i_splitlen = 2;
            spi.ext.i_xsinflen = 64;
            spi.ext.i_sinflen = 16;
            spi.ext.i_patchinfo = info;
        }

        private List<SpiSound> getSoundForMidiChannel(ref List<SpiSound> sounds, int midiChannel, bool omni)
        {
            List<SpiSound> channelSounds = new List<SpiSound>();
            foreach(SpiSound snd in sounds)
            {
                if (snd.midiChannel == midiChannel || omni)
                {
                    channelSounds.Add(snd);
                }
            }
            return channelSounds;
        }


        public void fillInSplit(ref EPSSSpi spi, ref List<SpiSound> sounds, bool omni)
        {
            List<EPSSSpi_midiChannelSplit> channels = new List<EPSSSpi_midiChannelSplit>();

            for (int ch = 0; ch < noOfMidiCh; ch++)
            {
                EPSSSpi_midiChannelSplit channel = new EPSSSpi_midiChannelSplit();
                channel.data = new EPSSSpi_soundAndPitch[128];

                List<SpiSound> sound = getSoundForMidiChannel(ref sounds, ch + 1, omni);

                bool useMidiSplit = false;
                foreach (SpiSound sndToFind in sound)
                {
                    if (sndToFind.startNote < 128 && sndToFind.endNote < 128)
                    {
                        useMidiSplit = true;
                        break;
                    }
                }

                if (sound.Count == 0) // empty channel
                {

                    for (int i = 0; i < 128; i++)
                    {
                        EPSSSpi_soundAndPitch sp = new EPSSSpi_soundAndPitch();
                        sp.sound = 0;
                        sp.pitch = 0;
                        sp.noSound = 1;
                        channel.data[i] = sp;


                    }
                    channels.Add(channel);
                }
                else if (useMidiSplit)
                {
                    for (int i = 0; i < 128; i++)
                    {
                        EPSSSpi_soundAndPitch sp = new EPSSSpi_soundAndPitch();
                        sp.sound = 0;
                        sp.pitch = 0;
                        sp.noSound = 1;
                        channel.data[i] = sp;
                    }
                    
                    foreach (SpiSound sndToFind in sound)
                    {
                        byte j = 0;
                        if (sndToFind.startNote < 128 && sndToFind.endNote < 128)
                        {
                            for (int i=sndToFind.startNote; i <= sndToFind.endNote; i ++)
                            {
                                EPSSSpi_soundAndPitch sp = new EPSSSpi_soundAndPitch();
                                sp.sound = (byte)sndToFind.soundNumber;
                                sp.pitch = (byte)(sndToFind.midiNote + j);
                                sp.noSound = 0;
                                j++;
                                channel.data[i] = sp;
                            }
                        }
                    }

                    channels.Add(channel);
                }
                else if (sound.Count == 1 && ch != 9)
                {
                    SpiSound snd = sound.First();


                    byte note = 60;
                    for (int i = 0; i < 128; i++)
                    {
                        EPSSSpi_soundAndPitch sp = new EPSSSpi_soundAndPitch();

                        if (i >= 36 && i <= 84)
                        {
                            sp.sound = (byte)snd.soundNumber;
                            sp.pitch = note++;
                            sp.noSound = 0;
                        }
                        else
                        {
                            sp.sound = 0;
                            sp.pitch = 0;
                            sp.noSound = 1;
                        }
                        channel.data[i] = sp;


                    }
                    channels.Add(channel);

                }
                else if (sound.Count > 1 && ch != 9)
                {
                    SpiSound snd = sound.First();
                    for (int i = 0; i < 128; i++)
                    {
                        EPSSSpi_soundAndPitch sp = new EPSSSpi_soundAndPitch();
                        bool found = false;
                        foreach (SpiSound sndToFind in sound)
                        {
                            if (sndToFind.midiNote == i+1)
                            {
                                sp.sound = (byte)sndToFind.soundNumber;
                                sp.pitch = 84;
                                sp.noSound = 0;
                                found = true;
                            }
                            if (found) break;
                        }
                        if (!found)
                        {
                            sp.sound = 0;
                            sp.pitch = 0;
                            sp.noSound = 1;

                        }
                        channel.data[i] = sp;
                    }
                    channels.Add(channel);
                }
                else if (ch == 9) // drums
                {

                    for (int i = 0; i < 128; i++)
                    {
                        EPSSSpi_soundAndPitch sp = new EPSSSpi_soundAndPitch();

                        byte note = 0;
                        byte spiSound = 0;
                        foreach (SpiSound snd in sound)
                        {
                            if (snd.midiNote == i)
                            {
                                note = 84; // normal pitch, TODO: add transpose here!
                                spiSound = (byte)snd.soundNumber;
                            }
                        }

                        if (note > 0)
                        {
                            sp.sound = spiSound;
                            sp.pitch = note;
                            sp.noSound = 0;
                        }
                        else
                        {
                            sp.sound = 0;
                            sp.pitch = 0;
                            sp.noSound = 1;
                        }

                        channel.data[i] = sp;

                    }
                    channels.Add(channel);
                }
                else if (sound.Count > 1)
                {
                    System.Windows.Forms.MessageBox.Show("More than one sound per midi channel not supported!");
                }

            }
            spi.split.channels = channels.ToArray();

            spi.main.i_sinfo_offset = (UInt16)(spi.main.length() + spi.ext.length() + spi.split.length());
        }


        public EPSSSpi_sample getSampleFromSpiSound(ref EPSSEditorData data, SpiSound snd, int freq)
        {
            EPSSSpi_sample smp = new EPSSSpi_sample();

            string outFile = Path.GetTempFileName();
            if (snd.convertSound(ref data, outFile, freq, AtariConstants.SampleBits, AtariConstants.SampleChannels))
            {
                using (var wav = File.OpenRead(outFile))
                {
                    wav.Seek(0, SeekOrigin.Begin);

                    WaveFileReader fr = new WaveFileReader(wav);

                    List<byte> spl = new List<byte>();
                    smp.data = new byte[fr.Length];
                    int read = fr.Read(smp.data, 0, smp.data.Length);
                }

                File.Delete(outFile);
            }


            /*
            Sound sound = data.getSoundFromSoundId(snd.soundId);
            if (sound != null)
            {
                string path = sound.path;

                float volume = 1.0f;
                float max = 1.0f;
                if (sound.parameters().normalize.normalize)
                {
                    volume = (float)(sound.parameters().normalize.normalizePercentage / 100.0);

                    max = 0;
                    using (var reader = new AudioFileReader(path))
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

                string outFile = Path.GetTempFileName();

                int outRate = 25033; // TODO: read the setting
                using (var reader = new AudioFileReader(path))
                {

                    // rewind and amplify
                    reader.Position = 0;
                    //                    reader.Volume = 1.0f / max;
                    reader.Volume = volume / max;


                    var outFormat = new WaveFormat(outRate, reader.WaveFormat.Channels);
                    using (var resampler = new MediaFoundationResampler(reader, outFormat))
                    {
                        resampler.ResamplerQuality = 60;
                        WaveFileWriter.CreateWaveFile(outFile, resampler);
                    }
                }



                using (var wav = File.OpenRead(outFile))
                {
                    wav.Seek(0, SeekOrigin.Begin);

                    WaveFileReader fr = new WaveFileReader(wav);

                    List<byte> spl = new List<byte>();

                    while (true)
                    {
                        float[] samples = fr.ReadNextSampleFrame();
                        if (samples == null) break;

                        byte b = (byte)((samples[0] + 1.0f) * 128.0f); // unsigned
                        //                  if (b < 128) b = (byte)(b + 128);
                        //                  else b = (byte)(b - 128);
                        spl.Add(b);
                    }
                    smp.data = spl.ToArray();
                }

                File.Delete(outFile);

            }
            else
            {
                //            smp.loadSpl(new Uri(@"D:\OneDrive\Atari\Emulators etc\HDimage\Syquest\LJUDMODF\F\SQR_WAV.SPL")); // TODO just for first test
                smp.loadSpl(new Uri(@"D:\OneDrive\Atari\Emulators etc\HDimage\Syquest\LJUDMODF\F\TECHNOSD.SPL")); // TODO just for first test
            }

            */

            return smp;
        }

        public EPSSSpi_soundInfo getSoundInfoFromSpiSound(EPSSSpi_sample smp, SpiSound snd)
        {
            EPSSSpi_soundInfo info = new EPSSSpi_soundInfo();
            info.s_sampstart = 0;
            info.s_sampend = (uint)smp.data.Length;
            info.s_loopstart = 0;
            info.s_loopmode.toneoffset = 0; // transpose?
            info.s_loopmode.loopmode = 1;
            info.s_loopmode.vvfe = 0;
            info.s_gr_freq.drum = 0;
            info.s_gr_freq.velocity = 0;
            info.s_gr_freq.soundType = 0;
            info.s_gr_freq.mode = 1;
            info.s_gr_freq.aftertouch = 0;
            info.s_gr_freq.stereoType = 0;
            info.s_gr_freq.stereoPan = 0;
            info.s_gr_freq.orgFreq = 3;

            return info;
        }

        public EPSSSpi_extSoundInfo getExtSoundInfoFromSpiSound(EPSSSpi_sample smp, SpiSound snd)
        {
            EPSSSpi_extSoundInfo extInfo = new EPSSSpi_extSoundInfo();
            extInfo.s_sampname = snd.name();  // "TstSam" + (i + 1).ToString();
            extInfo.s_extname = snd.extName(); ///  "Sample #" + (i + 1).ToString();
            extInfo.s_extvolume = 100;
            extInfo.s_subtone = 0;

            return extInfo;
        }


        public void fillInSamples(ref EPSSEditorData data, ref EPSSSpi spi, ref List<SpiSound> sounds, int sampFreq)
        {
            List<EPSSSpi_soundInfo> soundInfos = new List<EPSSSpi_soundInfo>();
            List<EPSSSpi_extSoundInfo> extSoundInfos = new List<EPSSSpi_extSoundInfo>();
            List<EPSSSpi_sample> samples = new List<EPSSSpi_sample>();

            foreach (SpiSound snd in sounds)
            {
                EPSSSpi_sample sample = getSampleFromSpiSound(ref data, snd, sampFreq);

                EPSSSpi_soundInfo sInfo = getSoundInfoFromSpiSound(sample, snd);

                EPSSSpi_extSoundInfo extSinfo = getExtSoundInfoFromSpiSound(sample, snd);


                samples.Add(sample);
                soundInfos.Add(sInfo);
                extSoundInfos.Add(extSinfo);
            }

            spi.sounds.sounds = soundInfos.ToArray();
            spi.extSounds.sounds = extSoundInfos.ToArray();
            spi.samples.samples = samples.ToArray();
        }


        public void fillInOffsets(ref EPSSSpi spi)
        {
            // Fill in the offsets
            spi.ext.i_sx_offset = (UInt16)(spi.main.length() + spi.ext.length() + spi.split.length() + spi.sounds.length());

            spi.main.i_sdata_offset = (UInt16)(spi.main.length() + spi.ext.length() + spi.split.length() + spi.sounds.length() + spi.extSounds.length());

            uint sampleStartOffset = spi.main.i_sdata_offset;
            foreach (EPSSSpi_soundInfo info in spi.sounds.sounds)
            {
                uint smpLen = info.s_sampend;

                info.s_sampstart += sampleStartOffset;
                info.s_sampend += sampleStartOffset;
                info.s_loopstart += sampleStartOffset;

                sampleStartOffset += smpLen;
            }

            spi.main.i_filelen = (UInt32)(spi.main.length() + spi.ext.length() + spi.split.length() + spi.sounds.length() + spi.extSounds.length() + spi.samples.length());
        }


        public EPSSSpi create(ref EPSSEditorData data, List<SpiSound> sounds, string name, string info, int sampFreq)
        {
            EPSSSpi spi = new EPSSSpi();

            initialize(ref spi);

            noOfSounds = sounds.Count;
            fillInMain(ref spi);

            fillInExt(ref spi, name, info);

            fillInSplit(ref spi, ref sounds, data.omni);

            fillInSamples(ref data, ref spi, ref sounds, sampFreq);

            fillInOffsets(ref spi);

            return spi;
        }


        public long length(ref EPSSEditorData data)
        {
            List<SpiSound> sounds = data.spiSounds;
            int noOfSounds = sounds.Count;

            long mainLength = 16;
            long spiExtLength = 64;
            long spiSplitLength = 256 * noOfMidiCh;
            long spiSoundsLength = 16 * noOfSounds;
            long spiExtSoundsLength = 64 * noOfSounds;
            long spiSamplesLength = 0;

            foreach (SpiSound snd in sounds) spiSamplesLength += snd.preLength(ref data);

            return mainLength + spiExtLength + spiSplitLength + spiSoundsLength + spiExtSoundsLength + spiSamplesLength;
         
        }



        public EPSSSpi createTestSpi()
        {
            noOfMidiCh = 16;
            noOfSounds = 16;


            EPSSSpi spi = new EPSSSpi();

            initialize(ref spi);
            /*            spi.main= new EPSSSpi_main();
                        spi.ext = new EPSSSpi_extended();
                        spi.split = new EPSSSpi_splitInfo();
                        spi.sounds = new EPSSSpi_sounds();
                        spi.extSounds = new EPSSSpi_extSounds();
                        spi.samples = new EPSSSpi_samples(); */


            /* Fill in main */
            fillInMain(ref spi);
            /*            spi.main.i_no_of_MIDIch.no_of_MIDICh = (byte)noOfMidiCh;

                        spi.main.i_no_of_sounds.no_of_sounds = (byte)(noOfSounds);

                        spi.main.i_patch_offset = (UInt16)(spi.main.length() + spi.ext.length());

                        spi.main.i_fileID.versionLow = 1;
                        spi.main.i_fileID.versionHigh = 1; */


            /* Fill in ext */
            fillInExt(ref spi, "SpiPc", "Spi created by EPSSSpi for Windows");
            /* //calculate spi.ext.i_sx_offset
             DateTime dr = DateTime.Now;
            spi.ext.i_crtime = dr.ToDosDateTime();
            spi.ext.i_crdate = dr.ToDosDateTime();
            spi.ext.i_chtime = dr.ToDosDateTime();
            spi.ext.i_chdate = dr.ToDosDateTime();
            spi.ext.i_pname = "SpiPc";
            spi.ext.i_mainlen = (UInt16)(spi.main.length() + spi.ext.length());
            spi.ext.i_splitlen = 2;
            spi.ext.i_xsinflen = 64;
            spi.ext.i_sinflen = 16;
            spi.ext.i_patchinfo = "Spi created by EPSSSpi for Windows"; */

            /* Fill in split */

  


            List<EPSSSpi_midiChannelSplit> channels = new List<EPSSSpi_midiChannelSplit>();
            for (int ch = 0; ch < noOfMidiCh; ch++)
            {
                EPSSSpi_midiChannelSplit channel = new EPSSSpi_midiChannelSplit();
                channel.data = new EPSSSpi_soundAndPitch[128];
                byte note = 60;
                for (int i = 0; i < 128; i++)
                {
                    EPSSSpi_soundAndPitch sp = new EPSSSpi_soundAndPitch();

                    if (i >= 36 && i <= 84)
                    {
                        sp.sound = (byte)ch;
                        sp.pitch = note++;
                        sp.noSound = 0;
                    }
                    else
                    {
                        sp.sound = 0;
                        sp.pitch = 0;
                        sp.noSound = 1;
                    }
                    channel.data[i] = sp;
         

                }
                channels.Add(channel);
               
            }
            spi.split.channels = channels.ToArray();

            spi.main.i_sinfo_offset = (UInt16)(spi.main.length() + spi.ext.length() + spi.split.length());


 
            /* Fill in ext soundinfo */

            /* Fill in info */

            /* Fill in samples */

            List<EPSSSpi_soundInfo> soundInfos = new List<EPSSSpi_soundInfo>();
            List<EPSSSpi_extSoundInfo> extSoundInfos = new List<EPSSSpi_extSoundInfo>();
            List<EPSSSpi_sample> samples = new List<EPSSSpi_sample>();

//            int sz = 10240;

            int tot = 0;
            for (int i = 0; i < noOfSounds; i++)
            {
                EPSSSpi_sample smp = new EPSSSpi_sample();
                // convert sample to internal format smp.data 
                if (i == 0)
                {
                    smp.loadSpl(new Uri(@"D:\OneDrive\Atari\Emulators etc\HDimage\Syquest\LJUDMODF\F\SQR_WAV.SPL"));
                }
                else
                {

                    int sz = 5000;
                    smp.data = new byte[sz];
                    byte b = 128;
                    int v = 1;
                    int startX = 1;
                    int x = startX;
                    for (int j = 0; j < sz; j++)
                    {
                        smp.data[j] = b;
                        if (x-- < 0)
                        {


                            if (b + v > 255)
                            {
                                v = -v;
                            }
                            else if (b + v < 0)
                            {
                                v = -v;
                            }
                            b = (byte)(b + v);
                            x = startX;
                        }
                    }
                }
                samples.Add(smp);

                EPSSSpi_soundInfo info = new EPSSSpi_soundInfo();
                info.s_sampstart = 0;
                info.s_sampend = (uint)smp.data.Length;
                info.s_loopstart = 0;
                info.s_loopmode.toneoffset = 0;
                info.s_loopmode.loopmode = 1;
                info.s_loopmode.vvfe = 0;
                info.s_gr_freq.drum = 0;
                info.s_gr_freq.velocity = 0;
                info.s_gr_freq.soundType = 0;
                info.s_gr_freq.mode = 1;
                info.s_gr_freq.aftertouch = 0;
                info.s_gr_freq.stereoType = 0;
                info.s_gr_freq.stereoPan = 0;
                info.s_gr_freq.orgFreq = 3;

                EPSSSpi_extSoundInfo extInfo = new EPSSSpi_extSoundInfo();
                extInfo.s_sampname = "TstSam" + (i+1).ToString();
                extInfo.s_extname = "Sample #" + (i+1).ToString();
                extInfo.s_extvolume = 100;
                extInfo.s_subtone = 0;


                soundInfos.Add(info);
                extSoundInfos.Add(extInfo);
                tot++;
               
            }
            spi.sounds.sounds = soundInfos.ToArray();
            spi.extSounds.sounds = extSoundInfos.ToArray();
            spi.samples.samples = samples.ToArray();


            // Fill in the offsets
            spi.ext.i_sx_offset = (UInt16)(spi.main.length() + spi.ext.length() + spi.split.length() + spi.sounds.length());

            spi.main.i_sdata_offset = (UInt16)(spi.main.length() + spi.ext.length() + spi.split.length() + spi.sounds.length() + spi.extSounds.length());

            uint sampleStartOffset = spi.main.i_sdata_offset;
            foreach (EPSSSpi_soundInfo info in spi.sounds.sounds)
            {
                uint smpLen = info.s_sampend;

                info.s_sampstart += sampleStartOffset;
                info.s_sampend += sampleStartOffset;
                info.s_loopstart += sampleStartOffset;

                sampleStartOffset += smpLen;
            }

            spi.main.i_filelen = (UInt32)(spi.main.length() + spi.ext.length() + spi.split.length() + spi.sounds.length() + spi.extSounds.length() + spi.samples.length());

            return spi;
        }


    }


    
}
