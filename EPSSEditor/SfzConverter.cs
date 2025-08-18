using EPSSEditor;
using M;
using NAudio.MediaFoundation;
using NAudio.Midi;
using NAudio.Mixer;
using NAudio.SoundFont;
using NAudio.Wave;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
//using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;
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
            catch (Exception ex)
            {
                Console.Write(ex.ToString());
                errorMessage = ex.Message;
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

        public static bool LoadSfzSound(EPSSEditorData data, int midiChannel, int programChange, string filePath, List<string> filesAdded, out string errorMessage)
        {
            bool result = false;
            errorMessage = "Unknown error";
            
            Dictionary<string, Sound> soundDict= new Dictionary<string, Sound>();
            foreach (Sound s in data.sounds)
            {
                soundDict.Add(s.path, s);
            }           

            errorMessage = "";
            ParseSfz p = new ParseSfz();
            List<SfzBase> bases = p.Parse(filePath);
            string basePath = Path.GetDirectoryName(filePath);

            byte defaultCenter = 60;
            byte defaultLoByte = (byte)(defaultCenter - 24);
            byte defaultHiByte = (byte)(defaultCenter + 24);
            byte defaultKey = 60;

            byte groupKeyCenter = defaultCenter;
            byte groupLoKey = defaultLoByte;
            byte groupHiKey = defaultHiByte;
            byte groupKey = defaultKey;
            string groupSample = "";
            try
            {
                foreach (SfzBase bas in bases)
                {
                    result = false;
                    if (bas is SfzGenericSection gSection)
                    {
                        if (gSection.header.Contains("group"))
                        {
                            groupSample = bas.FilePath(basePath);

                            bool handled = ParseKey(bas.GetValue("key"), out groupKey);
                            if (!handled) groupKey = defaultKey;
                            else
                            {
                                groupLoKey = groupKey;
                                groupHiKey = groupKey;
                                groupKeyCenter = groupKey;
                            }

                            handled = ParseKey(bas.GetValue("pitch_keycenter"), out byte parsedGroupCenter);
                            if (handled) groupKeyCenter = parsedGroupCenter;

                            handled = ParseKey(bas.GetValue("lokey"), out byte parsedLoKey);
                            if (handled) groupLoKey = parsedLoKey;

                            handled = ParseKey(bas.GetValue("hikey"), out byte parsedHiKey);
                            if (handled) groupHiKey = parsedHiKey;
                        }
                        result = true;
                    }

                    if (bas is SfzRegionSection tBase)
                    {
                        string fp = tBase.FilePath(basePath);
                        if (fp == null) fp = groupSample;

                        Sound s;
                        if (soundDict.ContainsKey(fp))
                        {
                            s = soundDict[fp];
                            result = true;
                        }
                        else
                        {
                            result = (data.AddSound(fp, out s, out errorMessage));
                            if (!result) Console.WriteLine($"Sound cannot be added: {fp}");
                        }

                        if (result)
                        {
                            byte kcByte = groupKeyCenter;
                            byte loByte = groupLoKey;
                            byte hiByte = groupHiKey;
                            byte keyByte = groupKey;

                            bool handled = ParseKey(tBase.GetValue("pitch_keycenter"), out byte kc);
                            if (handled) kcByte = kc;
                            handled = ParseKey(tBase.GetValue("lokey"), out byte lb);
                            if (handled) loByte = lb;
                            handled = ParseKey(tBase.GetValue("hikey"), out byte hb);
                            if (handled) hiByte = hb;

                            data.AddSfzSound(s, midiChannel, programChange, loByte, hiByte, kcByte, 0);
                            filesAdded.Add(fp);
                            if (!soundDict.ContainsKey(fp)) soundDict.Add(s.path, s);
                            result = true;
                        }
                    }
                    else
                    {
                        Console.WriteLine("No sample found, skipping region.");
                        result = true;
                    }
                    if (!result) break;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                errorMessage = ex.Message;
                result = false;
            }
            return result;
        }


        private static bool ParseKey(string kcS, out byte kc)
        {
            bool found = false;
            kc = 60; // default
            if (!String.IsNullOrEmpty(kcS))
            {
                found = true;
                if (!Utility.TryToByte(kcS, out kc))
                {
                    int v = Utility.ParseNoteToInt(kcS, 2);
                    if (v < 0 || v > 127)
                    {
                        kc = 128;
                    }
                    else
                    {
                        kc = (byte)v;
                    }
                }
            }
            return found;
        }


        public static Sf2Info Sf2Info(string filePath)
        {
            SoundFont sf = new SoundFont(filePath);

            Sf2Info info = new Sf2Info();

            foreach (var preset in sf.Presets)
            {
                int bank = preset.Bank;
                int patchNumber = preset.PatchNumber;
                info.AddPreset(bank, patchNumber, preset.Name);
            }
            return info;
        }

        public static bool Sf2ContainsMultipleBanks(Sf2Info info)
        {
            bool result = false;

            result = info.Banks().Count > 1;

            return result;
        }


        // Load sf2
        // TODO: Add input of banks here when we have multiple banks in sf2
        public static bool LoadSf2(EPSSEditorData data, int programChange, string filePath, string samplesPath, int wantedBank, List<string> filesAdded, out string errorMessage, Action<int, string> progressCallback = null)      
        {
            bool result = false;
            errorMessage = "Unknown error";
            Directory.CreateDirectory(samplesPath);

            Dictionary<string, Sound> soundDict = new Dictionary<string, Sound>();
            foreach (Sound s in data.sounds)
            {
                soundDict.Add(s.path, s);
            }


            SoundFont sf = new SoundFont(filePath);
            progressCallback(50, "Loading SF2: " + filePath);

            try
            {
                HashSet<string> used = new HashSet<string>();
                int totalPresets = sf.Presets.Count();
                double inc = 50.0 / totalPresets;
                double current = 50.0;
                foreach (var preset in sf.Presets)
                {
                    int bank = preset.Bank;

                    if (bank != wantedBank) continue;

                    progressCallback((int)current, "Loading Bank " + bank.ToString() + ", Preset " + preset.Name);
                    current += inc;

                    int patchNumber = preset.PatchNumber;
                    
                    Console.WriteLine($"Preset: {bank}:{patchNumber}");
                    foreach (var zone in preset.Zones)
                    {
                        Console.WriteLine($"Zone: {zone}");
                        foreach (var gen in zone.Generators)
                        {
                            Console.WriteLine($"Generator: {gen}");
                            if (gen.GeneratorType == GeneratorEnum.Instrument)
                            {
                                Instrument i = gen.Instrument;
                                string instName = i.Name;


                                foreach (var izone in i.Zones)
                                {
                                    SampleHeader sh = null;
                                    byte lo = 0;
                                    byte hi = 128;
                                    byte kcByte = 128;
                                    foreach (var igen in izone.Generators)
                                    {
                                        //Console.WriteLine($"Processing: {igen.GeneratorType}...");
                                        if (igen.GeneratorType == GeneratorEnum.SampleID)
                                        {
                                            sh = igen.SampleHeader;
                                            //uint start = sh.Start;
                                            //uint end = sh.End;
                                            Console.WriteLine($"SampleHeader: {sh}");
                                            //izone.Generators[0].GetType
                                        }
                                        else if (igen.GeneratorType == GeneratorEnum.KeyRange)
                                        {
                                            lo = igen.LowByteAmount;
                                            hi = igen.HighByteAmount;
                                            Console.WriteLine($"GeneratorType: {igen.GeneratorType} {lo} {hi}");
                                        }
                                        else if (igen.GeneratorType == GeneratorEnum.KeyNumber)
                                        {
                                            kcByte = (byte)igen.Int16Amount;
                                            Console.WriteLine($"GeneratorType: {igen.GeneratorType} {kcByte}");
                                        }
                                        else if (igen.GeneratorType == GeneratorEnum.OverridingRootKey)
                                        {
                                            kcByte = (byte)igen.UInt16Amount;
                                            Console.WriteLine($"GeneratorType: {igen.GeneratorType} {kcByte}");
                                        }
                                        else if (igen.GeneratorType == GeneratorEnum.VelocityRange)
                                        {
                                            byte hiVel = igen.HighByteAmount;
                                            byte loVel = igen.LowByteAmount;
                                            Console.WriteLine($"GeneratorType: {igen.GeneratorType} {hiVel} {loVel}");
                                        }
                                        else if (igen.GeneratorType == GeneratorEnum.Pan)
                                        {
                                            double v = (double)igen.Int16Amount / 10.0;
                                            Console.WriteLine($"GeneratorType: {igen.GeneratorType} {v}");
                                        }
                                        else if (igen.GeneratorType == GeneratorEnum.InitialAttenuation)
                                        {
                                            short v = igen.Int16Amount;
                                            Console.WriteLine($"GeneratorType: {igen.GeneratorType} {v}");
                                        }
                                        else if (igen.GeneratorType == GeneratorEnum.FineTune)
                                        {
                                            short v = igen.Int16Amount;
                                            Console.WriteLine($"GeneratorType: {igen.GeneratorType} {v}");
                                        }
                                        else if (igen.GeneratorType == GeneratorEnum.CoarseTune)
                                        {
                                            short v = igen.Int16Amount;
                                            Console.WriteLine($"GeneratorType: {igen.GeneratorType} {v}");
                                        }
                                        else
                                        {
                                            Console.WriteLine($"GeneratorType: {igen.GeneratorType}");
                                        }
                                    }

                                    if (sh != null)
                                    {
                                        string name = sh.SampleName;
                                        var fp = Path.Combine(samplesPath, name + ".wav");

                                        


                                        bool hasSound = false;
                                        Sound s = null;

                                        if (soundDict.ContainsKey(fp))
                                        {
                                            s = soundDict[fp];
                                            hasSound = true;
                                            Console.WriteLine($"Reusing sound {s}");
                                        }
                                        else
                                        {
                                            Console.WriteLine($"Adding sound: {fp}");
                                            if (ConvertSampleFromSf2(fp, sh, sf.SampleData, izone))
                                            {
                                                // TODO: Check why we get identical sound found!
                                                hasSound = data.AddSound(fp, out s, out errorMessage);
                                            }
                                            if (!hasSound) Console.WriteLine($"Sound cannot be added: {fp}");
                                        }

                                        if (hasSound && s != null)
                                        {
                                            if (kcByte == 128)
                                            {
                                                kcByte = (byte)sh.OriginalPitch;
                                            }

                                            int usedPatchNumber = Math.Min(patchNumber + programChange, 127);
                                            
                                            string soundKey = $"{name};{bank};{usedPatchNumber};{lo};{hi};{kcByte}";
                                            // Only use one bank as we cannot really handle multi bank
                                            // TODO read sf2 instruments first and let user choose which bank to convert
                                            if (!used.Contains(soundKey) && (bank == wantedBank || bank == 128)) {
                                                used.Add(soundKey);

                                                if (bank == 128)
                                                { // percussion, channel 10
                                                    data.AddSfzSound(s, 10, 128, lo, hi, kcByte, 0);
                                                }
                                                else
                                                {
                                                    data.AddSfzSound(s, 128, usedPatchNumber, lo, hi, kcByte, 0);
                                                }
                                                filesAdded.Add(fp);
                                                if (!soundDict.ContainsKey(fp)) soundDict.Add(s.path, s);
                                                result = true;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            data.SortSpiSounds();

            return result;
        }


        private static int OverridingRootKey(Zone izone)
        {
            UInt16 overridingRootKey = 0;
            foreach (var gen in izone.Generators)
            {

            }
            return overridingRootKey;
        }


        private static bool ConvertSampleFromSf2(string path, SampleHeader sh, byte[] sample, Zone izone)
        {
            bool result = false;
            
            if (sh != null)
            {
                uint ls = (sh.StartLoop - sh.Start);
                uint le = (sh.EndLoop - sh.Start);

                UInt16 sampleModes = 0;
                UInt16 overridingRootKey = 0;
                foreach (var gen in izone.Generators)
                {
                    if (gen.GeneratorType == GeneratorEnum.SampleModes)
                    {
                        sampleModes = gen.UInt16Amount;
                    }
                 }
                bool loop = sampleModes != 0;

                using (FileStream fs = new FileStream(path, FileMode.Create))
                {
                    using (var wfw = new WaveLoopFileWriter(fs, new WaveFormat((int)sh.SampleRate, 16, 1), loop, ls, le))
                    {
                        wfw.Write(sample, 2 * (int)sh.Start, 2 * (int)(sh.End - sh.Start));
                        result = true;
                    }
                }

            }

            return result;
        }

        /*
         * https://csharp.hotexamples.com/site/redirect?url=https%3A%2F%2Fgithub.com%2Fzeromus%2Fsf2xrni
        void ImportSamples(SoundFont sf2, Preset preset, XInstrument xrni)
        {
            var xl = new List<XSample>();
            var ml = new List<SampleMap>();
            var il = new List<int>();
            foreach (var pzone in preset.Zones)
            { // perc. bank likely has more than one instrument here.
                var i = pzone.Instrument();
                var kr = pzone.KeyRange(); // FIXME: where should I use it?
                if (i == null)
                    continue; // FIXME: is it possible?

                var vr = pzone.VelocityRange();

                // an Instrument contains a set of zones that contain sample headers.
                int sampleCount = 0;
                foreach (var izone in i.Zones)
                {
                    var ikr = izone.KeyRange();
                    var ivr = izone.VelocityRange();
                    var sh = izone.SampleHeader();
                    if (sh == null)
                        continue; // FIXME: is it possible?

                    // FIXME: sample data must become monoral (panpot neutral)
                    var xs = ConvertSample(sampleCount++, sh, sf2.SampleData, izone);
                    xs.Name = NormalizePathName(sh.SampleName);
                    ml.Add(new SampleMap(ikr, ivr, xs, sh));
                }
            }

            ml.Sort((m1, m2) =>
                m1.KeyLowRange != m2.KeyLowRange ? m1.KeyLowRange - m2.KeyLowRange :
                m1.KeyHighRange != m2.KeyHighRange ? m1.KeyHighRange - m2.KeyHighRange :
                m1.VelocityLowRange != m2.VelocityLowRange ? m1.VelocityLowRange - m2.VelocityLowRange :
                m1.VelocityHighRange - m2.VelocityHighRange);

            int prev = -1;
            foreach (var m in ml)
            {
                prev = m.KeyLowRange;
                il.Add(m.KeyLowRange);
                xl.Add(m.Sample);
            }

            xrni.SampleSplitMap = new SampleSplitMap();
            xrni.SampleSplitMap.NoteOnMappings = new SampleSplitMapNoteOnMappings();
            var nm = new SampleSplitMapping[ml.Count];
            xrni.SampleSplitMap.NoteOnMappings.NoteOnMapping = nm;
            for (int i = 0; i < ml.Count; i++)
            {
                var m = ml[i];
                var n = new SampleSplitMapping();
                n.BaseNote = m.Sample.BaseNote;
                n.NoteStart = m.KeyLowRange;
                n.NoteEnd = m.KeyHighRange <= 0 ? 128 : m.KeyHighRange;
                n.SampleIndex = i;
                if (m.VelocityHighRange > 0)
                {
                    n.MapVelocityToVolume = true;
                    n.VelocityStart = m.VelocityLowRange;
                    n.VelocityEnd = m.VelocityHighRange;
                }
                nm[i] = n;
            }

            xrni.Samples = new RenoiseInstrumentSamples();
            xrni.Samples.Sample = xl.ToArray();
        }

        		XSample ConvertSample (int count, SSampleHeader sh, byte [] sample, Zone izone)
		{
			// Indices in sf2 are numbers of samples, not byte length. So double them.
			var xs = new XSample ();
			xs.Extension = ".wav";
			xs.LoopStart =(sh.StartLoop - sh.Start);
			xs.LoopEnd = (sh.EndLoop - sh.Start);
			int sampleModes = izone.SampleModes ();
			xs.LoopMode = sampleModes == 0 ? InstrumentSampleLoopMode.Off : InstrumentSampleLoopMode.Forward;
			xs.Name = String.Format ("Sample{0:D02} ({1})", count, sh.SampleName);
			xs.BaseNote = (sbyte) izone.OverridingRootKey ();
//			xs.Volume = (izone.VelocityRange () & 0xFF00 >> 8); // low range
			if (xs.BaseNote == 0)
				xs.BaseNote = (sbyte) sh.OriginalPitch;
//Console.WriteLine ("{0} ({1}/{2}/{3}/{4}) {5}:{6}:{7}:{8}", xs.Name, sh.Start, sh.StartLoop, sh.EndLoop, sh.End, sh.SampleRate != 0xAC44 ? sh.SampleRate.ToString () : "", sh.OriginalPitch != 60 ? sh.OriginalPitch.ToString () : "", sh.PitchCorrection != 0 ? sh.PitchCorrection.ToString () : "", sampleModes);
			xs.FileName = xs.Name + ".wav";
			var ms = new MemoryStream ();
			var wfw = new WaveFileWriter (ms, new WaveFormat ((int) sh.SampleRate, 16, 1));
			wfw.WriteData (sample, 2 * (int) sh.Start, 2 * (int) (sh.End - sh.Start));
			wfw.Close ();
			xs.Buffer = ms.ToArray ();

			return xs;
		}

        		string NormalizePathName (string name)
		{
			foreach (char c in Path.GetInvalidPathChars ())
				name = name.Replace (c, '_');
			name = name.Replace (':', '_');
			return name;
		}

        */
    }
}
