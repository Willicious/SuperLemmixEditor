using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Drawing;
using System.Drawing.Imaging;
using System.Diagnostics;

namespace NLEditor
{
    /// <summary>
    /// This class contains all methods to modify bitmaps
    /// </summary>
    static class BmpModify
    {
        /// <summary>
        /// Initializes the dictionary ColorFuncDict.
        /// </summary>
        static BmpModify()
        {
            colorFuncDict = new Dictionary<C.CustDrawMode, Func<int, int, byte[]>>();
            colorFuncDict.Add(C.CustDrawMode.Default, null);
            colorFuncDict.Add(C.CustDrawMode.DefaultOWW, null);
            colorFuncDict.Add(C.CustDrawMode.Erase, ColorFunc_Erase);
            colorFuncDict.Add(C.CustDrawMode.NoOverwrite, null);
            colorFuncDict.Add(C.CustDrawMode.NoOverwriteOWW, null);
            colorFuncDict.Add(C.CustDrawMode.OnlyAtMask, null);
            colorFuncDict.Add(C.CustDrawMode.OnlyAtOWW, null);
            colorFuncDict.Add(C.CustDrawMode.ClearPhysics, ColorFunc_ClearPhysics);
            colorFuncDict.Add(C.CustDrawMode.ClearPhysicsOWW, ColorFunc_ClearPhysicsOWW);
            colorFuncDict.Add(C.CustDrawMode.ClearPhysicsSteel, ColorFunc_ClearPhysicsSteel);
            colorFuncDict.Add(C.CustDrawMode.ClearPhysicsNoOverwrite, ColorFunc_ClearPhysics);
            colorFuncDict.Add(C.CustDrawMode.ClearPhysicsNoOverwriteOWW, ColorFunc_ClearPhysicsOWW);
            colorFuncDict.Add(C.CustDrawMode.ClearPhysicsSteelNoOverwrite, ColorFunc_ClearPhysicsSteel);

            doDrawThisPixelDict = new Dictionary<C.CustDrawMode, Func<byte, byte, bool>>();
            doDrawThisPixelDict.Add(C.CustDrawMode.Default, DoDrawThisPixel_DrawNew);
            doDrawThisPixelDict.Add(C.CustDrawMode.DefaultOWW, DoDrawThisPixel_DrawNew);
            doDrawThisPixelDict.Add(C.CustDrawMode.Erase, DoDrawThisPixel_DrawNew);
            doDrawThisPixelDict.Add(C.CustDrawMode.NoOverwrite, DoDrawThisPixel_NotAtMask);
            doDrawThisPixelDict.Add(C.CustDrawMode.NoOverwriteOWW, DoDrawThisPixel_NotAtMask);
            doDrawThisPixelDict.Add(C.CustDrawMode.OnlyAtMask, DoDrawThisPixel_OnlyAtMask);
            doDrawThisPixelDict.Add(C.CustDrawMode.OnlyAtOWW, DoDrawThisPixel_OnlyAtOWW);
            doDrawThisPixelDict.Add(C.CustDrawMode.ClearPhysics, DoDrawThisPixel_DrawNew);
            doDrawThisPixelDict.Add(C.CustDrawMode.ClearPhysicsOWW, DoDrawThisPixel_DrawNew);
            doDrawThisPixelDict.Add(C.CustDrawMode.ClearPhysicsSteel, DoDrawThisPixel_DrawNew);
            doDrawThisPixelDict.Add(C.CustDrawMode.ClearPhysicsNoOverwrite, DoDrawThisPixel_NotAtMask);
            doDrawThisPixelDict.Add(C.CustDrawMode.ClearPhysicsNoOverwriteOWW, DoDrawThisPixel_NotAtMask);
            doDrawThisPixelDict.Add(C.CustDrawMode.ClearPhysicsSteelNoOverwrite, DoDrawThisPixel_NotAtMask);
        }

        private static Dictionary<C.CustDrawMode, Func<int, int, byte[]>> colorFuncDict;
        private static Dictionary<C.CustDrawMode, Func<byte, byte, bool>> doDrawThisPixelDict;

        const int BytesPerPixel = 4;

        private static readonly byte[] COLOR_ERASE = { 0, 0, 0, 0 };
        private static readonly byte[] COLOR_CLEAR_PHYSICS_LIGHT = { 200, 200, 200, 254 };
        private static readonly byte[] COLOR_CLEAR_PHYSICS_DARK = { 170, 170, 170, 254 };
        private static readonly byte[] COLOR_CLEAR_PHYSICS_LIGHT_OWW = { 200, 200, 200, 255 };
        private static readonly byte[] COLOR_CLEAR_PHYSICS_DARK_OWW = { 170, 170, 170, 255 };
        private static readonly byte[] COLOR_CLEAR_PHYSICS_STEEL_LIGHT = { 80, 80, 80, 254 };
        private static readonly byte[] COLOR_CLEAR_PHYSICS_STEEL_DARK = { 50, 50, 50, 254 };
        private static readonly byte[] COLOR_RECTANGLE_LIGHT = { 240, 240, 240, 255 };
        private static readonly byte[] COLOR_RECTANGLE_DARK = { 30, 30, 30, 255 };

        [Obsolete]
        public static void SetCustomDrawMode(Func<int, int, byte[]> colorFunc, Func<byte, byte, bool> drawTypeFunc)
        {
            colorFuncDict[C.CustDrawMode.Custom] = colorFunc;
            doDrawThisPixelDict[C.CustDrawMode.Custom] = (drawTypeFunc != null) ? drawTypeFunc : DoDrawThisPixel_DrawNew;
        }

        private static byte[] ColorFunc_Erase(int posX, int posY)
        {
            return COLOR_ERASE;
        }

        private static byte[] ColorFunc_ClearPhysics(int posX, int posY)
        {
            if ((posX + posY) % 2 == 0)
            {
                return COLOR_CLEAR_PHYSICS_LIGHT;
            }
            else
            {
                return COLOR_CLEAR_PHYSICS_DARK;
            }
        }

        private static byte[] ColorFunc_ClearPhysicsOWW(int posX, int posY)
        {
            if ((posX + posY) % 2 == 0)
            {
                return COLOR_CLEAR_PHYSICS_LIGHT_OWW;
            }
            else
            {
                return COLOR_CLEAR_PHYSICS_DARK_OWW;
            }
        }

        private static byte[] ColorFunc_ClearPhysicsSteel(int posX, int posY)
        {
            if ((posX + posY) % 2 == 0)
            {
                return COLOR_CLEAR_PHYSICS_STEEL_LIGHT;
            }
            else
            {
                return COLOR_CLEAR_PHYSICS_STEEL_DARK;
            }
        }

