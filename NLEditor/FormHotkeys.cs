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
                LoadHotkeysFromIniFile();
            else
                GetDefaultHotkeys();
            
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
            GetDefaultHotkeys();
            LoadHotkeysToListView();
            ResetUI();
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            WriteToHotkeyConfig();
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
                listViewHotkeys.Focus();

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

        private void UpdateKey()
        {
            if (listViewHotkeys.SelectedItems.Count > 0)
            {
                // Update the ListView's hotkey column
                selectedItem = listViewHotkeys.SelectedItems[0];
                selectedItem.SubItems[1].Text = FormatHotkeyString(selectedKey);

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
                    duplicateItem.BackColor = Color.MistyRose;

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

        private void WriteToHotkeyConfig()
        {
            foreach (ListViewItem item in listViewHotkeys.Items)
            {
                string subItemName = item.SubItems[1].Name;
                string subItemText = item.SubItems[1].Text;

                // Use the ParseHotkeyString method to convert the subItemText into the appropriate Keys enum
                Keys parsedKey = ParseHotkeyString(subItemText);

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
                    case "HotkeyOpenHotkeyConfig":
                        HotkeyConfig.HotkeyOpenHotkeyConfig = parsedKey;
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

        private void SaveHotkeysToIniFile()
        {
            var lines = new List<string>
            {
                "[Hotkeys]",
                $"HotkeyCreateNewLevel={HotkeyConfig.HotkeyCreateNewLevel}",
                $"HotkeyLoadLevel={HotkeyConfig.HotkeyLoadLevel}",
                $"HotkeySaveLevel={HotkeyConfig.HotkeySaveLevel}",
                $"HotkeySaveLevelAs={HotkeyConfig.HotkeySaveLevelAs}",
                $"HotkeyPlaytestLevel={HotkeyConfig.HotkeyPlaytestLevel}",
                $"HotkeyValidateLevel={HotkeyConfig.HotkeyValidateLevel}",
                $"HotkeyToggleClearPhysics={HotkeyConfig.HotkeyToggleClearPhysics}",
                $"HotkeyToggleTerrain={HotkeyConfig.HotkeyToggleTerrain}",
                $"HotkeyToggleObjects={HotkeyConfig.HotkeyToggleObjects}",
                $"HotkeyToggleTriggerAreas={HotkeyConfig.HotkeyToggleTriggerAreas}",
                $"HotkeyToggleScreenStart={HotkeyConfig.HotkeyToggleScreenStart}",
                $"HotkeyToggleBackground={HotkeyConfig.HotkeyToggleBackground}",
                $"HotkeyToggleSnapToGrid={HotkeyConfig.HotkeyToggleSnapToGrid}",
                $"HotkeyOpenSettings={HotkeyConfig.HotkeyOpenSettings}",
                $"HotkeyOpenHotkeyConfig={HotkeyConfig.HotkeyOpenHotkeyConfig}",
                $"HotkeyOpenAboutSLX={HotkeyConfig.HotkeyOpenAboutSLX}",
                $"HotkeySelectPieces={HotkeyConfig.HotkeySelectPieces}",
                $"HotkeyDragToScroll={HotkeyConfig.HotkeyDragToScroll}",
                $"HotkeyRemovePiecesAtCursor={HotkeyConfig.HotkeyRemovePiecesAtCursor}",
                $"HotkeyAddRemoveSinglePiece={HotkeyConfig.HotkeyAddRemoveSinglePiece}",
                $"HotkeySelectPiecesBelow={HotkeyConfig.HotkeySelectPiecesBelow}",
                $"HotkeyZoomIn={HotkeyConfig.HotkeyZoomIn}",
                $"HotkeyZoomOut={HotkeyConfig.HotkeyZoomOut}",
                $"HotkeyScrollHorizontally={HotkeyConfig.HotkeyScrollHorizontally}",
                $"HotkeyScrollVertically={HotkeyConfig.HotkeyScrollVertically}",
                $"HotkeyMoveScreenStart={HotkeyConfig.HotkeyMoveScreenStart}",
                $"HotkeyShowPreviousPiece={HotkeyConfig.HotkeyShowPreviousPiece}",
                $"HotkeyShowNextPiece={HotkeyConfig.HotkeyShowNextPiece}",
                $"HotkeyShowPreviousGroup={HotkeyConfig.HotkeyShowPreviousGroup}",
                $"HotkeyShowNextGroup={HotkeyConfig.HotkeyShowNextGroup}",
                $"HotkeyShowPreviousStyle={HotkeyConfig.HotkeyShowPreviousStyle}",
                $"HotkeyShowNextStyle={HotkeyConfig.HotkeyShowNextStyle}",
                $"HotkeySwitchBrowser={HotkeyConfig.HotkeySwitchBrowser}",
                $"HotkeyAddPiece1={HotkeyConfig.HotkeyAddPiece1}",
                $"HotkeyAddPiece2={HotkeyConfig.HotkeyAddPiece2}",
                $"HotkeyAddPiece3={HotkeyConfig.HotkeyAddPiece3}",
                $"HotkeyAddPiece4={HotkeyConfig.HotkeyAddPiece4}",
                $"HotkeyAddPiece5={HotkeyConfig.HotkeyAddPiece5}",
                $"HotkeyAddPiece6={HotkeyConfig.HotkeyAddPiece6}",
                $"HotkeyAddPiece7={HotkeyConfig.HotkeyAddPiece7}",
                $"HotkeyAddPiece8={HotkeyConfig.HotkeyAddPiece8}",
                $"HotkeyAddPiece9={HotkeyConfig.HotkeyAddPiece9}",
                $"HotkeyAddPiece10={HotkeyConfig.HotkeyAddPiece10}",
                $"HotkeyAddPiece11={HotkeyConfig.HotkeyAddPiece11}",
                $"HotkeyAddPiece12={HotkeyConfig.HotkeyAddPiece12}",
                $"HotkeyAddPiece13={HotkeyConfig.HotkeyAddPiece13}",
                $"HotkeyUndo={HotkeyConfig.HotkeyUndo}",
                $"HotkeyRedo={HotkeyConfig.HotkeyRedo}",
                $"HotkeyCut={HotkeyConfig.HotkeyCut}",
                $"HotkeyCopy={HotkeyConfig.HotkeyCopy}",
                $"HotkeyPaste={HotkeyConfig.HotkeyPaste}",
                $"HotkeyDuplicate={HotkeyConfig.HotkeyDuplicate}",
                $"HotkeyDelete={HotkeyConfig.HotkeyDelete}",
                $"HotkeyMoveUp={HotkeyConfig.HotkeyMoveUp}",
                $"HotkeyMoveDown={HotkeyConfig.HotkeyMoveDown}",
                $"HotkeyMoveLeft={HotkeyConfig.HotkeyMoveLeft}",
                $"HotkeyMoveRight={HotkeyConfig.HotkeyMoveRight}",
                $"HotkeyMove8Up={HotkeyConfig.HotkeyMove8Up}",
                $"HotkeyMove8Down={HotkeyConfig.HotkeyMove8Down}",
                $"HotkeyMove8Left={HotkeyConfig.HotkeyMove8Left}",
                $"HotkeyMove8Right={HotkeyConfig.HotkeyMove8Right}",
                $"HotkeyCustomMove={HotkeyConfig.HotkeyCustomMove}",
                $"HotkeyDragHorizontally={HotkeyConfig.HotkeyDragHorizontally}",
                $"HotkeyDragVertically={HotkeyConfig.HotkeyDragVertically}",
                $"HotkeyRotate={HotkeyConfig.HotkeyRotate}",
                $"HotkeyFlip={HotkeyConfig.HotkeyFlip}",
                $"HotkeyInvert={HotkeyConfig.HotkeyInvert}",
                $"HotkeyGroup={HotkeyConfig.HotkeyGroup}",
                $"HotkeyUngroup={HotkeyConfig.HotkeyUngroup}",
                $"HotkeyErase={HotkeyConfig.HotkeyErase}",
                $"HotkeyNoOverwrite={HotkeyConfig.HotkeyNoOverwrite}",
                $"HotkeyOnlyOnTerrain={HotkeyConfig.HotkeyOnlyOnTerrain}",
                $"HotkeyAllowOneWay={HotkeyConfig.HotkeyAllowOneWay}",
                $"HotkeyDrawLast={HotkeyConfig.HotkeyDrawLast}",
                $"HotkeyDrawSooner={HotkeyConfig.HotkeyDrawSooner}",
                $"HotkeyDrawLater={HotkeyConfig.HotkeyDrawLater}",
                $"HotkeyDrawFirst={HotkeyConfig.HotkeyDrawFirst}",
                $"HotkeyCloseWindow={HotkeyConfig.HotkeyCloseWindow}",
                $"HotkeyCloseEditor={HotkeyConfig.HotkeyCloseEditor}"
            };

            System.IO.File.WriteAllLines("SLXEditorHotkeys.ini", lines);
        }

        private void LoadHotkeysFromIniFile()
        {
            var lines = System.IO.File.ReadAllLines("SLXEditorHotkeys.ini");
            foreach (var line in lines)
            {
                if (line.StartsWith("HotkeyCreateNewLevel="))
                    HotkeyConfig.HotkeyCreateNewLevel = (Keys)Enum.Parse(typeof(Keys), line.Split('=')[1]);
                if (line.StartsWith("HotkeyLoadLevel="))
                    HotkeyConfig.HotkeyLoadLevel = (Keys)Enum.Parse(typeof(Keys), line.Split('=')[1]);
                if (line.StartsWith("HotkeySaveLevel="))
                    HotkeyConfig.HotkeySaveLevel = (Keys)Enum.Parse(typeof(Keys), line.Split('=')[1]);
                if (line.StartsWith("HotkeySaveLevelAs="))
                    HotkeyConfig.HotkeySaveLevelAs = (Keys)Enum.Parse(typeof(Keys), line.Split('=')[1]);
                if (line.StartsWith("HotkeyPlaytestLevel="))
                    HotkeyConfig.HotkeyPlaytestLevel = (Keys)Enum.Parse(typeof(Keys), line.Split('=')[1]);
                if (line.StartsWith("HotkeyValidateLevel="))
                    HotkeyConfig.HotkeyValidateLevel = (Keys)Enum.Parse(typeof(Keys), line.Split('=')[1]);
                if (line.StartsWith("HotkeyToggleClearPhysics="))
                    HotkeyConfig.HotkeyToggleClearPhysics = (Keys)Enum.Parse(typeof(Keys), line.Split('=')[1]);
                if (line.StartsWith("HotkeyToggleTerrain="))
                    HotkeyConfig.HotkeyToggleTerrain = (Keys)Enum.Parse(typeof(Keys), line.Split('=')[1]);
                if (line.StartsWith("HotkeyToggleObjects="))
                    HotkeyConfig.HotkeyToggleObjects = (Keys)Enum.Parse(typeof(Keys), line.Split('=')[1]);
                if (line.StartsWith("HotkeyToggleTriggerAreas="))
                    HotkeyConfig.HotkeyToggleTriggerAreas = (Keys)Enum.Parse(typeof(Keys), line.Split('=')[1]);
                if (line.StartsWith("HotkeyToggleScreenStart="))
                    HotkeyConfig.HotkeyToggleScreenStart = (Keys)Enum.Parse(typeof(Keys), line.Split('=')[1]);
                if (line.StartsWith("HotkeyToggleBackground="))
                    HotkeyConfig.HotkeyToggleBackground = (Keys)Enum.Parse(typeof(Keys), line.Split('=')[1]);
                if (line.StartsWith("HotkeyToggleSnapToGrid="))
                    HotkeyConfig.HotkeyToggleSnapToGrid = (Keys)Enum.Parse(typeof(Keys), line.Split('=')[1]);
                if (line.StartsWith("HotkeyOpenSettings="))
                    HotkeyConfig.HotkeyOpenSettings = (Keys)Enum.Parse(typeof(Keys), line.Split('=')[1]);
                if (line.StartsWith("HotkeyOpenHotkeyConfig="))
                    HotkeyConfig.HotkeyOpenHotkeyConfig = (Keys)Enum.Parse(typeof(Keys), line.Split('=')[1]);
                if (line.StartsWith("HotkeyOpenAboutSLX="))
                    HotkeyConfig.HotkeyOpenAboutSLX = (Keys)Enum.Parse(typeof(Keys), line.Split('=')[1]);
                if (line.StartsWith("HotkeySelectPieces="))
                    HotkeyConfig.HotkeySelectPieces = (Keys)Enum.Parse(typeof(Keys), line.Split('=')[1]);
                if (line.StartsWith("HotkeyDragToScroll="))
                    HotkeyConfig.HotkeyDragToScroll = (Keys)Enum.Parse(typeof(Keys), line.Split('=')[1]);
                if (line.StartsWith("HotkeyRemovePiecesAtCursor="))
                    HotkeyConfig.HotkeyRemovePiecesAtCursor = (Keys)Enum.Parse(typeof(Keys), line.Split('=')[1]);
                if (line.StartsWith("HotkeyAddRemoveSinglePiece="))
                    HotkeyConfig.HotkeyAddRemoveSinglePiece = (Keys)Enum.Parse(typeof(Keys), line.Split('=')[1]);
                if (line.StartsWith("HotkeySelectPiecesBelow="))
                    HotkeyConfig.HotkeySelectPiecesBelow = (Keys)Enum.Parse(typeof(Keys), line.Split('=')[1]);
                if (line.StartsWith("HotkeyZoomIn="))
                    HotkeyConfig.HotkeyZoomIn = (Keys)Enum.Parse(typeof(Keys), line.Split('=')[1]);
                if (line.StartsWith("HotkeyZoomOut="))
                    HotkeyConfig.HotkeyZoomOut = (Keys)Enum.Parse(typeof(Keys), line.Split('=')[1]);
                if (line.StartsWith("HotkeyScrollHorizontally="))
                    HotkeyConfig.HotkeyScrollHorizontally = (Keys)Enum.Parse(typeof(Keys), line.Split('=')[1]);
                if (line.StartsWith("HotkeyScrollVertically="))
                    HotkeyConfig.HotkeyScrollVertically = (Keys)Enum.Parse(typeof(Keys), line.Split('=')[1]);
                if (line.StartsWith("HotkeyMoveScreenStart="))
                    HotkeyConfig.HotkeyMoveScreenStart = (Keys)Enum.Parse(typeof(Keys), line.Split('=')[1]);
                if (line.StartsWith("HotkeyShowPreviousPiece="))
                    HotkeyConfig.HotkeyShowPreviousPiece = (Keys)Enum.Parse(typeof(Keys), line.Split('=')[1]);
                if (line.StartsWith("HotkeyShowNextPiece="))
                    HotkeyConfig.HotkeyShowNextPiece = (Keys)Enum.Parse(typeof(Keys), line.Split('=')[1]);
                if (line.StartsWith("HotkeyShowPreviousGroup="))
                    HotkeyConfig.HotkeyShowPreviousGroup = (Keys)Enum.Parse(typeof(Keys), line.Split('=')[1]);
                if (line.StartsWith("HotkeyShowNextGroup="))
                    HotkeyConfig.HotkeyShowNextGroup = (Keys)Enum.Parse(typeof(Keys), line.Split('=')[1]);
                if (line.StartsWith("HotkeyShowPreviousStyle="))
                    HotkeyConfig.HotkeyShowPreviousStyle = (Keys)Enum.Parse(typeof(Keys), line.Split('=')[1]);
                if (line.StartsWith("HotkeyShowNextStyle="))
                    HotkeyConfig.HotkeyShowNextStyle = (Keys)Enum.Parse(typeof(Keys), line.Split('=')[1]);
                if (line.StartsWith("HotkeySwitchBrowser="))
                    HotkeyConfig.HotkeySwitchBrowser = (Keys)Enum.Parse(typeof(Keys), line.Split('=')[1]);
                if (line.StartsWith("HotkeyAddPiece1="))
                    HotkeyConfig.HotkeyAddPiece1 = (Keys)Enum.Parse(typeof(Keys), line.Split('=')[1]);
                if (line.StartsWith("HotkeyAddPiece2="))
                    HotkeyConfig.HotkeyAddPiece2 = (Keys)Enum.Parse(typeof(Keys), line.Split('=')[1]);
                if (line.StartsWith("HotkeyAddPiece3="))
                    HotkeyConfig.HotkeyAddPiece3 = (Keys)Enum.Parse(typeof(Keys), line.Split('=')[1]);
                if (line.StartsWith("HotkeyAddPiece4="))
                    HotkeyConfig.HotkeyAddPiece4 = (Keys)Enum.Parse(typeof(Keys), line.Split('=')[1]);
                if (line.StartsWith("HotkeyAddPiece5="))
                    HotkeyConfig.HotkeyAddPiece5 = (Keys)Enum.Parse(typeof(Keys), line.Split('=')[1]);
                if (line.StartsWith("HotkeyAddPiece6="))
                    HotkeyConfig.HotkeyAddPiece6 = (Keys)Enum.Parse(typeof(Keys), line.Split('=')[1]);
                if (line.StartsWith("HotkeyAddPiece7="))
                    HotkeyConfig.HotkeyAddPiece7 = (Keys)Enum.Parse(typeof(Keys), line.Split('=')[1]);
                if (line.StartsWith("HotkeyAddPiece8="))
                    HotkeyConfig.HotkeyAddPiece8 = (Keys)Enum.Parse(typeof(Keys), line.Split('=')[1]);
                if (line.StartsWith("HotkeyAddPiece9="))
                    HotkeyConfig.HotkeyAddPiece9 = (Keys)Enum.Parse(typeof(Keys), line.Split('=')[1]);
                if (line.StartsWith("HotkeyAddPiece10="))
                    HotkeyConfig.HotkeyAddPiece10 = (Keys)Enum.Parse(typeof(Keys), line.Split('=')[1]);
                if (line.StartsWith("HotkeyAddPiece11="))
                    HotkeyConfig.HotkeyAddPiece11 = (Keys)Enum.Parse(typeof(Keys), line.Split('=')[1]);
                if (line.StartsWith("HotkeyAddPiece12="))
                    HotkeyConfig.HotkeyAddPiece12 = (Keys)Enum.Parse(typeof(Keys), line.Split('=')[1]);
                if (line.StartsWith("HotkeyAddPiece13="))
                    HotkeyConfig.HotkeyAddPiece13 = (Keys)Enum.Parse(typeof(Keys), line.Split('=')[1]);
                if (line.StartsWith("HotkeyUndo="))
                    HotkeyConfig.HotkeyUndo = (Keys)Enum.Parse(typeof(Keys), line.Split('=')[1]);
                if (line.StartsWith("HotkeyRedo="))
                    HotkeyConfig.HotkeyRedo = (Keys)Enum.Parse(typeof(Keys), line.Split('=')[1]);
                if (line.StartsWith("HotkeyCut="))
                    HotkeyConfig.HotkeyCut = (Keys)Enum.Parse(typeof(Keys), line.Split('=')[1]);
                if (line.StartsWith("HotkeyCopy="))
                    HotkeyConfig.HotkeyCopy = (Keys)Enum.Parse(typeof(Keys), line.Split('=')[1]);
                if (line.StartsWith("HotkeyPaste="))
                    HotkeyConfig.HotkeyPaste = (Keys)Enum.Parse(typeof(Keys), line.Split('=')[1]);
                if (line.StartsWith("HotkeyDuplicate="))
                    HotkeyConfig.HotkeyDuplicate = (Keys)Enum.Parse(typeof(Keys), line.Split('=')[1]);
                if (line.StartsWith("HotkeyDelete="))
                    HotkeyConfig.HotkeyDelete = (Keys)Enum.Parse(typeof(Keys), line.Split('=')[1]);
                if (line.StartsWith("HotkeyMoveUp="))
                    HotkeyConfig.HotkeyMoveUp = (Keys)Enum.Parse(typeof(Keys), line.Split('=')[1]);
                if (line.StartsWith("HotkeyMoveDown="))
                    HotkeyConfig.HotkeyMoveDown = (Keys)Enum.Parse(typeof(Keys), line.Split('=')[1]);
                if (line.StartsWith("HotkeyMoveLeft="))
                    HotkeyConfig.HotkeyMoveLeft = (Keys)Enum.Parse(typeof(Keys), line.Split('=')[1]);
                if (line.StartsWith("HotkeyMoveRight="))
                    HotkeyConfig.HotkeyMoveRight = (Keys)Enum.Parse(typeof(Keys), line.Split('=')[1]);
                if (line.StartsWith("HotkeyMove8Up="))
                    HotkeyConfig.HotkeyMoveUp = (Keys)Enum.Parse(typeof(Keys), line.Split('=')[1]);
                if (line.StartsWith("HotkeyMove8Down="))
                    HotkeyConfig.HotkeyMoveDown = (Keys)Enum.Parse(typeof(Keys), line.Split('=')[1]);
                if (line.StartsWith("HotkeyMove8Left="))
                    HotkeyConfig.HotkeyMoveLeft = (Keys)Enum.Parse(typeof(Keys), line.Split('=')[1]);
                if (line.StartsWith("HotkeyMove8Right="))
                    HotkeyConfig.HotkeyMoveRight = (Keys)Enum.Parse(typeof(Keys), line.Split('=')[1]);
                if (line.StartsWith("HotkeyCustomMove="))
                    HotkeyConfig.HotkeyCustomMove = (Keys)Enum.Parse(typeof(Keys), line.Split('=')[1]);
                if (line.StartsWith("HotkeyDragHorizontally="))
                    HotkeyConfig.HotkeyDragHorizontally = (Keys)Enum.Parse(typeof(Keys), line.Split('=')[1]);
                if (line.StartsWith("HotkeyDragVertically="))
                    HotkeyConfig.HotkeyDragVertically = (Keys)Enum.Parse(typeof(Keys), line.Split('=')[1]);
                if (line.StartsWith("HotkeyRotate="))
                    HotkeyConfig.HotkeyRotate = (Keys)Enum.Parse(typeof(Keys), line.Split('=')[1]);
                if (line.StartsWith("HotkeyFlip="))
                    HotkeyConfig.HotkeyFlip = (Keys)Enum.Parse(typeof(Keys), line.Split('=')[1]);
                if (line.StartsWith("HotkeyInvert="))
                    HotkeyConfig.HotkeyInvert = (Keys)Enum.Parse(typeof(Keys), line.Split('=')[1]);
                if (line.StartsWith("HotkeyGroup="))
                    HotkeyConfig.HotkeyGroup = (Keys)Enum.Parse(typeof(Keys), line.Split('=')[1]);
                if (line.StartsWith("HotkeyUngroup="))
                    HotkeyConfig.HotkeyUngroup = (Keys)Enum.Parse(typeof(Keys), line.Split('=')[1]);
                if (line.StartsWith("HotkeyErase="))
                    HotkeyConfig.HotkeyErase = (Keys)Enum.Parse(typeof(Keys), line.Split('=')[1]);
                if (line.StartsWith("HotkeyNoOverwrite="))
                    HotkeyConfig.HotkeyNoOverwrite = (Keys)Enum.Parse(typeof(Keys), line.Split('=')[1]);
                if (line.StartsWith("HotkeyOnlyOnTerrain="))
                    HotkeyConfig.HotkeyOnlyOnTerrain = (Keys)Enum.Parse(typeof(Keys), line.Split('=')[1]);
                if (line.StartsWith("HotkeyAllowOneWay="))
                    HotkeyConfig.HotkeyAllowOneWay = (Keys)Enum.Parse(typeof(Keys), line.Split('=')[1]);
                if (line.StartsWith("HotkeyDrawLast="))
                    HotkeyConfig.HotkeyDrawLast = (Keys)Enum.Parse(typeof(Keys), line.Split('=')[1]);
                if (line.StartsWith("HotkeyDrawSooner="))
                    HotkeyConfig.HotkeyDrawSooner = (Keys)Enum.Parse(typeof(Keys), line.Split('=')[1]);
                if (line.StartsWith("HotkeyDrawLater="))
                    HotkeyConfig.HotkeyDrawLater = (Keys)Enum.Parse(typeof(Keys), line.Split('=')[1]);
                if (line.StartsWith("HotkeyDrawFirst="))
                    HotkeyConfig.HotkeyDrawFirst = (Keys)Enum.Parse(typeof(Keys), line.Split('=')[1]);
                if (line.StartsWith("HotkeyCloseWindow="))
                    HotkeyConfig.HotkeyCloseWindow = (Keys)Enum.Parse(typeof(Keys), line.Split('=')[1]);
                if (line.StartsWith("HotkeyCloseEditor="))
                    HotkeyConfig.HotkeyCloseEditor = (Keys)Enum.Parse(typeof(Keys), line.Split('=')[1]);
            }
        }

        public void GetDefaultHotkeys()
        {
            HotkeyConfig.HotkeyCreateNewLevel = Keys.Control | Keys.N;
            HotkeyConfig.HotkeyLoadLevel = Keys.Control | Keys.O;
            HotkeyConfig.HotkeySaveLevel = Keys.Control | Keys.S;
            HotkeyConfig.HotkeySaveLevelAs = Keys.Control | Keys.Alt | Keys.S;
            HotkeyConfig.HotkeyPlaytestLevel = Keys.F12;
            HotkeyConfig.HotkeyValidateLevel = Keys.Control | Keys.F12;
            HotkeyConfig.HotkeyToggleClearPhysics = Keys.F1;
            HotkeyConfig.HotkeyToggleTerrain = Keys.F2;
            HotkeyConfig.HotkeyToggleObjects = Keys.F3;
            HotkeyConfig.HotkeyToggleTriggerAreas = Keys.F4;
            HotkeyConfig.HotkeyToggleScreenStart = Keys.F5;
            HotkeyConfig.HotkeyToggleBackground = Keys.F6;
            HotkeyConfig.HotkeyToggleSnapToGrid = Keys.F9;
            HotkeyConfig.HotkeyOpenSettings = Keys.F10;
            HotkeyConfig.HotkeyOpenHotkeyConfig = Keys.F11;
            HotkeyConfig.HotkeyOpenAboutSLX = Keys.Control | Keys.Alt | Keys.F11;
            HotkeyConfig.HotkeySelectPieces = Keys.LButton;
            HotkeyConfig.HotkeyDragToScroll = Keys.RButton;
            HotkeyConfig.HotkeyRemovePiecesAtCursor = Keys.MButton;
            HotkeyConfig.HotkeyAddRemoveSinglePiece = Keys.Control | Keys.LButton;
            HotkeyConfig.HotkeySelectPiecesBelow = Keys.Alt | Keys.LButton;
            HotkeyConfig.HotkeyZoomIn = Keys.Oemplus;
            HotkeyConfig.HotkeyZoomOut = Keys.OemMinus;
            HotkeyConfig.HotkeyScrollHorizontally = Keys.Control | Keys.None; // Come back to this later
            HotkeyConfig.HotkeyScrollVertically = Keys.Alt | Keys.None; // Come back to this later
            HotkeyConfig.HotkeyMoveScreenStart = Keys.P;
            HotkeyConfig.HotkeyShowPreviousPiece = Keys.Shift | Keys.Left;
            HotkeyConfig.HotkeyShowNextPiece = Keys.Shift | Keys.Right;
            HotkeyConfig.HotkeyShowPreviousGroup = Keys.Shift | Keys.Alt | Keys.Left;
            HotkeyConfig.HotkeyShowNextGroup = Keys.Shift | Keys.Alt | Keys.Right;
            HotkeyConfig.HotkeyShowPreviousStyle = Keys.Shift | Keys.Up;
            HotkeyConfig.HotkeyShowNextStyle = Keys.Shift | Keys.Down;
            HotkeyConfig.HotkeySwitchBrowser = Keys.Space;
            HotkeyConfig.HotkeyAddPiece1 = Keys.NumPad1;
            HotkeyConfig.HotkeyAddPiece2 = Keys.NumPad2;
            HotkeyConfig.HotkeyAddPiece3 = Keys.NumPad3;
            HotkeyConfig.HotkeyAddPiece4 = Keys.NumPad4;
            HotkeyConfig.HotkeyAddPiece5 = Keys.NumPad5;
            HotkeyConfig.HotkeyAddPiece6 = Keys.NumPad6;
            HotkeyConfig.HotkeyAddPiece7 = Keys.NumPad7;
            HotkeyConfig.HotkeyAddPiece8 = Keys.NumPad8;
            HotkeyConfig.HotkeyAddPiece9 = Keys.NumPad9;
            HotkeyConfig.HotkeyAddPiece10 = Keys.NumPad0;
            HotkeyConfig.HotkeyAddPiece11 = Keys.None; // Unassigned by default
            HotkeyConfig.HotkeyAddPiece12 = Keys.None; // Unassigned by default
            HotkeyConfig.HotkeyAddPiece13 = Keys.None; // Unassigned by default
            HotkeyConfig.HotkeyUndo = Keys.Control | Keys.Z;
            HotkeyConfig.HotkeyRedo = Keys.Control | Keys.Y;
            HotkeyConfig.HotkeyCut = Keys.Control | Keys.X;
            HotkeyConfig.HotkeyCopy = Keys.Control | Keys.C;
            HotkeyConfig.HotkeyPaste = Keys.Control | Keys.V;
            HotkeyConfig.HotkeyDuplicate = Keys.C;
            HotkeyConfig.HotkeyDelete = Keys.Delete;
            HotkeyConfig.HotkeyMoveUp = Keys.Up;
            HotkeyConfig.HotkeyMoveDown = Keys.Down;
            HotkeyConfig.HotkeyMoveLeft = Keys.Left;
            HotkeyConfig.HotkeyMoveRight = Keys.Right;
            HotkeyConfig.HotkeyMove8Up = Keys.Control | Keys.Up;
            HotkeyConfig.HotkeyMove8Down = Keys.Control | Keys.Down;
            HotkeyConfig.HotkeyMove8Left = Keys.Control | Keys.Left;
            HotkeyConfig.HotkeyMove8Right = Keys.Control | Keys.Right;
            HotkeyConfig.HotkeyCustomMove = Keys.Alt | Keys.Up;
            HotkeyConfig.HotkeyDragHorizontally = Keys.Control | Keys.Shift;
            HotkeyConfig.HotkeyDragVertically = Keys.Control | Keys.Alt;
            HotkeyConfig.HotkeyRotate = Keys.R;
            HotkeyConfig.HotkeyFlip = Keys.E;
            HotkeyConfig.HotkeyInvert = Keys.W;
            HotkeyConfig.HotkeyGroup = Keys.G;
            HotkeyConfig.HotkeyUngroup = Keys.H;
            HotkeyConfig.HotkeyErase = Keys.A;
            HotkeyConfig.HotkeyNoOverwrite = Keys.S;
            HotkeyConfig.HotkeyOnlyOnTerrain = Keys.D;
            HotkeyConfig.HotkeyAllowOneWay = Keys.F;
            HotkeyConfig.HotkeyDrawLast = Keys.Home;
            HotkeyConfig.HotkeyDrawSooner = Keys.PageUp;
            HotkeyConfig.HotkeyDrawLater = Keys.PageDown;
            HotkeyConfig.HotkeyDrawFirst = Keys.End;
            HotkeyConfig.HotkeyCloseWindow = Keys.Escape;
            HotkeyConfig.HotkeyCloseEditor = Keys.Alt | Keys.F4;
        }
    }
}