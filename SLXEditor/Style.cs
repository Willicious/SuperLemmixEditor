using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;

namespace SLXEditor
{
    /// <summary>
    /// Stores all data of a graphics style, except the images themselves.
    /// </summary>
    public class Style
    {
        /// <summary>
        /// Initializes a new instance of a Style by searching for pieces in the directory AppPath/StyleName/.
        /// </summary>
        /// <param name="styleName"></param>
        public Style(string styleName, BackgroundList backgroundList, bool randomize)
        {
            NameInDirectory = styleName;
            NameInEditor = styleName; // may be overwritten later when forming the StyleList
            Randomize = randomize;

            colorDict = LoadStylesFromFile.StyleColors(NameInDirectory);
        }

        Dictionary<C.StyleColor, Color> colorDict;

        List<string> terrainKeys;
        List<string> steelKeys;
        List<string> objectKeys;
        List<string> backgroundKeys;

        public string NameInDirectory { get; private set; }
        public string NameInEditor { get; set; }
        public bool Randomize { get; set; }

        public List<string> TerrainKeys
        {
            get
            {
                if (terrainKeys == null)
                    LoadTerrainAndObjects();
                return terrainKeys;
            }
        }

        public List<string> SteelKeys
        {
            get
            {
                if (steelKeys == null)
                    LoadTerrainAndObjects();
                return steelKeys;
            }
        }

        public List<string> ObjectKeys
        {
            get
            {
                if (objectKeys == null)
                    LoadTerrainAndObjects();
                return objectKeys;
            }
        }

        public List<string> BackgroundKeys
        {
            get
            {
                if (backgroundKeys == null)
                    LoadTerrainAndObjects();
                return backgroundKeys;
            }
        }

        /// <summary>
        /// Checks for equality of the style's FileName.
        /// </summary>
        /// <param name="otherStyle"></param>
        public bool Equals(Style otherStyle)
        {
            return this.NameInDirectory.Equals(otherStyle?.NameInDirectory);
        }

        /// <summary>
        /// Searches the style's directory for terrain and object pieces and sorts them.
        /// </summary>
        private void LoadTerrainAndObjects()
        {
            SearchDirectoryForTerrain();
            SearchDirectoryForSteel();
            SearchDirectoryForObjects();
            SearchDirectoryForBackgrounds();

            RemoveDuplicatedObjects();
            SortObjectNamesByObjectType();
        }


        /// <summary>
        /// Reads the style's color or a default value if no color is specified.
        /// </summary>
        /// <param name="colorType"></param>
        public Color GetColor(C.StyleColor colorType)
        {
            if (colorDict.ContainsKey(colorType))
                return colorDict[colorType];
            else
                return C.SLXColors[(colorType == C.StyleColor.BACKGROUND) ? C.SLXColor.BackDefault : C.SLXColor.OWWDefault];
        }

        /// <summary>
        /// Writes all pieces in AppPath/StyleName/backgrounds to the list of BackgroundNames.
        /// </summary>
        private void SearchDirectoryForBackgrounds()
        {
            string directoryPath = C.AppPathPieces + NameInDirectory + C.DirSep + "backgrounds";

            if (Directory.Exists(directoryPath))
            {
                backgroundKeys = Directory.GetFiles(directoryPath, "*.png", SearchOption.TopDirectoryOnly)
                                       .Select(file => ImageLibrary.CreatePieceKey(file))
                                       .ToList();
            }
            else // use empty list
            {
                backgroundKeys = new List<string>();
            }
        }

        /// <summary>
        /// Writes all pieces in AppPath/StyleName/terrain to the list of TerrainNames.
        /// </summary>
        private void SearchDirectoryForTerrain()
        {
            string directoryPath = C.AppPathPieces + NameInDirectory + C.DirSep + "terrain";

            if (Directory.Exists(directoryPath))
            {
                terrainKeys = Directory.GetFiles(directoryPath, "*.png", SearchOption.TopDirectoryOnly)
                                       .Select(file => ImageLibrary.CreatePieceKey(file))
                                       .Where(key => ImageLibrary.GetObjType(key) != C.OBJ.STEEL) // Filter out steel pieces
                                       .ToList();
            }
            else
            {
                terrainKeys = new List<string>();
            }
        }

        /// <summary>
        /// Writes all steel pieces in AppPath/StyleName/terrain to the list of SteelNames.
        /// </summary>
        private void SearchDirectoryForSteel()
        {
            // Load first the style-specific objects
            string directoryPath = C.AppPathPieces + NameInDirectory + C.DirSep + "terrain";

            if (Directory.Exists(directoryPath))
            {
                steelKeys = Directory.GetFiles(directoryPath, "*.png", SearchOption.TopDirectoryOnly)
                                     .Select(file => ImageLibrary.CreatePieceKey(file))
                                     .Where(key => ImageLibrary.GetObjType(key) == C.OBJ.STEEL) // Only include steel pieces
                                     .ToList();
            }
            else
            {
                steelKeys = new List<string>();
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
                objectKeys = Directory.GetFiles(directoryPath, "*.nxmo", SearchOption.TopDirectoryOnly)
                                       .Select(file => ImageLibrary.CreatePieceKey(Path.GetFullPath(file)))
                                       .ToList();
            }
            else
            {
                objectKeys = new List<string>();
            }

            // Load now the default objects into the list
            string directoryPathDefault = C.AppPathPieces + "default" + C.DirSep + "objects";

            if (Directory.Exists(directoryPathDefault))
            {
                try
                {
                    ObjectKeys.AddRange(Directory.GetFiles(directoryPathDefault, "*.nxmo", SearchOption.TopDirectoryOnly)
                                                  .Where(file => file.ToLowerInvariant().Substring(file.Length - 13) != "fallback.nxmo")
                                                  .Select(file => ImageLibrary.CreatePieceKey(Path.GetFullPath(file)))
                                                  .ToList());

                    ObjectKeys.Add("default" + C.DirSep + "objects" + C.DirSep + "lemming");
                }
                catch (Exception Ex)
                {
                    Utility.LogException(Ex);

                    System.Windows.Forms.MessageBox.Show("Warning:" + Ex.Message, "Files not found");
                    // ...but then start the editor as usual
                }
            }
        }

        /// <summary>
        /// Removes all default objects, that are already present in the actual style.
        /// </summary>
        private void RemoveDuplicatedObjects()
        {
            ObjectKeys.RemoveAll(obj => obj.StartsWith("default")
                                      && !ImageLibrary.GetObjType(obj).In(C.OBJ.NONE, C.OBJ.DECORATION)
                                      && ObjectKeys.Exists(obj2 => !obj2.StartsWith("default")
                                                              && ImageLibrary.GetObjType(obj) == ImageLibrary.GetObjType(obj2)));
        }

        /// <summary>
        /// Sorts the list of object names according to their object types.
        /// </summary>
        private void SortObjectNamesByObjectType()
        {
            ObjectKeys.Sort(delegate (string obj1, string obj2)
            {
                if (ImageLibrary.GetObjType(obj1) != ImageLibrary.GetObjType(obj2))
                {
                    return ImageLibrary.GetObjType(obj1).CompareTo(ImageLibrary.GetObjType(obj2));
                }
                else
                {
                    return Path.GetFileName(obj1).CompareTo(Path.GetFileName(obj2));
                }
            });
        }
    }
}
