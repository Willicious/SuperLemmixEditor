using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace SLXEditor
{
    /// <summary>
    /// Contains static methods to load styles and meta-infos of objects.
    /// </summary>
    static class LoadStylesFromFile
    {
        public static void AddInitialImagesToLibrary()
        {
            // Preplaced lemming
            string imageKey = ImageLibrary.CreatePieceKey("default", "lemming", true);
            Bitmap image = Properties.Resources.Lemming;
            Rectangle triggerArea = new Rectangle(C.LEM_OFFSET_X, C.LEM_OFFSET_Y, 1, 1);
            ImageLibrary.AddNewImage(imageKey, image, C.OBJ.LEMMING, triggerArea, C.Resize.None);
        }
        public static void AddRulersToLibrary()
        {
            Rectangle triggerArea = new Rectangle(0, 0, 0, 0);

            if (!Directory.Exists(C.AppPathRulers))
            {
                if (Directory.Exists(C.AppPath + "\\sketches\\")) // Backwards compatibility
                {
                    string parentPath = C.AppPath;
                    string oldName = "sketches";
                    string newName = "rulers";

                    string oldPath = Path.Combine(parentPath, oldName);
                    string newPath = Path.Combine(parentPath, newName);

                    Directory.Move(oldPath, newPath); // Rename the folder to "rulers"
                }
                else
                {
                    MessageBox.Show("Rulers directory not found!");
                    return;
                }
            }

            foreach (string file in Directory.EnumerateFiles(C.AppPathRulers))
            {
                string name = Path.GetFileNameWithoutExtension(file);

                using (Bitmap img = new Bitmap(file))
                {
                    string key = "rulers\\" + name;
                    ImageLibrary.AddNewImage(key, img, C.OBJ.RULER, triggerArea, C.Resize.None);
                    ImageLibrary.RegisterRuler(key);
                }
            }
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
        public static List<Style> OrderAndRenameStyles(List<Style> styleList, Settings settings)
        {
            string filePath = C.AppPath + "styles" + C.DirSep + "styles.ini";

            // Hard-coded "slx_" style names and order
            List<string> slxOrder = new List<string>
            {
                "slx_crystal",
                "slx_dirt",
                "slx_fire",
                "slx_marble",
                "slx_pillar",
                "slx_brick",
                "slx_bubble",
                "slx_rock",
                "slx_snow",
                "xmas"
            };

            Dictionary<string, string> slxNameOverrides = new Dictionary<string, string>
            {
                { "slx_crystal", "Crystal" },
                { "slx_dirt", "Dirt" },
                { "slx_fire", "Fire" },
                { "slx_marble", "Marble" },
                { "slx_pillar", "Pillar" },
                { "slx_brick", "Brick" },
                { "slx_bubble", "Bubble" },
                { "slx_rock", "Rock" },
                { "slx_snow", "Snow" },
                { "xmas", "Christmas" }
            };

            // Capture original indices for fallback ordering
            Dictionary<Style, int> originalIndices = styleList
                .Select((sty, idx) => new { sty, idx })
                .ToDictionary(x => x.sty, x => x.idx);

            // Optional: read styles.ini
            Dictionary<string, float> styleOrderDict = new Dictionary<string, float>();
            Dictionary<string, string> newStyleNameDict = new Dictionary<string, string>();
            if (File.Exists(filePath))
            {
                ReadStyleOrderFromFile(filePath, out styleOrderDict, out newStyleNameDict);

                // Rename all styles according to styles.ini
                foreach (string styleFileName in newStyleNameDict.Keys)
                {
                    Style curStyle = styleList.Find(sty => sty.NameInDirectory.Equals(styleFileName));
                    if (curStyle != null)
                        curStyle.NameInEditor = newStyleNameDict[styleFileName];
                }
            }

            // Override slx_ style display names if applicable
            if (settings.AutoPinOGStyles)
            {
                foreach (var kvp in slxNameOverrides)
                {
                    Style curStyle = styleList.Find(sty => sty.NameInDirectory.Equals(kvp.Key));
                    if (curStyle != null)
                        curStyle.NameInEditor = kvp.Value;
                }
            }

            // Sort styles: slx_ first in defined order, then styles.ini, then original order
            styleList.Sort((sty1, sty2) =>
            {
                if (settings.AutoPinOGStyles)
                {
                    int sty1SlxIndex = slxOrder.IndexOf(sty1.NameInDirectory);
                    int sty2SlxIndex = slxOrder.IndexOf(sty2.NameInDirectory);

                    if (sty1SlxIndex != -1 && sty2SlxIndex != -1)
                        return sty1SlxIndex.CompareTo(sty2SlxIndex);
                    if (sty1SlxIndex != -1)
                        return -1;
                    if (sty2SlxIndex != -1)
                        return 1;
                }

                if (styleOrderDict.ContainsKey(sty1.NameInDirectory) && styleOrderDict.ContainsKey(sty2.NameInDirectory))
                    return styleOrderDict[sty1.NameInDirectory].CompareTo(styleOrderDict[sty2.NameInDirectory]);
                if (styleOrderDict.ContainsKey(sty1.NameInDirectory))
                    return -1;
                if (styleOrderDict.ContainsKey(sty2.NameInDirectory))
                    return 1;

                // Fallback to original order from the passed styleList
                return originalIndices[sty1].CompareTo(originalIndices[sty2]);
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
        /// Reads further piece infos from a .nxmo/.nxmt file.
        /// Returns a finished BaseImageInfo containing both the image and the further info. <\para> 
        /// </summary>
        public static BaseImageInfo ImageInfo(string imageName)
        {
            if (imageName.Contains("rulers\\"))
                return null; // rulers don't have additional info

            string imagePath = C.AppPathPieces + imageName;

            if (File.Exists(imagePath + ".nxmo"))
            {
                // get object-specific info (needs .nxmo)
                return CreateNewObjectInfo(imagePath);
            }
            else
            {
                // get terrain-specific info (can handle missing .nxmt)
                return CreateNewTerrainInfo(imagePath);
            }
        }

        /// <summary>
        /// Reads further object infos from a .nxmo file.
        /// </summary>
        private static BaseImageInfo CreateNewObjectInfo(string filePath)
        {
            C.OBJ objType = C.OBJ.NONE;
            Rectangle triggerRect = new Rectangle(0, 0, 1, 1);
            C.Resize resizeMode = C.Resize.None;
            bool isDeprecated = false;
            bool? cropOverride = null;
            int defaultWidth = 0;
            int defaultHeight = 0;
            int markerX = 0, markerY = 0;

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
                        case "RESIZE_VERTICAL":
                            resizeMode = resizeMode.In(C.Resize.Horiz, C.Resize.Both) ? C.Resize.Both : C.Resize.Vert;
                            break;
                        case "RESIZE_HORIZONTAL":
                            resizeMode = resizeMode.In(C.Resize.Vert, C.Resize.Both) ? C.Resize.Both : C.Resize.Horiz;
                            break;
                        case "RESIZE_BOTH":
                            resizeMode = C.Resize.Both;
                            break;
                        case "DEFAULT_WIDTH":
                            defaultWidth = line.Value;
                            break;
                        case "DEFAULT_HEIGHT":
                            defaultHeight = line.Value;
                            break;
                        case "MARKER_X":
                            markerX = line.Value;
                            break;
                        case "MARKER_Y":
                            markerY = line.Value;
                            break;
                        case "DEPRECATED":
                            isDeprecated = true;
                            break;
                        case "EDITOR_CROP":
                            if (bool.TryParse(line.Text, out bool localCrop))
                                cropOverride = localCrop;
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
                                case "BLASTICINE":
                                    objType = C.OBJ.BLASTICINE;
                                    break;
                                case "VINEWATER":
                                    objType = C.OBJ.VINEWATER;
                                    break;
                                case "POISON":
                                    objType = C.OBJ.POISON;
                                    break;
                                case "LAVA":
                                    objType = C.OBJ.LAVA;
                                    break;
                                case "RADIATION":
                                    objType = C.OBJ.RADIATION;
                                    break;
                                case "SLOWFREEZE":
                                    objType = C.OBJ.SLOWFREEZE;
                                    break;
                                case "ONEWAYRIGHT":
                                case "ONEWAYLEFT":
                                case "ONEWAYDOWN":
                                case "ONEWAYUP":
                                    objType = C.OBJ.ONE_WAY_WALL;
                                    break;
                                case "PAINT":
                                    objType = C.OBJ.PAINT;
                                    break;
                                case "COLLECTIBLE":
                                    objType = C.OBJ.COLLECTIBLE;
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
                                case "DECORATION":
                                    objType = C.OBJ.DECORATION;
                                    break;
                                case "PORTAL":
                                    objType = C.OBJ.PORTAL;
                                    break;
                                case "NEUTRALIZER":
                                    objType = C.OBJ.NEUTRALIZER;
                                    break;
                                case "DENEUTRALIZER":
                                    objType = C.OBJ.DENEUTRALIZER;
                                    break;
                                case "NORMALIZER":
                                    objType = C.OBJ.DENEUTRALIZER;
                                    break;
                                case "ADDSKILL":
                                    objType = C.OBJ.PERMASKILL_ADD;
                                    break;
                                case "REMOVESKILLS":
                                    objType = C.OBJ.PERMASKILL_REMOVE;
                                    break;
                                case "SKILLASSIGNER":
                                    objType = C.OBJ.SKILL_ASSIGNER;
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
                                        nineSliceSizes[0] = fileLine.Value;
                                        break;
                                    case "NINE_SLICE_TOP":
                                        nineSliceSizes[1] = fileLine.Value;
                                        break;
                                    case "NINE_SLICE_RIGHT":
                                        nineSliceSizes[2] = fileLine.Value;
                                        break;
                                    case "NINE_SLICE_BOTTOM":
                                        nineSliceSizes[3] = fileLine.Value;
                                        break;
                                    case "WIDTH":
                                        primaryAnim.Width = fileLine.Value;
                                        break;
                                    case "HEIGHT":
                                        primaryAnim.Height = fileLine.Value;
                                        break;
                                }
                            }
                            break;

                        case "ANIMATION":
                            LoadStyleAnimData newAnim = new LoadStyleAnimData();

                            bool editorHide = false;

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
                                    case "WIDTH":
                                        newAnim.Width = fileLine.Value;
                                        break;
                                    case "HEIGHT":
                                        newAnim.Height = fileLine.Value;
                                        break;
                                    case "EDITOR_HIDE":
                                        editorHide = true;
                                        break;
                                }
                            }

                            if (!editorHide)
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

            bool disableCrop;
            if (cropOverride.HasValue)
                disableCrop = !cropOverride.Value;
            else
                disableCrop = (resizeMode != C.Resize.None) || ((triggerRect.Width == primaryAnim.Width) && (triggerRect.Height == primaryAnim.Height));

            Bitmap newBitmap = CreateCompositeImage(filePath, animData, primaryAnim,
              out int marginLeft, out int marginTop, out int marginRight, out int marginBottom,
              disableCrop);

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
              marginLeft, marginTop, marginRight, marginBottom, isDeprecated, nineSliceRect,
              defaultWidth, defaultHeight, markerX, markerY);
        }

        public static Bitmap CreateCompositeImage(string filePath, List<LoadStyleAnimData> anims, LoadStyleAnimData primaryAnim,
          out int marginLeft, out int marginTop, out int marginRight, out int marginBottom, bool forceOriginalSize = false)
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
                    anim.Frames = 54;

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
                else if (anim.Name?.ToUpperInvariant() == "*BLANK")
                {
                    anim.Frames = Math.Max(anim.Frames, 1);
                    anim.Width = Math.Max(anim.Width, 1);
                    anim.Height = Math.Max(anim.Height, 1);
                    anim.Image = new Bitmap(anim.Width, anim.Height * anim.Frames);
                    anim.HorizontalStrip = false;
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

            int minSolidX;
            int minSolidY;
            int maxSolidX;
            int maxSolidY;

            if (forceOriginalSize)
            {
                minSolidX = -minX + primaryAnim.OffsetX;
                minSolidY = -minY + primaryAnim.OffsetY;
                maxSolidX = minSolidX + primaryAnim.Width - 1;
                maxSolidY = minSolidY + primaryAnim.Height - 1;
            }
            else
            {
                minSolidX = result.Width - 1;
                minSolidY = result.Height - 1;
                maxSolidX = 0;
                maxSolidY = 0;

                for (int y = 0; y < result.Height / primaryAnim.Frames; y++)
                    for (int x = 0; x < result.Width; x++)
                        for (int f = 0; f < primaryAnim.Frames; f++)
                            if (result.GetPixel(x, y + (f * result.Height / primaryAnim.Frames)).A > 0)
                            {
                                minSolidX = Math.Min(minSolidX, x);
                                minSolidY = Math.Min(minSolidY, y);
                                maxSolidX = Math.Max(maxSolidX, x);
                                maxSolidY = Math.Max(maxSolidY, y);
                            }


                while (maxSolidX - minSolidX < 7)
                {
                    maxSolidX++;
                    minSolidX--;
                }

                while (maxSolidY - minSolidY < 7)
                {
                    maxSolidY++;
                    minSolidY--;
                }

                minSolidX = Math.Max(minSolidX, 0);
                minSolidY = Math.Max(minSolidY, 0);
                maxSolidX = Math.Min(maxSolidX, result.Width - 1);
                maxSolidY = Math.Min(maxSolidY, (result.Height / primaryAnim.Frames) - 1);
            }

            marginLeft -= minSolidX;
            marginTop -= minSolidY;
            marginRight -= result.Width - maxSolidX - 1;
            marginBottom -= (result.Height / primaryAnim.Frames) - maxSolidY - 1;

            Bitmap oldResult = result;
            result = new Bitmap(maxSolidX - minSolidX + 1, (maxSolidY - minSolidY + 1) * primaryAnim.Frames);

            for (int i = 0; i < primaryAnim.Frames; i++)
            {
                result.DrawOn(oldResult.Crop(new Rectangle(
                    minSolidX,
                    minSolidY + (i * oldResult.Height / primaryAnim.Frames),
                    maxSolidX - minSolidX + 1,
                    maxSolidY - minSolidY + 1
                    )),
                    new Point(0, i * (result.Height / primaryAnim.Frames)));
            }

            return result;
        }

        /// <summary>
        /// Reads further terrain infos from a .nxmt file.
        /// </summary>
        private static BaseImageInfo CreateNewTerrainInfo(string filePath)
        {
            bool IsSteel = false;
            bool isDeprecated = false;
            C.Resize Resize = C.Resize.None;
            int defaultWidth = 0;
            int defaultHeight = 0;
            int nineSliceLeft = 0;
            int nineSliceTop = 0;
            int nineSliceRight = 0;
            int nineSliceBottom = 0;

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
                            case "RESIZE_HORIZONTAL":
                                Resize = (Resize == C.Resize.None || Resize == C.Resize.Horiz) ? C.Resize.Horiz : C.Resize.Both;
                                break;
                            case "RESIZE_VERTICAL":
                                Resize = (Resize == C.Resize.None || Resize == C.Resize.Vert) ? C.Resize.Vert : C.Resize.Both;
                                break;
                            case "RESIZE_BOTH":
                                Resize = C.Resize.Both;
                                break;
                            case "DEFAULT_WIDTH":
                                defaultWidth = line.Value;
                                break;
                            case "DEFAULT_HEIGHT":
                                defaultHeight = line.Value;
                                break;
                            case "NINE_SLICE_LEFT":
                                nineSliceLeft = line.Value;
                                break;
                            case "NINE_SLICE_TOP":
                                nineSliceTop = line.Value;
                                break;
                            case "NINE_SLICE_RIGHT":
                                nineSliceRight = line.Value;
                                break;
                            case "NINE_SLICE_BOTTOM":
                                nineSliceBottom = line.Value;
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

            if (newBitmap == null)
               throw new Exception(); // Let missing pieces handle this

            Rectangle nineSliceRect = new Rectangle(nineSliceLeft, nineSliceTop, newBitmap.Width - nineSliceLeft - nineSliceRight, newBitmap.Height - nineSliceTop - nineSliceBottom);
            
            return new BaseImageInfo(newBitmap, IsSteel, Resize, isDeprecated, nineSliceRect, defaultWidth, defaultHeight);
        }
    }
}
