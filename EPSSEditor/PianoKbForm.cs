using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace EPSSEditor
{
    public partial class PianoKbForm : Form
    {
        private Form1 _form1;
        public PianoKbForm(Form1 form1, int midiChannel)
        {
            _form1 = form1;
            InitializeComponent();
            UpdateMidiChannel(midiChannel);
            UpdateMidiVel(127);
        }

        /*
        private void Form2_FormClosed(object sender, FormClosedEventArgs e)
        {
            _form1.NotifyClosed(this);
        }
        */


        public PianoKbForm()
        {
            InitializeComponent();
        }

        private void UpdateMidiChannel(int ch)
        {
            midiChTextBox.Text = ch.ToString();
            midiChTrackBar.Value = ch;
            midiChTextBox.Enabled = false;
            midiChTrackBar.Enabled = false; 
        }


        private void UpdateMidiVel(int vel)
        {
            midVelTrackBar.Value = vel;
            midVelTextBox.Text = vel.ToString();
        }

        private void trackBar1_Scroll(object sender, EventArgs e)
        {
            midVelTextBox.Text = midVelTrackBar.Value.ToString();
        }

        private void pianoBox2_PianoKeyDown(object sender, M.PianoKeyEventArgs args)
        {
            int midiChannel = midiChTrackBar.Value;
            int midiVelocity = midVelTrackBar.Value;
            _form1.pianoBox1_PianoKeyDown(sender, args, midiChannel, midiVelocity);
        }

        private void pianoBox2_PianoKeyUp(object sender, M.PianoKeyEventArgs args)
        {
            int midiChannel = midiChTrackBar.Value;
            _form1.pianoBox1_PianoKeyUp(sender, args, midiChannel);
        }
    }
}
