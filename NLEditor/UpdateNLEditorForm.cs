using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Windows.Forms;

namespace NLEditor
{
    public partial class NLEditForm
    {
        /*---------------------------------------------------------
        *   Main Form: This part defines the methods
        *     updating the form members
        * -------------------------------------------------------- */
        /// <summary>
        /// Initializes the intervals for all repeat buttons.
        /// </summary>
        private void SetRepeatButtonIntervals()
        {
            but_RotatePieces.SetInterval(1000);
            but_InvertPieces.SetInterval(1000);
            but_FlipPieces.SetInterval(1000);
            but_MoveBack.SetInterval(1000);
            but_MoveFront.SetInterval(1000);
            but_MoveBackOne.SetInterval(150);
            but_MoveFrontOne.SetInterval(150);
            but_PieceLeft.SetInterval(100, MouseButtons.Left);
            but_PieceLeft.SetInterval(300, MouseButtons.Right);
            but_PieceRight.SetInterval(100, MouseButtons.Left);
            but_PieceRight.SetInterval(300, MouseButtons.Right);
        }
        
        /// <summary>
        /// Displays the correct piece images for the piece selection.
        /// </summary>
        /// <param name="MyForm"></param>
        /// <param name="NewStyle"></param>
        private void LoadPiecesIntoPictureBox()
        {
            if (fPieceCurStyle == null)
            {
                ClearPiecesPictureBox();
                return;
            }

            // Get correct list of piece names
            List<string> ThisPieceNameList = fPieceDoDisplayObject ? fPieceCurStyle.ObjectNames : fPieceCurStyle.TerrainNames;
            if (ThisPieceNameList == null || ThisPieceNameList.Count == 0)
            {
                ClearPiecesPictureBox();
                return;
            }

            // load correct pictures
            for (int i = 0; i < picPieceList.Count; i++)
            {
                string ThisPieceName = ThisPieceNameList[(fPieceStartIndex + i) % ThisPieceNameList.Count];
                picPieceList[i].Image = ImageLibrary.GetImage(ThisPieceName);
            }

            return;
        }

        /// <summary>
        /// Returns a style with the requested name, or null if none such is found. 
        /// </summary>
        /// <param name="MyForm"></param>
        /// <param name="NewStyleName"></param>
        /// <returns></returns>
        private Style ValidateStyleName(string NewStyleName)
        {
            if (StyleList == null || StyleList.Count == 0) return null;

            return StyleList.Find(sty => sty.NameInEditor == NewStyleName);
        }

        /// <summary>
        /// Clears all piece selection PictureBoxes.
        /// </summary>
        /// <param name="MyForm"></param>
        private void ClearPiecesPictureBox()
        {
            picPieceList.ForEach(pic => pic.Image = null);
        }

        /// <summary>
        /// Updates the background color of the main level image and the piece selection according to the current main style.
        /// </summary>
        /// <param name="MyForm"></param>
        /// <param name="NewStyle"></param>
        private void UpdateBackgroundColor()
        {
            if (fCurLevel.MainStyle == null) return;

            Color NewBackColor = fCurLevel.MainStyle.BackgroundColor;
            if (NewBackColor == null) return;

            picPieceList.ForEach(pic => pic.BackColor = NewBackColor);

            // recreate level with the new background color (assuming we already have a renderer)
            if (fCurRenderer != null)
            {
                this.pic_Level.Image = fCurRenderer.CreateLevelImage();
            }
        }

