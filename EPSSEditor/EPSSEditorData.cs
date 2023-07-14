using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using System.IO;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using System.ComponentModel.Design;
using System.Runtime.InteropServices;
using static System.Net.Mime.MediaTypeNames;
using System.Drawing;


namespace EPSSEditor
{


    public class EPSSEditorData
    {
        public DrumSettingsHelper drumMappings;

        public List<Sound> sounds;


        public List<SpiSound> spiSounds;

        public string soundFileName;
        //public string spiFileName;

        public string spiName;
        public string spiDescription;

        public int previewSelected;

        public bool omni;

        private string _fileNameForListenConvertedSound = null;

        private Dictionary<int, SpiSound[]> _findSpiSoundArray;

        public EPSSEditorData() { }


        public void initialize(string drumSettingsFileName)
        {
            sounds = new List<Sound>();
            spiSounds = new List<SpiSound>();
            drumMappings = new DrumSettingsHelper();

            drumMappings.initialize(drumSettingsFileName);

            soundFileName = null;
            //spiFileName = null;

            spiName = "EPSSEDIT";
            spiDescription = "Created with EPSSEditor";

            previewSelected = 0;
            _findSpiSoundArray = null;
        }


        public bool VerifyFiles(string remapDir, ref string sampleNotFound)
        {
            foreach(Sound sound in sounds)
            {
                if (!File.Exists(sound.path))
                {
                    if (!String.IsNullOrEmpty(remapDir))
                    {
                        sound.path = remapDir + "\\" + Path.GetFileName(sound.path);
                        if (!File.Exists(sound.path))
                        {
                            sampleNotFound = Path.GetFileName(sound.path);
                            return false;
                        }
                    }
                    else
                    {
                        sampleNotFound = Path.GetFileName(sound.path);
                        return false;
                    }
                }
            }
            sampleNotFound = "";
            return true;
        }


