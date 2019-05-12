﻿using System;
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
      if (!File.Exists(filePath)) return colorDict;

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
              if (colorString.StartsWith("x")) colorString = colorString.Substring(1);
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

      if (!File.Exists(filePath)) return styleList;

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

          if (line == null) break;

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
      string imagePath = C.AppPathPieces + imageKey;

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
    public static BaseImageInfo ImageInfo(Bitmap image, string imageName)
    {
      string imagePath = C.AppPathPieces + imageName;

      if (File.Exists(imagePath + ".nxmo"))
      {
        // create a new object piece
        return CreateNewObjectInfo(image, imagePath + ".nxmo");
      }
      else if (File.Exists(imagePath + ".nxmt"))
      {
        // create a new object piece
        return CreateNewTerrainInfo(image, imagePath + ".nxmt");
      }
      else
      {
        // create a new terrain piece
        return new BaseImageInfo(image);
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
    private static BaseImageInfo CreateNewObjectInfo(Bitmap newBitmap, string filePath)
    {
      int numFrames = 1;
      bool isVert = true;
      C.OBJ objType = C.OBJ.NONE;
      Rectangle triggerRect = new Rectangle(0, 0, 1, 1);
      C.Resize resizeMode = C.Resize.None;

      Bitmap secImage = null;
      int secFrames = 0;
      bool secIsVert = true;
      int secOffsetX = 0;
      int secOffsetY = 0;


      FileParser parser;
      try
      {
        parser = new FileParser(filePath);
      }
      catch (Exception Ex)
      {
        Utility.LogException(Ex);
        MessageBox.Show(Ex.Message, "File corrupt");
        return new BaseImageInfo(newBitmap, objType, numFrames, isVert, triggerRect, resizeMode);
      }

      try
      {
        List<FileLine> fileLineList;
        while ((fileLineList = parser.GetNextLines()) != null)
        {
          System.Diagnostics.Debug.Assert(fileLineList.Count > 0, "FileParser returned empty list.");

          FileLine line = fileLineList[0];
          switch (line.Key)
          {
            case "FRAMES": numFrames = line.Value; break;
            case "TRIGGER_X": triggerRect.X = line.Value; break;
            case "TRIGGER_Y": triggerRect.Y = line.Value; break;
            case "TRIGGER_WIDTH": triggerRect.Width = line.Value; break;
            case "TRIGGER_HEIGHT": triggerRect.Height = line.Value; break;
            case "VERTICAL": isVert = true; break;
            case "HORIZONTAL": isVert = false; break;
            case "RESIZE_VERTICAL": resizeMode = resizeMode.In(C.Resize.Horiz, C.Resize.Both) ? C.Resize.Both : C.Resize.Vert; break;
            case "RESIZE_HORIZONTAL": resizeMode = resizeMode.In(C.Resize.Vert, C.Resize.Both) ? C.Resize.Both : C.Resize.Horiz; break;
            case "RESIZE_BOTH": resizeMode = C.Resize.Both; break;
            case "WINDOW": objType = C.OBJ.HATCH; break;
            case "EXIT": objType = C.OBJ.EXIT; break;
            case "TRAP": objType = C.OBJ.TRAP; break;
            case "SINGLE_USE_TRAP": objType = C.OBJ.TRAPONCE; break;
            case "WATER": objType = C.OBJ.WATER; break;
            case "FIRE": objType = C.OBJ.FIRE; break;
            case "ONE_WAY_RIGHT":
            case "ONE_WAY_LEFT":
            case "ONE_WAY_DOWN": objType = C.OBJ.ONE_WAY_WALL; break;
            case "BUTTON": objType = C.OBJ.BUTTON; break;
            case "LOCKED_EXIT": objType = C.OBJ.EXIT_LOCKED; break;
            case "PICKUP_SKILL": objType = C.OBJ.PICKUP; break;
            case "TELEPORTER": objType = C.OBJ.TELEPORTER; break;
            case "RECEIVER": objType = C.OBJ.RECEIVER; break;
            case "SPLITTER": objType = C.OBJ.SPLITTER; break;
            case "UPDRAFT": objType = C.OBJ.UPDRAFT; break;
            case "SPLATPAD": objType = C.OBJ.SPLAT; break;
            case "FORCE_RIGHT":
            case "FORCE_LEFT": objType = C.OBJ.FORCE_FIELD; break;
            case "BACKGROUND":
            case "MOVING_BACKGROUND": objType = C.OBJ.BACKGROUND; break;
            case "ANIMATION":
              {
                foreach (var fileLine in fileLineList)
                {
                  switch (fileLine.Key)
                  {
                    case "NAME":
                      {
                        var secondaryPath = filePath.Substring(0, filePath.Length - 5) + "_" + fileLine.Text + ".png";
                        secImage = Utility.CreateBitmapFromFile(secondaryPath);
                        break;
                      }
                    case "FRAMES": secFrames = fileLine.Value; break;
                    case "OFFSET_X": secOffsetX = fileLine.Value; break;
                    case "OFFSET_Y": secOffsetY = fileLine.Value; break;
                  }
                }
                break;
              }
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

      if (secImage == null)
      {
        return new BaseImageInfo(newBitmap, objType, numFrames, isVert, triggerRect, resizeMode);
      }
      else
      {
        return new BaseImageInfo(newBitmap, objType, numFrames, isVert, triggerRect, resizeMode,
          secImage, secFrames, secIsVert, secOffsetX, secOffsetY);
      }



      
    }

    /// <summary>
    /// Reads further terrain infos from a .nxtp file. Default values:
    /// <para> IsSteel = false </para>
    /// </summary>
    /// <param name="newBitmap"></param>
    /// <param name="FilePathInfo"></param>
    /// <returns></returns>
    private static BaseImageInfo CreateNewTerrainInfo(Bitmap newBitmap, string filePath)
    {
      bool IsSteel = false;

      FileParser parser;
      try
      {
        parser = new FileParser(filePath);
      }
      catch (Exception Ex)
      {
        Utility.LogException(Ex);
        MessageBox.Show(Ex.Message, "File corrupt");
        return new BaseImageInfo(newBitmap, IsSteel);
      }

      try
      {
        List<FileLine> fileLineList;
        while ((fileLineList = parser.GetNextLines()) != null)
        {
          System.Diagnostics.Debug.Assert(fileLineList.Count > 0, "FileParser returned empty list.");

          FileLine line = fileLineList[0];
          switch (line.Key)
          {
            case "STEEL": IsSteel = true; break;
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

      return new BaseImageInfo(newBitmap, IsSteel);
    }
  }
}
