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
    
    
    static class ImageLibrary
    {
        /*---------------------------------------------------------
         *  This class contains and modifies a list of all images
         *  It load new ones when required.
         * -------------------------------------------------------- */

        /* --------------------------------------------------------
         *   public methods:
         *     - GetImage(string ImageKey)
         *     - GetWidth(string ImageKey)
         *     - GetHeight(string ImageKey)
         *     - GetObjType(string ImageKey)
         *     - GetTrigger(string ImageKey)
         * -------------------------------------------------------- */

        static ImageLibrary()
        {
            fImageList = new Dictionary<string, BaseImageInfo>();
        }

        // The key is the file path below the "styles\\themes\\pieces" folder!
        static Dictionary<string, BaseImageInfo> fImageList;

        public static Bitmap GetImage(string ImageKey)
        {
            if (!fImageList.ContainsKey(ImageKey))
            {
                bool Success = AddNewImage(ImageKey);
                if (!Success) return null;
            }

            return fImageList[ImageKey].Image;
        }

        public static int GetWidth(string ImageKey)
        {
            if (!fImageList.ContainsKey(ImageKey))
            {
                bool Success = AddNewImage(ImageKey);
                if (!Success) return -1;
            }

            return fImageList[ImageKey].Width;
        }

        public static int GetHeight(string ImageKey)
        {
            if (!fImageList.ContainsKey(ImageKey))
            {
                bool Success = AddNewImage(ImageKey);
                if (!Success) return -1;
            }

            return fImageList[ImageKey].Height;
        }

        public static int GetObjType(string ImageKey)
        {
            if (!fImageList.ContainsKey(ImageKey))
            {
                bool Success = AddNewImage(ImageKey);
                if (!Success) return -1;
            }

            return fImageList[ImageKey].ObjectType;
        }

        public static Rectangle GetTrigger(string ImageKey)
        {
            if (!fImageList.ContainsKey(ImageKey))
            {
                bool Success = AddNewImage(ImageKey);
                if (!Success) return new Rectangle(0, 0, 0, 0);
            }

            return fImageList[ImageKey].TriggerRect;
        }


        static bool AddNewImage(string ImageKey)
        {
            // Load new image
            Bitmap NewBitmap = LoadFromFile.Image(ImageKey);

            // Check whether this Bitmap exists 
            if (NewBitmap == null) return false;

            BaseImageInfo NewImageInfo = LoadFromFile.ImageInfo(NewBitmap, ImageKey);

            // Add the image
            fImageList.Add(ImageKey, NewImageInfo);

            return true;
        }

        public static string CreatePieceKey(string FilePath)
        {
            string FullPath = Path.GetFullPath(FilePath);
            string RelativePath = FullPath.Remove(0, C.AppPathPieces.Length);
            return Path.ChangeExtension(RelativePath, null);
        }

        public static string CreatePieceKey(string StyleName, string PieceName, bool IsObject)
        {
            return StyleName + C.DirSep + (IsObject ? "objects" : "terrain")
                             + C.DirSep + PieceName;
        }
    }
}
