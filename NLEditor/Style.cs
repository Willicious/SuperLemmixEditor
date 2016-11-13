using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Windows.Forms;
using System.IO;

namespace NLEditor
{
    public class Style
    {
        /*---------------------------------------------------------
         *          This class stores all style infos
         * -------------------------------------------------------- */

        public Style(string StyleName)
        {
            this.fFileName = StyleName;
            this.fName = StyleName; // may be overwritten later when forming the StyleList

            SearchDirectoryForTerrain();
            SearchDirectoryForObjects();
            List<Color> StyleColors = LoadFromFile.StyleColors(FileName);
            fBackgroundColor = StyleColors[0];
        }

        string fFileName; // name that is used in the directory
        string fName; // name that is displayed in the editor
        List<string> fPieceNames;
        List<string> fObjectNames;
        Color fBackgroundColor;

        public string FileName { get { return fFileName; } }
        public string Name { get { return fName; } set { fName = value; } }
        public List<string> PieceNames { get { return fPieceNames; } }
        public List<string> ObjectNames { get { return fObjectNames; } }
        public Color BackgroundColor { get { return fBackgroundColor; } }

        public bool Equals(Style OtherStyle)
        {
            return this.FileName.Equals(OtherStyle.FileName);
        }

        private void SearchDirectoryForTerrain()
        {
            string DirectoryPath = C.AppPathPieces + FileName + C.DirSep + "terrain";

            fPieceNames = Directory.GetFiles(DirectoryPath, "*.png", SearchOption.TopDirectoryOnly)
                                   .Select(file => ImageLibrary.CreatePieceKey(file))
                                   .ToList();
        }

        private void SearchDirectoryForObjects()
        {
            // Load first the style-specific objects
            string DirectoryPath = C.AppPathPieces + FileName + C.DirSep + "objects";

            fObjectNames = Directory.GetFiles(DirectoryPath, "*.png", SearchOption.TopDirectoryOnly)
                                    .Select(file => ImageLibrary.CreatePieceKey(Path.GetFullPath(file)))
                                    .ToList();
            

            // Load now the default objects into the list
            DirectoryPath = C.AppPathPieces + "default" + C.DirSep + "objects";

            try
            {
                fObjectNames.AddRange(Directory.GetFiles(DirectoryPath, "*.png", SearchOption.TopDirectoryOnly)
                                               .Select(file => ImageLibrary.CreatePieceKey(Path.GetFullPath(file)))
                                               .ToList()
                                               .FindAll(key => !key.Contains("_mask_")));
            }
            catch (Exception Ex)
            {
                String ErrorPath = C.AppPath + "ErrorLog.txt";
                TextWriter TextFile = new StreamWriter(ErrorPath, true);
                TextFile.WriteLine(Ex.ToString());
                TextFile.Close();

                MessageBox.Show(Ex.Message);
                // but then start the editor as usual
            }
        }
    }
}
