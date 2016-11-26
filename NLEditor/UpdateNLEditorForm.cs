using System;
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
        private void LoadPiecesIntoPictureBox(NLEditForm MyForm, Style NewStyle)
        {
            if (NewStyle == null)
            {
                ClearPiecesPictureBox(MyForm);
                return;
            }

            // Get correct list of piece names
            List<string> ThisPieceNameList = MyForm.PieceDoDisplayObject ? NewStyle.ObjectNames : NewStyle.TerrainNames;
            if (ThisPieceNameList == null || ThisPieceNameList.Count == 0)
            {
                ClearPiecesPictureBox(MyForm);
                return;
            }

            // load correct pictures
            for (int i = 0; i < MyForm.picPieceList.Count; i++)
            {
                string ThisPieceName = ThisPieceNameList[(MyForm.PieceStartIndex + i) % ThisPieceNameList.Count];
                MyForm.picPieceList[i].Image = ImageLibrary.GetImage(ThisPieceName);
            }

            return;
        }

        /// <summary>
        /// Returns a style with the requested name, or null if none such is found. 
        /// </summary>
        /// <param name="MyForm"></param>
        /// <param name="NewStyleName"></param>
        /// <returns></returns>
        private Style ValidateStyleName(NLEditForm MyForm, string NewStyleName)
        {
            if (MyForm.StyleList == null || MyForm.StyleList.Count == 0) return null;

            return MyForm.StyleList.Find(sty => sty.NameInEditor == NewStyleName);
        }

        /// <summary>
        /// Clears all piece selection PictureBoxes.
        /// </summary>
        /// <param name="MyForm"></param>
        private void ClearPiecesPictureBox(NLEditForm MyForm)
        {
            MyForm.picPieceList.ForEach(pic => pic.Image = null);
        }

        /// <summary>
        /// Changes the background color of the main level image and the piece selection.
        /// </summary>
        /// <param name="MyForm"></param>
        /// <param name="NewStyle"></param>
        private void ChangeBackgroundColor(NLEditForm MyForm, Style NewStyle)
        {
            if (NewStyle == null) return;

            Color NewBackColor = NewStyle.BackgroundColor;
            if (NewBackColor == null) return;

            MyForm.picPieceList.ForEach(pic => pic.BackColor = NewBackColor);

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
