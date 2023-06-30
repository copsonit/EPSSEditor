using NAudio.MediaFoundation;
using NAudio.Midi;
using NAudio.Mixer;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace EPSSEditor
{
    public class SfzSplitInfo : IComparable<SfzSplitInfo>
    {
        public int Midich;
        public int Low;
        public int High;
        public int NoteStart;
        public int NoteEnd;
        public int LastPitch;
        public int LastMidich;
        public int SoundNo;

        public SfzSplitInfo() { 
            Midich = -1; Low = -1; High = -1; NoteStart = -1;  NoteEnd = -1; LastPitch = -1; LastMidich = -1; SoundNo = -1;
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

        public Dictionary<int, List<SfzSplitInfo>> Convert(ref EPSSSpi spi, ref Sound[] sounds, string outPath)
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
                        //int sound = sp.sound;

                        List<SfzSplitInfo> infos = dict[sp.sound];
                        SfzSplitInfo current = infos.Last();
                        if (current.Midich == -1) current.Midich = midich;
                        if (current.SoundNo == -1) current.SoundNo = sp.sound;

                        if (current.Low >= 0)
                        {
                            if (current.LastPitch == (sp.pitch - 1) &&
                                current.LastMidich == midich)
                            {
                                current.High = sp.pitch;
                                current.NoteEnd = currentMidiNote;
                                current.LastPitch = sp.pitch;
                                current.LastMidich = midich;
                            }
                            else
                            {
                                infos.Add(new SfzSplitInfo());
                                current = infos.Last();
                                current.Midich = midich;
                                current.SoundNo = sp.sound;

                                current.High = -1;
                                current.Low = sp.pitch;
                                current.LastPitch = sp.pitch;
                                current.LastMidich = midich;
                                current.NoteStart = currentMidiNote;

                            }
                        }
                        else
                        {
                            current.Low = sp.pitch;
                            current.LastPitch = sp.pitch;
                            current.LastMidich = midich;
                            current.NoteStart = currentMidiNote;

                        }

                        dict[sp.sound] = infos;

                    }
                    else
                    {

                    }
                    currentMidiNote++;
                }
            }


            return dict;

            /*

            string fileName = outPath + "\\" + spi.ext.i_pname + ".sfz";

            try
            {
                using (StreamWriter writer = new StreamWriter(fileName))
                {

                    writer.WriteLine("// EPSS SPI to SFZ Conversion.");
                    writer.WriteLine("// Original SPI: {0}", spi.ext.i_pname);

                    for (int midich = 0; midich < (spi.main.i_no_of_MIDIch.no_of_MIDICh - 1); midich++)
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

                                //foreach (var kvp in dict)
                                //{
                                int sound = info.SoundNo;
                                //List<SfzSplitInfo> sfzSplitInfos = kvp.Value;

                                //if (sfzSplitInfos.Count > 0)
                                //
                                //zSplitInfos.Sort();
                                //reach (var info in sfzSplitInfos)
                                //
                                if (info.Midich == midich)
                                {
                                    int noteStart = info.NoteStart;
                                    int noteEnd = info.NoteEnd;
                                    if (noteEnd < 0) noteEnd = noteStart;
                                    writer.WriteLine("<region> sample={0} lokey={1} hikey={2} pitch_keycenter={3}",
                                        Path.GetFileName(sounds[sound].path),
                                        noteStart,
                                        noteEnd,
                                        noteStart + 84 - info.Low);
                                        




                                }
                                //
                                //

                            }
                            writer.WriteLine("");
                        }
                    }
                }
            }
            catch (Exception exp)
            {
                Console.Write(exp.Message);
            }

        }
            */

        }

    }
}
