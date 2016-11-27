using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Drawing.Imaging;
using System.Diagnostics;

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
         *     - Clear(this Bitmap OrigBmp)
         *     - Clear(this Bitmap OrigBmp, Color ClearColor)
         *     - DrawOn(this Bitmap OrigBmp, Bitmap NewBmp, Point Pos)
         *     - DrawOn(this Bitmap OrigBmp, Bitmap NewBmp, Point Pos, C.CustDrawMode ColorSelect)
         *     - DrawOn(this Bitmap OrigBmp, Bitmap NewBmp, Bitmap MaskBmp, Point Pos, C.CustDrawMode ColorSelect)
         *     - Zoom(this Bitmap OrigBmp, int ZoomFactor)
         *     - Zoom(this Bitmap OrigBmp, int ZoomFactor, Size NewBmpSize)
         *     - DrawOnRectangles(this Bitmap OrigBmp, List<Rectangle> RectList, Color RectColor)
         *     - DrawOnFilledRectangles(this Bitmap OrigBmp, List<Rectangle> RectList, Color RectColor)
         *     - DrawOnDottedRectangles(this Bitmap OrigBmp, Rectangle Rect)
         * -------------------------------------------------------- */

        /// <summary>
        /// Initializes the dictionary ColorFuncDict.
        /// </summary>
        static BmpModify()
        {
            ColorFuncDict = new Dictionary<C.CustDrawMode, Func<int, int, byte[]>>();
            ColorFuncDict.Add(C.CustDrawMode.Default, null);
            ColorFuncDict.Add(C.CustDrawMode.DefaultOWW, null);
            ColorFuncDict.Add(C.CustDrawMode.Erase, ColorFunc_Erase);
            ColorFuncDict.Add(C.CustDrawMode.NoOverwrite, null);
            ColorFuncDict.Add(C.CustDrawMode.NoOverwriteOWW, null);
            ColorFuncDict.Add(C.CustDrawMode.OnlyAtMask, null);
            ColorFuncDict.Add(C.CustDrawMode.OnlyAtOWW, null);
            ColorFuncDict.Add(C.CustDrawMode.ClearPhysics, ColorFunc_ClearPhysics);
            ColorFuncDict.Add(C.CustDrawMode.ClearPhysicsOWW, ColorFunc_ClearPhysicsOWW);
            ColorFuncDict.Add(C.CustDrawMode.ClearPhysicsSteel, ColorFunc_ClearPhysicsSteel);
            ColorFuncDict.Add(C.CustDrawMode.ClearPhysicsNoOverwrite, ColorFunc_ClearPhysics);
            ColorFuncDict.Add(C.CustDrawMode.ClearPhysicsNoOverwriteOWW, ColorFunc_ClearPhysicsOWW);
            ColorFuncDict.Add(C.CustDrawMode.ClearPhysicsSteelNoOverwrite, ColorFunc_ClearPhysicsSteel);

            DoDrawThisPixelDict = new Dictionary<C.CustDrawMode, Func<byte, byte, bool>>();
            DoDrawThisPixelDict.Add(C.CustDrawMode.Default, DoDrawThisPixel_DrawNew);
            DoDrawThisPixelDict.Add(C.CustDrawMode.DefaultOWW, DoDrawThisPixel_DrawNew);
            DoDrawThisPixelDict.Add(C.CustDrawMode.Erase, DoDrawThisPixel_DrawNew);
            DoDrawThisPixelDict.Add(C.CustDrawMode.NoOverwrite, DoDrawThisPixel_NotAtMask);
            DoDrawThisPixelDict.Add(C.CustDrawMode.NoOverwriteOWW, DoDrawThisPixel_NotAtMask);
            DoDrawThisPixelDict.Add(C.CustDrawMode.OnlyAtMask, DoDrawThisPixel_OnlyAtMask);
            DoDrawThisPixelDict.Add(C.CustDrawMode.OnlyAtOWW, DoDrawThisPixel_OnlyAtOWW);
            DoDrawThisPixelDict.Add(C.CustDrawMode.ClearPhysics, DoDrawThisPixel_DrawNew);
            DoDrawThisPixelDict.Add(C.CustDrawMode.ClearPhysicsOWW, DoDrawThisPixel_DrawNew);
            DoDrawThisPixelDict.Add(C.CustDrawMode.ClearPhysicsSteel, DoDrawThisPixel_DrawNew);
            DoDrawThisPixelDict.Add(C.CustDrawMode.ClearPhysicsNoOverwrite, DoDrawThisPixel_NotAtMask);
            DoDrawThisPixelDict.Add(C.CustDrawMode.ClearPhysicsNoOverwriteOWW, DoDrawThisPixel_NotAtMask);
            DoDrawThisPixelDict.Add(C.CustDrawMode.ClearPhysicsSteelNoOverwrite, DoDrawThisPixel_NotAtMask);
        }

        private static Dictionary<C.CustDrawMode, Func<int, int, byte[]>> ColorFuncDict;
        private static Dictionary<C.CustDrawMode, Func<byte, byte, bool>> DoDrawThisPixelDict;

        private static readonly byte[] ColorErase = { 0, 0, 0, 0 };
        private static readonly byte[] ColorClearPhysicsLight = { 200, 200, 200, 254 };
        private static readonly byte[] ColorClearPhysicsDark = { 170, 170, 170, 254 };
        private static readonly byte[] ColorClearPhysicsLightOWW = { 200, 200, 200, 255 };
        private static readonly byte[] ColorClearPhysicsDarkOWW = { 170, 170, 170, 255 };
        private static readonly byte[] ColorClearPhysicsSteelLight = { 80, 80, 80, 254 };
        private static readonly byte[] ColorClearPhysicsSteelDark = { 50, 50, 50, 254 };

        private static byte[] ColorFunc_Erase(int PosX, int PosY)
        {
            return ColorErase;
        }

        private static byte[] ColorFunc_ClearPhysics(int PosX, int PosY)
        {
            if ((PosX + PosY) % 2 == 0)
            {
                return ColorClearPhysicsLight;
            }
            else
            {
                return ColorClearPhysicsDark;
            }
        }

        private static byte[] ColorFunc_ClearPhysicsOWW(int PosX, int PosY)
        {
            if ((PosX + PosY) % 2 == 0)
            {
                return ColorClearPhysicsLightOWW;
            }
            else
            {
                return ColorClearPhysicsDarkOWW;
            }
        }

        private static byte[] ColorFunc_ClearPhysicsSteel(int PosX, int PosY)
        {
            if ((PosX + PosY) % 2 == 0)
            {
                return ColorClearPhysicsSteelLight;
            }
            else
            {
                return ColorClearPhysicsSteelDark;
            }
        }

        private static bool DoDrawThisPixel_DrawNew(byte NewBmpAlpha, byte MaskBmpAlpha)
        {
            return (NewBmpAlpha > 63);
        }

        private static bool DoDrawThisPixel_OnlyAtMask(byte NewBmpAlpha, byte MaskBmpAlpha)
        {
            return (NewBmpAlpha > 63) && (MaskBmpAlpha > 63);
        }

        private static bool DoDrawThisPixel_OnlyAtOWW(byte NewBmpAlpha, byte MaskBmpAlpha)
        {
            return (NewBmpAlpha > 63) && (MaskBmpAlpha == 255);
        }

        private static bool DoDrawThisPixel_NotAtMask(byte NewBmpAlpha, byte MaskBmpAlpha)
        {
            return (NewBmpAlpha > 63) && (MaskBmpAlpha < 63);
        }

        /// <summary>
        /// Copies the ColorBytes to the pixel pointed to.
        /// <para> WARNING: Always make sure that ColorBytes has at least length 4. </para>
        /// </summary>
        /// <param name="PtrToPixel"></param>
        /// <param name="ColorBytes"></param>
        private static unsafe void ChangePixel(byte* PtrToPixel, byte[] ColorBytes)
        {
            PtrToPixel[0] = ColorBytes[0];
            PtrToPixel[1] = ColorBytes[1];
            PtrToPixel[2] = ColorBytes[2];
            PtrToPixel[3] = ColorBytes[3];
        }

        /// <summary>
        /// Copies the bytes of the NewPixel to the pixel pointed to in the first argument.
        /// </summary>
        /// <param name="PtrToPixel"></param>
        /// <param name="PtrToNewPixel"></param>
        private static unsafe void ChangePixel(byte* PtrToPixel, byte* PtrToNewPixel, byte Alpha = 255)
        {
            PtrToPixel[0] = (byte)PtrToNewPixel[0];
            PtrToPixel[1] = (byte)PtrToNewPixel[1];
            PtrToPixel[2] = (byte)PtrToNewPixel[2];
            PtrToPixel[3] = Alpha;
        }


        /// <summary>
        /// Crops the bitmap along a rectangle.
        /// </summary>
        /// <param name="OrigBmp"></param>
        /// <param name="CropRect"></param>
        /// <returns></returns>
        public static Bitmap Crop(this Bitmap OrigBmp, Rectangle CropRect)
        {
            CropRect.Intersect(new Rectangle(0, 0, OrigBmp.Width, OrigBmp.Height));

            return OrigBmp.Clone(CropRect, OrigBmp.PixelFormat);
        }

        /// <summary>
        /// Sets all pixels to transparent black.
        /// </summary>
        /// <param name="OrigBmp"></param>
        public static void Clear(this Bitmap OrigBmp)
        {
            OrigBmp.Clear(ColorTranslator.FromHtml("#00000000"));
        }

        /// <summary>
        /// Fills the bitmap with a uniform color.
        /// </summary>
        /// <param name="OrigBmp"></param>
        /// <param name="ClearColor"></param>
        public static void Clear(this Bitmap OrigBmp, Color ClearColor)
        {
            byte[] ColorBytes = { ClearColor.B, ClearColor.G, ClearColor.R, ClearColor.A };
            
            unsafe
            {
                // Get BitmapData for OrigBitmap
                int Height = OrigBmp.Height;
                int Width = OrigBmp.Width;
                BitmapData OrigBmpData = OrigBmp.LockBits(new Rectangle(0, 0, Width, Height), ImageLockMode.WriteOnly, OrigBmp.PixelFormat);
                // Get pointer to pixel-array
                byte* PtrOrigFirstPixel = (byte*)OrigBmpData.Scan0;
                // Check number of bytes per pixel
                const int BytesPerPixel = 4;
                Debug.Assert(Bitmap.GetPixelFormatSize(OrigBmp.PixelFormat) == 32, "Bitmap to be cleared has no alpha channel!");

                // Copy the pixels
                Parallel.For(0, Height, y =>
                {
                    byte* CurOrigLine = PtrOrigFirstPixel + y * OrigBmpData.Stride;

                    for (int x = 0; x < Width; x++)
                    {
                        ChangePixel(CurOrigLine + BytesPerPixel * x, ColorBytes);
                    }
                });

                // Unlock all pixel data
                OrigBmp.UnlockBits(OrigBmpData);
            }
        }

        /// <summary>
        /// Copies pixels from a new bitmap to the base bitmap. 
        /// </summary>
        /// <param name="OrigBmp"></param>
        /// <param name="NewBmp"></param>
        /// <param name="Pos"></param>
        public static void DrawOn(this Bitmap OrigBmp, Bitmap NewBmp, Point Pos)
        {
            OrigBmp.DrawOn(NewBmp, Pos, DoDrawThisPixel_DrawNew, 255);
        }

        /// <summary>
        /// Draws NewBmp to the base bitmap using the selected CustDrawMode.
        /// </summary>
        /// <param name="OrigBmp"></param>
        /// <param name="NewBmp"></param>
        /// <param name="Pos"></param>
        /// <param name="ColorSelect"></param>
        public static void DrawOn(this Bitmap OrigBmp, Bitmap NewBmp, Point Pos, C.CustDrawMode ColorSelect)
        {
            Func<int, int, byte[]> ColorFunc = ColorFuncDict[ColorSelect];
            Func<byte, byte, bool> DoDrawThisPixel = DoDrawThisPixelDict[ColorSelect];

            if (ColorFunc == null)
            {
                if (ColorSelect.In(C.CustDrawMode.DefaultOWW, C.CustDrawMode.NoOverwriteOWW))
                {
                    OrigBmp.DrawOn(NewBmp, Pos, DoDrawThisPixel, C.ALPHA_OWW);
                }
                else
                {
                    OrigBmp.DrawOn(NewBmp, Pos, DoDrawThisPixel, C.ALPHA_NOOWW);
                }
            }
            else
            {
                OrigBmp.DrawOn(NewBmp, Pos, ColorFunc, DoDrawThisPixel);
            }
        }

        /// <summary>
        /// Draws NewBmp to the base bitmap using the selected CustDrawMode using a mask.
        /// </summary>
        /// <param name="OrigBmp"></param>
        /// <param name="NewBmp"></param>
        /// <param name="MaskBmp"></param>
        /// <param name="Pos"></param>
        /// <param name="ColorSelect"></param>
        public static void DrawOn(this Bitmap OrigBmp, Bitmap NewBmp, Bitmap MaskBmp, Point Pos, C.CustDrawMode ColorSelect)
        {
            Func<int, int, byte[]> ColorFunc = ColorFuncDict[ColorSelect];
            Func<byte, byte, bool> DoDrawThisPixel = DoDrawThisPixelDict[ColorSelect];

            if (ColorFunc == null)
            {
                OrigBmp.DrawOn(NewBmp, MaskBmp, Pos, DoDrawThisPixel);
            }
            else
            {
                OrigBmp.DrawOn(NewBmp, MaskBmp, Pos, ColorFunc, DoDrawThisPixel);
            }
        }


        /// <summary>
        /// Copies pixels from a new bitmap to the base bitmap under condition specified by DoDrawThisPixel. 
        /// </summary>
        /// <param name="OrigBmp"></param>
        /// <param name="NewBmp"></param>
        /// <param name="Pos"></param>
        private static void DrawOn(this Bitmap OrigBmp, Bitmap NewBmp, Point Pos, Func<byte, byte, bool> DoDrawThisPixel, byte Alpha)
        {
            if (NewBmp == null || DoDrawThisPixel == null) return;

            // Get rectangle giving the area that is drawn onto
            Rectangle OrigBmpRect = new Rectangle(0, 0, OrigBmp.Width, OrigBmp.Height);
            Rectangle NewBmpRect = new Rectangle(Pos, NewBmp.Size);
            Rectangle DrawRect = Rectangle.Intersect(OrigBmpRect, NewBmpRect);

            unsafe
            {
                // Get BitmapData for OrigBitmap
                BitmapData OrigBmpData = OrigBmp.LockBits(OrigBmpRect, ImageLockMode.ReadWrite, OrigBmp.PixelFormat);
                // Get pointer to pixel-array
                byte* PtrOrigFirstPixel = (byte*)OrigBmpData.Scan0;
                // Check number of bytes per pixel
                const int BytesPerPixel = 4;
                Debug.Assert(Bitmap.GetPixelFormatSize(OrigBmp.PixelFormat) == 32, "Bitmap drawn onto has no alpha channel!");

                // Get BitmapData for NewBitmap
                BitmapData NewBmpData = NewBmp.LockBits(new Rectangle(0, 0, NewBmp.Width, NewBmp.Height), ImageLockMode.ReadOnly, NewBmp.PixelFormat);
                // Get pointer to pixel-array
                byte* PtrNewFirstPixel = (byte*)NewBmpData.Scan0;
                // Check number of bytes per pixel
                Debug.Assert(Bitmap.GetPixelFormatSize(NewBmp.PixelFormat) == 32, "Bitmap to drawn has no alpha channel!");

                // Copy the pixels
                Parallel.For(0, DrawRect.Height, y =>
                {
                    // We start CurOrigLine at pixel (DrawRect.Left, y + DrawRect.Top)!
                    byte* CurOrigLine = PtrOrigFirstPixel + ((y + DrawRect.Top) * OrigBmpData.Stride) + DrawRect.Left * BytesPerPixel;
                    // We start CurNewList at pixel (DrawRect.Left - NewBmpRect.Left, y + DrawRect.Top - NewBmpRect.Top)!
                    byte* CurNewLine = PtrNewFirstPixel + ((y + DrawRect.Top - NewBmpRect.Top) * NewBmpData.Stride) + (DrawRect.Left - NewBmpRect.Left) * BytesPerPixel;

                    for (int x = 0; x < DrawRect.Width * BytesPerPixel; x = x + BytesPerPixel)
                    {
                        if (DoDrawThisPixel((byte)CurNewLine[x + 3], (byte)CurOrigLine[x + 3]))
                        {
                            ChangePixel(CurOrigLine + x, CurNewLine + x, Alpha);
                        }
                    }
                });

                // Unlock all pixel data
                OrigBmp.UnlockBits(OrigBmpData);
                NewBmp.UnlockBits(NewBmpData);
            }
        }

        /// <summary>
        /// Copies pixels from a new bitmap to the base bitmap as specified by ColorFunc and DoDrawThisPixel. 
        /// </summary>
        /// <param name="OrigBmp"></param>
        /// <param name="NewBmp"></param>
        /// <param name="Pos"></param>
        /// <param name="ColorSelect"></param>
        private static void DrawOn(this Bitmap OrigBmp, Bitmap NewBmp, Point Pos, Func<int, int, byte[]> ColorFunc, Func<byte, byte, bool> DoDrawThisPixel)
        {
            if (NewBmp == null || ColorFunc == null || DoDrawThisPixel == null) return;

            // Get rectangle giving the area that is drawn onto
            Rectangle OrigBmpRect = new Rectangle(0, 0, OrigBmp.Width, OrigBmp.Height);
            Rectangle NewBmpRect = new Rectangle(Pos, NewBmp.Size);
            Rectangle DrawRect = Rectangle.Intersect(OrigBmpRect, NewBmpRect);

            unsafe
            {
                // Get BitmapData for OrigBitmap
                BitmapData OrigBmpData = OrigBmp.LockBits(OrigBmpRect, ImageLockMode.ReadWrite, OrigBmp.PixelFormat);
                // Get pointer to pixel-array
                byte* PtrOrigFirstPixel = (byte*)OrigBmpData.Scan0;
                // Check number of bytes per pixel
                const int BytesPerPixel = 4;
                Debug.Assert(Bitmap.GetPixelFormatSize(OrigBmp.PixelFormat) == 32, "Bitmap drawn onto has no alpha channel!");

                // Get BitmapData for NewBitmap
                BitmapData NewBmpData = NewBmp.LockBits(new Rectangle(0, 0, NewBmp.Width, NewBmp.Height), ImageLockMode.ReadOnly, NewBmp.PixelFormat);
                // Get pointer to pixel-array
                byte* PtrNewFirstPixel = (byte*)NewBmpData.Scan0;
                // Check number of bytes per pixel
                Debug.Assert(Bitmap.GetPixelFormatSize(NewBmp.PixelFormat) == 32, "Bitmap to drawn has no alpha channel!");

                // Copy the pixels
                Parallel.For(0, DrawRect.Height, y =>
                {
                    // We start CurOrigLine at pixel (DrawRect.Left, y + DrawRect.Top)!
                    byte* CurOrigLine = PtrOrigFirstPixel + ((y + DrawRect.Top) * OrigBmpData.Stride) + DrawRect.Left * BytesPerPixel;
                    // We start CurNewList at pixel (DrawRect.Left - NewBmpRect.Left, y + DrawRect.Top - NewBmpRect.Top)!
                    byte* CurNewLine = PtrNewFirstPixel + ((y + DrawRect.Top - NewBmpRect.Top) * NewBmpData.Stride) + (DrawRect.Left - NewBmpRect.Left) * BytesPerPixel;

                    for (int x = 0; x < DrawRect.Width; x++)
                    {
                        // We require an alpha value of 25%
                        if (DoDrawThisPixel((byte)CurNewLine[x * BytesPerPixel + 3], (byte)CurOrigLine[x * BytesPerPixel + 3]))
                        {
                            ChangePixel(CurOrigLine + x * BytesPerPixel, ColorFunc(x, y));
                        }
                    }
                });

                // Unlock all pixel data
                OrigBmp.UnlockBits(OrigBmpData);
                NewBmp.UnlockBits(NewBmpData);
            }
        }

        /// <summary>
        /// Copies pixels from a new bitmap to the base bitmap under condition specified by DoDrawThisPixel, using a mask.
        /// </summary>
        /// <param name="OrigBmp"></param>
        /// <param name="NewBmp"></param>
        /// <param name="MaskBmp"></param>
        /// <param name="Pos"></param>
        /// <param name="DoDrawThisPixel"></param>
        private static void DrawOn(this Bitmap OrigBmp, Bitmap NewBmp, Bitmap MaskBmp, Point Pos, Func<byte, byte, bool> DoDrawThisPixel)
        {
            if (NewBmp == null || MaskBmp == null || DoDrawThisPixel == null) return;

            // Get rectangle giving the area that is drawn onto
            Rectangle OrigBmpRect = new Rectangle(0, 0, OrigBmp.Width, OrigBmp.Height);
            Rectangle NewBmpRect = new Rectangle(Pos, NewBmp.Size);
            Rectangle DrawRect = Rectangle.Intersect(OrigBmpRect, NewBmpRect);
            Debug.Assert(OrigBmp.Width == MaskBmp.Width && OrigBmp.Height == MaskBmp.Height, "Bitmap mask has different size than target bitmap.");

            unsafe
            {
                // Get BitmapData for OrigBitmap
                BitmapData OrigBmpData = OrigBmp.LockBits(OrigBmpRect, ImageLockMode.ReadWrite, OrigBmp.PixelFormat);
                // Get pointer to pixel-array
                byte* PtrOrigFirstPixel = (byte*)OrigBmpData.Scan0;
                // Check number of bytes per pixel
                const int BytesPerPixel = 4;
                Debug.Assert(Bitmap.GetPixelFormatSize(OrigBmp.PixelFormat) == 32, "Bitmap drawn onto has no alpha channel!");

                // Get BitmapData for NewBitmap
                BitmapData NewBmpData = NewBmp.LockBits(new Rectangle(0, 0, NewBmp.Width, NewBmp.Height), ImageLockMode.ReadOnly, NewBmp.PixelFormat);
                // Get pointer to pixel-array
                byte* PtrNewFirstPixel = (byte*)NewBmpData.Scan0;
                // Check number of bytes per pixel
                Debug.Assert(Bitmap.GetPixelFormatSize(NewBmp.PixelFormat) == 32, "Bitmap to drawn has no alpha channel!");

                // Get BitmapData for MaskBitmap
                BitmapData MaskBmpData = MaskBmp.LockBits(OrigBmpRect, ImageLockMode.ReadOnly, MaskBmp.PixelFormat);
                // Get pointer to pixel-array
                byte* PtrMaskFirstPixel = (byte*)MaskBmpData.Scan0;
                // Check number of bytes per pixel
                Debug.Assert(Bitmap.GetPixelFormatSize(MaskBmp.PixelFormat) == 32, "Mask bitmap has no alpha channel!");


                // Copy the pixels
                Parallel.For(0, DrawRect.Height, y =>
                {
                    // We start CurOrigLine and CurMaskLine at pixel (DrawRect.Left, y + DrawRect.Top)!
                    byte* CurOrigLine = PtrOrigFirstPixel + ((y + DrawRect.Top) * OrigBmpData.Stride) + DrawRect.Left * BytesPerPixel;
                    byte* CurMaskLine = PtrMaskFirstPixel + ((y + DrawRect.Top) * MaskBmpData.Stride) + DrawRect.Left * BytesPerPixel;
                    // We start CurNewList at pixel (DrawRect.Left - NewBmpRect.Left, y + DrawRect.Top - NewBmpRect.Top)!
                    byte* CurNewLine = PtrNewFirstPixel + ((y + DrawRect.Top - NewBmpRect.Top) * NewBmpData.Stride) + (DrawRect.Left - NewBmpRect.Left) * BytesPerPixel;

                    for (int x = 0; x < DrawRect.Width * BytesPerPixel; x = x + BytesPerPixel)
                    {
                        byte CurNewAlpha = (byte)CurNewLine[x + 3];
                        byte CurMaskAlpha = (byte)CurMaskLine[x + 3];

                        if (DoDrawThisPixel(CurNewAlpha, CurMaskAlpha))
                        {
                            ChangePixel(CurOrigLine + x, CurNewLine + x);
                        }
                    }
                });

                // Unlock all pixel data
                OrigBmp.UnlockBits(OrigBmpData);
                NewBmp.UnlockBits(NewBmpData);
                MaskBmp.UnlockBits(MaskBmpData);
            }
        }

        /// <summary>
        /// Copies pixels from a new bitmap to the base bitmap as specified by ColorFunc and DoDrawThisPixel, using a mask. 
        /// </summary>
        /// <param name="OrigBmp"></param>
        /// <param name="NewBmp"></param>
        /// <param name="MaskBmp"></param>
        /// <param name="Pos"></param>
        /// <param name="ColorFunc"></param>
        /// <param name="DoDrawThisPixel"></param>
        private static void DrawOn(this Bitmap OrigBmp, Bitmap NewBmp, Bitmap MaskBmp, Point Pos, Func<int, int, byte[]> ColorFunc, Func<byte, byte, bool> DoDrawThisPixel)
        {
            if (NewBmp == null || MaskBmp == null || ColorFunc == null || DoDrawThisPixel == null) return;

            // Get rectangle giving the area that is drawn onto
            Rectangle OrigBmpRect = new Rectangle(0, 0, OrigBmp.Width, OrigBmp.Height);
            Rectangle NewBmpRect = new Rectangle(Pos, NewBmp.Size);
            Rectangle DrawRect = Rectangle.Intersect(OrigBmpRect, NewBmpRect);
            Debug.Assert(OrigBmp.Width == MaskBmp.Width && OrigBmp.Height == MaskBmp.Height, "Bitmap mask has different size than target bitmap.");

            unsafe
            {
                // Get BitmapData for OrigBitmap
                BitmapData OrigBmpData = OrigBmp.LockBits(OrigBmpRect, ImageLockMode.ReadWrite, OrigBmp.PixelFormat);
                // Get pointer to pixel-array
                byte* PtrOrigFirstPixel = (byte*)OrigBmpData.Scan0;
                // Check number of bytes per pixel
                const int BytesPerPixel = 4;
                Debug.Assert(Bitmap.GetPixelFormatSize(OrigBmp.PixelFormat) == 32, "Bitmap drawn onto has no alpha channel!");

                // Get BitmapData for NewBitmap
                BitmapData NewBmpData = NewBmp.LockBits(new Rectangle(0, 0, NewBmp.Width, NewBmp.Height), ImageLockMode.ReadOnly, NewBmp.PixelFormat);
                // Get pointer to pixel-array
                byte* PtrNewFirstPixel = (byte*)NewBmpData.Scan0;
                // Check number of bytes per pixel
                Debug.Assert(Bitmap.GetPixelFormatSize(NewBmp.PixelFormat) == 32, "Bitmap to drawn has no alpha channel!");

                // Get BitmapData for MaskBitmap
                BitmapData MaskBmpData = MaskBmp.LockBits(OrigBmpRect, ImageLockMode.ReadOnly, MaskBmp.PixelFormat);
                // Get pointer to pixel-array
                byte* PtrMaskFirstPixel = (byte*)MaskBmpData.Scan0;
                // Check number of bytes per pixel
                Debug.Assert(Bitmap.GetPixelFormatSize(MaskBmp.PixelFormat) == 32, "Mask bitmap has no alpha channel!");


                // Copy the pixels
                Parallel.For(0, DrawRect.Height, y =>
                {
                    // We start CurOrigLine and CurMaskLine at pixel (DrawRect.Left, y + DrawRect.Top)!
                    byte* CurOrigLine = PtrOrigFirstPixel + ((y + DrawRect.Top) * OrigBmpData.Stride) + DrawRect.Left * BytesPerPixel;
                    byte* CurMaskLine = PtrMaskFirstPixel + ((y + DrawRect.Top) * MaskBmpData.Stride) + DrawRect.Left * BytesPerPixel;
                    // We start CurNewList at pixel (DrawRect.Left - NewBmpRect.Left, y + DrawRect.Top - NewBmpRect.Top)!
                    byte* CurNewLine = PtrNewFirstPixel + ((y + DrawRect.Top - NewBmpRect.Top) * NewBmpData.Stride) + (DrawRect.Left - NewBmpRect.Left) * BytesPerPixel;

                    for (int x = 0; x < DrawRect.Width; x++)
                    {
                        byte CurNewAlpha = (byte)CurNewLine[x * BytesPerPixel + 3];
                        byte CurMaskAlpha = (byte)CurMaskLine[x * BytesPerPixel + 3];

                        if (DoDrawThisPixel(CurNewAlpha, CurMaskAlpha))
                        {
                            ChangePixel(CurOrigLine + x * BytesPerPixel, ColorFunc(x, y));
                        }
                    }
                });

                // Unlock all pixel data
                OrigBmp.UnlockBits(OrigBmpData);
                NewBmp.UnlockBits(NewBmpData);
                MaskBmp.UnlockBits(MaskBmpData);
            }
        }


        /// <summary>
        /// Zooms a bitmap.
        /// </summary>
        /// <param name="OrigBmp"></param>
        /// <param name="ZoomFactor"></param>
        /// <returns></returns>
        public static Bitmap Zoom(this Bitmap OrigBmp, int ZoomFactor)
        {
            int NewWidth = (ZoomFactor < 0) ? OrigBmp.Width / (Math.Abs(ZoomFactor) + 1) : OrigBmp.Width * (ZoomFactor + 1);
            int NewHeight = (ZoomFactor < 0) ? OrigBmp.Height / (Math.Abs(ZoomFactor) + 1) : OrigBmp.Height * (ZoomFactor + 1);

            return OrigBmp.Zoom(ZoomFactor, new Size(NewWidth, NewHeight));
        }


        /// <summary>
        /// Zooms a bitmap and crops it to a smaller size.
        /// </summary>
        /// <param name="OrigBmp"></param>
        /// <param name="ZoomFactor"></param>
        /// <param name="NewBmpSize"></param>
        /// <returns></returns>
        public static Bitmap Zoom(this Bitmap OrigBmp, int ZoomFactor, Size NewBmpSize)
        {
            Bitmap NewBmp = new Bitmap(NewBmpSize.Width, NewBmpSize.Height);

            unsafe
            {
                // Get BitmapData for OldBitmap
                BitmapData OrigBmpData = OrigBmp.LockBits(new Rectangle(0, 0, OrigBmp.Width, OrigBmp.Height), ImageLockMode.ReadOnly, OrigBmp.PixelFormat);
                // Get pointer to pixel-array
                byte* PtrOrigFirstPixel = (byte*)OrigBmpData.Scan0;
                // Check number of bytes per pixel
                const int BytesPerPixel = 4;
                Debug.Assert(Bitmap.GetPixelFormatSize(OrigBmp.PixelFormat) == 32, "Zoomed Bitmap has no alpha channel!");


                // Get BitmapData for NewBitmap
                BitmapData NewBmpData = NewBmp.LockBits(new Rectangle(0, 0, NewBmp.Width, NewBmp.Height), ImageLockMode.WriteOnly, NewBmp.PixelFormat);
                // Get pointer to pixel-array
                byte* PtrNewFirstPixel = (byte*)NewBmpData.Scan0;

                if (ZoomFactor < 0)
                {
                    ZoomFactor = Math.Abs(ZoomFactor) + 1;

                    // Copy the pixels
                    for(int y = 0; y < NewBmp.Height; y++)
                    {
                        byte* CurNewLine = PtrNewFirstPixel + y * NewBmpData.Stride;
                        byte* CurOrigLine = PtrOrigFirstPixel + y * ZoomFactor * OrigBmpData.Stride;

                        for (int x = 0; x < NewBmp.Width; x++)
                        {
                            CurNewLine[x * BytesPerPixel + 0] = (byte)CurOrigLine[x * BytesPerPixel * ZoomFactor + 0];
                            CurNewLine[x * BytesPerPixel + 1] = (byte)CurOrigLine[x * BytesPerPixel * ZoomFactor + 1];
                            CurNewLine[x * BytesPerPixel + 2] = (byte)CurOrigLine[x * BytesPerPixel * ZoomFactor + 2];
                            CurNewLine[x * BytesPerPixel + 3] = (byte)CurOrigLine[x * BytesPerPixel * ZoomFactor + 3];
                        }
                    }
                }
                else
                {
                    ZoomFactor++;
                    
                    // Copy the pixels
                    for(int y = 0; y < OrigBmp.Height; y++)
                    {
                        byte* CurOrigLine = PtrOrigFirstPixel + y * OrigBmpData.Stride;

                        for (int i = 0; i < ZoomFactor; i++)
                        {
                            byte* CurNewLine = PtrNewFirstPixel + (y * ZoomFactor + i) * NewBmpData.Stride;

                            for (int x = 0; x < OrigBmp.Width; x++)
                            {
                                for (int j = 0; j < ZoomFactor; j++) 
                                {
                                    CurNewLine[(ZoomFactor * x + j) * BytesPerPixel + 0] = (byte)CurOrigLine[x * BytesPerPixel + 0];
                                    CurNewLine[(ZoomFactor * x + j) * BytesPerPixel + 1] = (byte)CurOrigLine[x * BytesPerPixel + 1];
                                    CurNewLine[(ZoomFactor * x + j) * BytesPerPixel + 2] = (byte)CurOrigLine[x * BytesPerPixel + 2];
                                    CurNewLine[(ZoomFactor * x + j) * BytesPerPixel + 3] = (byte)CurOrigLine[x * BytesPerPixel + 3];
                                }
                            }
                        }
                    }
                }

                // Unlock all pixel data
                OrigBmp.UnlockBits(OrigBmpData);
                NewBmp.UnlockBits(NewBmpData);
            }

            return NewBmp;
        }

        /// <summary>
        /// Draws a list of filled rectangles on a bitmap.
        /// </summary>
        /// <param name="OrigBmp"></param>
        /// <param name="RectList"></param>
        /// <param name="RectColor"></param>
        public static void DrawOnFilledRectangles(this Bitmap OrigBmp, List<Rectangle> RectList, Color RectColor)
        {
            if (RectList == null) return;
            
            using (Graphics g = Graphics.FromImage(OrigBmp))
            {
                using (Brush b = new SolidBrush(RectColor))
                {
                    RectList.ForEach(rect => g.FillRectangle(b, rect));
                }
                g.Dispose();
            }
        }

        /// <summary>
        /// Draws a list of rectangles on a bitmap.
        /// </summary>
        /// <param name="OrigBmp"></param>
        /// <param name="RectList"></param>
        /// <param name="RectColor"></param>
        public static void DrawOnRectangles(this Bitmap OrigBmp, List<Rectangle> RectList, Color RectColor)
        {
            if (RectList == null) return;
            
            using (Graphics g = Graphics.FromImage(OrigBmp))
            {
                using (Pen p = new Pen(RectColor))
                {
                    RectList.ForEach(rect => g.DrawRectangle(p, rect));
                }
                g.Dispose();
            }
        }

        /// <summary>
        /// Draws a dotted rectangle on a bitmap.
        /// </summary>
        /// <param name="OrigBmp"></param>
        /// <param name="Rect"></param>
        public static void DrawOnDottedRectangle(this Bitmap OrigBmp, Rectangle Rect)
        {
            Rectangle OrigBmpRect = new Rectangle(0, 0, OrigBmp.Width, OrigBmp.Height);

            // Shrink rectangle to Bitmap size
            Rect.Intersect(OrigBmpRect);

            unsafe
            {
                // Get BitmapData for OrigBitmap
                BitmapData OrigBmpData = OrigBmp.LockBits(OrigBmpRect, ImageLockMode.WriteOnly, OrigBmp.PixelFormat);
                // Get pointer to pixel-array
                byte* PtrOrigFirstPixel = (byte*)OrigBmpData.Scan0;
                // Check number of bytes per pixel
                const int BytesPerPixel = 4;
                Debug.Assert(Bitmap.GetPixelFormatSize(OrigBmp.PixelFormat) == 32, "Bitmap drawn onto has no alpha channel!");
       
                byte* OrigLine;

                // Top and bottom
                for (int i = 0; i < 2; i++)
                {
                    int PosY = (i == 0) ? Rect.Top : Rect.Bottom - 1;
                    
                    OrigLine = PtrOrigFirstPixel + PosY * OrigBmpData.Stride;

                    for (int x = Rect.Left; x < Rect.Right; x++)
                    {
                        if ((PosY + x) % 6 < 3)
                        {
                            // dark gray
                            OrigLine[BytesPerPixel * x] = 30;
                            OrigLine[BytesPerPixel * x + 1] = 30;
                            OrigLine[BytesPerPixel * x + 2] = 30;
                            OrigLine[BytesPerPixel * x + 3] = 255;
                        }
                        else
                        {
                            // off-white
                            OrigLine[BytesPerPixel * x] = 240;
                            OrigLine[BytesPerPixel * x + 1] = 240;
                            OrigLine[BytesPerPixel * x + 2] = 240;
                            OrigLine[BytesPerPixel * x + 3] = 255;
                        }
                    }
                }

                // Right and left side
                for (int y = Rect.Top; y < Rect.Bottom; y++)
                {
                    OrigLine = PtrOrigFirstPixel + y * OrigBmpData.Stride;

                    for (int i = 0; i < 2; i++)
                    {
                        int PosX = (i == 0) ? Rect.Left : Rect.Right - 1;
                        
                        if ((y + PosX) % 6 < 3)
                        {
                            // dark gray
                            OrigLine[BytesPerPixel * PosX] = 30;
                            OrigLine[BytesPerPixel * PosX + 1] = 30;
                            OrigLine[BytesPerPixel * PosX + 2] = 30;
                            OrigLine[BytesPerPixel * PosX + 3] = 255;
                        }
                        else
                        {
                            // off-white
                            OrigLine[BytesPerPixel * PosX] = 240;
                            OrigLine[BytesPerPixel * PosX + 1] = 240;
                            OrigLine[BytesPerPixel * PosX + 2] = 240;
                            OrigLine[BytesPerPixel * PosX + 3] = 255;
                        }
                    }
                }

                // Unlock all pixel data
                OrigBmp.UnlockBits(OrigBmpData);
            }
        }

    }
}
