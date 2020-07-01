using System;
using System.Collections.Generic;
using System.Linq;
using System.Drawing;

namespace NLEditor
{
    /// <summary>
    /// Stores and modifies the data of a lemming level.
    /// </summary>
    class Level : IEquatable<Level>
    {
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
            this.Background = null;

            // Create a random 64bit hex number
            this.LevelID = (ulong)Utility.Random().Next() +
                           ((ulong)Utility.Random().Next(0x00000001, 0x7FFFFFFF) << 32); 

            this.Width = 320;
            this.Height = 160;
            this.StartPosX = 160;
            this.StartPosY = 80;

            this.TerrainList = new List<TerrainPiece>();
            this.GadgetList = new List<GadgetPiece>();
            this.GroupList = new List<GroupPiece>();

            this.NumLems = 40;
            this.SaveReq = 20;
            this.SpawnRate = 50;
            this.IsSpawnRateFix = false;
            this.TimeLimit = 0;
            this.IsNoTimeLimit = true;

            this.SkillSet = new Dictionary<C.Skill, int>();
            foreach (C.Skill skill in C.SkillArray)
            {
                SkillSet.Add(skill, 0);
            }

            this.PreviewText = new List<string>();
            this.PostviewText = new List<string>();
            this.Talismans = new List<Talisman>();
        }

        public string Title { get; set; }
        public string Author { get; set; }
        public Style MainStyle { get; set; }
        public string MusicFile { get; set; }

        public ulong LevelID { get; set; }
        public string FilePathToSave { get; set; }

        public int Width { get; set; }
        public int Height { get; set; }
        public Rectangle Size => new Rectangle(0, 0, Width, Height);

        public Point StartPos
        {
            get
            {
                return new Point(StartPosX, StartPosY);
            }

            set
            {
                StartPosX = value.X;
                StartPosY = value.Y;
            }
        }

        public int StartPosX { get; set; }
        public int StartPosY { get; set; }

        public List<TerrainPiece> TerrainList { get; set; }
        public List<GadgetPiece> GadgetList { get; set; }
        public List<GroupPiece> GroupList { get; set; }

        public Background Background { get; set; }

        public int NumLems { get; set; }
        public int SaveReq { get; set; }
        public int SpawnRate { get; set; }
        public bool IsSpawnRateFix { get; set; }
        public int TimeLimit { get; set; }
        public bool IsNoTimeLimit { get; set; }

        public Dictionary<C.Skill, int> SkillSet { get; set; }

        public List<Talisman> Talismans { get; set; }  // not changable in the editor
        public List<string> PreviewText { get; set; }  // not changable in the editor
        public List<string> PostviewText { get; set; } // not changable in the editor

        public Size ScreenSize => C.ScreenSize.ScreenArea(Width, Height);

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
            newLevel.FilePathToSave = this.FilePathToSave; // shallow copy is fine here
            newLevel.Background = this.Background; // shallow copy is fine here

            newLevel.Width = this.Width;
            newLevel.Height = this.Height;
            newLevel.StartPosX = this.StartPosX;
            newLevel.StartPosY = this.StartPosY;

            newLevel.TerrainList = new List<TerrainPiece>(this.TerrainList.Select(ter => (TerrainPiece)ter.Clone()));
            newLevel.GadgetList = new List<GadgetPiece>(this.GadgetList.Select(gad => (GadgetPiece)gad.Clone()));
            newLevel.GroupList = new List<GroupPiece>(this.GroupList);

            newLevel.TerrainList.ForEach(ter => ter.IsSelected = false);
            newLevel.GadgetList.ForEach(gad => gad.IsSelected = false);

            newLevel.NumLems = this.NumLems;
            newLevel.SaveReq = this.SaveReq;
            newLevel.SpawnRate = this.SpawnRate;
            newLevel.IsSpawnRateFix = this.IsSpawnRateFix;
            newLevel.TimeLimit = this.TimeLimit;
            newLevel.IsNoTimeLimit = this.IsNoTimeLimit;

            newLevel.SkillSet = new Dictionary<C.Skill, int>();
            foreach (C.Skill skill in C.SkillArray)
            {
                newLevel.SkillSet.Add(skill, this.SkillSet[skill]);
            }

            newLevel.Talismans = new List<Talisman>(this.Talismans);
            newLevel.PreviewText = new List<string>(this.PreviewText);
            newLevel.PostviewText = new List<string>(this.PostviewText);