        public bool LoadSpiFile(ref EPSSSpi spi, string soundDir, ref string errorMessage)
        {
            _findSpiSoundArray = null;
            bool result = true;
            for (int i = 0; i < spi.main.i_no_of_sounds.no_of_sounds; i++)
            {
                string safe = spi.extSounds.sounds[i].s_sampname.Trim();
                safe = Utility.ReplaceIllegalCharacters(safe);
                if (String.IsNullOrEmpty(safe)) safe = "NULL";

                string outPath = soundDir + '\\' + safe + ".wav";
                int n = 1;
                while (File.Exists(outPath))
                {
                    outPath = soundDir + '\\' + safe + " (Copy " + n.ToString() + ").wav";
                    n++;
                }

                Sound snd = new Sound(spi.samples.samples[i].data, outPath);
                sounds.Add(snd);
            }


            spiName = spi.ext.i_pname.Trim();
            spiDescription = spi.ext.i_patchinfo.Trim();

            SfzConverter c = new SfzConverter();
            Dictionary<int, List<SfzSplitInfo>> soundNoToSplit = c.Convert(ref spi);

            for (int midich = 0; midich < spi.main.i_no_of_MIDIch.no_of_MIDICh; midich++)
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
                            /*int noteStart = info.NoteStart;
                            int noteEnd = info.NoteEnd;
                            if (noteEnd < 0) noteEnd = noteStart;
                            int center = noteStart + 84 - info.Low;
                            int transpose = info.Transpose;
                            */

                            /*
                            writer.WriteLine("<region> sample={0} lokey={1} hikey={2} pitch_keycenter={3}",
                                Path.GetFileName(sounds[sound].path),
                                noteStart,
                                noteEnd,
                                noteStart + 84 - info.Low);
                            */
                            Sound s = sounds[sound];

                            SpiSound spiSnd = new SpiSound(s, info);
                            spiSounds.Add(spiSnd);

                            //AddSfzSound(ref s, midich, (byte)noteStart, (byte)noteEnd, (byte)center, (sbyte)transpose);
                        }
                    }
                }
            }
            return result;
        }


        public Dictionary<int, List<SfzSplitInfo>> ConvertToSfzSplitInfoForSfzExport()
        {
            Dictionary<int, List<SfzSplitInfo>> soundNoToSplit = new Dictionary<int, List<SfzSplitInfo>>();

            foreach (var s in spiSounds)
            {
                int sound = getSoundNumberFromGuid(s.soundId);
                List<SfzSplitInfo> splits;

                if (soundNoToSplit.ContainsKey(sound))
                {
                    splits = soundNoToSplit[sound];

                } else
                {
                    splits = new List<SfzSplitInfo>();
                    soundNoToSplit.Add(sound, splits);
                }

                SfzSplitInfo split = new SfzSplitInfo(sound, s);
                splits.Add(split);

                soundNoToSplit[sound] = splits;
            }

            return soundNoToSplit;

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
                if (snd.sampleDataLength == 0)
                {
                    snd.sampleDataLength = new System.IO.FileInfo(snd.path).Length;
                }
            }
        }


        public Sound getSoundFromSoundId(Guid id)
        {
            foreach (Sound snd in sounds)
            {
                if (snd.id() == id)
                {
                    return snd;
                }
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

        public List<SpiSound> GetSpiSoundsForMidiChannel(int midiChannel, bool omni)
        {
            List<SpiSound> channelSounds = new List<SpiSound>();
            foreach (SpiSound snd in spiSounds)
            {
                if (snd.midiChannel == midiChannel || omni)
                {
                    channelSounds.Add(snd);
                }
            }
            return channelSounds;
        }


        public SpiSound SpiSoundAtIndex(int idx)
        {
            return spiSounds[idx];
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


        public SortedDictionary<int, SpiSound> GetUsedSounds()
        {
            SortedDictionary<int, SpiSound> sortedSounds = new SortedDictionary<int, SpiSound>();
            HashSet<int> usedSounds = new HashSet<int>();
            foreach (SpiSound snd in spiSounds)
            {
                int soundNumber = getSoundNumberFromGuid(snd.soundId);
                if (!usedSounds.Contains(soundNumber))
                {
                    sortedSounds.Add(soundNumber, snd);
                    usedSounds.Add(soundNumber);
                }
            }
            return sortedSounds;
        }

        public List<SpiSound> SpiSounds()
        {
            return spiSounds;
        }


        public bool HasSpiSounds()
        {
            return spiSounds.Count > 0;
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


        public bool OverlapWithAnyExisting(int midiChannel, byte lo, byte hi)
        {
            bool overlapping = false;
            foreach (var spiSnd in spiSounds)
            {
                if (spiSnd.midiChannel == midiChannel)
                {
                    if ((spiSnd.endNote < lo && spiSnd.startNote < lo) ||
                        (spiSnd.startNote > hi && spiSnd.endNote > hi))
                    {
                        overlapping = false;
                    }
                    else
                    {
                        overlapping = true;
                    }
                }
                else
                {
                    overlapping = false;
                }

                if (overlapping) break;
            }
            return overlapping;
        }
        
        // Used when loading sound from sfz file
        public void AddSfzSound(Sound sound, int midiChannel, byte lo, byte hi, byte center, sbyte transpose)
        {
            SpiSound spiSnd = new SpiSound(sound);
            spiSnd.startNote = lo;
            spiSnd.endNote = hi;
            spiSnd.midiNote = (byte)(84 - (center - lo));
            spiSnd.midiChannel = (byte)midiChannel;
            spiSnd.transpose = transpose;
            spiSounds.Add(spiSnd);
        }


        public void RefreshSpiSounds()
        {
            foreach (var spiSnd in spiSounds)
            {
                spiSnd.SetNameFromSound(getSoundFromSoundId(spiSnd.soundId));
            }
        }


        public void ClearSpiSounds()
        {
            spiSounds.Clear();
            _findSpiSoundArray = null;
        }


        public void RemoveSpiSound(int index)
        {
            spiSounds.RemoveAt(index);
            _findSpiSoundArray = null;
        }

        public bool ExportSoundsToDir(string exportDir, ref string errorMessage)
        {
            bool result = true;

            try
            {
                foreach (var sound in sounds)
                {
                    string path = exportDir + "\\" + Path.GetFileName(sound.path);
                    if (File.Exists(path))
                    {
                        long oldSize = new System.IO.FileInfo(sound.path).Length;
                        long newSize = new System.IO.FileInfo(path).Length;
                        if (oldSize != newSize)
                        {
                            string backupPath = path + ".bak";
                            string dirName = Path.GetDirectoryName(path);
                            int i = 1;
                            while (File.Exists(backupPath))
                            {
                                backupPath = dirName + "\\" + Path.GetFileNameWithoutExtension(path) + " (Backup " + i.ToString() + ")" + ".bak";
                                i++;
                            }

                            string tmp = Path.GetTempFileName();
                            File.Copy(sound.path, tmp, true);
                            File.Replace(tmp, path, backupPath);
                        }
                    }
                    else
                    {
                        File.Copy(sound.path, path);
                    }
                }
            }
            catch (Exception ex)
            {
                result = false;
                errorMessage = ex.Message;
            }
            return result;
        }


        public bool AddSoundToSpiSound(ref Sound sound, int midiChannel, byte startNote, byte endNote)
        {
            if (OverlapWithAnyExisting(midiChannel, startNote, endNote))
            {
                return false;
            }
            SpiSound spiSnd = new SpiSound(sound);
            spiSnd.midiChannel = (byte)midiChannel;
            spiSnd.midiNote = 84;
            spiSnd.startNote = startNote;
            spiSnd.endNote = endNote;
            spiSounds.Add(spiSnd);
            _findSpiSoundArray = null;
            return true;
        }


        public void AddSpiSound(SpiSound spiSnd)
        {
            spiSounds.Add(spiSnd);
            _findSpiSoundArray = null;
        }

        public CachedSound cachedSound(SpiSound snd, int newFreq, int note, int vel)
        {
            CachedSound cs = snd.cachedSound();
            if (cs == null) { 
                int newBits = AtariConstants.SampleBits;
                int newChannels = AtariConstants.SampleChannels;

                MemoryStream ms = snd.getWaveStream(this, newFreq, newBits, newChannels);
                if (ms != null)
                {
                    ms.Position = 0;
                    bool loop = snd.loopMode == 2;
                    //Console.WriteLine("Making cached sound: newFreq: {0}, newBits: {1} newChannels: {2}, loopStart: {3}, loopEnd: {4}",
                    // newFreq, newBits, newChannels, snd.loopStart, snd.loopEnd);

                    float pan = 0;
                    if (snd.name().Contains("KAGGE"))
                    {
                        pan = 0;
                    }
                    else if (snd.name().Contains("CONGA_H"))
                    {
                        pan = -0.9f;
                    }
                    else if (snd.name().Contains("CONGA_L"))
                    {
                        pan = 0.9f;
                    }
                    else if (snd.name().Contains("SD_HIP"))
                    {
                        pan = 0.4f;
                    }
                    else if (snd.name().Contains("ACOU_SD"))
                    {
                        pan = -0.4f;
                    }
                    else if (snd.name().Contains("CHH"))
                    {
                        pan = 0.6f;
                    }
                    else if (snd.name().Contains("OHH"))
                    {
                        pan = -0.6f;
                    }
                    else if (snd.name().Contains("EXPOSE_B"))
                    {
                        pan = 0;
                    }
                    else if (snd.name().Contains("ILIKE_IT"))
                    {
                        pan = 0.2f;
                    }
                    else if (snd.name().Contains("808_RIM"))
                    {
                        pan = 0.2f;
                    }
                   else if (snd.name().Contains("PI_ACCHA"))
                    {
                        pan = 0.2f;
                    }
                    else if (snd.name().Contains("PI_ACCHG"))
                    {
                        pan = -0.2f;
                    }
                    else if (snd.name().Contains("EL_GURA"))
                    {
                        pan = 0; ;
                    }



                    cs = snd.cachedSound(ms, loop, (int)snd.loopStart, (int)snd.orgSampleCount, pan);
                }
            }

            int center = snd.CenterNote();
            int relNote = note - center;
            //sb.Append(" pitch_keycenter=");
            //sb.Append(noteStart + 84 - info.Low - info.Transpose);

            // note = 60
            //int relNote = data.EPSSNoteToMidiNote(note); // -24 to +24
            //relNote += snd.transpose;
            //Console.WriteLine(relNote);

            cs.pitch = Math.Pow(2, (double)relNote / 12.0);

            int vvfeMul = 1;
            switch (snd.vvfe)
            {
                case 0x3b: vvfeMul = 0; break;
                case 0x3c: vvfeMul = 2; break;
                case 0x3d: vvfeMul = 4; break;
                case 0x3e: vvfeMul = 8; break;
                case 0x3f: vvfeMul = 16; break;
                case 0: vvfeMul = 32; break;
                case 1: vvfeMul = 64; break;
                case 2: vvfeMul = 128; break;
                default: vvfeMul = 0; break;
            }

            cs.vvfeOffset = (127-vel) * vvfeMul;

            return cs;
        }


        public void InitFinder()
        {
            _findSpiSoundArray = new Dictionary<int, SpiSound[]>();
            for (int i = 0; i < 16; i++)
            {
                _findSpiSoundArray.Add(i, new SpiSound[128]);
            }
            foreach (SpiSound snd in spiSounds)
            {
                SpiSound[] sounds = _findSpiSoundArray[snd.midiChannel - 1];
                for (int note = snd.startNote; note <= (snd.endNote); note++) // 0 - 127
                {
                    sounds[note] = snd;
                }
                _findSpiSoundArray[snd.midiChannel - 1] = sounds;
            }
        }


        public SpiSound FindSpiSound(int midiChannel, int note)
        {
            if (_findSpiSoundArray == null) { InitFinder(); }
            SpiSound[] sounds = _findSpiSoundArray[midiChannel - 1];
            return sounds[note];
        }
    }

}
