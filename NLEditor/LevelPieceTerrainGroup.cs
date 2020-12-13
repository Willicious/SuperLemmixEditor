using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace NLEditor
{
    /// <summary>
    /// This stored all data of a terrain group. Inherits from LevelPiece.
    /// </summary>
    [Serializable]
    class GroupPiece : TerrainPiece
    {
        public GroupPiece(GroupPiece oldGroupPiece, Point pos)
            : base(oldGroupPiece.Key, pos)
        {
            terrainPieces = oldGroupPiece.terrainPieces;
        }

        public GroupPiece(GroupPiece oldGroupPiece, Point pos, int rotation, bool isInvert, bool isErase, bool isNoOverwrite, bool isOneWay)
            : base(oldGroupPiece.Key, pos, rotation, isInvert, isErase, isNoOverwrite, isOneWay, 0, 0)
        {
            terrainPieces = oldGroupPiece.terrainPieces;
        }

        public GroupPiece(List<TerrainPiece> terPieceList, string groupName = null)
            : base(groupName != null ? "default\\terrain\\" + groupName : GetKeyFromTerPieceList(terPieceList), GetPrelimPosFromTerPieceList(terPieceList))
        {
            terrainPieces = terPieceList.ConvertAll(ter => (TerrainPiece)ter.Clone()).ToList();
            terrainPieces.ForEach(ter => { ter.PosX -= this.PosX; ter.PosY -= this.PosY; });
            bool isSteelGroup = terrainPieces.Exists(ter => ter.IsSteel && !ter.IsErase);

            // Create a cropped image of the group
            Bitmap groupImage;
            using (Renderer groupRenderer = new Renderer())
            {
                bool oldClearPhysics = DisplaySettings.IsDisplayed(C.DisplayType.ClearPhysics);

                if (oldClearPhysics)
                    DisplaySettings.ChangeDisplayed(C.DisplayType.ClearPhysics);

                groupImage = groupRenderer.CreateTerrainGroupImage(terrainPieces);

                if (oldClearPhysics)
                    DisplaySettings.ChangeDisplayed(C.DisplayType.ClearPhysics);
            }

            Rectangle cropRect = groupImage.GetCropTransparentRectangle();
            groupImage = groupImage.Crop(cropRect);
            // Adapt positions to cropped rectangle
            if (cropRect.X != 0 || cropRect.Y != 0)
            {
                this.PosX += cropRect.X;
                this.PosY += cropRect.Y;
                terrainPieces.ForEach(ter => { ter.PosX -= cropRect.X; ter.PosY -= cropRect.Y; });
            }
            // Add the group image to the image library
            ImageLibrary.AddNewImage(Key, groupImage, isSteelGroup ? C.OBJ.STEEL : C.OBJ.TERRAIN,
                                     new Rectangle(), C.Resize.None);
        }

        List<TerrainPiece> terrainPieces; // already with adapted positions

        /// <summary>
        /// Creates the group key from the terrain list.
        /// </summary>
        /// <param name="terPieceList"></param>
        /// <returns></returns>
        private static string GetKeyFromTerPieceList(List<TerrainPiece> terPieceList)
        {
            Point groupPos = GetPrelimPosFromTerPieceList(terPieceList);

            StringBuilder keyString = new StringBuilder();
            foreach (TerrainPiece piece in terPieceList)
            {
                keyString.Append(piece.Style)
                         .Append(piece.Name)
                         .Append(piece.PosX - groupPos.X)
                         .Append(piece.PosY - groupPos.Y)
                         .Append(piece.IsRotatedInPlayer)
                         .Append(piece.IsFlippedInPlayer)
                         .Append(piece.IsInvertedInPlayer)
                         .Append(piece.IsNoOverwrite)
                         .Append(piece.IsOneWay)
                         .Append(piece.IsErase);
            }

            string hashKeyString = keyString.ToString().GetHashCode().ToString();
            return "default" + C.DirSep + "terrain" + C.DirSep + "Group" + hashKeyString;
        }

        /// <summary>
        /// Gets the preliminarly position (before cropping) of the group from a raw terrain list.
        /// </summary>
        /// <param name="terPieceList"></param>
        /// <returns></returns>
        private static Point GetPrelimPosFromTerPieceList(List<TerrainPiece> terPieceList)
        {
            int minXPos = terPieceList.Min(ter => ter.PosX);
            int minYPos = terPieceList.Min(ter => ter.PosY);
            return new Point(minXPos, minYPos);
        }

        public override LevelPiece Clone()
        {
            return new GroupPiece(this, new Point(PosX, PosY), Rotation,
                                  IsInvert, IsErase, IsNoOverwrite, IsOneWay);
        }

        /// <summary>
        /// Gets a lits of all terrain pieces that are part of the group.
        /// </summary>
        /// <returns></returns>
        public List<TerrainPiece> GetConstituents()
        {
            var terPieceList = terrainPieces.ConvertAll(ter => (TerrainPiece)ter.Clone()).ToList();
            terPieceList.ForEach(ter => { ter.PosX += this.PosX; ter.PosY += this.PosY; });
            return terPieceList;
        }

        /// <summary>
        /// Checks whether a given level piece is a constituent of the group.
        /// </summary>
        /// <param name="piece"></param>
        /// <returns></returns>
        public bool ContainsConstituent(LevelPiece piece)
        {
            return terrainPieces.Exists(ter => ter.HasSameKey(piece));
        }
    }

}
