using System;
using System.Collections.Generic;
using System.Linq;
using System.Drawing;

namespace NLEditor
{
    /// <summary>
    /// Stores and modifies the data of a lemming level.
    /// </summary>
    public class Level
    {
        /*---------------------------------------------------------
         *          This class stores all global level infos
         * -------------------------------------------------------- */

        /*---------------------------------------------------------
         *  public methods:
         *    Level(Style MainStyle = null)
         *    
         *    Clone()
         *    Equals()
         *    AddPiece(Style NewStyle, bool IsObject, int NewPieceIndex, Point CenterPos)
         *    
         *    SelectOnePiece(Point Pos, bool IsAdded, bool IsHighest)
         *    SelectAreaPiece(Rectangle Rect, bool IsAdded)
         *    UnselectAll()
         *    SelectionList()
         *    SelectionRectangle()
         *    HasSelectionAtPos(Point Pos)
         *    MovePieces(C.DIR Direcion, int Step = 1)
         *    RotatePieces()
         *    InvertPieces()
         *    FlipPieces()
         *    GroupSelectedPieces()
         *    SetNoOverwrite(bool DoAdd)
         *    SetErase(bool DoAdd)
         *    SetOnlyOnTerrain(bool DoAdd)
         *    SetOneWay(bool DoAdd)
         *    SetSkillForObjects(int Skill, bool DoAdd)
         *    MoveSelectedIndex(bool ToTop, bool OnlyOneStep)
         *    PairTeleporters()
         * -------------------------------------------------------- */

        /// <summary>
        /// Creates a new level with the default values.
        /// </summary>
        /// <param name="mainStyle"></param>
        public Level(Style mainStyle = null)
        {
            this.Title = "";
            this.Author = "";
            this.MainStyle = mainStyle;
            this.MusicFile = "";
            this.backgroundKey = "";

            Random rnd = new Random();
            this.LevelID = (uint)rnd.Next();

            this.Width = 320;
            this.Height = 160;
            this.StartPosX = 0;
            this.StartPosY = 0;

            this.TerrainList = new List<TerrainPiece>();
            this.GadgetList = new List<GadgetPiece>();

            this.NumLems = 40;
            this.SaveReq = 20;
            this.ReleaseRate = 50;
            this.IsReleaseRateFix = false;
            this.TimeLimit = 0;
            this.IsNoTimeLimit = true;

            this.SkillSet = new Dictionary<C.Skill, int>();
            foreach (C.Skill skill in C.SkillArray)
            {
                SkillSet.Add(skill, 0);
            }
        }

        public string Title { get; set; }
        public string Author { get; set; }
        public Style MainStyle { get; set; }
        public string MusicFile { get; set; }

        public uint LevelID { get; set; }
        public string FilePathToSave { get; set; }

        public int Width { get; set; }
        public int Height { get; set; }

        public Point StartPos => new Point(StartPosX, StartPosY);
        public int StartPosX { get; set; }
        public int StartPosY { get; set; }

        public List<TerrainPiece> TerrainList { get; set; }
        public List<GadgetPiece> GadgetList { get; set; }

        string backgroundKey;
        public string BackgroundKey 
        {
            get { 
                if (string.IsNullOrEmpty(backgroundKey)) return "--none--";
                else return MainStyle.NameInDirectory + C.DirSep + "backgrounds" + C.DirSep + backgroundKey; 
                } 
            set { backgroundKey = value; } 
        }

        public int NumLems { get; set; }
        public int SaveReq { get; set; }
        public int ReleaseRate { get; set; }
        public bool IsReleaseRateFix { get; set; }
        public int TimeLimit { get; set; }
        public bool IsNoTimeLimit { get; set; }

        public Dictionary<C.Skill, int> SkillSet { get; set; }

