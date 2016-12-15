using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
         *    DeleteAllSelections()
         *    SelectionList()
         *    HasSelectionAtPos(Point Pos)
         *    MovePieces(C.DIR Direcion, int Step = 1)
         *    RotatePieces()
         *    InvertPieces()
         *    FlipPieces()
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
        /// <param name="MainStyle"></param>
        public Level(Style MainStyle = null)
        {
            this.fTitle = "";
            this.fAuthor = "";
            this.fMainStyle = MainStyle;
            this.MusicFile = "";
            this.fBackgroundKey = "";

            Random rnd = new Random();
            this.LevelID = (uint)rnd.Next();

            this.fWidth = 320;
            this.fHeight = 160;
            this.fStartPos = new Point(0, 0);

            this.fTerrainList = new List<TerrainPiece>();
            this.fGadgetList = new List<GadgetPiece>();

            this.fNumLems = 40;
            this.fSaveReq = 20;
            this.fReleaseRate = 50;
            this.fIsReleaseRateFix = false;
            this.fTimeLimit = 0;
            this.fIsNoTimeLimit = true;

            this.SkillCount = new int[C.SKI_COUNT];
            for (int i = 0; i < C.SKI_COUNT; i++)
            {
                this.SkillCount[i] = 0;
            }
        }


        string fTitle;
        string fAuthor;
        Style fMainStyle;
        string fMusicFile;

        uint fLevelID;
        string fFilePathToSave;

        int fWidth;
        int fHeight;
        Point fStartPos;

        List<TerrainPiece> fTerrainList;
        List<GadgetPiece> fGadgetList;
        string fBackgroundKey;

        int fNumLems;
        int fSaveReq;
        int fReleaseRate;
        bool fIsReleaseRateFix;
        int fTimeLimit;
        bool fIsNoTimeLimit;

        public int[] SkillCount { get; set; }

        public string Title { get { return fTitle; } set { fTitle = value; } }
        public string Author { get { return fAuthor; } set { fAuthor = value; } }
        public Style MainStyle { get { return fMainStyle; } set { fMainStyle = value; } }
        public string MusicFile { get { return fMusicFile; } set { fMusicFile = value; } }

        public uint LevelID { get { return fLevelID; } set { fLevelID = value; } }
        public string FilePathToSave { get { return fFilePathToSave; } set { fFilePathToSave = value; } }

        public int Width { get { return fWidth; } set { fWidth = value; } }
        public int Height { get { return fHeight; } set { fHeight = value; } }
        public Point StartPos { get { return fStartPos; }}
        public int StartPosX { get { return fStartPos.X; } set { fStartPos.X = value; } }
        public int StartPosY { get { return fStartPos.Y; } set { fStartPos.Y = value; } }

        public List<TerrainPiece> TerrainList { get { return fTerrainList; } set { fTerrainList = value; } }
        public List<GadgetPiece> GadgetList { get { return fGadgetList; } set { fGadgetList = value; } }
        public string BackgroundKey 
        {
            get { return fMainStyle.NameInDirectory + C.DirSep + "backgrounds" + C.DirSep + fBackgroundKey; } 
            set { fBackgroundKey = value; } 
        }

        public int NumLems { get { return fNumLems; } set { fNumLems = value; } }
        public int SaveReq { get { return fSaveReq; } set { fSaveReq = value; } }
        public int ReleaseRate { get { return fReleaseRate; } set { fReleaseRate = value; } }
        public bool IsReleaseRateFix { get { return fIsReleaseRateFix; } set { fIsReleaseRateFix = value; } }
        public int TimeLimit { get { return fTimeLimit; } set { fTimeLimit = value; } }
        public bool IsNoTimeLimit { get { return fIsNoTimeLimit; } set { fIsNoTimeLimit = value; } }


        /// <summary>
        /// Creates a deep copy of the level.
        /// </summary>
        /// <returns></returns>
        public Level Clone()
        {
            Level NewLevel = new Level();
            NewLevel.fTitle = String.Copy(this.fTitle);
            NewLevel.fAuthor = String.Copy(this.fAuthor);
            NewLevel.fMainStyle = this.fMainStyle;
            NewLevel.fMusicFile = String.Copy(this.fMusicFile);
            NewLevel.fLevelID = this.fLevelID;
            NewLevel.fFilePathToSave = this.fFilePathToSave; // shallow copy is fine here
            NewLevel.fBackgroundKey = String.Copy(this.fBackgroundKey);

            NewLevel.fWidth = this.fWidth;
            NewLevel.fHeight = this.fHeight;
            NewLevel.fStartPos = new Point(this.fStartPos.X, this.fStartPos.Y);

            NewLevel.fTerrainList = new List<TerrainPiece>(this.fTerrainList.Select(ter => (TerrainPiece)ter.Clone()));
            NewLevel.fGadgetList = new List<GadgetPiece>(this.fGadgetList.Select(obj => (GadgetPiece)obj.Clone()));

            NewLevel.fTerrainList.ForEach(ter => ter.IsSelected = false);
            NewLevel.fGadgetList.ForEach(obj => obj.IsSelected = false);

            NewLevel.fNumLems = this.fNumLems;
            NewLevel.fSaveReq = this.fSaveReq;
            NewLevel.fReleaseRate = this.fReleaseRate;
            NewLevel.fIsReleaseRateFix = this.fIsReleaseRateFix;
            NewLevel.fTimeLimit = this.fTimeLimit;
            NewLevel.fIsNoTimeLimit = this.fIsNoTimeLimit;

            NewLevel.SkillCount = new int[C.SKI_COUNT];
            for (int Skill = 0; Skill < C.SKI_COUNT; Skill++)
            {
                NewLevel.SkillCount[Skill] = this.SkillCount[Skill];
            }

            return NewLevel;
        }

        /// <summary>
        /// Compares two Levels for equality.
        /// </summary>
        /// <param name="OtherLevel"></param>
        /// <returns></returns>
        public bool Equals(Level OtherLevel)
        {
            if ( OtherLevel == null
                || !this.Title.Equals(OtherLevel.Title)
                || !this.Author.Equals(OtherLevel.Author)
                || !this.MainStyle.NameInDirectory.Equals(OtherLevel.MainStyle.NameInDirectory)
                || !this.MusicFile.Equals(OtherLevel.MusicFile)
                || this.LevelID != OtherLevel.LevelID
                || !this.BackgroundKey.Equals(OtherLevel.BackgroundKey)
                || this.Width != OtherLevel.Width
                || this.Height != OtherLevel.Height
                || !this.StartPos.Equals(OtherLevel.StartPos)
                || this.TerrainList.Count != OtherLevel.TerrainList.Count
                || this.GadgetList.Count != OtherLevel.GadgetList.Count
                || this.NumLems != OtherLevel.NumLems
                || this.SaveReq != OtherLevel.SaveReq
                || this.ReleaseRate != OtherLevel.ReleaseRate
                || this.IsReleaseRateFix != OtherLevel.IsReleaseRateFix
                || this.IsNoTimeLimit != OtherLevel.IsNoTimeLimit
                || (this.TimeLimit != OtherLevel.TimeLimit && !this.IsNoTimeLimit))
            {
                return false;
            }

            for (int i = 0; i < this.TerrainList.Count; i++)
            {
                if (!this.TerrainList[i].Equals(OtherLevel.TerrainList[i])) return false;
            }

            for (int i = 0; i < this.GadgetList.Count; i++)
            {
                if (!this.GadgetList[i].Equals(OtherLevel.GadgetList[i])) return false;
            }

            return true;
        }


        /// <summary>
        /// Creates a new piece and adds it to the level.
        /// </summary>
        /// <param name="NewStyle"></param>
        /// <param name="IsObject"></param>
        /// <param name="NewPieceIndex"></param>
        /// <param name="CenterPos"></param>
        public void AddPiece(Style NewStyle, bool IsObject, int NewPieceIndex, Point CenterPos)
        {
            string PieceKey = IsObject ? NewStyle.ObjectNames[NewPieceIndex] : NewStyle.TerrainNames[NewPieceIndex];

            Point PiecePos = new Point(CenterPos.X - ImageLibrary.GetWidth(PieceKey) / 2,
                                       CenterPos.Y - ImageLibrary.GetHeight(PieceKey) / 2);

            if (IsObject)
            {
                GadgetList.Add(new GadgetPiece(PieceKey, PiecePos));
            }
            else
            {
                TerrainList.Add(new TerrainPiece(PieceKey, PiecePos));
            }
        }

        /// <summary>
        /// Sets the "IsSelected" flag for the first piece that can receive it.
        /// </summary>
        /// <param name="Pos"></param>
        /// <param name="IsAdded"></param>
        /// <param name="DoPriorityInvert"></param>
        public void SelectOnePiece(Point Pos, bool IsAdded, bool DoPriorityInvert)
        {
            LevelPiece SelPiece = GetOnePiece(Pos, IsAdded, DoPriorityInvert);

            if (SelPiece != null)
            {
                SelPiece.IsSelected = IsAdded;
            }
        }

        /// <summary>
        /// Determines the piece to select.
        /// </summary>
        /// <param name="Pos"></param>
        /// <param name="IsUnselected"></param>
        /// <param name="DoPriorityInvert"></param>
        /// <returns></returns>
        private LevelPiece GetOnePiece(Point Pos, bool IsUnselected, bool DoPriorityInvert)
        {
            LevelPiece SelPiece;

            if (DoPriorityInvert)
            {
                SelPiece = TerrainList.Find(ter => ter.ImageRectangle.Contains(Pos)
                                   && (IsUnselected ^ ter.IsSelected));
                if (SelPiece == null)
                {
                    SelPiece = GadgetList.Find(obj => obj.ImageRectangle.Contains(Pos)
                                                      && (IsUnselected ^ obj.IsSelected));
                }
            }
            else
            {
                SelPiece = GadgetList.FindLast(obj => obj.ImageRectangle.Contains(Pos)
                                      && (IsUnselected ^ obj.IsSelected));
                if (SelPiece == null)
                {
                    SelPiece = TerrainList.FindLast(ter => ter.ImageRectangle.Contains(Pos)
                                                           && (IsUnselected ^ ter.IsSelected));
                }
            }

            return SelPiece;
        }

        /// <summary>
        /// Select all pieces that intersect with a given area.
        /// </summary>
        /// <param name="Rect"></param>
        /// <param name="IsAdded"></param>
        public void SelectAreaPiece(Rectangle Rect, bool IsAdded)
        {
            TerrainList.FindAll(ter => ter.ImageRectangle.IntersectsWith(Rect))
                       .ForEach(ter => ter.IsSelected = IsAdded);
            GadgetList.FindAll(obj => obj.ImageRectangle.IntersectsWith(Rect))
                      .ForEach(obj => obj.IsSelected = IsAdded);
        }

        /// <summary>
        /// Removes the "IsSelected" flag from all pieces.
        /// </summary>
        public void DeleteAllSelections()
        {
            TerrainList.ForEach(ter => ter.IsSelected = false);
            GadgetList.ForEach(obj => obj.IsSelected = false);
        }

        /// <summary>
        /// Gets a list of a currently selected pieces in the level.
        /// </summary>
        /// <returns></returns>
        public List<LevelPiece> SelectionList()
        {
            List<LevelPiece> SelectedPieceList = new List<LevelPiece>();
            SelectedPieceList.AddRange(TerrainList.FindAll(ter => ter.IsSelected));
            SelectedPieceList.AddRange(GadgetList.FindAll(obj => obj.IsSelected));
            return SelectedPieceList;
        }

        /// <summary>
        /// Gets the smallest rectangle around all selected pieces in the level.
        /// </summary>
        /// <returns></returns>
        private Rectangle SelectionRectangle()
        {
            List<LevelPiece> SelectedPieceList = SelectionList();

            int Left = SelectedPieceList.Min(item => item.PosX);
            int Right = SelectedPieceList.Max(item => item.PosX + item.Width);
            int Top = SelectedPieceList.Min(item => item.PosY);
            int Bottom = SelectedPieceList.Max(item => item.PosY + item.Height);

            return new Rectangle(Left, Top, Right - Left, Bottom - Top);
        }

        /// <summary>
        /// Returns whether there is a selected terrain piece at a point.
        /// </summary>
        /// <param name="Pos"></param>
        /// <returns></returns>
        public bool HasSelectionAtPos(Point Pos)
        {
            return SelectionList().Exists(item => item.ImageRectangle.Contains(Pos));
        }

        /// <summary>
        /// Moves all selected pieces a given number of pixels into a given direction. 
        /// </summary>
        /// <param name="Direction"></param>
        /// <param name="Step"></param>
        public void MovePieces(C.DIR Direction, int Step = 1)
        {
            SelectionList().ForEach(item => item.Move(Direction, Step));
        }

        /// <summary>
        /// Rotates all selected pieces in their rectangular hull.
        /// </summary>
        public void RotatePieces()
        { 
            Rectangle BorderRect = SelectionRectangle();
            SelectionList().ForEach(item => item.RotateInRect(BorderRect));
        }

        /// <summary>
        /// Inverts all selected pieces in their rectangular hull.
        /// </summary>
        public void InvertPieces()
        {
            Rectangle BorderRect = SelectionRectangle();
            SelectionList().ForEach(item => item.InvertInRect(BorderRect));
        }

        /// <summary>
        /// Flips all selected pieces in their rectangular hull.
        /// </summary>
        public void FlipPieces()
        {
            Rectangle BorderRect = SelectionRectangle();
            SelectionList().ForEach(item => item.FlipInRect(BorderRect));
        }

        /// <summary>
        /// Sets the NoOverwrite flag for all objects and terrain pieces.
        /// </summary>
        /// <param name="DoAdd"></param>
        public void SetNoOverwrite(bool DoAdd)
        {
            TerrainList.FindAll(ter => ter.IsSelected)
                       .ForEach(ter => { ter.IsNoOverwrite = DoAdd; if (DoAdd) ter.IsErase = false; });
            GadgetList.FindAll(obj => obj.IsSelected)
                      .ForEach(obj => { obj.IsNoOverwrite = DoAdd; if (DoAdd) obj.IsOnlyOnTerrain = false; });
        }

        /// <summary>
        /// Sets the Erase flag for all terrain pieces.
        /// </summary>
        /// <param name="DoAdd"></param>
        public void SetErase(bool DoAdd)
        {
            TerrainList.FindAll(ter => ter.IsSelected)
                       .ForEach(ter => { ter.IsErase = DoAdd; if (DoAdd) ter.IsNoOverwrite = false; });
        }

        /// <summary>
        /// Sets the OnlyOnTerrain flag for all objects.
        /// </summary>
        /// <param name="DoAdd"></param>
        public void SetOnlyOnTerrain(bool DoAdd)
        {
            GadgetList.FindAll(obj => obj.IsSelected)
                      .ForEach(obj => { obj.IsOnlyOnTerrain = DoAdd; if (DoAdd) obj.IsNoOverwrite = false; });
        }

        /// <summary>
        /// Sets the OneWay flag for all terrain pieces.
        /// </summary>
        /// <param name="DoAdd"></param>
        public void SetOneWay(bool DoAdd)
        {
            TerrainList.FindAll(ter => ter.IsSelected && !ter.IsSteel)
                       .ForEach(ter => ter.IsOneWay = DoAdd);
        }

        /// <summary>
        /// Adds or removes a skill flag from all selected objects 
        /// </summary>
        /// <param name="Skill"></param>
        public void SetSkillForObjects(int Skill, bool DoAdd)
        {
            GadgetList.FindAll(obj => obj.IsSelected)
                      .ForEach(obj => obj.SetSkillFlag(Skill, DoAdd));
        }

        /// <summary>
        /// Changes the index of all selected pieces.
        /// </summary>
        /// <param name="ToTop"></param>
        /// <param name="OnlyOneStep"></param>
        public void MoveSelectedIndex(bool ToTop, bool OnlyOneStep)
        {
            if (OnlyOneStep && ToTop)
            {
                MoveSelectedIndexOneToTop();
            }
            else if (OnlyOneStep && !ToTop)
            {
                MoveSelectedIndexOneToBottom();
            }
            else
            {
                MoveSelectedIndexMaximally(ToTop);
            }
        }

        /// <summary>
        /// Moves all selected pieces to the beginning or the end of their respective lists.
        /// </summary>
        /// <param name="ToTop"></param>
        private void MoveSelectedIndexMaximally(bool ToTop)
        {
            List<GadgetPiece> SelectedGadgets = GadgetList.FindAll(obj => obj.IsSelected);
            List<GadgetPiece> NonSelectedGadgets = GadgetList.FindAll(obj => !obj.IsSelected);
            if (ToTop)
            {
                GadgetList = NonSelectedGadgets.Concat(SelectedGadgets).ToList();
            }
            else
            {
                GadgetList = SelectedGadgets.Concat(NonSelectedGadgets).ToList();
            }

            List<TerrainPiece> SelectedTerrain = TerrainList.FindAll(obj => obj.IsSelected);
            List<TerrainPiece> NonSelectedTerrain = TerrainList.FindAll(obj => !obj.IsSelected);
            if (ToTop)
            {
                TerrainList = NonSelectedTerrain.Concat(SelectedTerrain).ToList();
            }
            else
            {
                TerrainList = SelectedTerrain.Concat(NonSelectedTerrain).ToList();
            }
        }

        /// <summary>
        /// Moves all selected pieces one index upwards.
        /// </summary>
        private void MoveSelectedIndexOneToTop()
        {
            for (int Index = TerrainList.Count - 1; Index > 0; Index--)
            {
                if (!TerrainList[Index].IsSelected && TerrainList[Index - 1].IsSelected)
                {
                    TerrainList.Swap(Index, Index - 1);
                }
            }

            for (int Index = GadgetList.Count - 1; Index > 0; Index--)
            {
                if (!GadgetList[Index].IsSelected && GadgetList[Index - 1].IsSelected)
                {
                    GadgetList.Swap(Index, Index - 1);
                }
            }
        }

        /// <summary>
        /// Moves all selected pieces one index downwards.
        /// </summary>
        private void MoveSelectedIndexOneToBottom()
        {
            for (int Index = 0; Index < TerrainList.Count - 1; Index++)
            {
                if (!TerrainList[Index].IsSelected && TerrainList[Index + 1].IsSelected)
                {
                    TerrainList.Swap(Index, Index + 1);
                }
            }

            for (int Index = 0; Index < GadgetList.Count - 1; Index++)
            {
                if (!GadgetList[Index].IsSelected && GadgetList[Index + 1].IsSelected)
                {
                    GadgetList.Swap(Index, Index + 1);
                }
            }
        }

        /// <summary>
        /// Pairs a selected teleporter and receiver.
        /// </summary>
        public void PairTeleporters()
        { 
            GadgetPiece MyTeleporter = (GadgetPiece)SelectionList().Find(obj => obj.ObjType == C.OBJ.TELEPORTER);
            GadgetPiece MyReceiver = (GadgetPiece)SelectionList().Find(obj => obj.ObjType == C.OBJ.RECEIVER);

            System.Diagnostics.Debug.Assert(MyTeleporter != null, "Tried to pair teleporters without a selected teleporter!");
            System.Diagnostics.Debug.Assert(MyReceiver != null, "Tried to pair teleporters without a selected teleporter!");

            if (MyTeleporter.Val_L != 0) RemovePairingValue(MyTeleporter.Val_L);
            if (MyReceiver.Val_L != 0) RemovePairingValue(MyReceiver.Val_L);

            int NewPairingValue = FindNewPairingValue();

            MyTeleporter.SetTeleporterValue(NewPairingValue);
            MyReceiver.SetTeleporterValue(NewPairingValue);
        }

        /// <summary>
        /// Removes the key-value RemoveValue from all teleporter and receiver objects.
        /// </summary>
        /// <param name="RemoveValue"></param>
        private void RemovePairingValue(int RemoveValue)
        {
            fGadgetList.FindAll(obj => obj.ObjType.In(C.OBJ.TELEPORTER, C.OBJ.RECEIVER))
                       .ForEach(obj => obj.SetTeleporterValue(0)); 
        }

        /// <summary>
        /// Find the lowest unused teleporter/receiver key value.
        /// </summary>
        /// <returns></returns>
        private int FindNewPairingValue()
        {
            int PairingValue = 0;
            do
            {
                PairingValue++;
            } while (fGadgetList.Exists(obj => obj.ObjType.In(C.OBJ.TELEPORTER, C.OBJ.RECEIVER)
                                               && obj.Val_L == PairingValue));

            return PairingValue;
        }

    }
}
