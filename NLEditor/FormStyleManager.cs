using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using static NLEditor.FormStyleManager;

namespace NLEditor
{
    public partial class FormStyleManager : Form
    {
        public class StyleEntry
        {
            public string FolderName { get; set; }
            public string DisplayName { get; set; }
            public int Order { get; set; }
            public bool PinnedTop { get; set; }
            public bool PinnedBottom { get; set; }
        }

        private List<StyleEntry> styles = new List<StyleEntry>();
        
        private string styleFilePath = C.AppPath + "styles" + C.DirSep + "styles.ini";
        
        private NLEditForm mainForm;

        internal FormStyleManager(NLEditForm parentForm)
        {
            InitializeComponent();
            mainForm = parentForm;
        }

        private void LoadStylesIntoListView()
        {
            if (!File.Exists(styleFilePath))
            {
                MessageBox.Show("Could not find styles.ini. Style Manager will now close");
                Close();
                return;
            }

            string[] lines = File.ReadAllLines(styleFilePath);

            StyleEntry current = null;

            foreach (var rawLine in lines)
            {
                string line = rawLine.Trim();
                if (line.Length == 0)
                    continue; // Skip blank lines

                // SECTION HEADER
                if (line.StartsWith("[") && line.EndsWith("]"))
                {
                    string folder = line.Trim('[', ']').Trim();

                    // Ignore empty data
                    if (string.IsNullOrWhiteSpace(folder))
                        continue;

                    // Prevent duplicates
                    if (styles.Any(s => s.FolderName.Equals(folder, StringComparison.OrdinalIgnoreCase)))
                        continue;

                    current = new StyleEntry
                    {
                        FolderName = folder,
                        DisplayName = folder, // Default until overwritten
                        Order = 0
                    };

                    styles.Add(current);
                    continue;
                }

                // Ignore data that does not belong to a section
                if (current == null)
                    continue;

                // NAME
                if (line.StartsWith("Name=", StringComparison.OrdinalIgnoreCase))
                {
                    current.DisplayName = line.Substring(5).Trim();
                    if (string.IsNullOrWhiteSpace(current.DisplayName))
                        current.DisplayName = current.FolderName;
                    continue;
                }

                // ORDER
                if (line.StartsWith("Order=", StringComparison.OrdinalIgnoreCase))
                {
                    string num = line.Substring(6).Trim();

                    if (double.TryParse(num, // Parse as double for backwards compatibility
                        System.Globalization.NumberStyles.Any,
                        System.Globalization.CultureInfo.InvariantCulture,
                        out double dval))
                    {
                        int val = (int)dval; // Convert to integer

                        if (val < 0) val = 0;
                        if (val > 1000000) val = 1000000;

                        current.Order = val;
                    }
                    else
                    {
                        current.Order = 0;
                    }

                    continue;
                }

                // PINNED
                if (line.StartsWith("Pinned=") && current != null)
                {
                    if (int.TryParse(line.Substring(7).Trim(), out int pval))
                    {
                        switch (pval)
                        {
                            case 1:
                                current.PinnedTop = true;
                                current.PinnedBottom = false;
                                break;
                            case 2:
                                current.PinnedTop = false;
                                current.PinnedBottom = true;
                                break;
                            default:
                                current.PinnedTop = false;
                                current.PinnedBottom = false;
                                break;
                        }
                    }
                    else
                    {
                        current.PinnedTop = false;
                        current.PinnedBottom = false;
                    }
                }
            }

            // Sort entries
            styles = styles.OrderBy(s => s.Order).ToList();

            // Populate list view
            foreach (var s in styles)
            {
                var item = new ListViewItem(s.FolderName);
                item.SubItems.Add(s.DisplayName);
                item.SubItems.Add(s.PinnedTop ? "↑" : s.PinnedBottom ? "↓" : "");
                item.Tag = s;
                listStyles.Items.Add(item);
            }
        }