        /// <summary>
        /// Creates a deep copy of the level.
        /// </summary>
        /// <returns></returns>
        public Level Clone()
        {
            Level newLevel = new Level(this.MainStyle);
            newLevel.Title = string.Copy(this.Title);
            newLevel.Author = string.Copy(this.Author);
            newLevel.MusicFile = string.Copy(this.MusicFile);
            newLevel.LevelID = this.LevelID;
            newLevel.FilePathToSave = this.FilePathToSave; // shallow copy is fine here.
            newLevel.backgroundKey = string.Copy(this.backgroundKey);

            newLevel.Width = this.Width;
            newLevel.Height = this.Height;
            newLevel.StartPosX = this.StartPosX;
            newLevel.StartPosY = this.StartPosY;

            newLevel.TerrainList = new List<TerrainPiece>(this.TerrainList.Select(ter => (TerrainPiece)ter.Clone()));
            newLevel.GadgetList = new List<GadgetPiece>(this.GadgetList.Select(gad => (GadgetPiece)gad.Clone()));

            newLevel.TerrainList.ForEach(ter => ter.IsSelected = false);
            newLevel.GadgetList.ForEach(gad => gad.IsSelected = false);

            newLevel.NumLems = this.NumLems;
            newLevel.SaveReq = this.SaveReq;
            newLevel.ReleaseRate = this.ReleaseRate;
            newLevel.IsReleaseRateFix = this.IsReleaseRateFix;
            newLevel.TimeLimit = this.TimeLimit;
            newLevel.IsNoTimeLimit = this.IsNoTimeLimit;

            newLevel.SkillSet = new Dictionary<C.Skill, int>();
            foreach (C.Skill skill in C.SkillArray)
            {
                newLevel.SkillSet.Add(skill, this.SkillSet[skill]);
            }

            return newLevel;
        }

        /// <summary>
        /// Compares two Levels for equality.
        /// </summary>
        /// <param name="otherLevel"></param>
        /// <returns></returns>
        public bool Equals(Level otherLevel)
        {
            if ( otherLevel == null
                || !this.Title.Equals(otherLevel.Title)
                || !this.Author.Equals(otherLevel.Author)
                || !this.MainStyle.NameInDirectory.Equals(otherLevel.MainStyle.NameInDirectory)
                || !this.MusicFile.Equals(otherLevel.MusicFile)
                || this.LevelID != otherLevel.LevelID
                || !this.BackgroundKey.Equals(otherLevel.BackgroundKey)
                || this.Width != otherLevel.Width
                || this.Height != otherLevel.Height
                || this.StartPosX != otherLevel.StartPosX
                || this.StartPosY != otherLevel.StartPosY 
                || this.TerrainList.Count != otherLevel.TerrainList.Count
                || this.GadgetList.Count != otherLevel.GadgetList.Count
                || this.NumLems != otherLevel.NumLems
                || this.SaveReq != otherLevel.SaveReq
                || this.ReleaseRate != otherLevel.ReleaseRate
                || this.IsReleaseRateFix != otherLevel.IsReleaseRateFix
                || this.IsNoTimeLimit != otherLevel.IsNoTimeLimit
                || (this.TimeLimit != otherLevel.TimeLimit && !this.IsNoTimeLimit))
            {
                return false;
            }

            for (int i = 0; i < this.TerrainList.Count; i++)
            {
                if (!this.TerrainList[i].Equals(otherLevel.TerrainList[i])) return false;
            }

            for (int i = 0; i < this.GadgetList.Count; i++)
            {
                if (!this.GadgetList[i].Equals(otherLevel.GadgetList[i])) return false;
            }

            return true;
        }


        /// <summary>
        /// Creates a new piece and adds it to the level.
        /// </summary>
        /// <param name="newStyle"></param>
        /// <param name="isObject"></param>
        /// <param name="newPieceIndex"></param>
        /// <param name="centerPos"></param>
        public void AddPiece(Style newStyle, bool isObject, int newPieceIndex, Point centerPos)
        {
            string pieceKey = isObject ? newStyle.ObjectKeys[newPieceIndex] : newStyle.TerrainKeys[newPieceIndex];

            Point piecePos = new Point(centerPos.X - ImageLibrary.GetWidth(pieceKey) / 2,
                                       centerPos.Y - ImageLibrary.GetHeight(pieceKey) / 2);

            if (isObject)
            {
                GadgetList.Add(new GadgetPiece(pieceKey, piecePos));
            }
            else
            {
                TerrainList.Add(new TerrainPiece(pieceKey, piecePos));
            }
        }

