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
            SearchDirectoryForBackgrounds();

            RemoveDuplicatedObjects();
            SortObjectNamesByObjectType();

            fColorDict = LoadStylesFromFile.StyleColors(NameInDirectory);
        }

        string fNameInDirectory;
        string fNameInEditor;
        List<string> fTerrainNames;
        List<string> fObjectNames;
        List<string> fBackgroundNames;
        Dictionary<C.StyleColor, Color> fColorDict;

        public string NameInDirectory { get { return fNameInDirectory; } }
        public string NameInEditor { get { return fNameInEditor; } set { fNameInEditor = value; } }
        public List<string> TerrainNames { get { return fTerrainNames; } }
        public List<string> ObjectNames { get { return fObjectNames; } }
        public List<string> BackgroundNames { get { return fBackgroundNames; } }

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
        /// Reads the style's color or a default value if no color is specified.
        /// </summary>
        /// <param name="ColorType"></param>
        /// <returns></returns>
        public Color GetColor(C.StyleColor ColorType)
        {
            if (fColorDict.ContainsKey(ColorType)) return fColorDict[ColorType];
            else
            {
                switch (ColorType)
                {
                    case C.StyleColor.BACKGROUND: return Color.Black;
                    default: return Color.Azure;
                }
            } 
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
            else // use empty list
            {
                fTerrainNames = new List<string>();
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

                fObjectNames.Add("default" + C.DirSep + "objects" + C.DirSep + "lemming");
            }
            catch (Exception Ex)
            {
                Utility.LogException(Ex);

                System.Windows.Forms.MessageBox.Show("Warning:" + Ex.Message);
                // ...but then start the editor as usual
            }

            if (fObjectNames == null) fObjectNames = new List<string>();
        }

        /// <summary>
        /// Writes all pieces in AppPath/StyleName/backgrounds to the list of BackgroundNames.
        /// </summary>
        private void SearchDirectoryForBackgrounds()
        {
            string DirectoryPath = C.AppPathPieces + NameInDirectory + C.DirSep + "backgrounds";

            if (Directory.Exists(DirectoryPath))
            {
                fBackgroundNames = Directory.GetFiles(DirectoryPath, "*.png", SearchOption.TopDirectoryOnly)
                                            .Select(file => ImageLibrary.CreatePieceKey(file))
                                            .ToList();
            }
            else // use empty list
            {
                fBackgroundNames = new List<string>();
            }
        }

        /// <summary>
        /// Removes all default objects, that are already present in the actual style.
        /// </summary>
        private void RemoveDuplicatedObjects()
        {
            fObjectNames.RemoveAll(obj => obj.StartsWith("default")
                                          && !ImageLibrary.GetObjType(obj).In(C.OBJ.NONE, C.OBJ.ANIMATION, C.OBJ.BACKGROUND)
                                          && fObjectNames.Exists(obj2 => !obj2.StartsWith("default")
                                                                 && ImageLibrary.GetObjType(obj) == ImageLibrary.GetObjType(obj2)));
        }

        /// <summary>
        /// Sorts the list of object names according to their object types.
        /// </summary>
        private void SortObjectNamesByObjectType()
        {
            fObjectNames.Sort((obj1, obj2) => ImageLibrary.GetObjType(obj1).CompareTo(ImageLibrary.GetObjType(obj2)));
        }
    }
}
