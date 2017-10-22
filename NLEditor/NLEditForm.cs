using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using System.Diagnostics;

namespace NLEditor
{
    /// <summary>
    /// Main editor form: Receives user input.
    /// </summary>
    partial class NLEditForm : Form
    {        
        /// <summary>
        /// Initializes all important components and load an empty level.
        /// </summary>
        public NLEditForm()
        {
            InitializeComponent();
            RemoveFocus();
            SetRepeatButtonIntervals();
            SetMusicList();

            C.ScreenSize = new ScreenSize();
            C.ScreenSize.InizializeSettings();

            LoadStylesFromFile.AddInitialImagesToLibrary();

            picPieceList = new List<PictureBox> 
                { 
                    picPiece0, picPiece1, picPiece2, picPiece3,
                    picPiece4, picPiece5, picPiece6, picPiece7 
                };

            checkboxesSkillFlags = new Dictionary<C.Skill, CheckBox>()
                {
                    { C.Skill.Climber, check_Piece_Climber }, { C.Skill.Floater, check_Piece_Floater },
                    { C.Skill.Bomber, check_Piece_Exploder }, { C.Skill.Blocker, check_Piece_Blocker },
                    { C.Skill.Builder, check_Piece_Builder }, { C.Skill.Basher, check_Piece_Basher },
                    { C.Skill.Miner, check_Piece_Miner }, { C.Skill.Digger, check_Piece_Digger },
                    { C.Skill.Walker, check_Piece_Walker }, { C.Skill.Swimmer, check_Piece_Swimmer },
                    { C.Skill.Glider, check_Piece_Glider }, { C.Skill.Disarmer, check_Piece_Disarmer },
                    { C.Skill.Stoner, check_Piece_Stoner }, { C.Skill.Platformer, check_Piece_Platformer },
                    { C.Skill.Stacker, check_Piece_Stacker }, { C.Skill.Cloner, check_Piece_Cloner },
                    { C.Skill.Fencer, check_Piece_Fencer },  { C.Skill.Zombie, check_Piece_Zombie }
                };

            numericsSkillSet = new Dictionary<C.Skill, NumericUpDown>()
                {
                    { C.Skill.Climber, num_Ski_Climber }, { C.Skill.Floater, num_Ski_Floater },
                    { C.Skill.Bomber, num_Ski_Exploder }, { C.Skill.Blocker, num_Ski_Blocker },
                    { C.Skill.Builder, num_Ski_Builder }, { C.Skill.Basher, num_Ski_Basher },
                    { C.Skill.Miner, num_Ski_Miner }, { C.Skill.Digger, num_Ski_Digger },
                    { C.Skill.Walker, num_Ski_Walker }, { C.Skill.Swimmer, num_Ski_Swimmer },
                    { C.Skill.Glider, num_Ski_Glider }, { C.Skill.Disarmer, num_Ski_Disarmer },
                    { C.Skill.Stoner, num_Ski_Stoner }, { C.Skill.Platformer, num_Ski_Platformer },
                    { C.Skill.Stacker, num_Ski_Stacker }, { C.Skill.Cloner, num_Ski_Cloner },
                    { C.Skill.Fencer, num_Ski_Fencer }
                };

            var displayTabItems = new Dictionary<C.DisplayType, ToolStripMenuItem>()
                {
                    { C.DisplayType.Background, backgroundToolStripMenuItem },
                    { C.DisplayType.ClearPhysics, clearPhysicsToolStripMenuItem },
                    { C.DisplayType.Objects, objectRenderingToolStripMenuItem },
                    { C.DisplayType.ScreenStart, screenStartToolStripMenuItem },
                    { C.DisplayType.Terrain, terrainRenderingToolStripMenuItem },
                    { C.DisplayType.Trigger, triggerAreasToolStripMenuItem }
                };
            DisplaySettings.SetMenuTabItems(displayTabItems);

            curSettings = new Settings(this);

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

            InitializeSettings();
            if (!curSettings.UseTooltipBotton) toolTipButton.Active = false;

            pieceStartIndex = 0;
            pieceDoDisplayObject = false;
            try
            {
                pieceCurStyle = ValidateStyleName(combo_PieceStyle.SelectedItem.ToString());
            }
            catch (NullReferenceException)
            {
                pieceCurStyle = null;
            }
            LoadPiecesIntoPictureBox();

            UpdateBackgroundComboItems();

            dragNewPieceTimer = new Timer();
            dragNewPieceTimer.Tick += new EventHandler((object sender, EventArgs e) => UpdateNewPiecePicBox());

            stopWatchKey = new Stopwatch();
            stopWatchKey.Start();
            stopWatchMouse = new Stopwatch();
            stopWatchMouse.Start();

            mouseButtonPressed = null;

            // For our Linux users: Ignore first resize event to default size and don't try to move non-existing components around.
            // But now we want to apply the size informations given by the settings.
            if (curSettings.IsFormMaximized) ClientSize = curSettings.FormSize;
            this.Resize += new EventHandler(NLEditForm_Resize);
            if (curSettings.IsFormMaximized) WindowState = FormWindowState.Maximized;
            else ClientSize = curSettings.FormSize;
        }

