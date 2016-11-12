﻿using System;
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

        /* --------------------------------------------------------
         *   public methods:

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

            this.fScreenPos = new Point(0, 0);
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

        Point fScreenPos;

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

        public Point ScreenPos { get { return fScreenPos; } }
        public int ScreenPosX { get { return fScreenPos.X; } set { fScreenPos.X = value; } }
        public int ScreenPosY { get { return fScreenPos.Y; } set { fScreenPos.Y = value; } }
    }
}
