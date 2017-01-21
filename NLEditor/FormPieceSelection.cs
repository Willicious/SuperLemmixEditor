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
    public partial class FormPieceSelection : Form
    {
        public FormPieceSelection(NLEditForm editorForm, List<string> pieceKeys, int startIndex, Style style, 
                                  Point mousePosOnScreen, Point mousePosInLevel)
        {
            InitializeComponent();

            stopWatchKey = new Stopwatch();
            stopWatchKey.Start();

            this.editorForm = editorForm;
            this.pieceKeys = pieceKeys;
            this.style = style;
            this.curIndex = Math.Max(Math.Min(startIndex - 3, pieceKeys.Count - 6), 0);
            this.mousePosInLevel = mousePosInLevel;

            picPieceList = new List<PictureBox>
                { picSelPiece0, picSelPiece1, picSelPiece2, picSelPiece3, picSelPiece4, picSelPiece5 };

            lblPieceList = new List<Label>
                { lblPieceSel0, lblPieceSel1, lblPieceSel2, lblPieceSel3, lblPieceSel4, lblPieceSel5 };


            scrollPieceSelect.Maximum = Math.Max(pieceKeys.Count - 6, 0) + 1;
            scrollPieceSelect.Value = curIndex;

            // Make form smaller, if less pieces are available.
            if (pieceKeys.Count < 6)
            {
                scrollPieceSelect.Enabled = false;
                scrollPieceSelect.Visible = false;
                foreach (var picBox in picPieceList.GetRange(pieceKeys.Count))
                {
                    this.Controls.Remove(picBox);
                    picBox.Dispose();
                }
                foreach (var label in lblPieceList.GetRange(pieceKeys.Count))
                {
                    this.Controls.Remove(label);
                    label.Dispose();
                }

                picPieceList = picPieceList.GetRange(0, pieceKeys.Count);
                lblPieceList = lblPieceList.GetRange(0, pieceKeys.Count);

                this.Width -= 16;
                this.Height = 25 + 66 * pieceKeys.Count;
            }
        }

        Stopwatch stopWatchKey;

        readonly List<PictureBox> picPieceList;
        readonly List<Label> lblPieceList;

        readonly NLEditForm editorForm;

        readonly Style style;
        readonly List<string> pieceKeys;
        int curIndex;

        readonly Point mousePosInLevel; 

        /// <summary>
        /// Displays the correct picutures, descriptions and sets the scroll bar correctly.
        /// </summary>
        private void UpdatePiecePictures()
        {
            for (int picIndex = 0; picIndex < picPieceList.Count; picIndex++)
            {
                string pieceKey = pieceKeys[curIndex + picIndex];
                string pieceDescription = System.IO.Path.GetFileNameWithoutExtension(pieceKey)
                                          + C.NewLine
                                          + C.TooltipList[ImageLibrary.GetObjType(pieceKey)];
                lblPieceList[picIndex].Text = pieceDescription;

                int frameIndex = (ImageLibrary.GetObjType(pieceKey).In(C.OBJ.PICKUP, C.OBJ.EXIT_LOCKED, C.OBJ.BUTTON, C.OBJ.TRAPONCE)) ? 1 : 0;
                Bitmap pieceImage = ImageLibrary.GetImage(pieceKey, RotateFlipType.RotateNoneFlipNone, frameIndex);
                if (pieceKey.StartsWith("default") && ImageLibrary.GetObjType(pieceKey) == C.OBJ.ONE_WAY_WALL)
                {
                    pieceImage = BmpModify.RecolorOWW(pieceImage, style);
                }
                picPieceList[picIndex].Image = pieceImage;
            }

            scrollPieceSelect.Value = curIndex;
        }

        /// <summary>
        /// Changes the current index by a given value;
        /// </summary>
        /// <param name="delta"></param>
        private void ChangeCurIndex(int delta)
        {
            curIndex += delta;
            curIndex = Math.Max(Math.Min(curIndex, pieceKeys.Count - picPieceList.Count), 0);
            UpdatePiecePictures();
        }

        /// <summary>
        /// Sets the current index to a new value;
        /// </summary>
        /// <param name="newCurIndex"></param>
        private void SetCurIndex(int newCurIndex)
        {
            curIndex = Math.Max(Math.Min(newCurIndex, pieceKeys.Count - picPieceList.Count), 0);
            UpdatePiecePictures();
        }

        private void picSelPiece_Click(object sender, EventArgs e)
        {
            // TODO: Add correct piece to selection
        }

        private void FormPieceSelection_KeyDown(object sender, KeyEventArgs e)
        {
            if (stopWatchKey.ElapsedMilliseconds < 50) return;

            // The main key-handling routine
            if (e.KeyCode == Keys.Escape || (e.Alt && e.KeyCode == Keys.F4))
            {
                this.Dispose();
            }
            else if (e.KeyCode == Keys.Up)
            {
                ChangeCurIndex(-1);
            }
            else if (e.KeyCode == Keys.Down)
            {
                ChangeCurIndex(1);
            }
            else
            {
                return; // and don't restart the StopWatch
            }

            stopWatchKey.Restart();
        }

        private void scrollPieceSelect_Scroll(object sender, ScrollEventArgs e)
        {
            SetCurIndex(scrollPieceSelect.Value);
        }
    }
}
