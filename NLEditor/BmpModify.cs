using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace NLEditor
{
    static class BmpModify
    {
        /*---------------------------------------------------------
         *  This class contains all methods to modify bitmaps
         * -------------------------------------------------------- */

        /* --------------------------------------------------------
         *   public methods:
         *     - Crop(this Bitmap OrigBmp, Rectangle CropRect)
         *    // - DrawOn(this Bitmap OrigBmp, Bitmap NewBmp)
         *    // - DrawOn(this Bitmap OrigBmp, Bitmap NewBmp, Point Pos)
         *    // - Clear(this Bitmap OrigBmp)
         * -------------------------------------------------------- */

        public static Bitmap Crop(this Bitmap OrigBmp, Rectangle CropRect)
        {
            //------------------------------------------------
            // This method crops OrigBmp along the CropRect
            //------------------------------------------------

            return OrigBmp.Clone(CropRect, OrigBmp.PixelFormat);
        }
    }
}
