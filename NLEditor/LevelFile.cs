using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
        static readonly string[] fSkillNames = 
            { 
                "    CLIMBER ", "    FLOATER ", "     BOMBER ", "    BLOCKER ",
                "    BUILDER ", "     BASHER ", "      MINER ", "     DIGGER ",               
                "     WALKER ", "    SWIMMER ", "     GLIDER ", "   DISARMER ",                  
                "     STONER ", " PLATFORMER ", "    STACKER ", "     CLONER ",
                "     ZOMBIE ",
            };
        
        /// <summary>
        /// Opens file browser and creates level from a .nxlv file.
        /// <para> Returns null if process is aborted or file is corrupt. </para>
        /// </summary>
        /// <param name="StyleList"></param>
        /// <returns></returns>
        static public Level LoadLevel(List<Style> StyleList)
        {
            OpenFileDialog CurOpenFileDialog = new OpenFileDialog();

            CurOpenFileDialog.InitialDirectory = C.AppPath;
            CurOpenFileDialog.Multiselect = false;
            CurOpenFileDialog.Filter = "NeoLemmix level files (*.nxlv)|*.nxlv";
            CurOpenFileDialog.RestoreDirectory = true;
            CurOpenFileDialog.CheckFileExists = true;

            Level NewLevel = null;

            if (CurOpenFileDialog.ShowDialog() == DialogResult.OK)
            {
                string FilePath = CurOpenFileDialog.FileName;
                NewLevel = LoadLevelFromFile(FilePath, StyleList);
            }

            CurOpenFileDialog.Dispose();

            return NewLevel;
        }

        /// <summary>
        /// Creates level from a .nxlv file.
        /// <para> Null if file cannot be opened. </para>
        /// </summary>
        /// <param name="FilePath"></param>
        /// <param name="StyleList"></param>
        /// <returns></returns>
        static private Level LoadLevelFromFile(string FilePath, List<Style> StyleList)
        {
            Level NewLevel = new Level();

            FileParser MyParser;
            try
            {
                MyParser = new FileParser(FilePath);
            }
            catch (Exception Ex)
            {
                Utility.LogException(Ex);
                MessageBox.Show(Ex.Message);
                return NewLevel;
            }


            try
            {
                List<FileLine> FileLineList;
                while ((FileLineList = MyParser.GetNextLines()) != null)
                {
                    System.Diagnostics.Debug.Assert(FileLineList.Count > 0, "FileParser returned empty list.");

                    FileLine Line = FileLineList[0];
                    switch (Line.Key)
                    {
                        case "TITLE": NewLevel.Title = Line.Text; break;
                        case "AUTHOR": NewLevel.Author = Line.Text; break;
                        case "ID":
                            {
                                string IDString = (Line.Text.StartsWith("x")) ? Line.Text.Substring(1) : Line.Text;
                                NewLevel.LevelID = uint.Parse(IDString, System.Globalization.NumberStyles.HexNumber); 
                                break;
                            }
                        case "MUSIC": NewLevel.MusicFile = Line.Text; break;
                        case "WIDTH": NewLevel.Width = Line.Value; break;
                        case "HEIGHT": NewLevel.Height = Line.Value; break;
                        case "START_X": NewLevel.StartPosX = Line.Value; break;
                        case "START_Y": NewLevel.StartPosY = Line.Value; break;
                        case "THEME": NewLevel.MainStyle = StyleList.Find(sty => sty.NameInDirectory == Line.Text); break;
                        case "LEMMINGS": NewLevel.NumLems = Line.Value; break;
                        case "REQUIREMENT": NewLevel.SaveReq = Line.Value; break;
                        case "TIME_LIMIT": NewLevel.TimeLimit = Line.Value;
                                           NewLevel.IsNoTimeLimit = false; break;
                        case "RELEASE_RATE": NewLevel.ReleaseRate = Line.Value; break;
                        case "RELEAST_RATE_LOCKED": NewLevel.IsReleaseRateFix = true; break;
                        case "BACKGROUND": NewLevel.BackgroundKey = Line.Text; break;

                        case "SKILLSET": ReadSkillSetFromLines(FileLineList, NewLevel); break;
                        case "OBJECT": 
                        case "LEMMING": NewLevel.GadgetList.Add(ReadGadgetFromLines(FileLineList)); break;
                        case "TERRAIN": NewLevel.TerrainList.Add(ReadTerrainFromLines(FileLineList)); break;
                    }
                }
            }
            catch (Exception Ex)
            {
                Utility.LogException(Ex);
                MessageBox.Show(Ex.Message);
            }


            if (MyParser != null) MyParser.DisposeStreamReader();

            return NewLevel;
        }

        /// <summary>
        /// Reads the skill set from the skill section.
        /// </summary>
        /// <param name="FileLineList"></param>
        /// <param name="NewLevel"></param>
        static private void ReadSkillSetFromLines(List<FileLine> FileLineList, Level NewLevel)
        {
            for (int SkillIndex = 0; SkillIndex < C.SKI_COUNT; SkillIndex++)
            {
                FileLine Line = FileLineList.Find(line => line.Key == fSkillNames[SkillIndex].Trim());
                if (Line != null)
                {
                    if (Line.Text == "INFINITE")
                    {
                        NewLevel.SkillCount[SkillIndex] = 100;
                    }
                    else
                    {
                        NewLevel.SkillCount[SkillIndex] = Line.Value;
                    }
                }
            }
        }


        /// <summary>
        /// Creates a gadget from a block of file lines.
        /// </summary>
        /// <param name="FileLineList"></param>
        /// <returns></returns>
        static private GadgetPiece ReadGadgetFromLines(List<FileLine> FileLineList)
        { 
            // First read in all infos
            string StyleName = "default"; // default value, because they are not set for preplaced lemmings 
            string GadgetName = "lemming"; // default value, because they are not set for preplaced lemmings 
            int PosX = 0;
            int PosY = 0;
            bool IsNoOverwrite = false;
            bool IsOnlyOnTerrain = false;
            int SpecWidth = -1;
            int SpecHeight = -1;

            bool DoRotate = false;
            bool DoInvert = false;
            bool DoFlip = false;
            int Val_L = 0;
            int Val_S = 0;

            foreach (FileLine Line in FileLineList)
            {
                switch (Line.Key)
                {
                    case "COLLECTION": StyleName = Line.Text; break;
                    case "PIECE": GadgetName = Line.Text; break;
                    case "X": PosX = Line.Value; break;
                    case "Y": PosY = Line.Value; break;
                    case "WIDTH": SpecWidth = Line.Value; break;
                    case "HEIGHT": SpecHeight = Line.Value; break;
                    case "NO_OVERWRITE": IsNoOverwrite = true; break;
                    case "ONLY_ON_TERRAIN": IsOnlyOnTerrain = true; break;
                    case "ROTATE": DoRotate = true; break;
                    case "FLIP_HORIZONTAL": DoFlip = true; break;
                    case "FLIP_VERTICAL": DoInvert = true; break;
                    case "DIRECTION": DoFlip = Line.Text.ToUpper().StartsWith("L"); break;
                    case "FLIP_LEMMING": DoFlip = true; break;
                    case "PAIRING": Val_L = Line.Value; break;
                }
            }

            // ... then create the correct Gadget piece
            string Key = ImageLibrary.CreatePieceKey(StyleName, GadgetName, true);
            Point Pos = new Point(PosX, PosY);
            GadgetPiece NewGadget = new GadgetPiece(Key, Pos, 0, false, IsNoOverwrite, IsOnlyOnTerrain, Val_L, Val_S, SpecWidth, SpecHeight);

            // Read in skill information
            for (int SkillIndex = 0; SkillIndex < C.SKI_COUNT + 1; SkillIndex++)
            {
                FileLine Line = FileLineList.Find(line => line.Key == fSkillNames[SkillIndex].Trim()
                                 || (line.Key == "SKILL" && line.Text == fSkillNames[SkillIndex].Trim()));
                if (Line != null)
                {
                    NewGadget.SetSkillFlag(SkillIndex, true);
                }
            }

            // For compatibility with player: NoOverwrite + OnlyOnTerrain gadgets work like OnlyOnTerrain 
            if (NewGadget.IsNoOverwrite && NewGadget.IsOnlyOnTerrain) NewGadget.IsNoOverwrite = false;

            if (DoRotate) NewGadget.RotateInRect(NewGadget.ImageRectangle);
            if (DoFlip) NewGadget.FlipInRect(NewGadget.ImageRectangle);
            if (DoInvert) NewGadget.InvertInRect(NewGadget.ImageRectangle);
            //Reposition gadget to be sure...
            NewGadget.PosX = Pos.X;
            NewGadget.PosY = Pos.Y;

            NewGadget.IsSelected = false;

            return NewGadget;
        }

        /// <summary>
        /// Creates a terrain piece from a block of file lines.
        /// </summary>
        /// <param name="FileLineList"></param>
        /// <returns></returns>
        static private TerrainPiece ReadTerrainFromLines(List<FileLine> FileLineList)
        {
            // First read in all infos
            string StyleName = "";
            string TerrainName = "";
            int PosX = 0;
            int PosY = 0;

            bool IsNoOverwrite = false;
            bool IsErase = false;
            bool IsOneWay = false;

            bool DoRotate = false;
            bool DoInvert = false;
            bool DoFlip = false;

            foreach (FileLine Line in FileLineList)
            {
                switch (Line.Key)
                {
                    case "COLLECTION": StyleName = Line.Text; break;
                    case "PIECE": TerrainName = Line.Text; break;
                    case "X": PosX = Line.Value; break;
                    case "Y": PosY = Line.Value; break;
                    case "NO_OVERWRITE": IsNoOverwrite = true; break;
                    case "ERASE": IsErase = true; break;
                    case "ONE_WAY": IsOneWay = true; break;
                    case "ROTATE": DoRotate = true; break;
                    case "FLIP_HORIZONTAL": DoFlip = true; break;
                    case "FLIP_VERTICAL": DoInvert = true; break;
                }
            }

            // ... then create the correct Gadget piece
            string Key = ImageLibrary.CreatePieceKey(StyleName, TerrainName, false);
            Point Pos = new Point(PosX, PosY);
            TerrainPiece NewTerrain = new TerrainPiece(Key, Pos, 0, false, IsErase, IsNoOverwrite, IsOneWay);

            // For compatibility with player: NoOverwrite + Erase pieces work like NoOverWrite
            if (NewTerrain.IsNoOverwrite && NewTerrain.IsErase) NewTerrain.IsErase = false;
            if (NewTerrain.IsSteel) NewTerrain.IsOneWay = false;

            if (DoRotate) NewTerrain.RotateInRect(NewTerrain.ImageRectangle);
            if (DoFlip) NewTerrain.FlipInRect(NewTerrain.ImageRectangle);
            if (DoInvert) NewTerrain.InvertInRect(NewTerrain.ImageRectangle);
            //Reposition gadget to be sure...
            NewTerrain.PosX = Pos.X;
            NewTerrain.PosY = Pos.Y;

            NewTerrain.IsSelected = false;

            return NewTerrain;
        }

        /// <summary>
        /// Opens file browser and saves the current level to a .nxlv file.
        /// </summary>
        /// <param name="CurLevel"></param>
        static public void SaveLevel(Level CurLevel)
        {
            SaveFileDialog CurSaveFileDialog = new SaveFileDialog();

            CurSaveFileDialog.AddExtension = true;
            CurSaveFileDialog.InitialDirectory = C.AppPath;
            CurSaveFileDialog.OverwritePrompt = true;
            CurSaveFileDialog.Filter = "NeoLemmix level files (*.nxlv)|*.nxlv";
            CurSaveFileDialog.RestoreDirectory = true;

            if (CurSaveFileDialog.ShowDialog() == DialogResult.OK)
            {
                string FilePath = CurSaveFileDialog.FileName;
                try
                {
                    SaveLevelToFile(FilePath, CurLevel);
                    CurLevel.FilePathToSave = FilePath;
                }
                catch (Exception Ex)
                {
                    Utility.LogException(Ex);
                    MessageBox.Show("Could not save the level file!" + Environment.NewLine + Ex.Message);
                }
            }

            CurSaveFileDialog.Dispose();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="FilePath"></param>
        /// <param name="CurLevel"></param>
        static public void SaveLevelToFile(string FilePath, Level CurLevel)
        {
            // Create new empty file
            try
            {
                File.Create(FilePath).Close();
            }
            catch (Exception Ex)
            {
                Utility.LogException(Ex);
                MessageBox.Show(Ex.Message);
                return;
            }
            
            TextWriter TextFile = new StreamWriter(FilePath, true);

            TextFile.WriteLine("# ----------------------------- ");
            TextFile.WriteLine("#        NeoLemmix Level        ");
            TextFile.WriteLine("#   Created with NLEditor " + C.Version);
            TextFile.WriteLine("# ----------------------------- ");
            TextFile.WriteLine(" ");
            TextFile.WriteLine("#        Level info             ");
            TextFile.WriteLine("# ----------------------------- ");
            TextFile.WriteLine(" TITLE " + CurLevel.Title);
            TextFile.WriteLine(" AUTHOR " + CurLevel.Author);
            if (CurLevel.MusicFile != null & CurLevel.MusicFile.Length > 0)
            {
                TextFile.WriteLine(" MUSIC " + Path.GetFileNameWithoutExtension(CurLevel.MusicFile));
            }
            TextFile.WriteLine(" ID x" + CurLevel.LevelID.ToString("X"));
            TextFile.WriteLine(" ");

            TextFile.WriteLine("#       Level dimensions        ");
            TextFile.WriteLine("# ----------------------------- ");
            TextFile.WriteLine(" WIDTH   " + CurLevel.Width.ToString().PadLeft(4));
            TextFile.WriteLine(" HEIGHT  " + CurLevel.Height.ToString().PadLeft(4));
            TextFile.WriteLine(" START_X " + CurLevel.StartPosX.ToString().PadLeft(4));
            TextFile.WriteLine(" START_Y " + CurLevel.StartPosY.ToString().PadLeft(4));
            TextFile.WriteLine(" THEME " + CurLevel.MainStyle.NameInDirectory);
            if (CurLevel.MainStyle.BackgroundNames.Contains(CurLevel.BackgroundKey))
            {
                TextFile.WriteLine(" BACKGROUND " + Path.GetFileName(CurLevel.BackgroundKey));
            }
            TextFile.WriteLine(" ");

            TextFile.WriteLine("#         Level stats           ");
            TextFile.WriteLine("# ----------------------------- ");
            TextFile.WriteLine(" LEMMINGS     " + CurLevel.NumLems.ToString().PadLeft(4));
            TextFile.WriteLine(" REQUIREMENT  " + CurLevel.SaveReq.ToString().PadLeft(4));
            if (!CurLevel.IsNoTimeLimit)
            {
                TextFile.WriteLine(" TIME_LIMIT   " + CurLevel.TimeLimit.ToString().PadLeft(4));
            }
            TextFile.WriteLine(" RELEASE_RATE " + CurLevel.ReleaseRate.ToString().PadLeft(4));
            if (CurLevel.IsReleaseRateFix)
            {
                TextFile.WriteLine(" RELEASE_RATE_FIXED ");
            }
            TextFile.WriteLine(" ");

            TextFile.WriteLine(" $SKILLSET ");
            for (int SkillNum = 0; SkillNum < C.SKI_COUNT; SkillNum++)
            {
                if (IsSkillRequired(CurLevel, SkillNum))
                {
                    if (CurLevel.SkillCount[SkillNum] > 99)
                    {
                        TextFile.WriteLine(fSkillNames[SkillNum] + "INFINITE");
                    }
                    else
                    {
                        TextFile.WriteLine(fSkillNames[SkillNum] + CurLevel.SkillCount[SkillNum].ToString().PadLeft(4));
                    }
                }
            }
            TextFile.WriteLine(" $END ");
            TextFile.WriteLine(" ");

            TextFile.WriteLine("#     Interactive objects       ");
            TextFile.WriteLine("# ----------------------------- ");
            CurLevel.GadgetList.FindAll(obj => obj.ObjType != C.OBJ.LEMMING)
                               .ForEach(obj => WriteObject(TextFile, obj));
            TextFile.WriteLine(" ");

            TextFile.WriteLine("#        Terrain pieces         ");
            TextFile.WriteLine("# ----------------------------- ");
            CurLevel.TerrainList.ForEach(ter => WriteTerrain(TextFile, ter));
            TextFile.WriteLine(" ");

            TextFile.WriteLine("#      Preplaced lemmings       ");
            TextFile.WriteLine("# ----------------------------- ");
            CurLevel.GadgetList.FindAll(obj => obj.ObjType == C.OBJ.LEMMING)
                               .ForEach(lem => WriteObject(TextFile, lem));

            TextFile.WriteLine(" ");

            TextFile.Close();
        }

        /// <summary>
        /// Returns whether the skill is in the skill set or available as a pickup skill. 
        /// </summary>
        /// <param name="CurLevel"></param>
        /// <param name="SkillNum"></param>
        /// <returns></returns>
        static private bool IsSkillRequired(Level CurLevel, int SkillNum)
        {
            return    (CurLevel.SkillCount[SkillNum] > 0)
                   || (CurLevel.GadgetList.Exists(obj => obj.ObjType == C.OBJ.PICKUP && obj.Val_S == SkillNum));
        }

        /// <summary>
        /// Writes all object infos in a text file.
        /// </summary>
        /// <param name="TextFile"></param>
        /// <param name="MyGadget"></param>
        static private void WriteObject(TextWriter TextFile, GadgetPiece MyGadget)
        {
            if (MyGadget == null) return;

            if (MyGadget.ObjType == C.OBJ.LEMMING)
            {
                TextFile.WriteLine(" $LEMMING");
            }
            else
            {
                TextFile.WriteLine(" $OBJECT");
                TextFile.WriteLine("   COLLECTION " + MyGadget.Style);
                TextFile.WriteLine("   PIECE      " + MyGadget.Name);
            }
            TextFile.WriteLine("   X " + MyGadget.PosX.ToString().PadLeft(5));
            TextFile.WriteLine("   Y " + MyGadget.PosY.ToString().PadLeft(5));

            if (MyGadget.SpecWidth > 0)
            {
                TextFile.WriteLine("   WIDTH  " + MyGadget.SpecWidth.ToString().PadLeft(5));
            }
            if (MyGadget.SpecHeight > 0)
            {
                TextFile.WriteLine("   HEIGHT " + MyGadget.SpecWidth.ToString().PadLeft(5));
            }
            if (MyGadget.IsNoOverwrite)
            {
                TextFile.WriteLine("   NO_OVERWRITE");
            }
            if (MyGadget.IsOnlyOnTerrain)
            {
                TextFile.WriteLine("   ONLY_ON_TERRAIN");
            }
            if (MyGadget.IsRotatedInPlayer)
            {
                TextFile.WriteLine("   ROTATE");
            }
            if (MyGadget.IsInvertedInPlayer)
            {
                TextFile.WriteLine("   FLIP_VERTICAL");
            }
            if (MyGadget.IsFlippedInPlayer)
            {
                TextFile.WriteLine("   FLIP_HORIZONTAL");
            }

            if (MyGadget.ObjType.In(C.OBJ.HATCH, C.OBJ.SPLITTER, C.OBJ.LEMMING))
            {
                TextFile.WriteLine("   DIRECTION " + ((MyGadget.IsFlippedInPlayer) ? "left" : "right"));
            }
            else if (MyGadget.ObjType.In(C.OBJ.TELEPORTER))
            {
                if (MyGadget.IsFlippedInPlayer) TextFile.WriteLine("   FLIP_LEMMING ");
            }

            if (MyGadget.ObjType.In(C.OBJ.HATCH, C.OBJ.LEMMING))
            {
                for (int SkillNum = 0; SkillNum < C.SKI_COUNT + 1; SkillNum++)
                {
                    if (MyGadget.HasSkillFlag(SkillNum))
                    {
                        TextFile.WriteLine("   " + fSkillNames[SkillNum].Trim() + " ");
                    }
                }
            }
            else if (MyGadget.ObjType.In(C.OBJ.PICKUP))
            {
                for (int SkillNum = 0; SkillNum < C.SKI_COUNT + 1; SkillNum++)
                {
                    if (MyGadget.HasSkillFlag(SkillNum))
                    {
                        TextFile.WriteLine("  SKILL" + fSkillNames[SkillNum].Trim());
                    }
                }
            }

            if (MyGadget.ObjType.In(C.OBJ.TELEPORTER, C.OBJ.RECEIVER))
            {
                TextFile.WriteLine("   PAIRING " + MyGadget.Val_L.ToString().PadLeft(4));
            }

            TextFile.WriteLine(" $END");
            TextFile.WriteLine(" ");
        }

        /// <summary>
        /// Writes all terrain piece infos in a text file.
        /// </summary>
        /// <param name="TextFile"></param>
        /// <param name="MyTerrain"></param>
        static private void WriteTerrain(TextWriter TextFile, TerrainPiece MyTerrain)
        {
            TextFile.WriteLine(" $TERRAIN");
            TextFile.WriteLine("   COLLECTION " + MyTerrain.Style);
            TextFile.WriteLine("   PIECE      " + MyTerrain.Name);
            TextFile.WriteLine("   X " + MyTerrain.PosX.ToString().PadLeft(5));
            TextFile.WriteLine("   Y " + MyTerrain.PosY.ToString().PadLeft(5));
            if (MyTerrain.IsNoOverwrite)
            {
                TextFile.WriteLine("   NO_OVERWRITE");
            }
            if (MyTerrain.IsErase)
            {
                TextFile.WriteLine("   ERASE");
            }
            if (MyTerrain.IsRotatedInPlayer)
            {
                TextFile.WriteLine("   ROTATE");
            }
            if (MyTerrain.IsInvertedInPlayer)
            {
                TextFile.WriteLine("   FLIP_VERTICAL");
            }
            if (MyTerrain.IsFlippedInPlayer)
            {
                TextFile.WriteLine("   FLIP_HORIZONTAL");
            }
            if (MyTerrain.IsOneWay)
            {
                TextFile.WriteLine("   ONE_WAY");
            }
            TextFile.WriteLine(" $END");
            TextFile.WriteLine(" ");

        }


    }
}
