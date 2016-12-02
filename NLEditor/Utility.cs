using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Windows.Forms;
using System.Reflection;

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
        /// <param name="MyList"></param>
        /// <param name="Index1"></param>
        /// <param name="Index2"></param>
        public static void Swap<T>(this IList<T> MyList, int Index1, int Index2)
        {
            T item = MyList[Index1];
            MyList[Index1] = MyList[Index2];
            MyList[Index2] = item;
        }

        /// <summary>
        /// Loads a bitmap from a file and closes the file again.
        /// </summary>
        /// <param name="FilePath"></param>
        /// <returns></returns>
        public static Bitmap CreateBitmapFromFile(string FilePath)
        {
            Bitmap MyImage;
            using (var TempImage = new Bitmap(FilePath))
            {
                MyImage = new Bitmap(TempImage);
            }
            return MyImage;
        }



        /// <summary>
        /// Creates a new rectangle having the two points as diagonally opposite vertices.
        /// </summary>
        /// <param name="Pos1"></param>
        /// <param name="Pos2"></param>
        /// <returns></returns>
        public static Rectangle RectangleFrom(Point Pos1, Point Pos2)
        {
            int Left = Math.Min(Pos1.X, Pos2.X);
            int Top = Math.Min(Pos1.Y, Pos2.Y);
            int Width = Math.Abs(Pos1.X - Pos2.X);
            int Height = Math.Abs(Pos1.Y - Pos2.Y);

            return new Rectangle(Left, Top, Width, Height);
        }

        /// <summary>
        /// Logs an exception message to AppPath/ErrorLog.txt.
        /// </summary>
        /// <param name="Ex"></param>
        public static void LogException(Exception Ex)
        {
            String ErrorPath = C.AppPath + "ErrorLog.txt";
            System.IO.TextWriter TextFile = new System.IO.StreamWriter(ErrorPath, true);
            TextFile.WriteLine(Ex.ToString());
            TextFile.Close();
        }

        /// <summary>
        /// Logs a profiling result to AppPath/ProfilingLog.txt.
        /// </summary>
        /// <param name="Time"></param>
        public static void LogProfiling(long Time)
        {
            String LogPath = C.AppPath + "ProfilingLog.txt";
            System.IO.TextWriter TextFile = new System.IO.StreamWriter(LogPath, true);
            TextFile.WriteLine(Time.ToString());
            TextFile.Close();
        }

    }


    static class Profiler
    { 
        static Profiler()
        {
            fStopWatch = new System.Diagnostics.Stopwatch();
        }

        static System.Diagnostics.Stopwatch fStopWatch;

        public static void Start()
        {
            fStopWatch.Restart();
        }

        public static void Stop()
        {
            fStopWatch.Stop();
            Utility.LogProfiling(fStopWatch.ElapsedMilliseconds);
        }
    }
}
