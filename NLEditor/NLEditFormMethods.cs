﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Diagnostics;
using System.Drawing;

namespace NLEditor
{
    public partial class NLEditForm : Form
    {
        /*---------------------------------------------------------
         *   Main Form: This part defines the methods
         *     called from user input
         * -------------------------------------------------------- */

        private void CreateStyleList()
        {
            // get list of all existing style names
            List<string> StyleNameList = null;

            try
            {
                StyleNameList = Directory.GetDirectories(C.AppPathPieces)
                                         .Select(dir => Path.GetFileName(dir))
                                         .ToList();
            }
            catch (Exception Ex)
            {
                Utility.LogException(Ex);

                MessageBox.Show(Ex.Message);
                Environment.Exit(-1);
            }

            StyleNameList.RemoveAll(sty => sty == "default");
            StyleNameList = LoadFromFile.OrderStyleNames(StyleNameList);

            // Add the actual styles
            fStyleList = StyleNameList.Select(sty => new Style(sty)).ToList();
        }

        private void ReadLevelInfoFromForm()
        {
            CurLevel.Author = this.txt_LevelAuthor.Text;
            CurLevel.Title = this.txt_LevelTitle.Text;
            CurLevel.MusicFile = this.combo_Music.Text;
            CurLevel.MainStyle = ValidateStyleName(this, this.combo_MainStyle.Text);
            CurLevel.Width = Decimal.ToInt32(this.num_Lvl_SizeX.Value);
            CurLevel.Height = Decimal.ToInt32(this.num_Lvl_SizeY.Value);
            CurLevel.StartPosX = Decimal.ToInt32(this.num_Lvl_StartX.Value);
            CurLevel.StartPosY = Decimal.ToInt32(this.num_Lvl_StartY.Value);
            CurLevel.NumLems = Decimal.ToInt32(this.num_Lvl_Lems.Value);
            CurLevel.SaveReq = Decimal.ToInt32(this.num_Lvl_Rescue.Value);
            CurLevel.ReleaseRate = Decimal.ToInt32(this.num_Lvl_RR.Value);
            CurLevel.IsReleaseRateFix = this.check_Lvl_LockRR.Checked;
            CurLevel.TimeLimit = Decimal.ToInt32(this.num_Lvl_TimeMin.Value) * 60
                                    + Decimal.ToInt32(this.num_Lvl_TimeSec.Value);
            CurLevel.IsNoTimeLimit = this.check_Lvl_InfTime.Checked;

            CurLevel.SkillCount[C.SKI_CLIMBER] = Decimal.ToInt32(this.num_Ski_Climber.Value);
            CurLevel.SkillCount[C.SKI_FLOATER] = Decimal.ToInt32(this.num_Ski_Floater.Value);
            CurLevel.SkillCount[C.SKI_BLOCKER] = Decimal.ToInt32(this.num_Ski_Blocker.Value);
            CurLevel.SkillCount[C.SKI_EXPLODER] = Decimal.ToInt32(this.num_Ski_Exploder.Value);
            CurLevel.SkillCount[C.SKI_BUILDER] = Decimal.ToInt32(this.num_Ski_Builder.Value);
            CurLevel.SkillCount[C.SKI_BASHER] = Decimal.ToInt32(this.num_Ski_Basher.Value);
            CurLevel.SkillCount[C.SKI_MINER] = Decimal.ToInt32(this.num_Ski_Miner.Value);
            CurLevel.SkillCount[C.SKI_DIGGER] = Decimal.ToInt32(this.num_Ski_Digger.Value);
            CurLevel.SkillCount[C.SKI_WALKER] = Decimal.ToInt32(this.num_Ski_Walker.Value);
            CurLevel.SkillCount[C.SKI_SWIMMER] = Decimal.ToInt32(this.num_Ski_Swimmer.Value);
            CurLevel.SkillCount[C.SKI_GLIDER] = Decimal.ToInt32(this.num_Ski_Glider.Value);
            CurLevel.SkillCount[C.SKI_DISARMER] = Decimal.ToInt32(this.num_Ski_Disarmer.Value);
            CurLevel.SkillCount[C.SKI_STONER] = Decimal.ToInt32(this.num_Ski_Stoner.Value);
            CurLevel.SkillCount[C.SKI_PLATFORMER] = Decimal.ToInt32(this.num_Ski_Platformer.Value);
            CurLevel.SkillCount[C.SKI_STACKER] = Decimal.ToInt32(this.num_Ski_Stacker.Value);
            CurLevel.SkillCount[C.SKI_CLONER] = Decimal.ToInt32(this.num_Ski_Cloner.Value);
        }

