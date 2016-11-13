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
    static class LoadFromFile
    {
        public static List<Color> StyleColors(string StyleName)
        {
            string FilePath = C.AppPath + "styles" + C.DirSep + "themes" + C.DirSep + StyleName + ".nxtm";
            
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
                        if (Line.Length > 10 && Line.Trim().Substring(0, 10).ToUpper() == "BACKGROUND")
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
            // --------------------------------------------------------!!!
            NewStyleNames = StyleNames;

            return NewStyleNames;
        }

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

        private static BaseImageInfo CreateNewObjectInfo(Bitmap NewBitmap, string FilePathInfo)
        {
            int NumFrames = 1;
            bool IsVert = false;
            int ObjType = C.OBJ_NONE;
            Rectangle TriggerRect = new Rectangle(0, 0, 0, 0);

            try
            {
                using (StreamReader Stream = new StreamReader(FilePathInfo))
                {
                    string Line;
                    while ((Line = Stream.ReadLine()) != null)
                    {
                        if (Line.Length > 6 && Line.Substring(0, 6).ToUpper() == "FRAMES")
                        {
                            NumFrames = Int32.Parse(Line.Substring(6).Trim());
                        }
                        else if (Line.Length > 9 && Line.Substring(0, 7).ToUpper() == "TRIGGER")
                        {
                            int TrigNum = Int32.Parse(Line.Substring(9).Trim());
                            switch (Line.Substring(8, 1).ToUpper())
                            {
                                case "X": TriggerRect.X = TrigNum; break;
                                case "Y": TriggerRect.Y = TrigNum; break;
                                case "W": TriggerRect.Width = TrigNum; break;
                                case "H": TriggerRect.Height = TrigNum; break;
                            }
                        }
                        else if (Line.ToUpper().Trim() == "VERTICAL")
                        {
                            IsVert = true;
                        }
                        else if (Line.ToUpper().Trim() == "HORIZONTAL")
                        {
                            IsVert = false;
                        }
                        else if (Line.ToUpper().Trim() == "WINDOW")
                        {
                            ObjType = C.OBJ_HATCH;
                        }
                        else if (Line.ToUpper().Trim() == "EXIT")
                        {
                            ObjType = C.OBJ_EXIT;
                        }
                        else if (Line.ToUpper().Trim() == "TRAP")
                        {
                            ObjType = C.OBJ_TRAP;
                        }
                        else if (Line.ToUpper().Trim() == "WATER")
                        {
                            ObjType = C.OBJ_WATER;
                        }
                        else if (Line.ToUpper().Trim() == "FIRE")
                        {
                            ObjType = C.OBJ_FIRE;
                        }
                        else if (Line.ToUpper().Trim() == "OWR_ARROW")
                        {
                            ObjType = C.OBJ_OWW_LEFT;
                        }
                        else if (Line.ToUpper().Trim() == "OWL_ARROW")
                        {
                            ObjType = C.OBJ_OWW_LEFT;
                        }
                        else if (Line.ToUpper().Trim() == "OWD_ARROW")
                        {
                            ObjType = C.OBJ_OWW_DOWN;
                        }
                        else if (Line.ToUpper().Trim() == "BUTTON")
                        {
                            ObjType = C.OBJ_BUTTON;
                        }
                        else if (Line.ToUpper().Trim() == "LOCKED_EXIT")
                        {
                            ObjType = C.OBJ_EXIT_LOCKED;
                        }
                        else if (Line.ToUpper().Trim() == "PICKUP")
                        {
                            ObjType = C.OBJ_PICKUP;
                        }
                        else if (Line.ToUpper().Trim() == "TELEPORTER")
                        {
                            ObjType = C.OBJ_TELEPORTER;
                        }
                        else if (Line.ToUpper().Trim() == "RECEIVER")
                        {
                            ObjType = C.OBJ_RECEIVER;
                        }
                        else if (Line.ToUpper().Trim() == "SPLITTER")
                        {
                            ObjType = C.OBJ_SPLITTER;
                        }
                        else if (Line.ToUpper().Trim() == "RADIATION")
                        {
                            ObjType = C.OBJ_RADIATION;
                        }
                        else if (Line.ToUpper().Trim() == "SLOWFREEZE")
                        {
                            ObjType = C.OBJ_SLOWFREEZE;
                        }
                        else if (Line.ToUpper().Trim() == "UPDRAFT")
                        {
                            ObjType = C.OBJ_UPDRAFT;
                        }
                        else if (Line.ToUpper().Trim() == "SPLAT")
                        {
                            ObjType = C.OBJ_SPLAT;
                        }
                        else if (Line.ToUpper().Trim() == "ANTISPLAT")
                        {
                            ObjType = C.OBJ_NOSPLAT;
                        }
                        else if (Line.ToUpper().Trim() == "OWR_FIELD")
                        {
                            ObjType = C.OBJ_FORCE_RIGHT;
                        }
                        else if (Line.ToUpper().Trim() == "OWL_FIELD")
                        {
                            ObjType = C.OBJ_FORCE_LEFT;
                        }
                        else if (Line.ToUpper().Trim() == "BACKGROUND")
                        {
                            ObjType = C.OBJ_BACKGROUND;
                        }

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

        private static BaseImageInfo CreateNewTerrainInfo(Bitmap NewBitmap, string FilePathInfo)
        {
            bool IsSteel = false;

            try
            {
                using (StreamReader Stream = new StreamReader(FilePathInfo))
                {
                    string Line;
                    while ((Line = Stream.ReadLine()) != null)
                    {
                        if (Line.ToUpper().Trim() == "STEEL")
                        {
                            IsSteel = true;
                        }
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
