using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace EPSSEditor
{


    public class EPSSEditorData
    {
        
        public DrumSettingsHelper drumMappings;


        public List<Sound> sounds;


        public List<SpiSound> spiSounds;



        public string soundFileName;
        public string spiFileName;

      
        public EPSSEditorData() {  }


        public void initialize()
        {
            sounds = new List<Sound>();
            spiSounds = new List<SpiSound>();
            drumMappings = new DrumSettingsHelper();

            drumMappings.initialize();

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
    }
}
