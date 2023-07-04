﻿using NAudio.MediaFoundation;
using NAudio.Midi;
using NAudio.Mixer;
using System;
using System.Collections.Generic;
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
        public int Midich;
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
        public int Loopmode;
        public UInt32 Start;
        public UInt32 End;
        public UInt32 LoopStart;

        public SfzSplitInfo() {
            Midich = -1; Low = -1; High = -1; NoteStart = -1; NoteEnd = -1; LastPitch = -1; LastMidich = -1; SoundNo = -1; Transpose = -1; Loopmode = -1;
        }


        public SfzSplitInfo(int soundNo, SpiSound sound)
        {
            Midich = sound.midiChannel;

            NoteStart = sound.startNote;
            NoteEnd = sound.endNote;

            Low = sound.midiNote;
            High = sound.midiNote + (NoteEnd - NoteStart);

            LastPitch = LastMidich = -1;
            SoundNo = soundNo;
            Transpose = sound.transpose;

            Loopmode = sound.loopMode;
            Start = sound.start;
            End = sound.end;
            LoopStart = sound.loopStart;
        }

        public void Update(int sound, int midich, int loopmode, int toneOffset, UInt32 start, UInt32 end, UInt32 loopStart)
        {
            if (Midich == -1)
            {
                SoundNo = sound;
                Midich = midich;
                Loopmode = loopmode;
                Transpose = toneOffset;
                Start = start;
                End = end;
                LoopStart = loopStart;
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

        public Dictionary<int, List<SfzSplitInfo>> Convert(ref EPSSSpi spi/*, ref Sound[] sounds, string outPath*/)
        {

            Dictionary<int, List<SfzSplitInfo>> dict = new Dictionary<int, List<SfzSplitInfo>>();
            for (int i = 0; i < spi.main.i_no_of_sounds.no_of_sounds; i++)
            {
                List<SfzSplitInfo> list = new List<SfzSplitInfo>();
                SfzSplitInfo si = new SfzSplitInfo();
                list.Add(si);
                dict.Add(i, list);
            }

            for (int midich = 0; midich < (spi.main.i_no_of_MIDIch.no_of_MIDICh - 1); midich++)
            {
                EPSSSpi_midiChannelSplit split = spi.split.channels[midich];


                byte currentMidiNote = 0;
                foreach (EPSSSpi_soundAndPitch sp in split.data)
                {
                    if (sp.noSound == 0) // We have a sound defined
                    {
                        int sound = sp.sound;
                        sbyte toneOffset = spi.sounds.sounds[sound].s_loopmode.toneoffset;
                        byte lm = spi.sounds.sounds[sound].s_loopmode.loopmode;
                        Console.WriteLine("Loopmode:{0}", lm);
                        UInt32 startInSpi = spi.sounds.sounds[sound].s_sampstart;
                        UInt32 start = 0;
                        UInt32 end = spi.sounds.sounds[sound].s_sampend - startInSpi;
                        UInt32 loopStart = spi.sounds.sounds[sound].s_loopstart - startInSpi;
                        
                        List<SfzSplitInfo> infos = dict[sound];
                        SfzSplitInfo current = infos.Last();
                        current.Update(sound, midich, lm, toneOffset, start, end, loopStart);

                        if (current.Low >= 0)
                        {
                            if (current.LastPitch == (sp.pitch - 1) &&
                                current.LastMidich == midich)
                            {
                                current.UpdateHigh(midich, sp.pitch, currentMidiNote);

                            }
                            else
                            {
                                infos.Add(new SfzSplitInfo());
                                current = infos.Last();
                                current.Update(sound, midich, lm, toneOffset, start, end, loopStart);

                                current.UpdateLow(midich, sp.pitch, currentMidiNote);
                                current.Low = sp.pitch;
                            }
                        }
                        else
                        {
                            current.UpdateLow(midich, sp.pitch, currentMidiNote);

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


        public bool SaveSFZ(ref Dictionary<int, List<SfzSplitInfo>> dict, ref List<Sound> sounds, string outPath, string patchName, string filenameWithExt, ref string errorMessage)
        {
            bool result = true;
            string fileName = outPath + "\\" + filenameWithExt;

            try
            {
                using (StreamWriter writer = new StreamWriter(fileName))
                {

                    writer.WriteLine("// EPSS SPI to SFZ Conversion.");
                    writer.WriteLine("// Original SPI: {0}", patchName);

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


                            writer.WriteLine("<master> loprog={0} hiprog={0} master_label=Midi channel {0}", midich);
                            writer.WriteLine("<group>");

                            foreach (var info in splitsForChannel)
                            {


                                int sound = info.SoundNo;

                                if (info.Midich == midich)
                                {
                                    int noteStart = info.NoteStart;
                                    int noteEnd = info.NoteEnd;
                                    if (noteEnd < 0) noteEnd = noteStart;
                                    StringBuilder sb = new StringBuilder();
                                    sb.Append("<region>");
                                    
                                    sb.Append(" sample=");
                                    sb.Append(Path.GetFileName(sounds[sound].path));

                                    if (info.Loopmode == 1)
                                    {
                                        sb.Append(" loop_mode=one_shot");
                                    } else if (info.Loopmode > 1)
                                    {
                                        sb.Append(" loop_mode=loop_continuous");
                                        sb.Append(" offset=");
                                        sb.Append(info.Start);

                                        sb.Append(" loop_end=");
                                        sb.Append(info.End);

                                        sb.Append(" loop_start=");
                                        sb.Append(info.LoopStart);                                    }

                                    sb.Append(" lokey=");
                                    sb.Append(noteStart);

                                    sb.Append(" hikey=");
                                    sb.Append(noteEnd);

                                    sb.Append(" pitch_keycenter=");
                                    sb.Append(noteStart + 84 - info.Low - info.Transpose);

                                    writer.WriteLine(sb.ToString());

                                    /*
                                    writer.WriteLine("<region> sample={0} lokey={1} hikey={2} pitch_keycenter={3}",
                                       ,
                                        noteStart,
                                        noteEnd,
                                        noteStart + 84 - info.Low - info.Transpose);
                                    Console.WriteLine(info.Transpose);
                                    */





                                }

                            }
                            writer.WriteLine("");
                        }
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
    }
}
