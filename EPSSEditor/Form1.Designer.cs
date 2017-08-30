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
            this.button1 = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.soundListBox = new System.Windows.Forms.ListBox();
            this.button2 = new System.Windows.Forms.Button();
            this.listBox2 = new System.Windows.Forms.ListBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.label12 = new System.Windows.Forms.Label();
            this.label11 = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.progressBar1 = new System.Windows.Forms.ProgressBar();
            this.textBox2 = new System.Windows.Forms.TextBox();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.playButton = new System.Windows.Forms.Button();
            this.loadSoundButton = new System.Windows.Forms.Button();
            this.label13 = new System.Windows.Forms.Label();
            this.deleteSoundButton = new System.Windows.Forms.Button();
            this.groupBox5 = new System.Windows.Forms.GroupBox();
            this.textBox6 = new System.Windows.Forms.TextBox();
            this.textBox5 = new System.Windows.Forms.TextBox();
            this.clearAllSoundsButton = new System.Windows.Forms.Button();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.drumsComboBox1 = new System.Windows.Forms.ComboBox();
            this.checkBox1 = new System.Windows.Forms.CheckBox();
            this.label8 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.progressBar2 = new System.Windows.Forms.ProgressBar();
            this.textBox4 = new System.Windows.Forms.TextBox();
            this.textBox3 = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.trackBar1 = new System.Windows.Forms.TrackBar();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.loadProjectToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveProjectToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.aboutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.loadSoundFileDialog = new System.Windows.Forms.OpenFileDialog();
            this.groupBox2.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.groupBox5.SuspendLayout();
            this.groupBox4.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.trackBar1)).BeginInit();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(578, 366);
            this.button1.Margin = new System.Windows.Forms.Padding(2);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(110, 27);
            this.button1.TabIndex = 8;
            this.button1.Text = "OK";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Location = new System.Drawing.Point(11, 317);
            this.groupBox1.Margin = new System.Windows.Forms.Padding(2);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Padding = new System.Windows.Forms.Padding(2);
            this.groupBox1.Size = new System.Drawing.Size(670, 45);
            this.groupBox1.TabIndex = 1;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "groupBox1";
            // 
            // soundListBox
            // 
            this.soundListBox.AllowDrop = true;
            this.soundListBox.FormattingEnabled = true;
            this.soundListBox.Items.AddRange(new object[] {
            "Bass Drum.wav"});
            this.soundListBox.Location = new System.Drawing.Point(13, 34);
            this.soundListBox.Margin = new System.Windows.Forms.Padding(2);
            this.soundListBox.Name = "soundListBox";
            this.soundListBox.Size = new System.Drawing.Size(216, 95);
            this.soundListBox.TabIndex = 2;
            this.soundListBox.DragDrop += new System.Windows.Forms.DragEventHandler(this.soundListBox_DragDrop);
            this.soundListBox.DragEnter += new System.Windows.Forms.DragEventHandler(this.soundListBox_DragEnter);
            this.soundListBox.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.soundListBox_KeyPress);
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(16, 293);
            this.button2.Margin = new System.Windows.Forms.Padding(2);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(99, 20);
            this.button2.TabIndex = 3;
            this.button2.Text = "Load all sounds...";
            this.button2.UseVisualStyleBackColor = true;
            // 
            // listBox2
            // 
            this.listBox2.FormattingEnabled = true;
            this.listBox2.Items.AddRange(new object[] {
            "1      1      Bass Drum  27kb"});
            this.listBox2.Location = new System.Drawing.Point(9, 37);
            this.listBox2.Margin = new System.Windows.Forms.Padding(2);
            this.listBox2.Name = "listBox2";
            this.listBox2.Size = new System.Drawing.Size(241, 108);
            this.listBox2.TabIndex = 4;
            this.listBox2.SelectedIndexChanged += new System.EventHandler(this.listBox2_SelectedIndexChanged);
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.label12);
            this.groupBox2.Controls.Add(this.label11);
            this.groupBox2.Controls.Add(this.label10);
            this.groupBox2.Controls.Add(this.label9);
            this.groupBox2.Controls.Add(this.label4);
            this.groupBox2.Controls.Add(this.label3);
            this.groupBox2.Controls.Add(this.label2);
            this.groupBox2.Controls.Add(this.progressBar1);
            this.groupBox2.Controls.Add(this.textBox2);
            this.groupBox2.Controls.Add(this.textBox1);
            this.groupBox2.Controls.Add(this.listBox2);
            this.groupBox2.Location = new System.Drawing.Point(473, 35);
            this.groupBox2.Margin = new System.Windows.Forms.Padding(2);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Padding = new System.Windows.Forms.Padding(2);
            this.groupBox2.Size = new System.Drawing.Size(259, 238);
            this.groupBox2.TabIndex = 5;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "SPI";
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
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(213, 197);
            this.label4.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(35, 13);
            this.label4.TabIndex = 10;
            this.label4.Text = "14MB";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(126, 197);
            this.label3.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(29, 13);
            this.label3.TabIndex = 9;
            this.label3.Text = "4MB";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(94, 197);
            this.label2.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(29, 13);
            this.label2.TabIndex = 8;
            this.label2.Text = "1MB";
            // 
            // progressBar1
            // 
            this.progressBar1.Location = new System.Drawing.Point(94, 214);
            this.progressBar1.Margin = new System.Windows.Forms.Padding(2);
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Size = new System.Drawing.Size(147, 20);
            this.progressBar1.TabIndex = 7;
            this.progressBar1.Value = 50;
            // 
            // textBox2
            // 
            this.textBox2.Location = new System.Drawing.Point(125, 175);
            this.textBox2.Margin = new System.Windows.Forms.Padding(2);
            this.textBox2.Name = "textBox2";
            this.textBox2.Size = new System.Drawing.Size(122, 20);
            this.textBox2.TabIndex = 6;
            this.textBox2.Text = "Total Size in Bytes";
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(120, 154);
            this.textBox1.Margin = new System.Windows.Forms.Padding(2);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(128, 20);
            this.textBox1.TabIndex = 5;
            this.textBox1.Text = "Filename.SPI";
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
            this.groupBox3.Location = new System.Drawing.Point(16, 33);
            this.groupBox3.Margin = new System.Windows.Forms.Padding(2);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Padding = new System.Windows.Forms.Padding(2);
            this.groupBox3.Size = new System.Drawing.Size(247, 247);
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
            this.playButton.Click += new System.EventHandler(this.playButton_Click);
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
            this.deleteSoundButton.UseVisualStyleBackColor = true;
            this.deleteSoundButton.Click += new System.EventHandler(this.deleteSoundButton_Click);
            // 
            // groupBox5
            // 
            this.groupBox5.Controls.Add(this.textBox6);
            this.groupBox5.Controls.Add(this.textBox5);
            this.groupBox5.Location = new System.Drawing.Point(9, 159);
            this.groupBox5.Margin = new System.Windows.Forms.Padding(2);
            this.groupBox5.Name = "groupBox5";
            this.groupBox5.Padding = new System.Windows.Forms.Padding(2);
            this.groupBox5.Size = new System.Drawing.Size(220, 76);
            this.groupBox5.TabIndex = 3;
            this.groupBox5.TabStop = false;
            this.groupBox5.Text = "Sample Info";
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
            // textBox5
            // 
            this.textBox5.Location = new System.Drawing.Point(148, 48);
            this.textBox5.Margin = new System.Windows.Forms.Padding(2);
            this.textBox5.Name = "textBox5";
            this.textBox5.Size = new System.Drawing.Size(68, 20);
            this.textBox5.TabIndex = 0;
            this.textBox5.Text = "Freq 48kHz";
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
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.drumsComboBox1);
            this.groupBox4.Controls.Add(this.checkBox1);
            this.groupBox4.Controls.Add(this.label8);
            this.groupBox4.Controls.Add(this.label7);
            this.groupBox4.Controls.Add(this.label6);
            this.groupBox4.Controls.Add(this.label5);
            this.groupBox4.Controls.Add(this.progressBar2);
            this.groupBox4.Controls.Add(this.textBox4);
            this.groupBox4.Controls.Add(this.textBox3);
            this.groupBox4.Controls.Add(this.label1);
            this.groupBox4.Controls.Add(this.trackBar1);
            this.groupBox4.Location = new System.Drawing.Point(291, 33);
            this.groupBox4.Margin = new System.Windows.Forms.Padding(2);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Padding = new System.Windows.Forms.Padding(2);
            this.groupBox4.Size = new System.Drawing.Size(171, 247);
            this.groupBox4.TabIndex = 7;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "Conversion parameters";
            // 
            // drumsComboBox1
            // 
            this.drumsComboBox1.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.drumsComboBox1.FormattingEnabled = true;
            this.drumsComboBox1.Location = new System.Drawing.Point(26, 223);
            this.drumsComboBox1.Margin = new System.Windows.Forms.Padding(2);
            this.drumsComboBox1.Name = "drumsComboBox1";
            this.drumsComboBox1.Size = new System.Drawing.Size(141, 21);
            this.drumsComboBox1.TabIndex = 11;
            // 
            // checkBox1
            // 
            this.checkBox1.AutoSize = true;
            this.checkBox1.Location = new System.Drawing.Point(4, 189);
            this.checkBox1.Margin = new System.Windows.Forms.Padding(2);
            this.checkBox1.Name = "checkBox1";
            this.checkBox1.Size = new System.Drawing.Size(176, 17);
            this.checkBox1.TabIndex = 10;
            this.checkBox1.Text = "Use Full MIDI Channel Mapping";
            this.checkBox1.UseVisualStyleBackColor = true;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(24, 210);
            this.label8.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(76, 13);
            this.label8.TabIndex = 9;
            this.label8.Text = "GM Drum Map";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(116, 159);
            this.label7.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(27, 13);
            this.label7.TabIndex = 7;
            this.label7.Text = "99%";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(4, 159);
            this.label6.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(21, 13);
            this.label6.TabIndex = 6;
            this.label6.Text = "0%";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(4, 123);
            this.label5.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(67, 13);
            this.label5.TabIndex = 5;
            this.label5.Text = "Compression";
            // 
            // progressBar2
            // 
            this.progressBar2.Location = new System.Drawing.Point(4, 138);
            this.progressBar2.Margin = new System.Windows.Forms.Padding(2);
            this.progressBar2.Name = "progressBar2";
            this.progressBar2.Size = new System.Drawing.Size(131, 17);
            this.progressBar2.TabIndex = 4;
            this.progressBar2.Value = 50;
            // 
            // textBox4
            // 
            this.textBox4.Location = new System.Drawing.Point(64, 96);
            this.textBox4.Margin = new System.Windows.Forms.Padding(2);
            this.textBox4.Name = "textBox4";
            this.textBox4.Size = new System.Drawing.Size(90, 20);
            this.textBox4.TabIndex = 3;
            this.textBox4.Text = "Size kb after";
            this.textBox4.TextChanged += new System.EventHandler(this.textBox4_TextChanged);
            // 
            // textBox3
            // 
            this.textBox3.Location = new System.Drawing.Point(64, 28);
            this.textBox3.Margin = new System.Windows.Forms.Padding(2);
            this.textBox3.Name = "textBox3";
            this.textBox3.Size = new System.Drawing.Size(90, 20);
            this.textBox3.TabIndex = 2;
            this.textBox3.Text = "Size kb before";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(18, 46);
            this.label1.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(67, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Compression";
            // 
            // trackBar1
            // 
            this.trackBar1.Location = new System.Drawing.Point(21, 61);
            this.trackBar1.Margin = new System.Windows.Forms.Padding(2);
            this.trackBar1.Name = "trackBar1";
            this.trackBar1.Size = new System.Drawing.Size(116, 45);
            this.trackBar1.TabIndex = 0;
            this.trackBar1.Scroll += new System.EventHandler(this.trackBar1_Scroll);
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
            this.menuStrip1.Size = new System.Drawing.Size(840, 24);
            this.menuStrip1.TabIndex = 8;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.loadProjectToolStripMenuItem,
            this.saveProjectToolStripMenuItem,
            this.exitToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
            this.fileToolStripMenuItem.Text = "File";
            // 
            // loadProjectToolStripMenuItem
            // 
            this.loadProjectToolStripMenuItem.Name = "loadProjectToolStripMenuItem";
            this.loadProjectToolStripMenuItem.Size = new System.Drawing.Size(149, 22);
            this.loadProjectToolStripMenuItem.Text = "Load Project...";
            // 
            // saveProjectToolStripMenuItem
            // 
            this.saveProjectToolStripMenuItem.Name = "saveProjectToolStripMenuItem";
            this.saveProjectToolStripMenuItem.Size = new System.Drawing.Size(149, 22);
            this.saveProjectToolStripMenuItem.Text = "Save Project...";
            // 
            // exitToolStripMenuItem
            // 
            this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            this.exitToolStripMenuItem.Size = new System.Drawing.Size(149, 22);
            this.exitToolStripMenuItem.Text = "Exit";
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
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(840, 471);
            this.Controls.Add(this.groupBox4);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.menuStrip1);
            this.MainMenuStrip = this.menuStrip1;
            this.Margin = new System.Windows.Forms.Padding(2);
            this.Name = "Form1";
            this.Text = "Form1";
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.Form1_KeyDown);
            this.KeyUp += new System.Windows.Forms.KeyEventHandler(this.Form1_KeyUp);
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.groupBox5.ResumeLayout(false);
            this.groupBox5.PerformLayout();
            this.groupBox4.ResumeLayout(false);
            this.groupBox4.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.trackBar1)).EndInit();
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.ListBox soundListBox;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.ListBox listBox2;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.TextBox textBox2;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TrackBar trackBar1;
        private System.Windows.Forms.TextBox textBox4;
        private System.Windows.Forms.TextBox textBox3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ProgressBar progressBar1;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.ProgressBar progressBar2;
        private System.Windows.Forms.GroupBox groupBox5;
        private System.Windows.Forms.TextBox textBox6;
        private System.Windows.Forms.TextBox textBox5;
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
        private System.Windows.Forms.CheckBox checkBox1;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.ComboBox drumsComboBox1;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.Button deleteSoundButton;
        private System.Windows.Forms.Button clearAllSoundsButton;
        private System.Windows.Forms.Button loadSoundButton;
        private System.Windows.Forms.OpenFileDialog loadSoundFileDialog;
        private System.Windows.Forms.Button playButton;
    }
}

