using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Xml.Serialization;
using System.IO;
using System.Configuration;  // Add a reference to System.Configuration.dll
using System.Reflection;

namespace EPSSEditor
{
    public partial class Form1 : Form
    {
        public EPSSEditorData data;
        private bool ctrlAPressed;
        private bool callbacks = true;
        private int initialize;
        private bool dataNeedsSaving;
        private readonly ControlScrollListener _processListViewScrollListener;
        private AudioPlaybackEngine audio = null;
        private SpiSoundInstrument spiSoundInstrument;
        private PianoKbForm pianoKbForm = null;
        private List<CachedSound> _playingSounds;
        private int spoolStep;
        private bool wasPlayingBeforeSpool;


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


        private void InitEpssEditorData(bool forceNewProject = false)
        {
            string projectFile = Properties.Settings.Default.ProjectFile;
            bool initNewProjectFile = false;
            if (forceNewProject || projectFile == null | projectFile == "")
            {
                initNewProjectFile = true;
            }
            else
            {
                if (!LoadProjectSettings(projectFile))
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
                        if (LoadProjectSettingsFileDialog())
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
                    data.Initialize(DrumMappingsFileName());
                    UpdateDialog();
                    dataNeedsSaving = true;
                    Undo.New(data);
                    //SaveProjectSettings();
                }
            }
            int newFreq = FrequencyFromCompressionTrackBar(1); // TODO settings to set freq for midi player instrument
            spiSoundInstrument = new SpiSoundInstrument(GetEPSSEditorDataCallBack, audio, newFreq);
            spiSoundInstrument.NoteOnEvent += SpiSoundInstrument_NoteOnEvent;
            spiSoundInstrument.NoteOffEvent += SpiSoundInstrument_NoteOffEvent;

            defaultMidiMapRadioButton.Checked = true;
            mappingModeMidiRadioButton.Checked = true;
            SetMidiChannel(1);
            UpdateTotalSize();
            CheckUpdates.CheckForApplicationUpdate(this, GetRunningVersion());
        }
      

        public void IgnoreThisUpdate(string version) 
        {
            Console.WriteLine($"Ignore {version}");
            Properties.Settings.Default.IgnoreVersion = version;
            Properties.Settings.Default.Save();
        }


        private EPSSEditorData GetEPSSEditorDataCallBack()
        {
            return data;
        }


        private string DrumMappingsFileName()
        {
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
            return combined;
        }


        private void Exit()
        {
            timer1.Stop();
            MidPlayer.StopPlaying();
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
            SaveProjectSettings();
            ExitAudioSystem();
        }


        private void InitAudioSystem()
        {
            if (audio == null) audio = new AudioPlaybackEngine();
            audio.Start();
        }


        private void ExitAudioSystem()
        {
            if (audio != null)
            {
                audio.Stop();
                audio.Dispose();
                audio = null;
            }
        }

        private void UpdateDialog()
        {
            string file = Properties.Settings.Default.ProjectFile;
            this.Text = "EPSS Editor v" + GetRunningVersion().ToString() + "   -   Project: " + file;

            UpdateDrumSettings();
            UpdateSoundListBox();
            UpdateSpiSoundListBox();
            UpdateSpiSoundButtons();
            UpdatePreview();
            UpdateMappingMode();
            UpdateSoundDialog();
            UpdateConversionSettings();
        }


        private void UpdateSpiSoundButtons()
        {
            bool enabled = SelectedSpiSounds().Count > 0;
            deleteSpiSoundButton.Enabled = enabled;
            button1.Enabled = enabled;
            spiSoundListenButton.Enabled = enabled;
        }


        private void UpdateConversionSettings()
        {
            bool enabled = soundListBox.SelectedItems.Count == 1;
            conversionTextBox.Enabled = enabled;
            groupBox6.Enabled = enabled;
            groupBox7.Enabled = enabled;
            groupBox1.Enabled = enabled;
        }


        private void UpdateSoundDialog()
        {
            groupBox5.Enabled = soundListBox.SelectedItems.Count == 1;
            deleteSoundButton.Enabled = soundListBox.SelectedItems.Count > 0;
        }