        private static bool DoDrawThisPixel_DrawNew(byte newBmpAlpha, byte maskBmpAlpha)
        {
            return (newBmpAlpha > 63);
        }

        private static bool DoDrawThisPixel_OnlyAtMask(byte newBmpAlpha, byte maskBmpAlpha)
        {
            return (newBmpAlpha > 63) && (maskBmpAlpha > 63);
        }

        public static bool DoDrawThisPixel_OnlyAtOWW(byte newBmpAlpha, byte maskBmpAlpha)
        {
            return (newBmpAlpha > 63) && (maskBmpAlpha == 255);
        }

        private static bool DoDrawThisPixel_NotAtMask(byte newBmpAlpha, byte maskBmpAlpha)
        {
            return (newBmpAlpha > 63) && (maskBmpAlpha < 63);
        }

        /// <summary>
        /// Copies the ColorBytes to the pixel pointed to.
        /// <para> WARNING: Always make sure that ColorBytes has at least length 4. </para>
        /// </summary>
        /// <param name="ptrToPixel"></param>
        /// <param name="colorBytes"></param>
        private static unsafe void ChangePixel(byte* ptrToPixel, byte[] colorBytes)
        {
            ptrToPixel[0] = colorBytes[0];
            ptrToPixel[1] = colorBytes[1];
            ptrToPixel[2] = colorBytes[2];
            ptrToPixel[3] = colorBytes[3];
        }

        /// <summary>
        /// Copies the bytes of the NewPixel to the pixel pointed to in the first argument.
        /// </summary>
        /// <param name="ptrToPixel"></param>
        /// <param name="ptrToNewPixel"></param>
        private static unsafe void ChangePixel(byte* ptrToPixel, byte* ptrToNewPixel, byte alpha = 255)
        {
            ptrToPixel[0] = ptrToNewPixel[0];
            ptrToPixel[1] = ptrToNewPixel[1];
            ptrToPixel[2] = ptrToNewPixel[2];
            ptrToPixel[3] = alpha;
        }

        /// <summary>
        /// Automatically blends the bytes of the NewPixel with a given color and assigns this value.
        /// </summary>
        /// <param name="ptrToPixel"></param>
        /// <param name="ptrToNewPixel"></param>
        /// <param name="colorBytes"></param>
        private static unsafe void ChangePixel(byte* ptrToPixel, byte* ptrToNewPixel, byte[] colorBytes)
        {
            ptrToPixel[0] = (byte)(ptrToNewPixel[0] * colorBytes[0] / 255);
            ptrToPixel[1] = (byte)(ptrToNewPixel[1] * colorBytes[1] / 255);
            ptrToPixel[2] = (byte)(ptrToNewPixel[2] * colorBytes[2] / 255);
            ptrToPixel[3] = 255;
        }


        /// <summary>
        /// Copies the bytes of the NewPixel to the pixel pointed to in the first argument using swapped alpha blending.
        /// </summary>
        /// <param name="ptrToPixel"></param>
        /// <param name="ptrToNewPixel"></param>
        private static unsafe void ChangePixelBlend(byte* ptrToPixel, byte* ptrToNewPixel)
        {
            int NewAlphaFact = ptrToNewPixel[3];
            int OrigAlphaFact = (255 - NewAlphaFact) / 4; // because the orig bitmap has alpha 25%
            ptrToPixel[0] = (byte)((ptrToPixel[0] * OrigAlphaFact + ptrToNewPixel[0] * NewAlphaFact) / (OrigAlphaFact + NewAlphaFact));
            ptrToPixel[1] = (byte)((ptrToPixel[1] * OrigAlphaFact + ptrToNewPixel[1] * NewAlphaFact) / (OrigAlphaFact + NewAlphaFact));
            ptrToPixel[2] = (byte)((ptrToPixel[2] * OrigAlphaFact + ptrToNewPixel[2] * NewAlphaFact) / (OrigAlphaFact + NewAlphaFact));
            ptrToPixel[3] = 255;
        }


        /// <summary>
        /// Crops the bitmap along a rectangle.
        /// </summary>
        /// <param name="origBmp"></param>
        /// <param name="cropRect"></param>
        /// <returns></returns>
        public static Bitmap Crop(this Bitmap origBmp, Rectangle cropRect)
        {
            cropRect.Intersect(new Rectangle(0, 0, origBmp.Width, origBmp.Height));

            return origBmp.Clone(cropRect, origBmp.PixelFormat);
        }

        /// <summary>
        /// Sets all pixels to transparent black.
        /// </summary>
        /// <param name="origBmp"></param>
        public static void Clear(this Bitmap origBmp)
        {
            origBmp.Clear(ColorTranslator.FromHtml("#00000000"));
        }

        /// <summary>
        /// Fills the bitmap with a uniform color.
        /// </summary>
        /// <param name="origBmp"></param>
        /// <param name="clearColor"></param>
        public static void Clear(this Bitmap origBmp, Color clearColor)
        {
            byte[] ColorBytes = { clearColor.B, clearColor.G, clearColor.R, clearColor.A };
            
            unsafe
            {
                // Get pointer to first pixel of OrigBitmap
                int height = origBmp.Height;
                int width = origBmp.Width;
                BitmapData origBmpData = origBmp.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.WriteOnly, origBmp.PixelFormat);
                byte* ptrOrigFirstPixel = (byte*)origBmpData.Scan0;
                Debug.Assert(Bitmap.GetPixelFormatSize(origBmp.PixelFormat) == 32, "Bitmap to be cleared has no alpha channel!");

                Parallel.For(0, height, y =>
                {
                    byte* curOrigLine = ptrOrigFirstPixel + y * origBmpData.Stride;

                    for (int x = 0; x < width; x++)
                    {
                        ChangePixel(curOrigLine + BytesPerPixel * x, ColorBytes);
                    }
                });

                origBmp.UnlockBits(origBmpData);
            }
        }

        /// <summary>
        /// Copies pixels from a new bitmap to the base bitmap. 
        /// </summary>
        /// <param name="origBmp"></param>
        /// <param name="newBmp"></param>
        /// <param name="pos"></param>
        public static void DrawOn(this Bitmap origBmp, Bitmap newBmp, Point pos, byte alpha = 255)
        {
            origBmp.DrawOn(newBmp, pos, DoDrawThisPixel_DrawNew, alpha);
        }

