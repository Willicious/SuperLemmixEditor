using System;
using System.Collections.Generic;
using System.Linq;
using System.Drawing;

namespace NLEditor
{
    /// <summary>
    /// This stored all data of a gadget. Inherits from LevelPiece.
    /// </summary>
    [Serializable]
    class GadgetPiece : LevelPiece
    {
        public GadgetPiece(string key, Point pos)
            : base(key, true, pos)
        {
            IsNoOverwrite = !ObjType.In(C.OBJ.ONE_WAY_WALL, C.OBJ.PAINT, C.OBJ.LEMMING);
            IsOnlyOnTerrain = (ObjType.In(C.OBJ.ONE_WAY_WALL, C.OBJ.PAINT));
            Val_L = 0;
            SkillFlags = new HashSet<C.Skill>();
            SpecWidth = Utility.EvaluateResizable(0, DefaultWidth, base.Width, MayResizeHoriz());
            SpecHeight = Utility.EvaluateResizable(0, DefaultHeight, base.Height, MayResizeVert());
            CountdownLength = 10;
        }

        public GadgetPiece(string key, Point pos, int rotation, bool isInvert, bool isNoOverwrite,
                           bool isOnlyOnTerrain, int valL, HashSet<C.Skill> skillFlags,
                           int specWidth = -1, int specHeight = -1,
                           int dnSpeed = 0, int dnAngle = 0, int lemmingCap = 0,
                           int countdownLength = 10)
            : base(key, true, pos, rotation, isInvert)
        {
            IsNoOverwrite = isNoOverwrite;
            IsOnlyOnTerrain = isOnlyOnTerrain;
            Val_L = valL;
            SkillFlags = new HashSet<C.Skill>(skillFlags);
            SpecWidth = Utility.EvaluateResizable(specWidth, DefaultWidth, base.Width, MayResizeHoriz());
            SpecHeight = Utility.EvaluateResizable(specHeight, DefaultHeight, base.Height, MayResizeVert());
            DecorationAngle = dnAngle;
            DecorationSpeed = dnSpeed;
            LemmingCap = lemmingCap;
            CountdownLength = countdownLength;
        }

        public bool IsNoOverwrite { get; set; }
        public bool IsOnlyOnTerrain { get; set; }
        public int Val_L { get; private set; }
        public HashSet<C.Skill> SkillFlags { get; private set; }
        public bool IsZombie => SkillFlags.Contains(C.Skill.Zombie);
        public bool IsRival => SkillFlags.Contains(C.Skill.Rival)
                           && !SkillFlags.Contains(C.Skill.Zombie);
        public bool IsNeutral => SkillFlags.Contains(C.Skill.Neutral)
                             && !SkillFlags.Contains(C.Skill.Zombie)
                             && !SkillFlags.Contains(C.Skill.Rival);
        public int DecorationAngle { get; set; }
        public int DecorationSpeed { get; set; }
        public int LemmingCap { get; set; }
        public int CountdownLength { get; set; }

