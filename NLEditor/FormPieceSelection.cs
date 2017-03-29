using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using System.Diagnostics;

namespace NLEditor
{
    public partial class FormPieceSelection : Form
    {
        public FormPieceSelection(NLEditForm editorForm, Style style, bool doDisplayObjects, int startIndex, 
                                  Point mousePosInLevel, Style mainStyle = null)
        {
            InitializeComponent();

            stopWatchKey = new Stopwatch();
            stopWatchKey.Start();

            this.editorForm = editorForm;
            this.style = style;
            this.doDisplayObjects = doDisplayObjects;
            this.mainStyle = mainStyle;
            this.curIndex = Math.Max(Math.Min(startIndex - 3, pieceKeys.Count - 6), 0);
            this.mousePosInLevel = mousePosInLevel;

            picPieceList = new List<PictureBox>
                { picSelPiece0, picSelPiece1, picSelPiece2, picSelPiece3, picSelPiece4, picSelPiece5 };

            lblPieceList = new List<Label>
                { lblPieceSel0, lblPieceSel1, lblPieceSel2, lblPieceSel3, lblPieceSel4, lblPieceSel5 };

            scrollPieceSelect.Maximum = Math.Max(pieceKeys.Count - 6, 0);
            scrollPieceSelect.Value = curIndex;

            UpdatePiecePictures();
        }

        Stopwatch stopWatchKey;

        readonly List<PictureBox> picPieceList;
        readonly List<Label> lblPieceList;

        readonly NLEditForm editorForm;

        readonly Style style;
        readonly Style mainStyle; // only for recoloring OWWs
        bool doDisplayObjects;
        List<string> pieceKeys => doDisplayObjects ? style.ObjectKeys : style.TerrainKeys;
        int curIndex;

        readonly Point mousePosInLevel;

        private void ClosePieceSelection()
        {
            foreach (Control control in this.Controls)
            {
                this.Controls.Remove(control);
                control.Dispose();
            }

            this.Close();
            this.Dispose();
        }


        /// <summary>
        /// Displays the correct picutures, descriptions and sets the scroll bar correctly.
        /// </summary>
        private void UpdatePiecePictures()
        {
            for (int picIndex = 0; picIndex < 6; picIndex++)
            {
                if (curIndex + picIndex >= pieceKeys.Count)
                {
                    lblPieceList[picIndex].Text = "";
                    picPieceList[picIndex].BackColor = mainStyle?.GetColor(C.StyleColor.BACKGROUND) ?? C.NLColors[C.NLColor.BackDefault];
                    picPieceList[picIndex].Image = null;
                }
                else
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
                        pieceImage = BmpModify.RecolorOWW(pieceImage, mainStyle);
                    }
                    picPieceList[picIndex].BackColor = mainStyle?.GetColor(C.StyleColor.BACKGROUND) ?? C.NLColors[C.NLColor.BackDefault];
                    picPieceList[picIndex].Image = pieceImage;
                }
            }

            scrollPieceSelect.Value = curIndex;
        }

        /// <summary>
        /// Changes the current index by a given value;
        /// </summary>
        /// <param name="delta"></param>
        private void ChangeCurIndex(int delta)
        {
            SetCurIndex(curIndex + delta);
        }

        /// <summary>
        /// Sets the current index to a new value;
        /// </summary>
        /// <param name="newCurIndex"></param>
        private void SetCurIndex(int newCurIndex)
        {
            curIndex = Math.Max(Math.Min(newCurIndex, pieceKeys.Count - 6), 0);
            UpdatePiecePictures();
        }

        private void picSelPiece_Click(object sender, EventArgs e)
        {
            int picIndex = curIndex + picPieceList.FindIndex(pic => pic.Equals(sender));
            if (picIndex >= pieceKeys.Count) return;

            editorForm.AddNewPieceToLevel(pieceKeys[picIndex], mousePosInLevel);
            ClosePieceSelection();
        }

        private void FormPieceSelection_KeyDown(object sender, KeyEventArgs e)
        {
            if (stopWatchKey.ElapsedMilliseconds < 50) return;

            // The main key-handling routine
            if (e.KeyCode == Keys.Escape || (e.Alt && e.KeyCode == Keys.F4))
            {
                ClosePieceSelection();
            }
            else if (e.KeyCode == Keys.Up)
            {
                ChangeCurIndex(-1);
            }
            else if (e.KeyCode == Keys.Down)
            {
                ChangeCurIndex(1);
            }
            else if (e.KeyCode == Keys.Space)
            {
                doDisplayObjects = !doDisplayObjects;
                if (pieceKeys.Count == 0)
                {
                    // revert change
                    doDisplayObjects = !doDisplayObjects;
                    return;
                }
                curIndex = Math.Max(Math.Min(curIndex, pieceKeys.Count - 6), 0);
                scrollPieceSelect.Maximum = Math.Max(pieceKeys.Count - 6, 0);
                scrollPieceSelect.Value = Math.Min(scrollPieceSelect.Value, scrollPieceSelect.Maximum);
                UpdatePiecePictures();
            }
            else
            {
                return; // and don't restart the StopWatch
            }

            stopWatchKey.Restart();
        }

        private void FormPieceSelection_MouseWheel(object sender, MouseEventArgs e)
        {
            int delta = -e.Delta / SystemInformation.MouseWheelScrollDelta;
            ChangeCurIndex(delta);
        }

        private void scrollPieceSelect_Scroll(object sender, ScrollEventArgs e)
        {
            SetCurIndex(scrollPieceSelect.Value);
        }

        private void FormPieceSelection_Deactivate(object sender, EventArgs e)
        {
            ClosePieceSelection();
        }
    }
}
