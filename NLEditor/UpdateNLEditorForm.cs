﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;


namespace NLEditor
{
    public partial class NLEditForm
    {
        /*---------------------------------------------------------
        *   Main Form: This part defines the methods
        *     updating the form members
        * -------------------------------------------------------- */
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

            // MOVE FRONT/BACK ---------------------------> TODO!!

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
            
            check_Pieces_OneWay.Enabled = SelectionList.Exists(p => p is TerrainPiece);
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
        }



    }
}