        public override LevelPiece Clone()
        {
            int val_l = Val_L;
            return new GadgetPiece(Key, Pos, Rotation, IsInvert, IsNoOverwrite, IsOnlyOnTerrain,
                                   val_l, SkillFlags, SpecWidth, SpecHeight, DecorationSpeed, DecorationAngle,
                                   LemmingCap, CountdownLength);
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
                && this.DecorationAngle == piece.DecorationAngle
                && this.DecorationSpeed == piece.DecorationSpeed
                && this.LemmingCap == piece.LemmingCap
                && this.CountdownLength == piece.CountdownLength;
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
                if (IsRotatedInPlayer)
                {   
                    // When both resized and rotated:
                    // To get the new height value, we add the new "width" increment, and subtract the original object height (which is now the width!)
                    if (ResizeMode == C.Resize.Both || ResizeMode == C.Resize.Vert)
                        trigRect.Height += Width - ImageLibrary.GetHeight(Key);

                    // Vice versa to get the new width value
                    if (ResizeMode == C.Resize.Both || ResizeMode == C.Resize.Horiz)
                        trigRect.Width += Height - ImageLibrary.GetWidth(Key);
                }
                else
                {   // When resized but not rotated:
                    // To get the new width value, we add the new "width" increment, and subtract the original object width
                    if (ResizeMode.In(C.Resize.Both, C.Resize.Horiz))
                        trigRect.Width += Width - ImageLibrary.GetWidth(Key);

                    // Vice versa to get the new height value
                    if (ResizeMode.In(C.Resize.Both, C.Resize.Vert))
                        trigRect.Height += Height - ImageLibrary.GetHeight(Key);
                }

                if (ObjType != C.OBJ.ONE_WAY_WALL) // For all objects except one-way-walls
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

        public override int Width => Utility.EvaluateResizable(SpecWidth, DefaultWidth, base.Width, MayResizeHoriz());
        public override int Height => Utility.EvaluateResizable(SpecHeight, DefaultHeight, base.Height, MayResizeVert());

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
                    if (SkillFlags.Contains(skill))
                        return (int)skill;
                }
                return 0;
            }
            else if (ObjType.In(C.OBJ.EXIT_LOCKED, C.OBJ.BUTTON, C.OBJ.COLLECTIBLE, C.OBJ.TRAPONCE))
            {
                return 1;
            }
            else
                return base.GetFrameIndex();
        }


        public override bool MayRotate()
        {
            return !(ObjType.In(C.OBJ.HATCH, C.OBJ.SPLITTER, C.OBJ.LEMMING, C.OBJ.PICKUP));
        }

        public override bool MayFlip()
        {
            return !(ObjType.In(C.OBJ.PICKUP));
        }

        public override bool MayInvert()
        {
            return !(ObjType.In(C.OBJ.HATCH, C.OBJ.SPLITTER, C.OBJ.LEMMING, C.OBJ.PICKUP));
        }

        public override bool MayReceiveSkill(C.Skill skill)
        {
            switch (ObjType)
            {
                case C.OBJ.HATCH:
                    {
                        return skill.In(C.Skill.Slider, C.Skill.Climber, C.Skill.Floater, C.Skill.Glider,
                                        C.Skill.Disarmer, C.Skill.Swimmer,
                                        C.Skill.Zombie, C.Skill.Rival, C.Skill.Neutral);
                    }
                case C.OBJ.LEMMING:
                    {
                        return skill.In(C.Skill.Slider, C.Skill.Climber, C.Skill.Floater, C.Skill.Glider,
                                        C.Skill.Disarmer, C.Skill.Swimmer, 
                                        C.Skill.Ballooner, C.Skill.Blocker, C.Skill.Shimmier,
                                        C.Skill.Zombie, C.Skill.Rival, C.Skill.Neutral);
                    }
                case C.OBJ.PICKUP:
                    {
                        return !skill.In(C.Skill.Zombie, C.Skill.Rival, C.Skill.Neutral);
                    }
                case C.OBJ.EXIT:
                case C.OBJ.EXIT_LOCKED:
                    {
                        return skill.In(C.Skill.Rival);
                    }
                default:
                    return false;
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
            if (!MayReceiveSkill(skill))
                return;

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

                        if (skill == C.Skill.Rival)
                        {
                            SkillFlags.Remove(C.Skill.Zombie);
                            SkillFlags.Remove(C.Skill.Neutral);
                        }
                        else if (skill == C.Skill.Neutral || skill == C.Skill.Zombie)
                        {
                            SkillFlags.Remove(C.Skill.Rival);
                        }

                        if (doAdd)
                            SkillFlags.Add(skill);
                        else
                            SkillFlags.Remove(skill);
                        break;
                    }
                case C.OBJ.PICKUP:
                case C.OBJ.EXIT:
                case C.OBJ.EXIT_LOCKED:
                    {
                        SkillFlags.Clear();
                        if (doAdd)
                            SkillFlags.Add(skill);
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
            if (MayResizeHoriz())
                SpecWidth = Math.Max(newWidth, 1);
        }

        /// <summary>
        /// Sets the height of resizable objects taking rotation into account.
        /// </summary>
        /// <param name="newHeight"></param>
        public void SetSpecHeight(int newHeight)
        {
            if (MayResizeVert())
                SpecHeight = Math.Max(newHeight, 1);
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

        public void SetCountdownLength(int newValue)
        {
            System.Diagnostics.Debug.Assert(new[] { C.OBJ.RADIATION, C.OBJ.SLOWFREEZE }.Contains(ObjType), "Countdown length set for incompatible object.");
            CountdownLength = newValue;
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

}