        /// <summary>
        /// Draws NewBmp to the base bitmap using the selected CustDrawMode.
        /// </summary>
        /// <param name="origBmp"></param>
        /// <param name="newBmp"></param>
        /// <param name="pos"></param>
        /// <param name="colorSelect"></param>
        public static void DrawOn(this Bitmap origBmp, Bitmap newBmp, Point pos, C.CustDrawMode colorSelect)
        {
            var colorFunc = colorFuncDict[colorSelect];
            var doDrawThisPixel = doDrawThisPixelDict[colorSelect];

            if (colorFunc == null)
            {
                if (colorSelect.In(C.CustDrawMode.DefaultOWW, C.CustDrawMode.NoOverwriteOWW))
                {
                    origBmp.DrawOn(newBmp, pos, doDrawThisPixel, C.ALPHA_OWW);
                }
                else
                {
                    origBmp.DrawOn(newBmp, pos, doDrawThisPixel, C.ALPHA_NOOWW);
                }
            }
            else
            {
                origBmp.DrawOn(newBmp, pos, colorFunc, doDrawThisPixel);
            }
        }

        /// <summary>
        /// Draws NewBmp to the base bitmap using the selected CustDrawMode using a mask.
        /// </summary>
        /// <param name="origBmp"></param>
        /// <param name="newBmp"></param>
        /// <param name="maskBmp"></param>
        /// <param name="pos"></param>
        /// <param name="colorSelect"></param>
        public static void DrawOn(this Bitmap origBmp, Bitmap newBmp, Bitmap maskBmp, Point pos, C.CustDrawMode colorSelect)
        {
            var colorFunc = colorFuncDict[colorSelect];
            var doDrawThisPixel = doDrawThisPixelDict[colorSelect];

            if (colorFunc == null)
            {
                origBmp.DrawOn(newBmp, maskBmp, pos, doDrawThisPixel);
            }
            else
            {
                origBmp.DrawOn(newBmp, maskBmp, pos, colorFunc, doDrawThisPixel);
            }
        }

        /// <summary>
        /// Copies pixels from a new bitmap to the base bitmap under condition specified by DoDrawThisPixel. 
        /// <para> All pixels will be drawn with fixed alpha value. </para>
        /// </summary>
        /// <param name="origBmp"></param>
        /// <param name="newBmp"></param>
        /// <param name="pos"></param>
        /// <param name="doDrawThisPixel"></param>
        /// <param name="alpha"></param>
        private static void DrawOn(this Bitmap origBmp, Bitmap newBmp, Point pos, 
                                   Func<byte, byte, bool> doDrawThisPixel, byte alpha)
        {
            if (newBmp == null || doDrawThisPixel == null) return;

            // Get rectangle giving the area that is drawn onto
            Rectangle origBmpRect = new Rectangle(0, 0, origBmp.Width, origBmp.Height);
            Rectangle newBmpRect = new Rectangle(pos, newBmp.Size);
            Rectangle drawRect = Rectangle.Intersect(origBmpRect, newBmpRect);

            unsafe
            {
                // Get pointer to first pixel of OrigBitmap
                BitmapData origBmpData = origBmp.LockBits(origBmpRect, ImageLockMode.ReadWrite, origBmp.PixelFormat);
                byte* ptrOrigFirstPixel = (byte*)origBmpData.Scan0;
                Debug.Assert(Bitmap.GetPixelFormatSize(origBmp.PixelFormat) == 32, "Bitmap drawn onto has no alpha channel!");

                // Get pointer to first pixel of NewBitmap
                BitmapData newBmpData = newBmp.LockBits(new Rectangle(0, 0, newBmp.Width, newBmp.Height), ImageLockMode.ReadOnly, newBmp.PixelFormat);
                byte* ptrNewFirstPixel = (byte*)newBmpData.Scan0;
                Debug.Assert(Bitmap.GetPixelFormatSize(newBmp.PixelFormat) == 32, "Bitmap to drawn has no alpha channel!");

                Parallel.For(0, drawRect.Height, y =>
                {
                    // We start CurOrigLine at pixel (DrawRect.Left, y + DrawRect.Top)!
                    byte* curOrigLine = ptrOrigFirstPixel + ((y + drawRect.Top) * origBmpData.Stride) + drawRect.Left * BytesPerPixel;
                    // We start CurNewList at pixel (DrawRect.Left - NewBmpRect.Left, y + DrawRect.Top - NewBmpRect.Top)!
                    byte* curNewLine = ptrNewFirstPixel + ((y + drawRect.Top - newBmpRect.Top) * newBmpData.Stride) + (drawRect.Left - newBmpRect.Left) * BytesPerPixel;

                    for (int x = 0; x < drawRect.Width * BytesPerPixel; x = x + BytesPerPixel)
                    {
                        if (doDrawThisPixel(curNewLine[x + 3], curOrigLine[x + 3]))
                        {
                            ChangePixel(curOrigLine + x, curNewLine + x, alpha);
                        }
                    }
                });

                origBmp.UnlockBits(origBmpData);
                newBmp.UnlockBits(newBmpData);
            }
        }

        /// <summary>
        /// Copies pixels from a new bitmap to the base bitmap as specified by ColorFunc and DoDrawThisPixel. 
        /// </summary>
        /// <param name="origBmp"></param>
        /// <param name="newBmp"></param>
        /// <param name="pos"></param>
        /// <param name="ColorSelect"></param>
        private static void DrawOn(this Bitmap origBmp, Bitmap newBmp, Point pos, 
                                   Func<int, int, byte[]> colorFunc, Func<byte, byte, bool> doDrawThisPixel)
        {
            if (newBmp == null || colorFunc == null || doDrawThisPixel == null) return;

            // Get rectangle giving the area that is drawn onto
            Rectangle origBmpRect = new Rectangle(0, 0, origBmp.Width, origBmp.Height);
            Rectangle newBmpRect = new Rectangle(pos, newBmp.Size);
            Rectangle drawRect = Rectangle.Intersect(origBmpRect, newBmpRect);

            unsafe
            {
                // Get pointer to first pixel of OrigBitmap
                BitmapData origBmpData = origBmp.LockBits(origBmpRect, ImageLockMode.ReadWrite, origBmp.PixelFormat);
                byte* ptrOrigFirstPixel = (byte*)origBmpData.Scan0;
                Debug.Assert(Bitmap.GetPixelFormatSize(origBmp.PixelFormat) == 32, "Bitmap drawn onto has no alpha channel!");

                // Get pointer to first pixel of NewBitmap
                BitmapData newBmpData = newBmp.LockBits(new Rectangle(0, 0, newBmp.Width, newBmp.Height), ImageLockMode.ReadOnly, newBmp.PixelFormat);
                byte* ptrNewFirstPixel = (byte*)newBmpData.Scan0;
                Debug.Assert(Bitmap.GetPixelFormatSize(newBmp.PixelFormat) == 32, "Bitmap to drawn has no alpha channel!");

                // Copy the pixels
                Parallel.For(0, drawRect.Height, y =>
                {
                    // We start CurOrigLine at pixel (DrawRect.Left, y + DrawRect.Top)!
                    byte* curOrigLine = ptrOrigFirstPixel + ((y + drawRect.Top) * origBmpData.Stride) + drawRect.Left * BytesPerPixel;
                    // We start CurNewList at pixel (DrawRect.Left - NewBmpRect.Left, y + DrawRect.Top - NewBmpRect.Top)!
                    byte* curNewLine = ptrNewFirstPixel + ((y + drawRect.Top - newBmpRect.Top) * newBmpData.Stride) + (drawRect.Left - newBmpRect.Left) * BytesPerPixel;

                    for (int x = 0; x < drawRect.Width; x++)
                    {
                        if (doDrawThisPixel(curNewLine[x * BytesPerPixel + 3], curOrigLine[x * BytesPerPixel + 3]))
                        {
                            ChangePixel(curOrigLine + x * BytesPerPixel, colorFunc(x, y));
                        }
                    }
                });

                origBmp.UnlockBits(origBmpData);
                newBmp.UnlockBits(newBmpData);
            }
        }

