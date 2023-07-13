using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.StartPanel;

namespace EPSSEditor
{
    public partial class PianoKbForm : Form
    {
        private Form1 _form1;
        private int midiChannel;

        public PianoKbForm(Form1 form1, int midiChannel)
        {
            _form1 = form1;
            InitializeComponent();
            SetMidiChannel(midiChannel);
            SetMidiVel(127);
        }

        
        private void Form2_FormClosed(object sender, FormClosedEventArgs e)
        {
            _form1.NotifyClosed(this);
        }
        


        public PianoKbForm()
        {
            InitializeComponent();
        }

        public void SetMidiChannel(int ch)
        {
            midiChannel = ch;
            midiChTextBox.Text = ch.ToString();
            midiChTrackBar.Value = ch;
            midiChTextBox.Enabled = true;
            midiChTrackBar.Enabled = true;
        }


        private void SetMidiVel(int vel)
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

        public void NoteOnOff(int midiChannel, int note, bool onOff, bool suppressEvent)
        {
            if (IsHandleCreated)
            {
                if (midiChannel == this.midiChannel)
                {
                    BeginInvoke(new Action(() =>
                {
                    pianoBox2.SetKey(note, onOff, suppressEvent);
                }));
                }
            }
        }

        public void ShowNotes(int midiChannel, int startNote, int endNote)
        {
            if (IsHandleCreated)
            {
                if (midiChannel == this.midiChannel)
                {
                    BeginInvoke(new Action(() =>
                    {
                        for (int note = startNote; note <= endNote; note++)
                        {
                            pianoBox2.SetKey(note, true, true);
                        }
                    }));
                }
            }
        }

        public void SetKeyAllOff()
        {
            for (int i = 0; i < 128; i++)
            {
                pianoBox2.SetKey(i, false, true);
            }
        }


        public void SetCenterKey(int note)
        {
            pianoBox2.CenterKey = note;
        }

        private void midiChTrackBar_Scroll(object sender, EventArgs e)
        {
            SetKeyAllOff();
            SetMidiChannel(midiChTrackBar.Value);
        }

        private void pianoBox2_MouseDown(object sender, MouseEventArgs e)
        {
            SetKeyAllOff();
        }
    }
}
