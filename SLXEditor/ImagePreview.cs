using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SLXEditor
{
    internal static class ImagePreview
    {
        public static void PreviewPiece(string key, PictureBox pic, Style style)
        {
            pic.Image?.Dispose();

            if (string.IsNullOrEmpty(key))
            {
                pic.Image = null;
                return;
            }

            int frameIndex = (ImageLibrary.GetObjType(key).In(C.OBJ.PICKUP, C.OBJ.EXIT_LOCKED, C.OBJ.BUTTON, C.OBJ.COLLECTIBLE, C.OBJ.TRAPONCE)) ? 1 : 0;
            Bitmap pieceImage = ImageLibrary.GetImage(key, RotateFlipType.RotateNoneFlipNone, frameIndex);

            if (pieceImage == null)
            {
                pic.Image = null;
                return;
            }

            if (key.StartsWith("default") && ImageLibrary.GetObjType(key) == C.OBJ.ONE_WAY_WALL)
            {
                Color blendColor = style?.GetColor(C.StyleColor.ONE_WAY_WALL) ?? C.SLXColors[C.SLXColor.OWWDefault];
                pieceImage = pieceImage.ApplyThemeColor(blendColor);
            }

            ZoomImageWithNearestNeighbor(pieceImage, pic);
        }

        private static void ZoomImageWithNearestNeighbor(Bitmap originalImage, PictureBox pic)
        {
            // Add padding to the image before rendering to prevent cropping
            int padding = 1;
            Bitmap paddedImage = new Bitmap(originalImage.Width + padding, originalImage.Height + padding);

            using (Graphics g = Graphics.FromImage(paddedImage))
            {
                g.Clear(Color.Transparent);
                g.DrawImage(originalImage, padding, padding);
            }

            int maxWidth = pic.Width;
            int maxHeight = pic.Height;

            int currentWidth = paddedImage.Width;
            int currentHeight = paddedImage.Height;

            // Keep doubling the image size until it exceeds the PictureBox size
            while (currentWidth * 2 <= maxWidth && currentHeight * 2 <= maxHeight)
            {
                currentWidth *= 2;
                currentHeight *= 2;
            }

            Bitmap zoomedImage = new Bitmap(currentWidth, currentHeight);

            // Draw the scaled image with NearestNeighbor interpolation for accuracy
            using (Graphics g = Graphics.FromImage(zoomedImage))
            {
                g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.NearestNeighbor;
                g.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
                g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.None;
                g.DrawImage(paddedImage, 0, 0, currentWidth, currentHeight);
            }

            pic.Image = zoomedImage;
        }
    }
}
