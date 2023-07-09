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
using System.Web;

namespace EPSSEditor
{
    public partial class Form1 : Form
    {

        public EPSSEditorData data;
        private bool deletePressed = false;
        private bool ctrlAPressed;
        private bool callbacks = true;
        private int initialize;
        private bool dataNeedsSaving;
        private ControlScrollListener _processListViewScrollListener;
        private AudioPlaybackEngine audio = null;

        public Form1()
        {
            InitializeComponent();
            initialize = 0;
            dataNeedsSaving = false;

            _processListViewScrollListener = new ControlScrollListener(spiSoundListView);
            _processListViewScrollListener.ControlScrolled += SpiSoundListViewScrollListener_ControlScrolled;
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


        private void initEpssEditorData(bool forceNewProject = false)
        {
            string projectFile = Properties.Settings.Default.ProjectFile;
            bool initNewProjectFile = false;
            if (forceNewProject || projectFile == null | projectFile == "")
            {
                initNewProjectFile = true;


            }
            else
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


        private void exit()
        {
            Properties.Settings.Default.WinState = this.WindowState;
            if (this.WindowState == FormWindowState.Normal)
            {
                // save location and size if the state is normal
                //Properties.Settings.Default.WinLocation = this.Location;
                Properties.Settings.Default.WinSize = this.Size;
            }
            else
            {
                // save the RestoreBounds if the form is minimized or maximized!
                //Properties.Settings.Default.F1Location = this.RestoreBounds.Location;
                Properties.Settings.Default.WinSize = this.RestoreBounds.Size;
            }


            Properties.Settings.Default.Save();
            saveProjectSettings();
            ExitAudioSystem();
        }


        private void InitAudioSystem()
        {
            if (audio == null) audio = new AudioPlaybackEngine();
            audio.Start();
        }


        private void ExitAudioSystem()
        {
            if (audio != null) {
                audio.Stop();
                audio.Dispose();
                audio = null;
            }
        }

        private void updateDialog()
        {
            string file = Properties.Settings.Default.ProjectFile;
            this.Text = "EPSS Editor v" + GetRunningVersion().ToString() + "   -   Project: " + file;

            updateDrumSettings();
            updateSoundListBox();
            updateSpiSoundListBox();
            updatePreview();
            updateMappingMode();
            UpdateSoundDialog();
        }


        private void UpdateSoundDialog()
        {
            groupBox5.Enabled = soundListBox.SelectedItems.Count == 1;
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
                groupBox11.Text = "MIDI Channel";
            }
            else
            {
                drumsComboBox1.Enabled = midiMappingMode;
                midiChTrackBar.Maximum = 128;
                groupBox11.Text = "Program number";
            }


            MultiSampleRadioButton.Enabled = midiMappingMode;
            CustomSampleRadioButton.Enabled = midiMappingMode;
            midiToneLabel.Enabled = midiMappingMode;
            midiToneTextBox.Enabled = midiMappingMode;
            custMidiToneFromTextBox.Enabled = midiMappingMode;
            custMidiToneLabel.Enabled = midiMappingMode;
            custMidiToneToTextBox.Enabled = midiMappingMode;

        }


        private void updatePreview()
        {
            if (initialize > 0)
            {
                previewComboBox.SelectedIndex = data.previewSelected;
            }
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
            spiSoundListView.Columns.Add("Vvfe", 35, HorizontalAlignment.Left);
            spiSoundListView.Columns.Add("D", 20, HorizontalAlignment.Left);
            spiSoundListView.Columns.Add("V", 20, HorizontalAlignment.Left);
            spiSoundListView.Columns.Add("T", 20, HorizontalAlignment.Left);
            spiSoundListView.Columns.Add("M", 20, HorizontalAlignment.Left);
            spiSoundListView.Columns.Add("A", 20, HorizontalAlignment.Left);
            spiSoundListView.Columns.Add("S", 20, HorizontalAlignment.Left);
            spiSoundListView.Columns.Add("P", 20, HorizontalAlignment.Left);
            spiSoundListView.Columns.Add("F", 20, HorizontalAlignment.Left);
            spiSoundListView.Columns.Add("Start", 65, HorizontalAlignment.Left);
            spiSoundListView.Columns.Add("Loop", 65, HorizontalAlignment.Left);
            spiSoundListView.Columns.Add("End", 65, HorizontalAlignment.Left);
            spiSoundListView.Columns.Add("L", 20, HorizontalAlignment.Left);
            spiSoundListView.Columns.Add("Vol", 40, HorizontalAlignment.Left);
            spiSoundListView.Columns.Add("Sub", 40, HorizontalAlignment.Left);
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
                item.SubItems.Add(s.VvfeString());

                EPSSSpi_s_gr_frek props = new EPSSSpi_s_gr_frek();
                props.data = s.s_gr_frek;

                item.SubItems.Add(props.drum.ToString());
                item.SubItems.Add(props.velocity.ToString());
                item.SubItems.Add(props.soundType.ToString());
                item.SubItems.Add(props.mode.ToString());
                item.SubItems.Add(props.aftertouch.ToString());
                item.SubItems.Add(props.stereoType.ToString());
                item.SubItems.Add(props.stereoPan.ToString());
                item.SubItems.Add(props.orgFreq.ToString());


                item.SubItems.Add(s.start.ToString("X8"));
                item.SubItems.Add(s.loopStart.ToString("X8"));
                item.SubItems.Add(s.end.ToString("X8"));
                item.SubItems.Add(s.loopMode.ToString());

                item.SubItems.Add(s.extVolume.ToString());
                item.SubItems.Add(s.subTone.ToString());

                spiSoundListView.Items.Add(item);
            }

            bool spiSaveEnabled = data.spiSounds.Count > 0;
            var mi = menuStrip1.Items.Find("saveSPIToolStripMenuItem", true);
            foreach (var item in mi)
            {
                item.Enabled = spiSaveEnabled;
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


        // Settings

        private bool loadProjectSettings(string file)
        {
            bool result = true;
            try
            {
                XmlSerializer ser = new XmlSerializer(typeof(EPSSEditorData)); // Enable Options - Debugging - Just My Code to not show the FileNotFoundExceptions.
                using (FileStream fs = new FileStream(file, FileMode.Open))
                {
                    data = (EPSSEditorData)ser.Deserialize(fs);

                    data.fixOldVersions();



                }
                updateDialog();
                updateTotalSize();
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

        // SPI


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
                string patchName = Utility.ReplaceIllegalCharacters(spi.ext.i_pname);
                if (String.IsNullOrEmpty(patchName))
                {
                    patchName = Path.GetFileName(file);
                }

                string path = Path.GetDirectoryName(Properties.Settings.Default.ProjectFile); // Sample create dir
                path += "\\" + patchName;

                //if (doConversion)
                if (WarnAndConfirmDir(path, "Directory for conversion of SPI already exists.\nDo you want to delete it?"))
                {

                    result = data.LoadSpiFile(ref spi, path, ref errorMessage);
                    if (result)
                    {
                        updateDialog();
                        updateTotalSize();
                        dataNeedsSaving = true;
                        saveProjectSettings();
                    }
                }
            }
            return result;
        }


        private bool doLoadSpiFileDialog(ref string errorMessage)
        {
            bool result = false;
            string s = Properties.Settings.Default.SpiFile;
            if (String.IsNullOrEmpty(s))
            {
                s = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                s = Path.Combine(s, "EPSS Projects", "default.spi");
            }
            else if (Path.GetExtension(s).ToLower() != "spi")
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


        private void doSaveSpi()
        {
            List<SpiSound> soundsToSave = data.spiSounds;
            if (soundsToSave.Count > 0)
            {
                string startDir;
                string startFile = Properties.Settings.Default.SpiExportFile;

                if (String.IsNullOrEmpty(startFile))
                {
                    startDir = Path.GetDirectoryName(Properties.Settings.Default.ProjectFile);
                }
                else
                {
                    startDir = Path.GetDirectoryName(startFile);
                }

                saveSpiFileDialog.FileName = spiNameTextBox.Text + ".spi";
                saveSpiFileDialog.InitialDirectory = startDir;

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
                            Properties.Settings.Default.SpiExportFile = spiFile;
                            Properties.Settings.Default.Save();

                            dataNeedsSaving = true;
                            saveProjectSettings();
                            MessageBox.Show("SPI exported successfully!", "EPSS Editor", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                        else
                        {
                            MessageBox.Show("Error occured during save! Patch file might not be valid.");
                        }
                    }
                }

            }
            else
            {
                System.Windows.Forms.MessageBox.Show("No sounds to save!");
            }
        }


        // Sound

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


        // Sound List Box

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


        private List<CachedSound> playSelectedSound()
        {
            List<CachedSound> soundsPlaying = new List<CachedSound>();
            try
            {
                List<Sound> sounds = getSelectedSounds();
                foreach (var snd in sounds)
                {
                    CachedSound cs = snd.cachedSound();
                    audio.PlaySound(cs);
                    soundsPlaying.Add(cs);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Cannot play sound:" + ex.Message);
            }
            return soundsPlaying;
        }




        // SPI Sound List View

        ListViewItem SelectedListViewItem(Point lwPos)
        {
            ListViewHitTestInfo info = spiSoundListView.HitTest(lwPos.X, lwPos.Y);
            ListViewItem item = info.Item;

            return item;
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
                foreach (int index in idxRemoved)
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


        private List<CachedSound> playConvertedSound()
        {

            List<int> selectedSnds = new List<int>();
            foreach (ListViewItem item in spiSoundListView.SelectedItems)
            {
                selectedSnds.Add(item.Index);
            }

            if (selectedSnds.Count > 0)
            {
                List<CachedSound> playedSounds = new List<CachedSound>();
                foreach (var selected in selectedSnds)
                {
                    SpiSound snd = data.spiSounds[selected];
                    int newFreq = frequencyFromCompressionTrackBar(compressionTrackBar.Value);
                    int newBits = AtariConstants.SampleBits;
                    int newChannels = AtariConstants.SampleChannels;
 
                    MemoryStream ms = snd.getWaveStream(data, newFreq, newBits, newChannels);
                    if (ms != null)
                    {
                        ms.Position = 0;
                        bool loop = snd.loopMode != 1;
                        CachedSound cs = snd.cachedSound(ms, newFreq, newBits, newChannels, loop);
                        audio.PlaySound(cs);
                        playedSounds.Add(cs);
                    }
                }
                return playedSounds;
            }
            return null;
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
                            if (snd.convertSound(data, outFile, frequencyFromCompressionTrackBar(compressionTrackBar.Value), AtariConstants.SampleBits, AtariConstants.SampleChannels))
                            {
                            }

                        }
                        else
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


        // SFZ

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
                    }
                    else
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
                    data.AddSfzSound(ref s, midiChannel, loByte, hiByte, kcByte, 0);

                }
            }

            updateDialog();
            dataNeedsSaving = true;
            saveProjectSettings();

            return anyFile;
        }


