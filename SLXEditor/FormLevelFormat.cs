using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SLXEditor
{
    public partial class FormLevelFormat : Form
    {
        public string SelectedExtension { get; private set; } = null;

        public FormLevelFormat()
        {
            InitializeComponent();
            comboBoxFormats.SelectedIndex = 0;
        }

        private void btnCleanseLevels_Click(object sender, EventArgs e)
        {
            switch (comboBoxFormats.SelectedIndex)
            {
                case 1:
                    SelectedExtension = ".sxlv";
                    break;
                case 2:
                    SelectedExtension = ".nxlv";
                    break;
                default:
                    SelectedExtension = null; // keep original
                    break;
            }

            DialogResult = DialogResult.OK;
            Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }
    }
}
