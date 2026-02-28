using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ToolTip;

namespace SLXEditor
{
    /// <summary>
    /// Main editor form: Receives user input.
    /// </summary>
    partial class SLXEditForm : Form
    {
        public static bool isNeoLemmixOnly { get; private set; }

        /// <summary>
        /// Initializes all important components and load an empty level.
        /// </summary>
        public SLXEditForm()
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
            LoadStylesFromFile.AddRulersToLibrary();

            ImageLibrary.SetEditorForm(this);

            picPieceList = new List<PictureBox>
                {
                    picPiece0, picPiece1, picPiece2, picPiece3,
                    picPiece4, picPiece5, picPiece6, picPiece7
                };

            checkboxesSkillFlags = new Dictionary<C.Skill, CheckBox>()
                {
                    { C.Skill.Climber, checkClimber }, { C.Skill.Floater, checkFloater },
                    { C.Skill.Bomber, checkBomber }, { C.Skill.Blocker, checkBlocker },
                    { C.Skill.Builder, checkBuilder }, { C.Skill.Basher, checkBasher },
                    { C.Skill.Miner, checkMiner }, { C.Skill.Digger, checkDigger },
                    { C.Skill.Walker, checkWalker }, { C.Skill.Swimmer, checkSwimmer },
                    { C.Skill.Glider, checkGlider }, { C.Skill.Ballooner, checkBallooner },
                    { C.Skill.Disarmer, checkDisarmer },{ C.Skill.Freezer, checkFreezer },
                    { C.Skill.Stoner, checkStoner },
                    { C.Skill.Ladderer, checkLadderer }, { C.Skill.Platformer, checkPlatformer },
                    { C.Skill.Stacker, checkStacker }, { C.Skill.Cloner, checkCloner },
                    { C.Skill.Fencer, checkFencer },  { C.Skill.Shimmier, checkShimmier },
                    { C.Skill.Jumper, checkJumper }, { C.Skill.Slider, checkSlider },
                    { C.Skill.Laserer, checkLaserer }, { C.Skill.Spearer, checkSpearer },
                    { C.Skill.Grenader, checkGrenader }, { C.Skill.Timebomber, checkTimebomber },
                    { C.Skill.Zombie, checkZombie }, { C.Skill.Rival, checkRival },
                    { C.Skill.Neutral, checkNeutral }
                };

            numericsSkillSet = new Dictionary<C.Skill, NumericUpDown>()
                {
                    { C.Skill.Climber, numClimber }, { C.Skill.Floater, numFloater },
                    { C.Skill.Bomber, numBomber }, { C.Skill.Blocker, numBlocker },
                    { C.Skill.Builder, numBuilder }, { C.Skill.Basher, numBasher },
                    { C.Skill.Miner, numMiner }, { C.Skill.Digger, numDigger },
                    { C.Skill.Walker, numWalker }, { C.Skill.Swimmer, numSwimmer },
                    { C.Skill.Glider, numGlider }, { C.Skill.Ballooner, numBallooner },
                    { C.Skill.Disarmer, numDisarmer }, { C.Skill.Freezer, numFreezer },
                    { C.Skill.Stoner, numStoner },
                    { C.Skill.Ladderer, numLadderer }, { C.Skill.Platformer, numPlatformer },
                    { C.Skill.Stacker, numStacker }, { C.Skill.Cloner, numCloner },
                    { C.Skill.Fencer, numFencer }, { C.Skill.Shimmier, numShimmier },
                    { C.Skill.Jumper, numJumper }, { C.Skill.Slider, numSlider },
                    { C.Skill.Laserer, numLaserer }, { C.Skill.Spearer, numSpearer },
                    { C.Skill.Grenader, numGrenader }, { C.Skill.Timebomber, numTimebomber },
                };

            var displayTabItems = new Dictionary<C.DisplayType, ToolStripMenuItem>()
                {
                    { C.DisplayType.Background, backgroundToolStripMenuItem },
                    { C.DisplayType.ClearPhysics, clearPhysicsToolStripMenuItem },
                    { C.DisplayType.Objects, objectToolStripMenuItem },
                    { C.DisplayType.ScreenStart, screenStartToolStripMenuItem },
                    { C.DisplayType.Terrain, terrainToolStripMenuItem },
                    { C.DisplayType.Triggers, triggerAreasToolStripMenuItem },
                    { C.DisplayType.Rulers, rulersToolStripMenuItem },
                    { C.DisplayType.Deprecated, deprecatedPiecesToolStripMenuItem }
                };
            DisplaySettings.SetMenuTabItems(displayTabItems);

