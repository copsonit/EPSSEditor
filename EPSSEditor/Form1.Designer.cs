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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.soundListBox = new System.Windows.Forms.ListBox();
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.button1 = new System.Windows.Forms.Button();
            this.previewComboBox = new System.Windows.Forms.ComboBox();
            this.label9 = new System.Windows.Forms.Label();
            this.spiSoundListenButton = new System.Windows.Forms.Button();
            this.spiSoundListView = new System.Windows.Forms.ListView();
            this.deleteSpiSoundButton = new System.Windows.Forms.Button();
            this.groupBox9 = new System.Windows.Forms.GroupBox();
            this.groupBox12 = new System.Windows.Forms.GroupBox();
            this.mappingModeProgramRadioButton = new System.Windows.Forms.RadioButton();
            this.mappingModeMidiRadioButton = new System.Windows.Forms.RadioButton();
            this.groupBox11 = new System.Windows.Forms.GroupBox();
            this.custMidiToneCtrLabel = new System.Windows.Forms.Label();
            this.custMidToneCentreTextBox = new System.Windows.Forms.TextBox();
            this.defaultMidiMapRadioButton = new System.Windows.Forms.RadioButton();
            this.midiChTextBox = new System.Windows.Forms.TextBox();
            this.custMidiToneToTextBox = new System.Windows.Forms.TextBox();
            this.custMidiToneLabel = new System.Windows.Forms.Label();
            this.midiChTrackBar = new System.Windows.Forms.TrackBar();
            this.custMidiToneFromTextBox = new System.Windows.Forms.TextBox();
            this.CustomSampleRadioButton = new System.Windows.Forms.RadioButton();
            this.midiToneLabel = new System.Windows.Forms.Label();
            this.midiToneTextBox = new System.Windows.Forms.TextBox();
            this.MultiSampleRadioButton = new System.Windows.Forms.RadioButton();
            this.GmPercMidiMappingRadioButton = new System.Windows.Forms.RadioButton();
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
            this.sizeTextBox = new System.Windows.Forms.TextBox();
            this.freqTextBox = new System.Windows.Forms.TextBox();
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
            this.useInSpiButton = new System.Windows.Forms.Button();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.newProjectToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.loadProjectToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveProjectToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.loadSPIToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.importSFZToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveSPIToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveSFZToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.clearSettingsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.helpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.checkForUpdatesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.loadSoundFileDialog = new System.Windows.Forms.OpenFileDialog();
            this.infoToolTip = new System.Windows.Forms.ToolTip(this.components);
            this.omniPatchCheckBox = new System.Windows.Forms.CheckBox();
            this.gen2CheckBox = new System.Windows.Forms.CheckBox();
            this.loadMidButton = new System.Windows.Forms.Button();
            this.stopMidButton = new System.Windows.Forms.Button();
            this.playMidButton = new System.Windows.Forms.Button();
            this.revMidButton = new System.Windows.Forms.Button();
            this.ffwMidButton = new System.Windows.Forms.Button();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.groupBox8 = new System.Windows.Forms.GroupBox();
            this.groupBox10 = new System.Windows.Forms.GroupBox();
            this.label22 = new System.Windows.Forms.Label();
            this.label23 = new System.Windows.Forms.Label();
            this.spiInfoTextBox = new System.Windows.Forms.TextBox();
            this.saveSpiFileDialog = new System.Windows.Forms.SaveFileDialog();
            this.saveProjectFileDialog = new System.Windows.Forms.SaveFileDialog();
            this.loadProjectFileDialog = new System.Windows.Forms.OpenFileDialog();
            this.openDrumMappingsFileDialog = new System.Windows.Forms.OpenFileDialog();
            this.saveSampleFileDialog = new System.Windows.Forms.SaveFileDialog();
            this.loadSpiFileDialog = new System.Windows.Forms.OpenFileDialog();
            this.saveSfzFileDialog = new System.Windows.Forms.SaveFileDialog();
            this.loadSfzFileDialog = new System.Windows.Forms.OpenFileDialog();
            this.helpProvider1 = new System.Windows.Forms.HelpProvider();
            this.folderBrowserDialog1 = new System.Windows.Forms.FolderBrowserDialog();
            this.groupBox13 = new System.Windows.Forms.GroupBox();
            this.midFileBarTextBox = new System.Windows.Forms.TextBox();
            this.loadMidFileDialog = new System.Windows.Forms.OpenFileDialog();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.revMidTimer = new System.Windows.Forms.Timer(this.components);
            this.contextMenuStrip1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox9.SuspendLayout();
            this.groupBox12.SuspendLayout();
            this.groupBox11.SuspendLayout();
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
            this.groupBox10.SuspendLayout();
            this.groupBox13.SuspendLayout();
            this.SuspendLayout();
            // 
            // soundListBox
            // 
            this.soundListBox.AllowDrop = true;
            this.soundListBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.soundListBox.ContextMenuStrip = this.contextMenuStrip1;
            this.soundListBox.FormattingEnabled = true;
            this.soundListBox.Location = new System.Drawing.Point(11, 27);
            this.soundListBox.Margin = new System.Windows.Forms.Padding(2, 1, 2, 1);
            this.soundListBox.Name = "soundListBox";
            this.soundListBox.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
            this.soundListBox.Size = new System.Drawing.Size(213, 394);
            this.soundListBox.TabIndex = 2;
            this.soundListBox.SelectedIndexChanged += new System.EventHandler(this.soundListBox_SelectedIndexChanged);
            this.soundListBox.DragDrop += new System.Windows.Forms.DragEventHandler(this.soundListBox_DragDrop);
            this.soundListBox.DragEnter += new System.Windows.Forms.DragEventHandler(this.soundListBox_DragEnter);
            this.soundListBox.KeyDown += new System.Windows.Forms.KeyEventHandler(this.soundListBox_KeyDown);
            this.soundListBox.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.soundListBox_KeyPress);
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripMenuItem1});
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(130, 26);
            // 
            // toolStripMenuItem1
            // 
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            this.toolStripMenuItem1.Size = new System.Drawing.Size(129, 22);
            this.toolStripMenuItem1.Text = "Rename ...";
            this.toolStripMenuItem1.Click += new System.EventHandler(this.toolStripMenuItem1_Click);
            // 
            // groupBox2
            // 
            this.groupBox2.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox2.Controls.Add(this.button1);
            this.groupBox2.Controls.Add(this.previewComboBox);
            this.groupBox2.Controls.Add(this.label9);
            this.groupBox2.Controls.Add(this.spiSoundListenButton);
            this.groupBox2.Controls.Add(this.spiSoundListView);
            this.groupBox2.Controls.Add(this.deleteSpiSoundButton);
            this.helpProvider1.SetHelpKeyword(this.groupBox2, "spiSounds");
            this.helpProvider1.SetHelpString(this.groupBox2, "Change transpose with Shift+mouse wheel on sound. Reset transpose with Shift+midd" +
        "le mouse button on sound.");
            this.groupBox2.Location = new System.Drawing.Point(502, 27);
            this.groupBox2.Margin = new System.Windows.Forms.Padding(2, 1, 2, 1);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Padding = new System.Windows.Forms.Padding(2, 1, 2, 1);
            this.helpProvider1.SetShowHelp(this.groupBox2, true);
            this.groupBox2.Size = new System.Drawing.Size(565, 460);
            this.groupBox2.TabIndex = 5;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "SPI Sounds";
            this.infoToolTip.SetToolTip(this.groupBox2, "Change transpose with Shift+mouse wheel on sound. Reset transpose with Shift+midd" +
        "le mouse button on sound.");
            // 
            // button1
            // 
            this.button1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.button1.Enabled = false;
            this.button1.Location = new System.Drawing.Point(349, 427);
            this.button1.Margin = new System.Windows.Forms.Padding(2, 1, 2, 1);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(71, 23);
            this.button1.TabIndex = 18;
            this.button1.Text = "Save...";
            this.infoToolTip.SetToolTip(this.button1, "Saving all selected SPI Sounds to file one by one.");
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.saveSampleButton_Click);
            // 
            // previewComboBox
            // 
            this.previewComboBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.previewComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.previewComboBox.FormattingEnabled = true;
            this.previewComboBox.Items.AddRange(new object[] {
            "Premixed for 4 Channel EPSS",
            "Premixed for 8 Channel EPSS",
            "Unmixed 8 Bit "});
            this.previewComboBox.Location = new System.Drawing.Point(55, 430);
            this.previewComboBox.Margin = new System.Windows.Forms.Padding(2, 1, 2, 1);
            this.previewComboBox.Name = "previewComboBox";
            this.previewComboBox.Size = new System.Drawing.Size(290, 21);
            this.previewComboBox.TabIndex = 17;
            this.previewComboBox.SelectedIndexChanged += new System.EventHandler(this.previewComboBox_SelectedIndexChanged);
            // 
            // label9
            // 
            this.label9.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(4, 432);
            this.label9.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(48, 13);
            this.label9.TabIndex = 16;
            this.label9.Text = "Preview:";
            // 
            // spiSoundListenButton
            // 
            this.spiSoundListenButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.spiSoundListenButton.Location = new System.Drawing.Point(425, 427);
            this.spiSoundListenButton.Name = "spiSoundListenButton";
            this.spiSoundListenButton.Size = new System.Drawing.Size(66, 23);
            this.spiSoundListenButton.TabIndex = 8;
            this.spiSoundListenButton.Text = "Listen";
            this.infoToolTip.SetToolTip(this.spiSoundListenButton, "Listen to SPI Sound sample. Only first selected is played.");
            this.spiSoundListenButton.UseVisualStyleBackColor = true;
            this.spiSoundListenButton.Click += new System.EventHandler(this.spiSoundListenButton_Click);
            this.spiSoundListenButton.MouseDown += new System.Windows.Forms.MouseEventHandler(this.spiSoundListenButton_MouseDown);
            this.spiSoundListenButton.MouseUp += new System.Windows.Forms.MouseEventHandler(this.spiSoundListenButton_MouseUp);
            // 
            // spiSoundListView
            // 
            this.spiSoundListView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.spiSoundListView.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
            this.spiSoundListView.HideSelection = false;
            this.spiSoundListView.Location = new System.Drawing.Point(5, 19);
            this.spiSoundListView.Name = "spiSoundListView";
            this.spiSoundListView.Size = new System.Drawing.Size(556, 402);
            this.spiSoundListView.TabIndex = 15;
            this.spiSoundListView.UseCompatibleStateImageBehavior = false;
            this.spiSoundListView.ItemSelectionChanged += new System.Windows.Forms.ListViewItemSelectionChangedEventHandler(this.spiSoundListView_ItemSelectionChanged);
            this.spiSoundListView.KeyDown += new System.Windows.Forms.KeyEventHandler(this.spiSoundListView_KeyDown);
            this.spiSoundListView.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.spiSoundListView_MouseDoubleClick);
            this.spiSoundListView.MouseUp += new System.Windows.Forms.MouseEventHandler(this.spiSoundListView_MouseUp);
            // 
            // deleteSpiSoundButton
            // 
            this.deleteSpiSoundButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.deleteSpiSoundButton.Enabled = false;
            this.deleteSpiSoundButton.Location = new System.Drawing.Point(500, 427);
            this.deleteSpiSoundButton.Name = "deleteSpiSoundButton";
            this.deleteSpiSoundButton.Size = new System.Drawing.Size(61, 23);
            this.deleteSpiSoundButton.TabIndex = 8;
            this.deleteSpiSoundButton.Text = "Delete";
            this.infoToolTip.SetToolTip(this.deleteSpiSoundButton, "You can also use the Delete button on your keyboard to delete an SPI Sound from t" +
        "he list");
            this.deleteSpiSoundButton.UseVisualStyleBackColor = true;
            this.deleteSpiSoundButton.Click += new System.EventHandler(this.deleteSpiSoundButton_Click);
            // 
            // groupBox9
            // 
            this.groupBox9.Controls.Add(this.groupBox12);
            this.groupBox9.Controls.Add(this.groupBox11);
            this.groupBox9.Location = new System.Drawing.Point(6, 305);
            this.groupBox9.Name = "groupBox9";
            this.groupBox9.Size = new System.Drawing.Size(211, 271);
            this.groupBox9.TabIndex = 11;
            this.groupBox9.TabStop = false;
            this.groupBox9.Text = "Sound Mapping";
            // 
            // groupBox12
            // 
            this.groupBox12.Controls.Add(this.mappingModeProgramRadioButton);
            this.groupBox12.Controls.Add(this.mappingModeMidiRadioButton);
            this.groupBox12.Location = new System.Drawing.Point(8, 20);
            this.groupBox12.Name = "groupBox12";
            this.groupBox12.Size = new System.Drawing.Size(201, 40);
            this.groupBox12.TabIndex = 23;
            this.groupBox12.TabStop = false;
            this.groupBox12.Text = "Mapping mode";
            // 
            // mappingModeProgramRadioButton
            // 
            this.mappingModeProgramRadioButton.AutoSize = true;
            this.mappingModeProgramRadioButton.Location = new System.Drawing.Point(99, 17);
            this.mappingModeProgramRadioButton.Name = "mappingModeProgramRadioButton";
            this.mappingModeProgramRadioButton.Size = new System.Drawing.Size(104, 17);
            this.mappingModeProgramRadioButton.TabIndex = 1;
            this.mappingModeProgramRadioButton.TabStop = true;
            this.mappingModeProgramRadioButton.Text = "Program Change";
            this.mappingModeProgramRadioButton.UseVisualStyleBackColor = true;
            this.mappingModeProgramRadioButton.CheckedChanged += new System.EventHandler(this.mappingModeProgramRadioButton_CheckedChanged);
            // 
            // mappingModeMidiRadioButton
            // 
            this.mappingModeMidiRadioButton.AutoSize = true;
            this.mappingModeMidiRadioButton.Location = new System.Drawing.Point(8, 17);
            this.mappingModeMidiRadioButton.Name = "mappingModeMidiRadioButton";
            this.mappingModeMidiRadioButton.Size = new System.Drawing.Size(90, 17);
            this.mappingModeMidiRadioButton.TabIndex = 0;
            this.mappingModeMidiRadioButton.TabStop = true;
            this.mappingModeMidiRadioButton.Text = "MIDI Channel";
            this.mappingModeMidiRadioButton.UseVisualStyleBackColor = true;
            this.mappingModeMidiRadioButton.CheckedChanged += new System.EventHandler(this.mappingModeMidiRadioButton_CheckedChanged);
            // 
            // groupBox11
            // 
            this.groupBox11.Controls.Add(this.custMidiToneCtrLabel);
            this.groupBox11.Controls.Add(this.custMidToneCentreTextBox);
            this.groupBox11.Controls.Add(this.defaultMidiMapRadioButton);
            this.groupBox11.Controls.Add(this.midiChTextBox);
            this.groupBox11.Controls.Add(this.custMidiToneToTextBox);
            this.groupBox11.Controls.Add(this.custMidiToneLabel);
            this.groupBox11.Controls.Add(this.midiChTrackBar);
            this.groupBox11.Controls.Add(this.custMidiToneFromTextBox);
            this.groupBox11.Controls.Add(this.CustomSampleRadioButton);
            this.groupBox11.Controls.Add(this.midiToneLabel);
            this.groupBox11.Controls.Add(this.midiToneTextBox);
            this.groupBox11.Controls.Add(this.MultiSampleRadioButton);
            this.groupBox11.Controls.Add(this.GmPercMidiMappingRadioButton);
            this.groupBox11.Controls.Add(this.drumsComboBox1);
            this.groupBox11.Location = new System.Drawing.Point(8, 60);
            this.groupBox11.Name = "groupBox11";
            this.groupBox11.Size = new System.Drawing.Size(195, 205);
            this.groupBox11.TabIndex = 22;
            this.groupBox11.TabStop = false;
            this.groupBox11.Text = "Mapping type";
            // 
            // custMidiToneCtrLabel
            // 
            this.custMidiToneCtrLabel.AutoSize = true;
            this.custMidiToneCtrLabel.Location = new System.Drawing.Point(146, 178);
            this.custMidiToneCtrLabel.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.custMidiToneCtrLabel.Name = "custMidiToneCtrLabel";
            this.custMidiToneCtrLabel.Size = new System.Drawing.Size(19, 13);
            this.custMidiToneCtrLabel.TabIndex = 23;
            this.custMidiToneCtrLabel.Text = "ctr";
            // 
            // custMidToneCentreTextBox
            // 
            this.custMidToneCentreTextBox.Location = new System.Drawing.Point(163, 173);
            this.custMidToneCentreTextBox.Name = "custMidToneCentreTextBox";
            this.custMidToneCentreTextBox.Size = new System.Drawing.Size(30, 20);
            this.custMidToneCentreTextBox.TabIndex = 22;
            // 
            // defaultMidiMapRadioButton
            // 
            this.defaultMidiMapRadioButton.AutoSize = true;
            this.defaultMidiMapRadioButton.Location = new System.Drawing.Point(12, 56);
            this.defaultMidiMapRadioButton.Name = "defaultMidiMapRadioButton";
            this.defaultMidiMapRadioButton.Size = new System.Drawing.Size(160, 17);
            this.defaultMidiMapRadioButton.TabIndex = 16;
            this.defaultMidiMapRadioButton.TabStop = true;
            this.defaultMidiMapRadioButton.Text = "Default MIDI mapping C3-C7";
            this.infoToolTip.SetToolTip(this.defaultMidiMapRadioButton, "If one sound selected, this sound is used on all keys. If multiple sounds selecte" +
        "d, it assumes the sounds have been imported from sfz.");
            this.defaultMidiMapRadioButton.UseVisualStyleBackColor = true;
            // 
            // midiChTextBox
            // 
            this.midiChTextBox.Location = new System.Drawing.Point(139, 14);
            this.midiChTextBox.Name = "midiChTextBox";
            this.midiChTextBox.ReadOnly = true;
            this.midiChTextBox.Size = new System.Drawing.Size(41, 20);
            this.midiChTextBox.TabIndex = 13;
            // 
            // custMidiToneToTextBox
            // 
            this.custMidiToneToTextBox.Location = new System.Drawing.Point(116, 173);
            this.custMidiToneToTextBox.Name = "custMidiToneToTextBox";
            this.custMidiToneToTextBox.Size = new System.Drawing.Size(30, 20);
            this.custMidiToneToTextBox.TabIndex = 21;
            this.custMidiToneToTextBox.TextChanged += new System.EventHandler(this.custMidiToneToTextBox_TextChanged);
            // 
            // custMidiToneLabel
            // 
            this.custMidiToneLabel.AutoSize = true;
            this.custMidiToneLabel.Location = new System.Drawing.Point(99, 178);
            this.custMidiToneLabel.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.custMidiToneLabel.Name = "custMidiToneLabel";
            this.custMidiToneLabel.Size = new System.Drawing.Size(16, 13);
            this.custMidiToneLabel.TabIndex = 11;
            this.custMidiToneLabel.Text = "to";
            // 
            // midiChTrackBar
            // 
            this.midiChTrackBar.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.midiChTrackBar.Location = new System.Drawing.Point(4, 14);
            this.midiChTrackBar.Margin = new System.Windows.Forms.Padding(2, 1, 2, 1);
            this.midiChTrackBar.Maximum = 16;
            this.midiChTrackBar.Minimum = 1;
            this.midiChTrackBar.Name = "midiChTrackBar";
            this.midiChTrackBar.Size = new System.Drawing.Size(129, 45);
            this.midiChTrackBar.TabIndex = 5;
            this.midiChTrackBar.Value = 1;
            this.midiChTrackBar.Scroll += new System.EventHandler(this.midiChTrackBar_Scroll);
            // 
            // custMidiToneFromTextBox
            // 
            this.custMidiToneFromTextBox.Location = new System.Drawing.Point(70, 173);
            this.custMidiToneFromTextBox.Name = "custMidiToneFromTextBox";
            this.custMidiToneFromTextBox.Size = new System.Drawing.Size(30, 20);
            this.custMidiToneFromTextBox.TabIndex = 20;
            this.custMidiToneFromTextBox.TextChanged += new System.EventHandler(this.custMidiToneFromTextBox_TextChanged);
            // 
            // CustomSampleRadioButton
            // 
            this.CustomSampleRadioButton.AutoSize = true;
            this.CustomSampleRadioButton.Location = new System.Drawing.Point(12, 176);
            this.CustomSampleRadioButton.Name = "CustomSampleRadioButton";
            this.CustomSampleRadioButton.Size = new System.Drawing.Size(60, 17);
            this.CustomSampleRadioButton.TabIndex = 19;
            this.CustomSampleRadioButton.TabStop = true;
            this.CustomSampleRadioButton.Text = "Custom";
            this.CustomSampleRadioButton.UseVisualStyleBackColor = true;
            // 
            // midiToneLabel
            // 
            this.midiToneLabel.AutoSize = true;
            this.midiToneLabel.Location = new System.Drawing.Point(26, 146);
            this.midiToneLabel.Name = "midiToneLabel";
            this.midiToneLabel.Size = new System.Drawing.Size(82, 13);
            this.midiToneLabel.TabIndex = 8;
            this.midiToneLabel.Text = "Start MIDI-note:";
            this.infoToolTip.SetToolTip(this.midiToneLabel, "Enter the MIDI note as a number or string. Note convention is C-2=0, C3=60, G8=12" +
        "7.");
            // 
            // midiToneTextBox
            // 
            this.midiToneTextBox.Location = new System.Drawing.Point(114, 143);
            this.midiToneTextBox.Name = "midiToneTextBox";
            this.midiToneTextBox.Size = new System.Drawing.Size(45, 20);
            this.midiToneTextBox.TabIndex = 8;
            // 
            // MultiSampleRadioButton
            // 
            this.MultiSampleRadioButton.AutoSize = true;
            this.MultiSampleRadioButton.Location = new System.Drawing.Point(12, 126);
            this.MultiSampleRadioButton.Name = "MultiSampleRadioButton";
            this.MultiSampleRadioButton.Size = new System.Drawing.Size(150, 17);
            this.MultiSampleRadioButton.TabIndex = 18;
            this.MultiSampleRadioButton.TabStop = true;
            this.MultiSampleRadioButton.Text = "Multisample (1 snd/1 note)";
            this.MultiSampleRadioButton.UseVisualStyleBackColor = true;
            // 
            // GmPercMidiMappingRadioButton
            // 
            this.GmPercMidiMappingRadioButton.AutoSize = true;
            this.GmPercMidiMappingRadioButton.Location = new System.Drawing.Point(12, 79);
            this.GmPercMidiMappingRadioButton.Name = "GmPercMidiMappingRadioButton";
            this.GmPercMidiMappingRadioButton.Size = new System.Drawing.Size(143, 17);
            this.GmPercMidiMappingRadioButton.TabIndex = 17;
            this.GmPercMidiMappingRadioButton.TabStop = true;
            this.GmPercMidiMappingRadioButton.Text = "GM Percussion mapping:";
            this.GmPercMidiMappingRadioButton.UseVisualStyleBackColor = true;
            this.GmPercMidiMappingRadioButton.CheckedChanged += new System.EventHandler(this.GmPercMidiMappingRadioButton_CheckedChanged);
            // 
            // drumsComboBox1
            // 
            this.drumsComboBox1.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.drumsComboBox1.FormattingEnabled = true;
            this.drumsComboBox1.Location = new System.Drawing.Point(18, 101);
            this.drumsComboBox1.Margin = new System.Windows.Forms.Padding(2, 1, 2, 1);
            this.drumsComboBox1.Name = "drumsComboBox1";
            this.drumsComboBox1.Size = new System.Drawing.Size(164, 21);
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
            this.totalSizeProgressBar.Margin = new System.Windows.Forms.Padding(2, 1, 2, 1);
            this.totalSizeProgressBar.Maximum = 14000;
            this.totalSizeProgressBar.Name = "totalSizeProgressBar";
            this.totalSizeProgressBar.Size = new System.Drawing.Size(147, 20);
            this.totalSizeProgressBar.TabIndex = 7;
            this.totalSizeProgressBar.Value = 100;
            // 
            // totalSizeTextBox
            // 
            this.totalSizeTextBox.Location = new System.Drawing.Point(5, 55);
            this.totalSizeTextBox.Margin = new System.Windows.Forms.Padding(2, 1, 2, 1);
            this.totalSizeTextBox.Name = "totalSizeTextBox";
            this.totalSizeTextBox.Size = new System.Drawing.Size(147, 20);
            this.totalSizeTextBox.TabIndex = 6;
            this.totalSizeTextBox.Text = "Total Size in Bytes";
            // 
            // spiNameTextBox
            // 
            this.spiNameTextBox.Location = new System.Drawing.Point(64, 13);
            this.spiNameTextBox.Margin = new System.Windows.Forms.Padding(2, 1, 2, 1);
            this.spiNameTextBox.MaxLength = 8;
            this.spiNameTextBox.Name = "spiNameTextBox";
            this.spiNameTextBox.Size = new System.Drawing.Size(174, 20);
            this.spiNameTextBox.TabIndex = 5;
            this.spiNameTextBox.Text = "EPSSEDIT";
            this.spiNameTextBox.TextChanged += new System.EventHandler(this.spiNameTextBox_TextChanged);
            // 
            // groupBox3
            // 
            this.groupBox3.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.groupBox3.Controls.Add(this.playButton);
            this.groupBox3.Controls.Add(this.loadSoundButton);
            this.groupBox3.Controls.Add(this.label13);
            this.groupBox3.Controls.Add(this.deleteSoundButton);
            this.groupBox3.Controls.Add(this.groupBox5);
            this.groupBox3.Controls.Add(this.soundListBox);
            this.groupBox3.Location = new System.Drawing.Point(11, 27);
            this.groupBox3.Margin = new System.Windows.Forms.Padding(2, 1, 2, 1);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Padding = new System.Windows.Forms.Padding(2, 1, 2, 1);
            this.groupBox3.Size = new System.Drawing.Size(229, 582);
            this.groupBox3.TabIndex = 6;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Input";
            // 
            // playButton
            // 
            this.playButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.playButton.Location = new System.Drawing.Point(122, 429);
            this.playButton.Name = "playButton";
            this.playButton.Size = new System.Drawing.Size(46, 23);
            this.playButton.TabIndex = 0;
            this.playButton.Text = "Listen";
            this.playButton.UseVisualStyleBackColor = true;
            this.playButton.MouseDown += new System.Windows.Forms.MouseEventHandler(this.playButton_MouseDown);
            this.playButton.MouseUp += new System.Windows.Forms.MouseEventHandler(this.playButton_MouseUp);
            // 
            // loadSoundButton
            // 
            this.loadSoundButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.loadSoundButton.Location = new System.Drawing.Point(67, 429);
            this.loadSoundButton.Name = "loadSoundButton";
            this.loadSoundButton.Size = new System.Drawing.Size(49, 23);
            this.loadSoundButton.TabIndex = 7;
            this.loadSoundButton.Text = "Load...";
            this.infoToolTip.SetToolTip(this.loadSoundButton, "You can drag sounds to the  box to add them faster!");
            this.loadSoundButton.UseVisualStyleBackColor = true;
            this.loadSoundButton.Click += new System.EventHandler(this.loadSoundButton_Click);
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Location = new System.Drawing.Point(10, 14);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(46, 13);
            this.label13.TabIndex = 6;
            this.label13.Text = "Sounds:";
            // 
            // deleteSoundButton
            // 
            this.deleteSoundButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.deleteSoundButton.Location = new System.Drawing.Point(174, 429);
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
            this.groupBox5.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox5.Controls.Add(this.label16);
            this.groupBox5.Controls.Add(this.label15);
            this.groupBox5.Controls.Add(this.label8);
            this.groupBox5.Controls.Add(this.label5);
            this.groupBox5.Controls.Add(this.channelsTextBox);
            this.groupBox5.Controls.Add(this.bitsTextBox);
            this.groupBox5.Controls.Add(this.sizeTextBox);
            this.groupBox5.Controls.Add(this.freqTextBox);
            this.groupBox5.Location = new System.Drawing.Point(5, 454);
            this.groupBox5.Margin = new System.Windows.Forms.Padding(2, 1, 2, 1);
            this.groupBox5.Name = "groupBox5";
            this.groupBox5.Padding = new System.Windows.Forms.Padding(2, 1, 2, 1);
            this.groupBox5.Size = new System.Drawing.Size(220, 124);
            this.groupBox5.TabIndex = 3;
            this.groupBox5.TabStop = false;
            this.groupBox5.Text = "Sample Info";
            // 
            // label16
            // 
            this.label16.AutoSize = true;
            this.label16.Location = new System.Drawing.Point(8, 94);
            this.label16.Name = "label16";
            this.label16.Size = new System.Drawing.Size(54, 13);
            this.label16.TabIndex = 7;
            this.label16.Text = "Channels:";
            // 
            // label15
            // 
            this.label15.AutoSize = true;
            this.label15.Location = new System.Drawing.Point(8, 69);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(27, 13);
            this.label15.TabIndex = 6;
            this.label15.Text = "Bits:";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(6, 44);
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
            this.channelsTextBox.Location = new System.Drawing.Point(148, 91);
            this.channelsTextBox.Name = "channelsTextBox";
            this.channelsTextBox.Size = new System.Drawing.Size(67, 20);
            this.channelsTextBox.TabIndex = 3;
            // 
            // bitsTextBox
            // 
            this.bitsTextBox.Location = new System.Drawing.Point(148, 65);
            this.bitsTextBox.Name = "bitsTextBox";
            this.bitsTextBox.Size = new System.Drawing.Size(68, 20);
            this.bitsTextBox.TabIndex = 2;
            // 
            // sizeTextBox
            // 
            this.sizeTextBox.Location = new System.Drawing.Point(148, 17);
            this.sizeTextBox.Margin = new System.Windows.Forms.Padding(2, 1, 2, 1);
            this.sizeTextBox.Name = "sizeTextBox";
            this.sizeTextBox.Size = new System.Drawing.Size(68, 20);
            this.sizeTextBox.TabIndex = 1;
            this.sizeTextBox.Text = "Size kb";
            // 
            // freqTextBox
            // 
            this.freqTextBox.Location = new System.Drawing.Point(148, 40);
            this.freqTextBox.Margin = new System.Windows.Forms.Padding(2, 1, 2, 1);
            this.freqTextBox.Name = "freqTextBox";
            this.freqTextBox.Size = new System.Drawing.Size(68, 20);
            this.freqTextBox.TabIndex = 0;
            this.freqTextBox.Text = "Freq 48kHz";
            // 
            // compressionTypeTextBox
            // 
            this.compressionTypeTextBox.Controls.Add(this.groupBox1);
            this.compressionTypeTextBox.Controls.Add(this.conversionTextBox);
            this.compressionTypeTextBox.Controls.Add(this.label1);
            this.compressionTypeTextBox.Controls.Add(this.groupBox6);
            this.compressionTypeTextBox.Controls.Add(this.groupBox9);
            this.compressionTypeTextBox.Controls.Add(this.groupBox7);
            this.compressionTypeTextBox.Location = new System.Drawing.Point(244, 27);
            this.compressionTypeTextBox.Margin = new System.Windows.Forms.Padding(2, 1, 2, 1);
            this.compressionTypeTextBox.Name = "compressionTypeTextBox";
            this.compressionTypeTextBox.Padding = new System.Windows.Forms.Padding(2, 1, 2, 1);
            this.compressionTypeTextBox.Size = new System.Drawing.Size(226, 583);
            this.compressionTypeTextBox.TabIndex = 7;
            this.compressionTypeTextBox.TabStop = false;
            this.compressionTypeTextBox.Text = "Conversion parameters";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.normalizeTextBox);
            this.groupBox1.Controls.Add(this.normalizeCheckBox);
            this.groupBox1.Controls.Add(this.normalizeTrackBar);
            this.groupBox1.Location = new System.Drawing.Point(5, 216);
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
            this.normalizeTrackBar.Location = new System.Drawing.Point(5, 27);
            this.normalizeTrackBar.Margin = new System.Windows.Forms.Padding(2, 1, 2, 1);
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
            this.conversionTextBox.Location = new System.Drawing.Point(4, 30);
            this.conversionTextBox.Margin = new System.Windows.Forms.Padding(2, 1, 2, 1);
            this.conversionTextBox.Multiline = true;
            this.conversionTextBox.Name = "conversionTextBox";
            this.conversionTextBox.ReadOnly = true;
            this.conversionTextBox.Size = new System.Drawing.Size(212, 40);
            this.conversionTextBox.TabIndex = 14;
            this.conversionTextBox.Text = "Size kb before";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(5, 14);
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
            this.groupBox6.Location = new System.Drawing.Point(5, 153);
            this.groupBox6.Name = "groupBox6";
            this.groupBox6.Size = new System.Drawing.Size(211, 57);
            this.groupBox6.TabIndex = 9;
            this.groupBox6.TabStop = false;
            this.groupBox6.Text = "Sound size after compression:";
            // 
            // showCompProgressBar
            // 
            this.showCompProgressBar.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.showCompProgressBar.Location = new System.Drawing.Point(4, 17);
            this.showCompProgressBar.Margin = new System.Windows.Forms.Padding(2, 1, 2, 1);
            this.showCompProgressBar.Maximum = 200;
            this.showCompProgressBar.Name = "showCompProgressBar";
            this.showCompProgressBar.Size = new System.Drawing.Size(138, 17);
            this.showCompProgressBar.TabIndex = 4;
            this.showCompProgressBar.Value = 50;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(5, 38);
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
            this.label7.Location = new System.Drawing.Point(121, 38);
            this.label7.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(33, 13);
            this.label7.TabIndex = 7;
            this.label7.Text = "200%";
            // 
            // soundSizeAfterTextBox
            // 
            this.soundSizeAfterTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.soundSizeAfterTextBox.Location = new System.Drawing.Point(148, 18);
            this.soundSizeAfterTextBox.Margin = new System.Windows.Forms.Padding(2, 1, 2, 1);
            this.soundSizeAfterTextBox.Name = "soundSizeAfterTextBox";
            this.soundSizeAfterTextBox.Size = new System.Drawing.Size(59, 20);
            this.soundSizeAfterTextBox.TabIndex = 3;
            // 
            // groupBox7
            // 
            this.groupBox7.Controls.Add(this.label20);
            this.groupBox7.Controls.Add(this.label19);
            this.groupBox7.Controls.Add(this.label18);
            this.groupBox7.Controls.Add(this.label17);
            this.groupBox7.Controls.Add(this.compressionTrackBar);
            this.groupBox7.Location = new System.Drawing.Point(5, 75);
            this.groupBox7.Name = "groupBox7";
            this.groupBox7.Size = new System.Drawing.Size(212, 72);
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
            this.compressionTrackBar.Margin = new System.Windows.Forms.Padding(2, 1, 2, 1);
            this.compressionTrackBar.Maximum = 3;
            this.compressionTrackBar.Name = "compressionTrackBar";
            this.compressionTrackBar.Size = new System.Drawing.Size(202, 45);
            this.compressionTrackBar.TabIndex = 0;
            this.compressionTrackBar.Scroll += new System.EventHandler(this.compressionTrackBar_Scroll);
            // 
            // useInSpiButton
            // 
            this.useInSpiButton.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.useInSpiButton.Enabled = false;
            this.useInSpiButton.Location = new System.Drawing.Point(477, 46);
            this.useInSpiButton.Name = "useInSpiButton";
            this.useInSpiButton.Size = new System.Drawing.Size(24, 402);
            this.useInSpiButton.TabIndex = 11;
            this.useInSpiButton.Text = "A\r\nD\r\nD\r\n\r\n->\r\n->\r\n->\r\n\r\nS\r\nO\r\nU\r\nN\r\nD";
            this.useInSpiButton.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.useInSpiButton.UseVisualStyleBackColor = true;
            this.useInSpiButton.Click += new System.EventHandler(this.useInSpiButton_Click);
            // 
            // menuStrip1
            // 
            this.menuStrip1.ImageScalingSize = new System.Drawing.Size(24, 24);
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.helpToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Padding = new System.Windows.Forms.Padding(4, 1, 0, 1);
            this.menuStrip1.Size = new System.Drawing.Size(1078, 24);
            this.menuStrip1.TabIndex = 8;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.newProjectToolStripMenuItem,
            this.loadProjectToolStripMenuItem,
            this.saveProjectToolStripMenuItem,
            this.toolStripSeparator1,
            this.loadSPIToolStripMenuItem,
            this.importSFZToolStripMenuItem,
            this.saveSPIToolStripMenuItem,
            this.saveSFZToolStripMenuItem,
            this.toolStripSeparator2,
            this.clearSettingsToolStripMenuItem,
            this.exitToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 22);
            this.fileToolStripMenuItem.Text = "File";
            // 
            // newProjectToolStripMenuItem
            // 
            this.newProjectToolStripMenuItem.Name = "newProjectToolStripMenuItem";
            this.newProjectToolStripMenuItem.Size = new System.Drawing.Size(154, 22);
            this.newProjectToolStripMenuItem.Text = "New Project...";
            this.newProjectToolStripMenuItem.Click += new System.EventHandler(this.newProjectToolStripMenuItem_Click_1);
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
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(151, 6);
            // 
            // loadSPIToolStripMenuItem
            // 
            this.loadSPIToolStripMenuItem.Name = "loadSPIToolStripMenuItem";
            this.loadSPIToolStripMenuItem.Size = new System.Drawing.Size(154, 22);
            this.loadSPIToolStripMenuItem.Text = "Import SPI...";
            this.loadSPIToolStripMenuItem.Click += new System.EventHandler(this.loadSPIToolStripMenuItem_Click);
            // 
            // importSFZToolStripMenuItem
            // 
            this.importSFZToolStripMenuItem.Name = "importSFZToolStripMenuItem";
            this.importSFZToolStripMenuItem.Size = new System.Drawing.Size(154, 22);
            this.importSFZToolStripMenuItem.Text = "Import SFZ...";
            this.importSFZToolStripMenuItem.Click += new System.EventHandler(this.importSFZToolStripMenuItem_Click);
            // 
            // saveSPIToolStripMenuItem
            // 
            this.saveSPIToolStripMenuItem.Name = "saveSPIToolStripMenuItem";
            this.saveSPIToolStripMenuItem.Size = new System.Drawing.Size(154, 22);
            this.saveSPIToolStripMenuItem.Text = "Export SPI...";
            this.saveSPIToolStripMenuItem.Click += new System.EventHandler(this.saveSPIToolStripMenuItem_Click);
            // 
            // saveSFZToolStripMenuItem
            // 
            this.saveSFZToolStripMenuItem.Name = "saveSFZToolStripMenuItem";
            this.saveSFZToolStripMenuItem.Size = new System.Drawing.Size(154, 22);
            this.saveSFZToolStripMenuItem.Text = "Export SFZ...";
            this.saveSFZToolStripMenuItem.Click += new System.EventHandler(this.saveSFZToolStripMenuItem_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(151, 6);
            // 
            // clearSettingsToolStripMenuItem
            // 
            this.clearSettingsToolStripMenuItem.Name = "clearSettingsToolStripMenuItem";
            this.clearSettingsToolStripMenuItem.Size = new System.Drawing.Size(154, 22);
            this.clearSettingsToolStripMenuItem.Text = "Clear settings...";
            this.clearSettingsToolStripMenuItem.Click += new System.EventHandler(this.clearSettingsToolStripMenuItem_Click);
            // 
            // exitToolStripMenuItem
            // 
            this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            this.exitToolStripMenuItem.Size = new System.Drawing.Size(154, 22);
            this.exitToolStripMenuItem.Text = "Exit";
            this.exitToolStripMenuItem.Click += new System.EventHandler(this.exitToolStripMenuItem_Click);
            // 
            // helpToolStripMenuItem
            // 
            this.helpToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.checkForUpdatesToolStripMenuItem});
            this.helpToolStripMenuItem.Name = "helpToolStripMenuItem";
            this.helpToolStripMenuItem.Size = new System.Drawing.Size(44, 22);
            this.helpToolStripMenuItem.Text = "Help";
            // 
            // checkForUpdatesToolStripMenuItem
            // 
            this.checkForUpdatesToolStripMenuItem.Name = "checkForUpdatesToolStripMenuItem";
            this.checkForUpdatesToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.checkForUpdatesToolStripMenuItem.Text = "Check for Updates...";
            this.checkForUpdatesToolStripMenuItem.Click += new System.EventHandler(this.checkForUpdatesToolStripMenuItem_Click);
            // 
            // loadSoundFileDialog
            // 
            this.loadSoundFileDialog.FileName = "openFileDialog1";
            this.loadSoundFileDialog.Filter = "wav files (*.wav)|*.wav|All files (*.*)|*.*";
            this.loadSoundFileDialog.Multiselect = true;
            this.loadSoundFileDialog.Title = "Load Sound...";
            // 
            // omniPatchCheckBox
            // 
            this.omniPatchCheckBox.AutoSize = true;
            this.omniPatchCheckBox.Location = new System.Drawing.Point(9, 58);
            this.omniPatchCheckBox.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.omniPatchCheckBox.Name = "omniPatchCheckBox";
            this.omniPatchCheckBox.Size = new System.Drawing.Size(80, 17);
            this.omniPatchCheckBox.TabIndex = 19;
            this.omniPatchCheckBox.Text = "Omni patch";
            this.infoToolTip.SetToolTip(this.omniPatchCheckBox, "Create the patch with the same sound on all MIDI channels in the patch.");
            this.omniPatchCheckBox.UseVisualStyleBackColor = true;
            this.omniPatchCheckBox.CheckedChanged += new System.EventHandler(this.omniPatchCheckBox_CheckedChanged);
            // 
            // gen2CheckBox
            // 
            this.gen2CheckBox.AutoSize = true;
            this.gen2CheckBox.Enabled = false;
            this.gen2CheckBox.Location = new System.Drawing.Point(162, 58);
            this.gen2CheckBox.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.gen2CheckBox.Name = "gen2CheckBox";
            this.gen2CheckBox.Size = new System.Drawing.Size(72, 17);
            this.gen2CheckBox.TabIndex = 20;
            this.gen2CheckBox.Text = "SPI Gen2";
            this.infoToolTip.SetToolTip(this.gen2CheckBox, "Only supported in EPSS Driver 3.7 and newer! Create a patch that contains both MI" +
        "DI Channel mapping and program change mappings.");
            this.gen2CheckBox.UseVisualStyleBackColor = true;
            this.gen2CheckBox.CheckedChanged += new System.EventHandler(this.gen2CheckBox_CheckedChanged);
            // 
            // loadMidButton
            // 
            this.loadMidButton.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.loadMidButton.Location = new System.Drawing.Point(5, 19);
            this.loadMidButton.Name = "loadMidButton";
            this.loadMidButton.Size = new System.Drawing.Size(145, 22);
            this.loadMidButton.TabIndex = 8;
            this.loadMidButton.Text = "Load and Play MID...";
            this.infoToolTip.SetToolTip(this.loadMidButton, "You can drag sounds to the  box to add them faster!");
            this.loadMidButton.UseVisualStyleBackColor = true;
            this.loadMidButton.Click += new System.EventHandler(this.loadMidButton_Click);
            // 
            // stopMidButton
            // 
            this.stopMidButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.stopMidButton.Location = new System.Drawing.Point(39, 71);
            this.stopMidButton.Name = "stopMidButton";
            this.stopMidButton.Size = new System.Drawing.Size(37, 22);
            this.stopMidButton.TabIndex = 9;
            this.stopMidButton.Text = "Stop";
            this.infoToolTip.SetToolTip(this.stopMidButton, "You can drag sounds to the  box to add them faster!");
            this.stopMidButton.UseVisualStyleBackColor = true;
            this.stopMidButton.Click += new System.EventHandler(this.stopMidButton_Click);
            this.stopMidButton.KeyDown += new System.Windows.Forms.KeyEventHandler(this.stopMidButton_KeyDown);
            // 
            // playMidButton
            // 
            this.playMidButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.playMidButton.Location = new System.Drawing.Point(82, 71);
            this.playMidButton.Name = "playMidButton";
            this.playMidButton.Size = new System.Drawing.Size(35, 22);
            this.playMidButton.TabIndex = 25;
            this.playMidButton.Text = "Play";
            this.infoToolTip.SetToolTip(this.playMidButton, "You can drag sounds to the  box to add them faster!");
            this.playMidButton.UseVisualStyleBackColor = true;
            this.playMidButton.Click += new System.EventHandler(this.playMidButton_Click);
            this.playMidButton.KeyDown += new System.Windows.Forms.KeyEventHandler(this.playMidButton_KeyDown);
            // 
            // revMidButton
            // 
            this.revMidButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.revMidButton.Location = new System.Drawing.Point(6, 71);
            this.revMidButton.Name = "revMidButton";
            this.revMidButton.Size = new System.Drawing.Size(27, 22);
            this.revMidButton.TabIndex = 26;
            this.revMidButton.Text = "<<";
            this.infoToolTip.SetToolTip(this.revMidButton, "You can drag sounds to the  box to add them faster!");
            this.revMidButton.UseVisualStyleBackColor = true;
            this.revMidButton.KeyDown += new System.Windows.Forms.KeyEventHandler(this.revMidButton_KeyDown);
            this.revMidButton.KeyUp += new System.Windows.Forms.KeyEventHandler(this.revMidButton_KeyUp);
            this.revMidButton.MouseDown += new System.Windows.Forms.MouseEventHandler(this.revMidButton_MouseDown);
            this.revMidButton.MouseUp += new System.Windows.Forms.MouseEventHandler(this.revMidButton_MouseUp);
            // 
            // ffwMidButton
            // 
            this.ffwMidButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.ffwMidButton.Location = new System.Drawing.Point(123, 71);
            this.ffwMidButton.Name = "ffwMidButton";
            this.ffwMidButton.Size = new System.Drawing.Size(27, 22);
            this.ffwMidButton.TabIndex = 27;
            this.ffwMidButton.Text = ">>";
            this.infoToolTip.SetToolTip(this.ffwMidButton, "You can drag sounds to the  box to add them faster!");
            this.ffwMidButton.UseVisualStyleBackColor = true;
            this.ffwMidButton.KeyDown += new System.Windows.Forms.KeyEventHandler(this.ffwMidButton_KeyDown);
            this.ffwMidButton.KeyUp += new System.Windows.Forms.KeyEventHandler(this.ffwMidButton_KeyUp);
            this.ffwMidButton.MouseDown += new System.Windows.Forms.MouseEventHandler(this.ffwMidButton_MouseDown);
            this.ffwMidButton.MouseUp += new System.Windows.Forms.MouseEventHandler(this.ffwMidButton_MouseUp);
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.label4);
            this.groupBox4.Controls.Add(this.label3);
            this.groupBox4.Controls.Add(this.label2);
            this.groupBox4.Controls.Add(this.totalSizeProgressBar);
            this.groupBox4.Controls.Add(this.totalSizeTextBox);
            this.groupBox4.Location = new System.Drawing.Point(6, 14);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(167, 96);
            this.groupBox4.TabIndex = 15;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "Size";
            // 
            // groupBox8
            // 
            this.groupBox8.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox8.Controls.Add(this.groupBox10);
            this.groupBox8.Controls.Add(this.groupBox4);
            this.groupBox8.Location = new System.Drawing.Point(631, 492);
            this.groupBox8.Name = "groupBox8";
            this.groupBox8.Size = new System.Drawing.Size(431, 117);
            this.groupBox8.TabIndex = 11;
            this.groupBox8.TabStop = false;
            this.groupBox8.Text = "EPSS SPI - Sound Patch Information";
            // 
            // groupBox10
            // 
            this.groupBox10.Controls.Add(this.gen2CheckBox);
            this.groupBox10.Controls.Add(this.omniPatchCheckBox);
            this.groupBox10.Controls.Add(this.label22);
            this.groupBox10.Controls.Add(this.label23);
            this.groupBox10.Controls.Add(this.spiInfoTextBox);
            this.groupBox10.Controls.Add(this.spiNameTextBox);
            this.groupBox10.Location = new System.Drawing.Point(179, 14);
            this.groupBox10.Name = "groupBox10";
            this.groupBox10.Size = new System.Drawing.Size(243, 96);
            this.groupBox10.TabIndex = 19;
            this.groupBox10.TabStop = false;
            this.groupBox10.Text = "Save...";
            // 
            // label22
            // 
            this.label22.AutoSize = true;
            this.label22.Location = new System.Drawing.Point(6, 16);
            this.label22.Name = "label22";
            this.label22.Size = new System.Drawing.Size(58, 13);
            this.label22.TabIndex = 16;
            this.label22.Text = "SPI Name:";
            // 
            // label23
            // 
            this.label23.AutoSize = true;
            this.label23.Location = new System.Drawing.Point(7, 40);
            this.label23.Name = "label23";
            this.label23.Size = new System.Drawing.Size(48, 13);
            this.label23.TabIndex = 17;
            this.label23.Text = "SPI Info:";
            // 
            // spiInfoTextBox
            // 
            this.spiInfoTextBox.Location = new System.Drawing.Point(64, 38);
            this.spiInfoTextBox.Margin = new System.Windows.Forms.Padding(2, 1, 2, 1);
            this.spiInfoTextBox.MaxLength = 16;
            this.spiInfoTextBox.Name = "spiInfoTextBox";
            this.spiInfoTextBox.Size = new System.Drawing.Size(174, 20);
            this.spiInfoTextBox.TabIndex = 18;
            this.spiInfoTextBox.Text = "Created with EPSSEditor";
            this.spiInfoTextBox.TextChanged += new System.EventHandler(this.spiInfoTextBox_TextChanged);
            // 
            // saveSpiFileDialog
            // 
            this.saveSpiFileDialog.Filter = "Spi files (*.spi)|*.spi|All files (*.*)|*.*\"";
            this.saveSpiFileDialog.Title = "Export project to EPSS SPI format";
            // 
            // saveProjectFileDialog
            // 
            this.saveProjectFileDialog.Filter = "EPSS Project files (*.epf)|*.epf|All files (*.*)|*.*";
            this.saveProjectFileDialog.Title = "Save EPSS Project";
            // 
            // loadProjectFileDialog
            // 
            this.loadProjectFileDialog.FileName = "openFileDialog1";
            this.loadProjectFileDialog.Filter = "EPSS Project files (*.epf)|*.epf|All files (*.*)|*.*";
            this.loadProjectFileDialog.Title = "Load EPSS Project";
            // 
            // openDrumMappingsFileDialog
            // 
            this.openDrumMappingsFileDialog.Filter = "XML Drum mappings (*.xml)|*.xml|All files (*.*)|*.*";
            // 
            // loadSpiFileDialog
            // 
            this.loadSpiFileDialog.FileName = "loadSpiFileDialog";
            this.loadSpiFileDialog.Filter = "SPI files (*.spi)|*.spi|All files (*.*)|*.*";
            this.loadSpiFileDialog.Title = "Import EPSS SPI file";
            // 
            // saveSfzFileDialog
            // 
            this.saveSfzFileDialog.Filter = "Sfz files (*.sfz)|*.sfz|All files (*.*)|*.*";
            this.saveSfzFileDialog.Title = "Export project to SFZ format";
            // 
            // loadSfzFileDialog
            // 
            this.loadSfzFileDialog.FileName = "loadSfzFileDialog";
            this.loadSfzFileDialog.Filter = "SFZ files (*.sfz)|*.sfz|All files (*.*)|*.*";
            this.loadSfzFileDialog.Title = "Import SFZ file";
            // 
            // groupBox13
            // 
            this.groupBox13.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox13.Controls.Add(this.ffwMidButton);
            this.groupBox13.Controls.Add(this.revMidButton);
            this.groupBox13.Controls.Add(this.playMidButton);
            this.groupBox13.Controls.Add(this.midFileBarTextBox);
            this.groupBox13.Controls.Add(this.stopMidButton);
            this.groupBox13.Controls.Add(this.loadMidButton);
            this.groupBox13.Location = new System.Drawing.Point(469, 492);
            this.groupBox13.Name = "groupBox13";
            this.groupBox13.Size = new System.Drawing.Size(156, 118);
            this.groupBox13.TabIndex = 20;
            this.groupBox13.TabStop = false;
            this.groupBox13.Text = "MIDI";
            // 
            // midFileBarTextBox
            // 
            this.midFileBarTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.midFileBarTextBox.Font = new System.Drawing.Font("Courier New", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.midFileBarTextBox.Location = new System.Drawing.Point(8, 47);
            this.midFileBarTextBox.Name = "midFileBarTextBox";
            this.midFileBarTextBox.Size = new System.Drawing.Size(142, 20);
            this.midFileBarTextBox.TabIndex = 24;
            this.midFileBarTextBox.Text = "1";
            this.midFileBarTextBox.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // loadMidFileDialog
            // 
            this.loadMidFileDialog.FileName = "loadMidFileDialog";
            this.loadMidFileDialog.Filter = "MID files (*.mid)|*.mid|All files (*.*)|*.*";
            this.loadMidFileDialog.Title = "Import EPSS SPI file";
            // 
            // timer1
            // 
            this.timer1.Interval = 80;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // revMidTimer
            // 
            this.revMidTimer.Interval = 20;
            this.revMidTimer.Tick += new System.EventHandler(this.revMidTimer_Tick);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1078, 611);
            this.Controls.Add(this.groupBox13);
            this.Controls.Add(this.groupBox8);
            this.Controls.Add(this.useInSpiButton);
            this.Controls.Add(this.compressionTypeTextBox);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.menuStrip1);
            this.DataBindings.Add(new System.Windows.Forms.Binding("Location", global::EPSSEditor.Properties.Settings.Default, "WinLocation", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.helpProvider1.SetHelpKeyword(this, "");
            this.helpProvider1.SetHelpString(this, "EPSS Editor");
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.KeyPreview = true;
            this.Location = global::EPSSEditor.Properties.Settings.Default.WinLocation;
            this.MainMenuStrip = this.menuStrip1;
            this.Margin = new System.Windows.Forms.Padding(2, 1, 2, 1);
            this.MinimumSize = new System.Drawing.Size(1094, 650);
            this.Name = "Form1";
            this.helpProvider1.SetShowHelp(this, false);
            this.Text = "EPSS Editor v1.08 - 20210116";
            this.Activated += new System.EventHandler(this.Form1_Activated);
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);
            this.Load += new System.EventHandler(this.Form1_Load);
            this.contextMenuStrip1.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox9.ResumeLayout(false);
            this.groupBox12.ResumeLayout(false);
            this.groupBox12.PerformLayout();
            this.groupBox11.ResumeLayout(false);
            this.groupBox11.PerformLayout();
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
            this.groupBox10.ResumeLayout(false);
            this.groupBox10.PerformLayout();
            this.groupBox13.ResumeLayout(false);
            this.groupBox13.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.ListBox soundListBox;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.TextBox totalSizeTextBox;
        private System.Windows.Forms.TextBox spiNameTextBox;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.GroupBox compressionTypeTextBox;
        private System.Windows.Forms.TrackBar compressionTrackBar;
        private System.Windows.Forms.TextBox soundSizeAfterTextBox;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ProgressBar totalSizeProgressBar;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.ProgressBar showCompProgressBar;
        private System.Windows.Forms.GroupBox groupBox5;
        private System.Windows.Forms.TextBox sizeTextBox;
        private System.Windows.Forms.TextBox freqTextBox;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem loadProjectToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem saveProjectToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
        private System.Windows.Forms.ComboBox drumsComboBox1;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.Button deleteSoundButton;
        private System.Windows.Forms.Button loadSoundButton;
        private System.Windows.Forms.OpenFileDialog loadSoundFileDialog;
        private System.Windows.Forms.Button playButton;
        private System.Windows.Forms.ToolTip infoToolTip;
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
        private System.Windows.Forms.ListView spiSoundListView;
        private System.Windows.Forms.GroupBox groupBox10;
        private System.Windows.Forms.OpenFileDialog openDrumMappingsFileDialog;
        private System.Windows.Forms.Button spiSoundListenButton;
        private System.Windows.Forms.ComboBox previewComboBox;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.SaveFileDialog saveSampleFileDialog;
        private System.Windows.Forms.Label midiToneLabel;
        private System.Windows.Forms.TextBox midiToneTextBox;
        private System.Windows.Forms.RadioButton MultiSampleRadioButton;
        private System.Windows.Forms.CheckBox omniPatchCheckBox;
        private System.Windows.Forms.ToolStripMenuItem newProjectToolStripMenuItem;
        private System.Windows.Forms.TextBox custMidiToneToTextBox;
        private System.Windows.Forms.Label custMidiToneLabel;
        private System.Windows.Forms.TextBox custMidiToneFromTextBox;
        private System.Windows.Forms.RadioButton CustomSampleRadioButton;
        private System.Windows.Forms.GroupBox groupBox12;
        private System.Windows.Forms.RadioButton mappingModeProgramRadioButton;
        private System.Windows.Forms.RadioButton mappingModeMidiRadioButton;
        private System.Windows.Forms.GroupBox groupBox11;
        private System.Windows.Forms.CheckBox gen2CheckBox;
        private System.Windows.Forms.ToolStripMenuItem loadSPIToolStripMenuItem;
        private System.Windows.Forms.OpenFileDialog loadSpiFileDialog;
        private System.Windows.Forms.ToolStripMenuItem saveSFZToolStripMenuItem;
        private System.Windows.Forms.SaveFileDialog saveSfzFileDialog;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripMenuItem saveSPIToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem importSFZToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.OpenFileDialog loadSfzFileDialog;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem1;
        private System.Windows.Forms.HelpProvider helpProvider1;
        private System.Windows.Forms.FolderBrowserDialog folderBrowserDialog1;
        private System.Windows.Forms.GroupBox groupBox13;
        private System.Windows.Forms.Button loadMidButton;
        private System.Windows.Forms.OpenFileDialog loadMidFileDialog;
        private System.Windows.Forms.Button stopMidButton;
        private System.Windows.Forms.Label custMidiToneCtrLabel;
        private System.Windows.Forms.TextBox custMidToneCentreTextBox;
        private System.Windows.Forms.ToolStripMenuItem helpToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem checkForUpdatesToolStripMenuItem;
        private System.Windows.Forms.TextBox midFileBarTextBox;
        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.Button playMidButton;
        private System.Windows.Forms.Button ffwMidButton;
        private System.Windows.Forms.Button revMidButton;
        private System.Windows.Forms.Timer revMidTimer;
    }
}

