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

namespace EPSSEditor
{
    
    public partial class Form1 : Form
    {
        public DrumSettingsHelper drumMappings;
        public List<Sound> sounds;

        public Form1()
        {
            InitializeComponent();

            initDrumSettings();

            sounds = new List<Sound>();
            initTestSounds(ref sounds);
            updateSoundListBox();

        }

        private void initDrumSettings()
        {
            DrumSettingsHelper drumMappings = new DrumSettingsHelper();
            drumMappings.initialize();

            foreach (Mapping m in drumMappings.mappings)
            {
                drumsComboBox1.Items.Add(m.description);
            }
            drumsComboBox1.SelectedIndex = 0;
        }


        private void updateSoundListBox()
        {
            soundListBox.Items.Clear();
            foreach (Sound s in sounds)
            {
                soundListBox.Items.Add(s.name());
            }
        }


        private void initTestSounds(ref List<Sound> soundList)
        {
            Sound s0 = new Sound(@"c:\windows\media\Alarm01.wav");


            soundList.Add(s0);

            Sound s1 = new Sound(@"c:\windows\media\Alarm02.wav");

            soundList.Add(s1);
        }


        private void button1_Click(object sender, EventArgs e)
        {

        }

        private void textBox4_TextChanged(object sender, EventArgs e)
        {

        }

        private void trackBar1_Scroll(object sender, EventArgs e)
        {
            // Play sound when released with current chosen compression
        }


        private void listBox3_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void listBox2_SelectedIndexChanged(object sender, EventArgs e)
        {

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
            foreach (string filePath in files)
            {
                Sound s = new Sound(filePath);

                sounds.Add(s);
                updateSoundListBox();
                
                Console.WriteLine(filePath);
            }
  
        }

        private void clearAllSoundsButton_Click(object sender, EventArgs e)
        {
            sounds = new List<Sound>();
            updateSoundListBox();
        }

        private void deleteSoundButton_Click(object sender, EventArgs e)
        {
            MessageBox.Show("NYI");
        }

        private void loadSoundButton_Click(object sender, EventArgs e)
        {
            //OpenFileDialog loadFile= new OpenFileDialog();






            if (loadSoundFileDialog.ShowDialog() == DialogResult.OK)
            {

                foreach (string name in loadSoundFileDialog.FileNames)
                {
                    Sound s = new Sound(name);
                    sounds.Add(s);
                }
                updateSoundListBox();

            }

        }

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {




            
        }

        private void Form1_KeyUp(object sender, KeyEventArgs e)
        {
      
        }

        private void playButton_Click(object sender, EventArgs e)
        {

        }

        private void playButton_KeyDown(object sender, KeyEventArgs e)
        {
            playSelectedSound();
        }

        private void playSelectedSound()
        {
            int idx = soundListBox.SelectedIndex;
            if (idx >= 0)
            {
                Sound[] snds = sounds.ToArray();
                Sound snd = snds[idx];

                FileStream wav = File.OpenRead(snd.path);
                wav.Seek(0, SeekOrigin.Begin);

                WaveStream ws = new WaveFileReader(wav);
                ws = WaveFormatConversionStream.CreatePcmStream(ws);

                WaveOutEvent output = new WaveOutEvent();
                output.Init(ws);
                output.Play();

            }
        }


        private void playButton_KeyUp(object sender, KeyEventArgs e)
        {
            //player.Stop();
        }

        private void soundListBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            playSelectedSound();
        }
    }
}