        /// <summary>
        /// Copies pixels from a new bitmap to the base bitmap under condition specified by DoDrawThisPixel, using a mask.
        /// </summary>
        /// <param name="origBmp"></param>
        /// <param name="newBmp"></param>
        /// <param name="maskBmp"></param>
        /// <param name="pos"></param>
        /// <param name="doDrawThisPixel"></param>
        private static void DrawOn(this Bitmap origBmp, Bitmap newBmp, Bitmap maskBmp, Point pos, 
                                   Func<byte, byte, bool> doDrawThisPixel)
        {
            if (newBmp == null || maskBmp == null || doDrawThisPixel == null) return;

            // Get rectangle giving the area that is drawn onto
            Rectangle origBmpRect = new Rectangle(0, 0, origBmp.Width, origBmp.Height);
            Rectangle newBmpRect = new Rectangle(pos, newBmp.Size);
            Rectangle drawRect = Rectangle.Intersect(origBmpRect, newBmpRect);
            Debug.Assert(origBmp.Size.Equals(maskBmp.Size), "Bitmap mask has different size than target bitmap.");

            unsafe
            {
                // Get pointer to first pixel of OrigBitmap
                BitmapData origBmpData = origBmp.LockBits(origBmpRect, ImageLockMode.ReadWrite, origBmp.PixelFormat);
                byte* ptrOrigFirstPixel = (byte*)origBmpData.Scan0;
                Debug.Assert(Bitmap.GetPixelFormatSize(origBmp.PixelFormat) == 32, "Bitmap drawn onto has no alpha channel!");

                // Get pointer to first pixel of NewBitmap
                BitmapData newBmpData = newBmp.LockBits(new Rectangle(0, 0, newBmp.Width, newBmp.Height), ImageLockMode.ReadOnly, newBmp.PixelFormat);
                byte* ptrNewFirstPixel = (byte*)newBmpData.Scan0;
                Debug.Assert(Bitmap.GetPixelFormatSize(newBmp.PixelFormat) == 32, "Bitmap to drawn has no alpha channel!");

                // Get pointer to first pixel of MaskBitmap
                BitmapData maskBmpData = maskBmp.LockBits(origBmpRect, ImageLockMode.ReadOnly, maskBmp.PixelFormat);
                byte* ptrMaskFirstPixel = (byte*)maskBmpData.Scan0;
                Debug.Assert(Bitmap.GetPixelFormatSize(maskBmp.PixelFormat) == 32, "Mask bitmap has no alpha channel!");


                Parallel.For(0, drawRect.Height, y =>
                {
                    // We start CurOrigLine and CurMaskLine at pixel (DrawRect.Left, y + DrawRect.Top)!
                    byte* curOrigLine = ptrOrigFirstPixel + ((y + drawRect.Top) * origBmpData.Stride) + drawRect.Left * BytesPerPixel;
                    byte* curMaskLine = ptrMaskFirstPixel + ((y + drawRect.Top) * maskBmpData.Stride) + drawRect.Left * BytesPerPixel;
                    // We start CurNewList at pixel (DrawRect.Left - NewBmpRect.Left, y + DrawRect.Top - NewBmpRect.Top)!
                    byte* curNewLine = ptrNewFirstPixel + ((y + drawRect.Top - newBmpRect.Top) * newBmpData.Stride) + (drawRect.Left - newBmpRect.Left) * BytesPerPixel;

                    for (int x = 0; x < drawRect.Width * BytesPerPixel; x = x + BytesPerPixel)
                    {
                        byte curNewAlpha = curNewLine[x + 3];
                        byte curMaskAlpha = curMaskLine[x + 3];

                        if (doDrawThisPixel(curNewAlpha, curMaskAlpha))
                        {
                            ChangePixel(curOrigLine + x, curNewLine + x);
                        }
                    }
                });

                origBmp.UnlockBits(origBmpData);
                newBmp.UnlockBits(newBmpData);
                maskBmp.UnlockBits(maskBmpData);
            }
        }

