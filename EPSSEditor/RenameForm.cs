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
    public partial class RenameForm : Form
    {
        //public string text;

        public RenameForm(String text)
        {
            InitializeComponent();
            textBox1.Text = text;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            
            Close();
        }

        public string GetText()
        {
            return textBox1.Text;
        }
    }
}