        Dictionary<C.Skill, CheckBox> checkboxesSkillFlags;
        Dictionary<C.Skill, NumericUpDown> numericsSkillSet;

        public List<PictureBox> picPieceList { get; private set; }
        Style pieceCurStyle;
        int pieceStartIndex;
        bool pieceDoDisplayObject;

        string dragNewPieceKey;
        Timer dragNewPieceTimer;

        public Level CurLevel { get; private set; }
        public List<Style> StyleList { get; private set; }
        public BackgroundList Backgrounds { get; private set; }
        Renderer curRenderer;
        Settings curSettings;

        List<Level> oldLevelList;
        int curOldLevelIndex;
        List<LevelPiece> clipboard;
        Level lastSavedLevel;

        int gridSize => curSettings.GridSize;
        
        Stopwatch stopWatchKey;
        Stopwatch stopWatchMouse;
        MouseButtons? mouseButtonPressed;

        bool isShiftPressed;
        bool isCtrlPressed;
        bool isAltPressed;
        bool isPPressed;

        private static System.Threading.Mutex mutexMouseDown = new System.Threading.Mutex();
        private static System.Threading.Mutex mutexMouseUp = new System.Threading.Mutex();
        private static System.Threading.Mutex mutexMouseMove = new System.Threading.Mutex();
        private static System.Threading.Mutex mutexMouseWheel = new System.Threading.Mutex();
        private static System.Threading.Mutex mutexKeyDown = new System.Threading.Mutex();


        private void NLEditForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            try
            {
                curSettings.WriteSettingsToFile();

                Utility.DeleteFile(C.AppPathTempLevel);
                Utility.DeleteFile(System.IO.Path.ChangeExtension(C.AppPathTempLevel, ".nxsv"));

                if (e.CloseReason.In(CloseReason.UserClosing, CloseReason.ApplicationExitCall))
                {
                    AskUserWhetherSaveLevel();
                }
            }
            catch (Exception Ex)
            {
                // Log the exception, but we cannot do anything more.
                try
                {
                    Utility.LogException(Ex);
                }
                catch
                {
                    // do nothing - we can't even save a lot entry.
                }
            }
        }

        private void NLEditForm_Click(object sender, EventArgs e)
        {
            RemoveFocus();
        }

        private void NLEditForm_Resize(object sender, EventArgs e)
        {
            // Don't do anything on minimizing the form!
            if (WindowState == FormWindowState.Minimized) return;

            MoveControlsOnFormResize();
            curRenderer.EnsureScreenPosInLevel();
            pic_Level.Image = curRenderer.CreateLevelImage();
            curSettings.SetFormSize();
        }

        private void NLEditForm_Activated(object sender, EventArgs e)
        {
            UpdateIsSystemKeyPressed();
        }

        private void tabLvlProperties_Click(object sender, EventArgs e)
        {
            RemoveFocus();
        }

        private void textbox_Leave(object sender, EventArgs e)
        {
            ReadLevelInfoFromForm();
            SaveChangesToOldLevelList();
        }

        private void toolTipButton_Popup(object sender, PopupEventArgs e)
        {
            if (!curSettings.UseTooltipBotton) toolTipButton.Active = false;
            curSettings.NumTooltipBottonDisplay--;
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
            DisplaySettings.ChangeDisplayed(C.DisplayType.ClearPhysics);
            pic_Level.Image = curRenderer.CreateLevelImage();
        }

