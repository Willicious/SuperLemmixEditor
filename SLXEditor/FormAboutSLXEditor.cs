using System;
using System.Drawing;
using System.Windows.Forms;

namespace SLXEditor
{
    public partial class FormAboutSLXEditor : Form
    {
        private Settings curSettings;
        internal FormAboutSLXEditor(Settings settings)
        {
            curSettings = settings;

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

            pictureWhatsNew.Top = topMargin;
            pictureWhatsNew.Left = GetCenter(pictureWhatsNew);
            WriteWhatsNewText();

            lblPreviousUpdates.Left = GetCenter(lblPreviousUpdates);
            WritePreviousUpdatesText();

            lblSuperLemmixEditor.Text = $"SuperLemmix Editor (Version {C.Version}-beta)";
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
            check_ShowThisWindow.Checked = curSettings.ShowAboutAtStartup;
        }

        private void Check_ShowThisWindow_CheckedChanged(object sender, EventArgs e)
        {
            curSettings.ShowAboutAtStartup = check_ShowThisWindow.Checked;
            curSettings.WriteSettingsToFile();
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

            // Test version text
            WriteBoldText(richTextBox, "This version of the Editor is for test purposes only!\n");
            richTextBox.AppendText("Please do not distribute it publicly as stability cannot be guaranteed. Thanks\n\n");

            // =======================
            // Latest Version Features
            // (Delete and merge these into 'all features' below with each update)  
            // =======================

            WriteBoldText(richTextBox, $"================ Version {C.Version} Updates ================\n");
            WriteBoldText(richTextBox, "\n• Piece Browser\n");
            richTextBox.AppendText(" • Added a 'Random' button to the Piece Browser which, when clicked, randomized the piece style selection. It's possible to specify which styles are Randomized in the Style Manager; if no styles are specified, the entire list is randomized\n");

            WriteBoldText(richTextBox, "\n• Default Author Name\n");
            richTextBox.AppendText(" • Added a setting which automatically applies a default author name when a new level is created\n");

            WriteBoldText(richTextBox, "\n• Bugfixes\n");
            richTextBox.AppendText(" • No Overwrite checkbox is once again available for all pieces (except Rulers)\n");
            richTextBox.AppendText(" • Fixed incremental indenting of terrain pieces in level file\n");
            richTextBox.AppendText(" • All settings are now externalized to SLXEditorSettings.ini\n");

            // =======================
            // All Features
            // =======================
            WriteBoldText(richTextBox, $"\n\n================ Previous Updates ================\n");
            
            WriteBoldText(richTextBox, "\n• New SuperLemmix-Specific Format (.sxlv)\n");
            richTextBox.AppendText(" • It's now possible to file-associate SuperLemmix levels with the SuperLemmix Editor, and NeoLemmix Levels with the NeoLemmix Editor. The SuperLemmix Format (.sxlv) will always be preferred as the default format unless NeoLemmix Mode is active. You can still choose to save to .nxlv as long as the level does not contain any SuperLemmix-specific features\n");

            WriteBoldText(richTextBox, "\n• Added support for new 'Steel Type' level property\n");
            richTextBox.AppendText(" • It's now possible to set steel as 'only where visible (NeoLemmix-style)' or 'always steel' on a per-level basis (supported in SuperLemmix 3.0)\n");

            WriteBoldText(richTextBox, "\n• 'Sketches' are now 'Rulers'\n");
            richTextBox.AppendText(" • Rulers are drawn to their own layer and with their own selection rectangle colour for easier identification. The layer can be toggled on/off via hotkey/menu item. Full backwards compatibility with existing Sketches is preserved (NOTE: the 'sketches' folder is auto-renamed to 'rulers' if the Rulers folder is not found).\n");

            WriteBoldText(richTextBox, "\n• Refresh Rulers\n");
            richTextBox.AppendText(" • Refresh Styles now also refreshes rulers\n");

            WriteBoldText(richTextBox, "\n• Batch Exporter\n");
            richTextBox.AppendText(" • Use the batch exporter to convert levels to the following formats: SuperLemmix (.sxlv), NeoLemmix (.nxlv), RetroLemmini (.rlv) and Lemmini (.ini)\n");

            WriteBoldText(richTextBox, "\n• INI Exporter\n");
            richTextBox.AppendText(" • Export individual levels to RetroLemmini (.rlv) and Lemmini (.ini)\n");
            richTextBox.AppendText(" • Create translation tables to link SuperLemmix style pieces to their (Retro)Lemmini counterparts\n");

            WriteBoldText(richTextBox, "\n• Style Manager\n");
            richTextBox.AppendText(" • Added a style manager to create/edit the style list\n");

            WriteBoldText(richTextBox, "\n• NeoLemmix 12.14 Objects\n");
            richTextBox.AppendText(" • Added support for NeoLemmix 12.14 objects (Portals, De-Neutralizer, Skill De-Assigner)\n");

            WriteBoldText(richTextBox, "\n• Automatic Lem Count Button\n");
            richTextBox.AppendText(" • Added a button to the Globals tab which automatically sets the lem/save counts to the most appropriate for the number and type of pre-placed lemmings\n");

            WriteBoldText(richTextBox, "\n• Skills Tab\n");
            richTextBox.AppendText(" • Re-added \"Clear Skillset\" button\n");
            richTextBox.AppendText(" • Random can now go from 0 to 100\n");
            richTextBox.AppendText(" • Removed \"Apply Custom Skillset\" button (the combo now applies the skillset when changed)\n");

            WriteBoldText(richTextBox, "\n• Cleanse Levels\n");
            richTextBox.AppendText(" • It's now possible to save to either .sxlv or .nxlv when cleansing\n");
            richTextBox.AppendText(" • It's now possible to delete deprecated pieces\n");
            richTextBox.AppendText(" • A more complete list of errors is now shown (Missing Pieces, Deprecated Pieces, No Lemmings/Exits) and a .txt report is generated in the target level folder\n");

            WriteBoldText(richTextBox, "\n• Validate Levels\n");
            richTextBox.AppendText(" • Bugfix - \"Edit Level\" button is not shown if validator opens when cleansing\n");

            WriteBoldText(richTextBox, "\n• UI\n");
            richTextBox.AppendText(" • Improved Entrance Hatch arrow rendering\n");
            richTextBox.AppendText(" • Steel now has its own selection rectangle colour\n");
            richTextBox.AppendText(" • Added zoom factor indicator to corner text\n");
            richTextBox.AppendText(" • It's now possible to switch between Release Rate & Spawn Interval\n");
            richTextBox.AppendText(" • Improved layout of piece data & 'Load Style' button in Pieces tab\n");

            WriteBoldText(richTextBox, "\n• Bugfixes\n");
            richTextBox.AppendText(" • Combos are now correctly refreshed when changing levels\n");
            richTextBox.AppendText(" • Music list now searches recursively in subfolders of 'music'\n");
            richTextBox.AppendText(" • NeoLemmix levels are marked as such when saving\n");
            richTextBox.AppendText(" • Restored 'changing main style also changes piece style when no pieces are added' behaviour\n");
            richTextBox.AppendText(" • Piece Search now finds all object types correctly\n");
            richTextBox.AppendText(" • Piece-editing buttons now become unavailable after deleting a piece\n");
            richTextBox.AppendText(" • Piece size label is emptied when no piece is selected\n");
            richTextBox.AppendText(" • Scrollbars on Level Arranger window no longer eat hotkey presses\n");
            richTextBox.AppendText(" • Cursor is correctly reset when clicking status bar buttons\n");
            richTextBox.AppendText(" • The Editor now closes if the 'styles' folder cannot be found\n");
            richTextBox.AppendText(" • Many other general improvements, typo fixes, tweaks, spit and polish\n");

            // =======================
            // Piece Browser
            // =======================
            WriteBoldText(richTextBox, "\n• Piece Browser\n");
            richTextBox.AppendText(" • Added 'Steel' tab\n");
            richTextBox.AppendText(" • 3-way option 'Data/Descriptions/Pieces Only' switches between showing additional piece data, descriptions (previously 'Show piece names'), or just the pieces\n");
            richTextBox.AppendText(" • Added option to show type rather than name for objects\n");
            richTextBox.AppendText(" • Added option to either scroll piece browser infinitely, or stop at the lowest/highest pieces in each tab\n");
            richTextBox.AppendText(" • Added resizing info to the tooltips\n");
            richTextBox.AppendText(" • Info labels are now drawn with a filled background to ensure visability\n");

            WriteBoldText(richTextBox, "\n• Piece Search\n");
            richTextBox.AppendText(" • Search for specific pieces by name, style, object type, and various other properties\n");

            // =======================
            // Pieces Tab
            // =======================
            WriteBoldText(richTextBox, "\n• Pieces Tab\n");
            richTextBox.AppendText(" • Clicking (and selecting) a piece now opens the \"Pieces\" tab)\n");
            richTextBox.AppendText(" • Piece Metadata is now displayed, showing name, style, type and size\n");
            richTextBox.AppendText(" • Added a \"Load Style\" button to load the style of the selected piece into the browser\n");

            // =======================
            // Skills Tab
            // =======================
            WriteBoldText(richTextBox, "\n• Skills Tab\n");
            WriteBoldText(richTextBox, "• Custom Skillsets");
            richTextBox.AppendText(" - Added a button for \"Save As Custom Skillset\" which allows the user to save the currently-applied skillset as a custom preset. When saving, entering the name of an existing custom skillset will overwrite the existing one\n");

            WriteBoldText(richTextBox, "• Custom Skillsets");
            richTextBox.AppendText(" - Added a dropdown menu for custom preset skillsets (using NLCustomSkillsets.ini)\n");

            WriteBoldText(richTextBox, "• Random Skillset");
            richTextBox.AppendText(" - Added button for Random Skillset, which creates a skillset of up to 10 skills at random, using specified amounts per-skill\n");

            WriteBoldText(richTextBox, "• Set All Non-Zero Skills to [N]");
            richTextBox.AppendText(" - Added a new button to change all non-zero skills to N, where N can be specified using a numeric control\n");

            WriteBoldText(richTextBox, "• Clear Skillset");
            richTextBox.AppendText(" - Added button to clear the skillset, resetting all numerics to 0\n");

            // =======================
            // Hotkeys
            // =======================
            WriteBoldText(richTextBox, "\n• Hotkeys\n");
            WriteBoldText(richTextBox, "• Fully-customisable hotkeys");
            richTextBox.AppendText(" - Choose your own hotkey layout for the Editor's features!\n");

            WriteBoldText(richTextBox, "• Duplicate Up/Down/Left/Right");
            richTextBox.AppendText(" - Duplicate piece(s) to the immediate N/E/S/W of the selected piece(s)\n");

            WriteBoldText(richTextBox, "• Custom Move");
            richTextBox.AppendText(" - Move selected pieces by a custom amount (specified in the F10 settings menu - the default is 64px)\n");

            WriteBoldText(richTextBox, "• Move by Grid Amount");
            richTextBox.AppendText(" - Previous hotkeys to move pieces by 8px now move pieces by the specified grid size\n");

            WriteBoldText(richTextBox, "• Group/Ungroup Pieces");
            richTextBox.AppendText(" - Added Group/Ungroup Pieces hotkeys\n");

            WriteBoldText(richTextBox, "• Horizontal-Only Move");
            richTextBox.AppendText(" - Move selected pieces along the X-axis only\n");

            WriteBoldText(richTextBox, "• Vertical-Only Move");
            richTextBox.AppendText(" - Move selected pieces along the Y-axis only\n");

            WriteBoldText(richTextBox, "• Set Screen Start to Mouse Cursor");
            richTextBox.AppendText(" - Set the screen start to the mouse cursor position\n");

            WriteBoldText(richTextBox, "• Expand/Collapse All Tabs");
            richTextBox.AppendText(" - Expanded or collapse the Globals/Pieces/Skills/Misc tabs\n");

            WriteBoldText(richTextBox, "• Select All");
            richTextBox.AppendText(" - Select all pieces in the level area (Ctrl+A by default)\n");

            // =======================
            // Level Validation
            // =======================
            WriteBoldText(richTextBox, "\n• Level Validation\n");
            richTextBox.AppendText(" • Added a setting to toggle automatic level validation on/off when manually saving a level\n");
            richTextBox.AppendText(" • Expanded validation checks and fixing options\n");
            richTextBox.AppendText(" • Validation now has a minimum time limit of 1 second\n");
            richTextBox.AppendText(" • Dialog now alerts the user that the lem count is higher than the pre-placed lem count (where relevant) rather than just showing \"missing hatch\"\n");

            // =======================
            // Talismans
            // =======================
            WriteBoldText(richTextBox, "\n• Talisman Creation\n");
            richTextBox.AppendText(" • Added support for \"Max Skill Types\" talisman\n");
            richTextBox.AppendText(" • Dialog now shows only the skills that have already been added to the skillset\n");
            richTextBox.AppendText(" • Renamed 'Add Requirement' button to 'Add This Requirement to List' for further clarity\n");
            richTextBox.AppendText(" • A default title is added if the Title field is empty\n");

            // =======================
            // SuperLemmix-Specific
            // =======================
            WriteBoldText(richTextBox, "\n• SuperLemmix-Specific Features\n");
            WriteBoldText(richTextBox, "• SLX Skills");
            richTextBox.AppendText(" - Added support for Ballooner, Timebomber, Freezer, Ladderer, Spearer and Grenader\n");
            WriteBoldText(richTextBox, "• Rivals");
            richTextBox.AppendText(" - Support for Rival lemmings added\n");
            WriteBoldText(richTextBox, "• Superlemming Mode");
            richTextBox.AppendText(" - \"Activate Superlemming Mode\" checkbox added\n");
            WriteBoldText(richTextBox, "• Invincibility");
            richTextBox.AppendText(" - Allowing this for Collectibles is designer-side optional; if checked, the lemming to grab the final Collectible will become Invincible for the remainder of the level!\n");
            WriteBoldText(richTextBox, "• SLX Objects");
            richTextBox.AppendText(" - Support added for Collectibles, Blasticine, Vinewater, Poison, Lava, Radiation, Slowfreeze, and Decoration objects\n");
            WriteBoldText(richTextBox, "• SLX Talismans");
            richTextBox.AppendText(" - Support for \"Kill All Zombies\", \"Play in Classic Mode\" and \"Play Without Pressing Pause\" talismans added\n");


            // =======================
            // UI
            // =======================
            WriteBoldText(richTextBox, "\n• Editor Mode\n");
            richTextBox.AppendText(" • It's now [pssible to switch between SuperLemmix (shows all controls), NeoLemmix (shows only NeoLemmix-relevant controls), and Auto (detects Super/NeoLemmix.exe, prefers Super) modes.\n");

            WriteBoldText(richTextBox, "\n• UI\n");
            richTextBox.AppendText(" • Scroll wheel can be used to change items when mousing over a dropdown list (without clicking)\n");
            richTextBox.AppendText(" • It's no longer possible to type into dropdown lists (to prevent accidental typing). However, it's now possible to use A-Z keys to quickly jump to a style/author when the list is active\n");
            richTextBox.AppendText(" • All secondary windows can now be closed using the [Esc] key\n");
            richTextBox.AppendText(" • Zoom factor is now 1 instead of 0 when opening the Editor\n");
            richTextBox.AppendText(" • Increased maximum zoom level\n");
            richTextBox.AppendText(" • Editor now opens Maximized by default\n");
            richTextBox.AppendText(" • Auto-start checkbox is no longer checked by default, but its state is remembered per-level when closing and re-loading the Editor\n");

            WriteBoldText(richTextBox, "• Level Arranger Window");
            richTextBox.AppendText(" - The Level Arranger can now be opened in its own pop-out window to accompany the Level Arranger. It's external-display compatible, and size & location are remembered between sessions\n");

            WriteBoldText(richTextBox, "• Piece Browser Window");
            richTextBox.AppendText(" - The Piece Browser can now be opened in its own pop-out window to accompany the Level Arranger. It's external-display compatible, and size & location are remembered between sessions\n");

            WriteBoldText(richTextBox, "• Highlight Grouped Pieces");
            richTextBox.AppendText(" - It's now possible to highlight all grouped pieces\n");

            WriteBoldText(richTextBox, "• Highlight Eraser Pieces");
            richTextBox.AppendText(" - It's now possible to highlight all pieces designated as 'Erase'\n");

            WriteBoldText(richTextBox, "• Trigger area colours");
            richTextBox.AppendText(" - It's now possible to choose between 5 different trigger area colours\n");

            WriteBoldText(richTextBox, "• Snap-to-Grid");
            richTextBox.AppendText(" - When snap-to-grid is active, the grid lines are now displayed in a colour of your choice (they can also be invisible, as before)\n");

            WriteBoldText(richTextBox, "• Pre-placed Lemming");
            richTextBox.AppendText(" - Added pink (X, Y) location pin to pre-placed lemming)\n");

            WriteBoldText(richTextBox, "• Helper Icons");
            richTextBox.AppendText(" - Added helper icons to show pre-placed-lem/hatch/exit skills & properties\n");

            WriteBoldText(richTextBox, "• Preview/Postview Text Input");
            richTextBox.AppendText(" - Widened and heightened the text input dialog, also added a \"Preview\" button to show how the text will appear on the screen in-game\n");

            WriteBoldText(richTextBox, "\n• Layout\n");
            richTextBox.AppendText(" • Larger scrollbars for easier access when fine-editing a level\n");
            richTextBox.AppendText(" • Theme/style dropdowns widened for easier reading\n");
            richTextBox.AppendText(" • Tabs widened for easier reading\n");
            richTextBox.AppendText(" • Set minimum window size to 900 x 600)\n");
            richTextBox.AppendText(" • \"Clear Backgrounds\" button moved to above the Piece Browser for better access\n");
            richTextBox.AppendText(" • Improved Settings dialog layout\n");
            richTextBox.AppendText(" • Revised toolbar menu layout\n");
            richTextBox.AppendText(" • Updated all menu dropdrowns to display the hotkey to the right\n");
            richTextBox.AppendText(" • All dialogs (Hotkeys, Options, About, Validate Level, etc) now appear center-screen\n");

            // =======================
            // Misc Features
            // =======================
            WriteBoldText(richTextBox, "\n• Miscellaneous\n");

            WriteBoldText(richTextBox, "• Refresh Styles");
            richTextBox.AppendText(" - It's now possible to refresh the styles without closing and re-opening the Editor. So, if a style is modified during a level editing session, it can be refreshed without interrupting workflow! This feature is accessed via a menu item and customizable hotkey (Ctrl+Shift+F8 by default)\n");

            WriteBoldText(richTextBox, "• Save As Image");
            richTextBox.AppendText(" - Added Save As Image option (plus hotkey) to the File menu; this saves a .png image of the currently loaded level\n");

            WriteBoldText(richTextBox, "• Cleanse Levels");
            richTextBox.AppendText(" - Added \"Cleanse Levels\" menu item - this automatically re-saves all levels in a specified pack to ensure compatibility with NL\n");

            WriteBoldText(richTextBox, "• Level Size");
            richTextBox.AppendText(" - Maximum level width increased to 6400px, maximum height decreased to 1600px\n");

            WriteBoldText(richTextBox, "• Maximum Lemmings Count");
            richTextBox.AppendText(" - 999 is now the maximum number of lemmings supported by the Editor; this is to match NL Player skill panel display\n");

            // =======================
            // Bugfixes
            // =======================
            WriteBoldText(richTextBox, "\n• Bugfixes\n");
            WriteBoldText(richTextBox, "• Bugfix - Missing Piece Handling");
            richTextBox.AppendText(" - Levels with missing pieces no longer create multiple popups; instead, a status bar is used to inform the player that the level has missing pieces\n");

            WriteBoldText(richTextBox, "• Bugfixes - UI\n");
            richTextBox.AppendText(" • All secondary windows can now be closed using the [Esc] key\n");
            richTextBox.AppendText(" • Increased minimum selectable grid size to 2px\n");
            richTextBox.AppendText(" • Settings form now stays on top when active\n");
            richTextBox.AppendText(" • Improved mouseover handling for dropdown lists\n");
            richTextBox.AppendText(" • Fixed bug affecting the position of the screen area in relation to the scrollbars when zoomed in\n");
            richTextBox.AppendText(" • Character limits increased to SLX Player UI limits: Title (62), Author (60), Talisman Title (54)\n");
            richTextBox.AppendText(" • Cursor anchor is now correctly preserved when zooming in and out\n");

            WriteBoldText(richTextBox, "• Bugfix - Cursor Zoom");
            richTextBox.AppendText(" - Cursor anchor is now correctly preserved when zooming in and out\n");

            WriteBoldText(richTextBox, "• Bugfix - Preview/Postview Text");
            richTextBox.AppendText(" - Text is now displayed centred for better previewing\n");

            WriteBoldText(richTextBox, "• Bugfix - Flipped/Inverted/Rotated Pieces\n");
            richTextBox.AppendText(" • Fixed trigger area repositionings for flipped/inverted/rotated objects\n");
            richTextBox.AppendText(" • When flipping a hatch horizontally, the Flip Offset value is calculated and written to the level file so the Player can match its position as seen in the Editor)\n");
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