        /// <summary>
        /// Sets the "IsSelected" flag for the first piece that can receive it.
        /// </summary>
        /// <param name="pos"></param>
        /// <param name="isAdded"></param>
        /// <param name="doPriorityInvert"></param>
        public void SelectOnePiece(Point pos, bool isAdded, bool doPriorityInvert)
        {
            LevelPiece selectedPiece = GetOnePiece(pos, isAdded, doPriorityInvert);

            if (selectedPiece != null)
            {
                selectedPiece.IsSelected = isAdded;
            }
        }

        /// <summary>
        /// Determines the piece to select.
        /// </summary>
        /// <param name="pos"></param>
        /// <param name="isUnselected"></param>
        /// <param name="doPriorityInvert"></param>
        /// <returns></returns>
        private LevelPiece GetOnePiece(Point pos, bool isUnselected, bool doPriorityInvert)
        {
            LevelPiece selectedPiece;

            if (doPriorityInvert)
            {
                selectedPiece = TerrainList.Find(ter => ter.ImageRectangle.Contains(pos)
                                                    && (isUnselected ^ ter.IsSelected));
                if (selectedPiece == null)
                {
                    selectedPiece = GadgetList.Find(gad => gad.ImageRectangle.Contains(pos)
                                                       && (isUnselected ^ gad.IsSelected));
                }
            }
            else
            {
                selectedPiece = GadgetList.FindLast(gad => gad.ImageRectangle.Contains(pos)
                                                       && (isUnselected ^ gad.IsSelected));
                if (selectedPiece == null)
                {
                    selectedPiece = TerrainList.FindLast(ter => ter.ImageRectangle.Contains(pos)
                                                            && (isUnselected ^ ter.IsSelected));
                }
            }

            return selectedPiece;
        }

        /// <summary>
        /// Select all pieces that intersect with a given area.
        /// </summary>
        /// <param name="rectangle"></param>
        /// <param name="isAdded"></param>
        public void SelectAreaPiece(Rectangle rectangle, bool isAdded)
        {
            TerrainList.FindAll(ter => ter.ImageRectangle.IntersectsWith(rectangle))
                       .ForEach(ter => ter.IsSelected = isAdded);
            GadgetList.FindAll(gad => gad.ImageRectangle.IntersectsWith(rectangle))
                      .ForEach(gad => gad.IsSelected = isAdded);
        }

        /// <summary>
        /// Removes the "IsSelected" flag from all pieces.
        /// </summary>
        public void UnselectAll()
        {
            TerrainList.ForEach(ter => ter.IsSelected = false);
            GadgetList.ForEach(gad => gad.IsSelected = false);
        }

        /// <summary>
        /// Gets a list of a currently selected pieces in the level.
        /// </summary>
        /// <returns></returns>
        public List<LevelPiece> SelectionList()
        {
            var selectedPieces = new List<LevelPiece>();
            selectedPieces.AddRange(TerrainList.FindAll(ter => ter.IsSelected));
            selectedPieces.AddRange(GadgetList.FindAll(obj => obj.IsSelected));
            return selectedPieces;
        }

        /// <summary>
        /// Gets the smallest rectangle around all selected pieces in the level.
        /// </summary>
        /// <returns></returns>
        public Rectangle SelectionRectangle()
        {
            var selectedPieces = SelectionList();

            int left = selectedPieces.Min(item => item.PosX);
            int right = selectedPieces.Max(item => item.PosX + item.Width);
            int top = selectedPieces.Min(item => item.PosY);
            int bottom = selectedPieces.Max(item => item.PosY + item.Height);

            return new Rectangle(left, top, right - left, bottom - top);
        }

