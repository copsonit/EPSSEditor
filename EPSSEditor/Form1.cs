using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Linq;
using System.Xml.XPath; // for XPathSelectElements
using System.Xml.Serialization;
using System.IO;
using NAudio.Wave;

using System.Configuration;  // Add a reference to System.Configuration.dll
using System.Reflection;
using System.Deployment;

using System.Threading.Tasks;
using System.Runtime.InteropServices;
using System.Diagnostics;
using System.Management;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace EPSSEditor
{


    /*
     * TODO:
     * OK. Show and reset settings
     * OK. Check for adding same drum sound on the same note
     * OK. Auto increase for adding drums
     * OK. Multisample patch
     * OK. Omni switch
     * OK. Multisample for drums, automatic spread
     * OK. Remove the checkbox look of the right window.
     * 
     * OK. Delete should work for selected samples in left window!
     * TODO Support 32-bit sounds! Some newly found Wav reports as 32-bit Wav. 
     * TODO Until we support, warn when loading the sound!
     * OK  Support for loading sfz
     * WIP  Support to build SPI v3 with program change.
     * OK      UI and add sounds
     * NYI     Save as SPI v3 if program change sounds are used.
     * WIP  Load SPI.
     * 
     * Bugs:
     * OK.  Cannot find the drumMappings.xml in installed version.
     * OK.  Did not update the sample size field for Sound correctly.
     *      DOS-date interpreted wrong in EPSS. 2000 issue, needs to be fixed in EPSS!
     * OK.  Did not save the directories for project, samples and spi correctly. Fixed in 1.04.
     * TODO If no sounds found, shoulnt crash:
     *         Need to show load dialog for each sound to let the user find missing sounds:
     *         Use file name to try to find them. After loading of a sound, try to find all in same folder.
     * TODO When Loading a patch, not everything is updated: windows, Total Size in Bytes etc.
     * TODO Let sample window info should be disabled if nothing is selected, i.e. first time loaded, alt auto click first sound.
     * TODO Sound size after compression also need to be initialized correctly.
     * TODO MIDI Mapping button has to be correctly set according to what is saved in patch when loaded.
     * TODO Change default text to "Created with EPSS Editor" + version number. Or somewhere store which version it is created in, in SPI Info. Reserve a few bytes for this?
     *      
     * 
     * Wishlist:
     * OK      Multiple selection of sounds to automatically add? Mutisample + Drums
     * WIP      Play converted sound on PC? What is not working?
     *          Project should save all samples as well in the project file. Stream it to xml? Binary format.
     *          Idea: project file as sqlite instead of db3?
     *          Check what sample formats that we support through the libs and make more possible to read? 
     *          SoundFonts?
     * 
     * 
     * */



    public partial class Form1 : Form
    {

        public EPSSEditorData data;
        public bool deletePressed;
        public bool callbacks = true;
        public int initialize;
        public bool dataNeedsSaving;
        private ControlScrollListener _processListViewScrollListener;

        public Form1()
        {
            InitializeComponent();
            initialize = 0;
            dataNeedsSaving = false;

            _processListViewScrollListener = new ControlScrollListener(spiSoundListView);
            _processListViewScrollListener.ControlScrolled += SpiSoundListViewScrollListener_ControlScrolled;
        }


        private void Form1_Load(object sender, EventArgs e)
        {
            this.Text = "EPSS Editor v" + GetRunningVersion().ToString();



        }


        private Version GetRunningVersion()
        {
            try
            {
                return System.Deployment.Application.ApplicationDeployment.CurrentDeployment.CurrentVersion;
            }
            catch
            {
                return Assembly.GetExecutingAssembly().GetName().Version;
            }
        }


        private void Form1_Activated(object sender, EventArgs e)
        {

            if (initialize < 1)
            {
                initialize++;

                initEpssEditorData();
            }

        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            exit();
        }


        private void exit()
        {
            Properties.Settings.Default.Save();
            saveProjectSettings();
        }


        private void initEpssEditorData(bool forceNewProject = false)
        {
            string projectFile = Properties.Settings.Default.ProjectFile;
            bool initNewProjectFile = false;
            if (forceNewProject || projectFile == null | projectFile == "")
            {
                initNewProjectFile = true;


            } else
            {
                if (!loadProjectSettings(projectFile))
                {
                    initNewProjectFile = true;
                }
            }

            if (initNewProjectFile)
            {
                bool projectFileDefined = false;

                while (true)
                {
                    if (!forceNewProject && MessageBox.Show("No SPI Project file found!\nPress Yes load an existing\nproject or No to initialize a new project.", "Confirm", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                    {

                        if (loadProjectSettingsFileDialog())
                        {
                            projectFileDefined = true;
                            break;
                        }
                        else
                        {
                            if (MessageBox.Show("EPSS needs a project file to continue.\nDo you want to exit the program?", "Confirm", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                            {
                                Close();
                                break;
                                //throw new ApplicationException("Exit by user");
                            }
                        }
                    }

                    else
                    {


                        saveProjectFileDialog.Title = "Choose filename for new EPSS project ...";
                        if (saveProjectFileDialog.ShowDialog() == DialogResult.OK)
                        {
                            string file = saveProjectFileDialog.FileName;
                            Properties.Settings.Default.ProjectFile = file;
                            Properties.Settings.Default.Save();
                            projectFileDefined = true;
                            break;
                        }
                        else
                        {
                            if (!forceNewProject)
                            {
                                if (MessageBox.Show("EPSS needs a project file to continue.\nDo you want to exit the program?", "Confirm", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                                {
                                    //throw new ApplicationException("Exit by user");
                                    Close();
                                    break;
                                }
                            }
                            else
                            {
                                break;
                            }
                        }
                    }

                }

                if (projectFileDefined)
                {

                    data = new EPSSEditorData();



                    //string dir = Path.GetDirectoryName(System.Reflection.Assembly.G‌​etEntryAssembly().Lo‌​cation);
                    string dir = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
                    string combined = Path.Combine(dir, "drumMappings.xml");
                    FileInfo fi = new FileInfo(combined);
                    if (!fi.Exists)
                    {
                        MessageBox.Show("drumMapping.xml cannot be found.\n" + combined + "\nSelect location", "EPSS Editor");
                        if (openDrumMappingsFileDialog.ShowDialog() == DialogResult.OK)
                        {
                            combined = openDrumMappingsFileDialog.FileName;
                        }
                    }
                    data.initialize(combined);

                    updateDialog();

                    saveProjectSettings();
                }
            }
            defaultMidiMapRadioButton.Checked = true;
            mappingModeMidiRadioButton.Checked = true;
            setMidiChannel(1);
            updateTotalSize();

        }


        private void updateMappingMode()
        {
            bool midiMappingMode = mappingModeMidiRadioButton.Checked;
            if (!midiMappingMode)
            {
                defaultMidiMapRadioButton.Checked = true;
            }
            GmPercMidiMappingRadioButton.Enabled = midiMappingMode;

            if (midiMappingMode)
            {
                int ch = midiChTrackBar.Value;
                drumsComboBox1.Enabled = ch == 10 ? true : false;

                midiChTrackBar.Maximum = 16;
            } else
            {
                drumsComboBox1.Enabled = midiMappingMode;
                midiChTrackBar.Maximum = 128;
            }


            MultiSampleRadioButton.Enabled = midiMappingMode;
            CustomSampleRadioButton.Enabled = midiMappingMode;
            midiToneLabel.Enabled = midiMappingMode;
            midiToneTextBox.Enabled = midiMappingMode;
            custMidiToneFromTextBox.Enabled = midiMappingMode;
            custMidiToneLabel.Enabled = midiMappingMode;
            custMidiToneToTextBox.Enabled = midiMappingMode;

        }


        private void updateDialog()
        {
            updateDrumSettings();
            updateSoundListBox();
            updateSpiSoundListBox();
            updatePreview();
            updateMappingMode();
        }


        private void updatePreview()
        {
            if (initialize > 0)
            {
                previewComboBox.SelectedIndex = data.previewSelected;
            }
        }


        private bool loadProjectSettings(string file)
        {
            bool result = true;
            try
            {
                XmlSerializer ser = new XmlSerializer(typeof(EPSSEditorData));
                using (FileStream fs = new FileStream(file, FileMode.Open))
                {
                    data = (EPSSEditorData)ser.Deserialize(fs);

                    data.fixOldVersions();



                }
                updateDialog();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                MessageBox.Show("Loading project settings error:\n" + ex.Message);
                result = false;
            }
            return result;
        }




        private void saveProjectSettings()
        {
            if (dataNeedsSaving)
            {
                string file = Properties.Settings.Default.ProjectFile;
                if (file != "")
                {

                    XmlSerializer ser = new XmlSerializer(typeof(EPSSEditorData));
                    using (FileStream fs = new FileStream(file, FileMode.Create))
                    {
                        ser.Serialize(fs, data);
                    }
                    dataNeedsSaving = false;
                }
            }
        }


        private bool WarnAndConfirmDir(string dir, string message)
        {
            bool result = true;

            
            if (Directory.Exists(dir))
            {
                if (MessageBox.Show(message, "Confirm", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    Directory.Delete(dir, true);
                }
                else
                {
                    result = false;
                }
            }

            if (result)
            {
                Directory.CreateDirectory(dir);
            }


            return result;
        }


        private bool loadSpiFile(string file, ref string errorMessage)
        {
            bool result = false;
            EPSSSpiLoader loader = new EPSSSpiLoader();
            Uri url = new Uri(file);
            EPSSSpi spi = loader.Load(url, ref errorMessage);
            if (spi != null)
            {
                // warn user, if we have spi sounds, these will be cleared
                string patchName = spi.ext.i_pname;
                if (String.IsNullOrEmpty(patchName))
                {
                    patchName = Path.GetFileName(file);
                }                                

                string path = Path.GetDirectoryName(Properties.Settings.Default.ProjectFile); // Sample create dir
                path += "\\" + spi.ext.i_pname;
                
                //if (doConversion)
                if (WarnAndConfirmDir(path, "Directory for conversion of SPI already exists.\nDo you want to delete it?"))
                {

                    result = data.LoadSpiFile(ref spi, path, ref errorMessage);
                    if (result)
                    {
                        updateDialog();
                        dataNeedsSaving = true;
                        saveProjectSettings();
                    }
                }
            }
            return result;
        }



        private void updateDrumSettings()
        {
            foreach (Mapping m in data.drumMappings.mappings)
            {
                drumsComboBox1.Items.Add(m.dropDownDescription());
            }
            drumsComboBox1.SelectedIndex = 0;
        }


        private void updateSoundListBox()
        {
            soundListBox.Items.Clear();
            foreach (Sound s in data.sounds)
            {
                soundListBox.Items.Add(s.name());
            }
        }


        ListViewItem SelectedListViewItem(Point lwPos)
        {
            ListViewHitTestInfo info = spiSoundListView.HitTest(lwPos.X, lwPos.Y);
            ListViewItem item = info.Item;

            return item;
        }


        // Only with shift key pressed!
        void SpiSoundListViewScrollListener_ControlScrolled(object sender, EventArgs e, int delta, Point pos)
        {
            if ((Control.ModifierKeys & Keys.Shift) != Keys.None)
            {
                Point lwPos = spiSoundListView.PointToClient(pos);
                ListViewItem item = SelectedListViewItem(lwPos);
                if (item != null)
                {
                    SpiSound snd = data.spiSounds[item.Index];
                    if (delta > 0 && snd.transpose < 127) snd.transpose++;
                    else if (delta < 0 && snd.transpose > -128) snd.transpose--;

                    updateListViewItemValue(snd, item, 7);
                }
            }
        }


        private void spiSoundListView_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Middle && (Control.ModifierKeys & Keys.Shift) != Keys.None)
            {

                Point pos = e.Location;
                ListViewItem item = SelectedListViewItem(pos);
                if (item != null)
                {
                    SpiSound snd = data.spiSounds[item.Index];
                    snd.transpose = 0;
                    updateListViewItemValue(snd, item, 7);

                    Console.WriteLine($"Item: {item}");
                }

            }

        }

        private void updateListViewItemValue(SpiSound snd, ListViewItem item, int columnIndex)
        {
            if (columnIndex == 7)
            {
                ListViewItem.ListViewSubItem subItem = item.SubItems[7];
                subItem.Text = snd.transposeString();
            }
        }

        private void updateSpiSoundListBox()
        {
            spiSoundListView.Clear();
            spiSoundListView.Columns.Add("Id", 40, HorizontalAlignment.Left);
            spiSoundListView.Columns.Add("MIDI", 35, HorizontalAlignment.Right);
            spiSoundListView.Columns.Add("Note", 55, HorizontalAlignment.Right);
            spiSoundListView.Columns.Add("Program", 55, HorizontalAlignment.Right);

            spiSoundListView.Columns.Add("Sound", 165, HorizontalAlignment.Left);
            spiSoundListView.Columns.Add("#", 25, HorizontalAlignment.Right);
            spiSoundListView.Columns.Add("Size", 55, HorizontalAlignment.Left);
            spiSoundListView.Columns.Add("Transpose", 55, HorizontalAlignment.Left);
            spiSoundListView.View = View.Details;

            spiSoundListView.FullRowSelect = true;
            int i = 0;
            foreach (SpiSound s in data.spiSounds)
            {
                //ListViewItem item = new ListViewItem(i++.ToString());
                ListViewItem item = new ListViewItem(i++.ToString());
                //item.Tag = i;

                if (s.midiChannel <= 16)
                {
                    item.SubItems.Add(s.midiChannel.ToString());
                }
                else
                {
                    item.SubItems.Add("-");
                }
                
                if (s.startNote < 128 && s.endNote < 128)
                {
                    item.SubItems.Add(s.startNote.ToString() + "-" + s.endNote.ToString());
                }
                else
                {
                    item.SubItems.Add(s.midiNote.ToString());
                }

                if (s.programNumber < 128)
                {
                    item.SubItems.Add(s.programNumber.ToString());
                }
                else
                {
                    item.SubItems.Add("-");
                }

                item.SubItems.Add(s.name());
                int nr = data.getSoundNumberFromGuid(s.soundId);
                item.SubItems.Add(nr.ToString());

                item.SubItems.Add(Ext.ToPrettySize(s.preLength(ref data), 2));
                item.SubItems.Add(s.transposeString());

                spiSoundListView.Items.Add(item);
            }

            bool spiSaveEnabled = data.spiSounds.Count > 0;
            var mi = menuStrip1.Items.Find("saveSPIToolStripMenuItem", true);
            foreach (var item in mi)
            {
                item.Enabled= spiSaveEnabled;
            }


            Sound snd = getSoundAtSelectedIndex();
            if (snd != null)
            {
                List<SpiSound> spiSounds = data.getSpiSoundsFromSound(ref snd);
                deleteSoundButton.Enabled = spiSounds.Count == 0;

            }

            spiNameTextBox.Text = data.spiName;
            spiInfoTextBox.Text = data.spiDescription;
            omniPatchCheckBox.Checked = data.omni;
        }


        private void updateTotalSize()
        {
            if (data != null)
            {
                EPSSSpiCreator creator = new EPSSSpiCreator(spiVersion());
                long sz = creator.length(ref data);
                totalSizeTextBox.Text = Ext.ToPrettySize(sz, 2);

                int v = (int)(sz / 1024);
                totalSizeProgressBar.Value = Math.Min(14000, v); // Only show up to max 14MB but we support unlimited size..
            }
        }


        private void setMidiChannel(int ch)
        {
            midiChTextBox.Text = ch.ToString();
            midiChTrackBar.Value = ch;

            updateMappingMode();

        }


        private void deleteSelectedSound()
        {
            var indices = soundListBox.SelectedIndices;
            bool anySoundsReferSPI = false;
            foreach (int index in indices)
            {
                if (soundRefersToSPISound(index))
                {
                    anySoundsReferSPI = true;
                    break;
                }
            }


            if (anySoundsReferSPI)
            {
                MessageBox.Show("The sound still refers to SPI sounds.\nPlease remove them first.");
            }
            else
            {
                int removed = 0;
                int idx = 0;
                foreach (int index in indices)
                {
                    idx = index - removed;
                    data.sounds.RemoveAt(idx);
                    removed++;
                }


                updateSoundListBox();
                int itemsLeft = soundListBox.Items.Count;
                if (itemsLeft > 0)
                {
                    if (idx >= itemsLeft)
                    {
                        idx = itemsLeft - 1;
                    }
                    soundListBox.SelectedIndex = idx;
                    useInSpiButton.Enabled = true;
                }
                else
                {
                    useInSpiButton.Enabled = false;
                }
                dataNeedsSaving = true;
                saveProjectSettings();
            }
        }


        private bool soundRefersToSPISound(int idx)
        {

            Sound snd = data.sounds[idx];
            List<SpiSound> spiSounds = data.getSpiSoundsFromSound(ref snd);
            return spiSounds.Count > 0;
        }


        private void deleteSelectedSpiSound()
        {
            List<int> idxRemoved = new List<int>();
            foreach (ListViewItem item in spiSoundListView.SelectedItems)
            {
                idxRemoved.Add(item.Index);
            }

            if (idxRemoved.Count > 0)
            {
                int removed = 0;
                foreach(int index in idxRemoved)
                {
                    data.spiSounds.RemoveAt(index - removed);
                    removed++;
                }
                dataNeedsSaving = true;
                updateSpiSoundListBox();
                saveProjectSettings();
                updateTotalSize();
            }

        }


        private Sound getSoundAtSelectedIndex()
        {
            int idx = soundListBox.SelectedIndex;
            if (idx >= 0)
            {
                Sound[] snds = data.sounds.ToArray();
                return snds[idx];
            }
            return null;
        }


        private List<Sound> getSelectedSounds()
        {
            ListBox.SelectedIndexCollection soundIndices = soundListBox.SelectedIndices;

            List<Sound> sounds = new List<Sound>();
            Sound[] snds = data.sounds.ToArray();
            foreach (int index in soundIndices)
            {
                sounds.Add(snds[index]);
            }

            return sounds;
        }


        private void playSelectedSound()
        {
            try
            {
                Sound snd = getSoundAtSelectedIndex();
                if (snd != null)
                {
                    FileStream wav = File.OpenRead(snd.path);
                    wav.Seek(0, SeekOrigin.Begin);

                    WaveStream ws = new WaveFileReader(wav);
                    ws = WaveFormatConversionStream.CreatePcmStream(ws);

                    WaveOutEvent output = new WaveOutEvent();
                    output.Init(ws);
                    output.Play();


                }
            } catch (Exception ex)
            {
                MessageBox.Show("Cannot play sound:"+ex.Message);
            }
        }


        private void playConvertedSound()
        {

         
            int selected = -1;
            foreach (ListViewItem item in spiSoundListView.SelectedItems)
            {
                selected = item.Index;
                break;
            }

            if (selected >= 0)
            {
                SpiSound snd = data.spiSounds[selected];

                MemoryStream ms = snd.getWaveStream(ref data, frequencyFromCompressionTrackBar(compressionTrackBar.Value), AtariConstants.SampleBits, AtariConstants.SampleChannels);
                if (ms != null) {
                    ms.Position = 0;
                    using (WaveStream blockAlignedStream =
                        new BlockAlignReductionStream(
                            WaveFormatConversionStream.CreatePcmStream(
                                new WaveFileReader(ms))))
                    {
                        using (WaveOut waveOut = new WaveOut(WaveCallbackInfo.FunctionCallback()))
                        {
                            waveOut.Init(blockAlignedStream);
                            waveOut.Play();
                            while (waveOut.PlaybackState == PlaybackState.Playing)
                            {
                                System.Threading.Thread.Sleep(100);
                            }
                        }
                    }
                }

                /*
                string outFile = data.convertSoundFileName();

                if (snd.convertSound(ref data, outFile, frequencyFromCompressionTrackBar(compressionTrackBar.Value), AtariConstants.SampleBits, AtariConstants.SampleChannels))
                {
                    FileStream wav = File.OpenRead(outFile);
                    wav.Seek(0, SeekOrigin.Begin);

                    WaveStream ws = new WaveFileReader(wav);
                    ws = WaveFormatConversionStream.CreatePcmStream(ws);

                    WaveOutEvent output = new WaveOutEvent();
                    output.Init(ws);
                    output.Play();

                    wav.Close();
                    ws.Close();
                } else
                {
                    System.Windows.Forms.MessageBox.Show("Sound could not be converted!", "EPSS Editor");
                }
                */

                /*
                Sound sound = data.getSoundFromSoundId(snd.soundId);
                if (sound != null)
                {
                    string path = sound.path;

                    string outFile = Path.GetTempFileName();

                    // TODO Normalize as well...
                    // TODO use this principle when converting spi sounds instead of manually do it


                    using (var reader = new WaveFileReader(path))
                    {
                        var newFormat = new WaveFormat(frequencyFromCompressionTrackBar(compressionTrackBar.Value), AtariConstants.SampleBits, AtariConstants.SampleChannels);
                        using (var conversionStream = new WaveFormatConversionStream(newFormat, reader))
                        {
                            WaveFileWriter.CreateWaveFile(outFile, conversionStream);
                        }
                    }


                    FileStream wav = File.OpenRead(outFile);
                    wav.Seek(0, SeekOrigin.Begin);

                    WaveStream ws = new WaveFileReader(wav);
                    ws = WaveFormatConversionStream.CreatePcmStream(ws);

                    WaveOutEvent output = new WaveOutEvent();
                    output.Init(ws);
                    output.Play();

                    //TODO delete tmp file after it has been finished playing.. how to determine? Alternatively use a fixed path every time to avoid filling up with tmp files...
                    //File.Delete(outFile);

                }
                */
            }
        }


        private void updateAfterSoundChange(ref Sound snd, int toFreq)
        {
            callbacks = false;
            int numberOfDecimals = 2;
            sizeTextBox.Text = Ext.ToPrettySize(snd.length, numberOfDecimals);
            channelsTextBox.Text = snd.channels.ToString() + " Channels";
            freqTextBox.Text = snd.samplesPerSecond.ToString() + " Hz";
            bitsTextBox.Text = snd.bitsPerSample.ToString() + " Bit";

            int toBit = AtariConstants.SampleBits;

            snd.updateConversionParameters(toBit, toFreq);
            conversionTextBox.Text = snd.parameters().description();

            long lengthAfter = snd.parameters().sizeAfterConversion(ref snd);

            soundSizeAfterTextBox.Text = Ext.ToPrettySize(lengthAfter, numberOfDecimals);

            updateShowCompressionProgressBar(snd.length, lengthAfter);

            bool normalized = snd.parameters().normalize.normalize;
            normalizeCheckBox.Checked = normalized;
            if (normalized)
            {
                normalizeTextBox.Text = snd.parameters().normalize.normalizePercentage + "%";
                normalizeTrackBar.Value = snd.parameters().normalize.normalizePercentage;
            }
            else
            {
                normalizeTextBox.Text = "";
                normalizeTrackBar.Value = 100;
            }
            normalizeTrackBar.Enabled = snd.parameters().normalize.normalize;

            callbacks = true;
        }


        private void updateShowCompressionProgressBar(long lengthBefore, long lengthAfter)
        {
            int v = (int)(((double)lengthAfter / (double)lengthBefore) * 100);
            showCompProgressBar.Value = v;
        }


        private int frequencyFromCompressionTrackBar(int v)
        {
            if (v == 0) return AtariConstants.SampleFreq50k;
            else if (v == 1) return AtariConstants.SampleFreq25k;
            else if (v == 2) return AtariConstants.SampleFreq12k;
            else if (v == 3) return AtariConstants.SampleFreq6k;

            return AtariConstants.SampleFreq25k;
        }

        private int compressionTrackBarValueFromFrequency(int f)
        {
            if (f == AtariConstants.SampleFreq50k) return 0;
            else if (f == AtariConstants.SampleFreq25k) return 1;
            else if (f == AtariConstants.SampleFreq12k) return 2;
            else if (f == AtariConstants.SampleFreq6k) return 3;

            return 2;
        }


        private int currentMidiChannel()
        {
            return midiChTrackBar.Value;
        }


        private byte percussionNote()
        {
            int i = 0;
            foreach (Mapping m in data.drumMappings.mappings)
            {
                int selected = drumsComboBox1.SelectedIndex;
                if (i == selected) return m.midiNote;
                i++;
            }
            return 0;
        }


        private void loadSound()
        {
            loadSoundFileDialog.InitialDirectory = Path.GetDirectoryName(data.soundFileName);

            if (loadSoundFileDialog.ShowDialog() == DialogResult.OK)
            {
                string anyFile = "";
                foreach (string name in loadSoundFileDialog.FileNames)
                {
                    Sound s = new Sound(name);
                    data.sounds.Add(s);
                    anyFile = name;
                }
                updateSoundListBox();

                data.soundFileName = anyFile;
                dataNeedsSaving = true;
                saveProjectSettings();
            }
        }        


        private bool loadProjectSettingsFileDialog()
        {
            bool result = false;
            string s = Properties.Settings.Default.ProjectFile;
            if (s == null || s == "")
            {
                s = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                s = Path.Combine(s, "EPSS Projects", "default.epf");
            }


            string projFile = Path.GetDirectoryName(s);
            loadProjectFileDialog.InitialDirectory = projFile;
            string fileName = Path.GetFileName(s);
            loadProjectFileDialog.FileName = fileName;
            if (loadProjectFileDialog.ShowDialog() == DialogResult.OK)
            {
                string file = loadProjectFileDialog.FileName;
                Properties.Settings.Default.ProjectFile = file;
                Properties.Settings.Default.Save();
                loadProjectSettings(file);
                result = true;
            }

            return result;

        }


        private bool doLoadSpiFileDialog(ref string errorMessage)
        {
            bool result = false;
            string s = Properties.Settings.Default.SpiFile;
            if (s == null || s == "")
            {
                s = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                s = Path.Combine(s, "EPSS Projects", "default.spi");
            } else if (Path.GetExtension(s).ToLower() != "spi")
            {
                s = Path.ChangeExtension(s, "spi");
            }
                        
            string spiFile = Path.GetDirectoryName(s);
            loadSpiFileDialog.InitialDirectory = spiFile;
            string fileName = Path.GetFileName(s);
            loadSpiFileDialog.FileName = fileName;
            if (loadSpiFileDialog.ShowDialog() == DialogResult.OK)
            {
                string file = loadSpiFileDialog.FileName;
                Properties.Settings.Default.SpiFile = file;
                Properties.Settings.Default.Save();
                result = loadSpiFile(file, ref errorMessage);
                if (result)
                {

                }
            }

            return result;

        }


        private bool doLoadSfzFileDialog(ref string errorMessage)
        {
            bool result = false;
            string s = Properties.Settings.Default.SfzFile;
            if (s == null || s == "")
            {
                s = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                s = Path.Combine(s, "EPSS Projects", "default.sfz");
            }
            else if (Path.GetExtension(s).ToLower() != "sfz")
            {
                s = Path.ChangeExtension(s, "sfz");
            }

            string sfzFile = Path.GetDirectoryName(s);
            loadSfzFileDialog.InitialDirectory = sfzFile;
            string fileName = Path.GetFileName(s);
            loadSfzFileDialog.FileName = fileName;
            if (loadSfzFileDialog.ShowDialog() == DialogResult.OK)
            {
                string file = loadSfzFileDialog.FileName;
                Properties.Settings.Default.SfzFile = file;
                Properties.Settings.Default.Save();

                string anyFile = LoadSfzSound(file);
 

                data.soundFileName = anyFile;
                result = true;
            
            }

            return result;
        }


        private void saveProjectSettingsFileDialog()
        {
            string s = Properties.Settings.Default.ProjectFile;
            if (s == null || s == "")
            {
                s = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                s = Path.Combine(s, "EPSS Projects", "default.epf");
            }
            string projFile = Path.GetDirectoryName(s);
            saveProjectFileDialog.InitialDirectory = projFile;
            string fileName = Path.GetFileName(s);
            saveProjectFileDialog.FileName = fileName;
            saveProjectFileDialog.Title = "Choose filename where EPSS project should be saved...";
            if (saveProjectFileDialog.ShowDialog() == DialogResult.OK)
            {
                string file = saveProjectFileDialog.FileName;
                Properties.Settings.Default.ProjectFile = file;
                Properties.Settings.Default.Save();
                saveProjectSettings();
            }
        }


        private void saveSampleWithFileDialog()
        {
            bool anySelected = false;
            foreach (ListViewItem item in spiSoundListView.SelectedItems)
            {
                anySelected = true;
               
                break;

            }


            if (anySelected)
            {

                foreach (ListViewItem item in spiSoundListView.SelectedItems)
                {
                    int selected = item.Index;

                    if (selected >= 0)
                    {

                        saveSampleFileDialog.InitialDirectory = Path.GetDirectoryName(data.soundFileName);
                        if (saveSampleFileDialog.ShowDialog() == DialogResult.OK)
                        {
                            string outFile = saveSampleFileDialog.FileName;
                            SpiSound snd = data.spiSounds[selected];
                            if (snd.convertSound(ref data, outFile, frequencyFromCompressionTrackBar(compressionTrackBar.Value), AtariConstants.SampleBits, AtariConstants.SampleChannels))
                            {
                            }

                        } else
                        {
                            if (MessageBox.Show("Do you want to stop saving sounds?", "Confirm", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                            {
                                break;
                            }
                        }
                    }
                  

                }
            }
        }


        // Control Events


        private void soundListBox_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.Text))
            {
                e.Effect = DragDropEffects.Copy;
            }
            else if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                e.Effect = DragDropEffects.Copy;
            }
            else
            {
                e.Effect = DragDropEffects.None;
            }

        }


        private string addOneSoundDirect(string file, string baseName)
        {
            Sound s = new Sound(file);

            data.sounds.Add(s);
            return file;
        }

     

        private string LoadSfzSound(string filePath)
        {
            //List<string> soundsAdded = new List<string>();
            Dictionary<string, Sound> sounds = new Dictionary<string, Sound>();
            foreach (Sound s in data.sounds)
            {
                sounds.Add(s.path, s);
            }

            string anyFile = "";
            ParseSfz p = new ParseSfz();
            List<SfzBase> bases = p.parse(filePath);
            string basePath = Path.GetDirectoryName(filePath);
            int midiChannel = currentMidiChannel();
            bool skipFirstGroup = true;
            foreach (SfzBase bas in bases)
            {
                var gSection = bas as SfzGenericSection;
                if (gSection != null)
                {
                    if (gSection.header.Contains("group"))
                    {
                        if (!skipFirstGroup)
                        {
                            midiChannel++;
                            if (midiChannel > 16)
                            {
                                System.Windows.Forms.MessageBox.Show("Midi channel over 16. Will skip rest of sfz file.");
                                break;
                            }
                        }
                        skipFirstGroup = false;
                    }
                }


                var tBase = bas as SfzRegionSection;
                if (tBase != null)
                {
                    string fp = tBase.FilePath(basePath);

                    Sound s;
                    if (sounds.ContainsKey(fp))
                    {
                        s = sounds[fp];
                    } else
                    {
                        s = new Sound(fp);
                        s.description = Path.GetFileNameWithoutExtension(fp);
                        data.sounds.Add(s);
                        sounds.Add(fp, s);
                    }
                    


                    //Sound s = new Sound(fp);
                    //s.description = baseName + Path.GetFileNameWithoutExtension(fp);
                    byte loByte;
                    string loKeyS = tBase.variables["lokey"];
                    if (!TryToByte(loKeyS, out loByte))
                    {
                        int v = parseNoteToInt(loKeyS, 0);
                        if (v < 0 || v > 127)
                        {
                            loByte = 128;
                        }
                        else
                        {
                            loByte = (byte)v;
                        }
                    }
                    //s.loKey = loByte;



                    byte hiByte;
                    string hiKeyS = tBase.variables["hikey"];
                    if (!TryToByte(hiKeyS, out hiByte))
                    {
                        int v = parseNoteToInt(hiKeyS, 0);
                        if (v < 0 || v > 127)
                        {
                            hiByte = 128;
                        }
                        else
                        {
                            hiByte = (byte)v;
                        }
                    }
                    //s.hiKey = hiByte;


                    byte kcByte;
                    string kcS = tBase.variables["pitch_keycenter"];
                    if (!TryToByte(kcS, out kcByte))
                    {
                        int v = parseNoteToInt(kcS, 0);
                        if (v < 0 || v > 127)
                        {
                            kcByte = 128;
                        }
                        else
                        {
                            kcByte = (byte)v;
                        }
                    }
                    //s.keyCenter = kcByte;


                    //if (!data.IdenticalSoundExists(s))
                    //{
                    //data.sounds.Add(s);
                    //}
                    anyFile = fp;
                    data.AddSfzSound(ref s, midiChannel, loByte, hiByte, kcByte);

                }
            }

            updateDialog();
            dataNeedsSaving = true;
            saveProjectSettings();

            return anyFile;
        }


        private void soundListBox_DragDrop(object sender, DragEventArgs e)
        {
            string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
            string anyFile = "";


            foreach (var file in files)
            {
                string filePath = file;
                string ext = Path.GetExtension(filePath).ToUpper();


                if (ext == ".SFZ")
                {
                    anyFile = LoadSfzSound(filePath);
                }
                else
                {
                    string baseName = Path.GetFileNameWithoutExtension(filePath);
                    baseName = baseName.Substring(0, Math.Min(baseName.Length - 1, 13));
                    anyFile = addOneSoundDirect(filePath, baseName);
                }
            }


            /*
            if (files.Length == 1)
            {
                string filePath = files[0];
                string ext = Path.GetExtension(filePath).ToUpper();

                string baseName = Path.GetFileNameWithoutExtension(filePath);
                baseName = baseName.Substring(0, Math.Min(baseName.Length-1, 13));
                if (ext == ".SFZ")
                {
                    
                }
                else
                {
                    Sound s = new Sound(filePath);

                    data.sounds.Add(s);
                    anyFile = filePath;
                }
            }
            else
            {

                foreach (string filePath in files)
                {

                    Sound s = new Sound(filePath);

                    data.sounds.Add(s);
                    anyFile = filePath;
                }

            }
            */
            updateSoundListBox();
            data.soundFileName = anyFile;
            dataNeedsSaving = true;
            saveProjectSettings();
        }



        private void soundListBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!deletePressed)
            {
                playSelectedSound();
            }
        }


        private void soundListBox_KeyDown(object sender, KeyEventArgs e)
        {
            deletePressed = false;
            if (e.KeyCode == Keys.Delete)
            {
                deleteSelectedSound();
                deletePressed = true;
            }
        }


        private void soundListBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            Sound snd = getSoundAtSelectedIndex();
            if (snd != null)
            {
                int toFreq;
                if (snd.parameters().hasParameters())
                {
                    toFreq = snd.parameters().freq.toFreq;
                }
                else
                {
                    toFreq = Properties.Settings.Default.SampleConversionDestFreq;
                }

                updateAfterSoundChange(ref snd, toFreq);
                compressionTrackBar.Value = compressionTrackBarValueFromFrequency(toFreq);

                useInSpiButton.Enabled = true;

                List<SpiSound> spiSounds = data.getSpiSoundsFromSound(ref snd);
                deleteSoundButton.Enabled = spiSounds.Count == 0;



            }
            else
            {
                useInSpiButton.Enabled = false;
            }
        }


        private void loadSoundButton_Click(object sender, EventArgs e)
        {
            loadSound();
        }



        private void deleteSoundButton_Click(object sender, EventArgs e)
        {
            deleteSelectedSound();
        }


        
        private void playButton_KeyDown(object sender, KeyEventArgs e)
        {
            playSelectedSound();
        }


        private void playButton_KeyUp(object sender, KeyEventArgs e)
        {
            //player.Stop();
        }



        private void compressionTrackBar_Scroll(object sender, EventArgs e)
        {
            Sound snd = getSoundAtSelectedIndex();
            if (snd != null)
            {
                int toFreq = frequencyFromCompressionTrackBar(compressionTrackBar.Value);
                updateAfterSoundChange(ref snd, toFreq);

            }
        }


        private void midiChTrackBar_Scroll(object sender, EventArgs e)
        {
            int ch = midiChTrackBar.Value;
            setMidiChannel(ch);

            if (mappingModeMidiRadioButton.Checked)
            {
                if (ch == 10)
                {
                    GmPercMidiMappingRadioButton.Checked = true;
                }
                else
                {

                    defaultMidiMapRadioButton.Checked = true;
                }
            } else
            {

            }
            
        }

        private void GmPercMidiMappingRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            setMidiChannel(10);
        }


        private void defaultMidiMapRadioButton_CheckedChanged(object sender, EventArgs e)
        { 
            //int ch = data.getNextFreeMidiChannel();
            //if (ch > 0) setMidiChannel(ch);
        }

        public static bool TryToByte(object value, out byte result)
        {
            if (value == null)
            {
                result = 0;
                return false;
            }

            return byte.TryParse(value.ToString(), out result);
        }

        private int parseNoteToInt(string midiNote, int octaveOffset)
        {
            midiNote = midiNote.ToUpper();
            int v = 0;
            Dictionary<string, byte> notes = new Dictionary<string, byte>()
            {
                { "C", 0 }, { "C#", 1}, { "D", 2 }, { "D#", 3 }, { "E", 4 },  { "F", 5 }, { "F#", 6 },
                { "G", 7 }, { "G#", 8 }, { "A", 9 }, { "A#", 10 }, { "B", 11 },
                { "H", 11 }, { "DB", 1 }, { "EB", 3 }, { "GB", 6 }, { "AB", 8 }, {"BB", 10}
            };


            try
            {
                int i = 0;
                foreach (var c in midiNote)
                {
                    if ((c >= '0' && c <= '9') || c == '-')
                    {
                        string n = midiNote.Substring(0, i);
                        short oct = (short)((Convert.ToInt16(midiNote.Substring(i, midiNote.Length - i)) + octaveOffset) * 12);
                        v = Convert.ToInt32(oct);
                        v += notes[n];
                        break;
                    }
                    i++;
                }
                return v;
            }
            catch (Exception ex)
            {
                return -1;
            }
        }


        private byte parseMidiTone(string s)
        {
            byte note = 128;
            if (!TryToByte(s, out note))
            {
                int v = parseNoteToInt(s, 2);
                if (v < 0 || v > 127)
                {
                    System.Windows.Forms.MessageBox.Show("Unsupported MIDI Note!");
                }
                else
                {
                    note = (byte)v;
                }
            }
            return note;
        }


        private void useInSpiButton_Click(object sender, EventArgs e)
        {
            List<Sound> sounds = getSelectedSounds();

            if (mappingModeMidiRadioButton.Checked)
            {
                if (sounds.Count >= 1)
                {
                    bool mappingOk = false;
                    byte startNote = 128;
                    if (MultiSampleRadioButton.Checked)
                    {
                        startNote = parseMidiTone(midiToneTextBox.Text);
                        mappingOk = true;
                    }

                    else if (GmPercMidiMappingRadioButton.Checked)
                    {
                        mappingOk = true;
                        startNote = percussionNote();
                    }
                    else if (defaultMidiMapRadioButton.Checked)
                    {
                        mappingOk = true;
                        startNote = 128;
                    }

                    if (mappingOk)
                    {
                        foreach (Sound sound in sounds)
                        {
                            Sound s = sound;
                            SpiSound spiSnd = new SpiSound(ref s);

                            if (defaultMidiMapRadioButton.Checked)
                            {
                                spiSnd.startNote = s.loKey;
                                spiSnd.endNote = s.hiKey;
                                spiSnd.midiNote = (byte)(84 - (s.keyCenter - s.loKey));
                            }
                            else
                            {
                                spiSnd.midiNote = startNote++;
                            }


                            int midiChannel = currentMidiChannel();
                            spiSnd.midiChannel = (byte)midiChannel;

                            int toFreq;
                            if (s.parameters().hasParameters())
                            {
                                toFreq = s.parameters().freq.toFreq;
                            }
                            else
                            {
                                toFreq = Properties.Settings.Default.SampleConversionDestFreq;
                            }

                            updateAfterSoundChange(ref s, toFreq);
                            compressionTrackBar.Value = compressionTrackBarValueFromFrequency(toFreq);

                            if (!defaultMidiMapRadioButton.Checked)
                            {
                                data.removeSpiSound(spiSnd.midiChannel, spiSnd.midiNote);
                            }

                            data.spiSounds.Add(spiSnd);

                        }
                        updateSpiSoundListBox();
                        dataNeedsSaving = true;
                        saveProjectSettings();
                        updateTotalSize();

                    }
                    else
                    {
                        System.Windows.Forms.MessageBox.Show("Need to have multi sample mapping or GM Percussion mapping selected!");
                    }


                }
                else
                {
                    Sound s = getSoundAtSelectedIndex();
                    if (s != null)
                    {
                        if (CustomSampleRadioButton.Checked)
                        {
                            byte startNote = parseMidiTone(custMidiToneFromTextBox.Text);
                            byte endNote = parseMidiTone(custMidiToneToTextBox.Text);
                            if (startNote < 128 && endNote < 128 && startNote < endNote)
                            {
                                int midiChannel = currentMidiChannel();
                                SpiSound spiSnd = new SpiSound(ref s);
                                spiSnd.midiChannel = (byte)midiChannel;
                                spiSnd.midiNote = 84;
                                spiSnd.startNote = startNote;
                                spiSnd.endNote = endNote;
                                data.spiSounds.Add(spiSnd);

                                updateSpiSoundListBox();
                                dataNeedsSaving = true;
                                saveProjectSettings();
                                updateTotalSize();
                            }
                            else
                            {
                                System.Windows.Forms.MessageBox.Show("Incorrect from and to note values!");
                            }
                        }
                        else
                        {
                            bool addOk = true;
                            SpiSound spiSnd = new SpiSound(ref s);

                            int midiChannel = currentMidiChannel();
                            spiSnd.midiChannel = (byte)midiChannel;

                            if (midiChannel == 10)
                            {
                                spiSnd.midiNote = percussionNote(); // 1-128
                            }

                            if (midiChannel != 10)
                            {
                                if (data.isMidiChannelOccupied(midiChannel))
                                {
                                    addOk = false;
                                    System.Windows.Forms.MessageBox.Show("MIDI channel " + midiChannel.ToString() + " already occupied!");
                                }
                            }
                            else if (midiChannel == 10)
                            {
                                if (data.isDrumSoundOccupied(spiSnd.midiNote))
                                {
                                    addOk = false;
                                    System.Windows.Forms.MessageBox.Show("Drum sound " + spiSnd.midiNote.ToString() + " already occupied!");
                                }
                            }


                            if (addOk)
                            {
                                data.spiSounds.Add(spiSnd);
                                updateSpiSoundListBox();


                                if (defaultMidiMapRadioButton.Checked)
                                {
                                    int ch = data.getNextFreeMidiChannel();
                                    if (ch > 0) setMidiChannel(ch);
                                }
                                else
                                {
                                    int idx = drumsComboBox1.SelectedIndex;
                                    if (idx < drumsComboBox1.Items.Count - 1)
                                    {
                                        drumsComboBox1.SelectedIndex = idx + 1;
                                    }
                                }

                                dataNeedsSaving = true;
                                saveProjectSettings();
                                updateTotalSize();

                            }
                        }
                    }
                }
            }
            else
            {
                if (sounds.Count > 1)
                {
                    foreach (Sound sound in sounds)
                    {
                        Sound s = sound;
                        SpiSound spiSnd = new SpiSound(ref s);

                        spiSnd.midiChannel = 128;
                        spiSnd.startNote = s.loKey;
                        spiSnd.endNote = s.hiKey;
                        spiSnd.midiNote = (byte)(84 - (s.keyCenter - s.loKey));
                        spiSnd.programNumber = (byte)currentMidiChannel();

                        data.spiSounds.Add(spiSnd);
                    }

                    updateSpiSoundListBox();
                    dataNeedsSaving = true;
                    saveProjectSettings();
                    updateTotalSize();
                }
                else if (sounds.Count == 1)
                {
                    Sound s = getSoundAtSelectedIndex();
                    if (s != null)
                    {

                        // TODO Check if program have been used before, remove if it is the case

                        SpiSound spiSnd = new SpiSound(ref s);
                        spiSnd.midiChannel = 128;
                        spiSnd.startNote = s.loKey;
                        spiSnd.endNote = s.hiKey;
                        spiSnd.midiNote = (byte)(84 - (s.keyCenter - s.loKey));
                        spiSnd.programNumber = (byte)currentMidiChannel();


                        data.spiSounds.Add(spiSnd);

                        updateSpiSoundListBox();
                        dataNeedsSaving = true;
                        saveProjectSettings();
                        updateTotalSize();

                    }
                }
            }
        }


        private void deleteSpiSoundButton_Click(object sender, EventArgs e)
        {
            deleteSelectedSpiSound();
        }



        // Menu

        private void newProjectToolStripMenuItem_Click_1(object sender, EventArgs e)
        {
            bool doNew = true;
            if (dataNeedsSaving)
            {
                if (MessageBox.Show("You have unsaved data in current project!\nDo you really want to create new project?", "Confirm", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
                {
                    doNew= false;
                }
            }
            if (doNew) {
                initEpssEditorData(forceNewProject: true);
                dataNeedsSaving = true;
                saveProjectSettings();
            }
        }


        private void saveProjectToolStripMenuItem_Click(object sender, EventArgs e)
        {
            saveProjectSettingsFileDialog();
        }


        private void loadProjectToolStripMenuItem_Click(object sender, EventArgs e)
        {
            bool doLoad = true;
            if (dataNeedsSaving)
            {
                if (MessageBox.Show("You have unsaved data in current project!\nDo you really want to load new project?", "Confirm", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
                {
                    doLoad = false;
                }
            }

            if (doLoad)
            {
                loadProjectSettingsFileDialog();
            }
        }


        private void loadSPIToolStripMenuItem_Click(object sender, EventArgs e)
        {
            bool doLoad = true;
            if (dataNeedsSaving)
            {
                if (MessageBox.Show("You have unsaved data in current project!\nDo you really want to load spi and clear\nall existing data?", "Confirm", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
                {
                    doLoad = false;
                }
            }

            if (doLoad)
            {
                string errorMessage = "";
                bool result = doLoadSpiFileDialog(ref errorMessage);
                if (!result && !String.IsNullOrEmpty(errorMessage))
                {
                    MessageBox.Show("SPI file cannot be loaded:\n" + errorMessage);
                }
            }
        }

        private void importSFZToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string errorMessage = "";
            bool result = doLoadSfzFileDialog(ref errorMessage);
            if (!result && !String.IsNullOrEmpty(errorMessage))
            {
                MessageBox.Show("SFZ file cannot be loaded:\n" + errorMessage);
            }
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Close();
        }


        private void normalizeTrackBar_Scroll(object sender, EventArgs e)
        {
            if (callbacks)
            {
                normalizeTextBox.Text = normalizeTrackBar.Value.ToString() + "%";

                Sound snd = getSoundAtSelectedIndex();
                if (snd != null)
                {

                    snd.updateNormalize(normalizeCheckBox.Checked, normalizeTrackBar.Value);
                }
                else
                {

                }
            }
        }
        

        private void normalizeCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            if (callbacks)
            {
                Sound snd = getSoundAtSelectedIndex();
                if (snd != null)
                {
                    snd.updateNormalize(normalizeCheckBox.Checked, normalizeTrackBar.Value);
                }

                normalizeTrackBar.Enabled = normalizeCheckBox.Checked;
            }
        }


        private int spiVersion()
        {
            return gen2CheckBox.Checked ? 2 : 1;
        }


        private void doSaveSpi()
        {
            List<SpiSound> soundsToSave = data.getSortedSpiSounds();
            if (soundsToSave.Count > 0)
            {
                saveSpiFileDialog.InitialDirectory = Path.GetDirectoryName(data.spiFileName);

                if (saveSpiFileDialog.ShowDialog() == DialogResult.OK)
                {
                    string spiFile = saveSpiFileDialog.FileName;
                    Uri url = new Uri(spiFile);

                    EPSSSpiCreator creator = new EPSSSpiCreator(spiVersion());
                    int sampFreq = AtariConstants.SampleFreq25k;
                    data.omni = omniPatchCheckBox.Checked;
                    EPSSSpi spi = creator.create(ref data, soundsToSave, spiNameTextBox.Text, spiInfoTextBox.Text, sampFreq);

                    if (spi != null)
                    {
                        int result = spi.save(url);
                        if (result == 0)
                        {
                            data.spiFileName = spiFile;
                            dataNeedsSaving = true;
                            saveProjectSettings();
                        }
                        else
                        {
                            System.Windows.Forms.MessageBox.Show("Error occured during save! Patch file might not be valid.");
                        }
                    }
                }

            }
            else
            {
                System.Windows.Forms.MessageBox.Show("No sounds to save!");
            }
        }


        private void saveSpiButton_Click(object sender, EventArgs e)
        {


        }


        private void saveSFZToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string startDir;
            if (data.spiFileName != null)
            {
                startDir = Path.GetDirectoryName(data.spiFileName);
            } else
            {
                startDir = Path.GetDirectoryName(Properties.Settings.Default.ProjectFile);
            }
            saveSfzFileDialog.InitialDirectory = startDir;
           
            if (saveSfzFileDialog.ShowDialog() == DialogResult.OK)
            {
                string sfzFile = saveSfzFileDialog.FileName;
                string name = Path.GetFileNameWithoutExtension(sfzFile);
                string sfzDir = Path.GetDirectoryName(sfzFile);
                string sfzFileName = Path.GetFileName(sfzFile);

                bool doSave = true;
                string[] filesInDir = Directory.GetFiles(sfzDir);
                if (filesInDir.Length > 0)
                {
                    if (System.Windows.Forms.MessageBox.Show("Chosen directory for sfz is not empty.\nAll used samples will be copied here.\nDo you want to continue?", "Confirm", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
                    {
                        doSave = false;
                    }
                }

                if (doSave) {
                    Dictionary<int, List<SfzSplitInfo>> dict = data.ConvertToSfzSplitInfo();
                    SfzConverter c = new SfzConverter();
                    string errorMessage = "";
                    bool result = c.SaveSFZ(ref dict, ref data.sounds, sfzDir, name, sfzFileName, ref errorMessage);
                    if (result)
                    {
                        foreach (var sound in data.sounds)
                        {
                            string path = sfzDir + "\\" + Path.GetFileName(sound.path);
                            if (File.Exists(path))
                            {
                                long oldSize =new System.IO.FileInfo(sound.path).Length;
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

                    } else
                    {
                        MessageBox.Show("Save sfz failed:\n" + errorMessage);
                    }
                }
            }
        }


            private void clearSettingsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string settingsFile = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.PerUserRoamingAndLocal).FilePath;
            if (System.Windows.Forms.MessageBox.Show("Settings files is stored:\n" + settingsFile + "\nDo you want to clear them?", "Confirm", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                Properties.Settings.Default.Reset();
                System.Windows.Forms.MessageBox.Show("Restart application with clear settings.");
            }
        }

        private void spiSoundListView_SelectedIndexChanged(object sender, EventArgs e)
        {
            
        }

        private void spiSoundListView_ItemCheck(object sender, ItemCheckEventArgs e)
        {

        }

        private void spiSoundListView_ItemChecked(object sender, ItemCheckedEventArgs e)
        {
        }


        private void spiSoundListView_ItemSelectionChanged(object sender, ListViewItemSelectionChangedEventArgs e)
        {
            bool enabled = false;
            foreach (ListViewItem item in spiSoundListView.SelectedItems)
            {
                enabled = true;
            }
            deleteSpiSoundButton.Enabled = enabled;
            button1.Enabled = enabled;
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void spiSoundListenButton_Click(object sender, EventArgs e)
        {
            playConvertedSound();

        }

        private void previewComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            data.previewSelected = previewComboBox.SelectedIndex;
            dataNeedsSaving = true;
            saveProjectSettings();
        }

        private void saveSampleButton_Click(object sender, EventArgs e)
        {
            saveSampleWithFileDialog();
        }

        
        /*private void selectAllButton_Click(object sender, EventArgs e)
        {
            List<int> idxRemoved = new List<int>();
            foreach (ListViewItem item in spiSoundListView.Items)
            {
                item.Checked = true;
            }

        }*/


        private void spiNameTextBox_TextChanged(object sender, EventArgs e)
        {
            data.spiName = spiNameTextBox.Text;
            dataNeedsSaving = true;
            saveProjectSettings();
        }

        private void spiInfoTextBox_TextChanged(object sender, EventArgs e)
        {
            data.spiDescription = spiInfoTextBox.Text;
            dataNeedsSaving = true;
            saveProjectSettings();
        }

        private void omniPatchCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            data.omni = omniPatchCheckBox.Checked;
            dataNeedsSaving = true;
            saveProjectSettings();
        }

        private void playButton_Click(object sender, EventArgs e)
        {

            playSelectedSound();
        }


        private void custMidiToneToTextBox_TextChanged(object sender, EventArgs e)
        {

        }

        private void label11_Click(object sender, EventArgs e)
        {

        }

        private void custMidiToneFromTextBox_TextChanged(object sender, EventArgs e)
        {

        }

        private void CustomSampleRadioButton_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void label10_Click(object sender, EventArgs e)
        {

        }

        private void midiToneTextBox_TextChanged(object sender, EventArgs e)
        {

        }

        private void MultiSampleRadioButton_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void midiChTextBox_TextChanged(object sender, EventArgs e)
        {

        }

        private void label21_Click(object sender, EventArgs e)
        {

        }

        private void drumsComboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }



        private void mappingModeMidiRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            if (mappingModeMidiRadioButton.Checked)
            {
                int v = midiChTrackBar.Value;
                if (v > 16)
                {
                    setMidiChannel(1);

                }
            }
            updateMappingMode();

        }

        private void mappingModeProgramRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            if (mappingModeMidiRadioButton.Checked)
            {
                int v = midiChTrackBar.Value;
                if (v > 16)
                {
                    setMidiChannel(1);
            
                }
            }
            updateMappingMode();
        }

        private void spiSoundListView_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            Console.WriteLine(e.X + " " + e.Y);
        }

        private void saveSPIToolStripMenuItem_Click(object sender, EventArgs e)
        {
            doSaveSpi();
        }
    }




    internal class ControlScrollListener : NativeWindow, IDisposable
    {
        public event ControlScrolledEventHandler ControlScrolled;
        public delegate void ControlScrolledEventHandler(object sender, EventArgs e, int delta, Point pos);

        private const uint WM_HSCROLL = 0x114;
        private const uint WM_VSCROLL = 0x115;
        private const uint WM_MOUSEWHEEL = 0x020A;
        private readonly Control _control;

        public ControlScrollListener(Control control)
        {
            _control = control;
            AssignHandle(control.Handle);
        }

        protected bool Disposed { get; set; }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            if (Disposed) return;

            if (disposing)
            {
                // Free other managed objects that implement IDisposable only
            }

            // release any unmanaged objects
            // set the object references to null
            ReleaseHandle();

            Disposed = true;
        }

        protected override void WndProc(ref Message m)
        {
            HandleControlScrollMessages(m);
            base.WndProc(ref m);
        }

        private void HandleControlScrollMessages(Message m)
        {
            //if (m.Msg == WM_HSCROLL | m.Msg == WM_VSCROLL)
            if (m.Msg == WM_MOUSEWHEEL)
            {
                if (ControlScrolled != null)
                {
                    int delta = GET_WHEEL_DELTA_WPARAM(m.WParam);
                    Point pos = new Point(m.LParam.ToInt32());
                    //Console.WriteLine(delta);
                    //Console.WriteLine(pos);
                    ControlScrolled(_control, new EventArgs(), delta, pos);
                }
            }
        }

        internal static ushort HIWORD(IntPtr dwValue)
        {
            return (ushort)((((long)dwValue) >> 0x10) & 0xffff);
        }
        internal static ushort HIWORD(uint dwValue)
        {
            return (ushort)(dwValue >> 0x10);
        }

        internal static int GET_WHEEL_DELTA_WPARAM(uint wParam)
        {
            return (short)HIWORD(wParam);
        }

        internal static int GET_WHEEL_DELTA_WPARAM(IntPtr wParam)
        {
            return (short)HIWORD(wParam);
        }
    }
}
