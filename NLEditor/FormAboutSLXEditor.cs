﻿using System;
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
    public partial class FormAboutSLXEditor : Form
    {
        public FormAboutSLXEditor()
        {
            int GetCenter(Control component)
            {
                return (this.ClientSize.Width - component.Width) / 2;
            }

            KeyPreview = true;

            Int32 rtbWidth = 720;
            Int32 topMargin = 12;
            Int32 padding = 12;

            InitializeComponent();

            richTextBox_WhatsNew.Width = rtbWidth;
            richTextBox_PreviousUpdates.Width = rtbWidth;
            picturePadding.Left = richTextBox_WhatsNew.Right;
            pictureClimber.Left = richTextBox_WhatsNew.Right;

            lblWhatsNew.Text = "What's New in SuperLemmix Editor"; //+ C.Version;
            lblWhatsNew.Top = topMargin;
            lblWhatsNew.Left = GetCenter(lblWhatsNew);
            WriteWhatsNewText();

            lblPreviousUpdates.Left = GetCenter(lblPreviousUpdates);
            WritePreviousUpdatesText();

            lblSuperLemmixEditor.Text = "SuperLemmix Editor (Version " + C.Version + "-B)";
            lblSuperLemmixEditor.Top = richTextBox_PreviousUpdates.Bottom + padding;
            lblSuperLemmixEditor.Left = GetCenter(lblSuperLemmixEditor);

            lblAuthor.Top = lblSuperLemmixEditor.Bottom + padding;
            lblAuthor.Left = GetCenter(lblAuthor);
            lblBasedOn.Top = lblAuthor.Bottom;
            lblBasedOn.Left = GetCenter(lblBasedOn);

            lblThanksTo.Top = lblBasedOn.Bottom + padding;
            lblThanksTo.Left = GetCenter(lblThanksTo);
            lblDMA.Top = lblThanksTo.Bottom;
            lblDMA.Left = GetCenter(lblDMA);
            lblLFCommunity.Top = lblDMA.Bottom;
            lblLFCommunity.Left = GetCenter(lblLFCommunity);
            linkLF.Top = lblLFCommunity.Bottom;
            linkLF.Left = GetCenter(linkLF);

            check_ShowThisWindow.Top = linkLF.Bottom + padding;
            check_ShowThisWindow.Left = GetCenter(check_ShowThisWindow);
            check_ShowThisWindow.Checked = Properties.Settings.Default.ShowAboutSLXWindowAtStartup;
        }

        private void Check_ShowThisWindow_CheckedChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.ShowAboutSLXWindowAtStartup = check_ShowThisWindow.Checked;
            Properties.Settings.Default.Save();
        }

        private void FormAboutSLXEditor_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                Close();
            }
        }

        private void linkLF_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            string url = "https://www.lemmingsforums.net";
            System.Diagnostics.Process.Start(url);
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

            // Version 2.8.X features
            WriteBoldText(richTextBox, "Version 2.8.7\n");
            WriteBoldText(richTextBox, "• New Objects Support");
            richTextBox.AppendText(" - Added support for NeoLemmix 12.14 new objects (portal, (de)assigner, (de)neutralizer)\n");

            WriteBoldText(richTextBox, "• Piece Browser");
            richTextBox.AppendText(" - Added 'Steel' tab for steel pieces\n");

            WriteBoldText(richTextBox, "• Piece Browser");
            richTextBox.AppendText(" - Piece data (size, resize/nine-slice info) and object descriptions are now (optionally) shown in Piece Browser\n");

            WriteBoldText(richTextBox, "• Piece Browser");
            richTextBox.AppendText(" - 3-way option 'Data/Descriptions/Pieces Only' switches between showing additional piece data, descriptions (previously 'Show piece names'), or just the pieces\n");
            richTextBox.AppendText(" - Note that for objects, the Type rather than the Name is shown when the 'Data' or 'Descriptions' option is active\n");

            WriteBoldText(richTextBox, "• Piece Browser");
            richTextBox.AppendText(" - Added resizing info to the tooltips\n");

            WriteBoldText(richTextBox, "• Piece Browser");
            richTextBox.AppendText(" - Info labels are now drawn with a filled background to ensure visability\n");

            WriteBoldText(richTextBox, "• Piece Selection");
            richTextBox.AppendText(" - Added piece size info to metadata in Pieces tab\n");

            WriteBoldText(richTextBox, "• Bugfix - Piece Search");
            richTextBox.AppendText(" - Terrain pieces are now included in the search for resizable/nine-sliced pieces\n");

            WriteBoldText(richTextBox, "• Bugfix - Piece Search");
            richTextBox.AppendText(" - Adding a piece to the level via the Piece Search now focuses the Pieces tab\n");

            // Version 2.8.X features
            WriteBoldText(richTextBox, "• NeoLemmix Mode");
            richTextBox.AppendText(" - Added support for NeoLemmix CE\n");

            WriteBoldText(richTextBox, "• NeoLemmix Mode");
            richTextBox.AppendText(" - The maximum number of skill types in a randomly-generated skillset is now 10 when in NeoLemmix Mode\n");

            WriteBoldText(richTextBox, "• Editor Mode");
            richTextBox.AppendText(" - Editor Mode is now set to Auto by default\n");

            WriteBoldText(richTextBox, "• Bugfix - Pre/PostView Text dialog");
            richTextBox.AppendText(" - Added a button to clear the text input, and blank text is now handled more gracefully\n");

            WriteBoldText(richTextBox, "• Bugfix - Piece Search");
            richTextBox.AppendText(" - Search is now case-insensitive, preventing errors when adding pieces from style sets with uppercase characters\n");

            WriteBoldText(richTextBox, "• Bugfix - Piece Search");
            richTextBox.AppendText(" - Added support for (Anti)SplatPads and Decoration objects to Piece Search\n");

            WriteBoldText(richTextBox, "• Level Arranger Window");
            richTextBox.AppendText(" - It's now possible to open the level arranger in a pop-out window which can be resized and moved between displays. Its size and position are remembered between sessions\n");

            WriteBoldText(richTextBox, "• Expand/Collapse All Tabs");
            richTextBox.AppendText(" - Tabs can be expanded and collapsed on-the-fly using a hotkey/menu item (this feature is intended to accompany the level arranger window, but can be used at any time)\n");

            WriteBoldText(richTextBox, "• Pre-placed Lemming");
            richTextBox.AppendText(" - Added pink (X, Y) location pin to pre-placed lemming)\n");

            WriteBoldText(richTextBox, "• Piece Selection");
            richTextBox.AppendText(" - Piece Metadata is now displayed in the \"Pieces\" tab, showing name, style and type. Also added a \"Load Style\" button to load the style of the selected piece into the browser\n");

            WriteBoldText(richTextBox, "• Piece Selection");
            richTextBox.AppendText(" - Clicking (and selecting) a piece now opens the \"Pieces\" tab)\n");

            WriteBoldText(richTextBox, "• Piece Selection");
            richTextBox.AppendText(" - Added a \"Select All\" hotkey (Ctrl+A by default)\n");

            WriteBoldText(richTextBox, "• Snap-to-Grid");
            richTextBox.AppendText(" - When snap-to-grid is active, the grid lines are now displayed in a colour of your choice (they can also be invisible, as before)\n");

            WriteBoldText(richTextBox, "• Preview/Postview Text Input");
            richTextBox.AppendText(" - Widened and heightened the text input dialog, also added a \"Preview\" button to show how the text will appear on the screen in-game\n");

            WriteBoldText(richTextBox, "• Layout");
            richTextBox.AppendText(" - Decreased scrollbar thickness slightly)\n");

            WriteBoldText(richTextBox, "• Layout");
            richTextBox.AppendText(" - Improved position of 'Clear Background' and 'Search Pieces' buttons\n");

            WriteBoldText(richTextBox, "• Layout");
            richTextBox.AppendText(" - Set minimum window size to 900 x 600)\n");

            WriteBoldText(richTextBox, "• Bugfix - Entrance Hatches");
            richTextBox.AppendText(" - When flipping a hatch horizontally, the Flip Offset value is calculated and written to the level file so the Player can match its position as seen in the Editor)\n");

            WriteBoldText(richTextBox, "• Bugfix - Character Limits");
            richTextBox.AppendText(" - Character limit for title and author is now 54 (determined by Preview Screen in FullScreen)\n");

            WriteBoldText(richTextBox, "• Bugfix - Settings");
            richTextBox.AppendText(" - Fixed cancelling of changes to Settings when using the \"Cancel\" button)\n");

            WriteBoldText(richTextBox, "• Fully-customisable hotkeys");
            richTextBox.AppendText(" - Choose your own hotkey layout for the Editor's features - - (As of 2.8.4 the form is now displayed at the correct size in all screen resolutions)\n");

            WriteBoldText(richTextBox, "• Piece Search dialog added to browser");
            richTextBox.AppendText(" - Search for specific pieces by name, style, object type, and other properties\n");

            WriteBoldText(richTextBox, "• NeoLemmix Mode");
            richTextBox.AppendText(" - Activates NeoLemmix-specific controls and features across the Editor's UI - (As of 2.8.4 it's now possible to switch between Neo/Super/Auto Modes without closing and re-opening the Editor) \n");

            WriteBoldText(richTextBox, "• Settings");
            richTextBox.AppendText(" - Improved Settings form with more sophisticated UI\n");

            WriteBoldText(richTextBox, "• Talismans");
            richTextBox.AppendText(" - Added support for \"Max Skill Types\" talisman\n");

            WriteBoldText(richTextBox, "• Hotkeys");
            richTextBox.AppendText(" - Added Group/Ungroup Pieces hotkeys, plus various others for new features\n");

            WriteBoldText(richTextBox, "• Bugfix - Cursor Zoom");
            richTextBox.AppendText(" - Cursor anchor is now correctly preserved when zooming in and out\n");

            WriteBoldText(richTextBox, "• Bugfix - Preview/Postview Text");
            richTextBox.AppendText(" - Text is now displayed centred for better previewing\n");

            WriteBoldText(richTextBox, "• Bugfix - Talismans");
            richTextBox.AppendText(" - Dialog now shows only the skills that have already been added to the skillset\n");

            WriteBoldText(richTextBox, "• Bugfix - Talismans");
            richTextBox.AppendText(" - Dialog now adds a default title if the Title field is empty\n");

            WriteBoldText(richTextBox, "• Bugfix - Character Limits");
            richTextBox.AppendText(" - Character limits increased to SLX Player UI limits: Title (62), Author (60), Talisman TItle (85)\n");

            WriteBoldText(richTextBox, "• Bugfix - UI");
            richTextBox.AppendText(" - All secondary windows can now be closed using the [Esc] key\n");

            WriteBoldText(richTextBox, "• Bugfix - Cleanse Levels");
            richTextBox.AppendText(" - Confirmation dialog is now shown before proceeding with the cleanse\n");

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

            WriteBoldText(richTextBox, "• Bugfix - Trigger Areas");
            richTextBox.AppendText(" - Fixed trigger area repositionings for flipped/inverted/rotated objects\n");

            WriteBoldText(richTextBox, "• Bugfix - UI");
            richTextBox.AppendText(" - Zoom factor is now 1 instead of 0 when opening the Editor\n");

            WriteBoldText(richTextBox, "• Bugfix - UI");
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

            WriteBoldText(richTextBox, "• Bugfix - Missing Piece Handling");
            richTextBox.AppendText(" - Levels with missing pieces no longer create infinite popups; instead, a status bar is used to inform the player that the level has missing pieces\n");

            WriteBoldText(richTextBox, "• Bugfix - Missing Piece Handling");
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
            WriteBoldText(richTextBox, "• Bugfix - Talismans");
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

            WriteBoldText(richTextBox, "• Bugfix - UI");
            richTextBox.AppendText(" - The state of the AutoStart checkbox is now remembered per-level when Closing and re-opening the Editor\n");

            WriteBoldText(richTextBox, "• Centred Dialogs");
            richTextBox.AppendText(" - All dialogs (Hotkeys, Options, About, Validate Level, etc) now appear centre-screen\n");

            WriteBoldText(richTextBox, "• Tab Display");
            richTextBox.AppendText(" - \"Display Tabs\" is no longer an option - tab display is the default and only option (necessary due to the various 2.X updates\n");

            WriteBoldText(richTextBox, "• Bugfix - Talismans");
            richTextBox.AppendText(" - A num up/down box was displayed for Kill All Zombies talisman when it didn't need to be, and wasn't displaying when it did need to be for other talismans\n");

            // Version 2.1.X features
            WriteBoldText(richTextBox, "\nVersion 2.1.X\n");
            WriteBoldText(richTextBox, "• Bugfix - UI");
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
            WriteBoldText(richTextBox, "• Bugfix - UI");
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
    }
}
