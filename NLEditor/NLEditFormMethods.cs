using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NLEditor
{
    partial class NLEditForm
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
            // UsePieceSelectionNames is automatically updated when calling LoadPiecesIntoPictureBox(), so this doesn't need to be done here.
            pic_Level.Image = curRenderer.CombineLayers();

            snapToGridToolStripMenuItem.Checked = curSettings.UseGridForPieces;

            MoveControlsOnFormResize();
            ResetLevelImage();
        }

        /// <summary>
        /// Sets fStyleList and creates the styles, but does not yet load sprites.
        /// </summary>
        private void CreateStyleList()
        {
            Backgrounds = new BackgroundList();

            // get list of all existing style names
            List<string> styleNameList = new List<string>();

            if (System.IO.Directory.Exists(C.AppPathPieces))
            {
                try
                {
                    styleNameList = System.IO.Directory.GetDirectories(C.AppPathPieces)
                                                       .Select(dir => System.IO.Path.GetFileName(dir))
                                                       .ToList();
                }
                catch (Exception Ex)
                {
                    Utility.LogException(Ex);

                    MessageBox.Show("Error: Could not read the style folders." + C.NewLine + Ex.Message, "Error loading styles");
                }
            }
            else
            {
                MessageBox.Show("Warning: The folder 'styles' is missing.", "Styles missing");
            }
            // Create the StyleList from the StyleNameList
            styleNameList.RemoveAll(sty => sty == "default");
            StyleList = styleNameList.ConvertAll(sty => new Style(sty, Backgrounds));
            StyleList = LoadStylesFromFile.OrderAndRenameStyles(StyleList);

            Backgrounds.SortBackgrounds();
        }

        /// <summary>
        /// Sets the music options according to available files in the music folder.
        /// </summary>
        private void SetMusicList()
        {
            List<string> musicNames = null;
            if (System.IO.Directory.Exists(C.AppPathMusic))
            {
                musicNames = System.IO.Directory.GetFiles(C.AppPathMusic)
                                                .ToList()
                                                .FindAll(dir => System.IO.Path.GetExtension(dir).In(C.MusicExtensions))
                                                .ConvertAll(dir => System.IO.Path.GetFileNameWithoutExtension(dir));
            }
            else
            {
                musicNames = C.MusicNames;
            }

            combo_Music.Items.Clear();
            musicNames.ForEach(music => combo_Music.Items.Add(music));
        }

        /// <summary>
        /// Removes focus from the current control and moves it to the default location txt_Focus.
        /// </summary>
        private void PullFocusFromTextInputs()
        {
            this.ActiveControl = txt_Focus;
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
            isPPressed = ((ModifierKeys & Keys.P) != 0);
        }

        /// <summary>
        /// Takes the global level data input on the form and stores it in the current level.
        /// </summary>
        private void ReadLevelInfoFromForm(bool allowWriteBack)
        {
            CurLevel.Author = txt_LevelAuthor.Text;
            CurLevel.Title = txt_LevelTitle.Text;
            CurLevel.MusicFile = System.IO.Path.ChangeExtension(combo_Music.Text, null);
            CurLevel.MainStyle = ValidateStyleName(combo_MainStyle.Text);
            CurLevel.Width = decimal.ToInt32(num_Lvl_SizeX.Value);
            CurLevel.Height = decimal.ToInt32(num_Lvl_SizeY.Value);
            CurLevel.AutoStartPos = chk_Lvl_AutoStart.Checked;
            CurLevel.StartPosX = decimal.ToInt32(num_Lvl_StartX.Value);
            CurLevel.StartPosY = decimal.ToInt32(num_Lvl_StartY.Value);
            CurLevel.NumLems = decimal.ToInt32(num_Lvl_Lems.Value);
            CurLevel.SaveReq = decimal.ToInt32(num_Lvl_Rescue.Value);
            CurLevel.SpawnRate = decimal.ToInt32(num_Lvl_SR.Value);
            CurLevel.IsSpawnRateFix = check_Lvl_LockSR.Checked;
            CurLevel.IsSuperlemming = check_Lvl_Superlemming.Checked;
            CurLevel.TimeLimit = decimal.ToInt32(num_Lvl_TimeMin.Value) * 60
                                    + decimal.ToInt32(num_Lvl_TimeSec.Value);
            CurLevel.IsNoTimeLimit = check_Lvl_InfTime.Checked;
            CurLevel.IsInvincibility = check_Lvl_Invincibility.Checked;
           

            string idText = txt_LevelID.Text;
            if (idText.Length < 16)
                idText = idText.PadLeft(16);
            if (idText.Length > 16)
                idText = idText.Substring(16);

            if (ulong.TryParse(idText, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out ulong newID))
            {
                if (newID != 0)
                    CurLevel.LevelID = newID;
            }

            if (allowWriteBack && txt_LevelID.Text != CurLevel.LevelID.ToString("X16"))
                txt_LevelID.Text = CurLevel.LevelID.ToString("X16");

            foreach (C.Skill skill in numericsSkillSet.Keys)
            {
                CurLevel.SkillSet[skill] = decimal.ToInt32(numericsSkillSet[skill].Value);
            }
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
                txt_LevelAuthor.Text = CurLevel.Author;
                txt_LevelTitle.Text = CurLevel.Title;
                combo_Music.Text = CurLevel.MusicFile;
                combo_MainStyle.Text = (CurLevel.MainStyle != null) ? CurLevel.MainStyle.NameInEditor : "";

                // Set size and start position, but without calling the Value_Changed methods,
                // because they automatically call validation of the start position resp. render the level again.
                num_Lvl_SizeX.ValueChanged -= num_Lvl_SizeX_ValueChanged;
                num_Lvl_SizeY.ValueChanged -= num_Lvl_SizeY_ValueChanged;
                num_Lvl_StartX.ValueChanged -= num_Lvl_StartX_ValueChanged;
                num_Lvl_StartY.ValueChanged -= num_Lvl_StartY_ValueChanged;

                num_Lvl_SizeX.Value = CurLevel.Width;
                num_Lvl_SizeY.Value = CurLevel.Height;
                num_Lvl_StartX.Maximum = CurLevel.Width - 1;
                num_Lvl_StartY.Maximum = CurLevel.Height - 1;
                num_Lvl_StartX.Value = CurLevel.StartPosX;
                num_Lvl_StartY.Value = CurLevel.StartPosY;
                chk_Lvl_AutoStart.Checked = CurLevel.AutoStartPos;

                num_Lvl_SizeX.ValueChanged += num_Lvl_SizeX_ValueChanged;
                num_Lvl_SizeY.ValueChanged += num_Lvl_SizeY_ValueChanged;
                num_Lvl_StartX.ValueChanged += num_Lvl_StartX_ValueChanged;
                num_Lvl_StartY.ValueChanged += num_Lvl_StartY_ValueChanged;

                // Add the rest of the values
                num_Lvl_Lems.Value = CurLevel.NumLems;
                num_Lvl_Rescue.Value = CurLevel.SaveReq;
                num_Lvl_SR.Value = CurLevel.SpawnRate;
                check_Lvl_LockSR.Checked = CurLevel.IsSpawnRateFix;
                num_Lvl_TimeMin.Value = CurLevel.TimeLimit / 60;
                num_Lvl_TimeSec.Value = CurLevel.TimeLimit % 60;
                check_Lvl_InfTime.Checked = CurLevel.IsNoTimeLimit;
                check_Lvl_Superlemming.Checked = CurLevel.IsSuperlemming;
                check_Lvl_Invincibility.Checked = CurLevel.IsInvincibility;

                txt_LevelID.Text = CurLevel.LevelID.ToString("X16");

                foreach (C.Skill skill in numericsSkillSet.Keys)
                {
                    numericsSkillSet[skill].Value = CurLevel.SkillSet[skill];
                }

                lbl_Global_Version.Text = "Version: " + CurLevel.LevelVersion.ToString("X16");

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
        private void CreateNewLevelAndRenderer()
        {
            if (AskUserWhetherSaveLevel())
                return;

            Style mainStyle = StyleList?.Find(sty => sty.NameInEditor == combo_MainStyle.Text);
            CurLevel = new Level(mainStyle);
            // Get new renderer with the standard display options
            if (curRenderer != null)
                curRenderer.Dispose();
            curRenderer = new Renderer(CurLevel, pic_Level, curSettings);

            oldLevelList = new List<Level>();
            oldLevelList.Add(CurLevel.Clone());
            curOldLevelIndex = 0;
            lastSavedLevel = CurLevel.Clone();

            WriteLevelInfoToForm();
            UpdateBackgroundImage();
            UpdateFlagsForPieceActions();
            RepositionPicLevel();
            pic_Level.Image = curRenderer.CreateLevelImage();

            UpdateSpecialLemmingCounter();
        }

        /// <summary>
        /// Displays a file browser (if path not specified) and loads the selected level
        /// </summary>
        private void LoadNewLevel(string filename = null)
        {
            if (AskUserWhetherSaveLevel())
                return;

            Level level;

            if (filename == null)
                level = LevelFile.LoadLevel(StyleList, Backgrounds, levelDirectory);
            else
                level = LevelFile.LoadLevelFromFile(filename, StyleList, Backgrounds);

            if (level == null)
                return;

            levelDirectory = System.IO.Path.GetDirectoryName(level.FilePathToSave);
            
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
            pic_Level.Image = curRenderer.CreateLevelImage();

            combo_PieceStyle.Text = CurLevel.MainStyle?.NameInEditor;

            UpdateSpecialLemmingCounter();
        }

        // Store the names of the missing pieces for the current level
        private HashSet<string> missingPieces = new HashSet<string>();

        /// <summary>
        /// Checks for & removes all pieces for which no image in the corresponding style exists.
        /// <para> A warning is displayed if pieces are removed. </para>
        /// </summary>
        private void ValidateLevelPieces()
        {
            if (CurLevel == null)
                return;

            // Initialise missingPieces list
            missingPieces.Clear();

            // Initialise status strip
            statusBar.Visible = false;

            CurLevel.TerrainList.FindAll(ter => !ter.ExistsImage())
                              .ForEach(ter => missingPieces.Add(ter.Name + " in style " + ter.Style));
            CurLevel.GadgetList.FindAll(gad => !gad.ExistsImage())
                             .ForEach(gad => missingPieces.Add(gad.Name + " in style " + gad.Style));

            if (missingPieces.Count > 0)
            {
                // Append "_MissingPieces" to the file name
                string originalFilePath = CurLevel.FilePathToSave;
                string directory = Path.GetDirectoryName(originalFilePath);
                string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(originalFilePath);
                string fileExtension = Path.GetExtension(originalFilePath);
                string newFileName = $"{fileNameWithoutExtension}_MissingPieces{fileExtension}";

                // Check for unique file name
                int count = 1;
                string newFilePath = Path.Combine(directory, newFileName);
                while (File.Exists(newFilePath))
                {
                    newFileName = $"{fileNameWithoutExtension}_MissingPieces ({count}){fileExtension}";
                    newFilePath = Path.Combine(directory, newFileName);
                    count++;
                }

                // Update the file path
                CurLevel.FilePathToSave = newFilePath;

                // Delete missing pieces
                CurLevel.TerrainList.RemoveAll(ter => !ter.ExistsImage());
                CurLevel.GadgetList.RemoveAll(gad => !gad.ExistsImage());

                // Update status strip
                statusBar.Visible = true;
                statusBarLabel1.Text = "This level contains missing pieces (click to show).";
                statusBarLabel2.Text = "If saved, a new copy called " + newFileName +
                                             " will be created to prevent overwriting the original.";

                // Store the filename of the level with missing pieces
                levelsWithMissingPieces.Add(originalFilePath);
            }

            // Return true if no missing images were found
            return;
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

        /// <summary>
        /// Checks for presence of Neo/SuperLemmix.exe in Editor's base folder
        /// </summary>
        private void DetectLemmixVersions()
        {
            string baseFolder = AppDomain.CurrentDomain.BaseDirectory;

            bool isNeoLemmixDetected = File.Exists(Path.Combine(baseFolder, "NeoLemmix.exe"));
            bool isSuperLemmixDetected = File.Exists(Path.Combine(baseFolder, "SuperLemmix.exe"));

            var curMode = curSettings.CurrentEditorMode;

            isNeoLemmixOnly =
               ((curMode == Settings.EditorMode.Auto && isNeoLemmixDetected && !isSuperLemmixDetected)
              || curMode == Settings.EditorMode.NeoLemmix)
              && curMode != Settings.EditorMode.SuperLemmix;
        }

        /// <summary>
        /// Sets non-updating controls according to Lemmix version/user preferences
        /// </summary>
        private void SetLemmixVersionFeatures()
        {
            if (isNeoLemmixOnly)
            {
                lbl_Skill_Stoner.Enabled = true;
                lbl_Skill_Stoner.Visible = true;
                lbl_Skill_Freezer.Enabled = false;
                lbl_Skill_Freezer.Visible = false;

                num_Ski_Stoner.Visible = true;
                num_Ski_Stoner.Enabled = true;
                num_Ski_Freezer.Enabled = false;
                num_Ski_Freezer.Visible = false;

                check_Piece_Stoner.Visible = true;
                check_Piece_Freezer.Enabled = false;
                check_Piece_Freezer.Visible = false;

                lbl_Skill_Ballooner.Enabled = false;
                lbl_Skill_Timebomber.Enabled = false;
                lbl_Skill_Ladderer.Enabled = false;
                lbl_Skill_Spearer.Enabled = false;
                lbl_Skill_Grenader.Enabled = false;

                num_Ski_Ballooner.Enabled = false;
                num_Ski_Timebomber.Enabled = false;
                num_Ski_Ladderer.Enabled = false;
                num_Ski_Spearer.Enabled = false;
                num_Ski_Grenader.Enabled = false;

                check_Lvl_Superlemming.Enabled = false;
            }
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

            LevelFile.SaveLevel(CurLevel, levelDirectory);
            SaveChangesToOldLevelList();
            levelDirectory = System.IO.Path.GetDirectoryName(CurLevel.FilePathToSave);
            if (!isPlaytest)
                lastSavedLevel = CurLevel.Clone();

            WriteLevelInfoToForm();
        }

        /// <summary>
        /// Saves the current level in the current location. If no location is known, the file browser is opened.
        /// </summary>
        private void SaveLevel(bool isPlaytest = false)
        {
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

        private void ShowCleanseLevelsDialog()
        {
            using (var folderBrowserDialog = new FolderBrowserDialog())
            {
                folderBrowserDialog.Description = "Choose a folder of levels to cleanse";

                DialogResult result = folderBrowserDialog.ShowDialog();

                if (result == DialogResult.OK && !string.IsNullOrWhiteSpace(folderBrowserDialog.SelectedPath))
                {
                    targetFolder = folderBrowserDialog.SelectedPath;

                    var confirmResult = MessageBox.Show(
                        $"Are you sure you want to cleanse all levels in \"{targetFolder}\"?",
                        "Confirm Cleansing",
                        MessageBoxButtons.YesNo,
                        MessageBoxIcon.Warning);

                    if (confirmResult == DialogResult.Yes)
                    {
                        CleanseLevels();
                    }
                }
            }
        }

        // Store filenames of levels with missing pieces
        List<string> levelsWithMissingPieces = new List<string>();

        /// <summary>
        /// Opens and saves all .nxlv files in a directory in order to ensure compatibility and update the file
        /// </summary>
        private async void CleanseLevels()
        {
            if (string.IsNullOrEmpty(targetFolder))
            {
                MessageBox.Show("Please select a target folder.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Initialise list
            levelsWithMissingPieces.Clear();

            // Get all .nxlv files in the target folder and its subdirectories
            string[] files = Directory.GetFiles(targetFolder, "*.nxlv", SearchOption.AllDirectories);

            // Show progress bar
            using (FormProgress progressForm = new FormProgress())
            {
                progressForm.ProgressBar.Maximum = files.Length;
                progressForm.Show();

                foreach (string file in files)
                {
                    LoadNewLevel(file);
                    SaveLevel(false);

                    // Update the progress bar
                    int progressPercentage = (Array.IndexOf(files, file) + 1) * 100 / files.Length;
                    progressForm.UpdateProgress(progressPercentage, $"Processing file {Array.IndexOf(files, file) + 1} of {files.Length}: {Path.GetFileName(file)}");

                    // Give a short delay to allow status to update
                    await Task.Delay(10);
                }

                progressForm.Close();

                // Re-initialize the Editor
                CreateNewLevelAndRenderer();
                statusBar.Visible = false;

                // Display completion message
                string completionMessage = "All .nxlv files have been cleansed successfully.";
                
                if (levelsWithMissingPieces.Count > 0)
                {
                    completionMessage += "\n\nLevels with missing pieces:\n\n";
                    completionMessage += string.Join("\n", levelsWithMissingPieces.Select(Path.GetFileName));
                    completionMessage += "\n\nThese levels have been saved with '_MissingPieces' appended to the filename.";

                }
                MessageBox.Show(completionMessage, "Cleanse Levels Complete", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        /// <summary>
        /// Saves the level as TempTestLevel.nxlv and loads this level in the SuperLemmix player.
        /// </summary>
        private void PlaytestLevel()
        {
            ReadLevelInfoFromForm(true);
            SaveChangesToOldLevelList();

            // Save the level as TempTestLevel.nxlv.
            string origFilePath = CurLevel.FilePathToSave;
            CurLevel.FilePathToSave = C.AppPathTempLevel;
            SaveLevel(true);
            CurLevel.FilePathToSave = origFilePath;

            if (!System.IO.File.Exists(C.AppPathSuperLemmix))
            {
                MessageBox.Show("Error: Player SuperLemmix.exe not found in editor directory.", "File not found");
            }
            else
            {
                try
                {
                    // Start the SuperLemmix player.
                    var playerStartInfo = new System.Diagnostics.ProcessStartInfo();
                    playerStartInfo.FileName = C.AppPathSuperLemmix;
                    playerStartInfo.Arguments = "test " + "\"" + C.AppPathTempLevel + "\"";

                    System.Diagnostics.Process.Start(playerStartInfo);
                }
                catch (Exception Ex)
                {
                    Utility.LogException(Ex);
                    MessageBox.Show("Error: Starting SuperLemmix.exe failed or was aborted.", "Application start failed");
                }
            }
        }

        /// <summary>
        /// Creates a new LevelValidator, runs the validation and displays the result in a new form.
        /// </summary>
        private void ValidateLevel()
        {
            ReadLevelInfoFromForm(true);
            var validator = new LevelValidator(CurLevel);
            validator.Validate();
        }


        /// <summary>
        /// Returns a style with the requested name, or null if none such is found. 
        /// </summary>
        /// <param name="styleName"></param>
        /// <returns></returns>
        private Style ValidateStyleName(string styleName)
        {
            return StyleList?.Find(sty => sty.NameInEditor == styleName);
        }


        /// <summary>
        /// Switches between displaying objects and terrain for newly added pieces.
        /// </summary>
        private void ChangeObjTerrPieceDisplay(C.SelectPieceType newKind)
        {
            if (newKind != pieceDoDisplayKind)
            {
                pieceDoDisplayKind = newKind;

                but_PieceTerr.Font = new Font(but_PieceTerr.Font, FontStyle.Regular);
                but_PieceObj.Font = new Font(but_PieceObj.Font, FontStyle.Regular);
                but_PieceBackground.Font = new Font(but_PieceBackground.Font, FontStyle.Regular);
                but_PieceSketches.Font = new Font(but_PieceSketches.Font, FontStyle.Regular);

                switch (newKind)
                {
                    case C.SelectPieceType.Terrain:
                        but_PieceTerr.Font = new Font(but_PieceTerr.Font, FontStyle.Bold);
                        break;
                    case C.SelectPieceType.Objects:
                        but_PieceObj.Font = new Font(but_PieceObj.Font, FontStyle.Bold);
                        break;
                    case C.SelectPieceType.Backgrounds:
                        but_PieceBackground.Font = new Font(but_PieceBackground.Font, FontStyle.Bold);
                        break;
                    case C.SelectPieceType.Sketches:
                        but_PieceSketches.Font = new Font(but_PieceSketches.Font, FontStyle.Bold);
                        break;
                }

                pieceStartIndex = 0;
                LoadPiecesIntoPictureBox();
            }
        }

        /// <summary>
        /// Moves the screen start position to the given level coordinates.
        /// </summary>
        /// <param name="newCenter"></param>
        private void MoveScreenStartPosition(Point newCenter)
        {
            // Ensure that the new center position is within the correct bounds.
            int newCenterX = newCenter.X.Restrict(0, (int)num_Lvl_StartX.Maximum);
            int newCenterY = newCenter.Y.Restrict(0, (int)num_Lvl_StartY.Maximum);

            // Remove these events to combine layers only once.
            num_Lvl_StartX.ValueChanged -= num_Lvl_StartX_ValueChanged;
            num_Lvl_StartY.ValueChanged -= num_Lvl_StartY_ValueChanged;

            num_Lvl_StartX.Value = newCenterX;
            num_Lvl_StartY.Value = newCenterY;
            CurLevel.StartPosX = newCenterX;
            CurLevel.StartPosY = newCenterY;

            num_Lvl_StartX.ValueChanged += num_Lvl_StartX_ValueChanged;
            num_Lvl_StartY.ValueChanged += num_Lvl_StartY_ValueChanged;

            // Save the changes and combine the layers now.
            pic_Level.Image = curRenderer.CombineLayers();
            SaveChangesToOldLevelList();
        }

        /// <summary>
        /// Moves the current screen start position by 8 pixels in the given direction.
        /// </summary>
        /// <param name="direction"></param>
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
        /// <param name="movement"></param>
        private void MoveTerrPieceSelection(int movement)
        {       
            List<string> pieceNameList;

            switch (pieceDoDisplayKind)
            {
                case C.SelectPieceType.Objects:
                    pieceNameList = pieceCurStyle?.ObjectKeys;
                    break;
                case C.SelectPieceType.Terrain:
                    pieceNameList = pieceCurStyle?.TerrainKeys;
                    break;
                case C.SelectPieceType.Backgrounds:
                    pieceNameList = pieceCurStyle?.BackgroundKeys;
                    break;
                case C.SelectPieceType.Sketches:
                    pieceNameList = Style.SketchKeys;
                    break;
                default:
                    throw new ArgumentException();
            }

            if (pieceNameList == null || pieceNameList.Count == 0)
                return;

            // Pass to correct piece index
            pieceStartIndex = (pieceStartIndex + movement) % pieceNameList.Count;
            // ensure that PieceStartIndex is positive
            pieceStartIndex = (pieceStartIndex + pieceNameList.Count) % pieceNameList.Count;

            LoadPiecesIntoPictureBox();
        }

        /// <summary>
        /// Changes the style for newly added pieces and displays the new pieces.
        /// </summary>
        /// <param name="movement"></param>
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

            this.combo_PieceStyle.SelectedIndex = newStyleIndex;
        }

        private void CyclePieceBrowser()
        {
            C.SelectPieceType newKind;
            switch (pieceDoDisplayKind)
            {
                case C.SelectPieceType.Terrain:
                    newKind = C.SelectPieceType.Objects;
                    break;
                case C.SelectPieceType.Objects:
                    newKind = C.SelectPieceType.Backgrounds;
                    break;
                case C.SelectPieceType.Backgrounds:
                    newKind = C.SelectPieceType.Sketches;
                    break;
                case C.SelectPieceType.Sketches:
                    newKind = C.SelectPieceType.Terrain;
                    break;
                default:
                    throw new ArgumentException();
            }

            ChangeObjTerrPieceDisplay(newKind);
        }

        /// <summary>
        /// Gets the key from the index of the clicked PieceBox.
        /// </summary>
        /// <param name="picPieceIndex"></param>
        /// <returns></returns>
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
                case C.SelectPieceType.Backgrounds:
                    pieceList = pieceCurStyle?.BackgroundKeys;
                    break;
                case C.SelectPieceType.Sketches:
                    pieceList = Style.SketchKeys;
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
            if (picPieceList.Count >= hotkeyIndex)
            {
                AddNewPieceToLevel(hotkeyIndex);
                UpdateFlagsForPieceActions();
            }
        }

        /// <summary>
        /// Adds a new piece to the level and displays the result to the user.
        /// </summary>
        /// <param name="picPieceIndex"></param>
        private void AddNewPieceToLevel(int picPieceIndex)
        {
            ReadLevelInfoFromForm(true);
            SaveChangesToOldLevelList();

            string pieceKey = GetPieceKeyFromIndex(picPieceIndex);

            if (pieceKey != "")
                switch (pieceDoDisplayKind)
                {
                    case C.SelectPieceType.Terrain:
                    case C.SelectPieceType.Objects:
                    case C.SelectPieceType.Sketches:
                        AddNewPieceToLevel(pieceKey, curRenderer.GetCenterPoint());
                        break;
                    case C.SelectPieceType.Backgrounds:
                        string[] splitKey = pieceKey.Split('/', '\\');
                        CurLevel.Background = new Background(pieceCurStyle, splitKey[2]);
                        UpdateBackgroundImage();
                        pic_Level.SetImage(curRenderer.CombineLayers());
                        break;
                }
        }

        /// <summary>
        /// Adds a new piece to the level and displays the result to the user.
        /// </summary>
        /// <param name="pieceKey"></param>
        /// <param name="centerPosition"></param>
        public void AddNewPieceToLevel(string pieceKey, Point centerPosition)
        {
            CurLevel.UnselectAll();
            CurLevel.AddPiece(pieceKey, centerPosition, gridSize);

            SaveChangesToOldLevelList();
            pic_Level.Image = curRenderer.CreateLevelImage();
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

            if (mouseButtonPressed == MouseButtons.Left)
            {
                if (isCtrlPressed || isShiftPressed)
                {
                    // Add or remove a single piece, depending on whether there is a selected piece at the mouse position 
                    bool doAdd = !CurLevel.HasSelectionAtPos(levelPos);
                    CurLevel.SelectOnePiece(levelPos, doAdd, isAltPressed);
                }
                else
                {
                    // Select only the one that is below the mouse cursor
                    CurLevel.UnselectAll();
                    CurLevel.SelectOnePiece(levelPos, true, isAltPressed);
                }
            }
            else if (mouseButtonPressed == MouseButtons.Middle)
            {
                // Remove all pieces below the mouse point.
                var selectArea = new Rectangle(levelPos.X, levelPos.Y, 1, 1);
                CurLevel.SelectAreaPiece(selectArea, false);
            }
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
                // Delete all existing selections
                if (!isCtrlPressed)
                {
                    CurLevel.UnselectAll();
                }

                // Add all pieces intersection SelectArea
                CurLevel.SelectAreaPiece((Rectangle)selectArea, true);
            }
            else if (mouseButtonPressed == MouseButtons.Middle)
            {
                // Remove all pieces intersection SelectArea
                CurLevel.SelectAreaPiece((Rectangle)selectArea, false);
            }
        }

        /// <summary>
        /// Moves all selected pieces of the level and displays the result.
        /// </summary>
        /// <param name="direction"></param>
        /// <param name="step"></param>
        private void MoveLevelPiecesOrScreenPos(C.DIR direction, int step = 1)
        {
            movementActionPerformed = true;

            if (CurLevel.SelectionList().Count > 0)
            {
                CurLevel.MovePieces(direction, step, gridSize);
                pic_Level.Image = curRenderer.CreateLevelImage();
            }
            else
            {
                curRenderer.MoveScreenPos(direction, step * 8);
                pic_Level.SetImage(curRenderer.GetScreenImage());
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
            pic_Level.Image = curRenderer.CreateLevelImage();
        }

        /// <summary>
        /// Inverts all selected pieces in the level and displays the result.
        /// </summary>
        private void InvertLevelPieces()
        {
            CurLevel.InvertPieces();
            SaveChangesToOldLevelList();
            pic_Level.Image = curRenderer.CreateLevelImage();
        }

        /// <summary>
        /// Flips all selected pieces in the level and displays the result.
        /// </summary>
        private void FlipLevelPieces()
        {
            CurLevel.FlipPieces();
            SaveChangesToOldLevelList();
            pic_Level.Image = curRenderer.CreateLevelImage();
        }

        /// <summary>
        /// Sets the NoOverwrite flag for all selected pieces and displays the result.
        /// </summary>
        /// <param name="doAdd"></param>
        private void SetNoOverwrite(bool doAdd)
        {
            CurLevel.SetNoOverwrite(doAdd);
            UpdateFlagsForPieceActions();
            SaveChangesToOldLevelList();
            pic_Level.Image = curRenderer.CreateLevelImage();
        }

        /// <summary>
        /// Sets the Erase flag for all selected pieces and displays the result.
        /// </summary>
        /// <param name="doAdd"></param>
        private void SetErase(bool doAdd)
        {
            CurLevel.SetErase(doAdd);
            UpdateFlagsForPieceActions();
            SaveChangesToOldLevelList();
            pic_Level.Image = curRenderer.CreateLevelImage();
        }

        /// <summary>
        /// Sets the OnlyOnTerrain flag for all selected pieces and displays the result.
        /// </summary>
        /// <param name="doAdd"></param>
        private void SetOnlyOnTerrain(bool doAdd)
        {
            CurLevel.SetOnlyOnTerrain(doAdd);
            UpdateFlagsForPieceActions();
            SaveChangesToOldLevelList();
            pic_Level.Image = curRenderer.CreateLevelImage();
        }

        /// <summary>
        /// Sets the OneWayAdmissible flag for all selected pieces and displays the result.
        /// </summary>
        /// <param name="doAdd"></param>
        private void SetOneWay(bool doAdd)
        {
            CurLevel.SetOneWay(doAdd);
            UpdateFlagsForPieceActions();
            SaveChangesToOldLevelList();
            pic_Level.Image = curRenderer.CreateLevelImage();
        }

        /// <summary>
        /// Sets skill flags for all selected objects.
        /// </summary>
        /// <param name="skill"></param>
        /// <param name="doAdd"></param>
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
        /// <param name="toFront"></param>
        /// <param name="onlyOneStep"></param>
        private void MovePieceIndex(bool toFront, bool onlyOneStep)
        {
            CurLevel.MoveSelectedPieces(toFront, onlyOneStep);
            pic_Level.Image = curRenderer.CreateLevelImage();

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

            lbl_Global_LemmingTypes.Text = newText;
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
            pic_Level.Image = curRenderer.CreateLevelImage();
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
        private void DuplicateSelectedPieces()
        {
            if (CurLevel.SelectionList().Count == 0)
                return;
            var selection = CurLevel.SelectionList();
            CurLevel.UnselectAll();
            CurLevel.AddMultiplePieces(selection);
            SaveChangesToOldLevelList();
            pic_Level.Image = curRenderer.CreateLevelImage();
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
            pic_Level.Image = curRenderer.CreateLevelImage();
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
            pic_Level.Image = curRenderer.CreateLevelImage();
        }

        /// <summary>
        /// Centers the collection of pieces around the cursor.
        /// </summary>
        /// <param name="clipPieces"></param>
        /// <returns></returns>
        private IEnumerable<LevelPiece> CenterPiecesAtCursor(IEnumerable<LevelPiece> clipPieces)
        {
            Point mousePos = curRenderer.GetMousePosInLevel(pic_Level.PointToClient(Cursor.Position));
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
                CurLevel.GroupSelection();
                SaveChangesToOldLevelList();
                UpdateFlagsForPieceActions();
                pic_Level.Image = curRenderer.CreateLevelImage();
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
                pic_Level.Image = curRenderer.CreateLevelImage();
            }
        }

        /// <summary>
        /// Switches between Grid usage and no such
        /// </summary>
        private void SwitchGridUsage()
        {
            curSettings.SwitchGridUsage();
            snapToGridToolStripMenuItem.Checked = curSettings.UseGridForPieces;
            pic_Level.SetImage(curRenderer.GetScreenImage());
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
                LevelFile.SaveLevelToFile(C.AppPathAutosave + filename + ".nxlv", tempLevel);

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
                string[] files = Directory.GetFiles(C.AppPathAutosave, "*.nxlv");
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

        private void ShowAboutSLXEditor()
        {
            using (var aboutSLXEditor = new FormAboutSLXEditor())
            {
                aboutSLXEditor.ShowDialog(this);
            }
        }

        private void SetAllSkillsToZero()
        {
            foreach (Control ctrl in tabSkills.Controls)
            {
                if (ctrl is NumericUpDown numBox && numBox != num_RandomMinLimit
                                                 && numBox != num_RandomMaxLimit)
                {
                    numBox.Value = 0;
                }
            }
        }

        private void GenerateRandomSkillset()
        {
            SetAllSkillsToZero(); // Zero the skillset first
            Random random = new Random();

            int minValue = (int)num_RandomMinLimit.Value;
            int maxValue = (int)num_RandomMaxLimit.Value;

            // List and shuffle the numeric controls on tabSkills (excluding the randomizer limits and disabled controls)
            List<NumericUpDown> numericUpDowns = tabSkills.Controls.OfType<NumericUpDown>()
                .Where(n => n != num_RandomMinLimit && n != num_RandomMaxLimit && n.Enabled)
                .ToList();
            numericUpDowns = numericUpDowns.OrderBy(x => random.Next()).ToList();

            // Select up to 14 skills and populate them with a number between minValue and maxValue
            List<NumericUpDown> selectedControls = numericUpDowns.Take(14).ToList();
            foreach (var numBox in selectedControls)
            {
                numBox.Value = random.Next(minValue, maxValue + 1); // maxValue + 1 because Random.Next is exclusive on the upper bound
            }
        }

        private void SaveLevelAsImage()
        {
            // Handle the file naming format
            string baseFileName = string.IsNullOrEmpty(CurLevel.Title) ? "Level" : CurLevel.Title;
            string fileName = baseFileName + ".png";

            int count = 0;
            while (File.Exists(fileName))
            {
                count++;
                fileName = $"{baseFileName} ({count}).png";
            }

            // Get the full level image and save it to a .png file
            Bitmap fullLevelImage = curRenderer.GetFullLevelImage();
            fullLevelImage.Save(fileName, System.Drawing.Imaging.ImageFormat.Png);

            // Confirm save with a popup message
            string savedFilePath = Path.GetFullPath(fileName);
            MessageBox.Show($"Image saved as {savedFilePath}", "Save Successful", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void ToggleClearPhysics()
        {
            DisplaySettings.ChangeDisplayed(C.DisplayType.ClearPhysics);
            pic_Level.SetImage(curRenderer.CreateLevelImage());
        }

        private void ToggleTerrain()
        {
            DisplaySettings.ChangeDisplayed(C.DisplayType.Terrain);
            pic_Level.SetImage(curRenderer.CombineLayers());
        }

        private void ToggleObjects()
        {
            DisplaySettings.ChangeDisplayed(C.DisplayType.Objects);
            pic_Level.SetImage(curRenderer.CombineLayers());
        }

        private void ToggleTriggerAreas()
        {
            DisplaySettings.ChangeDisplayed(C.DisplayType.Trigger);
            pic_Level.SetImage(curRenderer.CombineLayers());
        }

        private void ToggleScreenStart()
        {
            DisplaySettings.ChangeDisplayed(C.DisplayType.ScreenStart);
            pic_Level.SetImage(curRenderer.CombineLayers());
        }

        private void ToggleBackground()
        {
            DisplaySettings.ChangeDisplayed(C.DisplayType.Background);
            pic_Level.SetImage(curRenderer.CombineLayers());
        }

        private void ToggleDeprecatedPieces()
        {
            DisplaySettings.ChangeDisplayed(C.DisplayType.Deprecated);
            LoadPiecesIntoPictureBox();
        }

        private void ZoomIn()
        {
            curRenderer.ChangeZoom(1, false);
            RepositionPicLevel();
            pic_Level.SetImage(curRenderer.GetScreenImage());
        }

        private void ZoomOut()
        {
            curRenderer.ChangeZoom(-1, false);
            RepositionPicLevel();
            pic_Level.SetImage(curRenderer.GetScreenImage());
        }

        private void SetHotkeys()
        {
            // Load hotkeys
            if (File.Exists("SLXEditorHotkeys.ini"))
                HotkeyConfig.LoadHotkeysFromIniFile();
            else
                HotkeyConfig.GetDefaultHotkeys();

            InitializeHotkeyActions();
            UpdateMenuShortcutKeyDisplayStrings();
        }

        private void InitializeHotkeyActions()
        {
            hotkeyActions = new Dictionary<Keys, Action>();

            void AddHotkey(Keys key, Action action)
            {
                if (key != Keys.None) // Skip if the key is "None"
                {
                    hotkeyActions.Add(key, action);
                }
            }

            AddHotkey(HotkeyConfig.HotkeyCreateNewLevel, () => CreateNewLevelAndRenderer());
            AddHotkey(HotkeyConfig.HotkeyLoadLevel, () => LoadNewLevel());
            AddHotkey(HotkeyConfig.HotkeySaveLevel, () => SaveLevel());
            AddHotkey(HotkeyConfig.HotkeySaveLevelAs, () => SaveLevelAsNewFile());
            AddHotkey(HotkeyConfig.HotkeySaveLevelAsImage, () => SaveLevelAsImage());
            AddHotkey(HotkeyConfig.HotkeyPlaytestLevel, () => PlaytestLevel());
            AddHotkey(HotkeyConfig.HotkeyValidateLevel, () => ValidateLevel());
            AddHotkey(HotkeyConfig.HotkeyCleanseLevels, () => ShowCleanseLevelsDialog());
            AddHotkey(HotkeyConfig.HotkeyToggleClearPhysics, () => ToggleClearPhysics());
            AddHotkey(HotkeyConfig.HotkeyToggleTerrain, () => ToggleTerrain());
            AddHotkey(HotkeyConfig.HotkeyToggleObjects, () => ToggleObjects());
            AddHotkey(HotkeyConfig.HotkeyToggleTriggerAreas, () => ToggleTriggerAreas());
            AddHotkey(HotkeyConfig.HotkeyToggleScreenStart, () => ToggleScreenStart());
            AddHotkey(HotkeyConfig.HotkeyToggleBackground, () => ToggleBackground());
            AddHotkey(HotkeyConfig.HotkeyToggleDeprecatedPieces, () => ToggleDeprecatedPieces());
            AddHotkey(HotkeyConfig.HotkeyShowMissingPieces, () => ShowMissingPiecesDialog());
            AddHotkey(HotkeyConfig.HotkeyToggleSnapToGrid, () => SwitchGridUsage());
            AddHotkey(HotkeyConfig.HotkeyOpenSettings, () => settingsToolStripMenuItem_Click(null, null));
            AddHotkey(HotkeyConfig.HotkeyOpenConfigHotkeys, () => hotkeysToolStripMenuItem_Click(null, null));
            AddHotkey(HotkeyConfig.HotkeyOpenAboutSLX, () => ShowAboutSLXEditor());

            //AddHotkey(HotkeyConfig.HotkeySelectPieces, () => "mouse mandatory" ());
            //AddHotkey(HotkeyConfig.HotkeyDragToScroll, () => "mouse mandatory" ());

            //AddHotkey(HotkeyConfig.HotkeyRemovePiecesAtCursor, () => "only requires cursor position, hotkey can be anything" ());
            //AddHotkey(HotkeyConfig.HotkeyAddRemoveSinglePiece, () => "only requires cursor position, hotkey can be anything" ());
            //AddHotkey(HotkeyConfig.HotkeySelectPiecesBelow, () => "only requires cursor position, hotkey can be anything" ());

            AddHotkey(HotkeyConfig.HotkeyZoomIn, () => ZoomIn());
            AddHotkey(HotkeyConfig.HotkeyZoomOut, () => ZoomOut());
            AddHotkey(HotkeyConfig.HotkeyScrollHorizontally, () => scrollHorizontallyPressed = true);
            AddHotkey(HotkeyConfig.HotkeyScrollVertically, () => scrollVerticallyPressed = true);

            //AddHotkey(HotkeyConfig.HotkeyDragScreenStart, () => "mouse mandatory" ());

            //AddHotkey(HotkeyConfig.HotkeyNudgeScreenStartUp, () => "currently uses direction arrows, but doesn't necessarily need to" ());
            //AddHotkey(HotkeyConfig.HotkeyNudgeScreenStartDown, () => "currently uses direction arrows, but doesn't necessarily need to" ());
            //AddHotkey(HotkeyConfig.HotkeyNudgeScreenStartLeft, () => "currently uses direction arrows, but doesn't necessarily need to" ());
            //AddHotkey(HotkeyConfig.HotkeyNudgeScreenStartRight, () => "currently uses direction arrows, but doesn't necessarily need to" ());

            AddHotkey(HotkeyConfig.HotkeyShowPreviousPiece, () => MoveTerrPieceSelection(-1));
            AddHotkey(HotkeyConfig.HotkeyShowNextPiece, () => MoveTerrPieceSelection(1));
            AddHotkey(HotkeyConfig.HotkeyShowPreviousGroup, () => MoveTerrPieceSelection(-13));
            AddHotkey(HotkeyConfig.HotkeyShowNextGroup, () => MoveTerrPieceSelection(13));
            AddHotkey(HotkeyConfig.HotkeyShowPreviousStyle, () => ChangeNewPieceStyleSelection(-1));
            AddHotkey(HotkeyConfig.HotkeyShowNextStyle, () => ChangeNewPieceStyleSelection(1));
            AddHotkey(HotkeyConfig.HotkeyCycleBrowser, () => CyclePieceBrowser());
            AddHotkey(HotkeyConfig.HotkeyAddPiece1, () => AddPieceViaHotkey(1));
            AddHotkey(HotkeyConfig.HotkeyAddPiece2, () => AddPieceViaHotkey(2));
            AddHotkey(HotkeyConfig.HotkeyAddPiece3, () => AddPieceViaHotkey(3));
            AddHotkey(HotkeyConfig.HotkeyAddPiece4, () => AddPieceViaHotkey(4));
            AddHotkey(HotkeyConfig.HotkeyAddPiece5, () => AddPieceViaHotkey(5));
            AddHotkey(HotkeyConfig.HotkeyAddPiece6, () => AddPieceViaHotkey(6));
            AddHotkey(HotkeyConfig.HotkeyAddPiece7, () => AddPieceViaHotkey(7));
            AddHotkey(HotkeyConfig.HotkeyAddPiece8, () => AddPieceViaHotkey(8));
            AddHotkey(HotkeyConfig.HotkeyAddPiece9, () => AddPieceViaHotkey(9));
            AddHotkey(HotkeyConfig.HotkeyAddPiece10, () => AddPieceViaHotkey(10));
            AddHotkey(HotkeyConfig.HotkeyAddPiece11, () => AddPieceViaHotkey(11));
            AddHotkey(HotkeyConfig.HotkeyAddPiece12, () => AddPieceViaHotkey(12));
            AddHotkey(HotkeyConfig.HotkeyAddPiece13, () => AddPieceViaHotkey(13));
            AddHotkey(HotkeyConfig.HotkeyUndo, () => UndoLastChange());
            AddHotkey(HotkeyConfig.HotkeyRedo, () => CancelLastUndo());
            AddHotkey(HotkeyConfig.HotkeyCut, () => DeleteSelectedPieces());
            AddHotkey(HotkeyConfig.HotkeyCopy, () => WriteToClipboard());
            AddHotkey(HotkeyConfig.HotkeyPaste, () => AddFromClipboard(true));
            AddHotkey(HotkeyConfig.HotkeyPasteInPlace, () => AddFromClipboard(false));
            AddHotkey(HotkeyConfig.HotkeyDuplicate, () => DuplicateSelectedPieces());
            AddHotkey(HotkeyConfig.HotkeyDelete, () => DeleteSelectedPieces(false));
            AddHotkey(HotkeyConfig.HotkeyMoveUp, () => MoveLevelPiecesOrScreenPos(C.DIR.N, 1));
            AddHotkey(HotkeyConfig.HotkeyMoveDown, () => MoveLevelPiecesOrScreenPos(C.DIR.S, 1));
            AddHotkey(HotkeyConfig.HotkeyMoveLeft, () => MoveLevelPiecesOrScreenPos(C.DIR.W, 1));
            AddHotkey(HotkeyConfig.HotkeyMoveRight, () => MoveLevelPiecesOrScreenPos(C.DIR.E, 1));
            AddHotkey(HotkeyConfig.HotkeyMove8Up, () => MoveLevelPiecesOrScreenPos(C.DIR.N, 8));
            AddHotkey(HotkeyConfig.HotkeyMove8Down, () => MoveLevelPiecesOrScreenPos(C.DIR.S, 8));
            AddHotkey(HotkeyConfig.HotkeyMove8Left, () => MoveLevelPiecesOrScreenPos(C.DIR.W, 8));
            AddHotkey(HotkeyConfig.HotkeyMove8Right, () => MoveLevelPiecesOrScreenPos(C.DIR.E, 8));
            AddHotkey(HotkeyConfig.HotkeyCustomMoveUp, () => MoveLevelPiecesOrScreenPos(C.DIR.N, customMove));
            AddHotkey(HotkeyConfig.HotkeyCustomMoveDown, () => MoveLevelPiecesOrScreenPos(C.DIR.S, customMove));
            AddHotkey(HotkeyConfig.HotkeyCustomMoveLeft, () => MoveLevelPiecesOrScreenPos(C.DIR.W, customMove));
            AddHotkey(HotkeyConfig.HotkeyCustomMoveRight, () => MoveLevelPiecesOrScreenPos(C.DIR.E, customMove));

            //AddHotkey(HotkeyConfig.HotkeyDragHorizontally, () => "mouse mandatory" ());
            //AddHotkey(HotkeyConfig.HotkeyDragVertically, () => "mouse mandatory" ());

            AddHotkey(HotkeyConfig.HotkeyRotate, () => RotateLevelPieces());
            AddHotkey(HotkeyConfig.HotkeyFlip, () => FlipLevelPieces());
            AddHotkey(HotkeyConfig.HotkeyInvert, () => InvertLevelPieces());
            AddHotkey(HotkeyConfig.HotkeyGroup, () => GroupSelectedPieces());
            AddHotkey(HotkeyConfig.HotkeyUngroup, () => UngroupSelectedPieces());
            AddHotkey(HotkeyConfig.HotkeyErase, () => check_Pieces_Erase.Checked = !check_Pieces_Erase.Checked);
            AddHotkey(HotkeyConfig.HotkeyNoOverwrite, () => check_Pieces_NoOv.Checked = !check_Pieces_NoOv.Checked);
            AddHotkey(HotkeyConfig.HotkeyOnlyOnTerrain, () => check_Pieces_OnlyOnTerrain.Checked = !check_Pieces_OnlyOnTerrain.Checked);
            AddHotkey(HotkeyConfig.HotkeyAllowOneWay, () => check_Pieces_OneWay.Checked = !check_Pieces_OneWay.Checked);
            AddHotkey(HotkeyConfig.HotkeyDrawLast, () => MovePieceIndex(true, false));
            AddHotkey(HotkeyConfig.HotkeyDrawSooner, () => MovePieceIndex(true, true));
            AddHotkey(HotkeyConfig.HotkeyDrawLater, () => MovePieceIndex(false, true));
            AddHotkey(HotkeyConfig.HotkeyDrawFirst, () => MovePieceIndex(false, false));
            AddHotkey(HotkeyConfig.HotkeyCloseWindow, () => {} ); // All secondary windows respond to [Esc] by closing anyway
            AddHotkey(HotkeyConfig.HotkeyCloseEditor, () => Application.Exit());
        }

        /// <summary>
        /// This only updates the hotkey strings in menu items
        /// The hotkey-action linkups are done in InitializeHotkeyActions
        /// </summary>
        private void UpdateMenuShortcutKeyDisplayStrings()
        {           
            newToolStripMenuItem.ShortcutKeyDisplayString =
                HotkeyConfig.FormatHotkeyString(HotkeyConfig.HotkeyCreateNewLevel);

            loadToolStripMenuItem.ShortcutKeyDisplayString =
                HotkeyConfig.FormatHotkeyString(HotkeyConfig.HotkeyLoadLevel);

            saveToolStripMenuItem.ShortcutKeyDisplayString =
                HotkeyConfig.FormatHotkeyString(HotkeyConfig.HotkeySaveLevel);

            saveAsToolStripMenuItem.ShortcutKeyDisplayString =
                HotkeyConfig.FormatHotkeyString(HotkeyConfig.HotkeySaveLevelAs);

            saveAsImageToolStripMenuItem.ShortcutKeyDisplayString =
                HotkeyConfig.FormatHotkeyString(HotkeyConfig.HotkeySaveLevelAsImage);

            exitToolStripMenuItem.ShortcutKeyDisplayString =
                HotkeyConfig.FormatHotkeyString(HotkeyConfig.HotkeyCloseEditor);

            playLevelToolStripMenuItem.ShortcutKeyDisplayString =
                HotkeyConfig.FormatHotkeyString(HotkeyConfig.HotkeyPlaytestLevel);

            validateLevelToolStripMenuItem.ShortcutKeyDisplayString =
                HotkeyConfig.FormatHotkeyString(HotkeyConfig.HotkeyValidateLevel);

            cleanseLevelsToolStripMenuItem.ShortcutKeyDisplayString =
                HotkeyConfig.FormatHotkeyString(HotkeyConfig.HotkeyCleanseLevels);

            undoToolStripMenuItem.ShortcutKeyDisplayString =
                HotkeyConfig.FormatHotkeyString(HotkeyConfig.HotkeyUndo);

            redoToolStripMenuItem.ShortcutKeyDisplayString =
                HotkeyConfig.FormatHotkeyString(HotkeyConfig.HotkeyRedo);

            cutToolStripMenuItem.ShortcutKeyDisplayString =
                HotkeyConfig.FormatHotkeyString(HotkeyConfig.HotkeyCut);

            copyToolStripMenuItem.ShortcutKeyDisplayString =
                HotkeyConfig.FormatHotkeyString(HotkeyConfig.HotkeyCopy);

            pasteToolStripMenuItem.ShortcutKeyDisplayString =
                HotkeyConfig.FormatHotkeyString(HotkeyConfig.HotkeyPaste);

            pasteInPlaceToolStripMenuItem.ShortcutKeyDisplayString =
                HotkeyConfig.FormatHotkeyString(HotkeyConfig.HotkeyPasteInPlace);

            duplicateToolStripMenuItem.ShortcutKeyDisplayString =
                HotkeyConfig.FormatHotkeyString(HotkeyConfig.HotkeyDuplicate);

            groupToolStripMenuItem.ShortcutKeyDisplayString =
                HotkeyConfig.FormatHotkeyString(HotkeyConfig.HotkeyGroup);

            ungroupToolStripMenuItem.ShortcutKeyDisplayString =
                HotkeyConfig.FormatHotkeyString(HotkeyConfig.HotkeyUngroup);

            clearPhysicsToolStripMenuItem.ShortcutKeyDisplayString =
                HotkeyConfig.FormatHotkeyString(HotkeyConfig.HotkeyToggleClearPhysics);

            terrainToolStripMenuItem.ShortcutKeyDisplayString =
                HotkeyConfig.FormatHotkeyString(HotkeyConfig.HotkeyToggleTerrain);

            objectToolStripMenuItem.ShortcutKeyDisplayString =
                HotkeyConfig.FormatHotkeyString(HotkeyConfig.HotkeyToggleObjects);

            triggerAreasToolStripMenuItem.ShortcutKeyDisplayString =
                HotkeyConfig.FormatHotkeyString(HotkeyConfig.HotkeyToggleTriggerAreas);

            screenStartToolStripMenuItem.ShortcutKeyDisplayString =
                HotkeyConfig.FormatHotkeyString(HotkeyConfig.HotkeyToggleScreenStart);

            backgroundToolStripMenuItem.ShortcutKeyDisplayString =
                HotkeyConfig.FormatHotkeyString(HotkeyConfig.HotkeyToggleBackground);

            deprecatedPiecesToolStripMenuItem.ShortcutKeyDisplayString =
                HotkeyConfig.FormatHotkeyString(HotkeyConfig.HotkeyToggleDeprecatedPieces);

            showMissingPiecesToolStripMenuItem.ShortcutKeyDisplayString =
                HotkeyConfig.FormatHotkeyString(HotkeyConfig.HotkeyShowMissingPieces);

            snapToGridToolStripMenuItem.ShortcutKeyDisplayString =
                HotkeyConfig.FormatHotkeyString(HotkeyConfig.HotkeyToggleSnapToGrid);

            settingsToolStripMenuItem.ShortcutKeyDisplayString =
                HotkeyConfig.FormatHotkeyString(HotkeyConfig.HotkeyOpenSettings);

            hotkeysToolStripMenuItem.ShortcutKeyDisplayString =
                HotkeyConfig.FormatHotkeyString(HotkeyConfig.HotkeyOpenConfigHotkeys);

            aboutToolStripMenuItem.ShortcutKeyDisplayString =
                HotkeyConfig.FormatHotkeyString(HotkeyConfig.HotkeyOpenAboutSLX);
        }
    }
}