        /// <summary>
        /// Returns whether there is a selected terrain piece at a point.
        /// </summary>
        /// <param name="pos"></param>
        /// <returns></returns>
        public bool HasSelectionAtPos(Point pos)
        {
            return SelectionList().Exists(item => item.ImageRectangle.Contains(pos));
        }

        /// <summary>
        /// Moves all selected pieces a given number of pixels into a given direction. 
        /// </summary>
        /// <param name="direction"></param>
        /// <param name="step"></param>
        public void MovePieces(C.DIR direction, int step = 1)
        {
            SelectionList().ForEach(item => item.Move(direction, step));
        }

        /// <summary>
        /// Moves all selected pieces to the target position. 
        /// </summary>
        /// <param name="targetPos">Location of the rectangle spanning all selected pieces.</param>
        public void MovePieces(Point targetPos)
        {
            Point referencePos = SelectionRectangle().Location;
            foreach (LevelPiece piece in SelectionList())
            {
                int pieceTargetX = targetPos.X + piece.PosX - referencePos.X;
                int pieceTargetY = targetPos.Y + piece.PosY - referencePos.Y;
                piece.Move(new Point(pieceTargetX, pieceTargetY));
            }
        }


        /// <summary>
        /// Rotates all selected pieces in their rectangular hull.
        /// </summary>
        public void RotatePieces()
        { 
            Rectangle borderRect = SelectionRectangle();
            SelectionList().ForEach(item => item.RotateInRect(borderRect));
        }

        /// <summary>
        /// Inverts all selected pieces in their rectangular hull.
        /// </summary>
        public void InvertPieces()
        {
            Rectangle borderRect = SelectionRectangle();
            SelectionList().ForEach(item => item.InvertInRect(borderRect));
        }

        /// <summary>
        /// Flips all selected pieces in their rectangular hull.
        /// </summary>
        public void FlipPieces()
        {
            Rectangle borderRect = SelectionRectangle();
            SelectionList().ForEach(item => item.FlipInRect(borderRect));
        }

        /// <summary>
        /// Groups all selected terrain (non-steel) pieces into one piece. The new group is not selected.
        /// </summary>
        /// <returns></returns>
        public GroupPiece ReplaceSelectedByGroup()
        {
            var selectedTerrains = SelectionList().FindAll(item => item.ObjType == C.OBJ.TERRAIN);
            GroupPiece newGroup = new GroupPiece(selectedTerrains.ConvertAll(item => (TerrainPiece)item));
            TerrainList.RemoveAll(item => item.ObjType == C.OBJ.TERRAIN);
            TerrainList.Add(newGroup);

            return newGroup; 
        }

        /// <summary>
        /// Sets the NoOverwrite flag for all objects and terrain pieces.
        /// </summary>
        /// <param name="doAdd"></param>
        public void SetNoOverwrite(bool doAdd)
        {
            TerrainList.FindAll(ter => ter.IsSelected)
                       .ForEach(ter => { ter.IsNoOverwrite = doAdd; if (doAdd) ter.IsErase = false; });
            GadgetList.FindAll(gad => gad.IsSelected)
                      .ForEach(gad => { gad.IsNoOverwrite = doAdd; if (doAdd) gad.IsOnlyOnTerrain = false; });
        }

        /// <summary>
        /// Sets the Erase flag for all terrain pieces.
        /// </summary>
        /// <param name="doAdd"></param>
        public void SetErase(bool doAdd)
        {
            TerrainList.FindAll(ter => ter.IsSelected)
                       .ForEach(ter => { ter.IsErase = doAdd; if (doAdd) ter.IsNoOverwrite = false; });
        }

