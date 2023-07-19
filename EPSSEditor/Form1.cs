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
        private ControlScrollListener _processListViewScrollListener;
        private AudioPlaybackEngine audio = null;
        private SpiSoundInstrument spiSoundInstrument;
        private PianoKbForm pianoKbForm = null;

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
                    item.SubItems.Add((s.programNumber+1).ToString());
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
            string errorString;
            bool spiSaveEnabled = data.IsValidForSpiExport(out errorString);
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
            gen2CheckBox.Checked = data.spiVersion == 2;
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
                    bool vf = data.VerifyFiles(newDir, ref sampleNotFound);

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
            //if (!data.VerifyFiles(newDir))
            //{
            if (MessageBox.Show("Sample " + sampleNotFound + " cannot be found.\nDo you want to point to other directory for samples?", "EPSS Editor", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                //folderBrowserDialog1.SelectedPath = Properties.Settings.Default.ProjectFile;
                folderBrowserDialog1.ShowNewFolderButton = false;
                //folderBrowserDialog1.RootFolder = Environment.SpecialFolder.Personal;
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
            //}
            //return 0; // OK
        }


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
                    data = new EPSSEditorData();
                    data.initialize(DrumMappingsFileName());

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
            //List<SpiSound> soundsToSave = data.spiSounds;
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


        private void playConvertedSound(int midiChannel, int note)
        {
            spiSoundInstrument.NoteOn(midiChannel, note, 127);

            /*
            List<int> selectedSnds = SelectedSpiSounds();
            if (selectedSnds.Count > 0)
            {
                List<CachedSound> playedSounds = new List<CachedSound>();
                foreach (var selected in selectedSnds)
                {
                    int newFreq = frequencyFromCompressionTrackBar(compressionTrackBar.Value);
                    SpiSound snd = data.spiSounds[selected];
                    CachedSound cs = data.cachedSound(snd, newFreq, note, 127);
                    if (cs != null)
                    {
                        audio.PlaySound(cs);
                        playedSounds.Add(cs);
                    }
                }
                return playedSounds;
            }
            return null;
            */
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
            bool abortLoad = false;
            foreach (SfzBase bas in bases)
            {
                if (abortLoad) break;
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
                        if (Path.GetExtension(fp).ToLower() == ".wav")
                        {
                            s = new Sound(fp);
                            s.description = Path.GetFileNameWithoutExtension(fp);
                            data.sounds.Add(s);
                            sounds.Add(fp, s);
                        }
                        else
                        {
                            MessageBox.Show("Unsupported format for samples. Only supports WAV.");
                            abortLoad = true;
                            s = null;
                        }
                    }


                    string kcS = tBase.GetValue("pitch_keycenter");
                    byte kcByte = 0;
                    if (!String.IsNullOrEmpty(kcS))
                    {

                        //                    string kcS = tBase.variables[""];
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
                    }

                    //Sound s = new Sound(fp);
                    //s.description = baseName + Path.GetFileNameWithoutExtension(fp);
                    string loKeyS = tBase.GetValue("lokey");
                    byte loByte = 0;
                    if (!String.IsNullOrEmpty(loKeyS))
                    {

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
                    }
                    else
                    {
                        loByte = (byte)Math.Max(0, (int)kcByte - 24);
                    }
                    //s.loKey = loByte;



                    string hiKeyS = tBase.GetValue("hikey");
                    byte hiByte = 0;
                    if (!String.IsNullOrEmpty(hiKeyS))
                    {

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
                    }
                    else
                    {
                        hiByte = (byte)Math.Min(127, (int)kcByte + 24);
                    }
                    //s.hiKey = hiByte;



                    //s.keyCenter = kcByte;


                    //if (!data.IdenticalSoundExists(s))
                    //{
                    //data.sounds.Add(s);
                    //}
                    if (s != null)
                    {
                        anyFile = fp;
                        data.AddSfzSound(s, midiChannel, loByte, hiByte, kcByte, 0);

                    }

                }
            }

            updateDialog();
            dataNeedsSaving = true;
            saveProjectSettings();

            int ch = data.getNextFreeMidiChannel();
            if (ch > 0) setMidiChannel(ch);

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
                    try
                    {
                        Directory.Delete(sfzDir, true);
                    }
                    catch(Exception ex)
                    {
                        MessageBox.Show("Cannot delete directory as another process is locking it. Aborting save.");
                        return false;
                    }
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
            if (!TryToByte(s, out byte note))
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
            foreach (var file in files)
            {
                string filePath = file;
                string ext = Path.GetExtension(filePath).ToUpper();


                if (ext == ".SFZ")
                {
                    anyFile = LoadSfzSound(filePath);
                }
                else if (ext == ".WAV")
                {
                    string baseName = Path.GetFileNameWithoutExtension(filePath);
                    baseName = baseName.Substring(0, Math.Min(baseName.Length - 1, 13));
                    anyFile = addOneSoundDirect(filePath, baseName);
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
            UpdateConversionSettings();
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

                updateAfterSoundChange(ref snd, toFreq);
                compressionTrackBar.Value = compressionTrackBarValueFromFrequency(toFreq);

                useInSpiButton.Enabled = true;

                List<SpiSound> spiSounds = data.getSpiSoundsFromSound(ref snd);
                deleteSoundButton.Enabled = spiSounds.Count == 0;

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
                    // Map multiple sounds
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
                            SpiSound spiSnd = new SpiSound(s);

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
                    Sound s = getSoundAtSelectedIndex();
                    if (s != null)
                    {
                        if (defaultMidiMapRadioButton.Checked || CustomSampleRadioButton.Checked)
                        {
                            bool doAdd = true;
                            byte startNote = 60;
                            byte endNote = 108;
                            byte center = 84;
                            if (CustomSampleRadioButton.Checked)
                            {
                                startNote = parseMidiTone(custMidiToneFromTextBox.Text);
                                endNote = parseMidiTone(custMidiToneToTextBox.Text);
                                string centerText = custMidToneCentreTextBox.Text;
                                if (!String.IsNullOrEmpty(centerText)) center = parseMidiTone(centerText);
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
                                bool added = data.AddSoundToSpiSound(ref s, currentMidiChannel(), startNote, endNote, center);
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
                            SpiSound spiSnd = new SpiSound(s);

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
                                byte startNote = parseMidiTone(midiToneTextBox.Text);
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



        private void spiSoundListenButton_MouseDown(object sender, MouseEventArgs e)
        {

        }

        private void spiSoundListenButton_MouseUp(object sender, MouseEventArgs e)
        {

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

                //kb.Size = new Size(40, 20);
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
            //Console.WriteLine(e.X + " " + e.Y);

            List<int> selected = SelectedSpiSounds();
            if (selected.Count > 0)
            {
                SpiSound snd = data.SpiSoundAtIndex(selected.First());
                if (snd != null)
                {
                    int midiChannel = snd.midiChannel;
                    ShowMidiKeyboard(midiChannel);
                    SetKeyAllOff();

                    ShowMidiChannel(midiChannel);
                    ShowNotes(midiChannel, snd.startNote, snd.endNote);
                    //for (int i = snd.startNote; i <= snd.endNote; i++) {
                      //  ShowNote(snd.midiChannel, i, true, true);
                    //}
                    int centerKey = snd.CenterNote();
                    ShowNote(snd.midiChannel, centerKey, true, true);
                    SetCenterKey(centerKey);
                    int playNote = snd.startNote;
                    if (centerKey >= snd.startNote && centerKey <= snd.endNote) playNote = centerKey;
                    playNote = Math.Min(127, Math.Max(0, playNote));
                    playConvertedSound(snd.midiChannel, playNote);
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

        private void loadMidButton_Click(object sender, EventArgs e)
        {
            string midFile = Properties.Settings.Default.MidFile;
            loadMidFileDialog.FileName = midFile;
            if (loadMidFileDialog.ShowDialog() == DialogResult.OK)
            {
                midFile = loadMidFileDialog.FileName;
                MidPlayer.LoadMidFile(midFile);
                //int newFreq = frequencyFromCompressionTrackBar(compressionTrackBar.Value);
                //CachedSound cs = data.cachedSound(selected, newFreq);
                //SpiSoundInstrument spiSoundInstrument = new SpiSoundInstrument(data, audio, newFreq);

                MidPlayer.RegisterInstrument(spiSoundInstrument);
                //MidPlayer.StartPlaying(ref midPlayerTimer);
                MidPlayer.StartPlaying(this);

                Properties.Settings.Default.MidFile = midFile;
                Properties.Settings.Default.Save();
            }
        }


        private void SetKeyAllOff()
        {
            if (pianoKbForm != null)
            {
                pianoKbForm.SetKeyAllOff();
            }
        }


        private void SetCenterKey(int note)
        {
            if (pianoKbForm != null)
            {
                pianoKbForm.SetCenterKey(note);
            }
        }

        private void ShowNote(int midiChannel, int note, bool onOff, bool suppressEvent)
        {
            if (pianoKbForm != null)
            {
                pianoKbForm.NoteOnOff(midiChannel, note, onOff, suppressEvent);
            }
        }

        private void ShowNotes(int midiChannel, int startNote, int endNote)
        {
            if (pianoKbForm != null) {
                pianoKbForm.ShowNotes(midiChannel, startNote, endNote);
            }
        }

        private void ShowMidiChannel(int midiChannel)
        {
            if (pianoKbForm != null)
            {
                pianoKbForm.SetMidiChannel(midiChannel);
            }
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


        private void midPlayerTimer_Tick(object sender, EventArgs e)
        {
            MidPlayer.Tick();        
        }


        private void stopMidButton_Click(object sender, EventArgs e)
        {
            MidPlayer.StopPlaying();
            spiSoundInstrument.AllNotesOff();
        }

    }

    public delegate EPSSEditorData GetEPSSEditorDataCallBack();
    public class SpiSoundInstrument : MidiInstrument
    {
        //public EPSSEditorData data;

        private Dictionary<int, CachedSound[]> playingContext;

        private readonly AudioPlaybackEngine audio;

        private int newFreq;
        //private int testTone;

        //private CachedSound cachedCachedSound;

        private GetEPSSEditorDataCallBack _getEditorDataCallBack;

        public event EventHandler<SpiSoundInstrumentEventArgs> NoteOnEvent;
        public event EventHandler<SpiSoundInstrumentEventArgs> NoteOffEvent;
        

        public SpiSoundInstrument() { }


        public SpiSoundInstrument(GetEPSSEditorDataCallBack callback, AudioPlaybackEngine audio, int newFreq)
        {
            playingContext = new Dictionary<int, CachedSound[]>();
            for (int channel = 0; channel < 16; channel++)
            {
                playingContext.Add(channel, new CachedSound[128]);
            }
            //this.data = data;
            this.audio = audio;
            this.newFreq = newFreq;
            _getEditorDataCallBack = callback;

            /*
            SpiSound snd = data.FindSpiSound(1, 1);
            cachedCachedSound = data.cachedSound(snd, newFreq);
            testTone = 1;
            */
            //Sound snd = data.sounds[0];
            //cachedCachedSound = snd.cachedSound();
        }

        public override void NoteOn(int midiChannel, int note, int vel)
        {
            //Console.WriteLine($"Spi got Note on: {midiChannel} {note} {vel}");

            /*
            if (midiChannel == 1 && note == 36)
            {
                cachedCachedSound.pitch = Math.Pow(2, (double)(testTone-1) / 12.0);
                Console.WriteLine($"{cachedCachedSound.pitch}");
                PlaySound(cachedCachedSound, midiChannel, testTone++);
            }
            */
            if (_getEditorDataCallBack != null)
            {
                EPSSEditorData data = _getEditorDataCallBack();

                SpiSound snd = data.FindSpiSound(midiChannel, note);
                if (snd != null)
                {
                    //Console.WriteLine($"Found sound: {snd.name()}");
                    CachedSound cs = data.cachedSound(snd, newFreq, note, vel);

                    PlaySound(cs, midiChannel, note);
                    DoNoteOnEvent(midiChannel, note);
                }
                else
                {
                    Console.WriteLine($"!!!! No sound found for Midi:{midiChannel} Note: {note}");
                }
            }
        }

        protected void DoNoteOnEvent(int midiChannel, int note)
        {
            if (NoteOnEvent != null)
            {
                NoteOnEvent(this, new SpiSoundInstrumentEventArgs(midiChannel, note));
            }
        }


        public override void NoteOff(int midiChannel, int note)
        {
            //Console.WriteLine($"Spi got Note off: {midiChannel} {note}");

            CachedSound[] channelMap = playingContext[midiChannel - 1];
            CachedSound oldSnd = channelMap[note];
            if (oldSnd != null) {
                //Console.WriteLine("Stopping: {0}", oldSnd.WaveFormat.ToString()); /*audio.StopSound(oldSnd); */
                audio.StopSound(oldSnd);
                channelMap[note] = null;
                DoNoteOffEvent(midiChannel, note);
            }
            playingContext[midiChannel - 1] = channelMap;
        }


        protected void DoNoteOffEvent(int midiChannel, int note)
        {
            if (NoteOffEvent != null)
            {
                NoteOffEvent(this, new SpiSoundInstrumentEventArgs(midiChannel, note));
            }
        }

        private void PlaySound(CachedSound snd, int midiChannel, int note)
        {
            CachedSound[] channelMap = playingContext[midiChannel-1];
            CachedSound oldSnd = channelMap[note];
            if (oldSnd != null) {
                //Console.WriteLine("Stopping: {0}", oldSnd.WaveFormat.ToString()) ; /*audio.StopSound(oldSnd); */
                audio.StopSound(oldSnd);
            }

            audio.PlaySound(snd);
            //Console.WriteLine("Starting: {0}", snd.WaveFormat.ToString());
            channelMap[note] = snd;
            playingContext[midiChannel - 1] = channelMap;
        }


        public void AllNotesOff()
        {
            for (int midiChannel = 0; midiChannel < 16; midiChannel++)
            {
                CachedSound[] channelMap = playingContext[midiChannel];
                for (int i = 0; i < 128; i++)
                {
                    CachedSound snd = channelMap[i];
                    if (snd != null)
                    {
                        audio.StopSound(snd);
                        channelMap[i] = null;
                    }
                }
                playingContext[midiChannel] = channelMap;
            }
        }
    }


    public class SpiSoundInstrumentEventArgs: EventArgs
    {
        public int midiChannel { get; private set; }
        public int note { get; private set; }

        public SpiSoundInstrumentEventArgs(int midiChannel, int note)
        {
            this.midiChannel = midiChannel;
            this.note = note;
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
