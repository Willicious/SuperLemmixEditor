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

        public LevelPiece(string Style, string Name, bool IsObj, Point Pos, 
                          int Rotation = 0, bool IsInvert = false)
        {
            this.fName = Name;
            this.fStyle = Style;
            this.Pos = Pos;

            this.fRotation = Rotation;
            this.fInvert = IsInvert;

            this.fKey = ImageLibrary.CreatePieceKey(Style, Name, IsObj);
        }

        Point fPos;
        string fStyle;
        string fName;
        protected string fKey;

        // RULE: FIRST INVERT - THEN ROTATE
        int fRotation;
        bool fInvert;

        public Point Pos { get { return fPos; } set { fPos = value; } }
        public int PosX { get { return fPos.X; } set { fPos.X = value; } }
        public int PosY { get { return fPos.Y; } set { fPos.Y = value; } }
        public string Style { get { return fStyle; } }
        public string Name { get { return fName; } }
        
        // Metainfo from BaseImageInfo
        public Bitmap Image { get { return ImageLibrary.GetImage(fKey); } }
        public int ObjType { get { return ImageLibrary.GetObjType(fKey); } }

        public void Move(int Direction, int Step = 1)
        {
            switch (Direction)
            {
                case C.DIR_N: PosY = Math.Max(PosY - Step, -1000); break;
                case C.DIR_E: PosX = Math.Min(PosX + Step, 3400); break;
                case C.DIR_S: PosY = Math.Min(PosY + Step, 3400); break;
                case C.DIR_W: PosX = Math.Max(PosY - Step, -1000); break;
            }
        }

        public void Rotate()
        {
            fRotation = (++fRotation) % 4;
        }

        public void Invert()
        {
            fRotation = (4 - fRotation) % 4;
            fInvert = !fInvert;
        }

        public void Flip()
        {
            fRotation = (6 - fRotation) % 4;
            fInvert = !fInvert;
        }
    }

    public class TerrainPiece : LevelPiece
    {
        /*---------------------------------------------------------
         *      This class stores infos about terrain pieces
         * -------------------------------------------------------- */    
    
        public TerrainPiece(string Style, string Name, Point Pos)
            : base(Style, Name, false, Pos)
        {
            fIsErase = false;
            fIsNoOverwrite = false;
            fIsOneWay = true;
        }

        public TerrainPiece(string Style, string Name, Point Pos,
                            int Rotation, bool IsInvert, bool IsErase, bool IsNoOv, bool IsOneWay)
            : base(Style, Name, false, Pos, Rotation, IsInvert)
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
        public GadgetPiece(string Style, string Name, Point Pos)
            : base(Style, Name, false, Pos)
        {
            fIsNoOverwrite = false;
            fIsOnlyOnTerrain = this.ObjType.In(C.OBJ_OWW_LEFT, C.OBJ_OWW_RIGHT, C.OBJ_OWW_DOWN);
        }

        public GadgetPiece(string Style, string Name, Point Pos,
                            int Rotation, bool IsInvert, bool IsNoOv, bool IsOnlyOnTerr)
            : base(Style, Name, false, Pos, Rotation, IsInvert)
        {
            fIsNoOverwrite = IsNoOv;
            fIsOnlyOnTerrain = IsOnlyOnTerr;
        }

        bool fIsNoOverwrite;
        bool fIsOnlyOnTerrain;

        public bool IsNoOverwrite { get { return fIsNoOverwrite; } set { fIsNoOverwrite = value; } }
        public bool IsOnlyOnTerrain { get { return fIsOnlyOnTerrain; } set { fIsOnlyOnTerrain = value; } }

        public Rectangle TriggerRect { get 
        { 
            // TODO: Rotation, ...
            return ImageLibrary.GetTrigger(fKey);
        } }

    }

}
