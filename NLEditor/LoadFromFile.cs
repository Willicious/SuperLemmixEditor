using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.IO;

namespace NLEditor
{
    static class LoadFromFile
    {
        public static string CreatePieceKey(string FilePath)
        {
            return Path.GetFullPath(FilePath).Remove(0, C.AppPathPieces.Length);
        }

        public static string CreatePieceKey(string StyleName, string PieceName, bool IsObject)
        {
            return StyleName + C.DirSep + (IsObject ? "objects" : "terrain") 
                             + C.DirSep + PieceName + ".png";
        }

        public static List<Color> StyleColors(string StyleName)
        {
            string FilePath = C.AppPath + "styles" + C.DirSep + "themes" + C.DirSep;
            
            List<Color> ColorList = new List<Color>();
            // Write default colors in it
            ColorList.Add(Color.Black);

            // return default list if no further infos exist
            if (!File.Exists(FilePath)) return ColorList;

            // otherwise read the file
            try
            {
                using (StreamReader Stream = new StreamReader(FilePath))
                {
                    string Line;
                    while ((Line = Stream.ReadLine()) != null)
                    {
                        if (Line.Trim().Substring(0, 10).ToUpper() == "BACKGROUND")
                        {
                            ColorList[0] = ColorTranslator.FromHtml("#" + Line.Trim().Substring(10).Trim());
                        }
                    }
                }
            }
            finally
            {
                // do nothing
            }

            return ColorList;
        }

        public static List<string> OrderStyleNames(List<string> StyleNames)
        {
            string FilePath = C.AppPath + "styles" + C.DirSep + "styles.ini";

            if (!File.Exists(FilePath)) return StyleNames;
            
            // Otherwise order the styles according to styles.ini
            List<string> NewStyleNames = new List<string>();

            // TODO
            NewStyleNames = StyleNames;

            return NewStyleNames;
        }


    }
}