            return newLevel;
        }

        /// <summary>
        /// Compares two Levels for equality.
        /// </summary>
        /// <param name="otherLevel"></param>
        /// <returns></returns>
        public bool Equals(Level otherLevel)
        {
            if (otherLevel == null
                || !this.Title.Equals(otherLevel.Title)
                || !this.Author.Equals(otherLevel.Author)
                || !((this.MainStyle == null && otherLevel.MainStyle == null) ||
                     (this.MainStyle != null && this.MainStyle.NameInDirectory.Equals(otherLevel.MainStyle?.NameInDirectory)))
                || !this.MusicFile.Equals(otherLevel.MusicFile)
                || !this.LevelID.Equals(otherLevel.LevelID)
                || !((this.Background == null && otherLevel.Background == null) ||
                     (this.Background != null && this.Background.Equals(otherLevel.Background)))
                || this.Width != otherLevel.Width
                || this.Height != otherLevel.Height
                || this.StartPosX != otherLevel.StartPosX
                || this.StartPosY != otherLevel.StartPosY
                || this.TerrainList.Count != otherLevel.TerrainList.Count
                || this.GadgetList.Count != otherLevel.GadgetList.Count
                || this.NumLems != otherLevel.NumLems
                || this.SaveReq != otherLevel.SaveReq
                || this.SpawnRate != otherLevel.SpawnRate
                || this.IsSpawnRateFix != otherLevel.IsSpawnRateFix
                || this.IsNoTimeLimit != otherLevel.IsNoTimeLimit
                || (this.TimeLimit != otherLevel.TimeLimit && !this.IsNoTimeLimit)
                || this.Talismans.Count != otherLevel.Talismans.Count
                || !this.PreviewText.ToString().Equals(otherLevel.PreviewText.ToString())
                || !this.PostviewText.ToString().Equals(otherLevel.PostviewText.ToString()))
            {
                return false;
            }

            foreach (C.Skill skill in C.SkillArray)
            {
                if (this.SkillSet.ContainsKey(skill) != otherLevel.SkillSet.ContainsKey(skill))
                    return false;

                if (this.SkillSet[skill] != otherLevel.SkillSet[skill])
                    return false;
            }

            for (int i = 0; i < this.TerrainList.Count; i++)
            {
                if (!this.TerrainList[i].Equals(otherLevel.TerrainList[i]))
                    return false;
            }

            for (int i = 0; i < this.GadgetList.Count; i++)
            {
                if (!this.GadgetList[i].Equals(otherLevel.GadgetList[i]))
                    return false;
            }

            foreach (Talisman talisman in this.Talismans)
            {
                if (!otherLevel.Talismans.Contains(talisman))
                    return false;
            }

            return true;
        }

        /// <summary>
        /// Returns the theme color as specified by the main style.
        /// </summary>
        /// <param name="styleColor"></param>
        /// <returns></returns>
        public Color GetThemeColor(C.StyleColor styleColor)
        {
            return MainStyle?.GetColor(styleColor) ?? C.NLColors[styleColor.ToNLColor()];
        }

        /// <summary>
        /// Adds a list of pieces to the level.
        /// </summary>
        /// <param name="pieces"></param>
        public void AddMultiplePieces(IEnumerable<LevelPiece> pieces)
        {
            if (pieces == null)
                return;

            foreach (LevelPiece piece in pieces)
            {
                LevelPiece newPiece = piece.Clone();
                newPiece.IsSelected = true;

                if (newPiece is TerrainPiece)
                {
                    TerrainList.Add((TerrainPiece)newPiece);
                }
                else if (newPiece is GadgetPiece)
                {
                    if (newPiece.ObjType.In(C.OBJ.TELEPORTER, C.OBJ.RECEIVER))
                        ((GadgetPiece)newPiece).SetTeleporterValue(0);

                    GadgetList.Add((GadgetPiece)newPiece);
                }
            }
        }


        /// <summary>
        /// Creates a new piece and adds it to the level. 
        /// </summary>
        /// <param name="pieceKey"></param>
        /// <param name="centerPos"></param>
        public void AddPiece(string pieceKey, Point centerPos, int gridSize)
        {
            int piecePosX = (centerPos.X - ImageLibrary.GetWidth(pieceKey) / 2).RoundToMultiple(gridSize);
            int piecePosY = (centerPos.Y - ImageLibrary.GetHeight(pieceKey) / 2).RoundToMultiple(gridSize);
            Point piecePos = new Point(piecePosX, piecePosY);

            if (pieceKey.Contains("object"))
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
            LevelPiece selectedPiece = null;

            if (doPriorityInvert)
            {
                if (DisplaySettings.IsDisplayed(C.DisplayType.Terrain))
                {
                    selectedPiece = TerrainList.Find(ter => ter.ImageRectangle.Contains(pos)
                                                        && (isUnselected ^ ter.IsSelected));
                }
                if (selectedPiece == null && DisplaySettings.IsDisplayed(C.DisplayType.Objects))
                {
                    selectedPiece = GadgetList.Find(gad => gad.ImageRectangle.Contains(pos)
                                                       && (isUnselected ^ gad.IsSelected));
                }
            }
            else
            {
                if (DisplaySettings.IsDisplayed(C.DisplayType.Objects))
                {
                    selectedPiece = GadgetList.FindLast(gad => gad.ImageRectangle.Contains(pos)
                                                           && (isUnselected ^ gad.IsSelected));
                }
                if (selectedPiece == null && DisplaySettings.IsDisplayed(C.DisplayType.Terrain))
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
            if (DisplaySettings.IsDisplayed(C.DisplayType.Terrain))
            {
                TerrainList.FindAll(ter => ter.ImageRectangle.IntersectsWith(rectangle))
                           .ForEach(ter => ter.IsSelected = isAdded);
            }
            if (DisplaySettings.IsDisplayed(C.DisplayType.Objects))
            {
                GadgetList.FindAll(gad => gad.ImageRectangle.IntersectsWith(rectangle))
                          .ForEach(gad => gad.IsSelected = isAdded);
            }
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
            if (selectedPieces.Count == 0)
                return new Rectangle(0, 0, 1, 1);

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
        /// Returns whether there is any terrain or object piece at this point.
        /// </summary>
        /// <param name="pos"></param>
        /// <returns></returns>
        public bool HasPieceAtPos(Point pos)
        {
            return TerrainList.Exists(item => item.ImageRectangle.Contains(pos))
                || GadgetList.Exists(item => item.ImageRectangle.Contains(pos));
        }


        /// <summary>
        /// Moves all selected pieces a given number of pixels into a given direction. 
        /// </summary>
        /// <param name="direction"></param>
        /// <param name="step"></param>
        public void MovePieces(C.DIR direction, int step, int gridSize)
        {
            SelectionList().ForEach(item => item.Move(direction, step, gridSize));
        }

        /// <summary>
        /// Moves all selected pieces to the target position. 
        /// </summary>
        /// <param name="targetPos">Location of the rectangle spanning all selected pieces.</param>
        public void MovePieces(Point targetPos, int gridSize)
        {
            Point referencePos = SelectionRectangle().Location;
            foreach (LevelPiece piece in SelectionList())
            {
                int pieceTargetX = targetPos.X + piece.PosX - referencePos.X;
                int pieceTargetY = targetPos.Y + piece.PosY - referencePos.Y;
                piece.Move(new Point(pieceTargetX, pieceTargetY), gridSize);
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

            // check that paired teleporters/receivers got flipped correctly
            var Teleporters = SelectionList().FindAll(item => item.ObjType.In(C.OBJ.TELEPORTER, C.OBJ.RECEIVER) && (item as GadgetPiece).Val_L != 0);
            foreach (GadgetPiece item in Teleporters)
            {
                var pairedObject = GadgetList.Find(gadget => gadget != item && gadget.Val_L == item.Val_L);
                if (pairedObject != null && !Teleporters.Contains(pairedObject))
                {
                    pairedObject.FlipInRect(pairedObject.ImageRectangle);
                }
            }
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
            TerrainList.FindAll(ter => ter.IsSelected && !ter.IsSketch)
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
            TerrainList.FindAll(ter => ter.IsSelected && !ter.IsSketch)
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
            TerrainList.FindAll(ter => ter.IsSelected && !ter.IsSteel && !ter.IsSketch)
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

            if (teleporter.Val_L != 0)
                RemovePairingValue(teleporter.Val_L);
            if (receiver.Val_L != 0)
                RemovePairingValue(receiver.Val_L);

            int newPairingValue = FindNewPairingValue();

            teleporter.SetTeleporterValue(newPairingValue);
            receiver.SetTeleporterValue(newPairingValue);

            // Set flipping of receiver according to teleporter
            if (receiver.IsFlippedInPlayer != teleporter.IsFlippedInPlayer)
            {
                receiver.FlipInRect(receiver.ImageRectangle);
            }
        }

        /// <summary>
        /// Sets the pickup skill count to the first piece in the selection list.
        /// </summary>
        /// <param name="value"></param>
        public void SetPickupSkillCount(int value)
        {
            GadgetPiece pickup = (GadgetPiece)SelectionList().First();
            System.Diagnostics.Debug.Assert(pickup != null && pickup.ObjType == C.OBJ.PICKUP, "Set pickup skill count, but first selected piece is not a pickup object!");

            pickup.SetPickupSkillCount(value);
        }

        public void SetLemmingLimit(int value)
        {
            GadgetPiece gadget = (GadgetPiece)SelectionList().First();
            System.Diagnostics.Debug.Assert(gadget != null && new[] { C.OBJ.EXIT, C.OBJ.EXIT_LOCKED, C.OBJ.HATCH }.Contains(gadget.ObjType), "Set lemming limit, but first selected piece is not able to have this value!");

            gadget.SetLemmingLimit(value);
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

        /// <summary>
        /// Checks whether we may group the currently selected pieces.
        /// </summary>
        /// <returns></returns>
        public bool MayGroupSelection()
        {
            var selection = TerrainList.FindAll(ter => ter.IsSelected);
            var selectionVisible = selection.FindAll(ter => !ter.IsErase);
            return selection.Count > 1 && selectionVisible.Count > 0
                && (selectionVisible.TrueForAll(ter => ter.IsSteel) || selectionVisible.TrueForAll(ter => !ter.IsSteel));
        }

        /// <summary>
        /// Checks whether we may ungroup (some of) the currently selected pieces.
        /// </summary>
        /// <returns></returns>
        public bool MayUngroupSelection()
        {
            return TerrainList.Exists(ter => ter.IsSelected && ter is GroupPiece);
        }

        /// <summary>
        /// Groups all selected terrain pieces.
        /// </summary>
        public void GroupSelection()
        {
            int insertIndex = TerrainList.FindIndex(ter => ter.IsSelected);
            var selection = TerrainList.FindAll(ter => ter.IsSelected);
            TerrainList.RemoveAll(ter => ter.IsSelected);
            GroupPiece group = null;
            try
            {
                group = new GroupPiece(selection);
            }
            catch (ArgumentException)
            {
                // Selection has no visible pixel, so don't add any kind of group piece.
                return;
            }

            // Check whether the same group was already created
            if (!GroupList.Exists(grp => grp.HasSameKey(group)))
            {
                var newGroup = (GroupPiece)group.Clone();
                newGroup.PosX = 0;
                newGroup.PosY = 0;
                GroupList.Add(newGroup);
            }

            group.IsSelected = true;
            TerrainList.Insert(insertIndex, group);
        }

        /// <summary>
        /// Ungroups all selected group pieces and adds the single pieces to the terrain list. 
        /// </summary>
        public void UngroupSelection()
        {
            var selectedGroups = TerrainList.FindAll(ter => ter.IsSelected && ter is GroupPiece);
            selectedGroups.ForEach(grp => UnGroup((GroupPiece)grp));
        }

        /// <summary>
        /// Ungroups a given group and inserts the single pieces into the terrain list.
        /// </summary>
        /// <param name="group"></param>
        private void UnGroup(GroupPiece group)
        {
            int insertIndex = TerrainList.IndexOf(group);
            TerrainList.Remove(group);
            TerrainList.InsertRange(insertIndex, group.GetConstituents());

            // Check whether this was the last occurence of this group
            if (!TerrainList.Exists(ter => ter.HasSameKey(group))
                && !GroupList.Exists(grp => grp.ContainsConstituent(group)))
            {
                GroupList.Remove(group);
            }
        }

        public void PrepareForSave()
        {
            ValidateTalismanIDs();
        }

        public void ValidateTalismanIDs()
        {
            foreach (var talisman in this.Talismans.Where(tal => tal.ID < 0))
            {
                int i = 0;
                while (Talismans.Count(tal => tal.ID == i) > 0)
                    i++;
                talisman.ID = i;
            }
        }
    }
}
