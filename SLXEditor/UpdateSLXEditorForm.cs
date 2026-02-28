using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using static SLXEditor.Settings;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.StartPanel;

namespace SLXEditor
{
    /// <summary>
    /// Main Form: This part defines the methods updating the form members
    /// </summary>
    partial class SLXEditForm
    {
        public EditorMode previousEditorMode;
        private int pieceBrowserTop = 26;

        /// <summary>
        /// Initializes the intervals for all repeat buttons.
        /// </summary>
        private void SetRepeatButtonIntervals()
        {
            btnRotate.SetInterval(1000);
            btnInvert.SetInterval(1000);
            btnFlip.SetInterval(1000);
            btnDrawSooner.SetInterval(150);
            btnDrawLater.SetInterval(150);
            btnPieceLeft.SetInterval(100, MouseButtons.Left);
            btnPieceLeft.SetInterval(30, MouseButtons.Right);
            btnPieceRight.SetInterval(100, MouseButtons.Left);
            btnPieceRight.SetInterval(30, MouseButtons.Right);
        }

        /// <summary>
        /// Displays the correct controls for Release Rate and Spawn Interval
        /// </summary>
        public void UpdateRRSIControls()
        {
            checkLockRRSI.Text = curSettings.UseSpawnInterval ? "Lock Spawn Interval" : "Lock Release Rate";
            numRR.Visible = !curSettings.UseSpawnInterval;
            numSI.Visible = curSettings.UseSpawnInterval;
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
                case C.SelectPieceType.Terrain:
                    pieceKeys = pieceCurStyle?.TerrainKeys;
                    break;
                case C.SelectPieceType.Steel:
                    pieceKeys = pieceCurStyle?.SteelKeys;
                    break;
                case C.SelectPieceType.Objects:
                    pieceKeys = pieceCurStyle?.ObjectKeys;
                    break;
                case C.SelectPieceType.Backgrounds:
                    pieceKeys = pieceCurStyle?.BackgroundKeys;
                    break;
                case C.SelectPieceType.Rulers:
                    pieceKeys = new List<string>(ImageLibrary.RulerKeys);
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
                    btnPieceRight.StopRepeatAction();
                    btnPieceLeft.StopRepeatAction();
                }

                if (ImageLibrary.GetDeprecated(pieceKey) && !DisplaySettings.IsDisplayed(C.DisplayType.Deprecated))
                {
                    skipDeprecatedOffset++;
                    i--;
                    continue;
                }

                int frameIndex = (ImageLibrary.GetObjType(pieceKey).In(C.OBJ.PICKUP, C.OBJ.EXIT_LOCKED, C.OBJ.BUTTON, C.OBJ.COLLECTIBLE, C.OBJ.TRAPONCE)) ? 1 : 0;
                Bitmap pieceImage;

                bool preferObjectName = curSettings.PreferObjectName;

                if (curSettings.CurrentPieceBrowserMode == PieceBrowserMode.ShowDescriptions)
                {
                    if (preferObjectName)
                        pieceImage = ImageLibrary.GetImageWithName(pieceKey, frameIndex);
                    else
                        pieceImage = ImageLibrary.GetImageWithDescription(pieceKey, frameIndex);
                }
                else if (curSettings.CurrentPieceBrowserMode == PieceBrowserMode.ShowData)
                {
                    if (preferObjectName)
                        pieceImage = ImageLibrary.GetImageWithNameAndData(pieceKey, frameIndex);
                    else
                        pieceImage = ImageLibrary.GetImageWithDescriptionAndData(pieceKey, frameIndex);
                }
                else
                    pieceImage = ImageLibrary.GetImage(pieceKey, RotateFlipType.RotateNoneFlipNone, frameIndex);

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
        private void SetToolTipsForPicPiece(PictureBox picPiece, string pieceKey)
        {
            string toolTipText = "unknown";
            C.OBJ pieceObjType = (pieceKey == null) ? C.OBJ.NULL : ImageLibrary.GetObjType(pieceKey);
            if (C.ObjectDescriptions.ContainsKey(pieceObjType))
            {
                toolTipText = C.ObjectDescriptions[pieceObjType];
            }
            if (pieceKey != null && ImageLibrary.GetWidth(pieceKey) > 0 && ImageLibrary.GetHeight(pieceKey) > 0)
            {
                toolTipText += $" ({ImageLibrary.GetWidth(pieceKey)} x {ImageLibrary.GetHeight(pieceKey)}";

                if (ImageLibrary.GetResizeMode(pieceKey) != C.Resize.None)
                {
                    toolTipText += $", Resize: {ImageLibrary.GetResizeMode(pieceKey)}";
                }

                if (ImageLibrary.IsNineSliced(pieceKey))
                {
                    toolTipText += ", 9S";
                }

                toolTipText += ")";
            }

            toolTipPieces.SetToolTip(picPiece, toolTipText);
        }