            curSettings = new Settings(this);
            InitializeSettings();
            DetectLemmixVersions();
            UpdateLemmixVersionFeatures();

            CreateStyleList();
            if (StyleList.Count > 0)
            {
                this.comboTheme.Items.AddRange(StyleList.Where(sty => File.Exists(C.AppPathThemeInfo(sty.NameInDirectory))).Select(sty => sty.NameInEditor).ToArray());
                this.comboTheme.SelectedIndex = 0;

                this.comboPieceStyle.Items.AddRange(StyleList.ConvertAll(sty => sty.NameInEditor).ToArray());
                this.comboPieceStyle.SelectedIndex = 0;
            }

            CreateNewLevelAndRenderer();
            UpdateFlagsForPieceActions();
            UpdatePieceMetaData();

            ResetLevelImage();
            MoveControlsOnFormResize();

            if (!curSettings.UseTooltipBotton)
                toolTipButton.Active = false;

            pieceStartIndex = 0;
            pieceDoDisplayKind = C.SelectPieceType.Terrain;
            try
            {
                pieceCurStyle = ValidateStyleName(comboPieceStyle.SelectedItem.ToString());
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

        public string LevelDirectory; // for starting directory for saving/loading

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

                Utility.DeleteFile(C.AppPathTempLevel + LevelFileExtension());
                Utility.DeleteFile(Path.ChangeExtension(C.AppPathTempLevel, ".nxsv"));

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
            picLevel.Image = curRenderer.CombineLayers();
            curRenderer.EnsureScreenPosInLevel();
            picLevel.SetImage(curRenderer.CreateLevelImage());
        }

        private void NLEditForm_Activated(object sender, EventArgs e)
        {
            UpdateIsSystemKeyPressed();
        }

        private void tabLvlProperties_Click(object sender, EventArgs e)
        {
            PullFocusFromTextInputs();
        }

        private void CommitLevelChanges()
        {
            if (_IsWritingToForm) return;
            ReadLevelInfoFromForm(true);
            SaveChangesToOldLevelList();
        }

        private void textbox_Leave(object sender, EventArgs e)
        {
            CommitLevelChanges();
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

        private void rulersToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ToggleRulers();
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
            ValidateLevel(false, false);
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
            picLevel.SetImage(curRenderer.GetScreenImage());
        }

        private void scrollPicLevelVert_Scroll(object sender, ScrollEventArgs e)
        {
            curRenderer.ScreenPosY = e.NewValue;
            picLevel.SetImage(curRenderer.GetScreenImage());
        }

        /* -----------------------------------------------------------
         *              Global Level Info Tab
         * ----------------------------------------------------------- */

        private void combo_ThemeStyle_TextChanged(object sender, EventArgs e)
        {
            Style newStyle = ValidateStyleName(comboTheme.Text);

            if (newStyle == null || CurLevel == null)
                return;

            CurLevel.ThemeStyle = newStyle;
            UpdateBackgroundImage();
            LoadPiecesIntoPictureBox();
            picLevel.SetImage(curRenderer.CreateLevelImage());

            // If the level is empty, switch piece style, too
            if (CurLevel.GadgetList.Count == 0 && CurLevel.TerrainList.Count == 0)
            {
                comboPieceStyle.Text = newStyle.NameInEditor;
            }
        }

        private void numSizeX_ValueChanged(object sender, EventArgs e)
        {
            CurLevel.Width = (int)numWidth.Value;

            // Adapt max start position
            numStartX.Maximum = CurLevel.Width - 1;
            CurLevel.StartPosX = (int)numStartX.Value;

            // Update screen position and render level
            curRenderer.ChangeZoom(0);
            RepositionPicLevel();
            picLevel.SetImage(curRenderer.CreateLevelImage());
        }

