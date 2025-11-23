using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ToolTip;

namespace NLEditor
{
    /// <summary>
    /// Main editor form: Receives user input.
    /// </summary>
    partial class NLEditForm : Form
    {
        public static bool isNeoLemmixOnly { get; private set; }

        /// <summary>
        /// Initializes all important components and load an empty level.
        /// </summary>
        public NLEditForm()
        {
            InitializeComponent();
            UpdateExpandedTabs();
            PullFocusFromTextInputs();
            SetRepeatButtonIntervals();
            SetCustomSkillsetList();
            SetMusicList();

            C.ScreenSize = new ScreenSize();
            C.ScreenSize.InizializeSettings();

            LoadStylesFromFile.AddInitialImagesToLibrary();

            ImageLibrary.SetEditorForm(this);

            picPieceList = new List<PictureBox>
                {
                    picPiece0, picPiece1, picPiece2, picPiece3,
                    picPiece4, picPiece5, picPiece6, picPiece7
                };

            checkboxesSkillFlags = new Dictionary<C.Skill, CheckBox>()
                {
                    { C.Skill.Climber, check_Piece_Climber }, { C.Skill.Floater, check_Piece_Floater },
                    { C.Skill.Bomber, check_Piece_Bomber }, { C.Skill.Blocker, check_Piece_Blocker },
                    { C.Skill.Builder, check_Piece_Builder }, { C.Skill.Basher, check_Piece_Basher },
                    { C.Skill.Miner, check_Piece_Miner }, { C.Skill.Digger, check_Piece_Digger },
                    { C.Skill.Walker, check_Piece_Walker }, { C.Skill.Swimmer, check_Piece_Swimmer },
                    { C.Skill.Glider, check_Piece_Glider }, { C.Skill.Ballooner, check_Piece_Ballooner },
                    { C.Skill.Disarmer, check_Piece_Disarmer },{ C.Skill.Freezer, check_Piece_Freezer },
                    { C.Skill.Stoner, check_Piece_Stoner },
                    { C.Skill.Ladderer, check_Piece_Ladderer }, { C.Skill.Platformer, check_Piece_Platformer },
                    { C.Skill.Stacker, check_Piece_Stacker }, { C.Skill.Cloner, check_Piece_Cloner },
                    { C.Skill.Fencer, check_Piece_Fencer },  { C.Skill.Shimmier, check_Piece_Shimmier },
                    { C.Skill.Jumper, check_Piece_Jumper }, { C.Skill.Slider, check_Piece_Slider },
                    { C.Skill.Laserer, check_Piece_Laserer }, { C.Skill.Spearer, check_Piece_Spearer },
                    { C.Skill.Grenader, check_Piece_Grenader }, { C.Skill.Timebomber, check_Piece_Timebomber },
                    { C.Skill.Zombie, check_Piece_Zombie }, { C.Skill.Rival, check_Piece_Rival },
                    { C.Skill.Neutral, check_Piece_Neutral }
                };

            numericsSkillSet = new Dictionary<C.Skill, NumericUpDown>()
                {
                    { C.Skill.Climber, num_Ski_Climber }, { C.Skill.Floater, num_Ski_Floater },
                    { C.Skill.Bomber, num_Ski_Bomber }, { C.Skill.Blocker, num_Ski_Blocker },
                    { C.Skill.Builder, num_Ski_Builder }, { C.Skill.Basher, num_Ski_Basher },
                    { C.Skill.Miner, num_Ski_Miner }, { C.Skill.Digger, num_Ski_Digger },
                    { C.Skill.Walker, num_Ski_Walker }, { C.Skill.Swimmer, num_Ski_Swimmer },
                    { C.Skill.Glider, num_Ski_Glider }, { C.Skill.Ballooner, num_Ski_Ballooner },
                    { C.Skill.Disarmer, num_Ski_Disarmer }, { C.Skill.Freezer, num_Ski_Freezer },
                    { C.Skill.Stoner, num_Ski_Stoner },
                    { C.Skill.Ladderer, num_Ski_Ladderer }, { C.Skill.Platformer, num_Ski_Platformer },
                    { C.Skill.Stacker, num_Ski_Stacker }, { C.Skill.Cloner, num_Ski_Cloner },
                    { C.Skill.Fencer, num_Ski_Fencer }, { C.Skill.Shimmier, num_Ski_Shimmier },
                    { C.Skill.Jumper, num_Ski_Jumper }, { C.Skill.Slider, num_Ski_Slider },
                    { C.Skill.Laserer, num_Ski_Laserer }, { C.Skill.Spearer, num_Ski_Spearer },
                    { C.Skill.Grenader, num_Ski_Grenader }, { C.Skill.Timebomber, num_Ski_Timebomber },
                };

            var displayTabItems = new Dictionary<C.DisplayType, ToolStripMenuItem>()
                {
                    { C.DisplayType.Background, backgroundToolStripMenuItem },
                    { C.DisplayType.ClearPhysics, clearPhysicsToolStripMenuItem },
                    { C.DisplayType.Objects, objectToolStripMenuItem },
                    { C.DisplayType.ScreenStart, screenStartToolStripMenuItem },
                    { C.DisplayType.Terrain, terrainToolStripMenuItem },
                    { C.DisplayType.Trigger, triggerAreasToolStripMenuItem },
                    { C.DisplayType.Deprecated, deprecatedPiecesToolStripMenuItem }
                };
            DisplaySettings.SetMenuTabItems(displayTabItems);

            curSettings = new Settings(this);

            CreateStyleList();
            if (StyleList.Count > 0)
            {
                this.combo_MainStyle.Items.AddRange(StyleList.Where(sty => File.Exists(C.AppPathThemeInfo(sty.NameInDirectory))).Select(sty => sty.NameInEditor).ToArray());
                this.combo_MainStyle.SelectedIndex = 0;

                this.combo_PieceStyle.Items.AddRange(StyleList.ConvertAll(sty => sty.NameInEditor).ToArray());
                this.combo_PieceStyle.SelectedIndex = 0;
            }

            CreateNewLevelAndRenderer();
            UpdateFlagsForPieceActions();
            UpdatePieceMetaData();

            InitializeSettings();
            DetectLemmixVersions();
            UpdateLemmixVersionFeatures();

            if (!curSettings.UseTooltipBotton)
                toolTipButton.Active = false;

            pieceStartIndex = 0;
            pieceDoDisplayKind = C.SelectPieceType.Terrain;
            try
            {
                pieceCurStyle = ValidateStyleName(combo_PieceStyle.SelectedItem.ToString());
            }
            catch (NullReferenceException)
            {
                pieceCurStyle = null;
            }
            LoadPiecesIntoPictureBox();

            dragNewPieceTimer = new Timer();
            dragNewPieceTimer.Tick += new EventHandler((object sender, EventArgs e) => UpdateNewPiecePicBox());

            stopWatchKey = new Stopwatch();
            stopWatchKey.Start();
            stopWatchMouse = new Stopwatch();
            stopWatchMouse.Start();

            mouseButtonPressed = null;

            // For our Linux users: Ignore first resize event to default size and don't try to move non-existing components around.
            // But now we want to apply the size informations given by the settings.
            if (curSettings.IsFormMaximized)
                ClientSize = curSettings.FormSize;
            this.Resize += new EventHandler(NLEditForm_Resize);
            if (curSettings.IsFormMaximized)
                WindowState = FormWindowState.Maximized;
            else
                ClientSize = curSettings.FormSize;

            SetAutosaveTimer();

            var args = Environment.GetCommandLineArgs();

            if (args.Length >= 2)
                LoadNewLevel(args[1]);
        }

