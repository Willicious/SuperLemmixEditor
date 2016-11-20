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
         *    AddPiece(Style NewStyle, bool IsObject, int NewPieceIndex, Point CenterPos)
         *    
         *    SelectOnePiece(Point Pos, bool IsAdded, bool IsHighest)
         *    SelectAreaPiece(Rectangle Rect, bool IsAdded)
         *    DeleteAllSelections()
         *    MovePieces(C.DIR Direcion)
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

        public int NumLems { get { return fNumLems; } set { fNumLems = value; } }
        public int SaveReq { get { return fSaveReq; } set { fSaveReq = value; } }
        public int ReleaseRate { get { return fReleaseRate; } set { fReleaseRate = value; } }
        public bool IsReleaseRateFix { get { return fIsReleaseRateFix; } set { fIsReleaseRateFix = value; } }
        public int TimeLimit { get { return fTimeLimit; } set { fTimeLimit = value; } }
        public bool IsNoTimeLimit { get { return fIsNoTimeLimit; } set { fIsNoTimeLimit = value; } }

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
        /// Moves a piece a given number of pixels into a given direction. 
        /// </summary>
        /// <param name="Direction"></param>
        /// <param name="Step"></param>
        public void MovePieces(C.DIR Direction, int Step = 1)
        {
            TerrainList.FindAll(ter => ter.IsSelected)
                       .ForEach(ter => ter.Move(Direction, Step));
            GadgetList.FindAll(obj => obj.IsSelected)
                      .ForEach(obj => obj.Move(Direction, Step));
        }

    }
}
