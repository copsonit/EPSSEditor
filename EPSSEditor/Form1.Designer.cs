namespace EPSSEditor
{
    partial class Form1
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
            this.components = new System.ComponentModel.Container();
            this.soundListBox = new System.Windows.Forms.ListBox();
            this.saveSpiButton = new System.Windows.Forms.Button();
            this.spiSoundListBox = new System.Windows.Forms.ListBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.deleteSpiSoundButton = new System.Windows.Forms.Button();
            this.label12 = new System.Windows.Forms.Label();
            this.label11 = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.groupBox9 = new System.Windows.Forms.GroupBox();
            this.GmPercMidiMappingRadioButton = new System.Windows.Forms.RadioButton();
            this.defaultMidiMapRadioButton = new System.Windows.Forms.RadioButton();
            this.midiChTrackBar = new System.Windows.Forms.TrackBar();
            this.midiChTextBox = new System.Windows.Forms.TextBox();
            this.label21 = new System.Windows.Forms.Label();
            this.drumsComboBox1 = new System.Windows.Forms.ComboBox();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.totalSizeProgressBar = new System.Windows.Forms.ProgressBar();
            this.totalSizeTextBox = new System.Windows.Forms.TextBox();
            this.spiNameTextBox = new System.Windows.Forms.TextBox();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.playButton = new System.Windows.Forms.Button();
            this.loadSoundButton = new System.Windows.Forms.Button();
            this.label13 = new System.Windows.Forms.Label();
            this.deleteSoundButton = new System.Windows.Forms.Button();
            this.groupBox5 = new System.Windows.Forms.GroupBox();
            this.label16 = new System.Windows.Forms.Label();
            this.label15 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.channelsTextBox = new System.Windows.Forms.TextBox();
            this.bitsTextBox = new System.Windows.Forms.TextBox();
            this.textBox6 = new System.Windows.Forms.TextBox();
            this.freqTextBox = new System.Windows.Forms.TextBox();
            this.clearAllSoundsButton = new System.Windows.Forms.Button();
            this.compressionTypeTextBox = new System.Windows.Forms.GroupBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.normalizeTextBox = new System.Windows.Forms.TextBox();
            this.normalizeCheckBox = new System.Windows.Forms.CheckBox();
            this.normalizeTrackBar = new System.Windows.Forms.TrackBar();
            this.conversionTextBox = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.groupBox6 = new System.Windows.Forms.GroupBox();
            this.showCompProgressBar = new System.Windows.Forms.ProgressBar();
            this.label6 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.soundSizeAfterTextBox = new System.Windows.Forms.TextBox();
            this.groupBox7 = new System.Windows.Forms.GroupBox();
            this.label20 = new System.Windows.Forms.Label();
            this.label19 = new System.Windows.Forms.Label();
            this.label18 = new System.Windows.Forms.Label();
            this.label17 = new System.Windows.Forms.Label();
            this.compressionTrackBar = new System.Windows.Forms.TrackBar();
            this.label14 = new System.Windows.Forms.Label();
            this.soundSizeTextBox = new System.Windows.Forms.TextBox();
            this.useInSpiButton = new System.Windows.Forms.Button();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.loadProjectToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveProjectToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.aboutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.loadSoundFileDialog = new System.Windows.Forms.OpenFileDialog();
            this.infoToolTip = new System.Windows.Forms.ToolTip(this.components);
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.groupBox8 = new System.Windows.Forms.GroupBox();
            this.spiInfoTextBox = new System.Windows.Forms.TextBox();
            this.label23 = new System.Windows.Forms.Label();
            this.label22 = new System.Windows.Forms.Label();
            this.saveSpiFileDialog = new System.Windows.Forms.SaveFileDialog();
            this.saveProjectFileDialog = new System.Windows.Forms.SaveFileDialog();
            this.loadProjectFileDialog = new System.Windows.Forms.OpenFileDialog();
            this.clearSettingsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.groupBox2.SuspendLayout();
            this.groupBox9.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.midiChTrackBar)).BeginInit();
            this.groupBox3.SuspendLayout();
            this.groupBox5.SuspendLayout();
            this.compressionTypeTextBox.SuspendLayout();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.normalizeTrackBar)).BeginInit();
            this.groupBox6.SuspendLayout();
            this.groupBox7.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.compressionTrackBar)).BeginInit();
            this.menuStrip1.SuspendLayout();
            this.groupBox4.SuspendLayout();
            this.groupBox8.SuspendLayout();
            this.SuspendLayout();
            // 
            // soundListBox
            // 
            this.soundListBox.AllowDrop = true;
            this.soundListBox.FormattingEnabled = true;
            this.soundListBox.Location = new System.Drawing.Point(13, 34);
            this.soundListBox.Margin = new System.Windows.Forms.Padding(2);
            this.soundListBox.Name = "soundListBox";
            this.soundListBox.Size = new System.Drawing.Size(216, 95);
            this.soundListBox.TabIndex = 2;
            this.soundListBox.SelectedIndexChanged += new System.EventHandler(this.soundListBox_SelectedIndexChanged);
            this.soundListBox.DragDrop += new System.Windows.Forms.DragEventHandler(this.soundListBox_DragDrop);
            this.soundListBox.DragEnter += new System.Windows.Forms.DragEventHandler(this.soundListBox_DragEnter);
            this.soundListBox.KeyDown += new System.Windows.Forms.KeyEventHandler(this.soundListBox_KeyDown);
            this.soundListBox.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.soundListBox_KeyPress);
            // 
            // saveSpiButton
            // 
            this.saveSpiButton.Location = new System.Drawing.Point(537, 84);
            this.saveSpiButton.Margin = new System.Windows.Forms.Padding(2);
            this.saveSpiButton.Name = "saveSpiButton";
            this.saveSpiButton.Size = new System.Drawing.Size(221, 20);
            this.saveSpiButton.TabIndex = 3;
            this.saveSpiButton.Text = "Save SPI...";
            this.saveSpiButton.UseVisualStyleBackColor = true;
            this.saveSpiButton.Click += new System.EventHandler(this.saveSpiButton_Click);
            // 
            // spiSoundListBox
            // 
            this.spiSoundListBox.FormattingEnabled = true;
            this.spiSoundListBox.Location = new System.Drawing.Point(9, 37);
            this.spiSoundListBox.Margin = new System.Windows.Forms.Padding(2);
            this.spiSoundListBox.Name = "spiSoundListBox";
            this.spiSoundListBox.Size = new System.Drawing.Size(241, 95);
            this.spiSoundListBox.TabIndex = 4;
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.deleteSpiSoundButton);
            this.groupBox2.Controls.Add(this.label12);
            this.groupBox2.Controls.Add(this.label11);
            this.groupBox2.Controls.Add(this.label10);
            this.groupBox2.Controls.Add(this.label9);
            this.groupBox2.Controls.Add(this.groupBox9);
            this.groupBox2.Controls.Add(this.spiSoundListBox);
            this.groupBox2.Location = new System.Drawing.Point(612, 33);
            this.groupBox2.Margin = new System.Windows.Forms.Padding(2);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Padding = new System.Windows.Forms.Padding(2);
            this.groupBox2.Size = new System.Drawing.Size(270, 369);
            this.groupBox2.TabIndex = 5;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "SPI Sounds";
            // 
            // deleteSpiSoundButton
            // 
            this.deleteSpiSoundButton.Location = new System.Drawing.Point(200, 137);
            this.deleteSpiSoundButton.Name = "deleteSpiSoundButton";
            this.deleteSpiSoundButton.Size = new System.Drawing.Size(50, 23);
            this.deleteSpiSoundButton.TabIndex = 8;
            this.deleteSpiSoundButton.Text = "Delete";
            this.infoToolTip.SetToolTip(this.deleteSpiSoundButton, "You can also use the Delete button on your keyboard to delete souds from the list" +
        "");
            this.deleteSpiSoundButton.UseVisualStyleBackColor = true;
            this.deleteSpiSoundButton.Click += new System.EventHandler(this.deleteSpiSoundButton_Click);
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(217, 17);
            this.label12.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(27, 13);
            this.label12.TabIndex = 14;
            this.label12.Text = "Size";
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(94, 17);
            this.label11.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(42, 13);
            this.label11.TabIndex = 13;
            this.label11.Text = "Sample";
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(140, 17);
            this.label10.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(73, 13);
            this.label10.TabIndex = 12;
            this.label10.Text = "Sample Name";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(9, 17);
            this.label9.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(75, 13);
            this.label9.TabIndex = 11;
            this.label9.Text = "MIDI Channel:";
            // 
            // groupBox9
            // 
            this.groupBox9.Controls.Add(this.GmPercMidiMappingRadioButton);
            this.groupBox9.Controls.Add(this.defaultMidiMapRadioButton);
            this.groupBox9.Controls.Add(this.midiChTrackBar);
            this.groupBox9.Controls.Add(this.midiChTextBox);
            this.groupBox9.Controls.Add(this.label21);
            this.groupBox9.Controls.Add(this.drumsComboBox1);
            this.groupBox9.Location = new System.Drawing.Point(25, 166);
            this.groupBox9.Name = "groupBox9";
            this.groupBox9.Size = new System.Drawing.Size(225, 173);
            this.groupBox9.TabIndex = 11;
            this.groupBox9.TabStop = false;
            this.groupBox9.Text = "MIDI Mapping";
            // 
            // GmPercMidiMappingRadioButton
            // 
            this.GmPercMidiMappingRadioButton.AutoSize = true;
            this.GmPercMidiMappingRadioButton.Location = new System.Drawing.Point(15, 108);
            this.GmPercMidiMappingRadioButton.Name = "GmPercMidiMappingRadioButton";
            this.GmPercMidiMappingRadioButton.Size = new System.Drawing.Size(143, 17);
            this.GmPercMidiMappingRadioButton.TabIndex = 17;
            this.GmPercMidiMappingRadioButton.TabStop = true;
            this.GmPercMidiMappingRadioButton.Text = "GM Percussion mapping:";
            this.GmPercMidiMappingRadioButton.UseVisualStyleBackColor = true;
            this.GmPercMidiMappingRadioButton.CheckedChanged += new System.EventHandler(this.GmPercMidiMappingRadioButton_CheckedChanged);
            // 
            // defaultMidiMapRadioButton
            // 
            this.defaultMidiMapRadioButton.AutoSize = true;
            this.defaultMidiMapRadioButton.Location = new System.Drawing.Point(15, 85);
            this.defaultMidiMapRadioButton.Name = "defaultMidiMapRadioButton";
            this.defaultMidiMapRadioButton.Size = new System.Drawing.Size(160, 17);
            this.defaultMidiMapRadioButton.TabIndex = 16;
            this.defaultMidiMapRadioButton.TabStop = true;
            this.defaultMidiMapRadioButton.Text = "Default MIDI mapping C2-C6";
            this.defaultMidiMapRadioButton.UseVisualStyleBackColor = true;
            this.defaultMidiMapRadioButton.CheckedChanged += new System.EventHandler(this.defaultMidiMapRadioButton_CheckedChanged);
            // 
            // midiChTrackBar
            // 
            this.midiChTrackBar.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.midiChTrackBar.Location = new System.Drawing.Point(10, 45);
            this.midiChTrackBar.Margin = new System.Windows.Forms.Padding(2);
            this.midiChTrackBar.Maximum = 16;
            this.midiChTrackBar.Minimum = 1;
            this.midiChTrackBar.Name = "midiChTrackBar";
            this.midiChTrackBar.Size = new System.Drawing.Size(190, 45);
            this.midiChTrackBar.TabIndex = 5;
            this.midiChTrackBar.Value = 1;
            this.midiChTrackBar.Scroll += new System.EventHandler(this.midiChTrackBar_Scroll);
            // 
            // midiChTextBox
            // 
            this.midiChTextBox.Location = new System.Drawing.Point(112, 20);
            this.midiChTextBox.Name = "midiChTextBox";
            this.midiChTextBox.ReadOnly = true;
            this.midiChTextBox.Size = new System.Drawing.Size(68, 20);
            this.midiChTextBox.TabIndex = 13;
            // 
            // label21
            // 
            this.label21.AutoSize = true;
            this.label21.Location = new System.Drawing.Point(12, 25);
            this.label21.Name = "label21";
            this.label21.Size = new System.Drawing.Size(75, 13);
            this.label21.TabIndex = 12;
            this.label21.Text = "MIDI Channel:";
            // 
            // drumsComboBox1
            // 
            this.drumsComboBox1.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.drumsComboBox1.FormattingEnabled = true;
            this.drumsComboBox1.Location = new System.Drawing.Point(21, 130);
            this.drumsComboBox1.Margin = new System.Windows.Forms.Padding(2);
            this.drumsComboBox1.Name = "drumsComboBox1";
            this.drumsComboBox1.Size = new System.Drawing.Size(141, 21);
            this.drumsComboBox1.TabIndex = 11;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(124, 14);
            this.label4.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(35, 13);
            this.label4.TabIndex = 10;
            this.label4.Text = "14MB";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(37, 14);
            this.label3.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(29, 13);
            this.label3.TabIndex = 9;
            this.label3.Text = "4MB";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(5, 14);
            this.label2.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(29, 13);
            this.label2.TabIndex = 8;
            this.label2.Text = "1MB";
            // 
            // totalSizeProgressBar
            // 
            this.totalSizeProgressBar.Location = new System.Drawing.Point(5, 31);
            this.totalSizeProgressBar.Margin = new System.Windows.Forms.Padding(2);
            this.totalSizeProgressBar.Maximum = 14000;
            this.totalSizeProgressBar.Name = "totalSizeProgressBar";
            this.totalSizeProgressBar.Size = new System.Drawing.Size(147, 20);
            this.totalSizeProgressBar.TabIndex = 7;
            this.totalSizeProgressBar.Value = 100;
            // 
            // totalSizeTextBox
            // 
            this.totalSizeTextBox.Location = new System.Drawing.Point(28, 55);
            this.totalSizeTextBox.Margin = new System.Windows.Forms.Padding(2);
            this.totalSizeTextBox.Name = "totalSizeTextBox";
            this.totalSizeTextBox.Size = new System.Drawing.Size(122, 20);
            this.totalSizeTextBox.TabIndex = 6;
            this.totalSizeTextBox.Text = "Total Size in Bytes";
            // 
            // spiNameTextBox
            // 
            this.spiNameTextBox.Location = new System.Drawing.Point(620, 15);
            this.spiNameTextBox.Margin = new System.Windows.Forms.Padding(2);
            this.spiNameTextBox.MaxLength = 8;
            this.spiNameTextBox.Name = "spiNameTextBox";
            this.spiNameTextBox.Size = new System.Drawing.Size(128, 20);
            this.spiNameTextBox.TabIndex = 5;
            this.spiNameTextBox.Text = "EPSSEDIT";
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.playButton);
            this.groupBox3.Controls.Add(this.loadSoundButton);
            this.groupBox3.Controls.Add(this.label13);
            this.groupBox3.Controls.Add(this.deleteSoundButton);
            this.groupBox3.Controls.Add(this.groupBox5);
            this.groupBox3.Controls.Add(this.soundListBox);
            this.groupBox3.Controls.Add(this.clearAllSoundsButton);
            this.groupBox3.Location = new System.Drawing.Point(31, 33);
            this.groupBox3.Margin = new System.Windows.Forms.Padding(2);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Padding = new System.Windows.Forms.Padding(2);
            this.groupBox3.Size = new System.Drawing.Size(228, 359);
            this.groupBox3.TabIndex = 6;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Input";
            // 
            // playButton
            // 
            this.playButton.Location = new System.Drawing.Point(127, 131);
            this.playButton.Name = "playButton";
            this.playButton.Size = new System.Drawing.Size(46, 23);
            this.playButton.TabIndex = 0;
            this.playButton.Text = "Listen";
            this.playButton.UseVisualStyleBackColor = true;
            this.playButton.KeyDown += new System.Windows.Forms.KeyEventHandler(this.playButton_KeyDown);
            this.playButton.KeyUp += new System.Windows.Forms.KeyEventHandler(this.playButton_KeyUp);
            // 
            // loadSoundButton
            // 
            this.loadSoundButton.Location = new System.Drawing.Point(75, 131);
            this.loadSoundButton.Name = "loadSoundButton";
            this.loadSoundButton.Size = new System.Drawing.Size(46, 23);
            this.loadSoundButton.TabIndex = 7;
            this.loadSoundButton.Text = "Load...";
            this.infoToolTip.SetToolTip(this.loadSoundButton, "You can drag sounds to the  box to add them faster!");
            this.loadSoundButton.UseVisualStyleBackColor = true;
            this.loadSoundButton.Click += new System.EventHandler(this.loadSoundButton_Click);
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Location = new System.Drawing.Point(10, 19);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(46, 13);
            this.label13.TabIndex = 6;
            this.label13.Text = "Sounds:";
            // 
            // deleteSoundButton
            // 
            this.deleteSoundButton.Location = new System.Drawing.Point(179, 131);
            this.deleteSoundButton.Name = "deleteSoundButton";
            this.deleteSoundButton.Size = new System.Drawing.Size(50, 23);
            this.deleteSoundButton.TabIndex = 5;
            this.deleteSoundButton.Text = "Delete";
            this.infoToolTip.SetToolTip(this.deleteSoundButton, "You can also use the Delete button on your keyboard to delete souds from the list" +
        "");
            this.deleteSoundButton.UseVisualStyleBackColor = true;
            this.deleteSoundButton.Click += new System.EventHandler(this.deleteSoundButton_Click);
            // 
            // groupBox5
            // 
            this.groupBox5.Controls.Add(this.label16);
            this.groupBox5.Controls.Add(this.label15);
            this.groupBox5.Controls.Add(this.label8);
            this.groupBox5.Controls.Add(this.label5);
            this.groupBox5.Controls.Add(this.channelsTextBox);
            this.groupBox5.Controls.Add(this.bitsTextBox);
            this.groupBox5.Controls.Add(this.textBox6);
            this.groupBox5.Controls.Add(this.freqTextBox);
            this.groupBox5.Location = new System.Drawing.Point(9, 159);
            this.groupBox5.Margin = new System.Windows.Forms.Padding(2);
            this.groupBox5.Name = "groupBox5";
            this.groupBox5.Padding = new System.Windows.Forms.Padding(2);
            this.groupBox5.Size = new System.Drawing.Size(220, 160);
            this.groupBox5.TabIndex = 3;
            this.groupBox5.TabStop = false;
            this.groupBox5.Text = "Sample Info";
            // 
            // label16
            // 
            this.label16.AutoSize = true;
            this.label16.Location = new System.Drawing.Point(14, 101);
            this.label16.Name = "label16";
            this.label16.Size = new System.Drawing.Size(54, 13);
            this.label16.TabIndex = 7;
            this.label16.Text = "Channels:";
            // 
            // label15
            // 
            this.label15.AutoSize = true;
            this.label15.Location = new System.Drawing.Point(14, 75);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(27, 13);
            this.label15.TabIndex = 6;
            this.label15.Text = "Bits:";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(5, 46);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(60, 13);
            this.label8.TabIndex = 5;
            this.label8.Text = "Frequency:";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(5, 20);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(30, 13);
            this.label5.TabIndex = 4;
            this.label5.Text = "Size:";
            // 
            // channelsTextBox
            // 
            this.channelsTextBox.Location = new System.Drawing.Point(148, 101);
            this.channelsTextBox.Name = "channelsTextBox";
            this.channelsTextBox.Size = new System.Drawing.Size(67, 20);
            this.channelsTextBox.TabIndex = 3;
            // 
            // bitsTextBox
            // 
            this.bitsTextBox.Location = new System.Drawing.Point(147, 73);
            this.bitsTextBox.Name = "bitsTextBox";
            this.bitsTextBox.Size = new System.Drawing.Size(68, 20);
            this.bitsTextBox.TabIndex = 2;
            // 
            // textBox6
            // 
            this.textBox6.Location = new System.Drawing.Point(148, 17);
            this.textBox6.Margin = new System.Windows.Forms.Padding(2);
            this.textBox6.Name = "textBox6";
            this.textBox6.Size = new System.Drawing.Size(68, 20);
            this.textBox6.TabIndex = 1;
            this.textBox6.Text = "Size kb";
            // 
            // freqTextBox
            // 
            this.freqTextBox.Location = new System.Drawing.Point(148, 48);
            this.freqTextBox.Margin = new System.Windows.Forms.Padding(2);
            this.freqTextBox.Name = "freqTextBox";
            this.freqTextBox.Size = new System.Drawing.Size(68, 20);
            this.freqTextBox.TabIndex = 0;
            this.freqTextBox.Text = "Freq 48kHz";
            // 
            // clearAllSoundsButton
            // 
            this.clearAllSoundsButton.Location = new System.Drawing.Point(13, 131);
            this.clearAllSoundsButton.Name = "clearAllSoundsButton";
            this.clearAllSoundsButton.Size = new System.Drawing.Size(59, 23);
            this.clearAllSoundsButton.TabIndex = 4;
            this.clearAllSoundsButton.Text = "Clear all";
            this.clearAllSoundsButton.UseVisualStyleBackColor = true;
            this.clearAllSoundsButton.Click += new System.EventHandler(this.clearAllSoundsButton_Click);
            // 
            // compressionTypeTextBox
            // 
            this.compressionTypeTextBox.Controls.Add(this.groupBox1);
            this.compressionTypeTextBox.Controls.Add(this.conversionTextBox);
            this.compressionTypeTextBox.Controls.Add(this.label1);
            this.compressionTypeTextBox.Controls.Add(this.groupBox6);
            this.compressionTypeTextBox.Controls.Add(this.groupBox7);
            this.compressionTypeTextBox.Controls.Add(this.label14);
            this.compressionTypeTextBox.Controls.Add(this.soundSizeTextBox);
            this.compressionTypeTextBox.Location = new System.Drawing.Point(291, 33);
            this.compressionTypeTextBox.Margin = new System.Windows.Forms.Padding(2);
            this.compressionTypeTextBox.Name = "compressionTypeTextBox";
            this.compressionTypeTextBox.Padding = new System.Windows.Forms.Padding(2);
            this.compressionTypeTextBox.Size = new System.Drawing.Size(228, 374);
            this.compressionTypeTextBox.TabIndex = 7;
            this.compressionTypeTextBox.TabStop = false;
            this.compressionTypeTextBox.Text = "Conversion parameters";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.normalizeTextBox);
            this.groupBox1.Controls.Add(this.normalizeCheckBox);
            this.groupBox1.Controls.Add(this.normalizeTrackBar);
            this.groupBox1.Location = new System.Drawing.Point(9, 286);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(212, 82);
            this.groupBox1.TabIndex = 11;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Normalizing";
            // 
            // normalizeTextBox
            // 
            this.normalizeTextBox.Location = new System.Drawing.Point(97, 9);
            this.normalizeTextBox.Name = "normalizeTextBox";
            this.normalizeTextBox.ReadOnly = true;
            this.normalizeTextBox.Size = new System.Drawing.Size(100, 20);
            this.normalizeTextBox.TabIndex = 2;
            // 
            // normalizeCheckBox
            // 
            this.normalizeCheckBox.AutoSize = true;
            this.normalizeCheckBox.Location = new System.Drawing.Point(6, 12);
            this.normalizeCheckBox.Name = "normalizeCheckBox";
            this.normalizeCheckBox.Size = new System.Drawing.Size(72, 17);
            this.normalizeCheckBox.TabIndex = 1;
            this.normalizeCheckBox.Text = "Normalize";
            this.normalizeCheckBox.UseVisualStyleBackColor = true;
            this.normalizeCheckBox.CheckedChanged += new System.EventHandler(this.normalizeCheckBox_CheckedChanged);
            // 
            // normalizeTrackBar
            // 
            this.normalizeTrackBar.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.normalizeTrackBar.Location = new System.Drawing.Point(5, 28);
            this.normalizeTrackBar.Margin = new System.Windows.Forms.Padding(2);
            this.normalizeTrackBar.Maximum = 1000;
            this.normalizeTrackBar.Name = "normalizeTrackBar";
            this.normalizeTrackBar.Size = new System.Drawing.Size(202, 45);
            this.normalizeTrackBar.TabIndex = 0;
            this.normalizeTrackBar.TickFrequency = 100;
            this.normalizeTrackBar.TickStyle = System.Windows.Forms.TickStyle.Both;
            this.normalizeTrackBar.Value = 100;
            this.normalizeTrackBar.Scroll += new System.EventHandler(this.normalizeTrackBar_Scroll);
            // 
            // conversionTextBox
            // 
            this.conversionTextBox.Location = new System.Drawing.Point(10, 69);
            this.conversionTextBox.Margin = new System.Windows.Forms.Padding(2);
            this.conversionTextBox.Multiline = true;
            this.conversionTextBox.Name = "conversionTextBox";
            this.conversionTextBox.ReadOnly = true;
            this.conversionTextBox.Size = new System.Drawing.Size(186, 60);
            this.conversionTextBox.TabIndex = 14;
            this.conversionTextBox.Text = "Size kb before";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(7, 54);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(97, 13);
            this.label1.TabIndex = 13;
            this.label1.Text = "Used compression:";
            // 
            // groupBox6
            // 
            this.groupBox6.Controls.Add(this.showCompProgressBar);
            this.groupBox6.Controls.Add(this.label6);
            this.groupBox6.Controls.Add(this.label7);
            this.groupBox6.Controls.Add(this.soundSizeAfterTextBox);
            this.groupBox6.Location = new System.Drawing.Point(5, 223);
            this.groupBox6.Name = "groupBox6";
            this.groupBox6.Size = new System.Drawing.Size(216, 57);
            this.groupBox6.TabIndex = 9;
            this.groupBox6.TabStop = false;
            this.groupBox6.Text = "Sound after compression:";
            // 
            // showCompProgressBar
            // 
            this.showCompProgressBar.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.showCompProgressBar.Location = new System.Drawing.Point(5, 18);
            this.showCompProgressBar.Margin = new System.Windows.Forms.Padding(2);
            this.showCompProgressBar.Name = "showCompProgressBar";
            this.showCompProgressBar.Size = new System.Drawing.Size(143, 17);
            this.showCompProgressBar.TabIndex = 4;
            this.showCompProgressBar.Value = 50;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(5, 37);
            this.label6.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(21, 13);
            this.label6.TabIndex = 6;
            this.label6.Text = "0%";
            // 
            // label7
            // 
            this.label7.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(121, 37);
            this.label7.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(27, 13);
            this.label7.TabIndex = 7;
            this.label7.Text = "99%";
            // 
            // soundSizeAfterTextBox
            // 
            this.soundSizeAfterTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.soundSizeAfterTextBox.Location = new System.Drawing.Point(152, 18);
            this.soundSizeAfterTextBox.Margin = new System.Windows.Forms.Padding(2);
            this.soundSizeAfterTextBox.Name = "soundSizeAfterTextBox";
            this.soundSizeAfterTextBox.Size = new System.Drawing.Size(59, 20);
            this.soundSizeAfterTextBox.TabIndex = 3;
            this.soundSizeAfterTextBox.Text = "SizeAfter";
            // 
            // groupBox7
            // 
            this.groupBox7.Controls.Add(this.label20);
            this.groupBox7.Controls.Add(this.label19);
            this.groupBox7.Controls.Add(this.label18);
            this.groupBox7.Controls.Add(this.label17);
            this.groupBox7.Controls.Add(this.compressionTrackBar);
            this.groupBox7.Location = new System.Drawing.Point(5, 135);
            this.groupBox7.Name = "groupBox7";
            this.groupBox7.Size = new System.Drawing.Size(191, 82);
            this.groupBox7.TabIndex = 10;
            this.groupBox7.TabStop = false;
            this.groupBox7.Text = "Sample Conversion";
            // 
            // label20
            // 
            this.label20.AutoSize = true;
            this.label20.Location = new System.Drawing.Point(161, 51);
            this.label20.Name = "label20";
            this.label20.Size = new System.Drawing.Size(19, 13);
            this.label20.TabIndex = 4;
            this.label20.Text = "6k";
            // 
            // label19
            // 
            this.label19.AutoSize = true;
            this.label19.Location = new System.Drawing.Point(109, 51);
            this.label19.Name = "label19";
            this.label19.Size = new System.Drawing.Size(25, 13);
            this.label19.TabIndex = 3;
            this.label19.Text = "12k";
            // 
            // label18
            // 
            this.label18.AutoSize = true;
            this.label18.Location = new System.Drawing.Point(55, 51);
            this.label18.Name = "label18";
            this.label18.Size = new System.Drawing.Size(25, 13);
            this.label18.TabIndex = 2;
            this.label18.Text = "25k";
            // 
            // label17
            // 
            this.label17.AutoSize = true;
            this.label17.Location = new System.Drawing.Point(7, 51);
            this.label17.Name = "label17";
            this.label17.Size = new System.Drawing.Size(25, 13);
            this.label17.TabIndex = 1;
            this.label17.Text = "50k";
            // 
            // compressionTrackBar
            // 
            this.compressionTrackBar.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.compressionTrackBar.Location = new System.Drawing.Point(5, 18);
            this.compressionTrackBar.Margin = new System.Windows.Forms.Padding(2);
            this.compressionTrackBar.Maximum = 3;
            this.compressionTrackBar.Name = "compressionTrackBar";
            this.compressionTrackBar.Size = new System.Drawing.Size(181, 45);
            this.compressionTrackBar.TabIndex = 0;
            this.compressionTrackBar.Scroll += new System.EventHandler(this.compressionTrackBar_Scroll);
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.Location = new System.Drawing.Point(6, 30);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(66, 13);
            this.label14.TabIndex = 12;
            this.label14.Text = "Original size:";
            // 
            // soundSizeTextBox
            // 
            this.soundSizeTextBox.Location = new System.Drawing.Point(77, 27);
            this.soundSizeTextBox.Margin = new System.Windows.Forms.Padding(2);
            this.soundSizeTextBox.Name = "soundSizeTextBox";
            this.soundSizeTextBox.Size = new System.Drawing.Size(90, 20);
            this.soundSizeTextBox.TabIndex = 2;
            this.soundSizeTextBox.Text = "Size kb before";
            // 
            // useInSpiButton
            // 
            this.useInSpiButton.Enabled = false;
            this.useInSpiButton.Location = new System.Drawing.Point(534, 152);
            this.useInSpiButton.Name = "useInSpiButton";
            this.useInSpiButton.Size = new System.Drawing.Size(48, 77);
            this.useInSpiButton.TabIndex = 11;
            this.useInSpiButton.Text = "Use sound in SPI --->";
            this.useInSpiButton.UseVisualStyleBackColor = true;
            this.useInSpiButton.Click += new System.EventHandler(this.useInSpiButton_Click);
            // 
            // menuStrip1
            // 
            this.menuStrip1.ImageScalingSize = new System.Drawing.Size(24, 24);
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.toolsToolStripMenuItem,
            this.aboutToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Padding = new System.Windows.Forms.Padding(4, 2, 0, 2);
            this.menuStrip1.Size = new System.Drawing.Size(910, 24);
            this.menuStrip1.TabIndex = 8;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.loadProjectToolStripMenuItem,
            this.saveProjectToolStripMenuItem,
            this.clearSettingsToolStripMenuItem,
            this.exitToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
            this.fileToolStripMenuItem.Text = "File";
            // 
            // loadProjectToolStripMenuItem
            // 
            this.loadProjectToolStripMenuItem.Name = "loadProjectToolStripMenuItem";
            this.loadProjectToolStripMenuItem.Size = new System.Drawing.Size(154, 22);
            this.loadProjectToolStripMenuItem.Text = "Load Project...";
            this.loadProjectToolStripMenuItem.Click += new System.EventHandler(this.loadProjectToolStripMenuItem_Click);
            // 
            // saveProjectToolStripMenuItem
            // 
            this.saveProjectToolStripMenuItem.Name = "saveProjectToolStripMenuItem";
            this.saveProjectToolStripMenuItem.Size = new System.Drawing.Size(154, 22);
            this.saveProjectToolStripMenuItem.Text = "Save Project...";
            this.saveProjectToolStripMenuItem.Click += new System.EventHandler(this.saveProjectToolStripMenuItem_Click);
            // 
            // exitToolStripMenuItem
            // 
            this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            this.exitToolStripMenuItem.Size = new System.Drawing.Size(154, 22);
            this.exitToolStripMenuItem.Text = "Exit";
            this.exitToolStripMenuItem.Click += new System.EventHandler(this.exitToolStripMenuItem_Click);
            // 
            // toolsToolStripMenuItem
            // 
            this.toolsToolStripMenuItem.Name = "toolsToolStripMenuItem";
            this.toolsToolStripMenuItem.Size = new System.Drawing.Size(47, 20);
            this.toolsToolStripMenuItem.Text = "Tools";
            // 
            // aboutToolStripMenuItem
            // 
            this.aboutToolStripMenuItem.Name = "aboutToolStripMenuItem";
            this.aboutToolStripMenuItem.Size = new System.Drawing.Size(52, 20);
            this.aboutToolStripMenuItem.Text = "About";
            // 
            // loadSoundFileDialog
            // 
            this.loadSoundFileDialog.FileName = "openFileDialog1";
            this.loadSoundFileDialog.Filter = "wav files (*.wav)|*.wav|All files (*.*)|*.*\"";
            this.loadSoundFileDialog.Multiselect = true;
            this.loadSoundFileDialog.Title = "Load Sound...";
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.label4);
            this.groupBox4.Controls.Add(this.label3);
            this.groupBox4.Controls.Add(this.label2);
            this.groupBox4.Controls.Add(this.totalSizeProgressBar);
            this.groupBox4.Controls.Add(this.totalSizeTextBox);
            this.groupBox4.Location = new System.Drawing.Point(31, 21);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(161, 83);
            this.groupBox4.TabIndex = 15;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "Size";
            // 
            // groupBox8
            // 
            this.groupBox8.Controls.Add(this.spiInfoTextBox);
            this.groupBox8.Controls.Add(this.label23);
            this.groupBox8.Controls.Add(this.label22);
            this.groupBox8.Controls.Add(this.groupBox4);
            this.groupBox8.Controls.Add(this.saveSpiButton);
            this.groupBox8.Controls.Add(this.spiNameTextBox);
            this.groupBox8.Location = new System.Drawing.Point(31, 407);
            this.groupBox8.Name = "groupBox8";
            this.groupBox8.Size = new System.Drawing.Size(769, 122);
            this.groupBox8.TabIndex = 11;
            this.groupBox8.TabStop = false;
            this.groupBox8.Text = "EPSS SPI - Sound Patch Information";
            // 
            // spiInfoTextBox
            // 
            this.spiInfoTextBox.Location = new System.Drawing.Point(620, 52);
            this.spiInfoTextBox.Margin = new System.Windows.Forms.Padding(2);
            this.spiInfoTextBox.MaxLength = 16;
            this.spiInfoTextBox.Name = "spiInfoTextBox";
            this.spiInfoTextBox.Size = new System.Drawing.Size(128, 20);
            this.spiInfoTextBox.TabIndex = 18;
            this.spiInfoTextBox.Text = "Created with EPSSEditor";
            // 
            // label23
            // 
            this.label23.AutoSize = true;
            this.label23.Location = new System.Drawing.Point(493, 52);
            this.label23.Name = "label23";
            this.label23.Size = new System.Drawing.Size(48, 13);
            this.label23.TabIndex = 17;
            this.label23.Text = "SPI Info:";
            // 
            // label22
            // 
            this.label22.AutoSize = true;
            this.label22.Location = new System.Drawing.Point(493, 21);
            this.label22.Name = "label22";
            this.label22.Size = new System.Drawing.Size(58, 13);
            this.label22.TabIndex = 16;
            this.label22.Text = "SPI Name:";
            // 
            // saveSpiFileDialog
            // 
            this.saveSpiFileDialog.Filter = "Spi files (*.spi)|*.spi|All files (*.*)|*.*\"";
            this.saveSpiFileDialog.Title = "Save EPSS Spi...";
            // 
            // saveProjectFileDialog
            // 
            this.saveProjectFileDialog.Filter = "EPSS Project files (*.epf)|*.epf|All files (*.*)|*.*\"";
            this.saveProjectFileDialog.Title = "Save EPSS Project...";
            // 
            // loadProjectFileDialog
            // 
            this.loadProjectFileDialog.FileName = "openFileDialog1";
            this.loadProjectFileDialog.Title = "Load EPSS Project...";
            // 
            // clearSettingsToolStripMenuItem
            // 
            this.clearSettingsToolStripMenuItem.Name = "clearSettingsToolStripMenuItem";
            this.clearSettingsToolStripMenuItem.Size = new System.Drawing.Size(154, 22);
            this.clearSettingsToolStripMenuItem.Text = "Clear settings...";
            this.clearSettingsToolStripMenuItem.Click += new System.EventHandler(this.clearSettingsToolStripMenuItem_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(910, 544);
            this.Controls.Add(this.groupBox8);
            this.Controls.Add(this.useInSpiButton);
            this.Controls.Add(this.compressionTypeTextBox);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.menuStrip1);
            this.MainMenuStrip = this.menuStrip1;
            this.Margin = new System.Windows.Forms.Padding(2);
            this.Name = "Form1";
            this.Text = "EPSS Editor v1.0";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox9.ResumeLayout(false);
            this.groupBox9.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.midiChTrackBar)).EndInit();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.groupBox5.ResumeLayout(false);
            this.groupBox5.PerformLayout();
            this.compressionTypeTextBox.ResumeLayout(false);
            this.compressionTypeTextBox.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.normalizeTrackBar)).EndInit();
            this.groupBox6.ResumeLayout(false);
            this.groupBox6.PerformLayout();
            this.groupBox7.ResumeLayout(false);
            this.groupBox7.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.compressionTrackBar)).EndInit();
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.groupBox4.ResumeLayout(false);
            this.groupBox4.PerformLayout();
            this.groupBox8.ResumeLayout(false);
            this.groupBox8.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.ListBox soundListBox;
        private System.Windows.Forms.Button saveSpiButton;
        private System.Windows.Forms.ListBox spiSoundListBox;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.TextBox totalSizeTextBox;
        private System.Windows.Forms.TextBox spiNameTextBox;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.GroupBox compressionTypeTextBox;
        private System.Windows.Forms.TrackBar compressionTrackBar;
        private System.Windows.Forms.TextBox soundSizeAfterTextBox;
        private System.Windows.Forms.TextBox soundSizeTextBox;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ProgressBar totalSizeProgressBar;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.ProgressBar showCompProgressBar;
        private System.Windows.Forms.GroupBox groupBox5;
        private System.Windows.Forms.TextBox textBox6;
        private System.Windows.Forms.TextBox freqTextBox;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem loadProjectToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem saveProjectToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem toolsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem aboutToolStripMenuItem;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.ComboBox drumsComboBox1;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.Button deleteSoundButton;
        private System.Windows.Forms.Button clearAllSoundsButton;
        private System.Windows.Forms.Button loadSoundButton;
        private System.Windows.Forms.OpenFileDialog loadSoundFileDialog;
        private System.Windows.Forms.Button playButton;
        private System.Windows.Forms.ToolTip infoToolTip;
        private System.Windows.Forms.Label label14;
        private System.Windows.Forms.TextBox conversionTextBox;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.GroupBox groupBox9;
        private System.Windows.Forms.GroupBox groupBox6;
        private System.Windows.Forms.GroupBox groupBox7;
        private System.Windows.Forms.Label label16;
        private System.Windows.Forms.Label label15;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox channelsTextBox;
        private System.Windows.Forms.TextBox bitsTextBox;
        private System.Windows.Forms.Label label20;
        private System.Windows.Forms.Label label19;
        private System.Windows.Forms.Label label18;
        private System.Windows.Forms.Label label17;
        private System.Windows.Forms.TrackBar midiChTrackBar;
        private System.Windows.Forms.TextBox midiChTextBox;
        private System.Windows.Forms.Label label21;
        private System.Windows.Forms.RadioButton GmPercMidiMappingRadioButton;
        private System.Windows.Forms.RadioButton defaultMidiMapRadioButton;
        private System.Windows.Forms.Button useInSpiButton;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.GroupBox groupBox8;
        private System.Windows.Forms.Button deleteSpiSoundButton;
        private System.Windows.Forms.SaveFileDialog saveSpiFileDialog;
        private System.Windows.Forms.TextBox spiInfoTextBox;
        private System.Windows.Forms.Label label23;
        private System.Windows.Forms.Label label22;
        private System.Windows.Forms.SaveFileDialog saveProjectFileDialog;
        private System.Windows.Forms.OpenFileDialog loadProjectFileDialog;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.TextBox normalizeTextBox;
        private System.Windows.Forms.CheckBox normalizeCheckBox;
        private System.Windows.Forms.TrackBar normalizeTrackBar;
        private System.Windows.Forms.ToolStripMenuItem clearSettingsToolStripMenuItem;
    }
}

