using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;
using System.Diagnostics;

namespace NLEditor
{
    class Renderer
    {
        /*---------------------------------------------------------
         *          This class renderes the level data
         *       to produce bitmaps for the editor screen
         * -------------------------------------------------------- */

        /*---------------------------------------------------------
         *  public methods:
         *    Renderer(Level MyLevel) // constructor
         *    
         *    CreateLevelImage() // recreates all layers and combines them
         *    CombineLayers() // only combines existing layers
         * 
         *    ChangeIsClearPhsyics() 
         *    ChangeIsTerrainLayer()
         *    ChangeIsTriggerLayer() 
         *    ChangeIsScreenStart() 
         *    SetLevel(Level NewLevel)
         *    
         *  public varaibles:
         *    ScreenPos
         *    ScreenPosX
         *    ScreenPosY
         *    Zoom
         * -------------------------------------------------------- */

        public Renderer(Level MyLevel, PictureBox pic_Level)
        {
            this.fMyLevel = MyLevel;
            Debug.Assert(MyLevel != null, "Renderer created while passing a null level!");

            this.fLayerList = new List<Bitmap>(C.LAY_COUNT);
            for (int i = 0; i < C.LAY_COUNT; i++)
            {
                this.fLayerList.Add(new Bitmap(MyLevel.Width, MyLevel.Height));
            }
                
            this.fIsClearPhysics = false;
            this.fIsTerrainLayer = true;
            this.fIsObjectLayer = true;
            this.fIsTriggerLayer = false;
            this.fIsScreenStart = false;

            this.fScreenPos = new Point(0, 0);
            this.fZoom = 0;

            this.fPicBoxSize = pic_Level.Size;
        }

        List<Bitmap> fLayerList;
        Level fMyLevel;
        bool fIsClearPhysics;
        bool fIsTerrainLayer;
        bool fIsObjectLayer;
        bool fIsTriggerLayer;
        bool fIsScreenStart;
        
        Point fScreenPos;
        int fZoom;
        Size fPicBoxSize;

        public Point ScreenPos { get { return fScreenPos; } }
        public int ScreenPosX { get { return fScreenPos.X; } set { fScreenPos.X = value; } }
        public int ScreenPosY { get { return fScreenPos.Y; } set { fScreenPos.Y = value; } }
        public int Zoom { get { return fZoom; } set { fZoom = value; } }

        public void ChangeIsClearPhsyics() 
        {
            fIsClearPhysics = !fIsClearPhysics;
        }

        public void ChangeIsTerrainLayer()
        { 
            fIsTerrainLayer = !fIsTerrainLayer; 
        } 

        public void ChangeIsObjectLayer() 
        { 
            fIsObjectLayer = !fIsObjectLayer; 
        }

        public void ChangeIsTriggerLayer() 
        { 
            fIsTriggerLayer = !fIsTriggerLayer; 
        }
        
        public void ChangeIsScreenStart() 
        { 
            fIsScreenStart = !fIsScreenStart;
        }

        public void SetLevel(Level NewLevel)
        {
            fMyLevel = NewLevel;
        }

        public Bitmap CreateLevelImage()
        {
            CreateObjectBackLayer();
            if (!fIsClearPhysics)
            {
                CreateTerrainLayer();
            }
            else
            { 
                // TODO ----------> !!!
            }
            CreateObjectTopLayer();
            CreateTriggerLayer();

            return CombineLayers();
        }

        private void CreateObjectBackLayer()
        {
            fLayerList[C.LAY_OBJBACK].Clear();

            foreach (GadgetPiece MyGadget in fMyLevel.GadgetList.FindAll(obj => obj.IsNoOverwrite))
            {
                fLayerList[C.LAY_OBJBACK].DrawOn(MyGadget.Image, MyGadget.Pos);
            }
        }

        private void CreateTerrainLayer()
        {
            // We create the OWW-layer as well!
            fLayerList[C.LAY_TERRAIN].Clear();
            fLayerList[C.LAY_OWWTERRAIN].Clear();

            foreach (TerrainPiece MyTerrPiece in fMyLevel.TerrainList)
            {
                if (MyTerrPiece.IsErase)
                {
                    fLayerList[C.LAY_TERRAIN].DrawOnErase(MyTerrPiece.Image, MyTerrPiece.Pos);
                    fLayerList[C.LAY_OWWTERRAIN].DrawOnErase(MyTerrPiece.Image, MyTerrPiece.Pos);
                }
                else if (MyTerrPiece.IsNoOverwrite)
                {
                    if (!MyTerrPiece.IsOneWay)
                    {
                        fLayerList[C.LAY_TERRAIN].DrawOnNoOw(MyTerrPiece.Image, MyTerrPiece.Pos);
                    }
                    else
                    {
                        fLayerList[C.LAY_TERRAIN].DrawOnNoOw(MyTerrPiece.Image, MyTerrPiece.Pos, fLayerList[C.LAY_OWWTERRAIN]);
                    }
                }
                else
                {
                    fLayerList[C.LAY_TERRAIN].DrawOn(MyTerrPiece.Image, MyTerrPiece.Pos);
                    if (MyTerrPiece.IsOneWay) fLayerList[C.LAY_OWWTERRAIN].DrawOn(MyTerrPiece.Image, MyTerrPiece.Pos);
                }
            }
        }

