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


namespace EPSSEditor
{


    /*
     * 
     * TODO:
     * OK. Show and reset settings
     * OK. Check for adding same drum sound on the same note
     * Auto increase for adding drums
     * 
     * */



    public partial class Form1 : Form
    {

        public EPSSEditorData data;
        public bool deletePressed;
        public bool callbacks = true;

        public Form1()
        {
            InitializeComponent();

            initEpssEditorData();

            defaultMidiMapRadioButton.Checked = true;
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


        private void initEpssEditorData()
        {
            string projectFile = Properties.Settings.Default.ProjectFile;
            
            if (projectFile == null | projectFile == "")
            {
                if (System.Windows.Forms.MessageBox.Show("No SPI Project file found!\nDo you want to load an existing file?", "Confirm", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    loadProjectSettingsFileDialog();
                }
                else
                {
                    if (saveProjectFileDialog.ShowDialog() == DialogResult.OK)
                    {
                        string file = saveProjectFileDialog.FileName;
                        Properties.Settings.Default.ProjectFile = file;
                        Properties.Settings.Default.Save();
                    }
                    data = new EPSSEditorData();
                    data.initialize();
                    saveProjectSettings();
                }

            } else
            {
                loadProjectSettings(projectFile);
            }
            
            compressionTrackBar.Enabled = false;
        }


        private void loadProjectSettings(string file)
        {
            XmlSerializer ser = new XmlSerializer(typeof(EPSSEditorData));
            using (FileStream fs = new FileStream(file, FileMode.Open))
            {
                data = (EPSSEditorData)ser.Deserialize(fs);

                data.fixOldVersions();

                updateDrumSettings();
                updateSoundListBox();
                updateSpiSoundListBox();
            }
        }


        private void saveProjectSettings()
        {
            string file = Properties.Settings.Default.ProjectFile;
            if (file != "")
            {

                XmlSerializer ser = new XmlSerializer(typeof(EPSSEditorData));
                using (FileStream fs = new FileStream(file, FileMode.Create))
                {
                    ser.Serialize(fs, data);
                }
            }
        }


        private void updateDrumSettings()
        {
            foreach (Mapping m in data.drumMappings.mappings)
            {
                drumsComboBox1.Items.Add(m.description);
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


        private void updateAfterSoundChange(ref Sound snd, int toFreq)
        {
            callbacks = false;
            int numberOfDecimals = 2;
            soundSizeTextBox.Text = Ext.ToPrettySize(snd.length, numberOfDecimals);
            channelsTextBox.Text = snd.channels.ToString() + " Channels";
            freqTextBox.Text = snd.samplesPerSecond.ToString() + " Hz";
            bitsTextBox.Text = snd.bitsPerSample.ToString() + " Bit";

            int toBit = 8; // always!

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


        private void loadProjectSettingsFileDialog()
        {
            if (loadProjectFileDialog.ShowDialog() == DialogResult.OK)
            {
                string file = loadProjectFileDialog.FileName;
                Properties.Settings.Default.ProjectFile = file;
                Properties.Settings.Default.Save();
                loadProjectSettings(file);
            }
        }


        private void saveProjectSettingsFileDialog()
        {
            if (saveProjectFileDialog.ShowDialog() == DialogResult.OK)
            {
                string file = saveProjectFileDialog.FileName;
                Properties.Settings.Default.ProjectFile = file;
                Properties.Settings.Default.Save();
                saveProjectSettings();
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
                saveProjectSettings();
            }
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
                        System.Windows.Forms.MessageBox.Show("MIDI channel "+ midiChannel.ToString() + " already occupied!");
                    }
                } else if (midiChannel == 10)
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
                        if (idx < drumsComboBox1.Items.Count-1)
                        {
                            drumsComboBox1.SelectedIndex = idx + 1;
                        }
                    }

                    saveProjectSettings();
                    updateTotalSize();

                }
            }
        }
        

        private void deleteSpiSoundButton_Click(object sender, EventArgs e)
        {
            deleteSelectedSpiSound();
        }

  
        private void saveProjectToolStripMenuItem_Click(object sender, EventArgs e)
        {
            saveProjectSettingsFileDialog();


        }


        private void loadProjectToolStripMenuItem_Click(object sender, EventArgs e)
        {

            loadProjectSettingsFileDialog();
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
                    EPSSSpi spi = creator.create(ref data, soundsToSave, spiNameTextBox.Text, spiInfoTextBox.Text);

                    spi.save(url);

                    data.spiFileName = spiFile;
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
    }
}
