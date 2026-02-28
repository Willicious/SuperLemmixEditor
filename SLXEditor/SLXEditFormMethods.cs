using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static SLXEditor.HotkeyConfig;

namespace SLXEditor
{
    partial class SLXEditForm
    {
        /*---------------------------------------------------------
         *   Main Form: This part defines the methods
         *     called from user input
         * -------------------------------------------------------- */

        /// <summary>
        /// Reads the user's settings from the file SLXEditorSettings.ini and applies these options.
        /// </summary>
        private void InitializeSettings()
        {
            curSettings.ReadSettingsFromFile();

            snapToGridToolStripMenuItem.Checked = curSettings.UseGridForPieces;
            highlightGroupedPiecesToolStripMenuItem.Checked = BmpModify.HighlightGroups;
            highlightEraserPiecesToolStripMenuItem.Checked = BmpModify.HighlightErasers;

            UpdateRRSIControls();
        }

        /// <summary>
        /// Sets fStyleList and creates the styles, but does not yet load sprites.
        /// </summary>
        private void CreateStyleList()
        {
            Backgrounds = new BackgroundList();

            // get list of all existing style names
            List<string> styleNameList = new List<string>();

            if (System.IO.Directory.Exists(C.AppPathStyles))
            {
                try
                {
                    styleNameList = System.IO.Directory.GetDirectories(C.AppPathStyles)
                                                       .Select(dir => System.IO.Path.GetFileName(dir))
                                                       .ToList();
                }
                catch (Exception Ex)
                {
                    Utility.LogException(Ex);

                    MessageBox.Show("Error: Could not read the style folders. The Editor will now close." + C.NewLine + Ex.Message, "Error loading styles");
                    throw new ApplicationException("Fatal error loading styles", Ex);
                }
            }
            else
            {
                MessageBox.Show("Error: The folder 'styles' is missing.\n\n" +
                                "Ensure that the Editor is in the same directory as SuperLemmix.exe\n" +
                                "and that the 'styles' folder is present.\n\n" +
                                "The Editor will now close.", "Styles missing");
                throw new ApplicationException("Fatal error loading styles");
            }
            // Create the StyleList from the StyleNameList
            styleNameList.RemoveAll(sty => sty == "default");
            StyleList = styleNameList.ConvertAll(sty => new Style(sty, Backgrounds, false));
            StyleList = LoadStylesFromFile.OrderAndRenameStyles(StyleList, curSettings);

            Backgrounds.SortBackgrounds();
        }

        /// <summary>
        /// Creates a simple input dialog
        /// </summary>
        public static class InputDialog
        {
            public static string Show(string prompt, string title)
            {
                Form form = new Form();
                Label label = new Label();
                TextBox textBox = new TextBox();
                Button buttonOk = new Button();
                Button buttonCancel = new Button();

                form.Text = title;
                label.Text = prompt;

                buttonOk.Text = "OK";
                buttonCancel.Text = "Cancel";
                buttonOk.DialogResult = DialogResult.OK;
                buttonCancel.DialogResult = DialogResult.Cancel;

                label.SetBounds(9, 10, 372, 13);
                textBox.SetBounds(12, 30, 372, 20);
                buttonOk.SetBounds(228, 60, 75, 23);
                buttonCancel.SetBounds(309, 60, 75, 23);

                label.AutoSize = true;
                textBox.Anchor = textBox.Anchor | AnchorStyles.Right;
                buttonOk.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
                buttonCancel.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;

                form.ClientSize = new Size(396, 100);
                form.Controls.AddRange(new Control[] { label, textBox, buttonOk, buttonCancel });
                form.FormBorderStyle = FormBorderStyle.FixedDialog;
                form.StartPosition = FormStartPosition.CenterScreen;
                form.MinimizeBox = false;
                form.MaximizeBox = false;
                form.AcceptButton = buttonOk;
                form.CancelButton = buttonCancel;

                return form.ShowDialog() == DialogResult.OK ? textBox.Text : null;
            }
        }

        /// <summary>
        /// Reduces multiple consecutive blank lines in to a single blank line.
        /// </summary>
        private static void CollapseMultipleBlankLines(string filePath)
        {
            var lines = File.ReadAllLines(filePath, Encoding.Unicode);
            var cleanedLines = new List<string>();

            bool previousWasBlank = false;

            foreach (var line in lines)
            {
                if (string.IsNullOrWhiteSpace(line))
                {
                    if (!previousWasBlank)
                    {
                        cleanedLines.Add(string.Empty); // Keep a single blank line
                        previousWasBlank = true;
                    }
                }
                else // Skip additional blank lines
                {
                    cleanedLines.Add(line);
                    previousWasBlank = false;
                }
            }

            File.WriteAllLines(filePath, cleanedLines, Encoding.Unicode);
        }