        /// <summary>
        /// Copies pixels from a new bitmap to the base bitmap as specified by ColorFunc and DoDrawThisPixel, using a mask. 
        /// </summary>
        /// <param name="origBmp"></param>
        /// <param name="newBmp"></param>
        /// <param name="maskBmp"></param>
        /// <param name="pos"></param>
        /// <param name="colorFunc"></param>
        /// <param name="doDrawThisPixel"></param>
        private static void DrawOn(this Bitmap origBmp, Bitmap newBmp, Bitmap maskBmp, Point pos, 
                                   Func<int, int, byte[]> colorFunc, Func<byte, byte, bool> doDrawThisPixel)
        {
            if (newBmp == null || maskBmp == null || colorFunc == null || doDrawThisPixel == null) return;

            // Get rectangle giving the area that is drawn onto
            Rectangle origBmpRect = new Rectangle(0, 0, origBmp.Width, origBmp.Height);
            Rectangle newBmpRect = new Rectangle(pos, newBmp.Size);
            Rectangle drawRect = Rectangle.Intersect(origBmpRect, newBmpRect);
            Debug.Assert(origBmp.Size.Equals(maskBmp.Size), "Bitmap mask has different size than target bitmap.");

            unsafe
            {
                // Get pointer to first pixel of OrigBitmap
                BitmapData origBmpData = origBmp.LockBits(origBmpRect, ImageLockMode.ReadWrite, origBmp.PixelFormat);
                byte* ptrOrigFirstPixel = (byte*)origBmpData.Scan0;
                Debug.Assert(Bitmap.GetPixelFormatSize(origBmp.PixelFormat) == 32, "Bitmap drawn onto has no alpha channel!");

                // Get pointer to first pixel of NewBitmap
                BitmapData newBmpData = newBmp.LockBits(new Rectangle(0, 0, newBmp.Width, newBmp.Height), ImageLockMode.ReadOnly, newBmp.PixelFormat);
                byte* ptrNewFirstPixel = (byte*)newBmpData.Scan0;
                Debug.Assert(Bitmap.GetPixelFormatSize(newBmp.PixelFormat) == 32, "Bitmap to drawn has no alpha channel!");

                // Get pointer to first pixel of MaskBitmap
                BitmapData maskBmpData = maskBmp.LockBits(origBmpRect, ImageLockMode.ReadOnly, maskBmp.PixelFormat);
                byte* ptrMaskFirstPixel = (byte*)maskBmpData.Scan0;
                Debug.Assert(Bitmap.GetPixelFormatSize(maskBmp.PixelFormat) == 32, "Mask bitmap has no alpha channel!");


                Parallel.For(0, drawRect.Height, y =>
                {
                    // We start CurOrigLine and CurMaskLine at pixel (DrawRect.Left, y + DrawRect.Top)!
                    byte* curOrigLine = ptrOrigFirstPixel + ((y + drawRect.Top) * origBmpData.Stride) + drawRect.Left * BytesPerPixel;
                    byte* curMaskLine = ptrMaskFirstPixel + ((y + drawRect.Top) * maskBmpData.Stride) + drawRect.Left * BytesPerPixel;
                    // We start CurNewList at pixel (DrawRect.Left - NewBmpRect.Left, y + DrawRect.Top - NewBmpRect.Top)!
                    byte* curNewLine = ptrNewFirstPixel + ((y + drawRect.Top - newBmpRect.Top) * newBmpData.Stride) + (drawRect.Left - newBmpRect.Left) * BytesPerPixel;

                    for (int x = 0; x < drawRect.Width; x++)
                    {
                        byte curNewAlpha = curNewLine[x * BytesPerPixel + 3];
                        byte curMaskAlpha = curMaskLine[x * BytesPerPixel + 3];

                        if (doDrawThisPixel(curNewAlpha, curMaskAlpha))
                        {
                            ChangePixel(curOrigLine + x * BytesPerPixel, colorFunc(x, y));
                        }
                    }
                });

                origBmp.UnlockBits(origBmpData);
                newBmp.UnlockBits(newBmpData);
                maskBmp.UnlockBits(maskBmpData);
            }
        }

        /// <summary>
        /// Copies pixels from a new bitmap to the base bitmap using swapped alpha blending.
        /// <para> All pixels of the original bitmap have alpha 128, all pixels in the result have alpha 255.</para>
        /// </summary>
        /// <param name="origBmp"></param>
        /// <param name="newBmp"></param>
        /// <param name="pos"></param>
        public static void DrawOnWithAlpha(this Bitmap origBmp, Bitmap newBmp, Point pos)
        {
            if (newBmp == null) return;

            // Get rectangle giving the area that is drawn onto
            Rectangle origBmpRect = new Rectangle(0, 0, origBmp.Width, origBmp.Height);
            Rectangle newBmpRect = new Rectangle(pos, newBmp.Size);
            Rectangle drawRect = Rectangle.Intersect(origBmpRect, newBmpRect);

            unsafe
            {
                // Get pointer to first pixel of OrigBitmap
                BitmapData origBmpData = origBmp.LockBits(origBmpRect, ImageLockMode.ReadWrite, origBmp.PixelFormat);
                byte* ptrOrigFirstPixel = (byte*)origBmpData.Scan0;
                Debug.Assert(Bitmap.GetPixelFormatSize(origBmp.PixelFormat) == 32, "Bitmap drawn onto has no alpha channel!");

                // Get pointer to first pixel of NewBitmap
                BitmapData newBmpData = newBmp.LockBits(new Rectangle(0, 0, newBmp.Width, newBmp.Height), ImageLockMode.ReadOnly, newBmp.PixelFormat);
                byte* ptrNewFirstPixel = (byte*)newBmpData.Scan0;
                Debug.Assert(Bitmap.GetPixelFormatSize(newBmp.PixelFormat) == 32, "Bitmap to drawn has no alpha channel!");

                Parallel.For(0, drawRect.Height, y =>
                {
                    // We start CurOrigLine at pixel (DrawRect.Left, y + DrawRect.Top)!
                    byte* curOrigLine = ptrOrigFirstPixel + ((y + drawRect.Top) * origBmpData.Stride) + drawRect.Left * BytesPerPixel;
                    // We start CurNewList at pixel (DrawRect.Left - NewBmpRect.Left, y + DrawRect.Top - NewBmpRect.Top)!
                    byte* curNewLine = ptrNewFirstPixel + ((y + drawRect.Top - newBmpRect.Top) * newBmpData.Stride) + (drawRect.Left - newBmpRect.Left) * BytesPerPixel;

                    for (int x = 0; x < drawRect.Width * BytesPerPixel; x = x + BytesPerPixel)
                    {
                        if (curNewLine[x + 3] > 0)
                        {
                            ChangePixelBlend(curOrigLine + x, curNewLine + x);
                        }
                    }
                });

                origBmp.UnlockBits(origBmpData);
                newBmp.UnlockBits(newBmpData);
            }
        }

