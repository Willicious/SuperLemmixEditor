﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using static NLEditor.Settings;

namespace NLEditor
{
    /// <summary>
    /// Main Form: This part defines the methods updating the form members
    /// </summary>
    partial class NLEditForm
    {
        public void ShowMustRestartMessage()
        {
            string modeString;

            if (curSettings.CurrentEditorMode == Settings.EditorMode.NeoLemmix)
                modeString = "NeoLemmix Mode";
            else if (curSettings.CurrentEditorMode == Settings.EditorMode.SuperLemmix)
                modeString = "SuperLemmix Mode";
            else
                modeString = "Auto Mode";

            statusBar.Visible = true;
            statusBarLabel1.Text = "";
            statusBarLabel2.Text = "SuperLemmix Editor must be restarted for " + modeString + " to take effect. " +
                                   "Please save your work and restart the Editor";
            statusBarButton1.Visible = false;
        }
        /// <summary>
        /// Initializes the intervals for all repeat buttons.
        /// </summary>
        private void SetRepeatButtonIntervals()
        {
            but_RotatePieces.SetInterval(1000);
            but_InvertPieces.SetInterval(1000);
            but_FlipPieces.SetInterval(1000);
            but_MoveBackOne.SetInterval(150);
            but_MoveFrontOne.SetInterval(150);
            but_PieceLeft.SetInterval(100, MouseButtons.Left);
            but_PieceLeft.SetInterval(30, MouseButtons.Right);
            but_PieceRight.SetInterval(100, MouseButtons.Left);
            but_PieceRight.SetInterval(30, MouseButtons.Right);
        }

