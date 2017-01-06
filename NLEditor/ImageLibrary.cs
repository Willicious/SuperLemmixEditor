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
            : this(NewImage, IsSteel ? C.OBJ.STEEL : C.OBJ.TERRAIN, 1, false, new Rectangle(0, 0, 0, 0), C.Resize.None)
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
        public BaseImageInfo(Bitmap NewImage, C.OBJ ObjType, int NumFrames, bool IsVert, Rectangle TriggerRect, C.Resize ResizeMode)
        {
            this.fImages = SeparateFrames(NewImage, NumFrames, IsVert);
            this.fWidth = this.fImages[0].Width;
            this.fHeight = this.fImages[0].Height;
            this.fObjectType = ObjType;
            this.fTriggerRect = TriggerRect;
            this.fResizeMode = ResizeMode;
            this.fImageRotated = new Dictionary<RotateFlipType, List<Bitmap>>();
        }

        readonly List<Bitmap> fImages;
        readonly int fWidth;
        readonly int fHeight;
        readonly C.OBJ fObjectType;
        readonly Rectangle fTriggerRect;
        readonly C.Resize fResizeMode;

        readonly Dictionary<RotateFlipType, List<Bitmap>> fImageRotated;


        public Bitmap Image(RotateFlipType RotFlipType)
        {
            if (!fImageRotated.ContainsKey(RotFlipType)) CreateRotatedImages(RotFlipType);
            return fImageRotated[RotFlipType][0];
        }
        public Bitmap Image(RotateFlipType RotFlipType, int Index)
        {
            if (!fImageRotated.ContainsKey(RotFlipType)) CreateRotatedImages(RotFlipType);
            return fImageRotated[RotFlipType][Index % fImageRotated[RotFlipType].Count];
        }
        public int Width { get { return fWidth; } }
        public int Height { get { return fHeight; } }
        public C.OBJ ObjectType { get { return fObjectType; } }
        public Rectangle TriggerRect { get { return fTriggerRect; } }
        public C.Resize ResizeMode { get { return fResizeMode; } }

        /// <summary>
        /// Removes additional frames from an image and keeps only frame 0.
        /// </summary>
        /// <param name="NewBitmap"></param>
        /// <param name="NumFrames"></param>
        /// <param name="IsVert"></param>
        /// <returns></returns>
        private List<Bitmap> SeparateFrames(Bitmap NewBitmap, int NumFrames, bool IsVert)
        {
            List<Bitmap> ImageFrames = new List<Bitmap>();
            
            int NewWidth = NewBitmap.Width;
            int NewHeight = NewBitmap.Height;
            NumFrames = Math.Max(NumFrames, 1);

            if (IsVert)
            {
                NewHeight = NewHeight / NumFrames;
            }
            else
            {
                NewWidth = NewWidth / NumFrames;
            }

            for (int Index = 0; Index < NumFrames; Index++)
            {
                int StartX = (IsVert) ? 0 : Index * NewWidth;
                int StartY = (IsVert) ? Index * NewHeight : 0;
                Rectangle ThisRect = new Rectangle(StartX, StartY, NewWidth, NewHeight);
                ImageFrames.Add(NewBitmap.Crop(ThisRect));
            }

            return ImageFrames;
        }

        /// <summary>
        /// Creates rotated images of the desired orientation, if these do not yet exist.
        /// </summary>
        /// <param name="RotFlipType"></param>
        private void CreateRotatedImages(RotateFlipType RotFlipType)
        {
            fImageRotated[RotFlipType] = new List<Bitmap>();
            foreach (Bitmap ImageFrame in fImages)
            {
                Bitmap RotImage = (Bitmap)ImageFrame.Clone();
                RotImage.RotateFlip(RotFlipType);
                fImageRotated[RotFlipType].Add(RotImage);
            }
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
         *     - ExistsKey(string ImageKey)
         *     - GetImage(string ImageKey, RotateFlipType RotFlipType)
         *     - GetImage(string ImageKey, RotateFlipType RotFlipType, int Index)
         *     - GetWidth(string ImageKey)
         *     - GetHeight(string ImageKey)
         *     - GetObjType(string ImageKey)
         *     - GetTrigger(string ImageKey)
         *     - GetResizeMode(string ImageKey)
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
        /// Returns whether an image with this ImageKey exists.
        /// </summary>
        /// <param name="ImageKey"></param>
        /// <returns></returns>
        public static bool ExistsKey(string ImageKey)
        {
            if (fImageList.ContainsKey(ImageKey)) return true;
            else return AddNewImage(ImageKey);
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
            return GetImage(ImageKey, RotFlipType, 0);
        }

        /// <summary>
        /// Returns a correctly oriented image corresponding to the key and index, or null if image cannot be found. 
        /// <para> Warning: The Bitmap is passed by reference, so NEVER change its value! </para>
        /// </summary>
        /// <param name="ImageKey"></param>
        /// <param name="RotFlipType"></param>
        /// <returns></returns>
        public static Bitmap GetImage(string ImageKey, RotateFlipType RotFlipType, int Index)
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

            return fImageList[ImageKey].Image(RotFlipType, Index);
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
        /// Returns the resize mode of the piece corresponding to the key, or C.Resize.None if image cannot be found. 
        /// </summary>
        /// <param name="ImageKey"></param>
        /// <returns></returns>
        public static C.Resize GetResizeMode(string ImageKey)
        {
            if (!fImageList.ContainsKey(ImageKey))
            {
                bool Success = AddNewImage(ImageKey);
                if (!Success) return C.Resize.None;
            }

            return fImageList[ImageKey].ResizeMode;
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

            try
            {
                fImageList[ImageKey] = LoadStylesFromFile.ImageInfo(NewBitmap, ImageKey);
            }
            catch (Exception Ex)
            {
                Utility.LogException(Ex);
                System.Windows.Forms.MessageBox.Show("Warning: Could not read .nxmo or .nxmt file at " + ImageKey + C.NewLine + Ex.Message);

                fImageList[ImageKey] = new BaseImageInfo(new Bitmap(1, 1));
            }

            return true;
        }

        /// <summary>
        /// Adds by hand a new image to the ImagelIbrary, assuming the ImageKey doesn't exist yet. 
        /// </summary>
        /// <param name="ImageKey"></param>
        /// <param name="Image"></param>
        /// <param name="ObjType"></param>
        /// <param name="TriggerRect"></param>
        public static void AddNewImage(string ImageKey, Bitmap Image, C.OBJ ObjType, Rectangle TriggerRect, C.Resize ResizeMode)
        {
            if (fImageList.ContainsKey(ImageKey)) return;

            try
            {
                fImageList[ImageKey] = new BaseImageInfo(Image, ObjType, 1, false, TriggerRect, ResizeMode);
            }
            catch (Exception Ex)
            {
                Utility.LogException(Ex);
                System.Windows.Forms.MessageBox.Show("Warning: Could not read .nxmo or .nxmt file at " + ImageKey + C.NewLine + Ex.Message);
                
                fImageList[ImageKey] = new BaseImageInfo(new Bitmap(1, 1));
            }
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
        /// <para> Do NOT use this for background images! </para>
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