        /// <summary>
        /// Enables actionable commands for selected pieces and sets checkbox checks correctly.
        /// </summary>
        private void UpdateFlagsForPieceActions()
        {
            List<LevelPiece> SelectionList = fCurLevel.SelectionList();

            but_RotatePieces.Enabled = SelectionList.Exists(p => p.MayRotate());
            but_FlipPieces.Enabled = SelectionList.Exists(p => p.MayFlip());
            but_InvertPieces.Enabled = SelectionList.Exists(p => p.MayInvert());


            but_MoveBack.Enabled = (SelectionList.Count > 0);
            but_MoveFront.Enabled = (SelectionList.Count > 0);
            but_MoveBackOne.Enabled = (SelectionList.Count > 0);
            but_MoveFrontOne.Enabled = (SelectionList.Count > 0);


            check_Pieces_NoOv.Enabled = (SelectionList.Count > 0);
            // Set check-mark correctly, without firing the CheckedChanged event
            check_Pieces_NoOv.CheckedChanged -= check_Pieces_NoOv_CheckedChanged;
            check_Pieces_NoOv.Checked = SelectionList.Exists(p => (p is GadgetPiece && (p as GadgetPiece).IsNoOverwrite) 
                                                               || (p is TerrainPiece && (p as TerrainPiece).IsNoOverwrite));
            check_Pieces_NoOv.CheckedChanged += check_Pieces_NoOv_CheckedChanged;

            check_Pieces_Erase.Enabled = SelectionList.Exists(p => p is TerrainPiece);
            // Set check-mark correctly, without firing the CheckedChanged event
            check_Pieces_Erase.CheckedChanged -= check_Pieces_Erase_CheckedChanged;
            check_Pieces_Erase.Checked = SelectionList.Exists(p => p is TerrainPiece && (p as TerrainPiece).IsErase);
            check_Pieces_Erase.CheckedChanged += check_Pieces_Erase_CheckedChanged;
            
            check_Pieces_OneWay.Enabled = SelectionList.Exists(p => p is TerrainPiece && !(p as TerrainPiece).IsSteel);
            // Set check-mark correctly, without firing the CheckedChanged event
            check_Pieces_OneWay.CheckedChanged -= check_Pieces_OneWay_CheckedChanged;
            check_Pieces_OneWay.Checked = SelectionList.Exists(p => p is TerrainPiece && (p as TerrainPiece).IsOneWay);
            check_Pieces_OneWay.CheckedChanged += check_Pieces_OneWay_CheckedChanged;

            check_Pieces_OnlyOnTerrain.Enabled = SelectionList.Exists(p => p is GadgetPiece);
            // Set check-mark correctly, without firing the CheckedChanged event
            check_Pieces_OnlyOnTerrain.CheckedChanged -= check_Pieces_OnlyOnTerrain_CheckedChanged;
            check_Pieces_OnlyOnTerrain.Checked = SelectionList.Exists(p => p is GadgetPiece && (p as GadgetPiece).IsOnlyOnTerrain);
            check_Pieces_OnlyOnTerrain.CheckedChanged += check_Pieces_OnlyOnTerrain_CheckedChanged;


            for (int Skill = 0; Skill < fcheckSkillFlagList.Count; Skill++)
            {
                fcheckSkillFlagList[Skill].Enabled = SelectionList.Exists(p => p.MayReceiveSkill(Skill));

                // Set check-mark correctly, without firing the CheckedChanged event
                fcheckSkillFlagList[Skill].CheckedChanged -= check_Piece_Skill_CheckedChanged;
                fcheckSkillFlagList[Skill].Checked = SelectionList.Exists(p => p is GadgetPiece && (p as GadgetPiece).HasSkillFlag(Skill));
                fcheckSkillFlagList[Skill].CheckedChanged += check_Piece_Skill_CheckedChanged;
            }

            if (SelectionList.Count == 1 && SelectionList[0] is GadgetPiece)
            { 
                GadgetPiece MyGadget = (GadgetPiece)SelectionList[0];
                lbl_Resize_Width.Visible = MyGadget.ResizeMode.In(C.Resize.Horiz, C.Resize.Both);
                num_Resize_Width.Maximum = fCurLevel.Width;
                num_Resize_Width.Value = Math.Min(Math.Max(MyGadget.SpecWidth, num_Resize_Width.Minimum), num_Resize_Width.Maximum);
                num_Resize_Width.Visible = MyGadget.ResizeMode.In(C.Resize.Horiz, C.Resize.Both);

                lbl_Resize_Height.Visible = MyGadget.ResizeMode.In(C.Resize.Vert, C.Resize.Both);
                num_Resize_Height.Maximum = fCurLevel.Height;
                num_Resize_Height.Value = Math.Min(Math.Max(MyGadget.SpecHeight, num_Resize_Height.Minimum), num_Resize_Height.Maximum);
                num_Resize_Height.Visible = MyGadget.ResizeMode.In(C.Resize.Vert, C.Resize.Both);
            }
            else
            {
                lbl_Resize_Width.Visible = false;
                num_Resize_Width.Visible = false;
                lbl_Resize_Height.Visible = false;
                num_Resize_Height.Visible = false;
            }

        }

