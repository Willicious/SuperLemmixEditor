using System;
using System.Collections.Generic;
using System.Linq;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using System.Globalization;

namespace NLEditor
{
    /// <summary>
    /// Contains static methods to load styles and meta-infos of objects.
    /// </summary>
    static class LoadStylesFromFile
    {
        public static void AddInitialImagesToLibrary()
        {
            // preplaced lemming
            string imageKey = ImageLibrary.CreatePieceKey("default", "lemming", true);
            Bitmap image = Properties.Resources.Lemming;
            Rectangle triggerArea = new Rectangle(C.LEM_OFFSET_X, C.LEM_OFFSET_Y, 1, 1);
            ImageLibrary.AddNewImage(imageKey, image, C.OBJ.LEMMING, triggerArea, C.Resize.None);
        }

        static readonly Dictionary<string, C.StyleColor> KeyToStyleColorDict = new Dictionary<string, C.StyleColor>
      {
        { "BACKGROUND", C.StyleColor.BACKGROUND },
        { "MASK", C.StyleColor.MASK },
        { "ONE_WAYS", C.StyleColor.ONE_WAY_WALL },
        { "PICKUP_BORDER", C.StyleColor.PICKUP_BORDER },
        { "PICKUP_INSIDE", C.StyleColor.PICKUP_INSIDE }
      };


        /// <summary>
        /// Reads style colors from a .nxtm file.
        /// <para> Color 0: Background (default: black) </para>
        /// </summary>
        /// <param name="styleName"></param>
        /// <returns></returns>
        public static Dictionary<C.StyleColor, Color> StyleColors(string styleName)
        {
            var colorDict = new Dictionary<C.StyleColor, Color>();
            string filePath = C.AppPathThemeInfo(styleName);
            if (!File.Exists(filePath))
                return colorDict;

            FileParser parser;
            try
            {
                parser = new FileParser(filePath);
            }
            catch (Exception Ex)
            {
                Utility.LogException(Ex);
                MessageBox.Show(Ex.Message, "File corrupt");
                return colorDict;
            }


            try
            {
                List<FileLine> newFileLine;
                while ((newFileLine = parser.GetNextLines()) != null)
                {
                    foreach (string key in KeyToStyleColorDict.Keys)
                    {
                        FileLine colorLine = newFileLine.Find(line => line.Key == key);
                        if (colorLine != null)
                        {
                            string colorString = colorLine.Text;
                            if (colorString.StartsWith("x"))
                                colorString = colorString.Substring(1);
                            try
                            {
                                colorDict[KeyToStyleColorDict[key]] = ColorTranslator.FromHtml("#" + colorString);
                            }
                            catch
                            {
                                // ignore the problematic color
                            }
                        }
                    }
                }
            }
            catch (Exception Ex)
            {
                // log, but otherwise ignore the exception
                Utility.LogException(Ex);
                MessageBox.Show(Ex.Message, "Error reading color file for " + styleName);
            }
            finally
            {
                parser.DisposeStreamReader();
            }

            return colorDict;
        }


        /// <summary>
        /// Reads the styles.ini file and orders and renames styles accordingly.
        /// </summary>
        /// <param name="styleList"></param>
        /// <returns></returns>
        public static List<Style> OrderAndRenameStyles(List<Style> styleList)
        {
            string filePath = C.AppPath + "styles" + C.DirSep + "styles.ini";

            if (!File.Exists(filePath))
                return styleList;

            // Otherwise order the styles according to styles.ini
            Dictionary<string, float> styleOrderDict;
            Dictionary<string, string> newStyleNameDict;
            ReadStyleOrderFromFile(filePath, out styleOrderDict, out newStyleNameDict);

            // Rename all custom names
            foreach (string styleFileName in newStyleNameDict.Keys)
            {
                Style curStyle = styleList.Find(sty => sty.NameInDirectory.Equals(styleFileName));
                if (curStyle != null)
                {
                    curStyle.NameInEditor = newStyleNameDict[styleFileName];
                }
            }

            // Reorder the styles
            styleList.Sort((sty1, sty2) =>
              {
                  if (styleOrderDict.ContainsKey(sty1.NameInDirectory) && styleOrderDict.ContainsKey(sty2.NameInDirectory))
                  {
                      return styleOrderDict[sty1.NameInDirectory].CompareTo(styleOrderDict[sty2.NameInDirectory]);
                  }
                  else if (styleOrderDict.ContainsKey(sty1.NameInDirectory))
                  {
                      return -1;
                  }
                  else if (styleOrderDict.ContainsKey(sty2.NameInDirectory))
                  {
                      return 1;
                  }
                  else
                  {
                      return styleList.FindIndex(sty => sty == sty1).CompareTo(styleList.FindIndex(sty => sty == sty2));
                  }
              });

            return styleList;
        }

