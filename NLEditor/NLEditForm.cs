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
            fPieceCurStyle = ValidateStyleName(this, this.combo_PieceStyle.SelectedItem.ToString());
            LoadPiecesIntoPictureBox(this, PieceCurStyle);

            fStopWatchKey = new Stopwatch();
            fStopWatchKey.Start();
            fStopWatchMouse = new Stopwatch();
            fStopWatchMouse.Start();

            fMouseButtonPressed = null;

            fStopWatchProfiling = new Stopwatch();
        }

        List<PictureBox> fpicPieceList;
        List<CheckBox> fcheckSkillFlagList;

        List<Style> fStyleList;
        Style fPieceCurStyle;
        int fPieceStartIndex;
        bool fPieceDoDisplayObject;

        Level fCurLevel;
        Renderer fCurRenderer;

        Stopwatch fStopWatchKey;
        Stopwatch fStopWatchMouse;
        MouseButtons? fMouseButtonPressed;


        Stopwatch fStopWatchProfiling; // use ONLY for debugging/profiling purposes!

        bool fIsShiftPressed;
        bool fIsCtrlPressed;
        bool fIsAltPressed;
        

        public List<PictureBox> picPieceList { get { return fpicPieceList; } }
        public List<Style> StyleList { get { return fStyleList; } }
        public Level CurLevel { get { return fCurLevel; } }

        // Variables for the piece selection menu
        public Style PieceCurStyle { get { return fPieceCurStyle; } private set { fPieceCurStyle = value; } }
        public int PieceStartIndex { get { return fPieceStartIndex; } set { fPieceStartIndex = value; } }
        public bool PieceDoDisplayObject { get { return fPieceDoDisplayObject; } private set { fPieceDoDisplayObject = value; } }
        

        private void NLEditForm_Click(object sender, EventArgs e)
        {
            RemoveFocus();
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
            UpdateFlagsForPieceActions();
        }

        private void loadToolStripMenuItem_Click(object sender, EventArgs e)
        {
            LoadNewLevel();
            UpdateFlagsForPieceActions();
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
            ExitEditor();
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

        /* -----------------------------------------------------------
         *              Global Level Info Tab
         * ----------------------------------------------------------- */

        private void combo_MainStyle_TextChanged(object sender, EventArgs e)
        {
            Style NewStyle = ValidateStyleName(this, this.combo_MainStyle.Text);

            if (NewStyle == null || CurLevel == null || NewStyle == CurLevel.MainStyle) return;

            // Load new style into PictureBoxes
            CurLevel.MainStyle = NewStyle;
            ChangeBackgroundColor(this, NewStyle);
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
        }


        /* -----------------------------------------------------------
         *              Piece Selection
         * ----------------------------------------------------------- */

        private void combo_PieceStyle_TextChanged(object sender, EventArgs e)
        {
            Style NewStyle = ValidateStyleName(this, this.combo_PieceStyle.Text);

            if (NewStyle == null || NewStyle == PieceCurStyle) return;

            // Load new style into PictureBoxes
            PieceCurStyle = NewStyle;
            PieceStartIndex = 0;
            LoadPiecesIntoPictureBox(this, PieceCurStyle);
        }

        private void combo_PieceStyle_Leave(object sender, EventArgs e)
        {
            // Check whether to delete all pieces due to wrong style name
            Style NewStyle = ValidateStyleName(this, this.combo_PieceStyle.Text);

            if (NewStyle == null)
            {
                PieceCurStyle = null;
                PieceStartIndex = 0;
                ClearPiecesPictureBox(this);           
            }
        }

        private void but_PieceTerrObj_Click(object sender, EventArgs e)
        {
            ChangeObjTerrPieceDisplay();
            RemoveFocus();
        }

        private void but_PieceLeft_MouseDown(object sender, MouseEventArgs e)
        {
            but_PieceLeft.Interval = (e.Button == MouseButtons.Right) ? 300 : 100;
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
                    Movement = -8;
                }
                else
                {
                    Movement = -1;
                }

                MoveTerrPieceSelection(Movement);
            }
        }

        private void but_PieceRight_MouseDown(object sender, MouseEventArgs e)
        {
            but_PieceRight.Interval = (e.Button == MouseButtons.Right) ? 300 : 100;
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
                    Movement = 8;
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
                ExitEditor();
            }
            else if (e.Control && e.KeyCode == Keys.N)
            {
                CreateNewLevelAndRenderer();
                UpdateFlagsForPieceActions();
            }
            else if (e.Control && e.KeyCode == Keys.O)
            {
                LoadNewLevel();
                UpdateFlagsForPieceActions();
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
        }

        private void NLEditForm_MouseWheel(object sender, MouseEventArgs e)
        {
            int Movement = e.Delta / SystemInformation.MouseWheelScrollDelta;
            if (Movement > 0)
            {
                fCurRenderer.ChangeZoom(1);
            }
            else if (Movement < 0)
            {
                fCurRenderer.ChangeZoom(-1);
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
        }

        private void pic_Level_MouseMove(object sender, MouseEventArgs e)
        {
            if (fCurRenderer.MouseStartPos != null)
            {
                // Update selection area
                fCurRenderer.MouseCurPos = e.Location;
                this.pic_Level.Image = fCurRenderer.CombineLayers();
            }
        }

        private void pic_Level_MouseUp(object sender, MouseEventArgs e)
        {
            fCurRenderer.MouseCurPos = e.Location;

            if (fStopWatchMouse.ElapsedMilliseconds < 200) // usual click takes <100ms
            {
                LevelSelectSinglePiece();
            }
            else
            {
                LevelSelectAreaPieces();
            }

            // Delete mouse selection area...
            fCurRenderer.MouseStartPos = null;
            fCurRenderer.MouseCurPos = null;
            // ...before updating the level image
            this.pic_Level.Image = fCurRenderer.CreateLevelImage();
            UpdateFlagsForPieceActions();

            fMouseButtonPressed = null;
            RemoveFocus();
        }





    }
}
