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
            KeyPreview = true;

            Int32 topMargin = 10;

            InitializeComponent();

            lblWhatsNew.Text = "What's New in SuperLemmix Editor"; //+ C.Version;
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

            // Version 2.8.3 features
            WriteBoldText(richTextBox, "Version 2.8.3\n");
            WriteBoldText(richTextBox, "• NeoLemmix Mode");
            richTextBox.AppendText(" - Activates NeoLemmix-specific controls and features\n");

            WriteBoldText(richTextBox, "• Bugfix");
            richTextBox.AppendText(" - Cursor anchor is now correctly preserved when zooming in and out\n");

            WriteBoldText(richTextBox, "• Bugfix");
            richTextBox.AppendText(" - Talisman Creation dialog now shows only the skills that have already been added to the skillset\n");

            WriteBoldText(richTextBox, "• Bugfix");
            richTextBox.AppendText(" - Talisman Creation dialog now adds a default title if the Title field is empty\n");

            WriteBoldText(richTextBox, "• Bugfix");
            richTextBox.AppendText(" - Character limits increased to SLX Player UI limits: Title (62), Author (60), Talisman TItle (85)\n");

            WriteBoldText(richTextBox, "• Talismans");
            richTextBox.AppendText(" - Added support for \"Max Skill Types\" talisman\n");

            // Version 2.8.X features
            WriteBoldText(richTextBox, "\nVersion 2.8.X\n");
            WriteBoldText(richTextBox, "• Maximum Lemmings Count");
            richTextBox.AppendText(" - 999 is now the maximum number of lemmings supported by the Editor; this is to match SLX Player skill panel display\n");

            WriteBoldText(richTextBox, "• Skillset Features");
            richTextBox.AppendText(" - Added buttons for Random Skillset and Set All Skills To Zero\n");

            WriteBoldText(richTextBox, "• Rivals");
            richTextBox.AppendText(" - Support for Rival lemmings added\n");

            WriteBoldText(richTextBox, "• Helper Icons");
            richTextBox.AppendText(" - Added helper icons to show pre-placed-lem/hatch/exit skills & properties\n");

            WriteBoldText(richTextBox, "• Hotkeys");
            richTextBox.AppendText(" - Added option to use (<,) and (>.) keys for draw sooner/draw later\n");

            WriteBoldText(richTextBox, "• Save As Image");
            richTextBox.AppendText(" - Added Save As Image option (plus shortcut) to the File menu; this saves a .png image of the currently loaded level\n");

            WriteBoldText(richTextBox, "• Cleanse Levels update");
            richTextBox.AppendText(" - We now show a progress bar during the process, and re-initialize the Editor when it's finished\n");

            WriteBoldText(richTextBox, "• Bugfix");
            richTextBox.AppendText(" - Fixed trigger area repositionings for flipped/inverted/rotated objects\n");

            WriteBoldText(richTextBox, "• Bugfix");
            richTextBox.AppendText(" - Zoom factor is now 1 instead of 0 when opening the Editor\n");

            WriteBoldText(richTextBox, "• Bugfix");
            richTextBox.AppendText(" - Editor now opens Maximized by default\n");

            // Version 2.7.X features
            WriteBoldText(richTextBox, "\nVersion 2.7.X\n");
            WriteBoldText(richTextBox, "• Cleanse Levels");
            richTextBox.AppendText(" - Added \"Cleanse Levels\" menu item - this automatically re-saves all levels in a specified pack to ensure compatibility with SLX\n");

            WriteBoldText(richTextBox, "• Pickup Skills");
            richTextBox.AppendText(" - Improved Pickup Skill graphics so that the icon is displayed more clearly\n");

            WriteBoldText(richTextBox, "• Invincibility");
            richTextBox.AppendText(" - Allowing this for Collectibles is designer-side optional; if checked, the lemming to grab the final Collectible will become Invincible for the remainder of the level!\n");

            WriteBoldText(richTextBox, "• New Objects");
            richTextBox.AppendText(" - Support added for Collectibles, Lava and Decoration objects\n");

            WriteBoldText(richTextBox, "• Bugfix");
            richTextBox.AppendText(" - Levels with missing pieces no longer create infinite popups; instead, a status bar is used to inform the player that the level has missing pieces\n");

            WriteBoldText(richTextBox, "• Bugfix");
            richTextBox.AppendText(" - Saving a level with missing pieces will create a unique file rather than overwriting the original\n");

            // Version 2.6.X features
            WriteBoldText(richTextBox, "\nVersion 2.6.X\n");
            WriteBoldText(richTextBox, "• Maximum Lemmings Count");
            richTextBox.AppendText(" - Increased maximum lemmings count to 1000\n");

            WriteBoldText(richTextBox, "• Level Size");
            richTextBox.AppendText(" - Maximum level width increased to 6400px, maximum height decreased to 1600px\n");

            WriteBoldText(richTextBox, "• New Skills");
            richTextBox.AppendText(" - Added support for Ballooner and Ladderer\n");

            // Version 2.5.X features
            WriteBoldText(richTextBox, "\nVersion 2.5.X\n");
            WriteBoldText(richTextBox, "• Bugfix");
            richTextBox.AppendText(" - Talisman requirements that aren't number-related (e.g. Play in Classic Mode, Kill All Zombies) no longer display \": 0\" unnecessarily when added\n");

            WriteBoldText(richTextBox, "• Radiation & Slowfreeze");
            richTextBox.AppendText(" - Added support for variable countdown on Radiation and Slowfreeze objects\n");

            // Version 2.4.X features
            WriteBoldText(richTextBox, "\nVersion 2.4.X\n");
            WriteBoldText(richTextBox, "• Hotkeys");
            richTextBox.AppendText(" - Ctrl + Wheel now scrolls the level horizontally, whilst Shift/Alt + Wheel scrolls the level vertically (when zoomed in)\n");

            WriteBoldText(richTextBox, "• Hotkeys");
            richTextBox.AppendText(" - Shift + LMB now selects/deselects individual pieces as well as Ctrl + LMB\n");

            WriteBoldText(richTextBox, "• Hotkeys");
            richTextBox.AppendText(" - Horizontal Drag is now controlled by Ctrl + RMB as well as Ctrl + Alt + LMB\n");

            WriteBoldText(richTextBox, "• Hotkeys");
            richTextBox.AppendText(" - Vertical Drag is now controlled by Alt/Shift + RMB as well as Ctrl + Shift + LMB\n");

            WriteBoldText(richTextBox, "• Hotkeys");
            richTextBox.AppendText(" - (RMB on its own still performs drag-to-scroll)\n");

            WriteBoldText(richTextBox, "• Superlemming Mode");
            richTextBox.AppendText(" - \"Activate Superlemming Mode\" checkbox added\n");

            WriteBoldText(richTextBox, "• Talismans");
            richTextBox.AppendText(" - Support for \"Play in Classic Mode\" and \"Play Without Pressing Pause\" talismans added\n");

            WriteBoldText(richTextBox, "• New Objects");
            richTextBox.AppendText(" - Support for Poison, Radiation and Slowfreeze added\n");

            WriteBoldText(richTextBox, "• UI Improvements");
            richTextBox.AppendText(" - Moved Custom Move numeric to Settings menu instead of Pieces tab (seems more appropriate there)\n");

            WriteBoldText(richTextBox, "• UI Improvements");
            richTextBox.AppendText(" - Updated all menu dropdrowns to display the hotkey to the right\n");

            WriteBoldText(richTextBox, "• UI Improvements");
            richTextBox.AppendText(" - Condensed \"Tools\" and \"Options\" to a single menu\n");

            WriteBoldText(richTextBox, "• UI Improvements");
            richTextBox.AppendText(" - Moved \"Play Level\" and \"Validate Level (which now has a Ctrl+F12 hotkey)\" to the File menu\n");

            WriteBoldText(richTextBox, "• UI Improvements");
            richTextBox.AppendText(" - Refinements to Hotkey dialog layout\n");

            WriteBoldText(richTextBox, "• UI Improvements");
            richTextBox.AppendText(" - Further refinements to scrollbars\n");

            WriteBoldText(richTextBox, "• Bugfix");
            richTextBox.AppendText(" - The state of the AutoStart checkbox is now remembered per-level when Closing and re-opening the Editor\n");

            WriteBoldText(richTextBox, "• Centred Dialogs");
            richTextBox.AppendText(" - All dialogs (Hotkeys, Options, About, Validate Level, etc) now appear centre-screen\n");

            WriteBoldText(richTextBox, "• Tab Display");
            richTextBox.AppendText(" - \"Display Tabs\" is no longer an option - tab display is the default and only option (necessary due to the various 2.X updates\n");

            WriteBoldText(richTextBox, "• Bugfix");
            richTextBox.AppendText(" - A num up/down box was displayed for Kill All Zombies talisman when it didn't need to be, and wasn't displaying when it did need to be for other talismans\n");

            // Version 2.1.X features
            WriteBoldText(richTextBox, "\nVersion 2.1.X\n");
            WriteBoldText(richTextBox, "• Bugfix");
            richTextBox.AppendText(" - Auto-start checkbox is no longer checked by default, but its state is remembered when closing and re-loading the Editor\n");

            WriteBoldText(richTextBox, "• Custom Move");
            richTextBox.AppendText(" - Alt + Arrow keys now move selected pieces by a custom amount (specified in the F10 settings menu - the default is 64px)\n");

            WriteBoldText(richTextBox, "• Horizontal-Only Move");
            richTextBox.AppendText(" - Pressing Ctrl + Alt before moving selected pieces with the mouse activates Horizontal-only movement, allowing the selected pieces to be moved only along the X-axis\n");

            WriteBoldText(richTextBox, "• Vertical-Only Move");
            richTextBox.AppendText(" - Pressing Ctrl + Shift before moving selected pieces with the mouse activates Vertical-only movement, allowing the selected pieces to be moved only along the Y-axis\n");

            WriteBoldText(richTextBox, "• New Objects");
            richTextBox.AppendText(" - Support for Blasticine and Vinewater added\n");

            // Version 2.0.X features
            WriteBoldText(richTextBox, "\nVersion 2.0.X\n");
            WriteBoldText(richTextBox, "• Bugfix");
            richTextBox.AppendText(" - Fixed bug affecting the position of the screen area in relation to the scrollbars when zoomed in\n");

            WriteBoldText(richTextBox, "• UI Improvements");
            richTextBox.AppendText(" - \"Clear Backgrounds\" button moved to above the piece scroller for better access\n");

            WriteBoldText(richTextBox, "• UI Improvements");
            richTextBox.AppendText(" - Larger scrollbars for easier access when fine-editing a level\n");

            WriteBoldText(richTextBox, "• UI Improvements");
            richTextBox.AppendText(" - Theme/style dropdowns widened for easier reading\n");

            WriteBoldText(richTextBox, "• New Talismans");
            richTextBox.AppendText(" - Added support for Play in Classic Mode, Play Without Pressing Pause, and Kill All Zombies talismans\n");

            WriteBoldText(richTextBox, "• New Skills");
            richTextBox.AppendText(" - Added support for Spearer, Grenader and Timebomber\n");

            WriteBoldText(richTextBox, "• About Dialog");
            richTextBox.AppendText(" - \"About\" info updated\n");

            WriteBoldText(richTextBox, "• UI updates");
            richTextBox.AppendText(" - New icon and title added\n");
        }

        /// <summary>
        /// Populates "Previous Updates" field with text
        /// </summary>
        private void WritePreviousUpdatesText()
        {
            var richTextBox = richTextBox_PreviousUpdates;
            richTextBox.Clear();
        }

        private void FormWhatsNew_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                Close();
            }
        }
    }
}