        /// <summary>
        /// Updates the background color of the main level image and the piece selection according to the current main style.
        /// </summary>
        private void UpdateBackgroundImage()
        {
            if (CurLevel.ThemeStyle == null)
                return;
            Color backColor = CurLevel.ThemeStyle?.GetColor(C.StyleColor.BACKGROUND) ?? C.SLXColors[C.SLXColor.BackDefault];

            picPieceList.ForEach(pic => pic.BackColor = backColor);
            curRenderer?.CreateBackgroundLayer();
        }

        /// <summary>
        /// Enables actionable commands for selected pieces and sets checkbox checks correctly.
        /// </summary>
        private void UpdateFlagsForPieceActions()
        {
            List<LevelPiece> selectionList = CurLevel.SelectionList();

            btnRotate.Enabled = selectionList.Exists(p => p.MayRotate());
            btnFlip.Enabled = selectionList.Exists(p => p.MayFlip());
            btnInvert.Enabled = selectionList.Exists(p => p.MayInvert());

            btnDrawFirst.Enabled = (selectionList.Count > 0);
            btnDrawLast.Enabled = (selectionList.Count > 0);
            btnDrawSooner.Enabled = (selectionList.Count > 0);
            btnDrawLater.Enabled = (selectionList.Count > 0);

            checkNoOverwrite.Enabled = selectionList.Exists(p => p.ObjType != C.OBJ.RULER);
            // Set check-mark correctly, without firing the CheckedChanged event
            checkNoOverwrite.CheckedChanged -= check_Pieces_NoOv_CheckedChanged;
            checkNoOverwrite.Checked = selectionList.Exists(p => (p is GadgetPiece && (p as GadgetPiece).IsNoOverwrite && (p.ObjType != C.OBJ.RULER))
                                                               || (p is TerrainPiece && (p as TerrainPiece).IsNoOverwrite));
            checkNoOverwrite.CheckedChanged += check_Pieces_NoOv_CheckedChanged;

            checkErase.Enabled = selectionList.Exists(p => p is TerrainPiece tp);
            // Set check-mark correctly, without firing the CheckedChanged event
            checkErase.CheckedChanged -= check_Pieces_Erase_CheckedChanged;
            checkErase.Checked = selectionList.Exists(p => p is TerrainPiece && (p as TerrainPiece).IsErase);
            checkErase.CheckedChanged += check_Pieces_Erase_CheckedChanged;

            checkAllowOneWay.Enabled = selectionList.Exists(p => (p is TerrainPiece tp) && !tp.IsSteel);
            // Set check-mark correctly, without firing the CheckedChanged event
            checkAllowOneWay.CheckedChanged -= check_Pieces_OneWay_CheckedChanged;
            checkAllowOneWay.Checked = selectionList.Exists(p => p is TerrainPiece && (p as TerrainPiece).IsOneWay);
            checkAllowOneWay.CheckedChanged += check_Pieces_OneWay_CheckedChanged;

            checkOnlyOnTerrain.Enabled = selectionList.Exists(p => p is GadgetPiece && p.ObjType != C.OBJ.RULER);
            // Set check-mark correctly, without firing the CheckedChanged event
            checkOnlyOnTerrain.CheckedChanged -= check_Pieces_OnlyOnTerrain_CheckedChanged;
            checkOnlyOnTerrain.Checked = selectionList.Exists(p => p is GadgetPiece && (p as GadgetPiece).IsOnlyOnTerrain);
            checkOnlyOnTerrain.CheckedChanged += check_Pieces_OnlyOnTerrain_CheckedChanged;

            btnGroupSelection.Enabled = CurLevel.MayGroupSelection();
            btnUngroupSelection.Enabled = CurLevel.MayUngroupSelection();

            foreach (C.Skill skill in checkboxesSkillFlags.Keys)
            {
                checkboxesSkillFlags[skill].Enabled = selectionList.Exists(p => p.MayReceiveSkill(skill));

                // Set check-mark correctly, without firing the CheckedChanged event
                checkboxesSkillFlags[skill].CheckedChanged -= checkSkill_CheckedChanged;
                checkboxesSkillFlags[skill].Checked = selectionList.Exists(p => p is GadgetPiece && (p as GadgetPiece).SkillFlags.Contains(skill));
                checkboxesSkillFlags[skill].CheckedChanged += checkSkill_CheckedChanged;
            }

            if (selectionList.Count > 0)
            {
                int specWidth = selectionList[0].Width;
                int specHeight = selectionList[0].Height;
                bool mayResizeHoriz = selectionList.All(item => item.MayResizeHoriz() && item.Width == specWidth);
                bool mayResizeVert = selectionList.All(item => item.MayResizeVert() && item.Height == specHeight);

                lblResizeWidth.Visible = mayResizeHoriz;
                numResizeWidth.Visible = mayResizeHoriz;
                if (mayResizeHoriz)
                {
                    numResizeWidth.Maximum = CurLevel.Width + 320;
                    numResizeWidth.Value = Math.Min(Math.Max(specWidth, numResizeWidth.Minimum), numResizeWidth.Maximum);
                }
                lblResizeHeight.Visible = mayResizeVert;
                numResizeHeight.Visible = mayResizeVert;
                if (mayResizeVert)
                {
                    numResizeHeight.Maximum = CurLevel.Height + 160;
                    numResizeHeight.Value = Math.Min(Math.Max(specHeight, numResizeHeight.Minimum), numResizeHeight.Maximum);
                }
            }
            else
            {
                lblResizeWidth.Visible = false;
                numResizeWidth.Visible = false;
                lblResizeHeight.Visible = false;
                numResizeHeight.Visible = false;
            }

            if (selectionList.Count > 0 && selectionList.All(item => item.ObjType == C.OBJ.DECORATION))
            {
                var gadget = selectionList[0] as GadgetPiece;
                int dirIndex = gadget.DecorationAngle * 2 / 45;
                int speed = gadget.DecorationSpeed;

                lblDecorationDirection.Visible = true;
                lblDecorationSpeed.Visible = true;
                comboDecorationDirection.Visible = true;
                numDecorationSpeed.Visible = true;

                comboDecorationDirection.SelectedIndex = dirIndex;
                numDecorationSpeed.Value = speed;
            }
            else
            {
                lblDecorationDirection.Visible = false;
                lblDecorationSpeed.Visible = false;
                comboDecorationDirection.Visible = false;
                numDecorationSpeed.Visible = false;
            }

            if (selectionList.Count == 1 && selectionList[0] is GadgetPiece)
            {
                GadgetPiece gadget = (GadgetPiece)selectionList[0];
                if (gadget.ObjType == C.OBJ.PICKUP)
                {
                    lblPickupSkillCount.Visible = true;
                    numPickupSkillCount.Value = Math.Min(Math.Max(gadget.Val_L, numPickupSkillCount.Minimum), numPickupSkillCount.Maximum);
                    numPickupSkillCount.Visible = true;
                }
                else
                {
                    lblPickupSkillCount.Visible = false;
                    numPickupSkillCount.Visible = false;
                }

                if (new[] { C.OBJ.RADIATION, C.OBJ.SLOWFREEZE }.Contains(gadget.ObjType))
                {
                    lblCountdown.Visible = true;
                    numCountdown.Value = Math.Min(Math.Max(gadget.CountdownLength, numCountdown.Minimum), numCountdown.Maximum);
                    numCountdown.Visible = true;
                }
                else
                {
                    lblCountdown.Visible = false;
                    numCountdown.Visible = false;
                }

                if (new[] { C.OBJ.EXIT, C.OBJ.EXIT_LOCKED, C.OBJ.HATCH }.Contains(gadget.ObjType))
                {
                    lblLemmingLimit.Visible = true;
                    numLemmingLimit.Value = Math.Min(Math.Max(gadget.LemmingCap, 0), 999);
                    numLemmingLimit.Visible = true;
                }
                else
                {
                    lblLemmingLimit.Visible = false;
                    numLemmingLimit.Visible = false;
                }
            }
            else
            {
                lblPickupSkillCount.Visible = false;
                numPickupSkillCount.Visible = false;
                lblLemmingLimit.Visible = false;
                numLemmingLimit.Visible = false;
                lblCountdown.Visible = false;
                numCountdown.Visible = false;
            }

            if (selectionList.Count == 2 &&
                   (
                       (selectionList.Exists(item => item.ObjType == C.OBJ.TELEPORTER) && selectionList.Exists(item => item.ObjType == C.OBJ.RECEIVER)) ||
                       (selectionList.Count(item => item.ObjType == C.OBJ.PORTAL) == 2)
                    )
               )
            {
                GadgetPiece MyTeleporter = (GadgetPiece)selectionList.Find(item => item.ObjType == C.OBJ.TELEPORTER);
                GadgetPiece MyReceiver = (GadgetPiece)selectionList.Find(item => item.ObjType == C.OBJ.RECEIVER);

                if (MyTeleporter == null || MyReceiver == null)
                {
                    MyTeleporter = (GadgetPiece)selectionList.Find(item => item.ObjType == C.OBJ.PORTAL);
                    MyReceiver = (GadgetPiece)selectionList.Find(item => (item != MyTeleporter) && (item.ObjType == C.OBJ.PORTAL));
                }

                if (MyTeleporter.Val_L > 0 && MyTeleporter.Val_L == MyReceiver.Val_L)
                {
                    btnPairTeleporter.Text = "Already Paired";
                    btnPairTeleporter.Enabled = false;
                }
                else
                {
                    btnPairTeleporter.Text = "Pair Teleporters";
                    btnPairTeleporter.Enabled = true;
                }
                btnPairTeleporter.Visible = true;
            }
            else
            {
                btnPairTeleporter.Visible = false;
            }
            if (selectionList.Exists(p => p.ObjType == C.OBJ.COLLECTIBLE))
            {
                checkInvincibility.Visible = true;
            }
            else
            {
                checkInvincibility.Visible = false;
            }
        }

