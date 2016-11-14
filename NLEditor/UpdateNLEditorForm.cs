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

        private Style ValidateStyleName(NLEditForm MyForm, string NewStyleName)
        {
            if (MyForm.StyleList == null || MyForm.StyleList.Count == 0) return null;

            return MyForm.StyleList.Find(sty => sty.Name == NewStyleName);
        }

        private void ClearPiecesPictureBox(NLEditForm MyForm)
        {
            MyForm.picPieceList.ForEach(pic => pic.Image = null);
        }

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
    }
}
