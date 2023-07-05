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


        public string GetText()
        {
            return textBox1.Text;
        }

        private void textBox1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                DialogResult = DialogResult.OK;
                Close();
                e.Handled = true;
            }
        }
    }
}
