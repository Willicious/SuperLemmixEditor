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

        private void FormHotkeys_Load(object sender, EventArgs e)
        {
            LoadDefaultHotkeysToListView();
            LoadHotkeysFromIniFile();

            comboBoxChooseKey.DataSource = Enum.GetValues(typeof(Keys)).Cast<Keys>().ToList();
        }

        private void FormHotkeys_Shown(object sender, EventArgs e)
        {
            // Ensure the list is selected and focused when the Form is shown
            AutoSelectListItem();
        }

        private void listViewHotkeys_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listViewHotkeys.SelectedItems.Count > 0)
            {
                selectedItem = listViewHotkeys.SelectedItems[0];

                // Update action to be assigned label
                lblActionToBeAssigned.Text = selectedItem.SubItems[0].Text;

                if (Enum.TryParse(selectedItem.SubItems[1].Text, out Keys assignedKey))
                {
                    comboBoxChooseKey.SelectedItem = assignedKey;

                    // Update the modifier checkboxes based on the assigned key
                    checkModCtrl.Checked = assignedKey.HasFlag(Keys.Control);
                    checkModShift.Checked = assignedKey.HasFlag(Keys.Shift);
                    checkModAlt.Checked = assignedKey.HasFlag(Keys.Alt);

                    // Update currently assigned key label
                    lblCurrentHotkey.Text = FormatHotkeyString(assignedKey);
                }
            }
        }

        private void comboBoxChooseKey_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listViewHotkeys.SelectedItems.Count > 0)
            {
                // Reference the currently selected ListView item
                selectedItem = listViewHotkeys.SelectedItems[0];

                // Update the global `selectedKey` with the value from the ComboBox
                if (comboBoxChooseKey.SelectedItem is Keys key)
                {
                    selectedKey = key;

                    // Combine the key with modifiers (Ctrl, Shift, Alt)
                    if (checkModCtrl.Checked) selectedKey |= Keys.Control;
                    if (checkModShift.Checked) selectedKey |= Keys.Shift;
                    if (checkModAlt.Checked) selectedKey |= Keys.Alt;

                    // Dynamically preview the hotkey string
                    lblChosenHotkey.Text = FormatHotkeyString(selectedKey);
                }
            }
        }


        private void btnListen_Click(object sender, EventArgs e)
        {
            KeyPreview = true;
            KeyDown += FormHotkeys_KeyDown;

            comboBoxChooseKey.Enabled = false;
            lblChosenKey.Text = "Listening for key...";
            lblChosenHotkey.Visible = false;
        }

        private void FormHotkeys_KeyDown(object sender, KeyEventArgs e)
        {
            // Capture the pressed key and stop listening
            listenedKey = e.KeyData;
            comboBoxChooseKey.SelectedItem = listenedKey & ~Keys.Modifiers; // Remove modifiers for display
            checkModCtrl.Checked = listenedKey.HasFlag(Keys.Control);
            checkModShift.Checked = listenedKey.HasFlag(Keys.Shift);
            checkModAlt.Checked = listenedKey.HasFlag(Keys.Alt);

            // Disable key listening
            KeyPreview = false;
            KeyDown -= FormHotkeys_KeyDown;

            comboBoxChooseKey.Enabled = true;
            lblChosenKey.Text = "Chosen Key:";
            lblChosenHotkey.Visible = true;

            selectedKey = listenedKey;
            UpdateSelectedHotkey();
        }

        private void checkModCtrl_CheckedChanged(object sender, EventArgs e)
        {
            UpdateSelectedHotkey();
        }

        private void checkModShift_CheckedChanged(object sender, EventArgs e)
        {
            UpdateSelectedHotkey();
        }

        private void checkModAlt_CheckedChanged(object sender, EventArgs e)
        {
            UpdateSelectedHotkey();
        }


        private void btnAssignChosenKey_Click(object sender, EventArgs e)
        {
            UpdateSelectedHotkey();
            UpdateKey();
        }

        private void btnClearAllKeys_Click(object sender, EventArgs e)
        {
            ClearAllKeys();
        }

        private void btnResetToDefaultKeys_Click(object sender, EventArgs e)
        {
            LoadDefaultHotkeysToListView();
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            SaveHotkeysToIniFile();
            MessageBox.Show("Hotkeys saved successfully!", "Hotkey Config", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }


        private void btnCancel_Click(object sender, EventArgs e)
        {
            Close();
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
        }

        private void UpdateSelectedHotkey()
        {
            if (listViewHotkeys.SelectedItems.Count > 0)
            {
                selectedItem = listViewHotkeys.SelectedItems[0];

                // Combine the key with modifiers
                if (comboBoxChooseKey.SelectedItem is Keys key)
                {
                    selectedKey = key;

                    if (checkModCtrl.Checked) selectedKey |= Keys.Control;
                    if (checkModShift.Checked) selectedKey |= Keys.Shift;
                    if (checkModAlt.Checked) selectedKey |= Keys.Alt;

                    // Update the ListView hotkey display with the formatted string
                    lblChosenHotkey.Text = FormatHotkeyString(selectedKey);
                }
            }
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

        private void LoadDefaultHotkeysToListView()
        {
            //listViewHotkeys.Items[0].SubItems[1].Text = HotkeyConfig.ToggleScreenStartHotkey.ToString();
            //listViewHotkeys.Items[1].SubItems[1].Text = HotkeyConfig.ToggleScreenStartHotkey.ToString();
            //listViewHotkeys.Items[2].SubItems[1].Text = HotkeyConfig.ToggleScreenStartHotkey.ToString();
            //listViewHotkeys.Items[3].SubItems[1].Text = HotkeyConfig.ToggleScreenStartHotkey.ToString();
            //listViewHotkeys.Items[4].SubItems[1].Text = HotkeyConfig.ToggleScreenStartHotkey.ToString();
        }

        private void UpdateKey()
        {
            if (listViewHotkeys.SelectedItems.Count > 0)
            {
                // Update the ListView's hotkey column
                selectedItem = listViewHotkeys.SelectedItems[0];
                selectedItem.SubItems[1].Text = FormatHotkeyString(selectedKey);
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