        private void numSizeY_ValueChanged(object sender, EventArgs e)
        {
            CurLevel.Height = (int)numHeight.Value;

            // Adapt max start position
            numStartY.Maximum = CurLevel.Height - 1;
            CurLevel.StartPosY = (int)numStartY.Value;

            // Update screen position and render level
            curRenderer.ChangeZoom(0);
            RepositionPicLevel();
            picLevel.SetImage(curRenderer.CreateLevelImage());
        }

        private void numStartX_ValueChanged(object sender, EventArgs e)
        {
            CurLevel.StartPosX = (int)numStartX.Value;
            picLevel.SetImage(curRenderer.GetScreenImage());
        }

        private void numStartY_ValueChanged(object sender, EventArgs e)
        {
            CurLevel.StartPosY = (int)numStartY.Value;
            picLevel.SetImage(curRenderer.GetScreenImage());
        }


        /* -----------------------------------------------------------
         *              Piece Info Tab
         * ----------------------------------------------------------- */

        private void btnRotatePieces_Click(object sender, EventArgs e)
        {
            if (!btnRotate.IsRepeatedAction || stopWatchMouse.ElapsedMilliseconds > btnRotate.Interval() / 2)
            {
                stopWatchMouse.Restart();
                RotateLevelPieces();
            }
        }

        private void btnRotatePieces_MouseUp(object sender, MouseEventArgs e)
        {
            PullFocusFromTextInputs();
        }

        private void btnInvertPieces_Click(object sender, EventArgs e)
        {
            if (!btnInvert.IsRepeatedAction || stopWatchMouse.ElapsedMilliseconds > btnInvert.Interval() / 2)
            {
                stopWatchMouse.Restart();
                InvertLevelPieces();
            }
        }

        private void btnInvertPieces_MouseUp(object sender, MouseEventArgs e)
        {
            PullFocusFromTextInputs();
        }

        private void btnFlipPieces_Click(object sender, EventArgs e)
        {
            if (!btnFlip.IsRepeatedAction || stopWatchMouse.ElapsedMilliseconds > btnFlip.Interval() / 2)
            {
                stopWatchMouse.Restart();
                FlipLevelPieces();
            }
        }

        private void btnFlipPieces_MouseUp(object sender, MouseEventArgs e)
        {
            PullFocusFromTextInputs();
        }


        private void btnMoveFront_Click(object sender, EventArgs e)
        {
            MovePieceIndex(true, false);
            PullFocusFromTextInputs();
        }

        private void btnMoveBack_Click(object sender, EventArgs e)
        {
            MovePieceIndex(false, false);
            PullFocusFromTextInputs();
        }

        private void btnMoveFrontOne_Click(object sender, EventArgs e)
        {
            if (!btnDrawLater.IsRepeatedAction || stopWatchMouse.ElapsedMilliseconds > btnDrawLater.Interval() / 2)
            {
                stopWatchMouse.Restart();
                MovePieceIndex(true, true);
            }
        }

        private void btnMoveFrontOne_MouseUp(object sender, MouseEventArgs e)
        {
            PullFocusFromTextInputs();
        }

        private void btnMoveBackOne_Click(object sender, EventArgs e)
        {
            if (!btnDrawSooner.IsRepeatedAction || stopWatchMouse.ElapsedMilliseconds > btnDrawSooner.Interval() / 2)
            {
                stopWatchMouse.Restart();
                MovePieceIndex(false, true);
            }
        }

        private void btnMoveBackOne_MouseUp(object sender, MouseEventArgs e)
        {
            PullFocusFromTextInputs();
        }

        private void btnGroupSelection_Click(object sender, EventArgs e)
        {
            GroupSelectedPieces();
            PullFocusFromTextInputs();
        }

        private void btnUngroupSelection_Click(object sender, EventArgs e)
        {
            UngroupSelectedPieces();
            PullFocusFromTextInputs();
        }


        private void check_Pieces_Erase_CheckedChanged(object sender, EventArgs e)
        {
            bool isChecked = (checkErase.CheckState == CheckState.Checked);
            SetErase(isChecked);
            PullFocusFromTextInputs();
        }

        private void check_Pieces_NoOv_CheckedChanged(object sender, EventArgs e)
        {
            bool isChecked = (checkNoOverwrite.CheckState == CheckState.Checked);
            SetNoOverwrite(isChecked);
            PullFocusFromTextInputs();
        }