        private void PerformStyleSearch()
        {
            string search = txtSearch.Text.Trim().ToLowerInvariant();

            UpdateButtonsAfterSearch(search != string.Empty);

            listStyles.BeginUpdate();
            listStyles.Items.Clear();

            foreach (var s in styles)
            {
                // Match FolderName or DisplayName
                if (string.IsNullOrEmpty(search) ||
                    s.FolderName.ToLowerInvariant().Contains(search) ||
                    s.DisplayName.ToLowerInvariant().Contains(search))
                {
                    var item = new ListViewItem(s.FolderName);
                    item.SubItems.Add(s.DisplayName);
                    item.SubItems.Add(s.PinnedTop ? "↑" : s.PinnedBottom ? "↓" : "");

                    // Store the reference to the original StyleEntry
                    item.Tag = s;

                    listStyles.Items.Add(item);
                }
            }

            listStyles.EndUpdate();
        }
        private void ShowSelectedItemsInList()
        {
            if (listStyles.SelectedItems.Count == 0)
                return;

            // Get all selected StyleEntry objects from tags
            var selectedStyles = listStyles.SelectedItems
                .Cast<ListViewItem>()
                .Select(item => item.Tag as StyleEntry)
                .Where(s => s != null)
                .ToList();

            if (selectedStyles.Count == 0)
                return;

            // Clear search to restore full list
            txtSearch.Clear();

            // Rebuild ListView with tags
            listStyles.BeginUpdate();
            listStyles.Items.Clear();
            foreach (var s in styles)
            {
                var item = new ListViewItem(s.FolderName);
                item.SubItems.Add(s.DisplayName);
                item.Tag = s;
                listStyles.Items.Add(item);
            }
            listStyles.EndUpdate();

            // Restore selection
            foreach (var s in selectedStyles)
            {
                int idx = styles.IndexOf(s);
                if (idx >= 0)
                    listStyles.Items[idx].Selected = true;
            }

            // Scroll to the first selected item
            int firstIndex = styles.IndexOf(selectedStyles[0]);
            if (firstIndex >= 0)
                listStyles.Items[firstIndex].EnsureVisible();

            listStyles.Focus();
        }

        private void UpdateButtonsAfterSearch(bool searchActive)
        {
            if (searchActive)
            {
                btnClearSearch.Enabled = true;
                btnShowSelectedItemsInList.Enabled = true;
                btnShowSelectedItemsInList.Visible = true;
                btnAddNew.Enabled = false;
                btnAddNew.Visible = false;
                btnMoveUp1.Enabled = false;
                btnMoveUp1.Visible = false;
                btnMoveUp10.Enabled = false;
                btnMoveUp10.Visible = false;
                btnMoveDown1.Enabled = false;
                btnMoveDown1.Visible = false;
                btnMoveDown10.Enabled = false;
                btnMoveDown10.Visible = false;
                btnPinToTop.Enabled = false;
                btnPinToTop.Visible = false;
                btnPinToBottom.Enabled = false;
                btnPinToBottom.Visible = false;
            }
            else
            {
                btnClearSearch.Enabled = false;
                btnShowSelectedItemsInList.Enabled = false;
                btnShowSelectedItemsInList.Visible = false;
                btnAddNew.Enabled = true;
                btnAddNew.Visible = true;
                btnMoveUp1.Enabled = true;
                btnMoveUp1.Visible = true;
                btnMoveUp10.Enabled = true;
                btnMoveUp10.Visible = true;
                btnMoveDown1.Enabled = true;
                btnMoveDown1.Visible = true;
                btnMoveDown10.Enabled = true;
                btnMoveDown10.Visible = true;
                btnPinToTop.Enabled = true;
                btnPinToTop.Visible = true;
                btnPinToBottom.Enabled = true;
                btnPinToBottom.Visible = true;
            }
        }

