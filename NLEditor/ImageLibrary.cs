using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.IO;

namespace NLEditor
{
    public class BaseImageInfo
    {
        public BaseImageInfo(Bitmap NewImage, bool IsSteel = false)
        {
            this.Image = NewImage;
            this.Width = this.Image.Width;
            this.Height = this.Image.Height;
            this.TriggerRect = new Rectangle(0, 0, 0, 0);
            if (IsSteel)
            {
                this.ObjectType = C.OBJ_SPLAT;
            }
            else 
            {
                this.ObjectType = C.OBJ_NULL;
            }
        }

        public BaseImageInfo(Bitmap NewImage, int ObjType, int NumFrames, bool IsVert, Rectangle TriggerRect)
        {
            this.Image = SeparateFrames(NewImage, NumFrames, IsVert);
            this.Width = this.Image.Width;
            this.Height = this.Image.Height;
            this.ObjectType = ObjType;
            this.TriggerRect = TriggerRect;
        }

        public Bitmap Image;
        public int Width;
        public int Height;
        public int ObjectType;
        public Rectangle TriggerRect;


        private Bitmap SeparateFrames(Bitmap NewBitmap, int NumFrames, bool IsVert)
        {
            int NewWidth = NewBitmap.Width;
            int NewHeight = NewBitmap.Height;
            
            if (IsVert)
            {
                NewHeight = NewHeight / NumFrames;
            }
            else
            {
                NewWidth = NewWidth / NumFrames;
            }

            Rectangle ThisRect = new Rectangle(0, 0, NewWidth, NewHeight);
            return NewBitmap.Crop(ThisRect);
        }
        
    }
    
    
    public class ImageLibrary
    {
        /*---------------------------------------------------------
         *  This class contains and modifies a list of all images
         *  It load new ones when required.
         * -------------------------------------------------------- */

        /* --------------------------------------------------------
         *   public methods:
         *     - ImageLibrary() // constructor
         *     - GetImage(string ImageName)
         *     - GetWidth(string ImageName)
         *     - GetHeight(string ImageName)
         *     - GetObjType(string ImageName)
         * -------------------------------------------------------- */

        public ImageLibrary()
        {
            fImageList = new Dictionary<string, BaseImageInfo>();
            fImagePathList = GetAllImagePaths();
        }

        private Dictionary<string, BaseImageInfo> fImageList;
        // The key is the file path below the "styles\\themes\\pieces" folder!
        private ILookup<string, string> fImagePathList; 


        private ILookup<string, string> GetAllImagePaths()
        {
            return Directory.GetFiles(C.AppPathPieces, "*.png", SearchOption.AllDirectories)
                            .ToLookup(file => LoadFromFile.CreatePieceKey(file),
                                      file => Path.GetDirectoryName(file));
        }


        public Bitmap GetImage(string ImageName)
        {
            if (!fImageList.ContainsKey(ImageName))
            {
                bool Success = AddNewImage(ImageName);
                if (!Success) return null;
            }

            return fImageList[ImageName].Image;
        }

        public int GetWidth(string ImageName)
        {
            if (!fImageList.ContainsKey(ImageName))
            {
                bool Success = AddNewImage(ImageName);
                if (!Success) return -1;
            }

            return fImageList[ImageName].Width;
        }

        public int GetHeight(string ImageName)
        {
            if (!fImageList.ContainsKey(ImageName))
            {
                bool Success = AddNewImage(ImageName);
                if (!Success) return -1;
            }

            return fImageList[ImageName].Height;
        }

        public int GetObjType(string ImageName)
        {
            if (!fImageList.ContainsKey(ImageName))
            {
                bool Success = AddNewImage(ImageName);
                if (!Success) return -1;
            }

            return fImageList[ImageName].ObjectType;
        }



