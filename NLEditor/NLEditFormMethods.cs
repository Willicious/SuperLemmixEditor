using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;

namespace NLEditor
{
    public partial class NLEditForm
    {
        /*---------------------------------------------------------
         *   Main Form: This part defines the methods
         *     called from user input
         * -------------------------------------------------------- */

        /// <summary>
        /// Sets fStyleList and creates the styles, but does not yet load sprites.
        /// </summary>
        private void CreateStyleList()
        {
            // get list of all existing style names
            List<string> StyleNameList = null;

            try
            {
                StyleNameList = System.IO.Directory.GetDirectories(C.AppPathPieces)
                                                   .Select(dir => System.IO.Path.GetFileName(dir))
                                                   .ToList();
            }
            catch (Exception Ex)
            {
                Utility.LogException(Ex);

                MessageBox.Show(Ex.Message);
                Environment.Exit(-1);
            }

            // Create the StyleList from the StyleNameList
            StyleNameList.RemoveAll(sty => sty == "default");
            fStyleList = StyleNameList.Select(sty => new Style(sty)).ToList();
            fStyleList = LoadStylesFromFile.OrderAndRenameStyles(fStyleList);
        }

        /// <summary>
        /// Removes focus from the current control and moves it to the default location txt_Focus.
        /// </summary>
        private void RemoveFocus()
        {
            this.ActiveControl = this.txt_Focus;
        }


        /// <summary>
        /// Takes the global level data input on the form and stores it in the current level.
        /// </summary>
        private void ReadLevelInfoFromForm()
        {
            CurLevel.Author = this.txt_LevelAuthor.Text;
            CurLevel.Title = this.txt_LevelTitle.Text;
            CurLevel.MusicFile = this.combo_Music.Text;
            CurLevel.MainStyle = ValidateStyleName(this.combo_MainStyle.Text);
            CurLevel.Width = Decimal.ToInt32(this.num_Lvl_SizeX.Value);
            CurLevel.Height = Decimal.ToInt32(this.num_Lvl_SizeY.Value);
            CurLevel.StartPosX = Decimal.ToInt32(this.num_Lvl_StartX.Value);
            CurLevel.StartPosY = Decimal.ToInt32(this.num_Lvl_StartY.Value);
            CurLevel.NumLems = Decimal.ToInt32(this.num_Lvl_Lems.Value);
            CurLevel.SaveReq = Decimal.ToInt32(this.num_Lvl_Rescue.Value);
            CurLevel.ReleaseRate = Decimal.ToInt32(this.num_Lvl_RR.Value);
            CurLevel.IsReleaseRateFix = this.check_Lvl_LockRR.Checked;
            CurLevel.TimeLimit = Decimal.ToInt32(this.num_Lvl_TimeMin.Value) * 60
                                    + Decimal.ToInt32(this.num_Lvl_TimeSec.Value);
            CurLevel.IsNoTimeLimit = this.check_Lvl_InfTime.Checked;

            CurLevel.SkillCount[C.SKI_CLIMBER] = Decimal.ToInt32(this.num_Ski_Climber.Value);
            CurLevel.SkillCount[C.SKI_FLOATER] = Decimal.ToInt32(this.num_Ski_Floater.Value);
            CurLevel.SkillCount[C.SKI_BLOCKER] = Decimal.ToInt32(this.num_Ski_Blocker.Value);
            CurLevel.SkillCount[C.SKI_EXPLODER] = Decimal.ToInt32(this.num_Ski_Exploder.Value);
            CurLevel.SkillCount[C.SKI_BUILDER] = Decimal.ToInt32(this.num_Ski_Builder.Value);
            CurLevel.SkillCount[C.SKI_BASHER] = Decimal.ToInt32(this.num_Ski_Basher.Value);
            CurLevel.SkillCount[C.SKI_MINER] = Decimal.ToInt32(this.num_Ski_Miner.Value);
            CurLevel.SkillCount[C.SKI_DIGGER] = Decimal.ToInt32(this.num_Ski_Digger.Value);
            CurLevel.SkillCount[C.SKI_WALKER] = Decimal.ToInt32(this.num_Ski_Walker.Value);
            CurLevel.SkillCount[C.SKI_SWIMMER] = Decimal.ToInt32(this.num_Ski_Swimmer.Value);
            CurLevel.SkillCount[C.SKI_GLIDER] = Decimal.ToInt32(this.num_Ski_Glider.Value);
            CurLevel.SkillCount[C.SKI_DISARMER] = Decimal.ToInt32(this.num_Ski_Disarmer.Value);
            CurLevel.SkillCount[C.SKI_STONER] = Decimal.ToInt32(this.num_Ski_Stoner.Value);
            CurLevel.SkillCount[C.SKI_PLATFORMER] = Decimal.ToInt32(this.num_Ski_Platformer.Value);
            CurLevel.SkillCount[C.SKI_STACKER] = Decimal.ToInt32(this.num_Ski_Stacker.Value);
            CurLevel.SkillCount[C.SKI_CLONER] = Decimal.ToInt32(this.num_Ski_Cloner.Value);
        }

