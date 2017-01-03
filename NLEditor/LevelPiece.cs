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

        // RULE: FIRST ROTATE CLOCKWISE - THEN INVERT
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

        // For cloning the piece
        protected int Rotation { get { return fRotation; } }
        protected bool IsInvert { get { return fInvert; } }

        // For writing the save file
        public bool IsRotatedInPlayer { get { return (fRotation % 2 == 1); } }
        public bool IsInvertedInPlayer { get { return (fInvert && fRotation % 4 < 2) || (!fInvert && fRotation % 4 > 1); } }
        public bool IsFlippedInPlayer { get { return (fRotation % 4 > 1); } }

        /// <summary>
        /// Get piece image correctly rotated and flipped.
        /// </summary>
        public virtual Bitmap Image { get { return ImageLibrary.GetImage(fKey, GetRotateFlipType(), GetFrameIndex()); } }

        public C.OBJ ObjType { get { return ImageLibrary.GetObjType(fKey); } }
        public C.Resize ResizeMode { get { return ImageLibrary.GetResizeMode(fKey); } }

        public bool IsSelected { get { return fIsSelected; } set { fIsSelected = value; } }

        public Rectangle ImageRectangle { get { return new Rectangle(fPos.X, fPos.Y, Width, Height); } }

        /// <summary>
        /// Returns whether the ImageLibrary can find an image corresponding to this piece.
        /// </summary>
        /// <returns></returns>
        public bool ExistsImage()
        {
            return ImageLibrary.ExistsKey(fKey);
        }

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
        /// Creates a deep copy of the piece.
        /// </summary>
        /// <returns></returns>
        public abstract LevelPiece Clone();

        /// <summary>
        /// Compares two LevelPieces for equality.
        /// </summary>
        /// <param name="Piece"></param>
        /// <returns></returns>
        public virtual bool Equals(LevelPiece Piece)
        {
            return this.PosX == Piece.PosX
                && this.PosY == Piece.PosY
                && this.fKey.Equals(Piece.fKey)
                && this.Rotation == Piece.Rotation
                && this.IsInvert == Piece.IsInvert;
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
        /// Determines whether this piece can receive a flag for a given skill.
        /// </summary>
        /// <returns></returns>
        public abstract bool MayReceiveSkill(int Skill);

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

        /// <summary>
        /// Translates stored piece data to a RotateFlipType that can be applied to images.
        /// </summary>
        /// <returns></returns>
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

        /// <summary>
        /// Returns the correct frame to load the image.
        /// </summary>
        /// <returns></returns>
        protected virtual int GetFrameIndex()
        {
            return 0;
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
            fIsErase = IsErase;
            fIsNoOverwrite = IsNoOv;
            fIsOneWay = IsOneWay;
        }

        bool fIsErase;
        bool fIsNoOverwrite;
        bool fIsOneWay;

        public bool IsErase { get { return fIsErase; } set { fIsErase = value; } }
        public bool IsNoOverwrite { get { return fIsNoOverwrite; } set { fIsNoOverwrite = value; } }
        public bool IsOneWay { get { return fIsOneWay; } set { fIsOneWay = value; } }
        public bool IsSteel { get { return this.ObjType == C.OBJ.STEEL; } }

        public override LevelPiece Clone()
        {
            return new TerrainPiece(this.fKey, new Point(this.PosX, this.PosY), this.Rotation, 
                                    this.IsInvert, this.IsErase, this.IsNoOverwrite, this.IsOneWay);
        }

        /// <summary>
        /// Compares two TerrainPieces for equality.
        /// </summary>
        /// <param name="Piece"></param>
        /// <returns></returns>
        public bool Equals(TerrainPiece Piece)
        {
            return base.Equals(Piece)
                && this.IsErase == Piece.IsErase
                && this.IsNoOverwrite == Piece.IsNoOverwrite
                && this.IsOneWay == Piece.IsOneWay;
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

        public override bool MayReceiveSkill(int Skill)
        {
            return false;
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
            fIsNoOverwrite = !(this.ObjType == C.OBJ.ONE_WAY_WALL);
            fIsOnlyOnTerrain = (this.ObjType == C.OBJ.ONE_WAY_WALL);
            fVal_L = 0;
            fVal_S = 0;
            fSpecWidth = base.Width;
            fSpecHeight = base.Height;
        }

        public GadgetPiece(string Key, Point Pos, int Rotation, bool IsInvert, bool IsNoOv, 
                           bool IsOnlyOnTerr, int valL, int valS, int SpecWidth = -1, int SpecHeight = -1)
            : base(Key, true, Pos, Rotation, IsInvert)
        {
            fIsNoOverwrite = IsNoOv;
            fIsOnlyOnTerrain = IsOnlyOnTerr;
            fVal_L = valL;
            fVal_S = valS;
            fSpecWidth = (SpecWidth > 0) ? SpecWidth : base.Width;
            fSpecHeight = (SpecHeight > 0) ? SpecHeight : base.Height;
        }

        bool fIsNoOverwrite;
        bool fIsOnlyOnTerrain;
        int fVal_L;
        int fVal_S;
        int fSpecWidth;
        int fSpecHeight;

        public bool IsNoOverwrite { get { return fIsNoOverwrite; } set { fIsNoOverwrite = value; } }
        public bool IsOnlyOnTerrain { get { return fIsOnlyOnTerrain; } set { fIsOnlyOnTerrain = value; } }
        public int Val_L { get { return fVal_L; } }
        public int Val_S { get { return fVal_S; } }
        public int SpecWidth { get { return (Rotation % 2 == 0) ? fSpecWidth : fSpecHeight; } }
        public int SpecHeight { get { return (Rotation % 2 == 0) ? fSpecHeight : fSpecWidth; } }

        public override LevelPiece Clone()
        {
            return new GadgetPiece(this.fKey, new Point(this.PosX, this.PosY), this.Rotation, 
                                   this.IsInvert, this.IsNoOverwrite, this.IsOnlyOnTerrain, 
                                   this.Val_L, this.Val_S, this.SpecWidth, this.SpecHeight);
        }


        /// <summary>
        /// Compares two GadgetPieces for equality.
        /// </summary>
        /// <param name="Piece"></param>
        /// <returns></returns>
        public bool Equals(GadgetPiece Piece)
        {
            return base.Equals(Piece)
                && this.IsNoOverwrite == Piece.IsNoOverwrite
                && this.IsOnlyOnTerrain == Piece.IsOnlyOnTerrain
                && this.Val_L == Piece.Val_L
                && this.Val_S == Piece.Val_S
                && this.SpecWidth == Piece.SpecWidth
                && this.SpecHeight == Piece.SpecHeight;
        }

        /// <summary>
        /// Returns the position of the trigger area.
        /// </summary>
        public Rectangle TriggerRect { get 
        {
            Rectangle TrigRect = ImageLibrary.GetTrigger(fKey);
            // Adjust to resizing
            if (ResizeMode.In(C.Resize.Horiz, C.Resize.Both))
            {
                TrigRect.Width += this.Width - base.Width;
            }
            if (ResizeMode.In(C.Resize.Vert, C.Resize.Both))
            {
                TrigRect.Height += this.Height - base.Height;
            }

            // Adjust to flipping
            if (IsFlippedInPlayer && !IsInvertedInPlayer && !IsRotatedInPlayer)
            {
                TrigRect.X = this.ImageRectangle.Width - TrigRect.Right;
            }
            // Shift to position relative to level
            TrigRect.X += this.PosX;
            TrigRect.Y += this.PosY;
            return TrigRect;
        } }

        public override Bitmap Image { get
        {
            if (ResizeMode == C.Resize.None) return base.Image;
            else if (SpecWidth < 1 || SpecHeight < 1) return new Bitmap(1, 1); // should never happen
            else return base.Image.PaveArea(new Rectangle(0, 0, Width, Height));
        } }

        public override int Width { get
        {
            if (ResizeMode == C.Resize.None) return base.Width;
            else return (Rotation % 2 == 0) ? fSpecWidth : fSpecHeight;
        } }

        public override int Height { get
        {
            if (ResizeMode == C.Resize.None) return base.Height;
            else return (Rotation % 2 == 0) ? fSpecHeight : fSpecWidth; 
        } }

        /// <summary>
        /// Returns the correct frame to load the image.
        /// </summary>
        /// <returns></returns>
        protected override int GetFrameIndex()
        {
            if (ObjType == C.OBJ.PICKUP)
            {
                // Return the index of the skill + 1 or return 0 if no skill is selected
                int SkillNum = C.SKI_COUNT;
                while (!HasSkillFlag(SkillNum) && SkillNum >= 0)
                {
                    SkillNum--;
                }

                return ++SkillNum;
            }
            else return base.GetFrameIndex();
        }


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

        public override bool MayReceiveSkill(int Skill)
        {
            switch (ObjType)
            {
                case C.OBJ.HATCH:
                    {
                        return Skill.In(C.SKI_CLIMBER, C.SKI_FLOATER, C.SKI_GLIDER, C.SKI_DISARMER,
                                        C.SKI_SWIMMER, C.SKI_ZOMBIE); 
                    }
                case C.OBJ.LEMMING:
                    {
                        return Skill.In(C.SKI_CLIMBER, C.SKI_FLOATER, C.SKI_GLIDER, C.SKI_DISARMER,
                                        C.SKI_SWIMMER, C.SKI_ZOMBIE, C.SKI_BLOCKER);
                    }
                case C.OBJ.PICKUP:
                    {
                        return Skill != C.SKI_ZOMBIE;
                    }
                default: return false; 
            }
        }

        /// <summary>
        /// Adjusts the flag for the specified skill, depending on the object type.
        /// </summary>
        /// <param name="Skill"></param>
        /// <param name="DoAdd"></param>
        public void SetSkillFlag(int Skill, bool DoAdd)
        {
            if (!MayReceiveSkill(Skill)) return;

            switch (ObjType)
            {
                case C.OBJ.HATCH:
                case C.OBJ.LEMMING:
                    {
                        if (Skill == C.SKI_FLOATER)
                        {
                            SetOneSkillFlag(C.SKI_GLIDER, false);
                        }
                        else if (Skill == C.SKI_GLIDER)
                        {
                            SetOneSkillFlag(C.SKI_FLOATER, false);
                        }
                        
                        SetOneSkillFlag(Skill, DoAdd);
                        break;
                    }
                case C.OBJ.PICKUP:
                    {
                        for (int CurSkill = 0; CurSkill < C.SKI_COUNT; CurSkill++)
                        {
                            SetOneSkillFlag(CurSkill, false);
                        }
                        SetOneSkillFlag(Skill, DoAdd);
                        break;
                    }
            }
        }

        /// <summary>
        /// Changes the skill flag of this object.
        /// </summary>
        /// <param name="Skill"></param>
        /// <param name="DoAdd"></param>
        private void SetOneSkillFlag(int Skill, bool DoAdd)
        {
            fVal_L |= 1 << Skill; // always true now
            if (!DoAdd) fVal_L ^= 1 << Skill; // always false now
        }

        /// <summary>
        /// Returns whether the object has the flag for a specific skill.
        /// </summary>
        /// <param name="Skill"></param>
        /// <returns></returns>
        public bool HasSkillFlag(int Skill)
        { 
            return (fVal_L & 1 << Skill) != 0;
        }

        /// <summary>
        /// Sets the width of resizable objects.
        /// </summary>
        /// <param name="NewWidth"></param>
        public void SetSpecWidth(int NewWidth)
        {
            if (ResizeMode.In(C.Resize.Horiz, C.Resize.Both))
            {
                if (Rotation % 2 == 0) fSpecWidth = Math.Max(NewWidth, 1);
                else fSpecHeight = Math.Max(NewWidth, 1);
            }
        }

        /// <summary>
        /// Sets the height of resizable objects.
        /// </summary>
        /// <param name="NewHeight"></param>
        public void SetSpecHeight(int NewHeight)
        {
            if (ResizeMode.In(C.Resize.Vert, C.Resize.Both))
            {
                if (Rotation % 2 == 0) fSpecHeight = Math.Max(NewHeight, 1);
                else fSpecWidth = Math.Max(NewHeight, 1);
            }
        }

        /// <summary>
        /// Sets the key-value for pairing teleporters to receivers.
        /// </summary>
        /// <param name="NewValue"></param>
        public void SetTeleporterValue(int NewValue)
        {
            System.Diagnostics.Debug.Assert(this.ObjType.In(C.OBJ.TELEPORTER, C.OBJ.RECEIVER), "Teleporter pairing key set for object, that is neither teleporter nor receiver.");            
            fVal_L = NewValue;
        }

    }

}