        private void DoSaveSfz()
        {
            string sfzFile = SfzExportFileName();
            string sampleSubDir = "samples";
            string sfzDir = "";
            string sampleDir = "";
            string name = "";
            if (CheckSfzDirectories(sfzFile, sampleSubDir, ref name, ref sfzDir, ref sampleDir))
            {
                Dictionary<int, List<SfzSplitInfo>> dict = data.ConvertToSfzSplitInfoForSfzExport();
                SfzConverter c = new SfzConverter();
                string errorMessage = "";
                bool result = c.SaveSFZ(ref dict, ref data.sounds, sfzDir, sampleSubDir, name, ref errorMessage);
                if (result) result = data.ExportSoundsToDir(sampleDir, ref errorMessage);
                if (result) MessageBox.Show("Exported successfully!", "EPSS Editor", MessageBoxButtons.OK, MessageBoxIcon.Information);
                else MessageBox.Show("Save sfz failed:\n" + errorMessage);

            }
        }

        private string SfzExportFileName()
        {
            string startDir;
            string startFile = Properties.Settings.Default.SfzExportFile;

            if (String.IsNullOrEmpty(startFile))
            {
                startDir = Path.GetDirectoryName(Properties.Settings.Default.ProjectFile);
            }
            else
            {
                startDir = Path.GetDirectoryName(startFile);
            }
            saveSfzFileDialog.FileName = spiNameTextBox.Text;
            saveSfzFileDialog.InitialDirectory = startDir;

            if (saveSfzFileDialog.ShowDialog() == DialogResult.OK)
            {
                string sfzFile = saveSfzFileDialog.FileName;
                Properties.Settings.Default.SfzExportFile = sfzFile;
                Properties.Settings.Default.Save();
                return sfzFile;
            }
            return null;
        }

