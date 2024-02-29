using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace NLEditor
{
    /// <summary>
    /// Stores the common unchangable data of pieces.
    /// </summary>
    class BaseImageInfo
    {
        /// <summary>
        /// Use this to create the base-info of a new terrain piece.
        /// </summary>
        /// <param name="newImage"></param>
        /// <param name="isSteel"></param>
        public BaseImageInfo(Bitmap newImage, bool isSteel = false, C.Resize resizeMode = C.Resize.None,
            bool isDeprecated = false, Rectangle? nineSlicingArea = null, int defaultWidth = 0, int defaultHeight = 0)
            : this(newImage, isSteel ? C.OBJ.STEEL : C.OBJ.TERRAIN, 1, new Rectangle(0, 0, 0, 0), resizeMode,
               0, 0, 0, 0, isDeprecated, nineSlicingArea, defaultWidth, defaultHeight)
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
        public BaseImageInfo(Bitmap newImage, C.OBJ objType, int numFrames, Rectangle triggerRect,
          C.Resize resizeMode, int leftMargin = 0, int topMargin = 0, int rightMargin = 0, int bottomMargin = 0,
          bool isDeprecated = false, Rectangle? nineSlicingArea = null, int defaultWidth = 0, int defaultHeight = 0)
        {
            this.images = new Dictionary<RotateFlipType, List<Bitmap>>();
            this.images[RotateFlipType.RotateNoneFlipNone] = SeparateFrames(newImage, numFrames, true);
            this.Width = this.baseImages[0].Width;
            this.Height = this.baseImages[0].Height;
            this.DefaultWidth = defaultWidth;
            this.DefaultHeight = defaultHeight;
            this.ObjectType = objType;
            this.TriggerRect = triggerRect;
            this.ResizeMode = resizeMode;
            this.PrimaryImageLocation = new Rectangle(leftMargin, topMargin, this.Width - leftMargin - rightMargin, this.Height - topMargin - bottomMargin);
            this.NineSlicingArea = nineSlicingArea;
            this.Deprecated = isDeprecated;
        }

        Dictionary<RotateFlipType, List<Bitmap>> images;
        List<Bitmap> baseImages => images[RotateFlipType.RotateNoneFlipNone];
        List<Bitmap> imageWithPieceNames;

        public Bitmap Image(RotateFlipType rotFlipType)
        {
            if (!images.ContainsKey(rotFlipType))
                CreateRotatedImages(rotFlipType);
            return images[rotFlipType][0];
        }
        public Bitmap Image(RotateFlipType rotFlipType, int index)
        {
            if (!images.ContainsKey(rotFlipType))
                CreateRotatedImages(rotFlipType);
            return images[rotFlipType][index % images[rotFlipType].Count];
        }
        public Bitmap ImageWithPieceName(int index, string pieceKey)
        {
            if (imageWithPieceNames == null)
                CreateImagesWithPieceNames(pieceKey);
            return imageWithPieceNames[index % imageWithPieceNames.Count];
        }
        public Bitmap WindowImageWithDirection(RotateFlipType rotFlipType, int index)
        {
            // Warning: Ignore rotFlipType for actual image and use it only for the directional arrow!
            Bitmap image = (Bitmap)Image(RotateFlipType.RotateNoneFlipNone, index).Clone();
            bool isFlipped = rotFlipType.In(RotateFlipType.RotateNoneFlipX, RotateFlipType.RotateNoneFlipXY, RotateFlipType.Rotate90FlipY, RotateFlipType.Rotate90FlipXY);
            string directionString = isFlipped ? "←" : "→";
            Point bottomRightCorner = new Point(image.Width, image.Height);
            image.WriteText(directionString, bottomRightCorner, C.NLColors[C.NLColor.Text], 7, ContentAlignment.BottomRight, new Size(12, 9));
            return image;
        }
        public int Width { get; private set; }
        public int Height { get; private set; }
        public int DefaultWidth { get; private set; }
        public int DefaultHeight { get; private set; }
        public C.OBJ ObjectType { get; private set; }
        public Rectangle TriggerRect { get; private set; }
        public C.Resize ResizeMode { get; private set; }
        public Rectangle PrimaryImageLocation { get; private set; }
        public Rectangle? NineSlicingArea { get; private set; }
        public bool Deprecated { get; private set; }

        /// <summary>
        /// Separates the various frames in one bitmap.
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
                newImage.DrawOn(imageFrame, new Point(posX, posY), 254);

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
            if (imageDict.ContainsKey(imageKey))
                return true;
            else
                return AddNewImage(imageKey);
        }

        /// <summary>
        /// This checks whether an image exists or may be loaded. It does not actually load the image itself.
        /// </summary>
        /// <param name="imagekey"></param>
        /// <returns></returns>
        public static bool IsImageLoadable(string imageKey)
        {
            string filePath = C.AppPathPieces + imageKey + ".png";
            if (imageDict.ContainsKey(imageKey))
                return true;
            else if (!System.IO.File.Exists(filePath))
                return false;
            else
            {
                try
                {
                    System.IO.File.Open(filePath, System.IO.FileMode.Open, System.IO.FileAccess.Read).Dispose();
                }
                catch (System.IO.IOException)
                {
                    return false;
                }
                return true;
            }
        }

        /// <summary>
        /// Returns a correctly oriented image corresponding to the key, or null if image cannot be found. 
        /// <para> Warning: The Bitmap is passed by reference, so NEVER change its value! </para>
        /// </summary>
        /// <param name="imageKey"></param>
        /// <param name="rotFlipType"></param>
        /// <returns></returns>
        public static Bitmap GetImage(string imageKey, RotateFlipType rotFlipType = RotateFlipType.RotateNoneFlipNone)
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
        /// Returns the image with the directional arrow at the bottom right.
        /// </summary>
        /// <param name="imageKey"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        public static Bitmap GetWindowImageWithDirection(string imageKey, RotateFlipType rotFlipType, int index)
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

            return imageDict[imageKey].WindowImageWithDirection(rotFlipType, index);
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
                if (!success)
                    return 0;
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
                if (!success)
                    return 0;
            }

            return imageDict[imageKey].Height;
        }

        /// <summary>
        /// Returns the default width of the piece corresponding to the key, or -1 if image cannot be found. 
        /// </summary>
        /// <param name="imageKey"></param>
        /// <returns></returns>
        public static int GetDefaultWidth(string imageKey)
        {
            if (!imageDict.ContainsKey(imageKey))
            {
                bool success = AddNewImage(imageKey);
                if (!success)
                    return 0;
            }

            return imageDict[imageKey].DefaultWidth;
        }

        /// <summary>
        /// Returns the default height of the piece corresponding to the key, or -1 if image cannot be found. 
        /// </summary>
        /// <param name="imageKey"></param>
        /// <returns></returns>
        public static int GetDefaultHeight(string imageKey)
        {
            if (!imageDict.ContainsKey(imageKey))
            {
                bool success = AddNewImage(imageKey);
                if (!success)
                    return 0;
            }

            return imageDict[imageKey].DefaultHeight;
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
                if (!success)
                    return C.OBJ.NULL;
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
                if (!success)
                    return new Rectangle(0, 0, 0, 0);
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
                if (!success)
                    return C.Resize.None;
            }

            return imageDict[imageKey].ResizeMode;
        }

        public static bool GetDeprecated(string imageKey)
        {
            if (!imageDict.ContainsKey(imageKey))
            {
                bool success = AddNewImage(imageKey);
                if (!success)
                    return false;
            }

            return imageDict[imageKey].Deprecated;
        }

        public static Rectangle? GetNineSliceArea(string imageKey, RotateFlipType rotFlipType = RotateFlipType.RotateNoneFlipNone)
        {
            if (!imageDict.ContainsKey(imageKey))
            {
                bool success = AddNewImage(imageKey);
                if (!success)
                    return null;
            }

            Rectangle? baseArea = imageDict[imageKey].NineSlicingArea;
            if (baseArea == null)
                return null;

            int height = imageDict[imageKey].Height;
            int width = imageDict[imageKey].Width;
            Rectangle area = baseArea.Value;

            // Apply rotation
            switch (rotFlipType)
            {
                case RotateFlipType.RotateNoneFlipNone:
                    return baseArea;
                case RotateFlipType.RotateNoneFlipX:
                    return new Rectangle(width - area.Right, area.Top, area.Width, area.Height);
                case RotateFlipType.RotateNoneFlipY:
                    return new Rectangle(area.Left, height - area.Bottom, area.Width, area.Height);
                case RotateFlipType.RotateNoneFlipXY:
                    return new Rectangle(area.Left, height - area.Bottom, area.Width, area.Height);
                case RotateFlipType.Rotate90FlipNone:
                    return new Rectangle(height - area.Bottom, area.Left, area.Height, area.Width);
                case RotateFlipType.Rotate90FlipX:
                    return new Rectangle(area.Top, area.Left, area.Height, area.Width);
                case RotateFlipType.Rotate90FlipY:
                    return new Rectangle(height - area.Bottom, width - area.Right, area.Height, area.Width);
                case RotateFlipType.Rotate90FlipXY:
                    return new Rectangle(area.Top, width - area.Right, area.Height, area.Width);
                default:
                    return null;
            }
        }

        /// <summary>
        /// Loads a new image into the ImageLibrary. Returns false, if image cannot be found.
        /// </summary>
        /// <param name="imageKey"></param>
        /// <returns></returns>
        static bool AddNewImage(string imageKey)
        {
            try
            {
                imageDict[imageKey] = LoadStylesFromFile.ImageInfo(imageKey);

                return true;
            }
            catch (Exception Ex)
            {
                Utility.LogException(Ex);
                return false;
            }

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
            if (imageDict.ContainsKey(imageKey))
                return;

            try
            {
                imageDict[imageKey] = new BaseImageInfo(image, objType, 1, triggerRect, resizeMode);
            }
            catch (Exception Ex)
            {
                Utility.LogException(Ex);
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
        /// Creates the sketch image key from a file path (relative or absolute). 
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public static string CreateSketchKey(string filePath)
        {
            string fullPath = System.IO.Path.GetFullPath(filePath);
            string relativePath = fullPath.Remove(0, (C.AppPath + "sketches\\").Length);
            return "*sketch:" + System.IO.Path.ChangeExtension(relativePath, null);
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

        /// <summary>
        /// Transforms the piece location from level file location (using only the primary image)
        /// to editor location (using the merge of primary and secondary animations)
        /// </summary>
        /// <param name="levelFileLocation"></param>
        /// <returns></returns>
        public static Point LevelFileToEditorCoordinates(string imageKey, Point levelFileLocation,
          bool rotate, bool flip, bool invert)
        {
            if (!imageDict.ContainsKey(imageKey))
            {
                bool success = AddNewImage(imageKey);
                if (!success)
                    return levelFileLocation;
            }

            var img = imageDict[imageKey];
            var primaryImageLocation = img.PrimaryImageLocation;
            var boundsRect = new Rectangle(0, 0, img.Width, img.Height);

            if (rotate)
            {
                int oldX = primaryImageLocation.X;
                primaryImageLocation.X = boundsRect.Height - primaryImageLocation.Y - primaryImageLocation.Height;
                primaryImageLocation.Y = oldX;

                int oldW = primaryImageLocation.Width;
                primaryImageLocation.Width = primaryImageLocation.Height;
                primaryImageLocation.Height = oldW;

                if (flip)
                    primaryImageLocation.X = boundsRect.Bottom - primaryImageLocation.Left - primaryImageLocation.Width;
                if (invert)
                    primaryImageLocation.Y = boundsRect.Right - primaryImageLocation.Top - primaryImageLocation.Height;
            }
            else
            {
                if (flip)
                    primaryImageLocation.X = boundsRect.Right - primaryImageLocation.Right;
                if (invert)
                    primaryImageLocation.Y = boundsRect.Bottom - primaryImageLocation.Bottom;
            }

            return new Point(levelFileLocation.X - primaryImageLocation.X,
                             levelFileLocation.Y - primaryImageLocation.Y);
        }

        /// <summary>
        /// Transforms the piece location from editor location (using the merge of primary and secondary animations)
        /// to level file location (using only the primary image)
        /// </summary>
        /// <param name="imageKey"></param>
        /// <param name="editorLocation"></param>
        /// <returns></returns>
        public static Point EditorToLevelFileCoordinates(string imageKey, Point editorLocation,
          bool rotate, bool flip, bool invert)
        {
            if (!imageDict.ContainsKey(imageKey))
            {
                bool success = AddNewImage(imageKey);
                if (!success)
                    return editorLocation;
            }

            var img = imageDict[imageKey];
            var primaryImageLocation = img.PrimaryImageLocation;
            var boundsRect = new Rectangle(0, 0, img.Width, img.Height);

            if (rotate)
            {
                int oldX = primaryImageLocation.X;
                primaryImageLocation.X = boundsRect.Height - primaryImageLocation.Y - primaryImageLocation.Height;
                primaryImageLocation.Y = oldX;

                int oldW = primaryImageLocation.Width;
                primaryImageLocation.Width = primaryImageLocation.Height;
                primaryImageLocation.Height = oldW;

                if (flip)
                    primaryImageLocation.X = boundsRect.Bottom - primaryImageLocation.Left - primaryImageLocation.Width;
                if (invert)
                    primaryImageLocation.Y = boundsRect.Right - primaryImageLocation.Top - primaryImageLocation.Height;
            }
            else
            {
                if (flip)
                    primaryImageLocation.X = boundsRect.Right - primaryImageLocation.Right;
                if (invert)
                    primaryImageLocation.Y = boundsRect.Bottom - primaryImageLocation.Bottom;
            }

            return new Point(editorLocation.X + primaryImageLocation.X,
                             editorLocation.Y + primaryImageLocation.Y);
        }
    }
}