        /// <summary>
        /// Reads a style file and returns new positions and custom names for styles.
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="styleOrderDict"></param>
        /// <param name="newStyleNameDict"></param>
        private static void ReadStyleOrderFromFile(string filePath, out Dictionary<string, float> styleOrderDict,
                                                                    out Dictionary<string, string> newStyleNameDict)
        {
            styleOrderDict = new Dictionary<string, float>();
            newStyleNameDict = new Dictionary<string, string>();

            StreamReader fileStream = null;
            string styleFileName = null;

            try
            {
                fileStream = new StreamReader(filePath);

                while (true)
                {
                    string line = fileStream.ReadLine()?.Trim();

                    if (line == null)
                        break;

                    if (line.StartsWith("["))
                    {
                        styleFileName = line.Substring(1, line.Length - 2);
                    }
                    else if (line.ToUpper().StartsWith("NAME") && line.Length > 5)
                    {
                        string styleNewName = line.Substring(5).Trim();
                        if (styleFileName != null)
                        {
                            newStyleNameDict[styleFileName] = styleNewName;
                        }
                    }
                    else if (line.ToUpper().StartsWith("ORDER") && line.Length > 6)
                    {
                        float styleNewPos;
                        if (styleFileName != null &&
                            float.TryParse(line.Substring(6).Trim(), NumberStyles.Number, CultureInfo.InvariantCulture, out styleNewPos))
                        {
                            styleOrderDict[styleFileName] = styleNewPos;
                        }
                    }
                }
            }
            finally
            {
                fileStream?.Dispose();
            }
        }

        /// <summary>
        /// Loads a .png image or null if the image could not be loaded.
        /// </summary>
        /// <param name="imageKey"></param>
        /// <returns></returns>
        public static Bitmap Image(string imageKey)
        {
            string imagePath;

            if (Path.IsPathRooted(imageKey))
                imagePath = imageKey;
            else
                imagePath = C.AppPathPieces + imageKey;

            try
            {
                return Utility.CreateBitmapFromFile(imagePath + ".png");
            }
            catch (Exception Ex)
            {
                Utility.LogException(Ex);
                return null;
            }
        }

        /// <summary>
        /// Reads further piece infos from a .nxob resp. nxtp file.
        /// <para> Returns a finished BaseImageInfo containing both the image and the further info. <\para> 
        /// </summary>
        /// <param name="image"></param>
        /// <param name="imageName"></param>
        /// <returns></returns>
        public static BaseImageInfo ImageInfo(string imageName)
        {
            string imagePath = C.AppPathPieces + imageName;

            if (File.Exists(imagePath + ".nxmo"))
            {
                // create a new object piece
                return CreateNewObjectInfo(imagePath);
            }
            else
            {
                // create a new terrain piece
                return CreateNewTerrainInfo(imagePath); // This can handle the NXMT file not existing.
            }
        }