        private bool CheckSfzDirectories(string sfzFile, string sampleSubDir, ref string name, ref string sfzDir, ref string sampleDir)
        {
            if (String.IsNullOrEmpty(sfzFile)) return false;


            name = Path.GetFileNameWithoutExtension(sfzFile);
            sfzDir = Path.GetDirectoryName(sfzFile) + "\\" + name;

            if (Directory.Exists(sfzDir))
            {
                if (System.Windows.Forms.MessageBox.Show("Directory " + name + " already exists here.\nDo you want to delete it?", "Confirm", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    Directory.Delete(sfzDir, true);
                }
            }
            Directory.CreateDirectory(sfzDir);

            bool doSave = true;

            //string sampleSubDir = "samples";
            sampleDir = sfzDir + "\\" + sampleSubDir;
            if (Directory.Exists(sampleDir))
            {
                if (System.Windows.Forms.MessageBox.Show("Directory 'samples' already exists here.\nAll used samples will be copied here.\nDo you want to continue?", "Confirm", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
                {
                    doSave = false;
                }
            }
            else
            {
                Directory.CreateDirectory(sampleDir);
            }

            return doSave;
        }

        // Other functions

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


        private string addOneSoundDirect(string file, string baseName)
        {
            Sound s = new Sound(file);

            data.sounds.Add(s);
            return file;
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


        private int spiVersion()
        {
            return gen2CheckBox.Checked ? 2 : 1;
        }


        private void setMidiChannel(int ch)
        {
            midiChTextBox.Text = ch.ToString();
            midiChTrackBar.Value = ch;

            updateMappingMode();

        }


        // Events

        private void Form1_Load(object sender, EventArgs e)
        {
            Size s = Properties.Settings.Default.WinSize;

            if (s.Width == 0) Properties.Settings.Default.Upgrade();

            if (s.Width == 0 || s.Height == 0)
            {
                // Do nothing, use default here.
            }
            else
            {
                this.WindowState = Properties.Settings.Default.WinState;

                if (this.WindowState == FormWindowState.Minimized) this.WindowState = FormWindowState.Normal;

                this.Size = s;
            }

            InitAudioSystem();

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


        private void SpiSoundListViewScrollListener_ControlScrolled(object sender, EventArgs e, int delta, Point pos)
        {
            if ((Control.ModifierKeys & Keys.Shift) != Keys.None)             // Only with shift key pressed!
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


        private void spiSoundListView_ItemSelectionChanged(object sender, ListViewItemSelectionChangedEventArgs e)
        {
            bool enabled = false;
            foreach (ListViewItem item in spiSoundListView.SelectedItems)
            {
                enabled = true;
                break;
            }
            deleteSpiSoundButton.Enabled = enabled;
            button1.Enabled = enabled;
        }


        private void spiSoundListView_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Delete)
            {
                deleteSelectedSpiSound();
                e.Handled = true;
            }
            else if (e.KeyCode == Keys.A && e.Control)
            {
                spiSoundListView.BeginUpdate();

                for (int i = 0; i < spiSoundListView.Items.Count; i++)
                {
                    spiSoundListView.Items[i].Selected = true;
                }

                spiSoundListView.EndUpdate();
                e.Handled = true;
            }
        }


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


        private void soundListBox_DragDrop(object sender, DragEventArgs e)
        {
            string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
            string anyFile = "";

            List<string> filesAdded = new List<string>();
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
                    filesAdded.Add(anyFile);
                }
            }

            if (filesAdded.Count > 0)
            {
                updateSoundListBox();

                soundListBox.BeginUpdate();
                Sound[] snds = data.sounds.ToArray();

                foreach (string file in filesAdded)
                {
                    for (int i = 0; i < snds.Count(); i++)
                    {
                        if (snds[i].path == file)
                        {
                            soundListBox.SetSelected(i, true);
                            break;
                        }
                    }
                }
            }

            soundListBox.EndUpdate();

            UpdateSoundDialog();
            data.soundFileName = anyFile;
            dataNeedsSaving = true;
            saveProjectSettings();
        }


        private void soundListBox_KeyPress(object sender, KeyPressEventArgs e)
        {

            if (e.KeyChar == ' ')
            {
                playSelectedSound();
                e.Handled = true;
            }
            if (ctrlAPressed) e.Handled = true;
        }


        private void soundListBox_KeyDown(object sender, KeyEventArgs e)
        {
            deletePressed = false;
            ctrlAPressed = false;
            if (e.KeyCode == Keys.Delete)
            {
                deleteSelectedSound();
                deletePressed = true;
                e.Handled = true;
            }
            else if (e.KeyCode == Keys.A && e.Control)
            {
                soundListBox.BeginUpdate();

                for (int i = 0; i < soundListBox.Items.Count; i++)
                {
                    soundListBox.SetSelected(i, true);
                }

                soundListBox.EndUpdate();
                e.Handled = true;
                ctrlAPressed = true;
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

                if ((Control.ModifierKeys & Keys.Alt) != Keys.None)
                {
                    foreach (ListViewItem item in spiSoundListView.Items) {
                        item.Selected = false;
                    }


                    foreach (ListViewItem item in spiSoundListView.Items)
                    {
                        int selected = item.Index;

                        if (selected >= 0)
                        {
                            SpiSound selectedSnd = data.spiSounds[selected];
                            foreach (var spiSnd in spiSounds)
                            {
                                if (spiSnd == selectedSnd)
                                {
                                    item.Selected = true;
                                }
                            }
                        }
                    }
                }
            }
            else
            {
                useInSpiButton.Enabled = false;
            }
            UpdateSoundDialog();
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
            }
            else
            {

            }

        }


        private void GmPercMidiMappingRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            setMidiChannel(10);
        }