        /// <summary>
        /// Sets non-updating controls according to Lemmix version/user preferences
        /// </summary>
        public void UpdateLemmixVersionFeatures()
        {
            if (CurLevel != null)
                CurLevel.Format = isNeoLemmixOnly ? "NeoLemmix" : "SuperLemmix";

            lblStoner.Enabled = isNeoLemmixOnly;
            lblStoner.Visible = isNeoLemmixOnly;
            lblFreezer.Enabled = !isNeoLemmixOnly;
            lblFreezer.Visible = !isNeoLemmixOnly;

            numStoner.Visible = isNeoLemmixOnly;
            numStoner.Enabled = isNeoLemmixOnly;
            numFreezer.Enabled = !isNeoLemmixOnly;
            numFreezer.Visible = !isNeoLemmixOnly;

            checkStoner.Visible = isNeoLemmixOnly;
            checkFreezer.Visible = !isNeoLemmixOnly;

            lblBallooner.Enabled = !isNeoLemmixOnly;
            lblTimebomber.Enabled = !isNeoLemmixOnly;
            lblLadderer.Enabled = !isNeoLemmixOnly;
            lblSpearer.Enabled = !isNeoLemmixOnly;
            lblGrenader.Enabled = !isNeoLemmixOnly;

            numBallooner.Enabled = !isNeoLemmixOnly;
            numTimebomber.Enabled = !isNeoLemmixOnly;
            numLadderer.Enabled = !isNeoLemmixOnly;
            numSpearer.Enabled = !isNeoLemmixOnly;
            numGrenader.Enabled = !isNeoLemmixOnly;

            checkSuperlemming.Enabled = !isNeoLemmixOnly;
            radAlwaysSteel.Enabled = !isNeoLemmixOnly;
            radOnlyWhenVisible.Enabled = !isNeoLemmixOnly;
        }

