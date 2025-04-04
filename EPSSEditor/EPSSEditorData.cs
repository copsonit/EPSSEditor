﻿using System;
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
using System.Data.SqlTypes;
using System.Text.RegularExpressions;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices.ComTypes;
using System.Windows.Forms;

namespace EPSSEditor
{


    public class EPSSEditorData : ICloneable
    {
            // All public fields are saved in ProjectSettings.
        public DrumSettingsHelper drumMappings;
        public List<Sound> sounds;
        public List<SpiSound> spiSounds;
        public string soundFileName;
        public string spiName;
        public string spiDescription;
        public int previewSelected;
        public bool omni;
        public int spiVersion;
        public DateTime created;
        public DateTime changed;

        private string _fileNameForListenConvertedSound = null;
        private Dictionary<int, SpiSound[]> _findSpiSoundArray; // midiChannel -> Sound[128]
        private Dictionary<int, SpiSound[]> _programArray;  // program -> Sound[128]

        private bool _dataNeedsSaving;


        public EPSSEditorData() { }


        public virtual object Clone()
        {
            EPSSEditorData o = (EPSSEditorData)this.MemberwiseClone();
            o.Initialize(null);
            o.drumMappings = (DrumSettingsHelper)drumMappings.Clone();
            foreach (var s in sounds) { o.sounds.Add((Sound)s.Clone());  }
            foreach (var s in spiSounds) { o.spiSounds.Add((SpiSound)s.Clone()); }          
            o._findSpiSoundArray = null;
            o._programArray = null;
            o._dataNeedsSaving = this._dataNeedsSaving;

            return o;
        }

        public void Initialize(string drumSettingsFileName)
        {
            sounds = new List<Sound>();
            spiSounds = new List<SpiSound>();
            drumMappings = new DrumSettingsHelper();
            drumMappings.initialize(drumSettingsFileName);
            soundFileName = null;
            spiName = "EPSSEDIT";
            spiDescription = "Created with EPSSEditor";
            previewSelected = 0;
            _findSpiSoundArray = null;
            _programArray = null;
            created = DateTime.Now;
            changed = DateTime.Now;
            _dataNeedsSaving = false;
        }

        public void SetDataNeedsSaving(bool flag)
        {
            _dataNeedsSaving =  flag;
        }

        public bool GetDataNeedsSaving()
        {
            return _dataNeedsSaving;
        }

        public void ClearAll()
        {
            ClearSounds();
            ClearSpiSounds();
        }


        public bool VerifyFiles(string remapDir, out string sampleNotFound)
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


        public bool LoadSpiFile(EPSSSpi spi, string soundDir, out string errorMessage)
        {
            errorMessage = null;
            _findSpiSoundArray = null;
            bool result = true;
            for (int i = 0; i < spi.main.i_no_of_sounds.No_of_sounds; i++)
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

                uint sampleStart = spi.sounds.sounds[i].s_sampstart;
                bool loop = spi.sounds.sounds[i].s_loopmode.Loopmode == 2;
                long loopStart = spi.sounds.sounds[i].s_loopstart - sampleStart;
                long loopEnd = spi.sounds.sounds[i].s_sampend - sampleStart;

                Sound snd = new Sound(spi.samples.samples[i].data, outPath, loop, loopStart, loopEnd);
                AddSound(snd);
            }


            spiName = spi.ext.i_pname.Trim();
            spiDescription = spi.ext.i_patchinfo.Trim();
            created = spi.ext.GetCreationDateTime();
            changed = spi.ext.GetChangeDateTime();

            SfzConverter c = new SfzConverter();
            Dictionary<int, List<SfzSplitInfo>> soundNoToSplit = c.Convert(spi);

            for (int midich = 0; midich < spi.main.i_no_of_MIDIch.No_of_MIDICh; midich++)
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
                int sound = GetSoundNumberFromGuid(s.soundId);
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

             
        public string ConvertSoundFileName()
        {
            if (_fileNameForListenConvertedSound == null)
                _fileNameForListenConvertedSound = Path.ChangeExtension(Path.GetTempFileName(), ".wav");
            return _fileNameForListenConvertedSound;
        }


