using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SLXEditor
{
    public partial class FormBatchExporter : Form
    {
        INILevelExporter Exporter;
        SLXEditForm MainForm;

        internal FormBatchExporter(SLXEditForm parentForm)
        {
            InitializeComponent();
            MainForm = parentForm;
        }

        private bool UpdateExportButtonAvailability()
        {
            if (listViewLevels.Items.Count > 0)
            {
                if (rbINI.Checked || rbRLV.Checked)
                {
                    foreach (ListViewItem item in listViewLevels.Items)
                        return item.SubItems[1].Text != "<Select style>";
                }
                else if (rbNXLV.Checked || rbSXLV.Checked)
                    return true;
            }
            return false;
        }

        private void RemoveSelectedLevels()
        {
            foreach (ListViewItem item in listViewLevels.SelectedItems)
            {
                item.Remove();
            }
        }

        private void UpdateControls()
        {
            listViewLevels.Columns[1].Width = (rbINI.Checked || rbRLV.Checked) ? 150 : 1;

            btnRemoveLevels.Enabled = listViewLevels.SelectedItems.Count > 0;
            btnClearList.Enabled = listViewLevels.Items.Count > 0;

            lblSelectOutputStyle.Visible = (rbINI.Checked || rbRLV.Checked) && (listViewLevels.Items.Count > 0);
            comboStyles.Visible = (rbINI.Checked || rbRLV.Checked) && (listViewLevels.Items.Count > 0);

            btnExport.Enabled = UpdateExportButtonAvailability();
        }

        private void AddLevelsViaBrowser()
        {
            using (OpenFileDialog ofd = new OpenFileDialog())
            {
                ofd.Title = "Select Levels to Export";
                ofd.InitialDirectory = C.AppPathLevels;
                ofd.Filter = "Levels|*.nxlv;*.sxlv|All files|*.*";
                ofd.Multiselect = true;

                if (ofd.ShowDialog() != DialogResult.OK)
                    return;

                foreach (string file in ofd.FileNames)
                {
                    string levelName = Path.GetFileName(file);

                    // Only add if not already in the list
                    if (!listViewLevels.Items.Cast<ListViewItem>()
                                             .Any(i => i.Text.Equals(levelName, StringComparison.OrdinalIgnoreCase)))
                    {
                        var item = new ListViewItem(levelName);
                        // Default style (empty) — will select later
                        item.SubItems.Add("<Select style>");
                        // store full path in Tag
                        item.Tag = file;
                        listViewLevels.Items.Add(item);
                    }
                }

                UpdateControls();
            }
        }

        private void UpdateOutputStyles()
        {
            if (comboStyles.SelectedIndex <= 0)
                return;

            if (listViewLevels.SelectedItems.Count <= 0)
                foreach (ListViewItem item in listViewLevels.Items)
                    item.SubItems[1].Text = comboStyles.Text;
            else
                foreach (ListViewItem item in listViewLevels.SelectedItems)
                    item.SubItems[1].Text = comboStyles.Text;
        }

        private async Task ExportAllLevels()
        {
            var items = listViewLevels.Items
                                      .Cast<ListViewItem>()
                                      .Where(i => i.Tag is string)
                                      .ToArray();
            if (items.Length == 0)
                return;

            string chosenExt = null;
            if (rbINI.Checked) chosenExt = ".ini";
            else if (rbRLV.Checked) chosenExt = ".rlv";
            else if (rbNXLV.Checked) chosenExt = ".nxlv";
            else if (rbSXLV.Checked) chosenExt = ".sxlv";

            var failedExports = new List<Tuple<string, string>>();

            using (FormProgress progressForm = new FormProgress())
            {
                progressForm.ProgressBar.Maximum = items.Length;
                progressForm.Show(this);

                for (int index = 0; index < items.Length; index++)
                {
                    var item = items[index];
                    string file = item.Tag as string;

                    try
                    {
                        // Load
                        Level level = LevelFile.LoadLevelFromFile(file, MainForm.StyleList, MainForm.Backgrounds);

                        // Update extension
                        String ext = chosenExt;

                        // Export to INI/RLV
                        if (rbINI.Checked || rbRLV.Checked)
                        {
                            String selectedStyle = item.SubItems[1].Text;
                            if (selectedStyle == null || selectedStyle.Contains("<Select style>"))
                            {
                                failedExports.Add(Tuple.Create(file, $" - error: invalid style ({selectedStyle})"));
                                continue;
                            }

                            Exporter.ExportLevelToIni(level, selectedStyle, Path.ChangeExtension(file, ext));
                        }
                        // Save as SXLV/NXLV (this is essentially CleanseLevels without the reporting)
                        else
                        {
                            if (!MainForm.CanSaveToEitherFormat(level)) ext = ".sxlv"; // Override if the level contains SuperLemmix-specific features
                            if (ext != null)
                            {
                                level.FilePathToSave = Path.Combine(
                                    Path.GetDirectoryName(file),
                                    Path.GetFileNameWithoutExtension(file) + ext
                                );

                                LevelFile.SaveLevelToFile(level.FilePathToSave, level);
                            }
                            else
                            {
                                failedExports.Add(Tuple.Create(file, $"Invalid extension ({ext})"));
                                continue;
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        failedExports.Add(Tuple.Create(file, ex.Message));
                        continue;
                    }

                    // Progress update
                    progressForm.UpdateProgress(index + 1,
                        $"Processing {index + 1} of {items.Length}: {Path.GetFileName(file)}"
                    );

                    // Reset Editor before moving on to the next level
                    MainForm.CreateNewLevelAndRenderer();

                    // Let UI update
                    await Task.Delay(10);
                }

                progressForm.Close();

                // Re-initialize the Editor
                MainForm.CreateNewLevelAndRenderer();
            }

            if (failedExports.Count > 0)
            {
                string reportPath = Path.Combine(C.AppPath, "BatchExportErrors.txt");

                var lines = new List<string>();

                foreach (var entry in failedExports)
                {
                    lines.Add("File: " + Path.GetFileName(entry.Item1));
                    lines.Add("Error: " + entry.Item2);
                    lines.Add("");
                }

                System.IO.File.WriteAllLines(reportPath, lines);

                MessageBox.Show(
                    "Some levels failed to export.\n\n" +
                    $"A report has been saved to:\n{reportPath}",
                    "Batch Export Completed with Errors",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning
                );
            }
            else
            {
                MessageBox.Show(
                    "Batch export complete!",
                    "Export Finished",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information
                );
            }
        }

        private void FormBatchExporter_Load(object sender, EventArgs e)
        {
            Exporter = new INILevelExporter();

            var styles = Exporter.LoadTranslationStyles();
            comboStyles.Items.Clear();
            comboStyles.Items.Add("<Select style>");
            comboStyles.Items.AddRange(styles.ToArray());

            if (comboStyles.Items.Count > 0)
                comboStyles.SelectedIndex = 0;

            UpdateControls();
        }
        private void radioFormatCheckedChanged(object sender, EventArgs e)
        {
            UpdateControls();
        }

        private void btnAddLevels_Click(object sender, EventArgs e)
        {
            AddLevelsViaBrowser();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            Close();
        }

        private async void btnExport_Click(object sender, EventArgs e)
        {
            await ExportAllLevels();
        }

        private void comboStyles_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateOutputStyles();
            UpdateControls();
        }

        private void listViewLevels_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateControls();
        }

        private void btnRemoveLevels_Click(object sender, EventArgs e)
        {
            RemoveSelectedLevels();
        }

        private void btnClearList_Click(object sender, EventArgs e)
        {
            listViewLevels.Items.Clear();
        }
    }
}