        private void WriteLevelInfoToForm()
        {
            this.txt_LevelAuthor.Text = CurLevel.Author;
            this.txt_LevelTitle.Text = CurLevel.Title;
            this.combo_Music.Text = CurLevel.MusicFile;
            this.combo_MainStyle.Text = (CurLevel.MainStyle != null) ? CurLevel.MainStyle.Name : "";
            this.num_Lvl_SizeX.Value = CurLevel.Width;
            this.num_Lvl_SizeY.Value = CurLevel.Height;
            this.num_Lvl_StartX.Value = CurLevel.StartPosX;
            this.num_Lvl_StartY.Value = CurLevel.StartPosY;
            this.num_Lvl_Lems.Value = CurLevel.NumLems;
            this.num_Lvl_Rescue.Value = CurLevel.SaveReq;
            this.num_Lvl_RR.Value = CurLevel.ReleaseRate;
            this.check_Lvl_LockRR.Checked = CurLevel.IsReleaseRateFix;
            this.num_Lvl_TimeMin.Value = CurLevel.TimeLimit / 60;
            this.num_Lvl_TimeSec.Value = CurLevel.TimeLimit % 60;
            this.check_Lvl_InfTime.Checked = CurLevel.IsNoTimeLimit;

            this.num_Ski_Climber.Value = CurLevel.SkillCount[C.SKI_CLIMBER];
            this.num_Ski_Floater.Value = CurLevel.SkillCount[C.SKI_FLOATER];
            this.num_Ski_Blocker.Value = CurLevel.SkillCount[C.SKI_BLOCKER];
            this.num_Ski_Exploder.Value = CurLevel.SkillCount[C.SKI_EXPLODER];
            this.num_Ski_Builder.Value = CurLevel.SkillCount[C.SKI_BUILDER];
            this.num_Ski_Basher.Value = CurLevel.SkillCount[C.SKI_BASHER];
            this.num_Ski_Miner.Value = CurLevel.SkillCount[C.SKI_MINER];
            this.num_Ski_Digger.Value = CurLevel.SkillCount[C.SKI_DIGGER];
            this.num_Ski_Walker.Value = CurLevel.SkillCount[C.SKI_WALKER];
            this.num_Ski_Swimmer.Value = CurLevel.SkillCount[C.SKI_SWIMMER];
            this.num_Ski_Glider.Value = CurLevel.SkillCount[C.SKI_GLIDER];
            this.num_Ski_Disarmer.Value = CurLevel.SkillCount[C.SKI_DISARMER];
            this.num_Ski_Stoner.Value = CurLevel.SkillCount[C.SKI_STONER];
            this.num_Ski_Platformer.Value = CurLevel.SkillCount[C.SKI_PLATFORMER];
            this.num_Ski_Stacker.Value = CurLevel.SkillCount[C.SKI_STACKER];
            this.num_Ski_Cloner.Value = CurLevel.SkillCount[C.SKI_CLONER];
        }


        private void ExitEditor()
        {
            Application.Exit();
        }

        private void ChangeObjTerrPieceDisplay()
        {
            // Switch between displaying objects and terrain pieces
            PieceDoDisplayObject = !PieceDoDisplayObject;

            PieceStartIndex = 0;
            LoadPiecesIntoPictureBox(this, PieceCurStyle);

            this.but_PieceTerrObj.Text = PieceDoDisplayObject ? "Get Terrain" : "Get Objects";
        }


        private void MoveTerrPieceSelection(int Movement)
        {
            Style CurStyle = PieceCurStyle;
            if (CurStyle == null) return;

            List<string> PieceNameList = PieceDoDisplayObject ? CurStyle.ObjectNames : CurStyle.TerrainNames;
            if (PieceNameList == null || PieceNameList.Count == 0) return;

            // Pass to correct piece index
            PieceStartIndex = (PieceStartIndex + Movement) % PieceNameList.Count;
            // ensure that PieceStartIndex is positive
            PieceStartIndex = (PieceStartIndex + PieceNameList.Count) % PieceNameList.Count;

            LoadPiecesIntoPictureBox(this, CurStyle);
        }

        private void ChangeNewPieceStyleSelection(int Movement)
        {
            if (StyleList == null || StyleList.Count == 0) return;

            int NewStyleIndex;

            if (PieceCurStyle == null)
            {
                NewStyleIndex = ((Movement % StyleList.Count) + StyleList.Count) % StyleList.Count;
            }
            else 
            {
                int CurStyleIndex = StyleList.FindIndex(sty => sty.Equals(PieceCurStyle));
                Debug.Assert(CurStyleIndex != -1, "Current style for new pieces not found in StyleList.");

                NewStyleIndex = Math.Min(Math.Max(CurStyleIndex + Movement, 0), StyleList.Count - 1);
            }

            PieceCurStyle = StyleList[NewStyleIndex];
            PieceStartIndex = 0;
            LoadPiecesIntoPictureBox(this, PieceCurStyle);

            this.combo_PieceStyle.SelectedIndex = NewStyleIndex;
        }

        private void AddNewTerrainPiece(int picPieceIndex)
        { 
            List<string> CurPieceList = fPieceDoDisplayObject ?  fPieceCurStyle.ObjectNames : fPieceCurStyle.ObjectNames;
            int PieceIndex = (picPieceIndex + fPieceStartIndex) % CurPieceList.Count;

            CurLevel.AddPiece(fPieceCurStyle, fPieceDoDisplayObject, PieceIndex, fCurRenderer.GetCenterPoint());

            this.pic_Level.Image = fCurRenderer.CreateLevelImage();
        }

    }
}
