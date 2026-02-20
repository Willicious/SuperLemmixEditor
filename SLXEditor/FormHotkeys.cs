using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace SLXEditor
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
            if (HotkeyConfig.DefaultHotkeysLoaded)
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

            // Get the selected item
            var selectedItem = listViewHotkeys.SelectedItems.Count > 0 ? listViewHotkeys.SelectedItems[0] : null;

            if (selectedItem != null && selectedItem.Tag is HotkeyConfig.HotkeyName hotkeyName)
            {
                var hotkeyData = HotkeyConfig.GetHotkey(hotkeyName);

                if (hotkeyData != null && hotkeyData.RequiresMouseButton)
                {
                    // Only allow mouse keys
                    if (!HotkeyConfig.MouseKeys.Contains(listenedKey & ~Keys.Modifiers))
                    {
                        MessageBox.Show(
                            "This hotkey requires a mouse button key (e.g., LButton, RButton, MButton, etc.).",
                            "Invalid Key",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Warning
                        );

                        ResetUI();
                        return;
                    }
                }
            }

            // Ignore Enter/Return keys
            if (selectedItem != null && (listenedKey == Keys.Return || listenedKey == Keys.Enter))
            {
                ResetUI();
                return;
            }

            // Set the selected key and update the combo box
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

        private void UpdateComboBox(Keys key)
        {
            if (listViewHotkeys.SelectedItems.Count == 0)
                return;

            var selectedItem = listViewHotkeys.SelectedItems[0];

            if (!(selectedItem.Tag is HotkeyConfig.HotkeyName hotkeyName))
                return;

            var hotkeyData = HotkeyConfig.GetHotkey(hotkeyName);
            if (hotkeyData == null)
                return;

            // Build formatted list of keys
            var formattedHotkeys = defaultKeyList.Select(HotkeyConfig.FormatHotkeyString).ToList();
            string formattedKey = HotkeyConfig.FormatHotkeyString(key);

            if (!formattedHotkeys.Contains(formattedKey))
                formattedHotkeys.Add(formattedKey);

            // Use mouse keys if required
            if (hotkeyData.RequiresMouseButton)
                comboBoxChooseKey.DataSource = HotkeyConfig.MouseKeys
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

                if (!(selectedItem.Tag is HotkeyConfig.HotkeyName hotkeyName))
                {
                    DoCheckForDuplicates = true;
                    return;
                }

                var hotkeyData = HotkeyConfig.GetHotkey(hotkeyName);
                if (hotkeyData == null)
                {
                    DoCheckForDuplicates = true;
                    return;
                }

                // Parse the currently assigned key
                Keys assignedKey = hotkeyData.CurrentKeys;

                // Update combo box with the base key
                Keys baseKey = assignedKey & ~(Keys.Control | Keys.Shift | Keys.Alt);
                UpdateComboBox(baseKey);

                // Update modifier checkboxes
                checkModCtrl.Checked = assignedKey.HasFlag(Keys.Control);
                checkModShift.Checked = assignedKey.HasFlag(Keys.Shift);
                checkModAlt.Checked = assignedKey.HasFlag(Keys.Alt);

                ResetComponents();

                // Special UI for Select/Drag Pieces hotkey
                if (hotkeyData.Name == HotkeyConfig.HotkeyName.HotkeySelectPieces)
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
            listViewHotkeys.Items.Clear();

            foreach (var hotkey in HotkeyConfig.AllHotkeys)
            {
                // SubItem[0] holds the hotkey description
                var item = new ListViewItem(hotkey.Description);

                // SubItem[1] holds the current formatted key
                item.SubItems.Add(HotkeyConfig.FormatHotkeyString(hotkey.CurrentKeys));

                // Store the hotkey name in the item tag for later lookup
                item.Tag = hotkey.Name;

                listViewHotkeys.Items.Add(item);
            }

            if (listViewHotkeys.Items.Count > 0)
            {
                listViewHotkeys.Items[0].Selected = true;
                listViewHotkeys.Focus();
            }

            // Rebuild mandatory mouse items after populating
            GetMouseMandatoryHotkeys();
        }

        private void GetMouseMandatoryHotkeys()
        {
            HotkeyConfig.mouseMandatoryItems.Clear();

            foreach (ListViewItem item in listViewHotkeys.Items)
            {
                if (item.Tag is HotkeyConfig.HotkeyName hotkeyName)
                {
                    var hotkeyData = HotkeyConfig.GetHotkey(hotkeyName);
                    if (hotkeyData != null && hotkeyData.RequiresMouseButton)
                    {
                        HotkeyConfig.mouseMandatoryItems.Add(item);
                    }
                }
            }
        }

        private void WriteToHotkeyConfig()
        {
            foreach (ListViewItem item in listViewHotkeys.Items)
            {
                if (!(item.Tag is HotkeyConfig.HotkeyName hotkeyName))
                    continue;

                var hotkeyData = HotkeyConfig.GetHotkey(hotkeyName);
                if (hotkeyData == null)
                    continue;

                // Convert the displayed string back to Keys
                string subItemText = item.SubItems[1].Text;
                hotkeyData.CurrentKeys = HotkeyConfig.ParseHotkeyString(subItemText);
            }
        }
    }
}