        Dictionary<C.Skill, CheckBox> checkboxesSkillFlags;
        Dictionary<C.Skill, NumericUpDown> numericsSkillSet;

        public List<PictureBox> picPieceList { get; private set; }
        Style pieceCurStyle;
        int pieceStartIndex;
        C.SelectPieceType pieceDoDisplayKind;

        string dragNewPieceKey;
        Timer dragNewPieceTimer;

        public Level CurLevel { get; private set; }
        public List<Style> StyleList { get; private set; }
        public BackgroundList Backgrounds { get; private set; }
        Renderer curRenderer;
        Settings curSettings;

        private Dictionary<Keys, Action> hotkeyActions;

        List<Level> oldLevelList;
        int curOldLevelIndex;
        Level lastSavedLevel;

        string levelDirectory; // for starting directory for saving/loading
        string targetFolder;

        int gridSize => curSettings.GridSize;
        int gridMoveAmount => curSettings.GridMoveAmount;
        int customMove => curSettings.CustomMove;

        public int editorMinWidth = 1000;
        public int editorMinHeight = 600;

        Stopwatch stopWatchKey;
        Stopwatch stopWatchMouse;
        MouseButtons? mouseButtonPressed;

        bool allTabsExpanded = false;

        bool repositionAfterZooming = true;
        bool movementActionPerformed = false;

        bool scrollHorizontallyPressed = false;
        bool scrollVerticallyPressed = false;

        bool dragToScrollPressed = false;
        bool dragHorizontallyPressed = false;
        bool dragVerticallyPressed = false;
        bool dragScreenStartPressed = false;

        bool removeAllPiecesAtCursorPressed = false;
        bool addOrRemoveSinglePiecePressed = false;
        bool selectPiecesBelowPressed = false;

        bool isShiftPressed = false;
        bool isCtrlPressed = false;
        bool isAltPressed = false;

        private static System.Threading.Mutex mutexMouseDown = new System.Threading.Mutex();
        private static System.Threading.Mutex mutexMouseUp = new System.Threading.Mutex();
        private static System.Threading.Mutex mutexMouseMove = new System.Threading.Mutex();
        private static System.Threading.Mutex mutexMouseWheel = new System.Threading.Mutex();
        private static System.Threading.Mutex mutexKeyDown = new System.Threading.Mutex();

        private FormLevelArranger levelArrangerWindow;
        private FormPieceBrowser pieceBrowserWindow;

        private void NLEditForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            try
            {
                curSettings.WriteSettingsToFile();

                Utility.DeleteFile(C.AppPathTempLevel);
                Utility.DeleteFile(System.IO.Path.ChangeExtension(C.AppPathTempLevel, ".nxsv"));

                if (e.CloseReason.In(CloseReason.UserClosing, CloseReason.ApplicationExitCall))
                {
                    e.Cancel = AskUserWhetherSaveLevel();
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
            PullFocusFromTextInputs();
        }

        private void NLEditForm_Resize(object sender, EventArgs e)
        {
            if (this == null || curRenderer == null)
                return;

            this.MinimumSize = new System.Drawing.Size(editorMinWidth, editorMinHeight);

            // Don't do anything on minimizing the form!
            if (WindowState == FormWindowState.Minimized)
                return;

            MoveControlsOnFormResize();
            ResetLevelImage();
            curSettings.SetFormSize();
        }

        private void ResetLevelImage()
        {
            curRenderer.EnsureScreenPosInLevel();
            pic_Level.SetImage(curRenderer.CreateLevelImage());
        }

        private void NLEditForm_Activated(object sender, EventArgs e)
        {
            UpdateIsSystemKeyPressed();
        }

        private void tabLvlProperties_Click(object sender, EventArgs e)
        {
            PullFocusFromTextInputs();
        }

        private void textbox_Leave(object sender, EventArgs e)
        {
            if (_IsWritingToForm) return;
            ReadLevelInfoFromForm(true);
            SaveChangesToOldLevelList();
        }

        private void textbox_Modify(object sender, EventArgs e)
        {
            ReadLevelInfoFromForm(false);
            SaveChangesToOldLevelList();
        }

        private void toolTipButton_Popup(object sender, PopupEventArgs e)
        {
            if (!curSettings.UseTooltipBotton)
                toolTipButton.Active = false;
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

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveLevel();
        }

        private void saveAsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveLevelAsNewFile();
        }

