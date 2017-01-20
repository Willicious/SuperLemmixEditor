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
            but_PieceLeft.SetInterval(30, MouseButtons.Right);
            but_PieceRight.SetInterval(100, MouseButtons.Left);
            but_PieceRight.SetInterval(30, MouseButtons.Right);
        }
        
        /// <summary>
        /// Displays the correct piece images for the piece selection.
        /// </summary>
        private void LoadPiecesIntoPictureBox()
        {
            if (pieceCurStyle == null)
            {
                ClearPiecesPictureBox();
                return;
            }

            // Get correct list of piece names
            var pieceNameList = pieceDoDisplayObject ? pieceCurStyle.ObjectNames : pieceCurStyle.TerrainNames;
            if (pieceNameList == null || pieceNameList.Count == 0)
            {
                ClearPiecesPictureBox();
                return;
            }

            // load correct pictures
            for (int i = 0; i < picPieceList.Count; i++)
            {
                string pieceName = pieceNameList[(pieceStartIndex + i) % pieceNameList.Count];
                if (pieceName.StartsWith("default") && ImageLibrary.GetObjType(pieceName) == C.OBJ.ONE_WAY_WALL)
                {
                    picPieceList[i].Image = GetRecoloredOWW(pieceName);
                }
                else
                {
                    int frameIndex = (ImageLibrary.GetObjType(pieceName).In(C.OBJ.PICKUP, C.OBJ.EXIT_LOCKED, C.OBJ.BUTTON, C.OBJ.TRAPONCE)) ? 1 : 0;
                    picPieceList[i].Image = ImageLibrary.GetImage(pieceName, RotateFlipType.RotateNoneFlipNone, frameIndex);
                }
                SetToolTipsForPicPiece(picPieceList[i], pieceName);
            }

            return;
        }

        /// <summary>
        /// Returns a recolored OWW according to the OWW color of the current main style.
        /// </summary>
        /// <param name="pieceName"></param>
        /// <returns></returns>
        private Bitmap GetRecoloredOWW(string pieceName)
        {
            Color owwColor = CurLevel.MainStyle?.GetColor(C.StyleColor.ONE_WAY_WALL) ?? Color.Linen;
            byte[] owwColorbytes = new byte[] { owwColor.B, owwColor.G, owwColor.R, 255 };
            BmpModify.SetCustomDrawMode((x, y) => owwColorbytes, null);
            Bitmap origBmp = ImageLibrary.GetImage(pieceName, RotateFlipType.RotateNoneFlipNone);
            Bitmap newBmp = new Bitmap(origBmp.Width, origBmp.Height);
            newBmp.DrawOn(origBmp, new Point(0, 0), C.CustDrawMode.Custom);
            return newBmp;
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
            Color backColor = CurLevel.MainStyle.GetColor(C.StyleColor.BACKGROUND);

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
            pic_Level.Left = 188;
            pic_Level.Width = this.Width - 200;
            pic_Level.Height = this.Height - 155;

            if (!curSettings.UseLvlPropertiesTabs)
            {
                pic_Level.Left += 328;
                pic_Level.Width -= 328;
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
            picPiece.Width = 84;
            picPiece.Height = 84;
            picPiece.Top = this.Height - 122;
            picPiece.BorderStyle = BorderStyle.Fixed3D;
            picPiece.SizeMode = PictureBoxSizeMode.CenterImage;

            picPiece.Click += new EventHandler(picPieces_Click);

            this.Controls.Add(picPiece);

            return picPiece;
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
                string[] backgroundNames = CurLevel.MainStyle.BackgroundNames.Select(name => 
                                               System.IO.Path.GetFileName(name)).ToArray();
                combo_Background.Items.AddRange(backgroundNames);
            }
        }



        /// <summary>
        /// Displays a list of all hotkeys in a new form window.
        /// </summary>
        private void DisplayHotkeyForm()
        {
            Form hotkeyForm = new Form();
            hotkeyForm.Width = 450;
            hotkeyForm.Height = 410;
            hotkeyForm.MaximizeBox = false;
            hotkeyForm.FormBorderStyle = FormBorderStyle.FixedToolWindow;
            hotkeyForm.Text = "NLEditor - Hotkeys";

            TabControl hotkeyTabs = new TabControl();
            hotkeyTabs.Location = new Point(0, 0);
            hotkeyTabs.SelectedIndex = 0;
            hotkeyTabs.Size = new Size(hotkeyForm.Width - 6, hotkeyForm.Height - 23);
            hotkeyTabs.TabIndex = 1;
            hotkeyTabs.TabStop = false;

            TabPage tabHotkeysGeneral = new TabPage("General");
            TabPage tabHotkeysPieces = new TabPage("Piece modification");
            Label lblHotkeysGeneralKeys = new Label();
            lblHotkeysGeneralKeys.Location = new Point(0, 4);
            lblHotkeysGeneralKeys.Size = new Size(150, hotkeyTabs.Height - 4);
            lblHotkeysGeneralKeys.Text = String.Join(C.NewLine, C.HotkeyDict[C.HotkeyTabs.General]);
            Label lblHotkeysGeneralDescription = new Label();
            lblHotkeysGeneralDescription.Location = new Point(150, 4);
            lblHotkeysGeneralDescription.Size = new Size(hotkeyTabs.Width - 154, hotkeyTabs.Height - 4);
            lblHotkeysGeneralDescription.Text = String.Join(C.NewLine, C.DescriptionDict[C.HotkeyTabs.General]);

            Label lblHotkeysPiecesKeys = new Label();
            lblHotkeysPiecesKeys.Location = new Point(0, 4);
            lblHotkeysPiecesKeys.Size = new Size(150, hotkeyTabs.Height - 4);
            lblHotkeysPiecesKeys.Text = String.Join(C.NewLine, C.HotkeyDict[C.HotkeyTabs.Pieces]);
            Label lblHotkeysPiecesDescription = new Label();
            lblHotkeysPiecesDescription.Location = new Point(150, 4);
            lblHotkeysPiecesDescription.Size = new Size(hotkeyTabs.Width - 154, hotkeyTabs.Height - 4);
            lblHotkeysPiecesDescription.Text = String.Join(C.NewLine, C.DescriptionDict[C.HotkeyTabs.Pieces]);

            tabHotkeysGeneral.Controls.Add(lblHotkeysGeneralKeys);
            tabHotkeysGeneral.Controls.Add(lblHotkeysGeneralDescription);
            tabHotkeysPieces.Controls.Add(lblHotkeysPiecesKeys);
            tabHotkeysPieces.Controls.Add(lblHotkeysPiecesDescription);
            hotkeyTabs.Controls.Add(tabHotkeysGeneral);
            hotkeyTabs.Controls.Add(tabHotkeysPieces);
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
                if (tabPieces.Parent == this.tabLvlPieces)
                {
                    this.tabLvlPieces.Enabled = false;
                    this.tabLvlPieces.Visible = false;
                    this.tabLvlPieces.TabPages.Remove(tabPieces);
                    this.tabLvlProperties.TabPages.Add(tabPieces);
                }
                
                if (tabSkills.Parent == this.tabLvlSkills)
                {
                    this.tabLvlSkills.Enabled = false;
                    this.tabLvlSkills.Visible = false;
                    this.tabLvlSkills.TabPages.Remove(tabSkills);
                    this.tabLvlProperties.TabPages.Add(tabSkills);
                }
            }
            else
            {
                if (tabWithPieces == this.tabLvlProperties)
                {
                    this.tabLvlPieces.Enabled = true;
                    this.tabLvlPieces.Visible = true;
                    this.tabLvlProperties.TabPages.Remove(tabPieces);
                    this.tabLvlPieces.TabPages.Add(tabPieces);
                }

                if (tabWithSkills == this.tabLvlProperties)
                {
                    this.tabLvlSkills.Enabled = true;
                    this.tabLvlSkills.Visible = true;
                    this.tabLvlProperties.TabPages.Remove(tabSkills);
                    this.tabLvlSkills.TabPages.Add(tabSkills);
                }
            }

            RepositionPicLevel();
            curRenderer.EnsureScreenPosInLevel();
            this.pic_Level.Image = curRenderer.CreateLevelImage();
        }




    }
}