        /// <summary>
        /// Reads further object infos from a .nxob file. Default values:
        /// <para> NumFrames = 1 </para>
        /// <para> ObjType = C.OBJ.NONE </para>
        /// <para> TriggerRect = Rectangle(0, 0, 0, 0) </para>
        /// </summary>
        /// <param name="newBitmap"></param>
        /// <param name="FilePathInfo"></param>
        /// <returns></returns>
        private static BaseImageInfo CreateNewObjectInfo(string filePath)
        {
            C.OBJ objType = C.OBJ.NONE;
            Rectangle triggerRect = new Rectangle(0, 0, 1, 1);
            C.Resize resizeMode = C.Resize.None;
            bool isDeprecated = false;

            int[] nineSliceSizes = new int[4]; // Not appropriate to use a Rectangle yet. File contains the width/height of the slice,
                                               // not a rectangle of the center; and we don't know the width of the object yet. Order
                                               // in the array is Left, Top, Right, Bottom.

            List<LoadStyleAnimData> animData = new List<LoadStyleAnimData>();
            LoadStyleAnimData primaryAnim = new LoadStyleAnimData()
            {
                Frame = -1,
                ZIndex = 1
            };

            animData.Add(primaryAnim);

            FileParser parser = null;
            try
            {
                parser = new FileParser(filePath + ".nxmo");

                List<FileLine> fileLineList;
                while ((fileLineList = parser.GetNextLines()) != null)
                {
                    System.Diagnostics.Debug.Assert(fileLineList.Count > 0, "FileParser returned empty list.");

                    FileLine line = fileLineList[0];
                    switch (line.Key)
                    {
                        case "FRAMES":
                            primaryAnim.Frames = line.Value;
                            break;
                        case "TRIGGER_X":
                            triggerRect.X = line.Value;
                            break;
                        case "TRIGGER_Y":
                            triggerRect.Y = line.Value;
                            break;
                        case "TRIGGER_WIDTH":
                            triggerRect.Width = line.Value;
                            break;
                        case "TRIGGER_HEIGHT":
                            triggerRect.Height = line.Value;
                            break;
                        case "HORIZONTAL_STRIP":
                            primaryAnim.HorizontalStrip = true;
                            break;
                        case "INITIAL_FRAME":
                            primaryAnim.Frame = line.Value;
                            break;
                        case "RESIZE_VERTICAL":
                            resizeMode = resizeMode.In(C.Resize.Horiz, C.Resize.Both) ? C.Resize.Both : C.Resize.Vert;
                            break;
                        case "RESIZE_HORIZONTAL":
                            resizeMode = resizeMode.In(C.Resize.Vert, C.Resize.Both) ? C.Resize.Both : C.Resize.Horiz;
                            break;
                        case "RESIZE_BOTH":
                            resizeMode = C.Resize.Both;
                            break;
                        case "NINE_SLICE_LEFT":
                            nineSliceSizes[0] = line.Value;
                            break;
                        case "NINE_SLICE_TOP":
                            nineSliceSizes[1] = line.Value;
                            break;
                        case "NINE_SLICE_RIGHT":
                            nineSliceSizes[2] = line.Value;
                            break;
                        case "NINE_SLICE_BOTTOM":
                            nineSliceSizes[3] = line.Value;
                            break;
                        case "DEPRECATED":
                            isDeprecated = true;
                            break;

                        case "WINDOW":
                            objType = C.OBJ.HATCH;
                            break;
                        case "EXIT":
                            objType = C.OBJ.EXIT;
                            break;
                        case "TRAP":
                            objType = C.OBJ.TRAP;
                            break;
                        case "SINGLE_USE_TRAP":
                            objType = C.OBJ.TRAPONCE;
                            break;
                        case "WATER":
                            objType = C.OBJ.WATER;
                            break;
                        case "FIRE":
                            objType = C.OBJ.FIRE;
                            break;
                        case "ONE_WAY_RIGHT":
                        case "ONE_WAY_LEFT":
                        case "ONE_WAY_DOWN":
                        case "ONE_WAY_UP":
                            objType = C.OBJ.ONE_WAY_WALL;
                            break;
                        case "BUTTON":
                            objType = C.OBJ.BUTTON;
                            break;
                        case "LOCKED_EXIT":
                            objType = C.OBJ.EXIT_LOCKED;
                            break;
                        case "PICKUP_SKILL":
                            objType = C.OBJ.PICKUP;
                            break;
                        case "TELEPORTER":
                            objType = C.OBJ.TELEPORTER;
                            break;
                        case "RECEIVER":
                            objType = C.OBJ.RECEIVER;
                            break;
                        case "SPLITTER":
                            objType = C.OBJ.SPLITTER;
                            break;
                        case "UPDRAFT":
                            objType = C.OBJ.UPDRAFT;
                            break;
                        case "ANTISPLATPAD":
                        case "SPLATPAD":
                            objType = C.OBJ.SPLAT;
                            break;
                        case "FORCE_RIGHT":
                        case "FORCE_LEFT":
                            objType = C.OBJ.FORCE_FIELD;
                            break;
                        case "BACKGROUND":
                        case "MOVING_BACKGROUND":
                            objType = C.OBJ.BACKGROUND;
                            break;

                        case "EFFECT":
                            switch (line.Text.Trim().ToUpperInvariant())
                            {
                                case "ENTRANCE":
                                    objType = C.OBJ.HATCH;
                                    break;
                                case "EXIT":
                                    objType = C.OBJ.EXIT;
                                    break;
                                case "TRAP":
                                    objType = C.OBJ.TRAP;
                                    break;
                                case "TRAPONCE":
                                    objType = C.OBJ.TRAPONCE;
                                    break;
                                case "WATER":
                                    objType = C.OBJ.WATER;
                                    break;
                                case "FIRE":
                                    objType = C.OBJ.FIRE;
                                    break;
                                case "ONEWAYRIGHT":
                                case "ONEWAYLEFT":
                                case "ONEWAYDOWN":
                                case "ONEWAYUP":
                                    objType = C.OBJ.ONE_WAY_WALL;
                                    break;
                                case "UNLOCKBUTTON":
                                    objType = C.OBJ.BUTTON;
                                    break;
                                case "LOCKEDEXIT":
                                    objType = C.OBJ.EXIT_LOCKED;
                                    break;
                                case "PICKUPSKILL":
                                    objType = C.OBJ.PICKUP;
                                    break;
                                case "TELEPORTER":
                                    objType = C.OBJ.TELEPORTER;
                                    break;
                                case "RECEIVER":
                                    objType = C.OBJ.RECEIVER;
                                    break;
                                case "SPLITTER":
                                    objType = C.OBJ.SPLITTER;
                                    break;
                                case "UPDRAFT":
                                    objType = C.OBJ.UPDRAFT;
                                    break;
                                case "ANTISPLATPAD":
                                case "SPLATPAD":
                                    objType = C.OBJ.SPLAT;
                                    break;
                                case "FORCERIGHT":
                                case "FORCELEFT":
                                    objType = C.OBJ.FORCE_FIELD;
                                    break;
                                case "BACKGROUND":
                                    objType = C.OBJ.BACKGROUND;
                                    break;
                            }
                            break;

                        case "PRIMARY_ANIMATION":
                            foreach (var fileLine in fileLineList)
                            {
                                switch (fileLine.Key)
                                {
                                    case "NAME":
                                        primaryAnim.Name = fileLine.Text;
                                        break;
                                    case "FRAMES":
                                        primaryAnim.Frames = fileLine.Value;
                                        break;
                                    case "HORIZONTAL_STRIP":
                                        primaryAnim.HorizontalStrip = true;
                                        break;
                                    case "Z_INDEX":
                                        primaryAnim.ZIndex = fileLine.Value;
                                        break;
                                    case "INITIAL_FRAME":
                                        primaryAnim.Frame = Math.Max(0, fileLine.Value);
                                        break;
                                    case "OFFSET_X":
                                        primaryAnim.OffsetX = fileLine.Value;
                                        break;
                                    case "OFFSET_Y":
                                        primaryAnim.OffsetY = fileLine.Value;
                                        break;
                                    case "HIDDEN":
                                        primaryAnim.Hidden = true;
                                        break;
                                    case "NINE_SLICE_LEFT":
                                        nineSliceSizes[0] = line.Value;
                                        break;
                                    case "NINE_SLICE_TOP":
                                        nineSliceSizes[1] = line.Value;
                                        break;
                                    case "NINE_SLICE_RIGHT":
                                        nineSliceSizes[2] = line.Value;
                                        break;
                                    case "NINE_SLICE_BOTTOM":
                                        nineSliceSizes[3] = line.Value;
                                        break;
                                }
                            }
                            break;

                        case "ANIMATION":
                            LoadStyleAnimData newAnim = new LoadStyleAnimData();
                            foreach (var fileLine in fileLineList)
                            {
                                switch (fileLine.Key)
                                {
                                    case "NAME":
                                        newAnim.Name = fileLine.Text;
                                        break;
                                    case "FRAMES":
                                        newAnim.Frames = fileLine.Value;
                                        break;
                                    case "HORIZONTAL_STRIP":
                                        newAnim.HorizontalStrip = true;
                                        break;
                                    case "Z_INDEX":
                                        newAnim.ZIndex = fileLine.Value;
                                        break;
                                    case "INITIAL_FRAME":
                                        newAnim.Frame = Math.Max(0, fileLine.Value);
                                        break;
                                    case "OFFSET_X":
                                        newAnim.OffsetX = fileLine.Value;
                                        break;
                                    case "OFFSET_Y":
                                        newAnim.OffsetY = fileLine.Value;
                                        break;
                                    case "HIDE":
                                        newAnim.Hidden = true;
                                        break;
                                }
                            }

                            animData.Add(newAnim);
                            break;
                    }
                }
            }
            catch (Exception Ex)
            {
                Utility.LogException(Ex);
                MessageBox.Show(Ex.Message, "File corrupt");
            }
            finally
            {
                parser?.DisposeStreamReader();
            }

            Bitmap newBitmap = CreateCompositeImage(filePath, animData, primaryAnim,
              out int marginLeft, out int marginTop, out int marginRight, out int marginBottom);

            // Convert the nine-slice sizes to a nine-slice center rectangle
            Rectangle? nineSliceRect;
            if (nineSliceSizes.Any(size => size != 0))
            {
                int oneFrameWidth = primaryAnim.Width;
                int oneFrameHeight = primaryAnim.Height;

                nineSliceRect = new Rectangle(nineSliceSizes[0] + marginLeft, nineSliceSizes[1] + marginTop,
                                              oneFrameWidth - nineSliceSizes[0] - nineSliceSizes[2] - marginRight,
                                              oneFrameHeight - nineSliceSizes[1] - nineSliceSizes[3] - marginBottom);
            }
            else
                nineSliceRect = null;

            triggerRect.Offset(marginLeft, marginTop);

            return new BaseImageInfo(newBitmap, objType, primaryAnim.Frames, triggerRect, resizeMode,
              marginLeft, marginTop, marginRight, marginBottom, isDeprecated, nineSliceRect);
        }

