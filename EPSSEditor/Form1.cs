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

namespace EPSSEditor
{


    /*
     * TODO:
     * OK. Show and reset settings
     * OK. Check for adding same drum sound on the same note
     * OK. Auto increase for adding drums
     * OK. Multisample patch
     * OK. Omni switch
     *     Multisample for drums, automatic spread
     * 
     * Bugs:
     * OK.  Cannot find the drumMappings.xml in installed version.
     * OK.  Did not update the sample size field for Sound correctly.
     * EPSS DOS-date interpreted wrong in EPSS. 2000 issue, needs to be fixed in EPSS!
     * OK.  Did not save the directories for project, samples and spi correctly. Fixed in 1.04.
     *      If no sounds found, shoulnt crash
     *      
     * 
     * 
     * Wishlist:
     * WIP      Multiple selection of sounds to automatically add? Mutisample + Drums
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


        public Form1()
        {
            InitializeComponent();
            initialize = 0;
            dataNeedsSaving = false;


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
                    defaultMidiMapRadioButton.Checked = true;

                    saveProjectSettings();
                }
            }
        }


        private void updateDialog()
        {
            updateDrumSettings();
            updateSoundListBox();
            updateSpiSoundListBox();
            updatePreview();
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


        private void updateSpiSoundListBox()
        {
            spiSoundListView.Clear();
            spiSoundListView.Columns.Add("", 40, HorizontalAlignment.Left);
            spiSoundListView.Columns.Add("MIDI", 35, HorizontalAlignment.Right);
            spiSoundListView.Columns.Add("Note", 35, HorizontalAlignment.Right);
            spiSoundListView.Columns.Add("#", 20, HorizontalAlignment.Right);
            spiSoundListView.Columns.Add("Sound", 165, HorizontalAlignment.Left);
            spiSoundListView.Columns.Add("Size", 55, HorizontalAlignment.Left);
            spiSoundListView.View = View.Details;

            int i = 0;
            foreach (SpiSound s in data.spiSounds)
            {
                ListViewItem item = new ListViewItem(i++.ToString());

                item.SubItems.Add(s.midiChannel.ToString());
                item.SubItems.Add(s.midiNote.ToString());

                int nr= data.getSoundNumberFromGuid(s.soundId);
                item.SubItems.Add(nr.ToString());

                item.SubItems.Add(s.name());
                item.SubItems.Add(Ext.ToPrettySize(s.preLength(ref data), 2));
                spiSoundListView.Items.Add(item);
            }

            saveSpiButton.Enabled = data.spiSounds.Count > 0;

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
            SpiCreator creator = new SpiCreator();
            long sz = creator.length(ref data);
            totalSizeTextBox.Text = Ext.ToPrettySize(sz, 2);

            int v = (int)(sz / 1024);
            totalSizeProgressBar.Value = v;
           
        }


        private void setMidiChannel(int ch)
        {
            midiChTextBox.Text = ch.ToString();
            midiChTrackBar.Value = ch;

            drumsComboBox1.Enabled = ch == 10 ? true : false;
        }



        private void deleteSelectedSound()
        {
                      
            int idx = soundListBox.SelectedIndex;
            if (idx >= 0)
            {

                Sound snd = data.sounds[idx];
                List<SpiSound> spiSounds = data.getSpiSoundsFromSound(ref snd);
                if (spiSounds.Count > 0)
                {
                    System.Windows.Forms.MessageBox.Show("The sound still refers to SPI sounds.\nPlease remove them first.");
                }
                else
                {
                    data.sounds.RemoveAt(idx);
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
        }


        private void deleteSelectedSpiSound()
        {
            List<int> idxRemoved = new List<int>();
            foreach(ListViewItem item in spiSoundListView.CheckedItems)
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
        }


        private void playConvertedSound()
        {

         
            int selected = -1;
            foreach (ListViewItem item in spiSoundListView.CheckedItems)
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
            saveSampleFileDialog.InitialDirectory = Path.GetDirectoryName(data.soundFileName);
            if (saveSampleFileDialog.ShowDialog() == DialogResult.OK)
            {
                string outFile = saveSampleFileDialog.FileName;

                int selected = -1;
                foreach (ListViewItem item in spiSoundListView.CheckedItems)
                {
                    selected = item.Index;
                    break;

                }

                if (selected >= 0)
                {
                    SpiSound snd = data.spiSounds[selected];

                    

                    if (snd.convertSound(ref data, outFile, frequencyFromCompressionTrackBar(compressionTrackBar.Value), AtariConstants.SampleBits, AtariConstants.SampleChannels))
                    {
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

        private void soundListBox_DragDrop(object sender, DragEventArgs e)
        {
            string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
            string anyFile = "";
            foreach (string filePath in files)
            {
                Sound s = new Sound(filePath);

                data.sounds.Add(s);
                updateSoundListBox();
                
                anyFile = filePath;
            }
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
            

            if (ch == 10)
            {
                GmPercMidiMappingRadioButton.Checked = true;
            }
            else
            {

                defaultMidiMapRadioButton.Checked = true;
            }
            
        }

        private void GmPercMidiMappingRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            setMidiChannel(10);
        }


        private void defaultMidiMapRadioButton_CheckedChanged(object sender, EventArgs e)
        { 
            int ch = data.getNextFreeMidiChannel();
            if (ch > 0) setMidiChannel(ch);
        }



        private void useInSpiButton_Click(object sender, EventArgs e)
        {
            List<Sound> sounds = getSelectedSounds();

            if (sounds.Count > 1)
            {
                if (MultiSampleRadioButton.Checked)
                {

                    byte startNote = 1;
                    string st = midiToneTextBox.Text;
                    // TODO parse string midi
                    try {
                        startNote = Convert.ToByte(st);
                    }
                    catch (Exception ex)
                    {

                    }
    

                    foreach(Sound sound in sounds)
                    {
                        Sound s = sound;
                        SpiSound spiSnd = new SpiSound(ref s);

                        spiSnd.midiNote = startNote++;

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

                        data.spiSounds.Add(spiSnd);

                    }
                    updateSpiSoundListBox();
                    dataNeedsSaving = true;
                    saveProjectSettings();
                    updateTotalSize();

                } else
                {
                    System.Windows.Forms.MessageBox.Show("Need to have multi sample selected!");
                }

            }
            else
            {


                Sound s = getSoundAtSelectedIndex();
                if (s != null)
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





        private void saveSpiButton_Click(object sender, EventArgs e)
        {

            List<SpiSound> soundsToSave = data.getSortedSpiSounds();
            if (soundsToSave.Count > 0)
            {
                saveSpiFileDialog.InitialDirectory = Path.GetDirectoryName(data.spiFileName);

                if (saveSpiFileDialog.ShowDialog() == DialogResult.OK)
                {
                    string spiFile = saveSpiFileDialog.FileName;
                    Uri url = new Uri(spiFile);

                    SpiCreator creator = new SpiCreator();
                    int sampFreq = AtariConstants.SampleFreq25k;
                    data.omni = omniPatchCheckBox.Checked;
                    EPSSSpi spi = creator.create(ref data, soundsToSave, spiNameTextBox.Text, spiInfoTextBox.Text, sampFreq);

                    spi.save(url);

                    data.spiFileName = spiFile;
                    dataNeedsSaving = true;
                    saveProjectSettings();
                }

            }
            else
            {
                System.Windows.Forms.MessageBox.Show("No sounds to save!");
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
            bool enabled = false;
            foreach (ListViewItem item in spiSoundListView.CheckedItems)
            {
                enabled = true;
            }
            deleteSpiSoundButton.Enabled = enabled;
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

        private void selectAllButton_Click(object sender, EventArgs e)
        {
            List<int> idxRemoved = new List<int>();
            foreach (ListViewItem item in spiSoundListView.Items)
            {
                item.Checked = true;
            }

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

        private void playButton_Click(object sender, EventArgs e)
        {

            playSelectedSound();
        }

    }
}
