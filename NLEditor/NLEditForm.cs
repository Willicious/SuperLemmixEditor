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

            // Set list of all piectures for single pieces
            PictureBox[] PicBoxArr = { this.picPiece1, this.picPiece2, this.picPiece3, this.picPiece4,
                                       this.picPiece5, this.picPiece6, this.picPiece7, this.picPiece8 };
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

            // Create a new level
            Style NewMainStyle = (StyleList == null || StyleList.Count == 0) ? null : StyleList[0];
            fCurLevel = new Level(NewMainStyle);
            WriteLevelInfoToForm();
            UpdateForm.ChangeBackgroundColor(this, NewMainStyle);

            // Create a new renderer
            fCurRenderer = new Renderer(fCurLevel, this.pic_Level);
            this.pic_Level.Image = fCurRenderer.CreateLevelImage();

            // Load pieces into the picPieces
            fPieceStartIndex = 0;
            fPieceDoDisplayObject = false;
            fPieceCurStyle = UpdateForm.ValidateStyleName(this, this.combo_PieceStyle.SelectedItem.ToString());
            UpdateForm.LoadPiecesIntoPictureBox(this, PieceCurStyle);

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

        
        private void newToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Create new level
            // TODO
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ExitEditor();
        }

        private void combo_PieceStyle_TextChanged(object sender, EventArgs e)
        {
            Style NewStyle = UpdateForm.ValidateStyleName(this, this.combo_PieceStyle.Text);

            if (NewStyle == null || NewStyle == PieceCurStyle) return;

            // Load new style into PictureBoxes
            PieceCurStyle = NewStyle;
            PieceStartIndex = 0;
            UpdateForm.LoadPiecesIntoPictureBox(this, PieceCurStyle);
        }

        private void combo_PieceStyle_Leave(object sender, EventArgs e)
        {
            // Check whether to delete all pieces due to wrong style name
            Style NewStyle = UpdateForm.ValidateStyleName(this, this.combo_PieceStyle.Text);

            if (NewStyle == null)
            {
                PieceCurStyle = null;
                PieceStartIndex = 0;
                UpdateForm.ClearPiecesPictureBox(this);           
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
        }

        private void but_PieceRight_MouseDown(object sender, MouseEventArgs e)
        {
            int Movement = (e.Button == MouseButtons.Right) ? 8 : 1;
            MoveTerrPieceSelection(Movement);
        }

        private void combo_MainStyle_TextChanged(object sender, EventArgs e)
        {
            Style NewStyle = UpdateForm.ValidateStyleName(this, this.combo_MainStyle.Text);

            if (NewStyle == null || CurLevel == null || NewStyle == CurLevel.MainStyle) return;

            // Load new style into PictureBoxes
            CurLevel.MainStyle = NewStyle;
            UpdateForm.ChangeBackgroundColor(this, NewStyle);
        }

        private void combo_MainStyle_Leave(object sender, EventArgs e)
        {
            // can't really do anything here
        }

        private void NLEditForm_KeyDown(object sender, KeyEventArgs e)
        {
            if (fStopWatch.ElapsedMilliseconds < 50) return;

            // The main key-handling routine
            if (e.Alt && e.KeyCode == Keys.Left)
            {
                MoveTerrPieceSelection(-1);
                fStopWatch.Restart();
            }
            else if (e.Alt && e.KeyCode == Keys.Right)
            {
                MoveTerrPieceSelection(1);
                fStopWatch.Restart();
            }
            else if (e.Alt && e.KeyCode == Keys.Up)
            {
                ChangeNewPieceStyleSelection(-1);
                fStopWatch.Restart();
            }
            else if (e.Alt && e.KeyCode == Keys.Down)
            {
                ChangeNewPieceStyleSelection(1);
                fStopWatch.Restart();
            }
        }

    }
}
