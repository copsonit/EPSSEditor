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
    public partial class UpdateAvailable : Form
    {
        private Form1 caller;
        private string version;
        private string link;

        public UpdateAvailable()
        {
            InitializeComponent();
        }

        public UpdateAvailable(Form1 caller, string msg, string link, string version, bool inStart) : this()
        {
            this.caller = caller;
            this.version = version;
            this.link = link;

            linkLabel1.Text = msg;
            linkLabel1.Links.Clear();
            linkLabel1.LinkArea = new LinkArea(0, msg.Length);

            if (!inStart)
            {
                button2.Text = "Close";
                button1.Visible = false;
            }

            //linkLabel1.Links.Add(0, msg.Length, link);
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            // Specify that the link was visited.
            linkLabel1.LinkVisited = true;

            // Navigate to a URL.
            System.Diagnostics.Process.Start(link);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            caller.IgnoreThisUpdate(version);
            Close();
        }
    }
}
