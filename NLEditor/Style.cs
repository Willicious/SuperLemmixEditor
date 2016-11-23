using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.IO;

namespace NLEditor
{
    /// <summary>
    /// Stores all data of a graphics style, except the images themselves.
    /// </summary>
    public class Style
    {
        /*---------------------------------------------------------
         *          This class stores all style infos
         * -------------------------------------------------------- */
        /// <summary>
        /// Initializes a new instance of a Style by searching for pieces in the directory AppPath/StyleName/.
        /// </summary>
        /// <param name="StyleName"></param>
        public Style(string StyleName)
        {
            this.fNameInDirectory = StyleName;
            this.fNameInEditor = StyleName; // may be overwritten later when forming the StyleList

            SearchDirectoryForTerrain();
            SearchDirectoryForObjects();
            List<Color> StyleColors = LoadStylesFromFile.StyleColors(NameInDirectory);
            fBackgroundColor = StyleColors[0];
        }

        string fNameInDirectory;
        string fNameInEditor;
        List<string> fTerrainNames;
        List<string> fObjectNames;
        Color fBackgroundColor;

        public string NameInDirectory { get { return fNameInDirectory; } }
        public string NameInEditor { get { return fNameInEditor; } set { fNameInEditor = value; } }
        public List<string> TerrainNames { get { return fTerrainNames; } }
        public List<string> ObjectNames { get { return fObjectNames; } }
        public Color BackgroundColor { get { return fBackgroundColor; } }

        /// <summary>
        /// Checks for equality of the style's FileName.
        /// </summary>
        /// <param name="OtherStyle"></param>
        /// <returns></returns>
        public bool Equals(Style OtherStyle)
        {
            return this.NameInDirectory.Equals(OtherStyle.NameInDirectory);
        }

        /// <summary>
        /// Writes all pieces in AppPath/StyleName/terrain to the list of TerrainNames.
        /// </summary>
        private void SearchDirectoryForTerrain()
        {
            string DirectoryPath = C.AppPathPieces + NameInDirectory + C.DirSep + "terrain";

            if (Directory.Exists(DirectoryPath))
            {
                fTerrainNames = Directory.GetFiles(DirectoryPath, "*.png", SearchOption.TopDirectoryOnly)
                                         .Select(file => ImageLibrary.CreatePieceKey(file))
                                         .ToList();
            }
        }

        /// <summary>
        /// Writes all pieces in AppPath/StyleName/objects and /default to the list of ObjectNames.
        /// </summary>
        private void SearchDirectoryForObjects()
        {
            // Load first the style-specific objects
            string DirectoryPath = C.AppPathPieces + NameInDirectory + C.DirSep + "objects";

            if (Directory.Exists(DirectoryPath))
            {
                fObjectNames = Directory.GetFiles(DirectoryPath, "*.png", SearchOption.TopDirectoryOnly)
                                        .Select(file => ImageLibrary.CreatePieceKey(Path.GetFullPath(file)))
                                        .ToList();
            }
            else
            {
                fObjectNames = new List<string>();
            }

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
                Utility.LogException(Ex);

                System.Windows.Forms.MessageBox.Show("Warning:" + Ex.Message);
                // ...but then start the editor as usual
            }
        }
    }
}
