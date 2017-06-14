using System;
using System.Collections.Generic;
using System.Linq;
using System.Drawing;
using System.Windows.Forms;

namespace NLEditor
{
    /// <summary>
    /// Main Form: This part defines the methods updating the form members
    /// </summary>
    partial class NLEditForm
    {
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
            var pieceKeys = pieceDoDisplayObject ? pieceCurStyle.ObjectKeys : pieceCurStyle.TerrainKeys;
            if (pieceKeys == null || pieceKeys.Count == 0)
            {
                ClearPiecesPictureBox();
                return;
            }

            // load correct pictures
            for (int i = 0; i < picPieceList.Count; i++)
            {
                string pieceKey = pieceKeys[(pieceStartIndex + i) % pieceKeys.Count];
                int frameIndex = (ImageLibrary.GetObjType(pieceKey).In(C.OBJ.PICKUP, C.OBJ.EXIT_LOCKED, C.OBJ.BUTTON, C.OBJ.TRAPONCE)) ? 1 : 0;
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

            toolTipPieces.SetToolTip(picPiece, toolTipText);
        }

        /// <summary>
        /// Updates the background color of the main level image and the piece selection according to the current main style.
        /// </summary>
        private void UpdateBackgroundImage()
        {
            if (CurLevel.MainStyle == null) return;
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

            check_Pieces_NoOv.Enabled = (selectionList.Count > 0);
            // Set check-mark correctly, without firing the CheckedChanged event
            check_Pieces_NoOv.CheckedChanged -= check_Pieces_NoOv_CheckedChanged;
            check_Pieces_NoOv.Checked = selectionList.Exists(p => (p is GadgetPiece && (p as GadgetPiece).IsNoOverwrite) 
                                                               || (p is TerrainPiece && (p as TerrainPiece).IsNoOverwrite));
            check_Pieces_NoOv.CheckedChanged += check_Pieces_NoOv_CheckedChanged;

            check_Pieces_Erase.Enabled = selectionList.Exists(p => p is TerrainPiece);
            // Set check-mark correctly, without firing the CheckedChanged event
            check_Pieces_Erase.CheckedChanged -= check_Pieces_Erase_CheckedChanged;
            check_Pieces_Erase.Checked = selectionList.Exists(p => p is TerrainPiece && (p as TerrainPiece).IsErase);
            check_Pieces_Erase.CheckedChanged += check_Pieces_Erase_CheckedChanged;
            
            check_Pieces_OneWay.Enabled = selectionList.Exists(p => p is TerrainPiece && !(p as TerrainPiece).IsSteel);
            // Set check-mark correctly, without firing the CheckedChanged event
            check_Pieces_OneWay.CheckedChanged -= check_Pieces_OneWay_CheckedChanged;
            check_Pieces_OneWay.Checked = selectionList.Exists(p => p is TerrainPiece && (p as TerrainPiece).IsOneWay);
            check_Pieces_OneWay.CheckedChanged += check_Pieces_OneWay_CheckedChanged;

            check_Pieces_OnlyOnTerrain.Enabled = selectionList.Exists(p => p is GadgetPiece);
            // Set check-mark correctly, without firing the CheckedChanged event
            check_Pieces_OnlyOnTerrain.CheckedChanged -= check_Pieces_OnlyOnTerrain_CheckedChanged;
            check_Pieces_OnlyOnTerrain.Checked = selectionList.Exists(p => p is GadgetPiece && (p as GadgetPiece).IsOnlyOnTerrain);
            check_Pieces_OnlyOnTerrain.CheckedChanged += check_Pieces_OnlyOnTerrain_CheckedChanged;

            but_GroupSelection.Enabled = false; //CurLevel.MayGroupSelection();
            but_UngroupSelection.Enabled = false; //CurLevel.MayUngroupSelection();

            foreach (C.Skill skill in checkboxesSkillFlags.Keys)
            {
                checkboxesSkillFlags[skill].Enabled = selectionList.Exists(p => p.MayReceiveSkill(skill));
                // Set check-mark correctly, without firing the CheckedChanged event
                checkboxesSkillFlags[skill].CheckedChanged -= check_Piece_Skill_CheckedChanged;
                checkboxesSkillFlags[skill].Checked = selectionList.Exists(p => p is GadgetPiece && (p as GadgetPiece).SkillFlags.Contains(skill));
                checkboxesSkillFlags[skill].CheckedChanged += check_Piece_Skill_CheckedChanged;
            }

            if (selectionList.Count == 1 && selectionList[0] is GadgetPiece)
            { 
                GadgetPiece MyGadget = (GadgetPiece)selectionList[0];
                lbl_Resize_Width.Visible = MyGadget.MayResizeHoriz();
                num_Resize_Width.Maximum = CurLevel.Width;
                num_Resize_Width.Value = Math.Min(Math.Max(MyGadget.SpecWidth, num_Resize_Width.Minimum), num_Resize_Width.Maximum);
                num_Resize_Width.Visible = MyGadget.MayResizeHoriz();

                lbl_Resize_Height.Visible = MyGadget.MayResizeVert();
                num_Resize_Height.Maximum = CurLevel.Height;
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

        }


        /// <summary>
        /// Repositions the controls after resizing the main form.
        /// </summary>
        private void MoveControlsOnFormResize()
        {
            scrollPicLevelHoriz.Top = this.Height - 144;
            scrollPicLevelVert.Left = this.Width - 29;

            RepositionPicLevel();

            foreach (TabControl tabControl in this.Controls.OfType<TabControl>())
            {
                tabControl.Height = this.Height - 178;
            }

            combo_PieceStyle.Top = this.Height - 149;
            but_PieceTerrObj.Top = this.Height - 149;

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
            pic_Level.Left = 188 + (curSettings.UseLvlPropertiesTabs ? 0 : 328);

            Size newPicLevelSize = new Size(this.Width - 200, this.Height - 155);

            if (!curSettings.UseLvlPropertiesTabs)
            {
                newPicLevelSize.Width -= 328;
                scrollPicLevelVert.Left += 328; // Width is set correctly in CheckEnableLevelScrollbars().
            }

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

            if (displayScrollHoriz) newPicBoxSize.Height -= 16;
            if (displayScrollVert) newPicBoxSize.Width -= 16;

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
                int maxValue = CurLevel.Width - displayedLevelRect.Width + 1;
                scrollPicLevelHoriz.Maximum = maxValue;
                scrollPicLevelHoriz.SmallChange = 1;
                scrollPicLevelHoriz.LargeChange = 2;
                scrollPicLevelHoriz.Value = Math.Min(displayedLevelRect.Left, maxValue - 1);
            }
            scrollPicLevelHoriz.Enabled = displayScrollHoriz;
            scrollPicLevelHoriz.Visible = displayScrollHoriz;

            
            // Set scrollPicLevelVert
            if (displayScrollVert)
            {
                int maxValue = CurLevel.Height - displayedLevelRect.Height + 1;
                scrollPicLevelVert.Maximum = maxValue;
                scrollPicLevelVert.SmallChange = 1;
                scrollPicLevelVert.LargeChange = 2;
                scrollPicLevelVert.Value = Math.Min(displayedLevelRect.Top, maxValue - 1);
            }
            scrollPicLevelVert.Enabled = displayScrollVert;
            scrollPicLevelVert.Visible = displayScrollVert;

            // finally resize scrollbars correctly
            scrollPicLevelHoriz.Width = newPicBoxSize.Width - 2;
            scrollPicLevelVert.Height = newPicBoxSize.Height - 2;

            return newPicBoxSize;
        }