        /// <summary>
        /// Sets the OnlyOnTerrain flag for all objects.
        /// </summary>
        /// <param name="doAdd"></param>
        public void SetOnlyOnTerrain(bool doAdd)
        {
            GadgetList.FindAll(gad => gad.IsSelected)
                      .ForEach(gad => { gad.IsOnlyOnTerrain = doAdd; if (doAdd) gad.IsNoOverwrite = false; });
        }

        /// <summary>
        /// Sets the OneWay flag for all terrain pieces.
        /// </summary>
        /// <param name="doAdd"></param>
        public void SetOneWay(bool doAdd)
        {
            TerrainList.FindAll(ter => ter.IsSelected && !ter.IsSteel)
                       .ForEach(ter => ter.IsOneWay = doAdd);
        }

        /// <summary>
        /// Adds or removes a skill flag from all selected objects 
        /// </summary>
        /// <param name="skill"></param>
        public void SetSkillForObjects(C.Skill skill, bool doAdd)
        {
            GadgetList.FindAll(gad => gad.IsSelected)
                      .ForEach(gad => gad.SetSkillFlag(skill, doAdd));
        }

        /// <summary>
        /// Changes the index of all selected pieces.
        /// </summary>
        /// <param name="toTop"></param>
        /// <param name="onlyOneStep"></param>
        public void MoveSelectedPieces(bool toTop, bool onlyOneStep)
        {
            if (onlyOneStep && toTop)
            {
                MoveSelectedOneToTop();
            }
            else if (onlyOneStep && !toTop)
            {
                MoveSelectedOneToBottom();
            }
            else
            {
                MoveSelectedMaximally(toTop);
            }
        }

        /// <summary>
        /// Moves all selected pieces to the beginning or the end of their respective lists.
        /// </summary>
        /// <param name="toTop"></param>
        private void MoveSelectedMaximally(bool toTop)
        {
            if (toTop)
            {
                TerrainList = MoveSelectedAllToTop(TerrainList, TerrainList.Count - 1);
                GadgetList = MoveSelectedAllToTop(GadgetList, GadgetList.Count - 1);
            }
            else
            {
                TerrainList = MoveSelectedAllToBottom(TerrainList, 0);
                GadgetList = MoveSelectedAllToBottom(GadgetList, 0);
            }
        }

        /// <summary>
        /// Moves all selected pieces one index upwards.
        /// </summary>
        private void MoveSelectedOneToTop()
        {
            int endTerrIndex = GetMoveTopEndIndex(TerrainList);
            TerrainList = MoveSelectedAllToTop(TerrainList, endTerrIndex);

            int endGadgetIndex = GetMoveTopEndIndex(GadgetList);
            GadgetList = MoveSelectedAllToTop(GadgetList, endGadgetIndex);
        }

        /// <summary>
        /// Moves all selected pieces one index downwards.
        /// </summary>
        private void MoveSelectedOneToBottom()
        {
            int startTerrIndex = GetMoveBottomStartIndex(TerrainList);
            TerrainList = MoveSelectedAllToBottom(TerrainList, startTerrIndex);

            int startGadgetIndex = GetMoveBottomStartIndex(GadgetList);
            GadgetList = MoveSelectedAllToBottom(GadgetList, startGadgetIndex);
        }

        /// <summary>
        /// Finds the last index of a non-selected piece that when moved to top past all selected pieces changes output.
        /// </summary>
        /// <param name="pieceList"></param>
        /// <returns></returns>
        private int GetMoveBottomStartIndex<T>(List<T> pieceList) where T : LevelPiece
        {
            for (int i = pieceList.Count - 1; i >= 0; i--)
            {
                if (!pieceList[i].IsSelected)
                {
                    Rectangle pieceImageRect = pieceList[i].ImageRectangle;
                    if (pieceList.GetRange(i + 1)
                                 .FindAll(item => item.IsSelected)
                                 .Exists(item => item.ImageRectangle.IntersectsWith(pieceImageRect)))
                    {
                        return i;
                    }
                }
            }
            return 0;
        }

