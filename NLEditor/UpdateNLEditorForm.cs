using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace NLEditor
{
    static class UpdateForm
    {
        public static void LoadPiecesIntoPictureBox(NLEditForm MyForm, Style NewStyle)
        {
            // Style ThisStyle = ValidateStyleName(MyForm, NewStyleName);
            if (NewStyle == null)
            {
                ClearPiecesPictureBox(MyForm);
                return;
            }

            // Get correct list of piece names
            List<string> ThisPieceNameList = MyForm.PieceDoDisplayObject ? NewStyle.ObjectNames : NewStyle.PieceNames;
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

        public static Style ValidateStyleName(NLEditForm MyForm, string NewStyleName)
        {
            if (MyForm.StyleList == null || MyForm.StyleList.Count == 0) return null;

            return MyForm.StyleList.Find(sty => sty.Name == NewStyleName);
        }

        public static void ClearPiecesPictureBox(NLEditForm MyForm)
        {
            MyForm.picPieceList.ForEach(pic => pic.Image = null);
        }

        public static void ChangeBackgroundColor(NLEditForm MyForm, Style NewStyle)
        {
            if (NewStyle == null) return;

            Color NewBackColor = NewStyle.BackgroundColor;
            if (NewBackColor == null) return;

            MyForm.picPieceList.ForEach(pic => pic.BackColor = NewBackColor);
        }

        public static void ChangePieceStartIndex(NLEditForm MyForm, int Movement)
        {
            Style CurStyle = MyForm.PieceCurStyle;
            if (CurStyle == null) return;

            List<string> PieceNameList = MyForm.PieceDoDisplayObject ? CurStyle.ObjectNames : CurStyle.PieceNames;
            if (PieceNameList == null || PieceNameList.Count == 0) return;

            // Pass to correct piece index
            MyForm.PieceStartIndex = (MyForm.PieceStartIndex + Movement) % PieceNameList.Count;
            // ensure that PieceStartIndex is positive
            MyForm.PieceStartIndex = (MyForm.PieceStartIndex + PieceNameList.Count) % PieceNameList.Count;

            UpdateForm.LoadPiecesIntoPictureBox(MyForm, CurStyle);
        }

    }
}