        /// <summary>
        /// Takes the global level settings and displays them in the correct form fields.
        /// </summary>
        private void WriteLevelInfoToForm()
        {
            this.txt_LevelAuthor.Text = CurLevel.Author;
            this.txt_LevelTitle.Text = CurLevel.Title;
            this.combo_Music.Text = CurLevel.MusicFile;
            this.combo_MainStyle.Text = (CurLevel.MainStyle != null) ? CurLevel.MainStyle.NameInEditor : "";
            this.num_Lvl_SizeX.Value = CurLevel.Width;
            this.num_Lvl_SizeY.Value = CurLevel.Height;
            this.num_Lvl_StartX.Value = CurLevel.StartPosX;
            this.num_Lvl_StartY.Value = CurLevel.StartPosY;
            this.num_Lvl_Lems.Value = CurLevel.NumLems;
            this.num_Lvl_Rescue.Value = CurLevel.SaveReq;
            this.num_Lvl_RR.Value = CurLevel.ReleaseRate;
            this.check_Lvl_LockRR.Checked = CurLevel.IsReleaseRateFix;
            this.num_Lvl_TimeMin.Value = CurLevel.TimeLimit / 60;
            this.num_Lvl_TimeSec.Value = CurLevel.TimeLimit % 60;
            this.check_Lvl_InfTime.Checked = CurLevel.IsNoTimeLimit;

            this.num_Ski_Climber.Value = CurLevel.SkillCount[C.SKI_CLIMBER];
            this.num_Ski_Floater.Value = CurLevel.SkillCount[C.SKI_FLOATER];
            this.num_Ski_Blocker.Value = CurLevel.SkillCount[C.SKI_BLOCKER];
            this.num_Ski_Exploder.Value = CurLevel.SkillCount[C.SKI_EXPLODER];
            this.num_Ski_Builder.Value = CurLevel.SkillCount[C.SKI_BUILDER];
            this.num_Ski_Basher.Value = CurLevel.SkillCount[C.SKI_BASHER];
            this.num_Ski_Miner.Value = CurLevel.SkillCount[C.SKI_MINER];
            this.num_Ski_Digger.Value = CurLevel.SkillCount[C.SKI_DIGGER];
            this.num_Ski_Walker.Value = CurLevel.SkillCount[C.SKI_WALKER];
            this.num_Ski_Swimmer.Value = CurLevel.SkillCount[C.SKI_SWIMMER];
            this.num_Ski_Glider.Value = CurLevel.SkillCount[C.SKI_GLIDER];
            this.num_Ski_Disarmer.Value = CurLevel.SkillCount[C.SKI_DISARMER];
            this.num_Ski_Stoner.Value = CurLevel.SkillCount[C.SKI_STONER];
            this.num_Ski_Platformer.Value = CurLevel.SkillCount[C.SKI_PLATFORMER];
            this.num_Ski_Stacker.Value = CurLevel.SkillCount[C.SKI_STACKER];
            this.num_Ski_Cloner.Value = CurLevel.SkillCount[C.SKI_CLONER];

            this.num_Lvl_StartX.Maximum = CurLevel.Width - 320;
            this.num_Lvl_StartY.Maximum = CurLevel.Height - 160;
        }

        /// <summary>
        /// Warning: Does not yet check whether the level was saved!
        /// </summary>
        private void ExitEditor()
        {
            DeleteFile(C.AppPath + "TempTestLevel.nxlv");
            DeleteFile(C.AppPath + "TempTestLevel.nxsv");
            
            Application.Exit();
        }