        /// <summary>
        /// Finds the last index of a non-selected piece that when moved to bottom past all selected pieces changes output.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="pieceList"></param>
        /// <returns></returns>
        private int GetMoveTopEndIndex<T>(List<T> pieceList) where T : LevelPiece
        {
            for (int i = 0; i < pieceList.Count; i++)
            {
                if (!pieceList[i].IsSelected)
                {
                    Rectangle pieceImageRect = pieceList[i].ImageRectangle;
                    if (pieceList.GetRange(0, i)
                                 .FindAll(item => item.IsSelected)
                                 .Exists(item => item.ImageRectangle.IntersectsWith(pieceImageRect)))
                    {
                        return i;
                    }
                }
            }
            return pieceList.Count - 1;
        }

        /// <summary>
        /// Moves all selected pieces to bottom in the range starting from startIndex.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="pieceList"></param>
        /// <param name="startIndex"></param>
        /// <returns></returns>
        private List<T> MoveSelectedAllToBottom<T>(List<T> pieceList, int startIndex) where T : LevelPiece
        {
            return pieceList.GetRange(0, startIndex)
                            .Concat(pieceList.GetRange(startIndex).FindAll(item => item.IsSelected))
                            .Concat(pieceList.GetRange(startIndex).FindAll(item => !item.IsSelected))
                            .ToList();
        }

        /// <summary>
        /// Moves all selected pieces to top in the range ending with endIndex.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="pieceList"></param>
        /// <param name="endIndex"></param>
        /// <returns></returns>
        private List<T> MoveSelectedAllToTop<T>(List<T> pieceList, int endIndex) where T : LevelPiece
        {
            return pieceList.GetRange(0, endIndex + 1).FindAll(item => !item.IsSelected)
                            .Concat(pieceList.GetRange(0, endIndex + 1).FindAll(item => item.IsSelected))
                            .Concat(pieceList.GetRange(endIndex + 1))
                            .ToList();
        }

        /// <summary>
        /// Pairs a selected teleporter and receiver.
        /// </summary>
        public void PairTeleporters()
        { 
            GadgetPiece teleporter = (GadgetPiece)SelectionList().Find(gad => gad.ObjType == C.OBJ.TELEPORTER);
            GadgetPiece receiver = (GadgetPiece)SelectionList().Find(gad => gad.ObjType == C.OBJ.RECEIVER);

            System.Diagnostics.Debug.Assert(teleporter != null, "Tried to pair teleporters without a selected teleporter!");
            System.Diagnostics.Debug.Assert(receiver != null, "Tried to pair teleporters without a selected teleporter!");

            if (teleporter.Val_L != 0) RemovePairingValue(teleporter.Val_L);
            if (receiver.Val_L != 0) RemovePairingValue(receiver.Val_L);

            int newPairingValue = FindNewPairingValue();

            teleporter.SetTeleporterValue(newPairingValue);
            receiver.SetTeleporterValue(newPairingValue);
        }

        /// <summary>
        /// Removes the key-value RemoveValue from all teleporter and receiver objects.
        /// </summary>
        /// <param name="removeValue"></param>
        private void RemovePairingValue(int removeValue)
        {
            GadgetList.FindAll(gad => gad.ObjType.In(C.OBJ.TELEPORTER, C.OBJ.RECEIVER) && gad.Val_L == removeValue)
                      .ForEach(gad => gad.SetTeleporterValue(0)); 
        }

        /// <summary>
        /// Find the lowest unused teleporter/receiver key value.
        /// </summary>
        /// <returns></returns>
        private int FindNewPairingValue()
        {
            var existingPairingValues = GadgetList.FindAll(gad => gad.ObjType.In(C.OBJ.TELEPORTER, C.OBJ.RECEIVER))
                                                  .ConvertAll(gad => gad.Val_L);

            return Enumerable.Range(1, int.MaxValue - 1)
                             .Except(existingPairingValues)
                             .FirstOrDefault();
        }

    }
}
