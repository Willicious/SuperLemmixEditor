using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace NLEditor
{
    class Level
    {
        /*---------------------------------------------------------
         *          This class stores all global level infos
         * -------------------------------------------------------- */

        /* --------------------------------------------------------
         *   public methods:
         *   public variables:

         * -------------------------------------------------------- */

        public Level()
        {
            this.fName = "";
            this.fAuthor = "";
            this.fMainStyle = ""; // DO WE REALLY WANT THIS???
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
            this.fTimeLimit = -1; // Infinite time

            this.fSkillCount = new int[C.SKI_COUNT];
            for (int i = 0; i < Math.Min(8, C.SKI_COUNT); i++)
            {
                this.fSkillCount[i] = 10;
            }
            for (int i = 8; i < C.SKI_COUNT; i++)
            {
                this.fSkillCount[i] = -1;
            }

            this.fScreenPos = new Point(0, 0);
        }


        string fName;
        string fAuthor;
        string fMainStyle;
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

        int[] fSkillCount;

        Point fScreenPos;

        public string Name { get { return fName; } set { fName = value; } }
        public string Author { get { return fAuthor; } set { fAuthor = value; } }
        public string MainStyle { get { return fMainStyle; } set { fMainStyle = value; } }
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

        public Point ScreenPos { get { return fScreenPos; } }
        public int ScreenPosX { get { return fScreenPos.X; } set { fScreenPos.X = value; } }
        public int ScreenPosY { get { return fScreenPos.Y; } set { fScreenPos.Y = value; } }
    }
}
