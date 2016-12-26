using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics;

namespace NLEditor
{
    /// <summary>
    /// Main editor form.
    /// </summary>
    public partial class NLEditForm : Form
    {
        /*---------------------------------------------------------
         *   Main Form: This part defines the variables 
         *     and reads all the user input
         * -------------------------------------------------------- */
        
        /// <summary>
        /// Initializes all important components and load an empty level.
        /// </summary>
        public NLEditForm()
        {
            InitializeComponent();
            RemoveFocus();
            SetRepeatButtonIntervals();
            LoadStylesFromFile.AddInitialImagesToLibrary();

            this.MouseWheel += new System.Windows.Forms.MouseEventHandler(NLEditForm_MouseWheel);

            fpicPieceList = new List<PictureBox> 
                { 
                    this.picPiece0, this.picPiece1, this.picPiece2, this.picPiece3,
                    this.picPiece4, this.picPiece5, this.picPiece6, this.picPiece7 
                };

            fcheckSkillFlagList = new List<CheckBox>
                { 
                    this.check_Piece_Climber, this.check_Piece_Floater, this.check_Piece_Blocker,
                    this.check_Piece_Exploder, this.check_Piece_Builder, this.check_Piece_Basher,
                    this.check_Piece_Miner, this.check_Piece_Digger, this.check_Piece_Walker,
                    this.check_Piece_Swimmer, this.check_Piece_Glider, this.check_Piece_Disarmer,
                    this.check_Piece_Stoner, this.check_Piece_Platformer, this.check_Piece_Stacker,
                    this.check_Piece_Cloner, this.check_Piece_Zombie
                };

            CreateStyleList();
            if (StyleList.Count > 0)
            {
                this.combo_MainStyle.Items.AddRange(StyleList.ConvertAll(sty => sty.NameInEditor).ToArray());
                this.combo_MainStyle.SelectedIndex = 0;

                this.combo_PieceStyle.Items.AddRange(StyleList.ConvertAll(sty => sty.NameInEditor).ToArray());
                this.combo_PieceStyle.SelectedIndex = 0;
            }

            CreateNewLevelAndRenderer();
            UpdateFlagsForPieceActions();

            fPieceStartIndex = 0;
            fPieceDoDisplayObject = false;
            fPieceCurStyle = ValidateStyleName(this.combo_PieceStyle.SelectedItem.ToString());
            LoadPiecesIntoPictureBox();

            fStopWatchKey = new Stopwatch();
            fStopWatchKey.Start();
            fStopWatchMouse = new Stopwatch();
            fStopWatchMouse.Start();

            fMouseButtonPressed = null;
        }

        List<PictureBox> fpicPieceList;
        List<CheckBox> fcheckSkillFlagList;

        List<Style> fStyleList;
        Style fPieceCurStyle;
        int fPieceStartIndex;
        bool fPieceDoDisplayObject;

        Level fCurLevel;
        Renderer fCurRenderer;
        List<Level> fOldLevelList;
        int fCurOldLevelIndex;
        List<LevelPiece> fOldSelectedList;
        Level fLastSavedLevel;

        Stopwatch fStopWatchKey;
        Stopwatch fStopWatchMouse;
        MouseButtons? fMouseButtonPressed;


        bool fIsShiftPressed;
        bool fIsCtrlPressed;
        bool fIsAltPressed;
        

        public List<PictureBox> picPieceList { get { return fpicPieceList; } }
        public List<Style> StyleList { get { return fStyleList; } }
        public Level CurLevel { get { return fCurLevel; } }

        private void NLEditForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            Utility.DeleteFile(C.AppPath + "TempTestLevel.nxlv");
            Utility.DeleteFile(C.AppPath + "TempTestLevel.nxsv");
            
            if (e.CloseReason.In(CloseReason.UserClosing, CloseReason.ApplicationExitCall))
            {
                AskUserWhetherSaveLevel();
            }
        }

        private void NLEditForm_Click(object sender, EventArgs e)
        {
            RemoveFocus();
        }

        private void NLEditForm_Resize(object sender, EventArgs e)
        {
            MoveControlsOnFormResize();
            fCurRenderer.EnsureScreenPosInLevel();
            this.pic_Level.Image = fCurRenderer.CreateLevelImage();
        }

        private void tabLvlProperties_Click(object sender, EventArgs e)
        {
            RemoveFocus();
        }

        /* -----------------------------------------------------------
         *              Menu Items
         * ----------------------------------------------------------- */


