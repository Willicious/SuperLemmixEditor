using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics.Eventing.Reader;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NLEditor
{
    public partial class FormHotkeys : Form
    {
        // For mandatory mouse hotkeys
        private List<string> mandatoryMouseHotkeyText = new List<string>
        {
            "HotkeySelectPieces",
            "HotkeyDragToScroll",
            "HotkeyDragScreenStart",
            "HotkeyDragHorizontally",
            "HotkeyDragVertically"
        };

        private List<Keys> mandatoryMouseKeys = new List<Keys>
        {
            Keys.LButton,
            Keys.RButton,
            Keys.MButton,
            Keys.XButton1,
            Keys.XButton2
        };

        private List<ListViewItem> mouseMandatoryItems = new List<ListViewItem>();

        private Keys selectedKey;
        private ListViewItem selectedItem;
        private bool DoCheckForDuplicates = true;

        public FormHotkeys()
        {
            InitializeComponent();
            GetMouseMandatoryHotkeys();
            SetSubItemNames();
        }

        private void FormHotkeys_Load(object sender, EventArgs e)
        {
            KeyPreview = true;
        }

        private void FormHotkeys_Shown(object sender, EventArgs e)
        {
            // Load hotkeys
            if (System.IO.File.Exists("SLXEditorHotkeys.ini"))
                HotkeyConfig.LoadHotkeysFromIniFile();
            else
                HotkeyConfig.GetDefaultHotkeys();
            
            // Load and focus the listview
            LoadHotkeysToListView();
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
                string enteredKeyText = comboBoxChooseKey.Text.Trim();
                if (Enum.TryParse(enteredKeyText, true, out Keys parsedKey))
                {
                    ClearHighlights();
                    selectedKey = parsedKey;

                    comboBoxChooseKey.SelectedItem = parsedKey;
                    CheckForDuplicateKeys();
                }
                else
                {
                    MessageBox.Show($"Invalid key name: '{enteredKeyText}'", "Invalid Input", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }

                e.Handled = true; // Prevent default behavior for Enter
            }
        }

        private void FormHotkeys_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                if (lblListening.Visible)
                    return;
                else if (lblDuplicateDetected.Visible)
                    ResetUI();
                else if (ActiveControl == comboBoxChooseKey)
                    listViewHotkeys.Focus();
                else
                    Close();
            }
        }

        private void HandleListenedInput(object sender, EventArgs e, Keys listenedKey)
        {            
            // Clear existing modifiers for initial processing
            checkModCtrl.Checked = false;
            checkModShift.Checked = false;
            checkModAlt.Checked = false;

            // Update the combo box with the selected key/button
            comboBoxChooseKey.SelectedItem = listenedKey;

            // Disable key & mouse events
            KeyDown -= HandleListeningForKey;
            MouseDown -= FormHotkeys_MouseDown;

            selectedKey = listenedKey;
            CheckForDuplicateKeys();
        }

        private void HandleListeningForKey(object sender, KeyEventArgs e)
        {
            HandleListenedInput(sender, e, e.KeyData);
        }

        private void FormHotkeys_MouseDown(object sender, MouseEventArgs e)
        {
            // Convert mouse buttons to Keys
            Keys listenedKey = e.Button == MouseButtons.Left ? Keys.LButton :
                               e.Button == MouseButtons.Right ? Keys.RButton :
                               e.Button == MouseButtons.Middle ? Keys.MButton :
                               e.Button == MouseButtons.XButton1 ? Keys.XButton1 :
                               e.Button == MouseButtons.XButton2 ? Keys.XButton2 :
                               Keys.None;

            HandleListenedInput(sender, e, listenedKey);
        }

        private void btnListen_Click(object sender, EventArgs e)
        {
            ClearHighlights();

            // Enable key & mouse events
            KeyDown += HandleListeningForKey;
            MouseDown += FormHotkeys_MouseDown;

            ResetComponents();

            // Update labels
            lblListening.Visible = true;
            lblAddModifier.Visible = false;
            checkModAlt.Enabled = false;
            checkModAlt.Visible = false;
            checkModCtrl.Enabled = false;
            checkModCtrl.Visible = false;
            checkModShift.Enabled = false;
            checkModShift.Visible = false;

            // Update buttons
            btnListen.Enabled = false;
            btnCancel.Enabled = true;
            btnCancel.Focus();

            // Disable combo box
            comboBoxChooseKey.Enabled = false;
        }

        private void checkModifiers_Click(object sender, EventArgs e)
        {
            ClearHighlights();
            CheckForDuplicateKeys();
        }

        private void btnAssignChosenKey_Click(object sender, EventArgs e)
        {
            UpdateChosenKey();
            UpdateListview();
            UpdateCaption();
        }

        private void btnClearAllKeys_Click(object sender, EventArgs e)
        {
            ClearAllKeys();
            ResetUI();
            UpdateCaption();
        }

        private void btnResetToDefaultKeys_Click(object sender, EventArgs e)
        {
            HotkeyConfig.GetDefaultHotkeys();
            LoadHotkeysToListView();
            ResetUI();
            UpdateCaption();
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            WriteToHotkeyConfig();
            HotkeyConfig.SaveHotkeysToIniFile();
            ResetUI();
            UpdateCaption(true);
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            ResetUI();
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void UpdateCaption(bool isSaved = false)
        {
            lblEditedSaved.Visible = true;

            if (isSaved)
            {
                lblEditedSaved.ForeColor = Color.MediumSeaGreen;
                lblEditedSaved.Text = "Hotkey Configuration saved successfully!";
                this.Text = "SLX Editor - Hotkey Configuration";
            }
            else
            {
                lblEditedSaved.ForeColor = Color.DarkViolet;
                lblEditedSaved.Text = "Hotkey Configuration edited...";
                this.Text = "SLX Editor - Hotkey Configuration - [Edited]";
            }  
        }

        /// <summary>
        /// Iterates over the initial list for string matches
        /// Assigns each match as a mouse-mandatory item
        /// </summary>
        private void GetMouseMandatoryHotkeys()
        {
            foreach (var item in listViewHotkeys.Items)
            {
                var listViewItem = item as ListViewItem;

                foreach (var subItemText in mandatoryMouseHotkeyText)
                {
                    if (listViewItem.SubItems[1].Text == subItemText)
                    {
                        mouseMandatoryItems.Add(listViewItem);
                        break;
                    }
                }
            }
        }

        private void UpdateComboBox(Keys key)
        {
            var selectedItem = listViewHotkeys.SelectedItems[0];
            MessageBox.Show("Item is selected: " + selectedItem.Text);

            // Check if the selected item is part of the mouse-mandatory items
            if (mouseMandatoryItems.Contains(selectedItem))
            {
                MessageBox.Show("Mouse mandatory items loaded");
                comboBoxChooseKey.DataSource = mandatoryMouseKeys; // Load mouse-only keys
            }
            else // Load default key list
            {
                MessageBox.Show("Default items loaded");
                comboBoxChooseKey.DataSource = Enum.GetValues(typeof(Keys)).Cast<Keys>().ToList();
            }
                
            comboBoxChooseKey.SelectedItem = key;
            MessageBox.Show("Combo box should be showing key: " + key.ToString());
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

            ResetComponents();
        }

        private void ClearHighlights()
        {
            foreach (ListViewItem item in listViewHotkeys.Items)
            {
                item.BackColor = SystemColors.Window;
            }
        }

        private void UpdateListview()
        {
            // Update the listview in preparation for writing to Config
            if (listViewHotkeys.SelectedItems.Count > 0)
            {
                selectedItem = listViewHotkeys.SelectedItems[0];
                selectedItem.SubItems[1].Text = HotkeyConfig.FormatHotkeyString(selectedKey);

                ResetUI();
            }
        }

        private void UpdateChosenKey()
        {
            if (listViewHotkeys.SelectedItems.Count > 0)
            {
                selectedItem = listViewHotkeys.SelectedItems[0];

                ResetComponents();

                // Update labels
                lblChosenKey.Enabled = true;
                lblChosenHotkey.ForeColor = Color.MediumSeaGreen;
                lblChosenHotkey.Enabled = true;
                lblChosenHotkey.Visible = true;
                lblChosenHotkey.Text = HotkeyConfig.FormatHotkeyString(selectedKey);

                // Update buttons and other components
                btnAssignChosenKey.Enabled = true;
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

                    ResetComponents();

                    // Update labels
                    lblChosenKey.Enabled = true;
                    lblChosenHotkey.ForeColor = Color.Red;
                    lblChosenHotkey.Enabled = true;
                    lblChosenHotkey.Visible = true;
                    lblChosenHotkey.Text = HotkeyConfig.FormatHotkeyString(selectedKey);
                    lblDuplicateDetected.Visible = true;
                    lblDuplicateAction.Visible = true;
                    lblDuplicateAction.Text = duplicateItem.SubItems[0].Text;

                    // Update buttons
                    btnCancel.Enabled = true;
                }
                else
                {
                    UpdateChosenKey();
                }
            }
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
                UpdateComboBox(baseKey);

                // Update the modifier checkboxes
                checkModCtrl.Checked = assignedKey.HasFlag(Keys.Control);
                checkModShift.Checked = assignedKey.HasFlag(Keys.Shift);
                checkModAlt.Checked = assignedKey.HasFlag(Keys.Alt);

                ResetComponents();
            }

            DoCheckForDuplicates = true;
        }

        private void ResetComponents()
        {           
            // Reset labels
            lblActionToBeAssigned.Text = selectedItem.SubItems[0].Text;
            lblCurrentHotkey.Text = selectedItem.SubItems[1].Text;
            lblListening.Visible = false;
            lblAddModifier.Visible = true;
            checkModAlt.Enabled = true;
            checkModAlt.Visible = true;
            checkModCtrl.Enabled = true;
            checkModCtrl.Visible = true;
            checkModShift.Enabled = true;
            checkModShift.Visible = true;
            lblChosenKey.Enabled = false;
            lblChosenHotkey.Enabled = false;
            lblChosenHotkey.Visible = false;
            lblChosenHotkey.Text = "";
            lblDuplicateDetected.Visible = false;
            lblDuplicateAction.Visible = false;
            lblDuplicateAction.Text = "";
            lblEditedSaved.Visible = false;
            lblEditedSaved.Text = "";

            // Reset buttons
            btnAssignChosenKey.Enabled = false;
            btnCancel.Enabled = false;
            btnListen.Enabled = true;

            // Enable combo box
            comboBoxChooseKey.Enabled = true;
        }

        private ListViewItem FindDuplicateHotkey(Keys hotkey, ListView listView, ListViewItem currentItem)
        {
            foreach (ListViewItem item in listView.Items)
            {
                if (item == currentItem) continue; // Skip the current item being edited

                string itemHotkeyText = item.SubItems[1].Text;

                // Skip if the hotkey string is empty or blank
                if (string.IsNullOrWhiteSpace(itemHotkeyText)) continue;

                // Compare the entire hotkey string, including modifiers
                if (itemHotkeyText == HotkeyConfig.FormatHotkeyString(hotkey))
                {
                    return item; // Return the duplicate item
                }
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
            var fields = typeof(HotkeyConfig).GetFields(System.Reflection.BindingFlags.Public |
                                                        System.Reflection.BindingFlags.Static);

            int n = 0;

            foreach (var field in fields)
            {
                if (field.FieldType == typeof(Keys)) // Ensure the field is of type Keys
                {
                    Keys hotkey = (Keys)field.GetValue(null); // Retrieve the currently assigned hotkey
                    listViewHotkeys.Items[n].SubItems[1].Text = HotkeyConfig.FormatHotkeyString(hotkey);
                    n++;
                }
            }
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
                    case "HotkeySaveLevelAsImage":
                        HotkeyConfig.HotkeySaveLevelAsImage = parsedKey;
                        break;
                    case "HotkeyPlaytestLevel":
                        HotkeyConfig.HotkeyPlaytestLevel = parsedKey;
                        break;
                    case "HotkeyValidateLevel":
                        HotkeyConfig.HotkeyValidateLevel = parsedKey;
                        break;
                    case "HotkeyCleanseLevels":
                        HotkeyConfig.HotkeyCleanseLevels = parsedKey;
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
                    case "HotkeyToggleDeprecatedPieces":
                        HotkeyConfig.HotkeyToggleDeprecatedPieces = parsedKey;
                        break;
                    case "HotkeyShowMissingPieces":
                        HotkeyConfig.HotkeyShowMissingPieces = parsedKey;
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
                    case "HotkeyCycleBrowser":
                        HotkeyConfig.HotkeyCycleBrowser = parsedKey;
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
                    case "HotkeyPasteInPlace":
                        HotkeyConfig.HotkeyPasteInPlace = parsedKey;
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
                    case "HotkeyCustomMoveUp":
                        HotkeyConfig.HotkeyCustomMoveUp = parsedKey;
                        break;
                    case "HotkeyCustomMoveDown":
                        HotkeyConfig.HotkeyCustomMoveDown = parsedKey;
                        break;
                    case "HotkeyCustomMoveLeft":
                        HotkeyConfig.HotkeyCustomMoveLeft = parsedKey;
                        break;
                    case "HotkeyCustomMoveRight":
                        HotkeyConfig.HotkeyCustomMoveRight = parsedKey;
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