        /// <summary>
        /// Displays the correct piece images for the piece selection.
        /// </summary>
        public void LoadPiecesIntoPictureBox()
        {
            if (pieceCurStyle == null)
            {
                ClearPiecesPictureBox();
                return;
            }

            // Get correct list of piece names
            List<string> pieceKeys;

            switch (pieceDoDisplayKind)
            {
                case C.SelectPieceType.Objects:
                    pieceKeys = pieceCurStyle?.ObjectKeys;
                    break;
                case C.SelectPieceType.Terrain:
                    pieceKeys = pieceCurStyle?.TerrainKeys;
                    break;
                case C.SelectPieceType.Backgrounds:
                    pieceKeys = pieceCurStyle?.BackgroundKeys;
                    break;
                case C.SelectPieceType.Sketches:
                    pieceKeys = Style.SketchKeys;
                    break;
                default:
                    throw new ArgumentException();
            }

            if (pieceKeys == null || pieceKeys.Count == 0)
            {
                ClearPiecesPictureBox();
                return;
            }

            int skipDeprecatedOffset = 0;

            // load correct pictures
            for (int i = 0; i < picPieceList.Count; i++)
            {
                string pieceKey = pieceKeys[(pieceStartIndex + i + skipDeprecatedOffset) % pieceKeys.Count];
                if (!ImageLibrary.IsImageLoadable(pieceKey))
                {
                    // Make sure to stop the repeat-buttons from firing again.
                    but_PieceRight.StopRepeatAction();
                    but_PieceLeft.StopRepeatAction();
                }

                if (ImageLibrary.GetDeprecated(pieceKey) && !DisplaySettings.IsDisplayed(C.DisplayType.Deprecated))
                {
                    skipDeprecatedOffset++;
                    i--;
                    continue;
                }

                int frameIndex = (ImageLibrary.GetObjType(pieceKey).In(C.OBJ.PICKUP, C.OBJ.EXIT_LOCKED, C.OBJ.BUTTON, C.OBJ.COLLECTIBLE, C.OBJ.TRAPONCE)) ? 1 : 0;
                Bitmap pieceImage;

                if (curSettings.UsePieceSelectionNames)
                {
                    pieceImage = ImageLibrary.GetImageWithPieceName(pieceKey, frameIndex);
                }
                else
                {
                    pieceImage = ImageLibrary.GetImage(pieceKey, RotateFlipType.RotateNoneFlipNone, frameIndex);
                }

                if (pieceKey.StartsWith("default") && ImageLibrary.GetObjType(pieceKey) == C.OBJ.ONE_WAY_WALL)
                {
                    pieceImage = pieceImage.ApplyThemeColor(CurLevel.GetThemeColor(C.StyleColor.ONE_WAY_WALL), 255);
                }

                picPieceList[i].Image = pieceImage;
                SetToolTipsForPicPiece(picPieceList[i], pieceKey);
            }

            return;
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
        /// <param name="picPiece"></param>
        /// <param name="pieceKey"></param>
        private void SetToolTipsForPicPiece(PictureBox picPiece, string pieceKey)
        {
            string toolTipText = "unknown";
            C.OBJ pieceObjType = (pieceKey == null) ? C.OBJ.NULL : ImageLibrary.GetObjType(pieceKey);
            if (C.TooltipList.ContainsKey(pieceObjType))
            {
                toolTipText = C.TooltipList[pieceObjType];
            }
            if (pieceKey != null && ImageLibrary.GetWidth(pieceKey) > 0 && ImageLibrary.GetHeight(pieceKey) > 0)
            {
                toolTipText += " (" + ImageLibrary.GetWidth(pieceKey).ToString() + ",";
                toolTipText += ImageLibrary.GetHeight(pieceKey).ToString() + ")";
            }

            toolTipPieces.SetToolTip(picPiece, toolTipText);
        }

        /// <summary>
        /// Updates the background color of the main level image and the piece selection according to the current main style.
        /// </summary>
        private void UpdateBackgroundImage()
        {
            if (CurLevel.MainStyle == null)
                return;
            Color backColor = CurLevel.MainStyle?.GetColor(C.StyleColor.BACKGROUND) ?? C.NLColors[C.NLColor.BackDefault];

            picPieceList.ForEach(pic => pic.BackColor = backColor);
            curRenderer?.CreateBackgroundLayer();
        }

        /// <summary>
        /// Enables actionable commands for selected pieces and sets checkbox checks correctly.
        /// </summary>
        private void UpdateFlagsForPieceActions()
        {
            List<LevelPiece> selectionList = CurLevel.SelectionList();

            but_RotatePieces.Enabled = selectionList.Exists(p => p.MayRotate());
            but_FlipPieces.Enabled = selectionList.Exists(p => p.MayFlip());
            but_InvertPieces.Enabled = selectionList.Exists(p => p.MayInvert());

            but_MoveBack.Enabled = (selectionList.Count > 0);
            but_MoveFront.Enabled = (selectionList.Count > 0);
            but_MoveBackOne.Enabled = (selectionList.Count > 0);
            but_MoveFrontOne.Enabled = (selectionList.Count > 0);

            check_Pieces_NoOv.Enabled = selectionList.Exists(p => !(p is TerrainPiece) || !(p as TerrainPiece).IsSketch);
            // Set check-mark correctly, without firing the CheckedChanged event
            check_Pieces_NoOv.CheckedChanged -= check_Pieces_NoOv_CheckedChanged;
            check_Pieces_NoOv.Checked = selectionList.Exists(p => (p is GadgetPiece && (p as GadgetPiece).IsNoOverwrite)
                                                               || (p is TerrainPiece && (p as TerrainPiece).IsNoOverwrite));
            check_Pieces_NoOv.CheckedChanged += check_Pieces_NoOv_CheckedChanged;

            check_Pieces_Erase.Enabled = selectionList.Exists(p => (p is TerrainPiece tp) && (!tp.IsSketch));
            // Set check-mark correctly, without firing the CheckedChanged event
            check_Pieces_Erase.CheckedChanged -= check_Pieces_Erase_CheckedChanged;
            check_Pieces_Erase.Checked = selectionList.Exists(p => p is TerrainPiece && (p as TerrainPiece).IsErase);
            check_Pieces_Erase.CheckedChanged += check_Pieces_Erase_CheckedChanged;

            check_Pieces_OneWay.Enabled = selectionList.Exists(p => (p is TerrainPiece tp) && !tp.IsSteel && !tp.IsSketch);
            // Set check-mark correctly, without firing the CheckedChanged event
            check_Pieces_OneWay.CheckedChanged -= check_Pieces_OneWay_CheckedChanged;
            check_Pieces_OneWay.Checked = selectionList.Exists(p => p is TerrainPiece && (p as TerrainPiece).IsOneWay);
            check_Pieces_OneWay.CheckedChanged += check_Pieces_OneWay_CheckedChanged;

            check_Pieces_OnlyOnTerrain.Enabled = selectionList.Exists(p => p is GadgetPiece);
            // Set check-mark correctly, without firing the CheckedChanged event
            check_Pieces_OnlyOnTerrain.CheckedChanged -= check_Pieces_OnlyOnTerrain_CheckedChanged;
            check_Pieces_OnlyOnTerrain.Checked = selectionList.Exists(p => p is GadgetPiece && (p as GadgetPiece).IsOnlyOnTerrain);
            check_Pieces_OnlyOnTerrain.CheckedChanged += check_Pieces_OnlyOnTerrain_CheckedChanged;

            but_GroupSelection.Enabled = CurLevel.MayGroupSelection();
            but_UngroupSelection.Enabled = CurLevel.MayUngroupSelection();

            foreach (C.Skill skill in checkboxesSkillFlags.Keys)
            {
                checkboxesSkillFlags[skill].Enabled = selectionList.Exists(p => p.MayReceiveSkill(skill));

                // Set check-mark correctly, without firing the CheckedChanged event
                checkboxesSkillFlags[skill].CheckedChanged -= check_Piece_Skill_CheckedChanged;
                checkboxesSkillFlags[skill].Checked = selectionList.Exists(p => p is GadgetPiece && (p as GadgetPiece).SkillFlags.Contains(skill));
                checkboxesSkillFlags[skill].CheckedChanged += check_Piece_Skill_CheckedChanged;
            }

            if (selectionList.Count > 0)
            {
                int specWidth = selectionList[0].Width;
                int specHeight = selectionList[0].Height;
                bool mayResizeHoriz = selectionList.All(item => item.MayResizeHoriz() && item.Width == specWidth);
                bool mayResizeVert = selectionList.All(item => item.MayResizeVert() && item.Height == specHeight);

                lbl_Resize_Width.Visible = mayResizeHoriz;
                num_Resize_Width.Visible = mayResizeHoriz;
                if (mayResizeHoriz)
                {
                    num_Resize_Width.Maximum = CurLevel.Width + 320;
                    num_Resize_Width.Value = Math.Min(Math.Max(specWidth, num_Resize_Width.Minimum), num_Resize_Width.Maximum);
                }
                lbl_Resize_Height.Visible = mayResizeVert;
                num_Resize_Height.Visible = mayResizeVert;
                if (mayResizeVert)
                {
                    num_Resize_Height.Maximum = CurLevel.Height + 160;
                    num_Resize_Height.Value = Math.Min(Math.Max(specHeight, num_Resize_Height.Minimum), num_Resize_Height.Maximum);
                }
            }
            else
            {
                lbl_Resize_Width.Visible = false;
                num_Resize_Width.Visible = false;
                lbl_Resize_Height.Visible = false;
                num_Resize_Height.Visible = false;
            }

            if (selectionList.Count > 0 && selectionList.All(item => item.ObjType == C.OBJ.DECORATION))
            {
                var gadget = selectionList[0] as GadgetPiece;
                int dirIndex = gadget.DecorationAngle * 2 / 45;
                int speed = gadget.DecorationSpeed;

                lbl_Decoration_Direction.Visible = true;
                lbl_Decoration_Speed.Visible = true;
                cb_Decoration_Direction.Visible = true;
                num_Decoration_Speed.Visible = true;

                cb_Decoration_Direction.SelectedIndex = dirIndex;
                num_Decoration_Speed.Value = speed;
            }
            else
            {
                lbl_Decoration_Direction.Visible = false;
                lbl_Decoration_Speed.Visible = false;
                cb_Decoration_Direction.Visible = false;
                num_Decoration_Speed.Visible = false;
            }

            if (selectionList.Count == 1 && selectionList[0] is GadgetPiece)
            {
                GadgetPiece gadget = (GadgetPiece)selectionList[0];
                if (gadget.ObjType == C.OBJ.PICKUP)
                {
                    lbl_PickupSkillCount.Visible = true;
                    num_PickupSkillCount.Value = Math.Min(Math.Max(gadget.Val_L, num_PickupSkillCount.Minimum), num_PickupSkillCount.Maximum);
                    num_PickupSkillCount.Visible = true;
                }
                else
                {
                    lbl_PickupSkillCount.Visible = false;
                    num_PickupSkillCount.Visible = false;
                }

                if (new[] { C.OBJ.RADIATION, C.OBJ.SLOWFREEZE }.Contains(gadget.ObjType))
                {
                    lbl_SR_Countdown.Visible = true;
                    num_SR_Countdown.Value = Math.Min(Math.Max(gadget.CountdownLength, num_SR_Countdown.Minimum), num_SR_Countdown.Maximum);
                    num_SR_Countdown.Visible = true;
                }
                else
                {
                    lbl_SR_Countdown.Visible = false;
                    num_SR_Countdown.Visible = false;
                }

                if (new[] { C.OBJ.EXIT, C.OBJ.EXIT_LOCKED, C.OBJ.HATCH }.Contains(gadget.ObjType))
                {
                    lbl_LemmingLimit.Visible = true;
                    num_LemmingLimit.Value = Math.Min(Math.Max(gadget.LemmingCap, 0), 999);
                    num_LemmingLimit.Visible = true;
                }
                else
                {
                    lbl_LemmingLimit.Visible = false;
                    num_LemmingLimit.Visible = false;
                }
            }
            else
            {
                lbl_PickupSkillCount.Visible = false;
                num_PickupSkillCount.Visible = false;
                lbl_LemmingLimit.Visible = false;
                num_LemmingLimit.Visible = false;
                lbl_SR_Countdown.Visible = false;
                num_SR_Countdown.Visible = false;
            }

            if (selectionList.Count == 2
                && selectionList.Exists(item => item.ObjType == C.OBJ.TELEPORTER)
                && selectionList.Exists(item => item.ObjType == C.OBJ.RECEIVER))
            {
                GadgetPiece MyTeleporter = (GadgetPiece)selectionList.Find(item => item.ObjType == C.OBJ.TELEPORTER);
                GadgetPiece MyReceiver = (GadgetPiece)selectionList.Find(item => item.ObjType == C.OBJ.RECEIVER);

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
            if (selectionList.Exists(p => p.ObjType == C.OBJ.COLLECTIBLE))
            {
                check_Lvl_Invincibility.Visible = true;
            }
            else
            {
                check_Lvl_Invincibility.Visible = false;
            }
        }


        /// <summary>
        /// Repositions the controls after resizing the main form.
        /// </summary>
        private void MoveControlsOnFormResize()
        {
            scrollPicLevelHoriz.Top = this.Height - 182;
            scrollPicLevelVert.Left = this.Width - 29;

            RepositionPicLevel();

            foreach (TabControl tabControl in this.Controls.OfType<TabControl>())
            {
                tabControl.Height = this.Height - 178;
            }

            combo_PieceStyle.Top = this.Height - 149;
            but_PieceTerr.Top = this.Height - 149;
            but_PieceObj.Top = this.Height - 149;
            but_PieceBackground.Top = this.Height - 149;
            but_ClearBackground.Top = this.Height - 149;
            but_PieceSketches.Top = this.Height - 149;

            but_PieceLeft.Top = this.Height - 122;
            but_PieceRight.Top = this.Height - 122;
            but_PieceRight.Left = this.Width - 44;

            bool updateImages = MovePicPiecesOnResize();
            if (updateImages)
            {
                UpdateBackgroundImage();
                LoadPiecesIntoPictureBox();
            }
        }

        /// <summary>
        /// Positions pic_Level at the correct place and resizes it accordingly.
        /// </summary>
        private void RepositionPicLevel()
        {
            pic_Level.Left = 264;

            Size newPicLevelSize = new Size(this.Width - 276, this.Height - 178);

            // Check for scroll bars. This method resizes pic_Level accordingly (if necessary).
            newPicLevelSize = CheckEnableLevelScrollbars(newPicLevelSize);

            pic_Level.Size = newPicLevelSize;
            curRenderer.EnsureScreenPosInLevel();
        }

        /// <summary>
        /// Checks whether the level fits into the picLevel and enables scrollbars if necessary.
        /// <para> Warning: Always call RepositionPicLevel() instead of this method! </para>
        /// </summary>
        private Size CheckEnableLevelScrollbars(Size newPicBoxSize)
        {
            Rectangle displayedLevelRect = curRenderer.GetLevelBmpRect(newPicBoxSize);
            bool displayScrollHoriz = false;
            bool displayScrollVert = false;

            displayScrollHoriz = (displayedLevelRect.Width + 1 < CurLevel.Width);
            displayScrollVert = (displayedLevelRect.Height + 1 < CurLevel.Height);

            if (displayScrollHoriz)
                newPicBoxSize.Height -= 16;
            if (displayScrollVert)
                newPicBoxSize.Width -= 16;

            // Check whether shrinking the level size made other scrollbar necessary, too
            if (displayScrollHoriz ^ displayScrollVert)
            {
                displayedLevelRect = curRenderer.GetLevelBmpRect(newPicBoxSize);
                if (!displayScrollHoriz && displayedLevelRect.Width + 1 < CurLevel.Width)
                {
                    displayScrollHoriz = true;
                    newPicBoxSize.Height -= 16;
                }
                if (!displayScrollVert && displayedLevelRect.Height + 1 < CurLevel.Height)
                {
                    displayScrollVert = true;
                    newPicBoxSize.Width -= 16;
                }
            }

            // Update displayed level area
            displayedLevelRect = curRenderer.GetLevelBmpRect(newPicBoxSize);

            // Set scrollPicLevelHoriz
            if (displayScrollHoriz)
            {
                int maxValue = CurLevel.Width + (Renderer.AllowedGrayBorder + 20) - displayedLevelRect.Width + 1;
                scrollPicLevelHoriz.Minimum = -Renderer.AllowedGrayBorder;
                scrollPicLevelHoriz.Maximum = maxValue;
                scrollPicLevelHoriz.SmallChange = 8;
                scrollPicLevelHoriz.LargeChange = 16;
                scrollPicLevelHoriz.Value = Math.Max(Math.Min(displayedLevelRect.Left, maxValue - 1), -Renderer.AllowedGrayBorder);
            }
            scrollPicLevelHoriz.Enabled = displayScrollHoriz;
            scrollPicLevelHoriz.Visible = displayScrollHoriz;


            // Set scrollPicLevelVert
            if (displayScrollVert)
            {
                int maxValue = CurLevel.Height + (Renderer.AllowedGrayBorder + 10) - displayedLevelRect.Height + 1;
                scrollPicLevelVert.Minimum = -Renderer.AllowedGrayBorder;
                scrollPicLevelVert.Maximum = maxValue;
                scrollPicLevelVert.SmallChange = 4;
                scrollPicLevelVert.LargeChange = 8;
                scrollPicLevelVert.Value = Math.Max(Math.Min(displayedLevelRect.Top, maxValue - 1), -Renderer.AllowedGrayBorder);
            }
            scrollPicLevelVert.Enabled = displayScrollVert;
            scrollPicLevelVert.Visible = displayScrollVert;

            // finally resize scrollbars correctly
            if (scrollPicLevelHoriz.Enabled)
                scrollPicLevelVert.Height = newPicBoxSize.Height + 2;
            else 
                scrollPicLevelVert.Height = newPicBoxSize.Height - 2;

            if (scrollPicLevelVert.Enabled)
                scrollPicLevelHoriz.Width = newPicBoxSize.Width - 25;
            else
                scrollPicLevelHoriz.Width = newPicBoxSize.Width - 4;
                scrollPicLevelHoriz.Left = 264;

            scrollPicLevelVert.Left = scrollPicLevelVert.Parent.ClientRectangle.Width - scrollPicLevelVert.Width;

            return newPicBoxSize;
        }

        /// <summary>
        /// Sets the scrollbar values for the editor screen position correctly.
        /// </summary>
        private void UpdateScrollBarValues()
        {
            if (scrollPicLevelHoriz.Enabled)
            {
                scrollPicLevelHoriz.Value = Math.Max(Math.Min(curRenderer.ScreenPosX, scrollPicLevelHoriz.Maximum - 1), scrollPicLevelHoriz.Minimum);
            }
            if (scrollPicLevelVert.Enabled)
            {
                scrollPicLevelVert.Value = Math.Max(Math.Min(curRenderer.ScreenPosY, scrollPicLevelVert.Maximum - 1), scrollPicLevelVert.Minimum);
            }
        }


        /// <summary>
        /// Moves the picture boxes to select new pieces to the correct position.
        /// </summary>
        private bool MovePicPiecesOnResize()
        {
            picPieceList.ForEach(pic => pic.Top = this.Height - 122);

            int numPicPieces = (this.Width - 170) / 90 + 1;

            while (picPieceList.Count > numPicPieces)
            {
                PictureBox oldPicPieces = picPieceList[picPieceList.Count - 1];
                picPieceList.Remove(oldPicPieces);
                this.Controls.Remove(oldPicPieces);
                oldPicPieces.Dispose();
            }

            bool needUpdatePicPieceImages = (picPieceList.Count < numPicPieces);
            while (picPieceList.Count < numPicPieces)
            {
                picPieceList.Add(CreatePicPiece());
            }

            for (int picPieceIndex = 0; picPieceIndex < numPicPieces; picPieceIndex++)
            {
                picPieceList[picPieceIndex].Left = 36 + picPieceIndex * (this.Width - 170) / (numPicPieces - 1);
            }

            return needUpdatePicPieceImages;
        }

        /// <summary>
        /// Creates a new picture box for selecting new pieces.
        /// </summary>
        private PictureBox CreatePicPiece()
        {
            PictureBox picPiece = new PictureBox();
            picPiece.Size = C.PicPieceSize;
            picPiece.Top = this.Height - 122;
            picPiece.BorderStyle = BorderStyle.Fixed3D;
            picPiece.SizeMode = PictureBoxSizeMode.CenterImage;

            picPiece.Click += new EventHandler(picPieces_Click);
            picPiece.MouseDown += new MouseEventHandler(picPieces_MouseDown);
            picPiece.MouseUp += new MouseEventHandler(pic_Level_MouseUp);

            this.Controls.Add(picPiece);

            return picPiece;
        }

        /// <summary>
        /// Updates the dragNewPiecePicBox.
        /// </summary>
        private void UpdateNewPiecePicBox()
        {
            Point mousePos = PointToClient(MousePosition);
            Point mousePosPicLevel = pic_Level.PointToClient(MousePosition);

            if (curRenderer.MouseDragAction != C.DragActions.DragNewPiece
                || MouseButtons != MouseButtons.Left)
            {
                // Stop timer and make PicBox invisible
                dragNewPieceTimer.Enabled = false;
                pic_DragNewPiece.Visible = false;
                if (curRenderer.MouseDragAction == C.DragActions.DragNewPiece)
                {
                    curRenderer.DeleteDraggingVars();
                }
            }
            else if (curRenderer.IsPointInLevelArea(mousePosPicLevel))
            {
                // Display the piece via the renderer in the level
                pic_DragNewPiece.Visible = false;

                curRenderer.MouseCurPos = mousePosPicLevel;
                pic_Level.Image = curRenderer.CombineLayers(dragNewPieceKey);
            }
            else
            {
                // Display the piece via the picture box.
                if (!pic_DragNewPiece.Visible)
                {
                    dragNewPieceTimer.Interval = 50;
                    pic_DragNewPiece.BringToFront();
                    pic_DragNewPiece.Visible = true;
                    pic_Level.Image = curRenderer.CombineLayers();
                }
                // Reposition the PicBox
                int newPosX = mousePos.X - pic_DragNewPiece.Width / 2;
                int newPosY = mousePos.Y - pic_DragNewPiece.Height / 2;
                pic_DragNewPiece.Location = new Point(newPosX, newPosY);
            }
        }
    }
}
