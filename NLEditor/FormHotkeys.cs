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
        private Keys selectedKey;
        private ListViewItem selectedItem;
        private bool DoCheckForDuplicates = true;

        private readonly List<Keys> defaultKeyList = new List<Keys>()
        {
            Keys.None, Keys.LButton, Keys.RButton, Keys.MButton, Keys.XButton1, Keys.XButton2,
            Keys.F1, Keys.F2, Keys.F3, Keys.F4, Keys.F5, Keys.F6, Keys.F7, Keys.F8, Keys.F9, Keys.F10, Keys.F11, Keys.F12,
            Keys.Insert, Keys.Delete, Keys.Back, Keys.OemMinus, Keys.Oemplus,
            Keys.D1, Keys.D2, Keys.D3, Keys.D4, Keys.D5, Keys.D6, Keys.D7, Keys.D8, Keys.D9, Keys.D0,
            Keys.Capital, Keys.LWin, Keys.RWin,
            Keys.A, Keys.B, Keys.C, Keys.D, Keys.E, Keys.F, Keys.G, Keys.H, Keys.I, Keys.J, Keys.K, Keys.L, Keys.M,
            Keys.N, Keys.O, Keys.P, Keys.Q, Keys.R, Keys.S, Keys.T, Keys.U, Keys.V, Keys.W, Keys.X, Keys.Y, Keys.Z,
            Keys.Oemcomma, Keys.OemPeriod, Keys.OemQuestion,
            Keys.Up, Keys.Down, Keys.Left, Keys.Right,
            Keys.PageUp, Keys.PageDown, Keys.End, Keys.Home,
            Keys.NumLock, Keys.NumPad0, Keys.NumPad1, Keys.NumPad2, Keys.NumPad3, Keys.NumPad4,
            Keys.NumPad5, Keys.NumPad6, Keys.NumPad7, Keys.NumPad8, Keys.NumPad9,
            Keys.Divide, Keys.Multiply, Keys.Subtract, Keys.Add
        };

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
            if (System.IO.File.Exists(C.AppPathHotkeys))
                HotkeyConfig.LoadHotkeysFromIniFile();
            else
                HotkeyConfig.GetDefaultHotkeys();

            // Load and focus the listview
            LoadHotkeysToListView();
            AutoSelectListItem();

            // Show the "Edited" label if defaults have been loaded
            if (HotkeyConfig.defaultHotkeysLoaded)
                UpdateCaption();
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
            string selectedString = comboBoxChooseKey.SelectedItem as string;

            if (!string.IsNullOrEmpty(selectedString))
            {
                ClearHighlights();

                // Convert formatted string back to Keys
                selectedKey = HotkeyConfig.ParseHotkeyString(selectedString);

                CheckForDuplicateKeys();
                SetModifierAvailability();
            }
        }

        private void comboBoxChooseKey_KeyDown(object sender, KeyEventArgs e)
        {
            ClearHighlights();

            if (e.KeyCode == Keys.Enter)
            {
                string enteredKeyText = comboBoxChooseKey.Text.Trim();

                // Find the matching item (ignore upper/lowercase)
                var matchingItem = comboBoxChooseKey.Items
                    .Cast<string>()
                    .FirstOrDefault(item => item.IndexOf(enteredKeyText, StringComparison.OrdinalIgnoreCase) >= 0);

                if (matchingItem != null)
                    comboBoxChooseKey.SelectedItem = matchingItem;
                else
                    return;

                CheckForDuplicateKeys();
                SetModifierAvailability();

                e.Handled = true;
            }
        }

        private void FormHotkeys_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                if (lblListening.Visible)
                    return; // Prevent closing the form in listening mode
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
            ClearHighlights();

            // Disable key & mouse events
            KeyDown -= HandleListeningForKey;
            MouseDown -= FormHotkeys_MouseDown;

            // Determine if the selected item requires a mouse key
            var selectedItem = listViewHotkeys.SelectedItems.Count > 0 ? listViewHotkeys.SelectedItems[0] : null;

            if (selectedItem != null && HotkeyConfig.mouseMandatoryItems.Contains(selectedItem))
            {
                if (!HotkeyConfig.mandatoryMouseKeys.Contains(listenedKey))
                {
                    MessageBox.Show("This hotkey requires a mouse button key (e.g., LButton, RButton, MButton, etc.).",
                                    "Invalid Key", MessageBoxButtons.OK, MessageBoxIcon.Warning);

                    ResetUI();
                    return;
                }
            }

            // Ignore Enter/Return
            if (selectedItem != null && (listenedKey == Keys.Return || listenedKey == Keys.Enter))
            {
                ResetUI();
                return;
            }

            // Format the hotkey for display and set the selected key
            string formattedKey = HotkeyConfig.FormatHotkeyString(listenedKey);
            comboBoxChooseKey.SelectedItem = formattedKey;
            selectedKey = listenedKey;

            CheckForDuplicateKeys();
        }


        private void HandleListeningForKey(object sender, KeyEventArgs e)
        {
            // Escape cancels listening
            if (e.KeyCode == Keys.Escape)
            {
                ResetUI();
                return;
            }
            
            // Ignore modifier keys on their own when listening
            if (e.KeyCode == Keys.ControlKey || e.KeyCode == Keys.ShiftKey || e.KeyCode == Keys.Menu)
            {
                return;
            }

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
            
            // Textbox hidden behind the Listen button to catch input from all keys
            focusText.Focus(); 

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

        private void btnLoadDefault_Click(object sender, EventArgs e)
        {
            HotkeyConfig.GetDefaultHotkeys();
            LoadHotkeysToListView();
            ResetUI();
            UpdateCaption();
        }

        private void btnLoadClassic_Click(object sender, EventArgs e)
        {
            HotkeyConfig.GetClassicHotkeys();
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
            if (lblEditedSaved.Visible && lblEditedSaved.ForeColor == Color.DarkViolet)
            {
                // Ask the user if they want to save
                DialogResult result = MessageBox.Show("Do you want to save the current configuration?",
                                                      "Configuration Edited",
                                                      MessageBoxButtons.YesNoCancel,
                                                      MessageBoxIcon.Warning);
                if (result == DialogResult.Yes)
                {
                    WriteToHotkeyConfig();
                    HotkeyConfig.SaveHotkeysToIniFile();
                }
                else if (result == DialogResult.Cancel)
                {
                    return; // Don't close form if user clicks Cancel
                }
            }

            // Proceed with closing the form if no unsaved changes or after saving
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

                foreach (var subItemText in HotkeyConfig.mandatoryMouseHotkeyText)
                {
                    if (listViewItem.SubItems[1].Text == subItemText)
                    {
                        HotkeyConfig.mouseMandatoryItems.Add(listViewItem);
                        break;
                    }
                }
            }
        }

        private void UpdateComboBox(Keys key)
        {
            var selectedItem = listViewHotkeys.SelectedItems[0];
            var formattedHotkeys = defaultKeyList.Select(HotkeyConfig.FormatHotkeyString).ToList();

            string formattedKey = HotkeyConfig.FormatHotkeyString(key);

            // If the key isn't in the formatted list, include it
            if (!formattedHotkeys.Contains(formattedKey))
            {
                formattedHotkeys.Add(formattedKey);
            }

            if (HotkeyConfig.mouseMandatoryItems.Contains(selectedItem))
                comboBoxChooseKey.DataSource = HotkeyConfig.mandatoryMouseKeys
                    .Select(HotkeyConfig.FormatHotkeyString)
                    .ToList();
            else
                comboBoxChooseKey.DataSource = formattedHotkeys;

            comboBoxChooseKey.SelectedItem = formattedKey;
        }

        private void SetModifierAvailability()
        {
            if (comboBoxChooseKey.SelectedItem?.ToString() == "None")
            {
                lblAddModifier.Enabled = false;
                checkModCtrl.Enabled = false;
                checkModShift.Enabled = false;
                checkModAlt.Enabled = false;
            }
            else
            {
                lblAddModifier.Enabled = true;
                checkModCtrl.Enabled = true;
                checkModShift.Enabled = true;
                checkModAlt.Enabled = true;
            }
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
                if (selectedKey == Keys.None)
                {
                    UpdateChosenKey();
                    return;
                }

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

                if (selectedItem.Text == "Select/Drag Pieces")
                {
                    SetUIForSelectPiecesHotkey();
                }
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

        /// <summary>
        /// This hotkey is set to Left Mouse Button and cannot be changed
        /// </summary>
        private void SetUIForSelectPiecesHotkey()
        {
            // Set labels
            lblActionToBeAssigned.Text = selectedItem.SubItems[0].Text;
            lblCurrentHotkey.Text = selectedItem.SubItems[1].Text;
            lblListening.Visible = false;
            lblAddModifier.Visible = true;
            checkModAlt.Enabled = false;
            checkModAlt.Visible = true;
            checkModCtrl.Enabled = false;
            checkModCtrl.Visible = true;
            checkModShift.Enabled = false;
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

            // Set buttons
            btnAssignChosenKey.Enabled = false;
            btnCancel.Enabled = false;
            btnListen.Enabled = false;

            // Set combo box
            comboBoxChooseKey.Enabled = false;
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
                    // Retrieve the currently assigned hotkey and format it as a string
                    Keys hotkey = (Keys)field.GetValue(null);
                    listViewHotkeys.Items[n].SubItems[1].Text = HotkeyConfig.FormatHotkeyString(hotkey);
                    n++;
                }
            }
        }

        private void WriteToHotkeyConfig()
        {
            foreach (ListViewItem item in listViewHotkeys.Items)
            {
                string subItemText = item.SubItems[1].Text;

                // Convert string back to Keys
                Keys parsedKey = HotkeyConfig.ParseHotkeyString(subItemText);

                // Assign the parsed key to the correct HotkeyConfig property
                switch (item.SubItems[1].Name)
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
                    case "HotkeyPieceSearch":
                        HotkeyConfig.HotkeyPieceSearch = parsedKey;
                        break;
                    case "HotkeyShowMissingPieces":
                        HotkeyConfig.HotkeyShowMissingPieces = parsedKey;
                        break;
                    case "HotkeyToggleSnapToGrid":
                        HotkeyConfig.HotkeyToggleSnapToGrid = parsedKey;
                        break;
                    case "HotkeyOpenLevelWindow":
                        HotkeyConfig.HotkeyOpenLevelWindow = parsedKey;
                        break;
                    case "HotkeyToggleAllTabs":
                        HotkeyConfig.HotkeyToggleAllTabs = parsedKey;
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
                        HotkeyConfig.HotkeySelectPieces = Keys.LButton; // Just in case
                        break;
                    case "HotkeyDragToScroll":
                        HotkeyConfig.HotkeyDragToScroll = parsedKey;
                        break;
                    case "HotkeyDragHorizontally":
                        HotkeyConfig.HotkeyDragHorizontally = parsedKey;
                        break;
                    case "HotkeyDragVertically":
                        HotkeyConfig.HotkeyDragVertically = parsedKey;
                        break;
                    case "HotkeyMoveScreenStart":
                        HotkeyConfig.HotkeyMoveScreenStart = parsedKey;
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
                    case "HotkeySelectAll":
                        HotkeyConfig.HotkeySelectAll = parsedKey;
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
                    case "HotkeyCloseEditor":
                        HotkeyConfig.HotkeyCloseEditor = parsedKey;
                        break;
                }
            }
        }
    }
}