using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace NLEditor
{
  /// <summary>
  /// Abstract class for all pieces.
  /// </summary>
  [Serializable]
  abstract class LevelPiece
  {
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
    public string Key { get; private set; }

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
    /// Moves the piece in the level some pixels in a given direction.
    /// </summary>
    /// <param name="direction"></param>
    /// <param name="step"></param>
    public void Move(C.DIR direction, int step, int gridSize)
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
    /// Moves the piece in the level to the target position and rounds it to the grid.
    /// </summary>
    /// <param name="targetPos"></param>
    public void Move(Point targetPos, int gridSize)
    {
      PosX = targetPos.X.RoundToMultiple(gridSize);
      PosY = targetPos.Y.RoundToMultiple(gridSize);
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

    public bool HasSameKey(LevelPiece piece)
    {
      return this.Key.Equals(piece.Key);
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
    public abstract bool MayReceiveSkill(C.Skill skill);

    /// <summary>
    /// Rotates the piece around the center of a specified rectangle, if allowed for this piece.
    /// </summary>
    /// <param name="borderRect"></param>
    public virtual void RotateInRect(Rectangle borderRect)
    {
      if (!MayRotate()) return;

      Rectangle newPieceRect = new Rectangle(PosX, PosY, Width, Height).RotateInRectangle(borderRect);
      PosX = newPieceRect.Left;
      PosY = newPieceRect.Top;
      Rotation = (IsInvert ? Rotation + 3 : ++Rotation) % 4;
    }


    /// <summary>
    /// Inverts the piece wrt. a specified rectangle, if allowed for this piece.
    /// </summary>
    /// <param name="borderRect"></param>
    public virtual void InvertInRect(Rectangle borderRect)
    {
      PosY = borderRect.Top + borderRect.Bottom - PosY - Height;
      if (MayInvert()) IsInvert = !IsInvert;
    }

    /// <summary>
    /// Flips the piece wrt. a specified rectangle, if allowed for this piece.
    /// </summary>
    /// <param name="borderRect"></param>
    public virtual void FlipInRect(Rectangle borderRect)
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
    protected RotateFlipType GetRotateFlipType()
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

  /// <summary>
  /// This stored all data of a gadget. Inherits from LevelPiece.
  /// </summary>
  [Serializable]
  class GadgetPiece : LevelPiece
  {
    public GadgetPiece(string key, Point pos)
        : base(key, true, pos)
    {
      IsNoOverwrite = !ObjType.In(C.OBJ.ONE_WAY_WALL, C.OBJ.LEMMING);
      IsOnlyOnTerrain = (ObjType == C.OBJ.ONE_WAY_WALL);
      Val_L = 0;
      SkillFlags = new HashSet<C.Skill>();
      SpecWidth = base.Width;
      SpecHeight = base.Height;
    }

    public GadgetPiece(string key, Point pos, int rotation, bool isInvert, bool isNoOverwrite,
                       bool isOnlyOnTerrain, int valL, HashSet<C.Skill> skillFlags,
                       int specWidth = -1, int specHeight = -1,
                       int bgSpeed = 0, int bgAngle = 0, int lemmingCap = 0)
        : base(key, true, pos, rotation, isInvert)
    {
      IsNoOverwrite = isNoOverwrite;
      IsOnlyOnTerrain = isOnlyOnTerrain;
      Val_L = valL;
      SkillFlags = new HashSet<C.Skill>(skillFlags);
      SpecWidth = (specWidth > 0) ? specWidth : base.Width;
      SpecHeight = (specHeight > 0) ? specHeight : base.Height;
      BackgroundAngle = bgAngle;
      BackgroundSpeed = bgSpeed;
      LemmingCap = lemmingCap;
    }

    public bool IsNoOverwrite { get; set; }
    public bool IsOnlyOnTerrain { get; set; }
    public int Val_L { get; private set; }
    public HashSet<C.Skill> SkillFlags { get; private set; }
    public bool IsZombie => SkillFlags.Contains(C.Skill.Zombie);
    public int SpecWidth { get; set; }
    public int SpecHeight { get; set; }
    public int BackgroundAngle { get; set; }
    public int BackgroundSpeed { get; set; }
    public int LemmingCap { get; set; }

    public override LevelPiece Clone()
    {
      int val_l = ObjType.In(C.OBJ.TELEPORTER, C.OBJ.RECEIVER) ? 0 : Val_L;
      return new GadgetPiece(Key, Pos, Rotation, IsInvert, IsNoOverwrite, IsOnlyOnTerrain,
                             val_l, SkillFlags, SpecWidth, SpecHeight, BackgroundSpeed, BackgroundAngle);
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
          && this.SkillFlags.SetEquals(piece.SkillFlags)
          && this.SpecWidth == piece.SpecWidth
          && this.SpecHeight == piece.SpecHeight
          && this.BackgroundAngle == piece.BackgroundAngle
          && this.BackgroundSpeed == piece.BackgroundSpeed;
    }

    /// <summary>
    /// Returns the position of the trigger area.
    /// </summary>
    public Rectangle TriggerRect
    {
      get
      {
        Rectangle trigRect = ImageLibrary.GetTrigger(Key);
        // Adjust to resizing
        if (ResizeMode != C.Resize.None)
        {
          trigRect.Width = IsRotatedInPlayer ? SpecHeight : SpecWidth;
          trigRect.Height = IsRotatedInPlayer ? SpecWidth : SpecHeight;
        }

        if (ObjType != C.OBJ.ONE_WAY_WALL)
        {
          // Rotate the trigger area correctly
          if (IsRotatedInPlayer)
          {
            int origImageHeight = ImageRectangle.Width;
            trigRect = new Rectangle(origImageHeight - trigRect.Bottom, trigRect.X, trigRect.Height, trigRect.Width);
          }

          // Adjust to flipping
          if (IsFlippedInPlayer && ObjType != C.OBJ.HATCH)
          {
            trigRect.X = ImageRectangle.Width - trigRect.Right;
          }

          // Adjust to inverting
          if (IsInvertedInPlayer)
          {
            trigRect.Y = ImageRectangle.Height - trigRect.Bottom;
          }

          // Offset due to inverting and rotating
          if (IsInvertedInPlayer && !IsRotatedInPlayer)
          {
            trigRect.Y += 10;
          }
          else if (IsRotatedInPlayer)
          {
            trigRect.X += IsFlippedInPlayer ? -4 : 4;
            trigRect.Y += 5;
          }
        }

        // Shift to position relative to level
        trigRect.X += PosX;
        trigRect.Y += PosY;
        return trigRect;
      }
    }

    public override Bitmap Image
    {
      get
      {
        Bitmap image;
        if (ObjType == C.OBJ.PICKUP && Val_L > 1)
        {
          image = AddPickupSkillNumber(base.Image);
        }
        if (ObjType == C.OBJ.HATCH)
        {
          image = ImageLibrary.GetWindowImageWithDirection(Key, GetRotateFlipType(), GetFrameIndex());
        }
        else
        {
          image = base.Image;
        }

        if (ResizeMode == C.Resize.None) return image;
        else if (SpecWidth < 1 || SpecHeight < 1) return new Bitmap(1, 1); // should never happen
        else return image.PaveArea(new Rectangle(0, 0, Width, Height));
      }
    }

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
        foreach (C.Skill skill in C.SkillArray)
        {
          if (SkillFlags.Contains(skill)) return (int)skill;
        }
        return 0;
      }
      else if (ObjType.In(C.OBJ.EXIT_LOCKED, C.OBJ.BUTTON, C.OBJ.TRAPONCE))
      {
        return 1;
      }
      else return base.GetFrameIndex();
    }


    public override bool MayRotate()
    {
      return ObjType.In(C.OBJ.BACKGROUND, C.OBJ.NONE, C.OBJ.ONE_WAY_WALL,
                        C.OBJ.FIRE, C.OBJ.WATER, C.OBJ.TRAP, C.OBJ.TRAPONCE);
    }

    public override bool MayFlip()
    {
      return ObjType.In(C.OBJ.BACKGROUND, C.OBJ.FIRE, C.OBJ.HATCH, C.OBJ.LEMMING,
                        C.OBJ.NONE, C.OBJ.RECEIVER, C.OBJ.SPLAT, C.OBJ.SPLITTER,
                        C.OBJ.TELEPORTER, C.OBJ.TRAP, C.OBJ.TRAPONCE,
                        C.OBJ.WATER, C.OBJ.ONE_WAY_WALL);
    }

    public override bool MayInvert()
    {
      return ObjType.In(C.OBJ.BACKGROUND, C.OBJ.NONE, C.OBJ.ONE_WAY_WALL,
                        C.OBJ.FIRE, C.OBJ.WATER, C.OBJ.TRAP, C.OBJ.TRAPONCE);
    }

    public override bool MayReceiveSkill(C.Skill skill)
    {
      switch (ObjType)
      {
        case C.OBJ.HATCH:
          {
            return skill.In(C.Skill.Climber, C.Skill.Floater, C.Skill.Glider,
                            C.Skill.Disarmer, C.Skill.Swimmer, C.Skill.Zombie);
          }
        case C.OBJ.LEMMING:
          {
            return skill.In(C.Skill.Climber, C.Skill.Floater, C.Skill.Glider,
                            C.Skill.Disarmer, C.Skill.Swimmer, C.Skill.Zombie,
                            C.Skill.Blocker, C.Skill.Shimmier);
          }
        case C.OBJ.PICKUP:
          {
            return skill != C.Skill.Zombie;
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
    public void SetSkillFlag(C.Skill skill, bool doAdd)
    {
      if (!MayReceiveSkill(skill)) return;

      switch (ObjType)
      {
        case C.OBJ.HATCH:
        case C.OBJ.LEMMING:
          {
            if (skill == C.Skill.Floater)
            {
              SkillFlags.Remove(C.Skill.Glider);
            }
            else if (skill == C.Skill.Glider)
            {
              SkillFlags.Remove(C.Skill.Floater);
            }

            if (doAdd) SkillFlags.Add(skill);
            else SkillFlags.Remove(skill);
            break;
          }
        case C.OBJ.PICKUP:
          {
            SkillFlags.Clear();
            if (doAdd) SkillFlags.Add(skill);
            break;
          }
      }
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

    /// <summary>
    /// Sets the number of skills a pick-up skill gives the player.
    /// </summary>
    /// <param name="newValue"></param>
    public void SetPickupSkillCount(int newValue)
    {
      System.Diagnostics.Debug.Assert(ObjType == C.OBJ.PICKUP, "Pickup skill count set for object of another type.");
      Val_L = newValue;
    }

    public void SetLemmingLimit(int newValue)
    {
      System.Diagnostics.Debug.Assert(new[] { C.OBJ.EXIT, C.OBJ.EXIT_LOCKED, C.OBJ.HATCH }.Contains(ObjType), "Lemming limit set for incompatible object.");
      LemmingCap = newValue;
    }

    /// <summary>
    /// Adds the Pickup skill number to the base image
    /// </summary>
    /// <param name="baseImage"></param>
    /// <returns></returns>
    private Bitmap AddPickupSkillNumber(Bitmap baseImage)
    {
      Bitmap image = (Bitmap)baseImage.Clone();
      image.WriteTextEdged(Val_L.ToString(), new Point(image.Width + 4, image.Height + 1), Color.FromArgb(16, 16, 16), 5, ContentAlignment.BottomRight);
      image.WriteTextEdged(Val_L.ToString(), new Point(image.Width + 5, image.Height + 1), Color.FromArgb(240, 240, 240), 5, ContentAlignment.BottomRight);
      return image;
    }

  }

  /// <summary>
  /// This stored all data of a gadget. Inherits from LevelPiece.
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
        : base(oldGroupPiece.Key, pos, rotation, isInvert, isErase, isNoOverwrite, isOneWay)
    {
      terrainPieces = oldGroupPiece.terrainPieces;
    }

    public GroupPiece(List<TerrainPiece> terPieceList)
        : base(GetKeyFromTerPieceList(terPieceList), GetPrelimPosFromTerPieceList(terPieceList))
    {
      terrainPieces = terPieceList.ConvertAll(ter => (TerrainPiece)ter.Clone()).ToList();
      terrainPieces.ForEach(ter => { ter.PosX -= this.PosX; ter.PosY -= this.PosY; });
      bool isSteelGroup = terrainPieces.Exists(ter => ter.IsSteel && !ter.IsErase);
      // Create a cropped image of the group
      Bitmap groupImage;
      using (Renderer groupRenderer = new Renderer())
      {
        groupImage = groupRenderer.CreateTerrainGroupImage(terrainPieces);
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
