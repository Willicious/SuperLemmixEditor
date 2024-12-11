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
            SetSubItemNames();
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
            if (System.IO.File.Exists("SLXEditorHotkeys.ini"))
                HotkeyConfig.LoadHotkeysFromIniFile();
            else
                HotkeyConfig.GetDefaultHotkeys();

            LoadHotkeysToListView();

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
            HotkeyConfig.GetDefaultHotkeys();
            LoadHotkeysToListView();
            ResetUI();
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            WriteToHotkeyConfig();
            HotkeyConfig.SaveHotkeysToIniFile();
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
                listViewHotkeys.Focus();

                // Parse the hotkey string back to a Keys value
                Keys assignedKey = HotkeyConfig.ParseHotkeyString(selectedItem.SubItems[1].Text);

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

                // Enable combo box
                comboBoxChooseKey.Enabled = true;
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
                item.SubItems[1].Text = "None";
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

        private void UpdateKey()
        {
            if (listViewHotkeys.SelectedItems.Count > 0)
            {
                // Update the ListView's hotkey column
                selectedItem = listViewHotkeys.SelectedItems[0];
                selectedItem.SubItems[1].Text = HotkeyConfig.FormatHotkeyString(selectedKey);

                ResetUI();
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
                lblChosenHotkey.Text = HotkeyConfig.FormatHotkeyString(selectedKey);
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
                    duplicateItem.BackColor = Color.MistyRose;

                    // Update labels
                    lblActionToBeAssigned.Text = selectedItem.SubItems[0].Text;
                    lblCurrentHotkey.Text = selectedItem.SubItems[1].Text;
                    lblChosenKey.Enabled = true;
                    lblChosenKey.Text = "Chosen Key:";
                    lblChosenHotkey.ForeColor = Color.Red;
                    lblChosenHotkey.Enabled = true;
                    lblChosenHotkey.Visible = true;
                    lblChosenHotkey.Text = HotkeyConfig.FormatHotkeyString(selectedKey);
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

                if (HotkeyConfig.ParseHotkeyString(itemHotkeyText) == hotkey)
                    return item; // Return the duplicate item
            }
            return null; // No duplicates found
        }

        private void SetSubItemNames()
        {
            // Iterate through the items in the ListView
            foreach (ListViewItem item in listViewHotkeys.Items)
            {
                if (item.SubItems.Count > 1)
                {
                    // Set the Name of SubItem[1] to its Text value
                    item.SubItems[1].Name = item.SubItems[1].Text;
                }
            }
        }

        private void LoadHotkeysToListView()
        {
            listViewHotkeys.Items[0]. SubItems[1].Text = HotkeyConfig.FormatHotkeyString(HotkeyConfig.HotkeyCreateNewLevel);
            listViewHotkeys.Items[1]. SubItems[1].Text = HotkeyConfig.FormatHotkeyString(HotkeyConfig.HotkeyLoadLevel);
            listViewHotkeys.Items[2]. SubItems[1].Text = HotkeyConfig.FormatHotkeyString(HotkeyConfig.HotkeySaveLevel);
            listViewHotkeys.Items[3]. SubItems[1].Text = HotkeyConfig.FormatHotkeyString(HotkeyConfig.HotkeySaveLevelAs);
            listViewHotkeys.Items[4]. SubItems[1].Text = HotkeyConfig.FormatHotkeyString(HotkeyConfig.HotkeyPlaytestLevel);
            listViewHotkeys.Items[5]. SubItems[1].Text = HotkeyConfig.FormatHotkeyString(HotkeyConfig.HotkeyValidateLevel);
            listViewHotkeys.Items[6]. SubItems[1].Text = HotkeyConfig.FormatHotkeyString(HotkeyConfig.HotkeyToggleClearPhysics);
            listViewHotkeys.Items[7]. SubItems[1].Text = HotkeyConfig.FormatHotkeyString(HotkeyConfig.HotkeyToggleTerrain);
            listViewHotkeys.Items[8]. SubItems[1].Text = HotkeyConfig.FormatHotkeyString(HotkeyConfig.HotkeyToggleObjects);
            listViewHotkeys.Items[9]. SubItems[1].Text = HotkeyConfig.FormatHotkeyString(HotkeyConfig.HotkeyToggleTriggerAreas);
            listViewHotkeys.Items[10].SubItems[1].Text = HotkeyConfig.FormatHotkeyString(HotkeyConfig.HotkeyToggleScreenStart);
            listViewHotkeys.Items[11].SubItems[1].Text = HotkeyConfig.FormatHotkeyString(HotkeyConfig.HotkeyToggleBackground);
            listViewHotkeys.Items[12].SubItems[1].Text = HotkeyConfig.FormatHotkeyString(HotkeyConfig.HotkeyToggleSnapToGrid);
            listViewHotkeys.Items[13].SubItems[1].Text = HotkeyConfig.FormatHotkeyString(HotkeyConfig.HotkeyOpenSettings);
            listViewHotkeys.Items[14].SubItems[1].Text = HotkeyConfig.FormatHotkeyString(HotkeyConfig.HotkeyOpenConfigHotkeys);
            listViewHotkeys.Items[15].SubItems[1].Text = HotkeyConfig.FormatHotkeyString(HotkeyConfig.HotkeyOpenAboutSLX);
            listViewHotkeys.Items[16].SubItems[1].Text = HotkeyConfig.FormatHotkeyString(HotkeyConfig.HotkeySelectPieces);
            listViewHotkeys.Items[17].SubItems[1].Text = HotkeyConfig.FormatHotkeyString(HotkeyConfig.HotkeyDragToScroll);
            listViewHotkeys.Items[18].SubItems[1].Text = HotkeyConfig.FormatHotkeyString(HotkeyConfig.HotkeyRemovePiecesAtCursor);
            listViewHotkeys.Items[19].SubItems[1].Text = HotkeyConfig.FormatHotkeyString(HotkeyConfig.HotkeyAddRemoveSinglePiece);
            listViewHotkeys.Items[20].SubItems[1].Text = HotkeyConfig.FormatHotkeyString(HotkeyConfig.HotkeySelectPiecesBelow);
            listViewHotkeys.Items[21].SubItems[1].Text = HotkeyConfig.FormatHotkeyString(HotkeyConfig.HotkeyZoomIn);
            listViewHotkeys.Items[22].SubItems[1].Text = HotkeyConfig.FormatHotkeyString(HotkeyConfig.HotkeyZoomOut);
            listViewHotkeys.Items[23].SubItems[1].Text = HotkeyConfig.FormatHotkeyString(HotkeyConfig.HotkeyScrollHorizontally);
            listViewHotkeys.Items[24].SubItems[1].Text = HotkeyConfig.FormatHotkeyString(HotkeyConfig.HotkeyScrollVertically);
            listViewHotkeys.Items[25].SubItems[1].Text = HotkeyConfig.FormatHotkeyString(HotkeyConfig.HotkeyMoveScreenStart);
            listViewHotkeys.Items[26].SubItems[1].Text = HotkeyConfig.FormatHotkeyString(HotkeyConfig.HotkeyShowPreviousPiece);
            listViewHotkeys.Items[27].SubItems[1].Text = HotkeyConfig.FormatHotkeyString(HotkeyConfig.HotkeyShowNextPiece);
            listViewHotkeys.Items[28].SubItems[1].Text = HotkeyConfig.FormatHotkeyString(HotkeyConfig.HotkeyShowPreviousGroup);
            listViewHotkeys.Items[29].SubItems[1].Text = HotkeyConfig.FormatHotkeyString(HotkeyConfig.HotkeyShowNextGroup);
            listViewHotkeys.Items[30].SubItems[1].Text = HotkeyConfig.FormatHotkeyString(HotkeyConfig.HotkeyShowPreviousStyle);
            listViewHotkeys.Items[31].SubItems[1].Text = HotkeyConfig.FormatHotkeyString(HotkeyConfig.HotkeyShowNextStyle);
            listViewHotkeys.Items[32].SubItems[1].Text = HotkeyConfig.FormatHotkeyString(HotkeyConfig.HotkeySwitchBrowser);
            listViewHotkeys.Items[33].SubItems[1].Text = HotkeyConfig.FormatHotkeyString(HotkeyConfig.HotkeyAddPiece1);
            listViewHotkeys.Items[34].SubItems[1].Text = HotkeyConfig.FormatHotkeyString(HotkeyConfig.HotkeyAddPiece2);
            listViewHotkeys.Items[35].SubItems[1].Text = HotkeyConfig.FormatHotkeyString(HotkeyConfig.HotkeyAddPiece3);
            listViewHotkeys.Items[36].SubItems[1].Text = HotkeyConfig.FormatHotkeyString(HotkeyConfig.HotkeyAddPiece4);
            listViewHotkeys.Items[37].SubItems[1].Text = HotkeyConfig.FormatHotkeyString(HotkeyConfig.HotkeyAddPiece5);
            listViewHotkeys.Items[38].SubItems[1].Text = HotkeyConfig.FormatHotkeyString(HotkeyConfig.HotkeyAddPiece6);
            listViewHotkeys.Items[39].SubItems[1].Text = HotkeyConfig.FormatHotkeyString(HotkeyConfig.HotkeyAddPiece7);
            listViewHotkeys.Items[40].SubItems[1].Text = HotkeyConfig.FormatHotkeyString(HotkeyConfig.HotkeyAddPiece8);
            listViewHotkeys.Items[41].SubItems[1].Text = HotkeyConfig.FormatHotkeyString(HotkeyConfig.HotkeyAddPiece9);
            listViewHotkeys.Items[42].SubItems[1].Text = HotkeyConfig.FormatHotkeyString(HotkeyConfig.HotkeyAddPiece10);
            listViewHotkeys.Items[43].SubItems[1].Text = HotkeyConfig.FormatHotkeyString(HotkeyConfig.HotkeyAddPiece11);
            listViewHotkeys.Items[44].SubItems[1].Text = HotkeyConfig.FormatHotkeyString(HotkeyConfig.HotkeyAddPiece12);
            listViewHotkeys.Items[45].SubItems[1].Text = HotkeyConfig.FormatHotkeyString(HotkeyConfig.HotkeyAddPiece13);
            listViewHotkeys.Items[46].SubItems[1].Text = HotkeyConfig.FormatHotkeyString(HotkeyConfig.HotkeyUndo);
            listViewHotkeys.Items[47].SubItems[1].Text = HotkeyConfig.FormatHotkeyString(HotkeyConfig.HotkeyRedo);
            listViewHotkeys.Items[48].SubItems[1].Text = HotkeyConfig.FormatHotkeyString(HotkeyConfig.HotkeyCut);
            listViewHotkeys.Items[49].SubItems[1].Text = HotkeyConfig.FormatHotkeyString(HotkeyConfig.HotkeyCopy);
            listViewHotkeys.Items[50].SubItems[1].Text = HotkeyConfig.FormatHotkeyString(HotkeyConfig.HotkeyPaste);
            listViewHotkeys.Items[51].SubItems[1].Text = HotkeyConfig.FormatHotkeyString(HotkeyConfig.HotkeyDuplicate);
            listViewHotkeys.Items[52].SubItems[1].Text = HotkeyConfig.FormatHotkeyString(HotkeyConfig.HotkeyDelete);
            listViewHotkeys.Items[53].SubItems[1].Text = HotkeyConfig.FormatHotkeyString(HotkeyConfig.HotkeyMoveUp);
            listViewHotkeys.Items[54].SubItems[1].Text = HotkeyConfig.FormatHotkeyString(HotkeyConfig.HotkeyMoveDown);
            listViewHotkeys.Items[55].SubItems[1].Text = HotkeyConfig.FormatHotkeyString(HotkeyConfig.HotkeyMoveLeft);
            listViewHotkeys.Items[56].SubItems[1].Text = HotkeyConfig.FormatHotkeyString(HotkeyConfig.HotkeyMoveRight);
            listViewHotkeys.Items[57].SubItems[1].Text = HotkeyConfig.FormatHotkeyString(HotkeyConfig.HotkeyMove8Up);
            listViewHotkeys.Items[58].SubItems[1].Text = HotkeyConfig.FormatHotkeyString(HotkeyConfig.HotkeyMove8Down);
            listViewHotkeys.Items[59].SubItems[1].Text = HotkeyConfig.FormatHotkeyString(HotkeyConfig.HotkeyMove8Left);
            listViewHotkeys.Items[60].SubItems[1].Text = HotkeyConfig.FormatHotkeyString(HotkeyConfig.HotkeyMove8Right);
            listViewHotkeys.Items[61].SubItems[1].Text = HotkeyConfig.FormatHotkeyString(HotkeyConfig.HotkeyCustomMove);
            listViewHotkeys.Items[62].SubItems[1].Text = HotkeyConfig.FormatHotkeyString(HotkeyConfig.HotkeyDragHorizontally);
            listViewHotkeys.Items[63].SubItems[1].Text = HotkeyConfig.FormatHotkeyString(HotkeyConfig.HotkeyDragVertically);
            listViewHotkeys.Items[64].SubItems[1].Text = HotkeyConfig.FormatHotkeyString(HotkeyConfig.HotkeyRotate);
            listViewHotkeys.Items[65].SubItems[1].Text = HotkeyConfig.FormatHotkeyString(HotkeyConfig.HotkeyFlip);
            listViewHotkeys.Items[66].SubItems[1].Text = HotkeyConfig.FormatHotkeyString(HotkeyConfig.HotkeyInvert);
            listViewHotkeys.Items[67].SubItems[1].Text = HotkeyConfig.FormatHotkeyString(HotkeyConfig.HotkeyGroup);
            listViewHotkeys.Items[68].SubItems[1].Text = HotkeyConfig.FormatHotkeyString(HotkeyConfig.HotkeyUngroup);
            listViewHotkeys.Items[69].SubItems[1].Text = HotkeyConfig.FormatHotkeyString(HotkeyConfig.HotkeyErase);
            listViewHotkeys.Items[70].SubItems[1].Text = HotkeyConfig.FormatHotkeyString(HotkeyConfig.HotkeyNoOverwrite);
            listViewHotkeys.Items[71].SubItems[1].Text = HotkeyConfig.FormatHotkeyString(HotkeyConfig.HotkeyOnlyOnTerrain);
            listViewHotkeys.Items[72].SubItems[1].Text = HotkeyConfig.FormatHotkeyString(HotkeyConfig.HotkeyAllowOneWay);
            listViewHotkeys.Items[73].SubItems[1].Text = HotkeyConfig.FormatHotkeyString(HotkeyConfig.HotkeyDrawLast);
            listViewHotkeys.Items[74].SubItems[1].Text = HotkeyConfig.FormatHotkeyString(HotkeyConfig.HotkeyDrawSooner);
            listViewHotkeys.Items[75].SubItems[1].Text = HotkeyConfig.FormatHotkeyString(HotkeyConfig.HotkeyDrawLater);
            listViewHotkeys.Items[76].SubItems[1].Text = HotkeyConfig.FormatHotkeyString(HotkeyConfig.HotkeyDrawFirst);
            listViewHotkeys.Items[77].SubItems[1].Text = HotkeyConfig.FormatHotkeyString(HotkeyConfig.HotkeyCloseWindow);
            listViewHotkeys.Items[78].SubItems[1].Text = HotkeyConfig.FormatHotkeyString(HotkeyConfig.HotkeyCloseEditor);
        }

        private void WriteToHotkeyConfig()
        {
            foreach (ListViewItem item in listViewHotkeys.Items)
            {
                string subItemName = item.SubItems[1].Name;
                string subItemText = item.SubItems[1].Text;

                // Use the HotkeyConfig.ParseHotkeyString method to convert the subItemText into the appropriate Keys enum
                Keys parsedKey = HotkeyConfig.ParseHotkeyString(subItemText);

                // Assign the parsed key to the correct HotkeyConfig property
                switch (subItemName)
                {
                    case "HotkeyCreateNewLevel":
                        HotkeyConfig.HotkeyCreateNewLevel = parsedKey;
                        break;
                    case "HotkeyLoadLevel":
                        HotkeyConfig.HotkeyLoadLevel = parsedKey;
                        break;
                    case "HotkeySaveLevel":
                        HotkeyConfig.HotkeySaveLevel = parsedKey;
                        break;
                    case "HotkeySaveLevelAs":
                        HotkeyConfig.HotkeySaveLevelAs = parsedKey;
                        break;
                    case "HotkeyPlaytestLevel":
                        HotkeyConfig.HotkeyPlaytestLevel = parsedKey;
                        break;
                    case "HotkeyValidateLevel":
                        HotkeyConfig.HotkeyValidateLevel = parsedKey;
                        break;
                    case "HotkeyToggleClearPhysics":
                        HotkeyConfig.HotkeyToggleClearPhysics = parsedKey;
                        break;
                    case "HotkeyToggleTerrain":
                        HotkeyConfig.HotkeyToggleTerrain = parsedKey;
                        break;
                    case "HotkeyToggleObjects":
                        HotkeyConfig.HotkeyToggleObjects = parsedKey;
                        break;
                    case "HotkeyToggleTriggerAreas":
                        HotkeyConfig.HotkeyToggleTriggerAreas = parsedKey;
                        break;
                    case "HotkeyToggleScreenStart":
                        HotkeyConfig.HotkeyToggleScreenStart = parsedKey;
                        break;
                    case "HotkeyToggleBackground":
                        HotkeyConfig.HotkeyToggleBackground = parsedKey;
                        break;
                    case "HotkeyToggleSnapToGrid":
                        HotkeyConfig.HotkeyToggleSnapToGrid = parsedKey;
                        break;
                    case "HotkeyOpenSettings":
                        HotkeyConfig.HotkeyOpenSettings = parsedKey;
                        break;
                    case "HotkeyOpenConfigHotkeys":
                        HotkeyConfig.HotkeyOpenConfigHotkeys = parsedKey;
                        break;
                    case "HotkeyOpenAboutSLX":
                        HotkeyConfig.HotkeyOpenAboutSLX = parsedKey;
                        break;
                    case "HotkeySelectPieces":
                        HotkeyConfig.HotkeySelectPieces = parsedKey;
                        break;
                    case "HotkeyDragToScroll":
                        HotkeyConfig.HotkeyDragToScroll = parsedKey;
                        break;
                    case "HotkeyRemovePiecesAtCursor":
                        HotkeyConfig.HotkeyRemovePiecesAtCursor = parsedKey;
                        break;
                    case "HotkeyAddRemoveSinglePiece":
                        HotkeyConfig.HotkeyAddRemoveSinglePiece = parsedKey;
                        break;
                    case "HotkeySelectPiecesBelow":
                        HotkeyConfig.HotkeySelectPiecesBelow = parsedKey;
                        break;
                    case "HotkeyZoomIn":
                        HotkeyConfig.HotkeyZoomIn = parsedKey;
                        break;
                    case "HotkeyZoomOut":
                        HotkeyConfig.HotkeyZoomOut = parsedKey;
                        break;
                    case "HotkeyScrollHorizontally":
                        HotkeyConfig.HotkeyScrollHorizontally = parsedKey;
                        break;
                    case "HotkeyScrollVertically":
                        HotkeyConfig.HotkeyScrollVertically = parsedKey;
                        break;
                    case "HotkeyMoveScreenStart":
                        HotkeyConfig.HotkeyMoveScreenStart = parsedKey;
                        break;
                    case "HotkeyShowPreviousPiece":
                        HotkeyConfig.HotkeyShowPreviousPiece = parsedKey;
                        break;
                    case "HotkeyShowNextPiece":
                        HotkeyConfig.HotkeyShowNextPiece = parsedKey;
                        break;
                    case "HotkeyShowPreviousGroup":
                        HotkeyConfig.HotkeyShowPreviousGroup = parsedKey;
                        break;
                    case "HotkeyShowNextGroup":
                        HotkeyConfig.HotkeyShowNextGroup = parsedKey;
                        break;
                    case "HotkeyShowPreviousStyle":
                        HotkeyConfig.HotkeyShowPreviousStyle = parsedKey;
                        break;
                    case "HotkeyShowNextStyle":
                        HotkeyConfig.HotkeyShowNextStyle = parsedKey;
                        break;
                    case "HotkeySwitchBrowser":
                        HotkeyConfig.HotkeySwitchBrowser = parsedKey;
                        break;
                    case "HotkeyAddPiece1":
                        HotkeyConfig.HotkeyAddPiece1 = parsedKey;
                        break;
                    case "HotkeyAddPiece2":
                        HotkeyConfig.HotkeyAddPiece2 = parsedKey;
                        break;
                    case "HotkeyAddPiece3":
                        HotkeyConfig.HotkeyAddPiece3 = parsedKey;
                        break;
                    case "HotkeyAddPiece4":
                        HotkeyConfig.HotkeyAddPiece4 = parsedKey;
                        break;
                    case "HotkeyAddPiece5":
                        HotkeyConfig.HotkeyAddPiece5 = parsedKey;
                        break;
                    case "HotkeyAddPiece6":
                        HotkeyConfig.HotkeyAddPiece6 = parsedKey;
                        break;
                    case "HotkeyAddPiece7":
                        HotkeyConfig.HotkeyAddPiece7 = parsedKey;
                        break;
                    case "HotkeyAddPiece8":
                        HotkeyConfig.HotkeyAddPiece8 = parsedKey;
                        break;
                    case "HotkeyAddPiece9":
                        HotkeyConfig.HotkeyAddPiece9 = parsedKey;
                        break;
                    case "HotkeyAddPiece10":
                        HotkeyConfig.HotkeyAddPiece10 = parsedKey;
                        break;
                    case "HotkeyAddPiece11":
                        HotkeyConfig.HotkeyAddPiece11 = parsedKey;
                        break;
                    case "HotkeyAddPiece12":
                        HotkeyConfig.HotkeyAddPiece12 = parsedKey;
                        break;
                    case "HotkeyAddPiece13":
                        HotkeyConfig.HotkeyAddPiece13 = parsedKey;
                        break;
                    case "HotkeyUndo":
                        HotkeyConfig.HotkeyUndo = parsedKey;
                        break;
                    case "HotkeyRedo":
                        HotkeyConfig.HotkeyRedo = parsedKey;
                        break;
                    case "HotkeyCut":
                        HotkeyConfig.HotkeyCut = parsedKey;
                        break;
                    case "HotkeyCopy":
                        HotkeyConfig.HotkeyCopy = parsedKey;
                        break;
                    case "HotkeyPaste":
                        HotkeyConfig.HotkeyPaste = parsedKey;
                        break;
                    case "HotkeyDuplicate":
                        HotkeyConfig.HotkeyDuplicate = parsedKey;
                        break;
                    case "HotkeyDelete":
                        HotkeyConfig.HotkeyDelete = parsedKey;
                        break;
                    case "HotkeyMoveUp":
                        HotkeyConfig.HotkeyMoveUp = parsedKey;
                        break;
                    case "HotkeyMoveDown":
                        HotkeyConfig.HotkeyMoveDown = parsedKey;
                        break;
                    case "HotkeyMoveLeft":
                        HotkeyConfig.HotkeyMoveLeft = parsedKey;
                        break;
                    case "HotkeyMoveRight":
                        HotkeyConfig.HotkeyMoveRight = parsedKey;
                        break;
                    case "HotkeyMove8Up":
                        HotkeyConfig.HotkeyMove8Up = parsedKey;
                        break;
                    case "HotkeyMove8Down":
                        HotkeyConfig.HotkeyMove8Down = parsedKey;
                        break;
                    case "HotkeyMove8Left":
                        HotkeyConfig.HotkeyMove8Left = parsedKey;
                        break;
                    case "HotkeyMove8Right":
                        HotkeyConfig.HotkeyMove8Right = parsedKey;
                        break;
                    case "HotkeyCustomMove":
                        HotkeyConfig.HotkeyCustomMove = parsedKey;
                        break;
                    case "HotkeyDragHorizontally":
                        HotkeyConfig.HotkeyDragHorizontally = parsedKey;
                        break;
                    case "HotkeyDragVertically":
                        HotkeyConfig.HotkeyDragVertically = parsedKey;
                        break;
                    case "HotkeyRotate":
                        HotkeyConfig.HotkeyRotate = parsedKey;
                        break;
                    case "HotkeyFlip":
                        HotkeyConfig.HotkeyFlip = parsedKey;
                        break;
                    case "HotkeyInvert":
                        HotkeyConfig.HotkeyInvert = parsedKey;
                        break;
                    case "HotkeyGroup":
                        HotkeyConfig.HotkeyGroup = parsedKey;
                        break;
                    case "HotkeyUngroup":
                        HotkeyConfig.HotkeyUngroup = parsedKey;
                        break;
                    case "HotkeyErase":
                        HotkeyConfig.HotkeyErase = parsedKey;
                        break;
                    case "HotkeyNoOverwrite":
                        HotkeyConfig.HotkeyNoOverwrite = parsedKey;
                        break;
                    case "HotkeyOnlyOnTerrain":
                        HotkeyConfig.HotkeyOnlyOnTerrain = parsedKey;
                        break;
                    case "HotkeyAllowOneWay":
                        HotkeyConfig.HotkeyAllowOneWay = parsedKey;
                        break;
                    case "HotkeyDrawLast":
                        HotkeyConfig.HotkeyDrawLast = parsedKey;
                        break;
                    case "HotkeyDrawSooner":
                        HotkeyConfig.HotkeyDrawSooner = parsedKey;
                        break;
                    case "HotkeyDrawLater":
                        HotkeyConfig.HotkeyDrawLater = parsedKey;
                        break;
                    case "HotkeyDrawFirst":
                        HotkeyConfig.HotkeyDrawFirst = parsedKey;
                        break;
                    case "HotkeyCloseWindow":
                        HotkeyConfig.HotkeyCloseWindow = parsedKey;
                        break;
                    case "HotkeyCloseEditor":
                        HotkeyConfig.HotkeyCloseEditor = parsedKey;
                        break;
                }
            }
        }
    }
}