        private bool AddNewImage(string ImageName)
        {
            // Find directory with this file
            string DirPath = fImagePathList[ImageName + ".png"].FirstOrDefault();

            // Check whether the image exists
            if (DirPath == null) return false;

            // Load new image
            Bitmap NewBitmap = LoadImageFromFile(DirPath + "\\" + ImageName + ".png");

            BaseImageInfo NewImageInfo;

            if (File.Exists(DirPath + "\\" + ImageName + ".nxop"))
            {
                // create a new object piece
                NewImageInfo = CreateNewObjectInfo(NewBitmap, DirPath + "\\" + ImageName + ".nxop");
            }
            else if (File.Exists(DirPath + "\\" + ImageName + ".nxtp"))
            {
                // create a new object piece
                NewImageInfo = CreateNewTerrainInfo(NewBitmap, DirPath + "\\" + ImageName + ".nxop");
            }
            else
            {
                // create a new terrain piece
                NewImageInfo = new BaseImageInfo(NewBitmap);
            }

            // Add the image
            fImageList.Add(ImageName, NewImageInfo);

            return true;
        }

        private BaseImageInfo CreateNewObjectInfo(Bitmap NewBitmap, string FilePathInfo)
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
                        if (Line.Substring(0, 6).ToUpper() == "FRAMES")
                        {
                            NumFrames = Int32.Parse(Line.Substring(6).Trim());
                        }
                        else if (Line.Substring(0, 7).ToUpper() == "TRIGGER") 
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
                        else if (Line.ToUpper().Trim() == "EXIT")
                        {
                            ObjType = C.OBJ_EXIT;
                        }
                        else if (Line.ToUpper().Trim() == "TRAP")
                        {
                            ObjType = C.OBJ_TRAP;
                        }
                       
                    }
                }
            }
            catch (Exception Ex)
            {
                String ErrorPath = C.AppPath + "ErrorLog.txt";
                TextWriter TextFile = new StreamWriter(ErrorPath, true);
                TextFile.WriteLine(Ex.ToString());
                TextFile.Close();
            }

            return new BaseImageInfo(NewBitmap, ObjType, NumFrames, IsVert, TriggerRect);
        }

        private BaseImageInfo CreateNewTerrainInfo(Bitmap NewBitmap, string FilePathInfo)
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
                String ErrorPath = C.AppPath + "ErrorLog.txt";
                TextWriter TextFile = new StreamWriter(ErrorPath, true);
                TextFile.WriteLine(Ex.ToString());
                TextFile.Close();
            }

            return new BaseImageInfo(NewBitmap, IsSteel);
        }


        private Bitmap LoadImageFromFile(string FilePath)
        {
            try
            {
                return new Bitmap(FilePath);
            }
            catch (Exception Ex)
            {
                String ErrorPath = C.AppPath + "ErrorLog.txt";
                TextWriter TextFile = new StreamWriter(ErrorPath, true);
                TextFile.WriteLine(Ex.ToString());
                TextFile.Close();

                // return empty image
                return new Bitmap(1, 1);
            }
        }

        private void ReadMetaInfoFromFile(string FilePath, out int NumFrames, out bool IsHorizontal, out Point Offset, out int Type)
        {
            NumFrames = 1;
            IsHorizontal = true;
            Type = 0;
            Offset = new Point(0, 0);

            try
            {
                using (StreamReader Stream = new StreamReader(FilePath))
                {
                    string Line;
                    while ((Line = Stream.ReadLine()) != null)
                    {
                        switch (Line.Substring(0, 3).ToUpper())
                        {
                            case "FRA": NumFrames = Int32.Parse(Line.Substring(3)); break;
                            case "HOR": IsHorizontal = Line.Substring(3).ToUpper().Contains('H'); break;
                            case "OFF": Offset = new Point(Int32.Parse(Line.Substring(3, 3)), 
                                                           Int32.Parse(Line.Substring(6, 3))); break;
                        }
                    }
                }
            }
            catch (Exception Ex)
            {
                String ErrorPath = C.AppPath + "ErrorLog.txt";
                TextWriter TextFile = new StreamWriter(ErrorPath, true);
                TextFile.WriteLine(Ex.ToString());
                TextFile.Close();
            }
        }
    }
}