        private void check_Pieces_OnlyOnTerrain_CheckedChanged(object sender, EventArgs e)
        {
            bool isChecked = (checkOnlyOnTerrain.CheckState == CheckState.Checked);
            SetOnlyOnTerrain(isChecked);
            PullFocusFromTextInputs();
        }

        private void check_Pieces_OneWay_CheckedChanged(object sender, EventArgs e)
        {
            bool isChecked = (checkAllowOneWay.CheckState == CheckState.Checked);
            SetOneWay(isChecked);
            PullFocusFromTextInputs();
        }

        private void checkSkill_CheckedChanged(object sender, EventArgs e)
        {
            C.Skill skill = checkboxesSkillFlags.First(check => check.Value.Equals((CheckBox)sender)).Key;
            bool isChecked = ((CheckBox)sender).CheckState == CheckState.Checked;
            SetSkillForObjects(skill, isChecked);
            PullFocusFromTextInputs();
            picLevel.SetImage(curRenderer.CreateLevelImage());
        }

        private void num_Resize_Width_ValueChanged(object sender, EventArgs e)
        {
            int newWidth = (int)numResizeWidth.Value;
            CurLevel.SelectionList()
                    .ForEach(obj => obj.SpecWidth = newWidth);
            picLevel.SetImage(curRenderer.CreateLevelImage());
        }

        private void num_Resize_Height_ValueChanged(object sender, EventArgs e)
        {
            int newHeight = (int)numResizeHeight.Value;
            CurLevel.SelectionList()
                    .ForEach(obj => obj.SpecHeight = newHeight);
            picLevel.SetImage(curRenderer.CreateLevelImage());
        }

        private void cbDecorationDirection_SelectedIndexChanged(object sender, EventArgs e)
        {
            int newDir = comboDecorationDirection.SelectedIndex * 45 / 2;
            CurLevel.SelectionList()
                .FindAll(item => item.ObjType == C.OBJ.DECORATION)
                .ForEach(obj => (obj as GadgetPiece).DecorationAngle = newDir);
            SaveChangesToOldLevelList();
        }

        private void numDecorationSpeed_ValueChanged(object sender, EventArgs e)
        {
            int newSpeed = (int)numDecorationSpeed.Value;
            CurLevel.SelectionList()
                .FindAll(item => item.ObjType == C.OBJ.DECORATION)
                .ForEach(obj => (obj as GadgetPiece).DecorationSpeed = newSpeed);
            SaveChangesToOldLevelList();
        }

        private void num_SR_Countdown_ValueChanged(object sender, EventArgs e)
        {
            int countdownLength = (int)numCountdown.Value;
            CurLevel.SetCountdownLength(countdownLength);
            picLevel.SetImage(curRenderer.CreateLevelImage());
        }

        private void btnPairTeleporter_Click(object sender, EventArgs e)
        {
            PairTeleporters();
            PullFocusFromTextInputs();
            picLevel.SetImage(curRenderer.CreateLevelImage());
        }

        private void num_PickupSkillCount_ValueChanged(object sender, EventArgs e)
        {
            int newSkillCount = (int)numPickupSkillCount.Value;
            CurLevel.SetPickupSkillCount(newSkillCount);
            picLevel.SetImage(curRenderer.CreateLevelImage());
        }

        private void num_LemmingLimit_ValueChanged(object sender, EventArgs e)
        {
            int newLimit = (int)numLemmingLimit.Value;
            CurLevel.SetLemmingLimit(newLimit);
            picLevel.SetImage(curRenderer.CreateLevelImage());
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
            Style newStyle = ValidateStyleName(comboPieceStyle.Text);

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
            Style newStyle = ValidateStyleName(comboPieceStyle.Text);

            if (newStyle == null)
            {
                pieceCurStyle = null;
                pieceStartIndex = 0;
                ClearPiecesPictureBox();
            }
        }

        private void btnPieceTerr_Click(object sender, EventArgs e)
        {
            CyclePieceBrowserDisplay(C.SelectPieceType.Terrain);
            PullFocusFromTextInputs();
        }

