using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace NLEditor
{
    /// <summary>
    /// Stores the common unchangable data of pieces.
    /// </summary>
    public class BaseImageInfo
    {
        /// <summary>
        /// Use this to create the base-info of a new terrain piece.
        /// </summary>
        /// <param name="NewImage"></param>
        /// <param name="IsSteel"></param>
        public BaseImageInfo(Bitmap NewImage, bool IsSteel = false)
            : this(NewImage, IsSteel ? C.OBJ.STEEL : C.OBJ.NULL, 1, false, new Rectangle(0, 0, 0, 0))
        {
            // nothing more
        }

        /// <summary>
        /// Use this to create the base-info of a new object piece.
        /// </summary>
        /// <param name="NewImage"></param>
        /// <param name="ObjType"></param>
        /// <param name="NumFrames"></param>
        /// <param name="IsVert"></param>
        /// <param name="TriggerRect"></param>
        public BaseImageInfo(Bitmap NewImage, C.OBJ ObjType, int NumFrames, bool IsVert, Rectangle TriggerRect)
        {
            this.fImage = SeparateFrames(NewImage, NumFrames, IsVert);
            this.fWidth = this.fImage.Width;
            this.fHeight = this.fImage.Height;
            this.fObjectType = ObjType;
            this.fTriggerRect = TriggerRect;

            RotateFlipType[] RotFlipTypeArray = (RotateFlipType[])Enum.GetValues(typeof(RotateFlipType));
            this.fImageRotated = new Dictionary<RotateFlipType, Bitmap>();
            foreach (RotateFlipType RotFlipType in RotFlipTypeArray.Distinct())
            {
                Bitmap RotImage = (Bitmap)fImage.Clone();
                RotImage.RotateFlip(RotFlipType);
                this.fImageRotated.Add(RotFlipType, RotImage);
            }
        }

        readonly Bitmap fImage;
        readonly int fWidth;
        readonly int fHeight;
        readonly C.OBJ fObjectType;
        readonly Rectangle fTriggerRect;

        readonly Dictionary<RotateFlipType, Bitmap> fImageRotated;

        [Obsolete]
        public Bitmap Image()
        {
            return (Bitmap)fImage.Clone();
        }
        public Bitmap Image(RotateFlipType RotFlipType)
        {
            return fImageRotated[RotFlipType];
        }
        public int Width { get { return fWidth; } }
        public int Height { get { return fHeight; } }
        public C.OBJ ObjectType { get { return fObjectType; } }
        public Rectangle TriggerRect { get { return fTriggerRect; } }

        /// <summary>
        /// Removes additional frames from an image and keeps only frame 0.
        /// </summary>
        /// <param name="NewBitmap"></param>
        /// <param name="NumFrames"></param>
        /// <param name="IsVert"></param>
        /// <returns></returns>
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
    
    /// <summary>
    /// Provides images and associated data of pieces.
    /// <para> It loads them upon first usage of said piece. </para>
    /// </summary>
    static class ImageLibrary
    {
        /*---------------------------------------------------------
         *  This class contains and modifies a list of all images
         *  It load new ones when required.
         * -------------------------------------------------------- */

        /* --------------------------------------------------------
         *   public methods:
         *     - GetImage(string ImageKey) // [Obsolete]
         *     - GetImage(string ImageKey, RotateFlipType RotFlipType)
         *     - GetWidth(string ImageKey)
         *     - GetHeight(string ImageKey)
         *     - GetObjType(string ImageKey)
         *     - GetTrigger(string ImageKey)
         *     - CreatePieceKey(string FilePath)
         *     - CreatePieceKey(string StyleName, string PieceName, bool IsObject)
         * -------------------------------------------------------- */
        static ImageLibrary()
        {
            fImageList = new Dictionary<string, BaseImageInfo>();
        }

        // The key is the file path below the "styles\\pieces" folder!
        static Dictionary<string, BaseImageInfo> fImageList;


        /// <summary>
        /// Returns the image corresponding to the key, or null if image cannot be found. 
        /// <para> Warning: The Bitmap is passed by reference, so NEVER change its value! </para>
        /// </summary>
        /// <param name="ImageKey"></param>
        /// <returns></returns>
        public static Bitmap GetImage(string ImageKey)
        {
            return GetImage(ImageKey, RotateFlipType.RotateNoneFlipNone);
        }

        /// <summary>
        /// Returns a correctly oriented image corresponding to the key, or null if image cannot be found. 
        /// <para> Warning: The Bitmap is passed by reference, so NEVER change its value! </para>
        /// </summary>
        /// <param name="ImageKey"></param>
        /// <param name="RotFlipType"></param>
        /// <returns></returns>
        public static Bitmap GetImage(string ImageKey, RotateFlipType RotFlipType)
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

            return fImageList[ImageKey].Image(RotFlipType);
        }


        /// <summary>
        /// Returns the width of the piece corresponding to the key, or -1 if image cannot be found. 
        /// </summary>
        /// <param name="ImageKey"></param>
        /// <returns></returns>
        public static int GetWidth(string ImageKey)
        {
            if (!fImageList.ContainsKey(ImageKey))
            {
                bool Success = AddNewImage(ImageKey);
                if (!Success) return -1;
            }

            return fImageList[ImageKey].Width;
        }

        /// <summary>
        /// Returns the height of the piece corresponding to the key, or -1 if image cannot be found. 
        /// </summary>
        /// <param name="ImageKey"></param>
        /// <returns></returns>
        public static int GetHeight(string ImageKey)
        {
            if (!fImageList.ContainsKey(ImageKey))
            {
                bool Success = AddNewImage(ImageKey);
                if (!Success) return -1;
            }

            return fImageList[ImageKey].Height;
        }

        /// <summary>
        /// Returns the object type of the piece corresponding to the key, or C.OBJ.NULL if image cannot be found. 
        /// </summary>
        /// <param name="ImageKey"></param>
        /// <returns></returns>
        public static C.OBJ GetObjType(string ImageKey)
        {
            if (!fImageList.ContainsKey(ImageKey))
            {
                bool Success = AddNewImage(ImageKey);
                if (!Success) return C.OBJ.NULL;
            }

            return fImageList[ImageKey].ObjectType;
        }

        /// <summary>
        /// Returns the trigger area of the piece corresponding to the key, or an empty rectangle if image cannot be found. 
        /// </summary>
        /// <param name="ImageKey"></param>
        /// <returns></returns>
        public static Rectangle GetTrigger(string ImageKey)
        {
            if (!fImageList.ContainsKey(ImageKey))
            {
                bool Success = AddNewImage(ImageKey);
                if (!Success) return new Rectangle(0, 0, 0, 0);
            }

            return fImageList[ImageKey].TriggerRect;
        }

        /// <summary>
        /// Loads a new image into the ImageLibrary. Returns false, if image cannot be found.
        /// </summary>
        /// <param name="ImageKey"></param>
        /// <returns></returns>
        static bool AddNewImage(string ImageKey)
        {
            Bitmap NewBitmap = LoadStylesFromFile.Image(ImageKey);
            if (NewBitmap == null) return false;

            BaseImageInfo NewImageInfo = LoadStylesFromFile.ImageInfo(NewBitmap, ImageKey);
            fImageList.Add(ImageKey, NewImageInfo);

            return true;
        }

        /// <summary>
        /// Adds by hand a new image to the ImagelIbrary, assuming the ImageKey doesn't exist yet. 
        /// </summary>
        /// <param name="ImageKey"></param>
        /// <param name="Image"></param>
        /// <param name="ObjType"></param>
        /// <param name="TriggerRect"></param>
        public static void AddNewImage(string ImageKey, Bitmap Image, C.OBJ ObjType, Rectangle TriggerRect)
        {
            if (fImageList.ContainsKey(ImageKey)) return;

            BaseImageInfo NewImageInfo = new BaseImageInfo(Image, ObjType, 1, false, TriggerRect);
            fImageList.Add(ImageKey, NewImageInfo);
        }


        /// <summary>
        /// Creates the image key from a file path (relative or absolute). 
        /// </summary>
        /// <param name="FilePath"></param>
        /// <returns></returns>
        public static string CreatePieceKey(string FilePath)
        {
            string FullPath = System.IO.Path.GetFullPath(FilePath);
            string RelativePath = FullPath.Remove(0, C.AppPathPieces.Length);
            return System.IO.Path.ChangeExtension(RelativePath, null);
        }

        /// <summary>
        /// Creates the image key from the style and piece name.
        /// </summary>
        /// <param name="StyleName"></param>
        /// <param name="PieceName"></param>
        /// <param name="IsObject"></param>
        /// <returns></returns>
        public static string CreatePieceKey(string StyleName, string PieceName, bool IsObject)
        {
            return StyleName + C.DirSep + (IsObject ? "objects" : "terrain")
                             + C.DirSep + PieceName;
        }
    }
}
