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
    public partial class ProgressForm : Form
    {
        public ProgressForm(string label)
        {
            InitializeComponent();
            Text = label;
        }

        public void SetProgress(int n, string status)
        {
            progressBar1.Value = n;
            if (status != null)
            {
                statusLabel.Text = status;
            }
            Application.DoEvents();
        }
    }
}