        /// <summary>
        /// Deletes the specified file, if it exists.
        /// </summary>
        /// <param name="FilePath"></param>
        private void DeleteFile(string FilePath)
        {
            if (System.IO.File.Exists(FilePath))
            {
                try
                {
                    System.IO.File.Delete(FilePath);
                }
                finally
                {
                    // do nothing.
                }
            }
        }

        /// <summary>
        /// Creates a new instance of a Level and a new Renderer, then displays it on the form.
        /// </summary>
        private void CreateNewLevelAndRenderer()
        {
            Style NewMainStyle = null;
            if (StyleList != null)
            {
                NewMainStyle = StyleList.Find(sty => sty.NameInEditor == this.combo_MainStyle.Text);
            }
            fCurLevel = new Level(NewMainStyle);
            WriteLevelInfoToForm();
            UpdateBackgroundColor();

            fCurRenderer = new Renderer(fCurLevel, this.pic_Level);
            UpdateFlagsForPieceActions();
            this.pic_Level.Image = fCurRenderer.CreateLevelImage();
        }

        /// <summary>
        /// Displays a file browse and loads the selected level
        /// </summary>
        private void LoadNewLevel()
        {
            Level NewLevel = LevelFile.LoadLevel(StyleList);
            if (NewLevel == null) return;

            fCurLevel = NewLevel;
            fCurRenderer.SetLevel(fCurLevel);
            WriteLevelInfoToForm();
            UpdateFlagsForPieceActions();
            this.pic_Level.Image = fCurRenderer.CreateLevelImage();
        }

        /// <summary>
        /// Displays a file browser and saves the current level in chosen location. 
        /// </summary>
        private void SaveInNewFileLevel()
        {
            // get most up-to-date global info
            ReadLevelInfoFromForm();

            LevelFile.SaveLevel(fCurLevel);
        }

        /// <summary>
        /// Saves the current level in the current location. If no location is known, the file browser is opened.
        /// </summary>
        private void SaveLevel()
        { 
            if (fCurLevel.FilePathToSave == null)
            {
                SaveInNewFileLevel();
            }
            else 
            {
                // get most up-to-date global info
                ReadLevelInfoFromForm();

                LevelFile.SaveLevelToFile(fCurLevel.FilePathToSave, fCurLevel);
            }
        }

        /// <summary>
        /// Saves the level as TempTestLevel.nxlv and loads this level in the NeoLemmix player.
        /// </summary>
        private void PlaytestLevel()
        {
            // Save the level as TempTestLevel.nxlv.
            string OrigFilePath = fCurLevel.FilePathToSave;
            fCurLevel.FilePathToSave = C.AppPath + "TempTestLevel.nxlv";
            SaveLevel();
            fCurLevel.FilePathToSave = OrigFilePath;

            // Start the NeoLemmix player.
            System.Diagnostics.ProcessStartInfo PlayerStartInfo = new System.Diagnostics.ProcessStartInfo();
            PlayerStartInfo.FileName = C.AppPath + "NeoLemmix.exe";
            PlayerStartInfo.Arguments = C.AppPath + "TempTestLevel.nxlv";

            if (!System.IO.File.Exists(PlayerStartInfo.FileName))
            {
                MessageBox.Show("Error: Player NeoLemmix.exe not found in editor directory.");
            }
            else
            {
                System.Diagnostics.Process.Start(PlayerStartInfo);
            }
        }

        /// <summary>
        /// Creates a new LevelValidator, runs the validation and displays the result in a new form.
        /// </summary>
        private void ValidateLevel()
        {
            ReadLevelInfoFromForm();

            LevelValidator MyValidator = new LevelValidator(fCurLevel);
            MyValidator.Validate();
        }


        /// <summary>
        /// Switches between displaying objects and terrain for newly added pieces.
        /// </summary>
        private void ChangeObjTerrPieceDisplay()
        {
            fPieceDoDisplayObject = !fPieceDoDisplayObject;

            fPieceStartIndex = 0;
            LoadPiecesIntoPictureBox();

            this.but_PieceTerrObj.Text = fPieceDoDisplayObject ? "Get Terrain" : "Get Objects";
        }