        /// <summary>
        /// Saves the currently-applied skillset as a custom set
        /// </summary>
        public void SaveSkillsetAsCustom()
        {
            try
            {
                if (!File.Exists(C.AppPathCustomSkillsets))
                {
                    MessageBox.Show("Custom skillset file was not found.", "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                // Ask for a name for the new skillset
                string newName = InputDialog.Show("Enter a name for the custom skillset:", "Save Custom Skillset");
                if (string.IsNullOrWhiteSpace(newName))
                    return;

                // Overwrite existing section if it already exists (delete it first)
                WritePrivateProfileString(newName, null, null, C.AppPathCustomSkillsets);

                // Now write the values, beginning with a blank line
                File.AppendAllText(C.AppPathCustomSkillsets, Environment.NewLine, Encoding.Unicode);
                foreach (var entry in numericsSkillSet)
                {
                    C.Skill skill = entry.Key;
                    NumericUpDown numeric = entry.Value;

                    if (numeric.Value == 0)
                        continue; // Add only non-zero skills

                    string key = skill.ToString();
                    string value = numeric.Value.ToString();

                    WritePrivateProfileString(newName, key, value, C.AppPathCustomSkillsets);
                }

                // Clean up extra blank lines
                CollapseMultipleBlankLines(C.AppPathCustomSkillsets);

                // Refresh dropdown and auto-select the new entry
                SetCustomSkillsetList();
                comboCustomSkillset.SelectedItem = newName;

                MessageBox.Show("Custom skillset saved successfully!", "Success",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error saving custom skillset:\n" + ex.Message, "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        } [DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
          private static extern bool WritePrivateProfileString(string lpAppName, string lpKeyName, string lpString, string lpFileName);


        /// <summary>
        /// Applies the chosen custom skillset to the skill selection
        /// </summary>
        public void ApplyCustomSkillset()
        {
            if (comboCustomSkillset.SelectedIndex == 0)
                return;

            try
            {
                string selectedSkillset = comboCustomSkillset.Text;

                if (string.IsNullOrWhiteSpace(selectedSkillset))
                {
                    MessageBox.Show("Please select a skillset first.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                const int bufferSize = 4096;
                byte[] buffer = new byte[bufferSize];

                int length = GetPrivateProfileSection(selectedSkillset, buffer, bufferSize, C.AppPathCustomSkillsets);
                if (length == 0 && comboCustomSkillset.SelectedIndex > 0)
                {
                    MessageBox.Show($"No skills found for skillset '{selectedSkillset}'.", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                SetAllSkillsToZero(); // Reset all skills

                string sectionData = Encoding.Unicode.GetString(buffer, 0, length * 2).Trim('\0');
                string[] skills = sectionData.Split(new char[] { '\0' }, StringSplitOptions.RemoveEmptyEntries);

                foreach (string line in skills)
                {
                    string[] parts = line.Split('=');
                    if (parts.Length != 2) continue;

                    string skillName = parts[0].Trim();
                    string skillValueStr = parts[1].Trim();

                    if (!int.TryParse(skillValueStr, out int skillValue))
                        continue; // Skip invalid numbers
                    if (!Enum.TryParse(skillName, ignoreCase: true, out C.Skill skillEnum))
                        continue; // Skip unknown skills

                    if (numericsSkillSet.TryGetValue(skillEnum, out NumericUpDown numeric))
                    {
                        // Ensure value doesn't exceed min/max values of numerics
                        skillValue = Math.Max((int)numeric.Minimum, Math.Min((int)numeric.Maximum, skillValue));
                        numeric.Value = skillValue;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error applying skillset:\n" + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        } [DllImport("kernel32.dll", CharSet = CharSet.Auto)]
          private static extern int GetPrivateProfileSection(string lpAppName, byte[] lpszReturnBuffer, int nSize, string lpFileName);

        /// <summary>
        /// Populates the Custom Skillset combo with a list of preset/user skillsets
        /// </summary>
        private void SetCustomSkillsetList()
        {
            try
            {
                if (!File.Exists(C.AppPathCustomSkillsets))
                {
                    // Create the .ini file with default skillsets
                    string defaultContent = @"# SLXEditor Custom Skillsets

[Classic 8 - 10 of each]
Climber=10
Floater=10
Timebomber=10
Blocker=10
Builder=10
Basher=10
Miner=10
Digger=10

[Classic 8 - 20 of each]
Climber=20
Floater=20
Timebomber=20
Blocker=20
Builder=20
Basher=20
Miner=20
Digger=20

[SLX Exclusive - 10 of each]
Ballooner=10
Grenader=10
Spearer=10
Freezer=10
Ladderer=10";

                    // Ensure the directory exists
                    string directory = Path.GetDirectoryName(C.AppPathCustomSkillsets);
                    if (!Directory.Exists(directory))
                    {
                        Directory.CreateDirectory(directory);
                    }

                    File.WriteAllText(C.AppPathCustomSkillsets, defaultContent, Encoding.Unicode);
                }

                // At this point the file exists, so enable and populate combo
                comboCustomSkillset.Enabled = true;

                comboCustomSkillset.Items.Clear();
                comboCustomSkillset.Items.Add("Select Custom Skillset");

                // Read all section names (skillset names)
                string[] skillsetNames = GetSkillsetNames(C.AppPathCustomSkillsets);

                foreach (string name in skillsetNames)
                {
                    comboCustomSkillset.Items.Add(name);
                }

                // Optionally select the first item
                if (comboCustomSkillset.Items.Count > 0)
                    comboCustomSkillset.SelectedIndex = 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Error checking or creating custom skillset file:\n" + ex.Message,
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );

                comboCustomSkillset.Enabled = false;
            }
        }

        /// <summary>
        /// Returns all skillset names (INI section headers) from the given SLXCustomSkillsets.ini file.
        /// </summary>
        private static string[] GetSkillsetNames(string filePath)
        {
            const int bufferSize = 4096; // doubled to be safe
            byte[] buffer = new byte[bufferSize];

            int length = GetPrivateProfileSectionNames(buffer, bufferSize, filePath);
            if (length == 0)
                return Array.Empty<string>();

            string allSections = Encoding.Unicode.GetString(buffer, 0, length * 2).Trim('\0');
            return allSections.Split(new char[] { '\0' }, StringSplitOptions.RemoveEmptyEntries);
        } [DllImport("kernel32.dll", CharSet = CharSet.Auto)]
          private static extern int GetPrivateProfileSectionNames(byte[] lpszReturnBuffer, int nSize, string lpFileName);

        /// <summary>
        /// Sets the music options according to available files in the music folder.
        /// </summary>
        private void SetMusicList()
        {
            List<string> musicNames = null;
            if (System.IO.Directory.Exists(C.AppPathMusic))
            {
                string root = Path.GetFullPath(C.AppPathMusic)
                                  .TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar)
                                  + Path.DirectorySeparatorChar;

                musicNames = Directory
                    .GetFiles(root, "*.*", SearchOption.AllDirectories)
                    .Where(f => Path.GetExtension(f).In(C.MusicExtensions))
                    .Select(f =>
                    {
                        string fullPath = Path.GetFullPath(f);
                        string relativePath = fullPath.Substring(root.Length);
                        string noExt = Path.ChangeExtension(relativePath, null);

                        return noExt.Replace('\\', '/');
                    })
                    .ToList();
            }
            else
            {
                musicNames = C.MusicNames;
            }

            comboMusic.Items.Clear();
            comboMusic.Items.Add("");
            musicNames.ForEach(music => comboMusic.Items.Add(music));
        }

        /// <summary>
        /// Sets the correct size and position of the expanded tabs
        /// </summary>
        private void UpdateExpandedTabs()
        {
            tabPiecesExp.Size = tabProperties.Size;
            tabPiecesExp.Left = tabProperties.Right;
            tabPiecesExp.Top = tabProperties.Top;

            tabSkillsExp.Size = tabProperties.Size;
            tabSkillsExp.Left = tabPiecesExp.Right;
            tabSkillsExp.Top = tabProperties.Top;

            tabExtrasExp.Size = tabProperties.Size;
            tabExtrasExp.Left = tabSkillsExp.Right;
            tabExtrasExp.Top = tabProperties.Top;
        }

        /// <summary>
        /// Removes focus from the current control and moves it to the default location txt_Focus.
        /// </summary>
        private void PullFocusFromTextInputs()
        {
            if (pieceBrowserWindow != null)
                pieceBrowserWindow.ActiveControl = txtFocusPieceBrowser;

            this.ActiveControl = txtFocus;
            UpdateIsSystemKeyPressed();
        }

        /// <summary>
        /// Sets the key pressed state according to the current state.
        /// </summary>
        private void UpdateIsSystemKeyPressed()
        {
            isCtrlPressed = ((ModifierKeys & Keys.Control) != 0);
            isShiftPressed = ((ModifierKeys & Keys.Shift) != 0);
            isAltPressed = ((ModifierKeys & Keys.Alt) != 0);
        }

        /// <summary>
        /// Takes the global level data input on the form and stores it in the current level.
        /// </summary>
        private void ReadLevelInfoFromForm(bool allowWriteBack)
        {
            CurLevel.Author = txtLevelAuthor.Text;
            CurLevel.Title = txtLevelTitle.Text;
            CurLevel.MusicFile = Path.ChangeExtension(comboMusic.Text, null);
            CurLevel.ThemeStyle = ValidateStyleName(comboTheme.Text);
            CurLevel.Width = decimal.ToInt32(numWidth.Value);
            CurLevel.Height = decimal.ToInt32(numHeight.Value);
            CurLevel.AutoStartPos = checkAutoStart.Checked;
            CurLevel.StartPosX = decimal.ToInt32(numStartX.Value);
            CurLevel.StartPosY = decimal.ToInt32(numStartY.Value);
            CurLevel.NumLems = decimal.ToInt32(numLemmings.Value);
            CurLevel.SaveReq = decimal.ToInt32(numRescue.Value);
            CurLevel.SpawnInterval = decimal.ToInt32(numSI.Value);
            CurLevel.ReleaseRate = decimal.ToInt32(numRR.Value);
            CurLevel.IsSpawnRateFix = checkLockRRSI.Checked;
            CurLevel.IsSuperlemming = checkSuperlemming.Checked;
            CurLevel.TimeLimit = decimal.ToInt32(numTimeMins.Value) * 60
                                    + decimal.ToInt32(numTimeSecs.Value);
            CurLevel.HasTimeLimit = checkTimeLimit.Checked;
            CurLevel.IsInvincibility = checkInvincibility.Checked;
            CurLevel.SteelType = radAlwaysSteel.Checked ? 1 : 0;

            string idText = txtLevelID.Text;
            if (idText.Length < 16)
                idText = idText.PadLeft(16);
            if (idText.Length > 16)
                idText = idText.Substring(16);

            if (ulong.TryParse(idText, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out ulong newID))
            {
                if (newID != 0)
                    CurLevel.LevelID = newID;
            }

            if (allowWriteBack && txtLevelID.Text != CurLevel.LevelID.ToString("X16"))
                txtLevelID.Text = CurLevel.LevelID.ToString("X16");

            foreach (C.Skill skill in numericsSkillSet.Keys)
            {
                CurLevel.SkillSet[skill] = decimal.ToInt32(numericsSkillSet[skill].Value);
            }
        }
        private string GetDefaultAuthorName()
        {
            string name = curSettings.DefaultAuthorName;

            if (string.IsNullOrWhiteSpace(name))
            {
                try
                {
                    var fileReader = new StreamReader(C.AppPathPlayerSettings);

                    string line;
                    while ((line = fileReader.ReadLine()) != null)
                    {
                        if (line.StartsWith("UserName=", StringComparison.OrdinalIgnoreCase))
                        {
                            name = line.Substring("UserName=".Length).Trim();
                            break;
                        }
                    }
                }
                catch
                {
                    name = string.Empty;
                }
            }

            return name;
        }

        private bool _IsWritingToForm;

        /// <summary>
        /// Takes the global level settings and displays them in the correct form fields.
        /// </summary>
        private void WriteLevelInfoToForm()
        {
            _IsWritingToForm = true;
            try
            {
                txtLevelAuthor.Text = CurLevel.Author;
                txtLevelTitle.Text = CurLevel.Title;

                if (!string.IsNullOrEmpty(CurLevel.MusicFile) && comboMusic.Items.Contains(CurLevel.MusicFile))
                    comboMusic.SelectedItem = CurLevel.MusicFile;
                else
                    comboMusic.SelectedIndex = 0;

                if ((CurLevel.ThemeStyle != null) && comboTheme.Items.Contains(CurLevel.ThemeStyle.NameInEditor))
                    comboTheme.SelectedItem = CurLevel.ThemeStyle.NameInEditor;
                else
                    comboTheme.SelectedIndex = 0;

                // Set size and start position, but without calling the Value_Changed methods,
                // because they automatically call validation of the start position resp. render the level again.
                numWidth.ValueChanged -= numSizeX_ValueChanged;
                numHeight.ValueChanged -= numSizeY_ValueChanged;
                numStartX.ValueChanged -= numStartX_ValueChanged;
                numStartY.ValueChanged -= numStartY_ValueChanged;

                numWidth.Value = CurLevel.Width;
                numHeight.Value = CurLevel.Height;
                numStartX.Maximum = CurLevel.Width - 1;
                numStartY.Maximum = CurLevel.Height - 1;
                numStartX.Value = CurLevel.StartPosX;
                numStartY.Value = CurLevel.StartPosY;
                checkAutoStart.Checked = CurLevel.AutoStartPos;

                numWidth.ValueChanged += numSizeX_ValueChanged;
                numHeight.ValueChanged += numSizeY_ValueChanged;
                numStartX.ValueChanged += numStartX_ValueChanged;
                numStartY.ValueChanged += numStartY_ValueChanged;

                // Add the rest of the values
                numLemmings.Value = CurLevel.NumLems;
                numRescue.Value = CurLevel.SaveReq;
                numSI.Value = CurLevel.SpawnInterval;
                numRR.Value = CurLevel.ReleaseRate;
                checkLockRRSI.Checked = CurLevel.IsSpawnRateFix;
                numTimeMins.Value = CurLevel.TimeLimit / 60;
                numTimeSecs.Value = CurLevel.TimeLimit % 60;
                checkTimeLimit.Checked = CurLevel.HasTimeLimit;
                checkSuperlemming.Checked = CurLevel.IsSuperlemming;
                checkInvincibility.Checked = CurLevel.IsInvincibility;
                radAlwaysSteel.Checked = CurLevel.SteelType >= 1;
                radOnlyWhenVisible.Checked = CurLevel.SteelType <= 0;

                txtLevelID.Text = CurLevel.LevelID.ToString("X16");

                foreach (C.Skill skill in numericsSkillSet.Keys)
                {
                    numericsSkillSet[skill].Value = CurLevel.SkillSet[skill];
                }

                lblLevelVersion.Text = "Version: " + CurLevel.LevelVersion.ToString("X16");

                RegenerateTalismanList();
            }
            finally
            {
                _IsWritingToForm = false;
            }
        }

        private void RegenerateTalismanList()
        {
            lbTalismans.Items.Clear();
            foreach (var talisman in CurLevel.Talismans)
            {
                lbTalismans.Items.Add(talisman);
            }

        }

        /// <summary>
        /// Creates a new instance of a Level and a new Renderer, then displays it on the form.
        /// </summary>
        public void CreateNewLevelAndRenderer()
        {
            if (AskUserWhetherSaveLevel())
                return;

            Style themeStyle = StyleList?.Find(sty => sty.NameInEditor == comboTheme.Text);
            CurLevel = new Level(themeStyle);
            CurLevel.Author = GetDefaultAuthorName();

            // Get new renderer with the standard display options
            if (curRenderer != null)
                curRenderer.Dispose();
            curRenderer = new Renderer(CurLevel, picLevel, curSettings);

            oldLevelList = new List<Level>();
            oldLevelList.Add(CurLevel.Clone());
            curOldLevelIndex = 0;
            lastSavedLevel = CurLevel.Clone();

            WriteLevelInfoToForm();
            UpdateBackgroundImage();
            UpdateFlagsForPieceActions();
            RepositionPicLevel();
            picLevel.Image = curRenderer.CreateLevelImage();

            UpdateSpecialLemmingCounter();

            if (curSettings.DefaultTemplate != string.Empty)
                LoadLevelFromDefaultTemplate();
        }

        private void LoadLevelFromDefaultTemplate()
        {
            string templatePath = Path.Combine(C.AppPathTemplates, curSettings.DefaultTemplate + ".template");
            try
            {
                LoadNewLevel(templatePath);
                CurLevel.FilePathToSave = null;
                LevelDirectory = C.AppPathLevels;
            }
            catch (Exception Ex)
            {
                Utility.LogException(Ex);
                MessageBox.Show("Error while loading the level." + C.NewLine + Ex.Message, "Level load error");
            }
        }

        /// <summary>
        /// Displays a file browser (if path not specified) and loads the selected level
        /// </summary>
        public void LoadNewLevel(string filename = null)
        {
            if (AskUserWhetherSaveLevel())
                return;

            Level level;

            if (filename == null)
                level = LevelFile.LoadLevel(StyleList, Backgrounds, LevelDirectory);
            else
                level = LevelFile.LoadLevelFromFile(filename, StyleList, Backgrounds);

            if (level == null)
                return;

            LevelDirectory = Path.GetDirectoryName(level.FilePathToSave);
            
            CurLevel = level;
            curRenderer.SetLevel(CurLevel);
            ValidateLevelPieces();
            UpdateBackgroundImage();

            oldLevelList = new List<Level>();
            oldLevelList.Add(CurLevel.Clone());
            curOldLevelIndex = 0;
            lastSavedLevel = level.Clone();

            WriteLevelInfoToForm();
            UpdateFlagsForPieceActions();
            RepositionPicLevel();
            picLevel.Image = curRenderer.CreateLevelImage();

            comboPieceStyle.Text = CurLevel.ThemeStyle?.NameInEditor;

            UpdateSpecialLemmingCounter();
        }

        /// <summary>
        /// Redraws the current level image - useful when changing settings
        /// </summary>
        public void RefreshLevel()
        {
            picLevel.Image = curRenderer.CreateLevelImage();
        }

        /// <summary>
        /// Returns the requested style or the first available valid style
        /// </summary>
        private string ValidateStyleList(Style style)
        {
            // Fallback to first valid style if requested style is null or invalid
            if (style == null)
            {
                MessageBox.Show("Style cannot be null. Using the first available style.", "Warning",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);

                return StyleList[0].NameInEditor;
            }

            // Return requested style if it exists
            if (StyleList.Any(sty => sty.NameInEditor == style.NameInEditor))
                return style.NameInEditor;

            // Fallback to first valid style if requested style is missing
            MessageBox.Show(
                $"Style '{style.NameInEditor}' could not be found.\n" +
                $"Using '{StyleList[0].NameInEditor}' instead.",
                "Style Not Found",
                MessageBoxButtons.OK, MessageBoxIcon.Warning);

            return StyleList[0].NameInEditor;
        }

        /// <summary>
        /// Reloads all styles to keep pieces up-to-date without closing & reopening the Editor
        /// </summary>
        public void RefreshStyles(bool refreshedFromStyleManager = false)
        {
            if (CurLevel == null || pieceCurStyle == null)
                return;

            if (!Directory.Exists(C.AppPathStyles) || Directory.GetDirectories(C.AppPathStyles).Length == 0)
            {
                MessageBox.Show("Cannot refresh. The 'styles' folder appears to be empty or missing.");
                return;
            }

            Style themeStyle = CurLevel.ThemeStyle;
            Style pieceStyle = pieceCurStyle;

            StyleList.Clear();
            comboTheme.Items.Clear();
            comboPieceStyle.Items.Clear();

            ImageLibrary.Clear();
            LoadStylesFromFile.AddInitialImagesToLibrary();
            LoadStylesFromFile.AddRulersToLibrary();

            CreateStyleList();

            if (StyleList.Count > 0)
            {
                this.comboTheme.Items.AddRange(StyleList.Where(sty => File.Exists(C.AppPathThemeInfo(sty.NameInDirectory))).Select(sty => sty.NameInEditor).ToArray());
                this.comboTheme.Text = ValidateStyleList(themeStyle);

                this.comboPieceStyle.Items.AddRange(StyleList.ConvertAll(sty => sty.NameInEditor).ToArray());
                this.comboPieceStyle.Text = ValidateStyleList(pieceStyle);

                if (refreshedFromStyleManager)
                {
                    if ((CurLevel.ThemeStyle != null) && comboTheme.Items.Contains(CurLevel.ThemeStyle.NameInEditor))
                        comboTheme.SelectedItem = CurLevel.ThemeStyle.NameInEditor;
                    else
                        comboTheme.SelectedIndex = 0;

                    comboPieceStyle.SelectedIndex = 0;
                }
            }
            else
            {
                MessageBox.Show("The style list could not be built. The Editor will now close.");
                throw new ApplicationException("Fatal error loading styles");
            }

            // Reset style pieces and status bar
            ValidateStylePieces();
            UpdateStatusBar();

            // Reset level
            curRenderer.SetLevel(CurLevel);
            RefreshLevel();
            UpdateFlagsForPieceActions();
            UpdatePieceMetaData();
        }

        private void UpdateMissingPiecesMenuItems()
        {
            bool hasMissingPieces = missingPieces.Count > 0;
            showMissingPiecesToolStripMenuItem.Visible = hasMissingPieces;
            sepMissingPieces.Visible = hasMissingPieces;
        }

        /// <summary>
        /// Initialise lists, menu items and status bar before checking for missing pieces
        /// </summary>
        private void PrepareForPieceValidation()
        {
            missingPieces.Clear();
            UpdateMissingPiecesMenuItems();
            UpdateStatusBar();
        }

        /// <summary>
        /// Validates style pieces after a refresh
        /// </summary>
        private void ValidateStylePieces()
        {
            if (CurLevel == null)
                return;

            PrepareForPieceValidation();
            UpdateMissingPiecesList();
            UpdateMissingPiecesMenuItems();
            UpdateStatusBar();
        }

        // Store the names of the missing pieces for the current level
        public HashSet<string> missingPieces = new HashSet<string>();

        /// <summary>
        /// Checks for & removes all pieces for which no image in the corresponding style exists.
        /// </summary>
        private void ValidateLevelPieces()
        {
            if (CurLevel == null)
                return;

            PrepareForPieceValidation();
            UpdateMissingPiecesList();
            UpdateMissingPiecesMenuItems();
            UpdateStatusBar();

            // If cleansing, search for more piece information and populate lists
            if (!cleansingLevels)
                return;

            if (missingPieces.Count > 0)
                levelsWithMissingPieces.Add(CurLevel.FilePathToSave);

            int deprecatedPieces =
                CurLevel.TerrainList.Count(ter => ImageLibrary.GetDeprecated(ter.Key)) +
                CurLevel.GadgetList.Count(obj => ImageLibrary.GetDeprecated(obj.Key));

            if (deprecatedPieces > 0)
                levelsWithDeprecatedPieces.Add(CurLevel.FilePathToSave);

            if (!CurLevel.GadgetList.Exists(obj => obj.ObjType.In(C.OBJ.HATCH, C.OBJ.LEMMING)))
                levelsWithNoLemmings.Add(CurLevel.FilePathToSave);

            if (!CurLevel.GadgetList.Exists(obj => obj.ObjType.In(C.OBJ.EXIT, C.OBJ.EXIT_LOCKED)))
                levelsWithNoExits.Add(CurLevel.FilePathToSave);
        }

        /// <summary>
        ///  Handles status bar visibility and display text
        /// </summary>
        public void UpdateStatusBar()
        {
            if (missingPieces.Count > 0)
            {
                statusBarLabel1.Text = "This level contains missing pieces (click to show).";
                statusBarLabel2.Text = "Click the lemming button for more options";
                statusBar.Visible = true;
                statusBar.Enabled = true;
            }
            else
            {
                statusBar.Visible = false;
                statusBar.Enabled = false;
            }
        }

        /// <summary>
        /// Searches for missing pieces in the current level, and populates the list if found
        /// </summary>
        private void UpdateMissingPiecesList()
        {
            CurLevel.TerrainList.FindAll(ter => !ter.ExistsImage())
                  .ForEach(ter => missingPieces.Add($@"{ter.Style}\terrain\{ter.Name}"));
            CurLevel.GadgetList.FindAll(gad => !gad.ExistsImage())
                             .ForEach(gad => missingPieces.Add($@"{gad.Style}\objects\{gad.Name}"));
        }

        /// <summary>
        /// Removes missing pieces from the level
        /// </summary>
        private void DeleteMissingPieces()
        {
            if (missingPieces.Count > 0)
            {
                var result = MessageBox.Show(
                    "Are you sure you want to delete all missing pieces from the level?",
                    "Confirm Deletion",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Warning);

                if (result != DialogResult.Yes)
                    return;
            }

            CurLevel.TerrainList.RemoveAll(ter => !ter.ExistsImage());
            CurLevel.GadgetList.RemoveAll(gad => !gad.ExistsImage());
            missingPieces.Clear();
            UpdateStatusBar();
        }

        /// <summary>
        /// Opens the Export As INI dialog
        /// </summary>
        private void OpenExportAsINI()
        {
            using (var iniExporterForm = new FormINIExporter(CurLevel, this))
            {
                iniExporterForm.ShowDialog(this);
            }
        }

        /// <summary>
        /// Opens the Export As INI dialog
        /// </summary>
        public void OpenBatchExporter()
        {
            using (var batchExporterForm = new FormBatchExporter(this))
            {
                batchExporterForm.ShowDialog(this);
            }
        }

        /// <summary>
        /// Opens the Style Manager dialog
        /// </summary>
        private void OpenStyleManager()
        {
            using (var styleManagerForm = new FormStyleManager(this, curSettings))
            {
                styleManagerForm.ShowDialog(this);
            }
        }

        /// <summary>
        /// Chooses a random style for the piece browser
        /// </summary>
        private static readonly Random _rng = new Random();
        private void RandomizePieceStyle()
        {
            if (comboPieceStyle.Items.Count == 0)
                return;

            var randomizedNames = StyleList.Where(s => s.Randomize).Select(s => s.NameInEditor).ToList();

            string current = comboPieceStyle.SelectedItem as string;

            if (randomizedNames.Count == 0)
            {
                int index;
                do
                {
                    index = _rng.Next(comboPieceStyle.Items.Count);
                }
                while (comboPieceStyle.Items[index].Equals(current) &&
                       comboPieceStyle.Items.Count > 1);

                comboPieceStyle.SelectedIndex = index;
                return;
            }

            string chosen;
            do
            {
                chosen = randomizedNames[_rng.Next(randomizedNames.Count)];
            }
            while (chosen == current && randomizedNames.Count > 1);

            comboPieceStyle.SelectedItem = chosen;
        }

        /// <summary>
        /// Performs a search for pieces throughout the full styles folder
        /// </summary>
        private void OpenPieceSearch()
        {
            string rootPath = Application.StartupPath;
            Style curStyle = pieceCurStyle;

            if (curStyle is null)
            {
                MessageBox.Show("Current style is not defined.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            FormPieceSearch searchForm = new FormPieceSearch(rootPath, curStyle);

            searchForm.StyleSelected += (newStylePath) =>
            {
                // Find the style based on its directory (NameInDirectory)
                Style style = StyleList?.Find(sty => sty.NameInDirectory == newStylePath);

                if (style != null)
                {   // Set style based on its user-friendly name
                    comboPieceStyle.Text = style.NameInEditor;
                }
                else
                {
                    MessageBox.Show("The selected style could not be found.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }

                // Pass the current selected style back to the search form
                searchForm.curStyle = style;
            };

            searchForm.PieceSelected += (newPiece) =>
            {
                try
                {
                    AddNewPieceToLevel(newPiece, curRenderer.GetCenterPoint());
                    MaybeOpenPiecesTab();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error adding piece:\nPiece Key: {newPiece}\nException: {ex.Message}",
                                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            };

            searchForm.ShowDialog();
        }

        private void ShowMissingPiecesDialog()
        {
            if (missingPieces.Count > 0)
            {
                MessageBox.Show("Missing Style Pieces:" + Environment.NewLine + Environment.NewLine +
                                 string.Join(Environment.NewLine, missingPieces), "Missing Pieces",
                                 MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            else
            {
                MessageBox.Show("No missing pieces found.", "Missing Pieces", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
        private void AddRuler(string pieceKey)
        {
            Point pos = curRenderer.GetCenterPoint();
            AddNewPieceToLevel(pieceKey, pos);
            MaybeOpenPiecesTab();
        }

        private void OpenLevelArrangerWindow()
        {
            // Check if the Level Arranger window is already open
            if (levelArrangerWindow != null && !levelArrangerWindow.IsDisposed)
            {
                levelArrangerWindow.BringToFront();
                return;
            }

            // Create the pop-out window and pass pic_Level to it
            levelArrangerWindow = new FormLevelArranger(picLevel, this, curRenderer, curSettings);

            // Don't reposition pic_Level when zooming from within the Arrange Window
            repositionAfterZooming = false;

            // Subscribe to the PicLevelReturned event to handle re-parenting
            levelArrangerWindow.PicLevelReturned += () =>
            {
                this.Invoke(new Action(() =>
                {
                    repositionAfterZooming = true;
                    
                    // Re-parent pic_Level back to the main form
                    picLevel.Dock = DockStyle.None;
                    this.Controls.Add(picLevel);

                    // Reset the position of pic_Level
                    RepositionPicLevel();
                    picLevel.Image = curRenderer.CreateLevelImage();

                    picLevel.Show();
                    picLevel.Focus();
                }));
            };

            // Ensure the reference is cleared when the window is closed
            levelArrangerWindow.FormClosing += (s, e) => levelArrangerWindow = null;

            // Show the pop-out window
            levelArrangerWindow.Show();
        }


        private void OpenPieceBrowserWindow()
        {
            // Check if the Piece Browser window is already open
            if (pieceBrowserWindow != null && !pieceBrowserWindow.IsDisposed)
            {
                pieceBrowserWindow.BringToFront();
                return;
            }

            // Create the pop-out window and pass panelPieceBrowser to it
            pieceBrowserWindow = new FormPieceBrowser(panelPieceBrowser, this, curSettings);

            // Subscribe to the PieceBrowserReturned event to handle re-parenting
            pieceBrowserWindow.PieceBrowserReturned += () =>
            {
                this.Invoke(new Action(() =>
                {
                    // Re-parent panelPieceBrowser back to the main form
                    this.Controls.Add(panelPieceBrowser);

                    // Reset the position of panelPieceBrowser
                    RepositionPieceBrowser(false, this.Width);
                    RepositionPicPieces(false, this.Width);

                    panelPieceBrowser.Show();
                    panelPieceBrowser.Focus();
                }));
            };

            // Ensure the reference is cleared when the window is closed
            pieceBrowserWindow.FormClosing += (s, e) => pieceBrowserWindow = null;

            // Show the pop-out window
            pieceBrowserWindow.Show();
            txtFocusPieceBrowser.Focus();
        }


        private void ToggleExpandedTabs()
        {
            if (!allTabsExpanded)
                ExpandAllTabs();
            else
                CollapseAllTabs();

            // Update settings
            curSettings.AllTabsExpanded = allTabsExpanded;
            curSettings.WriteSettingsToFile();
        }

        private void ExpandAllTabs()
        {
            tabProperties.TabPages.Remove(tabPieces);
            tabPiecesExp.TabPages.Add(tabPieces);
            tabPiecesExp.Enabled = true;
            tabPiecesExp.Visible = true;

            tabProperties.TabPages.Remove(tabSkills);
            tabSkillsExp.TabPages.Add(tabSkills);
            tabSkillsExp.Enabled = true;
            tabSkillsExp.Visible = true;

            tabProperties.TabPages.Remove(tabExtras);
            tabExtrasExp.TabPages.Add(tabExtras);
            tabExtrasExp.Enabled = true;
            tabExtrasExp.Visible = true;

            expandAllTabsToolStripMenuItem.Text = "Collapse All Tabs";
            allTabsExpanded = true;
        }

        private void CollapseAllTabs()
        {
            tabPiecesExp.TabPages.Remove(tabPieces);
            tabProperties.TabPages.Add(tabPieces);
            tabPiecesExp.Enabled = false;
            tabPiecesExp.Visible = false;

            tabSkillsExp.TabPages.Remove(tabSkills);
            tabProperties.TabPages.Add(tabSkills);
            tabSkillsExp.Enabled = false;
            tabSkillsExp.Visible = false;

            tabExtrasExp.TabPages.Remove(tabExtras);
            tabProperties.TabPages.Add(tabExtras);
            tabExtrasExp.Enabled = false;
            tabExtrasExp.Visible = false;

            expandAllTabsToolStripMenuItem.Text = "Expand All Tabs";
            allTabsExpanded = false;
        }

        public void HandleCropLevel()
        {
            if (curRenderer.CropTool.Active)
                curRenderer.CropTool.Stop();
            else
                curRenderer.CropTool.Start();

            UpdateCropButtons();
            picLevel.SetImage(curRenderer.GetScreenImage());
            PullFocusFromTextInputs();
        }

        private void ApplyLevelCrop()
        {
            Rectangle cropRect = curRenderer.CropTool.LevelCropRect;
            int startX = (int)numStartX.Value;
            int startY = (int)numStartY.Value;

            SelectAllPieces();
            CurLevel.MovePieces(C.DIR.N, cropRect.Y, 1);
            CurLevel.MovePieces(C.DIR.W, cropRect.X, 1);
            CurLevel.UnselectAll();

            numWidth.Value = cropRect.Width;
            numHeight.Value = cropRect.Height;

            int newStartX = startX - cropRect.X;
            int newStartY = startY - cropRect.Y;

            newStartX = Math.Max(0, Math.Min(newStartX, cropRect.Width - 1));
            newStartY = Math.Max(0, Math.Min(newStartY, cropRect.Height - 1));

            numStartX.Value = newStartX;
            numStartY.Value = newStartY;

            CommitLevelChanges();
            HandleCropLevel();
        }

        /// <summary>
        /// Checks for presence of Neo/SuperLemmix.exe in Editor's base folder and set isNeoLemmixOnly
        /// </summary>
        public void DetectLemmixVersions()
        {
            string baseFolder = AppDomain.CurrentDomain.BaseDirectory;

            bool isNeoLemmixDetected = File.Exists(C.AppPathNeoLemmix) || File.Exists(C.AppPathNeoLemmixCE);
            bool isSuperLemmixDetected = File.Exists(C.AppPathSuperLemmix);

            var curMode = curSettings.CurrentEditorMode;

            isNeoLemmixOnly =
               ((curMode == Settings.EditorMode.Auto && isNeoLemmixDetected && !isSuperLemmixDetected)
              || curMode == Settings.EditorMode.NeoLemmix)
              && curMode != Settings.EditorMode.SuperLemmix;
        }

        /// <summary>
        /// If the level changed, displays a message box and asks whether to save the current level.  
        /// </summary>
        private bool AskUserWhetherSaveLevel()
        {
            if (lastSavedLevel == null || CurLevel.Equals(lastSavedLevel))
                return false;
            if (CurLevel.TerrainList.Count == 0 && CurLevel.GadgetList.Count == 0)
                return false;

            DialogResult dialogResult = MessageBox.Show("Do you want to save this level?", "Save level?", MessageBoxButtons.YesNoCancel);
            switch (dialogResult)
            {
                case DialogResult.Yes:
                    SaveLevel();
                    if (!LevelValidator.validationPassed)
                        return true;
                    break;
                case DialogResult.Cancel:
                    return true;
            }

            return false;
        }

        /// <summary>
        /// Displays a file browser and saves the current level in chosen location. 
        /// </summary>
        private void SaveLevelAsNewFile(bool isPlaytest = false)
        {
            // get most up-to-date global info
            ReadLevelInfoFromForm(true);

            LevelFile.SaveLevel(CurLevel, LevelDirectory, CanSaveToEitherFormat(CurLevel));
            SaveChangesToOldLevelList();
            LevelDirectory  = Path.GetDirectoryName(CurLevel.FilePathToSave);
            if (!isPlaytest)
                lastSavedLevel = CurLevel.Clone();

            WriteLevelInfoToForm();
        }

        /// <summary>
        /// Saves the current level in the current location. If no location is known, the file browser is opened.
        /// </summary>
        public void SaveLevel(bool isPlaytest = false, bool validate = true)
        {
            if (validate && curSettings.ValidateWhenSaving)
            {
                ValidateLevel(true, cleansingLevels);

                if (!LevelValidator.validationPassed)
                    return;
            }

            if (CurLevel.FilePathToSave == null)
            {
                SaveLevelAsNewFile();
            }
            else
            {
                // Get most up-to-date global info
                ReadLevelInfoFromForm(true);

                LevelFile.SaveLevelToFile(CurLevel.FilePathToSave, CurLevel);
                SaveChangesToOldLevelList();
                if (!isPlaytest)
                    lastSavedLevel = CurLevel.Clone();

                WriteLevelInfoToForm();
            }
        }

        private async void ShowCleanseLevelsDialog()
        {
            using (var folderBrowserDialog = new FolderBrowserDialog())
            {
                folderBrowserDialog.Description = "Choose a folder of levels to cleanse";
                folderBrowserDialog.SelectedPath = C.AppPathLevels;

                DialogResult result = folderBrowserDialog.ShowDialog();

                if (result == DialogResult.OK && !string.IsNullOrWhiteSpace(folderBrowserDialog.SelectedPath))
                {
                    await CleanseLevels(folderBrowserDialog.SelectedPath);
                }
            }
        }

        // Store filenames of levels with missing pieces
        List<string> levelsWithMissingPieces = new List<string>();
        List<string> levelsWithDeprecatedPieces = new List<string>();
        List<string> levelsWithNoLemmings = new List<string>();
        List<string> levelsWithNoExits = new List<string>();
        private bool cleansingLevels;

        public string LevelFileExtension()
        {
            return isNeoLemmixOnly ? ".nxlv" : ".sxlv";
        }

        public bool CanSaveToEitherFormat(Level level)
        {
            if (level.IsSuperlemming)
                return false;

            if (level.SteelType >= 1)
                return false;

            foreach (C.Skill skill in C.SuperLemmixSkills)
                if (LevelFile.IsSkillRequired(level, skill))
                    return false;

            if (level.GadgetList.Exists(obj => obj.ObjType.In(C.SuperLemmixObjects)))
                return false;

            var rivalObj = level.GadgetList
                .FirstOrDefault(obj => obj.ObjType.In(C.OBJ.LEMMING, C.OBJ.HATCH, C.OBJ.EXIT) && obj.IsRival);
            if (rivalObj != null)
                return false;

            return true;
        }

        /// <summary>
        /// Opens and saves all level files in a directory in order to ensure compatibility and update the file
        /// </summary>
        public async Task CleanseLevels(String targetFolder)
        {
            if (string.IsNullOrEmpty(targetFolder))
            {
                MessageBox.Show("Please select a target folder.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            cleansingLevels = true;

            // Disable group/eraser highlighting
            BmpModify.HighlightErasers = false;
            BmpModify.HighlightGroups = false;

            // Initialise list
            levelsWithMissingPieces.Clear();
            levelsWithDeprecatedPieces.Clear();
            levelsWithNoLemmings.Clear();
            levelsWithNoExits.Clear();

            // Get all .sxlv and .nxlv files in the target folder and its subdirectories
            string[] filesSXLV = Directory.GetFiles(targetFolder, "*.sxlv", SearchOption.AllDirectories);
            string[] filesNXLV = Directory.GetFiles(targetFolder, "*.nxlv", SearchOption.AllDirectories);

            // Combine both arrays
            string[] files = filesSXLV.Concat(filesNXLV).ToArray();

            // Ask the user to choose an output extension
            string chosenExt = null;
            bool applyFormatToLevelsNXMI = false;
            using (var dlg = new FormLevelFormat(targetFolder))
            {
                if (dlg.ShowDialog() == DialogResult.OK)
                {
                    chosenExt = dlg.SelectedExtension;
                    applyFormatToLevelsNXMI = dlg.ApplyFormatToLevelsNXMI;
                }
                else
                    return; // User cancelled
            }

            using (FormProgress progressForm = new FormProgress())
            {
                progressForm.ProgressBar.Maximum = files.Length;
                progressForm.Show(this);

                for (int index = 0; index < files.Length; index++)
                {
                    string file = files[index];
                    string ext = chosenExt;

                    // Load and save the file with the chosen extension
                    LoadNewLevel(file);
                    if (!CanSaveToEitherFormat(CurLevel)) ext = ".sxlv"; // Override if the level contains SuperLemmix-specific features
                    if (ext != "")
                    {
                        CurLevel.FilePathToSave = Path.Combine(
                            Path.GetDirectoryName(file),
                            Path.GetFileNameWithoutExtension(file) + ext
                        );
                    }
                    SaveLevel(false);

                    if (applyFormatToLevelsNXMI && (ext != ""))
                        ApplyFormatToLevelsNXMI(file, targetFolder, ext);

                    // Update the progress bar
                    int progressPercentage = (index + 1) * 100 / files.Length;
                    progressForm.UpdateProgress(
                        progressPercentage,
                        $"Processing file {index + 1} of {files.Length}: {Path.GetFileName(file)}"
                    );

                    // Reset Editor before moving on to the next level
                    CreateNewLevelAndRenderer();

                    // Allow UI to update
                    await Task.Delay(10);
                }

                progressForm.Close();

                // Re-initialize the Editor
                CreateNewLevelAndRenderer();
                statusBar.Visible = false;

                // Display completion message
                string cleanseMsg = "All levels cleansed successfully.";

                if (levelsWithMissingPieces.Count > 0)
                {
                    cleanseMsg += "\n\nLevels with missing pieces:\n\n";
                    cleanseMsg += string.Join("\n", levelsWithMissingPieces.Select(Path.GetFileName));
                }
                if (levelsWithDeprecatedPieces.Count > 0)
                {
                    cleanseMsg += "\n\nLevels with deprecated pieces:\n\n";
                    cleanseMsg += string.Join("\n", levelsWithDeprecatedPieces.Select(Path.GetFileName));
                }
                if (levelsWithNoLemmings.Count > 0)
                {
                    cleanseMsg += "\n\nLevels with no lemmings:\n\n";
                    cleanseMsg += string.Join("\n", levelsWithNoLemmings.Select(Path.GetFileName));
                }
                if (levelsWithNoExits.Count > 0)
                {
                    cleanseMsg += "\n\nLevels with no exits:\n\n";
                    cleanseMsg += string.Join("\n", levelsWithNoExits.Select(Path.GetFileName));
                }
                string reportPath = Path.Combine(targetFolder, "SLXEditorCleanseReport.txt");

                // Save report and show message
                string fullReport =
                    "Cleanse Report for:\n" + targetFolder + "\n\n" + 
                    cleanseMsg;

                File.WriteAllText(reportPath, fullReport);

                MessageBox.Show(
                    $"{cleanseMsg}\n\nReport saved to:\n{reportPath}",
                    "Cleanse Levels Complete",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);

                cleansingLevels = false;
            }
        }
        private void ApplyFormatToLevelsNXMI(string originalFilePath, string rootFolder, string newExt)
        {
            string oldFileName = Path.GetFileName(originalFilePath);
            string newFileName = Path.GetFileName(
                Path.ChangeExtension(originalFilePath, newExt)
            );

            // Nothing to do if already correct
            if (oldFileName.Equals(newFileName, StringComparison.OrdinalIgnoreCase))
                return;

            foreach (string nxmiPath in Directory.GetFiles(rootFolder, "levels.nxmi", SearchOption.AllDirectories))
            {
                var lines = File.ReadAllLines(nxmiPath);
                bool modified = false;

                for (int i = 0; i < lines.Length; i++)
                {
                    string line = lines[i];

                    if (!line.StartsWith("LEVEL ", StringComparison.OrdinalIgnoreCase))
                        continue;

                    int fileStart = line.LastIndexOf(oldFileName, StringComparison.OrdinalIgnoreCase);
                    if (fileStart < 0)
                        continue;

                    lines[i] = line.Substring(0, fileStart) + newFileName;
                    modified = true;
                    break;
                }

                if (modified)
                    File.WriteAllLines(nxmiPath, lines);
            }
        }

        /// <summary>
        /// Saves the level as TempTestLevel and loads this level in the Neo/SuperLemmix player.
        /// </summary>
        private void PlaytestLevel()
        {
            ReadLevelInfoFromForm(true);
            SaveChangesToOldLevelList();

            // Save the level as TempTestLevel with correct file extension
            string origFilePath = CurLevel.FilePathToSave;
            CurLevel.FilePathToSave = C.AppPathTempLevel + LevelFileExtension();
            SaveLevel(true);
            CurLevel.FilePathToSave = origFilePath;

            // Exit if validation failed
            if (!LevelValidator.validationPassed)
                return;

            string enginePath;
            string engineName;

            if (isNeoLemmixOnly)
            { 
                if (File.Exists(C.AppPathNeoLemmixCE))
                {
                    enginePath = C.AppPathNeoLemmixCE;
                    engineName = "NeoLemmixCE.exe";
                }
                else
                {
                    enginePath = C.AppPathNeoLemmix;
                    engineName = "NeoLemmix.exe";
                }
            }
            else
            {
                enginePath = C.AppPathSuperLemmix;
                engineName = "SuperLemmix.exe";
            }

            if (!System.IO.File.Exists(enginePath))
            {
                MessageBox.Show($"Error: Player {engineName} not found in editor directory.", "File not found");
            }
            else
            {
                try
                {
                    // Start the SuperLemmix player.
                    var playerStartInfo = new System.Diagnostics.ProcessStartInfo();
                    playerStartInfo.FileName = enginePath;
                    playerStartInfo.Arguments = "test " + "\"" + C.AppPathTempLevel + LevelFileExtension() + "\"";

                    System.Diagnostics.Process.Start(playerStartInfo);
                }
                catch (Exception Ex)
                {
                    Utility.LogException(Ex);
                    MessageBox.Show($"Error: Starting {engineName} failed or was aborted.", "Application start failed");
                }
            }
        }

        /// <summary>
        /// Creates a new LevelValidator, runs the validation and displays the result in a new form.
        /// </summary>
        private void ValidateLevel(bool openedViaSave, bool cleansingLevels)
        {
            ReadLevelInfoFromForm(true);
            var validator = new LevelValidator(CurLevel);
            validator.Validate(false, openedViaSave, cleansingLevels);
            picLevel.SetImage(curRenderer.CreateLevelImage());
        }


        /// <summary>
        /// Returns a style with the requested name, or null if none such is found. 
        /// </summary>
        private Style ValidateStyleName(string styleName)
        {
            return StyleList?.Find(sty => sty.NameInEditor == styleName);
        }


        /// <summary>
        /// Switches between displaying objects and terrain for newly added pieces.
        /// </summary>
        private void CyclePieceBrowserDisplay(C.SelectPieceType newKind)
        {
            if (newKind != pieceDoDisplayKind)
            {
                pieceDoDisplayKind = newKind;

                btnTerrain.Font = new Font(btnTerrain.Font, FontStyle.Regular);
                btnSteel.Font = new Font(btnSteel.Font, FontStyle.Regular);
                btnObjects.Font = new Font(btnObjects.Font, FontStyle.Regular);
                btnBackgrounds.Font = new Font(btnBackgrounds.Font, FontStyle.Regular);
                btnRulers.Font = new Font(btnRulers.Font, FontStyle.Regular);

                switch (newKind)
                {
                    case C.SelectPieceType.Terrain:
                        btnTerrain.Font = new Font(btnTerrain.Font, FontStyle.Bold);
                        break;
                    case C.SelectPieceType.Steel:
                        btnSteel.Font = new Font(btnSteel.Font, FontStyle.Bold);
                        break;
                    case C.SelectPieceType.Objects:
                        btnObjects.Font = new Font(btnObjects.Font, FontStyle.Bold);
                        break;
                    case C.SelectPieceType.Backgrounds:
                        btnBackgrounds.Font = new Font(btnBackgrounds.Font, FontStyle.Bold);
                        break;
                    case C.SelectPieceType.Rulers:
                        btnRulers.Font = new Font(btnRulers.Font, FontStyle.Bold);
                        break;
                }

                pieceStartIndex = 0;
                LoadPiecesIntoPictureBox();
            }
        }

        /// <summary>
        /// Moves the screen start position to the given level coordinates.
        /// </summary>
        private void MoveScreenStartPosition(Point newCenter)
        {
            // Ensure that the new center position is within the correct bounds.
            int newCenterX = newCenter.X.Restrict(0, (int)numStartX.Maximum);
            int newCenterY = newCenter.Y.Restrict(0, (int)numStartY.Maximum);

            // Remove these events to combine layers only once.
            numStartX.ValueChanged -= numStartX_ValueChanged;
            numStartY.ValueChanged -= numStartY_ValueChanged;

            numStartX.Value = newCenterX;
            numStartY.Value = newCenterY;
            CurLevel.StartPosX = newCenterX;
            CurLevel.StartPosY = newCenterY;

            numStartX.ValueChanged += numStartX_ValueChanged;
            numStartY.ValueChanged += numStartY_ValueChanged;

            // Save the changes and combine the layers now.
            picLevel.Image = curRenderer.CombineLayers();
            SaveChangesToOldLevelList();
        }

        /// <summary>
        /// Moves the current screen start position by 8 pixels in the given direction.
        /// </summary>
        private void MoveScreenStartPosition(C.DIR direction)
        {
            Point newCenter;
            switch (direction)
            {
                case C.DIR.N:
                    newCenter = new Point(CurLevel.StartPosX, CurLevel.StartPosY - 8);
                    break;
                case C.DIR.S:
                    newCenter = new Point(CurLevel.StartPosX, CurLevel.StartPosY + 8);
                    break;
                case C.DIR.E:
                    newCenter = new Point(CurLevel.StartPosX + 8, CurLevel.StartPosY);
                    break;
                case C.DIR.W:
                    newCenter = new Point(CurLevel.StartPosX - 8, CurLevel.StartPosY);
                    break;
                default:
                    newCenter = CurLevel.StartPos;
                    break;
            }

            MoveScreenStartPosition(newCenter);
        }


        /// <summary>
        /// Displays new pieces on the piece selection bar.
        /// </summary>
        public void MoveTerrPieceSelection(int movement)
        {       
            List<string> pieceNameList;

            switch (pieceDoDisplayKind)
            {
                case C.SelectPieceType.Terrain:
                    pieceNameList = pieceCurStyle?.TerrainKeys;
                    break;
                case C.SelectPieceType.Steel:
                    pieceNameList = pieceCurStyle?.SteelKeys;
                    break;
                case C.SelectPieceType.Objects:
                    pieceNameList = pieceCurStyle?.ObjectKeys;
                    break;
                case C.SelectPieceType.Backgrounds:
                    pieceNameList = pieceCurStyle?.BackgroundKeys;
                    break;
                case C.SelectPieceType.Rulers:
                    pieceNameList = new List<string>(ImageLibrary.RulerKeys);
                    break;
                default:
                    throw new ArgumentException();
            }

            if (pieceNameList == null || pieceNameList.Count == 0)
                return;

            if (curSettings.InfiniteScrolling)
                ScrollPiecesInfinitely(pieceNameList, movement);
            else
                ScrollPieces(pieceNameList, movement);

            LoadPiecesIntoPictureBox();
        }

        /// <summary>
        /// Previous infinite wrap-scrolling for piece browser, could bring this back optionally
        /// </summary>
        private void ScrollPiecesInfinitely(List<string> pieceNameList, int movement)
        {
            // Pass to correct piece index
            pieceStartIndex = (pieceStartIndex + movement) % pieceNameList.Count;
            // ensure that PieceStartIndex is positive
            pieceStartIndex = (pieceStartIndex + pieceNameList.Count) % pieceNameList.Count;
        }

        /// <summary>
        /// Scroll the piece browser left and right, stopping at the first and last pieces in each list
        /// </summary>
        private void ScrollPieces(List<string> pieceNameList, int movement)
        {
            if (pieceNameList == null || pieceNameList.Count == 0)
                return;

            if (pieceNameList.Count <= picPieceList.Count)
            {
                pieceStartIndex = 0; // No scrolling needed
                LoadPiecesIntoPictureBox();
                return;
            }

            int newIndex = pieceStartIndex + movement;

            if (newIndex < 0)
                newIndex = 0; // Stop scrolling left

            int maxIndex = pieceNameList.Count - picPieceList.Count;
            if (newIndex > maxIndex)
                newIndex = maxIndex; // Stop scrolling right

            if (newIndex != pieceStartIndex)
            {
                pieceStartIndex = newIndex;
                LoadPiecesIntoPictureBox();
            }
        }

        /// <summary>
        /// Changes the style for newly added pieces and displays the new pieces.
        /// </summary>
        private void ChangeNewPieceStyleSelection(int movement)
        {
            if (StyleList == null || StyleList.Count == 0)
                return;

            int newStyleIndex;

            int CurStyleIndex = StyleList.FindIndex(sty => sty.Equals(pieceCurStyle));
            if (CurStyleIndex < 0)
            {
                newStyleIndex = ((movement % StyleList.Count) + StyleList.Count) % StyleList.Count;
            }
            else
            {
                newStyleIndex = Math.Min(Math.Max(CurStyleIndex + movement, 0), StyleList.Count - 1);
            }

            pieceCurStyle = StyleList[newStyleIndex];
            pieceStartIndex = 0;
            LoadPiecesIntoPictureBox();

            this.comboPieceStyle.SelectedIndex = newStyleIndex;
        }

        private void CyclePieceBrowser()
        {
            C.SelectPieceType newKind;
            switch (pieceDoDisplayKind)
            {
                case C.SelectPieceType.Terrain:
                    newKind = C.SelectPieceType.Steel;
                    break;
                case C.SelectPieceType.Steel:
                    newKind = C.SelectPieceType.Objects;
                    break;
                case C.SelectPieceType.Objects:
                    newKind = C.SelectPieceType.Rulers;
                    break;
                case C.SelectPieceType.Rulers:
                    newKind = C.SelectPieceType.Backgrounds;
                    break;
                case C.SelectPieceType.Backgrounds:
                    newKind = C.SelectPieceType.Terrain;
                    break;
                default:
                    throw new ArgumentException();
            }

            CyclePieceBrowserDisplay(newKind);
        }

        private void MaybeOpenPiecesTab()
        {
            if ((CurLevel.SelectionList().Count > 0) && (!allTabsExpanded))
            {
                tabProperties.SelectedIndex = tabProperties.TabPages.IndexOf(tabPieces);
                PullFocusFromTextInputs();
            }
        }

        /// <summary>
        /// Checks that the selected style is valid and loads it into the piece style combo
        /// </summary>
        private void LoadStyleFromMetaData()
        {
            if (string.IsNullOrWhiteSpace(lblPieceStyle.Text))
            {
                MessageBox.Show("No valid style detected in metadata.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (comboPieceStyle.Items.Cast<string>().Contains(lblPieceStyle.Text))
            {
                comboPieceStyle.Text = lblPieceStyle.Text;
            }
            else
            {
                MessageBox.Show($"Style '{lblPieceStyle.Text}' not found in style list.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void UpdatePieceMetaData()
        {
            if (CurLevel == null)
                return;
            
            LevelPiece currentPiece;
            string pieceName;
            string pieceStyle;
            string pieceType;
            string pieceSize;

            if (CurLevel.SelectionList().Count == 1)
                currentPiece = CurLevel.SelectionList().First();
            else
            {
                panelPieceMetaData.Enabled = false;

                lblPieceName.Text = string.Empty;
                lblPieceStyle.Text = string.Empty;
                lblPieceType.Text = string.Empty;
                lblPieceSize.Text = string.Empty;
                btnLoadStyle.Visible = false;

                return;
            }

            // Get the name of the currently-selected piece
            pieceName = currentPiece.Name;

            // Find the style based on its directory (NameInDirectory)
            Style style = StyleList?.Find(sty => sty.NameInDirectory == currentPiece.Style);

            if (currentPiece.ObjType == C.OBJ.RULER)
                pieceStyle = "(Rulers)";
            else if (currentPiece is GroupPiece)
                pieceStyle = "(Group)";
            else if (style == null)
                pieceStyle = "(Default)";
            else
                pieceStyle = style.NameInEditor;

            // Get the type of the current piece and format appropriately
            pieceType = char.ToUpper(currentPiece.ObjType.ToString()[0]) +
                        currentPiece.ObjType.ToString().Substring(1).ToLower();

            // Get the size of the current piece
            pieceSize = $"{ImageLibrary.GetWidth(currentPiece.Key).ToString()} x {ImageLibrary.GetHeight(currentPiece.Key).ToString()}";

            // Update panel, labels and button
            panelPieceMetaData.Enabled = true;

            lblPieceName.Text = pieceName;
            lblPieceStyle.Text = pieceStyle;
            lblPieceType.Text = pieceType;
            lblPieceSize.Text = pieceSize;

            string[] nonLoadable = { "(Default)", "(Group)", "(Rulers)" };

            if (pieceCurStyle.NameInEditor != pieceStyle && !nonLoadable.Contains(pieceStyle))
                btnLoadStyle.Visible = true;
            else
                btnLoadStyle.Visible = false;
        }


        /// <summary>
        /// Gets the key from the index of the clicked PieceBox.
        /// </summary>
        private string GetPieceKeyFromIndex(int picPieceIndex)
        {
            List<string> pieceList;

            switch (pieceDoDisplayKind)
            {
                case C.SelectPieceType.Objects:
                    pieceList = pieceCurStyle?.ObjectKeys;
                    break;
                case C.SelectPieceType.Terrain:
                    pieceList = pieceCurStyle?.TerrainKeys;
                    break;
                case C.SelectPieceType.Steel:
                    pieceList = pieceCurStyle?.SteelKeys;
                    break;
                case C.SelectPieceType.Backgrounds:
                    pieceList = pieceCurStyle?.BackgroundKeys;
                    break;
                case C.SelectPieceType.Rulers:
                    pieceList = new List<string>(ImageLibrary.RulerKeys);
                    break;
                default:
                    throw new ArgumentException();
            }

            if (pieceList == null || pieceList.Count == 0)
                return String.Empty;

            int actualPicPieceIndex = -1;
            for (int i = 0; i <= picPieceIndex; i++)
            {
                actualPicPieceIndex++;
                if (!DisplaySettings.IsDisplayed(C.DisplayType.Deprecated))
                    while (ImageLibrary.GetDeprecated(pieceList[(pieceStartIndex + actualPicPieceIndex) % pieceList.Count]))
                        actualPicPieceIndex++;
            }

            return pieceList[(pieceStartIndex + actualPicPieceIndex) % pieceList.Count];
        }

        private void AddPieceViaHotkey(int hotkeyIndex)
        {
            Point pos = curRenderer.GetCenterPoint();

            if (picPieceList.Count >= hotkeyIndex -1)
            {
                AddNewPieceToLevel(hotkeyIndex -1, pos);
                UpdateFlagsForPieceActions();
            }
        }

        /// <summary>
        /// Adds a new piece to the level and displays the result to the user.
        /// </summary>
        private void AddNewPieceToLevel(int picPieceIndex, Point pos, bool useSelectedPos = false)
        {
            ReadLevelInfoFromForm(true);
            SaveChangesToOldLevelList();

            string pieceKey = GetPieceKeyFromIndex(picPieceIndex);

            if (pieceKey != "")
                switch (pieceDoDisplayKind)
                {
                    case C.SelectPieceType.Terrain:
                    case C.SelectPieceType.Steel:
                    case C.SelectPieceType.Objects:
                        AddNewPieceToLevel(pieceKey, pos, useSelectedPos);
                        break;
                    case C.SelectPieceType.Rulers:
                        AddRuler(pieceKey);
                        break;
                    case C.SelectPieceType.Backgrounds:
                        string[] splitKey = pieceKey.Split('/', '\\');
                        CurLevel.Background = new Background(pieceCurStyle, splitKey[2]);
                        UpdateBackgroundImage();
                        picLevel.SetImage(curRenderer.CombineLayers());
                        break;
                }

            MaybeOpenPiecesTab();
            UpdatePieceMetaData();
        }

        /// <summary>
        /// Adds a new piece to the level and displays the result to the user.
        /// </summary>
        public void AddNewPieceToLevel(string pieceKey, Point centerPosition, bool useSelectedPos = false)
        {
            CurLevel.UnselectAll();
            CurLevel.AddPiece(pieceKey, centerPosition, gridSize, useSelectedPos);

            SaveChangesToOldLevelList();
            picLevel.Image = curRenderer.CreateLevelImage();
            UpdateFlagsForPieceActions();
            PullFocusFromTextInputs();
        }

        /// <summary>
        /// Changes the selection of existing pieces by adding or removing one piece.
        /// </summary>
        private void LevelSelectSinglePiece()
        {
            // Check whether MouseStartPos is actually in pic_Level
            if (!curRenderer.IsPointInLevelArea() || curRenderer.GetMousePosInLevel() == null)
            {
                CurLevel.UnselectAll();
                return;
            }
            Point levelPos = (Point)curRenderer.GetMousePosInLevel();

            if (removeAllPiecesAtCursorPressed)
            {
                // Remove all pieces below the mouse point.
                var selectArea = new Rectangle(levelPos.X, levelPos.Y, 1, 1);
                CurLevel.SelectAreaPiece(selectArea, false);
            }
            else if (addOrRemoveSinglePiecePressed)
            {
                // Add or remove a single piece, depending on whether there is a selected piece at the mouse position 
                bool doAdd = !CurLevel.HasSelectionAtPos(levelPos);
                CurLevel.SelectOnePiece(levelPos, doAdd, selectPiecesBelowPressed);
            }
            else
            {
                // Select only the one that is below the mouse cursor
                CurLevel.UnselectAll();
                CurLevel.SelectOnePiece(levelPos, true, selectPiecesBelowPressed);
            }

            MaybeOpenPiecesTab();
            UpdatePieceMetaData();
            SaveChangesToOldLevelList();
        }

        /// <summary>
        /// Changes the selection of existing pieces by adding or removing all pieces in a certain area.
        /// </summary>
        private void LevelSelectAreaPieces()
        {
            // Get rectangle from user input
            Rectangle? selectArea = curRenderer.GetCurSelectionInLevel();
            if (selectArea == null)
                return;

            if (mouseButtonPressed == MouseButtons.Left)
            {
                // Delete all existing selections if no modifier is pressed
                if (!isCtrlPressed && !isShiftPressed && !isAltPressed)
                {
                    CurLevel.UnselectAll();
                }

                // Add all pieces intersection SelectArea
                CurLevel.SelectAreaPiece((Rectangle)selectArea, true);
            }
            else if (removeAllPiecesAtCursorPressed)
            {
                // Remove all pieces intersection SelectArea
                CurLevel.SelectAreaPiece((Rectangle)selectArea, false);
            }

            SaveChangesToOldLevelList();
        }

        /// <summary>
        /// Moves all selected pieces of the level and displays the result.
        /// </summary>
        private void HandleMovement(C.DIR direction, int step = 1)
        {
            movementActionPerformed = true;

            if (dragScreenStartPressed)
            {
                MoveScreenStartPosition(direction);
            }
            else if (CurLevel.SelectionList().Count > 0)
            {
                CurLevel.MovePieces(direction, step, gridSize);
                picLevel.Image = curRenderer.CreateLevelImage();
            }
            else
            {
                curRenderer.MoveScreenPos(direction, step * 8);
                picLevel.SetImage(curRenderer.GetScreenImage());
            }
        }

        /// <summary>
        /// Drags all selected pieces from the original mouse position to the current one and displays the result.
        /// </summary>
        private void DragSelectedPieces()
        {
            Point targetPos = curRenderer.GetNewPosFromDragging();
            CurLevel.MovePieces(targetPos, gridSize);
        }

        /// <summary>
        /// Drags all selected pieces along the X-axis only and displays the result.
        /// </summary>
        private void XDragSelectedPieces()
        {
            Point targetPos = curRenderer.GetNewPosFromXDragging();
            CurLevel.MovePieces(targetPos, gridSize);
        }

        /// <summary>
        /// Drags all selected pieces along the Y-axis only and displays the result.
        /// </summary>
        private void YDragSelectedPieces()
        {
            Point targetPos = curRenderer.GetNewPosFromYDragging();
            CurLevel.MovePieces(targetPos, gridSize);
        }

        /// <summary>
        /// Rotates all selected pieces in the level and displays the result.
        /// </summary>
        private void RotateLevelPieces()
        {
            CurLevel.RotatePieces();
            SaveChangesToOldLevelList();
            UpdateFlagsForPieceActions(); // needed for resizable pieces in selection
            picLevel.Image = curRenderer.CreateLevelImage();
        }

        /// <summary>
        /// Inverts all selected pieces in the level and displays the result.
        /// </summary>
        private void InvertLevelPieces()
        {
            CurLevel.InvertPieces();
            SaveChangesToOldLevelList();
            picLevel.Image = curRenderer.CreateLevelImage();
        }

        /// <summary>
        /// Flips all selected pieces in the level and displays the result.
        /// </summary>
        private void FlipLevelPieces()
        {
            CurLevel.FlipPieces();
            SaveChangesToOldLevelList();
            picLevel.Image = curRenderer.CreateLevelImage();
        }

        /// <summary>
        /// Sets the NoOverwrite flag for all selected pieces and displays the result.
        /// </summary>
        private void SetNoOverwrite(bool doAdd)
        {
            CurLevel.SetNoOverwrite(doAdd);
            UpdateFlagsForPieceActions();
            SaveChangesToOldLevelList();
            picLevel.Image = curRenderer.CreateLevelImage();
        }

        /// <summary>
        /// Sets the Erase flag for all selected pieces and displays the result.
        /// </summary>
        private void SetErase(bool doAdd)
        {
            CurLevel.SetErase(doAdd);
            UpdateFlagsForPieceActions();
            SaveChangesToOldLevelList();
            picLevel.Image = curRenderer.CreateLevelImage();
        }

        /// <summary>
        /// Sets the OnlyOnTerrain flag for all selected pieces and displays the result.
        /// </summary>
        private void SetOnlyOnTerrain(bool doAdd)
        {
            CurLevel.SetOnlyOnTerrain(doAdd);
            UpdateFlagsForPieceActions();
            SaveChangesToOldLevelList();
            picLevel.Image = curRenderer.CreateLevelImage();
        }

        /// <summary>
        /// Sets the OneWayAdmissible flag for all selected pieces and displays the result.
        /// </summary>
        private void SetOneWay(bool doAdd)
        {
            CurLevel.SetOneWay(doAdd);
            UpdateFlagsForPieceActions();
            SaveChangesToOldLevelList();
            picLevel.Image = curRenderer.CreateLevelImage();
        }

        /// <summary>
        /// Sets skill flags for all selected objects.
        /// </summary>
        private void SetSkillForObjects(C.Skill skill, bool doAdd)
        {
            CurLevel.SetSkillForObjects(skill, doAdd);
            UpdateFlagsForPieceActions();
            SaveChangesToOldLevelList();
        }

        private Level GetCurLevel()
        {
            return CurLevel;
        }

        /// <summary>
        /// Changes the index of all selected pieces and displays the result.
        /// </summary>
        private void MovePieceIndex(bool toFront, bool onlyOneStep)
        {
            CurLevel.MoveSelectedPieces(toFront, onlyOneStep);
            picLevel.Image = curRenderer.CreateLevelImage();

            SaveChangesToOldLevelList();
        }

        /// <summary>
        /// Saves the current level to the OldLevelList if there were any changes.
        /// </summary>
        private void SaveChangesToOldLevelList()
        {
            if (CurLevel.Equals(oldLevelList[curOldLevelIndex]))
                return;

            oldLevelList = oldLevelList.GetRange(0, curOldLevelIndex + 1);
            oldLevelList.Add(CurLevel.Clone());
            curOldLevelIndex = oldLevelList.Count - 1;

            UpdateSpecialLemmingCounter(); // KLUDGE: Could put this somewhere better.
        }

        private void UpdateSpecialLemmingCounter()
        {
            CurLevel.GetLemmingTypeCounts(out int normalCount, out int zombieCount, out int rivalCount, out int neutralCount);
            string newText =
                normalCount.ToString() + " Normal";
            if (zombieCount > 0)
                newText += ", " + zombieCount.ToString() + " Zombie";
            if (rivalCount > 0)
                newText += ", " + rivalCount.ToString() + " Rival";
            if (neutralCount > 0)
                newText += ", " + neutralCount.ToString() + " Neutral";

            btnLemCount.Text = newText;
        }

        /// <summary>
        /// Loads the level with index fCurOldLevelIndex from the fOldLevelList.
        /// </summary>
        private void LoadFromOldLevelList()
        {
            CurLevel = oldLevelList[curOldLevelIndex].Clone();
            curRenderer.SetLevel(CurLevel);

            WriteLevelInfoToForm();
            UpdateFlagsForPieceActions();
            picLevel.Image = curRenderer.CreateLevelImage();
        }

        /// <summary>
        /// Undos the last change to the level.
        /// </summary>
        private void UndoLastChange()
        {
            if (curOldLevelIndex > 0)
            {
                curOldLevelIndex--;
                LoadFromOldLevelList();
            }
        }

        /// <summary>
        /// Reverts the last Undo action.
        /// </summary>
        private void CancelLastUndo()
        {
            if (curOldLevelIndex < oldLevelList.Count - 1)
            {
                curOldLevelIndex++;
                LoadFromOldLevelList();
            }
        }

        private static readonly long InstanceID = DateTime.Now.Ticks;

        [Serializable()]
        private class ClipboardData
        {
            public List<LevelPiece> Pieces;
            public List<ClipboardGroup> GroupData;
            public long InstanceID;
        }

        [Serializable()]
        private class ClipboardGroup
        {
            public string Name;
            public List<TerrainPiece> Pieces;
        }

        /// <summary>
        /// Copies all currently selected pieces to the fOldSelectedList.
        /// </summary>
        private void WriteToClipboard()
        {
            List<LevelPiece> clipboardPieces = CurLevel.SelectionList().Select(piece => piece.Clone()).ToList();
            List<ClipboardGroup> groupData = new List<ClipboardGroup>();

            foreach (var piece in clipboardPieces)
                if (piece is GroupPiece gp)
                    PrepareClipboardGroup(gp, groupData);

            ClipboardData clipboardData = new ClipboardData()
            {
                Pieces = clipboardPieces,
                GroupData = groupData,
                InstanceID = groupData.Count == 0 ? 0 : InstanceID
            };

            Utility.SetDataToClipboard(clipboardData);
        }

        private void PrepareClipboardGroup(GroupPiece group, List<ClipboardGroup> groupData)
        {
            if (groupData.FirstOrDefault(gd => gd.Name == group.Name) == null)
            {
                ClipboardGroup newGroup = new ClipboardGroup();

                var contents = group.GetConstituents();

                newGroup.Name = group.Name;
                newGroup.Pieces = group.GetConstituents();

                groupData.Insert(0, newGroup);

                foreach (var piece in newGroup.Pieces)
                    if (piece is GroupPiece gp)
                        PrepareClipboardGroup(gp, groupData);
            }
        }

        /// <summary>
        /// Duplicates all selected pieces and displays the result.
        /// </summary>
        private void DuplicateSelectedPieces(C.DIR? direction = null)
        {
            if (CurLevel.SelectionList().Count == 0)
                return;

            var selection = CurLevel.SelectionList();

            CurLevel.UnselectAll();
            CurLevel.AddMultiplePieces(selection);

            if (direction.HasValue)
            {
                C.DIR dir = direction.Value;
                CurLevel.MovePieces(dir, GetDuplicationMoveAmount(dir, selection), gridSize);
            }

            SaveChangesToOldLevelList();
            picLevel.Image = curRenderer.CreateLevelImage();
        }

        /// <summary>
        /// Gets the movement amount for duplication based on the width/height of the selection (depending on direction)
        /// </summary>
        private int GetDuplicationMoveAmount(C.DIR direction, List<LevelPiece> selection)
        {
            if (selection == null || selection.Count == 0)
                return 0;

            int amountMin = 0;
            int amountMax = 0;

            if (direction == C.DIR.E || direction == C.DIR.W)
            {
                amountMin = selection.Min(piece => piece.PosX);
                amountMax = selection.Max(piece => piece.PosX + piece.Width);
            }

            if (direction == C.DIR.N || direction == C.DIR.S)
            {
                amountMin = selection.Min(piece => piece.PosY);
                amountMax = selection.Max(piece => piece.PosY + piece.Height);
            }

            return amountMax - amountMin;
        }

        /// <summary>
        /// Deletes all selected pieces, saves them in memory and displays the result.
        /// </summary>
        private void DeleteSelectedPieces(bool doSaveSelection = true)
        {
            if (doSaveSelection)
                WriteToClipboard();
            CurLevel.TerrainList.RemoveAll(ter => ter.IsSelected);
            CurLevel.GadgetList.RemoveAll(obj => obj.IsSelected);
            SaveChangesToOldLevelList();
            picLevel.Image = curRenderer.CreateLevelImage();
            UpdateFlagsForPieceActions();
        }

        /// <summary>
        /// Selects all pieces
        /// </summary>
        public void SelectAllPieces()
        {
            CurLevel.TerrainList.ForEach(ter => ter.IsSelected = true);
            CurLevel.GadgetList.ForEach(gad => gad.IsSelected = true);

            picLevel.SetImage(curRenderer.GetScreenImage());
            UpdateFlagsForPieceActions();
            PullFocusFromTextInputs();
            UpdatePieceMetaData();
        }

        /// <summary>
        /// Adds all pieces that are stored in memory by previously deleting/copying them.
        /// </summary>
        private void AddFromClipboard(bool doCenterAtCursor)
        {
            CurLevel.UnselectAll();

            ClipboardData clipboardData;
            List<LevelPiece> clipboardPieces = null;
            List<ClipboardGroup> groupData = null;

            try
            {
                clipboardData = Utility.GetDataFromClipboard<ClipboardData>();

                clipboardPieces = clipboardData.Pieces;
                groupData = clipboardData.GroupData;

                if (clipboardPieces == null || clipboardPieces.Count == 0)
                    return;
            }
            catch
            {
                return;
            }

            foreach (var group in groupData)
                new GroupPiece(group.Pieces, group.Name); // Don't need to actually place it at this point.

            if (doCenterAtCursor)
            {
                var newPieces = CenterPiecesAtCursor(clipboardPieces);
                CurLevel.AddMultiplePieces(newPieces);
            }
            else
            {
                CurLevel.AddMultiplePieces(clipboardPieces);
            }
            SaveChangesToOldLevelList();
            picLevel.Image = curRenderer.CreateLevelImage();
        }

        /// <summary>
        /// Centers the collection of pieces around the cursor.
        /// </summary>
        private IEnumerable<LevelPiece> CenterPiecesAtCursor(IEnumerable<LevelPiece> clipPieces)
        {
            Point mousePos = curRenderer.GetMousePosInLevel(picLevel.PointToClient(Cursor.Position));
            int clipPosX = clipPieces.Min(piece => piece.PosX);
            int clipPosY = clipPieces.Min(piece => piece.PosY);
            int clipWidth = clipPieces.Max(piece => piece.PosX + piece.Width) - clipPosX;
            int clipHeight = clipPieces.Max(piece => piece.PosY + piece.Height) - clipPosY;

            var newPieces = new List<LevelPiece>();

            foreach (LevelPiece piece in clipPieces)
            {
                var newPiece = piece.Clone();
                newPiece.PosX = mousePos.X - clipWidth / 2 + (piece.PosX - clipPosX);
                newPiece.PosY = mousePos.Y - clipHeight / 2 + (piece.PosY - clipPosY);
                newPieces.Add(newPiece);
            }

            return newPieces;
        }


        /// <summary>
        /// Pairs a selected teleporter and receiver.
        /// </summary>
        private void PairTeleporters()
        {
            CurLevel.PairTeleporters();
            UpdateFlagsForPieceActions();
        }

        /// <summary>
        /// Groups the selected pieces, if possible.
        /// </summary>
        private void GroupSelectedPieces()
        {
            if (CurLevel.MayGroupSelection())
            {
                bool userHasHighlightErasersEnabled = BmpModify.HighlightErasers;

                // Temporarily disable highlit eraser pieces to ensure these are grouped correctly
                if (userHasHighlightErasersEnabled)
                {
                    BmpModify.HighlightErasers = false;
                    picLevel.Image = curRenderer.CreateLevelImage();
                }

                CurLevel.GroupSelection();
                SaveChangesToOldLevelList();
                UpdateFlagsForPieceActions();
                UpdatePieceMetaData();

                // Reset option before redrawing level
                BmpModify.HighlightErasers = userHasHighlightErasersEnabled;

                picLevel.Image = curRenderer.CreateLevelImage();
            }
        }

        /// <summary>
        /// Ungroups the selected pieces, if possible.
        /// </summary>
        private void UngroupSelectedPieces()
        {
            if (CurLevel.MayUngroupSelection())
            {
                CurLevel.UngroupSelection();
                SaveChangesToOldLevelList();
                UpdateFlagsForPieceActions();
                UpdatePieceMetaData();
                picLevel.Image = curRenderer.CreateLevelImage();
            }
        }

        /// <summary>
        /// Toggles snap-to-grid on and off
        /// </summary>
        public void ToggleSnapToGrid(bool fromHotkey = false)
        {
            if (fromHotkey) curSettings.SwitchGridUsage();

            snapToGridToolStripMenuItem.Checked = curSettings.UseGridForPieces;

            curRenderer.CreateGridLayer();
            picLevel.SetImage(curRenderer.CombineLayers());
        }

        private const string INVALID_AUTOSAVE_NAME_CHARS = "<>:\"/\\|?*.";

        private void MakeAutoSave()
        {
            try
            {
                if (!Directory.Exists(C.AppPathAutosave))
                    Directory.CreateDirectory(C.AppPathAutosave);

                string filename = DateTime.Now.ToString() + " - " + CurLevel.Title;

                foreach (char c in INVALID_AUTOSAVE_NAME_CHARS)
                    filename = filename.Replace(c, '_');

                Level tempLevel = CurLevel.Clone();
                LevelFile.SaveLevelToFile(C.AppPathAutosave + filename + LevelFileExtension(), tempLevel);

                ClearOldAutosaves();
            }
            catch
            {
                // Do nothing. If it fails, it fails.
            }
        }

        private void ClearOldAutosaves()
        {
            if (curSettings.KeepAutosaveCount > 0)
            {
                string[] files = Directory.GetFiles(C.AppPathAutosave, "*" + LevelFileExtension());
                if (files.Length > curSettings.KeepAutosaveCount)
                {
                    List<KeyValuePair<string, long>> fileTimes = new List<KeyValuePair<string, long>>();
                    foreach (var file in files)
                        fileTimes.Add(new KeyValuePair<string, long>(file, File.GetLastWriteTime(file).Ticks));
                    fileTimes = fileTimes.OrderByDescending(item => item.Value).ToList();
                    for (int i = curSettings.KeepAutosaveCount; i < fileTimes.Count; i++)
                        File.Delete(fileTimes[i].Key);
                }
            }
        }

        private void SetAutosaveTimer()
        {
            if (curSettings.AutosaveFrequency > 0)
            {
                timerAutosave.Interval = curSettings.AutosaveFrequency * 60000;
                timerAutosave.Start();
            }
            else
                timerAutosave.Stop();
        }

        private void OpenTemplatesLoader()
        {
            using (var templatesLoader = new FormTemplates(this, curSettings))
            {
                templatesLoader.ShowDialog(this);
            }
        }

        private void ShowAboutSLXEditor()
        {
            using (var aboutSLXEditor = new FormAboutSLXEditor(curSettings))
            {
                aboutSLXEditor.ShowDialog(this);
            }
        }

        private void SetMetaDataPanel()
        {
            panelPieceMetaData.ForeColor = Color.SteelBlue;
            btnLoadStyle.Top = tabPieces.Height - btnLoadStyle.Height;
            panelPieceMetaData.Top = btnLoadStyle.Top - panelPieceMetaData.Height;
            panelPieceMetaData.Left = tabPieces.Left;
            panelPieceMetaData.Width = tabPieces.Width - 5;
        }

        private void SetAllSkillsToZero()
        {
            foreach (Control ctrl in tabSkills.Controls)
            {
                if (ctrl is NumericUpDown numBox && numBox != numRandomMinLimit
                                                 && numBox != numRandomMaxLimit
                                                 && numBox != numAllNonZeroSkillsToN)
                {
                    numBox.Value = 0;
                }
            }
        }

        private void SetAllNonZeroSkillsToN()
        {
            foreach (Control ctrl in tabSkills.Controls)
            {
                if (ctrl is NumericUpDown numBox && numBox != numRandomMinLimit
                                                 && numBox != numRandomMaxLimit
                                                 && numBox != numAllNonZeroSkillsToN)
                {
                    if (numBox.Value != 0)
                        numBox.Value = numAllNonZeroSkillsToN.Value;
                }
            }
        }

        private void GenerateRandomSkillset()
        {
            SetAllSkillsToZero(); // Zero the skillset first
            Random random = new Random();

            int minValue = (int)numRandomMinLimit.Value;
            int maxValue = (int)numRandomMaxLimit.Value;

            // List and shuffle the numeric controls on tabSkills (excluding the randomizer limits and disabled controls)
            List<NumericUpDown> numericUpDowns = tabSkills.Controls.OfType<NumericUpDown>()
                .Where(n => n != numRandomMinLimit &&
                            n != numRandomMaxLimit &&
                            n != numAllNonZeroSkillsToN &&
                            n.Enabled)
                .ToList();
            numericUpDowns = numericUpDowns.OrderBy(x => random.Next()).ToList();

            int maxSkills;

            if (isNeoLemmixOnly)
                maxSkills = 10;
            else
                maxSkills = 14;

            // Select up to 14 skills and populate them with a number between minValue and maxValue
            List<NumericUpDown> selectedControls = numericUpDowns.Take(maxSkills).ToList();
            foreach (var numBox in selectedControls)
            {
                numBox.Value = random.Next(minValue, maxValue + 1); // maxValue + 1 because Random.Next is exclusive on the upper bound
            }
        }

        private string SanitizeNameForFileSystem(string name)
        {
            char[] invalid = Path.GetInvalidFileNameChars();
            return new string(name.Select(c => invalid.Contains(c) ? '_' : c).ToArray());
        }

        private string EnsureUniqueFileName(string folder, string baseName, string ext)
        {
            string name = baseName;
            int count = 0;
            string fullPath = Path.Combine(folder, name + ext);

            while (File.Exists(fullPath))
            {
                count++;
                name = $"{baseName} ({count})";
                fullPath = Path.Combine(folder, name + ext);
            }

            return fullPath;
        }

        private void SaveLevelBitmapAsImage(string path)
        {
            Bitmap fullLevelImage = curRenderer.GetFullLevelImage();
            fullLevelImage.Save(path, ImageFormat.Png);
        }

        private void SaveLevelAsTemplate()
        {
            string newName = InputDialog.Show("Enter a name for the template:", "Save Level As Template");
            if (string.IsNullOrWhiteSpace(newName)) return;

            newName = SanitizeNameForFileSystem(newName);

            // Save the level
            string templatePath = Path.Combine(C.AppPathTemplates, newName + ".template");
            if (File.Exists(templatePath))
            {
                var result = MessageBox.Show($"{newName} already exists in templates folder. Would you like to overwrite it?",
                    "Template Already Exists",
                    MessageBoxButtons.YesNo);

                if (result == DialogResult.No)
                    templatePath = EnsureUniqueFileName(C.AppPathTemplates, newName, ".template");
            }
            LevelFile.SaveLevelToFile(templatePath, CurLevel);

            // Save the image with matching name
            string pngPath = Path.ChangeExtension(templatePath, ".png");
            SaveLevelBitmapAsImage(pngPath);

            MessageBox.Show($"Template saved successfully!");
        }

        private void SaveLevelAsImage()
        {
            string baseName = string.IsNullOrEmpty(CurLevel.Title) ? "Level" : CurLevel.Title;
            baseName = SanitizeNameForFileSystem(baseName);
            string imagePath = EnsureUniqueFileName(C.AppPath, baseName, ".png");

            SaveLevelBitmapAsImage(imagePath);

            MessageBox.Show($"Image saved as:\n{imagePath}", "Save Successful",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void HighlightGroupedPieces()
        {
            BmpModify.HighlightGroups = !BmpModify.HighlightGroups;
            highlightGroupedPiecesToolStripMenuItem.Checked = BmpModify.HighlightGroups;
            picLevel.SetImage(curRenderer.CreateLevelImage());
            Properties.Settings.Default.Save();
        }

        private void HighlightEraserPieces()
        {
            BmpModify.HighlightErasers = !BmpModify.HighlightErasers;
            highlightEraserPiecesToolStripMenuItem.Checked = BmpModify.HighlightErasers;
            picLevel.SetImage(curRenderer.CreateLevelImage());
            curSettings.WriteSettingsToFile();
        }

        private void ToggleClearPhysics()
        {
            DisplaySettings.ChangeDisplayed(C.DisplayType.ClearPhysics);
            picLevel.SetImage(curRenderer.CreateLevelImage());
        }

        private void ToggleTerrain()
        {
            DisplaySettings.ChangeDisplayed(C.DisplayType.Terrain);
            picLevel.SetImage(curRenderer.CombineLayers());
        }

        private void ToggleObjects()
        {
            DisplaySettings.ChangeDisplayed(C.DisplayType.Objects);
            picLevel.SetImage(curRenderer.CombineLayers());
        }

        private void ToggleTriggerAreas()
        {
            DisplaySettings.ChangeDisplayed(C.DisplayType.Triggers);
            picLevel.SetImage(curRenderer.CombineLayers());
        }
        private void ToggleRulers()
        {
            DisplaySettings.ChangeDisplayed(C.DisplayType.Rulers);
            picLevel.SetImage(curRenderer.CombineLayers());
        }

        private void ToggleScreenStart()
        {
            DisplaySettings.ChangeDisplayed(C.DisplayType.ScreenStart);
            picLevel.SetImage(curRenderer.CombineLayers());
        }

        private void ToggleBackground()
        {
            DisplaySettings.ChangeDisplayed(C.DisplayType.Background);
            picLevel.SetImage(curRenderer.CombineLayers());
        }

        private void ToggleDeprecatedPieces()
        {
            DisplaySettings.ChangeDisplayed(C.DisplayType.Deprecated);
            LoadPiecesIntoPictureBox();
        }

        private void SetScreenStartToCursor()
        {
            Point mousePos = curRenderer.GetMousePosInLevel(picLevel.PointToClient(Cursor.Position));
            MoveScreenStartPosition(new Point(mousePos.X, mousePos.Y));
        }

        private void ZoomIn()
        {
            curRenderer.ChangeZoom(1, false);
            RepositionPicLevel();
            picLevel.SetImage(curRenderer.GetScreenImage());
        }

        private void ZoomOut()
        {
            curRenderer.ChangeZoom(-1, false);
            RepositionPicLevel();
            picLevel.SetImage(curRenderer.GetScreenImage());
        }

        /// <summary>
        /// This automatically links all controls to the mouse events that show hints in the hint label
        /// </summary>
        private void LinkControlsToMouseEvents(Control parent)
        {
            foreach (Control ctrl in parent.Controls)
            {
                if (ctrl is Button || ctrl is CheckBox || ctrl is ComboBox ||
                    ctrl is TextBox || ctrl is RadioButton)
                {
                    ctrl.MouseEnter += Control_MouseEnter;
                    ctrl.MouseLeave += Control_MouseLeave;
                }

                if (ctrl is NumUpDownOverwrite)
                {
                    ctrl.Enter += Control_MouseEnter;
                    ctrl.MouseMove += Control_MouseEnter;
                    ctrl.MouseLeave += Control_MouseLeave;
                }

                if (ctrl.HasChildren)
                    LinkControlsToMouseEvents(ctrl);
            }
        }

        public void UpdateControlHintLabel(bool showHint, object sender)
        {
            lblHint.Visible = false;
            lblHint.Text = "";

            if (!curSettings.ShowControlHints || !showHint)
                return;

            if (sender is Control ctrl && ctrl.Tag is string hint)
            {
                lblHint.Text = hint;
                lblHint.Visible = true;
            }
        }

        private void UpdateControlTags()
        {
            // --- Globals Tab --- //
            txtLevelTitle.Tag = "Enter a title for your level";
            txtLevelAuthor.Tag = "Enter an author name";
            comboMusic.Tag = "Choose a music track for your level (note that you can also set music for a full pack of levels by including a music.nxmi file with your pack)";
            comboTheme.Tag = "Choose a theme style for your level";
            numWidth.Tag = "Set the level width";
            numHeight.Tag = "Set the level height";
            btnCropLevel.Tag = "Activate crop rectangle to easily change the width and height of the level";
            btnApplyCrop.Tag = "Apply crop rectangle as new width and height";
            btnCancelCrop.Tag = "Cancel crop and close the rectangle";
            numStartX.Tag = "Set the horizontal start position";
            numStartY.Tag = "Set the vertical start position";
            checkAutoStart.Tag = "Automatically set the start position";
            numLemmings.Tag = "Set the total number of lemmings";
            numRescue.Tag = "Set the amount of lemmings to be saved";
            btnLemCount.Tag = "Automatically set the lemming count based on the number of pre-placed lemmings and zombies";
            numRR.Tag = "Set the minimum release rate";
            numSI.Tag = "Set the maximum spawn interval";
            string textRRSI = curSettings.UseSpawnInterval ? "spawn interval" : "release rate";
            checkLockRRSI.Tag = $"Lock the {textRRSI} (prevents it from being changed in-game)";
            checkTimeLimit.Tag = "Apply a time limit, or leave this unchecked for infinite time";
            numTimeMins.Tag = "Set the number of minutes for the time limit";
            numTimeSecs.Tag = "Set the number of seconds for the time limit";
            txtLevelID.Tag = "Edit the Level ID (note that doing so will unlink any existing replays for this level)";
            btnRandomID.Tag = "Generate a random Level ID (note that doing so will unlink any existing replays for this level)";

            // --- Pieces Tab --- //
            btnRotate.Tag = "Rotate all selected pieces clockwise";
            btnInvert.Tag = "Invert all selected pieces";
            btnFlip.Tag = "Flip all selected pieces";
            btnDrawLast.Tag = "Move all selected pieces to the backmost layer";
            btnDrawLater.Tag = "Move all selected pieces one layer backward";
            btnDrawSooner.Tag = "Move all selected pieces one layer forward";
            btnDrawFirst.Tag = "Move all selected pieces to the frontmost layer";
            btnGroupSelection.Tag = "Group the selected pieces (note that terrain can only be grouped with terrain, steel can only be grouped with steel, and objects cannot be grouped)";
            btnUngroupSelection.Tag = "Ungroup the selected pieces (if they are grouped)";
            checkErase.Tag = "Set selected pieces as eraser pieces";
            checkNoOverwrite.Tag = "Set selected pieces as no-overwrite (terrain is drawn to the backmost layer and is not erased by eraser pieces, objects are drawn to the frontmost layer)";
            checkOnlyOnTerrain.Tag = "When checked, selected object pieces are drawn only where there is terrain";
            checkAllowOneWay.Tag = "When checked, selected terrain pieces can have One-Way objects applied to them";
            numLemmingLimit.Tag = "Set the maximum lemming limit for the selected object";
            numPickupSkillCount.Tag = "Set the skill count applied to the selected pickup";
            btnPairTeleporter.Tag = "Pair the selected teleporter and receiver (or pair of portals)";
            numCountdown.Tag = "Set the countdown timer for the selected radiation/slowfreeze object";
            checkInvincibility.Tag = "When checked, the first lemming to grab the last remaining collectible will become invincible! (note that the level must have at least 1 collectible for this to apply)";
            comboDecorationDirection.Tag = "Set the movement direction of the selected decoration piece";
            numDecorationSpeed.Tag = "Set the movement speed of the selected decoration piece";
            numResizeWidth.Tag = "Set the width of the selected piece";
            numResizeHeight.Tag = "Set the height of the selected piece";
            btnLoadStyle.Tag = "Load the style of the selected piece into the Piece Browser";

            // --- Skills Tab --- //

            foreach (Control ctrl in tabSkills.Controls)
            {
                if (ctrl is NumericUpDown numBox && numBox != numRandomMinLimit && numBox != numRandomMaxLimit && ctrl != numAllNonZeroSkillsToN)
                {
                    string skillName = ctrl.Name.Replace("num", ""); // Get the skill name from the control name
                    skillName = new string(skillName.SelectMany(c => char.IsUpper(c) ? new[] { ' ', c } : new[] { c }).ToArray()).Trim();
                    ctrl.Tag = $"Set the number of available {skillName} skills";
                }
            }
            btnSaveAsCustomSkillset.Tag = "Save the current skillset as a custom skillset that can be loaded in any level";
            comboCustomSkillset.Tag = "Load and apply a custom skillset";
            btnRandomSkillset.Tag = "Generate a random skillset (you can set the lower and upper limits for the randomizer using the two number inputs below)";
            numRandomMinLimit.Tag = "Set the minimum limit for the random skillset generator";
            numRandomMaxLimit.Tag = "Set the maximum limit for the random skillset generator";
            btnAllNonZeroSkillsToN.Tag = "Set all non-zero skills to the same number (set that number using the input to the right)";
            numAllNonZeroSkillsToN.Tag = "Set the number used by the 'Set All Non-Zero Skills To N' button";
            btnClearAllSkills.Tag = "Set all skills to zero";

            // --- Extras Tab --- //

            btnTalismanAdd.Tag = "Add a new Talisman (an additional challenge for the player to complete)";
            btnTalismanEdit.Tag = "Edit the currently selected Talisman";
            btnTalismanDelete.Tag = "Delete the currently selected Talisman";
            btnTalismanMoveUp.Tag = "Move the currently selected Talisman up in the list";
            btnTalismanMoveDown.Tag = "Move the currently selected Talisman down in the list";
            btnEditPreview.Tag = "Edit onscreen text shown before the level is played";
            btnEditPostview.Tag = "Edit onscreen text shown after the level is completed";
            checkSuperlemming.Tag = "Set the game to superlemming speed (3x normal speed) for the current level";
            radOnlyWhenVisible.Tag = "Set steel to only apply where it is visible (NeoLemmix-style)";
            radAlwaysSteel.Tag = "Set steel to apply wherever there is a steel piece in the level, even if it is partially or fully obscured";

            // --- Piece Browser --- //
            btnStyleRandom.Tag = "Open a random style in the Piece Browser (you can add styles to the randomizer in Style Manager)";
            comboPieceStyle.Tag = "Open a style in the Piece Browser";
            btnTerrain.Tag = "Show Terrain pieces in the Piece Browser";
            btnSteel.Tag = "Show Steel pieces in the Piece Browser";
            btnObjects.Tag = "Show Objects (Entrances, Exits, Water, etc) in the Piece Browser";
            btnRulers.Tag = "Show Rulers in the Piece Browser (these can be used to fine-tune your level, and will not appear when the level is played in SuperLemmix)";
            btnBackgrounds.Tag = "Show Background wallpapers in the Piece Browser (these are purely decorative and do not affect gameplay)";
            btnClearBackground.Tag = "Remove the currently-active background wallpaper";
            btnSearchPieces.Tag = "Search the styles collection for pieces by name, object type, and various other properties";
        }

        private void SetHotkeys()
        {
            HotkeyConfig.GetDefaultHotkeys();

            if (File.Exists(C.AppPathHotkeys))
            {
                HotkeyConfig.LoadHotkeysFromIniFile();

                // Merge player hotkeys with any newly-added defaults
                if (HotkeyConfig.PlayerHotkeysLoaded)
                    HotkeyConfig.SaveHotkeysToIniFile();
            }

            InitializeHotkeyActions();
            UpdateMenuShortcutKeyDisplayStrings();
        }

        private void InitializeHotkeyActions()
        {
            hotkeyActions = new Dictionary<Keys, Action>();

            void AddHotkey(HotkeyConfig.HotkeyName name, Action action)
            {
                var hotkeyData = HotkeyConfig.GetHotkey(name);
                if (hotkeyData != null && hotkeyData.CurrentKeys != Keys.None)
                {
                    hotkeyActions[hotkeyData.CurrentKeys] = action;
                }
            }

            AddHotkey(HotkeyName.HotkeyCreateNewLevel, () => CreateNewLevelAndRenderer());
            AddHotkey(HotkeyName.HotkeyLoadLevel, () => LoadNewLevel());
            AddHotkey(HotkeyName.HotkeySaveLevel, () => SaveLevel());
            AddHotkey(HotkeyName.HotkeySaveLevelAs, () => SaveLevelAsNewFile());
            AddHotkey(HotkeyName.HotkeySaveLevelAsImage, () => SaveLevelAsImage());
            AddHotkey(HotkeyName.HotkeyExportLevelAsINI, () => OpenExportAsINI());
            AddHotkey(HotkeyName.HotkeyOpenTemplate, () => OpenTemplatesLoader());
            AddHotkey(HotkeyName.HotkeySaveLevelAsTemplate, () => SaveLevelAsTemplate());
            AddHotkey(HotkeyName.HotkeyPlaytestLevel, () => PlaytestLevel());
            AddHotkey(HotkeyName.HotkeyValidateLevel, () => ValidateLevel(false, false));
            AddHotkey(HotkeyName.HotkeyCleanseLevels, () => ShowCleanseLevelsDialog());
            AddHotkey(HotkeyName.HotkeyHighlightGroupedPieces, () => HighlightGroupedPieces());
            AddHotkey(HotkeyName.HotkeyHighlightEraserPieces, () => HighlightEraserPieces());
            AddHotkey(HotkeyName.HotkeyToggleClearPhysics, () => ToggleClearPhysics());
            AddHotkey(HotkeyName.HotkeyToggleTerrain, () => ToggleTerrain());
            AddHotkey(HotkeyName.HotkeyToggleObjects, () => ToggleObjects());
            AddHotkey(HotkeyName.HotkeyToggleTriggerAreas, () => ToggleTriggerAreas());
            AddHotkey(HotkeyName.HotkeyToggleRulers, () => ToggleRulers());
            AddHotkey(HotkeyName.HotkeyToggleScreenStart, () => ToggleScreenStart());
            AddHotkey(HotkeyName.HotkeyToggleBackground, () => ToggleBackground());
            AddHotkey(HotkeyName.HotkeyToggleDeprecatedPieces, () => ToggleDeprecatedPieces());
            AddHotkey(HotkeyName.HotkeyShowMissingPieces, () => ShowMissingPiecesDialog());
            AddHotkey(HotkeyName.HotkeyRefreshStyles, () => RefreshStyles());
            AddHotkey(HotkeyName.HotkeyOpenStyleManager, () => OpenStyleManager());
            AddHotkey(HotkeyName.HotkeyPieceSearch, () => OpenPieceSearch());
            AddHotkey(HotkeyName.HotkeyToggleSnapToGrid, () => ToggleSnapToGrid(true));
            AddHotkey(HotkeyName.HotkeyToggleCrop, () => HandleCropLevel());
            AddHotkey(HotkeyName.HotkeyOpenLevelArrangerWindow, () => OpenLevelArrangerWindow());
            AddHotkey(HotkeyName.HotkeyOpenPieceBrowserWindow, () => OpenPieceBrowserWindow());
            AddHotkey(HotkeyName.HotkeyToggleAllTabs, () => ToggleExpandedTabs());
            AddHotkey(HotkeyName.HotkeyOpenSettings, () => settingsToolStripMenuItem_Click(null, null));
            AddHotkey(HotkeyName.HotkeyOpenConfigHotkeys, () => hotkeysToolStripMenuItem_Click(null, null));
            AddHotkey(HotkeyName.HotkeyOpenAboutSLX, () => ShowAboutSLXEditor());
            AddHotkey(HotkeyName.HotkeySelectPieces, () => {/* deliberately does nothing */});
            AddHotkey(HotkeyName.HotkeyDragToScroll, () => dragToScrollPressed = true);
            AddHotkey(HotkeyName.HotkeyDragHorizontally, () => dragHorizontallyPressed = true);
            AddHotkey(HotkeyName.HotkeyDragVertically, () => dragVerticallyPressed = true);
            AddHotkey(HotkeyName.HotkeyMoveScreenStart, () => dragScreenStartPressed = true);
            AddHotkey(HotkeyName.HotkeySetScreenStartToCursor, () => SetScreenStartToCursor());
            AddHotkey(HotkeyName.HotkeyRemovePiecesAtCursor, () => removeAllPiecesAtCursorPressed = true);
            AddHotkey(HotkeyName.HotkeyAddRemoveSinglePiece, () => addOrRemoveSinglePiecePressed = true);
            AddHotkey(HotkeyName.HotkeySelectPiecesBelow, () => selectPiecesBelowPressed = true);
            AddHotkey(HotkeyName.HotkeyZoomIn, () => ZoomIn());
            AddHotkey(HotkeyName.HotkeyZoomOut, () => ZoomOut());
            AddHotkey(HotkeyName.HotkeyScrollHorizontally, () => scrollHorizontallyPressed = true);
            AddHotkey(HotkeyName.HotkeyScrollVertically, () => scrollVerticallyPressed = true);
            AddHotkey(HotkeyName.HotkeyShowPreviousPiece, () => MoveTerrPieceSelection(-1));
            AddHotkey(HotkeyName.HotkeyShowNextPiece, () => MoveTerrPieceSelection(1));
            AddHotkey(HotkeyName.HotkeyShowPreviousGroup, () => MoveTerrPieceSelection(-13));
            AddHotkey(HotkeyName.HotkeyShowNextGroup, () => MoveTerrPieceSelection(13));
            AddHotkey(HotkeyName.HotkeyShowPreviousStyle, () => ChangeNewPieceStyleSelection(-1));
            AddHotkey(HotkeyName.HotkeyShowNextStyle, () => ChangeNewPieceStyleSelection(1));
            AddHotkey(HotkeyName.HotkeyCycleBrowser, () => CyclePieceBrowser());
            AddHotkey(HotkeyName.HotkeyAddPiece1, () => AddPieceViaHotkey(1));
            AddHotkey(HotkeyName.HotkeyAddPiece2, () => AddPieceViaHotkey(2));
            AddHotkey(HotkeyName.HotkeyAddPiece3, () => AddPieceViaHotkey(3));
            AddHotkey(HotkeyName.HotkeyAddPiece4, () => AddPieceViaHotkey(4));
            AddHotkey(HotkeyName.HotkeyAddPiece5, () => AddPieceViaHotkey(5));
            AddHotkey(HotkeyName.HotkeyAddPiece6, () => AddPieceViaHotkey(6));
            AddHotkey(HotkeyName.HotkeyAddPiece7, () => AddPieceViaHotkey(7));
            AddHotkey(HotkeyName.HotkeyAddPiece8, () => AddPieceViaHotkey(8));
            AddHotkey(HotkeyName.HotkeyAddPiece9, () => AddPieceViaHotkey(9));
            AddHotkey(HotkeyName.HotkeyAddPiece10, () => AddPieceViaHotkey(10));
            AddHotkey(HotkeyName.HotkeyAddPiece11, () => AddPieceViaHotkey(11));
            AddHotkey(HotkeyName.HotkeyAddPiece12, () => AddPieceViaHotkey(12));
            AddHotkey(HotkeyName.HotkeyAddPiece13, () => AddPieceViaHotkey(13));
            AddHotkey(HotkeyName.HotkeyUndo, () => UndoLastChange());
            AddHotkey(HotkeyName.HotkeyRedo, () => CancelLastUndo());
            AddHotkey(HotkeyName.HotkeySelectAll, () => SelectAllPieces());
            AddHotkey(HotkeyName.HotkeyCut, () => DeleteSelectedPieces());
            AddHotkey(HotkeyName.HotkeyCopy, () => WriteToClipboard());
            AddHotkey(HotkeyName.HotkeyPaste, () => AddFromClipboard(true));
            AddHotkey(HotkeyName.HotkeyPasteInPlace, () => AddFromClipboard(false));
            AddHotkey(HotkeyName.HotkeyDuplicate, () => DuplicateSelectedPieces());
            AddHotkey(HotkeyName.HotkeyDuplicateUp, () => DuplicateSelectedPieces(C.DIR.N));
            AddHotkey(HotkeyName.HotkeyDuplicateDown, () => DuplicateSelectedPieces(C.DIR.S));
            AddHotkey(HotkeyName.HotkeyDuplicateLeft, () => DuplicateSelectedPieces(C.DIR.W));
            AddHotkey(HotkeyName.HotkeyDuplicateRight, () => DuplicateSelectedPieces(C.DIR.E));
            AddHotkey(HotkeyName.HotkeyDelete, () => DeleteSelectedPieces(false));
            AddHotkey(HotkeyName.HotkeyMoveUp, () => HandleMovement(C.DIR.N, 1));
            AddHotkey(HotkeyName.HotkeyMoveDown, () => HandleMovement(C.DIR.S, 1));
            AddHotkey(HotkeyName.HotkeyMoveLeft, () => HandleMovement(C.DIR.W, 1));
            AddHotkey(HotkeyName.HotkeyMoveRight, () => HandleMovement(C.DIR.E, 1));
            AddHotkey(HotkeyName.HotkeyGridMoveUp, () => HandleMovement(C.DIR.N, gridMoveAmount));
            AddHotkey(HotkeyName.HotkeyGridMoveDown, () => HandleMovement(C.DIR.S, gridMoveAmount));
            AddHotkey(HotkeyName.HotkeyGridMoveLeft, () => HandleMovement(C.DIR.W, gridMoveAmount));
            AddHotkey(HotkeyName.HotkeyGridMoveRight, () => HandleMovement(C.DIR.E, gridMoveAmount));
            AddHotkey(HotkeyName.HotkeyCustomMoveUp, () => HandleMovement(C.DIR.N, customMove));
            AddHotkey(HotkeyName.HotkeyCustomMoveDown, () => HandleMovement(C.DIR.S, customMove));
            AddHotkey(HotkeyName.HotkeyCustomMoveLeft, () => HandleMovement(C.DIR.W, customMove));
            AddHotkey(HotkeyName.HotkeyCustomMoveRight, () => HandleMovement(C.DIR.E, customMove));
            AddHotkey(HotkeyName.HotkeyRotate, () => RotateLevelPieces());
            AddHotkey(HotkeyName.HotkeyFlip, () => FlipLevelPieces());
            AddHotkey(HotkeyName.HotkeyInvert, () => InvertLevelPieces());
            AddHotkey(HotkeyName.HotkeyGroup, () => GroupSelectedPieces());
            AddHotkey(HotkeyName.HotkeyUngroup, () => UngroupSelectedPieces());
            AddHotkey(HotkeyName.HotkeyErase, () => checkErase.Checked = !checkErase.Checked);
            AddHotkey(HotkeyName.HotkeyNoOverwrite, () => checkNoOverwrite.Checked = !checkNoOverwrite.Checked);
            AddHotkey(HotkeyName.HotkeyOnlyOnTerrain, () => checkOnlyOnTerrain.Checked = !checkOnlyOnTerrain.Checked);
            AddHotkey(HotkeyName.HotkeyAllowOneWay, () => checkAllowOneWay.Checked = !checkAllowOneWay.Checked);
            AddHotkey(HotkeyName.HotkeyDrawLast, () => MovePieceIndex(true, false));
            AddHotkey(HotkeyName.HotkeyDrawSooner, () => MovePieceIndex(true, true));
            AddHotkey(HotkeyName.HotkeyDrawLater, () => MovePieceIndex(false, true));
            AddHotkey(HotkeyName.HotkeyDrawFirst, () => MovePieceIndex(false, false));
            AddHotkey(HotkeyName.HotkeyCloseEditor, () => Application.Exit());
        }

        /// <summary>
        /// This only updates the hotkey strings in menu items
        /// The hotkey-action linkups are done in InitializeHotkeyActions
        /// </summary>
        private void UpdateMenuShortcutKeyDisplayStrings()
        {           
            newToolStripMenuItem.ShortcutKeyDisplayString =
                FormatHotkeyString(HotkeyName.HotkeyCreateNewLevel);

            loadToolStripMenuItem.ShortcutKeyDisplayString =
                FormatHotkeyString(HotkeyName.HotkeyLoadLevel);

            saveToolStripMenuItem.ShortcutKeyDisplayString =
                FormatHotkeyString(HotkeyName.HotkeySaveLevel);

            saveAsToolStripMenuItem.ShortcutKeyDisplayString =
                FormatHotkeyString(HotkeyName.HotkeySaveLevelAs);

            saveAsImageToolStripMenuItem.ShortcutKeyDisplayString =
                FormatHotkeyString(HotkeyName.HotkeySaveLevelAsImage);

            exportAsINIToolStripMenuItem.ShortcutKeyDisplayString =
                FormatHotkeyString(HotkeyName.HotkeyExportLevelAsINI);

            openTemplateToolStripMenuItem.ShortcutKeyDisplayString =
                FormatHotkeyString(HotkeyName.HotkeyOpenTemplate);

            saveAsTemplateToolStripMenuItem.ShortcutKeyDisplayString =
                FormatHotkeyString(HotkeyName.HotkeySaveLevelAsTemplate);

            exitToolStripMenuItem.ShortcutKeyDisplayString =
                FormatHotkeyString(HotkeyName.HotkeyCloseEditor);

            playLevelToolStripMenuItem.ShortcutKeyDisplayString =
                FormatHotkeyString(HotkeyName.HotkeyPlaytestLevel);

            validateLevelToolStripMenuItem.ShortcutKeyDisplayString =
                FormatHotkeyString(HotkeyName.HotkeyValidateLevel);

            cleanseLevelsToolStripMenuItem.ShortcutKeyDisplayString =
                FormatHotkeyString(HotkeyName.HotkeyCleanseLevels);

            undoToolStripMenuItem.ShortcutKeyDisplayString =
                FormatHotkeyString(HotkeyName.HotkeyUndo);

            redoToolStripMenuItem.ShortcutKeyDisplayString =
                FormatHotkeyString(HotkeyName.HotkeyRedo);

            selectAllToolStripMenuItem.ShortcutKeyDisplayString =
                FormatHotkeyString(HotkeyName.HotkeySelectAll);

            cutToolStripMenuItem.ShortcutKeyDisplayString =
                FormatHotkeyString(HotkeyName.HotkeyCut);

            copyToolStripMenuItem.ShortcutKeyDisplayString =
                FormatHotkeyString(HotkeyName.HotkeyCopy);

            pasteToolStripMenuItem.ShortcutKeyDisplayString =
                FormatHotkeyString(HotkeyName.HotkeyPaste);

            pasteInPlaceToolStripMenuItem.ShortcutKeyDisplayString =
                FormatHotkeyString(HotkeyName.HotkeyPasteInPlace);

            duplicateToolStripMenuItem.ShortcutKeyDisplayString =
                FormatHotkeyString(HotkeyName.HotkeyDuplicate);

            groupToolStripMenuItem.ShortcutKeyDisplayString =
                FormatHotkeyString(HotkeyName.HotkeyGroup);

            ungroupToolStripMenuItem.ShortcutKeyDisplayString =
                FormatHotkeyString(HotkeyName.HotkeyUngroup);

            highlightGroupedPiecesToolStripMenuItem.ShortcutKeyDisplayString =
                FormatHotkeyString(HotkeyName.HotkeyHighlightGroupedPieces);

            highlightEraserPiecesToolStripMenuItem.ShortcutKeyDisplayString =
                FormatHotkeyString(HotkeyName.HotkeyHighlightEraserPieces);

            clearPhysicsToolStripMenuItem.ShortcutKeyDisplayString =
                FormatHotkeyString(HotkeyName.HotkeyToggleClearPhysics);

            terrainToolStripMenuItem.ShortcutKeyDisplayString =
                FormatHotkeyString(HotkeyName.HotkeyToggleTerrain);

            objectToolStripMenuItem.ShortcutKeyDisplayString =
                FormatHotkeyString(HotkeyName.HotkeyToggleObjects);

            triggerAreasToolStripMenuItem.ShortcutKeyDisplayString =
                FormatHotkeyString(HotkeyName.HotkeyToggleTriggerAreas);

            rulersToolStripMenuItem.ShortcutKeyDisplayString =
                FormatHotkeyString(HotkeyName.HotkeyToggleRulers);

            screenStartToolStripMenuItem.ShortcutKeyDisplayString =
                FormatHotkeyString(HotkeyName.HotkeyToggleScreenStart);

            backgroundToolStripMenuItem.ShortcutKeyDisplayString =
                FormatHotkeyString(HotkeyName.HotkeyToggleBackground);

            deprecatedPiecesToolStripMenuItem.ShortcutKeyDisplayString =
                FormatHotkeyString(HotkeyName.HotkeyToggleDeprecatedPieces);

            showMissingPiecesToolStripMenuItem.ShortcutKeyDisplayString =
                FormatHotkeyString(HotkeyName.HotkeyShowMissingPieces);

            refreshStylesToolStripMenuItem.ShortcutKeyDisplayString =
                FormatHotkeyString(HotkeyName.HotkeyRefreshStyles);

            styleManagerToolStripMenuItem.ShortcutKeyDisplayString =
                FormatHotkeyString(HotkeyName.HotkeyOpenStyleManager);

            searchPiecesToolStripMenuItem.ShortcutKeyDisplayString =
                FormatHotkeyString(HotkeyName.HotkeyPieceSearch);

            snapToGridToolStripMenuItem.ShortcutKeyDisplayString =
                FormatHotkeyString(HotkeyName.HotkeyToggleSnapToGrid);

            openLevelWindowToolStripMenuItem.ShortcutKeyDisplayString =
                FormatHotkeyString(HotkeyName.HotkeyOpenLevelArrangerWindow);

            openPieceBrowserWindowToolStripMenuItem.ShortcutKeyDisplayString =
                FormatHotkeyString(HotkeyName.HotkeyOpenPieceBrowserWindow);

            expandAllTabsToolStripMenuItem.ShortcutKeyDisplayString =
                FormatHotkeyString(HotkeyName.HotkeyToggleAllTabs);

            settingsToolStripMenuItem.ShortcutKeyDisplayString =
                FormatHotkeyString(HotkeyName.HotkeyOpenSettings);

            hotkeysToolStripMenuItem.ShortcutKeyDisplayString =
                FormatHotkeyString(HotkeyName.HotkeyOpenConfigHotkeys);

            aboutToolStripMenuItem.ShortcutKeyDisplayString =
                FormatHotkeyString(HotkeyName.HotkeyOpenAboutSLX);
        }
    }
}
