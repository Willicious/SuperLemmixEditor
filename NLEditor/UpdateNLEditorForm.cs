using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NLEditor
{
    static class UpdateForm
    {
        public static void LoadPiecesIntoPictureBox(NLEditForm MyForm, string NewStyleName)
        {
            if (MyForm.StyleList == null || MyForm.StyleList.Count == 0) return;

            // Get style
            Style ThisStyle = MyForm.StyleList.Find(sty => sty.Name == NewStyleName);
            if (ThisStyle == null) return;

            // Get correct list of piece names
            List<string> ThisPieceNameList = MyForm.PieceDoDisplayObject ? ThisStyle.ObjectNames : ThisStyle.PieceNames;
            if (ThisPieceNameList == null || ThisPieceNameList.Count == 0) return;

            // load correct pictures
            for (int i = 0; i < MyForm.picPieceList.Count; i++)
            {
                string ThisPieceName = ThisPieceNameList[(MyForm.PieceStartIndex + i) % MyForm.picPieceList.Count];
                MyForm.picPieceList[i].Image = ImageLibrary.GetImage(ThisPieceName);
            }
        }

    }
}