        /// <summary>
        /// Style renaming
        /// </summary>
        private void RenameStyle()
        {
            if (listStyles.SelectedItems.Count != 1)
            {
                MessageBox.Show("Please select a single style to rename.", "Rename", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            var selectedItem = listStyles.SelectedItems[0];
            if (!(selectedItem.Tag is StyleEntry style))
                return;

            string newName = (txtDisplayName.Text ?? string.Empty).Trim();
            if (string.IsNullOrWhiteSpace(newName))
            {
                MessageBox.Show("Display name cannot be empty.", "Rename", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtDisplayName.Focus();
                return;
            }

            // Update the master data
            style.DisplayName = newName;

            // Update the ListView
            if (selectedItem.SubItems.Count < 2)
                selectedItem.SubItems.Add(newName);
            else
                selectedItem.SubItems[1].Text = newName;

            // Keep selection and focus consistent
            selectedItem.Selected = true;
            listStyles.Focus();
        }

        private void UpdateDisplayNameTextInput()
        {
            if (listStyles.SelectedItems.Count != 1)
            {
                txtDisplayName.Text = string.Empty;
                btnRename.Enabled = false;
                return;
            }

            var selectedItem = listStyles.SelectedItems[0];
            if (!(selectedItem.Tag is StyleEntry style))
            {
                txtDisplayName.Text = string.Empty;
                btnRename.Enabled = false;
                return;
            }

            txtDisplayName.Text = style.DisplayName ?? style.FolderName;
            btnRename.Enabled = true;
            txtDisplayName.Focus();
            txtDisplayName.SelectAll();
        }

        /// <summary>
        /// Moving styles
        /// </summary>
        private void MoveStylesUp(object sender)
        {
            if (listStyles.SelectedIndices.Count == 0) return;

            int moveAmount = (sender == btnMoveUp10) ? 10 : 1;

            // Sort indices ascending for moving up
            var indices = listStyles.SelectedIndices.Cast<int>().OrderBy(i => i).ToList();

            foreach (int index in indices)
            {
                // Determine top limit depending on pinned status
                var style = styles[index];
                int minIndex;

                if (style.PinnedTop)
                    minIndex = 0;
                else if (style.PinnedBottom)
                    minIndex = styles.FindIndex(s => s.PinnedBottom);
                else
                    minIndex = styles.FindIndex(s => !s.PinnedTop);

                int targetIndex = Math.Max(index - moveAmount, minIndex);
                if (targetIndex == index) continue;

                // Move in data list
                var tmp = styles[index];
                styles.RemoveAt(index);
                styles.Insert(targetIndex, tmp);

                // Move in ListView
                var item = listStyles.Items[index];
                listStyles.Items.RemoveAt(index);
                listStyles.Items.Insert(targetIndex, item);
                item.Selected = true;
            }

            listStyles.Focus();
        }
        private void MoveStylesDown(object sender)
        {
            if (listStyles.SelectedIndices.Count == 0) return;

            int moveAmount = (sender == btnMoveDown10) ? 10 : 1;

            // Sort indices descending for moving down
            var indices = listStyles.SelectedIndices.Cast<int>().OrderByDescending(i => i).ToList();

            foreach (int index in indices)
            {
                // Determine bottom limit depending on pinned status
                var style = styles[index];
                int maxIndex;

                if (style.PinnedBottom)
                    maxIndex = styles.Count - 1;
                else if (style.PinnedTop)
                    maxIndex = styles.FindLastIndex(s => s.PinnedTop);
                else
                    maxIndex = styles.FindLastIndex(s => !s.PinnedBottom);

                int targetIndex = Math.Min(index + moveAmount, maxIndex);
                if (targetIndex == index) continue;

                // Move in data list
                var tmp = styles[index];
                styles.RemoveAt(index);
                styles.Insert(targetIndex, tmp);

                // Move in ListView
                var item = listStyles.Items[index];
                listStyles.Items.RemoveAt(index);
                listStyles.Items.Insert(targetIndex, item);
                item.Selected = true;
            }

            listStyles.Focus();
        }

        /// <summary>
        /// Refreshes the list view
        /// </summary>
        private void RebuildListView()
        {
            txtSearch.Clear(); // Just in case

            listStyles.BeginUpdate();
            listStyles.Items.Clear();

            foreach (var s in styles)
            {
                var item = new ListViewItem(s.FolderName);
                item.SubItems.Add(s.DisplayName);
                item.SubItems.Add(s.PinnedTop ? "↑" : s.PinnedBottom ? "↓" : "");
                item.Tag = s;
                listStyles.Items.Add(item);
            }

            listStyles.EndUpdate();
        }

        /// <summary>
        /// Style pinning
        /// </summary>
        private void PinStylesToTopOfList()
        {
            if (listStyles.SelectedIndices.Count == 0)
                return;

            var selectedStyles = listStyles.SelectedIndices
                .Cast<int>()
                .OrderBy(i => i)
                .Select(i => styles[i])
                .ToList();

            foreach (var s in selectedStyles)
            {
                s.PinnedTop = true;
                s.PinnedBottom = false; // Ensure only one pin type is active
            }

            foreach (var s in selectedStyles)
                styles.Remove(s);

            styles.InsertRange(0, selectedStyles);

            RebuildListView();

            foreach (var s in selectedStyles)
            {
                int idx = styles.IndexOf(s);
                if (idx >= 0)
                    listStyles.Items[idx].Selected = true;
            }

            listStyles.EnsureVisible(0);
            listStyles.Focus();
        }

        private void PinStylesToBottomOfList()
        {
            if (listStyles.SelectedIndices.Count == 0)
                return;

            var selected = listStyles.SelectedIndices
                .Cast<int>()
                .OrderBy(i => i)
                .Select(i => styles[i])
                .ToList();

            foreach (var s in selected)
            {
                s.PinnedBottom = true;
                s.PinnedTop = false; // Ensure only one pin type is active
            }

            foreach (var s in selected)
                styles.Remove(s);

            styles.AddRange(selected);

            RebuildListView();

            foreach (var s in selected)
            {
                int idx = styles.IndexOf(s);
                listStyles.Items[idx].Selected = true;
            }

            listStyles.EnsureVisible(styles.Count - 1);
            listStyles.Focus();
        }

        /// <summary>
        /// Sorts all unpinned styles alphabetically
        /// </summary>
        private void SortAllStylesAlphabetically()
        {
            var pinnedTop = styles.Where(s => s.PinnedTop).ToList();
            var unpinned = styles.Where(s => !s.PinnedTop && !s.PinnedBottom)
                                 .OrderBy(s => s.FolderName, StringComparer.OrdinalIgnoreCase)
                                 .ToList();
            var pinnedBottom = styles.Where(s => s.PinnedBottom).ToList();
            
            styles = pinnedTop.Concat(unpinned).Concat(pinnedBottom).ToList();
            RebuildListView();
        }

        /// <summary>
        /// Style adding
        /// </summary>
        private void AddNewStyle() // TODO - Improve folder browser
        {
            using (var fbd = new FolderBrowserDialog())
            {
                fbd.Description = "Select a new style folder to add";
                fbd.SelectedPath = Path.Combine(C.AppPath, "styles");
                fbd.ShowNewFolderButton = true;

                if (fbd.ShowDialog() != DialogResult.OK)
                    return;

                string selectedFolderPath = fbd.SelectedPath;
                string folderName = Path.GetFileName(
                    selectedFolderPath.TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar));

                if (string.IsNullOrWhiteSpace(folderName))
                {
                    MessageBox.Show("Invalid folder selected.", "Add Style",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Check for duplicates
                int existingIndex = styles.FindIndex(s =>
                    string.Equals(s.FolderName, folderName, StringComparison.OrdinalIgnoreCase));

                if (existingIndex >= 0)
                {
                    // Already exists — select it
                    listStyles.Items[existingIndex].Selected = true;
                    listStyles.EnsureVisible(existingIndex);
                    txtDisplayName.Text = styles[existingIndex].DisplayName;
                    btnRename.Enabled = true;
                    return;
                }

                // Create new style
                var newStyle = new StyleEntry
                {
                    FolderName = folderName,
                    DisplayName = folderName,
                    PinnedTop = false,
                    PinnedBottom = false
                };

                // -------------------------------------------------------------
                // INSERTION LOGIC:
                //   1. Pinned styles stay at top (keep user order).
                //   2. Insert new style alphabetically *after* the pinned block.
                // -------------------------------------------------------------

                // Count pinned styles
                int pinnedCount = styles.Count(s => s.PinnedTop);

                // Find alphabetical position among unpinned styles
                int relativeIndex = styles
                    .Skip(pinnedCount)
                    .ToList()
                    .FindIndex(s =>
                        string.Compare(s.FolderName, folderName,
                        StringComparison.OrdinalIgnoreCase) > 0);

                // Convert to absolute index
                int insertIndex = (relativeIndex >= 0)
                    ? pinnedCount + relativeIndex
                    : styles.Count;

                // Insert into data list
                styles.Insert(insertIndex, newStyle);

                // Insert into ListView
                var item = new ListViewItem(newStyle.FolderName);
                item.SubItems.Add(newStyle.DisplayName);
                listStyles.Items.Insert(insertIndex, item);

                // Select the new item
                item.Selected = true;
                listStyles.EnsureVisible(insertIndex);
                txtDisplayName.Text = newStyle.DisplayName;
                btnRename.Enabled = true;
                txtDisplayName.Focus();
                txtDisplayName.SelectAll();
            }
        }

        /// <summary>
        /// Saves the styles data list to styles.ini
        /// </summary>
        private void SaveStylesList()
        {
            var result = MessageBox.Show(
                "Are you sure you want to save the current style list?",
                "Confirm Save",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question
            );

            if (result != DialogResult.Yes)
                return;

            if (styles.Count == 0)
            {
                MessageBox.Show("No styles to save.", "Save", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            StringBuilder sb = new StringBuilder();

            int orderValue = 1;

            foreach (var style in styles)
            {
                string folder = style.FolderName?.Trim();
                if (string.IsNullOrWhiteSpace(folder))
                    folder = "Unknown";

                string display = (style.DisplayName ?? folder).Trim();

                int pinnedVal = style.PinnedTop ? 1 : (style.PinnedBottom ? 2 : 0);

                sb.AppendLine($"[{folder}]");
                sb.AppendLine($"Name={display}");
                sb.AppendLine($"Order={orderValue}");
                sb.AppendLine($"Pinned={pinnedVal}");
                sb.AppendLine("");

                orderValue++;
            }

            try
            {
                File.WriteAllText(styleFilePath, sb.ToString());
                MessageBox.Show("List successfully saved to styles.ini");
                mainForm.RefreshStyles();
                // Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to save styles.ini:\n" + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ListStyles_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateDisplayNameTextInput();
        }

        private void FormStyleManager_Load(object sender, EventArgs e)
        {
            LoadStylesIntoListView();
        }

        private void BtnMoveUp_Click(object sender, EventArgs e)
        {
            MoveStylesUp(sender);
        }

        private void BtnMoveDown_Click(object sender, EventArgs e)
        {
            MoveStylesDown(sender);
        }

        private void BtnRename_Click(object sender, EventArgs e)
        {
            RenameStyle();
        }

        private void BtnSave_Click(object sender, EventArgs e)
        {
            SaveStylesList();
        }

        private void BtnAddNew_Click(object sender, EventArgs e)
        {
            AddNewStyle();
        }

        private void txtDisplayName_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
                RenameStyle();
        }

        private void BtnCancel_Click(object sender, EventArgs e)
        {
            Close(); // Close without saving
        }

        private void btnSortAlphabetically_Click(object sender, EventArgs e)
        {
            SortAllStylesAlphabetically();
        }

        private void btnPinToTop_Click(object sender, EventArgs e)
        {
            PinStylesToTopOfList();
        }

        private void btnPinToBottom_Click(object sender, EventArgs e)
        {
            PinStylesToBottomOfList();
        }

        private void FormStyleManager_Click(object sender, EventArgs e)
        {
            btnAddNew.Focus();
        }

        private void FormStyleManager_Shown(object sender, EventArgs e)
        {
            btnAddNew.Focus();
        }

        private void txtSearch_TextChanged(object sender, EventArgs e)
        {
            PerformStyleSearch();
        }

        private void btnShowSelectedItemsInList_Click(object sender, EventArgs e)
        {
            ShowSelectedItemsInList();
        }

        private void btnClearSearch_Click(object sender, EventArgs e)
        {
            txtSearch.Clear();
        }
    }
}