        private void btnPieceSteel_Click(object sender, EventArgs e)
        {
            CyclePieceBrowserDisplay(C.SelectPieceType.Steel);
            PullFocusFromTextInputs();
        }

        private void btnPieceObj_Click(object sender, EventArgs e)
        {
            CyclePieceBrowserDisplay(C.SelectPieceType.Objects);
            PullFocusFromTextInputs();
        }

        private void btnPieceBackground_Click(object sender, EventArgs e)
        {
            CyclePieceBrowserDisplay(C.SelectPieceType.Backgrounds);
            PullFocusFromTextInputs();
        }

        private void btnPieceRuler_Click(object sender, EventArgs e)
        {
            CyclePieceBrowserDisplay(C.SelectPieceType.Rulers);
            PullFocusFromTextInputs();
        }

        private void btnPieceLeft_MouseUp(object sender, MouseEventArgs e)
        {
            PullFocusFromTextInputs();
        }

        private void btnPieceLeft_Click(object sender, EventArgs e)
        {
            stopWatchMouse.Restart();
            MoveTerrPieceSelection(-1);
        }

        private void btnPieceRight_MouseUp(object sender, MouseEventArgs e)
        {
            PullFocusFromTextInputs();
        }

        private void btnPieceRight_Click(object sender, EventArgs e)
        {
            stopWatchMouse.Restart();
            MoveTerrPieceSelection(1);
        }

        private void picPieces_Click(object sender, EventArgs e)
        {
            int picIndex = picPieceList.FindIndex(pic => pic.Equals(sender));
            Debug.Assert(picIndex != -1, "PicBox not found in picPieceList.");

            var singlePieceSelected = CurLevel.SelectionList().Count == 1;

            Point pos = curRenderer.GetCenterPoint();
            bool useSelectedPiecePosition = false;

            if (singlePieceSelected)
            {
                var selectedPiece = CurLevel.SelectionList()[0];

                // Use selected piece position
                if (isCtrlPressed || isShiftPressed || isAltPressed)
                {
                    pos = new Point(selectedPiece.PosX, selectedPiece.PosY);
                    useSelectedPiecePosition = true;
                }

                // Replace selected piece
                if (isCtrlPressed || isShiftPressed)
                {
                    DeleteSelectedPieces(false);
                }
            }

            AddNewPieceToLevel(picIndex, pos, useSelectedPiecePosition);
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
                    picDragNewPiece.Width = ImageLibrary.GetWidth(dragNewPieceKey);
                    picDragNewPiece.Height = ImageLibrary.GetHeight(dragNewPieceKey);
                    picDragNewPiece.Image = ImageLibrary.GetImage(dragNewPieceKey);

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

            // Handle Enter/Esc for crop
            if (curRenderer.CropTool.Active)
            {
                switch (e.KeyCode)
                {
                    case Keys.Enter:
                        ApplyLevelCrop();
                        break;
                    case Keys.Escape:
                        HandleCropLevel();
                        break;
                }
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

            if (ActiveControl != txtFocus)
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
                case var key when key == HotkeyConfig.GetHotkey(HotkeyConfig.HotkeyName.HotkeyScrollHorizontally).CurrentKeys:
                    scrollHorizontallyPressed = false;
                    break;
                case var key when key == HotkeyConfig.GetHotkey(HotkeyConfig.HotkeyName.HotkeyScrollVertically).CurrentKeys:
                    scrollVerticallyPressed = false;
                    break;
                case var key when key == HotkeyConfig.GetHotkey(HotkeyConfig.HotkeyName.HotkeyDragToScroll).CurrentKeys:
                    dragToScrollPressed = false;
                    break;
                case var key when key == HotkeyConfig.GetHotkey(HotkeyConfig.HotkeyName.HotkeyDragHorizontally).CurrentKeys:
                    dragHorizontallyPressed = false;
                    break;
                case var key when key == HotkeyConfig.GetHotkey(HotkeyConfig.HotkeyName.HotkeyDragVertically).CurrentKeys:
                    dragVerticallyPressed = false;
                    break;
                case var key when key == HotkeyConfig.GetHotkey(HotkeyConfig.HotkeyName.HotkeyMoveScreenStart).CurrentKeys:
                    dragScreenStartPressed = false;
                    break;
                case var key when key == HotkeyConfig.GetHotkey(HotkeyConfig.HotkeyName.HotkeyRemovePiecesAtCursor).CurrentKeys:
                    removeAllPiecesAtCursorPressed = false;
                    break;
                case var key when key == HotkeyConfig.GetHotkey(HotkeyConfig.HotkeyName.HotkeyAddRemoveSinglePiece).CurrentKeys:
                    addOrRemoveSinglePiecePressed = false;
                    break;
                case var key when key == HotkeyConfig.GetHotkey(HotkeyConfig.HotkeyName.HotkeySelectPiecesBelow).CurrentKeys:
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
                    Point mousePosRelPicLevel = picLevel.PointToClient(this.PointToScreen(e.Location));
                    curRenderer.SetZoomMousePos(mousePosRelPicLevel);
                    curRenderer.ChangeZoom(movement > 0 ? 1 : -1, true);
                }
            }