        /// <summary>
        /// Repositions the controls after resizing the main form.
        /// </summary>
        public void MoveControlsOnFormResize()
        {
            scrollPicLevelHoriz.Top = this.Height - 178;
            scrollPicLevelVert.Left = this.Width - 30;

            RepositionPicLevel();

            foreach (TabControl tabControl in this.Controls.OfType<TabControl>())
            {
                tabControl.Height = this.Height - 178;
            }

            bool pieceBrowserIsWindowed = pieceBrowserWindow != null;
            int width = pieceBrowserWindow != null ? pieceBrowserWindow.Width : this.Width;

            RepositionPieceBrowser(pieceBrowserIsWindowed, width);
            RepositionPicPieces(pieceBrowserIsWindowed, width);
        }

        /// <summary>
        /// Adds and repositions Piece Browser images based on form width
        /// </summary>
        public void RepositionPicPieces(bool windowed, int width)
        {
            bool updateImages = MovePicPiecesOnResize(windowed, width);
            if (updateImages)
            {
                UpdateBackgroundImage();
                LoadPiecesIntoPictureBox();
            }
        }

        /// <summary>
        /// Positions panelPieceBrowser at the correct place on the main form
        /// </summary>
        public void RepositionPieceBrowser(bool isWindowed, int windowWidth = 0)
        {
            int posLeft = 0;
            int posTop = 0;
            int width = 0;
            int height = 148;
            int rightButtonOffset = 0;
            
            if (isWindowed)
            {
                width = windowWidth;
                rightButtonOffset = 50;
            }
            else
            {
                posLeft = tabProperties.Left - 6;
                posTop = this.Height - height;
                width = this.Width - 12;
                rightButtonOffset = 36;
            }

            panelPieceBrowser.Left = posLeft;
            panelPieceBrowser.Top = posTop;
            panelPieceBrowser.Width = width;
            panelPieceBrowser.Height = height;

            bool showRandom = curSettings.ShowRandomButton;
            btnStyleRandom.Top = 0;
            btnStyleRandom.Left = 5;
            btnStyleRandom.Visible = showRandom ? true : false;
            comboPieceStyle.Top = 0;
            comboPieceStyle.Left = showRandom ? btnStyleRandom.Right + 5 : 5;
            comboPieceStyle.Width = showRandom ? 200 : 265;

            btnTerrain.Top = 0;
            btnSteel.Top = 0;
            btnObjects.Top = 0;
            btnRulers.Top = 0;
            btnBackgrounds.Top = 0;

            btnPieceLeft.Top = pieceBrowserTop;
            btnPieceRight.Top = pieceBrowserTop;
            btnPieceRight.Left = panelPieceBrowser.Width - rightButtonOffset;

            btnSearchPieces.Top = 0;
            btnSearchPieces.Left = btnPieceRight.Right - 4 - btnSearchPieces.Width;
            btnClearBackground.Top = 0;
            btnClearBackground.Left = btnSearchPieces.Left - 6 - btnClearBackground.Width;
        }

