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
         *    GetCenterPoint()
         *    GetLevelPosFromMousePos(Point MousePos)
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
         *    ChangeZoom(bool DoZoomIn)
         *    ChangeScreenPos(int DeltaX, int Delta Y);
         * 
         *  public varaibles:
         *    ScreenPos
         *    ScreenPosX
         *    ScreenPosY
         *    Zoom
         *    MouseStartPos
         *    MouseCurPos
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
        Rectangle PicBoxRect { get { return new Rectangle(0, 0, fPicBoxSize.Width, fPicBoxSize.Height); } }

        Point? fMouseStartPos;
        Point? fMouseCurPos;

        public Point ScreenPos { get { return fScreenPos; } }
        public int ScreenPosX { get { return fScreenPos.X; } }
        public int ScreenPosY { get { return fScreenPos.Y; } }
        public int Zoom { get { return fZoom; } }

        public Point? MouseStartPos { get { return fMouseStartPos; } set { fMouseStartPos = value; } }
        public Point? MouseCurPos { get { return fMouseCurPos; } set { fMouseCurPos = value; } }

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

        public Point GetCenterPoint()
        { 
            Size LevelBmpSize = GetLevelBmpSize(); // Size without zoom!

            return new Point(fScreenPos.X + LevelBmpSize.Width / 2, fScreenPos.Y + LevelBmpSize.Height / 2);
        }

        private int ApplyZoom(int LvlCoord)
        {
            return (Zoom < 0) ? (LvlCoord / (1 - Zoom)) : (LvlCoord * (Zoom + 1));
        }

        private int ApplyUnZoom(int ZoomCoord)
        {
            return (Zoom < 0) ? (ZoomCoord * (1 - Zoom)) : (ZoomCoord / (Zoom + 1));
        }

        private int BorderWidth()
        {
            return Math.Max(0, (fPicBoxSize.Width - ApplyZoom(fMyLevel.Width)) / 2);
        }

        private int BorderHeight()
        {
            return Math.Max(0, (fPicBoxSize.Height - ApplyZoom(fMyLevel.Height)) / 2);
        }


        public Point? GetMousePosInLevel(bool IsCurrent = true)
        {
            Point? MousePos = IsCurrent ? fMouseCurPos : fMouseStartPos;

            if (MousePos == null) return null;
            if (!PicBoxRect.Contains((Point)MousePos)) return null;

            int OrigPosX = ((Point)MousePos).X;
            int OrigPosY = ((Point)MousePos).Y;

            // Adapt to images that do not fill the whole pic_Level
            if (BorderWidth() > 0)
            {
                OrigPosX = Math.Min(Math.Max(OrigPosX, BorderWidth()), fPicBoxSize.Width - BorderWidth());
                OrigPosX -= BorderWidth();
            }
            if (BorderHeight() > 0)
            {
                OrigPosY = Math.Min(Math.Max(OrigPosY, BorderHeight()), fPicBoxSize.Height - BorderHeight());
                OrigPosY -= BorderHeight();
            }
            
            int PosX = ScreenPosX + ApplyUnZoom(OrigPosX);
            int PosY = ScreenPosY + ApplyUnZoom(OrigPosY) ;
            return new Point(PosX, PosY);
        }

        public Rectangle? GetCurSelectionInLevel()
        {
            Point? LevelPos1 = GetMousePosInLevel(false);
            Point? LevelPos2 = GetMousePosInLevel(true);

            if (LevelPos1 == null || LevelPos2 == null) return null;

            return Utility.RectangleFrom((Point)LevelPos1, (Point)LevelPos2);
        }


        public Bitmap CreateLevelImage()
        {
            UpdateLayerBmpSize();
            
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

        private void UpdateLayerBmpSize()
        {
            if (fMyLevel.Width != fLayerList[0].Width || fMyLevel.Height != fLayerList[0].Height)
            { 
                fLayerList.Select(bmp => bmp = new Bitmap(fMyLevel.Width, fMyLevel.Height));
            }
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
                    obj.IsOnlyOnTerrain && !obj.ObjType.In(C.OBJ.OWW_LEFT, C.OBJ.OWW_RIGHT, C.OBJ.OWW_DOWN));
            foreach (GadgetPiece MyGadget in OnlyOnTerrainGadgetList)
            {
                fLayerList[C.LAY_OBJTOP].DrawOnMask(MyGadget.Image, MyGadget.Pos, fLayerList[C.LAY_TERRAIN]);
            }

            List<GadgetPiece> OWWGadgetList = fMyLevel.GadgetList.FindAll(obj => 
                    obj.ObjType.In(C.OBJ.OWW_LEFT, C.OBJ.OWW_RIGHT, C.OBJ.OWW_DOWN));
            foreach (GadgetPiece MyGadget in OWWGadgetList)
            {
                fLayerList[C.LAY_OBJTOP].DrawOnMask(MyGadget.Image, MyGadget.Pos, fLayerList[C.LAY_OWWTERRAIN]);
            }

            List<GadgetPiece> UsualGadgetList = fMyLevel.GadgetList.FindAll(obj => 
                    !obj.IsNoOverwrite && !obj.IsOnlyOnTerrain && !obj.ObjType.In(C.OBJ.OWW_LEFT, C.OBJ.OWW_RIGHT, C.OBJ.OWW_DOWN));
            foreach (GadgetPiece MyGadget in UsualGadgetList)
            {
                fLayerList[C.LAY_OBJTOP].DrawOn(MyGadget.Image, MyGadget.Pos);
            }   
        }

        private void CreateTriggerLayer()
        {
            fLayerList[C.LAY_TRIGGER].Clear();

            List<Rectangle> TriggerRectList = fMyLevel.GadgetList.Select(obj => obj.TriggerRect).ToList();
            fLayerList[C.LAY_TRIGGER].DrawOnFilledRectangles(TriggerRectList, Color.Violet);
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
            LevelBmp = AddMouseSelectionArea(LevelBmp);

            return LevelBmp;
        }

        private Size GetLevelBmpSize()
        {
            int LevelBmpWidth = ApplyUnZoom(fPicBoxSize.Width);
            int LevelBmpHeight = ApplyUnZoom(fPicBoxSize.Height);

            // Ensure that the LevelBmpSize is at most the size of the level
            LevelBmpWidth = Math.Min(LevelBmpWidth, fMyLevel.Width);
            LevelBmpHeight = Math.Min(LevelBmpHeight, fMyLevel.Height);
                
            return new Size(LevelBmpWidth, LevelBmpHeight);
        }

        private Bitmap AddSelectedRectangles(Bitmap LevelBmp)
        {
            // Get List of all Rectangled to draw
            List<Rectangle> RectList = fMyLevel.TerrainList.FindAll(ter => ter.IsSelected)
                                                           .Select(ter => ter.ImageRectangle)
                                                           .ToList();
            RectList.AddRange(fMyLevel.GadgetList.FindAll(obj => obj.IsSelected)
                                                 .Select(obj => obj.ImageRectangle));

            List<Rectangle> RectOnPicList = RectList.Select(rect => GetPicRectFromLevelRect(rect)).ToList();

            LevelBmp.DrawOnRectangles(RectOnPicList, Color.Gold);

            return LevelBmp;
        }

        private Rectangle GetPicRectFromLevelRect(Rectangle OrigRect)
        {
            int PosX = ApplyZoom(OrigRect.X - fScreenPos.X);
            int PosY = ApplyZoom(OrigRect.Y - fScreenPos.Y);

            int Width = ApplyZoom(OrigRect.Width - 1);
            int Height = ApplyZoom(OrigRect.Height - 1);

            if (Zoom > 0)
            {
                Width += Zoom;
                Height += Zoom;
            }

            return new Rectangle(PosX, PosY, Width, Height);
        }

        private Bitmap AddMouseSelectionArea(Bitmap LevelBmp)
        {
            if (MouseStartPos == null || MouseCurPos == null) return LevelBmp;

            Rectangle MouseRect = Utility.RectangleFrom((Point)MouseStartPos, (Point)MouseCurPos);
            
            // Adapt to borders
            MouseRect.X -= BorderWidth();
            MouseRect.Y -= BorderHeight();

            LevelBmp.DrawOnDottedRectangle((Rectangle)MouseRect);
            
            return LevelBmp;
        }


        public void ChangeZoom(int Change)
        {
            int OldZoom = Zoom;
            fZoom = Math.Max(Math.Min(OldZoom + Change, 7), -2);

            // Change screen position
            float ChangeFactor;
            if (Zoom + OldZoom > 0) // both at least equal to 0
            {
                ChangeFactor = ((float)(Zoom - OldZoom)) / ((OldZoom + 1) * (Zoom + 1) * 2);
            }
            else // both at most equal to 0
            {
                ChangeFactor = ((float)(Zoom - OldZoom)) / 2;
            }

            fScreenPos.X += (int)(fPicBoxSize.Width * ChangeFactor);
            fScreenPos.Y += (int)(fPicBoxSize.Height * ChangeFactor);
            EnsureScreenPosInLevel();
        }

        private void EnsureScreenPosInLevel()
        {
            EnsureScreenPosInLevel(true);
            EnsureScreenPosInLevel(false);
        }

        private void EnsureScreenPosInLevel(bool IsVert)
        {
            int LevelLength = IsVert ? fMyLevel.Height : fMyLevel.Width;
            int PicBoxLength = IsVert ? fPicBoxSize.Height : fPicBoxSize.Width;
            int PicBoxLengthUnzoomed = ApplyUnZoom(PicBoxLength);
            int MaxCoord = LevelLength - PicBoxLengthUnzoomed;

            // do not interchange Max and Min because of possibly negative MaxCoord
            if (IsVert)
            {
                fScreenPos.Y = Math.Max(Math.Min(fScreenPos.Y, MaxCoord), 0); 
            }
            else 
            {
                fScreenPos.X = Math.Max(Math.Min(fScreenPos.X, MaxCoord), 0);
            }      
        }

        public void ChangeScreenPos(int DeltaX, int DeltaY)
        {
            fScreenPos.X += DeltaX;
            fScreenPos.Y += DeltaY;

            EnsureScreenPosInLevel();
        }

    }
}