        private void saveAsImageToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveLevelAsImage();
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void highlightGroupedPiecesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            HighlightGroupedPieces();
        }

        private void highlightEraserPiecesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            HighlightEraserPieces();
        }

        private void clearPhysicsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ToggleClearPhysics();
        }

        private void terrainToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ToggleTerrain();
        }

        private void objectToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ToggleObjects();
        }

        private void triggerAreasToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ToggleTriggerAreas();
        }

        private void screenStartToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ToggleScreenStart();
        }

        private void backgroundToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ToggleBackground();
        }

        private void deprecatedPiecesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ToggleDeprecatedPieces();
        }

        private void showMissingPiecesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ShowMissingPiecesDialog();
        }

        private void snapToGridToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ToggleSnapToGrid(true);
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
            WriteToClipboard();
        }

        private void duplicateToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DuplicateSelectedPieces();
        }

        private void groupToolStripMenuItem_Click(object sender, EventArgs e)
        {
            GroupSelectedPieces();
        }

        private void ungroupToolStripMenuItem_Click(object sender, EventArgs e)
        {
            UngroupSelectedPieces();
        }

        private void playLevelToolStripMenuItem_Click(object sender, EventArgs e)
        {
            PlaytestLevel();
        }

        private void validateLevelToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ValidateLevel(false);
        }

        private void cleanseLevelsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ShowCleanseLevelsDialog();
        }

        private void hotkeysToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (FormHotkeys formHotkeys = new FormHotkeys())
            {
                formHotkeys.ShowDialog();
            }

            SetHotkeys();
        }

        private void settingsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            curSettings.OpenSettingsWindow();
            SetAutosaveTimer();
        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ShowAboutSLXEditor();
        }

        /* -----------------------------------------------------------
         *              Scrollbars for pic_Level
         * ----------------------------------------------------------- */

        private void scrollPicLevelHoriz_Scroll(object sender, ScrollEventArgs e)
        {
            curRenderer.ScreenPosX = e.NewValue;
            pic_Level.SetImage(curRenderer.GetScreenImage());
        }

        private void scrollPicLevelVert_Scroll(object sender, ScrollEventArgs e)
        {
            curRenderer.ScreenPosY = e.NewValue;
            pic_Level.SetImage(curRenderer.GetScreenImage());
        }

        /* -----------------------------------------------------------
         *              Global Level Info Tab
         * ----------------------------------------------------------- */

        private void combo_MainStyle_TextChanged(object sender, EventArgs e)
        {
            Style newStyle = ValidateStyleName(combo_MainStyle.Text);

            if (newStyle == null || CurLevel == null || newStyle == CurLevel.MainStyle)
                return;

            CurLevel.MainStyle = newStyle;
            UpdateBackgroundImage();
            LoadPiecesIntoPictureBox();
            pic_Level.SetImage(curRenderer.CreateLevelImage());

            // If the level is empty, switch piece style, too
            if (CurLevel.GadgetList.Count == 0 && CurLevel.TerrainList.Count == 0)
            {
                combo_PieceStyle.Text = newStyle.NameInEditor;
            }
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
            pic_Level.SetImage(curRenderer.CreateLevelImage());
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
            pic_Level.SetImage(curRenderer.CreateLevelImage());
        }

        private void num_Lvl_StartX_ValueChanged(object sender, EventArgs e)
        {
            CurLevel.StartPosX = (int)num_Lvl_StartX.Value;
            pic_Level.SetImage(curRenderer.GetScreenImage());
        }

        private void num_Lvl_StartY_ValueChanged(object sender, EventArgs e)
        {
            CurLevel.StartPosY = (int)num_Lvl_StartY.Value;
            pic_Level.SetImage(curRenderer.GetScreenImage());
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
            PullFocusFromTextInputs();
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
            PullFocusFromTextInputs();
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
            PullFocusFromTextInputs();
        }


        private void but_MoveFront_Click(object sender, EventArgs e)
        {
            MovePieceIndex(true, false);
            PullFocusFromTextInputs();
        }

        private void but_MoveBack_Click(object sender, EventArgs e)
        {
            MovePieceIndex(false, false);
            PullFocusFromTextInputs();
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
            PullFocusFromTextInputs();
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
            PullFocusFromTextInputs();
        }

        private void but_GroupSelection_Click(object sender, EventArgs e)
        {
            GroupSelectedPieces();
            PullFocusFromTextInputs();
        }

        private void but_UngroupSelection_Click(object sender, EventArgs e)
        {
            UngroupSelectedPieces();
            PullFocusFromTextInputs();
        }


        private void check_Pieces_Erase_CheckedChanged(object sender, EventArgs e)
        {
            bool isChecked = (check_Pieces_Erase.CheckState == CheckState.Checked);
            SetErase(isChecked);
            PullFocusFromTextInputs();
        }

        private void check_Pieces_NoOv_CheckedChanged(object sender, EventArgs e)
        {
            bool isChecked = (check_Pieces_NoOv.CheckState == CheckState.Checked);
            SetNoOverwrite(isChecked);
            PullFocusFromTextInputs();
        }

        private void check_Pieces_OnlyOnTerrain_CheckedChanged(object sender, EventArgs e)
        {
            bool isChecked = (check_Pieces_OnlyOnTerrain.CheckState == CheckState.Checked);
            SetOnlyOnTerrain(isChecked);
            PullFocusFromTextInputs();
        }

        private void check_Pieces_OneWay_CheckedChanged(object sender, EventArgs e)
        {
            bool isChecked = (check_Pieces_OneWay.CheckState == CheckState.Checked);
            SetOneWay(isChecked);
            PullFocusFromTextInputs();
        }

        private void check_Piece_Skill_CheckedChanged(object sender, EventArgs e)
        {
            C.Skill skill = checkboxesSkillFlags.First(check => check.Value.Equals((CheckBox)sender)).Key;
            bool isChecked = ((CheckBox)sender).CheckState == CheckState.Checked;
            SetSkillForObjects(skill, isChecked);
            PullFocusFromTextInputs();
            pic_Level.SetImage(curRenderer.CreateLevelImage());
        }

        private void num_Resize_Width_ValueChanged(object sender, EventArgs e)
        {
            int newWidth = (int)num_Resize_Width.Value;
            CurLevel.SelectionList()
                    .ForEach(obj => obj.SpecWidth = newWidth);
            pic_Level.SetImage(curRenderer.CreateLevelImage());
        }

        private void num_Resize_Height_ValueChanged(object sender, EventArgs e)
        {
            int newHeight = (int)num_Resize_Height.Value;
            CurLevel.SelectionList()
                    .ForEach(obj => obj.SpecHeight = newHeight);
            pic_Level.SetImage(curRenderer.CreateLevelImage());
        }

        private void cb_Decoration_Direction_SelectedIndexChanged(object sender, EventArgs e)
        {
            int newDir = cb_Decoration_Direction.SelectedIndex * 45 / 2;
            CurLevel.SelectionList()
                .FindAll(item => item.ObjType == C.OBJ.DECORATION)
                .ForEach(obj => (obj as GadgetPiece).DecorationAngle = newDir);
            SaveChangesToOldLevelList();
        }

        private void num_Decoration_Speed_ValueChanged(object sender, EventArgs e)
        {
            int newSpeed = (int)num_Decoration_Speed.Value;
            CurLevel.SelectionList()
                .FindAll(item => item.ObjType == C.OBJ.DECORATION)
                .ForEach(obj => (obj as GadgetPiece).DecorationSpeed = newSpeed);
            SaveChangesToOldLevelList();
        }

        private void num_SR_Countdown_ValueChanged(object sender, EventArgs e)
        {
            int countdownLength = (int)num_SR_Countdown.Value;
            CurLevel.SetCountdownLength(countdownLength);
            pic_Level.SetImage(curRenderer.CreateLevelImage());
        }

        private void but_PairTeleporter_Click(object sender, EventArgs e)
        {
            PairTeleporters();
            PullFocusFromTextInputs();
            pic_Level.SetImage(curRenderer.CreateLevelImage());
        }

        private void num_PickupSkillCount_ValueChanged(object sender, EventArgs e)
        {
            int newSkillCount = (int)num_PickupSkillCount.Value;
            CurLevel.SetPickupSkillCount(newSkillCount);
            pic_Level.SetImage(curRenderer.CreateLevelImage());
        }

        private void num_LemmingLimit_ValueChanged(object sender, EventArgs e)
        {
            int newLimit = (int)num_LemmingLimit.Value;
            CurLevel.SetLemmingLimit(newLimit);
            pic_Level.SetImage(curRenderer.CreateLevelImage());
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
            Style newStyle = ValidateStyleName(combo_PieceStyle.Text);

            if (newStyle == null || newStyle == pieceCurStyle)
                return;

            // Load new style into PictureBoxes
            pieceCurStyle = newStyle;
            pieceStartIndex = 0;

            LoadPiecesIntoPictureBox();
            UpdatePieceMetaData();
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

        private void but_PieceTerr_Click(object sender, EventArgs e)
        {
            CyclePieceBrowserDisplay(C.SelectPieceType.Terrain);
            PullFocusFromTextInputs();
        }

        private void but_PieceSteel_Click(object sender, EventArgs e)
        {
            CyclePieceBrowserDisplay(C.SelectPieceType.Steel);
            PullFocusFromTextInputs();
        }

        private void but_PieceObj_Click(object sender, EventArgs e)
        {
            CyclePieceBrowserDisplay(C.SelectPieceType.Objects);
            PullFocusFromTextInputs();
        }

        private void but_PieceBackground_Click(object sender, EventArgs e)
        {
            CyclePieceBrowserDisplay(C.SelectPieceType.Backgrounds);
            PullFocusFromTextInputs();
        }

        private void but_PieceSketch_Click(object sender, EventArgs e)
        {
            CyclePieceBrowserDisplay(C.SelectPieceType.Sketches);
            PullFocusFromTextInputs();
        }

        private void but_PieceLeft_MouseUp(object sender, MouseEventArgs e)
        {
            PullFocusFromTextInputs();
        }

        private void but_PieceLeft_Click(object sender, EventArgs e)
        {
            stopWatchMouse.Restart();
            MoveTerrPieceSelection(-1);
        }

        private void but_PieceRight_MouseUp(object sender, MouseEventArgs e)
        {
            PullFocusFromTextInputs();
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
            if (pieceDoDisplayKind != C.SelectPieceType.Backgrounds)
            {
                int picIndex = picPieceList.FindIndex(pic => pic.Equals(sender));
                Debug.Assert(picIndex != -1, "PicBox not found in picPieceList.");

                dragNewPieceKey = GetPieceKeyFromIndex(picIndex);

                if (dragNewPieceKey != "")
                {
                    pic_DragNewPiece.Width = ImageLibrary.GetWidth(dragNewPieceKey);
                    pic_DragNewPiece.Height = ImageLibrary.GetHeight(dragNewPieceKey);
                    pic_DragNewPiece.Image = ImageLibrary.GetImage(dragNewPieceKey);

                    dragNewPieceTimer.Interval = 200;
                    dragNewPieceTimer.Enabled = true;

                    curRenderer.SetDraggingVars(new Point(0, 0), C.DragActions.DragNewPiece);
                }
            }
        }

        /* -----------------------------------------------------------
         *              Direct Key and Mouse imput
         * ----------------------------------------------------------- */
        public void NLEditForm_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.ShiftKey:
                    isShiftPressed = true;
                    break;
                case Keys.ControlKey:
                    isCtrlPressed = true;
                    break;
                case Keys.Menu:
                    isAltPressed = true;
                    break;
            }

            /// <summary>
            /// Determines if a key is used for text input.
            /// </summary>
            bool IsTextInputKey(Keys key)
            {
                if (key >= Keys.F1 && key <= Keys.F24) return false;
                if (key == Keys.Escape || key == Keys.Enter) return false;
                if (key == Keys.ControlKey || key == Keys.ShiftKey || key == Keys.Menu)
                    return false;

                // All other keys can be used for text input
                return true;
            }

            if (ActiveControl != txt_Focus)
            {
                if (IsTextInputKey(e.KeyCode) && e.Modifiers == Keys.None)
                {
                    return; // Allow typing when text input is focused
                }
            }

            // Process hotkey actions
            Keys hotkey = e.KeyData;
            if (hotkeyActions.TryGetValue(hotkey, out Action action))
            {
                action.Invoke();
                e.Handled = true;
            }
        }

        public void NLEditForm_KeyUp(object sender, KeyEventArgs e)
        {
            // Reset hotkey flags when keys are released
            switch (e.KeyCode)
            {
                case Keys.ShiftKey:
                    isShiftPressed = false;
                    break;
                case Keys.ControlKey:
                    isCtrlPressed = false;
                    break;
                case Keys.Menu:
                    isAltPressed = false;
                    break;
                case var key when key == HotkeyConfig.HotkeyScrollHorizontally:
                    scrollHorizontallyPressed = false;
                    break;
                case var key when key == HotkeyConfig.HotkeyScrollVertically:
                    scrollVerticallyPressed = false;
                    break;
                case var key when key == HotkeyConfig.HotkeyDragToScroll:
                    dragToScrollPressed = false;
                    break;
                case var key when key == HotkeyConfig.HotkeyDragHorizontally:
                    dragHorizontallyPressed = false;
                    break;
                case var key when key == HotkeyConfig.HotkeyDragVertically:
                    dragVerticallyPressed = false;
                    break;
                case var key when key == HotkeyConfig.HotkeyMoveScreenStart:
                    dragScreenStartPressed = false;
                    break;
                case var key when key == HotkeyConfig.HotkeyRemovePiecesAtCursor:
                    removeAllPiecesAtCursorPressed = false;
                    break;
                case var key when key == HotkeyConfig.HotkeyAddRemoveSinglePiece:
                    addOrRemoveSinglePiecePressed = false;
                    break;
                case var key when key == HotkeyConfig.HotkeySelectPiecesBelow:
                    selectPiecesBelowPressed = false;
                    break;
            }

            // Resolve movement-related actions
            if (movementActionPerformed)
            {
                SaveChangesToOldLevelList();
                movementActionPerformed = false;
            }
        }

        public void NLEditForm_MouseWheel(object sender, MouseEventArgs e)
        {
            mutexMouseWheel.WaitOne();

            int movement = e.Delta / SystemInformation.MouseWheelScrollDelta;

            // Browse left and right if cursor is the piece browser
            if (picPieceList[0].PointToClient(this.PointToScreen(e.Location)).Y > -5)
            {
                MoveTerrPieceSelection(movement > 0 ? 1 : -1);
            }
            else
            {
                if (scrollHorizontallyPressed && (curRenderer.ZoomFactor > 2))
                {
                    movement *= 4;
                    scrollPicLevelHoriz_Scroll(sender, new ScrollEventArgs(ScrollEventType.ThumbPosition, curRenderer.ScreenPosX, curRenderer.ScreenPosX + movement, ScrollOrientation.HorizontalScroll));
                }
                else if (scrollVerticallyPressed && (curRenderer.ZoomFactor > 2))
                {
                    movement *= 4;
                    scrollPicLevelVert_Scroll(sender, new ScrollEventArgs(ScrollEventType.ThumbPosition, curRenderer.ScreenPosY, curRenderer.ScreenPosY + movement, ScrollOrientation.VerticalScroll));
                }
                else // Zoom the level
                {
                    Point mousePosRelPicLevel = pic_Level.PointToClient(this.PointToScreen(e.Location));
                    curRenderer.SetZoomMousePos(mousePosRelPicLevel);
                    curRenderer.ChangeZoom(movement > 0 ? 1 : -1, true);
                }
            }

            // Update level image
            RepositionPicLevel();
            pic_Level.SetImage(curRenderer.GetScreenImage());

            mutexMouseWheel.ReleaseMutex();
        }

        private void pic_Level_MouseDown(object sender, MouseEventArgs e)
        {
            // Convert mouse buttons to Keys
            Keys mouseButtonKey = e.Button == MouseButtons.Left ? Keys.LButton :
                                  e.Button == MouseButtons.Right ? Keys.RButton :
                                  e.Button == MouseButtons.Middle ? Keys.MButton :
                                  e.Button == MouseButtons.XButton1 ? Keys.XButton1 :
                                  e.Button == MouseButtons.XButton2 ? Keys.XButton2 :
                                  Keys.None;

            // Include modifier keys (Ctrl, Shift, Alt)
            Keys hotkey = mouseButtonKey | Control.ModifierKeys;

            // Process hotkey actions
            if (hotkey != Keys.None && hotkeyActions.TryGetValue(hotkey, out Action action))
            {
                action.Invoke();
            }

            HandleMouseInput(sender, e);
        }

        private void HandleMouseInput(object sender, MouseEventArgs e)
        {
            mutexMouseDown.WaitOne();

            ReadLevelInfoFromForm(true);
            SaveChangesToOldLevelList();

            mouseButtonPressed = e.Button;
            stopWatchMouse.Restart();

            Point mousePos = curRenderer.GetMousePosInLevel(e.Location);
            bool hasSelectedPieceAtPos = CurLevel.HasSelectionAtPos(mousePos);
            bool hasPieceAtPos = curRenderer.GetLevelBmpRect().Contains(curRenderer.GetMousePosInLevel(e.Location, false))
                                  && CurLevel.HasPieceAtPos(mousePos);

            C.DragActions dragAction = C.DragActions.Null;

            // Set drag actions according to hotkeys & other conditions
            if (dragToScrollPressed)
            {
                dragAction = C.DragActions.MoveEditorPos;
                Cursor = Cursors.SizeAll;
            }
            else if (removeAllPiecesAtCursorPressed)
            {
                dragAction = C.DragActions.SelectArea;
            }
            else if (dragScreenStartPressed && !CurLevel.AutoStartPos)
            {
                if (curRenderer.ScreenStartRectangle().Contains(mousePos))
                {
                    DisplaySettings.SetDisplayed(C.DisplayType.ScreenStart, true);
                    dragAction = C.DragActions.MoveStartPos;
                }
            }
            else if (hasSelectedPieceAtPos)
            {
                curRenderer.MouseCurPos = e.Location;

                if (addOrRemoveSinglePiecePressed)
                {
                    LevelSelectSinglePiece();
                    pic_Level.SetImage(curRenderer.GetScreenImage());
                }

                dragAction = C.DragActions.DragPieces;
            }
            else if (hasSelectedPieceAtPos && dragHorizontallyPressed)
            {
                dragAction = C.DragActions.HorizontalDrag;
                Cursor = Cursors.SizeWE;
            }
            else if (hasSelectedPieceAtPos && dragVerticallyPressed)
            {
                dragAction = C.DragActions.VerticalDrag;
                Cursor = Cursors.SizeNS;
            }
            else if (hasPieceAtPos && mouseButtonPressed == MouseButtons.Left)
            {
                curRenderer.MouseCurPos = e.Location;
                LevelSelectSinglePiece();
                pic_Level.SetImage(curRenderer.GetScreenImage());
                dragAction = C.DragActions.MaybeDragPieces;
            }
            else
            {
                dragAction = C.DragActions.SelectArea;
            }

            curRenderer.SetDraggingVars(e.Location, dragAction);

            mutexMouseDown.ReleaseMutex();
        }

        private void pic_Level_MouseUp(object sender, MouseEventArgs e)
        {
            mutexMouseUp.WaitOne();

            curRenderer.MouseCurPos = e.Location;

            switch (curRenderer.MouseDragAction)
            {
                case C.DragActions.SelectArea:
                    {
                        if (stopWatchMouse.ElapsedMilliseconds < 200)
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
                case C.DragActions.HorizontalDrag:
                    {
                        Cursor = Cursors.Default;
                        XDragSelectedPieces();
                        SaveChangesToOldLevelList();
                        break;
                    }
                case C.DragActions.VerticalDrag:
                    {
                        Cursor = Cursors.Default;
                        YDragSelectedPieces();
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
            pic_Level.SetImage(curRenderer.CreateLevelImage());
            UpdateFlagsForPieceActions();

            mouseButtonPressed = null;
            PullFocusFromTextInputs();

            mutexMouseUp.ReleaseMutex();

            // Reset hotkey flags (just in case)
            scrollHorizontallyPressed = false;
            scrollVerticallyPressed = false;
            dragToScrollPressed = false;
            dragHorizontallyPressed = false;
            dragVerticallyPressed = false;
            dragScreenStartPressed = false;
            removeAllPiecesAtCursorPressed = false;
            addOrRemoveSinglePiecePressed = false;
            selectPiecesBelowPressed = false;
        }

        private void pic_Level_MouseMove(object sender, MouseEventArgs e)
        {
            if (curRenderer.MouseStartPos == null)
                return;

            mutexMouseMove.WaitOne();

            curRenderer.MouseCurPos = e.Location;

            if (ShouldUpdateDragAction(e))
            {
                mutexMouseMove.ReleaseMutex();
                return;
            }

            // Continue with the current drag action
            switch (curRenderer.MouseDragAction)
            {
                case C.DragActions.SelectArea:
                    pic_Level.SetImage(curRenderer.GetScreenImage());
                    break;

                case C.DragActions.MoveEditorPos:
                    curRenderer.UpdateScreenPos();
                    UpdateScrollBarValues();
                    pic_Level.SetImage(curRenderer.GetScreenImage());
                    break;

                case C.DragActions.MaybeDragPieces:
                    curRenderer.ConfirmDrag();
                    DragSelectedPieces();
                    pic_Level.SetImage(curRenderer.CreateLevelImage());
                    break;

                case C.DragActions.DragPieces:
                    DragSelectedPieces();
                    pic_Level.SetImage(curRenderer.CreateLevelImage());
                    break;

                case C.DragActions.HorizontalDrag:
                    XDragSelectedPieces();
                    pic_Level.SetImage(curRenderer.CreateLevelImage());
                    break;

                case C.DragActions.VerticalDrag:
                    YDragSelectedPieces();
                    pic_Level.SetImage(curRenderer.CreateLevelImage());
                    break;

                case C.DragActions.MoveStartPos:
                    Point newCenter = curRenderer.GetNewPosFromDragging();
                    MoveScreenStartPosition(newCenter);
                    pic_Level.SetImage(curRenderer.GetScreenImage());
                    break;
            }

            pic_Level.Refresh();
            mutexMouseMove.ReleaseMutex();
        }

        /// <summary>
        /// Allows switching between normal, horizontal-only, and vertical-only drag action
        /// </summary>
        private bool ShouldUpdateDragAction(MouseEventArgs e)
        {
            // Check current mouse button with modifiers
            Keys currentHotkey = mouseButtonPressed.HasValue
                ? (mouseButtonPressed.Value == MouseButtons.Left ? Keys.LButton :
                   mouseButtonPressed.Value == MouseButtons.Right ? Keys.RButton :
                   mouseButtonPressed.Value == MouseButtons.Middle ? Keys.MButton :
                   mouseButtonPressed.Value == MouseButtons.XButton1 ? Keys.XButton1 :
                   mouseButtonPressed.Value == MouseButtons.XButton2 ? Keys.XButton2 :
                   Keys.None) | Control.ModifierKeys
                : Keys.None;

            // Dynamically update drag action based on current hotkey during an existing drag
            if (curRenderer.MouseDragAction == C.DragActions.DragPieces ||
                curRenderer.MouseDragAction == C.DragActions.HorizontalDrag ||
                curRenderer.MouseDragAction == C.DragActions.VerticalDrag)
            {
                // Preserve current action by default
                C.DragActions dragAction = curRenderer.MouseDragAction;

                // Check if current hotkey matches horizontal/vertical drag hotkey
                if (currentHotkey == HotkeyConfig.HotkeyDragHorizontally)
                {
                    dragAction = C.DragActions.HorizontalDrag;
                    Cursor = Cursors.SizeWE;
                }
                else if (currentHotkey == HotkeyConfig.HotkeyDragVertically)
                {
                    dragAction = C.DragActions.VerticalDrag;
                    Cursor = Cursors.SizeNS;
                }
                else
                {
                    dragAction = C.DragActions.DragPieces;
                    Cursor = Cursors.Default;
                }

                // If a new drag action is set, update dragging vars and reset move event
                if (dragAction != curRenderer.MouseDragAction)
                {
                    curRenderer.SetDraggingVars(e.Location, dragAction);
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Handles hotkey interaction when style dropdown on Piece Browser window is focused
        /// </summary>
        private void HandlePieceBrowserWindowCombo(bool isMouseEnter)
        {
            if (pieceBrowserWindow == null)
                return;

            if (isMouseEnter)
                pieceBrowserWindow.KeyPreview = false; // Prevent hotkey interaction
            else
                pieceBrowserWindow.KeyPreview = true; // Re-enable hotkey interaction
        }

        private void btnTalismanMoveUp_Click(object sender, EventArgs e)
        {
            int talIndex = lbTalismans.SelectedIndex;

            if (talIndex > 0)
            {
                Talisman tal = CurLevel.Talismans[talIndex];
                CurLevel.Talismans.Remove(tal);
                CurLevel.Talismans.Insert(talIndex - 1, tal);
                RegenerateTalismanList();
            }
        }

        private void btnTalismanMoveDown_Click(object sender, EventArgs e)
        {
            int talIndex = lbTalismans.SelectedIndex;

            if ((talIndex >= 0) && (talIndex < CurLevel.Talismans.Count - 1))
            {
                Talisman tal = CurLevel.Talismans[talIndex];
                CurLevel.Talismans.Remove(tal);
                CurLevel.Talismans.Insert(talIndex + 1, tal);
                RegenerateTalismanList();
            }
        }

        private void btnTalismanDelete_Click(object sender, EventArgs e)
        {
            var tal = (Talisman)lbTalismans.SelectedItem;

            if (tal != null)
                CurLevel.Talismans.Remove(tal);

            RegenerateTalismanList();
        }

        private void btnTalismanAdd_Click(object sender, EventArgs e)
        {
            using (var talForm = new FormTalisman(CurLevel))
            {
                talForm.ShowDialog(this);
            }
            RegenerateTalismanList();
        }

        private void btnTalismanEdit_Click(object sender, EventArgs e)
        {
            var tal = (Talisman)lbTalismans.SelectedItem;

            if (tal != null)
            {
                using (var talForm = new FormTalisman(CurLevel, tal))
                {
                    talForm.ShowDialog(this);
                }
                RegenerateTalismanList();
            }
        }

        private void btnEditPreview_Click(object sender, EventArgs e)
        {
            using (var textForm = new FormPrePostText(CurLevel, true))
            {
                textForm.ShowDialog(this);
            }
        }

        private void btnEditPostview_Click(object sender, EventArgs e)
        {
            using (var textForm = new FormPrePostText(CurLevel, false))
            {
                textForm.ShowDialog(this);
            }
        }

        private void but_ClearBackground_Click(object sender, EventArgs e)
        {
            CurLevel.Background = null;
            UpdateBackgroundImage();
            pic_Level.SetImage(curRenderer.CombineLayers());
        }

        private void but_RandomID_Click(object sender, EventArgs e)
        {
            CurLevel.LevelID = (ulong)Utility.Random().Next() +
                               ((ulong)Utility.Random().Next() << 32);
            txt_LevelID.Text = CurLevel.LevelID.ToString("X16");
        }

        private void num_LemmingLimit_KeyPress(object sender, KeyEventArgs e)
        {
            num_LemmingLimit_ValueChanged(sender, null);
        }

        private void num_PickupSkillCount_KeyUp(object sender, KeyEventArgs e)
        {
            num_PickupSkillCount_ValueChanged(sender, null);
        }

        private void pasteInPlaceToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AddFromClipboard(false);
        }

        private void timerAutosave_Tick(object sender, EventArgs e)
        {
            timerAutosave.Stop();
            try
            {
                MakeAutoSave();
            }
            finally
            {
                timerAutosave.Start();
            }
        }

        private void NLEditForm_DragDrop(object sender, DragEventArgs e)
        {
            string[] files = e.Data.GetData(DataFormats.FileDrop) as string[];
            if (files.Length == 1)
                LoadNewLevel(files[0]);
        }

        private void NLEditForm_DragOver(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
                e.Effect = DragDropEffects.Link;
            else
                e.Effect = DragDropEffects.None;
        }

        private void chk_Lvl_AutoStart_Leave(object sender, EventArgs e)
        {
            if (_IsWritingToForm) return;
            textbox_Leave(sender, e);
            pic_Level.SetImage(curRenderer.GetScreenImage());
        }

        private void num_SR_Countdown_KeyUp(object sender, KeyEventArgs e)
        {
            num_SR_Countdown_ValueChanged(sender, null);
        }

        private void showMissingPiecesStatusBarMenuItem_Click(object sender, EventArgs e)
        {
            ShowMissingPiecesDialog();
        }

        private void oKStatusBarMenuItem_Click(object sender, EventArgs e)
        {
            statusBar.Visible = false;
        }

        private void deleteMissingPiecesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DeleteMissingPieces();
        }

        private void statusBarLabel1_Click(object sender, EventArgs e)
        {
            ShowMissingPiecesDialog();
        }

        private void toolStripLabel1_MouseEnter(object sender, EventArgs e)
        {
            Cursor = Cursors.Hand;
        }

        private void toolStripLabel1_MouseLeave(object sender, EventArgs e)
        {
            Cursor = Cursors.Default;
        }

        private void btnRandomSkillset_Click(object sender, EventArgs e)
        {
            GenerateRandomSkillset();
        }

        private void btnAllNonZeroSkillsToN_Click(object sender, EventArgs e)
        {
            SetAllNonZeroSkillsToN();
        }

        private void num_RandomLimit_ValueChanged(object sender, EventArgs e)
        {
            if (num_RandomMinLimit.Value > num_RandomMaxLimit.Value)
            {
                num_RandomMaxLimit.Value = num_RandomMinLimit.Value + 1;
            }
        }

        private void NLEditForm_Shown(object sender, EventArgs e)
        {
            SetHotkeys();

            if (Properties.Settings.Default.ShowAboutSLXWindowAtStartup)
                ShowAboutSLXEditor();

            if (Properties.Settings.Default.LevelArrangerIsOpen)
                OpenLevelArrangerWindow();

            SetMetaDataPanel();
        }

        private void whatsNewToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ShowAboutSLXEditor();
        }

        private void but_SearchPieces_Click(object sender, EventArgs e)
        {
            OpenPieceSearch();
        }

        private void searchPiecesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenPieceSearch();
        }

        private void but_LoadStyle_Click(object sender, EventArgs e)
        {
            LoadStyleFromMetaData();
        }

        private void selectAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SelectAllPieces();
        }

        private void openLevelWindowToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenLevelArrangerWindow();
        }

        private void openPieceBrowserWindowToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenPieceBrowserWindow();
        }

        private void expandAllTabsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ToggleExpandedTabs();
        }

        private void ComboMouseEnter(object sender, EventArgs e)
        {
            // Focus the combo if the mouse is hovered over it to allow wheel interaction
            if (sender is ComboBox combo)
            {
                combo.Focus();
                HandlePieceBrowserWindowCombo(true);
            }
        }

        private void ComboMouseLeave(object sender, EventArgs e)
        {
            // Return focus to main form if the combo is no longer focused
            if (sender is ComboBox combo)
            {
                BeginInvoke(new Action(() =>
                {
                    if (!combo.DroppedDown && !combo.Bounds.Contains(PointToClient(MousePosition)))
                    {
                        HandlePieceBrowserWindowCombo(false);
                        PullFocusFromTextInputs();
                    }
                }));
            }
        }

        private void ComboDropDownClosed(object sender, EventArgs e)
        {
            // Return focus to main form if the list is closed
            PullFocusFromTextInputs();
        }

        private void btnCustomSkillset_Click(object sender, EventArgs e)
        {
            ApplyCustomSkillset();
        }

        private void btnSaveAsCustomSkillset_Click(object sender, EventArgs e)
        {
            SaveSkillsetAsCustom();
        }

        private void refreshStylesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            RefreshStyles();
        }
    }
}
