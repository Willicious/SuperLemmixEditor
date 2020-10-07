using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

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
        /// Swaps the values of the two objects
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj1"></param>
        /// <param name="obj2"></param>
        public static void Swap<T>(ref T obj1, ref T obj2)
        {
            T temp = obj1;
            obj1 = obj2;
            obj2 = temp;
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
        /// Splits the string at all new-lines.
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static List<string> SplitAtNewLine(this string text)
        {
            return text.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None).ToList();
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
            return (((x + step / 2) / step) - (x < 0 ? 1 : 0)) * step;
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
        /// Sets the image of an existing PictureBox and disposes the used image
        /// </summary>
        /// <param name="picBox"></param>
        /// <param name="image"></param>
        public static void SetImage(this PictureBox picBox, Bitmap image)
        {
            Bitmap oldImage = picBox.Image as Bitmap;
            picBox.Image = image;
            if (oldImage != null)
                oldImage.Dispose();
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
        /// Rotates one rectangle relative to another border rectangle.
        /// </summary>
        /// <param name="origRect"></param>
        /// <param name="border"></param>
        /// <returns></returns>
        public static Rectangle RotateInRectangle(this Rectangle origRect, Rectangle border)
        {
            Point center = new Point(border.Left + border.Width / 2, border.Top + border.Height / 2);
            Point oldCorner = new Point(origRect.Left, origRect.Bottom);

            int newPosX = center.X + center.Y - oldCorner.Y;
            int newPosY = center.Y + oldCorner.X - center.X;

            return new Rectangle(newPosX, newPosY, origRect.Height, origRect.Width);
        }

        /// <summary>
        /// Handles a global unexpected exception and displays a warning message to the user.
        /// </summary>
        /// <param name="Ex"></param>
        public static void HandleGlobalException(Exception Ex)
        {
            try
            {
                LogException(Ex);
                string errorString = "Klopt niet: " + Ex.Message + C.NewLine + "Try to continue working on the level? Selecting 'no' will quit the level editor.";
                var result = MessageBox.Show(errorString, "Error", MessageBoxButtons.YesNo);
                if (result == DialogResult.No)
                    Application.Exit();
            }
            catch
            {
                Application.Exit();
            }
        }

        /// <summary>
        /// Logs an exception message to AppPath/ErrorLog.txt.
        /// </summary>
        /// <param name="ex"></param>
        public static void LogException(Exception ex)
        {
            string errorPath = C.AppPath + "ErrorLog.txt";
            System.IO.TextWriter textFile = new System.IO.StreamWriter(errorPath, true);
            textFile.WriteLine(DateTime.Now.ToString("yyyyMMddHHmmss"));
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
            if (hexString.Length == 6)
                hexString = "FF" + hexString;
            int argb = int.Parse(hexString, System.Globalization.NumberStyles.HexNumber);
            return Color.FromArgb(argb);
        }

        /// <summary>
        /// Returns the global instance of the random number generator.
        /// </summary>
        /// <returns></returns>
        public static Random Random()
        {
            if (rnd == null)
                rnd = new Random();
            return rnd;
        }
        static Random rnd;

        public static void SetDataToClipboard<T>(T data)
        {
            try
            {
                using (var stream = new System.IO.MemoryStream())
                {
                    var formatter = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
                    formatter.Serialize(stream, data);
                    DataObject dataObject = new DataObject();
                    dataObject.SetData("NLPieces", false, stream);
                    Clipboard.SetDataObject(dataObject, true);
                }
            }
            catch
            {
                return;
            }
        }

        public static T GetDataFromClipboard<T>()
        {
            try
            {
                DataObject retrievedData = Clipboard.GetDataObject() as DataObject;
                if (retrievedData == null || !retrievedData.GetDataPresent("NLPieces"))
                {
                    return default(T);
                }

                using (var stream = retrievedData.GetData("NLPieces") as System.IO.MemoryStream)
                {
                    if (stream == null)
                        return default(T);
                    stream.Seek(0, System.IO.SeekOrigin.Begin);
                    var formatter = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
                    return (T)formatter.Deserialize(stream);
                }
            }
            catch
            {
                return default(T);
            }
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
