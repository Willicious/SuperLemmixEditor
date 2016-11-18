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
    class LevelFile
    {
        /// <summary>
        /// Opens file browser and creates level from a .nxlv file.
        /// <para> Returns null if process is aborted or file is corrupt. </para>
        /// </summary>
        /// <param name="StyleList"></param>
        /// <returns></returns>
        static public Level LoadLevel(List<Style> StyleList)
        {

            return null;
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
                        case "ID": /*nothing*/ break;
                        case "WIDTH": NewLevel.Width = Line.Value; break;
                        case "HEIGHT": NewLevel.Height = Line.Value; break;
                        case "START_X": NewLevel.StartPosX = Line.Value; break;
                        case "START_Y": NewLevel.StartPosY = Line.Value; break;
                        case "THEME": NewLevel.MainStyle = StyleList.Find(sty => sty.FileName == Line.Text); break;
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

            if (DoRotate) NewGadget.Rotate();
            if (DoFlip) NewGadget.Flip();
            if (DoInvert) NewGadget.Invert();

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

            if (DoRotate) NewTerrain.Rotate();
            if (DoFlip) NewTerrain.Flip();
            if (DoInvert) NewTerrain.Invert();

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

            if (DoFlip) NewLem.Flip();

            return NewLem;
        }


    }
}
