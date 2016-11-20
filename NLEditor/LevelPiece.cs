using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace NLEditor
{
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
        public string Style { get { return fStyle; } }
        public string Name { get { return fName; } }

        public bool IsRotatedInPlayer { get { return (fRotation % 2 == 1); } }
        public bool IsInvertedInPlayer { get { return (fInvert && fRotation % 4 < 2) || (!fInvert && fRotation % 4 > 1); } }
        public bool IsFlippedInPlayer { get { return (fRotation % 4 > 1); } }

        // Metainfo from BaseImageInfo
        public Bitmap Image { get { return ImageLibrary.GetImage(fKey); } }
        public C.OBJ ObjType { get { return ImageLibrary.GetObjType(fKey); } }

        public bool IsSelected { get { return fIsSelected; } set { fIsSelected = value; } }

        public Rectangle ImageRectangle { get 
        {
            int ImageWidth;
            int ImageHeight;

            if (fRotation / 2 == 0)
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

        public void Rotate()
        {
            fRotation = (fInvert ? 4 - fRotation : ++fRotation) % 4;
        }

        public void Invert()
        {
            fInvert = !fInvert;
        }

        public void Flip() // = Invert + Rotate^2
        {
            fRotation = (fRotation + 2) % 4;
            fInvert = !fInvert;
        }
    }

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
    }

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

        public Rectangle TriggerRect { get 
        {
            Rectangle TrigRect = ImageLibrary.GetTrigger(fKey);
            // TODO: Rotation, ...
            // shift to correct position
            TrigRect.X += this.PosX;
            TrigRect.Y += this.PosY;
            return TrigRect;
        } }

    }

}