        private void CreateObjectTopLayer()
        {
            fLayerList[C.LAY_OBJTOP].Clear();

            List<GadgetPiece> OnlyOnTerrainGadgetList = fMyLevel.GadgetList.FindAll(obj => 
                    obj.IsOnlyOnTerrain && !obj.ObjType.In(C.OBJ_OWW_LEFT, C.OBJ_OWW_RIGHT, C.OBJ_OWW_DOWN));
            foreach (GadgetPiece MyGadget in OnlyOnTerrainGadgetList)
            {
                fLayerList[C.LAY_OBJTOP].DrawOnMask(MyGadget.Image, MyGadget.Pos, fLayerList[C.LAY_TERRAIN]);
            }

            List<GadgetPiece> OWWGadgetList = fMyLevel.GadgetList.FindAll(obj => 
                    obj.ObjType.In(C.OBJ_OWW_LEFT, C.OBJ_OWW_RIGHT, C.OBJ_OWW_DOWN));
            foreach (GadgetPiece MyGadget in OWWGadgetList)
            {
                fLayerList[C.LAY_OBJTOP].DrawOnMask(MyGadget.Image, MyGadget.Pos, fLayerList[C.LAY_OWWTERRAIN]);
            }

            List<GadgetPiece> UsualGadgetList = fMyLevel.GadgetList.FindAll(obj => 
                    !obj.IsNoOverwrite && !obj.IsOnlyOnTerrain && !obj.ObjType.In(C.OBJ_OWW_LEFT, C.OBJ_OWW_RIGHT, C.OBJ_OWW_DOWN));
            foreach (GadgetPiece MyGadget in UsualGadgetList)
            {
                fLayerList[C.LAY_OBJTOP].DrawOn(MyGadget.Image, MyGadget.Pos);
            }   
        }

        private void CreateTriggerLayer()
        {
            fLayerList[C.LAY_TRIGGER].Clear();

            using (Graphics g = Graphics.FromImage(fLayerList[C.LAY_TRIGGER]))
            {
                using (Brush b = new SolidBrush(Color.Violet))
                {
                    fMyLevel.GadgetList.ForEach(obj => g.FillRectangle(b, obj.TriggerRect));
                }
                g.Dispose();
            }
        }

        public Bitmap CombineLayers()
        {
            Size LevelBmpSize = GetLevelBmpSize();
            Bitmap LevelBmp = new Bitmap(LevelBmpSize.Width, LevelBmpSize.Height);

            int NegScreenPosX = (LevelBmpSize.Width != fMyLevel.Width) ? -fScreenPos.X : 0;
            int NegScreenPosY = (LevelBmpSize.Height != fMyLevel.Height) ? -fScreenPos.Y : 0;
            Point NegScreenPos = new Point(NegScreenPosX, NegScreenPosY);

            // Set background color
            LevelBmp.Clear(fMyLevel.MainStyle.BackgroundColor);

            if (fIsObjectLayer) 
            {
                LevelBmp.DrawOn(fLayerList[C.LAY_OBJBACK], NegScreenPos);
            }

            if (fIsTerrainLayer)
            {
                LevelBmp.DrawOn(fLayerList[C.LAY_TERRAIN], NegScreenPos);
            }

            if (fIsTerrainLayer && fIsObjectLayer)
            {
                LevelBmp.DrawOn(fLayerList[C.LAY_OBJTOP], NegScreenPos);
            }

            if (fIsTriggerLayer)
            {
                LevelBmp.DrawOn(fLayerList[C.LAY_TRIGGER], NegScreenPos);
            }

            // Zoom the LevelBmp correctly
            LevelBmp = LevelBmp.Zoom(fZoom);
            if (fPicBoxSize.Width < LevelBmp.Width || fPicBoxSize.Height < LevelBmp.Height)
            {
                LevelBmp.Crop(new Rectangle(0, 0, fPicBoxSize.Width, fPicBoxSize.Height));
            }

            // Add rectangles around selected pieces
            LevelBmp = AddSelectedRectangles(LevelBmp);

            return LevelBmp;
        }

        private Size GetLevelBmpSize()
        {
            int LevelBmpWidth;
            int LevelBmpHeight;

            if (Zoom < 0)
            {
                LevelBmpWidth = fPicBoxSize.Width * (Math.Abs(Zoom) + 1);
                LevelBmpHeight = fPicBoxSize.Height * (Math.Abs(Zoom) + 1);
            }
            else
            {
                // we are always rounding up here
                LevelBmpWidth = (fPicBoxSize.Width + Zoom) / (Zoom + 1); 
                LevelBmpHeight = (fPicBoxSize.Height + Zoom) / (Zoom + 1);
            }

            // Ensure that the LevelBmpSize is at most the size of the level
            LevelBmpWidth = Math.Min(LevelBmpWidth, fMyLevel.Width);
            LevelBmpHeight = Math.Min(LevelBmpHeight, fMyLevel.Height);
                
            return new Size(LevelBmpWidth, LevelBmpHeight);
        }

        private Bitmap AddSelectedRectangles(Bitmap LevelBmp)
        {
            // TODO ----------------------------------------->>
            return LevelBmp;
        }


    }
}