        /// <summary>
        /// Blends a sprite image with a given color.
        /// </summary>
        /// <param name="origBmp"></param>
        /// <param name="blendColor"></param>
        /// <returns></returns>
        public static Bitmap ApplyThemeColor(this Bitmap origBmp, Color blendColor)
        {
            byte[] blendColorBytes = new byte[] { blendColor.B, blendColor.G, blendColor.R, 255 };

            Rectangle origBmpRect = new Rectangle(0, 0, origBmp.Width, origBmp.Height);
            Bitmap newBmp = new Bitmap(origBmpRect.Width, origBmpRect.Height);

            unsafe
            {
                // Get pointer to first pixel of OrigBitmap
                BitmapData origBmpData = origBmp.LockBits(origBmpRect, ImageLockMode.ReadOnly, origBmp.PixelFormat);
                byte* ptrOrigFirstPixel = (byte*)origBmpData.Scan0;
                Debug.Assert(Bitmap.GetPixelFormatSize(origBmp.PixelFormat) == 32, "Bitmap drawn onto has no alpha channel!");

                // Get pointer to first pixel of NewBitmap
                BitmapData newBmpData = newBmp.LockBits(origBmpRect, ImageLockMode.WriteOnly, newBmp.PixelFormat);
                byte* ptrNewFirstPixel = (byte*)newBmpData.Scan0;
                Debug.Assert(Bitmap.GetPixelFormatSize(newBmp.PixelFormat) == 32, "Bitmap to drawn has no alpha channel!");

                // Copy the pixels
                Parallel.For(0, origBmpRect.Height, y =>
                {
                    // We start curOrigLine and curNewLine at pixel (0, y)
                    byte* curOrigLine = ptrOrigFirstPixel + y * origBmpData.Stride;
                    byte* curNewLine = ptrNewFirstPixel + y * newBmpData.Stride;

                    for (int x = 0; x < origBmpRect.Width; x++)
                    {
                        if (curOrigLine[x * BytesPerPixel + 3] > 63)
                        {
                            ChangePixel(curNewLine + x * BytesPerPixel, curOrigLine + x * BytesPerPixel, blendColorBytes);
                        }
                    }
                });

                origBmp.UnlockBits(origBmpData);
                newBmp.UnlockBits(newBmpData);
            }

            return newBmp;
        }


        /// <summary>
        /// Blends a sprite image with a given color, keeping all pixels with a specific alpha value.
        /// </summary>
        /// <param name="origBmp"></param>
        /// <param name="blendColor"></param>
        /// <param name="ignoreColor"></param>
        /// <returns></returns>
        public static Bitmap ApplyThemeColor(this Bitmap origBmp, Color blendColor, byte ignoreAlpha)
        {
            byte[] blendColorBytes = new byte[] { blendColor.B, blendColor.G, blendColor.R, 255 };

            Rectangle origBmpRect = new Rectangle(0, 0, origBmp.Width, origBmp.Height);
            Bitmap newBmp = new Bitmap(origBmpRect.Width, origBmpRect.Height);

            unsafe
            {
                // Get pointer to first pixel of OrigBitmap
                BitmapData origBmpData = origBmp.LockBits(origBmpRect, ImageLockMode.ReadOnly, origBmp.PixelFormat);
                byte* ptrOrigFirstPixel = (byte*)origBmpData.Scan0;
                Debug.Assert(Bitmap.GetPixelFormatSize(origBmp.PixelFormat) == 32, "Bitmap drawn onto has no alpha channel!");

                // Get pointer to first pixel of NewBitmap
                BitmapData newBmpData = newBmp.LockBits(origBmpRect, ImageLockMode.WriteOnly, newBmp.PixelFormat);
                byte* ptrNewFirstPixel = (byte*)newBmpData.Scan0;
                Debug.Assert(Bitmap.GetPixelFormatSize(newBmp.PixelFormat) == 32, "Bitmap to drawn has no alpha channel!");

                // Copy the pixels
                Parallel.For(0, origBmpRect.Height, y =>
                {
                    // We start curOrigLine and curNewLine at pixel (0, y)
                    byte* curOrigLine = ptrOrigFirstPixel + y * origBmpData.Stride;
                    byte* curNewLine = ptrNewFirstPixel + y * newBmpData.Stride;

                    for (int x = 0; x < origBmpRect.Width; x++)
                    {
                        if (curOrigLine[x * BytesPerPixel + 3] == ignoreAlpha)
                        {
                            ChangePixel(curNewLine + x * BytesPerPixel, curOrigLine + x * BytesPerPixel);
                        }
                        else if (curOrigLine[x * BytesPerPixel + 3] > 63)
                        {
                            ChangePixel(curNewLine + x * BytesPerPixel, curOrigLine + x * BytesPerPixel, blendColorBytes);
                        }
                    }
                });

                origBmp.UnlockBits(origBmpData);
                newBmp.UnlockBits(newBmpData);
            }

            return newBmp;
        }


        /// <summary>
        /// Zooms a bitmap.
        /// </summary>
        /// <param name="origBmp"></param>
        /// <param name="zoomFactor"></param>
        /// <returns></returns>
        public static Bitmap Zoom(this Bitmap origBmp, int zoomFactor)
        {
            int newWidth = (zoomFactor < 0) ? origBmp.Width / (Math.Abs(zoomFactor) + 1) : origBmp.Width * (zoomFactor + 1);
            int newHeight = (zoomFactor < 0) ? origBmp.Height / (Math.Abs(zoomFactor) + 1) : origBmp.Height * (zoomFactor + 1);

            return origBmp.Zoom(zoomFactor, new Size(newWidth, newHeight));
        }


        /// <summary>
        /// Zooms a bitmap and crops it to a smaller size.
        /// </summary>
        /// <param name="origBmp"></param>
        /// <param name="zoomFactor"></param>
        /// <param name="newBmpSize"></param>
        /// <returns></returns>
        public static Bitmap Zoom(this Bitmap origBmp, int zoomFactor, Size newBmpSize)
        {
            Bitmap newBmp = new Bitmap(newBmpSize.Width, newBmpSize.Height);

            unsafe
            {
                // Get pointer to first pixel of OldBitmap
                BitmapData origBmpData = origBmp.LockBits(new Rectangle(0, 0, origBmp.Width, origBmp.Height), ImageLockMode.ReadOnly, origBmp.PixelFormat);
                byte* ptrOrigFirstPixel = (byte*)origBmpData.Scan0;
                Debug.Assert(Bitmap.GetPixelFormatSize(origBmp.PixelFormat) == 32, "Zoomed Bitmap has no alpha channel!");

                // Get pointer to first pixel of NewBitmap
                BitmapData newBmpData = newBmp.LockBits(new Rectangle(0, 0, newBmp.Width, newBmp.Height), ImageLockMode.WriteOnly, newBmp.PixelFormat);
                byte* ptrNewFirstPixel = (byte*)newBmpData.Scan0;

                if (zoomFactor < 0)
                {
                    int newBmpWidth = newBmp.Width;
                    int newBmpHeight = newBmp.Height;

                    zoomFactor = Math.Abs(zoomFactor) + 1;

                    // Copy the pixels
                    Parallel.For(0, newBmpHeight, y =>
                    {
                        byte* curNewLine = ptrNewFirstPixel + y * newBmpData.Stride;
                        byte* curOrigLine = ptrOrigFirstPixel + y * zoomFactor * origBmpData.Stride;

                        for (int x = 0; x < newBmpWidth; x++)
                        {
                            ChangePixel(curNewLine + x * BytesPerPixel, curOrigLine + x * BytesPerPixel * zoomFactor);
                        }
                    });
                }
                else
                {
                    int origBmpWidth = origBmp.Width;
                    int origBmpHeight = origBmp.Height;

                    zoomFactor++;

                    // Copy the pixels
                    Parallel.For(0, origBmpHeight, y =>
                    {
                        byte* curOrigLine = ptrOrigFirstPixel + y * origBmpData.Stride;

                        for (int i = 0; i < zoomFactor; i++)
                        {
                            byte* curNewLine = ptrNewFirstPixel + (y * zoomFactor + i) * newBmpData.Stride;

                            for (int x = 0; x < origBmpWidth; x++)
                            {
                                for (int j = 0; j < zoomFactor; j++)
                                {
                                    ChangePixel(curNewLine + (zoomFactor * x + j) * BytesPerPixel, curOrigLine + x * BytesPerPixel);
                                }
                            }
                        }
                    });
                }

                origBmp.UnlockBits(origBmpData);
                newBmp.UnlockBits(newBmpData);
            }

            return newBmp;
        }

