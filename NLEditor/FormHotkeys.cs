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
    public partial class FormHotkeys : Form
    {
        public FormHotkeys()
        {
            InitializeComponent();
        }

        private Keys listenedKey;
        private Keys selectedKey;
        private ListViewItem selectedItem;
        private bool DoCheckForDuplicates = true;

        private void FormHotkeys_Load(object sender, EventArgs e)
        {
            comboBoxChooseKey.DataSource = Enum.GetValues(typeof(Keys)).Cast<Keys>().ToList();
        }

        private void FormHotkeys_Shown(object sender, EventArgs e)
        {
            // Load hotkeys
            LoadDefaultHotkeysToListView();
            LoadHotkeysFromIniFile();

            // Ensure the list is selected and focused when the Form is shown
            AutoSelectListItem();
        }

        private void listViewHotkeys_Click(object sender, EventArgs e)
        {
            ResetUI();
        }

        private void listViewHotkeys_SelectedIndexChanged(object sender, EventArgs e)
        {
            ResetUI();
        }

        private void comboBoxChooseKey_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBoxChooseKey.SelectedItem is Keys key)
            {
                ClearHighlights();

                selectedKey = key;
                CheckForDuplicateKeys();
            }
        }

        private void comboBoxChooseKey_KeyDown(object sender, KeyEventArgs e)
        {
            // Handle Enter key press to validate key
            if (e.KeyCode == Keys.Enter)
            {
                string enteredKeyText = comboBoxChooseKey.Text.Trim(); // Get the text from the ComboBox input
                if (Enum.TryParse(enteredKeyText, true, out Keys parsedKey)) // Try parsing the string into a Keys enum value
                {
                    ClearHighlights();
                    selectedKey = parsedKey; // Update the selected key

                    comboBoxChooseKey.SelectedItem = parsedKey; // Select the key in the ComboBox if valid
                    CheckForDuplicateKeys();
                }
                else
                {
                    MessageBox.Show($"Invalid key name: '{enteredKeyText}'", "Invalid Input", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }

                e.Handled = true; // Prevent default behavior for Enter
            }
        }

        private void btnListen_Click(object sender, EventArgs e)
        {
            ClearHighlights();

            // Enable key listening
            KeyPreview = true;
            KeyDown += FormHotkeys_KeyDown;

            // Disable combo box
            comboBoxChooseKey.Enabled = false;

            // Update labels
            lblActionToBeAssigned.Text = selectedItem.SubItems[0].Text;
            lblCurrentHotkey.Text = selectedItem.SubItems[1].Text;
            lblChosenKey.Enabled = true;
            lblChosenKey.Text = "Listening for key...";
            lblChosenHotkey.Enabled = false;
            lblChosenHotkey.Visible = false;
            lblChosenHotkey.Text = "";
            lblDuplicateDetected.Visible = false;
            lblDuplicateAction.Visible = false;
            lblDuplicateAction.Text = "";

            // Enable Cancel button
            btnCancel.Enabled = true;
        }

        private void FormHotkeys_KeyDown(object sender, KeyEventArgs e)
        {
            ClearHighlights();

            // Capture the pressed key, but clear existing modifiers for initial processing
            listenedKey = e.KeyData;
            checkModCtrl.Checked = false;
            checkModShift.Checked = false;
            checkModAlt.Checked = false;

            comboBoxChooseKey.SelectedItem = listenedKey;

            // Disable key listening
            KeyPreview = false;
            KeyDown -= FormHotkeys_KeyDown;

            // Enable combo box
            comboBoxChooseKey.Enabled = true;

            selectedKey = listenedKey;
            CheckForDuplicateKeys();
        }

        private void checkModifiers_Click(object sender, EventArgs e)
        {
            CheckForDuplicateKeys();
        }

        private void btnAssignChosenKey_Click(object sender, EventArgs e)
        {
            UpdateSelectedHotkey();
            UpdateKey();
        }

        private void btnClearAllKeys_Click(object sender, EventArgs e)
        {
            ClearAllKeys();
            ResetUI();
        }

        private void btnResetToDefaultKeys_Click(object sender, EventArgs e)
        {
            LoadDefaultHotkeysToListView();
            ResetUI();
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            SaveHotkeysToIniFile();
            MessageBox.Show("Hotkeys saved successfully!", "Hotkey Config", MessageBoxButtons.OK, MessageBoxIcon.Information);
            ResetUI();
        }


        private void btnCancel_Click(object sender, EventArgs e)
        {
            ResetUI();
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void ResetUI()
        {
            DoCheckForDuplicates = false;
            
            if (listViewHotkeys.SelectedItems.Count > 0)
            {
                ClearHighlights();
                selectedItem = listViewHotkeys.SelectedItems[0];
                selectedItem.EnsureVisible();

                // Parse the hotkey string back to a Keys value
                Keys assignedKey = ParseHotkeyString(selectedItem.SubItems[1].Text);

                // Set the combo box to the base key
                Keys baseKey = assignedKey & ~(Keys.Control | Keys.Shift | Keys.Alt);
                comboBoxChooseKey.SelectedItem = baseKey;

                // Update the modifier checkboxes
                checkModCtrl.Checked = assignedKey.HasFlag(Keys.Control);
                checkModShift.Checked = assignedKey.HasFlag(Keys.Shift);
                checkModAlt.Checked = assignedKey.HasFlag(Keys.Alt);

                // Update labels
                lblActionToBeAssigned.Text = selectedItem.SubItems[0].Text;
                lblCurrentHotkey.Text = selectedItem.SubItems[1].Text;
                lblChosenKey.Enabled = false;
                lblChosenHotkey.Enabled = false;
                lblChosenHotkey.Visible = false;
                lblChosenHotkey.Text = "";
                lblDuplicateDetected.Visible = false;
                lblDuplicateAction.Visible = false;
                lblDuplicateAction.Text = "";

                // Disable Assign and Cancel buttons
                btnAssignChosenKey.Enabled = false;
                btnCancel.Enabled = false;
            }

            DoCheckForDuplicates = true;
        }

        private void AutoSelectListItem()
        {
            // Ensure the first ListView item is selected
            if (listViewHotkeys.Items.Count > 0)
            {
                listViewHotkeys.Items[0].Selected = true;
                listViewHotkeys.Focus();
            }
        }

        private void ClearAllKeys()
        {
            foreach (ListViewItem item in listViewHotkeys.Items)
            {
                item.SubItems[1].Text = "";
            }

            // Update labels
            lblActionToBeAssigned.Text = selectedItem.SubItems[0].Text;
            lblCurrentHotkey.Text = selectedItem.SubItems[1].Text;
            lblChosenKey.Enabled = false;
            lblChosenHotkey.Enabled = false;
            lblChosenHotkey.Visible = false;
            lblChosenHotkey.Text = "";
            lblDuplicateDetected.Visible = false;
            lblDuplicateAction.Visible = false;
            lblDuplicateAction.Text = "";

            // Disable Assign and Cancel buttons
            btnAssignChosenKey.Enabled = false;
            btnCancel.Enabled = false;
        }

        private void ClearHighlights()
        {
            foreach (ListViewItem item in listViewHotkeys.Items)
            {
                item.BackColor = SystemColors.Window;
            }
        }

        private void UpdateSelectedHotkey()
        {
            if (listViewHotkeys.SelectedItems.Count > 0)
            {
                selectedItem = listViewHotkeys.SelectedItems[0];

                // Update labels
                lblActionToBeAssigned.Text = selectedItem.SubItems[0].Text;
                lblCurrentHotkey.Text = selectedItem.SubItems[1].Text;
                lblChosenKey.Enabled = true;
                lblChosenKey.Text = "Chosen Key:";
                lblChosenHotkey.ForeColor = Color.MediumSeaGreen;
                lblChosenHotkey.Enabled = true;
                lblChosenHotkey.Visible = true;
                lblChosenHotkey.Text = FormatHotkeyString(selectedKey);
                lblDuplicateDetected.Visible = false;
                lblDuplicateAction.Visible = false;
                lblDuplicateAction.Text = "";

                // Enable components
                btnAssignChosenKey.Enabled = true;
                comboBoxChooseKey.Enabled = true;
                btnCancel.Enabled = true;

                if (lblChosenHotkey.Text == lblCurrentHotkey.Text)
                    ResetUI();
            }
        }

        private void CheckForDuplicateKeys()
        {
            if (!DoCheckForDuplicates) return;
            
            if (selectedItem != null)
            {
                // Combine the key with modifiers, if selected
                selectedKey &= ~(Keys.Control | Keys.Shift | Keys.Alt); // Clear existing modifiers
                if (checkModCtrl.Checked) selectedKey |= Keys.Control;
                if (checkModShift.Checked) selectedKey |= Keys.Shift;
                if (checkModAlt.Checked) selectedKey |= Keys.Alt;

                // Check for duplicate hotkeys
                var duplicateItem = FindDuplicateHotkey(selectedKey, listViewHotkeys, selectedItem);
                if (duplicateItem != null)
                {
                    // Highlight the duplicate item in the ListView
                    listViewHotkeys.Focus();
                    duplicateItem.EnsureVisible();
                    duplicateItem.BackColor = Color.LightYellow;

                    // Update labels
                    lblActionToBeAssigned.Text = selectedItem.SubItems[0].Text;
                    lblCurrentHotkey.Text = selectedItem.SubItems[1].Text;
                    lblChosenKey.Enabled = true;
                    lblChosenKey.Text = "Chosen Key:";
                    lblChosenHotkey.ForeColor = Color.Red;
                    lblChosenHotkey.Enabled = true;
                    lblChosenHotkey.Visible = true;
                    lblChosenHotkey.Text = FormatHotkeyString(selectedKey);
                    lblDuplicateDetected.Visible = true;
                    lblDuplicateAction.Visible = true;
                    lblDuplicateAction.Text = duplicateItem.SubItems[0].Text;

                    // Disable Assign button and Enable Cancel button
                    btnAssignChosenKey.Enabled = false;
                    btnCancel.Enabled = true;
                }
                else
                {
                    UpdateSelectedHotkey();
                }
            }
        }

        private ListViewItem FindDuplicateHotkey(Keys hotkey, ListView listView, ListViewItem currentItem)
        {
            foreach (ListViewItem item in listView.Items)
            {
                if (item == currentItem) continue; // Skip the current item being edited

                string itemHotkeyText = item.SubItems[1].Text;

                // Skip if the hotkey string is empty or blank
                if (string.IsNullOrWhiteSpace(itemHotkeyText)) continue;

                if (ParseHotkeyString(itemHotkeyText) == hotkey)
                    return item; // Return the duplicate item
            }
            return null; // No duplicates found
        }

        private string FormatHotkeyString(Keys hotkey)
        {
            List<string> hotkeyParts = new List<string>();

            if (hotkey.HasFlag(Keys.Control)) hotkeyParts.Add("Ctrl");
            if (hotkey.HasFlag(Keys.Shift)) hotkeyParts.Add("Shift");
            if (hotkey.HasFlag(Keys.Alt)) hotkeyParts.Add("Alt");

            // Get the base key from the flags
            Keys baseKey = hotkey & ~(Keys.Control | Keys.Shift | Keys.Alt);
            hotkeyParts.Add(baseKey.ToString());

            return string.Join("+", hotkeyParts);
        }

        private Keys ParseHotkeyString(string hotkeyString)
        {
            Keys result = Keys.None;
            string[] parts = hotkeyString.Split('+');

            foreach (string part in parts)
            {
                switch (part.Trim())
                {
                    case "Ctrl":
                        result |= Keys.Control;
                        break;
                    case "Shift":
                        result |= Keys.Shift;
                        break;
                    case "Alt":
                        result |= Keys.Alt;
                        break;
                    default:
                        if (Enum.TryParse(part, out Keys key))
                            result |= key;
                        break;
                }
            }

            return result;
        }

        private void LoadDefaultHotkeysToListView()
        {
            listViewHotkeys.Items[0]. SubItems[1].Text = FormatHotkeyString(HotkeyConfig.HotkeyCreateNewLevel);
            listViewHotkeys.Items[1]. SubItems[1].Text = FormatHotkeyString(HotkeyConfig.HotkeyLoadLevel);
            listViewHotkeys.Items[2]. SubItems[1].Text = FormatHotkeyString(HotkeyConfig.HotkeySaveLevel);
            listViewHotkeys.Items[3]. SubItems[1].Text = FormatHotkeyString(HotkeyConfig.HotkeySaveLevelAs);
            listViewHotkeys.Items[4]. SubItems[1].Text = FormatHotkeyString(HotkeyConfig.HotkeyPlaytestLevel);
            listViewHotkeys.Items[5]. SubItems[1].Text = FormatHotkeyString(HotkeyConfig.HotkeyValidateLevel);
            listViewHotkeys.Items[6]. SubItems[1].Text = FormatHotkeyString(HotkeyConfig.HotkeyToggleClearPhysics);
            listViewHotkeys.Items[7]. SubItems[1].Text = FormatHotkeyString(HotkeyConfig.HotkeyToggleTerrain);
            listViewHotkeys.Items[8]. SubItems[1].Text = FormatHotkeyString(HotkeyConfig.HotkeyToggleObjects);
            listViewHotkeys.Items[9]. SubItems[1].Text = FormatHotkeyString(HotkeyConfig.HotkeyToggleTriggerAreas);
            listViewHotkeys.Items[10].SubItems[1].Text = FormatHotkeyString(HotkeyConfig.HotkeyToggleScreenStart);
            listViewHotkeys.Items[11].SubItems[1].Text = FormatHotkeyString(HotkeyConfig.HotkeyToggleBackground);
            listViewHotkeys.Items[12].SubItems[1].Text = FormatHotkeyString(HotkeyConfig.HotkeyToggleSnapToGrid);
            listViewHotkeys.Items[13].SubItems[1].Text = FormatHotkeyString(HotkeyConfig.HotkeyOpenSettings);
            listViewHotkeys.Items[14].SubItems[1].Text = FormatHotkeyString(HotkeyConfig.HotkeyOpenHotkeyConfig);
            listViewHotkeys.Items[15].SubItems[1].Text = FormatHotkeyString(HotkeyConfig.HotkeyOpenAboutSLX);
            listViewHotkeys.Items[16].SubItems[1].Text = FormatHotkeyString(HotkeyConfig.HotkeySelectPieces);
            listViewHotkeys.Items[17].SubItems[1].Text = FormatHotkeyString(HotkeyConfig.HotkeyDragToScroll);
            listViewHotkeys.Items[18].SubItems[1].Text = FormatHotkeyString(HotkeyConfig.HotkeyRemovePiecesAtCursor);
            listViewHotkeys.Items[19].SubItems[1].Text = FormatHotkeyString(HotkeyConfig.HotkeyAddRemoveSinglePiece);
            listViewHotkeys.Items[20].SubItems[1].Text = FormatHotkeyString(HotkeyConfig.HotkeySelectPiecesBelow);
            listViewHotkeys.Items[21].SubItems[1].Text = FormatHotkeyString(HotkeyConfig.HotkeyZoomIn);
            listViewHotkeys.Items[22].SubItems[1].Text = FormatHotkeyString(HotkeyConfig.HotkeyZoomOut);
            listViewHotkeys.Items[23].SubItems[1].Text = FormatHotkeyString(HotkeyConfig.HotkeyScrollHorizontally);
            listViewHotkeys.Items[24].SubItems[1].Text = FormatHotkeyString(HotkeyConfig.HotkeyScrollVertically);
            listViewHotkeys.Items[25].SubItems[1].Text = FormatHotkeyString(HotkeyConfig.HotkeyMoveScreenStart);
            listViewHotkeys.Items[26].SubItems[1].Text = FormatHotkeyString(HotkeyConfig.HotkeyShowPreviousPiece);
            listViewHotkeys.Items[27].SubItems[1].Text = FormatHotkeyString(HotkeyConfig.HotkeyShowNextPiece);
            listViewHotkeys.Items[28].SubItems[1].Text = FormatHotkeyString(HotkeyConfig.HotkeyShowPreviousGroup);
            listViewHotkeys.Items[29].SubItems[1].Text = FormatHotkeyString(HotkeyConfig.HotkeyShowNextGroup);
            listViewHotkeys.Items[30].SubItems[1].Text = FormatHotkeyString(HotkeyConfig.HotkeyShowPreviousStyle);
            listViewHotkeys.Items[31].SubItems[1].Text = FormatHotkeyString(HotkeyConfig.HotkeyShowNextStyle);
            listViewHotkeys.Items[32].SubItems[1].Text = FormatHotkeyString(HotkeyConfig.HotkeySwitchBrowser);
            listViewHotkeys.Items[33].SubItems[1].Text = FormatHotkeyString(HotkeyConfig.HotkeyAddPiece1);
            listViewHotkeys.Items[34].SubItems[1].Text = FormatHotkeyString(HotkeyConfig.HotkeyAddPiece2);
            listViewHotkeys.Items[35].SubItems[1].Text = FormatHotkeyString(HotkeyConfig.HotkeyAddPiece3);
            listViewHotkeys.Items[36].SubItems[1].Text = FormatHotkeyString(HotkeyConfig.HotkeyAddPiece4);
            listViewHotkeys.Items[37].SubItems[1].Text = FormatHotkeyString(HotkeyConfig.HotkeyAddPiece5);
            listViewHotkeys.Items[38].SubItems[1].Text = FormatHotkeyString(HotkeyConfig.HotkeyAddPiece6);
            listViewHotkeys.Items[39].SubItems[1].Text = FormatHotkeyString(HotkeyConfig.HotkeyAddPiece7);
            listViewHotkeys.Items[40].SubItems[1].Text = FormatHotkeyString(HotkeyConfig.HotkeyAddPiece8);
            listViewHotkeys.Items[41].SubItems[1].Text = FormatHotkeyString(HotkeyConfig.HotkeyAddPiece9);
            listViewHotkeys.Items[42].SubItems[1].Text = FormatHotkeyString(HotkeyConfig.HotkeyAddPiece10);
            listViewHotkeys.Items[43].SubItems[1].Text = FormatHotkeyString(HotkeyConfig.HotkeyAddPiece11);
            listViewHotkeys.Items[44].SubItems[1].Text = FormatHotkeyString(HotkeyConfig.HotkeyAddPiece12);
            listViewHotkeys.Items[45].SubItems[1].Text = FormatHotkeyString(HotkeyConfig.HotkeyAddPiece13);
            listViewHotkeys.Items[46].SubItems[1].Text = FormatHotkeyString(HotkeyConfig.HotkeyUndo);
            listViewHotkeys.Items[47].SubItems[1].Text = FormatHotkeyString(HotkeyConfig.HotkeyRedo);
            listViewHotkeys.Items[48].SubItems[1].Text = FormatHotkeyString(HotkeyConfig.HotkeyCut);
            listViewHotkeys.Items[49].SubItems[1].Text = FormatHotkeyString(HotkeyConfig.HotkeyCopy);
            listViewHotkeys.Items[50].SubItems[1].Text = FormatHotkeyString(HotkeyConfig.HotkeyPaste);
            listViewHotkeys.Items[51].SubItems[1].Text = FormatHotkeyString(HotkeyConfig.HotkeyDuplicate);
            listViewHotkeys.Items[52].SubItems[1].Text = FormatHotkeyString(HotkeyConfig.HotkeyDelete);
            listViewHotkeys.Items[53].SubItems[1].Text = FormatHotkeyString(HotkeyConfig.HotkeyMoveUp);
            listViewHotkeys.Items[54].SubItems[1].Text = FormatHotkeyString(HotkeyConfig.HotkeyMoveDown);
            listViewHotkeys.Items[55].SubItems[1].Text = FormatHotkeyString(HotkeyConfig.HotkeyMoveLeft);
            listViewHotkeys.Items[56].SubItems[1].Text = FormatHotkeyString(HotkeyConfig.HotkeyMoveRight);
            listViewHotkeys.Items[57].SubItems[1].Text = FormatHotkeyString(HotkeyConfig.HotkeyMove8Up);
            listViewHotkeys.Items[58].SubItems[1].Text = FormatHotkeyString(HotkeyConfig.HotkeyMove8Down);
            listViewHotkeys.Items[59].SubItems[1].Text = FormatHotkeyString(HotkeyConfig.HotkeyMove8Left);
            listViewHotkeys.Items[60].SubItems[1].Text = FormatHotkeyString(HotkeyConfig.HotkeyMove8Right);
            listViewHotkeys.Items[61].SubItems[1].Text = FormatHotkeyString(HotkeyConfig.HotkeyCustomMove);
            listViewHotkeys.Items[62].SubItems[1].Text = FormatHotkeyString(HotkeyConfig.HotkeyDragHorizontally);
            listViewHotkeys.Items[63].SubItems[1].Text = FormatHotkeyString(HotkeyConfig.HotkeyDragVertically);
            listViewHotkeys.Items[64].SubItems[1].Text = FormatHotkeyString(HotkeyConfig.HotkeyRotate);
            listViewHotkeys.Items[65].SubItems[1].Text = FormatHotkeyString(HotkeyConfig.HotkeyFlip);
            listViewHotkeys.Items[66].SubItems[1].Text = FormatHotkeyString(HotkeyConfig.HotkeyInvert);
            listViewHotkeys.Items[67].SubItems[1].Text = FormatHotkeyString(HotkeyConfig.HotkeyGroup);
            listViewHotkeys.Items[68].SubItems[1].Text = FormatHotkeyString(HotkeyConfig.HotkeyUngroup);
            listViewHotkeys.Items[69].SubItems[1].Text = FormatHotkeyString(HotkeyConfig.HotkeyErase);
            listViewHotkeys.Items[70].SubItems[1].Text = FormatHotkeyString(HotkeyConfig.HotkeyNoOverwrite);
            listViewHotkeys.Items[71].SubItems[1].Text = FormatHotkeyString(HotkeyConfig.HotkeyOnlyOnTerrain);
            listViewHotkeys.Items[72].SubItems[1].Text = FormatHotkeyString(HotkeyConfig.HotkeyAllowOneWay);
            listViewHotkeys.Items[73].SubItems[1].Text = FormatHotkeyString(HotkeyConfig.HotkeyDrawLast);
            listViewHotkeys.Items[74].SubItems[1].Text = FormatHotkeyString(HotkeyConfig.HotkeyDrawSooner);
            listViewHotkeys.Items[75].SubItems[1].Text = FormatHotkeyString(HotkeyConfig.HotkeyDrawLater);
            listViewHotkeys.Items[76].SubItems[1].Text = FormatHotkeyString(HotkeyConfig.HotkeyDrawFirst);
            listViewHotkeys.Items[77].SubItems[1].Text = FormatHotkeyString(HotkeyConfig.HotkeyCloseWindow);
            listViewHotkeys.Items[78].SubItems[1].Text = FormatHotkeyString(HotkeyConfig.HotkeyCloseEditor);
        }

        private void UpdateKey()
        {
            if (listViewHotkeys.SelectedItems.Count > 0)
            {
                // Update the ListView's hotkey column
                selectedItem = listViewHotkeys.SelectedItems[0];
                selectedItem.SubItems[1].Text = FormatHotkeyString(selectedKey);

                ResetUI();
            }

            //if (selectedItem.Text == "Toggle Screen Start")
            //    HotkeyConfig.ToggleScreenStartHotkey = selectedKey;
            //else if (selectedItem.Text == "Toggle Screen Start")
            //    HotkeyConfig.ToggleScreenStartHotkey = selectedKey;
            //else if (selectedItem.Text == "Toggle Screen Start")
            //    HotkeyConfig.ToggleScreenStartHotkey = selectedKey;
            //else if (selectedItem.Text == "Toggle Screen Start")
            //    HotkeyConfig.ToggleScreenStartHotkey = selectedKey;
            //else if (selectedItem.Text == "Toggle Screen Start")
            //    HotkeyConfig.ToggleScreenStartHotkey = selectedKey;
        }

        private void SaveHotkeysToIniFile()
        {
            var lines = new List<string>
            {
                //"[Hotkeys]",
                //$"ToggleScreenStart={HotkeyConfig.ToggleScreenStartHotkey}",
                //$"ToggleScreenStart={HotkeyConfig.ToggleScreenStartHotkey}",
                //$"ToggleScreenStart={HotkeyConfig.ToggleScreenStartHotkey}",
                //$"ToggleScreenStart={HotkeyConfig.ToggleScreenStartHotkey}",
                //$"ToggleScreenStart={HotkeyConfig.ToggleScreenStartHotkey}",
            };

            System.IO.File.WriteAllLines("SLXEditorHotkeys.ini", lines);
        }

        private void LoadHotkeysFromIniFile()
        {
            if (System.IO.File.Exists("SLXEditorHotkeys.ini"))
            {
                var lines = System.IO.File.ReadAllLines("SLXEditorHotkeys.ini");
                foreach (var line in lines)
                {
                    //if (line.StartsWith("ToggleScreenStart="))
                    //    HotkeyConfig.ToggleScreenStartHotkey = (Keys)Enum.Parse(typeof(Keys), line.Split('=')[1]);
                    //if (line.StartsWith("ToggleScreenStart="))
                    //    HotkeyConfig.ToggleScreenStartHotkey = (Keys)Enum.Parse(typeof(Keys), line.Split('=')[1]);
                    //if (line.StartsWith("ToggleScreenStart="))
                    //    HotkeyConfig.ToggleScreenStartHotkey = (Keys)Enum.Parse(typeof(Keys), line.Split('=')[1]);
                    //if (line.StartsWith("ToggleScreenStart="))
                    //    HotkeyConfig.ToggleScreenStartHotkey = (Keys)Enum.Parse(typeof(Keys), line.Split('=')[1]);
                    //if (line.StartsWith("ToggleScreenStart="))
                    //    HotkeyConfig.ToggleScreenStartHotkey = (Keys)Enum.Parse(typeof(Keys), line.Split('=')[1]);
                }
            }
        }
    }
}