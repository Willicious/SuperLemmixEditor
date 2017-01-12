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
