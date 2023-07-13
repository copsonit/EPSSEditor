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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PianoKbForm));
            this.midiChTextBox = new System.Windows.Forms.TextBox();
            this.midiChTrackBar = new System.Windows.Forms.TrackBar();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.midVelTextBox = new System.Windows.Forms.TextBox();
            this.midVelTrackBar = new System.Windows.Forms.TrackBar();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.label11 = new System.Windows.Forms.Label();
            this.label12 = new System.Windows.Forms.Label();
            this.pianoBox2 = new M.PianoBox();
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
            this.midiChTrackBar.Scroll += new System.EventHandler(this.midiChTrackBar_Scroll);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.midiChTextBox);
            this.groupBox1.Controls.Add(this.midiChTrackBar);
            this.groupBox1.Location = new System.Drawing.Point(12, 191);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(215, 76);
            this.groupBox1.TabIndex = 16;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "MIDI Channel";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.midVelTextBox);
            this.groupBox2.Controls.Add(this.midVelTrackBar);
            this.groupBox2.Location = new System.Drawing.Point(243, 191);
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
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 13);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(13, 13);
            this.label1.TabIndex = 19;
            this.label1.Text = "0";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(102, 13);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(19, 13);
            this.label2.TabIndex = 20;
            this.label2.Text = "12";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(192, 13);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(19, 13);
            this.label3.TabIndex = 21;
            this.label3.Text = "24";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(283, 13);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(19, 13);
            this.label4.TabIndex = 22;
            this.label4.Text = "36";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(373, 13);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(19, 13);
            this.label5.TabIndex = 23;
            this.label5.Text = "48";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(465, 13);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(19, 13);
            this.label6.TabIndex = 24;
            this.label6.Text = "60";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(557, 13);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(19, 13);
            this.label7.TabIndex = 25;
            this.label7.Text = "72";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(648, 13);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(19, 13);
            this.label8.TabIndex = 26;
            this.label8.Text = "84";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(738, 13);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(19, 13);
            this.label9.TabIndex = 27;
            this.label9.Text = "96";
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(831, 13);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(25, 13);
            this.label10.TabIndex = 28;
            this.label10.Text = "108";
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(921, 13);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(25, 13);
            this.label11.TabIndex = 29;
            this.label11.Text = "120";
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(974, 13);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(25, 13);
            this.label12.TabIndex = 30;
            this.label12.Text = "127";
            // 
            // pianoBox2
            // 
            this.pianoBox2.BlackKeyColor = System.Drawing.Color.Black;
            this.pianoBox2.BorderColor = System.Drawing.Color.Black;
            this.pianoBox2.CenterKey = 5;
            this.pianoBox2.HotKeys = new System.Windows.Forms.Keys[] {
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
            this.pianoBox2.Location = new System.Drawing.Point(12, 28);
            this.pianoBox2.Name = "pianoBox2";
            this.pianoBox2.NoteHighlightColor = System.Drawing.Color.Orange;
            this.pianoBox2.Octaves = 11;
            this.pianoBox2.Size = new System.Drawing.Size(977, 153);
            this.pianoBox2.TabIndex = 18;
            this.pianoBox2.Text = "pianoBox2";
            this.pianoBox2.WhiteKeyColor = System.Drawing.Color.White;
            this.pianoBox2.PianoKeyDown += new M.PianoKeyEventHandler(this.pianoBox2_PianoKeyDown);
            this.pianoBox2.PianoKeyUp += new M.PianoKeyEventHandler(this.pianoBox2_PianoKeyUp);
            this.pianoBox2.MouseDown += new System.Windows.Forms.MouseEventHandler(this.pianoBox2_MouseDown);
            // 
            // PianoKbForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1001, 277);
            this.Controls.Add(this.label12);
            this.Controls.Add(this.label11);
            this.Controls.Add(this.label10);
            this.Controls.Add(this.label9);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.pianoBox2);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "PianoKbForm";
            this.Text = "MIDI Keyboard";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.Form2_FormClosed);
            ((System.ComponentModel.ISupportInitialize)(this.midiChTrackBar)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.midVelTrackBar)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private TextBox midiChTextBox;
        private TrackBar midiChTrackBar;
        private GroupBox groupBox1;
        private GroupBox groupBox2;
        private TextBox midVelTextBox;
        private TrackBar midVelTrackBar;
        private M.PianoBox pianoBox2;
        private Label label1;
        private Label label2;
        private Label label3;
        private Label label4;
        private Label label5;
        private Label label6;
        private Label label7;
        private Label label8;
        private Label label9;
        private Label label10;
        private Label label11;
        private Label label12;
    }
}