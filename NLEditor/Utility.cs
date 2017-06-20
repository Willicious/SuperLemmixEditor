using System;
using System.Collections.Generic;
using System.Linq;
using System.Drawing;

namespace NLEditor
{
    static class Utility
    {
        /// <summary>
        /// Checks if an object is contained in an array.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public static bool In<T>(this T obj, params T[] args)
        {
            return args.Contains(obj);
        }

        /// <summary>
        /// Swaps two elements of given index in the list.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="myList"></param>
        /// <param name="index1"></param>
        /// <param name="index2"></param>
        public static void Swap<T>(this IList<T> myList, int index1, int index2)
        {
            T item = myList[index1];
            myList[index1] = myList[index2];
            myList[index2] = item;
        }

        /// <summary>
        /// Creates a shallow copy of a range of elements from index to the end of the list.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="myList"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        public static List<T> GetRange<T>(this List<T> myList, int index)
        {
            return myList.GetRange(index, myList.Count - index);
        }

        /// <summary>
        /// Restricts an integer to an interval between min and max (inclusive).
        /// </summary>
        /// <param name="x"></param>
        /// <param name="min"></param>
        /// <param name="max"></param>
        /// <returns></returns>
        public static int Restrict(this int x, int min, int max)
        {
            System.Diagnostics.Debug.Assert(max >= min, "int.Restrict called with minimum larger than the maximum.");
            return Math.Min(Math.Max(x, min), max);
        }

        /// <summary>
        /// Rounds to the nearest multiple of the value step.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="step"></param>
        /// <returns></returns>
        public static int RoundToMultiple(this int x, int step)
        {
            return ((x + step / 2) / step) * step;
        }

        /// <summary>
        /// Parses a string value to an enum of given type.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <returns></returns>
        public static T ParseEnum<T>(string value)
        {
            return (T)Enum.Parse(typeof(T), value, true);
        }

        /// <summary>
        /// Checks whether a string value can be parsed to an enum of given type.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool ExistsInEnum<T>(string value)
        {
            return Enum.IsDefined(typeof(T), value);
        }

        /// <summary>
        /// Loads a bitmap from a file and closes the file again.
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public static Bitmap CreateBitmapFromFile(string filePath)
        {
            Bitmap image;
            using (var tempImage = new Bitmap(filePath))
            {
                image = new Bitmap(tempImage);
            }
            return image;
        }


        /// <summary>
        /// Deletes the specified file, if it exists.
        /// </summary>
        /// <param name="filePath"></param>
        public static void DeleteFile(string filePath)
        {
            if (System.IO.File.Exists(filePath))
            {
                try
                {
                    System.IO.File.Delete(filePath);
                }
                finally
                {
                    // do nothing.
                }
            }
        }

        /// <summary>
        /// Creates a new rectangle having the two points as diagonally opposite vertices.
        /// </summary>
        /// <param name="pos1"></param>
        /// <param name="pos2"></param>
        /// <returns></returns>
        public static Rectangle RectangleFrom(Point pos1, Point pos2)
        {
            int left = Math.Min(pos1.X, pos2.X);
            int top = Math.Min(pos1.Y, pos2.Y);
            int width = Math.Abs(pos1.X - pos2.X);
            int height = Math.Abs(pos1.Y - pos2.Y);

            return new Rectangle(left, top, width, height);
        }

        public static Rectangle RotateInRectangle(this Rectangle origRect, Rectangle border)
        {
            Point center = new Point(border.Left + border.Width / 2, border.Top + border.Height / 2);
            Point oldCorner = new Point(origRect.Left, origRect.Bottom);

            int newPosX = center.X + center.Y - oldCorner.Y;
            int newPosY = center.Y + oldCorner.X - center.X;

            return new Rectangle(newPosX, newPosY, origRect.Height, origRect.Width);
        }



        /// <summary>
        /// Logs an exception message to AppPath/ErrorLog.txt.
        /// </summary>
        /// <param name="ex"></param>
        public static void LogException(Exception ex)
        {
            String errorPath = C.AppPath + "ErrorLog.txt";
            System.IO.TextWriter textFile = new System.IO.StreamWriter(errorPath, true);
            textFile.WriteLine(ex.ToString());
            textFile.Close();
        }

        /// <summary>
        /// Logs a profiling result to AppPath/ProfilingLog.txt.
        /// </summary>
        /// <param name="Text"></param>
        public static void LogProfiling(string text)
        {
            String logPath = C.AppPath + "ProfilingLog.txt";
            System.IO.TextWriter textFile = new System.IO.StreamWriter(logPath, true);
            textFile.WriteLine(text);
            textFile.Close();
        }

        /// <summary>
        /// Translates an (A)RGB hex string to a color.
        /// </summary>
        /// <param name="hexString"></param>
        /// <returns></returns>
        public static Color HexToColor(string hexString)
        {
            hexString = hexString.Replace('#', ' ').Trim();
            if (hexString.Length == 6) hexString = "FF" + hexString;
            int argb = int.Parse(hexString, System.Globalization.NumberStyles.HexNumber);
            return Color.FromArgb(argb);
        }

        /// <summary>
        /// Returns the global instance of the random number generator.
        /// </summary>
        /// <returns></returns>
        public static Random Random()
        {
            if (rnd == null) rnd = new Random();
            return rnd;
        }
        static Random rnd;
    }


    static class Profiler
    { 
        static Profiler()
        {
            stopWatch = new System.Diagnostics.Stopwatch();
        }

        static System.Diagnostics.Stopwatch stopWatch;

        public static void Start()
        {
            stopWatch.Restart();
        }

        public static void Stop()
        {
            stopWatch.Stop();
            Utility.LogProfiling(stopWatch.ElapsedMilliseconds.ToString());
        }
    }
}
