using System.Windows.Forms;

namespace EPSSEditor
{
    partial class PianoKbForm
    {



        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.midiChTextBox = new System.Windows.Forms.TextBox();
            this.midiChTrackBar = new System.Windows.Forms.TrackBar();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.pianoBox1 = new M.PianoBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.midVelTextBox = new System.Windows.Forms.TextBox();
            this.midVelTrackBar = new System.Windows.Forms.TrackBar();
            ((System.ComponentModel.ISupportInitialize)(this.midiChTrackBar)).BeginInit();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.midVelTrackBar)).BeginInit();
            this.SuspendLayout();
            // 
            // midiChTextBox
            // 
            this.midiChTextBox.Location = new System.Drawing.Point(163, 17);
            this.midiChTextBox.Name = "midiChTextBox";
            this.midiChTextBox.ReadOnly = true;
            this.midiChTextBox.Size = new System.Drawing.Size(41, 20);
            this.midiChTextBox.TabIndex = 15;
            // 
            // midiChTrackBar
            // 
            this.midiChTrackBar.Location = new System.Drawing.Point(11, 17);
            this.midiChTrackBar.Margin = new System.Windows.Forms.Padding(2, 1, 2, 1);
            this.midiChTrackBar.Maximum = 16;
            this.midiChTrackBar.Minimum = 1;
            this.midiChTrackBar.Name = "midiChTrackBar";
            this.midiChTrackBar.Size = new System.Drawing.Size(147, 45);
            this.midiChTrackBar.TabIndex = 14;
            this.midiChTrackBar.Value = 1;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.midiChTextBox);
            this.groupBox1.Controls.Add(this.midiChTrackBar);
            this.groupBox1.Location = new System.Drawing.Point(12, 175);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(215, 76);
            this.groupBox1.TabIndex = 16;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "MIDI Channel";
            // 
            // pianoBox1
            // 
            this.pianoBox1.BlackKeyColor = System.Drawing.Color.Black;
            this.pianoBox1.BorderColor = System.Drawing.Color.Black;
            this.pianoBox1.HotKeys = new System.Windows.Forms.Keys[] {
        System.Windows.Forms.Keys.None,
        System.Windows.Forms.Keys.None,
        System.Windows.Forms.Keys.None,
        System.Windows.Forms.Keys.None,
        System.Windows.Forms.Keys.None,
        System.Windows.Forms.Keys.None,
        System.Windows.Forms.Keys.None,
        System.Windows.Forms.Keys.None,
        System.Windows.Forms.Keys.None,
        System.Windows.Forms.Keys.None,
        System.Windows.Forms.Keys.None,
        System.Windows.Forms.Keys.None,
        System.Windows.Forms.Keys.None,
        System.Windows.Forms.Keys.None,
        System.Windows.Forms.Keys.None,
        System.Windows.Forms.Keys.None,
        System.Windows.Forms.Keys.None,
        System.Windows.Forms.Keys.None,
        System.Windows.Forms.Keys.None,
        System.Windows.Forms.Keys.None,
        System.Windows.Forms.Keys.None,
        System.Windows.Forms.Keys.None,
        System.Windows.Forms.Keys.None,
        System.Windows.Forms.Keys.None,
        System.Windows.Forms.Keys.None,
        System.Windows.Forms.Keys.None,
        System.Windows.Forms.Keys.None,
        System.Windows.Forms.Keys.None,
        System.Windows.Forms.Keys.None,
        System.Windows.Forms.Keys.None,
        System.Windows.Forms.Keys.None,
        System.Windows.Forms.Keys.None,
        System.Windows.Forms.Keys.None,
        System.Windows.Forms.Keys.None,
        System.Windows.Forms.Keys.None,
        System.Windows.Forms.Keys.None,
        System.Windows.Forms.Keys.None,
        System.Windows.Forms.Keys.None,
        System.Windows.Forms.Keys.None,
        System.Windows.Forms.Keys.None,
        System.Windows.Forms.Keys.None,
        System.Windows.Forms.Keys.None,
        System.Windows.Forms.Keys.None,
        System.Windows.Forms.Keys.None,
        System.Windows.Forms.Keys.None,
        System.Windows.Forms.Keys.None,
        System.Windows.Forms.Keys.None,
        System.Windows.Forms.Keys.None,
        System.Windows.Forms.Keys.Q,
        System.Windows.Forms.Keys.D2,
        System.Windows.Forms.Keys.W,
        System.Windows.Forms.Keys.D3,
        System.Windows.Forms.Keys.E,
        System.Windows.Forms.Keys.R,
        System.Windows.Forms.Keys.D5,
        System.Windows.Forms.Keys.T,
        System.Windows.Forms.Keys.D6,
        System.Windows.Forms.Keys.Y,
        System.Windows.Forms.Keys.D7,
        System.Windows.Forms.Keys.U,
        System.Windows.Forms.Keys.Z,
        System.Windows.Forms.Keys.S,
        System.Windows.Forms.Keys.X,
        System.Windows.Forms.Keys.D,
        System.Windows.Forms.Keys.C,
        System.Windows.Forms.Keys.V,
        System.Windows.Forms.Keys.G,
        System.Windows.Forms.Keys.B,
        System.Windows.Forms.Keys.H,
        System.Windows.Forms.Keys.N,
        System.Windows.Forms.Keys.J,
        System.Windows.Forms.Keys.M};
            this.pianoBox1.Location = new System.Drawing.Point(12, 12);
            this.pianoBox1.Name = "pianoBox1";
            this.pianoBox1.NoteHighlightColor = System.Drawing.Color.Orange;
            this.pianoBox1.Octaves = 11;
            this.pianoBox1.Size = new System.Drawing.Size(1005, 157);
            this.pianoBox1.TabIndex = 0;
            this.pianoBox1.Text = "pianoBox1";
            this.pianoBox1.WhiteKeyColor = System.Drawing.Color.White;
            this.pianoBox1.PianoKeyDown += new M.PianoKeyEventHandler(this.pianoBox1_PianoKeyDown);
            this.pianoBox1.PianoKeyUp += new M.PianoKeyEventHandler(this.pianoBox1_PianoKeyUp);
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.midVelTextBox);
            this.groupBox2.Controls.Add(this.midVelTrackBar);
            this.groupBox2.Location = new System.Drawing.Point(243, 175);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(215, 76);
            this.groupBox2.TabIndex = 17;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "MIDI Velocity";
            // 
            // midVelTextBox
            // 
            this.midVelTextBox.Location = new System.Drawing.Point(163, 17);
            this.midVelTextBox.Name = "midVelTextBox";
            this.midVelTextBox.ReadOnly = true;
            this.midVelTextBox.Size = new System.Drawing.Size(41, 20);
            this.midVelTextBox.TabIndex = 15;
            // 
            // midVelTrackBar
            // 
            this.midVelTrackBar.Location = new System.Drawing.Point(11, 17);
            this.midVelTrackBar.Margin = new System.Windows.Forms.Padding(2, 1, 2, 1);
            this.midVelTrackBar.Maximum = 127;
            this.midVelTrackBar.Name = "midVelTrackBar";
            this.midVelTrackBar.Size = new System.Drawing.Size(147, 45);
            this.midVelTrackBar.TabIndex = 14;
            this.midVelTrackBar.Value = 1;
            this.midVelTrackBar.Scroll += new System.EventHandler(this.trackBar1_Scroll);
            // 
            // PianoKbForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1029, 260);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.pianoBox1);
            this.Controls.Add(this.groupBox1);
            this.Name = "PianoKbForm";
            this.Text = "PianoKb";
            ((System.ComponentModel.ISupportInitialize)(this.midiChTrackBar)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.midVelTrackBar)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private M.PianoBox pianoBox1;
        private TextBox midiChTextBox;
        private TrackBar midiChTrackBar;
        private GroupBox groupBox1;
        private GroupBox groupBox2;
        private TextBox midVelTextBox;
        private TrackBar midVelTrackBar;
    }
}