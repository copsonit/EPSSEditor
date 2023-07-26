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
using System.Net.Mail;
using NAudio.Midi;
using System.Diagnostics.Eventing.Reader;
using M;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.StartPanel;
using System.Runtime.Remoting.Messaging;
using System.Text.RegularExpressions;

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
                    data.initialize(DrumMappingsFileName());
                    updateDialog();
                    saveProjectSettings();
                }
            }
            int newFreq = frequencyFromCompressionTrackBar(1); // TODO settings to set freq for midi player instrument
            spiSoundInstrument = new SpiSoundInstrument(GetEPSSEditorDataCallBack, audio, newFreq);
            spiSoundInstrument.NoteOnEvent += SpiSoundInstrument_NoteOnEvent;
            spiSoundInstrument.NoteOffEvent += SpiSoundInstrument_NoteOffEvent;

            defaultMidiMapRadioButton.Checked = true;
            mappingModeMidiRadioButton.Checked = true;
            setMidiChannel(1);
            updateTotalSize();
            CheckForUpdate();
        }


        private void CheckForUpdate(bool inStart=true)
        {          
            EPSSEditorAktuell akt = KollaEfterNyVersion.KollaEfterNy(@"https://copson.se/epss/wp-content/uploads/EPSSEditorCurrentVersionInfo2.xml");
            if (akt == null) return;

            int nMajor = akt.aktuell.maj;
            int nMinor = akt.aktuell.min;
            int nBuild = akt.aktuell.bld;
            string version = nMajor + "." + nMinor + "." + nBuild;
           
            if (inStart)
            {
                string ignore = Properties.Settings.Default.IgnoreVersion;
                if (!String.IsNullOrEmpty(ignore))
                {

                    if (version == ignore)
                    {
                        return;
                    }
                }
            }

            string strPath = akt.folder;

            // Get my own version's numbers
            Assembly assembly = Assembly.GetExecutingAssembly();
            FileVersionInfo fileVersionInfo = FileVersionInfo.GetVersionInfo(assembly.Location);

            int nAppMajor = fileVersionInfo.FileMajorPart;
            int nAppMinor = fileVersionInfo.FileMinorPart;
            int nAppBuild = fileVersionInfo.FileBuildPart;           

            if (nMajor > nAppMajor || (nMajor == nAppMajor && nMinor > nAppMinor) || (nMajor == nAppMajor && nMinor == nAppMinor && nBuild > nAppBuild))
            {
                string link = strPath;
                string updateMsg = "EPSS Editor v" + version + " released.";
                UpdateAvailable form = new UpdateAvailable(this, updateMsg, link, version, inStart);
                
                form.ShowDialog();
            } else if (!inStart)
            {
                MessageBox.Show("You are already running latest.");
            }          
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


        private void exit()
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
            if (audio != null)
            {
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
            UpdateSpiSoundButtons();
            updatePreview();
            updateMappingMode();
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

            deleteSoundButton.Enabled = soundListBox.SelectedItems.Count > 0;
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
                int nr = data.getSoundNumberFromGuid(s.soundId);
                item.SubItems.Add(nr.ToString());

                item.SubItems.Add(Ext.ToPrettySize(s.preLength(data), 2));
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

            Sound snd = getSoundAtSelectedIndex();
            if (snd != null)
            {
                List<SpiSound> spiSounds = data.getSpiSoundsFromSound(snd);
                deleteSoundButton.Enabled = spiSounds.Count == 0;

            }

            spiNameTextBox.Text = data.spiName;
            spiInfoTextBox.Text = data.spiDescription;
            omniPatchCheckBox.Checked = data.omni;
            gen2CheckBox.Checked = data.HasAnyProgramChange();

            string errorString;
            bool spiSaveEnabled = data.IsValidForSpiExport(out errorString);
            var mi = menuStrip1.Items.Find("saveSPIToolStripMenuItem", true);
            foreach (var item in mi)
            {
                item.Enabled = spiSaveEnabled;
            }
        }


        private void updateTotalSize()
        {
            if (data != null)
            {
                EPSSSpiCreator creator = new EPSSSpiCreator(spiVersion());
                long sz = creator.length(data);
                totalSizeTextBox.Text = Ext.ToPrettySize(sz, 2);

                int v = (int)(sz / 1024);
                totalSizeProgressBar.Value = Math.Min(14000, v); // Only show up to max 14MB but we support unlimited size..
            }
        }


        private void updateAfterSoundChange(Sound snd, int toFreq)
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

            updateShowCompressionProgressBar(snd.sampleDataLength, lengthAfter);

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
            if (lengthBefore > 0)
            {
                int v = (int)(((double)lengthAfter / (double)lengthBefore) * 100);
                showCompProgressBar.Value = v;
            }
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

                string newDir = "";
                string sampleNotFound = "";
                bool clearAllSamples = false;
                while (true)
                {
                    bool vf = data.VerifyFiles(newDir, out sampleNotFound);

                    if (!vf)
                    {
                        int verifyResult = VerifyFiles(sampleNotFound, ref newDir);
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
                        data.sounds.Clear();
                        data.ClearSpiSounds();

                    }
                    else
                    {
                        result = false;
                    }
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


        private int VerifyFiles(string sampleNotFound, ref string newDir)
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


        private bool loadSpiFile(string file, out string errorMessage)
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

                if (WarnAndConfirmDir(path, "Directory for conversion of SPI already exists.\nDo you want to delete it?"))
                {
                    data = new EPSSEditorData();
                    data.initialize(DrumMappingsFileName());

                    result = data.LoadSpiFile(spi, path, out errorMessage);
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


        private bool doLoadSpiFileDialog(out string errorMessage)
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
                result = loadSpiFile(file, out errorMessage);
            }

            return result;
        }


        private void doSaveSpi()
        {
            string errorString;
            if (data.IsValidForSpiExport(out errorString))
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

                    EPSSSpi spi = creator.create(data, spiNameTextBox.Text, spiInfoTextBox.Text, sampFreq);

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
                System.Windows.Forms.MessageBox.Show("Can not save SPI:" + errorString);
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

                deleteSoundButton.Enabled = soundListBox.SelectedItems.Count > 0;
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


        private List<CachedSound> playSelectedSound(bool forceLoopOff=false)
        {
            List<CachedSound> soundsPlaying = new List<CachedSound>();
            try
            {
                List<Sound> sounds = getSelectedSounds();
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


        private bool soundRefersToSPISound(int idx)
        {
            Sound snd = data.sounds[idx];
            List<SpiSound> spiSounds = data.getSpiSoundsFromSound(snd);
            return spiSounds.Count > 0;
        }


        private void deleteSelectedSpiSound()
        {
            List<int> idxRemoved = SelectedSpiSounds();
            if (idxRemoved.Count > 0)
            {
                int removed = 0;
                int lastRemoved = -1;
                foreach (int index in idxRemoved)
                {
                    lastRemoved = (index - removed);
                    data.RemoveSpiSound(index - removed);
                    removed++;
                }
                dataNeedsSaving = true;

                updateSpiSoundListBox();
                if (spiSoundListView.Items.Count > 0)
                {
                    int select = Math.Min(lastRemoved, spiSoundListView.Items.Count - 1);
                    select = Math.Max(0, select);
                    spiSoundListView.Items[select].Selected = true;
                    spiSoundListView.Items[select].Focused = true;
                    spiSoundListView.Items[select].EnsureVisible();
                }
                saveProjectSettings();
                updateTotalSize();
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


        private void playConvertedSound(int midiChannel, int programChange, int note)
        {
            if (programChange < 128)
            {
                spiSoundInstrument.ProgramChange(midiChannel, programChange);
            }
            spiSoundInstrument.NoteOn(midiChannel, note, 127);
        }


        private void saveSampleWithFileDialog()
        {
            List<int> selectedSpiSounds = SelectedSpiSounds();
            if (selectedSpiSounds.Count > 0)
            {
                foreach (int selected in selectedSpiSounds)
                {
                    if (selected >= 0)
                    {
                        saveSampleFileDialog.InitialDirectory = Path.GetDirectoryName(data.soundFileName);
                        if (saveSampleFileDialog.ShowDialog() == DialogResult.OK)
                        {
                            string outFile = saveSampleFileDialog.FileName;
                            SpiSound snd = data.SpiSoundAtIndex(selected);
                            if (!snd.convertSound(data, outFile, frequencyFromCompressionTrackBar(compressionTrackBar.Value), AtariConstants.SampleBits, AtariConstants.SampleChannels))
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

                List<string> filesAdded = new List<string>();

                int cm = currentMidiChannel();
                int midiChannel = mappingModeMidiRadioButton.Checked ? cm : 128;
                int programChange = mappingModeProgramRadioButton.Checked ? cm - 1 : 128;

                string anyFile = SfzConverter.LoadSfzSound(data, midiChannel, programChange, file, filesAdded);
                if (filesAdded.Count > 0)
                {
                    int ch = midiChannel < 128 ? data.getNextFreeMidiChannel() : data.getNextFreeProgramChange() + 1;
                    if (ch > 0) setMidiChannel(ch);
                    UpdateAfterSoundsAdded(filesAdded, anyFile, true);
                    data.soundFileName = anyFile;
                    result = true;
                }
                else
                {
                    errorMessage = "Nothing was loaded. Files already exists.";
                }
            }
            return result;
        }

 
        private void DoSaveSfz()
        {
            string sfzFile = SfzExportFileName();
            string sampleSubDir = "samples";
            string sfzDir, sampleDir, name;
            if (CheckSfzDirectories(sfzFile, sampleSubDir, out name, out sfzDir, out sampleDir))
            {
                Dictionary<int, List<SfzSplitInfo>> dict = data.ConvertToSfzSplitInfoForSfzExport();
                SfzConverter c = new SfzConverter();
                string errorMessage = "";
                bool result = c.SaveSFZ(dict, data.sounds, sfzDir, sampleSubDir, name, out errorMessage);
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
                if (System.Windows.Forms.MessageBox.Show("Directory " + name + " already exists here.\nDo you want to delete it?", "Confirm", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
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


        private string addOneSoundDirect(string file)
        {
            Sound s = new Sound(file);
            data.sounds.Add(s);
            return file;
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


        private void setProgramChange(int pc) // 1-128
        {
            updateMappingMode();
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
                    SpiSound snd = data.SpiSoundAtIndex(item.Index);
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
                    SpiSound snd = data.SpiSoundAtIndex(item.Index);
                    snd.transpose = 0;
                    updateListViewItemValue(snd, item, 7);
                    Console.WriteLine($"Item: {item}");
                }
            }
        }


        private void spiSoundListView_ItemSelectionChanged(object sender, ListViewItemSelectionChangedEventArgs e)
        {
            UpdateSpiSoundButtons();
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
            bool spiNeedsUpdate = false;
            foreach (var file in files)
            {
                string filePath = file;
                string ext = Path.GetExtension(filePath).ToUpper();
                if (ext == ".SFZ")
                {
                    int cm = currentMidiChannel();
                    int midiChannel = mappingModeMidiRadioButton.Checked ? cm : 128;
                    int programChange = mappingModeProgramRadioButton.Checked ? cm -1  : 128;
                    anyFile = SfzConverter.LoadSfzSound(data, midiChannel, programChange, filePath, filesAdded);
                    if (filesAdded.Count > 0)
                    {
                        int ch = midiChannel < 128 ? data.getNextFreeMidiChannel() : data.getNextFreeProgramChange() + 1;
                        if (ch > 0) setMidiChannel(ch);
                        spiNeedsUpdate = true;
                        Properties.Settings.Default.SfzFile = filePath;
                        Properties.Settings.Default.Save();
                    }
                }
                else if (ext == ".WAV")
                {
                    anyFile = addOneSoundDirect(filePath);
                    filesAdded.Add(anyFile);
                }
                else
                {
                    MessageBox.Show("Only support .WAV or .SFZ format.");
                    break;
                }
            }

            if (filesAdded.Count > 0)
            {
                UpdateAfterSoundsAdded(filesAdded, anyFile, spiNeedsUpdate);
            } else
            {
                MessageBox.Show("Nothing was loaded. Files already exists.");
            }
        }


        private void UpdateAfterSoundsAdded(List<string> filesAdded, string anyFile, bool spiNeedsUpdate)
        {
            if (filesAdded.Count > 0)
            {
                updateSoundListBox();
                if (spiNeedsUpdate) updateSpiSoundListBox();

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
                soundListBox.EndUpdate();
            }

            UpdateSoundDialog();
            UpdateConversionSettings();
            data.soundFileName = anyFile;
            dataNeedsSaving = true;
            saveProjectSettings();
        }


        private void soundListBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == ' ')
            {
                playSelectedSound(forceLoopOff:true);
                e.Handled = true;
            }
            if (ctrlAPressed) e.Handled = true;
        }


        private void soundListBox_KeyDown(object sender, KeyEventArgs e)
        {
            ctrlAPressed = false;
            if (e.KeyCode == Keys.Delete)
            {
                deleteSelectedSound();
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
                updateAfterSoundChange(snd, toFreq);
                compressionTrackBar.Value = compressionTrackBarValueFromFrequency(toFreq);
                useInSpiButton.Enabled = true;
                List<SpiSound> spiSounds = data.getSpiSoundsFromSound(snd);
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


        private void loadSoundButton_Click(object sender, EventArgs e)
        {
            loadSound();
        }


        private void deleteSoundButton_Click(object sender, EventArgs e)
        {
            deleteSelectedSound();
        }


        private void compressionTrackBar_Scroll(object sender, EventArgs e)
        {
            Sound snd = getSoundAtSelectedIndex();
            if (snd != null)
            {
                int toFreq = frequencyFromCompressionTrackBar(compressionTrackBar.Value);
                updateAfterSoundChange(snd, toFreq);
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

                            int midiChannel = currentMidiChannel();
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

                            updateAfterSoundChange(snd, toFreq);
                            compressionTrackBar.Value = compressionTrackBarValueFromFrequency(toFreq);

                            if (!defaultMidiMapRadioButton.Checked)
                            {
                                data.removeSpiSound(spiSnd.midiChannel, spiSnd.midiNote);
                            }
                            data.AddSpiSound(spiSnd);

                            if (defaultMidiMapRadioButton.Checked)
                            {
                                int ch = data.getNextFreeMidiChannel();
                                if (ch > 0) setMidiChannel(ch);
                            }
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
                    // Map single sound
                    Sound snd = getSoundAtSelectedIndex();
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
                                bool added = data.AddSoundToSpiSound(snd, currentMidiChannel(), startNote, endNote, center);
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
                            SpiSound spiSnd = new SpiSound(snd);

                            int midiChannel = currentMidiChannel();
                            spiSnd.midiChannel = (byte)midiChannel;

                            if (GmPercMidiMappingRadioButton.Checked) // Always channel 10 when gmperc is chosen. 
                            {
                                byte startNote = percussionNote(); // 0-127
                                spiSnd.midiNote = spiSnd.startNote = spiSnd.endNote = startNote;

                                if (data.isDrumSoundOccupied(startNote))
                                {
                                    addOk = false;
                                    System.Windows.Forms.MessageBox.Show("Drum sound " + spiSnd.midiNote.ToString() + " already occupied!");
                                }
                            }
                            else // multisample
                            {
                                byte startNote = Utility.ParseMidiTone(midiToneTextBox.Text);
                                spiSnd.midiNote = spiSnd.startNote = spiSnd.endNote = startNote;

                                if (data.isMidiChannelOccupied(midiChannel)) // 1-16
                                {
                                    addOk = false;
                                    System.Windows.Forms.MessageBox.Show("MIDI channel " + midiChannel.ToString() + " already occupied!");
                                }
                            }

                            if (addOk)
                            {
                                data.AddSpiSound(spiSnd);
                                updateSpiSoundListBox();

                                if (GmPercMidiMappingRadioButton.Checked)
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
            else // This is for patches with Program Change!
            {
                if (sounds.Count > 1)
                {
                    bool doAdd = true;
                    byte pcNumber = (byte)(currentMidiChannel() - 1); // We are now in program change mode
                    if (data.isProgramChangeOccupied(pcNumber))
                    {
                        MessageBox.Show("Program Change number " + pcNumber.ToString() + " already occupied!");
                        doAdd = false;
                    }
                    if (doAdd)
                    {
                        foreach (Sound sound in sounds)
                        {
                            Sound s = sound;
                            SpiSound spiSnd = new SpiSound(s);

                            spiSnd.midiChannel = 128;
                            spiSnd.startNote = 60;
                            spiSnd.endNote = 108;
                            spiSnd.midiNote = 84;
                            spiSnd.programNumber = pcNumber;
                            data.AddSpiSound(spiSnd);

                            pcNumber = (byte)data.getNextFreeProgramChange(); // in 0-127 range
                            if (pcNumber > 0) setProgramChange(pcNumber + 1); // in 1-128 range
                        }

                        updateSpiSoundListBox();
                        dataNeedsSaving = true;
                        saveProjectSettings();
                        updateTotalSize();
                    }
                }
                else if (sounds.Count == 1)
                {
                    Sound s = getSoundAtSelectedIndex();
                    if (s != null)
                    {
                        bool doAdd = true;
                        byte pcNumber = (byte)(currentMidiChannel() - 1); // We are now in program change mode
                        if (data.isProgramChangeOccupied(pcNumber)) {
                            MessageBox.Show("Program Change number " + pcNumber.ToString() + " already occupied!");
                            doAdd = false;
                        }

                        if (doAdd)
                        {
                            SpiSound spiSnd = new SpiSound(s);
                            spiSnd.midiChannel = 128;
                            spiSnd.startNote = 60;
                            spiSnd.endNote = 108;
                            spiSnd.midiNote = 84;
                            spiSnd.programNumber = pcNumber;

                            data.AddSpiSound(spiSnd);

                            pcNumber = (byte)data.getNextFreeProgramChange(); // in 0-127 range
                            if (pcNumber > 0) setProgramChange(pcNumber + 1); // in 1-128 range

                            updateSpiSoundListBox();
                            dataNeedsSaving = true;
                            saveProjectSettings();
                            updateTotalSize();
                        }
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
                bool result = doLoadSpiFileDialog(out errorMessage);
                if (!result && !String.IsNullOrEmpty(errorMessage))
                {
                    MessageBox.Show("SPI file cannot be loaded:\n" + errorMessage);
                }
            }
        }


        private void importSFZToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string errorMessage;
            bool result = DoLoadSfzFileDialog(out errorMessage);
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
                snd?.updateNormalize(normalizeCheckBox.Checked, normalizeTrackBar.Value);
            }
        }


        private void normalizeCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            if (callbacks)
            {
                Sound snd = getSoundAtSelectedIndex();
                snd?.updateNormalize(normalizeCheckBox.Checked, normalizeTrackBar.Value);
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


        private void ShowMidiKeyboard(int midiChannel)
        {
            if (pianoKbForm == null)
            {
                pianoKbForm = new PianoKbForm(this, midiChannel);
                pianoKbForm.StartPosition = FormStartPosition.Manual;

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


        private void spiSoundListenButton_Click(object sender, EventArgs e)
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


        private void gen2CheckBox_CheckedChanged(object sender, EventArgs e)
        {
            data.spiVersion = spiVersion();
            dataNeedsSaving = true;
            saveProjectSettings();
        }


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
                    playConvertedSound(midiChannel, programChange, playNote);
                }
            }
        }


        private void saveSPIToolStripMenuItem_Click(object sender, EventArgs e)
        {
            doSaveSpi();
        }


        private void toolStripMenuItem1_Click(object sender, EventArgs e)
        {
            List<Sound> sounds = getSelectedSounds();
            if (sounds.Count == 1)
            {
                Sound snd = sounds[0];
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
                        if (snd.Rename(s, out string errorString))
                        {
                            updateSoundListBox();
                            data.RefreshSpiSounds();
                            updateSpiSoundListBox();
                            dataNeedsSaving = true;
                            saveProjectSettings();
                        }
                        else
                        {
                            MessageBox.Show(errorString);
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


        internal void pianoBox1_PianoKeyDown(object sender, M.PianoKeyEventArgs args, int midiChannel, int velocity)
        {
            byte note = (byte)args.Key;
            spiSoundInstrument.NoteOn(midiChannel, note, velocity);
        }


        internal void pianoBox1_PianoKeyUp(object sender, M.PianoKeyEventArgs args, int midiChannel)
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


        private void loadMidButton_Click(object sender, EventArgs e)
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


        private void stopMidButton_Click(object sender, EventArgs e)
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


        private void playMidButton_Click(object sender, EventArgs e)
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


        private void checkForUpdatesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CheckForUpdate(inStart: false);
        }


        private void timer1_Tick(object sender, EventArgs e)
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


        private void revMidButton_MouseDown(object sender, MouseEventArgs e)
        {
            spoolStep = -32;
            revMidTimer.Start();
            wasPlayingBeforeSpool = MidPlayer.isPlaying;
            StopPlayingMid();
        }


        private void revMidTimer_Tick(object sender, EventArgs e)
        {
            MidPlayer.SpoolTick(spoolStep);
            UpdateSongPosition();
        }


        private void revMidButton_MouseUp(object sender, MouseEventArgs e)
        {
            revMidTimer.Stop();
            if (wasPlayingBeforeSpool) StartPlayingMid();
        }


        private void ffwMidButton_MouseDown(object sender, MouseEventArgs e)
        {
            spoolStep = 32;
            revMidTimer.Start();
            wasPlayingBeforeSpool = MidPlayer.isPlaying;
            StopPlayingMid();
        }


        private void ffwMidButton_MouseUp(object sender, MouseEventArgs e)
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


        private void playMidButton_KeyDown(object sender, KeyEventArgs e)
        {
            if (HandleTransportKeyDown(e.KeyCode)) e.Handled = true;
        }


        private void stopMidButton_KeyDown(object sender, KeyEventArgs e)
        {
            if (HandleTransportKeyDown(e.KeyCode)) e.Handled = true;
        }


        private void revMidButton_KeyDown(object sender, KeyEventArgs e)
        {
            if (HandleTransportKeyDown(e.KeyCode)) e.Handled = true;
        }


        private void ffwMidButton_KeyDown(object sender, KeyEventArgs e)
        {
            if (HandleTransportKeyDown(e.KeyCode)) e.Handled = true;
        }


        private void revMidButton_KeyUp(object sender, KeyEventArgs e)
        {
            if (HandleTransportKeyUp()) e.Handled = true;
        }


        private void ffwMidButton_KeyUp(object sender, KeyEventArgs e)
        {
            if (HandleTransportKeyUp()) e.Handled = true;
        }
    }
}