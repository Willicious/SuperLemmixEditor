using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace NLEditor
{
    /// <summary>
    /// Contains static methods to load styles and meta-infos of objects.
    /// </summary>
    static class LoadStylesFromFile
    {
        public static void AddInitialImagesToLibrary()
        { 
            // preplaced lemming
            string ImageKey = "default" + C.DirSep + "objects" + C.DirSep + "lemming";
            Bitmap Image = Properties.Resources.Lemming;
            ImageLibrary.AddNewImage(ImageKey, Image, C.OBJ.LEMMING, new Rectangle(2, 9, 1, 1));
        }
        
        
        /// <summary>
        /// Reads style colors from a .nxtm file.
        /// <para> Color 0: Background (default: black) </para>
        /// </summary>
        /// <param name="StyleName"></param>
        /// <returns></returns>
        public static List<Color> StyleColors(string StyleName)
        {
            string FilePath = C.AppPath + "styles" + C.DirSep + "themes" + C.DirSep + StyleName + ".nxtm";
            
            List<Color> ColorList = new List<Color>();
            // Write default colors in it
            ColorList.Add(Color.Black);

            // return default list if no further infos exist
            if (!File.Exists(FilePath)) return ColorList;

            FileParser MyParser;
            try
            {
                MyParser = new FileParser(FilePath);
            }
            catch (Exception Ex)
            {
                Utility.LogException(Ex);
                MessageBox.Show(Ex.Message);
                return ColorList;
            }

            try
            {
                List<FileLine> NewFileLine;
                while ((NewFileLine = MyParser.GetNextLines()) != null)
                {
                    System.Diagnostics.Debug.Assert(NewFileLine.Count > 0, "FileParser returned empty list.");
                    if (NewFileLine[0].Key == "BACKGROUND")
                    {
                        ColorList[0] = ColorTranslator.FromHtml("#" + NewFileLine[0].Text);
                    }
                }
            }
            catch
            {
                // do nothing
            }

            return ColorList;
        }

        /// <summary>
        /// Reads the styles.ini file and orders styles accordingly.
        /// <para> UNFINISHED: CURRENTLY DOES NOTHING! </para>
        /// </summary>
        /// <param name="StyleNames"></param>
        /// <returns></returns>
        public static List<string> OrderStyleNames(List<string> StyleNames)
        {
            string FilePath = C.AppPath + "styles" + C.DirSep + "styles.ini";

            if (!File.Exists(FilePath)) return StyleNames;
            
            // Otherwise order the styles according to styles.ini
            List<string> NewStyleNames = new List<string>();

            // TODO
            // --------------------------------------------------------!!!
            NewStyleNames = StyleNames;

            return NewStyleNames;
        }

        /// <summary>
        /// Loads a .png image.
        /// <para> Returns null on exception. </para>
        /// </summary>
        /// <param name="ImageKey"></param>
        /// <returns></returns>
        public static Bitmap Image(string ImageKey)
        {
            string ImagePath = C.AppPathPieces + ImageKey;
            
            try
            {
                return new Bitmap(ImagePath + ".png");
            }
            catch (Exception Ex)
            {
                Utility.LogException(Ex);
                return null;
            }
        }

        /// <summary>
        /// Reads further piece infos from a .nxob resp. nxtp file.
        /// <para> Returns a finished BaseImageInfo containing both the image and the further info. <\para> 
        /// </summary>
        /// <param name="Image"></param>
        /// <param name="ImageName"></param>
        /// <returns></returns>
        public static BaseImageInfo ImageInfo(Bitmap Image, string ImageName)
        {
            string ImagePath = C.AppPathPieces + ImageName;

            if (File.Exists(ImagePath + ".nxob"))
            {
                // create a new object piece
                return CreateNewObjectInfo(Image, ImagePath + ".nxob");
            }
            else if (File.Exists(ImagePath + ".nxtp"))
            {
                // create a new object piece
                return CreateNewTerrainInfo(Image, ImagePath + ".nxtp");
            }
            else
            {
                // create a new terrain piece
                return new BaseImageInfo(Image);
            }
        }

        /// <summary>
        /// Reads further object infos from a .nxob file. Default values:
        /// <para> NumFrames = 1 </para>
        /// <para> ObjType = C.OBJ.NONE </para>
        /// <para> TriggerRect = Rectangle(0, 0, 0, 0) </para>
        /// </summary>
        /// <param name="NewBitmap"></param>
        /// <param name="FilePathInfo"></param>
        /// <returns></returns>
        private static BaseImageInfo CreateNewObjectInfo(Bitmap NewBitmap, string FilePath)
        {
            int NumFrames = 1;
            bool IsVert = false;
            C.OBJ ObjType = C.OBJ.NONE;
            Rectangle TriggerRect = new Rectangle(0, 0, 0, 0);

            FileParser MyParser;
            try
            {
                MyParser = new FileParser(FilePath);
            }
            catch (Exception Ex)
            {
                Utility.LogException(Ex);
                MessageBox.Show(Ex.Message);
                return new BaseImageInfo(NewBitmap, ObjType, NumFrames, IsVert, TriggerRect);
            }

            try
            {
                List<FileLine> FileLineList;
                while ((FileLineList = MyParser.GetNextLines()) != null)
                {
                    System.Diagnostics.Debug.Assert(FileLineList.Count > 0, "FileParser returned empty list.");
                    
                    FileLine Line = FileLineList[0];
                    switch (Line.Key)
                    {
                        case "FRAMES": NumFrames = Line.Value; break;
                        case "TRIGGER_X": TriggerRect.X = Line.Value; break;
                        case "TRIGGER_Y": TriggerRect.Y = Line.Value; break;
                        case "TRIGGER_W": TriggerRect.Width = Line.Value; break;
                        case "TRIGGER_H": TriggerRect.Height = Line.Value; break;
                        case "VERTICAL": IsVert = true; break;
                        case "HORIZONTAL": IsVert = false; break;
                        case "WINDOW": ObjType = C.OBJ.HATCH; break;
                        case "EXIT": ObjType = C.OBJ.EXIT; break;
                        case "TRAP": ObjType = C.OBJ.TRAP; break;
                        case "WATER": ObjType = C.OBJ.WATER; break;
                        case "FIRE": ObjType = C.OBJ.FIRE; break;
                        case "OWR_ARROW": ObjType = C.OBJ.OWW_RIGHT; break;
                        case "OWL_ARROW": ObjType = C.OBJ.OWW_LEFT; break;
                        case "OWD_ARROW": ObjType = C.OBJ.OWW_DOWN; break;
                        case "BUTTON": ObjType =  C.OBJ.BUTTON; break;
                        case "LOCKED_EXIT": ObjType = C.OBJ.EXIT_LOCKED; break;
                        case "PICKUP": ObjType = C.OBJ.PICKUP; break;
                        case "TELEPORTER": ObjType = C.OBJ.TELEPORTER; break;
                        case "RECEIVER": ObjType = C.OBJ.RECEIVER; break;
                        case "SPLITTER": ObjType = C.OBJ.SPLITTER; break;
                        case "RADIATION": ObjType = C.OBJ.RADIATION; break;
                        case "SLOWFREEZE": ObjType = C.OBJ.SLOWFREEZE; break;
                        case "UPDRAFT": ObjType = C.OBJ.UPDRAFT; break;
                        case "SPLAT": ObjType = C.OBJ.SPLAT; break;
                        case "ANTISPLAT": ObjType = C.OBJ.NOSPLAT; break;
                        case "OWR_FIELD": ObjType = C.OBJ.FORCE_RIGHT; break;
                        case "OWL_FIELD": ObjType = C.OBJ.FORCE_LEFT; break;
                        case "BACKGROUND": ObjType = C.OBJ.BACKGROUND; break;
                    }
                }
            }
            catch (Exception Ex)
            {
                Utility.LogException(Ex);
                MessageBox.Show(Ex.Message);
            }

            return new BaseImageInfo(NewBitmap, ObjType, NumFrames, IsVert, TriggerRect);
        }

        /// <summary>
        /// Reads further terrain infos from a .nxtp file. Default values:
        /// <para> IsSteel = false </para>
        /// </summary>
        /// <param name="NewBitmap"></param>
        /// <param name="FilePathInfo"></param>
        /// <returns></returns>
        private static BaseImageInfo CreateNewTerrainInfo(Bitmap NewBitmap, string FilePath)
        {
            bool IsSteel = false;

            FileParser MyParser;
            try
            {
                MyParser = new FileParser(FilePath);
            }
            catch (Exception Ex)
            {
                Utility.LogException(Ex);
                MessageBox.Show(Ex.Message);
                return new BaseImageInfo(NewBitmap, IsSteel);
            }

            try
            {
                List<FileLine> FileLineList;
                while ((FileLineList = MyParser.GetNextLines()) != null)
                {
                    System.Diagnostics.Debug.Assert(FileLineList.Count > 0, "FileParser returned empty list.");

                    FileLine Line = FileLineList[0];
                    switch (Line.Key)
                    {
                        case "STEEL": IsSteel = true; break;
                    }
                }
            }
            catch (Exception Ex)
            {
                Utility.LogException(Ex);
                MessageBox.Show(Ex.Message);
            }

            return new BaseImageInfo(NewBitmap, IsSteel);
        }
    }
}
