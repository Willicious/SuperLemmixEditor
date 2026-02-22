using System;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace SLXEditor
{
    public partial class FormTemplates : Form
    {
        SLXEditForm mainForm;
        Settings curSettings;

        class TemplateInfo
        {
            public string FilePath { get; set; }
            public string FileName { get; set; }
            public string DisplayTitle { get; set; }
            public Level TemplateLevel { get; set; }

            public TemplateInfo(string filePath, string fileName, string displayTitle, Level level)
            {
                FilePath = filePath;
                FileName = fileName;
                DisplayTitle = displayTitle;
                TemplateLevel = level;
            }
            public override string ToString()
            {
                return DisplayTitle;
            }
        }

        internal FormTemplates(SLXEditForm parentForm, Settings settings)
        {
            InitializeComponent();
            mainForm = parentForm;
            curSettings = settings;
        }

        private void PopulateTemplatesList(int index = 0)
        {
            listTemplates.Items.Clear();

            try
            {
                if (!Directory.Exists(C.AppPathTemplates))
                {
                    Directory.CreateDirectory(C.AppPathTemplates);
                    return;
                }

                var files = Directory.GetFiles(
                    C.AppPathTemplates,
                    "*.*",
                    SearchOption.TopDirectoryOnly)
                    .Where(f => f.EndsWith(".template", StringComparison.OrdinalIgnoreCase));

                foreach (var file in files)
                {
                    try
                    {
                        Level level = LevelFile.LoadMetaData(file, mainForm.StyleList, mainForm.Backgrounds);

                        string displayTitle = string.IsNullOrWhiteSpace(level.Title)
                            ? Path.GetFileNameWithoutExtension(file)
                            : level.Title;

                        string name = Path.GetFileNameWithoutExtension(file);

                        if (curSettings.DefaultTemplate == name)
                            displayTitle = displayTitle + " (Default)";

                        var templateInfo = new TemplateInfo(file, name, displayTitle, level);
                        listTemplates.Items.Add(templateInfo);
                    }
                    catch
                    {
                        // Skip unloadable templates silently
                    }
                }

                // Select the given index (first item by default)
                int count = listTemplates.Items.Count;
                if (count > 0)
                    listTemplates.SelectedIndex = index >= count ? count - 1 : index;
            }
            catch (Exception ex)
            {
                Utility.LogException(ex);
                MessageBox.Show("Error loading templates:\n" + ex.Message);
            }
        }

        private bool noTemplatesFound = false;

        private void ClearDataPanel()
        {
            rtLevelData.Clear();
            rtSkillSetData.Clear();

            if (listTemplates.Items.Count == 0)
            {
                listTemplates.Items.Add("No templates found...");
                labelTitle.Text = "To add a template, create a level and choose Save As Template.";
                btnLoadTemplate.Visible = false;
                btnDelete.Visible = false;
                noTemplatesFound = true;
                listTemplates.Focus();
                return;
            }
        }

        private void PopulateTemplateDataPanel()
        {
            ClearDataPanel();

            if (noTemplatesFound)
                return;

            var templateInfo = (TemplateInfo)listTemplates.SelectedItem;

            if (templateInfo == null)
                return;

            Level level = templateInfo.TemplateLevel;

            bool levelIsDefault = curSettings.DefaultTemplate == templateInfo.FileName;
            btnSetAsDefault.Enabled = !levelIsDefault;

            if (!string.IsNullOrWhiteSpace(level.Title))
                labelTitle.Text = level.Title;
            else
                labelTitle.Text = templateInfo.DisplayTitle;

            void AddLine(string label, string value)
            {
                rtLevelData.SelectionFont = new Font(rtLevelData.Font, FontStyle.Bold);
                rtLevelData.AppendText(label);

                rtLevelData.SelectionFont = rtLevelData.Font;
                rtLevelData.AppendText(value + Environment.NewLine);
            }

            string YesNo(bool value) => value ? "Yes" : "No";

            // --- Basic Info ---
            AddLine("Author: ", level.Author);
            AddLine("Theme: ", level.MainStyle?.NameInEditor ?? "None");
            rtLevelData.AppendText(Environment.NewLine);

            // --- Size / Start ---
            AddLine("Size: ", $"{level.Width} x {level.Height}");
            AddLine("Start: ", $"{level.StartPosX}, {level.StartPosY}");
            rtLevelData.AppendText(Environment.NewLine);

            // --- Lemmings ---
            AddLine("Lemmings: ", $"{level.NumLems}");
            AddLine("Save: ", $"{level.SaveReq}");
            rtLevelData.AppendText(Environment.NewLine);

            // --- RR/SI / Time ---
            string textRRSI = curSettings.UseSpawnInterval ? "Spawn Interval" : "Release Rate";
            string valRRSI = curSettings.UseSpawnInterval ? $"{level.SpawnInterval}" : $"{level.ReleaseRate}";
            string lockedText = level.IsSpawnRateFix ? "(Locked)" : "";

            AddLine($"{textRRSI}: ", $"{valRRSI} {lockedText}");
            AddLine($"Time Limit: ", level.HasTimeLimit ? level.TimeLimit.ToString() : "Infinite");
            rtLevelData.AppendText(Environment.NewLine);

            // --- Flags ---
            string steelType = level.SteelType == 0 ? "Where Visible" : "Always";
            AddLine("Steel Type: ", $"{steelType}");
            AddLine("SuperLemming: ", $"{YesNo(level.IsSuperlemming)}");
            AddLine("Invincibility: ", $"{YesNo(level.IsInvincibility)}");

            // Reset selection
            rtLevelData.SelectionStart = 0;
            rtLevelData.SelectionLength = 0;

            // --- Skillset ---
            rtSkillSetData.SelectionFont = new Font(rtLevelData.Font, FontStyle.Bold);
            rtSkillSetData.AppendText("Skillset:" + Environment.NewLine);

            rtSkillSetData.SelectionFont = rtSkillSetData.Font;

            foreach (var skill in level.SkillSet)
            {
                if (skill.Value > 0)
                {
                    rtSkillSetData.AppendText($"    {skill.Key}: {skill.Value}{Environment.NewLine}");
                }
            }

            if (!level.SkillSet.Any(s => s.Value > 0))
            {
                rtSkillSetData.AppendText("    None" + Environment.NewLine);
            }

            // Reset selection
            rtSkillSetData.SelectionStart = 0;
            rtSkillSetData.SelectionLength = 0;

            // Finally, load the preview image
            LoadTemplatePreviewImage(templateInfo.FilePath);
        }

        private void LoadTemplatePreviewImage(string levelFilePath)
        {
            picPreview.Image?.Dispose();
            picPreview.Image = null;

            if (string.IsNullOrWhiteSpace(levelFilePath))
                return;

            string pngPath = Path.ChangeExtension(levelFilePath, ".png");

            if (!File.Exists(pngPath))
                return;

            try
            {
                using (var fs = new FileStream(pngPath, FileMode.Open, FileAccess.Read))
                {
                    picPreview.Image = Image.FromStream(fs);
                }
            }
            catch
            {
                // Silently ignore
            }
        }

        private Level LoadTemplateAsNewLevel()
        {
            if (listTemplates.SelectedItem == null)
            {
                MessageBox.Show("No template selected.");
                return null;
            }

            var templateInfo = (TemplateInfo)listTemplates.SelectedItem;            
            if (templateInfo == null)
            {
                MessageBox.Show("The level cannot be loaded due to an error with the template info.");
                return null;
            }

            string filePath = templateInfo.FilePath;
            Level newLevel = null;

            try
            {
                mainForm.LoadNewLevel(filePath);
                mainForm.CurLevel.FilePathToSave = null;
                mainForm.LevelDirectory = C.AppPathLevels;
            }
            catch (Exception Ex)
            {
                Utility.LogException(Ex);
                MessageBox.Show("Error while loading the level." + C.NewLine + Ex.Message, "Level load error");
                return newLevel;
            }

            return newLevel;
        }

        private void DeleteSelectedTemplate()
        {
            if (listTemplates.SelectedItem == null)
                return;

            var templateInfo = (TemplateInfo)listTemplates.SelectedItem;
            int index = listTemplates.SelectedIndex;

            try
            {
                // Delete the level file
                if (File.Exists(templateInfo.FilePath))
                    File.Delete(templateInfo.FilePath);

                // Delete the PNG preview
                string pngPath = Path.ChangeExtension(templateInfo.FilePath, ".png");
                if (File.Exists(pngPath))
                    File.Delete(pngPath);
            }
            catch (Exception ex)
            {
                Utility.LogException(ex);
                MessageBox.Show("Error deleting template: " + ex.Message);
                return;
            }

            // Refresh the list
            PopulateTemplatesList(index);
        }

        private void SetCurrentTemplateAsDefault()
        {
            if (listTemplates.SelectedItem == null)
                return;

            var templateInfo = (TemplateInfo)listTemplates.SelectedItem;
            int index = listTemplates.SelectedIndex;

            curSettings.DefaultTemplate = templateInfo.FileName;
            curSettings.WriteSettingsToFile();
            btnSetAsDefault.Enabled = false;
            PopulateTemplatesList(index);
        }

        private void FormTemplates_Load(object sender, EventArgs e)
        {
            PopulateTemplatesList();
            PopulateTemplateDataPanel();
        }

        private void btnLoadTemplate_Click(object sender, EventArgs e)
        {
            LoadTemplateAsNewLevel();
            Close();
        }

        private void listTemplates_SelectedIndexChanged(object sender, EventArgs e)
        {
            PopulateTemplateDataPanel();
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            DeleteSelectedTemplate();
        }

        private void btnSetAsDefault_Click(object sender, EventArgs e)
        {
            SetCurrentTemplateAsDefault();
        }
    }
}
