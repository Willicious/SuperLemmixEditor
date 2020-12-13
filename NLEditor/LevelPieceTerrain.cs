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

            if (this is GroupPiece)
            {
                SpecWidth = 0;
                SpecHeight = 0;
            }
            else
            {
                SpecWidth = Utility.EvaluateResizable(0, DefaultWidth, base.Width, MayResizeHoriz());
                SpecHeight = Utility.EvaluateResizable(0, DefaultHeight, base.Height, MayResizeVert());
            }
        }

        public TerrainPiece(string key, Point pos, int rotation, bool isInvert, bool isErase, bool isNoOv, bool isOneWay, int specWidth, int specHeight)
            : base(key, false, pos, rotation, isInvert)
        {
            IsErase = isErase;
            IsNoOverwrite = isNoOv;
            IsOneWay = isOneWay;
            SpecWidth = Utility.EvaluateResizable(specWidth, DefaultWidth, base.Width, MayResizeHoriz());
            SpecHeight = Utility.EvaluateResizable(specHeight, DefaultHeight, base.Height, MayResizeVert());
        }

        public bool IsErase { get; set; }
        public bool IsNoOverwrite { get; set; }
        public bool IsOneWay { get; set; }
        public bool IsSteel => ObjType == C.OBJ.STEEL;

        public override int Width => Utility.EvaluateResizable(SpecWidth, DefaultWidth, base.Width, MayResizeHoriz());
        public override int Height => Utility.EvaluateResizable(SpecHeight, DefaultHeight, base.Height, MayResizeVert());

        public override LevelPiece Clone()
        {
            return new TerrainPiece(Key, Pos, Rotation, IsInvert, IsErase, IsNoOverwrite, IsOneWay, SpecWidth, SpecHeight);
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
                && this.IsOneWay == piece.IsOneWay
                && this.SpecWidth == piece.SpecWidth
                && this.SpecHeight == piece.SpecHeight;
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

        public override void RotateInRect(Rectangle borderRect)
        {
            base.RotateInRect(borderRect);

            if (MayRotate())
            {
                // Swap special height and special width;
                int oldSpecWidth = SpecWidth;
                SpecWidth = SpecHeight;
                SpecHeight = oldSpecWidth;
            }
        }

        public override Bitmap Image
        {
            get
            {
                Bitmap image = base.Image;

                if (ResizeMode == C.Resize.None)
                {
                    return image;
                }
                else if (Width < 1 || Height < 1)
                {
                    return new Bitmap(1, 1); // should never happen
                }
                else
                {
                    Rectangle? nineSliceArea = ImageLibrary.GetNineSliceArea(Key, GetRotateFlipType());
                    if (nineSliceArea == null)
                    {
                        return image.PaveArea(new Rectangle(0, 0, Width, Height));
                    }
                    else
                    {
                        return image.NineSliceArea(new Rectangle(0, 0, Width, Height), nineSliceArea.Value);
                    }
                }
            }
        }
    }

}
