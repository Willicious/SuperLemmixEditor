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
    public partial class FormWhatsNew : Form
    {
        public FormWhatsNew()
        {
            Int32 topMargin = 10;

            InitializeComponent();

            lblWhatsNew.Text = "What's New in " + C.Version;
            lblWhatsNew.Left = (this.ClientSize.Width - lblWhatsNew.Width) / 2;
            lblWhatsNew.Top = topMargin;
            WriteWhatsNewText();

            lblPreviousUpdates.Left = (this.ClientSize.Width - lblPreviousUpdates.Width) / 2;
            WritePreviousUpdatesText();

            check_ShowWhatsNew.Left = (this.ClientSize.Width - check_ShowWhatsNew.Width) / 2;
            check_ShowWhatsNew.Checked = Properties.Settings.Default.ShowWhatsNew;
            check_ShowWhatsNew.CheckedChanged += Check_ShowWhatsNew_CheckedChanged;
        }

        private void Check_ShowWhatsNew_CheckedChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.ShowWhatsNew = check_ShowWhatsNew.Checked;
            Properties.Settings.Default.Save();
        }

        /// <summary>
        /// Helper function to write bold text
        /// </summary>
        private void WriteBoldText(RichTextBox richTextBox, string text)
        {
            var regularFont = richTextBox.Font;

            richTextBox.SelectionFont = new Font(richTextBox.Font, FontStyle.Bold);
            richTextBox.AppendText(text);
            richTextBox.SelectionFont = new Font(regularFont, FontStyle.Regular);
        }

        /// <summary>
        /// Populates "What's New" field with text
        /// </summary>
        private void WriteWhatsNewText()
        {
            var richTextBox = richTextBox_WhatsNew;
            richTextBox.Clear();

            WriteBoldText(richTextBox, "• NeoLemmix Mode");
            richTextBox.AppendText(" - Activates NeoLemmix-specific controls and features\n");

            WriteBoldText(richTextBox, "• Bugfix");
            richTextBox.AppendText(" - Cursor anchor is now correctly preserved when zooming in and out\n");

            WriteBoldText(richTextBox, "• Bugfix");
            richTextBox.AppendText(" - Talisman Creation dialog now shows only the skills that have already been added to the skillset\n");

            WriteBoldText(richTextBox, "• Bugfix");
            richTextBox.AppendText(" - Talisman Creation dialog now adds a default title if the Title field is empty\n");

            WriteBoldText(richTextBox, "• Plus");
            richTextBox.AppendText(" - Added support for ");
            WriteBoldText(richTextBox, "Max Skill Types");
            richTextBox.AppendText(" talisman\n");

            WriteBoldText(richTextBox, "• Plus");
            richTextBox.AppendText(" - Added this ");
            WriteBoldText(richTextBox, "What's New");
            richTextBox.AppendText(" screen!");
        }

        /// <summary>
        /// Populates "Previous Updates" field with text
        /// </summary>
        private void WritePreviousUpdatesText()
        {
            var richTextBox = richTextBox_PreviousUpdates;
            richTextBox.Clear();

            WriteBoldText(richTextBox, "• Example");
            richTextBox.AppendText(" - Example\n");

            WriteBoldText(richTextBox, "• Example");
            richTextBox.AppendText(" - Example\n");

            WriteBoldText(richTextBox, "• Example");
            richTextBox.AppendText(" - Example\n");

            WriteBoldText(richTextBox, "• Example");
            richTextBox.AppendText(" - Example\n");

            WriteBoldText(richTextBox, "• Example");
            richTextBox.AppendText(" - Example\n");

            WriteBoldText(richTextBox, "• Example");
            richTextBox.AppendText(" - Example\n");

            WriteBoldText(richTextBox, "• Example");
            richTextBox.AppendText(" - Example\n");

            WriteBoldText(richTextBox, "• Example");
            richTextBox.AppendText(" - Example\n");

            WriteBoldText(richTextBox, "• Example");
            richTextBox.AppendText(" - Example\n");

            WriteBoldText(richTextBox, "• Example");
            richTextBox.AppendText(" - Example\n");

            WriteBoldText(richTextBox, "• Example");
            richTextBox.AppendText(" - Example\n");

            WriteBoldText(richTextBox, "• Example");
            richTextBox.AppendText(" - Example\n");

            WriteBoldText(richTextBox, "• Example");
            richTextBox.AppendText(" - Example\n");

            WriteBoldText(richTextBox, "• Example");
            richTextBox.AppendText(" - Example\n");

            WriteBoldText(richTextBox, "• Example");
            richTextBox.AppendText(" - Example\n");

            WriteBoldText(richTextBox, "• Example");
            richTextBox.AppendText(" - Example\n");

            WriteBoldText(richTextBox, "• Example");
            richTextBox.AppendText(" - Example\n");

            WriteBoldText(richTextBox, "• Example");
            richTextBox.AppendText(" - Example\n");

            WriteBoldText(richTextBox, "• Example");
            richTextBox.AppendText(" - Example\n");

            WriteBoldText(richTextBox, "• Example");
            richTextBox.AppendText(" - Example\n");
        }
    }
}
