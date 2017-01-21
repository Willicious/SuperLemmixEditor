using System;
using System.Collections.Generic;
using System.Linq;
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
        /// <param name="styleName"></param>
        public Style(string styleName)
        {
            NameInDirectory = styleName;
            NameInEditor = styleName; // may be overwritten later when forming the StyleList

            SearchDirectoryForTerrain();
            SearchDirectoryForObjects();
            SearchDirectoryForBackgrounds();

            RemoveDuplicatedObjects();
            SortObjectNamesByObjectType();

            colorDict = LoadStylesFromFile.StyleColors(NameInDirectory);
        }

        Dictionary<C.StyleColor, Color> colorDict;

        public string NameInDirectory { get; private set; }
        public string NameInEditor { get; set; }
        public List<string> TerrainKeys { get; private set; }
        public List<string> ObjectKeys { get; private set; }
        public List<string> BackgroundKeys { get; private set; }

        /// <summary>
        /// Checks for equality of the style's FileName.
        /// </summary>
        /// <param name="otherStyle"></param>
        /// <returns></returns>
        public bool Equals(Style otherStyle)
        {
            return this.NameInDirectory.Equals(otherStyle.NameInDirectory);
        }

        /// <summary>
        /// Reads the style's color or a default value if no color is specified.
        /// </summary>
        /// <param name="colorType"></param>
        /// <returns></returns>
        public Color GetColor(C.StyleColor colorType)
        {
            if (colorDict.ContainsKey(colorType)) return colorDict[colorType];
            else return C.NLColors[(colorType == C.StyleColor.BACKGROUND) ? C.NLColor.BackDefault : C.NLColor.OWWDefault];
        }


        /// <summary>
        /// Writes all pieces in AppPath/StyleName/terrain to the list of TerrainNames.
        /// </summary>
        private void SearchDirectoryForTerrain()
        {
            string directoryPath = C.AppPathPieces + NameInDirectory + C.DirSep + "terrain";

            if (Directory.Exists(directoryPath))
            {
                TerrainKeys = Directory.GetFiles(directoryPath, "*.png", SearchOption.TopDirectoryOnly)
                                        .Select(file => ImageLibrary.CreatePieceKey(file))
                                        .ToList();
            }
            else // use empty list
            {
                TerrainKeys = new List<string>();
            }
        }

        /// <summary>
        /// Writes all pieces in AppPath/StyleName/objects and /default to the list of ObjectNames.
        /// </summary>
        private void SearchDirectoryForObjects()
        {
            // Load first the style-specific objects
            string directoryPath = C.AppPathPieces + NameInDirectory + C.DirSep + "objects";

            if (Directory.Exists(directoryPath))
            {
                ObjectKeys = Directory.GetFiles(directoryPath, "*.png", SearchOption.TopDirectoryOnly)
                                       .Select(file => ImageLibrary.CreatePieceKey(Path.GetFullPath(file)))
                                       .ToList();
            }
            else
            {
                ObjectKeys = new List<string>();
            }

            // Load now the default objects into the list
            string directoryPathDefault = C.AppPathPieces + "default" + C.DirSep + "objects";

            try
            {
                ObjectKeys.AddRange(Directory.GetFiles(directoryPathDefault, "*.png", SearchOption.TopDirectoryOnly)
                                              .Select(file => ImageLibrary.CreatePieceKey(Path.GetFullPath(file)))
                                              .ToList()
                                              .FindAll(key => !key.Contains("_mask_")));

                ObjectKeys.Add("default" + C.DirSep + "objects" + C.DirSep + "lemming");
            }
            catch (Exception Ex)
            {
                Utility.LogException(Ex);

                System.Windows.Forms.MessageBox.Show("Warning:" + Ex.Message, "Files not found");
                // ...but then start the editor as usual
            }
        }

        /// <summary>
        /// Writes all pieces in AppPath/StyleName/backgrounds to the list of BackgroundNames.
        /// </summary>
        private void SearchDirectoryForBackgrounds()
        {
            string directoryPath = C.AppPathPieces + NameInDirectory + C.DirSep + "backgrounds";

            if (Directory.Exists(directoryPath))
            {
                BackgroundKeys = Directory.GetFiles(directoryPath, "*.png", SearchOption.TopDirectoryOnly)
                                           .Select(file => ImageLibrary.CreatePieceKey(file))
                                           .ToList();
            }
            else // use empty list
            {
                BackgroundKeys = new List<string>();
            }
        }

        /// <summary>
        /// Removes all default objects, that are already present in the actual style.
        /// </summary>
        private void RemoveDuplicatedObjects()
        {
            ObjectKeys.RemoveAll(obj => obj.StartsWith("default")
                                      && !ImageLibrary.GetObjType(obj).In(C.OBJ.NONE, C.OBJ.ANIMATION, C.OBJ.BACKGROUND)
                                      && ObjectKeys.Exists(obj2 => !obj2.StartsWith("default")
                                                              && ImageLibrary.GetObjType(obj) == ImageLibrary.GetObjType(obj2)));
        }

        /// <summary>
        /// Sorts the list of object names according to their object types.
        /// </summary>
        private void SortObjectNamesByObjectType()
        {
            ObjectKeys.Sort((obj1, obj2) => ImageLibrary.GetObjType(obj1).CompareTo(ImageLibrary.GetObjType(obj2)));
        }
    }
}