        /// <summary>
        /// Repositions the controls after resizing the main form.
        /// </summary>
        private void MoveControlsOnFormResize()
        {
            pic_Level.Width = this.Width - 200;
            pic_Level.Height = this.Height - 155;
            
            tabLvlProperties.Height = this.Height - 178;

            combo_PieceStyle.Top = this.Height - 149;
            but_PieceTerrObj.Top = this.Height - 149;

            but_PieceLeft.Top = this.Height - 122;
            but_PieceRight.Top = this.Height - 122;
            but_PieceRight.Left = this.Width - 44;

            bool UpdateImages = MovePicPiecesOnResize();
            if (UpdateImages)
            {
                UpdateBackgroundColor();
                LoadPiecesIntoPictureBox();
            }
        }

        /// <summary>
        /// Moves the picture boxes to select new pieces to the correct position.
        /// </summary>
        /// <returns></returns>
        private bool MovePicPiecesOnResize()
        {
            fpicPieceList.ForEach(pic => pic.Top = this.Height - 122);

            int NumPicPieces = (this.Width - 170) / 90 + 1;

            while (fpicPieceList.Count > NumPicPieces)
            {
                PictureBox OldPicPieces = fpicPieceList[fpicPieceList.Count - 1];
                fpicPieceList.Remove(OldPicPieces);
                this.Controls.Remove(OldPicPieces);
                OldPicPieces.Dispose();
            }
            
            if (fpicPieceList.Count > NumPicPieces)
            {
                
                fpicPieceList.RemoveRange(NumPicPieces, fpicPieceList.Count - NumPicPieces);
            }

            bool NeedUpdatePicPieceImages = (fpicPieceList.Count < NumPicPieces);
            while (fpicPieceList.Count < NumPicPieces)
            {
                fpicPieceList.Add(CreatePicPiece());
            }

            for (int PicPieceIndex = 0; PicPieceIndex < NumPicPieces; PicPieceIndex++)
            {
                fpicPieceList[PicPieceIndex].Left = 36 + PicPieceIndex * (this.Width - 170) / (NumPicPieces - 1);
            }

            return NeedUpdatePicPieceImages;
        }

        /// <summary>
        /// Creates a new picture box for selecting new pieces.
        /// </summary>
        /// <returns></returns>
        private PictureBox CreatePicPiece()
        { 
            PictureBox NewPicPiece = new PictureBox();
            NewPicPiece.Width = 84;
            NewPicPiece.Height = 84;
            NewPicPiece.Top = this.Height - 122;
            NewPicPiece.BorderStyle = BorderStyle.Fixed3D;
            NewPicPiece.SizeMode = PictureBoxSizeMode.CenterImage;

            NewPicPiece.Click += new EventHandler(picPieces_Click);

            this.Controls.Add(NewPicPiece);

            return NewPicPiece;
        }

        /// <summary>
        /// Displays a list of all hotkeys in a new form window.
        /// </summary>
        private void DisplayHotkeyForm()
        {
            Form HotkeyForm = new Form();
            HotkeyForm.Width = 470;
            HotkeyForm.Height = 470;
            HotkeyForm.MaximizeBox = false;
            HotkeyForm.FormBorderStyle = FormBorderStyle.FixedToolWindow;
            HotkeyForm.Text = "NLEditor - Hotkeys";

            HotkeyForm.Show();

            using (Graphics HotkeyGraphics = HotkeyForm.CreateGraphics())
            using (SolidBrush MyBrush = new SolidBrush(Color.Black))
            using (Font MyFont = new Font("Microsoft Sans Serif", 8))
            {
                for (int i = 0; i < C.HotkeyList.Count; i++)
                {
                    HotkeyGraphics.DrawString(C.HotkeyList[i], MyFont, MyBrush, 6, 6 + 13 * i);
                    HotkeyGraphics.DrawString(C.DescriptionList[i], MyFont, MyBrush, 156, 6 + 13*i);
                }
            }
        }

        /// <summary>
        /// Displays the "About..." page with attribution and license.
        /// </summary>
        private void DisplayVersionForm()
        {
            Form VersionForm = new Form();
            VersionForm.Width = 310;
            VersionForm.Height = 170;
            VersionForm.MaximizeBox = false;
            VersionForm.FormBorderStyle = FormBorderStyle.FixedToolWindow;
            VersionForm.Text = "NLEditor - About";

            VersionForm.Show();

            using (Graphics HotkeyGraphics = VersionForm.CreateGraphics())
            using (SolidBrush MyBrush = new SolidBrush(Color.Black))
            using (Font MyFont = new Font("Microsoft Sans Serif", 8))
            {
                for (int i = 0; i < C.VersionList.Count; i++)
                {
                    HotkeyGraphics.DrawString(C.VersionList[i], MyFont, MyBrush, 6, 6 + 13 * i);
                }
            }
        }



    }
}