        public static Bitmap CreateCompositeImage(string filePath, List<LoadStyleAnimData> anims, LoadStyleAnimData primaryAnim,
          out int marginLeft, out int marginTop, out int marginRight, out int marginBottom)
        {
            int minX = 0;
            int minY = 0;
            int maxX = 0;
            int maxY = 0;

            var localAnims = anims.OrderBy(anim => anim.ZIndex);

            foreach (var anim in localAnims)
            {
                if (anim.Name?.ToUpperInvariant() == "*PICKUP")
                {
                    anim.Image = Properties.Resources.PickupAnim;
                    anim.Frames = 36;

                    Bitmap eraseImage;
                    var eraseAnim = localAnims.FirstOrDefault(item => item.Name == "skill_mask");
                    if (eraseAnim != null)
                    {
                        eraseAnim.Image = Image(filePath + "_skill_mask");
                        eraseImage = eraseAnim.Image;
                    }
                    else
                    {
                        eraseImage = new Bitmap(24, 48);
                        eraseImage.DrawOnFilledRectangles(new List<Rectangle>() { new Rectangle(0, 0, 24, 24) }, Color.FromArgb(255, 0, 0, 0));
                    }

                    for (int n = 0; n < anim.Frames; n++)
                        anim.Image.DrawOn(eraseAnim.Image.Crop(new Rectangle(0, (n % 2) * 24, 24, 24)), new Point(0, n * 24), C.CustDrawMode.Erase);
                }
                else if (anim.Image == null)
                {
                    anim.Image = Image(filePath + (string.IsNullOrEmpty(anim.Name) ? "" : "_" + anim.Name));
                }

                if (anim.HorizontalStrip)
                {
                    anim.Width = anim.Image.Width / anim.Frames;
                    anim.Height = anim.Image.Height;
                }
                else
                {
                    anim.Width = anim.Image.Width;
                    anim.Height = anim.Image.Height / anim.Frames;
                }

                minX = Math.Min(minX, anim.OffsetX);
                minY = Math.Min(minY, anim.OffsetY);
                maxX = Math.Max(maxX, anim.OffsetX + anim.Width);
                maxY = Math.Max(maxY, anim.OffsetY + anim.Height);
            }

            marginLeft = -minX;
            marginTop = -minY;
            marginRight = maxX - primaryAnim.Width;
            marginBottom = maxY - primaryAnim.Height;

            Bitmap result = new Bitmap(maxX - minX, (maxY - minY) * primaryAnim.Frames);

            for (int n = 0; n < primaryAnim.Frames; n++)
            {
                foreach (var anim in localAnims)
                {
                    if (anim.Hidden)
                        continue;

                    Rectangle srcRect = new Rectangle(0, 0, anim.Width, anim.Height);
                    int doFrame = anim.Frame >= 0 ? anim.Frame : n;

                    if (anim.HorizontalStrip)
                        srcRect.X = anim.Width * doFrame;
                    else
                        srcRect.Y = anim.Height * doFrame;

                    result.DrawOn(anim.Image.Crop(srcRect), new Point(anim.OffsetX + marginLeft, anim.OffsetY + marginTop + ((maxY - minY) * n)));
                }
            }

            return result;
        }

