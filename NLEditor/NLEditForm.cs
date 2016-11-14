using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Diagnostics;

namespace NLEditor
{
    public partial class NLEditForm : Form
    {
        /*---------------------------------------------------------
         *   Main Form: This part defines the variables 
         *     and reads all the user input
         * -------------------------------------------------------- */
        
        public NLEditForm()
        {
            InitializeComponent();
            this.MouseWheel += new System.Windows.Forms.MouseEventHandler(NLEditForm_MouseWheel);

            // Set list of all piectures for single pieces
            PictureBox[] PicBoxArr = { this.picPiece0, this.picPiece1, this.picPiece2, this.picPiece3,
                                       this.picPiece4, this.picPiece5, this.picPiece6, this.picPiece7 };
            fpicPieceList = new List<PictureBox>(PicBoxArr);

            // Create the list of all styles
            CreateStyleList();
            if (StyleList.Count > 0)
            {
                this.combo_MainStyle.Items.AddRange(StyleList.ConvertAll(sty => sty.Name).ToArray());
                this.combo_MainStyle.SelectedIndex = 0;

                this.combo_PieceStyle.Items.AddRange(StyleList.ConvertAll(sty => sty.Name).ToArray());
                this.combo_PieceStyle.SelectedIndex = 0;
            }

            // Create a new level and a new renderer
            CreateNewLevel();

            // Load pieces into the picPieces
            fPieceStartIndex = 0;
            fPieceDoDisplayObject = false;
            fPieceCurStyle = ValidateStyleName(this, this.combo_PieceStyle.SelectedItem.ToString());
            LoadPiecesIntoPictureBox(this, PieceCurStyle);

            fStopWatch = new Stopwatch();
            fStopWatch.Start();
        }

        List<PictureBox> fpicPieceList;

        List<Style> fStyleList;
        Style fPieceCurStyle;
        int fPieceStartIndex;
        bool fPieceDoDisplayObject;

        Level fCurLevel;
        Renderer fCurRenderer;

        Stopwatch fStopWatch;
          
        public List<PictureBox> picPieceList { get { return fpicPieceList; } }
        public List<Style> StyleList { get { return fStyleList; } }
        public Style PieceCurStyle { get { return fPieceCurStyle; } private set { fPieceCurStyle = value; } }
        public int PieceStartIndex { get { return fPieceStartIndex; } set { fPieceStartIndex = value; } }
        public bool PieceDoDisplayObject { get { return fPieceDoDisplayObject; } private set { fPieceDoDisplayObject = value; } }
        public Level CurLevel { get { return fCurLevel; } }

        private void NLEditForm_Click(object sender, EventArgs e)
        {
            this.ActiveControl = this.menuStrip; // remove focus
        }

        private void tabLvlProperties_Click(object sender, EventArgs e)
        {
            this.ActiveControl = this.menuStrip; // remove focus
        }
        
        /* -----------------------------------------------------------
         *              Menu Items
         * ----------------------------------------------------------- */


        private void newToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Create new level
            CreateNewLevel();
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ExitEditor();
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

        private void combo_MainStyle_Leave(object sender, EventArgs e)
        {
            // can't really do much here
            this.ActiveControl = this.menuStrip; // remove focus
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

        private void num_Lvl_SizeX_Leave(object sender, EventArgs e)
        {
            this.ActiveControl = this.menuStrip; // remove focus
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
        }

        private void but_PieceLeft_MouseDown(object sender, MouseEventArgs e)
        {
            int Movement = (e.Button == MouseButtons.Right) ? -8 : -1;
            MoveTerrPieceSelection(Movement);
            this.ActiveControl = this.menuStrip; // remove focus
        }

        private void but_PieceRight_MouseDown(object sender, MouseEventArgs e)
        {
            int Movement = (e.Button == MouseButtons.Right) ? 8 : 1;
            MoveTerrPieceSelection(Movement);
            this.ActiveControl = this.menuStrip; // remove focus
        }

        private void picPiece0_Click(object sender, EventArgs e)
        {
            AddNewTerrainPiece(0);
            this.ActiveControl = this.menuStrip; // remove focus
        }

        private void picPiece1_Click(object sender, EventArgs e)
        {
            AddNewTerrainPiece(1);
            this.ActiveControl = this.menuStrip; // remove focus
        }

        private void picPiece2_Click(object sender, EventArgs e)
        {
            AddNewTerrainPiece(2);
            this.ActiveControl = this.menuStrip; // remove focus
        }

        private void picPiece3_Click(object sender, EventArgs e)
        {
            AddNewTerrainPiece(3);
            this.ActiveControl = this.menuStrip; // remove focus
        }

        private void picPiece4_Click(object sender, EventArgs e)
        {
            AddNewTerrainPiece(4);
            this.ActiveControl = this.menuStrip; // remove focus
        }

        private void picPiece5_Click(object sender, EventArgs e)
        {
            AddNewTerrainPiece(5);
            this.ActiveControl = this.menuStrip; // remove focus
        }

        private void picPiece6_Click(object sender, EventArgs e)
        {
            AddNewTerrainPiece(6);
            this.ActiveControl = this.menuStrip; // remove focus
        }

        private void picPiece7_Click(object sender, EventArgs e)
        {
            AddNewTerrainPiece(7);
            this.ActiveControl = this.menuStrip; // remove focus
        }


        /* -----------------------------------------------------------
         *              Direct Key and Mouse imput
         * ----------------------------------------------------------- */

        private void NLEditForm_KeyDown(object sender, KeyEventArgs e)
        {
            if (fStopWatch.ElapsedMilliseconds < 50) return;

            // The main key-handling routine
            if (e.KeyCode == Keys.Escape || (e.Alt && e.KeyCode == Keys.F4))
            {
                ExitEditor();
            }
            else if (e.Control && e.KeyCode == Keys.N)
            {
                CreateNewLevel();
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
            else
            {
                return; // and don't restart the StopWatch
            }

            fStopWatch.Restart();
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






    }
}
