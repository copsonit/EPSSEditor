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

namespace EPSSEditor
{
    
    public partial class Form1 : Form
    {
        public DrumSettingsHelper drumMappings;

        public Form1()
        {
            InitializeComponent();

            DrumSettingsHelper drumMappings = new DrumSettingsHelper();
            drumMappings.initialize();

            foreach(Mapping m in drumMappings.mappings)
            {
                comboBox1.Items.Add(m.description);
            }
            comboBox1.SelectedIndex = 0;
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

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            //Drag drop
        }

        private void listBox3_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void listBox2_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
    }
}
