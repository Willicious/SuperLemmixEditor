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
            this.fImage = NewImage;
            this.fWidth = this.Image.Width;
            this.fHeight = this.Image.Height;
            this.fTriggerRect = new Rectangle(0, 0, 0, 0);
            if (IsSteel)
            {
                this.fObjectType = C.OBJ.STEEL;
            }
            else 
            {
                this.fObjectType = C.OBJ.NULL;
            }
        }

        public BaseImageInfo(Bitmap NewImage, C.OBJ ObjType, int NumFrames, bool IsVert, Rectangle TriggerRect)
        {
            this.fImage = SeparateFrames(NewImage, NumFrames, IsVert);
            this.fWidth = this.fImage.Width;
            this.fHeight = this.fImage.Height;
            this.fObjectType = ObjType;
            this.fTriggerRect = TriggerRect;
        }

        readonly Bitmap fImage;
        readonly int fWidth;
        readonly int fHeight;
        readonly C.OBJ fObjectType;
        readonly Rectangle fTriggerRect;

        public Bitmap Image { get { return (Bitmap)fImage.Clone(); } }
        public int Width { get { return fWidth; } }
        public int Height { get { return fHeight; } }
        public C.OBJ ObjectType { get { return fObjectType; } }
        public Rectangle TriggerRect { get { return fTriggerRect; } }

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
                if (!Success)
                {
                    System.Windows.Forms.MessageBox.Show("Cannot find image " + ImageKey + ".");
                    return null;
                }
            }

            // return (Bitmap)fImageList[ImageKey].Image.Clone();
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

        public static C.OBJ GetObjType(string ImageKey)
        {
            if (!fImageList.ContainsKey(ImageKey))
            {
                bool Success = AddNewImage(ImageKey);
                if (!Success) return C.OBJ.NULL;
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
