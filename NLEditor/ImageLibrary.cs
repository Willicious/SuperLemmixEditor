using System;
using System.Collections.Generic;
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
        /// <param name="newImage"></param>
        /// <param name="isSteel"></param>
        public BaseImageInfo(Bitmap newImage, bool isSteel = false)
            : this(newImage, isSteel ? C.OBJ.STEEL : C.OBJ.TERRAIN, 1, false, new Rectangle(0, 0, 0, 0), C.Resize.None)
        {
            // nothing more
        }

        /// <summary>
        /// Use this to create the base-info of a new object piece.
        /// </summary>
        /// <param name="newImage"></param>
        /// <param name="objType"></param>
        /// <param name="numFrames"></param>
        /// <param name="isVert"></param>
        /// <param name="triggerRect"></param>
        public BaseImageInfo(Bitmap newImage, C.OBJ objType, int numFrames, bool isVert, Rectangle triggerRect, C.Resize resizeMode)
        {
            this.images = new Dictionary<RotateFlipType, List<Bitmap>>();
            this.images[RotateFlipType.RotateNoneFlipNone] = SeparateFrames(newImage, numFrames, isVert);
            this.Width = this.baseImages[0].Width;
            this.Height = this.baseImages[0].Height;
            this.ObjectType = objType;
            this.TriggerRect = triggerRect;
            this.ResizeMode = resizeMode;
        }

        Dictionary<RotateFlipType, List<Bitmap>> images;
        List<Bitmap> baseImages => images[RotateFlipType.RotateNoneFlipNone];
        List<Bitmap> imageWithPieceNames;

        public Bitmap Image(RotateFlipType rotFlipType)
        {
            if (!images.ContainsKey(rotFlipType)) CreateRotatedImages(rotFlipType);
            return images[rotFlipType][0];
        }
        public Bitmap Image(RotateFlipType rotFlipType, int index)
        {
            if (!images.ContainsKey(rotFlipType)) CreateRotatedImages(rotFlipType);
            return images[rotFlipType][index % images[rotFlipType].Count];
        }
        public Bitmap ImageWithPieceName(int index, string pieceKey)
        {
            if (imageWithPieceNames == null) CreateImagesWithPieceNames(pieceKey);
            return imageWithPieceNames[index % imageWithPieceNames.Count];
        }
        public int Width { get; private set; }
        public int Height { get; private set; }
        public C.OBJ ObjectType { get; private set; }
        public Rectangle TriggerRect { get; private set; }
        public C.Resize ResizeMode { get; private set; }

        /// <summary>
        /// Removes additional frames from an image and keeps only frame 0.
        /// </summary>
        /// <param name="newBitmap"></param>
        /// <param name="numFrames"></param>
        /// <param name="isVert"></param>
        /// <returns></returns>
        private List<Bitmap> SeparateFrames(Bitmap newBitmap, int numFrames, bool isVert)
        {
            List<Bitmap> imageFrames = new List<Bitmap>();
            
            int newWidth = newBitmap.Width;
            int newHeight = newBitmap.Height;
            numFrames = Math.Max(numFrames, 1);

            if (isVert)
            {
                newHeight = newHeight / numFrames;
            }
            else
            {
                newWidth = newWidth / numFrames;
            }

            for (int index = 0; index < numFrames; index++)
            {
                int startX = (isVert) ? 0 : index * newWidth;
                int startY = (isVert) ? index * newHeight : 0;
                Rectangle frameRect = new Rectangle(startX, startY, newWidth, newHeight);
                imageFrames.Add(newBitmap.Crop(frameRect));
            }

            return imageFrames;
        }

        /// <summary>
        /// Creates rotated images of the desired orientation, if these do not yet exist.
        /// </summary>
        /// <param name="rotFlipType"></param>
        private void CreateRotatedImages(RotateFlipType rotFlipType)
        {
            images[rotFlipType] = new List<Bitmap>();
            foreach (Bitmap imageFrame in baseImages)
            {
                Bitmap rotImage = (Bitmap)imageFrame.Clone();
                rotImage.RotateFlip(rotFlipType);
                images[rotFlipType].Add(rotImage);
            }
        }

        /// <summary>
        /// Creates images with the piece name at the bottom right corner.
        /// </summary>
        /// <param name="pieceKey"></param>
        private void CreateImagesWithPieceNames(string pieceKey)
        {
            imageWithPieceNames = new List<Bitmap>();

            foreach (Bitmap imageFrame in baseImages)
            {
                Bitmap newImage = new Bitmap(C.PicPieceSize.Width, C.PicPieceSize.Height);
                int posX = (newImage.Width - imageFrame.Width) / 2;
                int posY = (newImage.Height - imageFrame.Height) / 2;
                newImage.DrawOn(imageFrame, new Point(posX, posY));

                string pieceName = System.IO.Path.GetFileNameWithoutExtension(pieceKey);
                Point bottomRightCorner = new Point(newImage.Width, newImage.Height);
                newImage.WriteText(pieceName, bottomRightCorner, C.NLColors[C.NLColor.Text], 8, ContentAlignment.BottomRight);

                imageWithPieceNames.Add(newImage);
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
            imageDict = new Dictionary<string, BaseImageInfo>();
        }

        // The key is the file path below the "styles\\pieces" folder!
        static Dictionary<string, BaseImageInfo> imageDict;
        
        /// <summary>
        /// Returns whether an image with this ImageKey exists.
        /// </summary>
        /// <param name="imageKey"></param>
        /// <returns></returns>
        public static bool ExistsKey(string imageKey)
        {
            if (imageDict.ContainsKey(imageKey)) return true;
            else return AddNewImage(imageKey);
        }


        /// <summary>
        /// Returns a correctly oriented image corresponding to the key, or null if image cannot be found. 
        /// <para> Warning: The Bitmap is passed by reference, so NEVER change its value! </para>
        /// </summary>
        /// <param name="imageKey"></param>
        /// <param name="rotFlipType"></param>
        /// <returns></returns>
        public static Bitmap GetImage(string imageKey, RotateFlipType rotFlipType)
        {
            return GetImage(imageKey, rotFlipType, 0);
        }

        /// <summary>
        /// Returns a correctly oriented image corresponding to the key and index, or null if image cannot be found. 
        /// <para> Warning: The Bitmap is passed by reference, so NEVER change its value! </para>
        /// </summary>
        /// <param name="imageKey"></param>
        /// <param name="rotFlipType"></param>
        /// <returns></returns>
        public static Bitmap GetImage(string imageKey, RotateFlipType rotFlipType, int index)
        {
            if (!imageDict.ContainsKey(imageKey))
            {
                bool success = AddNewImage(imageKey);
                if (!success)
                {
                    System.Windows.Forms.MessageBox.Show("Cannot find image " + imageKey + ".", "File not found");
                    return null;
                }
            }

            return imageDict[imageKey].Image(rotFlipType, index);
        }

        /// <summary>
        /// Returns the image with the piece's name at the bottom right.
        /// <para> These images are used for the PieceSelection. </para>
        /// </summary>
        /// <param name="imageKey"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        public static Bitmap GetImageWithPieceName(string imageKey, int index)
        {
            if (!imageDict.ContainsKey(imageKey))
            {
                bool success = AddNewImage(imageKey);
                if (!success)
                {
                    System.Windows.Forms.MessageBox.Show("Cannot find image " + imageKey + ".", "File not found");
                    return null;
                }
            }

            return imageDict[imageKey].ImageWithPieceName(index, imageKey);
        }

        /// <summary>
        /// Returns the width of the piece corresponding to the key, or -1 if image cannot be found. 
        /// </summary>
        /// <param name="imageKey"></param>
        /// <returns></returns>
        public static int GetWidth(string imageKey)
        {
            if (!imageDict.ContainsKey(imageKey))
            {
                bool success = AddNewImage(imageKey);
                if (!success) return 0;
            }

            return imageDict[imageKey].Width;
        }

        /// <summary>
        /// Returns the height of the piece corresponding to the key, or -1 if image cannot be found. 
        /// </summary>
        /// <param name="imageKey"></param>
        /// <returns></returns>
        public static int GetHeight(string imageKey)
        {
            if (!imageDict.ContainsKey(imageKey))
            {
                bool success = AddNewImage(imageKey);
                if (!success) return 0;
            }

            return imageDict[imageKey].Height;
        }

        /// <summary>
        /// Returns the object type of the piece corresponding to the key, or C.OBJ.NULL if image cannot be found. 
        /// </summary>
        /// <param name="imageKey"></param>
        /// <returns></returns>
        public static C.OBJ GetObjType(string imageKey)
        {
            if (!imageDict.ContainsKey(imageKey))
            {
                bool success = AddNewImage(imageKey);
                if (!success) return C.OBJ.NULL;
            }

            return imageDict[imageKey].ObjectType;
        }

        /// <summary>
        /// Returns the trigger area of the piece corresponding to the key, or an empty rectangle if image cannot be found. 
        /// </summary>
        /// <param name="imageKey"></param>
        /// <returns></returns>
        public static Rectangle GetTrigger(string imageKey)
        {
            if (!imageDict.ContainsKey(imageKey))
            {
                bool success = AddNewImage(imageKey);
                if (!success) return new Rectangle(0, 0, 0, 0);
            }

            return imageDict[imageKey].TriggerRect;
        }

        /// <summary>
        /// Returns the resize mode of the piece corresponding to the key, or C.Resize.None if image cannot be found. 
        /// </summary>
        /// <param name="imageKey"></param>
        /// <returns></returns>
        public static C.Resize GetResizeMode(string imageKey)
        {
            if (!imageDict.ContainsKey(imageKey))
            {
                bool success = AddNewImage(imageKey);
                if (!success) return C.Resize.None;
            }

            return imageDict[imageKey].ResizeMode;
        }

        /// <summary>
        /// Loads a new image into the ImageLibrary. Returns false, if image cannot be found.
        /// </summary>
        /// <param name="imageKey"></param>
        /// <returns></returns>
        static bool AddNewImage(string imageKey)
        {
            Bitmap newBitmap = LoadStylesFromFile.Image(imageKey);
            if (newBitmap == null) return false;

            try
            {
                imageDict[imageKey] = LoadStylesFromFile.ImageInfo(newBitmap, imageKey);
            }
            catch (Exception Ex)
            {
                Utility.LogException(Ex);
                System.Windows.Forms.MessageBox.Show("Warning: Could not read .nxmo or .nxmt file at " + imageKey + C.NewLine + Ex.Message, "File corrupt");

                imageDict[imageKey] = new BaseImageInfo(new Bitmap(1, 1));
            }

            return true;
        }

        /// <summary>
        /// Adds by hand a new image to the ImagelIbrary, assuming the ImageKey doesn't exist yet. 
        /// </summary>
        /// <param name="imageKey"></param>
        /// <param name="image"></param>
        /// <param name="objType"></param>
        /// <param name="triggerRect"></param>
        public static void AddNewImage(string imageKey, Bitmap image, C.OBJ objType, Rectangle triggerRect, C.Resize resizeMode)
        {
            if (imageDict.ContainsKey(imageKey)) return;

            try
            {
                imageDict[imageKey] = new BaseImageInfo(image, objType, 1, false, triggerRect, resizeMode);
            }
            catch (Exception Ex)
            {
                Utility.LogException(Ex);
                System.Windows.Forms.MessageBox.Show("Warning: Could not read .nxmo or .nxmt file at " + imageKey + C.NewLine + Ex.Message, "File corrupt");
                
                imageDict[imageKey] = new BaseImageInfo(new Bitmap(1, 1));
            }
        }


        /// <summary>
        /// Creates the image key from a file path (relative or absolute). 
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public static string CreatePieceKey(string filePath)
        {
            string fullPath = System.IO.Path.GetFullPath(filePath);
            string relativePath = fullPath.Remove(0, C.AppPathPieces.Length);
            return System.IO.Path.ChangeExtension(relativePath, null);
        }

        /// <summary>
        /// Creates the image key from the style and piece name.
        /// <para> Do NOT use this for background images! </para>
        /// </summary>
        /// <param name="styleName"></param>
        /// <param name="pieceName"></param>
        /// <param name="isObject"></param>
        /// <returns></returns>
        public static string CreatePieceKey(string styleName, string pieceName, bool isObject)
        {
            return styleName + C.DirSep + (isObject ? "objects" : "terrain")
                             + C.DirSep + pieceName;
        }
    }
}