        /// <summary>
        /// Reads further terrain infos from a .nxtp file. Default values:
        /// <para> IsSteel = false </para>
        /// </summary>
        /// <param name="newBitmap"></param>
        /// <param name="FilePathInfo"></param>
        /// <returns></returns>
        private static BaseImageInfo CreateNewTerrainInfo(string filePath)
        {
            bool IsSteel = false;
            bool isDeprecated = false;

            if (File.Exists(filePath + ".nxmt"))
            {
                FileParser parser = null;
                try
                {
                    parser = new FileParser(filePath + ".nxmt");

                    List<FileLine> fileLineList;
                    while ((fileLineList = parser.GetNextLines()) != null)
                    {
                        System.Diagnostics.Debug.Assert(fileLineList.Count > 0, "FileParser returned empty list.");

                        FileLine line = fileLineList[0];
                        switch (line.Key)
                        {
                            case "STEEL":
                                IsSteel = true;
                                break;
                            case "DEPRECATED":
                                isDeprecated = true;
                                break;
                        }
                    }
                }
                catch (Exception Ex)
                {
                    Utility.LogException(Ex);
                    MessageBox.Show(Ex.Message, "File corrupt");
                }
                finally
                {
                    parser?.DisposeStreamReader();
                }
            }

            Bitmap newBitmap = Image(filePath);

            return new BaseImageInfo(newBitmap, IsSteel, isDeprecated);
        }
    }
}