        private void UpdateMappingMode()
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
                drumsComboBox1.Enabled = ch == 10;

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
            custMidiToneCtrLabel.Enabled = midiMappingMode;
            custMidToneCentreTextBox.Enabled = midiMappingMode;
        }


        private void UpdatePreview()
        {
            if (initialize > 0)
            {
                previewComboBox.SelectedIndex = data.previewSelected;
            }
        }


        private void UpdateDrumSettings()
        {
            foreach (Mapping m in data.drumMappings.mappings)
            {
                drumsComboBox1.Items.Add(m.dropDownDescription());
            }
            drumsComboBox1.SelectedIndex = 0;
        }


        private void UpdateSoundListBox()
        {
            soundListBox.Items.Clear();
            foreach (Sound s in data.sounds)
            {
                soundListBox.Items.Add(s.name());
            }

            deleteSoundButton.Enabled = soundListBox.SelectedItems.Count > 0;
        }


        private void UpdateListViewItemValue(SpiSound snd, ListViewItem item, int columnIndex)
        {
            if (columnIndex == 7)
            {
                ListViewItem.ListViewSubItem subItem = item.SubItems[7];
                subItem.Text = snd.transposeString();
            }
        }


        private void UpdateSpiSoundListBox()
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
            foreach (SpiSound s in data.SpiSounds())
            {
                ListViewItem item = new ListViewItem(i++.ToString());

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
                    item.SubItems.Add((s.programNumber+1).ToString());
                }
                else
                {
                    item.SubItems.Add("-");
                }

                item.SubItems.Add(s.name());
                int nr = data.GetSoundNumberFromGuid(s.soundId);
                item.SubItems.Add(nr.ToString());

                item.SubItems.Add(Ext.ToPrettySize(s.preLength(data), 2));
                item.SubItems.Add(s.transposeString());
                item.SubItems.Add(s.VvfeString());

                EPSSSpi_s_gr_frek props = new EPSSSpi_s_gr_frek
                {
                    data = s.s_gr_frek
                };

                item.SubItems.Add(props.Drum.ToString());
                item.SubItems.Add(props.Velocity.ToString());
                item.SubItems.Add(props.SoundType.ToString());
                item.SubItems.Add(props.Mode.ToString());
                item.SubItems.Add(props.Aftertouch.ToString());
                item.SubItems.Add(props.StereoType.ToString());
                item.SubItems.Add(props.StereoPan.ToString());
                item.SubItems.Add(props.OrgFreq.ToString());

                item.SubItems.Add(s.start.ToString("X8"));
                item.SubItems.Add(s.loopStart.ToString("X8"));
                item.SubItems.Add(s.end.ToString("X8"));
                item.SubItems.Add(s.loopMode.ToString());

                item.SubItems.Add(s.extVolume.ToString());
                item.SubItems.Add(s.subTone.ToString());

                spiSoundListView.Items.Add(item);
            }

            Sound snd = GetSoundAtSelectedIndex();
            if (snd != null)
            {
                List<SpiSound> spiSounds = data.GetSpiSoundsFromSound(snd);
                deleteSoundButton.Enabled = spiSounds.Count == 0;

            }

            spiNameTextBox.Text = data.spiName;
            spiInfoTextBox.Text = data.spiDescription;
            omniPatchCheckBox.Checked = data.omni;
            gen2CheckBox.Checked = data.HasAnyProgramChange();

            bool spiSaveEnabled = data.IsValidForSpiExport();
            var mi = menuStrip1.Items.Find("saveSPIToolStripMenuItem", true);
            foreach (var item in mi)
            {
                item.Enabled = spiSaveEnabled;
            }
        }


        private void UpdateTotalSize()
        {
            if (data != null)
            {
                EPSSSpiCreator creator = new EPSSSpiCreator(SpiVersion());
                long sz = creator.Length(data);
                totalSizeTextBox.Text = Ext.ToPrettySize(sz, 2);

                int v = (int)(sz / 1024);
                totalSizeProgressBar.Value = Math.Min(14000, v); // Only show up to max 14MB but we support unlimited size..
            }
        }


        private void UpdateAfterSoundChange(Sound snd, int toFreq)
        {
            callbacks = false;
            int numberOfDecimals = 2;
            sizeTextBox.Text = Ext.ToPrettySize(snd.sampleDataLength, numberOfDecimals);
            channelsTextBox.Text = snd.channels.ToString() + " Channels";
            freqTextBox.Text = snd.samplesPerSecond.ToString() + " Hz";
            bitsTextBox.Text = snd.bitsPerSample.ToString() + " Bit";

            int toBit = AtariConstants.SampleBits;

            snd.updateConversionParameters(toBit, toFreq);
            conversionTextBox.Text = snd.parameters().description();

            long lengthAfter = snd.parameters().sizeAfterConversion(snd);

            soundSizeAfterTextBox.Text = Ext.ToPrettySize(lengthAfter, numberOfDecimals);

            UpdateShowCompressionProgressBar(snd.sampleDataLength, lengthAfter);

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


        private void UpdateAfterSoundsAdded(List<string> filesAdded, bool spiNeedsUpdate)
        {
            string anyFile = "";
            if (filesAdded.Count > 0)
            {
                UpdateSoundListBox();
                if (spiNeedsUpdate)
                {
                    UpdateSpiSoundListBox();
                    UpdateTotalSize();
                }

                soundListBox.BeginUpdate();
                foreach (string file in filesAdded)
                {
                    anyFile = file;
                    for (int i = 0; i < data.sounds.Count(); i++)
                    {
                        if (data.sounds[i].path == file)
                        {
                            soundListBox.SetSelected(i, true);
                            break;
                        }
                    }
                }
                soundListBox.EndUpdate();
            }

            UpdateSoundDialog();
            UpdateConversionSettings();
            if (!String.IsNullOrEmpty(anyFile)) data.soundFileName = anyFile;
            Undo.RegisterUndoChange(data);
            dataNeedsSaving = true;
            SaveProjectSettings();
        }


        private void UpdateShowCompressionProgressBar(long lengthBefore, long lengthAfter)
        {
            if (lengthBefore > 0)
            {
                int v = (int)(((double)lengthAfter / (double)lengthBefore) * 100);
                showCompProgressBar.Value = v;
            }
        }


        // Settings
        private bool LoadProjectSettings(string file)
        {
            bool result = true;
            try
            {
                XmlSerializer ser = new XmlSerializer(typeof(EPSSEditorData)); // Enable Options - Debugging - Just My Code to not show the FileNotFoundExceptions.
                using (FileStream fs = new FileStream(file, FileMode.Open))
                {
                    data = (EPSSEditorData)ser.Deserialize(fs);
                    data.FixOldVersions();
                }

                string newDir = "";
                string sampleNotFound = "";
                bool clearAllSamples = false;
                while (true)
                {
                    bool vf = data.VerifyFiles(newDir, out sampleNotFound);

                    if (!vf)
                    {
                        int verifyResult = VerifySamplesInProject(sampleNotFound, ref newDir);
                        if (verifyResult == 0) break;
                        else if (verifyResult == 2)
                        {
                            clearAllSamples = true;
                            break;
                        }
                        // remap
                        if (String.IsNullOrEmpty(newDir))
                        {
                            clearAllSamples = true;
                            break;
                        }
                    }
                    else
                    {
                        break;
                    }
                }

                if (clearAllSamples)
                {
                    if (MessageBox.Show("Do you really want to delete all samples.\nin this project and continue?", "EPSS Editor", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                    {
                        data.ClearAll();
                    }
                    else
                    {
                        result = false;
                    }
                }
                Undo.New(data);
                UpdateDialog();
                UpdateTotalSize();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                MessageBox.Show("Loading project settings error:\n" + ex.Message);
                result = false;
            }
            return result;
        }


        private int VerifySamplesInProject(string sampleNotFound, ref string newDir)
        {
            if (MessageBox.Show("Sample " + sampleNotFound + " cannot be found.\nDo you want to point to other directory for samples?", "EPSS Editor", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                folderBrowserDialog1.ShowNewFolderButton = false;
                if (folderBrowserDialog1.ShowDialog() == DialogResult.OK)
                {
                    StringBuilder stringBuilder = new StringBuilder();
                    stringBuilder.Append(folderBrowserDialog1.SelectedPath);
                    newDir = stringBuilder.ToString();
                    return 1; // Try to remap
                }
            }
            else
            {
                return 2; // We want to quit
            }
            return 0;
        }

        /*
         * TODO
        private void BackupProjectSettings()
        {
            string file = Properties.Settings.Default.ProjectFile;
            if (File.Exists(file))
            {
                string backupFile = Path.ChangeExtension(file, ".bak");
                int n = 1;
                while (File.Exists(backupFile))
                {
                    backupFile = Path.GetDirectoryName(backupFile) + '\\' + Path.GetFileNameWithoutExtension(file) + " (Copy " + n.ToString() + ").bak";
                    n++;
                }
                File.Copy(file, backupFile, false);
            }
        }
        */


        private void SaveProjectSettings()
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


        private bool LoadProjectSettingsFileDialog()
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
                LoadProjectSettings(file);
                result = true;
            }

            return result;
        }


        private void SaveProjectSettingsFileDialog()
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
                SaveProjectSettings();
            }
        }


        // SPI
        private bool WarnAndConfirmSpiDir(string dir, string message)
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


        private bool LoadSpiFile(string file, out string errorMessage)
        {
            bool result = false;
            EPSSSpiLoader loader = new EPSSSpiLoader();
            Uri url = new Uri(file);
            EPSSSpi spi = loader.Load(url, out errorMessage);
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

                if (WarnAndConfirmSpiDir(path, "Directory for conversion of SPI already exists.\nDo you want to delete it?"))
                {
                    data = new EPSSEditorData();
                    data.Initialize(DrumMappingsFileName());

                    result = data.LoadSpiFile(spi, path, out errorMessage);
                    if (result)
                    {
                        Undo.New(data);
                        UpdateDialog();
                        UpdateTotalSize();
                        dataNeedsSaving = true;
                        SaveProjectSettings();
                    }
                }
            }
            return result;
        }


        private bool DoLoadSpiFileDialog(out string errorMessage)
        {
            bool result = false;
            errorMessage = null;
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
                result = LoadSpiFile(file, out errorMessage);
            }

            return result;
        }


        private void DoSaveSpi()
        {
            if (data.IsValidForSpiExport(out string errorString))
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

                    EPSSSpiCreator creator = new EPSSSpiCreator(SpiVersion());
                    int sampFreq = AtariConstants.SampleFreq25k;
                    data.omni = omniPatchCheckBox.Checked;

                    EPSSSpi spi = creator.Create(data, spiNameTextBox.Text, spiInfoTextBox.Text, sampFreq);

                    if (spi != null)
                    {
                        int result = spi.Save(url);
                        if (result == 0)
                        {
                            Properties.Settings.Default.SpiExportFile = spiFile;
                            Properties.Settings.Default.Save();
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
                MessageBox.Show("Can not save SPI:" + errorString);
            }
        }


        // Sound
        private void LoadSound()
        {
            string s = data.soundFileName;

            int filterIdx = 3;
            if (s == null || s == "")
            {
                s = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                s = Path.Combine(s, "sample.wav");
            }
            string ext = Path.GetExtension(s).ToLower();
            if (ext == ".wav") filterIdx = 1;
            else if (ext == ".ogg") filterIdx = 2;

            loadSoundFileDialog.InitialDirectory = Path.GetDirectoryName(s);
            loadSoundFileDialog.FileName = Path.GetFileName(s);
            loadSoundFileDialog.FilterIndex = filterIdx;

            if (loadSoundFileDialog.ShowDialog() == DialogResult.OK)
            {
                string anyFile = "";
                foreach (string filePath in loadSoundFileDialog.FileNames)
                {
                    if (data.AddSound(filePath, out _, out string errorMessage))
                    {
                        anyFile = filePath;
                    }
                    else
                    {
                        string fileName = Path.GetFileName(filePath);
                        MessageBox.Show($"Error loading sound {fileName}:\n{errorMessage}");
                    }
                }
                if (!String.IsNullOrEmpty(anyFile)) {
                    UpdateSoundListBox();

                    data.soundFileName = anyFile;
                    Undo.RegisterUndoChange(data);
                    dataNeedsSaving = true;
                    SaveProjectSettings();
                }
            }
        }


        // Sound List Box
        private void DeleteSelectedSound()
        {
            var indices = soundListBox.SelectedIndices;
            bool anySoundsReferSPI = false;
            foreach (int index in indices)
            {
                if (data.SoundRefersToSPISound(index))
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

                //int removed = 0;
                
                int topIndex = soundListBox.TopIndex;
                int visible = soundListBox.Height / soundListBox.ItemHeight - 1;
                int dist = 0;
                int idx = -1;
                for (int index = indices.Count - 1; index >= 0; index--)
                {
                    int removeIdx = indices[index];
                    dist = Math.Min(visible, Math.Max(0, removeIdx - topIndex));
                    data.RemoveSound(removeIdx);
                    soundListBox.Items.RemoveAt(removeIdx);
                    if (idx == -1) idx = removeIdx;
                }
                

                int itemsLeft = soundListBox.Items.Count;
                if (itemsLeft > 0)
                {
                    if (idx >= itemsLeft)
                    {
                        idx = itemsLeft - 1;
                    }
                    soundListBox.SelectedIndex = idx;
                    useInSpiButton.Enabled = true;
                    soundListBox.TopIndex = Math.Max(0, idx - dist);

                    
                    
                }
                else
                {
                    useInSpiButton.Enabled = false;
                }
                Undo.RegisterUndoChange(data);
                dataNeedsSaving = true;
                SaveProjectSettings();

                deleteSoundButton.Enabled = soundListBox.SelectedItems.Count > 0;
            }
        }


        private Sound GetSoundAtSelectedIndex()
        {
            int idx = soundListBox.SelectedIndex;
            if (idx >= 0)
            {
                return data.sounds[idx];
            }
            return null;
        }


        private List<Sound> GetSelectedSounds()
        {
            ListBox.SelectedIndexCollection soundIndices = soundListBox.SelectedIndices;
            List<Sound> selected = new List<Sound>();
            foreach (int index in soundIndices)
            {
                selected.Add(data.sounds[index]);
            }
            return selected;
        }


        private List<CachedSound> PlaySelectedSound(bool forceLoopOff=false)
        {
            List<CachedSound> soundsPlaying = new List<CachedSound>();
            try
            {
                List<Sound> sounds = GetSelectedSounds();
                foreach (var snd in sounds)
                {
                    CachedSound cs = snd.cachedSound();
                    if (forceLoopOff) cs.loop = false;
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
            return spiSoundListView.HitTest(lwPos.X, lwPos.Y).Item;
        }


        private void DeleteSelectedSpiSound()
        {
            List<int> idxRemoved = SelectedSpiSounds();
            if (idxRemoved.Count > 0)
            {
                ListViewItem topItem = spiSoundListView.TopItem;
                int lastIndex = topItem.Index;
                
                int lastRemoved = -1;

                for (int index = idxRemoved.Count-1; index >= 0; index--)
                {
                    data.RemoveSpiSound(idxRemoved[index]);
                    spiSoundListView.Items[idxRemoved[index]].Remove();
                    lastRemoved = idxRemoved[index];
                }

                if (spiSoundListView.Items.Count > 0)
                {
                    int select = Math.Min(lastRemoved, spiSoundListView.Items.Count - 1);
                    select = Math.Max(0, select);
                    spiSoundListView.Items[select].Selected = true;
                    spiSoundListView.Items[select].Focused = true;
                    int dist = lastRemoved - lastIndex;
                    spiSoundListView.TopItem = spiSoundListView.Items[Math.Max(0, select-dist)];
                    spiSoundListView.Items[select].EnsureVisible();
                }
                Undo.RegisterUndoChange(data);
                dataNeedsSaving = true;
                SaveProjectSettings();
                UpdateTotalSize();
            }
        }


        private List<int> SelectedSpiSounds()
        {
            List<int> selectedSnds = new List<int>();
            foreach (ListViewItem item in spiSoundListView.SelectedItems)
            {
                selectedSnds.Add(item.Index);
            }
            return selectedSnds;
        }


        private void PlayConvertedSound(SpiSound snd, int midiChannel, int programChange, int note)
        {
            if (programChange < 128)
            {
                spiSoundInstrument.ProgramChange(midiChannel, programChange);
            }
            spiSoundInstrument.UpdateSoundForMidiChannel(snd, midiChannel, note);
            
            spiSoundInstrument.NoteOn(midiChannel, note, 127);
        }


        private void SaveSampleWithFileDialog()
        {
            string s = data.soundFileName;

            if (s == null || s == "")
            {
                s = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                s = Path.Combine(s, "sample.wav");
            }
            else if (Path.GetExtension(s).ToLower() != "wav")
            {
                s = Path.ChangeExtension(s, "wav");
            }


            List<int> selectedSpiSounds = SelectedSpiSounds();
            if (selectedSpiSounds.Count > 0)
            {
                foreach (int selected in selectedSpiSounds)
                {
                    if (selected >= 0)
                    {
                        saveSampleFileDialog.InitialDirectory = Path.GetDirectoryName(s);
                        saveSampleFileDialog.FileName = Path.GetFileName(s);
                        if (saveSampleFileDialog.ShowDialog() == DialogResult.OK)
                        {
                            string outFile = saveSampleFileDialog.FileName;
                            SpiSound snd = data.SpiSoundAtIndex(selected);
                            if (!snd.convertSound(data, outFile, FrequencyFromCompressionTrackBar(compressionTrackBar.Value), AtariConstants.SampleBits, AtariConstants.SampleChannels))
                            {
                                MessageBox.Show("Sound not saved.");
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
        private bool DoLoadSfzFileDialog(out string errorMessage)
        {
            bool result = false;
            errorMessage = "";
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
                                
                List<string> soundFilesAdded = new List<string>();
                result = LoadSfz(file, soundFilesAdded, out errorMessage);             
                if (result)
                {
                    UpdateAfterSoundsAdded(soundFilesAdded, true);
                }               
            }
            return result;
        }


        private bool LoadSfz(string filePath, List<string> soundFilesAdded, out string errorMessage)
        {
            int cm = CurrentMidiChannel();
            int midiChannel = mappingModeMidiRadioButton.Checked ? cm : 128;
            int programChange = mappingModeProgramRadioButton.Checked ? cm - 1 : 128;

            bool result = SfzConverter.LoadSfzSound(data, midiChannel, programChange, filePath, soundFilesAdded, out errorMessage);
            if (result && soundFilesAdded.Count > 0)
            {
                int ch = midiChannel < 128 ? data.GetNextFreeMidiChannel() : data.GetNextFreeProgramChange() + 1;
                if (ch > 0) SetMidiChannel(ch);
                Properties.Settings.Default.SfzFile = filePath;
                Properties.Settings.Default.Save();
            }
            else
            {
                errorMessage = "Nothing was loaded. Files already exists.";
            }
            return result;
        }

 
        private void DoSaveSfz()
        {
            string sfzFile = SfzExportFileName();
            string sampleSubDir = "samples";
            if (CheckSfzDirectories(sfzFile, sampleSubDir, out string name, out string sfzDir, out string sampleDir))
            {
                Dictionary<int, List<SfzSplitInfo>> dict = data.ConvertToSfzSplitInfoForSfzExport();
                SfzConverter c = new SfzConverter();
                bool result = c.SaveSFZ(dict, data.sounds, sfzDir, sampleSubDir, name, out string errorMessage);
                if (result) result = data.ExportSoundsToDir(sampleDir, out errorMessage);
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


        private bool CheckSfzDirectories(string sfzFile, string sampleSubDir, out string name, out string sfzDir, out string sampleDir)
        {
            name = sfzDir = sampleDir = "";
            if (String.IsNullOrEmpty(sfzFile))
            {
                return false;
            }

            name = Path.GetFileNameWithoutExtension(sfzFile);
            sfzDir = Path.GetDirectoryName(sfzFile) + "\\" + name;
            if (Directory.Exists(sfzDir))
            {
                if (MessageBox.Show("Directory " + name + " already exists here.\nDo you want to delete it?", "Confirm", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    try
                    {
                        Directory.Delete(sfzDir, true);
                    }
                    catch(Exception ex)
                    {
                        MessageBox.Show("Cannot delete directory as another process is locking it. Aborting save.");
                        Console.WriteLine("CheckSfzDirectories: {0}", ex.ToString());
                        return false;
                    }
                }
            }
            Directory.CreateDirectory(sfzDir);

            bool doSave = true;

            sampleDir = sfzDir + "\\" + sampleSubDir;
            if (Directory.Exists(sampleDir))
            {
                if (MessageBox.Show("Directory 'samples' already exists here.\nAll used samples will be copied here.\nDo you want to continue?", "Confirm", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
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
        private int FrequencyFromCompressionTrackBar(int v)
        {
            if (v == 0) return AtariConstants.SampleFreq50k;
            else if (v == 1) return AtariConstants.SampleFreq25k;
            else if (v == 2) return AtariConstants.SampleFreq12k;
            else if (v == 3) return AtariConstants.SampleFreq6k;
            return AtariConstants.SampleFreq25k;
        }


        private int CompressionTrackBarValueFromFrequency(int f)
        {
            if (f == AtariConstants.SampleFreq50k) return 0;
            else if (f == AtariConstants.SampleFreq25k) return 1;
            else if (f == AtariConstants.SampleFreq12k) return 2;
            else if (f == AtariConstants.SampleFreq6k) return 3;
            return 2;
        }


        private int CurrentMidiChannel()
        {
            return midiChTrackBar.Value;
        }


        private byte PercussionNote()
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





        private int SpiVersion()
        {
            return gen2CheckBox.Checked ? 2 : 1;
        }


        private void SetMidiChannel(int ch)
        {
            midiChTextBox.Text = ch.ToString();
            midiChTrackBar.Value = ch;
            UpdateMappingMode();
        }


        private void SetProgramChange(int pc) // 1-128
        {
            UpdateMappingMode();
            midiChTextBox.Text = pc.ToString();
            midiChTrackBar.Value = pc;
        }


        // Events
        private void Form1_Load(object sender, EventArgs e)
        {
            InitAudioSystem();

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
        }


        private void Form1_Activated(object sender, EventArgs e)
        {
            if (initialize < 1)
            {
                initialize++;
                InitEpssEditorData();
            }
        }


        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            Exit();
        }


        private void SpiSoundListViewScrollListener_ControlScrolled(object sender, EventArgs e, int delta, Point pos)
        {
            if ((Control.ModifierKeys & Keys.Shift) != Keys.None)             // Only with shift key pressed!
            {
                Point lwPos = spiSoundListView.PointToClient(pos);
                ListViewItem item = SelectedListViewItem(lwPos);
                if (item != null)
                {
                    SpiSound snd = data.SpiSoundAtIndex(item.Index);
                    if (delta > 0 && snd.transpose < 127) snd.transpose++;
                    else if (delta < 0 && snd.transpose > -128) snd.transpose--;
                    UpdateListViewItemValue(snd, item, 7);
                }
            }
        }


        private void SpiSoundListView_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Middle && (Control.ModifierKeys & Keys.Shift) != Keys.None)
            {
                Point pos = e.Location;
                ListViewItem item = SelectedListViewItem(pos);
                if (item != null)
                {
                    SpiSound snd = data.SpiSoundAtIndex(item.Index);
                    snd.transpose = 0;
                    UpdateListViewItemValue(snd, item, 7);
                    Console.WriteLine($"Item: {item}");
                }
            }
        }


        private void SpiSoundListView_ItemSelectionChanged(object sender, ListViewItemSelectionChangedEventArgs e)
        {
            UpdateSpiSoundButtons();
        }


        private void SpiSoundListView_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Delete)
            {
                DeleteSelectedSpiSound();
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
            else if (e.KeyCode == Keys.Z && e.Control)
            {
                EPSSEditorData newData = Undo.UndoLastOperation();
                if (newData != null)
                {
                    data = newData;
                    UpdateDialog();
                    UpdateTotalSize();
                }
            }
            else if (e.KeyCode == Keys.Y && e.Control)
            {
                EPSSEditorData newData = Undo.RedoLastOperation();
                if (newData != null)
                {
                    data = newData;
                    UpdateDialog();
                    UpdateTotalSize();
                }
            }
        }


        private void SoundListBox_DragEnter(object sender, DragEventArgs e)
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


        private void SoundListBox_DragDrop(object sender, DragEventArgs e)
        {
            string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
            List<string> filesAdded = new List<string>();
            bool spiNeedsUpdate = false;

            string lastFileName = "";
            bool result = false;
            Dictionary<string, string> filesWithErrors =   new Dictionary<string, string>();
            foreach (var filePath in files)
            {
                string errorMessage = "";
                lastFileName = Path.GetFileName(filePath);
                string ext = Path.GetExtension(filePath).ToLower();
                if (ext == ".sfz")
                {
                    result = LoadSfz(filePath, filesAdded, out errorMessage);
                    spiNeedsUpdate = true;
                }
                else 
                {
                     result = data.AddSound(filePath, out _, out errorMessage);
                    if (result) filesAdded.Add(filePath);
                }
                if (!result)
                {
                    filesWithErrors.Add(Path.GetFileName(filePath), errorMessage);
                }
            }
            if (filesWithErrors.Count > 0) {
                StringBuilder sb = new StringBuilder();
                sb.AppendLine($"{files.Length} files dropped, {filesAdded.Count} sounds loaded.");
                sb.AppendLine("Files not loaded:");
                foreach (var file in filesWithErrors)
                {
                    sb.AppendLine($"{file.Key} : {file.Value}");
                }
                MessageBox.Show(sb.ToString());
            }

            if (filesAdded.Count > 0)
            {
                UpdateAfterSoundsAdded(filesAdded, spiNeedsUpdate);
            } 
            else if (result)
            {
                MessageBox.Show("Nothing was loaded.");
            }
        }


        private void SoundListBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == ' ')
            {
                PlaySelectedSound(forceLoopOff:true);
                e.Handled = true;
            }
            if (ctrlAPressed) e.Handled = true;
        }


        private void SoundListBox_KeyDown(object sender, KeyEventArgs e)
        {
            ctrlAPressed = false;
            if (e.KeyCode == Keys.Delete)
            {
                DeleteSelectedSound();
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


        private void SoundListBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            Sound snd = GetSoundAtSelectedIndex();
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
                UpdateAfterSoundChange(snd, toFreq);
                compressionTrackBar.Value = CompressionTrackBarValueFromFrequency(toFreq);
                useInSpiButton.Enabled = true;
                List<SpiSound> spiSounds = data.GetSpiSoundsFromSound(snd);
                if ((Control.ModifierKeys & Keys.Alt) != Keys.None)
                {
                    foreach (ListViewItem item in spiSoundListView.Items)
                    {
                        item.Selected = false;
                    }
                    foreach (ListViewItem item in spiSoundListView.Items)
                    {
                        int selected = item.Index;
                        if (selected >= 0)
                        {
                            SpiSound selectedSnd = data.SpiSoundAtIndex(selected);
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
            UpdateConversionSettings();
        }


        private void LoadSoundButton_Click(object sender, EventArgs e)
        {
            LoadSound();
        }


        private void DeleteSoundButton_Click(object sender, EventArgs e)
        {
            DeleteSelectedSound();
        }


        private void CompressionTrackBar_Scroll(object sender, EventArgs e)
        {
            Sound snd = GetSoundAtSelectedIndex();
            if (snd != null)
            {
                int toFreq = FrequencyFromCompressionTrackBar(compressionTrackBar.Value);
                UpdateAfterSoundChange(snd, toFreq);
            }
        }


        private void MidiChTrackBar_Scroll(object sender, EventArgs e)
        {
            int ch = midiChTrackBar.Value;
            SetMidiChannel(ch);
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
        }


        private void GmPercMidiMappingRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            SetMidiChannel(10);
        }


        private void UseInSpiButton_Click(object sender, EventArgs e)
        {
            List<Sound> sounds = GetSelectedSounds();
            if (mappingModeMidiRadioButton.Checked)
            {
                if (sounds.Count > 1)
                {
                    // Map multiple sounds
                    bool mappingOk = false;
                    byte startNote = 128;
                    if (MultiSampleRadioButton.Checked)
                    {
                        startNote = Utility.ParseMidiTone(midiToneTextBox.Text);
                        mappingOk = true;
                    }
                    else if (GmPercMidiMappingRadioButton.Checked)
                    {
                        mappingOk = true;
                        startNote = PercussionNote();
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
                            Sound snd = sound;
                            SpiSound spiSnd = new SpiSound(snd);

                            if (defaultMidiMapRadioButton.Checked)
                            {
                                spiSnd.startNote = 60;
                                spiSnd.endNote = 108;
                                spiSnd.midiNote = 84;
                            }
                            else
                            {
                                spiSnd.midiNote = spiSnd.startNote = spiSnd.endNote = startNote;
                                startNote++;
                            }

                            int midiChannel = CurrentMidiChannel();
                            spiSnd.midiChannel = (byte)midiChannel;

                            int toFreq;
                            if (snd.parameters().hasParameters())
                            {
                                toFreq = snd.parameters().freq.toFreq;
                            }
                            else
                            {
                                toFreq = Properties.Settings.Default.SampleConversionDestFreq;
                            }

                            UpdateAfterSoundChange(snd, toFreq);
                            compressionTrackBar.Value = CompressionTrackBarValueFromFrequency(toFreq);

                            if (!defaultMidiMapRadioButton.Checked)
                            {
                                data.RemoveSpiSound(spiSnd.midiChannel, spiSnd.midiNote);
                            }
                            data.AddSpiSound(spiSnd);

                            if (defaultMidiMapRadioButton.Checked)
                            {
                                int ch = data.GetNextFreeMidiChannel();
                                if (ch > 0) SetMidiChannel(ch);
                            }
                        }
                        UpdateSpiSoundListBox();
                        UpdateTotalSize();
                        Undo.RegisterUndoChange(data);
                        dataNeedsSaving = true;
                        SaveProjectSettings();
                    }
                    else
                    {
                        MessageBox.Show("Need to have multi sample mapping or GM Percussion mapping selected!");
                    }
                }
                else
                {
                    // Map single sound
                    Sound snd = GetSoundAtSelectedIndex();
                    if (snd != null)
                    {
                        if (defaultMidiMapRadioButton.Checked || CustomSampleRadioButton.Checked)
                        {
                            bool doAdd = true;
                            byte startNote = 60;
                            byte endNote = 108;
                            byte center = 84;
                            if (CustomSampleRadioButton.Checked)
                            {
                                startNote = Utility.ParseMidiTone(custMidiToneFromTextBox.Text);
                                endNote = Utility.ParseMidiTone(custMidiToneToTextBox.Text);
                                string centerText = custMidToneCentreTextBox.Text;
                                if (!String.IsNullOrEmpty(centerText)) center = Utility.ParseMidiTone(centerText);
                                else center = 0;

                                doAdd = (startNote < 128 && endNote < 128 && startNote <= endNote && (startNote - endNote) <= 48);

                                if (center == 0) center = (byte)(startNote + (endNote - startNote) / 2);
                                else
                                {
                                    if (center < startNote || center > endNote) doAdd = false;
                                }
                                if (!doAdd) MessageBox.Show("Incorrect from, to, center note values!");
                            }
                            if (doAdd)
                            {
                                bool added = data.AddSoundToSpiSound(snd, CurrentMidiChannel(), startNote, endNote, center);
                                if (!added)
                                {
                                    MessageBox.Show("Notes overlap!");
                                }
                                else
                                {
                                    UpdateSpiSoundListBox();

                                    if (defaultMidiMapRadioButton.Checked)
                                    {
                                        int ch = data.GetNextFreeMidiChannel();
                                        if (ch > 0) SetMidiChannel(ch);
                                    }

                                    UpdateTotalSize();
                                    Undo.RegisterUndoChange(data);
                                    dataNeedsSaving = true;
                                    SaveProjectSettings();
                                }
                            }
                        }
                        else // Drums and Multisample (one sample per note)
                        {
                            bool addOk = true;
                            SpiSound spiSnd = new SpiSound(snd);

                            int midiChannel = CurrentMidiChannel();
                            spiSnd.midiChannel = (byte)midiChannel;

                            if (GmPercMidiMappingRadioButton.Checked) // Always channel 10 when gmperc is chosen. 
                            {
                                byte startNote = PercussionNote(); // 0-127
                                spiSnd.midiNote = spiSnd.startNote = spiSnd.endNote = startNote;

                                if (data.IsDrumSoundOccupied(startNote))
                                {
                                    addOk = false;
                                    MessageBox.Show("Drum sound " + spiSnd.midiNote.ToString() + " already occupied!");
                                }
                            }
                            else // multisample
                            {
                                byte startNote = Utility.ParseMidiTone(midiToneTextBox.Text);
                                spiSnd.midiNote = spiSnd.startNote = spiSnd.endNote = startNote;

                                if (data.IsMidiChannelOccupied(midiChannel)) // 1-16
                                {
                                    addOk = false;
                                    MessageBox.Show("MIDI channel " + midiChannel.ToString() + " already occupied!");
                                }
                            }

                            if (addOk)
                            {
                                data.AddSpiSound(spiSnd);
                                UpdateSpiSoundListBox();

                                if (GmPercMidiMappingRadioButton.Checked)
                                {
                                    int idx = drumsComboBox1.SelectedIndex;
                                    if (idx < drumsComboBox1.Items.Count - 1)
                                    {
                                        drumsComboBox1.SelectedIndex = idx + 1;
                                    }
                                }

                                UpdateTotalSize();
                                Undo.RegisterUndoChange(data);
                                dataNeedsSaving = true;
                                SaveProjectSettings();
                            }
                        }
                    }
                }
            }
            else // This is for patches with Program Change!
            {
                if (sounds.Count > 1)
                {
                    bool doAdd = true;
                    byte pcNumber = (byte)(CurrentMidiChannel() - 1); // We are now in program change mode
                    if (data.IsProgramChangeOccupied(pcNumber))
                    {
                        MessageBox.Show("Program Change number " + pcNumber.ToString() + " already occupied!");
                        doAdd = false;
                    }
                    if (doAdd)
                    {
                        foreach (Sound sound in sounds)
                        {
                            Sound s = sound;
                            SpiSound spiSnd = new SpiSound(s)
                            {
                                midiChannel = 128,
                                startNote = 60,
                                endNote = 108,
                                midiNote = 84,
                                programNumber = pcNumber
                            };
                            data.AddSpiSound(spiSnd);

                            pcNumber = (byte)data.GetNextFreeProgramChange(); // in 0-127 range
                            if (pcNumber > 0) SetProgramChange(pcNumber + 1); // in 1-128 range
                        }

                        UpdateSpiSoundListBox();
                        UpdateTotalSize();
                        Undo.RegisterUndoChange(data);
                        dataNeedsSaving = true;
                        SaveProjectSettings();
                    }
                }
                else if (sounds.Count == 1)
                {
                    Sound s = GetSoundAtSelectedIndex();
                    if (s != null)
                    {
                        bool doAdd = true;
                        byte pcNumber = (byte)(CurrentMidiChannel() - 1); // We are now in program change mode
                        if (data.IsProgramChangeOccupied(pcNumber)) {
                            MessageBox.Show("Program Change number " + pcNumber.ToString() + " already occupied!");
                            doAdd = false;
                        }

                        if (doAdd)
                        {
                            SpiSound spiSnd = new SpiSound(s)
                            {
                                midiChannel = 128,
                                startNote = 60,
                                endNote = 108,
                                midiNote = 84,
                                programNumber = pcNumber
                            };

                            data.AddSpiSound(spiSnd);

                            pcNumber = (byte)data.GetNextFreeProgramChange(); // in 0-127 range
                            if (pcNumber > 0) SetProgramChange(pcNumber + 1); // in 1-128 range

                            UpdateSpiSoundListBox();
                            UpdateTotalSize();
                            Undo.RegisterUndoChange(data);
                            dataNeedsSaving = true;
                            SaveProjectSettings();
                        }
                    }
                }
            }
        }


        private void DeleteSpiSoundButton_Click(object sender, EventArgs e)
        {
            DeleteSelectedSpiSound();
        }


        private void NewProjectToolStripMenuItem_Click_1(object sender, EventArgs e)
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
                InitEpssEditorData(forceNewProject: true);
                Undo.New(data);
                dataNeedsSaving = true;
                SaveProjectSettings();
            }
        }


        private void SaveProjectToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveProjectSettingsFileDialog();
        }


        private void SaveProjectToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            SaveProjectSettings();
        }


        private void LoadProjectToolStripMenuItem_Click(object sender, EventArgs e)
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
                LoadProjectSettingsFileDialog();
            }
        }


        private void LoadSPIToolStripMenuItem_Click(object sender, EventArgs e)
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
                bool result = DoLoadSpiFileDialog(out string errorMessage);
                if (!result && !String.IsNullOrEmpty(errorMessage))
                {
                    MessageBox.Show("SPI file cannot be loaded:\n" + errorMessage);
                }
            }
        }


        private void ImportSFZToolStripMenuItem_Click(object sender, EventArgs e)
        {
            bool result = DoLoadSfzFileDialog(out string errorMessage);
            if (!result && !String.IsNullOrEmpty(errorMessage))
            {
                MessageBox.Show("SFZ file cannot be loaded:\n" + errorMessage);
            }
        }


        private void ExitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Close();
        }


        private void NormalizeTrackBar_Scroll(object sender, EventArgs e)
        {
            if (callbacks)
            {
                normalizeTextBox.Text = normalizeTrackBar.Value.ToString() + "%";
                Sound snd = GetSoundAtSelectedIndex();
                snd?.updateNormalize(normalizeCheckBox.Checked, normalizeTrackBar.Value);
            }
        }


        private void NormalizeCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            if (callbacks)
            {
                Sound snd = GetSoundAtSelectedIndex();
                snd?.updateNormalize(normalizeCheckBox.Checked, normalizeTrackBar.Value);
                normalizeTrackBar.Enabled = normalizeCheckBox.Checked;
            }
        }


        private void SaveSFZToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DoSaveSfz();
        }


        private void ClearSettingsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string settingsFile = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.PerUserRoamingAndLocal).FilePath;
            if (MessageBox.Show("Settings files is stored:\n" + settingsFile + "\nDo you want to clear them?", "Confirm", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                Properties.Settings.Default.Reset();
                MessageBox.Show("Restart application with clear settings.");
            }
        }


        private void ShowMidiKeyboard(int midiChannel)
        {
            if (pianoKbForm == null)
            {
                pianoKbForm = new PianoKbForm(this, midiChannel)
                {
                    StartPosition = FormStartPosition.Manual
                };

                int xOffset = (this.Width - pianoKbForm.Width) / 2;
                Point p = this.Location + new Size(xOffset, this.Height);
                pianoKbForm.Location = p;
                EnsureVisible(pianoKbForm);
                pianoKbForm.Show(this);
            } else
            {
                //pianoKbForm.Focus();
            }
        }


        private void ShowMidiKeyboardForSelectedSpiSound()
        {
            List<int> selectedSounds = SelectedSpiSounds();
            if (selectedSounds.Count > 0)
            {
                SpiSound snd = data.SpiSoundAtIndex(selectedSounds.First());
                if (snd != null)
                {
                    int midiChannel = snd.midiChannel;
                    if (midiChannel >= 1 && midiChannel <= 16)
                    {
                        ShowMidiKeyboard(midiChannel);
                    }
                    else
                    {
                        MessageBox.Show("Invalid MIDI Channel in sound. Valid 1-16.");
                    }
                }
                else
                {
                    MessageBox.Show("Sound not possible to load.");
                }
            }
        }


        private void SpiSoundListenButton_Click(object sender, EventArgs e)
        {
            ShowMidiKeyboardForSelectedSpiSound();
        }


        internal void NotifyClosed(Form form)
        {
            pianoKbForm = null;
            spiSoundInstrument.AllNotesOff();
        }


        internal void NotifyAllNotesOff(Form form)
        {
            spiSoundInstrument.AllNotesOff();
        }


        private void EnsureVisible(Control ctrl)
        {
            Rectangle ctrlRect = ctrl.DisplayRectangle; //The dimensions of the ctrl
            ctrlRect.Y = ctrl.Top; //Add in the real Top and Left Vals
            ctrlRect.X = ctrl.Left;
            Rectangle screenRect = Screen.GetWorkingArea(ctrl); //The Working Area fo the screen showing most of the Ctrl

            //Now tweak the ctrl's Top and Left until it's fully visible. 
            ctrl.Left += Math.Min(0, screenRect.Left + screenRect.Width - ctrl.Left - ctrl.Width);
            ctrl.Left -= Math.Min(0, ctrl.Left - screenRect.Left);
            ctrl.Top += Math.Min(0, screenRect.Top + screenRect.Height - ctrl.Top - ctrl.Height);
            ctrl.Top -= Math.Min(0, ctrl.Top - screenRect.Top);
        }


        private void PreviewComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            data.previewSelected = previewComboBox.SelectedIndex;
            dataNeedsSaving = true;
            //SaveProjectSettings();
        }


        private void SaveSampleButton_Click(object sender, EventArgs e)
        {
            SaveSampleWithFileDialog();
        }


        private void SpiNameTextBox_TextChanged(object sender, EventArgs e)
        {
            data.spiName = spiNameTextBox.Text;
            dataNeedsSaving = true;
            Undo.RegisterUndoChange(data);
            //SaveProjectSettings();
        }


        private void SpiInfoTextBox_TextChanged(object sender, EventArgs e)
        {
            data.spiDescription = spiInfoTextBox.Text;
            dataNeedsSaving = true;
            Undo.RegisterUndoChange(data);
            //SaveProjectSettings();
        }


        private void OmniPatchCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            data.omni = omniPatchCheckBox.Checked;
            dataNeedsSaving = true;
            Undo.RegisterUndoChange(data);
            //SaveProjectSettings();
        }


        private void Gen2CheckBox_CheckedChanged(object sender, EventArgs e)
        {
            data.spiVersion = SpiVersion();
            //dataNeedsSaving = true;
            //SaveProjectSettings();
        }


        private void PlayButton_MouseDown(object sender, MouseEventArgs e)
        {
            _playingSounds = PlaySelectedSound();
        }


        private void PlayButton_MouseUp(object sender, MouseEventArgs e)
        {
            foreach (var sound in _playingSounds)
            {
                audio.StopSound(sound);
            }
        }


        private void MappingModeMidiRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            if (mappingModeMidiRadioButton.Checked)
            {
                int v = midiChTrackBar.Value;
                if (v > 16)
                {
                    SetMidiChannel(1);
                }
            }
            UpdateMappingMode();
        }


        private void MappingModeProgramRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            if (mappingModeMidiRadioButton.Checked)
            {
                int v = midiChTrackBar.Value;
                if (v > 16)
                {
                    SetMidiChannel(1);
                }
            }
            UpdateMappingMode();
        }


        private void SpiSoundListView_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            //Console.WriteLine(e.X + " " + e.Y);

            List<int> selected = SelectedSpiSounds();
            if (selected.Count > 0)
            {
                SpiSound snd = data.SpiSoundAtIndex(selected.First());
                if (snd != null)
                {
                    int midiChannel = snd.midiChannel;
                    int programChange = 128;
                    if (midiChannel == 128)
                    {
                        midiChannel = 1; // Program Change sounds
                        programChange = snd.programNumber;
                    }
                    ShowMidiKeyboard(midiChannel);
                    SetKeyAllOff();

                    ShowMidiChannel(midiChannel);
                    ShowNotes(midiChannel, snd.startNote, snd.endNote);
                    //for (int i = snd.startNote; i <= snd.endNote; i++) {
                      //  ShowNote(snd.midiChannel, i, true, true);
                    //}
                    int centerKey = snd.CenterNote();
                    ShowNote(midiChannel, centerKey, true, true);
                    SetCenterKey(centerKey);
                    int playNote = snd.startNote;
                    if (centerKey >= snd.startNote && centerKey <= snd.endNote) playNote = centerKey;
                    playNote = Math.Min(127, Math.Max(0, playNote));
                    PlayConvertedSound(snd, midiChannel, programChange, playNote);
                }
            }
        }


        private void SaveSPIToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DoSaveSpi();
        }


        private string RenameFormAt(Point p, string org)
        {
            RenameForm r = new RenameForm(org);
            {
                StartPosition = FormStartPosition.Manual;
            };
            r.Location = p;
            r.Size = new Size(40, 20);
            DialogResult res = r.ShowDialog();
            if (res == DialogResult.OK)
            {
                return r.GetText().Trim();
            }
            return "";
        }


        private void ToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            List<Sound> sounds = GetSelectedSounds();
            if (sounds.Count == 1)
            {
                Sound snd = sounds[0];
                /*
                ToolStripMenuItem b = (ToolStripMenuItem)sender;
                Point p = b.Owner.Location;
                string s = RenameFormAt(p, snd.name());
                */



                RenameForm r = new RenameForm(snd.name())
                {
                    StartPosition = FormStartPosition.Manual
                };
                ToolStripMenuItem b = (ToolStripMenuItem)sender;
                Point p = b.Owner.Location;
                r.Location = p;
                r.Size = new Size(40, 20);
                DialogResult res = r.ShowDialog();
                if (res == DialogResult.OK)
                {

                    string s = r.GetText().Trim();

                    if (!String.IsNullOrEmpty(s) && s != snd.name())
                    {
                        if (snd.Rename(s, out string errorString))
                        {
                            UpdateSoundListBox();
                            data.RefreshSpiSounds();
                            UpdateSpiSoundListBox();
                            Undo.New(data); // 
                            dataNeedsSaving = true;
                            SaveProjectSettings();
                        }
                        else
                        {
                            MessageBox.Show(errorString);
                        }
                    }
                }
            }
        }


        private void CustMidiToneFromTextBox_TextChanged(object sender, EventArgs e)
        {
            CustomSampleRadioButton.Checked = true;
        }


        private void CustMidiToneToTextBox_TextChanged(object sender, EventArgs e)
        {
            CustomSampleRadioButton.Checked = true;
        }


        private void SetKeyAllOff()
        {
            pianoKbForm?.SetKeyAllOff();
        }


        private void SetCenterKey(int note)
        {

            pianoKbForm?.SetCenterKey(note);
        }

        private void ShowNote(int midiChannel, int note, bool onOff, bool suppressEvent)
        {
            pianoKbForm?.NoteOnOff(midiChannel, note, onOff, suppressEvent);
        }


        private void ShowNotes(int midiChannel, int startNote, int endNote)
        {
            pianoKbForm?.ShowNotes(midiChannel, startNote, endNote);
        }


        private void ShowMidiChannel(int midiChannel)
        {
            pianoKbForm?.SetMidiChannel(midiChannel);
        }


        internal void PianoBox1_PianoKeyDown(object sender, M.PianoKeyEventArgs args, int midiChannel, int velocity)
        {
            byte note = (byte)args.Key;
            spiSoundInstrument.NoteOn(midiChannel, note, velocity);
        }


        internal void PianoBox1_PianoKeyUp(object sender, M.PianoKeyEventArgs args, int midiChannel)
        {
            byte note = (byte)args.Key;
            spiSoundInstrument.NoteOff(midiChannel, note);
        }


        private void SpiSoundInstrument_NoteOffEvent(object sender, SpiSoundInstrumentEventArgs e)
        {
            ShowNote(e.midiChannel, e.note, false, true);
        }


        private void SpiSoundInstrument_NoteOnEvent(object sender, SpiSoundInstrumentEventArgs e)
        {
            ShowNote(e.midiChannel, e.note, true, true);
        }


        private void LoadMidButton_Click(object sender, EventArgs e)
        {
            string midFile = Properties.Settings.Default.MidFile;
            loadMidFileDialog.FileName = midFile;
            if (loadMidFileDialog.ShowDialog() == DialogResult.OK)
            {
                MidPlayer.tickNum = -1;
                UpdateSongPosition();
                midFile = loadMidFileDialog.FileName;
                if (MidPlayer.LoadMidFile(midFile))
                {
                    StopPlayingMid();
                    Properties.Settings.Default.MidFile = midFile;
                    Properties.Settings.Default.Save();
                    playMidButton.Focus();
                    StartPlayingMid();
                } else
                {
                    MessageBox.Show("Mid file cannot be loaded.");
                }
            }
        }


        private void StopMidButton_Click(object sender, EventArgs e)
        {
            if (!MidPlayer.isPlaying)
            {
                MidPlayer.InitPlaying();
                UpdateSongPosition();
            }
            else
            {
                StopPlayingMid();
            }
        }


        private void PlayMidButton_Click(object sender, EventArgs e)
        {
            StartPlayingMid();
        }


        private void StopPlayingMid()
        {
            timer1.Stop();
            MidPlayer.StopPlaying();
            spiSoundInstrument.AllNotesOff();
        }


        private void StartPlayingMid()
        {
            spiSoundInstrument.Init();
            MidPlayer.RegisterInstrument(spiSoundInstrument);
            if (!MidPlayer.isPlaying)
            {
                MidPlayer.StartPlaying();
                timer1.Start();
            }
        }


        private void CheckForUpdatesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CheckUpdates.CheckForApplicationUpdate(this, GetRunningVersion(), inStart: false);
        }


        private void Timer1_Tick(object sender, EventArgs e)
        {
            UpdateSongPosition();
        }


        private void UpdateSongPosition()
        {
            StringBuilder sb = new StringBuilder();
            long tick = MidPlayer.tickNum;
            if (tick >= 0)
            {
                int denominator = MidPlayer.Denominator();

                long num = (long)(tick * Math.Pow(2, denominator)) / (long)(4 * MidPlayer.TicksPerQuarter() * MidPlayer.Numerator());
                long mod = (long)(tick * Math.Pow(2, denominator)) % (long)(4 * MidPlayer.TicksPerQuarter() * MidPlayer.Numerator());

                long bar = num + 1;
                long beat = mod / (4 * MidPlayer.TicksPerQuarter()) + 1;
                long pos = mod % (MidPlayer.TicksPerQuarter());

                sb.Append(bar.ToString().PadLeft(3, ' '));
                sb.Append(".");
                sb.Append(beat);
                sb.Append(".");
                sb.Append(pos.ToString().PadLeft(3, ' '));
            } else
            {
                sb.Append("-.-.-");
            }
            midFileBarTextBox.Text = sb.ToString();
        }


        private void RevMidButton_MouseDown(object sender, MouseEventArgs e)
        {
            spoolStep = -32;
            revMidTimer.Start();
            wasPlayingBeforeSpool = MidPlayer.isPlaying;
            StopPlayingMid();
        }


        private void RevMidTimer_Tick(object sender, EventArgs e)
        {
            MidPlayer.SpoolTick(spoolStep);
            UpdateSongPosition();
        }


        private void RevMidButton_MouseUp(object sender, MouseEventArgs e)
        {
            revMidTimer.Stop();
            if (wasPlayingBeforeSpool) StartPlayingMid();
        }


        private void FfwMidButton_MouseDown(object sender, MouseEventArgs e)
        {
            spoolStep = 32;
            revMidTimer.Start();
            wasPlayingBeforeSpool = MidPlayer.isPlaying;
            StopPlayingMid();
        }


        private void FfwMidButton_MouseUp(object sender, MouseEventArgs e)
        {
            revMidTimer.Stop();
            if (wasPlayingBeforeSpool) StartPlayingMid();
        }


        private bool HandleTransportKeyDown(Keys key)
        {
            bool handled = false;
            if (key == Keys.NumPad0 || key == Keys.Insert)
            { // Numeric 0
                StopPlayingMid();
                stopMidButton.Focus();
                handled = true;
            }
            else if (key == Keys.Space)
            {
                if (MidPlayer.isPlaying)
                {
                    StopPlayingMid();
                    stopMidButton.Focus();
                    handled = true;
                } else
                {
                    StartPlayingMid();
                    playMidButton.Focus();
                }
            }
            else if (key == Keys.Enter)
            { // Enter
                StartPlayingMid();
                playMidButton.Focus();
            }
            else if (key == Keys.Subtract)
            {
                if (!revMidTimer.Enabled)
                {
                    spoolStep = -32;
                    revMidTimer.Start();
                    wasPlayingBeforeSpool = MidPlayer.isPlaying;
                    StopPlayingMid();
                    handled = true;
                    revMidButton.Focus();
                }
            }
            else if (key == Keys.Add)
            {
                if (!revMidTimer.Enabled)
                {
                    spoolStep = 32;
                    revMidTimer.Start();
                    wasPlayingBeforeSpool = MidPlayer.isPlaying;
                    StopPlayingMid();
                    handled = true;
                    ffwMidButton.Focus();
                }
            }
            return handled;
        }


        private bool HandleTransportKeyUp()
        {
            bool handled = false;
            if (revMidTimer.Enabled)
            {
                revMidTimer.Stop();
                if (wasPlayingBeforeSpool)
                {
                    StartPlayingMid();
                    playMidButton.Focus();
                } else
                {
                    stopMidButton.Focus();
                }
                handled = true;
            }
            return handled;
        }


        private void PlayMidButton_KeyDown(object sender, KeyEventArgs e)
        {
            if (HandleTransportKeyDown(e.KeyCode)) e.Handled = true;
        }


        private void StopMidButton_KeyDown(object sender, KeyEventArgs e)
        {
            if (HandleTransportKeyDown(e.KeyCode)) e.Handled = true;
        }


        private void RevMidButton_KeyDown(object sender, KeyEventArgs e)
        {
            if (HandleTransportKeyDown(e.KeyCode)) e.Handled = true;
        }


        private void FfwMidButton_KeyDown(object sender, KeyEventArgs e)
        {
            if (HandleTransportKeyDown(e.KeyCode)) e.Handled = true;
        }


        private void RevMidButton_KeyUp(object sender, KeyEventArgs e)
        {
            if (HandleTransportKeyUp()) e.Handled = true;
        }


        private void FfwMidButton_KeyUp(object sender, KeyEventArgs e)
        {
            if (HandleTransportKeyUp()) e.Handled = true;
        }

    }
}