        private void newToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CreateNewLevelAndRenderer();
        }

        private void loadToolStripMenuItem_Click(object sender, EventArgs e)
        {
            LoadNewLevel();
        }

        private void saveAsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveInNewFileLevel();
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveLevel();
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void clearPhysicsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            fCurRenderer.ChangeIsClearPhsyics();
            this.pic_Level.Image = fCurRenderer.CreateLevelImage();
        }

        private void terrainRenderingToolStripMenuItem_Click(object sender, EventArgs e)
        {
            fCurRenderer.ChangeIsTerrainLayer();
            this.pic_Level.Image = fCurRenderer.CombineLayers();
        }

        private void objectRenderingToolStripMenuItem_Click(object sender, EventArgs e)
        {
            fCurRenderer.ChangeIsObjectLayer();
            this.pic_Level.Image = fCurRenderer.CombineLayers();
        }

        private void triggerAreasToolStripMenuItem_Click(object sender, EventArgs e)
        {
            fCurRenderer.ChangeIsTriggerLayer();
            this.pic_Level.Image = fCurRenderer.CombineLayers();
        }

        private void screenStartToolStripMenuItem_Click(object sender, EventArgs e)
        {
            fCurRenderer.ChangeIsScreenStart();
            this.pic_Level.Image = fCurRenderer.CombineLayers();
        }

        private void backgroundToolStripMenuItem_Click(object sender, EventArgs e)
        {
            fCurRenderer.ChangeIsBackgroundLayer();
            this.pic_Level.Image = fCurRenderer.CombineLayers();
        }

        private void undoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            UndoLastChange();
        }

        private void redoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CancelLastUndo();
        }

        private void deleteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DeleteSelectedPieces();
        }

        private void insertToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AddPiecesFromMemory();
        }

        private void copyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CopySelectedPieces();
        }

        private void playLevelToolStripMenuItem_Click(object sender, EventArgs e)
        {
            PlaytestLevel();
        }

        private void validateLevelToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ValidateLevel();
        }

        private void hotkeysToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DisplayHotkeyForm();
        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DisplayVersionForm();
        }

        /* -----------------------------------------------------------
         *              Global Level Info Tab
         * ----------------------------------------------------------- */

        private void combo_MainStyle_TextChanged(object sender, EventArgs e)
        {
            Style NewStyle = ValidateStyleName(this.combo_MainStyle.Text);

            if (NewStyle == null || CurLevel == null || NewStyle == CurLevel.MainStyle) return;

            // Load new style into PictureBoxes
            CurLevel.MainStyle = NewStyle;
            UpdateBackgroundComboItems();
            UpdateBackgroundImage();
            this.pic_Level.Image = fCurRenderer.CombineLayers();
        }

        private void num_Lvl_SizeX_ValueChanged(object sender, EventArgs e)
        {
            // Adapt max start position
            num_Lvl_StartX.Maximum = num_Lvl_SizeX.Value - 320;
            
            fCurLevel.Width = (int)num_Lvl_SizeX.Value;
            fCurLevel.StartPosX = (int)num_Lvl_StartX.Value;

            // Update screen position and render level
            fCurRenderer.ChangeZoom(0); 
            this.pic_Level.Image = fCurRenderer.CreateLevelImage();
        }

        private void num_Lvl_SizeY_ValueChanged(object sender, EventArgs e)
        {
            // Adapt max start position
            num_Lvl_StartY.Maximum = num_Lvl_SizeY.Value - 160;

            fCurLevel.Height = (int)num_Lvl_SizeY.Value;
            fCurLevel.StartPosY = (int)num_Lvl_StartY.Value;

            // Update screen position and render level
            fCurRenderer.ChangeZoom(0);
            this.pic_Level.Image = fCurRenderer.CreateLevelImage();
        }


        private void combo_Background_TextChanged(object sender, EventArgs e)
        {
            CurLevel.BackgroundKey = this.combo_Background.Text;
            UpdateBackgroundImage();
            this.pic_Level.Image = fCurRenderer.CombineLayers();
        }


        /* -----------------------------------------------------------
         *              Piece Info Tab
         * ----------------------------------------------------------- */

        private void but_RotatePieces_Click(object sender, EventArgs e)
        {
            if (!but_RotatePieces.IsRepeatedAction || fStopWatchMouse.ElapsedMilliseconds > but_RotatePieces.Interval / 2)
            {
                fStopWatchMouse.Restart();
                RotateLevelPieces();
            }
        }

        private void but_RotatePieces_MouseUp(object sender, MouseEventArgs e)
        {
            RemoveFocus();
        }

        private void but_InvertPieces_Click(object sender, EventArgs e)
        {
            if (!but_InvertPieces.IsRepeatedAction || fStopWatchMouse.ElapsedMilliseconds > but_InvertPieces.Interval / 2)
            {
                fStopWatchMouse.Restart();
                InvertLevelPieces();
            }
        }

        private void but_InvertPieces_MouseUp(object sender, MouseEventArgs e)
        {
            RemoveFocus();
        }

        private void but_FlipPieces_Click(object sender, EventArgs e)
        {
            if (!but_FlipPieces.IsRepeatedAction || fStopWatchMouse.ElapsedMilliseconds > but_FlipPieces.Interval / 2)
            {
                fStopWatchMouse.Restart();
                FlipLevelPieces();
            }
        }

        private void but_FlipPieces_MouseUp(object sender, MouseEventArgs e)
        {
            RemoveFocus();
        }


        private void but_MoveFront_Click(object sender, EventArgs e)
        {
            if (!but_MoveFront.IsRepeatedAction || fStopWatchMouse.ElapsedMilliseconds > but_MoveFront.Interval / 2)
            {
                fStopWatchMouse.Restart();
                MovePieceIndex(true, false);
            }
        }

        private void but_MoveFront_MouseUp(object sender, MouseEventArgs e)
        {
            RemoveFocus();
        }

        private void but_MoveBack_Click(object sender, EventArgs e)
        {
            if (!but_MoveBack.IsRepeatedAction || fStopWatchMouse.ElapsedMilliseconds > but_MoveBack.Interval / 2)
            {
                fStopWatchMouse.Restart();
                MovePieceIndex(false, false);
            }
        }

        private void but_MoveBack_MouseUp(object sender, MouseEventArgs e)
        {
            RemoveFocus();
        }

        private void but_MoveFrontOne_Click(object sender, EventArgs e)
        {
            if (!but_MoveFrontOne.IsRepeatedAction || fStopWatchMouse.ElapsedMilliseconds > but_MoveFrontOne.Interval / 2)
            {
                fStopWatchMouse.Restart();
                MovePieceIndex(true, true);
            }
        }

        private void but_MoveFrontOne_MouseUp(object sender, MouseEventArgs e)
        {
            RemoveFocus();
        }

        private void but_MoveBackOne_Click(object sender, EventArgs e)
        {
            if (!but_MoveBackOne.IsRepeatedAction || fStopWatchMouse.ElapsedMilliseconds > but_MoveBackOne.Interval / 2)
            {
                fStopWatchMouse.Restart();
                MovePieceIndex(false, true);
            }
        }

        private void but_MoveBackOne_MouseUp(object sender, MouseEventArgs e)
        {
            RemoveFocus();
        }


        private void check_Pieces_Erase_CheckedChanged(object sender, EventArgs e)
        {
            bool IsChecked = (check_Pieces_Erase.CheckState == CheckState.Checked);
            SetErase(IsChecked);
            RemoveFocus();
        }

        private void check_Pieces_NoOv_CheckedChanged(object sender, EventArgs e)
        {
            bool IsChecked = (check_Pieces_NoOv.CheckState == CheckState.Checked);
            SetNoOverwrite(IsChecked);
            RemoveFocus();
        }

        private void check_Pieces_OnlyOnTerrain_CheckedChanged(object sender, EventArgs e)
        {
            bool IsChecked = (check_Pieces_OnlyOnTerrain.CheckState == CheckState.Checked);
            SetOnlyOnTerrain(IsChecked);
            RemoveFocus();
        }

        private void check_Pieces_OneWay_CheckedChanged(object sender, EventArgs e)
        {
            bool IsChecked = (check_Pieces_OneWay.CheckState == CheckState.Checked);
            SetOneWay(IsChecked);
            RemoveFocus();
        }

        private void check_Piece_Skill_CheckedChanged(object sender, EventArgs e)
        {
            int Skill = fcheckSkillFlagList.FindIndex(check => check.Equals((CheckBox)sender));
            bool IsChecked = ((CheckBox)sender).CheckState == CheckState.Checked;
            SetSkillForObjects(Skill, IsChecked);
            RemoveFocus();
            this.pic_Level.Image = fCurRenderer.CreateLevelImage();
        }

        private void num_Resize_Width_ValueChanged(object sender, EventArgs e)
        {
            int NewWidth = (int)num_Resize_Width.Value;
            fCurLevel.SelectionList()
                     .FindAll(item => item is GadgetPiece)
                     .ForEach(obj => (obj as GadgetPiece).SetSpecWidth(NewWidth));
            this.pic_Level.Image = fCurRenderer.CreateLevelImage();
        }

        private void num_Resize_Height_ValueChanged(object sender, EventArgs e)
        {
            int NewHeight = (int)num_Resize_Height.Value;
            fCurLevel.SelectionList()
                     .FindAll(item => item is GadgetPiece)
                     .ForEach(obj => (obj as GadgetPiece).SetSpecHeight(NewHeight));
            this.pic_Level.Image = fCurRenderer.CreateLevelImage();
        }

        private void but_PairTeleporter_Click(object sender, EventArgs e)
        {
            PairTeleporters();
            RemoveFocus();
        }


        /* -----------------------------------------------------------
         *              Piece Selection
         * ----------------------------------------------------------- */

        private void combo_PieceStyle_TextChanged(object sender, EventArgs e)
        {
            Style NewStyle = ValidateStyleName(this.combo_PieceStyle.Text);

            if (NewStyle == null || NewStyle == fPieceCurStyle) return;

            // Load new style into PictureBoxes
            fPieceCurStyle = NewStyle;
            fPieceStartIndex = 0;
            LoadPiecesIntoPictureBox();
        }

        private void combo_PieceStyle_Leave(object sender, EventArgs e)
        {
            // Check whether to delete all pieces due to wrong style name
            Style NewStyle = ValidateStyleName(this.combo_PieceStyle.Text);

            if (NewStyle == null)
            {
                fPieceCurStyle = null;
                fPieceStartIndex = 0;
                ClearPiecesPictureBox();           
            }
        }

        private void but_PieceTerrObj_Click(object sender, EventArgs e)
        {
            ChangeObjTerrPieceDisplay();
            RemoveFocus();
        }

        private void but_PieceLeft_MouseUp(object sender, MouseEventArgs e)
        {
            RemoveFocus();
        }

        private void but_PieceLeft_Click(object sender, EventArgs e)
        {
            if (!but_PieceLeft.IsRepeatedAction || fStopWatchMouse.ElapsedMilliseconds > but_PieceLeft.Interval / 2)
            {
                fStopWatchMouse.Restart();
                
                int Movement;

                if (!(e is MouseEventArgs))
                {
                    Movement = -1;
                }
                else if ((e as MouseEventArgs).Button == MouseButtons.Right)
                {
                    Movement = -fpicPieceList.Count;
                }
                else
                {
                    Movement = -1;
                }

                MoveTerrPieceSelection(Movement);
            }
        }

        private void but_PieceRight_MouseUp(object sender, MouseEventArgs e)
        {
            RemoveFocus();
        }

        private void but_PieceRight_Click(object sender, EventArgs e)
        {
            if (!but_PieceRight.IsRepeatedAction || fStopWatchMouse.ElapsedMilliseconds > but_PieceRight.Interval / 2)
            {
                fStopWatchMouse.Restart();

                int Movement;

                if (!(e is MouseEventArgs))
                {
                    Movement = 1;
                }
                else if ((e as MouseEventArgs).Button == MouseButtons.Right)
                {
                    Movement = fpicPieceList.Count;
                }
                else
                {
                    Movement = 1;
                }

                MoveTerrPieceSelection(Movement);
            }
        }

        private void picPieces_Click(object sender, EventArgs e)
        {
            int PicIndex = fpicPieceList.FindIndex(pic => pic.Equals(sender));

            System.Diagnostics.Debug.Assert(PicIndex != -1, "PicBox not found in ´fpicPieceList.");

            AddNewPieceToLevel(PicIndex);
            UpdateFlagsForPieceActions();
            RemoveFocus();
        }


        /* -----------------------------------------------------------
         *              Direct Key and Mouse imput
         * ----------------------------------------------------------- */

        private void NLEditForm_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.ShiftKey: fIsShiftPressed = true; break;
                case Keys.ControlKey: fIsCtrlPressed = true; break;
                case Keys.Menu: fIsAltPressed = true; break;
            }

            
            if (fStopWatchKey.ElapsedMilliseconds < 50) return;

            // The main key-handling routine
            if (e.KeyCode == Keys.Escape || (e.Alt && e.KeyCode == Keys.F4))
            {
                Application.Exit();
            }
            else if (e.Control && e.KeyCode == Keys.N)
            {
                CreateNewLevelAndRenderer();
            }
            else if (e.Control && e.KeyCode == Keys.O)
            {
                LoadNewLevel();
            }
            else if (e.Control && e.Shift && e.KeyCode == Keys.S)
            {
                SaveInNewFileLevel();
            }
            else if (e.Control && e.KeyCode == Keys.S)
            {
                SaveLevel();
            }
            else if (e.Control && e.KeyCode == Keys.Z)
            {
                UndoLastChange();
            }
            else if (e.Control && e.KeyCode == Keys.Y)
            {
                CancelLastUndo();
            }
            else if (e.Control && e.KeyCode == Keys.X)
            {
                DeleteSelectedPieces();
            }
            else if (e.Control && e.KeyCode == Keys.V)
            {
                AddPiecesFromMemory();
            }
            else if (e.Control && e.KeyCode == Keys.C)
            {
                CopySelectedPieces();
            }
            else if (e.KeyCode == Keys.F1)
            {
                clearPhysicsToolStripMenuItem_Click(null, null);
            }
            else if (e.KeyCode == Keys.F2)
            {
                terrainRenderingToolStripMenuItem_Click(null, null);
            }
            else if (e.KeyCode == Keys.F3)
            {
                objectRenderingToolStripMenuItem_Click(null, null);
            }
            else if (e.KeyCode == Keys.F4)
            {
                triggerAreasToolStripMenuItem_Click(null, null);
            }
            else if (e.KeyCode == Keys.F5)
            {
                screenStartToolStripMenuItem_Click(null, null);
            }
            else if (e.KeyCode == Keys.F6)
            {
                backgroundToolStripMenuItem_Click(null, null);
            }
            else if (e.KeyCode == Keys.F11)
            {
                DisplayHotkeyForm();
            }
            else if (e.KeyCode == Keys.F12)
            {
                PlaytestLevel();
            }
            else if (e.Shift && e.KeyCode == Keys.Left)
            {
                MoveTerrPieceSelection(-1);
            }
            else if (e.Shift && e.KeyCode == Keys.Right)
            {
                MoveTerrPieceSelection(1);
            }
            else if (e.Shift && e.KeyCode == Keys.Up)
            {
                ChangeNewPieceStyleSelection(-1);
            }
            else if (e.Shift && e.KeyCode == Keys.Down)
            {
                ChangeNewPieceStyleSelection(1);
            }
            else if (e.Shift && e.KeyCode == Keys.Space)
            {
                ChangeObjTerrPieceDisplay();
            }
            else if (e.Shift && e.KeyCode >= Keys.D0 && e.KeyCode <= Keys.D9)
            {
                int KeyValue = e.KeyValue - (int)Keys.D0;
                if (KeyValue == 0) KeyValue = 10;

                if (picPieceList.Count >= KeyValue)
                {
                    AddNewPieceToLevel(KeyValue - 1);
                    UpdateFlagsForPieceActions();
                }
            }
            else if (e.Shift && e.KeyCode >= Keys.NumPad0 && e.KeyCode <= Keys.NumPad9)
            {
                int KeyValue = e.KeyValue - (int)Keys.NumPad0;
                if (KeyValue == 0) KeyValue = 10;

                if (picPieceList.Count >= KeyValue)
                {
                    AddNewPieceToLevel(KeyValue - 1);
                    UpdateFlagsForPieceActions();
                }
            }
            else if (e.Control && e.KeyCode == Keys.Left)
            {
                MoveLevelPieces(C.DIR.W, 8);
            }
            else if (e.Control && e.KeyCode == Keys.Right)
            {
                MoveLevelPieces(C.DIR.E, 8);
            }
            else if (e.Control && e.KeyCode == Keys.Up)
            {
                MoveLevelPieces(C.DIR.N, 8);
            }
            else if (e.Control && e.KeyCode == Keys.Down)
            {
                MoveLevelPieces(C.DIR.S, 8);
            }
            else if (e.KeyCode == Keys.Left)
            {
                MoveLevelPieces(C.DIR.W);
            }
            else if (e.KeyCode == Keys.Right)
            {
                MoveLevelPieces(C.DIR.E);
            }
            else if (e.KeyCode == Keys.Up)
            {
                MoveLevelPieces(C.DIR.N);
            }
            else if (e.KeyCode == Keys.Down)
            {
                MoveLevelPieces(C.DIR.S);
            }
            else if (e.KeyCode == Keys.Enter)
            {
                RemoveFocus();
            }
            else
            {
                return; // and don't restart the StopWatch
            }

            fStopWatchKey.Restart();
        }

        private void NLEditForm_KeyUp(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.ShiftKey: fIsShiftPressed = false; break;
                case Keys.ControlKey: fIsCtrlPressed = false; break;
                case Keys.Menu: fIsAltPressed = false; break;
            }

            if (e.KeyCode.In(Keys.Right, Keys.Left, Keys.Up, Keys.Down))
            {
                SaveChangesToOldLevelList();
            }
        }

        private void NLEditForm_MouseWheel(object sender, MouseEventArgs e)
        {
            int Movement = e.Delta / SystemInformation.MouseWheelScrollDelta;
            Point MousePosRelPicLevel = pic_Level.PointToClient(this.PointToScreen(e.Location));
            Rectangle PicLevelRect = new Rectangle(0, 0, pic_Level.Width, pic_Level.Height);

            if (PicLevelRect.Contains(MousePosRelPicLevel))
            {
                fCurRenderer.ChangeZoom(Movement > 0 ? 1 : -1, MousePosRelPicLevel);
            }
            else
            {
                fCurRenderer.ChangeZoom(Movement > 0 ? 1 : -1);
            }

            // Update level image
            this.pic_Level.Image = fCurRenderer.CombineLayers();
        }

        private void pic_Level_MouseDown(object sender, MouseEventArgs e)
        {
            fMouseButtonPressed = e.Button;
            fCurRenderer.MouseStartPos = e.Location;
            fCurRenderer.MouseCurPos = e.Location;
            fStopWatchMouse.Restart();

            bool HasSelectedPieceAtPos = fCurRenderer.GetMousePosInLevel() != null 
                                       && fCurLevel.HasSelectionAtPos((Point)fCurRenderer.GetMousePosInLevel());

            if (e.Button == MouseButtons.Right)
            {
                fCurRenderer.MouseDragAction = C.DragActions.MoveEditorPos;
            }
            else if (HasSelectedPieceAtPos && !fIsAltPressed && !fIsCtrlPressed && !fIsShiftPressed)
            {
                fCurRenderer.MouseDragAction = C.DragActions.DragPieces;
            }
            else
            {
                fCurRenderer.MouseDragAction = C.DragActions.SelectArea;
            }
        }

        private void pic_Level_MouseMove(object sender, MouseEventArgs e)
        {
            if (fCurRenderer.MouseStartPos == null) return;

            fCurRenderer.MouseCurPos = e.Location;

            switch (fCurRenderer.MouseDragAction)
            {
                case C.DragActions.SelectArea:
                case C.DragActions.MoveEditorPos:
                    {
                        this.pic_Level.Image = fCurRenderer.CombineLayers();
                        break;
                    }
                case C.DragActions.DragPieces:
                    {
                        DragSelectedPieces();
                        fCurRenderer.MouseStartPos = fCurRenderer.MouseCurPos;
                        fCurRenderer.MouseCurPos = null;
                        break;
                    }
            }
            pic_Level.Refresh();
        }

        private void pic_Level_MouseUp(object sender, MouseEventArgs e)
        {
            fCurRenderer.MouseCurPos = e.Location;

            switch (fCurRenderer.MouseDragAction)
            {
                case C.DragActions.SelectArea:
                    {
                        if (fStopWatchMouse.ElapsedMilliseconds < 200) // usual click takes <100ms
                        {
                            LevelSelectSinglePiece();
                        }
                        else
                        {
                            LevelSelectAreaPieces();
                        }
                        break;
                    }
                case C.DragActions.MoveEditorPos:
                    {
                        fCurRenderer.UpdateScreenPos();
                        break;
                    }
                case C.DragActions.DragPieces:
                    {
                        DragSelectedPieces();
                        SaveChangesToOldLevelList();
                        break;
                    }
            }

            // Delete mouse selection area...
            fCurRenderer.MouseStartPos = null;
            fCurRenderer.MouseCurPos = null;
            fCurRenderer.MouseDragAction = C.DragActions.Null;
            // ...before updating the level image
            this.pic_Level.Image = fCurRenderer.CreateLevelImage();
            UpdateFlagsForPieceActions();

            fMouseButtonPressed = null;
            RemoveFocus();
        }




    }
}