        /// <summary>
        /// Displays new pieces on the piece selection bar.
        /// </summary>
        /// <param name="Movement"></param>
        private void MoveTerrPieceSelection(int Movement)
        {
            if (fPieceCurStyle == null) return;

            List<string> PieceNameList = fPieceDoDisplayObject ? fPieceCurStyle.ObjectNames : fPieceCurStyle.TerrainNames;
            if (PieceNameList == null || PieceNameList.Count == 0) return;

            // Pass to correct piece index
            fPieceStartIndex = (fPieceStartIndex + Movement) % PieceNameList.Count;
            // ensure that PieceStartIndex is positive
            fPieceStartIndex = (fPieceStartIndex + PieceNameList.Count) % PieceNameList.Count;

            LoadPiecesIntoPictureBox();
        }

        /// <summary>
        /// Changes the style for newly added pieces and displays the new pieces.
        /// </summary>
        /// <param name="Movement"></param>
        private void ChangeNewPieceStyleSelection(int Movement)
        {
            if (StyleList == null || StyleList.Count == 0) return;

            int NewStyleIndex;

            if (fPieceCurStyle == null)
            {
                NewStyleIndex = ((Movement % StyleList.Count) + StyleList.Count) % StyleList.Count;
            }
            else 
            {
                int CurStyleIndex = StyleList.FindIndex(sty => sty.Equals(fPieceCurStyle));
                System.Diagnostics.Debug.Assert(CurStyleIndex != -1, "Current style for new pieces not found in StyleList.");

                NewStyleIndex = Math.Min(Math.Max(CurStyleIndex + Movement, 0), StyleList.Count - 1);
            }

            fPieceCurStyle = StyleList[NewStyleIndex];
            fPieceStartIndex = 0;
            LoadPiecesIntoPictureBox();

            this.combo_PieceStyle.SelectedIndex = NewStyleIndex;
        }

        /// <summary>
        /// Adds a new piece to the level and displays the result to the user.
        /// </summary>
        /// <param name="picPieceIndex"></param>
        private void AddNewPieceToLevel(int picPieceIndex)
        {
            fCurLevel.DeleteAllSelections();
            
            List<string> CurPieceList = fPieceDoDisplayObject ?  fPieceCurStyle.ObjectNames : fPieceCurStyle.TerrainNames;
            int PieceIndex = (picPieceIndex + fPieceStartIndex) % CurPieceList.Count;

            CurLevel.AddPiece(fPieceCurStyle, fPieceDoDisplayObject, PieceIndex, fCurRenderer.GetCenterPoint());

            this.pic_Level.Image = fCurRenderer.CreateLevelImage();
        }

        /// <summary>
        /// Changes the selection of existing pieces by adding or removing one piece.
        /// </summary>
        private void LevelSelectSinglePiece()
        {
            // Check whether MouseStartPos is actually in pic_Level
            Point? LevelPos = fCurRenderer.GetMousePosInLevel(false);
            if (LevelPos == null) return;

            if (fMouseButtonPressed == MouseButtons.Left)
            {
                // Delete all existing selections
                if (!fIsCtrlPressed)
                {
                    fCurLevel.DeleteAllSelections();
                }
                
                // Add a single piece
                fCurLevel.SelectOnePiece((Point)LevelPos, true, fIsAltPressed);
            }
            else if (fMouseButtonPressed == MouseButtons.Middle)
            {
                // Remove a single piece
                fCurLevel.SelectOnePiece((Point)LevelPos, false, fIsAltPressed);
            }
        }

        /// <summary>
        /// Changes the selection of existing pieces by adding or removing all pieces in a certain area.
        /// </summary>
        private void LevelSelectAreaPieces()
        {
            // Get rectangle from user input
            Rectangle? SelectArea = fCurRenderer.GetCurSelectionInLevel();
            if (SelectArea == null) return;

            if (fMouseButtonPressed == MouseButtons.Left)
            {
                // Delete all existing selections
                if (!fIsCtrlPressed)
                {
                    fCurLevel.DeleteAllSelections();
                }

                // Add all pieces intersection SelectArea
                fCurLevel.SelectAreaPiece((Rectangle)SelectArea, true);
            }
            else if (fMouseButtonPressed == MouseButtons.Middle)
            {
                // Remove all pieces intersection SelectArea
                fCurLevel.SelectAreaPiece((Rectangle)SelectArea, false);
            }
        }

