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
                return null;
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
                        case "ID": NewLevel.LevelID = uint.Parse(Line.Text, System.Globalization.NumberStyles.HexNumber); break;
                        case "WIDTH": NewLevel.Width = Line.Value; break;
                        case "HEIGHT": NewLevel.Height = Line.Value; break;
                        case "START_X": NewLevel.StartPosX = Line.Value; break;
                        case "START_Y": NewLevel.StartPosY = Line.Value; break;
                        case "THEME": NewLevel.MainStyle = StyleList.Find(sty => sty.NameInDirectory == Line.Text); break;
                        case "LEMMINGS": NewLevel.NumLems = Line.Value; break;
                        case "REQUIREMENT": NewLevel.SaveReq = Line.Value; break;
                        case "TIME_LIMIT": NewLevel.TimeLimit = Line.Value;
                                           NewLevel.IsNoTimeLimit = false; break;
                        case "MIN_RR": NewLevel.ReleaseRate = Line.Value; break;
                        case "FIXED_RR": NewLevel.ReleaseRate = Line.Value; 
                                         NewLevel.IsReleaseRateFix = true; break;

                        case "CLIMBER": NewLevel.SkillCount[C.SKI_CLIMBER] = Line.Value; break;
                        case "FLOATER": NewLevel.SkillCount[C.SKI_FLOATER] = Line.Value; break;
                        case "BOMBER": NewLevel.SkillCount[C.SKI_EXPLODER] = Line.Value; break;
                        case "BLOCKER": NewLevel.SkillCount[C.SKI_BLOCKER] = Line.Value; break;
                        case "BUILDER": NewLevel.SkillCount[C.SKI_BUILDER] = Line.Value; break;
                        case "BASHER": NewLevel.SkillCount[C.SKI_BASHER] = Line.Value; break;
                        case "MINER": NewLevel.SkillCount[C.SKI_MINER] = Line.Value; break;
                        case "DIGGER": NewLevel.SkillCount[C.SKI_DIGGER] = Line.Value; break;
                        case "WALKER": NewLevel.SkillCount[C.SKI_WALKER] = Line.Value; break;
                        case "SWIMMER": NewLevel.SkillCount[C.SKI_SWIMMER] = Line.Value; break;
                        case "GLIDER": NewLevel.SkillCount[C.SKI_GLIDER] = Line.Value; break;
                        case "DISARMER": NewLevel.SkillCount[C.SKI_DISARMER] = Line.Value; break;
                        case "STONER": NewLevel.SkillCount[C.SKI_STONER] = Line.Value; break;
                        case "PLATFORMER": NewLevel.SkillCount[C.SKI_PLATFORMER] = Line.Value; break;
                        case "STACKER": NewLevel.SkillCount[C.SKI_STACKER] = Line.Value; break;
                        case "CLONER": NewLevel.SkillCount[C.SKI_CLONER] = Line.Value; break;

                        case "OBJECT": NewLevel.GadgetList.Add(ReadGadgetFromLines(FileLineList)); break;
                        case "TERRAIN": NewLevel.TerrainList.Add(ReadTerrainFromLines(FileLineList)); break;
                        case "LEMMING": NewLevel.GadgetList.Add(ReadLemmingFromLines(FileLineList)); break;
                    }
                }
            }
            catch (Exception Ex)
            {
                Utility.LogException(Ex);
                MessageBox.Show(Ex.Message);
            }


            return NewLevel;
        }


        /// <summary>
        /// Creates a gadget from a block of file lines.
        /// </summary>
        /// <param name="FileLineList"></param>
        /// <returns></returns>
        static private GadgetPiece ReadGadgetFromLines(List<FileLine> FileLineList)
        { 
            // First read in all infos
            string StyleName = "";
            string GadgetName = "";
            int PosX = 0;
            int PosY = 0;
            bool IsNoOverwrite = false;
            bool IsOnlyOnTerrain = false;

            bool DoRotate = false;
            bool DoInvert = false;
            bool DoFlip = false;
            int Val_L = 0;
            int Val_S = 0;

            foreach (FileLine Line in FileLineList)
            {
                switch (Line.Key)
                {
                    case "SET": StyleName = Line.Text; break;
                    case "PIECE": GadgetName = Line.Text; break;
                    case "X": PosX = Line.Value; break;
                    case "Y": PosY = Line.Value; break;
                    case "WIDTH": /*nothing yet*/ break;
                    case "HEIGHT": /*nothing yet*/ break;
                    case "NO_OVERWRITE": IsNoOverwrite = true; break;
                    case "ONLY_ON_TERRAIN": IsOnlyOnTerrain = true; break;
                    case "ROTATE": DoRotate = true; break;
                    case "FLIP_HORIZONTAL": DoFlip = true; break;
                    case "FLIP_VERTICAL": DoInvert = true; break;
                    case "FACE_LEFT": DoFlip = true; break;
                    case "L": Val_L = Line.Value; break;
                    case "S": Val_S = Line.Value; break;
                }
            }

            // ... then create the correct Gadget piece
            string Key = ImageLibrary.CreatePieceKey(StyleName, GadgetName, true);
            Point Pos = new Point(PosX, PosY);
            GadgetPiece NewGadget = new GadgetPiece(Key, Pos, 0, false, IsNoOverwrite, IsOnlyOnTerrain, Val_L, Val_S);

            // For compatibility with player: NoOverwrite + OnlyOnTerrain gadgets work like OnlyOnTerrain 
            if (NewGadget.IsNoOverwrite && NewGadget.IsOnlyOnTerrain) NewGadget.IsNoOverwrite = false;

            if (DoRotate) NewGadget.RotateInRect(NewGadget.ImageRectangle);
            if (DoFlip) NewGadget.FlipInRect(NewGadget.ImageRectangle);
            if (DoInvert) NewGadget.InvertInRect(NewGadget.ImageRectangle);
            //Reposition gadget to be sure...
            NewGadget.Pos = Pos;

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
                    case "SET": StyleName = Line.Text; break;
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
            NewTerrain.Pos = Pos;

            NewTerrain.IsSelected = false;

            return NewTerrain;
        }

        /// <summary>
        /// Creates a preplaced lemming from a block of file lines.
        /// </summary>
        /// <param name="FileLineList"></param>
        /// <returns></returns>
        static private GadgetPiece ReadLemmingFromLines(List<FileLine> FileLineList)
        {
            // First read in all infos
            string StyleName = "";
            string TerrainName = "";
            int PosX = 0;
            int PosY = 0;

            bool DoFlip = false;
            int Val_L = 0;
            int Val_S = 0;

            foreach (FileLine Line in FileLineList)
            {
                switch (Line.Key)
                {
                    case "SET": StyleName = Line.Text; break;
                    case "PIECE": TerrainName = Line.Text; break;
                    case "X": PosX = Line.Value; break;
                    case "Y": PosY = Line.Value; break;
                    case "FACE_LEFT": DoFlip = true; break;
                    case "CLIMBER": Val_L ^= 1 << 0; break;
                    case "SWIMMER": Val_L ^= 1 << 1; break;
                    case "FLOATER": Val_L ^= 1 << 2; break;
                    case "GLIDER": Val_L ^= 1 << 3; break;
                    case "DISARMER": Val_L ^= 1 << 4; break;
                    case "BLOCKER": Val_L ^= 1 << 5; break;
                    case "ZOMBIE": Val_L ^= 1 << 6; break;
                }
            }

            // ... then create the correct Gadget piece
            string Key = ImageLibrary.CreatePieceKey("default", "lemming", true);
            Point Pos = new Point(PosX, PosY);
            GadgetPiece NewLem = new GadgetPiece(Key, Pos, 0, false, false, false, Val_L, Val_S);

            if (DoFlip) NewLem.FlipInRect(NewLem.ImageRectangle);

            NewLem.IsSelected = false;

            return NewLem;
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

            TextFile.WriteLine("# NeoLemmix Level");
            TextFile.WriteLine("# Created with NLEditor " + C.Version);
            TextFile.WriteLine(" ");
            TextFile.WriteLine("# Level info");
            TextFile.WriteLine(" TITLE " + CurLevel.Title);
            TextFile.WriteLine(" AUTHOR " + CurLevel.Author);
            if (CurLevel.MusicFile != null & CurLevel.MusicFile.Length > 0)
            {
                TextFile.WriteLine(" MUSIC " + CurLevel.MusicFile);
            }
            TextFile.WriteLine(" ID " + CurLevel.LevelID.ToString("X"));
            TextFile.WriteLine(" ");

            TextFile.WriteLine("# Level dimensions");
            TextFile.WriteLine(" WIDTH   " + CurLevel.Width.ToString().PadLeft(4));
            TextFile.WriteLine(" HEIGHT  " + CurLevel.Height.ToString().PadLeft(4));
            TextFile.WriteLine(" START_X " + CurLevel.StartPosX.ToString().PadLeft(4));
            TextFile.WriteLine(" START_Y " + CurLevel.StartPosY.ToString().PadLeft(4));
            TextFile.WriteLine(" THEME " + CurLevel.MainStyle.NameInDirectory);
            TextFile.WriteLine(" ");

            TextFile.WriteLine("# Level stats");
            TextFile.WriteLine(" LEMMINGS    " + CurLevel.NumLems.ToString().PadLeft(4));
            TextFile.WriteLine(" REQUIREMENT " + CurLevel.SaveReq.ToString().PadLeft(4));
            if (!CurLevel.IsNoTimeLimit)
            {
                TextFile.WriteLine(" TIME_LIMIT  " + CurLevel.TimeLimit.ToString().PadLeft(4));
            }
            if (CurLevel.IsReleaseRateFix)
            {
                TextFile.WriteLine(" FIXED_RR    " + CurLevel.ReleaseRate.ToString().PadLeft(4));
            }
            else
            {
                TextFile.WriteLine(" MIN_RR      " + CurLevel.ReleaseRate.ToString().PadLeft(4));
            }
            TextFile.WriteLine(" ");

            TextFile.WriteLine("# Level skillset");
            string[] SkillNames = { 
                "    CLIMBER ", "    FLOATER ", "     BOMBER ", "    BLOCKER ",
                "    BUILDER ", "     BASHER ", "      MINER ", "     DIGGER ",               
                "     WALKER ", "    SWIMMER ", "     GLIDER ", "   DISARMER ",                  
                "     STONER ", " PLATFORMER ", "    STACKER ", "     CLONER "};
            for (int SkillNum = 0; SkillNum < C.SKI_COUNT; SkillNum++)
            {
                if (IsSkillRequired(CurLevel, SkillNum))
                {
                    TextFile.WriteLine(SkillNames[SkillNum] + CurLevel.SkillCount[SkillNum].ToString().PadLeft(4));
                }
            }
            TextFile.WriteLine(" ");
            TextFile.WriteLine(" ");

            TextFile.WriteLine("# Interactive objects");
            CurLevel.GadgetList.FindAll(obj => obj.ObjType != C.OBJ.LEMMING)
                               .ForEach(obj => WriteObject(TextFile, obj));

            TextFile.WriteLine("# Terrains");
            CurLevel.TerrainList.ForEach(ter => WriteTerrain(TextFile, ter));

            TextFile.WriteLine("# Preplaced lemmings");
            CurLevel.GadgetList.FindAll(obj => obj.ObjType == C.OBJ.LEMMING)
                               .ForEach(lem => WriteLemming(TextFile, lem));

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
            TextFile.WriteLine(" OBJECT");
            TextFile.WriteLine("  SET    " + MyGadget.Style);
            TextFile.WriteLine("  PIECE  " + MyGadget.Name);
            TextFile.WriteLine("  X " + MyGadget.PosX.ToString().PadLeft(5));
            TextFile.WriteLine("  Y " + MyGadget.PosY.ToString().PadLeft(5));
            if (MyGadget.Val_L != 0)
            {
                TextFile.WriteLine("  L " + MyGadget.Val_L.ToString().PadLeft(5));
            }
            if (MyGadget.Val_S != 0)
            {
                TextFile.WriteLine("  S " + MyGadget.Val_S.ToString().PadLeft(5));
            }
            if (MyGadget.SpecWidth > 0)
            {
                TextFile.WriteLine("  WIDTH  " + MyGadget.SpecWidth.ToString().PadLeft(5));
            }
            if (MyGadget.SpecHeight > 0)
            {
                TextFile.WriteLine("  HEIGHT " + MyGadget.SpecWidth.ToString().PadLeft(5));
            }
            if (MyGadget.IsNoOverwrite)
            {
                TextFile.WriteLine("  NO_OVERWRITE");
            }
            if (MyGadget.IsOnlyOnTerrain)
            {
                TextFile.WriteLine("  ONLY_ON_TERRAIN");
            }
            if (MyGadget.IsRotatedInPlayer)
            {
                TextFile.WriteLine("  ROTATE");
            }
            if (MyGadget.IsInvertedInPlayer)
            {
                TextFile.WriteLine("  FLIP_VERTICAL");
            }
            if (MyGadget.IsFlippedInPlayer)
            {
                TextFile.WriteLine("  FLIP_HORIZONTAL");
                TextFile.WriteLine("  FACE_LEFT");
            }
            TextFile.WriteLine(" ");
        }

        /// <summary>
        /// Writes all terrain piece infos in a text file.
        /// </summary>
        /// <param name="TextFile"></param>
        /// <param name="MyTerrain"></param>
        static private void WriteTerrain(TextWriter TextFile, TerrainPiece MyTerrain)
        {
            TextFile.WriteLine(" TERRAIN");
            TextFile.WriteLine("  SET    " + MyTerrain.Style);
            TextFile.WriteLine("  PIECE  " + MyTerrain.Name);
            TextFile.WriteLine("  X " + MyTerrain.PosX.ToString().PadLeft(5));
            TextFile.WriteLine("  Y " + MyTerrain.PosY.ToString().PadLeft(5));
            if (MyTerrain.IsNoOverwrite)
            {
                TextFile.WriteLine("  NO_OVERWRITE");
            }
            if (MyTerrain.IsErase)
            {
                TextFile.WriteLine("  ERASE");
            }
            if (MyTerrain.IsRotatedInPlayer)
            {
                TextFile.WriteLine("  ROTATE");
            }
            if (MyTerrain.IsInvertedInPlayer)
            {
                TextFile.WriteLine("  FLIP_VERTICAL");
            }
            if (MyTerrain.IsFlippedInPlayer)
            {
                TextFile.WriteLine("  FLIP_HORIZONTAL");
            }
            if (MyTerrain.IsOneWay)
            {
                TextFile.WriteLine("  ONE_WAY");
            }
            TextFile.WriteLine(" ");

        }

        /// <summary>
        /// Writes all infos about a preplaced lemming in a text file.
        /// </summary>
        /// <param name="TextFile"></param>
        /// <param name="MyLem"></param>
        static private void WriteLemming(TextWriter TextFile, GadgetPiece MyLem)
        {
            System.Diagnostics.Debug.Assert(MyLem.ObjType == C.OBJ.LEMMING, "WriteLemming called for non-lemming object.");

            TextFile.WriteLine(" LEMMING");
            TextFile.WriteLine("  X " + MyLem.PosX.ToString().PadLeft(5));
            TextFile.WriteLine("  Y " + MyLem.PosY.ToString().PadLeft(5));
            if (MyLem.IsFlippedInPlayer)
            {
                TextFile.WriteLine("  FACE_LEFT");
            }
            if ((MyLem.Val_S & (1 << C.SKI_CLIMBER)) != 0)
            {
                TextFile.WriteLine("  CLIMBER");
            }
            if ((MyLem.Val_S & (1 << C.SKI_SWIMMER)) != 0)
            {
                TextFile.WriteLine("  SWIMMER");
            }
            if ((MyLem.Val_S & (1 << C.SKI_FLOATER)) != 0)
            {
                TextFile.WriteLine("  FLOATER");
            }
            if ((MyLem.Val_S & (1 << C.SKI_GLIDER)) != 0)
            {
                TextFile.WriteLine("  GLIDER");
            }
            if ((MyLem.Val_S & (1 << C.SKI_DISARMER)) != 0)
            {
                TextFile.WriteLine("  DISARMER");
            }
            if ((MyLem.Val_S & (1 << C.SKI_BLOCKER)) != 0)
            {
                TextFile.WriteLine("  BLOCKER");
            }
            if ((MyLem.Val_S & (1 << C.SKI_ZOMBIE)) != 0)
            {
                TextFile.WriteLine("  ZOMBIE");
            }

            TextFile.WriteLine(" ");
        }


    }
}