        private void terrainRenderingToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DisplaySettings.ChangeDisplayed(C.DisplayType.Terrain);
            pic_Level.Image = curRenderer.CombineLayers();
        }

        private void objectRenderingToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DisplaySettings.ChangeDisplayed(C.DisplayType.Objects);
            pic_Level.Image = curRenderer.CombineLayers();
        }

        private void triggerAreasToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DisplaySettings.ChangeDisplayed(C.DisplayType.Trigger);
            pic_Level.Image = curRenderer.CombineLayers();
        }

        private void screenStartToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DisplaySettings.ChangeDisplayed(C.DisplayType.ScreenStart);
            pic_Level.Image = curRenderer.CombineLayers();
        }

        private void backgroundToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DisplaySettings.ChangeDisplayed(C.DisplayType.Background);
            pic_Level.Image = curRenderer.CombineLayers();
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
            AddFromClipboard(true);
        }

        private void copyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DuplicateSelectedPieces();
        }

        private void groupToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //GroupSelectedPieces();
        }

        private void ungroupToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //UngroupSelectedPieces();
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

        private void settingsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            curSettings.OpenSettingsWindow();
        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DisplayVersionForm();
        }

        /* -----------------------------------------------------------
         *              Scrollbars for pic_Level
         * ----------------------------------------------------------- */

        private void scrollPicLevelHoriz_Scroll(object sender, ScrollEventArgs e)
        {
            curRenderer.ScreenPosX = e.NewValue;
            pic_Level.Image = curRenderer.CombineLayers();
        }

        private void scrollPicLevelVert_Scroll(object sender, ScrollEventArgs e)
        {
            curRenderer.ScreenPosY = e.NewValue;
            pic_Level.Image = curRenderer.CombineLayers();
        }

        /* -----------------------------------------------------------
         *              Global Level Info Tab
         * ----------------------------------------------------------- */

        private void combo_MainStyle_TextChanged(object sender, EventArgs e)
        {
            Style newStyle = ValidateStyleName(combo_MainStyle.Text);

            if (newStyle == null || CurLevel == null || newStyle == CurLevel.MainStyle) return;

            // Load new style into PictureBoxes
            CurLevel.MainStyle = newStyle;
            UpdateBackgroundComboItems();
            UpdateBackgroundImage();
            pic_Level.Image = curRenderer.CombineLayers();
        }

        private void num_Lvl_SizeX_ValueChanged(object sender, EventArgs e)
        {
            CurLevel.Width = (int)num_Lvl_SizeX.Value;

            // Adapt max start position
            num_Lvl_StartX.Maximum = CurLevel.Width - 1;
            CurLevel.StartPosX = (int)num_Lvl_StartX.Value;

            // Update screen position and render level
            curRenderer.ChangeZoom(0);
            RepositionPicLevel();
            pic_Level.Image = curRenderer.CreateLevelImage();
        }

        private void num_Lvl_SizeY_ValueChanged(object sender, EventArgs e)
        {
            CurLevel.Height = (int)num_Lvl_SizeY.Value;
            
            // Adapt max start position
            num_Lvl_StartY.Maximum = CurLevel.Height - 1;
            CurLevel.StartPosY = (int)num_Lvl_StartY.Value;

            // Update screen position and render level
            curRenderer.ChangeZoom(0);
            RepositionPicLevel();
            pic_Level.Image = curRenderer.CreateLevelImage();
        }

        private void num_Lvl_StartX_ValueChanged(object sender, EventArgs e)
        {
            CurLevel.StartPosX = (int)num_Lvl_StartX.Value;
            pic_Level.Image = curRenderer.CombineLayers();
        }

        private void num_Lvl_StartY_ValueChanged(object sender, EventArgs e)
        {
            CurLevel.StartPosY = (int)num_Lvl_StartY.Value;
            pic_Level.Image = curRenderer.CombineLayers();
        }


        private void combo_Background_TextChanged(object sender, EventArgs e)
        {
            CurLevel.Background = Backgrounds.Find(this.combo_Background.Text);
            UpdateBackgroundImage();
            pic_Level.Image = curRenderer.CombineLayers();
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
            MovePieceIndex(true, false);
            RemoveFocus();
        }

        private void but_MoveBack_Click(object sender, EventArgs e)
        {
            MovePieceIndex(false, false);
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

        private void but_GroupSelection_Click(object sender, EventArgs e)
        {
            GroupSelectedPieces();
            RemoveFocus();
        }

        private void but_UngroupSelection_Click(object sender, EventArgs e)
        {
            UngroupSelectedPieces();
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
            pic_Level.Image = curRenderer.CreateLevelImage();
        }

        private void num_Resize_Width_ValueChanged(object sender, EventArgs e)
        {
            int newWidth = (int)num_Resize_Width.Value;
            CurLevel.SelectionList()
                    .FindAll(item => item is GadgetPiece)
                    .ForEach(obj => (obj as GadgetPiece).SpecWidth = newWidth);
            pic_Level.Image = curRenderer.CreateLevelImage();
        }

        private void num_Resize_Height_ValueChanged(object sender, EventArgs e)
        {
            int newHeight = (int)num_Resize_Height.Value;
            CurLevel.SelectionList()
                    .FindAll(item => item is GadgetPiece)
                    .ForEach(obj => (obj as GadgetPiece).SpecHeight = newHeight);
            pic_Level.Image = curRenderer.CreateLevelImage();
        }

        private void but_PairTeleporter_Click(object sender, EventArgs e)
        {
            PairTeleporters();
            RemoveFocus();
        }

        /* -----------------------------------------------------------
         *              Skill Selection Tab
         * ----------------------------------------------------------- */

        private void num_Skill_ValueChanged(object sender, EventArgs e)
        {
            var numBox = (NumericUpDown)sender;

            if (numBox.Value == 0)
            {
                numBox.BackColor = SystemColors.InactiveBorder;
            }
            else
            {
                numBox.BackColor = SystemColors.Window;
            }
        }

        private void num_Skill_KeyDown(object sender, KeyEventArgs e)
        {
            (sender as NumericUpDown).BackColor = SystemColors.Window;
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
            Style newStyle = ValidateStyleName(combo_PieceStyle.Text);

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
            stopWatchMouse.Restart();
            MoveTerrPieceSelection(-1);
        }

        private void but_PieceRight_MouseUp(object sender, MouseEventArgs e)
        {
            RemoveFocus();
        }

        private void but_PieceRight_Click(object sender, EventArgs e)
        {
            stopWatchMouse.Restart();
            MoveTerrPieceSelection(1);
        }

        private void picPieces_Click(object sender, EventArgs e)
        {
            int picIndex = picPieceList.FindIndex(pic => pic.Equals(sender));
            Debug.Assert(picIndex != -1, "PicBox not found in picPieceList.");

            AddNewPieceToLevel(picIndex);
            curRenderer.DeleteDraggingVars();
        }

        private void picPieces_MouseDown(object sender, MouseEventArgs e)
        {
            int picIndex = picPieceList.FindIndex(pic => pic.Equals(sender));
            Debug.Assert(picIndex != -1, "PicBox not found in picPieceList.");

            dragNewPieceKey = GetPieceKeyFromIndex(picIndex);

            pic_DragNewPiece.Width = ImageLibrary.GetWidth(dragNewPieceKey);
            pic_DragNewPiece.Height = ImageLibrary.GetHeight(dragNewPieceKey);
            pic_DragNewPiece.Image = ImageLibrary.GetImage(dragNewPieceKey);

            dragNewPieceTimer.Interval = 200;
            dragNewPieceTimer.Enabled = true;

            curRenderer.SetDraggingVars(new Point(0, 0), C.DragActions.DragNewPiece);
        }

        /* -----------------------------------------------------------
         *              Direct Key and Mouse imput
         * ----------------------------------------------------------- */

        private void NLEditForm_KeyDown(object sender, KeyEventArgs e)
        {
            mutexKeyDown.WaitOne();

            switch (e.KeyCode)
            {
                case Keys.ShiftKey: isShiftPressed = true; break;
                case Keys.ControlKey: isCtrlPressed = true; break;
                case Keys.Menu: isAltPressed = true; break;
                case Keys.P: isPPressed = true; break;
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
            else if (e.KeyCode == Keys.F10)
            {
                settingsToolStripMenuItem_Click(null, null);
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
                MoveTerrPieceSelection(e.Alt ? -picPieceList.Count : -1);
            }
            else if (e.Shift && e.KeyCode == Keys.Right)
            {
                RemoveFocus();
                MoveTerrPieceSelection(e.Alt ? picPieceList.Count : 1);
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
            else if (this.ActiveControl != txt_Focus)
            {
                return; // and don't restart the StopWatch
            }
            else if (e.KeyCode == Keys.Space)
            {
                ChangeObjTerrPieceDisplay();
            }
            else if (e.KeyCode == Keys.Delete)
            {
                DeleteSelectedPieces(false);
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
                AddFromClipboard(true);
            }
            else if (e.Control && e.KeyCode == Keys.C)
            {
                WriteToClipboard();
            }
            else if (e.KeyCode == Keys.C)
            {
                DuplicateSelectedPieces();
            }
            else if (e.KeyCode >= Keys.D0 && e.KeyCode <= Keys.D9)
            {
                int keyValue = e.KeyValue - (int)Keys.D0;
                if (keyValue == 0) keyValue = 10;

                if (picPieceList.Count >= keyValue)
                {
                    AddNewPieceToLevel(keyValue - 1);
                    UpdateFlagsForPieceActions();
                }
            }
            else if (e.KeyCode >= Keys.NumPad0 && e.KeyCode <= Keys.NumPad9)
            {
                int keyValue = e.KeyValue - (int)Keys.NumPad0;
                if (keyValue == 0) keyValue = 10;

                if (picPieceList.Count >= keyValue)
                {
                    AddNewPieceToLevel(keyValue - 1);
                    UpdateFlagsForPieceActions();
                }
            }
            else if (e.KeyCode.In(Keys.Left, Keys.Right, Keys.Up, Keys.Down))
            {
                C.DIR direction;
                switch (e.KeyCode)
                {
                    case Keys.Left: direction = C.DIR.W; break;
                    case Keys.Right: direction = C.DIR.E; break;
                    case Keys.Up: direction = C.DIR.N; break;
                    case Keys.Down: direction = C.DIR.S; break;
                    default: direction = C.DIR.E; break;
                }

                // Move screen start position, if 'P' is pressed in addition.
                if (isPPressed)
                {
                    // ensure displaying the screen start
                    DisplaySettings.SetDisplayed(C.DisplayType.ScreenStart, true);
                    MoveScreenStartPosition(direction);
                }
                // ...or selected pieces if they exist 
                else if (CurLevel.SelectionList().Count > 0)
                {
                    MoveLevelPieces(direction, e.Control ? 8 * gridSize : gridSize);
                }
                // ...or the screen position otherwise
                else
                {
                    curRenderer.MoveScreenPos(direction, e.Control ? 64 : 8);
                    pic_Level.Image = curRenderer.CombineLayers();
                }
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
            /*
            else if (e.KeyCode == Keys.G)
            {
                GroupSelectedPieces();
            }
            else if (e.KeyCode == Keys.H)
            {
                UngroupSelectedPieces();
            }
            */
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

            mutexKeyDown.ReleaseMutex();
        }

        private void NLEditForm_KeyUp(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.ShiftKey: isShiftPressed = false; break;
                case Keys.ControlKey: isCtrlPressed = false; break;
                case Keys.Menu: isAltPressed = false; break;
                case Keys.P: isPPressed = false; break;
            }

            if (e.KeyCode.In(Keys.Right, Keys.Left, Keys.Up, Keys.Down))
            {
                SaveChangesToOldLevelList();
            }
        }

        private void NLEditForm_MouseWheel(object sender, MouseEventArgs e)
        {
            mutexMouseWheel.WaitOne();

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
            RepositionPicLevel();
            pic_Level.Image = curRenderer.CombineLayers();

            mutexMouseWheel.ReleaseMutex();
        }

        private void pic_Level_MouseDown(object sender, MouseEventArgs e)
        {
            mutexMouseDown.WaitOne();

            mouseButtonPressed = e.Button;
            stopWatchMouse.Restart();

            Point mousePos = curRenderer.GetMousePosInLevel(e.Location);
            bool hasSelectedPieceAtPos = CurLevel.HasSelectionAtPos(mousePos);

            C.DragActions dragAction = C.DragActions.Null;
            if (e.Button == MouseButtons.Right) // for scrolling
            {
                dragAction = C.DragActions.MoveEditorPos;
                Cursor = Cursors.SizeAll;
            }
            else if (e.Button == MouseButtons.Middle) // for removal
            {
                dragAction = C.DragActions.SelectArea;
            }
            else if (isPPressed) // for moving the screen start
            {
                // Only drag screen position, if it lies within the screen start rectangle
                if (curRenderer.ScreenStartRectangle().Contains(mousePos))
                {
                    // ensure displaying the screen start
                    DisplaySettings.SetDisplayed(C.DisplayType.ScreenStart, true);
                    dragAction = C.DragActions.MoveStartPos;
                }
            }
            else if (hasSelectedPieceAtPos && !isAltPressed && !isCtrlPressed && !isShiftPressed)
            {
                dragAction = C.DragActions.DragPieces;
            }
            else
            {
                dragAction = C.DragActions.SelectArea;
            }

            curRenderer.SetDraggingVars(e.Location, dragAction);

            mutexMouseDown.ReleaseMutex();
        }

        private void pic_Level_MouseMove(object sender, MouseEventArgs e)
        {
            if (curRenderer.MouseStartPos == null) return;

            mutexMouseMove.WaitOne();

            curRenderer.MouseCurPos = e.Location;

            switch (curRenderer.MouseDragAction)
            {
                case C.DragActions.SelectArea:
                    {
                        pic_Level.Image = curRenderer.CombineLayers();
                        break;
                    }
                case C.DragActions.MoveEditorPos:
                    {
                        curRenderer.UpdateScreenPos();
                        UpdateScrollBarValues();
                        pic_Level.Image = curRenderer.CombineLayers();
                        break;
                    }
                case C.DragActions.DragPieces:
                    {
                        DragSelectedPieces();
                        pic_Level.Image = curRenderer.CreateLevelImage();
                        break;
                    }
                case C.DragActions.MoveStartPos:
                    {
                        Point newCenter = curRenderer.GetNewPosFromDragging();
                        MoveScreenStartPosition(newCenter);
                        pic_Level.Image = curRenderer.CombineLayers();
                        break;
                    }
            }
            pic_Level.Refresh();

            mutexMouseMove.ReleaseMutex();
        }

        private void pic_Level_MouseUp(object sender, MouseEventArgs e)
        {
            mutexMouseUp.WaitOne();

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
                        Cursor = Cursors.Default;
                        curRenderer.UpdateScreenPos();
                        UpdateScrollBarValues();
                        break;
                    }
                case C.DragActions.DragPieces:
                    {
                        DragSelectedPieces();
                        SaveChangesToOldLevelList();
                        break;
                    }
                case C.DragActions.MoveStartPos:
                    {
                        Point newCenter = curRenderer.GetNewPosFromDragging();
                        MoveScreenStartPosition(newCenter);
                        SaveChangesToOldLevelList();
                        break;
                    }
                case C.DragActions.DragNewPiece:
                    {
                        Point mousePicBoxPos = pic_Level.PointToClient(MousePosition);
                        if (curRenderer.IsPointInLevelArea(mousePicBoxPos))
                        {
                            Point mouseLevelPos = curRenderer.GetMousePosInLevel(mousePicBoxPos);
                            AddNewPieceToLevel(dragNewPieceKey, mouseLevelPos);
                        }
                        dragNewPieceTimer.Enabled = false;
                        pic_DragNewPiece.Visible = false;
                        break;
                    }
            }

            curRenderer.DeleteDraggingVars();
            pic_Level.Image = curRenderer.CreateLevelImage();
            UpdateFlagsForPieceActions();

            mouseButtonPressed = null;
            RemoveFocus();

            mutexMouseUp.ReleaseMutex();
        }


        private void pic_Level_DoubleClick(object sender, EventArgs e)
        {
            // Removed ability to add pieces by double-clicking.
            /*
            if (!(e is MouseEventArgs) || (e as MouseEventArgs).Button != MouseButtons.Left) return;

            curRenderer.DeleteDraggingVars();

            Point mouseScreenPos = MousePosition;
            Point mousePicBoxPos = pic_Level.PointToClient(mouseScreenPos);
            Point mouseLevelPos = curRenderer.GetMousePosInLevel(mousePicBoxPos);

            List<string> pieceList = pieceDoDisplayObject ? pieceCurStyle?.ObjectKeys : pieceCurStyle?.TerrainKeys;
            if (pieceList == null || pieceList.Count == 0) return;
            int startIndex = (pieceStartIndex - 1 + picPieceList.Count / 2) % pieceList.Count;

            var selectForm = new FormPieceSelection(this, pieceCurStyle, pieceDoDisplayObject, startIndex, mouseLevelPos, CurLevel.MainStyle);

            int formStartPosX;
            if (mouseScreenPos.X + selectForm.Width < SystemInformation.VirtualScreen.Width - 12)
            {
                formStartPosX = mouseScreenPos.X + 4;
            }
            else
            {
                formStartPosX = mouseScreenPos.X - selectForm.Width - 4;
            }
            int formStartPosY = Math.Max(Math.Min(mouseScreenPos.Y - selectForm.Height / 3,
                                         SystemInformation.VirtualScreen.Height - selectForm.Height - 4), 0);

            selectForm.StartPosition = FormStartPosition.Manual;
            selectForm.Location = new Point(formStartPosX, formStartPosY);
            selectForm.Show();
            */
        }
    }
}
