﻿using System;
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
                if (ThisPieceName.StartsWith("default") && ImageLibrary.GetObjType(ThisPieceName) == C.OBJ.ONE_WAY_WALL)
                {
                    picPieceList[i].Image = GetRecoloredOWW(ThisPieceName);
                }
                else
                {
                    int FrameIndex = (ImageLibrary.GetObjType(ThisPieceName).In(C.OBJ.PICKUP, C.OBJ.EXIT_LOCKED, C.OBJ.BUTTON, C.OBJ.TRAPONCE)) ? 1 : 0;
                    picPieceList[i].Image = ImageLibrary.GetImage(ThisPieceName, RotateFlipType.RotateNoneFlipNone, FrameIndex);
                }
                SetToolTipsForPicPiece(picPieceList[i], ThisPieceName);
            }

            return;
        }

        /// <summary>
        /// Returns a recolored OWW according to the OWW color of the current main style.
        /// </summary>
        /// <param name="PieceName"></param>
        /// <returns></returns>
        private Bitmap GetRecoloredOWW(string PieceName)
        { 
            Color OWWColor = Color.Linen;            
            if (fCurLevel.MainStyle != null)
            {
                OWWColor = fCurLevel.MainStyle.GetColor(C.StyleColor.ONE_WAY_WALL);
            }
            BmpModify.SetCustomDrawMode((x, y) => new byte[]{ OWWColor.B, OWWColor.G, OWWColor.R, 255 }, null);
            Bitmap OrigBmp = ImageLibrary.GetImage(PieceName, RotateFlipType.RotateNoneFlipNone);
            Bitmap NewBmp = new Bitmap(OrigBmp.Width, OrigBmp.Height);
            NewBmp.DrawOn(OrigBmp, new Point(0, 0), C.CustDrawMode.Custom);
            return NewBmp;
        }


        /// <summary>
        /// Clears all piece selection PictureBoxes.
        /// </summary>
        /// <param name="MyForm"></param>
        private void ClearPiecesPictureBox()
        {
            picPieceList.ForEach(pic => 
                { 
                    pic.Image = null; 
                    SetToolTipsForPicPiece(pic, null); 
                });
        }

        /// <summary>
        /// Sets the correct tool tips for piece selection picture boxes.
        /// </summary>
        private void SetToolTipsForPicPiece(PictureBox MypicPiece, string PieceKey)
        {
            string ToolTipText = "";
            C.OBJ PieceObjType = (PieceKey == null) ? C.OBJ.NULL : ImageLibrary.GetObjType(PieceKey);
            if (C.TooltipList.ContainsKey(PieceObjType))
            {
                ToolTipText = C.TooltipList[PieceObjType];
            }

            toolTipPieces.SetToolTip(MypicPiece, ToolTipText);
        }

        /// <summary>
        /// Updates the background color of the main level image and the piece selection according to the current main style.
        /// </summary>
        /// <param name="MyForm"></param>
        /// <param name="NewStyle"></param>
        private void UpdateBackgroundImage()
        {
            if (fCurLevel.MainStyle == null) return;

            Color NewBackColor = fCurLevel.MainStyle.GetColor(C.StyleColor.BACKGROUND);
            if (NewBackColor == null) return;

            picPieceList.ForEach(pic => pic.BackColor = NewBackColor);

            // recreate level with the new background color/image (assuming we already have a renderer)
            if (fCurRenderer != null)
            {
                fCurRenderer.CreateBackgroundLayer();
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
                lbl_Resize_Width.Visible = MyGadget.MayResizeHoriz();
                num_Resize_Width.Maximum = fCurLevel.Width;
                num_Resize_Width.Value = Math.Min(Math.Max(MyGadget.SpecWidth, num_Resize_Width.Minimum), num_Resize_Width.Maximum);
                num_Resize_Width.Visible = MyGadget.MayResizeHoriz();

                lbl_Resize_Height.Visible = MyGadget.MayResizeVert();
                num_Resize_Height.Maximum = fCurLevel.Height;
                num_Resize_Height.Value = Math.Min(Math.Max(MyGadget.SpecHeight, num_Resize_Height.Minimum), num_Resize_Height.Maximum);
                num_Resize_Height.Visible = MyGadget.MayResizeVert();
            }
            else
            {
                lbl_Resize_Width.Visible = false;
                num_Resize_Width.Visible = false;
                lbl_Resize_Height.Visible = false;
                num_Resize_Height.Visible = false;
            }

            if (SelectionList.Count == 2
                && SelectionList.Exists(item => item.ObjType == C.OBJ.TELEPORTER)
                && SelectionList.Exists(item => item.ObjType == C.OBJ.RECEIVER))
            {
                GadgetPiece MyTeleporter = (GadgetPiece)SelectionList.Find(item => item.ObjType == C.OBJ.TELEPORTER);
                GadgetPiece MyReceiver = (GadgetPiece)SelectionList.Find(item => item.ObjType == C.OBJ.RECEIVER);

                if (MyTeleporter.Val_L > 0 && MyTeleporter.Val_L == MyReceiver.Val_L)
                {
                    but_PairTeleporter.Text = "Already Paired";
                    but_PairTeleporter.Enabled = false;
                }
                else
                {
                    but_PairTeleporter.Text = "Pair Teleporters";
                    but_PairTeleporter.Enabled = true;
                }
                but_PairTeleporter.Visible = true;
            }
            else 
            {
                but_PairTeleporter.Visible = false;
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
                UpdateBackgroundImage();
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
        /// Updates the possible background images depending on the currently selected main style.
        /// </summary>
        private void UpdateBackgroundComboItems()
        {
            combo_Background.Items.Clear();
            combo_Background.Items.Add("--none--");
            if (CurLevel.MainStyle != null)
            {
                string[] BackgroundNames = CurLevel.MainStyle.BackgroundNames.Select(name => 
                                               System.IO.Path.GetFileName(name)).ToArray();
                combo_Background.Items.AddRange(BackgroundNames);
            }
        }



        /// <summary>
        /// Displays a list of all hotkeys in a new form window.
        /// </summary>
        private void DisplayHotkeyForm()
        {
            Form HotkeyForm = new Form();
            HotkeyForm.Width = 450;
            HotkeyForm.Height = 400;
            HotkeyForm.MaximizeBox = false;
            HotkeyForm.FormBorderStyle = FormBorderStyle.FixedToolWindow;
            HotkeyForm.Text = "NLEditor - Hotkeys";

            TabControl HotkeyTabs = new TabControl();
            HotkeyTabs.Location = new Point(0, 0);
            HotkeyTabs.SelectedIndex = 0;
            HotkeyTabs.Size = new Size(HotkeyForm.Width - 6, HotkeyForm.Height - 23);
            HotkeyTabs.TabIndex = 1;
            HotkeyTabs.TabStop = false;

            TabPage tabHotkeysGeneral = new TabPage("General");
            TabPage tabHotkeysPieces = new TabPage("Piece modification");
            Label lblHotkeysGeneralKeys = new Label();
            lblHotkeysGeneralKeys.Location = new Point(0, 4);
            lblHotkeysGeneralKeys.Size = new Size(150, HotkeyTabs.Height - 4);
            lblHotkeysGeneralKeys.Text = String.Join(C.NewLine, C.HotkeyDict[C.HotkeyTabs.General]);
            Label lblHotkeysGeneralDescription = new Label();
            lblHotkeysGeneralDescription.Location = new Point(150, 4);
            lblHotkeysGeneralDescription.Size = new Size(HotkeyTabs.Width - 154, HotkeyTabs.Height - 4);
            lblHotkeysGeneralDescription.Text = String.Join(C.NewLine, C.DescriptionDict[C.HotkeyTabs.General]);

            Label lblHotkeysPiecesKeys = new Label();
            lblHotkeysPiecesKeys.Location = new Point(0, 4);
            lblHotkeysPiecesKeys.Size = new Size(150, HotkeyTabs.Height - 4);
            lblHotkeysPiecesKeys.Text = String.Join(C.NewLine, C.HotkeyDict[C.HotkeyTabs.Pieces]);
            Label lblHotkeysPiecesDescription = new Label();
            lblHotkeysPiecesDescription.Location = new Point(150, 4);
            lblHotkeysPiecesDescription.Size = new Size(HotkeyTabs.Width - 154, HotkeyTabs.Height - 4);
            lblHotkeysPiecesDescription.Text = String.Join(C.NewLine, C.DescriptionDict[C.HotkeyTabs.Pieces]);

            tabHotkeysGeneral.Controls.Add(lblHotkeysGeneralKeys);
            tabHotkeysGeneral.Controls.Add(lblHotkeysGeneralDescription);
            tabHotkeysPieces.Controls.Add(lblHotkeysPiecesKeys);
            tabHotkeysPieces.Controls.Add(lblHotkeysPiecesDescription);
            HotkeyTabs.Controls.Add(tabHotkeysGeneral);
            HotkeyTabs.Controls.Add(tabHotkeysPieces);
            HotkeyForm.Controls.Add(HotkeyTabs);

            HotkeyForm.Show();
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
