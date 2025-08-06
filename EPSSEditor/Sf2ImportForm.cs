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
    public partial class Sf2ImportForm : Form
    {
        public Sf2ImportForm()
        {
            InitializeComponent();
        }

        public TreeView TreeView()
        {
            return sf2ImportTreeView;
        }
    }
}
