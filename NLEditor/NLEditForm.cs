using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
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

            this.MouseWheel += new MouseEventHandler(NLEditForm_MouseWheel);

            picPieceList = new List<PictureBox> 
                { 
                    this.picPiece0, this.picPiece1, this.picPiece2, this.picPiece3,
                    this.picPiece4, this.picPiece5, this.picPiece6, this.picPiece7 
                };

            checkboxesSkillFlags = new Dictionary<C.Skill, CheckBox>()
                {
                    { C.Skill.Climber, this.check_Piece_Climber }, { C.Skill.Floater, this.check_Piece_Floater },
                    { C.Skill.Bomber, this.check_Piece_Exploder }, { C.Skill.Blocker, this.check_Piece_Blocker },
                    { C.Skill.Builder, this.check_Piece_Builder }, { C.Skill.Basher, this.check_Piece_Basher },
                    { C.Skill.Miner, this.check_Piece_Miner }, { C.Skill.Digger, this.check_Piece_Digger },
                    { C.Skill.Walker, this.check_Piece_Walker }, { C.Skill.Swimmer, this.check_Piece_Swimmer },
                    { C.Skill.Glider, this.check_Piece_Glider }, { C.Skill.Disarmer, this.check_Piece_Disarmer },
                    { C.Skill.Stoner, this.check_Piece_Stoner }, { C.Skill.Platformer, this.check_Piece_Platformer },
                    { C.Skill.Stacker, this.check_Piece_Stacker }, { C.Skill.Cloner, this.check_Piece_Cloner },
                    { C.Skill.Zombie, this.check_Piece_Zombie }
                };

            numericsSkillSet = new Dictionary<C.Skill, NumericUpDown>()
                {
                    { C.Skill.Climber, this.num_Ski_Climber }, { C.Skill.Floater, this.num_Ski_Floater },
                    { C.Skill.Bomber, this.num_Ski_Exploder }, { C.Skill.Blocker, this.num_Ski_Blocker },
                    { C.Skill.Builder, this.num_Ski_Builder }, { C.Skill.Basher, this.num_Ski_Basher },
                    { C.Skill.Miner, this.num_Ski_Miner }, { C.Skill.Digger, this.num_Ski_Digger },
                    { C.Skill.Walker, this.num_Ski_Walker }, { C.Skill.Swimmer, this.num_Ski_Swimmer },
                    { C.Skill.Glider, this.num_Ski_Glider }, { C.Skill.Disarmer, this.num_Ski_Disarmer },
                    { C.Skill.Stoner, this.num_Ski_Stoner }, { C.Skill.Platformer, this.num_Ski_Platformer },
                    { C.Skill.Stacker, this.num_Ski_Stacker }, { C.Skill.Cloner, this.num_Ski_Cloner }
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

            pieceStartIndex = 0;
            pieceDoDisplayObject = false;
            pieceCurStyle = ValidateStyleName(this.combo_PieceStyle.SelectedItem.ToString());
            LoadPiecesIntoPictureBox();

            stopWatchKey = new Stopwatch();
            stopWatchKey.Start();
            stopWatchMouse = new Stopwatch();
            stopWatchMouse.Start();

            mouseButtonPressed = null;
        }

        Dictionary<C.Skill, CheckBox> checkboxesSkillFlags;
        Dictionary<C.Skill, NumericUpDown> numericsSkillSet;

        public List<PictureBox> picPieceList { get; private set; }
        Style pieceCurStyle;
        int pieceStartIndex;
        bool pieceDoDisplayObject;

        public Level CurLevel { get; private set; }
        public List<Style> StyleList { get; private set; }
        Renderer curRenderer;
        List<Level> oldLevelList;
        int curOldLevelIndex;
        List<LevelPiece> oldSelectedList;
        Level lastSavedLevel;

        Stopwatch stopWatchKey;
        Stopwatch stopWatchMouse;
        MouseButtons? mouseButtonPressed;

        bool isShiftPressed;
        bool isCtrlPressed;
        bool isAltPressed;
        

        private void NLEditForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            Utility.DeleteFile(C.AppPathTempLevel);
            Utility.DeleteFile(System.IO.Path.ChangeExtension(C.AppPathTempLevel, ".nxsv"));
            
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
            curRenderer.EnsureScreenPosInLevel();
            this.pic_Level.Image = curRenderer.CreateLevelImage();
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
            curRenderer.ChangeIsClearPhsyics();
            this.pic_Level.Image = curRenderer.CreateLevelImage();
        }

        private void terrainRenderingToolStripMenuItem_Click(object sender, EventArgs e)
        {
            curRenderer.ChangeIsTerrainLayer();
            this.pic_Level.Image = curRenderer.CombineLayers();
        }

        private void objectRenderingToolStripMenuItem_Click(object sender, EventArgs e)
        {
            curRenderer.ChangeIsObjectLayer();
            this.pic_Level.Image = curRenderer.CombineLayers();
        }

        private void triggerAreasToolStripMenuItem_Click(object sender, EventArgs e)
        {
            curRenderer.ChangeIsTriggerLayer();
            this.pic_Level.Image = curRenderer.CombineLayers();
        }

        private void screenStartToolStripMenuItem_Click(object sender, EventArgs e)
        {
            curRenderer.ChangeIsScreenStart();
            this.pic_Level.Image = curRenderer.CombineLayers();
        }

        private void backgroundToolStripMenuItem_Click(object sender, EventArgs e)
        {
            curRenderer.ChangeIsBackgroundLayer();
            this.pic_Level.Image = curRenderer.CombineLayers();
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
            Style newStyle = ValidateStyleName(this.combo_MainStyle.Text);

            if (newStyle == null || CurLevel == null || newStyle == CurLevel.MainStyle) return;

            // Load new style into PictureBoxes
            CurLevel.MainStyle = newStyle;
            UpdateBackgroundComboItems();
            UpdateBackgroundImage();
            this.pic_Level.Image = curRenderer.CombineLayers();
        }

        private void num_Lvl_SizeX_ValueChanged(object sender, EventArgs e)
        {
            // Adapt max start position
            num_Lvl_StartX.Maximum = num_Lvl_SizeX.Value - 320;
            
            CurLevel.Width = (int)num_Lvl_SizeX.Value;
            CurLevel.StartPosX = (int)num_Lvl_StartX.Value;

            // Update screen position and render level
            curRenderer.ChangeZoom(0); 
            this.pic_Level.Image = curRenderer.CreateLevelImage();
        }

        private void num_Lvl_SizeY_ValueChanged(object sender, EventArgs e)
        {
            // Adapt max start position
            num_Lvl_StartY.Maximum = num_Lvl_SizeY.Value - 160;

            CurLevel.Height = (int)num_Lvl_SizeY.Value;
            CurLevel.StartPosY = (int)num_Lvl_StartY.Value;

            // Update screen position and render level
            curRenderer.ChangeZoom(0);
            this.pic_Level.Image = curRenderer.CreateLevelImage();
        }


        private void combo_Background_TextChanged(object sender, EventArgs e)
        {
            CurLevel.BackgroundKey = this.combo_Background.Text;
            UpdateBackgroundImage();
            this.pic_Level.Image = curRenderer.CombineLayers();
        }


        /* -----------------------------------------------------------
         *              Piece Info Tab
         * ----------------------------------------------------------- */

        private void but_RotatePieces_Click(object sender, EventArgs e)
        {
            if (!but_RotatePieces.IsRepeatedAction || stopWatchMouse.ElapsedMilliseconds > but_RotatePieces.Interval() / 2)
            {
                stopWatchMouse.Restart();
                RotateLevelPieces();
            }
        }

        private void but_RotatePieces_MouseUp(object sender, MouseEventArgs e)
        {
            RemoveFocus();
        }

        private void but_InvertPieces_Click(object sender, EventArgs e)
        {
            if (!but_InvertPieces.IsRepeatedAction || stopWatchMouse.ElapsedMilliseconds > but_InvertPieces.Interval() / 2)
            {
                stopWatchMouse.Restart();
                InvertLevelPieces();
            }
        }

        private void but_InvertPieces_MouseUp(object sender, MouseEventArgs e)
        {
            RemoveFocus();
        }

        private void but_FlipPieces_Click(object sender, EventArgs e)
        {
            if (!but_FlipPieces.IsRepeatedAction || stopWatchMouse.ElapsedMilliseconds > but_FlipPieces.Interval() / 2)
            {
                stopWatchMouse.Restart();
                FlipLevelPieces();
            }
        }

        private void but_FlipPieces_MouseUp(object sender, MouseEventArgs e)
        {
            RemoveFocus();
        }


        private void but_MoveFront_Click(object sender, EventArgs e)
        {
            if (!but_MoveFront.IsRepeatedAction || stopWatchMouse.ElapsedMilliseconds > but_MoveFront.Interval() / 2)
            {
                stopWatchMouse.Restart();
                MovePieceIndex(true, false);
            }
        }

        private void but_MoveFront_MouseUp(object sender, MouseEventArgs e)
        {
            RemoveFocus();
        }

        private void but_MoveBack_Click(object sender, EventArgs e)
        {
            if (!but_MoveBack.IsRepeatedAction || stopWatchMouse.ElapsedMilliseconds > but_MoveBack.Interval() / 2)
            {
                stopWatchMouse.Restart();
                MovePieceIndex(false, false);
            }
        }

        private void but_MoveBack_MouseUp(object sender, MouseEventArgs e)
        {
            RemoveFocus();
        }

        private void but_MoveFrontOne_Click(object sender, EventArgs e)
        {
            if (!but_MoveFrontOne.IsRepeatedAction || stopWatchMouse.ElapsedMilliseconds > but_MoveFrontOne.Interval() / 2)
            {
                stopWatchMouse.Restart();
                MovePieceIndex(true, true);
            }
        }

        private void but_MoveFrontOne_MouseUp(object sender, MouseEventArgs e)
        {
            RemoveFocus();
        }

        private void but_MoveBackOne_Click(object sender, EventArgs e)
        {
            if (!but_MoveBackOne.IsRepeatedAction || stopWatchMouse.ElapsedMilliseconds > but_MoveBackOne.Interval() / 2)
            {
                stopWatchMouse.Restart();
                MovePieceIndex(false, true);
            }
        }

        private void but_MoveBackOne_MouseUp(object sender, MouseEventArgs e)
        {
            RemoveFocus();
        }


        private void check_Pieces_Erase_CheckedChanged(object sender, EventArgs e)
        {
            bool isChecked = (check_Pieces_Erase.CheckState == CheckState.Checked);
            SetErase(isChecked);
            RemoveFocus();
        }

        private void check_Pieces_NoOv_CheckedChanged(object sender, EventArgs e)
        {
            bool isChecked = (check_Pieces_NoOv.CheckState == CheckState.Checked);
            SetNoOverwrite(isChecked);
            RemoveFocus();
        }

        private void check_Pieces_OnlyOnTerrain_CheckedChanged(object sender, EventArgs e)
        {
            bool isChecked = (check_Pieces_OnlyOnTerrain.CheckState == CheckState.Checked);
            SetOnlyOnTerrain(isChecked);
            RemoveFocus();
        }

        private void check_Pieces_OneWay_CheckedChanged(object sender, EventArgs e)
        {
            bool isChecked = (check_Pieces_OneWay.CheckState == CheckState.Checked);
            SetOneWay(isChecked);
            RemoveFocus();
        }

        private void check_Piece_Skill_CheckedChanged(object sender, EventArgs e)
        {
            C.Skill skill = checkboxesSkillFlags.First(check => check.Value.Equals((CheckBox)sender)).Key;
            bool isChecked = ((CheckBox)sender).CheckState == CheckState.Checked;
            SetSkillForObjects(skill, isChecked);
            RemoveFocus();
            this.pic_Level.Image = curRenderer.CreateLevelImage();
        }

        private void num_Resize_Width_ValueChanged(object sender, EventArgs e)
        {
            int newWidth = (int)num_Resize_Width.Value;
            CurLevel.SelectionList()
                    .FindAll(item => item is GadgetPiece)
                    .ForEach(obj => (obj as GadgetPiece).SpecWidth = newWidth);
            this.pic_Level.Image = curRenderer.CreateLevelImage();
        }

        private void num_Resize_Height_ValueChanged(object sender, EventArgs e)
        {
            int newHeight = (int)num_Resize_Height.Value;
            CurLevel.SelectionList()
                    .FindAll(item => item is GadgetPiece)
                    .ForEach(obj => (obj as GadgetPiece).SpecHeight = newHeight);
            this.pic_Level.Image = curRenderer.CreateLevelImage();
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
            Style newStyle = ValidateStyleName(this.combo_PieceStyle.Text);

            if (newStyle == null || newStyle == pieceCurStyle) return;

            // Load new style into PictureBoxes
            pieceCurStyle = newStyle;
            pieceStartIndex = 0;
            LoadPiecesIntoPictureBox();
        }

        private void combo_PieceStyle_Leave(object sender, EventArgs e)
        {
            // Check whether to delete all pieces due to wrong style name
            Style newStyle = ValidateStyleName(this.combo_PieceStyle.Text);

            if (newStyle == null)
            {
                pieceCurStyle = null;
                pieceStartIndex = 0;
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
            if (!but_PieceLeft.IsRepeatedAction || stopWatchMouse.ElapsedMilliseconds > but_PieceLeft.Interval() / 2)
            {
                stopWatchMouse.Restart();
                
                int movement;
                if (!(e is MouseEventArgs))
                {
                    movement = -1;
                }
                else if ((e as MouseEventArgs).Button == MouseButtons.Right)
                {
                    movement = -picPieceList.Count;
                }
                else
                {
                    movement = -1;
                }

                MoveTerrPieceSelection(movement);
            }
        }

        private void but_PieceRight_MouseUp(object sender, MouseEventArgs e)
        {
            RemoveFocus();
        }

        private void but_PieceRight_Click(object sender, EventArgs e)
        {
            if (!but_PieceRight.IsRepeatedAction || stopWatchMouse.ElapsedMilliseconds > but_PieceRight.Interval() / 2)
            {
                stopWatchMouse.Restart();

                int movement;
                if (!(e is MouseEventArgs))
                {
                    movement = 1;
                }
                else if ((e as MouseEventArgs).Button == MouseButtons.Right)
                {
                    movement = picPieceList.Count;
                }
                else
                {
                    movement = 1;
                }

                MoveTerrPieceSelection(movement);
            }
        }

        private void picPieces_Click(object sender, EventArgs e)
        {
            int picIndex = picPieceList.FindIndex(pic => pic.Equals(sender));
            Debug.Assert(picIndex != -1, "PicBox not found in picPieceList.");

            AddNewPieceToLevel(picIndex);
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
                case Keys.ShiftKey: isShiftPressed = true; break;
                case Keys.ControlKey: isCtrlPressed = true; break;
                case Keys.Menu: isAltPressed = true; break;
            }

            
            if (stopWatchKey.ElapsedMilliseconds < 50) return;

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
                RemoveFocus();
                MoveTerrPieceSelection(-1);
            }
            else if (e.Shift && e.KeyCode == Keys.Right)
            {
                RemoveFocus();
                MoveTerrPieceSelection(1);
            }
            else if (e.Shift && e.KeyCode == Keys.Up)
            {
                RemoveFocus();
                ChangeNewPieceStyleSelection(-1);
            }
            else if (e.Shift && e.KeyCode == Keys.Down)
            {
                RemoveFocus();
                ChangeNewPieceStyleSelection(1);
            }
            else if (e.Shift && e.KeyCode == Keys.Space)
            {
                RemoveFocus();
                ChangeObjTerrPieceDisplay();
            }
            /* --------------------------------------------------------------------
             * ONLY USE THE FOLLOWING KEYS IF FOCUS IS NOT ON ONE OF THE TEXTBOXES
             * --------------------------------------------------------------------*/
            else if (this.ActiveControl != this.txt_Focus)
            {
                return; // and don't restart the StopWatch
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
            else if (e.Control && e.KeyCode >= Keys.D0 && e.KeyCode <= Keys.D9)
            {
                int keyValue = e.KeyValue - (int)Keys.D0;
                if (keyValue == 0) keyValue = 10;

                if (picPieceList.Count >= keyValue)
                {
                    AddNewPieceToLevel(keyValue - 1);
                    UpdateFlagsForPieceActions();
                }
            }
            else if (e.Control && e.KeyCode >= Keys.NumPad0 && e.KeyCode <= Keys.NumPad9)
            {
                int keyValue = e.KeyValue - (int)Keys.NumPad0;
                if (keyValue == 0) keyValue = 10;

                if (picPieceList.Count >= keyValue)
                {
                    AddNewPieceToLevel(keyValue - 1);
                    UpdateFlagsForPieceActions();
                }
            }
            else if (e.KeyCode == Keys.Left)
            {
                MoveLevelPieces(C.DIR.W, e.Control ? 8 : 1);
            }
            else if (e.KeyCode == Keys.Right)
            {
                MoveLevelPieces(C.DIR.E, e.Control ? 8 : 1);
            }
            else if (e.KeyCode == Keys.Up)
            {
                MoveLevelPieces(C.DIR.N, e.Control ? 8 : 1);
            }
            else if (e.KeyCode == Keys.Down)
            {
                MoveLevelPieces(C.DIR.S, e.Control ? 8 : 1);
            }
            else if (e.KeyCode == Keys.R)
            {
                RotateLevelPieces();
            }
            else if (e.KeyCode == Keys.E)
            {
                FlipLevelPieces();
            }
            else if (e.KeyCode == Keys.W)
            {
                InvertLevelPieces();
            }
            else if (e.KeyCode == Keys.A)
            {
                check_Pieces_Erase.Checked = !check_Pieces_Erase.Checked;
            }
            else if (e.KeyCode == Keys.S)
            {
                check_Pieces_NoOv.Checked = !check_Pieces_NoOv.Checked;
            }
            else if (e.KeyCode == Keys.D)
            {
                check_Pieces_OnlyOnTerrain.Checked = !check_Pieces_OnlyOnTerrain.Checked;
            }
            else if (e.KeyCode == Keys.F)
            {
                check_Pieces_OneWay.Checked = !check_Pieces_OneWay.Checked;
            }
            else if (e.KeyCode == Keys.Home)
            {
                MovePieceIndex(true, false);
            }
            else if (e.KeyCode == Keys.PageUp)
            {
                MovePieceIndex(true, true);
            }
            else if (e.KeyCode == Keys.PageDown)
            {
                MovePieceIndex(false, true);
            }
            else if (e.KeyCode == Keys.End)
            {
                MovePieceIndex(false, false);
            }
            else if (e.KeyCode == Keys.Enter)
            {
                RemoveFocus();
            }
            else
            {
                return; // and don't restart the StopWatch
            }

            stopWatchKey.Restart();
        }

        private void NLEditForm_KeyUp(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.ShiftKey: isShiftPressed = false; break;
                case Keys.ControlKey: isCtrlPressed = false; break;
                case Keys.Menu: isAltPressed = false; break;
            }

            if (e.KeyCode.In(Keys.Right, Keys.Left, Keys.Up, Keys.Down))
            {
                SaveChangesToOldLevelList();
            }
        }

        private void NLEditForm_MouseWheel(object sender, MouseEventArgs e)
        {
            int movement = e.Delta / SystemInformation.MouseWheelScrollDelta;
            Point mousePosRelPicLevel = pic_Level.PointToClient(this.PointToScreen(e.Location));
            Rectangle picLevelRect = new Rectangle(0, 0, pic_Level.Width, pic_Level.Height);

            if (picLevelRect.Contains(mousePosRelPicLevel))
            {
                curRenderer.ChangeZoom(movement > 0 ? 1 : -1, mousePosRelPicLevel);
            }
            else
            {
                curRenderer.ChangeZoom(movement > 0 ? 1 : -1);
            }

            // Update level image
            this.pic_Level.Image = curRenderer.CombineLayers();
        }

        private void pic_Level_MouseDown(object sender, MouseEventArgs e)
        {
            mouseButtonPressed = e.Button;
            curRenderer.MouseStartPos = e.Location;
            curRenderer.MouseCurPos = e.Location;
            stopWatchMouse.Restart();

            bool hasSelectedPieceAtPos = curRenderer.GetMousePosInLevel() != null 
                                       && CurLevel.HasSelectionAtPos((Point)curRenderer.GetMousePosInLevel());

            if (e.Button == MouseButtons.Right)
            {
                curRenderer.MouseDragAction = C.DragActions.MoveEditorPos;
            }
            else if (hasSelectedPieceAtPos && !isAltPressed && !isCtrlPressed && !isShiftPressed)
            {
                curRenderer.MouseDragAction = C.DragActions.DragPieces;
            }
            else
            {
                curRenderer.MouseDragAction = C.DragActions.SelectArea;
            }
        }

        private void pic_Level_MouseMove(object sender, MouseEventArgs e)
        {
            if (curRenderer.MouseStartPos == null) return;

            curRenderer.MouseCurPos = e.Location;

            switch (curRenderer.MouseDragAction)
            {
                case C.DragActions.SelectArea:
                case C.DragActions.MoveEditorPos:
                    {
                        this.pic_Level.Image = curRenderer.CombineLayers();
                        break;
                    }
                case C.DragActions.DragPieces:
                    {
                        DragSelectedPieces();
                        curRenderer.MouseStartPos = curRenderer.MouseCurPos;
                        curRenderer.MouseCurPos = null;
                        break;
                    }
            }
            pic_Level.Refresh();
        }

        private void pic_Level_MouseUp(object sender, MouseEventArgs e)
        {
            curRenderer.MouseCurPos = e.Location;

            switch (curRenderer.MouseDragAction)
            {
                case C.DragActions.SelectArea:
                    {
                        if (stopWatchMouse.ElapsedMilliseconds < 200) // usual click takes <100ms
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
                        curRenderer.UpdateScreenPos();
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
            curRenderer.MouseStartPos = null;
            curRenderer.MouseCurPos = null;
            curRenderer.MouseDragAction = C.DragActions.Null;
            // ...before updating the level image
            this.pic_Level.Image = curRenderer.CreateLevelImage();
            UpdateFlagsForPieceActions();

            mouseButtonPressed = null;
            RemoveFocus();
        }




    }
}
