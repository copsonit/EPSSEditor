using EPSSEditor;
using NAudio.MediaFoundation;
using NAudio.Midi;
using NAudio.Mixer;
using NAudio.Wave;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace EPSSEditor
{
    public class SfzSplitInfo : IComparable<SfzSplitInfo>
    {
        public int Midich; // [0-15], 128 not used, using ProgramChange
        public int ProgramChange; // 0-127, 128: not used
        public int Low;
        public int High;
        public int NoteStart;
        private int _noteEnd;
        public int NoteEnd
        {
            get { return _noteEnd == -1 ? NoteStart : _noteEnd; }
            set { _noteEnd = value; }
        }
        public int LastPitch;
        public int LastMidich;
        public int SoundNo;
        public int Transpose;
        public int Vvfe;
        public UInt16 S_gr_frek;
        public int Loopmode;
        public UInt32 Start;
        public UInt32 End;
        public UInt32 LoopStart;
        public UInt32 LoopEnd;

        public UInt16 ExtVolume;
        public UInt16 SubTone;

        public SfzSplitInfo() {
            Midich = -1; Low = -1; High = -1; NoteStart = -1; NoteEnd = -1; LastPitch = -1; LastMidich = -1; SoundNo = -1; Transpose = -1; Vvfe = -1;  Loopmode = -1;
            S_gr_frek = 0;
            ExtVolume = 0;
            SubTone = 0;
            ProgramChange = 128;
        }


        public SfzSplitInfo(int soundNo, SpiSound sound) // Used when exporting sound to SFZ Format.
        { 
            Midich = sound.midiChannel -1;

            NoteStart = sound.startNote;
            NoteEnd = sound.endNote;

            Low = sound.midiNote;
            High = sound.midiNote + (NoteEnd - NoteStart);

            LastPitch = LastMidich = -1;
            SoundNo = soundNo;
            Transpose = sound.transpose;
            Vvfe = sound.vvfe;

            Loopmode = sound.loopMode;
            Start = sound.start;
            End = sound.end;
            LoopStart = sound.loopStart;
            LoopEnd = sound.loopEnd;
        }

        public void Update(int sound, int midich, int loopmode, int toneOffset, int vvfe, UInt16 s_gr_frek, UInt32 start, UInt32 end, UInt32 loopStart, UInt32 loopEnd, UInt16 extVolume, UInt16 subTone)
        {
            if (Midich == -1)
            {
                SoundNo = sound;
                Midich = midich;
                Loopmode = loopmode;
                Transpose = toneOffset;
                Vvfe = vvfe;
                S_gr_frek = s_gr_frek;
                Start = start;
                End = end;
                LoopStart = loopStart;
                LoopEnd = loopEnd;
                ExtVolume = extVolume;
                SubTone = subTone;
            }
        }

        public void UpdateHigh(int midich, int pitch, int current)
        {
            Midich = midich;

            High = pitch;
            NoteEnd = current;

            LastPitch = pitch;
            LastMidich = midich;
        }


        public void UpdateLow(int midich, int pitch, int current)
        {
            Midich = midich;

            Low = pitch;
            NoteStart = current;

            LastPitch = pitch;
            LastMidich = midich;

        }

        public int CompareTo(SfzSplitInfo other)
        {
            if (other == null) return 1;
            if (NoteStart < other.NoteStart) return -1;
            else if (NoteStart == other.NoteStart) return 0;
            return 1;
        }

    }


    public class SfzConverter
    {

        public SfzConverter() { }

        public Dictionary<int, List<SfzSplitInfo>> Convert(EPSSSpi spi)
        {

            Dictionary<int, List<SfzSplitInfo>> dict = new Dictionary<int, List<SfzSplitInfo>>();
            for (int i = 0; i < spi.main.i_no_of_sounds.No_of_sounds; i++)
            {
                List<SfzSplitInfo> list = new List<SfzSplitInfo>();
                SfzSplitInfo si = new SfzSplitInfo();
                list.Add(si);
                dict.Add(i, list);
            }

            for (int midich = 0; midich < spi.main.i_no_of_MIDIch.No_of_MIDICh; midich++)
            {
                EPSSSpi_midiChannelSplit split = spi.split.channels[midich];


                byte currentMidiNote = 0;
                foreach (EPSSSpi_soundAndPitch sp in split.data)
                {
                    if (sp.NoSound == 0) // We have a sound defined
                    {
                        int sound = sp.Sound;
                        int pitch = EPSSNoteToMidiNote(sp.Pitch); // converted pitch, ALWAYS 60-108!
                        sbyte toneOffset = spi.sounds.sounds[sound].s_loopmode.Toneoffset;
                        byte vvfe = spi.sounds.sounds[sound].s_loopmode.Vvfe;
                        byte lm = spi.sounds.sounds[sound].s_loopmode.Loopmode;
                        UInt16 s_gr_frek = spi.sounds.sounds[sound].s_gr_freq.data;

                        UInt32 startInSpi = spi.sounds.sounds[sound].s_sampstart;
                        UInt32 start = 0;
                        UInt32 end = spi.sounds.sounds[sound].s_sampend - startInSpi;
                        UInt32 loopStart = spi.sounds.sounds[sound].s_loopstart - startInSpi;
                        UInt32 loopEnd = spi.sounds.sounds[sound].s_sampend - startInSpi; // EPSS Always loops from end of sample

                        UInt16 extVolume = spi.extSounds.sounds[sound].s_extvolume;
                        UInt16 subTone = spi.extSounds.sounds[sound].s_subtone;

                        List<SfzSplitInfo> infos = dict[sound];
                        SfzSplitInfo current = infos.Last();
                        current.Update(sound, midich, lm, toneOffset, vvfe, s_gr_frek, start, end, loopStart, loopEnd, extVolume, subTone);

                        if (current.Low >= 0)
                        {
                            if (current.LastPitch == (pitch - 1) &&
                                current.LastMidich == midich)
                            {
                                current.UpdateHigh(midich, pitch, currentMidiNote);

                            }
                            else
                            {
                                infos.Add(new SfzSplitInfo());
                                current = infos.Last();
                                current.Update(sound, midich, lm, toneOffset, vvfe, s_gr_frek, start, end, loopStart, loopEnd, extVolume, subTone);

                                current.UpdateLow(midich, pitch, currentMidiNote);
                            }
                        }
                        else
                        {
                            current.UpdateLow(midich, pitch, currentMidiNote);

                        }

                        dict[sound] = infos;

                    }
                    else
                    {

                    }
                    currentMidiNote++;
                }
            }
            return dict;
        }


        public int EPSSNoteToMidiNote(int note)
        {
            // -24 to 24 

            int[] epssMap = new int[128];
            int idx = 0;
            for (int i = 0; i <= 24; i++)
            {
                epssMap[idx++] = i + 84;
            }
            for (int i = 25; i <= 47; i++)
            {
                epssMap[idx++] = i - 24 + 84;
            }
            for (int i = 48; i <= 59; i++)
            {
                epssMap[idx++] = 84;
            }
            for (int i = 60; i <= 108; i++)
            {
                epssMap[idx++] = i;
            }
            for (int i = 109; i < 128; i++)
            {
                epssMap[idx++] = 84;
            }

            return epssMap[note];
        }

        public bool SaveOneSFZ(int midiChannel, string patchName, string fileName, string sampleSubDir, List<SfzSplitInfo> splitsForChannel, List<Sound> sounds, out string errorMessage)
        {
            bool result = true;
            errorMessage = "";

            try
            {
                using (StreamWriter writer = new StreamWriter(fileName))
                {

                    writer.WriteLine("// EPSS SPI to SFZ Conversion.");
                    writer.WriteLine("// Original SPI: {0}", patchName);

                    writer.WriteLine("<master> loprog={0} hiprog={0} master_label=Midi channel {0}", midiChannel);
                    writer.WriteLine("<group>");
                    writer.WriteLine("volume=0");
                    writer.WriteLine("ampeg_attack=0.001");
                    writer.WriteLine("ampeg_decay=0.1"); // Does not matter as we use 100 as sustain
                    writer.WriteLine("ampeg_sustain=100");
                    writer.WriteLine("ampeg_release=0.1");

                    foreach (var info in splitsForChannel)
                    {
                        int sound = info.SoundNo;

                        int noteStart = info.NoteStart;
                        int noteEnd = info.NoteEnd;
                        if (noteEnd < 0) noteEnd = noteStart;
                        StringBuilder sb = new StringBuilder();
                        sb.Append("<region>");

                        sb.Append(" sample=");
                        sb.Append(sampleSubDir + "\\" + Path.GetFileName(sounds[sound].path));

                        if (info.Loopmode == 1)
                        {
                            sb.Append(" loop_mode=no_loop");
                        }
                        else if (info.Loopmode > 1)
                        {
                            sb.Append(" loop_mode=loop_continuous");
                            sb.Append(" offset=");
                            sb.Append(info.Start);

                            sb.Append(" loop_end=");
                            sb.Append(info.End);

                            sb.Append(" loop_start=");
                            sb.Append(info.LoopStart);
                        }

                        sb.Append(" lokey=");
                        sb.Append(noteStart);

                        sb.Append(" hikey=");
                        sb.Append(noteEnd);

                        sb.Append(" pitch_keycenter=");
                        sb.Append(info.Low - info.Transpose);

                        writer.WriteLine(sb.ToString());
                    }
                }
            }
            catch (Exception exp)
            {
                Console.Write(exp.Message);
                errorMessage = exp.Message;
                result = false;
            }
            return result;
        }


        public bool SaveSFZ(Dictionary<int, List<SfzSplitInfo>> dict, List<Sound> sounds, string outPath, string sampleSubDir, string patchName, out string errorMessage)
        {
            bool result = true;
            errorMessage = "";

            for (int midich = 0; midich < 15; midich++)
            {
                List<SfzSplitInfo> splitsForChannel = new List<SfzSplitInfo>();
                foreach (var kvp in dict)
                {
                    List<SfzSplitInfo> sfzSplitInfos = kvp.Value;
                    foreach (var split in sfzSplitInfos)
                    {
                        if (split.Midich == midich)
                        {
                            splitsForChannel.Add(split);
                        }
                    }
                }

                if (splitsForChannel.Count > 0)
                {
                    splitsForChannel.Sort();
                    int midiChannel = midich + 1;
                    string fileName = outPath + "\\" + "Channel " + midiChannel.ToString() + ".sfz";
                    bool saveOneSfzResult = SaveOneSFZ(midiChannel, patchName, fileName, sampleSubDir, splitsForChannel, sounds, out errorMessage);
                    if (!saveOneSfzResult) break;
                }
            }

            return result;
        }

        public static string LoadSfzSound(EPSSEditorData data, int midiChannel, int programChange, string filePath, List<string> filesAdded)
        {
            
            //List<string> soundsAdded = new List<string>();
            
            Dictionary<string, Sound> sounds = new Dictionary<string, Sound>();
            foreach (Sound s in data.sounds)
            {
                sounds.Add(s.path, s);
            }
            

            string anyFile = "";
            ParseSfz p = new ParseSfz();
            List<SfzBase> bases = p.parse(filePath);
            string basePath = Path.GetDirectoryName(filePath);
            bool skipFirstGroup = true;
            bool abortLoad = false;
            foreach (SfzBase bas in bases)
            {
                if (abortLoad) break;
                if (bas is SfzGenericSection gSection)
                {
                    if (gSection.header.Contains("group"))
                    {
                        if (!skipFirstGroup)
                        {
                            midiChannel++;
                            if (midiChannel > 16)
                            {
                                System.Windows.Forms.MessageBox.Show("Midi channel over 16. Will skip rest of sfz file.");
                                break;
                            }
                        }
                        skipFirstGroup = false;
                    }
                }

                if (bas is SfzRegionSection tBase)
                {
                    string fp = tBase.FilePath(basePath);

                    Sound s;
                    if (sounds.ContainsKey(fp))
                    {
                        s = sounds[fp];
                    }
                    else
                    {
                        s = new Sound(fp);
                        if (!String.IsNullOrEmpty(s.path))
                        {
                            s.description = Path.GetFileNameWithoutExtension(fp);
                            data.sounds.Add(s);
                            sounds.Add(fp, s);
                            filesAdded.Add(fp);
                        } else
                        {
                            MessageBox.Show("Unsupported format for samples. Only supports WAV and OGG.");
                            abortLoad = true;
                            s = null;
                        }
                    }


                    string kcS = tBase.GetValue("pitch_keycenter");
                    byte kcByte = 60; // default
                    if (!String.IsNullOrEmpty(kcS))
                    {

                        //                    string kcS = tBase.variables[""];
                        if (!Utility.TryToByte(kcS, out kcByte))
                        {
                            int v = Utility.ParseNoteToInt(kcS, 2);
                            if (v < 0 || v > 127)
                            {
                                kcByte = 128;
                            }
                            else
                            {
                                kcByte = (byte)v;
                            }
                        }
                    }

                    //Sound s = new Sound(fp);
                    //s.description = baseName + Path.GetFileNameWithoutExtension(fp);
                    string loKeyS = tBase.GetValue("lokey");
                    byte loByte = 0;
                    if (!String.IsNullOrEmpty(loKeyS))
                    {

                        if (!Utility.TryToByte(loKeyS, out loByte))
                        {
                            int v = Utility.ParseNoteToInt(loKeyS, 2);
                            if (v < 0 || v > 127)
                            {
                                loByte = 128;
                            }
                            else
                            {
                                loByte = (byte)v;
                            }
                        }
                    }
                    else
                    {
                        loByte = (byte)Math.Max(0, (int)kcByte - 24);
                    }
                    //s.loKey = loByte;



                    string hiKeyS = tBase.GetValue("hikey");
                    byte hiByte = 0;
                    if (!String.IsNullOrEmpty(hiKeyS))
                    {

                        if (!Utility.TryToByte(hiKeyS, out hiByte))
                        {
                            int v = Utility.ParseNoteToInt(hiKeyS, 0);
                            if (v < 0 || v > 127)
                            {
                                hiByte = 128;
                            }
                            else
                            {
                                hiByte = (byte)v;
                            }
                        }
                    }
                    else
                    {
                        hiByte = (byte)Math.Min(127, (int)kcByte + 24);
                    }
                    //s.hiKey = hiByte;



                    //s.keyCenter = kcByte;


                    //if (!data.IdenticalSoundExists(s))
                    //{
                    //data.sounds.Add(s);
                    //}
                    if (s != null)
                    {
                        anyFile = fp;
                        data.AddSfzSound(s, midiChannel, programChange, loByte, hiByte, kcByte, 0);

                    }

                }
            }

            return anyFile;
           
        }
    }
}
