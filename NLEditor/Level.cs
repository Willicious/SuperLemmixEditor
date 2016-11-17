using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace NLEditor
{
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
         * -------------------------------------------------------- */

        public Level(Style MainStyle = null)
        {
            this.fTitle = "";
            this.fAuthor = "";
            this.fMainStyle = MainStyle;
            this.MusicFile = "";

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
            for (int i = 0; i < Math.Min(8, C.SKI_COUNT); i++)
            {
                this.SkillCount[i] = 10;
            }
            for (int i = 8; i < C.SKI_COUNT; i++)
            {
                this.SkillCount[i] = 0;
            }
        }


        string fTitle;
        string fAuthor;
        Style fMainStyle;
        string fMusicFile;

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


        public void AddPiece(Style NewStyle, bool IsObject, int NewPieceIndex, Point CenterPos)
        {
            string PieceKey = IsObject ? NewStyle.ObjectNames[NewPieceIndex] : NewStyle.TerrainNames[NewPieceIndex];
            string PieceName = System.IO.Path.GetFileName(PieceKey);

            Point PiecePos = new Point(CenterPos.X - ImageLibrary.GetWidth(PieceKey) / 2,
                                       CenterPos.Y - ImageLibrary.GetHeight(PieceKey) / 2);

            if (IsObject)
            {
                GadgetList.Add(new GadgetPiece(NewStyle.FileName, PieceName, PiecePos));
            }
            else
            {
                TerrainList.Add(new TerrainPiece(NewStyle.FileName, PieceName, PiecePos));
            }
        }


        public void SelectOnePiece(Point Pos, bool IsAdded, bool IsHighest)
        {
            LevelPiece SelPiece = GetOnePiece(Pos, IsAdded, IsHighest);

            if (SelPiece != null)
            {
                SelPiece.IsSelected = IsAdded;
            }
        }

        private LevelPiece GetOnePiece(Point Pos, bool IsAdded, bool IsHighest)
        {
            LevelPiece SelPiece;

            if (IsHighest)
            {
                SelPiece = GadgetList.FindLast(obj => obj.ImageRectangle.Contains(Pos) 
                                                      && (IsAdded ^ obj.IsSelected));
                if (SelPiece == null)
                {
                    SelPiece = TerrainList.FindLast(ter => ter.ImageRectangle.Contains(Pos)
                                                           && (IsAdded ^ ter.IsSelected)); 
                }
            }
            else
            {
                SelPiece = TerrainList.Find(ter => ter.ImageRectangle.Contains(Pos)
                                                   && (IsAdded ^ ter.IsSelected)); 
                if (SelPiece == null)
                {
                    SelPiece = GadgetList.Find(obj => obj.ImageRectangle.Contains(Pos)
                                                      && (IsAdded ^ obj.IsSelected));
                }
            }

            return SelPiece;
        }

        public void SelectAreaPiece(Rectangle Rect, bool IsAdded)
        {
            TerrainList.FindAll(ter => ter.ImageRectangle.IntersectsWith(Rect))
                       .ForEach(ter => ter.IsSelected = IsAdded);
            GadgetList.FindAll(obj => obj.ImageRectangle.IntersectsWith(Rect))
                      .ForEach(obj => obj.IsSelected = IsAdded);
        }

        public void DeleteAllSelections()
        {
            TerrainList.ForEach(ter => ter.IsSelected = false);
            GadgetList.ForEach(obj => obj.IsSelected = false);
        }

    }
}
