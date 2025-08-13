namespace EPSSEditor
{
    partial class Sf2ImportForm
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
            this.sf2ImportTreeView = new System.Windows.Forms.TreeView();
            this.sf2ImportContinueButton = new System.Windows.Forms.Button();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // sf2ImportTreeView
            // 
            this.sf2ImportTreeView.Location = new System.Drawing.Point(13, 54);
            this.sf2ImportTreeView.Name = "sf2ImportTreeView";
            this.sf2ImportTreeView.Size = new System.Drawing.Size(380, 369);
            this.sf2ImportTreeView.TabIndex = 0;
            // 
            // sf2ImportContinueButton
            // 
            this.sf2ImportContinueButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.sf2ImportContinueButton.Location = new System.Drawing.Point(318, 429);
            this.sf2ImportContinueButton.Name = "sf2ImportContinueButton";
            this.sf2ImportContinueButton.Size = new System.Drawing.Size(75, 23);
            this.sf2ImportContinueButton.TabIndex = 1;
            this.sf2ImportContinueButton.Text = "Continue import";
            this.sf2ImportContinueButton.UseVisualStyleBackColor = true;
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(13, 13);
            this.textBox1.Multiline = true;
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(380, 35);
            this.textBox1.TabIndex = 2;
            this.textBox1.Text = "More than one bank of sounds found. Please select which bank you want to import i" +
    "n the window below:";
            // 
            // Sf2ImportForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(402, 463);
            this.Controls.Add(this.textBox1);
            this.Controls.Add(this.sf2ImportContinueButton);
            this.Controls.Add(this.sf2ImportTreeView);
            this.Name = "Sf2ImportForm";
            this.Text = "SF2 Import Settings";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TreeView sf2ImportTreeView;
        private System.Windows.Forms.Button sf2ImportContinueButton;
        private System.Windows.Forms.TextBox textBox1;
    }
}