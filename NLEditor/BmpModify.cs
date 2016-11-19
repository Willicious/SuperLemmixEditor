﻿using System;
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
         *     - DrawOnErase(this Bitmap OrigBmp, Bitmap NewBmp, Point Pos)
         *     - DrawOnNoOw(this Bitmap OrigBmp, Bitmap NewBmp, Point Pos)
         *     - DrawOnNoOw(this Bitmap OrigBmp, Bitmap NewBmp, Point Pos, Bitmap AddBmp)
         *     - DrawOnMask(this Bitmap OrigBmp, Bitmap NewBmp, Point Pos, Bitmap MaskBmp)
         *     - Zoom(this Bitmap OrigBmp, int ZoomFactor)
         *     - Zoom(this Bitmap OrigBmp, int ZoomFactor, Size NewBmpSize)
         *     - DrawOnRectangles(this Bitmap OrigBmp, List<Rectangle> RectList, Color RectColor)
         *     - DrawOnFilledRectangles(this Bitmap OrigBmp, List<Rectangle> RectList, Color RectColor)
         *     - DrawOnDottedRectangles(this Bitmap OrigBmp, Rectangle Rect)
         * -------------------------------------------------------- */

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
                        CurOrigLine[BytesPerPixel * x + 0] = ClearColor.B;
                        CurOrigLine[BytesPerPixel * x + 1] = ClearColor.G;
                        CurOrigLine[BytesPerPixel * x + 2] = ClearColor.R;
                        CurOrigLine[BytesPerPixel * x + 3] = ClearColor.A;
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
            if (NewBmp == null) return;

            // Get rectangle giving the area that is drawn onto
            Rectangle OrigBmpRect = new Rectangle(0, 0, OrigBmp.Width, OrigBmp.Height);
            Rectangle NewBmpRect = new Rectangle(Pos, NewBmp.Size);
            Rectangle DrawRect = Rectangle.Intersect(OrigBmpRect, NewBmpRect);

            unsafe
            {
                // Get BitmapData for OrigBitmap
                BitmapData OrigBmpData = OrigBmp.LockBits(OrigBmpRect, ImageLockMode.WriteOnly, OrigBmp.PixelFormat);
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
                        // We require an alpha value of 25%
                        if ((byte)CurNewLine[x + 3] > 63)
                        {
                            CurOrigLine[x] = (byte)CurNewLine[x]; // Blue
                            CurOrigLine[x + 1] = (byte)CurNewLine[x + 1]; // Green
                            CurOrigLine[x + 2] = (byte)CurNewLine[x + 2]; // Red
                            CurOrigLine[x + 3] = 255; // alpha = full non-transparent
                        }
                    }
                });

                // Unlock all pixel data
                OrigBmp.UnlockBits(OrigBmpData);
                NewBmp.UnlockBits(NewBmpData);
            }
        }

        /// <summary>
        /// Erases all pixels from the base bitmap, where the new bitmap is non-transparent 
        /// </summary>
        /// <param name="OrigBmp"></param>
        /// <param name="NewBmp"></param>
        /// <param name="Pos"></param>
        public static void DrawOnErase(this Bitmap OrigBmp, Bitmap NewBmp, Point Pos)
        {
            if (NewBmp == null) return;

            // Get rectangle giving the area that is drawn onto
            Rectangle OrigBmpRect = new Rectangle(0, 0, OrigBmp.Width, OrigBmp.Height);
            Rectangle NewBmpRect = new Rectangle(Pos, NewBmp.Size);
            Rectangle DrawRect = Rectangle.Intersect(OrigBmpRect, NewBmpRect);

            unsafe
            {
                // Get BitmapData for OrigBitmap
                BitmapData OrigBmpData = OrigBmp.LockBits(OrigBmpRect, ImageLockMode.WriteOnly, OrigBmp.PixelFormat);
                // Get pointer to pixel-array
                byte* PtrOrigFirstPixel = (byte*)OrigBmpData.Scan0;
                // Check number of bytes per pixel
                const int BytesPerPixel = 4;
                Debug.Assert(Bitmap.GetPixelFormatSize(OrigBmp.PixelFormat) == 32, "Bitmap erased from has no alpha channel!");

                // Get BitmapData for NewBitmap
                BitmapData NewBmpData = NewBmp.LockBits(new Rectangle(0, 0, NewBmp.Width, NewBmp.Height), ImageLockMode.ReadOnly, NewBmp.PixelFormat);
                // Get pointer to pixel-array
                byte* PtrNewFirstPixel = (byte*)NewBmpData.Scan0;
                // Check number of bytes per pixel
                Debug.Assert(Bitmap.GetPixelFormatSize(NewBmp.PixelFormat) == 32, "Bitmap to erase has no alpha channel!");

                // Copy the pixels
                Parallel.For(0, DrawRect.Height, y =>
                {
                    // We start CurOrigLine at pixel (DrawRect.Left, y + DrawRect.Top)!
                    byte* CurOrigLine = PtrOrigFirstPixel + ((y + DrawRect.Top) * OrigBmpData.Stride) + DrawRect.Left * BytesPerPixel;
                    // We start CurNewList at pixel (DrawRect.Left - NewBmpRect.Left, y + DrawRect.Top - NewBmpRect.Top)!
                    byte* CurNewLine = PtrNewFirstPixel + ((y + DrawRect.Top - NewBmpRect.Top) * NewBmpData.Stride) + (DrawRect.Left - NewBmpRect.Left) * BytesPerPixel;

                    for (int x = 0; x < DrawRect.Width * BytesPerPixel; x = x + BytesPerPixel)
                    {
                        // We require an alpha value of 25%
                        if ((byte)CurNewLine[x + 3] > 63)
                        {
                            CurOrigLine[x] = 0;
                            CurOrigLine[x + 1] = 0;
                            CurOrigLine[x + 2] = 0;
                            CurOrigLine[x + 3] = 0;
                        }
                    }
                });

                // Unlock all pixel data
                OrigBmp.UnlockBits(OrigBmpData);
                NewBmp.UnlockBits(NewBmpData);
            }
        }

        /// <summary>
        /// Copies pixels from a new bitmap at all pixels where the base bitmap is transparent. 
        /// </summary>
        /// <param name="OrigBmp"></param>
        /// <param name="NewBmp"></param>
        /// <param name="Pos"></param>
        public static void DrawOnNoOw(this Bitmap OrigBmp, Bitmap NewBmp, Point Pos)
        {
            if (NewBmp == null) return;

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
                        // We require an alpha value of 25%
                        if ((byte)CurNewLine[x + 3] > 63 && (byte)CurOrigLine[x + 3] > 63)
                        {
                            CurOrigLine[x] = (byte)CurNewLine[x]; // Blue
                            CurOrigLine[x + 1] = (byte)CurNewLine[x + 1]; // Green
                            CurOrigLine[x + 2] = (byte)CurNewLine[x + 2]; // Red
                            CurOrigLine[x + 3] = 255; // alpha = full non-transparent
                        }
                    }
                });

                // Unlock all pixel data
                OrigBmp.UnlockBits(OrigBmpData);
                NewBmp.UnlockBits(NewBmpData);
            }
        }

        /// <summary>
        /// Copies pixels from a new bitmap at all pixels where the base bitmap is transparent.
        /// <para> These pixels are copied onto the AddBmp as well. </para>
        /// </summary>
        /// <param name="OrigBmp"></param>
        /// <param name="NewBmp"></param>
        /// <param name="Pos"></param>
        /// <param name="AddBmp"></param>
        public static void DrawOnNoOw(this Bitmap OrigBmp, Bitmap NewBmp, Point Pos, Bitmap AddBmp)
        {
            if (NewBmp == null || AddBmp == null) return;

            // Get rectangle giving the area that is drawn onto
            Rectangle OrigBmpRect = new Rectangle(0, 0, OrigBmp.Width, OrigBmp.Height);
            Rectangle NewBmpRect = new Rectangle(Pos, NewBmp.Size);
            Rectangle DrawRect = Rectangle.Intersect(OrigBmpRect, NewBmpRect);
            Debug.Assert(OrigBmp.Width == AddBmp.Width && OrigBmp.Height == AddBmp.Height, "Additional bitmap has different size than target bitmap.");

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

                // Get BitmapData for AddBitmap
                BitmapData AddBmpData = AddBmp.LockBits(OrigBmpRect, ImageLockMode.WriteOnly, AddBmp.PixelFormat);
                // Get pointer to pixel-array
                byte* PtrAddFirstPixel = (byte*)AddBmpData.Scan0;
                // Check number of bytes per pixel
                Debug.Assert(Bitmap.GetPixelFormatSize(AddBmp.PixelFormat) == 32, "Additional bitmap has no alpha channel!");

                // Copy the pixels
                Parallel.For(0, DrawRect.Height, y =>
                {
                    // We start CurOrigLine at pixel (DrawRect.Left, y + DrawRect.Top)!
                    byte* CurOrigLine = PtrOrigFirstPixel + ((y + DrawRect.Top) * OrigBmpData.Stride) + DrawRect.Left * BytesPerPixel;
                    byte* CurAddLine = PtrAddFirstPixel + ((y + DrawRect.Top) * AddBmpData.Stride) + DrawRect.Left * BytesPerPixel;
                    // We start CurNewList at pixel (DrawRect.Left - NewBmpRect.Left, y + DrawRect.Top - NewBmpRect.Top)!
                    byte* CurNewLine = PtrNewFirstPixel + ((y + DrawRect.Top - NewBmpRect.Top) * NewBmpData.Stride) + (DrawRect.Left - NewBmpRect.Left) * BytesPerPixel;

                    for (int x = 0; x < DrawRect.Width * BytesPerPixel; x = x + BytesPerPixel)
                    {
                        // We require an alpha value of 25%
                        if ((byte)CurNewLine[x + 3] > 63 && (byte)CurOrigLine[x + 3] > 63)
                        {
                            CurOrigLine[x] = (byte)CurNewLine[x]; // Blue
                            CurOrigLine[x + 1] = (byte)CurNewLine[x + 1]; // Green
                            CurOrigLine[x + 2] = (byte)CurNewLine[x + 2]; // Red
                            CurOrigLine[x + 3] = 255; // alpha = full non-transparent

                            CurAddLine[x] = (byte)CurNewLine[x]; 
                            CurAddLine[x + 1] = (byte)CurNewLine[x + 1]; 
                            CurAddLine[x + 2] = (byte)CurNewLine[x + 2]; 
                            CurAddLine[x + 3] = 255;
                        }
                    }
                });

                // Unlock all pixel data
                OrigBmp.UnlockBits(OrigBmpData);
                NewBmp.UnlockBits(NewBmpData);
                AddBmp.UnlockBits(AddBmpData);
            }
        }

        /// <summary>
        /// Copies pixels from a new bitmap at all pixels where the mask bitmap is non-transparent. 
        /// </summary>
        /// <param name="OrigBmp"></param>
        /// <param name="NewBmp"></param>
        /// <param name="Pos"></param>
        /// <param name="MaskBmp"></param>
        public static void DrawOnMask(this Bitmap OrigBmp, Bitmap NewBmp, Point Pos, Bitmap MaskBmp)
        {
            if (NewBmp == null || MaskBmp == null) return;

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
                        // We require an alpha value of 25%
                        if ((byte)CurNewLine[x + 3] > 63 && (byte)CurMaskLine[x + 3] > 63)
                        {
                            CurOrigLine[x] = (byte)CurNewLine[x]; // Blue
                            CurOrigLine[x + 1] = (byte)CurNewLine[x + 1]; // Green
                            CurOrigLine[x + 2] = (byte)CurNewLine[x + 2]; // Red
                            CurOrigLine[x + 3] = 255; // alpha = full non-transparent
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
