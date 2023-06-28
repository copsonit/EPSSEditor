using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EPSSEditor
{
    public class SfzSplitInfo
    {
        public int Midich;
        public int Low;
        public int High;
        public int NoteStart;
        public int NoteEnd;

        public SfzSplitInfo() { 
            Midich = -1; Low = -1; High = -1; NoteStart = -1;  NoteEnd = -1;
        }

    }


    public class SfzConverter
    {

        public SfzConverter() { }  

        public void Convert(ref EPSSSpi spi)
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
                byte last = 0;

                byte currentMidiNote = 0;
                foreach (EPSSSpi_soundAndPitch sp in split.data)
                {
                    if (sp.noSound == 0) // We have a sound defined
                    {
                        //int sound = sp.sound;

                        List<SfzSplitInfo> infos = dict[sp.sound];
                        SfzSplitInfo current = infos.Last();
                        if (current.Midich == -1) current.Midich = midich;

                        if (current.Low >= 0)
                        {
                            if (sp.pitch - 1 == last)
                            {
                                current.High = sp.pitch;
                                current.NoteEnd = currentMidiNote;
                                last = sp.pitch;
                            }
                            else
                            {
                                infos.Add(new SfzSplitInfo());
                                current = infos.Last();
                                current.Midich = midich;

                                current.High = -1;
                                current.Low = sp.pitch;
                                last = sp.pitch;
                                current.NoteStart = currentMidiNote;
                            }
                        }
                        else
                        {
                            current.Low = sp.pitch;
                            last = sp.pitch;
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

            for (int midich = 0; midich < (spi.main.i_no_of_MIDIch.no_of_MIDICh - 1); midich++)
            {
                Console.WriteLine("Midi: {0}", midich);
                foreach (var kvp in dict)
                {
                    int sound = kvp.Key;
                    List<SfzSplitInfo> sfzSplitInfos = kvp.Value;

                    if (sfzSplitInfos.Count > 0)
                    {
                        foreach(var info in sfzSplitInfos)
                        {
                            if (info.Midich == midich)
                            {
                                Console.WriteLine("Sound {0}, lokey: {1}, hikey: {2}, pitch_keycenter: {3}", sound, info.NoteStart, info.NoteEnd, info.NoteStart + 84 - info.Low);
                            }
                        }
                    }

                }
            }

        }

        


    }
}
