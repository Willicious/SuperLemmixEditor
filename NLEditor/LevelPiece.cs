using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace NLEditor
{
    /// <summary>
    /// Abstract class for all pieces.
    /// </summary>
    public abstract class LevelPiece
    {
        /*---------------------------------------------------------
         *          This class stores infos about pieces
         * -------------------------------------------------------- */

        public LevelPiece(string Key, bool IsObj, Point Pos,
                          int Rotation = 0, bool IsInvert = false)
        {
            this.fKey = Key;
           
            this.fName = System.IO.Path.GetFileName(Key);
            this.fStyle = System.IO.Path.GetDirectoryName(System.IO.Path.GetDirectoryName(Key));
            this.Pos = Pos;

            this.fRotation = Rotation;
            this.fInvert = IsInvert;
            this.fIsSelected = true;

            System.Diagnostics.Debug.Assert(ImageLibrary.CreatePieceKey(this.fStyle, this.fName, IsObj) == this.fKey, "Style and name of level piece incompatible with key.");
        }

        Point fPos;
        string fStyle;
        string fName;
        protected string fKey;

        // RULE: FIRST INVERT - THEN ROTATE CLOCKWISE
        int fRotation;
        bool fInvert;

        bool fIsSelected;

        public Point Pos { get { return fPos; } set { fPos = value; } }
        public int PosX { get { return fPos.X; } set { fPos.X = value; } }
        public int PosY { get { return fPos.Y; } set { fPos.Y = value; } }
        public virtual int Width { get { return (fRotation % 2 == 0) ? ImageLibrary.GetWidth(fKey) : ImageLibrary.GetHeight(fKey); } }
        public virtual int Height { get { return (fRotation % 2 == 0) ? ImageLibrary.GetHeight(fKey) : ImageLibrary.GetWidth(fKey); } }
        public string Style { get { return fStyle; } }
        public string Name { get { return fName; } }

        // For writing the save file
        public bool IsRotatedInPlayer { get { return (fRotation % 2 == 1); } }
        public bool IsInvertedInPlayer { get { return (fInvert && fRotation % 4 < 2) || (!fInvert && fRotation % 4 > 1); } }
        public bool IsFlippedInPlayer { get { return (fRotation % 4 > 1); } }

        // Metainfo from BaseImageInfo
        /// <summary>
        /// Get piece image correctly rotated and flipped.
        /// </summary>
        public Bitmap Image { get 
        {
            Bitmap MyImage = ImageLibrary.GetImage(fKey);
            MyImage.RotateFlip(GetRotateFlipType());
            return MyImage;
        } }
        public C.OBJ ObjType { get { return ImageLibrary.GetObjType(fKey); } }

        // Whether selected in the editor
        public bool IsSelected { get { return fIsSelected; } set { fIsSelected = value; } }

        public Rectangle ImageRectangle { get 
        {
            int ImageWidth;
            int ImageHeight;

            if (fRotation % 2 == 0)
            {
                ImageWidth = ImageLibrary.GetWidth(fKey);
                ImageHeight = ImageLibrary.GetHeight(fKey);
            }
            else
            {
                ImageWidth = ImageLibrary.GetHeight(fKey);
                ImageHeight = ImageLibrary.GetWidth(fKey); 
            }

            return new Rectangle(fPos.X, fPos.Y, ImageWidth, ImageHeight);
        } }

        /// <summary>
        /// Moves the piece in the level.
        /// </summary>
        /// <param name="Direction"></param>
        /// <param name="Step"></param>
        public void Move(C.DIR Direction, int Step = 1)
        {
            switch (Direction)
            {
                case C.DIR.N: PosY = Math.Max(PosY - Step, -1000); break;
                case C.DIR.E: PosX = Math.Min(PosX + Step, 3400); break;
                case C.DIR.S: PosY = Math.Min(PosY + Step, 3400); break;
                case C.DIR.W: PosX = Math.Max(PosX - Step, -1000); break;
            }
        }

        /// <summary>
        /// Determines whether this piece can be rotated.
        /// </summary>
        /// <returns></returns>
        public abstract bool MayRotate();

        /// <summary>
        /// Determines whether this piece can be flipped.
        /// </summary>
        /// <returns></returns>
        public abstract bool MayFlip();

        /// <summary>
        /// Determines whether this piece can be inverted.
        /// </summary>
        /// <returns></returns>
        public abstract bool MayInvert();

        /// <summary>
        /// Rotates the piece while keeping its top left coordianate.
        /// </summary>
        private void Rotate()
        {
            fRotation = (fInvert ? fRotation + 3 : ++fRotation) % 4;
        }

        /// <summary>
        /// Rotates the piece around the center of a specified rectangle, if allowed for this piece.
        /// </summary>
        /// <param name="BorderRect"></param>
        public void RotateInRect(Rectangle BorderRect)
        {
            Point Center = new Point(BorderRect.Left + BorderRect.Width / 2, BorderRect.Top + BorderRect.Height / 2);
            Point OldCorner = new Point(PosX, PosY + Height);

            int NewPosX = Center.X + Center.Y - OldCorner.Y;
            int NewPosY = Center.Y + OldCorner.X - Center.X;

            Pos = new Point(NewPosX, NewPosY);

            if (MayRotate()) Rotate();
        }

        /// <summary>
        /// Inverts the piece while keeping its top left coordinate.
        /// </summary>
        private void Invert()
        {
            fInvert = !fInvert;
        }

        /// <summary>
        /// Inverts the piece wrt. a specified rectangle, if allowed for this piece.
        /// </summary>
        /// <param name="BorderRect"></param>
        public void InvertInRect(Rectangle BorderRect)
        {
            PosY = BorderRect.Top + BorderRect.Bottom - PosY - Height;
            if (MayInvert()) Invert();
        }

        /// <summary>
        /// Flips the piece while keeping its top left coordinate.
        /// </summary>
        private void Flip() // = Invert + Rotate^2
        {
            fRotation = (fRotation + 2) % 4;
            fInvert = !fInvert;
        }

        /// <summary>
        /// Flips the piece wrt. a specified rectangle, if allowed for this piece.
        /// </summary>
        /// <param name="BorderRect"></param>
        public void FlipInRect(Rectangle BorderRect)
        {
            PosX = BorderRect.Left + BorderRect.Right - PosX - Width;
            if (MayFlip()) Flip();
        }


        private RotateFlipType GetRotateFlipType()
        {
            switch (fRotation)
            { 
                case 0: return fInvert ? RotateFlipType.RotateNoneFlipY : RotateFlipType.RotateNoneFlipNone;
                case 1: return fInvert ? RotateFlipType.Rotate90FlipY : RotateFlipType.Rotate90FlipNone;
                case 2: return fInvert ? RotateFlipType.Rotate180FlipY : RotateFlipType.Rotate180FlipNone;
                case 3: return fInvert ? RotateFlipType.Rotate270FlipY : RotateFlipType.Rotate270FlipNone;
                default: return RotateFlipType.RotateNoneFlipNone;
            }
        }

    }

    /// <summary>
    /// This stored all data of a terrain piece. Inherits from LevelPiece.
    /// </summary>
    public class TerrainPiece : LevelPiece
    {
        /*---------------------------------------------------------
         *      This class stores infos about terrain pieces
         * -------------------------------------------------------- */

        public TerrainPiece(string Key, Point Pos)
            : base(Key, false, Pos)
        {
            fIsErase = false;
            fIsNoOverwrite = false;
            fIsOneWay = true;
        }

        public TerrainPiece(string Key, Point Pos, int Rotation, bool IsInvert, bool IsErase, bool IsNoOv, bool IsOneWay)
            : base(Key, false, Pos, Rotation, IsInvert)
        {
            fIsErase = IsInvert;
            fIsNoOverwrite = IsNoOv;
            fIsOneWay = IsOneWay;
        }

        bool fIsErase;
        bool fIsNoOverwrite;
        bool fIsOneWay;

        public bool IsErase { get { return fIsErase; } set { fIsErase = value; } }
        public bool IsNoOverwrite { get { return fIsNoOverwrite; } set { fIsNoOverwrite = value; } }
        public bool IsOneWay { get { return fIsOneWay; } set { fIsOneWay = value; } }

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
    }

    /// <summary>
    /// This stored all data of a gadget. Inherits from LevelPiece.
    /// </summary>
    public class GadgetPiece : LevelPiece
    { 
        public GadgetPiece(string Key, Point Pos)
            : base(Key, true, Pos)
        {
            fIsNoOverwrite = !this.ObjType.In(C.OBJ.OWW_LEFT, C.OBJ.OWW_RIGHT, C.OBJ.OWW_DOWN);
            fIsOnlyOnTerrain = this.ObjType.In(C.OBJ.OWW_LEFT, C.OBJ.OWW_RIGHT, C.OBJ.OWW_DOWN);
        }

        public GadgetPiece(string Key, Point Pos, int Rotation, bool IsInvert, bool IsNoOv, 
                           bool IsOnlyOnTerr, int valL, int valS)
            : base(Key, true, Pos, Rotation, IsInvert)
        {
            fIsNoOverwrite = IsNoOv;
            fIsOnlyOnTerrain = IsOnlyOnTerr;
            fVal_L = valL;
            fVal_S = valS;
        }

        bool fIsNoOverwrite;
        bool fIsOnlyOnTerrain;
        int fVal_L;
        int fVal_S;
        int fSpecWidth;
        int fSpecHeight;

        public bool IsNoOverwrite { get { return fIsNoOverwrite; } set { fIsNoOverwrite = value; } }
        public bool IsOnlyOnTerrain { get { return fIsOnlyOnTerrain; } set { fIsOnlyOnTerrain = value; } }
        public int Val_L { get { return fVal_L; } set { fVal_L = value; } }
        public int Val_S { get { return fVal_S; } set { fVal_S = value; } }
        public int SpecWidth { get { return fSpecWidth; } set { fSpecWidth = value; } }
        public int SpecHeight { get { return fSpecHeight; } set { fSpecHeight = value; } }

        /// <summary>
        /// Returns the position of the trigger area.
        /// </summary>
        public Rectangle TriggerRect { get 
        {
            Rectangle TrigRect = ImageLibrary.GetTrigger(fKey);
            // Adjust to flipping
            if (IsFlippedInPlayer && !IsInvertedInPlayer && !IsRotatedInPlayer)
            {
                TrigRect.X = this.ImageRectangle.Width - TrigRect.Right;
            }
            // Shift to correct position
            TrigRect.X += this.PosX;
            TrigRect.Y += this.PosY;
            return TrigRect;
        } }

        public override bool MayRotate()
        {
            return ObjType.In(C.OBJ.BACKGROUND, C.OBJ.NONE);
        }

        public override bool MayFlip()
        {
            return ObjType.In(C.OBJ.ANIMATION, C.OBJ.BACKGROUND, C.OBJ.FIRE, C.OBJ.HATCH, C.OBJ.LEMMING,
                              C.OBJ.NONE, C.OBJ.NOSPLAT, C.OBJ.RADIATION, C.OBJ.RECEIVER, C.OBJ.SLOWFREEZE,
                              C.OBJ.SPLAT, C.OBJ.SPLITTER, C.OBJ.TELEPORTER, C.OBJ.TRAP, C.OBJ.TRAPONCE);
        }

        public override bool MayInvert()
        {
            return ObjType.In(C.OBJ.BACKGROUND, C.OBJ.NONE);
        }

    }

}
