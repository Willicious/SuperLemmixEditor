using System;
using System.Drawing;

namespace NLEditor
{
    /// <summary>
    /// This stored all data of a terrain piece. Inherits from LevelPiece.
    /// </summary>
    [Serializable]
    class TerrainPiece : LevelPiece
    {
        public TerrainPiece(string key, Point pos)
            : base(key, false, pos)
        {
            IsErase = false;
            IsNoOverwrite = false;
            IsOneWay = true;
        }

        public TerrainPiece(string key, Point pos, int rotation, bool isInvert, bool isErase, bool isNoOv, bool isOneWay)
            : base(key, false, pos, rotation, isInvert)
        {
            IsErase = isErase;
            IsNoOverwrite = isNoOv;
            IsOneWay = isOneWay;
        }

        public bool IsErase { get; set; }
        public bool IsNoOverwrite { get; set; }
        public bool IsOneWay { get; set; }
        public bool IsSteel => ObjType == C.OBJ.STEEL;

        public override LevelPiece Clone()
        {
            return new TerrainPiece(Key, Pos, Rotation, IsInvert, IsErase, IsNoOverwrite, IsOneWay);
        }

        /// <summary>
        /// Compares two TerrainPieces for equality.
        /// </summary>
        /// <param name="piece"></param>
        /// <returns></returns>
        public bool Equals(TerrainPiece piece)
        {
            return base.Equals(piece)
                && this.IsErase == piece.IsErase
                && this.IsNoOverwrite == piece.IsNoOverwrite
                && this.IsOneWay == piece.IsOneWay;
        }


        public override bool MayRotate()
        {
            return true;
        }

        public override bool MayFlip()
        {
            return true;
        }

        public override bool MayInvert()
        {
            return true;
        }

        public override bool MayReceiveSkill(C.Skill skill)
        {
            return false;
        }

    }

}
