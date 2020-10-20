﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
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
        /// Reads the user's settings from the file NLEditorSettings.ini and applies these options.
        /// </summary>
        private void InitializeSettings()
        {
            curSettings.ReadSettingsFromFile();
            ApplyOptionLvlPropertiesTabs();
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
        private void RemoveFocus()
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
            CurLevel.StartPosX = decimal.ToInt32(num_Lvl_StartX.Value);
            CurLevel.StartPosY = decimal.ToInt32(num_Lvl_StartY.Value);
            CurLevel.NumLems = decimal.ToInt32(num_Lvl_Lems.Value);
            CurLevel.SaveReq = decimal.ToInt32(num_Lvl_Rescue.Value);
            CurLevel.SpawnRate = decimal.ToInt32(num_Lvl_SR.Value);
            CurLevel.IsSpawnRateFix = check_Lvl_LockSR.Checked;
            CurLevel.TimeLimit = decimal.ToInt32(num_Lvl_TimeMin.Value) * 60
                                    + decimal.ToInt32(num_Lvl_TimeSec.Value);
            CurLevel.IsNoTimeLimit = check_Lvl_InfTime.Checked;

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

        /// <summary>
        /// Takes the global level settings and displays them in the correct form fields.
        /// </summary>
        private void WriteLevelInfoToForm()
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

            txt_LevelID.Text = CurLevel.LevelID.ToString("X16");

            foreach (C.Skill skill in numericsSkillSet.Keys)
            {
                numericsSkillSet[skill].Value = CurLevel.SkillSet[skill];
            }

            RegenerateTalismanList();
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
            RemoveInvalidLevelPieces();
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

        /// <summary>
        /// Removes all pieces for which no image in the corresponding style exists.
        /// <para> A warning is displayed if pieces are removed. </para>
        /// </summary>
        private void RemoveInvalidLevelPieces()
        {
            if (CurLevel == null)
                return;

            HashSet<string> missingImageNames = new HashSet<string>();
            CurLevel.TerrainList.FindAll(ter => !ter.ExistsImage())
                                .ForEach(ter => missingImageNames.Add(ter.Name + " in style " + ter.Style));
            CurLevel.GadgetList.FindAll(gad => !gad.ExistsImage())
                               .ForEach(gad => missingImageNames.Add(gad.Name + " in style " + gad.Style));

            CurLevel.TerrainList.RemoveAll(ter => !ter.ExistsImage());
            CurLevel.GadgetList.RemoveAll(gad => !gad.ExistsImage());

            if (missingImageNames.Count > 0)
            {
                string message = "Warning: The following pieces are unknown: " + C.NewLine;
                message += string.Join(C.NewLine + " ", missingImageNames);
                MessageBox.Show(message, "Unknown level pieces");
            }
        }

        /// <summary>
        /// If the levle changed, displays a message box and asks whether to save the current level.  
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
        private void SaveInNewFileLevel(bool isPlaytest = false)
        {
            // get most up-to-date global info
            ReadLevelInfoFromForm(true);

            LevelFile.SaveLevel(CurLevel, levelDirectory);
            SaveChangesToOldLevelList();
            levelDirectory = System.IO.Path.GetDirectoryName(CurLevel.FilePathToSave);
            if (!isPlaytest)
                lastSavedLevel = CurLevel.Clone();
        }

        /// <summary>
        /// Saves the current level in the current location. If no location is known, the file browser is opened.
        /// </summary>
        private void SaveLevel(bool isPlaytest = false)
        {
            if (CurLevel.FilePathToSave == null)
            {
                SaveInNewFileLevel();
            }
            else
            {
                // get most up-to-date global info
                ReadLevelInfoFromForm(true);

                LevelFile.SaveLevelToFile(CurLevel.FilePathToSave, CurLevel);
                SaveChangesToOldLevelList();
                if (!isPlaytest)
                    lastSavedLevel = CurLevel.Clone();
            }
        }

        /// <summary>
        /// Saves the level as TempTestLevel.nxlv and loads this level in the NeoLemmix player.
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

            if (!System.IO.File.Exists(C.AppPathNeoLemmix))
            {
                MessageBox.Show("Error: Player NeoLemmix.exe not found in editor directory.", "File not found");
            }
            else
            {
                try
                {
                    // Start the NeoLemmix player.
                    var playerStartInfo = new System.Diagnostics.ProcessStartInfo();
                    playerStartInfo.FileName = C.AppPathNeoLemmix;
                    playerStartInfo.Arguments = "test " + "\"" + C.AppPathTempLevel + "\"";

                    System.Diagnostics.Process.Start(playerStartInfo);
                }
                catch (Exception Ex)
                {
                    Utility.LogException(Ex);
                    MessageBox.Show("Error: Starting NeoLemmix.exe failed or was aborted.", "Application start failed");
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
            RemoveFocus();
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
                if (isCtrlPressed)
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
        private void MoveLevelPieces(C.DIR direction, int step = 1)
        {
            CurLevel.MovePieces(direction, step, gridSize);
            pic_Level.Image = curRenderer.CreateLevelImage();
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
            CurLevel.GetLemmingTypeCounts(out int normalCount, out int zombieCount, out int neutralCount);
            string newText =
                normalCount.ToString() + " Normal";
            if (zombieCount > 0)
                newText += ", " + zombieCount.ToString() + " Zombie";
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
    }
}
