using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NLEditor
{
    public partial class NLEditForm : Form
    {
        public NLEditForm()
        {
            InitializeComponent();
        }

        private void newToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Create new level
            // TODO
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Exit editor
            Application.Exit();
        }
    }
}