        private void UpdateCropButtons()
        {
            bool cropActive = curRenderer.CropTool.Active;

            btnApplyCrop.Visible = cropActive;
            btnCancelCrop.Visible = cropActive;
            btnCropLevel.Enabled = !cropActive;
            btnCropLevel.Width = cropActive ? btnApplyCrop.Width : btnCancelCrop.Right - btnCropLevel.Left;
        }

        /// <summary>
        /// Positions pic_Level at the correct place and resizes it accordingly.
        /// </summary>
        private void RepositionPicLevel()
        {
            if (!repositionAfterZooming)
                return;
            
            picLevel.Left = 264;

            Size newPicLevelSize = new Size(this.Width - 276, this.Height - 178);

            // Check for scroll bars. This method resizes pic_Level accordingly (if necessary).
            newPicLevelSize = CheckEnableLevelScrollbars(newPicLevelSize);

            picLevel.Size = newPicLevelSize;
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
                int maxValue = CurLevel.Width + (Renderer.AllowedGrayBorder + 18) - displayedLevelRect.Width + 1;
                scrollPicLevelHoriz.Minimum = -Renderer.AllowedGrayBorder;
                scrollPicLevelHoriz.Maximum = maxValue;
                scrollPicLevelHoriz.SmallChange = 8;
                scrollPicLevelHoriz.LargeChange = 16;
                scrollPicLevelHoriz.Value = Math.Max(Math.Min(displayedLevelRect.Left, maxValue - 1), -Renderer.AllowedGrayBorder);
            }
            scrollPicLevelHoriz.Enabled = displayScrollHoriz;
            scrollPicLevelHoriz.Visible = displayScrollHoriz;
            curRenderer.ScrollHorizActive = displayScrollHoriz;