            // Update level image
            RepositionPicLevel();
            picLevel.SetImage(curRenderer.GetScreenImage());

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
            if (curRenderer.CropTool.Active)
            {
                curRenderer.CropTool.MouseDown(e.Location);
                picLevel.SetImage(curRenderer.GetScreenImage());
                return;
            }

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
                    picLevel.SetImage(curRenderer.GetScreenImage());
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
                picLevel.SetImage(curRenderer.GetScreenImage());
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
            if (curRenderer.CropTool.Active)
            {
                curRenderer.CropTool.MouseUp();
                picLevel.SetImage(curRenderer.GetScreenImage());
                return;
            }

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
                        Point mousePicBoxPos = picLevel.PointToClient(MousePosition);
                        if (curRenderer.IsPointInLevelArea(mousePicBoxPos))
                        {
                            Point mouseLevelPos = curRenderer.GetMousePosInLevel(mousePicBoxPos);
                            AddNewPieceToLevel(dragNewPieceKey, mouseLevelPos);
                        }
                        dragNewPieceTimer.Enabled = false;
                        picDragNewPiece.Visible = false;
                        break;
                    }
            }

            curRenderer.DeleteDraggingVars();
            picLevel.SetImage(curRenderer.CreateLevelImage());
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
            if (curRenderer.CropTool.Active)
            {
                curRenderer.CropTool.MouseMove(e.Location, curSettings);
                picLevel.SetImage(curRenderer.GetScreenImage());
                return;
            }

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
                    picLevel.SetImage(curRenderer.GetScreenImage());
                    break;

                case C.DragActions.MoveEditorPos:
                    curRenderer.UpdateScreenPos();
                    UpdateScrollBarValues();
                    picLevel.SetImage(curRenderer.GetScreenImage());
                    break;

                case C.DragActions.MaybeDragPieces:
                    curRenderer.ConfirmDrag();
                    DragSelectedPieces();
                    picLevel.SetImage(curRenderer.CreateLevelImage());
                    break;

                case C.DragActions.DragPieces:
                    DragSelectedPieces();
                    picLevel.SetImage(curRenderer.CreateLevelImage());
                    break;

                case C.DragActions.HorizontalDrag:
                    XDragSelectedPieces();
                    picLevel.SetImage(curRenderer.CreateLevelImage());
                    break;

                case C.DragActions.VerticalDrag:
                    YDragSelectedPieces();
                    picLevel.SetImage(curRenderer.CreateLevelImage());
                    break;

                case C.DragActions.MoveStartPos:
                    Point newCenter = curRenderer.GetNewPosFromDragging();
                    MoveScreenStartPosition(newCenter);
                    picLevel.SetImage(curRenderer.GetScreenImage());
                    break;
            }

            picLevel.Refresh();
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
                if (currentHotkey == HotkeyConfig.GetHotkey(HotkeyConfig.HotkeyName.HotkeyDragHorizontally).CurrentKeys)
                {
                    dragAction = C.DragActions.HorizontalDrag;
                    Cursor = Cursors.SizeWE;
                }
                else if (currentHotkey == HotkeyConfig.GetHotkey(HotkeyConfig.HotkeyName.HotkeyDragVertically).CurrentKeys)
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

        private void btnClearBackground_Click(object sender, EventArgs e)
        {
            CurLevel.Background = null;
            UpdateBackgroundImage();
            picLevel.SetImage(curRenderer.CombineLayers());
        }

        private void btnRandomID_Click(object sender, EventArgs e)
        {
            CurLevel.LevelID = (ulong)Utility.Random().Next() +
                               ((ulong)Utility.Random().Next() << 32);
            txtLevelID.Text = CurLevel.LevelID.ToString("X16");
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
            picLevel.SetImage(curRenderer.GetScreenImage());
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
            Cursor = Cursors.Default;
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
            if (numRandomMinLimit.Value > numRandomMaxLimit.Value)
            {
                numRandomMaxLimit.Value = numRandomMinLimit.Value + 1;
            }
        }

        private void NLEditForm_Shown(object sender, EventArgs e)
        {
            SetHotkeys();
            UpdateCropButtons();

            if (curSettings.AllTabsExpanded)
                ExpandAllTabs();

            if (curSettings.LevelArranger.IsOpen)
                OpenLevelArrangerWindow();

            if (curSettings.PieceBrowser.IsOpen)
                OpenPieceBrowserWindow();

            if (curSettings.ShowAboutAtStartup)
                ShowAboutSLXEditor();

            SetMetaDataPanel();
            MoveControlsOnFormResize();
            UpdateMissingPiecesMenuItems();
            LinkControlsToMouseEvents(this);
            UpdateControlTags();
        }

        private void whatsNewToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ShowAboutSLXEditor();
        }

        private void btnSearchPieces_Click(object sender, EventArgs e)
        {
            OpenPieceSearch();
        }

        private void searchPiecesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenPieceSearch();
        }

        private void btnLoadStyle_Click(object sender, EventArgs e)
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

        private void combo_CustomSkillset_SelectedIndexChanged(object sender, EventArgs e)
        {
            ApplyCustomSkillset();
        }

        private void btnSaveAsCustomSkillset_Click(object sender, EventArgs e)
        {
            SaveSkillsetAsCustom();
        }

        private void btnClearAllSkills_Click(object sender, EventArgs e)
        {
            SetAllSkillsToZero();
        }

        private void refreshStylesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            RefreshStyles();
        }

        private void styleManagerToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenStyleManager();
        }

        private void btnStyleRandom_Click(object sender, EventArgs e)
        {
            RandomizePieceStyle();
        }

        private void HandleSpawnIntervalNumerics(object sender, EventArgs e)
        {
            if (sender == numSI)
                numRR.Value = 103 - numSI.Value;

            if (sender == numRR)
                numSI.Value = 103 - numRR.Value;
        }

        private void exportAsINIToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenExportAsINI();
        }
        private void batchExportToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenBatchExporter();
        }

        private void btnLemCount_Click(object sender, EventArgs e)
        {
            CurLevel.GetLemmingTypeCounts(out int normalCount, out int zombieCount, out int rivalCount, out int neutralCount);

            int totalCount = normalCount + zombieCount + rivalCount + neutralCount;
            int possibleSaveCount = normalCount + rivalCount + neutralCount;

            if (totalCount == 0 || possibleSaveCount == 0)
            {
                MessageBox.Show("To use Automatic Lemming Count, please add at least 1 Entrance Hatch or Pre-placed Lemming to the level.",
                                "Automatic Lemming Count");
                return;
            }

            if (totalCount != numLemmings.Value)
                numLemmings.Value = totalCount;

            if (possibleSaveCount < numRescue.Value)
                numRescue.Value = possibleSaveCount;
        }

        private void btnCropLevel_Click(object sender, EventArgs e)
        {
            HandleCropLevel();
        }

        private void btnCancelCrop_Click(object sender, EventArgs e)
        {
            HandleCropLevel();
        }

        private void btnApplyCrop_Click(object sender, EventArgs e)
        {
            ApplyLevelCrop();
        }

        private void openTemplateToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenTemplatesLoader();
        }

        private void saveAsTemplateToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveLevelAsTemplate();
        }

        private void Control_MouseEnter(object sender, EventArgs e)
        {
            UpdateControlHintLabel(true, sender);
        }

        private void Control_MouseLeave(object sender, EventArgs e)
        {
            UpdateControlHintLabel(false, sender);
        }
    }
}
