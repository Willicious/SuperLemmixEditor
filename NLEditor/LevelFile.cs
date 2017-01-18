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
        static public Level LoadLevel(List<Style> styleList)
        {
            var openFileDialog = new OpenFileDialog();

            openFileDialog.InitialDirectory = C.AppPath;
            openFileDialog.Multiselect = false;
            openFileDialog.Filter = "NeoLemmix level files (*.nxlv)|*.nxlv|Old level files (*.lvl)|*.lvl";
            openFileDialog.RestoreDirectory = true;
            openFileDialog.CheckFileExists = true;


            Level newLevel = null;

            try
            {
                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    string filePath = openFileDialog.FileName;
                    if (filePath.EndsWith("nxlv"))
                    {
                        newLevel = LoadLevelFromFile(filePath, styleList);
                    }
                    else if (filePath.EndsWith("lvl"))
                    {
                        bool IsConverted = ConvertOldLevelType(filePath);
                        if (IsConverted)
                        {
                            newLevel = LoadLevelFromFile(C.AppPathTempLevel, styleList);
                        } 
                    }
                    
                }
            }
            catch (Exception Ex)
            {
                Utility.LogException(Ex);
                MessageBox.Show("Error while showing the file browser." + C.NewLine + Ex.Message);
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
        static private Level LoadLevelFromFile(string filePath, List<Style> styleList)
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
                MessageBox.Show(Ex.Message);
                parser?.DisposeStreamReader();
                return newLevel;
            }


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
                                string idString = (line.Text.StartsWith("x")) ? line.Text.Substring(1) : line.Text;
                                newLevel.LevelID = uint.Parse(idString, System.Globalization.NumberStyles.HexNumber);
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
                        case "RELEASE_RATE": newLevel.ReleaseRate = line.Value; break;
                        case "RELEAST_RATE_LOCKED": newLevel.IsReleaseRateFix = true; break;
                        case "BACKGROUND": newLevel.BackgroundKey = line.Text; break;

                        case "SKILLSET":
                            ReadSkillSetFromLines(fileLines, newLevel);
                            newLevel.SkillSet[C.Skill.Zombie] = 0; break;
                        case "OBJECT":
                        case "LEMMING": newLevel.GadgetList.Add(ReadGadgetFromLines(fileLines)); break;
                        case "TERRAIN": newLevel.TerrainList.Add(ReadTerrainFromLines(fileLines)); break;
                    }
                }
            }
            catch (Exception Ex)
            {
                Utility.LogException(Ex);
                MessageBox.Show(Ex.Message);
            }
            finally
            {
                parser?.DisposeStreamReader();
            }

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
                }
            }

            // ... then create the correct Gadget piece
            string key = ImageLibrary.CreatePieceKey(styleName, gadgetName, true);
            Point pos = new Point(posX, posY);
            GadgetPiece newGadget = new GadgetPiece(key, pos, 0, false, isNoOverwrite, isOnlyOnTerrain, val_L, skillFlags, specWidth, specHeight);

            // Read in skill information
            foreach (C.Skill skill in C.SkillArray)
            {
                if (fileLineList.Exists(line => line.Key == SkillString(skill)
                                    || (line.Key == "SKILL" && line.Text == SkillString(skill))))
                {
                    newGadget.SetSkillFlag(skill, true);
                }
            }

            // For compatibility with player: NoOverwrite + OnlyOnTerrain gadgets work like OnlyOnTerrain 
            if (newGadget.IsNoOverwrite && newGadget.IsOnlyOnTerrain) newGadget.IsNoOverwrite = false;

            if (doRotate) newGadget.RotateInRect(newGadget.ImageRectangle);
            if (doFlip) newGadget.FlipInRect(newGadget.ImageRectangle);
            if (doInvert) newGadget.InvertInRect(newGadget.ImageRectangle);
            //Reposition gadget to be sure...
            newGadget.PosX = pos.X;
            newGadget.PosY = pos.Y;

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

            // ... then create the correct Gadget piece
            string key = ImageLibrary.CreatePieceKey(styleName, pieceName, false);
            Point pos = new Point(posX, posY);
            TerrainPiece newTerrain = new TerrainPiece(key, pos, 0, false, isErase, isNoOverwrite, isOneWay);

            // For compatibility with player: NoOverwrite + Erase pieces work like NoOverWrite
            if (newTerrain.IsNoOverwrite && newTerrain.IsErase) newTerrain.IsErase = false;
            if (newTerrain.IsSteel) newTerrain.IsOneWay = false;

            if (doRotate) newTerrain.RotateInRect(newTerrain.ImageRectangle);
            if (doFlip) newTerrain.FlipInRect(newTerrain.ImageRectangle);
            if (doInvert) newTerrain.InvertInRect(newTerrain.ImageRectangle);
            //Reposition gadget to be sure...
            newTerrain.PosX = pos.X;
            newTerrain.PosY = pos.Y;

            newTerrain.IsSelected = false;

            return newTerrain;
        }

        /// <summary>
        /// Opens file browser and saves the current level to a .nxlv file.
        /// </summary>
        /// <param name="curLevel"></param>
        static public void SaveLevel(Level curLevel)
        {
            var saveFileDialog = new SaveFileDialog();

            saveFileDialog.AddExtension = true;
            saveFileDialog.InitialDirectory = C.AppPath;
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
                        MessageBox.Show("Could not save the level file!" + Environment.NewLine + Ex.Message);
                    }
                }
            }
            catch (Exception Ex)
            {
                Utility.LogException(Ex);
                MessageBox.Show("Error while showing the file browser." + Environment.NewLine + Ex.Message);
            }
            finally
            {
                saveFileDialog.Dispose();
            }
        }

        /// <summary>
        /// 
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
                MessageBox.Show("Error: Cannot create text file at " + filePath + "." + C.NewLine + Ex.Message);
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
                textFile.WriteLine(" MUSIC " + Path.GetFileNameWithoutExtension(curLevel.MusicFile));
            }
            textFile.WriteLine(" ID x" + curLevel.LevelID.ToString("X"));
            textFile.WriteLine(" ");

            textFile.WriteLine("#       Level dimensions        ");
            textFile.WriteLine("# ----------------------------- ");
            textFile.WriteLine(" WIDTH   " + curLevel.Width.ToString().PadLeft(4));
            textFile.WriteLine(" HEIGHT  " + curLevel.Height.ToString().PadLeft(4));
            textFile.WriteLine(" START_X " + curLevel.StartPosX.ToString().PadLeft(4));
            textFile.WriteLine(" START_Y " + curLevel.StartPosY.ToString().PadLeft(4));
            textFile.WriteLine(" THEME " + curLevel.MainStyle.NameInDirectory);
            if (curLevel.MainStyle.BackgroundNames.Contains(curLevel.BackgroundKey))
            {
                textFile.WriteLine(" BACKGROUND " + Path.GetFileName(curLevel.BackgroundKey));
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
            textFile.WriteLine(" RELEASE_RATE " + curLevel.ReleaseRate.ToString().PadLeft(4));
            if (curLevel.IsReleaseRateFix)
            {
                textFile.WriteLine(" RELEASE_RATE_FIXED ");
            }
            textFile.WriteLine(" ");

            textFile.WriteLine(" $SKILLSET ");
            foreach (C.Skill skill in C.SkillArray)
            {
                if (IsSkillRequired(curLevel, skill))
                {
                    if (curLevel.SkillSet[skill] > 99)
                    {
                        textFile.WriteLine(PaddedSkillString(skill) + "INFINITE");
                    }
                    else
                    {
                        textFile.WriteLine(PaddedSkillString(skill) + curLevel.SkillSet[skill].ToString().PadLeft(4));
                    }
                }
            }
            textFile.WriteLine(" $END ");
            textFile.WriteLine(" ");

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
            return    (curLevel.SkillSet[skill] > 0)
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
            textFile.WriteLine("   X " + gadget.PosX.ToString().PadLeft(5));
            textFile.WriteLine("   Y " + gadget.PosY.ToString().PadLeft(5));

            if (gadget.SpecWidth > 0)
            {
                textFile.WriteLine("   WIDTH  " + gadget.SpecWidth.ToString().PadLeft(5));
            }
            if (gadget.SpecHeight > 0)
            {
                textFile.WriteLine("   HEIGHT " + gadget.SpecWidth.ToString().PadLeft(5));
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
            if (gadget.IsFlippedInPlayer)
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
                    textFile.WriteLine("  SKILL" + SkillString(skill));
                }
            }

            if (gadget.ObjType.In(C.OBJ.TELEPORTER, C.OBJ.RECEIVER))
            {
                textFile.WriteLine("   PAIRING " + gadget.Val_L.ToString().PadLeft(4));
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
        /// <para> This calls NLConverter.exe written in Delphi. </para>
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        static bool ConvertOldLevelType(string filePath)
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
                MessageBox.Show(C.FileConverterErrorMsg[exitCode]);
            }
            else if (exitCode >= 10)
            {
                MessageBox.Show("Error: Level converter crashed due to unhandles exception.");
            }

            return (exitCode < 10);
        }

    }
}
