using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;

namespace NLEditor
{
    public partial class FormWelcomeScreen : Form
    {
        public FormWelcomeScreen()
        {
            Int32 topMargin = 10;

            InitializeComponent();

            lblWhatsNew.Text = "What's New in " + C.Version;
            lblWhatsNew.Left = (this.ClientSize.Width - lblWhatsNew.Width) / 2;
            lblWhatsNew.Top = topMargin;

            check_ShowWelcomeScreen.Left = (this.ClientSize.Width - check_ShowWelcomeScreen.Width) / 2;
            check_ShowWelcomeScreen.Checked = Properties.Settings.Default.ShowWelcomeScreen;
            check_ShowWelcomeScreen.CheckedChanged += Check_ShowWelcomeScreen_CheckedChanged;
        }

        private void Check_ShowWelcomeScreen_CheckedChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.ShowWelcomeScreen = check_ShowWelcomeScreen.Checked;
            Properties.Settings.Default.Save();
        }
    }
}
