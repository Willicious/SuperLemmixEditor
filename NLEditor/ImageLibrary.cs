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
            Bitmap NewBitmap = LoadFromFile.Image(ImageName);

            BaseImageInfo NewImageInfo = LoadFromFile.ImageInfo(NewBitmap, ImageName);

            // Add the image
            fImageList.Add(ImageName, NewImageInfo);

            return true;
        }
    }
}