            // Set scrollPicLevelVert
            if (displayScrollVert)
            {
                int maxValue = CurLevel.Height + (Renderer.AllowedGrayBorder + 8) - displayedLevelRect.Height + 1;
                scrollPicLevelVert.Minimum = -Renderer.AllowedGrayBorder;
                scrollPicLevelVert.Maximum = maxValue;
                scrollPicLevelVert.SmallChange = 4;
                scrollPicLevelVert.LargeChange = 8;
                scrollPicLevelVert.Value = Math.Max(Math.Min(displayedLevelRect.Top, maxValue - 1), -Renderer.AllowedGrayBorder);
            }
            scrollPicLevelVert.Enabled = displayScrollVert;
            scrollPicLevelVert.Visible = displayScrollVert;
            curRenderer.ScrollVertActive = displayScrollVert;

            // Finally resize scrollbars correctly
            if (scrollPicLevelHoriz.Enabled)
                scrollPicLevelVert.Height = newPicBoxSize.Height + 2;
            else 
                scrollPicLevelVert.Height = newPicBoxSize.Height - 2;

            if (scrollPicLevelVert.Enabled)
                scrollPicLevelHoriz.Width = newPicBoxSize.Width - 8;
            else
                scrollPicLevelHoriz.Width = newPicBoxSize.Width - 4;
                
            scrollPicLevelHoriz.Left = 268;
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
        private bool MovePicPiecesOnResize(bool windowed, int width)
        {
            int posTop = pieceBrowserTop;
            if (windowed) posTop = 26; // For Piece Browser window
            picPieceList.ForEach(pic => pic.Top = posTop);

            int numPicPieces = (width - 170) / 90 + 1;

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
                picPieceList[picPieceIndex].Left = 36 + picPieceIndex * (width - 170) / (numPicPieces - 1);
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
            picPiece.Top = pieceBrowserTop;
            picPiece.BorderStyle = BorderStyle.Fixed3D;
            picPiece.SizeMode = PictureBoxSizeMode.CenterImage;

            picPiece.Click += new EventHandler(picPieces_Click);
            picPiece.MouseDown += new MouseEventHandler(picPieces_MouseDown);
            picPiece.MouseUp += new MouseEventHandler(pic_Level_MouseUp);

            panelPieceBrowser.Controls.Add(picPiece);

            return picPiece;
        }

        /// <summary>
        /// Updates the dragNewPiecePicBox.
        /// </summary>
        private void UpdateNewPiecePicBox()
        {
            Point mousePos = PointToClient(MousePosition);
            Point mousePosPicLevel = picLevel.PointToClient(MousePosition);

            if (curRenderer.MouseDragAction != C.DragActions.DragNewPiece
                || MouseButtons != MouseButtons.Left)
            {
                // Stop timer and make PicBox invisible
                dragNewPieceTimer.Enabled = false;
                picDragNewPiece.Visible = false;
                if (curRenderer.MouseDragAction == C.DragActions.DragNewPiece)
                {
                    curRenderer.DeleteDraggingVars();
                }
            }
            else if (curRenderer.IsPointInLevelArea(mousePosPicLevel))
            {
                // Display the piece via the renderer in the level
                picDragNewPiece.Visible = false;

                curRenderer.MouseCurPos = mousePosPicLevel;
                picLevel.Image = curRenderer.CombineLayers(dragNewPieceKey);
            }
            else
            {
                // Display the piece via the picture box.
                if (!picDragNewPiece.Visible)
                {
                    dragNewPieceTimer.Interval = 50;
                    picDragNewPiece.BringToFront();
                    picDragNewPiece.Visible = true;
                    picLevel.Image = curRenderer.CombineLayers();
                }
                // Reposition the PicBox
                int newPosX = mousePos.X - picDragNewPiece.Width / 2;
                int newPosY = mousePos.Y - picDragNewPiece.Height / 2;
                picDragNewPiece.Location = new Point(newPosX, newPosY);
            }
        }
    }
}
