using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using System.IO;

namespace EPSSEditor
{


    public class EPSSEditorData
    {

        public DrumSettingsHelper drumMappings;


        public List<Sound> sounds;


        public List<SpiSound> spiSounds;



        public string soundFileName;
        public string spiFileName;

        public string spiName;
        public string spiDescription;

        public int previewSelected;

        public bool omni;

        private string _fileNameForListenConvertedSound = null;


        public EPSSEditorData() { }


        public void initialize(string drumSettingsFileName)
        {
            sounds = new List<Sound>();
            spiSounds = new List<SpiSound>();
            drumMappings = new DrumSettingsHelper();

            drumMappings.initialize(drumSettingsFileName);

            soundFileName = null;
            spiFileName = null;

            spiName = "EPSSEDIT";
            spiDescription = "Created with EPSSEditor";

            previewSelected = 0;

        }


        public void LoadSpiFile(ref EPSSSpi spi, string soundDir)
        {


            //List<Sound> spiSoundsList = new List<Sound>();
            for (int i = 0; i < spi.main.i_no_of_sounds.no_of_sounds; i++)
            {
                string outPath = soundDir + '\\' + spi.extSounds.sounds[i].s_sampname.Trim() + ".wav";
                Sound snd = new Sound(spi.samples.samples[i].data, outPath);
                sounds.Add(snd);
                //SpiSound sound = new SpiSound(spi.sounds.sounds[i], spi.extSounds.sounds[i], spi.samples.samples[i]);
                //spiSounds.Add(sound);
            }
            Sound[] spiSounds = sounds.ToArray();

            SfzConverter c = new SfzConverter();
            Dictionary<int, List<SfzSplitInfo>> soundNoToSplit = c.Convert(ref spi, ref spiSounds, soundDir);

            for (int midich = 0; midich < (spi.main.i_no_of_MIDIch.no_of_MIDICh - 1); midich++)
            {
                List<SfzSplitInfo> splitsForChannel = new List<SfzSplitInfo>();
                foreach (var kvp in soundNoToSplit)
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


                    //writer.WriteLine("<master> loprog={0} hiprog={0} master_label=Midi channel {0}", midich);
                    //writer.WriteLine("<group>");

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
                            int center = noteStart + 84 - info.Low;

                            /*
                            writer.WriteLine("<region> sample={0} lokey={1} hikey={2} pitch_keycenter={3}",
                                Path.GetFileName(sounds[sound].path),
                                noteStart,
                                noteEnd,
                                noteStart + 84 - info.Low);
                            */
                            Sound s = sounds[sound];
                            AddSfzSound(ref s, midich, (byte)noteStart, (byte)noteEnd, (byte)center);
                        }
                    }
                }
            }

        }




        public string convertSoundFileName()
        {
            if (_fileNameForListenConvertedSound == null)
                _fileNameForListenConvertedSound = Path.GetTempFileName();
            return _fileNameForListenConvertedSound;
        }


        public void fixOldVersions()
        {
            foreach (Sound snd in sounds)
            {
                if (snd.parameters().normalize == null)
                {
                    snd.parameters().normalize = new ConversionNormalize();
                }
            }
        }


        public Sound getSoundFromSoundId(Guid id)
        {
            foreach (Sound snd in sounds)
            {
                if (snd.id() == id) return snd;
            }
            return null;
        }


        public int getSoundNumberFromGuid(Guid guid)
        {
            int i = 0;
            foreach (Sound snd in sounds)
            {
                if (snd.id() == guid)
                {
                    return i;
                }
                i++;
            }
            return -1;
        }


        public List<SpiSound> getSpiSoundsFromSound(ref Sound sound)
        {
            List<SpiSound> sounds = new List<SpiSound>();
            if (sound != null)
            {
                foreach (SpiSound spiSnd in spiSounds)
                {
                    if (sound.id() == spiSnd.soundId)
                    {
                        sounds.Add(spiSnd);
                    }
                }
            }
            return sounds;
        }


        private bool[] getOccupiedMidiChannels()
        {
            bool[] occupied = new bool[16];
            for (int i = 0; i < 16; i++) occupied[i] = false;
            occupied[9] = true; // mark drum channel as used.
            foreach (SpiSound snd in spiSounds)
            {
                occupied[snd.midiChannel - 1] = true;
            }
            return occupied;
        }


        private bool[] getOccupiedChannel10()
        {
            bool[] occupied = new bool[128];

            foreach (SpiSound snd in spiSounds)
            {
                if (snd.midiChannel == 10)
                {
                    occupied[snd.midiNote - 1] = true;
                }
            }
            return occupied;
        }


        public bool isMidiChannelOccupied(int midiChannel)
        {
            bool[] occupied = getOccupiedMidiChannels();
            return occupied[midiChannel - 1];
        }


        public bool isDrumSoundOccupied(int drumNote)
        {
            bool[] occupied = getOccupiedChannel10();
            return occupied[drumNote - 1];
        }

        public int getNextFreeMidiChannel()
        {
            bool[] occupied = getOccupiedMidiChannels();

            for (int i = 0; i < 16; i++)
            {
                if (!occupied[i]) return i + 1;
            }
            return 0;
        }


        public List<SpiSound> getSortedSpiSounds()
        {
            int i = 0;
            foreach (SpiSound snd in spiSounds)
            {
                snd.soundNumber = i++;
            }
            return spiSounds;
        }


        public bool removeSpiSound(byte midiChannel, byte midiNote)
        {
            bool result = false;

            int j = spiSounds.Count();
            for (int i = 0; i < j; i++)
            {
                SpiSound snd = spiSounds.ElementAt(i);
                if (snd.midiNote == midiNote && snd.midiChannel == midiChannel)
                {
                    spiSounds.RemoveAt(i);
                    result = true;
                    j--;

                }
            }
            return result;
        }


        public bool IdenticalSoundExists(Sound s)
        {
            bool result = false;
            foreach (var snd in sounds)
            {
                if (snd.path == s.path && snd.loKey == s.loKey && snd.hiKey == s.hiKey && snd.keyCenter == s.keyCenter)
                {
                    result = true;
                    break;
                }
            }
            return result;
        }


        public void AddSfzSound(ref Sound sound, int midiChannel, byte lo, byte hi, byte center)
        {
            SpiSound spiSnd = new SpiSound(ref sound);
            spiSnd.startNote = lo;
            spiSnd.endNote = hi;
            spiSnd.midiNote = (byte)(84 - (center - lo));
            spiSnd.midiChannel = (byte)midiChannel;
            spiSounds.Add(spiSnd);
        }
    }
}