        private void useInSpiButton_Click(object sender, EventArgs e)
        {
            List<Sound> sounds = getSelectedSounds();

            if (mappingModeMidiRadioButton.Checked)
            {
                if (sounds.Count > 1)
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

                        if (defaultMidiMapRadioButton.Checked || CustomSampleRadioButton.Checked)
                        {
                            bool doAdd = true;
                            byte startNote = 60;
                            byte endNote = 108;
                            if (CustomSampleRadioButton.Checked)
                            {
                                startNote = parseMidiTone(custMidiToneFromTextBox.Text);
                                endNote = parseMidiTone(custMidiToneToTextBox.Text);
                                doAdd = (startNote < 128 && endNote < 128 && startNote < endNote);
                                if (!doAdd) MessageBox.Show("Incorrect from and to note values!");
                            }
                            if (doAdd)
                            {
                                bool added = data.AddSoundToSpiSound(ref s, currentMidiChannel(), startNote, endNote);
                                if (!added)
                                {
                                    MessageBox.Show("Notes overlap!");
                                }
                                else
                                {
                                    updateSpiSoundListBox();

                                    if (defaultMidiMapRadioButton.Checked)
                                    {
                                        int ch = data.getNextFreeMidiChannel();
                                        if (ch > 0) setMidiChannel(ch);
                                    }

                                    dataNeedsSaving = true;
                                    saveProjectSettings();
                                    updateTotalSize();
                                }
                            }
                        }
                        else // Drums and Multisample (one sample per note)
                        {
                            bool addOk = true;
                            SpiSound spiSnd = new SpiSound(ref s);

                            int midiChannel = currentMidiChannel();
                            spiSnd.midiChannel = (byte)midiChannel;

                            if (GmPercMidiMappingRadioButton.Checked) // Always channel 10 when gmperc is chosen. 
                            {
                                spiSnd.midiNote = percussionNote(); // 1-128

                                if (data.isDrumSoundOccupied(spiSnd.midiNote))
                                {
                                    addOk = false;
                                    System.Windows.Forms.MessageBox.Show("Drum sound " + spiSnd.midiNote.ToString() + " already occupied!");
                                }

                            } else // multisample
                            {
                                byte startNote = parseMidiTone(midiToneTextBox.Text);
                                spiSnd.midiNote = spiSnd.startNote = spiSnd.endNote = startNote;

                                if (data.isMidiChannelOccupied(midiChannel))
                                {
                                    addOk = false;
                                    System.Windows.Forms.MessageBox.Show("MIDI channel " + midiChannel.ToString() + " already occupied!");
                                }
                            }

                            if (addOk)
                            {
                                data.spiSounds.Add(spiSnd);
                                updateSpiSoundListBox();

                                if (GmPercMidiMappingRadioButton.Checked) {
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
            else // This is for patches with Program Change!
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


        private void newProjectToolStripMenuItem_Click_1(object sender, EventArgs e)
        {
            bool doNew = true;
            if (dataNeedsSaving)
            {
                if (MessageBox.Show("You have unsaved data in current project!\nDo you really want to create new project?", "Confirm", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
                {
                    doNew = false;
                }
            }
            if (doNew)
            {
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


        private void saveSFZToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DoSaveSfz();
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

        private List<CachedSound> playedSpiSounds;

        private void spiSoundListenButton_MouseDown(object sender, MouseEventArgs e)
        {
            playedSpiSounds = playConvertedSound();
        }

        private void spiSoundListenButton_MouseUp(object sender, MouseEventArgs e)
        {
            foreach (var sound in playedSpiSounds) audio.StopSound(sound);
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

        private List<CachedSound> _playingSounds;


        private void playButton_MouseDown(object sender, MouseEventArgs e)
        {
            _playingSounds = playSelectedSound();
        }

        private void playButton_MouseUp(object sender, MouseEventArgs e)
        {
            foreach (var sound in _playingSounds)
            {
                audio.StopSound(sound);
            }
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


        private void toolStripMenuItem1_Click(object sender, EventArgs e)
        {

            List<Sound> sounds = getSelectedSounds();
            //var indices = soundListBox.SelectedIndices;
            if (sounds.Count == 1)
            {
                Sound snd = sounds[0];
                //int index = indices[0];

                RenameForm r = new RenameForm(snd.name());
                r.StartPosition = FormStartPosition.Manual;

                System.Windows.Forms.ToolStripMenuItem b = (System.Windows.Forms.ToolStripMenuItem)sender;
                Point p = b.Owner.Location;
                r.Location = p;


                r.Size = new Size(40, 20);
                DialogResult res = r.ShowDialog();

                if (res == DialogResult.OK)
                {
                    string s = r.GetText().Trim();
                    if (!String.IsNullOrEmpty(s))
                    {
                        if (snd.Rename(s))
                        {
                            updateSoundListBox();
                            data.RefreshSpiSounds();
                            updateSpiSoundListBox();
                            dataNeedsSaving = true;
                            saveProjectSettings();
                        }
                    }
                }
            }



        }

        private void custMidiToneFromTextBox_TextChanged(object sender, EventArgs e)
        {
            CustomSampleRadioButton.Checked = true;
        }

        private void custMidiToneToTextBox_TextChanged(object sender, EventArgs e)
        {
            CustomSampleRadioButton.Checked = true;
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
