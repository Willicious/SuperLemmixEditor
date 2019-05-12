using System;
using System.Collections.Generic;
using System.Linq;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace NLEditor
{
  /// <summary>
  /// Contains static methods to load and save levels.
  /// </summary>
  static class LevelFile
  {
    /// <summary>
    /// Opens file browser and creates level from a .nxlv file.
    /// <para> Returns null if process is aborted or file is corrupt. </para>
    /// </summary>
    /// <param name="styleList"></param>
    /// <returns></returns>
    static public Level LoadLevel(List<Style> styleList, BackgroundList backgrounds, string levelDirectory)
    {
      var openFileDialog = new OpenFileDialog();

      if (!string.IsNullOrEmpty(levelDirectory) && Directory.Exists(levelDirectory))
      {
        openFileDialog.InitialDirectory = levelDirectory;
      }
      else
      {
        openFileDialog.InitialDirectory = Directory.Exists(C.AppPathLevels) ? C.AppPathLevels : C.AppPath;
      }
      openFileDialog.Multiselect = false;
      openFileDialog.Filter = "NeoLemmix level files (*.nxlv)|*.nxlv|Old level files (*.lvl, *.ini, *.lev)|*.lvl;*.ini;*.lev";
      openFileDialog.RestoreDirectory = true;
      openFileDialog.CheckFileExists = true;

      Level newLevel = null;

      try
      {
        if (openFileDialog.ShowDialog() == DialogResult.OK)
        {
          string filePath = openFileDialog.FileName;
          if (Path.GetExtension(filePath).Equals(".nxlv"))
          {
            newLevel = LoadLevelFromFile(filePath, styleList, backgrounds);
            newLevel.FilePathToSave = filePath;
          }
          else
          {
            bool IsConverted = ConvertOldLevelType(filePath);
            if (IsConverted)
            {
              newLevel = LoadLevelFromFile(C.AppPathTempLevel, styleList, backgrounds);
            }
          }

        }
      }
      catch (Exception Ex)
      {
        Utility.LogException(Ex);
        MessageBox.Show("Error while showing the file browser." + C.NewLine + Ex.Message, "File browser error");
        return newLevel;
      }
      finally
      {
        openFileDialog?.Dispose();
      }

      return newLevel;
    }

    /// <summary>
    /// Creates level from a .nxlv file.
    /// <para> Null if file cannot be opened. </para>
    /// </summary>
    /// <param name="filePath"></param>
    /// <param name="styleList"></param>
    /// <returns></returns>
    static private Level LoadLevelFromFile(string filePath, List<Style> styleList, BackgroundList backgrounds)
    {
      Level newLevel = new Level();

      FileParser parser = null;
      try
      {
        parser = new FileParser(filePath);
      }
      catch (Exception Ex)
      {
        Utility.LogException(Ex);
        MessageBox.Show(Ex.Message, "File corrupt");
        parser?.DisposeStreamReader();
        return newLevel;
      }

      List<int> hatchOrder = null;
      try
      {
        List<FileLine> fileLines;
        while ((fileLines = parser.GetNextLines()) != null)
        {
          System.Diagnostics.Debug.Assert(fileLines.Count > 0, "FileParser returned empty list.");

          FileLine line = fileLines[0];
          switch (line.Key)
          {
            case "TITLE": newLevel.Title = line.Text; break;
            case "AUTHOR": newLevel.Author = line.Text; break;
            case "ID":
              {
                string idString = (line.Text.StartsWith("x")) ? line.Text : 'x' + line.Text;
                // double ID for old lvl files
                if (idString.Length == 9) idString += idString.Substring(1);
                // Make sure this is a 64bit hex number
                if (idString.Length < 17) idString.PadRight(17, '0');
                else if (idString.Length > 17) idString.Substring(0, 17);
                newLevel.LevelID = idString;
                break;
              }
            case "MUSIC": newLevel.MusicFile = line.Text; break;
            case "WIDTH": newLevel.Width = line.Value; break;
            case "HEIGHT": newLevel.Height = line.Value; break;
            case "START_X": newLevel.StartPosX = line.Value; break;
            case "START_Y": newLevel.StartPosY = line.Value; break;
            case "THEME": newLevel.MainStyle = styleList.Find(sty => sty.NameInDirectory == line.Text); break;
            case "LEMMINGS": newLevel.NumLems = line.Value; break;
            case "REQUIREMENT": newLevel.SaveReq = line.Value; break;
            case "TIME_LIMIT":
              newLevel.TimeLimit = line.Value;
              newLevel.IsNoTimeLimit = false; break;
            case "MAX_SPAWN_INTERVAL": newLevel.SpawnRate = 103 - line.Value; break;
            case "SPAWN_INTERVAL_LOCKED": newLevel.IsSpawnRateFix = true; break;
            case "RELEASE_RATE": newLevel.SpawnRate = 50 + line.Value / 2; break;
            case "RELEASE_RATE_LOCKED": newLevel.IsSpawnRateFix = true; break;
            case "BACKGROUND": newLevel.Background = ReadBackgroundFromLines(line.Text, styleList, backgrounds); break;

            case "SKILLSET":
              ReadSkillSetFromLines(fileLines, newLevel);
              newLevel.SkillSet[C.Skill.Zombie] = 0; break;
            case "OBJECT":
            case "LEMMING": newLevel.GadgetList.Add(ReadGadgetFromLines(fileLines)); break;
            case "TERRAIN": newLevel.TerrainList.Add(ReadTerrainFromLines(fileLines)); break;
            case "SPAWN_ORDER": hatchOrder = fileLines.FindAll(lin => lin.Key == "object").ConvertAll(lin => lin.Value); break;

            case "PRETEXT":
              var pretexts = fileLines.ConvertAll(lin => lin.Text);
              pretexts.RemoveAt(0);
              newLevel.PreviewText = pretexts; break;
            case "POSTTEXT":
              var posttexts = fileLines.ConvertAll(lin => lin.Text);
              posttexts.RemoveAt(0);
              newLevel.PostviewText = posttexts; break;

            case "TALISMAN": newLevel.Talismans.Add(ReadTalismanFromLines(fileLines)); break;
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

      ApplyWindowOrder(hatchOrder, newLevel);
      SanitizeInput(newLevel);
      return newLevel;
    }

    /// <summary>
    /// Reads the skill set from the skill section.
    /// </summary>
    /// <param name="fileLines"></param>
    /// <param name="newLevel"></param>
    static private void ReadSkillSetFromLines(List<FileLine> fileLines, Level newLevel)
    {
      foreach (C.Skill skill in C.SkillArray)
      {
        FileLine line = fileLines.Find(lin => lin.Key == SkillString(skill));
        if (line != null)
        {
          newLevel.SkillSet[skill] = (line.Text == "INFINITE") ? 100 : line.Value;
        }
      }
    }


    /// <summary>
    /// Creates a gadget from a block of file lines.
    /// </summary>
    /// <param name="fileLineList"></param>
    /// <returns></returns>
    static private GadgetPiece ReadGadgetFromLines(List<FileLine> fileLineList)
    {
      // First read in all infos
      string styleName = "default"; // default value, because they are not set for preplaced lemmings 
      string gadgetName = "lemming"; // default value, because they are not set for preplaced lemmings 
      int posX = 0;
      int posY = 0;
      bool isNoOverwrite = false;
      bool isOnlyOnTerrain = false;
      int specWidth = -1;
      int specHeight = -1;

      bool doRotate = false;
      bool doInvert = false;
      bool doFlip = false;
      int val_L = 0;
      int bgSpeed = 0;
      int bgAngle = 0;
      HashSet<C.Skill> skillFlags = new HashSet<C.Skill>();

      foreach (FileLine line in fileLineList)
      {
        switch (line.Key)
        {
          case "COLLECTION": styleName = line.Text; break;
          case "PIECE": gadgetName = line.Text; break;
          case "X": posX = line.Value; break;
          case "Y": posY = line.Value; break;
          case "WIDTH": specWidth = line.Value; break;
          case "HEIGHT": specHeight = line.Value; break;
          case "NO_OVERWRITE": isNoOverwrite = true; break;
          case "ONLY_ON_TERRAIN": isOnlyOnTerrain = true; break;
          case "ROTATE": doRotate = true; break;
          case "FLIP_HORIZONTAL": doFlip = true; break;
          case "FLIP_VERTICAL": doInvert = true; break;
          case "DIRECTION": doFlip = line.Text.ToUpper().StartsWith("L"); break;
          case "FLIP_LEMMING": doFlip = true; break;
          case "PAIRING": val_L = line.Value; break;
          case "SKILLCOUNT": val_L = line.Value; break;
          case "SPEED": bgSpeed = line.Value; break;
          case "ANGLE": bgAngle = line.Value; break;
        }
      }

      if (doRotate)
      {
        // Swap width and height, to swap it again once the gadget is rotated
        Utility.Swap(ref specWidth, ref specHeight);
      }

      // ... then create the correct Gadget piece
      string key = ImageLibrary.CreatePieceKey(styleName, gadgetName, true);
      Point pos = new Point(posX, posY);
      GadgetPiece newGadget = new GadgetPiece(key, pos, 0, false, isNoOverwrite, isOnlyOnTerrain, 
        val_L, skillFlags, specWidth, specHeight, bgSpeed, bgAngle);

      // Read in skill information
      foreach (C.Skill skill in C.SkillArray)
      {
        if (fileLineList.Exists(line => line.Key == SkillString(skill)
                            || (line.Key == "SKILL" && line.Text == SkillString(skill))))
        {
          newGadget.SetSkillFlag(skill, true);
        }
      }

      // Ensure that pickup skills add at least one skill
      if (newGadget.ObjType == C.OBJ.PICKUP && newGadget.Val_L < 1)
      {
        newGadget.SetPickupSkillCount(1);
      }

      // For compatibility with player: NoOverwrite + OnlyOnTerrain gadgets work like OnlyOnTerrain 
      if (newGadget.IsNoOverwrite && newGadget.IsOnlyOnTerrain) newGadget.IsNoOverwrite = false;

      if (doRotate) newGadget.RotateInRect(newGadget.ImageRectangle);
      if (doFlip) newGadget.FlipInRect(newGadget.ImageRectangle);
      if (doInvert) newGadget.InvertInRect(newGadget.ImageRectangle);
      //Reposition gadget to be sure...
      newGadget.PosX = pos.X;
      newGadget.PosY = pos.Y;
      // and offset preplaced lemmings, because the level file saves the position of the trigger area
      if (newGadget.ObjType == C.OBJ.LEMMING)
      {
        newGadget.PosX -= C.LEM_OFFSET_X;
        newGadget.PosY -= C.LEM_OFFSET_Y;
      }

      newGadget.IsSelected = false;

      return newGadget;
    }

    /// <summary>
    /// Creates a terrain piece from a block of file lines.
    /// </summary>
    /// <param name="fileLineList"></param>
    /// <returns></returns>
    static private TerrainPiece ReadTerrainFromLines(List<FileLine> fileLineList)
    {
      // First read in all infos
      string styleName = "";
      string pieceName = "";
      int posX = 0;
      int posY = 0;

      bool isNoOverwrite = false;
      bool isErase = false;
      bool isOneWay = false;

      bool doRotate = false;
      bool doInvert = false;
      bool doFlip = false;

      foreach (FileLine line in fileLineList)
      {
        switch (line.Key)
        {
          case "COLLECTION": styleName = line.Text; break;
          case "PIECE": pieceName = line.Text; break;
          case "X": posX = line.Value; break;
          case "Y": posY = line.Value; break;
          case "NO_OVERWRITE": isNoOverwrite = true; break;
          case "ERASE": isErase = true; break;
          case "ONE_WAY": isOneWay = true; break;
          case "ROTATE": doRotate = true; break;
          case "FLIP_HORIZONTAL": doFlip = true; break;
          case "FLIP_VERTICAL": doInvert = true; break;
        }
      }

      // ... then create the correct Terrain piece
      string key = ImageLibrary.CreatePieceKey(styleName, pieceName, false);
      Point pos = new Point(posX, posY);
      TerrainPiece newTerrain = new TerrainPiece(key, pos, 0, false, isErase, isNoOverwrite, isOneWay);

      // For compatibility with player: NoOverwrite + Erase pieces work like NoOverWrite
      if (newTerrain.IsNoOverwrite && newTerrain.IsErase) newTerrain.IsErase = false;
      if (newTerrain.IsSteel) newTerrain.IsOneWay = false;

      if (doRotate) newTerrain.RotateInRect(newTerrain.ImageRectangle);
      if (doFlip) newTerrain.FlipInRect(newTerrain.ImageRectangle);
      if (doInvert) newTerrain.InvertInRect(newTerrain.ImageRectangle);
      //Reposition terrain piece to be sure...
      newTerrain.PosX = pos.X;
      newTerrain.PosY = pos.Y;

      newTerrain.IsSelected = false;

      return newTerrain;
    }

    /// <summary>
    /// Determines the background from the string in the level file.
    /// </summary>
    /// <param name="fileLineList"></param>
    /// <param name="styles"></param>
    /// <param name="backgrounds"></param>
    /// <returns></returns>
    static private Background ReadBackgroundFromLines(string text, List<Style> styles, BackgroundList backgrounds)
    {
      string[] bgInfo = text.Split(':');
      if (bgInfo.Length == 1) // only the background's name
      {
        return backgrounds.Find(bgInfo[0]);
      }
      else if (bgInfo.Length == 2) // background's style and name
      {
        Style bgStyle = styles.Find(sty => sty.NameInDirectory.Equals(bgInfo[0].Trim()));
        return backgrounds.Find(bgInfo[1], bgStyle);
      }
      else return null;
    }

    /// <summary>
    /// Reads the talisman info from a group of file lines.
    /// </summary>
    /// <param name="fileLineList"></param>
    /// <returns></returns>
    static private Talisman ReadTalismanFromLines(List<FileLine> fileLineList)
    {
      Talisman talisman = new Talisman();

      foreach (FileLine line in fileLineList)
      {
        switch (line.Key)
        {
          case "TITLE": talisman.Title = line.Text; break;
          case "COLOR": talisman.AwardType = Utility.ParseEnum<C.TalismanType>(line.Text); break;
          case "ID": talisman.ID = line.Value; break;
          default:
            {
              if (C.TalismanKeys.Values.Contains(line.Key))
              {
                C.TalismanReq requirement = C.TalismanKeys.First(pair => pair.Value.Equals(line.Key)).Key;
                if (requirement == C.TalismanReq.UseOnlySkill)
                {
                  talisman.Requirements[requirement] = (int)(Utility.ParseEnum<C.Skill>(line.Text));
                }
                else
                {
                  talisman.Requirements[requirement] = line.Value;
                }
              }
              break;
            }
        }
      }

      return talisman;
    }


    /// <summary>
    /// Applies a custom hatch order to the level. The correctly ordered hatches are appended at the beginning of the GadgetList.
    /// </summary>
    /// <param name="hatchOrderIndexes"></param>
    /// <param name="newLevel"></param>
    static private void ApplyWindowOrder(List<int> hatchOrderIndexes, Level newLevel)
    {
      if (hatchOrderIndexes == null) return;
      var hatchOrder = hatchOrderIndexes.FindAll(ind => ind >= 0 && ind < newLevel.GadgetList.Count)
                                        .ConvertAll(ind => (GadgetPiece)newLevel.GadgetList[ind].Clone());
      hatchOrder.RemoveAll(hat => hat.ObjType != C.OBJ.HATCH);
      newLevel.GadgetList.RemoveAll(obj => obj.ObjType == C.OBJ.HATCH);
      newLevel.GadgetList.InsertRange(0, hatchOrder);
    }

    /// <summary>
    /// Ensures that all level parameters are within sensible limits.
    /// </summary>
    /// <param name="newLevel"></param>
    static private void SanitizeInput(Level newLevel)
    {
      // Level size
      newLevel.Width = Math.Max(Math.Min(newLevel.Width, 2400), 1);
      newLevel.Height = Math.Max(Math.Min(newLevel.Height, 2400), 1);
      // Start position
      newLevel.StartPosX = Math.Max(Math.Min(newLevel.StartPosX, newLevel.Width - 1), 0);
      newLevel.StartPosY = Math.Max(Math.Min(newLevel.StartPosY, newLevel.Height - 1), 0);
      // Global level properties
      newLevel.NumLems = Math.Max(Math.Min(newLevel.NumLems, 500), 1);
      newLevel.SaveReq = Math.Max(Math.Min(newLevel.SaveReq, 500), 1);
      newLevel.SpawnRate = Math.Max(Math.Min(newLevel.SpawnRate, 99), 1);
      newLevel.TimeLimit = Math.Max(Math.Min(newLevel.TimeLimit, 5999), 0);
      // Skill numbers
      foreach (C.Skill skill in C.SkillArray)
      {
        newLevel.SkillSet[skill] = Math.Max(Math.Min(newLevel.SkillSet[skill], 100), 0);
      }
    }



    /// <summary>
    /// Opens file browser and saves the current level to a .nxlv file.
    /// </summary>
    /// <param name="curLevel"></param>
    static public void SaveLevel(Level curLevel, string levelDirectory)
    {
      var saveFileDialog = new SaveFileDialog();

      saveFileDialog.AddExtension = true;
      if (!string.IsNullOrEmpty(levelDirectory) && Directory.Exists(levelDirectory))
      {
        saveFileDialog.InitialDirectory = levelDirectory;
      }
      else
      {
        saveFileDialog.InitialDirectory = Directory.Exists(C.AppPathLevels) ? C.AppPathLevels : C.AppPath;
      }
      saveFileDialog.OverwritePrompt = true;
      saveFileDialog.Filter = "NeoLemmix level files (*.nxlv)|*.nxlv";
      saveFileDialog.RestoreDirectory = true;

      try
      {
        if (saveFileDialog.ShowDialog() == DialogResult.OK)
        {
          string filePath = saveFileDialog.FileName;
          try
          {
            SaveLevelToFile(filePath, curLevel);
            curLevel.FilePathToSave = filePath;
          }
          catch (Exception Ex)
          {
            Utility.LogException(Ex);
            MessageBox.Show("Could not save the level file!" + Environment.NewLine + Ex.Message, "Could not save");
          }
        }
      }
      catch (Exception Ex)
      {
        Utility.LogException(Ex);
        MessageBox.Show("Error while showing the file browser." + Environment.NewLine + Ex.Message, "File browser error");
      }
      finally
      {
        saveFileDialog.Dispose();
      }
    }

    /// <summary>
    /// Saves a level at the specified file path.
    /// </summary>
    /// <param name="filePath"></param>
    /// <param name="curLevel"></param>
    static public void SaveLevelToFile(string filePath, Level curLevel)
    {
      // Create new empty file
      try
      {
        File.Create(filePath).Close();
      }
      catch (Exception Ex)
      {
        Utility.LogException(Ex);
        MessageBox.Show("Error: Cannot create text file at " + filePath + "." + C.NewLine + Ex.Message, "Could not save");
        return;
      }

      TextWriter textFile = new StreamWriter(filePath, true);

      textFile.WriteLine("# ----------------------------- ");
      textFile.WriteLine("#        NeoLemmix Level        ");
      textFile.WriteLine("#   Created with NLEditor " + C.Version);
      textFile.WriteLine("# ----------------------------- ");
      textFile.WriteLine(" ");
      textFile.WriteLine("#        Level info             ");
      textFile.WriteLine("# ----------------------------- ");
      textFile.WriteLine(" TITLE " + curLevel.Title);
      textFile.WriteLine(" AUTHOR " + curLevel.Author);
      if (!string.IsNullOrEmpty(curLevel.MusicFile))
      {
        textFile.WriteLine(" MUSIC " + Path.ChangeExtension(curLevel.MusicFile, null));
      }
      textFile.WriteLine(" ID " + curLevel.LevelID);
      textFile.WriteLine(" AUTOSTEEL on ");
      textFile.WriteLine(" ");

      textFile.WriteLine("#       Level dimensions        ");
      textFile.WriteLine("# ----------------------------- ");
      textFile.WriteLine(" WIDTH   " + curLevel.Width.ToString().PadLeft(4));
      textFile.WriteLine(" HEIGHT  " + curLevel.Height.ToString().PadLeft(4));
      textFile.WriteLine(" START_X " + curLevel.StartPosX.ToString().PadLeft(4));
      textFile.WriteLine(" START_Y " + curLevel.StartPosY.ToString().PadLeft(4));
      textFile.WriteLine(" THEME " + curLevel.MainStyle?.NameInDirectory);
      if (curLevel.Background != null)
      {
        textFile.WriteLine(" BACKGROUND " + curLevel.Background.Style.NameInDirectory + ":"
                                          + curLevel.Background.Name);
      }
      textFile.WriteLine(" ");

      textFile.WriteLine("#         Level stats           ");
      textFile.WriteLine("# ----------------------------- ");
      textFile.WriteLine(" LEMMINGS     " + curLevel.NumLems.ToString().PadLeft(4));
      textFile.WriteLine(" REQUIREMENT  " + curLevel.SaveReq.ToString().PadLeft(4));
      if (!curLevel.IsNoTimeLimit)
      {
        textFile.WriteLine(" TIME_LIMIT   " + curLevel.TimeLimit.ToString().PadLeft(4));
      }
      textFile.WriteLine(" MAX_SPAWN_INTERVAL " + (103 - curLevel.SpawnRate).ToString().PadLeft(4));
      if (curLevel.IsSpawnRateFix)
      {
        textFile.WriteLine(" SPAWN_INTERVAL_LOCKED ");
      }
      textFile.WriteLine(" ");

      textFile.WriteLine(" $SKILLSET ");
      foreach (C.Skill skill in C.SkillArray)
      {
        if (IsSkillRequired(curLevel, skill))
        {
          var count = curLevel.SkillSet[skill] > 99 ? "INFINITE" : curLevel.SkillSet[skill].ToString().PadLeft(4);
          textFile.WriteLine(PaddedSkillString(skill) + count);
        }
      }
      textFile.WriteLine(" $END ");
      textFile.WriteLine(" ");

      if (curLevel.PreviewText?.Count > 0)
      {
        textFile.WriteLine(" $PRETEXT ");
        curLevel.PreviewText.ForEach(lin => textFile.WriteLine("  LINE " + lin));
        textFile.WriteLine(" $END ");
        textFile.WriteLine(" ");
      }

      if (curLevel.PostviewText?.Count > 0)
      {
        textFile.WriteLine(" $POSTTEXT ");
        curLevel.PostviewText.ForEach(lin => textFile.WriteLine("  LINE " + lin));
        textFile.WriteLine(" $END ");
        textFile.WriteLine(" ");
      }

      curLevel.Talismans.ForEach(tal => WriteTalisman(textFile, tal));

      textFile.WriteLine("#     Interactive objects       ");
      textFile.WriteLine("# ----------------------------- ");
      curLevel.GadgetList.FindAll(gad => gad.ObjType != C.OBJ.LEMMING)
                         .ForEach(gad => WriteObject(textFile, gad));
      textFile.WriteLine(" ");

      textFile.WriteLine("#        Terrain pieces         ");
      textFile.WriteLine("# ----------------------------- ");
      curLevel.TerrainList.ForEach(ter => WriteTerrain(textFile, ter));
      textFile.WriteLine(" ");

      if (curLevel.GadgetList.Exists(gad => gad.ObjType == C.OBJ.LEMMING))
      {
        textFile.WriteLine("#      Preplaced lemmings       ");
        textFile.WriteLine("# ----------------------------- ");
        curLevel.GadgetList.FindAll(gad => gad.ObjType == C.OBJ.LEMMING)
                           .ForEach(lem => WriteObject(textFile, lem));

        textFile.WriteLine(" ");
      }

      textFile.Close();
    }

    /// <summary>
    /// Returns whether the skill is in the skill set or available as a pickup skill. 
    /// </summary>
    /// <param name="curLevel"></param>
    /// <param name="skillNum"></param>
    /// <returns></returns>
    static private bool IsSkillRequired(Level curLevel, C.Skill skill)
    {
      return (curLevel.SkillSet[skill] > 0)
          || (curLevel.GadgetList.Exists(gad => gad.ObjType == C.OBJ.PICKUP && gad.SkillFlags.Contains(skill)));
    }

    /// <summary>
    /// Writes all object infos in a text file.
    /// </summary>
    /// <param name="textFile"></param>
    /// <param name="gadget"></param>
    static private void WriteObject(TextWriter textFile, GadgetPiece gadget)
    {
      if (gadget == null) return;
      if (gadget.ObjType == C.OBJ.PICKUP && gadget.SkillFlags.Count == 0) return;

      if (gadget.ObjType == C.OBJ.LEMMING)
      {
        textFile.WriteLine(" $LEMMING");
      }
      else
      {
        textFile.WriteLine(" $OBJECT");
        textFile.WriteLine("   COLLECTION " + gadget.Style);
        textFile.WriteLine("   PIECE      " + gadget.Name);
      }

      int posX = gadget.PosX + (gadget.ObjType == C.OBJ.LEMMING ? C.LEM_OFFSET_X : 0);
      int posY = gadget.PosY + (gadget.ObjType == C.OBJ.LEMMING ? C.LEM_OFFSET_Y : 0);
      textFile.WriteLine("   X      " + posX.ToString().PadLeft(5));
      textFile.WriteLine("   Y      " + posY.ToString().PadLeft(5));

      if (gadget.MayResizeHoriz())
      {
        textFile.WriteLine("   WIDTH  " + gadget.SpecWidth.ToString().PadLeft(5));
      }
      if (gadget.MayResizeVert())
      {
        textFile.WriteLine("   HEIGHT " + gadget.SpecHeight.ToString().PadLeft(5));
      }
      if (gadget.IsNoOverwrite)
      {
        textFile.WriteLine("   NO_OVERWRITE");
      }
      if (gadget.IsOnlyOnTerrain)
      {
        textFile.WriteLine("   ONLY_ON_TERRAIN");
      }
      if (gadget.IsRotatedInPlayer)
      {
        textFile.WriteLine("   ROTATE");
      }
      if (gadget.IsInvertedInPlayer)
      {
        textFile.WriteLine("   FLIP_VERTICAL");
      }
      if (gadget.IsFlippedInPlayer && gadget.ObjType != C.OBJ.HATCH)
      {
        textFile.WriteLine("   FLIP_HORIZONTAL");
      }

      if (gadget.ObjType.In(C.OBJ.HATCH, C.OBJ.SPLITTER, C.OBJ.LEMMING))
      {
        textFile.WriteLine("   DIRECTION " + ((gadget.IsFlippedInPlayer) ? "left" : "right"));
      }
      else if (gadget.ObjType.In(C.OBJ.TELEPORTER))
      {
        if (gadget.IsFlippedInPlayer) textFile.WriteLine("   FLIP_LEMMING ");
      }

      if (gadget.ObjType.In(C.OBJ.HATCH, C.OBJ.LEMMING))
      {
        foreach (C.Skill skill in gadget.SkillFlags)
        {
          textFile.WriteLine("   " + SkillString(skill) + " ");
        }
      }
      else if (gadget.ObjType.In(C.OBJ.PICKUP))
      {
        foreach (C.Skill skill in gadget.SkillFlags)
        {
          textFile.WriteLine("   SKILL " + SkillString(skill));
        }

        if (gadget.Val_L > 1)
        {
          textFile.WriteLine("   SKILLCOUNT " + gadget.Val_L.ToString());
        }
      }

      if (gadget.ObjType.In(C.OBJ.TELEPORTER, C.OBJ.RECEIVER))
      {
        textFile.WriteLine("   PAIRING " + gadget.Val_L.ToString().PadLeft(4));
      }

      if (gadget.ObjType.In(C.OBJ.BACKGROUND))
      {
        textFile.WriteLine("   SPEED   " + gadget.BackgroundSpeed.ToString().PadLeft(4));
        textFile.WriteLine("   ANGLE   " + gadget.BackgroundAngle.ToString().PadLeft(4));
      }

      textFile.WriteLine(" $END");
      textFile.WriteLine(" ");
    }

    /// <summary>
    /// Writes all terrain piece infos in a text file.
    /// </summary>
    /// <param name="textFile"></param>
    /// <param name="terrain"></param>
    static private void WriteTerrain(TextWriter textFile, TerrainPiece terrain)
    {
      textFile.WriteLine(" $TERRAIN");
      textFile.WriteLine("   COLLECTION " + terrain.Style);
      textFile.WriteLine("   PIECE      " + terrain.Name);
      textFile.WriteLine("   X " + terrain.PosX.ToString().PadLeft(5));
      textFile.WriteLine("   Y " + terrain.PosY.ToString().PadLeft(5));
      if (terrain.IsNoOverwrite)
      {
        textFile.WriteLine("   NO_OVERWRITE");
      }
      if (terrain.IsErase)
      {
        textFile.WriteLine("   ERASE");
      }
      if (terrain.IsRotatedInPlayer)
      {
        textFile.WriteLine("   ROTATE");
      }
      if (terrain.IsInvertedInPlayer)
      {
        textFile.WriteLine("   FLIP_VERTICAL");
      }
      if (terrain.IsFlippedInPlayer)
      {
        textFile.WriteLine("   FLIP_HORIZONTAL");
      }
      if (terrain.IsOneWay)
      {
        textFile.WriteLine("   ONE_WAY");
      }
      textFile.WriteLine(" $END");
      textFile.WriteLine(" ");
    }

    /// <summary>
    /// Writes aa talisman in a text file.
    /// </summary>
    /// <param name="textFile"></param>
    /// <param name="talisman"></param>
    static private void WriteTalisman(TextWriter textFile, Talisman talisman)
    {
      textFile.WriteLine(" $TALISMAN ");
      textFile.WriteLine("   TITLE " + talisman.Title);
      textFile.WriteLine("   ID " + talisman.ID);
      textFile.WriteLine("   COLOR " + talisman.AwardType.ToString());
      foreach (C.TalismanReq requirement in talisman.Requirements.Keys)
      {
        if (requirement == C.TalismanReq.UseOnlySkill)
        {
          textFile.WriteLine("   " + C.TalismanKeys[requirement] + " " + ((C.Skill)talisman.Requirements[requirement]).ToString());
        }
        else
        {
          textFile.WriteLine("   " + C.TalismanKeys[requirement] + " " + talisman.Requirements[requirement].ToString());
        }
      }
      textFile.WriteLine(" $END ");
      textFile.WriteLine(" ");
    }

    /// <summary>
    /// Returns the name of the skill as a string.
    /// </summary>
    /// <param name="skill"></param>
    /// <returns></returns>
    static string SkillString(C.Skill skill)
    {
      return Enum.GetName(typeof(C.Skill), skill).ToUpper();
    }


    /// <summary>
    /// Returns the name of the skill as a string, padded to length 12.
    /// </summary>
    /// <param name="skill"></param>
    /// <returns></returns>
    static string PaddedSkillString(C.Skill skill)
    {
      return SkillString(skill).PadLeft(11).PadRight(12);
    }

    /// <summary>
    /// Converts an old .lvl level file to the current .nxlv type.
    /// <para> This calls either NeoLemmix.exe or the NLConverter.exe written in Delphi. </para>
    /// </summary>
    /// <param name="filePath"></param>
    /// <returns></returns>
    static bool ConvertOldLevelType(string filePath)
    {
      if (Path.GetExtension(filePath).Equals(".lvl"))
      {
        return ContertWithNeoLemmix(filePath) || ConvertWithConverter(filePath);
      }
      else
      {
        return ContertWithNeoLemmix(filePath);
      }
    }

    /// <summary>
    /// Converts an old .lvl level file to the current .nxlv type.
    /// <para> This calls NLConverter.exe written in Delphi. </para>
    /// </summary>
    /// <param name="filepath"></param>
    /// <returns></returns>
    static bool ConvertWithConverter(string filePath)
    {
      try
      {
        // Before we are able to execute the NLConverter, we have to write it as a file to the disc!
        using (var converterStream = new FileStream(C.AppPath + "NLConverter.exe", FileMode.CreateNew, FileAccess.Write))
        {
          byte[] converterBytes = Properties.Resources.NLLevelConverter;
          converterStream.Write(converterBytes, 0, converterBytes.Length);
        }

        var converterStartInfo = new System.Diagnostics.ProcessStartInfo();
        converterStartInfo.FileName = C.AppPath + "NLConverter.exe";
        converterStartInfo.Arguments = filePath + " " + C.AppPathTempLevel;

        var converterProcess = System.Diagnostics.Process.Start(converterStartInfo);
        converterProcess.WaitForExit();
        int exitCode = converterProcess.ExitCode;

        Utility.DeleteFile(C.AppPath + "NLConverter.exe");

        if (C.FileConverterErrorMsg.ContainsKey(exitCode))
        {
          MessageBox.Show(C.FileConverterErrorMsg[exitCode], "File converter problem");
        }
        else if (exitCode >= 10)
        {
          MessageBox.Show("Error: Level converter crashed due to unhandles exception.", "File converter problem");
        }

        return (exitCode < 10);
      }
      catch
      {
        return false;
      }
    }

    /// <summary>
    /// Converts an old .lvl level file to the current .nxlv type.
    /// <para> This calls NeoLemmix.exe written in Delphi. </para>
    /// </summary>
    /// <param name="filePath"></param>
    /// <returns></returns>
    static bool ContertWithNeoLemmix(string filePath)
    {
      if (!File.Exists(C.AppPathNeoLemmix)) return false;

      // Compare version number of the NeoLemmix.exe file
      var versionInfo = System.Diagnostics.FileVersionInfo.GetVersionInfo(C.AppPathNeoLemmix);
      string[] fileVersion = versionInfo.FileVersion.Split('.');
      try
      {
        if (int.Parse(fileVersion[0]) < 11 || int.Parse(fileVersion[1]) < 14) return false;
      }
      // If that fails, the version is always wrong!
      catch (FormatException) { return false; }
      catch (ArgumentNullException) { return false; }

      try
      {
        Utility.DeleteFile(C.AppPathTempLevel);

        var converterStartInfo = new System.Diagnostics.ProcessStartInfo();
        converterStartInfo.FileName = C.AppPathNeoLemmix;
        converterStartInfo.Arguments = "convert \"" + filePath + "\" \"" + C.AppPathTempLevel + "\"";

        var converterProcess = System.Diagnostics.Process.Start(converterStartInfo);
        converterProcess.WaitForExit();

        return File.Exists(C.AppPathTempLevel);
      }
      catch
      {
        return false;
      }
    }

  }
}
