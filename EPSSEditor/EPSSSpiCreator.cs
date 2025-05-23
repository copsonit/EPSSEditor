﻿using NAudio.Wave;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EPSSEditor
{
    public class EPSSSpiCreator
    {
        public int noOfMidiCh = 16; // always create 16 channel maps from now on
        public int noOfSounds;
        public int maxNoOfSounds = 255;
        public int version = 0;

        public EPSSSpiCreator(int version)
        {
            this.version = version;
        }


        public void Initialize(EPSSSpi spi)
        {
            spi.Initialize();
        }


        public void FillInMain(EPSSSpi spi)
        {
            spi.main.i_no_of_MIDIch.No_of_MIDICh = (byte)noOfMidiCh;

            spi.main.i_no_of_sounds.No_of_sounds = (byte)(noOfSounds);

            spi.main.i_patch_offset = (UInt16)(spi.main.Length() + spi.ext.Length());

            if (version >= 2)
            {
                spi.main.i_fileID.VersionLow = 2;
                spi.main.i_fileID.VersionHigh = 0;
            }
            else
            {
                spi.main.i_fileID.VersionLow = 1;
                spi.main.i_fileID.VersionHigh = 1;
            }
        }


        public void FillInExt(EPSSSpi spi, string name, string info)
        {
            DateTime dr = DateTime.Now;
            spi.ext.SetCreationDateTime(DateTime.Now);
            spi.ext.SetChangeDateTime(DateTime.Now);
            spi.ext.i_pname = name;
            spi.ext.i_mainlen = (UInt16)(spi.main.Length() + spi.ext.Length());
            spi.ext.i_splitlen = 2;
            spi.ext.i_xsinflen = 64;
            spi.ext.i_sinflen = 16;
            spi.ext.i_patchinfo = info;
        }


        private List<EPSSSpi_midiChannelSplit> FillInMidiSplits(EPSSEditorData data)
        {
            List<EPSSSpi_midiChannelSplit> channels = new List<EPSSSpi_midiChannelSplit>();

            for (int midiChannel = 1; midiChannel <= noOfMidiCh; midiChannel++)
            {
                EPSSSpi_midiChannelSplit channel = new EPSSSpi_midiChannelSplit
                {
                    data = new EPSSSpi_soundAndPitch[128]
                };

                List<SpiSound> sound = data.GetSpiSoundsForMidiChannel(midiChannel, data.omni);

                bool useMidiSplit = false;
                foreach (SpiSound sndToFind in sound)
                {
                    if (sndToFind.startNote < 128 && sndToFind.endNote < 128)
                    {
                        useMidiSplit = true;
                        break;
                    }
                }

                if (sound.Count == 0) // empty channel
                {

                    for (int i = 0; i < 128; i++)
                    {
                        EPSSSpi_soundAndPitch sp = new EPSSSpi_soundAndPitch
                        {
                            Sound = 0,
                            Pitch = 0,
                            NoSound = 1
                        };
                        channel.data[i] = sp;


                    }
                    channels.Add(channel);
                }
                else if (useMidiSplit)
                {
                    for (int i = 0; i < 128; i++)
                    {
                        EPSSSpi_soundAndPitch sp = new EPSSSpi_soundAndPitch
                        {
                            Sound = 0,
                            Pitch = 0,
                            NoSound = 1
                        };
                        channel.data[i] = sp;
                    }

                    foreach (SpiSound sndToFind in sound)
                    {
                        if (sndToFind.startNote < 128 && sndToFind.endNote < 128)
                        {
                            for (int i = sndToFind.startNote; i <= sndToFind.endNote; i++)
                            {
                                int relativeNote = i - sndToFind.midiNote; // -24 to 24 ideally
                                relativeNote += 84; // Use new layout, i.e. 84 is center.
                                EPSSSpi_soundAndPitch sp = new EPSSSpi_soundAndPitch
                                {
                                    Sound = (byte)data.GetSoundNumberFromGuid(sndToFind.soundId),
                                    Pitch = (byte)relativeNote,
                                    NoSound = 0
                                };
                                channel.data[i] = sp;
                            }
                        }
                    }

                    channels.Add(channel);
                }
                else if (sound.Count == 1 && midiChannel != 10)
                {
                    // What type of map is this?
                    SpiSound snd = sound.First();
                    byte note = 60;
                    for (int i = 0; i < 128; i++)
                    {
                        EPSSSpi_soundAndPitch sp = new EPSSSpi_soundAndPitch();

                        if (i >= 36 && i <= 84) // C2 - C6
                        {
                            sp.Sound = (byte)data.GetSoundNumberFromGuid(snd.soundId);
                            sp.Pitch = snd.transposedNote(note);
                            note++;
                            sp.NoSound = 0;
                        }
                        else
                        {
                            sp.Sound = 0;
                            sp.Pitch = 0;
                            sp.NoSound = 1;
                        }
                        channel.data[i] = sp;


                    }
                    channels.Add(channel);

                }
                // What does this really do? Seems to be mapping samples like drum samples for other channels than MIDI 10?
                else if (sound.Count > 1 && midiChannel != 10)
                {

                    SpiSound snd = sound.First();
                    for (int i = 0; i < 128; i++)
                    {
                        EPSSSpi_soundAndPitch sp = new EPSSSpi_soundAndPitch();
                        bool found = false;
                        foreach (SpiSound sndToFind in sound)
                        {
                            if (sndToFind.midiNote == i + 1)
                            {
                                sp.Sound = (byte)data.GetSoundNumberFromGuid(sndToFind.soundId);
                                sp.Pitch = snd.transposedNote(84);
                                sp.NoSound = 0;
                                found = true;
                            }
                            if (found) break;
                        }
                        if (!found)
                        {
                            sp.Sound = 0;
                            sp.Pitch = 0;
                            sp.NoSound = 1;

                        }
                        channel.data[i] = sp;
                    }
                    channels.Add(channel);
                }
                else if (midiChannel == 10) // drums
                {

                    for (int i = 0; i < 128; i++)
                    {
                        EPSSSpi_soundAndPitch sp = new EPSSSpi_soundAndPitch();

                        byte note = 0;
                        byte spiSound = 0;
                        foreach (SpiSound snd in sound)
                        {

                            if (snd.midiNote == i)
                            {
                                note = snd.transposedNote(84);
                                spiSound = (byte)data.GetSoundNumberFromGuid(snd.soundId);
                                break; // Only use first found. UI only allows one sound per not so it should be safe.
                            }
                        }

                        if (note > 0)
                        {
                            sp.Sound = spiSound;
                            sp.Pitch = note;
                            sp.NoSound = 0;
                        }
                        else
                        {
                            sp.Sound = 0;
                            sp.Pitch = 0;
                            sp.NoSound = 1;
                        }

                        channel.data[i] = sp;

                    }
                    channels.Add(channel);
                }
                else if (sound.Count > 1)
                {
                    System.Windows.Forms.MessageBox.Show("More than one sound per MIDI channel only supported for MIDI channel 10!");
                }

            }

            return channels;
        }


        private List<EPSSSpi_programChangeSplit> FillInProgramChangeSplits(EPSSEditorData data)
        {
            // Initialize all splits to empty.
            List<EPSSSpi_programChangeSplit> programChanges = new List<EPSSSpi_programChangeSplit>();
            for (int i = 0; i < 128; i++)
            {
                List<EPSSSpi_soundAndPitchGen2> splits = new List<EPSSSpi_soundAndPitchGen2>();
                for (int note = 0; note < 128; note++)
                {
                    EPSSSpi_soundAndPitchGen2 pitchNote = new EPSSSpi_soundAndPitchGen2
                    {
                        Sound = 0,
                        Pitch = 0,
                        NoSound = 1 // Set all to unused initially.
                    };
                    splits.Add(pitchNote);
                }
                EPSSSpi_programChangeSplit pc = new EPSSSpi_programChangeSplit
                {
                    data = splits.ToArray()
                };
                programChanges.Add(pc);
            }


            // Look through all the snds and enter the splits for them.
            foreach (var snd in data.SpiSounds())
            {
                if (snd.programNumber < 128)
                {
                    byte loNote = snd.startNote;
                    byte hiNote = snd.endNote;
                    for (byte key = loNote; key <= hiNote; key++)
                    {
                        programChanges[snd.programNumber].data[key].NoSound = 0; // Mark that sound is used
                        programChanges[snd.programNumber].data[key].Pitch = (byte)(key); // 60 - 108 normal mapping
                        programChanges[snd.programNumber].data[key].Sound = (UInt16)data.GetSoundNumberFromGuid(snd.soundId);
                    }
                }
            }

            return programChanges;
        }


        public void FillInSplit(EPSSEditorData data, EPSSSpi spi)
        {
            List<EPSSSpi_midiChannelSplit> channels = FillInMidiSplits(data);

            spi.split.channels = channels.ToArray();

            EPSSSpi_splitInfo sp = spi.split;

            if (sp is EPSSSpi_splitInfoGen2 tSplit)
            {
                List<EPSSSpi_programChangeSplit> programChanges = FillInProgramChangeSplits(data);
                tSplit.programs = programChanges.ToArray();
            }

            int mainLength = spi.main.Length();
            int extLength = spi.ext.Length();
            int splitLength = spi.split.Length();
            int total = mainLength + extLength + splitLength;


            EPSSSpi_extended ext = spi.ext;
            if (ext is EPSSSpi_extendedGen2 tExt)
            {
                spi.main.i_sinfo_offset = 0;
                tExt.i_sinfo_offset_g2 = (UInt32)total;
            }
            else
            {
                spi.main.i_sinfo_offset = (UInt16)total;
            }
        }


        public EPSSSpi_sample GetSampleFromSpiSound(EPSSEditorData data, SpiSound snd, int freq)
        {
            EPSSSpi_sample smp = new EPSSSpi_sample();

            string outFile = Path.GetTempFileName();

            if (snd.convertSound(data, outFile, freq, AtariConstants.SampleBits, AtariConstants.SampleChannels))
            {
                using (var wav = File.OpenRead(outFile))
                {
                    wav.Seek(0, SeekOrigin.Begin);

                    WaveFileReader fr = new WaveFileReader(wav);

                    List<byte> spl = new List<byte>();
                    smp.data = new byte[fr.Length];
                    int read = fr.Read(smp.data, 0, smp.data.Length);
                }

                File.Delete(outFile);
            }


            /*
            Sound sound = data.getSoundFromSoundId(snd.soundId);
            if (sound != null)
            {
                string path = sound.path;

                float volume = 1.0f;
                float max = 1.0f;
                if (sound.parameters().normalize.normalize)
                {
                    volume = (float)(sound.parameters().normalize.normalizePercentage / 100.0);

                    max = 0;
                    using (var reader = new AudioFileReader(path))
                    {
                        // find the max peak
                        float[] buffer = new float[reader.WaveFormat.SampleRate];
                        int read;
                        do
                        {
                            read = reader.Read(buffer, 0, buffer.Length);
                            for (int n = 0; n < read; n++)
                            {
                                var abs = Math.Abs(buffer[n]);
                                if (abs > max) max = abs;
                            }
                        } while (read > 0);
                    }
                }

                string outFile = Path.GetTempFileName();

                int outRate = 25033; // TODO: read the setting
                using (var reader = new AudioFileReader(path))
                {

                    // rewind and amplify
                    reader.Position = 0;
                    //                    reader.Volume = 1.0f / max;
                    reader.Volume = volume / max;


                    var outFormat = new WaveFormat(outRate, reader.WaveFormat.Channels);
                    using (var resampler = new MediaFoundationResampler(reader, outFormat))
                    {
                        resampler.ResamplerQuality = 60;
                        WaveFileWriter.CreateWaveFile(outFile, resampler);
                    }
                }



                using (var wav = File.OpenRead(outFile))
                {
                    wav.Seek(0, SeekOrigin.Begin);

                    WaveFileReader fr = new WaveFileReader(wav);

                    List<byte> spl = new List<byte>();

                    while (true)
                    {
                        float[] samples = fr.ReadNextSampleFrame();
                        if (samples == null) break;

                        byte b = (byte)((samples[0] + 1.0f) * 128.0f); // unsigned
                        //                  if (b < 128) b = (byte)(b + 128);
                        //                  else b = (byte)(b - 128);
                        spl.Add(b);
                    }
                    smp.data = spl.ToArray();
                }

                File.Delete(outFile);

            }
            else
            {
                //            smp.loadSpl(new Uri(@"D:\OneDrive\Atari\Emulators etc\HDimage\Syquest\LJUDMODF\F\SQR_WAV.SPL")); // TODO just for first test
                smp.loadSpl(new Uri(@"D:\OneDrive\Atari\Emulators etc\HDimage\Syquest\LJUDMODF\F\TECHNOSD.SPL")); // TODO just for first test
            }

            */

            return smp;
        }

        public EPSSSpi_soundInfo GetSoundInfo(EPSSSpi_sample smp, SpiSound snd)
        {
            EPSSSpi_loopmode loopmode = new EPSSSpi_loopmode
            {
                Toneoffset = snd.transpose,
                Loopmode = snd.loopMode,
                Vvfe = snd.vvfe
            };

            EPSSSpi_s_gr_frek sgrfrek = new EPSSSpi_s_gr_frek
            {
                data = snd.s_gr_frek
            };

            EPSSSpi_soundInfo info = new EPSSSpi_soundInfo
            {
                s_sampstart = 0,
                s_sampend = (uint)smp.data.Length,
                s_loopstart = snd.MsLoopStart(),
                s_loopmode = loopmode,
                s_gr_freq = sgrfrek
            };

            return info;
        }


        public EPSSSpi_extSoundInfo GetExtSoundInfoFromSpiSound(SpiSound snd)
        {
            EPSSSpi_extSoundInfo extInfo = new EPSSSpi_extSoundInfo
            {
                s_sampname = snd.name(),  // "TstSam" + (i + 1).ToString();
                s_extname = snd.extName(), ///  "Sample #" + (i + 1).ToString();
                s_extvolume = snd.extVolume,
                s_subtone = snd.subTone
            };

            return extInfo;
        }


        public void FillInSamples(EPSSEditorData data, EPSSSpi spi, int sampFreq)
        {
            List<EPSSSpi_soundInfo> soundInfos = new List<EPSSSpi_soundInfo>();
            List<EPSSSpi_extSoundInfo> extSoundInfos = new List<EPSSSpi_extSoundInfo>();
            List<EPSSSpi_sample> samples = new List<EPSSSpi_sample>();

            SortedDictionary<int, SpiSound> sortedSounds = data.GetUsedSounds();
            foreach (KeyValuePair<int, SpiSound> entry in sortedSounds)
            {
                SpiSound snd = entry.Value;

                EPSSSpi_sample sample = GetSampleFromSpiSound(data, snd, sampFreq);

                EPSSSpi_soundInfo sInfo = GetSoundInfo(sample, snd);

                EPSSSpi_extSoundInfo extSinfo = GetExtSoundInfoFromSpiSound(snd);

                samples.Add(sample);
                soundInfos.Add(sInfo);
                extSoundInfos.Add(extSinfo);
            }

            spi.sounds.sounds = soundInfos.ToArray();
            spi.extSounds.sounds = extSoundInfos.ToArray();
            spi.samples.samples = samples.ToArray();
        }


        public void FillInOffsets(EPSSSpi spi)
        {
            // Fill in the offsets
            spi.ext.i_sx_offset = (UInt16)(spi.main.Length() + spi.ext.Length() + spi.split.Length() + spi.sounds.Length());

            spi.main.i_sdata_offset = (UInt16)(spi.main.Length() + spi.ext.Length() + spi.split.Length() + spi.sounds.Length() + spi.extSounds.Length());

            uint sampleStartOffset = spi.main.i_sdata_offset;
            foreach (EPSSSpi_soundInfo info in spi.sounds.sounds)
            {
                uint smpLen = info.s_sampend;

                info.s_sampstart += sampleStartOffset;
                info.s_sampend += sampleStartOffset;
                info.s_loopstart += sampleStartOffset;

                sampleStartOffset += smpLen;
            }

            int mainLen = spi.main.Length();
            int extLen = spi.ext.Length();
            int splitLen = spi.split.Length();
            int soundLen = spi.sounds.Length();
            int extSoundsLen = spi.extSounds.Length();
            int samplesLen = spi.samples.Length();
            int total = mainLen + extLen + splitLen + soundLen + extSoundsLen + samplesLen;

            spi.main.i_filelen = (UInt32)total;
        }


        public EPSSSpi Create(EPSSEditorData data, string name, string info, int sampFreq)
        {
            noOfSounds = data.GetUsedSounds().Count;

            if ((version <= 1 && noOfSounds < 256) ||
                (version >=2 && noOfSounds < 65536))
            {
                EPSSSpi spi = version >= 2 ? new EPSSSpiGen2() : (EPSSSpi)new EPSSSpiG0G1();
                Initialize(spi);
                FillInMain(spi);
                FillInExt(spi, name, info);
                FillInSplit(data, spi);
                FillInSamples(data, spi, sampFreq);
                FillInOffsets(spi);
                return spi;
            }
            else
            {
                System.Windows.Forms.MessageBox.Show("More than 256 sounds are not supported in an SPI file!");
            }

            return null;
        }


        public long Length(EPSSEditorData data)
        {
            SortedDictionary<int, SpiSound> usedSounds = data.GetUsedSounds();
            int noOfSounds = usedSounds.Count;

            long mainLength = 16;
            long spiExtLength = 64;
            long spiSplitLength = 256 * noOfMidiCh;
            long spiSoundsLength = 16 * noOfSounds;
            long spiExtSoundsLength = 64 * noOfSounds;
            long spiSamplesLength = 0;

            foreach (var idxAndSnd in usedSounds)
            {
                SpiSound snd = idxAndSnd.Value;
                spiSamplesLength += snd.preLength(data);              
            }

            return mainLength + spiExtLength + spiSplitLength + spiSoundsLength + spiExtSoundsLength + spiSamplesLength;
        }


        public EPSSSpi CreateTestSpi()
        {
            noOfMidiCh = 16;
            noOfSounds = 16;


            EPSSSpi spi = new EPSSSpiG0G1();

            Initialize(spi);
            /*            spi.main= new EPSSSpi_main();
                        spi.ext = new EPSSSpi_extended();
                        spi.split = new EPSSSpi_splitInfo();
                        spi.sounds = new EPSSSpi_sounds();
                        spi.extSounds = new EPSSSpi_extSounds();
                        spi.samples = new EPSSSpi_samples(); */


            /* Fill in main */
            FillInMain(spi);
            /*            spi.main.i_no_of_MIDIch.no_of_MIDICh = (byte)noOfMidiCh;

                        spi.main.i_no_of_sounds.no_of_sounds = (byte)(noOfSounds);

                        spi.main.i_patch_offset = (UInt16)(spi.main.length() + spi.ext.length());

                        spi.main.i_fileID.versionLow = 1;
                        spi.main.i_fileID.versionHigh = 1; */


            /* Fill in ext */
            FillInExt(spi, "SpiPc", "Spi created by EPSSSpi for Windows");
            /* //calculate spi.ext.i_sx_offset
             DateTime dr = DateTime.Now;
            spi.ext.i_crtime = dr.ToDosDateTime();
            spi.ext.i_crdate = dr.ToDosDateTime();
            spi.ext.i_chtime = dr.ToDosDateTime();
            spi.ext.i_chdate = dr.ToDosDateTime();
            spi.ext.i_pname = "SpiPc";
            spi.ext.i_mainlen = (UInt16)(spi.main.length() + spi.ext.length());
            spi.ext.i_splitlen = 2;
            spi.ext.i_xsinflen = 64;
            spi.ext.i_sinflen = 16;
            spi.ext.i_patchinfo = "Spi created by EPSSSpi for Windows"; */

            /* Fill in split */




            List<EPSSSpi_midiChannelSplit> channels = new List<EPSSSpi_midiChannelSplit>();
            for (int ch = 0; ch < noOfMidiCh; ch++)
            {
                EPSSSpi_midiChannelSplit channel = new EPSSSpi_midiChannelSplit
                {
                    data = new EPSSSpi_soundAndPitch[128]
                };
                byte note = 60;
                for (int i = 0; i < 128; i++)
                {
                    EPSSSpi_soundAndPitch sp = new EPSSSpi_soundAndPitch();

                    if (i >= 36 && i <= 84)
                    {
                        sp.Sound = (byte)ch;
                        sp.Pitch = note++;
                        sp.NoSound = 0;
                    }
                    else
                    {
                        sp.Sound = 0;
                        sp.Pitch = 0;
                        sp.NoSound = 1;
                    }
                    channel.data[i] = sp;


                }
                channels.Add(channel);

            }
            spi.split.channels = channels.ToArray();

            spi.main.i_sinfo_offset = (UInt16)(spi.main.Length() + spi.ext.Length() + spi.split.Length());



            /* Fill in ext soundinfo */

            /* Fill in info */

            /* Fill in samples */

            List<EPSSSpi_soundInfo> soundInfos = new List<EPSSSpi_soundInfo>();
            List<EPSSSpi_extSoundInfo> extSoundInfos = new List<EPSSSpi_extSoundInfo>();
            List<EPSSSpi_sample> samples = new List<EPSSSpi_sample>();

            //            int sz = 10240;

            int tot = 0;
            for (int i = 0; i < noOfSounds; i++)
            {
                EPSSSpi_sample smp = new EPSSSpi_sample();
                // convert sample to internal format smp.data 
                if (i == 0)
                {
                    smp.LoadSpl(new Uri(@"D:\OneDrive\Atari\Emulators etc\HDimage\Syquest\LJUDMODF\F\SQR_WAV.SPL"));
                }
                else
                {

                    int sz = 5000;
                    smp.data = new byte[sz];
                    byte b = 128;
                    int v = 1;
                    int startX = 1;
                    int x = startX;
                    for (int j = 0; j < sz; j++)
                    {
                        smp.data[j] = b;
                        if (x-- < 0)
                        {


                            if (b + v > 255)
                            {
                                v = -v;
                            }
                            else if (b + v < 0)
                            {
                                v = -v;
                            }
                            b = (byte)(b + v);
                            x = startX;
                        }
                    }
                }
                samples.Add(smp);

                EPSSSpi_soundInfo info = new EPSSSpi_soundInfo
                {
                    s_sampstart = 0,
                    s_sampend = (uint)smp.data.Length,
                    s_loopstart = 0
                };
                info.s_loopmode.Toneoffset = 0;
                info.s_loopmode.Loopmode = 1;
                info.s_loopmode.Vvfe = 0;
                info.s_gr_freq.Drum = 0;
                info.s_gr_freq.Velocity = 0;
                info.s_gr_freq.SoundType = 0;
                info.s_gr_freq.Mode = 1;
                info.s_gr_freq.Aftertouch = 0;
                info.s_gr_freq.StereoType = 0;
                info.s_gr_freq.StereoPan = 0;
                info.s_gr_freq.OrgFreq = 3;

                EPSSSpi_extSoundInfo extInfo = new EPSSSpi_extSoundInfo
                {
                    s_sampname = "TstSam" + (i + 1).ToString(),
                    s_extname = "Sample #" + (i + 1).ToString(),
                    s_extvolume = 100,
                    s_subtone = 0
                };


                soundInfos.Add(info);
                extSoundInfos.Add(extInfo);
                tot++;

            }
            spi.sounds.sounds = soundInfos.ToArray();
            spi.extSounds.sounds = extSoundInfos.ToArray();
            spi.samples.samples = samples.ToArray();


            // Fill in the offsets
            spi.ext.i_sx_offset = (UInt16)(spi.main.Length() + spi.ext.Length() + spi.split.Length() + spi.sounds.Length());

            spi.main.i_sdata_offset = (UInt16)(spi.main.Length() + spi.ext.Length() + spi.split.Length() + spi.sounds.Length() + spi.extSounds.Length());

            uint sampleStartOffset = spi.main.i_sdata_offset;
            foreach (EPSSSpi_soundInfo info in spi.sounds.sounds)
            {
                uint smpLen = info.s_sampend;

                info.s_sampstart += sampleStartOffset;
                info.s_sampend += sampleStartOffset;
                info.s_loopstart += sampleStartOffset;

                sampleStartOffset += smpLen;
            }

            spi.main.i_filelen = (UInt32)(spi.main.Length() + spi.ext.Length() + spi.split.Length() + spi.sounds.Length() + spi.extSounds.Length() + spi.samples.Length());

            return spi;
        }
    }
}