        /// <summary>
        /// Moves all selected pieces of the level and displays the result.
        /// </summary>
        /// <param name="Direction"></param>
        /// <param name="Step"></param>
        private void MoveLevelPieces(C.DIR Direction, int Step = 1)
        {
            fCurLevel.MovePieces(Direction, Step);
            this.pic_Level.Image = fCurRenderer.CreateLevelImage();
        }

        /// <summary>
        /// Drags all selected pieces from the original mouse position to the current one and displays the result.
        /// </summary>
        private void DragSelectedPieces()
        {
            Point DeltaPos = fCurRenderer.GetDeltaPos();
            fCurLevel.MovePieces(C.DIR.E, DeltaPos.X);
            fCurLevel.MovePieces(C.DIR.S, DeltaPos.Y);
            this.pic_Level.Image = fCurRenderer.CreateLevelImage();
        }

        /// <summary>
        /// Rotates all selected pieces in the level and displays the result.
        /// </summary>
        private void RotateLevelPieces()
        {
            fCurLevel.RotatePieces();
            this.pic_Level.Image = fCurRenderer.CreateLevelImage();
        }

        /// <summary>
        /// Inverts all selected pieces in the level and displays the result.
        /// </summary>
        private void InvertLevelPieces()
        {
            fCurLevel.InvertPieces();
            this.pic_Level.Image = fCurRenderer.CreateLevelImage();
        }

        /// <summary>
        /// Flips all selected pieces in the level and displays the result.
        /// </summary>
        private void FlipLevelPieces()
        {
            fCurLevel.FlipPieces();
            this.pic_Level.Image = fCurRenderer.CreateLevelImage();
        }

        /// <summary>
        /// Sets the NoOverwrite flag for all selected pieces and displays the result.
        /// </summary>
        /// <param name="DoAdd"></param>
        private void SetNoOverwrite(bool DoAdd)
        {
            fCurLevel.SetNoOverwrite(DoAdd);
            UpdateFlagsForPieceActions();
            this.pic_Level.Image = fCurRenderer.CreateLevelImage();
        }

        /// <summary>
        /// Sets the Erase flag for all selected pieces and displays the result.
        /// </summary>
        /// <param name="DoAdd"></param>
        private void SetErase(bool DoAdd)
        {
            fCurLevel.SetErase(DoAdd);
            UpdateFlagsForPieceActions();
            this.pic_Level.Image = fCurRenderer.CreateLevelImage();
        }

        /// <summary>
        /// Sets the OnlyOnTerrain flag for all selected pieces and displays the result.
        /// </summary>
        /// <param name="DoAdd"></param>
        private void SetOnlyOnTerrain(bool DoAdd)
        {
            fCurLevel.SetOnlyOnTerrain(DoAdd);
            UpdateFlagsForPieceActions();
            this.pic_Level.Image = fCurRenderer.CreateLevelImage();
        }

        /// <summary>
        /// Sets the OneWayAdmissible flag for all selected pieces and displays the result.
        /// </summary>
        /// <param name="DoAdd"></param>
        private void SetOneWay(bool DoAdd)
        {
            fCurLevel.SetOneWay(DoAdd);
            this.pic_Level.Image = fCurRenderer.CreateLevelImage();
        }

        /// <summary>
        /// Sets skill flags for all selected objects.
        /// </summary>
        /// <param name="Skill"></param>
        /// <param name="DoAdd"></param>
        private void SetSkillForObjects(int Skill, bool DoAdd)
        {
            fCurLevel.SetSkillForObjects(Skill, DoAdd);
            UpdateFlagsForPieceActions();
        }

        /// <summary>
        /// Changes the index of all selected pieces and displays the result.
        /// </summary>
        /// <param name="ToTop"></param>
        /// <param name="OnlyOneStep"></param>
        private void MovePieceIndex(bool ToFront, bool OnlyOneStep)
        {
            fCurLevel.MoveSelectedIndex(ToFront, OnlyOneStep);
            this.pic_Level.Image = fCurRenderer.CreateLevelImage();
        }


    }
}
