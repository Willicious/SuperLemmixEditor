using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace NLEditor
{
    /// <summary>
    /// Produces the level image and stores all data for displaying it.
    /// </summary>
    class Renderer
    {
        /*---------------------------------------------------------
         *          This class renderes the level data
         *       to produce bitmaps for the editor screen
         * -------------------------------------------------------- */

        /*---------------------------------------------------------
         *  IMPORTANT: The Terrain layer uses the alpha value 
         *             to encode the OWW-flag:
         *              One-Way-able     = C.ALPHA_OWW   = 255
         *              Not One-Way-able = C.ALPHA_NOOWW = 254
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

        /// <summary>
        /// Initializes a new instance of a Renderer. This resets all existing display options. 
        /// </summary>
        /// <param name="MyLevel"></param>
        /// <param name="pic_Level"></param>
        public Renderer(Level MyLevel, System.Windows.Forms.PictureBox pic_Level)
        {
            this.fMyLevel = MyLevel;
            System.Diagnostics.Debug.Assert(MyLevel != null, "Renderer created while passing a null level!");

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

            this.LevelPicBox = pic_Level;
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
        System.Windows.Forms.PictureBox LevelPicBox;
        Size fPicBoxSize { get { return LevelPicBox.Size; } }
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
            EnsureScreenPosInLevel();
        }

        /// <summary>
        /// Returns the middle point of pic_Level in level coordinates.
        /// </summary>
        /// <returns></returns>
        public Point GetCenterPoint()
        { 
            Size LevelBmpSize = GetLevelBmpSize(); // Size without zoom!

            return new Point(fScreenPos.X + LevelBmpSize.Width / 2, fScreenPos.Y + LevelBmpSize.Height / 2);
        }

        /// <summary>
        /// Translates level distances to screen distances.
        /// </summary>
        /// <param name="LvlCoord"></param>
        /// <returns></returns>
        private int ApplyZoom(int LvlCoord)
        {
            return (Zoom < 0) ? (LvlCoord / (1 - Zoom)) : (LvlCoord * (Zoom + 1));
        }

        /// <summary>
        /// Translates screen distances to level distances.
        /// </summary>
        /// <param name="ZoomCoord"></param>
        /// <returns></returns>
        private int ApplyUnZoom(int ZoomCoord)
        {
            return (Zoom < 0) ? (ZoomCoord * (1 - Zoom)) : (ZoomCoord / (Zoom + 1));
        }

        /// <summary>
        /// Returns the horizontal width of the border around the level image.
        /// </summary>
        /// <returns></returns>
        private int BorderWidth()
        {
            return Math.Max(0, (fPicBoxSize.Width - ApplyZoom(fMyLevel.Width)) / 2);
        }

        /// <summary>
        /// Returns the vertical height of the border around the level image.
        /// </summary>
        /// <returns></returns>
        private int BorderHeight()
        {
            return Math.Max(0, (fPicBoxSize.Height - ApplyZoom(fMyLevel.Height)) / 2);
        }

        /// <summary>
        /// Translates a point in screen coordinates (relative to pic_Level) into level coordinates.
        /// <para> Returns null if we modify the start position and it lies outside pic_Level. </para>
        /// </summary>
        /// <param name="IsCurrent"></param>
        /// <returns></returns>
        public Point? GetMousePosInLevel(bool IsCurrent = true)
        {
            Point? MousePos = IsCurrent ? fMouseCurPos : fMouseStartPos;

            if (MousePos == null) return null;
            if (!IsCurrent && !PicBoxRect.Contains((Point)MousePos)) return null;

            int OrigPosX = ((Point)MousePos).X;
            int OrigPosY = ((Point)MousePos).Y;

            // Adapt to images that do not fill the whole pic_Level and to Mouse positions outside the level
            OrigPosX = Math.Min(Math.Max(OrigPosX, BorderWidth()), fPicBoxSize.Width - BorderWidth());
            OrigPosX -= BorderWidth();
            OrigPosY = Math.Min(Math.Max(OrigPosY, BorderHeight()), fPicBoxSize.Height - BorderHeight());
            OrigPosY -= BorderHeight();
        
            int PosX = ScreenPosX + ApplyUnZoom(OrigPosX);
            int PosY = ScreenPosY + ApplyUnZoom(OrigPosY) ;
            return new Point(PosX, PosY);
        }

        /// <summary>
        /// Returns the rectangle in level coordinates spanned by the start and current position of the mouse.
        /// <para> Returns null if either mouse position lies outside pic_Level. </para>
        /// </summary>
        /// <returns></returns>
        public Rectangle? GetCurSelectionInLevel()
        {
            Point? LevelPos1 = GetMousePosInLevel(false);
            Point? LevelPos2 = GetMousePosInLevel(true);

            if (LevelPos1 == null || LevelPos2 == null) return null;

            return Utility.RectangleFrom((Point)LevelPos1, (Point)LevelPos2);
        }

        /// <summary>
        /// Renders all layers again and stores the result.
        /// <para> Then combines and crops them and returns the image to display on the screen.</para>
        /// </summary>
        /// <returns></returns>
        public Bitmap CreateLevelImage()
        {
            UpdateLayerBmpSize();
            
            CreateObjectBackLayer();
            CreateTerrainLayer();
            CreateObjectTopLayer();
            CreateTriggerLayer();

            return CombineLayers();
        }

        /// <summary>
        /// Adapt bitmap size of layers according to the level size.
        /// </summary>
        private void UpdateLayerBmpSize()
        {
            if (fMyLevel.Width != fLayerList[0].Width || fMyLevel.Height != fLayerList[0].Height)
            {
                fLayerList = fLayerList.Select(bmp => bmp = new Bitmap(fMyLevel.Width, fMyLevel.Height)).ToList();
            }
        }

        /// <summary>
        /// Renders all NoOverwrite objects.
        /// </summary>
        private void CreateObjectBackLayer()
        {
            fLayerList[C.LAY_OBJBACK].Clear();

            foreach (GadgetPiece MyGadget in fMyLevel.GadgetList.FindAll(obj => obj.IsNoOverwrite))
            {
                fLayerList[C.LAY_OBJBACK].DrawOn(MyGadget.Image, MyGadget.Pos);
            }
        }

        /// <summary>
        /// Renders all terrain pieces.
        /// </summary>
        private void CreateTerrainLayer()
        {
            fLayerList[C.LAY_TERRAIN].Clear();

            foreach (TerrainPiece MyTerrPiece in fMyLevel.TerrainList)
            {
                C.CustDrawMode MyDrawMode = GetDrawModeForTerrain(MyTerrPiece);
                fLayerList[C.LAY_TERRAIN].DrawOn(MyTerrPiece.Image, MyTerrPiece.Pos, MyDrawMode);
            }
        }

        /// <summary>
        /// Returns the correct CustDrawMode for the terrain piece.
        /// </summary>
        /// <param name="MyTerrPiece"></param>
        /// <returns></returns>
        private C.CustDrawMode GetDrawModeForTerrain(TerrainPiece MyTerrPiece)
        {
            if (MyTerrPiece.IsErase) return C.CustDrawMode.Erase;
            else if (MyTerrPiece.IsNoOverwrite)
            {
                if (fIsClearPhysics)
                { 
                    if (MyTerrPiece.IsSteel) return C.CustDrawMode.ClearPhysicsSteelNoOverwrite;
                    else if (MyTerrPiece.IsOneWay) return C.CustDrawMode.ClearPhysicsNoOverwriteOWW;
                    else return C.CustDrawMode.ClearPhysicsNoOverwrite; 
                }
                else 
                {
                    if (MyTerrPiece.IsSteel) return C.CustDrawMode.NoOverwrite;
                    else if (MyTerrPiece.IsOneWay) return C.CustDrawMode.NoOverwriteOWW;
                    else return C.CustDrawMode.NoOverwrite;
                }
            }
            else
            {
                if (fIsClearPhysics)
                {
                    if (MyTerrPiece.IsSteel) return C.CustDrawMode.ClearPhysicsSteel;
                    else if (MyTerrPiece.IsOneWay) return C.CustDrawMode.ClearPhysicsOWW;
                    else return C.CustDrawMode.ClearPhysics;
                }
                else
                {
                    if (MyTerrPiece.IsSteel) return C.CustDrawMode.Default;
                    else if (MyTerrPiece.IsOneWay) return C.CustDrawMode.DefaultOWW;
                    else return C.CustDrawMode.Default;
                }
            }
        }


        /// <summary>
        /// Renders all object, that overwrite usual terrain.
        /// </summary>
        private void CreateObjectTopLayer()
        {
            fLayerList[C.LAY_OBJTOP].Clear();

            List<GadgetPiece> OnlyOnTerrainGadgetList = fMyLevel.GadgetList.FindAll(obj => 
                    obj.IsOnlyOnTerrain && !obj.ObjType.In(C.OBJ.OWW_LEFT, C.OBJ.OWW_RIGHT, C.OBJ.OWW_DOWN));
            foreach (GadgetPiece MyGadget in OnlyOnTerrainGadgetList)
            {
                fLayerList[C.LAY_OBJTOP].DrawOn(MyGadget.Image, fLayerList[C.LAY_TERRAIN], MyGadget.Pos, C.CustDrawMode.OnlyAtMask);
            }

            List<GadgetPiece> OWWGadgetList = fMyLevel.GadgetList.FindAll(obj => 
                    obj.ObjType.In(C.OBJ.OWW_LEFT, C.OBJ.OWW_RIGHT, C.OBJ.OWW_DOWN));
            foreach (GadgetPiece MyGadget in OWWGadgetList)
            {
                fLayerList[C.LAY_OBJTOP].DrawOn(MyGadget.Image, fLayerList[C.LAY_TERRAIN], MyGadget.Pos, C.CustDrawMode.OnlyAtOWW);
            }

            List<GadgetPiece> UsualGadgetList = fMyLevel.GadgetList.FindAll(obj => 
                    !obj.IsNoOverwrite && !obj.IsOnlyOnTerrain && !obj.ObjType.In(C.OBJ.OWW_LEFT, C.OBJ.OWW_RIGHT, C.OBJ.OWW_DOWN));
            foreach (GadgetPiece MyGadget in UsualGadgetList)
            {
                fLayerList[C.LAY_OBJTOP].DrawOn(MyGadget.Image, MyGadget.Pos);
            }   
        }

        /// <summary>
        /// Renders all trigger areas.
        /// </summary>
        private void CreateTriggerLayer()
        {
            fLayerList[C.LAY_TRIGGER].Clear();

            List<Rectangle> TriggerRectList = fMyLevel.GadgetList.Select(obj => obj.TriggerRect).ToList();
            fLayerList[C.LAY_TRIGGER].DrawOnFilledRectangles(TriggerRectList, Color.Violet);
        }

        /// <summary>
        /// Combines and crops stored layers and returns the image to display on the screen.
        /// </summary>
        /// <returns></returns>
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

        /// <summary>
        /// Gets the size of the displayable area in level coordinates.
        /// </summary>
        /// <returns></returns>
        private Size GetLevelBmpSize()
        {
            int LevelBmpWidth = ApplyUnZoom(fPicBoxSize.Width);
            int LevelBmpHeight = ApplyUnZoom(fPicBoxSize.Height);

            // Ensure that the LevelBmpSize is at most the size of the level
            LevelBmpWidth = Math.Min(LevelBmpWidth, fMyLevel.Width);
            LevelBmpHeight = Math.Min(LevelBmpHeight, fMyLevel.Height);
                
            return new Size(LevelBmpWidth, LevelBmpHeight);
        }

        /// <summary>
        /// Draws rectangles around selected pieces on already zoomed and cropped image.
        /// </summary>
        /// <param name="LevelBmp"></param>
        /// <returns></returns>
        private Bitmap AddSelectedRectangles(Bitmap LevelBmp)
        {
            // Get List of all Rectangled to draw
            List<Rectangle> RectList = fMyLevel.SelectionList().Select(ter => ter.ImageRectangle).ToList();
            List<Rectangle> RectOnPicList = RectList.Select(rect => GetPicRectFromLevelRect(rect)).ToList();

            LevelBmp.DrawOnRectangles(RectOnPicList, Color.Gold);

            return LevelBmp;
        }

        /// <summary>
        /// Translates a rectangle in level coordinates into a screen coordinates (relative to pic_Level)
        /// </summary>
        /// <param name="OrigRect"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Draws the rectangle around the area currently selected with the mouse.
        /// </summary>
        /// <param name="LevelBmp"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Modifies the zoom level and adapts the screen position.
        /// </summary>
        /// <param name="Change"></param>
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

        /// <summary>
        /// Ensures that the screen position is chosen such that no unnecessary boundaries appear 
        /// </summary>
        private void EnsureScreenPosInLevel()
        {
            EnsureScreenPosInLevel(true);
            EnsureScreenPosInLevel(false);
        }

        /// <summary>
        /// Ensures that the screen top resp left position is chosen such that no unnecessary boundaries appear 
        /// </summary>
        /// <param name="IsVert"></param>
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

        /// <summary>
        /// Modified the screen position while ensuring that no unnecessary boundaries appear.
        /// </summary>
        /// <param name="DeltaX"></param>
        /// <param name="DeltaY"></param>
        public void ChangeScreenPos(int DeltaX, int DeltaY)
        {
            fScreenPos.X += DeltaX;
            fScreenPos.Y += DeltaY;

            EnsureScreenPosInLevel();
        }

    }
}
