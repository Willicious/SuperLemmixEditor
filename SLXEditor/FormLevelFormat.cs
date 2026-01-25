using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SLXEditor
{
    public partial class FormLevelFormat : Form
    {
        public string SelectedExtension { get; private set; } = null;
        public bool ApplyFormatToLevelsNXMI = false;

        public FormLevelFormat(String path)
        {
            InitializeComponent();
            SetTargetFolderLabel(path);
            comboBoxFormats.SelectedIndex = 0;
        }

        private void SetTargetFolderLabel(String path)
        {
            var dir = new DirectoryInfo(path.TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar));
            labelTargetFolder.Text = dir.Name;
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

            ApplyFormatToLevelsNXMI = checkApplyFormatToLevelsNXMI.Checked;
            DialogResult = DialogResult.OK;
            Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }

        private void comboBoxFormats_SelectedIndexChanged(object sender, EventArgs e)
        {
            labelNXLVWarning.Visible = comboBoxFormats.SelectedIndex == 2;
        }
    }
}