        /// <summary>
        /// Sets the scrollbar values for the editor screen position correctly.
        /// </summary>
        private void UpdateScrollBarValues()
        {
            if (scrollPicLevelHoriz.Enabled)
            {
                scrollPicLevelHoriz.Value = Math.Min(curRenderer.ScreenPosX, scrollPicLevelHoriz.Maximum - 1);
            }
            if (scrollPicLevelVert.Enabled)
            {
                scrollPicLevelVert.Value = Math.Min(curRenderer.ScreenPosY, scrollPicLevelVert.Maximum - 1);
            }
        }


        /// <summary>
        /// Moves the picture boxes to select new pieces to the correct position.
        /// </summary>
        /// <returns></returns>
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
        /// <returns></returns>
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
        /// Updates the possible background images depending on the currently selected main style.
        /// </summary>
        private void UpdateBackgroundComboItems()
        {
            combo_Background.Items.Clear();
            combo_Background.Items.AddRange(Backgrounds.GetDisplayNames(CurLevel.MainStyle).ToArray());
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


        /// <summary>
        /// Displays a list of all hotkeys in a new form window.
        /// </summary>
        private void DisplayHotkeyForm()
        {
            Form hotkeyForm = new Form();
            hotkeyForm.Width = 450;
            hotkeyForm.Height = 430;
            hotkeyForm.MaximizeBox = false;
            hotkeyForm.ShowInTaskbar = false;
            hotkeyForm.FormBorderStyle = FormBorderStyle.FixedToolWindow;
            hotkeyForm.Text = "NLEditor - Hotkeys";

            TabControl hotkeyTabs = new TabControl();
            hotkeyTabs.Location = new Point(0, 0);
            hotkeyTabs.SelectedIndex = 0;
            hotkeyTabs.Size = new Size(hotkeyForm.Width - 6, hotkeyForm.Height - 23);
            hotkeyTabs.TabIndex = 1;
            hotkeyTabs.TabStop = false;

            var tabDictionary = new Dictionary<C.HotkeyTabs, TabPage>
            {
                { C.HotkeyTabs.General, new TabPage("General") },
                { C.HotkeyTabs.Level, new TabPage("Level modification") },
                { C.HotkeyTabs.Pieces, new TabPage("Piece modification") },
            };

            foreach (C.HotkeyTabs tab in tabDictionary.Keys)
            {
                // Create labels
                Label lblKeys = new Label();
                lblKeys.Location = new Point(0, 4);
                lblKeys.Size = new Size(150, hotkeyTabs.Height - 4);
                lblKeys.Text = string.Join(C.NewLine, C.HotkeyDict[tab]);
                Label lblDescription = new Label();
                lblDescription.Location = new Point(150, 4);
                lblDescription.Size = new Size(hotkeyTabs.Width - 154, hotkeyTabs.Height - 4);
                lblDescription.Text = string.Join(C.NewLine, C.DescriptionDict[tab]);

                // Add labels to tab
                tabDictionary[tab].Controls.Add(lblKeys);
                tabDictionary[tab].Controls.Add(lblDescription);

                // Add TabPage to TabControl
                hotkeyTabs.Controls.Add(tabDictionary[tab]);
            }

            hotkeyForm.Controls.Add(hotkeyTabs);
            hotkeyForm.Show();
        }

        /// <summary>
        /// Displays the "About..." page with attribution and license.
        /// </summary>
        private void DisplayVersionForm()
        {
            Form versionForm = new Form();
            versionForm.Width = 310;
            versionForm.Height = 170;
            versionForm.MaximizeBox = false;
            versionForm.ShowInTaskbar = false;
            versionForm.FormBorderStyle = FormBorderStyle.FixedToolWindow;
            versionForm.Text = "NLEditor - About";

            versionForm.Show();

            using (Graphics hotkeyGraphics = versionForm.CreateGraphics())
            using (SolidBrush brush = new SolidBrush(Color.Black))
            using (Font font = new Font("Microsoft Sans Serif", 8))
            {
                for (int i = 0; i < C.VersionList.Count; i++)
                {
                    hotkeyGraphics.DrawString(C.VersionList[i], font, brush, 6, 6 + 13 * i);
                }
            }
        }

        /// <summary>
        /// Applies the option UseLvlPropertiesTabs to the editor.
        /// </summary>
        public void ApplyOptionLvlPropertiesTabs()
        {
            var tabWithPieces = (TabControl)tabPieces.Parent;
            var tabWithSkills = (TabControl)tabSkills.Parent;

            if (curSettings.UseLvlPropertiesTabs)
            {
                if (tabPieces.Parent == tabLvlPieces)
                {
                    tabLvlPieces.Enabled = false;
                    tabLvlPieces.Visible = false;
                    tabLvlPieces.TabPages.Remove(tabPieces);
                    tabLvlProperties.TabPages.Add(tabPieces);
                }
                
                if (tabSkills.Parent == tabLvlSkills)
                {
                    tabLvlSkills.Enabled = false;
                    tabLvlSkills.Visible = false;
                    tabLvlSkills.TabPages.Remove(tabSkills);
                    tabLvlProperties.TabPages.Add(tabSkills);
                }
            }
            else
            {
                if (tabWithPieces == tabLvlProperties)
                {
                    tabLvlPieces.Enabled = true;
                    tabLvlPieces.Visible = true;
                    tabLvlProperties.TabPages.Remove(tabPieces);
                    tabLvlPieces.TabPages.Add(tabPieces);
                }

                if (tabWithSkills == tabLvlProperties)
                {
                    tabLvlSkills.Enabled = true;
                    tabLvlSkills.Visible = true;
                    tabLvlProperties.TabPages.Remove(tabSkills);
                    tabLvlSkills.TabPages.Add(tabSkills);
                }
            }

            RepositionPicLevel();
            curRenderer.EnsureScreenPosInLevel();
            pic_Level.Image = curRenderer.CreateLevelImage();
        }
    }
}
