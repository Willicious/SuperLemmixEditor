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

        public LevelPiece(string key, bool isObj, Point pos,
                          int rotation = 0, bool isInvert = false)
        {
            this.Key = key;
           
            this.Name = System.IO.Path.GetFileName(key);
            this.Style = System.IO.Path.GetDirectoryName(System.IO.Path.GetDirectoryName(key));
            this.PosX = pos.X;
            this.PosY = pos.Y;

            this.Rotation = rotation;
            this.IsInvert = isInvert;
            this.IsSelected = true;

            System.Diagnostics.Debug.Assert(ImageLibrary.CreatePieceKey(Style, Name, isObj) == Key, "Style and name of level piece incompatible with key.");
        }

        public int PosX { get; set; }
        public int PosY { get; set; }
        public Point Pos => new Point(PosX, PosY);
        
        public virtual int Width => (Rotation % 2 == 0) ? ImageLibrary.GetWidth(Key) : ImageLibrary.GetHeight(Key);
        public virtual int Height => (Rotation % 2 == 0) ? ImageLibrary.GetHeight(Key) : ImageLibrary.GetWidth(Key);
        public string Style { get; private set; }
        public string Name { get; private set; }
        protected string Key { get; private set; }

        // RULE: FIRST ROTATE CLOCKWISE - THEN INVERT
        protected int Rotation { get; private set; }
        protected bool IsInvert { get; private set; }

        // For writing the save file
        public bool IsRotatedInPlayer => (Rotation % 2 == 1);
        public bool IsInvertedInPlayer => (IsInvert && Rotation % 4 < 2) || (!IsInvert && Rotation % 4 > 1);
        public bool IsFlippedInPlayer => (Rotation % 4 > 1);

        /// <summary>
        /// Get piece image correctly rotated and flipped.
        /// </summary>
        public virtual Bitmap Image => ImageLibrary.GetImage(Key, GetRotateFlipType(), GetFrameIndex());
        public Rectangle ImageRectangle => new Rectangle(PosX, PosY, Width, Height);
        public C.OBJ ObjType => ImageLibrary.GetObjType(Key);
        protected C.Resize ResizeMode => ImageLibrary.GetResizeMode(Key);
        public bool MayResizeHoriz()
        {
            C.Resize resizeMode = ResizeMode;
            return resizeMode == C.Resize.Both
               || (resizeMode == C.Resize.Horiz && Rotation % 2 == 0)
               || (resizeMode == C.Resize.Vert && Rotation % 2 == 1);
        }
        public bool MayResizeVert()
        {
            C.Resize resizeMode = ResizeMode;
            return resizeMode == C.Resize.Both
               || (resizeMode == C.Resize.Vert && Rotation % 2 == 0)
               || (resizeMode == C.Resize.Horiz && Rotation % 2 == 1);
        }

        public bool IsSelected { get; set; }

        /// <summary>
        /// Returns whether the ImageLibrary can find an image corresponding to this piece.
        /// </summary>
        /// <returns></returns>
        public bool ExistsImage()
        {
            return ImageLibrary.ExistsKey(Key);
        }

        /// <summary>
        /// Moves the piece in the level.
        /// </summary>
        /// <param name="direction"></param>
        /// <param name="step"></param>
        public void Move(C.DIR direction, int step = 1)
        {
            switch (direction)
            {
                case C.DIR.N: PosY = Math.Max(PosY - step, -1000); break;
                case C.DIR.E: PosX = Math.Min(PosX + step, 3400); break;
                case C.DIR.S: PosY = Math.Min(PosY + step, 3400); break;
                case C.DIR.W: PosX = Math.Max(PosX - step, -1000); break;
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
        /// <param name="piece"></param>
        /// <returns></returns>
        public virtual bool Equals(LevelPiece piece)
        {
            return this.PosX == piece.PosX
                && this.PosY == piece.PosY
                && this.Key.Equals(piece.Key)
                && this.Rotation == piece.Rotation
                && this.IsInvert == piece.IsInvert;
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
        public abstract bool MayReceiveSkill(int skill);

        /// <summary>
        /// Rotates the piece around the center of a specified rectangle, if allowed for this piece.
        /// </summary>
        /// <param name="borderRect"></param>
        public virtual void RotateInRect(Rectangle borderRect)
        {
            if (!MayRotate()) return;

            Point center = new Point(borderRect.Left + borderRect.Width / 2, borderRect.Top + borderRect.Height / 2);
            Point oldCorner = new Point(PosX, PosY + Height);

            PosX = center.X + center.Y - oldCorner.Y;
            PosY = center.Y + oldCorner.X - center.X;
            Rotation = (IsInvert ? Rotation + 3 : ++Rotation) % 4;
        }


        /// <summary>
        /// Inverts the piece wrt. a specified rectangle, if allowed for this piece.
        /// </summary>
        /// <param name="borderRect"></param>
        public void InvertInRect(Rectangle borderRect)
        {
            PosY = borderRect.Top + borderRect.Bottom - PosY - Height;
            if (MayInvert()) IsInvert = !IsInvert;
        }

        /// <summary>
        /// Flips the piece wrt. a specified rectangle, if allowed for this piece.
        /// </summary>
        /// <param name="borderRect"></param>
        public void FlipInRect(Rectangle borderRect)
        {
            PosX = borderRect.Left + borderRect.Right - PosX - Width;
            if (MayFlip())
            {
                Rotation = (Rotation + 2) % 4;
                IsInvert = !IsInvert;
            }
        }

        /// <summary>
        /// Translates stored piece data to a RotateFlipType that can be applied to images.
        /// </summary>
        /// <returns></returns>
        private RotateFlipType GetRotateFlipType()
        {
            switch (Rotation)
            { 
                case 0: return IsInvert ? RotateFlipType.RotateNoneFlipY : RotateFlipType.RotateNoneFlipNone;
                case 1: return IsInvert ? RotateFlipType.Rotate90FlipY : RotateFlipType.Rotate90FlipNone;
                case 2: return IsInvert ? RotateFlipType.Rotate180FlipY : RotateFlipType.Rotate180FlipNone;
                case 3: return IsInvert ? RotateFlipType.Rotate270FlipY : RotateFlipType.Rotate270FlipNone;
                default: throw new InvalidOperationException("GetRotateFlipType called with invalid Rotation value " + Rotation.ToString());
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

        public override bool MayReceiveSkill(int skill)
        {
            return false;
        }

    }

    /// <summary>
    /// This stored all data of a gadget. Inherits from LevelPiece.
    /// </summary>
    public class GadgetPiece : LevelPiece
    { 
        public GadgetPiece(string key, Point pos)
            : base(key, true, pos)
        {
            IsNoOverwrite = !(ObjType == C.OBJ.ONE_WAY_WALL);
            IsOnlyOnTerrain = (ObjType == C.OBJ.ONE_WAY_WALL);
            Val_L = 0;
            Val_S = 0;
            SpecWidth = base.Width;
            SpecHeight = base.Height;
        }

        public GadgetPiece(string key, Point pos, int rotation, bool isInvert, bool isNoOverwrite, 
                           bool isOnlyOnTerrain, int valL, int valS, int specWidth = -1, int specHeight = -1)
            : base(key, true, pos, rotation, isInvert)
        {
            IsNoOverwrite = isNoOverwrite;
            IsOnlyOnTerrain = isOnlyOnTerrain;
            Val_L = valL;
            Val_S = valS;
            SpecWidth = (specWidth > 0) ? specWidth : base.Width;
            SpecHeight = (specHeight > 0) ? specHeight : base.Height;
        }

        public bool IsNoOverwrite { get; set; }
        public bool IsOnlyOnTerrain { get; set; }
        public int Val_L { get; private set; }
        public int Val_S { get; private set; }
        public int SpecWidth { get; set; }
        public int SpecHeight { get; set; }

        public override LevelPiece Clone()
        {
            return new GadgetPiece(Key, Pos, Rotation, IsInvert, IsNoOverwrite, IsOnlyOnTerrain, 
                                   Val_L, Val_S, SpecWidth, SpecHeight);
        }


        /// <summary>
        /// Compares two GadgetPieces for equality.
        /// </summary>
        /// <param name="piece"></param>
        /// <returns></returns>
        public bool Equals(GadgetPiece piece)
        {
            return base.Equals(piece)
                && this.IsNoOverwrite == piece.IsNoOverwrite
                && this.IsOnlyOnTerrain == piece.IsOnlyOnTerrain
                && this.Val_L == piece.Val_L
                && this.Val_S == piece.Val_S
                && this.SpecWidth == piece.SpecWidth
                && this.SpecHeight == piece.SpecHeight;
        }

        /// <summary>
        /// Returns the position of the trigger area.
        /// <para> It does NOT adapt to rotation! </para>
        /// </summary>
        public Rectangle TriggerRect { get 
        {
            Rectangle trigRect = ImageLibrary.GetTrigger(Key);
            // Adjust to resizing
            if (ResizeMode != C.Resize.None)
            {
                trigRect.Width += SpecWidth - base.Width;
                trigRect.Height += SpecHeight - base.Height;
            }

            // Adjust to flipping
            if (IsFlippedInPlayer && !IsInvertedInPlayer && !IsRotatedInPlayer)
            {
                trigRect.X = ImageRectangle.Width - trigRect.Right;
            }

            // Shift to position relative to level
            trigRect.X += PosX;
            trigRect.Y += PosY;
            return trigRect;
        } }

        public override Bitmap Image { get
        {
            if (ResizeMode == C.Resize.None) return base.Image;
            else if (SpecWidth < 1 || SpecHeight < 1) return new Bitmap(1, 1); // should never happen
            else return base.Image.PaveArea(new Rectangle(0, 0, Width, Height));
        } }

        public override int Width => (ResizeMode == C.Resize.None) ? base.Width : SpecWidth;
        public override int Height => (ResizeMode == C.Resize.None) ? base.Height : SpecHeight;

        /// <summary>
        /// Returns the correct frame to load the image.
        /// </summary>
        /// <returns></returns>
        protected override int GetFrameIndex()
        {
            if (ObjType == C.OBJ.PICKUP)
            {
                // Return the index of the skill + 1 or return 0 if no skill is selected
                int skillNum = C.SKI_COUNT;
                while (!HasSkillFlag(skillNum) && skillNum >= 0)
                {
                    skillNum--;
                }

                return ++skillNum;
            }
            else if (ObjType.In(C.OBJ.EXIT_LOCKED, C.OBJ.BUTTON, C.OBJ.TRAPONCE))
            {
                return 1;
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

        public override bool MayReceiveSkill(int skill)
        {
            switch (ObjType)
            {
                case C.OBJ.HATCH:
                    {
                        return skill.In(C.SKI_CLIMBER, C.SKI_FLOATER, C.SKI_GLIDER, C.SKI_DISARMER,
                                        C.SKI_SWIMMER, C.SKI_ZOMBIE); 
                    }
                case C.OBJ.LEMMING:
                    {
                        return skill.In(C.SKI_CLIMBER, C.SKI_FLOATER, C.SKI_GLIDER, C.SKI_DISARMER,
                                        C.SKI_SWIMMER, C.SKI_ZOMBIE, C.SKI_BLOCKER);
                    }
                case C.OBJ.PICKUP:
                    {
                        return skill != C.SKI_ZOMBIE;
                    }
                default: return false; 
            }
        }

        /// <summary>
        /// Rotates the piece around the center of a specified rectangle, if allowed for this piece.
        /// </summary>
        /// <param name="borderRect"></param>
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


        /// <summary>
        /// Adjusts the flag for the specified skill, depending on the object type.
        /// </summary>
        /// <param name="skill"></param>
        /// <param name="doAdd"></param>
        public void SetSkillFlag(int skill, bool doAdd)
        {
            if (!MayReceiveSkill(skill)) return;

            switch (ObjType)
            {
                case C.OBJ.HATCH:
                case C.OBJ.LEMMING:
                    {
                        if (skill == C.SKI_FLOATER)
                        {
                            SetOneSkillFlag(C.SKI_GLIDER, false);
                        }
                        else if (skill == C.SKI_GLIDER)
                        {
                            SetOneSkillFlag(C.SKI_FLOATER, false);
                        }
                        
                        SetOneSkillFlag(skill, doAdd);
                        break;
                    }
                case C.OBJ.PICKUP:
                    {
                        for (int CurSkill = 0; CurSkill < C.SKI_COUNT; CurSkill++)
                        {
                            SetOneSkillFlag(CurSkill, false);
                        }
                        SetOneSkillFlag(skill, doAdd);
                        break;
                    }
            }
        }

        /// <summary>
        /// Changes the skill flag of this object.
        /// </summary>
        /// <param name="skill"></param>
        /// <param name="doAdd"></param>
        private void SetOneSkillFlag(int skill, bool doAdd)
        {
            Val_L |= 1 << skill; // always true now
            if (!doAdd) Val_L ^= 1 << skill; // always false now
        }

        /// <summary>
        /// Returns whether the object has the flag for a specific skill.
        /// </summary>
        /// <param name="skill"></param>
        /// <returns></returns>
        public bool HasSkillFlag(int skill)
        { 
            return (Val_L & 1 << skill) != 0;
        }

        /// <summary>
        /// Sets the width of resizable objects taking rotation into account.
        /// </summary>
        /// <param name="newWidth"></param>
        public void SetSpecWidth(int newWidth)
        {
            if (MayResizeHoriz()) SpecWidth = Math.Max(newWidth, 1);
        }

        /// <summary>
        /// Sets the height of resizable objects taking rotation into account.
        /// </summary>
        /// <param name="newHeight"></param>
        public void SetSpecHeight(int newHeight)
        {
            if (MayResizeVert()) SpecHeight = Math.Max(newHeight, 1);
        }

        /// <summary>
        /// Sets the key-value for pairing teleporters to receivers.
        /// </summary>
        /// <param name="newValue"></param>
        public void SetTeleporterValue(int newValue)
        {
            System.Diagnostics.Debug.Assert(ObjType.In(C.OBJ.TELEPORTER, C.OBJ.RECEIVER), "Teleporter pairing key set for object, that is neither teleporter nor receiver.");            
            Val_L = newValue;
        }

    }

    /// <summary>
    /// This stored all data of a gadget. Inherits from LevelPiece.
    /// </summary>
    public class GroupPiece : TerrainPiece
    { 
        public GroupPiece(GroupPiece oldGroupPiece, Point pos)
            : base(oldGroupPiece.Key, pos)
        {
            fTerPieceList = oldGroupPiece.fTerPieceList;
        }

        public GroupPiece(GroupPiece oldGroupPiece, Point pos, int rotation, bool isInvert, bool isErase, bool isNoOverwrite, bool isOneWay)
            : base(oldGroupPiece.Key, pos, rotation, isInvert, isErase, isNoOverwrite, isOneWay)
        {
            fTerPieceList = oldGroupPiece.fTerPieceList;
        }

        public GroupPiece(List<TerrainPiece> terPieceList)
            : base(GetKeyFromTerPieceList(terPieceList), GetPosFromTerPieceList(terPieceList))
        {
            fTerPieceList = terPieceList.ConvertAll(ter => (TerrainPiece)ter.Clone()).ToList();
            fTerPieceList.ForEach(ter => { ter.PosX -= this.PosX; ter.PosY -= this.PosY; ter.IsSelected = false; });
            // Add the group image to the image library
            Renderer groupRenderer = new Renderer();
            Bitmap groupImage = groupRenderer.CreateTerrainGroupImage(fTerPieceList);
            ImageLibrary.AddNewImage(Key, groupImage, C.OBJ.TERRAIN, new Rectangle(), C.Resize.None);
        }

        List<TerrainPiece> fTerPieceList; // already with adapted positions

        /// <summary>
        /// Creates the group key from the terrain list.
        /// </summary>
        /// <param name="terPieceList"></param>
        /// <returns></returns>
        private static string GetKeyFromTerPieceList(List<TerrainPiece> terPieceList)
        {
            Point groupPos = GetPosFromTerPieceList(terPieceList);
            
            StringBuilder keyString = new StringBuilder();
            foreach(TerrainPiece piece in terPieceList)
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
        /// Gets the position of the group from a raw terrain list.
        /// </summary>
        /// <param name="terPieceList"></param>
        /// <returns></returns>
        private static Point GetPosFromTerPieceList(List<TerrainPiece> terPieceList)
        {
            int minXPos = terPieceList.Min(ter => ter.PosX);
            int minYPos = terPieceList.Min(ter => ter.PosY);
            return new Point(minXPos, minYPos);
        }

        public override LevelPiece Clone()
        {
            return new GroupPiece(this, new Point(this.PosX, this.PosY), this.Rotation,
                                  this.IsInvert, this.IsErase, this.IsNoOverwrite, this.IsOneWay);
        }


    }
}
