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

      
        public EPSSEditorData() {  }


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


        public void initialize(ref EPSSSpi spi, string soundDir)
        {
            spiSounds = new List<SpiSound>();

            SfzConverter c = new SfzConverter();
            c.Convert(ref spi);


            for (int i =0; i < spi.main.i_no_of_sounds.no_of_sounds; i++)
            {
                string outPath = soundDir + '\\' + spi.extSounds.sounds[i].s_sampname.Trim() + ".wav";
                Sound snd = new Sound(spi.samples.samples[i].data, outPath);




                //SpiSound sound = new SpiSound(spi.sounds.sounds[i], spi.extSounds.sounds[i], spi.samples.samples[i]);
                //spiSounds.Add(sound);
            }

        }




        public string convertSoundFileName() {
            if(_fileNameForListenConvertedSound == null)
                _fileNameForListenConvertedSound = Path.GetTempFileName();
            return _fileNameForListenConvertedSound;
        }


        public void fixOldVersions()
        {
            foreach(Sound snd in sounds)
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
            foreach(Sound snd in sounds)
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
                    occupied[snd.midiNote-1] = true;
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
            return occupied[drumNote-1];
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
            foreach(SpiSound snd in spiSounds)
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
            foreach(var snd in sounds)
            {
                if (snd.path == s.path && snd.loKey == s.loKey && snd.hiKey == s.hiKey && snd.keyCenter == s.keyCenter)
                {
                    result = true;
                    break;
                }
            }
            return result;
        }

    }
}
