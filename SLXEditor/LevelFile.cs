using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection.Emit;
using System.Windows.Forms;

namespace SLXEditor
{
    /// <summary>
    /// Contains static methods to load and save levels.
    /// </summary>
    static class LevelFile
    {
        /// <summary>
        /// Opens file browser and creates level from file.
        /// <para> Returns null if process is aborted or file is corrupt. </para>
        /// </summary>
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
            openFileDialog.Filter = "SuperLemmix (.sxlv)|*.sxlv|NeoLemmix (.nxlv)|*.nxlv|All Levels|*.sxlv;*.nxlv";
            openFileDialog.RestoreDirectory = true;
            openFileDialog.CheckFileExists = true;

            Level newLevel = null;

            try
            {
                if (openFileDialog.ShowDialog() == DialogResult.OK)
                    newLevel = LoadLevelFromFile(openFileDialog.FileName, styleList, backgrounds);
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

        static public Level LoadLevelFromFile(string filePath, List<Style> styleList, BackgroundList backgrounds)
        {
            Level newLevel = null;

            try
            {
                newLevel = DoLoadLevelFromFile(filePath, styleList, backgrounds);
                newLevel.FilePathToSave = filePath;
            }
            catch (Exception Ex)
            {
                Utility.LogException(Ex);
                MessageBox.Show("Error while loading the level." + C.NewLine + Ex.Message, "Level load error");
                return newLevel;
            }

            return newLevel;
        }

        static private Level DoLoadLevelFromFile(string filePath, List<Style> styleList, BackgroundList backgrounds)
        {
            Level newLevel = new Level();
            SLXTextDataNode file = NLTextParser.LoadFile(filePath);

            newLevel.Title = file["TITLE"].Value;
            newLevel.Author = file["AUTHOR"].Value;
            newLevel.LevelID = file["ID"].ValueUInt64;
            newLevel.LevelVersion = file["VERSION"].ValueUInt64;

            newLevel.MainStyle = styleList.Find(sty => sty.NameInDirectory == Aliases.Dealias(file["THEME"].Value, AliasKind.Style).To);
            newLevel.Background = ParseBackground(Aliases.Dealias(file["BACKGROUND"].Value, AliasKind.Background).To, styleList, backgrounds);

            newLevel.MusicFile = file["MUSIC"].Value;

            newLevel.Width = file["WIDTH"].ValueInt;
            newLevel.Height = file["HEIGHT"].ValueInt;

            if (file.HasChildWithKey("START_X") && file.HasChildWithKey("START_Y"))
            {
                newLevel.StartPos = new Point(file["START_X"].ValueInt, file["START_Y"].ValueInt);
                newLevel.AutoStartPos = false;
            }
            else
                newLevel.AutoStartPos = true;

            newLevel.NumLems = file["LEMMINGS"].ValueInt;
            newLevel.SaveReq = file["SAVE_REQUIREMENT"].ValueInt;

            if (file.HasChildWithKey("TIME_LIMIT"))
            {
                newLevel.TimeLimit = file["TIME_LIMIT"].ValueInt;
                newLevel.HasTimeLimit = true;
            }

            newLevel.SpawnInterval = file["MAX_SPAWN_INTERVAL"].ValueInt;
            newLevel.ReleaseRate = 103 - file["MAX_SPAWN_INTERVAL"].ValueInt;
            newLevel.IsSpawnRateFix = file.HasChildWithKey("SPAWN_INTERVAL_LOCKED");
            newLevel.IsSuperlemming = file.HasChildWithKey("SUPERLEMMING");
            newLevel.IsInvincibility = file.HasChildWithKey("INVINCIBILITY");
            newLevel.SteelType = file["STEEL_TYPE"].ValueInt;

            LoadSkillset(newLevel, file["SKILLSET"]);

            foreach (var node in file.Children.FindAll(child => child.Key == "GADGET"))
                LoadGadget(newLevel, node);

            List<GroupPiece> groupPieceSamples = new List<GroupPiece>();

            foreach (var node in file.Children.FindAll(child => child.Key == "TERRAINGROUP"))
                LoadTerrainGroup(groupPieceSamples, node);

            foreach (var node in file.Children.FindAll(child => child.Key == "TERRAIN"))
                LoadTerrain(newLevel, groupPieceSamples, node);

            foreach (var node in file.Children.FindAll(child => child.Key == "LEMMING"))
                LoadLemming(newLevel, node);

            foreach (var node in file.Children.FindAll(child => child.Key == "SKETCH")) // Backwards compatibility
                LoadRulers(newLevel, node);
            foreach (var node in file.Children.FindAll(child => child.Key == "RULER"))
                LoadRulers(newLevel, node);

            foreach (var node in file.Children.FindAll(child => child.Key == "TALISMAN"))
                LoadTalisman(newLevel, node);

            foreach (var line in file["PRETEXT"].Children.FindAll(child => child.Key == "LINE"))
                newLevel.PreviewText.Add(line.Value);

            foreach (var line in file["POSTTEXT"].Children.FindAll(child => child.Key == "LINE"))
                newLevel.PostviewText.Add(line.Value);

            SanitizeInput(newLevel);
            return newLevel;
        }

        private static Background ParseBackground(string identifier, List<Style> styleList, BackgroundList backgrounds)
        {
            if (string.IsNullOrEmpty(identifier) || identifier.Trim() == ":")
                return null;

            string[] bgInfo = identifier.Split(':');
            if (bgInfo.Length == 2) // background's style and name
            {
                Style bgStyle = styleList.Find(sty => sty.NameInDirectory.Equals(bgInfo[0]));

                return new Background(bgStyle, bgInfo[1]);
            }
            else
                return null;
        }

        private static void LoadSkillset(Level level, SLXTextDataNode node)
        {
            foreach (C.Skill skill in C.SkillArray)
            {
                SLXTextDataNode subnode = node[SkillString(skill)];
                level.SkillSet[skill] = subnode.ValueTrimUpper == "INFINITE" ? 100 : subnode.ValueInt;
            }
        }

        private static void LoadGadget(Level level, SLXTextDataNode node)
        {
            // First read in all infos
            string styleName = node["STYLE"].Value;
            string gadgetName = node["PIECE"].Value;

            Alias gadgetAlias = Aliases.Dealias(styleName + ":" + gadgetName, AliasKind.Gadget);
            string[] dealiasName = gadgetAlias.To.Split(':');
            styleName = dealiasName[0];
            gadgetName = dealiasName[1];

            int posX = node["X"].ValueInt;
            int posY = node["Y"].ValueInt;
            bool isNoOverwrite = node.HasChildWithKey("NO_OVERWRITE");
            bool isOnlyOnTerrain = node.HasChildWithKey("ONLY_ON_TERRAIN");
            int specWidth = node.HasChildWithKey("WIDTH") ? node["WIDTH"].ValueInt : -1;
            int specHeight = node.HasChildWithKey("HEIGHT") ? node["HEIGHT"].ValueInt : -1;

            if (specWidth <= 0 && gadgetAlias.Width > 0) specWidth = gadgetAlias.Width;
            if (specHeight <= 0 && gadgetAlias.Height > 0) specHeight = gadgetAlias.Height;

            bool doRotate = node.HasChildWithKey("ROTATE");
            bool doInvert = node.HasChildWithKey("FLIP_VERTICAL");
            bool doFlip = node.HasChildWithKey("FLIP_HORIZONTAL");
            int val_L = node.HasChildWithKey("PAIRING") ? node["PAIRING"].ValueInt : node["SKILL_COUNT"].ValueInt;
            int dnSpeed = node["SPEED"].ValueInt;
            int dnAngle = node["ANGLE"].ValueInt;
            int lemmingCap = node["LEMMINGS"].ValueInt;
            int countdownLength = node["COUNTDOWN"].ValueInt;
            HashSet<C.Skill> skillFlags = new HashSet<C.Skill>();

            if (doRotate)
            {
                // Swap width and height, to swap it again once the gadget is rotated
                Utility.Swap(ref specWidth, ref specHeight);
            }

            // ... then create the correct Gadget piece
            string key = ImageLibrary.CreatePieceKey(styleName, gadgetName, true);
            Point levelFilePos = new Point(posX, posY);
            Point editorPos = ImageLibrary.LevelFileToEditorCoordinates(key, levelFilePos, doRotate, doFlip, doInvert);
            GadgetPiece newGadget = new GadgetPiece(key, editorPos, 0, false, isNoOverwrite, isOnlyOnTerrain,
              val_L, skillFlags, specWidth, specHeight, dnSpeed, dnAngle, lemmingCap, countdownLength);

            // Read in skill information
            foreach (C.Skill skill in C.SkillArray)
            {
                if (newGadget.ObjType.In(C.OBJ.PICKUP, C.OBJ.PERMASKILL_ADD, C.OBJ.SKILL_ASSIGNER))
                {
                    if (node["SKILL"].ValueTrimUpper == SkillString(skill))
                        newGadget.SetSkillFlag(skill, true);
                }
                else if (node.HasChildWithKey(SkillString(skill)))
                    newGadget.SetSkillFlag(skill, true);
            }

            // Ensure that pickup skills add at least one skill
            if (newGadget.ObjType == C.OBJ.PICKUP && newGadget.Val_L < 1)
            {
                newGadget.SetPickupSkillCount(1);
            }

            // Ensure radiation and slowfreeze countdown is set to 10 if no value is available
            if (newGadget.ObjType.In(C.OBJ.RADIATION, C.OBJ.SLOWFREEZE) && newGadget.CountdownLength < 1)
            {
                newGadget.SetCountdownLength(10);
            }

            // For compatibility with player: NoOverwrite + OnlyOnTerrain gadgets work like OnlyOnTerrain 
            if (newGadget.IsNoOverwrite && newGadget.IsOnlyOnTerrain)
                newGadget.IsNoOverwrite = false;

            if (doRotate)
                newGadget.RotateInRect(newGadget.ImageRectangle);
            if (doFlip)
                newGadget.FlipInRect(newGadget.ImageRectangle, newGadget.ObjType == C.OBJ.HATCH);
            if (doInvert)
                newGadget.InvertInRect(newGadget.ImageRectangle);

            //Reposition gadget to be sure...
            newGadget.PosX = editorPos.X;
            newGadget.PosY = editorPos.Y;

            newGadget.IsSelected = false;

            level.GadgetList.Add(newGadget);
        }

        private static void LoadLemming(Level level, SLXTextDataNode node)
        {
            // First read in all infos 
            int posX = node["X"].ValueInt;
            int posY = node["Y"].ValueInt;
            bool doFlip = node.HasChildWithKey("FLIP_HORIZONTAL");
            HashSet<C.Skill> skillFlags = new HashSet<C.Skill>();

            // ... then create the correct Gadget piece
            string key = ImageLibrary.CreatePieceKey("default", "lemming", true);
            Point levelFilePos = new Point(posX, posY);
            Point editorPos = ImageLibrary.LevelFileToEditorCoordinates(key, levelFilePos, false, doFlip, false);
            GadgetPiece newLemming = new GadgetPiece(key, editorPos, 0, false, false, false, 0, skillFlags, -1, -1, 0, 0, 0);

            // Read in skill information
            foreach (C.Skill skill in C.SkillArray)
            {
                if (node.HasChildWithKey(SkillString(skill)))
                    newLemming.SetSkillFlag(skill, true);
            }

            if (doFlip)
                newLemming.FlipInRect(newLemming.ImageRectangle);

            //Reposition gadget to be sure...
            newLemming.PosX = editorPos.X;
            newLemming.PosY = editorPos.Y;

            // and offset preplaced lemmings, because the level file saves the position of the trigger area
            newLemming.PosX -= C.LEM_OFFSET_X;
            newLemming.PosY -= C.LEM_OFFSET_Y;

            newLemming.IsSelected = false;

            level.GadgetList.Add(newLemming);
        }

        private static void LoadTerrainGroup(List<GroupPiece> samples, SLXTextDataNode node)
        {
            List<TerrainPiece> pieceList = new List<TerrainPiece>();

            foreach (var subnode in node.Children.FindAll(sn => sn.Key == "TERRAIN"))
                pieceList.Add(LoadTerrainData(samples, subnode));

            samples.Add(new GroupPiece(pieceList, node["NAME"].Value));
        }

        private static void LoadTerrain(Level level, List<GroupPiece> groupPieceSamples, SLXTextDataNode node)
        {
            level.TerrainList.Add(LoadTerrainData(groupPieceSamples, node));
        }

        private static TerrainPiece LoadTerrainData(List<GroupPiece> groupPieceSamples, SLXTextDataNode node)
        {
            // First read in all infos
            string styleName = node["STYLE"].Value;
            string pieceName = node["PIECE"].Value;

            Alias? pieceAlias = null;

            if (styleName.ToUpperInvariant() != "*GROUP")
            {
                pieceAlias = Aliases.Dealias(styleName + ":" + pieceName, AliasKind.Terrain);
                string[] dealiasName = pieceAlias.Value.To.Split(':');
                styleName = dealiasName[0];
                pieceName = dealiasName[1];
            };

            int posX = node["X"].ValueInt;
            int posY = node["Y"].ValueInt;
            Point pos = new Point(posX, posY);

            bool isNoOverwrite = node.HasChildWithKey("NO_OVERWRITE");
            bool isErase = node.HasChildWithKey("ERASE");
            bool isOneWay = node.HasChildWithKey("ONE_WAY");

            bool doRotate = node.HasChildWithKey("ROTATE");
            bool doInvert = node.HasChildWithKey("FLIP_VERTICAL");
            bool doFlip = node.HasChildWithKey("FLIP_HORIZONTAL");

            int specWidth = node["WIDTH"].ValueInt;
            int specHeight = node["HEIGHT"].ValueInt;

            if (pieceAlias.HasValue)
            {
                Alias alias = pieceAlias.Value;
                if (specWidth <= 0 && alias.Width > 0) specWidth = alias.Width;
                if (specHeight <= 0 && alias.Height > 0) specHeight = alias.Height;
            }

            if (doRotate)
            {
                // Swap width and height, to swap it again once the gadget is rotated
                Utility.Swap(ref specWidth, ref specHeight);
            }

            TerrainPiece newTerrain;

            if (styleName.ToUpperInvariant() == "*GROUP")
            {
                newTerrain = new GroupPiece(groupPieceSamples.FirstOrDefault(gs => gs.Name.ToUpperInvariant() == pieceName.ToUpperInvariant()), pos, 0, false, isErase, isNoOverwrite, isOneWay);
            }
            else
            {
                // ... then create the correct Terrain piece
                string key = ImageLibrary.CreatePieceKey(styleName, pieceName, false);
                newTerrain = new TerrainPiece(key, pos, 0, false, isErase, isNoOverwrite, isOneWay, specWidth, specHeight);
            }

            // For compatibility with player: NoOverwrite + Erase pieces work like NoOverWrite
            if (newTerrain.IsNoOverwrite && newTerrain.IsErase)
                newTerrain.IsErase = false;
            if (newTerrain.IsSteel)
                newTerrain.IsOneWay = false;

            if (doRotate)
                newTerrain.RotateInRect(newTerrain.ImageRectangle);
            if (doFlip)
                newTerrain.FlipInRect(newTerrain.ImageRectangle);
            if (doInvert)
                newTerrain.InvertInRect(newTerrain.ImageRectangle);

            //Reposition terrain piece to be sure...
            newTerrain.PosX = pos.X;
            newTerrain.PosY = pos.Y;

            newTerrain.IsSelected = false;

            return newTerrain;
        }

        private static void LoadRulers(Level level, SLXTextDataNode node)
        {
            // First read in all infos
            string pieceName = node["PIECE"].Value;
            int posX = node["X"].ValueInt;
            int posY = node["Y"].ValueInt;

            bool doRotate = node.HasChildWithKey("ROTATE");
            bool doInvert = node.HasChildWithKey("FLIP_VERTICAL");
            bool doFlip = node.HasChildWithKey("FLIP_HORIZONTAL");

            // ... then create the correct ruler
            string key = $"rulers\\" + pieceName;
            Point pos = new Point(posX, posY);
            GadgetPiece newRuler = new GadgetPiece(key, pos);

            if (doRotate)
                newRuler.RotateInRect(newRuler.ImageRectangle);
            if (doFlip)
                newRuler.FlipInRect(newRuler.ImageRectangle);
            if (doInvert)
                newRuler.InvertInRect(newRuler.ImageRectangle);
            //Reposition terrain piece to be sure...
            newRuler.PosX = pos.X;
            newRuler.PosY = pos.Y;

            newRuler.IsSelected = false;

            level.GadgetList.Add(newRuler);
        }

        // Counts the number of collectibles in the level
        static int CountCollectibles(Level curLevel)
        {
            return curLevel.GadgetList.Count(gad => gad.ObjType == C.OBJ.COLLECTIBLE);
        }

        private static void LoadTalisman(Level level, SLXTextDataNode node)
        {
            Talisman talisman = new Talisman();

            talisman.Title = node["TITLE"].Value;
            talisman.AwardType = Utility.ParseEnum<C.TalismanType>(node["COLOR"].Value);
            talisman.ID = node["ID"].ValueInt;

            foreach (KeyValuePair<C.TalismanReq, string> pair in C.TalismanKeys)
            {
                if (pair.Key == C.TalismanReq.UseOnlySkill)
                    continue;
                else if (node.HasChildWithKey(pair.Value))
                    talisman.Requirements[pair.Key] = node[pair.Value].ValueInt;
            }

            if (node.HasChildWithKey(C.TalismanKeys[C.TalismanReq.UseOnlySkill]))
            {
                string allowedSkill = node[C.TalismanKeys[C.TalismanReq.UseOnlySkill]].Value;
                for (int i = 0; i < C.TalismanSkills.Count; i++)
                    if (allowedSkill.ToUpperInvariant() == C.TalismanSkills[i].ToUpperInvariant())
                    {
                        talisman.Requirements[C.TalismanReq.UseOnlySkill] = i;
                        break;
                    }
            }

            level.Talismans.Add(talisman);
        }

        /// <summary>
        /// Ensures that all level parameters are within sensible limits.
        /// </summary>
        /// <param name="newLevel"></param>
        static private void SanitizeInput(Level newLevel)
        {
            // Level size
            newLevel.Width = Math.Max(Math.Min(newLevel.Width, 3200), 1);
            newLevel.Height = Math.Max(Math.Min(newLevel.Height, 1600), 1);
            // Start position
            newLevel.StartPosX = Math.Max(Math.Min(newLevel.StartPosX, newLevel.Width - 1), 0);
            newLevel.StartPosY = Math.Max(Math.Min(newLevel.StartPosY, newLevel.Height - 1), 0);
            // Global level properties
            newLevel.NumLems = Math.Max(Math.Min(newLevel.NumLems, 999), 1);
            newLevel.SaveReq = Math.Max(Math.Min(newLevel.SaveReq, 999), 1);
            newLevel.SpawnInterval = Math.Max(Math.Min(newLevel.SpawnInterval, 102), 4);
            newLevel.ReleaseRate = Math.Max(Math.Min(newLevel.ReleaseRate, 99), 1);
            newLevel.TimeLimit = Math.Max(Math.Min(newLevel.TimeLimit, 5999), 0);
            // Skill numbers
            foreach (C.Skill skill in C.SkillArray)
            {
                newLevel.SkillSet[skill] = Math.Max(Math.Min(newLevel.SkillSet[skill], 100), 0);
            }
        }



        /// <summary>
        /// Opens file browser and saves the current level to a specified file.
        /// </summary>
        /// <param name="curLevel"></param>
        static public void SaveLevel(Level curLevel, string levelDirectory, bool useBothFormats)
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

            if (useBothFormats)
                saveFileDialog.Filter = "SuperLemmix level files (*.sxlv)|*.sxlv|" + "NeoLemmix level files (*.nxlv)|*.nxlv";
            else
                saveFileDialog.Filter = "SuperLemmix level files (*.sxlv)|*.sxlv";

            saveFileDialog.DefaultExt = "sxlv";
            saveFileDialog.OverwritePrompt = true;
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

            curLevel.PrepareForSave();

            TextWriter textFile = new StreamWriter(filePath, true);

            textFile.WriteLine("# ----------------------------- ");
            textFile.WriteLine($"#        {curLevel.Format} Level      ");
            textFile.WriteLine("#   Created with SLXEditor " + C.Version);
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
            textFile.WriteLine(" ID x" + curLevel.LevelID.ToString("X16"));
            textFile.WriteLine(" VERSION x" + curLevel.LevelVersion.ToString("X16"));
            textFile.WriteLine(" ");

            textFile.WriteLine("#       Level dimensions        ");
            textFile.WriteLine("# ----------------------------- ");
            textFile.WriteLine(" WIDTH " + curLevel.Width.ToString());
            textFile.WriteLine(" HEIGHT " + curLevel.Height.ToString());
            if (!curLevel.AutoStartPos)
            {
                textFile.WriteLine(" START_X " + curLevel.StartPosX.ToString());
                textFile.WriteLine(" START_Y " + curLevel.StartPosY.ToString());
            }
            textFile.WriteLine(" THEME " + curLevel.MainStyle?.NameInDirectory);

            if (curLevel.Background != null)
            {
                string bgText = curLevel.Background.Style.NameInDirectory + ":" + curLevel.Background.Name;
                if (bgText.Trim() != ":")
                    textFile.WriteLine(" BACKGROUND " + bgText);
            }
            textFile.WriteLine(" ");

            textFile.WriteLine("#         Level stats           ");
            textFile.WriteLine("# ----------------------------- ");
            textFile.WriteLine(" LEMMINGS " + curLevel.NumLems.ToString());
            textFile.WriteLine(" SAVE_REQUIREMENT " + curLevel.SaveReq.ToString());
            if (curLevel.HasTimeLimit)
            {
                textFile.WriteLine(" TIME_LIMIT " + curLevel.TimeLimit.ToString());
            }
            textFile.WriteLine(" MAX_SPAWN_INTERVAL " + curLevel.SpawnInterval.ToString());
            if (curLevel.IsSpawnRateFix)
            {
                textFile.WriteLine(" SPAWN_INTERVAL_LOCKED");
            }
            textFile.WriteLine(" STEEL_TYPE " + curLevel.SteelType.ToString());
            int collectiblesCount = CountCollectibles(curLevel);
            if (collectiblesCount > 0)
            {
                textFile.WriteLine(" COLLECTIBLES " + collectiblesCount);
            }
            if (curLevel.IsSuperlemming)
            {
                textFile.WriteLine(" SUPERLEMMING");
            }
            if (curLevel.IsInvincibility && (collectiblesCount > 0))
            {
                textFile.WriteLine(" INVINCIBILITY");
            }
            textFile.WriteLine(" ");

            textFile.WriteLine(" $SKILLSET ");
            foreach (C.Skill skill in C.SkillArray)
            {
                if (IsSkillRequired(curLevel, skill))
                {
                    var count = curLevel.SkillSet[skill] > 99 ? "INFINITE" : curLevel.SkillSet[skill].ToString();
                    textFile.WriteLine(PaddedSkillString(skill) + count);
                }
            }
            textFile.WriteLine(" $END ");
            textFile.WriteLine(" ");

            if (GetTextNeedsSaving(curLevel.PreviewText))
            {
                textFile.WriteLine(" $PRETEXT ");
                curLevel.PreviewText.ForEach(lin => textFile.WriteLine("   LINE " + lin));
                textFile.WriteLine(" $END ");
                textFile.WriteLine(" ");
            }

            if (GetTextNeedsSaving(curLevel.PostviewText))
            {
                textFile.WriteLine(" $POSTTEXT ");
                curLevel.PostviewText.ForEach(lin => textFile.WriteLine("   LINE " + lin));
                textFile.WriteLine(" $END ");
                textFile.WriteLine(" ");
            }

            curLevel.Talismans.ForEach(tal => WriteTalisman(textFile, tal));

            textFile.WriteLine("#     Interactive objects       ");
            textFile.WriteLine("# ----------------------------- ");
            curLevel.GadgetList.FindAll(gad => !gad.ObjType.In(C.OBJ.LEMMING, C.OBJ.RULER))
                               .ForEach(gad => WriteObject(textFile, gad));
            textFile.WriteLine(" ");

            textFile.WriteLine("#           Rulers              ");
            textFile.WriteLine("# ----------------------------- ");
            curLevel.GadgetList.FindAll(gad => gad.ObjType == C.OBJ.RULER)
                               .ForEach(gad => WriteObject(textFile, gad));
            textFile.WriteLine(" ");

            if (curLevel.TerrainList.Exists(ter => ter is GroupPiece))
            {
                textFile.WriteLine("#        Terrain groups         ");
                textFile.WriteLine("# ----------------------------- ");

                List<GroupPiece> groupPieces = curLevel.TerrainList.FindAll(ter => ter is GroupPiece).Cast<GroupPiece>().ToList();
                List<GroupPiece> uniqueGroupPieces = new List<GroupPiece>();

                while (groupPieces.Count > 0)
                {
                    GroupPiece group = groupPieces[0];
                    groupPieces.RemoveAll(grp => grp.Key == group.Key);

                    if (!uniqueGroupPieces.Exists(grp => grp.Key == group.Key))
                    {
                        groupPieces.AddRange(group.GetConstituents().FindAll(ter => ter is GroupPiece).Cast<GroupPiece>());
                        uniqueGroupPieces.Add(group);
                    }
                }

                SortGroupPieces(uniqueGroupPieces);

                uniqueGroupPieces.ForEach(grp => WriteGroup(textFile, grp));

                textFile.WriteLine(" ");
            }

            textFile.WriteLine("#        Terrain pieces         ");
            textFile.WriteLine("# ----------------------------- ");
            curLevel.TerrainList.ForEach(ter => WriteTerrain(textFile, ter, curLevel.TerrainList.IndexOf(ter)));
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

        private static bool GetTextNeedsSaving(List<string> text)
        {
            if (text == null)
                return false;

            foreach (var line in text)
                if (!String.IsNullOrWhiteSpace(line))
                    return true;

            return false;
        }

        private static void SortGroupPieces(List<GroupPiece> pieces)
        {
            // The normal Sort function doesn't seem to compare every possible pairing, and thus gives an incorrect result.

            int i = 0;

            while (i + 1 < pieces.Count)
            {
                var a = pieces[i];

                for (int i2 = i + 1; i2 < pieces.Count; i2++)
                {
                    bool aContainsB = false;
                    bool bContainsA = false;
                    var b = pieces[i2];

                    foreach (GroupPiece piece in a.GetConstituents().FindAll(pc => pc is GroupPiece))
                        if (piece.Key == b.Key)
                        {
                            aContainsB = true;
                            break;
                        }

                    foreach (GroupPiece piece in b.GetConstituents().FindAll(pc => pc is GroupPiece))
                        if (piece.Key == a.Key)
                        {
                            bContainsA = true;
                            break;
                        }

                    if (aContainsB && bContainsA)
                        throw new Exception("Recursive terrain grouping");
                    else if (aContainsB)
                    {
                        pieces.Remove(b);
                        pieces.Insert(i, b);
                        i = -1;
                        break;
                    }
                }

                i++;
            }
        }

        /// <summary>
        /// Returns whether the skill is in the skill set or available as a pickup skill. 
        /// </summary>
        /// <param name="curLevel"></param>
        /// <param name="skillNum"></param>
        static public bool IsSkillRequired(Level curLevel, C.Skill skill)
        {
            return (curLevel.SkillSet[skill] > 0)
                || (curLevel.GadgetList.Exists(gad => gad.ObjType == C.OBJ.PICKUP && gad.SkillFlags.Contains(skill)));
        }

        static public bool NeedFlipOffset(GadgetPiece gadget)
        {
            if (gadget.ObjType == C.OBJ.HATCH && gadget.FlipOffset != 0)
                return true;
            else
                return false;
        }

        /// <summary>
        /// Writes all object infos in a text file.
        /// </summary>
        /// <param name="textFile"></param>
        /// <param name="gadget"></param>
        static private void WriteObject(TextWriter textFile, GadgetPiece gadget)
        {
            if (gadget == null)
                return;
            if (gadget.ObjType == C.OBJ.PICKUP && gadget.SkillFlags.Count == 0)
                return;

            if (gadget.ObjType == C.OBJ.LEMMING)
            {
                textFile.WriteLine(" $LEMMING");
            }
            else if (gadget.ObjType == C.OBJ.RULER)
            {
                textFile.WriteLine(" $RULER");
                textFile.WriteLine("   PIECE " + gadget.Name);
            }
            else
            {
                textFile.WriteLine(" $GADGET");
                textFile.WriteLine("   STYLE " + gadget.Style);
                textFile.WriteLine("   PIECE " + gadget.Name);
            }

            Point levelFilePos = ImageLibrary.EditorToLevelFileCoordinates(gadget.Key, gadget.Pos, gadget.IsRotatedInPlayer,
              gadget.IsFlippedInPlayer, gadget.IsInvertedInPlayer);
            int posX = levelFilePos.X + (gadget.ObjType == C.OBJ.LEMMING ? C.LEM_OFFSET_X : 0);
            int posY = levelFilePos.Y + (gadget.ObjType == C.OBJ.LEMMING ? C.LEM_OFFSET_Y : 0);
            textFile.WriteLine("   X " + posX.ToString());
            textFile.WriteLine("   Y " + posY.ToString());

            if (gadget.MayResizeHoriz())
            {
                textFile.WriteLine("   WIDTH " + gadget.Width.ToString());
            }
            if (gadget.MayResizeVert())
            {
                textFile.WriteLine("   HEIGHT " + gadget.Height.ToString());
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
            if (gadget.IsFlippedInPlayer)
            {
                textFile.WriteLine("   FLIP_HORIZONTAL");
            }
            if (NeedFlipOffset(gadget))
            {
                textFile.WriteLine($"   FLIP_X_OFFSET {gadget.FlipOffset}");
            }
            if (gadget.IsInvertedInPlayer)
            {
                textFile.WriteLine("   FLIP_VERTICAL");
            }
            if (gadget.ObjType.In(C.OBJ.HATCH, C.OBJ.LEMMING))
            {
                foreach (C.Skill skill in gadget.SkillFlags)
                {
                    textFile.WriteLine("   " + SkillString(skill) + " ");
                }
            }
            else if (gadget.ObjType.In(C.OBJ.EXIT, C.OBJ.EXIT_LOCKED))
            {
                if (gadget.SkillFlags.Contains(C.Skill.Rival))
                {
                    textFile.WriteLine("   " + SkillString(C.Skill.Rival) + " ");
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
                    textFile.WriteLine("   SKILL_COUNT " + gadget.Val_L.ToString());
                }
            }
            else if (gadget.ObjType.In(C.OBJ.PERMASKILL_ADD, C.OBJ.SKILL_ASSIGNER))
            {
                foreach (C.Skill skill in gadget.SkillFlags)
                {
                    textFile.WriteLine("   SKILL " + SkillString(skill));
                }
            }

            if (gadget.ObjType.In(C.OBJ.TELEPORTER, C.OBJ.RECEIVER, C.OBJ.PORTAL))
            {
                textFile.WriteLine("   PAIRING " + gadget.Val_L.ToString());
            }

            if (gadget.ObjType.In(C.OBJ.RADIATION, C.OBJ.SLOWFREEZE) && gadget.CountdownLength > 0)
            {
                textFile.WriteLine("   COUNTDOWN " + gadget.CountdownLength.ToString());
            }

            if (gadget.ObjType.In(C.OBJ.DECORATION))
            {
                textFile.WriteLine("   SPEED " + gadget.DecorationSpeed.ToString());
                textFile.WriteLine("   ANGLE " + gadget.DecorationAngle.ToString());
            }

            if (gadget.ObjType.In(C.OBJ.EXIT, C.OBJ.EXIT_LOCKED, C.OBJ.HATCH) && gadget.LemmingCap > 0)
            {
                textFile.WriteLine("   LEMMINGS " + gadget.LemmingCap);
            }

            textFile.WriteLine(" $END");
            textFile.WriteLine(" ");
        }

        private static void WriteGroup(TextWriter textFile, GroupPiece group)
        {
            textFile.WriteLine(" $TERRAINGROUP");
            textFile.WriteLine("   NAME " + group.Name);
            textFile.WriteLine("   ");

            foreach (TerrainPiece piece in group.GetConstituents())
                WriteTerrain(textFile, piece, 1);

            textFile.WriteLine(" $END");
            textFile.WriteLine(" ");
        }

        private static void WriteTerrain(TextWriter textFile, TerrainPiece terrain, int extraIndent)
        {
            WriteTerrain(textFile, terrain, -1, extraIndent);
        }

        /// <summary>
        /// Writes all terrain piece infos in a text file.
        /// </summary>
        /// <param name="textFile"></param>
        /// <param name="terrain"></param>
        static private void WriteTerrain(TextWriter textFile, TerrainPiece terrain, int index, int extraIndent = 0)
        {
            string prefix = new string(' ', extraIndent * 2);

            textFile.WriteLine(prefix + " $TERRAIN");

            if (terrain is GroupPiece)
                textFile.WriteLine(prefix + "   STYLE *group");
            else
                textFile.WriteLine(prefix + "   STYLE " + terrain.Style);

            textFile.WriteLine(prefix + "   PIECE " + terrain.Name);
            textFile.WriteLine(prefix + "   X " + terrain.PosX.ToString());
            textFile.WriteLine(prefix + "   Y " + terrain.PosY.ToString());
            if (terrain.IsNoOverwrite)
            {
                textFile.WriteLine(prefix + "   NO_OVERWRITE");
            }
            if (terrain.IsErase)
            {
                textFile.WriteLine(prefix + "   ERASE");
            }
            if (terrain.IsRotatedInPlayer)
            {
                textFile.WriteLine(prefix + "   ROTATE");
            }
            if (terrain.IsInvertedInPlayer)
            {
                textFile.WriteLine(prefix + "   FLIP_VERTICAL");
            }
            if (terrain.IsFlippedInPlayer)
            {
                textFile.WriteLine(prefix + "   FLIP_HORIZONTAL");
            }
            if (terrain.IsOneWay)
            {
                textFile.WriteLine(prefix + "   ONE_WAY");
            }
            if (terrain.MayResizeHoriz())
            {
                textFile.WriteLine(prefix + "   WIDTH " + terrain.Width.ToString());
            }
            if (terrain.MayResizeVert())
            {
                textFile.WriteLine(prefix + "   HEIGHT " + terrain.Height.ToString());
            }
            textFile.WriteLine(prefix + " $END");
            textFile.WriteLine(prefix + " ");
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
                    textFile.WriteLine("   " + C.TalismanKeys[requirement] + " " + C.TalismanSkills[talisman.Requirements[requirement]]);
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
        static string SkillString(C.Skill skill)
        {
            return Enum.GetName(typeof(C.Skill), skill).ToUpper();
        }


        /// <summary>
        /// Returns the name of the skill as a string, padded to length 12.
        /// </summary>
        /// <param name="skill"></param>
        static string PaddedSkillString(C.Skill skill)
        {
            return "   " + SkillString(skill) + " ";
        }

        /// <summary>
        /// Converts an old .lvl level file to the current format (sxlv/nxlv - we'll need to know the fileExt).
        /// Not currently used, but the code remains here because it may be useful for an auto-cleanse feature in the future.
        /// <para> This calls SuperLemmix.exe written in Delphi. </para>
        /// </summary>
        static bool ConvertWithSuperLemmix(string filePath, string fileExt)
        {
            if (!File.Exists(C.AppPathSuperLemmix))
                return false;

            // Compare version number of the SuperLemmix.exe file
            var versionInfo = System.Diagnostics.FileVersionInfo.GetVersionInfo(C.AppPathSuperLemmix);
            string[] fileVersion = versionInfo.FileVersion.Split('.');
            try
            {
                if (int.Parse(fileVersion[0]) < 12)
                    return false;
            }
            // If that fails, the version is always wrong!
            catch (FormatException) { return false; }
            catch (ArgumentNullException) { return false; }

            try
            {
                Utility.DeleteFile(C.AppPathTempLevel + fileExt);

                var converterStartInfo = new System.Diagnostics.ProcessStartInfo();
                converterStartInfo.FileName = C.AppPathSuperLemmix;
                converterStartInfo.Arguments = "convert \"" + filePath + "\" \"" + C.AppPathTempLevel + fileExt + "\"";

                var converterProcess = System.Diagnostics.Process.Start(converterStartInfo);
                converterProcess.WaitForExit();

                return File.Exists(C.AppPathTempLevel + fileExt);
            }
            catch
            {
                return false;
            }
        }
    }
}