        /// <summary>
        /// Draws a list of filled rectangles on a bitmap.
        /// </summary>
        /// <param name="origBmp"></param>
        /// <param name="rectList"></param>
        /// <param name="rectColor"></param>
        public static void DrawOnFilledRectangles(this Bitmap origBmp, List<Rectangle> rectList, Color rectColor)
        {
            if (rectList == null) return;
            
            using (Graphics g = Graphics.FromImage(origBmp))
            {
                g.CompositingMode = System.Drawing.Drawing2D.CompositingMode.SourceOver;
                using (Brush b = new SolidBrush(rectColor))
                {
                    rectList.ForEach(rect => g.FillRectangle(b, rect));
                }
            }
        }

        /// <summary>
        /// Draws a list of rectangles on a bitmap.
        /// </summary>
        /// <param name="origBmp"></param>
        /// <param name="rectList"></param>
        /// <param name="rectColor"></param>
        public static void DrawOnRectangles(this Bitmap origBmp, List<Rectangle> rectList, Color rectColor)
        {
            if (rectList == null) return;
            
            using (Graphics g = Graphics.FromImage(origBmp))
            {
                using (Pen p = new Pen(rectColor))
                {
                    rectList.ForEach(rect => g.DrawRectangle(p, rect));
                }
            }
        }

        /// <summary>
        /// Draws a dotted rectangle on a bitmap.
        /// </summary>
        /// <param name="origBmp"></param>
        /// <param name="rect"></param>
        public static void DrawOnDottedRectangle(this Bitmap origBmp, Rectangle rect)
        {
            Rectangle origBmpRect = new Rectangle(0, 0, origBmp.Width, origBmp.Height);

            // Shrink rectangle to Bitmap size
            rect.Intersect(origBmpRect);

            unsafe
            {
                // Get pointer to first pixel of OrigBitmap
                BitmapData origBmpData = origBmp.LockBits(origBmpRect, ImageLockMode.WriteOnly, origBmp.PixelFormat);
                byte* ptrOrigFirstPixel = (byte*)origBmpData.Scan0;
                Debug.Assert(Bitmap.GetPixelFormatSize(origBmp.PixelFormat) == 32, "Bitmap drawn onto has no alpha channel!");
      
                // Top and bottom
                for (int i = 0; i < 2; i++)
                {
                    int posY = (i == 0) ? rect.Top : rect.Bottom - 1;
                    byte* origLine = ptrOrigFirstPixel + posY * origBmpData.Stride;

                    for (int x = rect.Left; x < rect.Right; x++)
                    {
                        if ((posY + x) % 6 < 3)
                        {
                            ChangePixel(origLine + BytesPerPixel * x, COLOR_RECTANGLE_LIGHT);
                        }
                        else
                        {
                            ChangePixel(origLine + BytesPerPixel * x, COLOR_RECTANGLE_DARK);
                        }
                    }
                }

                // Right and left side
                for (int y = rect.Top; y < rect.Bottom; y++)
                {
                    byte* origLine = ptrOrigFirstPixel + y * origBmpData.Stride;

                    for (int i = 0; i < 2; i++)
                    {
                        int posX = (i == 0) ? rect.Left : rect.Right - 1;
                        
                        if ((y + posX) % 6 < 3)
                        {
                            ChangePixel(origLine + BytesPerPixel * posX, COLOR_RECTANGLE_LIGHT);
                        }
                        else
                        {
                            ChangePixel(origLine + BytesPerPixel * posX, COLOR_RECTANGLE_DARK);
                        }
                    }
                }

                origBmp.UnlockBits(origBmpData);
            }
        }

        /// <summary>
        /// Paves a rectangle with copies of the original bitmap and returns the new image.
        /// </summary>
        /// <param name="origBmp"></param>
        /// <param name="rect"></param>
        /// <returns></returns>
        public static Bitmap PaveArea(this Bitmap origBmp, Rectangle rect)
        {
            Bitmap newBmp = new Bitmap(rect.Width, rect.Height);
            int origWidth = origBmp.Width;
            int origHeight = origBmp.Height;

            int startX = rect.Left / origWidth - ((rect.Left < 0) ? 1 : 0);
            int endX = (rect.Right - 1) / origWidth - ((rect.Right - 1 < 0) ? 1 : 0);
            int startY = rect.Top / origHeight - ((rect.Top < 0) ? 1 : 0);
            int endY = (rect.Bottom - 1) / origHeight - ((rect.Bottom - 1 < 0) ? 1 : 0);

            for (int x = startX; x <= endX; x++)
            {
                for (int y = startY; y <= endY; y++)
                {
                    Point position = new Point(x * origWidth, y * origHeight);
                    newBmp.DrawOn(origBmp, position);
                }
            }

            return newBmp;
        }

        /// <summary>
        /// Writes a string at a specified position and color on the bitmap.
        /// </summary>
        /// <param name="origBmp"></param>
        /// <param name="text"></param>
        /// <param name="position"></param>
        /// <param name="textColor"></param>
        /// <param name="fontSize"></param>
        /// <param name="alignment"></param>
        public static void WriteText(this Bitmap origBmp, string text, Point position, Color textColor, int fontSize, ContentAlignment alignment = ContentAlignment.MiddleCenter)
        {
            // Reposition the text correctly according to its size.
            Font textFont = new Font("Tahoma", fontSize);
            Size textSize = System.Windows.Forms.TextRenderer.MeasureText(text, textFont);
            int textHeight = textSize.Height * 5 / 4;
            Point topLeftCorner = AlignText(position, textSize.Width, textHeight, alignment);

            using (Graphics g = Graphics.FromImage(origBmp))
            {
                using (Brush b = new SolidBrush(textColor))
                {
                    g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                    g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                    g.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.HighQuality;
                    g.DrawString(text, textFont, b, topLeftCorner);
                }
            }
        }