        public void FixOldVersions()
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
                if (snd.name() == null)
                {
                    snd.description = Path.GetFileNameWithoutExtension(snd.path);
                }
            }
            if (created == DateTime.MinValue)
            {
                created = DateTime.Now;
            }
            if (changed == DateTime.MinValue)
            {
                changed = DateTime.Now;
            }
        }


        public Sound GetSoundFromSoundId(Guid id)
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


        public int GetSoundNumberFromGuid(Guid guid)
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


        public List<SpiSound> GetSpiSoundsFromSound(Sound sound)
        {
            List<SpiSound> usedSounds= new List<SpiSound>();
            if (sound != null)
            {
                foreach (SpiSound spiSnd in spiSounds)
                {
                    if (sound.id() == spiSnd.soundId)
                    {
                        usedSounds.Add(spiSnd);
                    }
                }
            }
            return usedSounds;
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


        private bool[] GetOccupiedMidiChannels()
        {
            bool[] occupied = new bool[16];  // 0-15
            for (int i = 0; i < 16; i++) occupied[i] = false;
            occupied[9] = true; // mark drum channel as used.
            foreach (SpiSound snd in spiSounds)
            {
                if (snd.midiChannel < 16) // midiChannel is 128 for PC mapped sounds.
                {
                    occupied[snd.midiChannel - 1] = true;
                }
            }
            return occupied;
        }


        private bool[] GetOccupiedChannel10()
        {
            bool[] occupied = new bool[128];  // 0-127

            foreach (SpiSound snd in spiSounds)
            {
                if (snd.midiChannel == 10)
                {
                    occupied[snd.midiNote] = true;
                }
            }
            return occupied;
        }


        private bool[] GetOccupiedProgramChange()
        {
            bool[] occupied = new bool[128];  // 0-127

            foreach (SpiSound snd in spiSounds)
            {
                if (snd.programNumber < 128 && snd.midiChannel == 128) { 
                    occupied[snd.programNumber] = true;
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
                int soundNumber = GetSoundNumberFromGuid(snd.soundId);
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


        private bool HasSpiSounds()
        {
            return spiSounds.Count > 0;
        }


        public bool IsValidForSpiExport(out string errorString)
        {
            bool valid = false;
            errorString = "Unknown error.";
            bool hasPc = HasAnyProgramChange();

            if (HasSpiSounds())
            {
                if (spiVersion >= 2 && !hasPc)
                {
                    errorString = "Using G2 requires at least one Program Change definition!";
                }
                else if (spiVersion < 2 && hasPc)
                {
                    errorString = "Using Program Change requires G2 to be checked!";              
                } else if ((spiVersion < 2 && !hasPc) || (spiVersion >= 2 && hasPc))
                {
                    valid = true;
                }
            } else
            {
                errorString = "No SPI sounds defined!";
            }
            return valid;
        }


        public bool IsValidForSpiExport()
        {
            return IsValidForSpiExport(out _);
        }


        public bool IsMidiChannelOccupied(int midiChannel)
        {
            bool[] occupied = GetOccupiedMidiChannels();
            return occupied[midiChannel - 1];
        }


        public bool IsDrumSoundOccupied(int drumNote)
        {
            bool[] occupied = GetOccupiedChannel10();
            return occupied[drumNote];
        }


        public int GetNextFreeMidiChannel()
        {
            bool[] occupied = GetOccupiedMidiChannels();

            for (int i = 0; i < 16; i++)
            {
                if (!occupied[i]) return i + 1;
            }
            return 0;
        }


        public int GetNextFreeProgramChange()
        {
            bool[] occupied = GetOccupiedProgramChange();

            for (int i = 0; i < 128; i++)
            {
                if (!occupied[i]) return i;
            }
            return 0;
        }


        public bool IsProgramChangeOccupied(byte pc)
        {
            bool[] occupied = GetOccupiedProgramChange();
            if (pc < 128) return occupied[pc];
            return false;
        }


        public bool HasAnyProgramChange()
        {
            bool[] occupied = GetOccupiedProgramChange();
            for (int i = 0; i < 128; i++)
            {
                if (occupied[i]) return true;
            }
            return false;
        }


        public bool RemoveSpiSound(byte midiChannel, byte midiNote)
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
        public void AddSfzSound(Sound sound, int midiChannel, int programChange, byte lo, byte hi, byte center, sbyte transpose)
        {
            SpiSound spiSnd = new SpiSound(sound)
            {
                startNote = lo,
                endNote = hi,
                midiNote = center
            };

            if (midiChannel == 128)
            {
                spiSnd.midiChannel = 128;
                spiSnd.programNumber = (byte)programChange;
            } else
            {
                spiSnd.midiChannel = (byte)midiChannel;
                spiSnd.programNumber = 128;
            }

            spiSnd.transpose = transpose;
            spiSounds.Add(spiSnd);
            _findSpiSoundArray = null;
        }


        public void RefreshSpiSounds()
        {
            foreach (var spiSnd in spiSounds)
            {
                spiSnd.SetNameFromSound(GetSoundFromSoundId(spiSnd.soundId));
            }
        }


        private void ClearSpiSounds()
        {
            spiSounds.Clear();
            _findSpiSoundArray = null;
        }


        public void RemoveSpiSound(int index)
        {
            spiSounds.RemoveAt(index);
            _findSpiSoundArray = null;
        }

        public void SortSpiSounds()
        {
            spiSounds.Sort();
        }


        public bool ExportSoundsToDir(string exportDir, out string errorMessage)
        {
            bool result = true;
            errorMessage = null;
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
                Console.WriteLine(ex.ToString());
                errorMessage = ex.Message;
            }
            return result;
        }


        public void AddSound(Sound s)
        {
            sounds.Add(s);
        }

        /*
        public bool AddSound(string file, out string errorMessage)
        {
            Sound s = new Sound();
            if (s.InitSound(file, out errorMessage)) { 
            //if (!String.IsNullOrEmpty(s.path)) {
                AddSound(s);
                return true;
            }
            return false;
        }
        */


        public bool AddSound(string file, out Sound s, out string errorMessage)
        {

            //s = new Sound(file);
            s = new Sound();
            if (s.InitSound(file, out errorMessage))
            //if (!String.IsNullOrEmpty(s.path))
            {
                if (IdenticalSoundExists(s))
                {
                    errorMessage = "Identical sound exists.";
                }
                else
                {
                    AddSound(s);
                    return true;
                }
            }
            return false;
        }

        public void ClearSounds()
        {
            sounds.Clear();
        }


        public void RemoveSound(int idx)
        {
            sounds.RemoveAt(idx);
        }


        public bool SoundRefersToSPISound(int idx)
        {
            Sound snd = sounds[idx];
            List<SpiSound> spiSounds = GetSpiSoundsFromSound(snd);
            return spiSounds.Count > 0;
        }


        public bool AddSoundToSpiSound(Sound sound, int midiChannel, byte startNote, byte endNote, byte center)
        {
            if (OverlapWithAnyExisting(midiChannel, startNote, endNote))
            {
                return false;
            }
            SpiSound spiSnd = new SpiSound(sound)
            {
                midiChannel = (byte)midiChannel,
                midiNote = center,
                startNote = startNote,
                endNote = endNote
            };

            spiSounds.Add(spiSnd);
            _findSpiSoundArray = null;
            return true;
        }


        public void AddSpiSound(SpiSound spiSnd)
        {
            spiSounds.Add(spiSnd);
            _findSpiSoundArray = null;
        }


        public void EnsureCachedSound(SpiSound snd, int newFreq)
        {
            CachedSound cs = snd.cachedSound();
            if (cs == null)
            {
                int newBits = AtariConstants.SampleBits;
                int newChannels = AtariConstants.SampleChannels;

                MemoryStream ms = snd.getWaveStream(this, newFreq, newBits, newChannels);
                ms.Seek(0, SeekOrigin.Begin);
                cs = snd.cachedSound(ms, false, snd.MsLoopStart(), snd.MsLoopEnd(), DynamitePan(snd));
                cs.pitch = 1;
                cs.vvfeOffset = 0;

            }
        }


        private float DynamitePan(SpiSound snd)
        {
            return 0;
            /*
            // only experimental with fixed pans depending on name, as SPI normally dont have this information stored
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
            return pan;
            */
        }

        public CachedSound CachedSound(SpiSound snd, int newFreq, int note, int vel)
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
               
                    cs = snd.cachedSound(ms, loop, snd.MsLoopStart(), snd.MsLoopEnd(), DynamitePan(snd));
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
            

            int vvfeMul;
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
            _programArray = new Dictionary<int, SpiSound[]>();
            _findSpiSoundArray = new Dictionary<int, SpiSound[]>();
            for (int i = 0; i < 16; i++)
            {
                _findSpiSoundArray.Add(i, new SpiSound[128]);
            }
            for (int i = 0; i < 128; i++)
            {
                _programArray.Add(i, new SpiSound[128]);
            }
            foreach (SpiSound snd in spiSounds)
            {
                int idx = snd.midiChannel < 128 ? snd.midiChannel - 1 : snd.programNumber;
                Dictionary<int, SpiSound[]> useArray = snd.midiChannel < 128 ? _findSpiSoundArray : _programArray;                              
                SpiSound[] sounds = useArray[idx];
                int startNote = Math.Min(127, Math.Max(0, (int)snd.startNote));
                int endNote = Math.Min(127, Math.Max(0, (int)snd.endNote));
                for (int note = startNote; note <= (endNote); note++) // 0 - 127
                {
                    sounds[note] = snd;
                }
                useArray[idx] = sounds;

                /*
                if (snd.midiChannel < 128) // skip program change sounds here!
                {
                    SpiSound[] sounds = _findSpiSoundArray[snd.midiChannel - 1];
                    int startNote = Math.Min(127, Math.Max(0, (int)snd.startNote));
                    int endNote = Math.Min(127, Math.Max(0, (int)snd.endNote));
                    for (int note = startNote; note <= (endNote); note++) // 0 - 127
                    {
                        sounds[note] = snd;
                    }
                    _findSpiSoundArray[snd.midiChannel - 1] = sounds;
                }
                */
            }
        }


        public void UpdateSoundForMidiChannel(SpiSound snd, int midiChannel, int note)
        {
            if (_findSpiSoundArray == null) { InitFinder(); }
            int idx = midiChannel - 1;
            Dictionary<int, SpiSound[]> useArray = _findSpiSoundArray;
            SpiSound[] sounds = useArray[idx];
            sounds[note] = snd;
            useArray[idx] = sounds;
        }


        public SpiSound FindSpiSoundFromProgramChange(int programChange, int note)
        {
            if (_findSpiSoundArray == null) { InitFinder(); }
            SpiSound[] sounds = _programArray[programChange];
            return sounds[note];
        }


        public SpiSound FindSpiSoundFromMidiChannel(int midiChannel, int note)
        {
            if (_findSpiSoundArray == null) { InitFinder(); }
            SpiSound[] sounds = _findSpiSoundArray[midiChannel - 1];
            return sounds[note];
        }
    }

}