        public static void WriteTextEdged(this Bitmap origBmp, string text, Point position, Color textColor, int fontSize, ContentAlignment alignment = ContentAlignment.MiddleCenter)
        {
            // Reposition the text correctly according to its size.
            Font textFont = new Font("Tahoma", fontSize);
            Size textSize = System.Windows.Forms.TextRenderer.MeasureText(text, textFont);
            Point topLeftCorner = AlignText(position, textSize.Width, textSize.Height, alignment);

            using (Graphics g = Graphics.FromImage(origBmp))
            {
                using (Brush b = new SolidBrush(textColor))
                {
                    g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.None;
                    g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.Low;
                    g.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.None;
                    g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.SingleBitPerPixel;
                    g.DrawString(text, textFont, b, topLeftCorner);
                }
            }
        }

        /// <summary>
        /// Computes the top left corner of text of given size and alignment.
        /// </summary>
        /// <param name="position"></param>
        /// <param name="textWidth"></param>
        /// <param name="textHeight"></param>
        /// <param name="alignment"></param>
        /// <returns></returns>
        private static Point AlignText(Point position, int textWidth, int textHeight, ContentAlignment alignment)
        {
            int posX = position.X;
            int posY = position.Y;

            if (alignment.In(ContentAlignment.BottomCenter, ContentAlignment.MiddleCenter, ContentAlignment.TopCenter))
            {
                posX -= textWidth / 2;
            }
            else if (alignment.In(ContentAlignment.BottomRight, ContentAlignment.MiddleRight, ContentAlignment.TopRight))
            {
                posX -= textWidth;
            }

            if (alignment.In(ContentAlignment.MiddleCenter, ContentAlignment.MiddleLeft, ContentAlignment.MiddleRight))
            {
                posY -= textHeight / 2;
            }
            else if (alignment.In(ContentAlignment.BottomCenter, ContentAlignment.BottomLeft, ContentAlignment.BottomRight))
            {
                posY -= textHeight;
            }

            return new Point(posX, posY);
        }

        [Obsolete]
        /// <summary>
        /// Returns a recolored OWW according to the OWW color of the given style.
        /// </summary>
        /// <param name="pieceImage"></param>
        /// <param name="owwStyle"></param>
        /// <returns></returns>
        public static Bitmap RecolorOWW(Bitmap pieceImage, Style owwStyle)
        {
            Color owwColor = owwStyle?.GetColor(C.StyleColor.ONE_WAY_WALL) ?? Color.Linen;
            byte[] owwColorbytes = new byte[] { owwColor.B, owwColor.G, owwColor.R, 255 };
            Func<byte, byte, bool> owwDrawType = ((b1, b2) => (b1 == 255));
            BmpModify.SetCustomDrawMode((x, y) => owwColorbytes, owwDrawType);
            Bitmap newBmp = new Bitmap(pieceImage.Width, pieceImage.Height);
            newBmp.DrawOn(pieceImage, new Point(0, 0), C.CustDrawMode.Custom);
            return newBmp;
        }

        /// <summary>
        /// Gets the smallest rectangle around all non-transparent pixels of the bitmap.
        /// <para> Throws an ArgumentException is the bitmap is completely transparent.</para>
        /// </summary>
        /// <param name="origBmp"></param>
        /// <returns></returns>
        public static Rectangle GetCropTransparentRectangle(this Bitmap origBmp)
        {
            Rectangle cropRect = new Rectangle();

            unsafe
            {
                // Get pointer to first pixel of OrigBitmap
                Rectangle origBmpRect = new Rectangle(0, 0, origBmp.Width, origBmp.Height);
                BitmapData origBmpData = origBmp.LockBits(origBmpRect, ImageLockMode.WriteOnly, origBmp.PixelFormat);
                byte* ptrOrigFirstPixel = (byte*)origBmpData.Scan0;
                Debug.Assert(Bitmap.GetPixelFormatSize(origBmp.PixelFormat) == 32, "Bitmap to crop has no alpha channel!");

                // Compute top edge of visible image
                int top = 0;
                do
                {
                    byte* origLine = ptrOrigFirstPixel + top * origBmpData.Stride;
                    for (int x = 0; x < origBmpRect.Width; x++)
                    {
                        if (origLine[BytesPerPixel * x + 3] != 0)
                        {
                            cropRect.Y = top;
                            goto END_TOP;
                        }
                    }
                } while (++top != origBmpRect.Height);
                // We can only get here if the bitmap is completely empty. 
                // As we do not want to create empty bitmaps, we throw an exception!
                // Note that we don't need this check for the other coordinates, because the exception was already thrown here.
                throw new ArgumentException("Completely empty bitmap cropped of transparent pixels.");
                END_TOP:

                // Compute bottom edge of visible image
                int bottom = origBmpRect.Height;
                do
                {
                    byte* origLine = ptrOrigFirstPixel + (bottom - 1) * origBmpData.Stride;
                    for (int x = 0; x < origBmpRect.Width; x++)
                    {
                        if (origLine[BytesPerPixel * x + 3] != 0)
                        {
                            cropRect.Height = bottom - cropRect.Y;
                            goto END_BOTTOM;
                        }
                    }
                } while (--bottom != 0);
                END_BOTTOM:

                // Compute left edge of visible image
                int left = 0;
                do
                {
                    for (int y = cropRect.Top; y < cropRect.Bottom; y++)
                    {
                        byte* origPixel = ptrOrigFirstPixel + y * origBmpData.Stride + BytesPerPixel * left;
                        if (origPixel[3] != 0)
                        {
                            cropRect.X = left;
                            goto END_LEFT;
                        }
                    }
                } while (++left != origBmpRect.Width);
                END_LEFT:

                // Compute right edge of visible image
                int right = origBmpRect.Width;
                do
                {
                    for (int y = cropRect.Top; y < cropRect.Bottom; y++)
                    {
                        byte* origPixel = ptrOrigFirstPixel + y * origBmpData.Stride + BytesPerPixel * (right - 1);
                        if (origPixel[3] != 0)
                        {
                            cropRect.Width = right - cropRect.X;
                            goto END_RIGHT;
                        }
                    }
                } while (--right != 0);
                END_RIGHT:

                origBmp.UnlockBits(origBmpData);
            }

            return cropRect;
        }

    }
}
