using System;
using System.Collections.Generic;
using System.Linq;
using System.Drawing;
using System.Windows.Forms;

namespace NLEditor
{
    /// <summary>
    /// Produces the level image and stores all data for displaying it.
    /// </summary>
    class Renderer : IDisposable
    {
        /*---------------------------------------------------------
         *  IMPORTANT: The Terrain layer uses the alpha value 
         *             to encode the OWW-flag:
         *              One-Way-able     = C.ALPHA_OWW   = 255
         *              Not One-Way-able = C.ALPHA_NOOWW = 254
         * -------------------------------------------------------- */

        /// <summary>
        /// Initializes an empty Renderer. 
        /// </summary>
        public Renderer()
        {
            this.level = null;
        }

        /// <summary>
        /// Initializes a new instance of a Renderer. This resets all existing display options. 
        /// </summary>
        /// <param name="level"></param>
        /// <param name="pic_Level"></param>
        public Renderer(Level level, PictureBox pic_Level)
        {
            this.IsClearPhysics = false;
            this.IsTerrainLayer = true;
            this.IsObjectLayer = true;
            this.IsTriggerLayer = false;
            this.IsScreenStart = false;
            this.IsBackgroundLayer = false;

            this.ScreenPosX = 0;
            this.ScreenPosY = 0;
            this.ZoomFactor = 0;

            this.levelPicBox = pic_Level;

            SetLevel(level);
            ClearLayers();
        }

        Dictionary<C.Layer, Bitmap> layerImages;
        Level level;
        public bool IsClearPhysics { get; private set; }
        public bool IsTerrainLayer { get; private set; }
        public bool IsObjectLayer { get; private set; }
        public bool IsTriggerLayer { get; private set; }
        public bool IsScreenStart { get; private set; }
        public bool IsBackgroundLayer { get; private set; }
        
        PictureBox levelPicBox;
        int picBoxWidth => levelPicBox.Size.Width - 4;
        int picBoxHeight => levelPicBox.Size.Height - 5;
        Rectangle picBoxRect => new Rectangle(0, 0, picBoxWidth, picBoxHeight);

        public Point ScreenPos => new Point(ScreenPosX, ScreenPosY);
        public int ScreenPosX { get; set; }
        public int ScreenPosY { get; set; }
        public int ZoomFactor { get; private set; }

        public Point? MouseStartPos { get; private set; }
        public Point? LevelStartPos { get; private set; }
        public Point? MouseCurPos { get; set; }
        public C.DragActions MouseDragAction { get; private set; }

        public void Dispose()
        {
            if (layerImages != null)
            {
                layerImages.Values.ToList().ForEach(bmp => bmp.Dispose());
            }
        }

        private void ClearLayers()
        {
            Dispose();
            layerImages = C.LayerList.ToDictionary(layer => layer, layer => new Bitmap(level.Width, level.Height));
        }

        public void ChangeIsClearPhsyics() 
        {
            IsClearPhysics = !IsClearPhysics;
        }

        public void ChangeIsTerrainLayer()
        { 
            IsTerrainLayer = !IsTerrainLayer; 
        } 

        public void ChangeIsObjectLayer() 
        { 
            IsObjectLayer = !IsObjectLayer; 
        }

        public void ChangeIsTriggerLayer() 
        { 
            IsTriggerLayer = !IsTriggerLayer; 
        }
        
        public void ChangeIsScreenStart() 
        { 
            IsScreenStart = !IsScreenStart;
        }

        public void ChangeIsBackgroundLayer()
        {
            IsBackgroundLayer = !IsBackgroundLayer;
        }

        public void SetLevel(Level newLevel)
        {
            level = newLevel;
            EnsureScreenPosInLevel();
        }

        /// <summary>
        /// Sets the start position of the mouse and the screen position respectively the selection position.
        /// </summary>
        /// <param name="mousePos"></param>
        public void SetDraggingVars(Point mousePos, C.DragActions dragAction)
        {
            if (dragAction == C.DragActions.Null) return;

            MouseDragAction = dragAction;
            MouseStartPos = mousePos;
            MouseCurPos = mousePos;

            switch (dragAction)
            {
                case C.DragActions.MoveEditorPos: LevelStartPos = ScreenPos; break;
                case C.DragActions.DragPieces: LevelStartPos = level.SelectionRectangle().Location; break;
                case C.DragActions.MoveStartPos: LevelStartPos = level.StartPos; break;
            }
        }

        /// <summary>
        /// Resets all mouse and start positions to null.
        /// </summary>
        public void DeleteDraggingVars()
        {
            MouseStartPos = null;
            MouseCurPos = null;
            LevelStartPos = null;
            MouseDragAction = C.DragActions.Null;
        }

        /// <summary>
        /// Returns the middle point of pic_Level in level coordinates.
        /// </summary>
        /// <returns></returns>
        public Point GetCenterPoint()
        { 
            Size levelBmpSize = GetLevelBmpSize(); // Size without zoom!
            return new Point(ScreenPosX + levelBmpSize.Width / 2, ScreenPosY + levelBmpSize.Height / 2);
        }

        /// <summary>
        /// Translates level distances to screen distances.
        /// </summary>
        /// <param name="lvlCoord"></param>
        /// <returns></returns>
        public int ApplyZoom(int lvlCoord)
        {
            return (ZoomFactor < 0) ? (lvlCoord / (1 - ZoomFactor)) : (lvlCoord * (ZoomFactor + 1));
        }

        /// <summary>
        /// Translates screen distances to level distances.
        /// </summary>
        /// <param name="screenCoord"></param>
        /// <returns></returns>
        public int ApplyUnZoom(int screenCoord)
        {
            return (ZoomFactor < 0) ? (screenCoord * (1 - ZoomFactor)) : (screenCoord / (ZoomFactor + 1));
        }

        /// <summary>
        /// Returns the horizontal (screen) width of the border around the level image.
        /// </summary>
        /// <returns></returns>
        private int BorderWidth()
        {
            return Math.Max(0, (picBoxWidth - ApplyZoom(level.Width)) / 2);
        }

        /// <summary>
        /// Returns the vertical (screen) height of the border around the level image.
        /// </summary>
        /// <returns></returns>
        private int BorderHeight()
        {
            return Math.Max(0, (picBoxHeight - ApplyZoom(level.Height)) / 2);
        }

        /// <summary>
        /// Returns whether a point in screen corrdinates relative to pic_Level lies in the level area.
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        public bool IsPointInLevelArea(Point? point)
        {
            if (point == null) return false;
            Rectangle levelRect = new Rectangle(BorderWidth(), BorderHeight(), picBoxWidth - 2 * BorderWidth(), picBoxHeight - 2 * BorderHeight());
            return levelRect.Contains((Point)point);
        }

        /// <summary>
        /// Returns whether the current mouse position (as stored by the renderer) lies in the level area.
        /// </summary>
        /// <returns></returns>
        public bool IsPointInLevelArea()
        {
            return IsPointInLevelArea(MouseCurPos);
        }

        /// <summary>
        /// Translates a point in screen coordinates (relative to pic_Level) into level coordinates.
        /// </summary>
        /// <param name="mouseScreenPos"></param>
        /// <returns></returns>
        public Point GetMousePosInLevel(Point mouseScreenPos)
        {
            // Adapt to images that do not fill the whole pic_Level and to Mouse positions outside the level
            int mouseScreenPosX = Math.Min(Math.Max(mouseScreenPos.X, BorderWidth()), picBoxWidth - BorderWidth())
                                    - BorderWidth();
            int mouseScreenPosY = Math.Min(Math.Max(mouseScreenPos.Y, BorderHeight()), picBoxHeight - BorderHeight())
                                    - BorderHeight();

            int posX = ScreenPosX + ApplyUnZoom(mouseScreenPosX);
            int posY = ScreenPosY + ApplyUnZoom(mouseScreenPosY);
            return new Point(posX, posY);
        }

        
        /// <summary>
        /// Returns the start or current mouse position in level coordinates.
        /// <para> Returns null if this position lies outside pic_Level. </para>
        /// </summary>
        /// <param name="isCurrent"></param>
        /// <returns></returns>
        public Point? GetMousePosInLevel(bool isCurrent = true)
        {
            Point? MousePos = isCurrent ? MouseCurPos : MouseStartPos;

            if (MousePos == null) return null;
            if (!isCurrent && !picBoxRect.Contains((Point)MousePos)) return null;

            return GetMousePosInLevel((Point)MousePos);
        }

        /// <summary>
        /// Returns the rectangle in level coordinates spanned by the start and current position of the mouse.
        /// <para> Returns null if either mouse position lies outside pic_Level. </para>
        /// </summary>
        /// <returns></returns>
        public Rectangle? GetCurSelectionInLevel()
        {
            Point? lvlPos1 = GetMousePosInLevel(false);
            Point? lvlPos2 = GetMousePosInLevel(true);

            if (lvlPos1 == null || lvlPos2 == null) return null;

            return Utility.RectangleFrom((Point)lvlPos1, (Point)lvlPos2);
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
            if (level.Width != layerImages[C.Layer.Terrain].Width || level.Height != layerImages[C.Layer.Terrain].Height)
            {
                ClearLayers();
                CreateBackgroundLayer();
            }
        }

        /// <summary>
        /// Creates the background layer with the correct background color and background image.
        /// </summary>
        public void CreateBackgroundLayer()
        {
            // Set background color
            layerImages[C.Layer.Background].Clear(level.MainStyle?.GetColor(C.StyleColor.BACKGROUND) ?? C.NLColors[C.NLColor.BackDefault]);

            // Display background images, if selected
            if (level.Background != null)
            {
                Bitmap backgroundImage = ImageLibrary.GetImage(level.Background.Key, RotateFlipType.RotateNoneFlipNone)
                                                     .PaveArea(new Rectangle(0, 0, level.Width, level.Height));
                
                layerImages[C.Layer.Background].DrawOn(backgroundImage, new Point(0, 0));
            }
        }

        /// <summary>
        /// Renders all NoOverwrite objects.
        /// </summary>
        private void CreateObjectBackLayer()
        {
            layerImages[C.Layer.ObjBack].Clear();

            foreach (GadgetPiece gadget in level.GadgetList.FindAll(obj => obj.IsNoOverwrite))
            {
                layerImages[C.Layer.ObjBack].DrawOn(gadget.Image, gadget.Pos);
            }
        }

        /// <summary>
        /// Renders all terrain pieces.
        /// </summary>
        private void CreateTerrainLayer()
        {
            layerImages[C.Layer.Terrain].Clear();

            foreach (TerrainPiece terrPiece in level.TerrainList)
            {
                C.CustDrawMode drawMode = GetDrawModeForTerrain(terrPiece);
                layerImages[C.Layer.Terrain].DrawOn(terrPiece.Image, terrPiece.Pos, drawMode);
            }
        }

        /// <summary>
        /// Draws the piece with the given Key on the levelBmp at the current mouse position.
        /// </summary>
        /// <param name="levelBmp"></param>
        /// <param name="newPieceKey"></param>
        private void AddDragNewPiece(Bitmap levelBmp, string newPieceKey, Point offset)
        {
            if (MouseCurPos == null) return;

            Bitmap pieceImage = ImageLibrary.GetImage(newPieceKey);
            Point mouseLevelPos = GetMousePosInLevel((Point)MouseCurPos);

            Point piecePos = new Point(mouseLevelPos.X - pieceImage.Width / 2 + offset.X,
                                       mouseLevelPos.Y - pieceImage.Height / 2 + offset.Y);
            levelBmp.DrawOn(pieceImage, piecePos, C.CustDrawMode.Default);
        }

        /// <summary>
        /// Renders all terrain pieces in the TerrPieceList.
        /// <para> This assumes IsClearPhysics = false.</para>
        /// </summary>
        /// <param name="terrPieces"></param>
        /// <returns></returns>
        public Bitmap CreateTerrainGroupImage(List<TerrainPiece> terrPieces)
        {
            int width = terrPieces.Max(ter => ter.PosX + ter.Width);
            int height = terrPieces.Max(ter => ter.PosY + ter.Height);

            Bitmap GroupImage = new Bitmap(width, height);

            foreach (TerrainPiece terrain in terrPieces)
            {
                C.CustDrawMode drawMode = GetDrawModeForTerrain(terrain);
                GroupImage.DrawOn(terrain.Image, terrain.Pos, drawMode);
            }

            return GroupImage;
        }


        /// <summary>
        /// Returns the correct CustDrawMode for the terrain piece.
        /// </summary>
        /// <param name="terrPiece"></param>
        /// <returns></returns>
        private C.CustDrawMode GetDrawModeForTerrain(TerrainPiece terrPiece)
        {
            if (terrPiece.IsErase) return C.CustDrawMode.Erase;
            else if (terrPiece.IsNoOverwrite)
            {
                if (IsClearPhysics)
                { 
                    if (terrPiece.IsSteel) return C.CustDrawMode.ClearPhysicsSteelNoOverwrite;
                    else if (terrPiece.IsOneWay) return C.CustDrawMode.ClearPhysicsNoOverwriteOWW;
                    else return C.CustDrawMode.ClearPhysicsNoOverwrite; 
                }
                else 
                {
                    if (terrPiece.IsSteel) return C.CustDrawMode.NoOverwrite;
                    else if (terrPiece.IsOneWay) return C.CustDrawMode.NoOverwriteOWW;
                    else return C.CustDrawMode.NoOverwrite;
                }
            }
            else
            {
                if (IsClearPhysics)
                {
                    if (terrPiece.IsSteel) return C.CustDrawMode.ClearPhysicsSteel;
                    else if (terrPiece.IsOneWay) return C.CustDrawMode.ClearPhysicsOWW;
                    else return C.CustDrawMode.ClearPhysics;
                }
                else
                {
                    if (terrPiece.IsSteel) return C.CustDrawMode.Default;
                    else if (terrPiece.IsOneWay) return C.CustDrawMode.DefaultOWW;
                    else return C.CustDrawMode.Default;
                }
            }
        }


        /// <summary>
        /// Renders all object, that overwrite usual terrain.
        /// </summary>
        private void CreateObjectTopLayer()
        {
            layerImages[C.Layer.ObjTop].Clear();

            var onlyOnTerrainGadgetList = level.GadgetList.FindAll(gad => 
                    gad.IsOnlyOnTerrain && gad.ObjType != C.OBJ.ONE_WAY_WALL);
            foreach (GadgetPiece gadget in onlyOnTerrainGadgetList)
            {
                layerImages[C.Layer.ObjTop].DrawOn(gadget.Image, layerImages[C.Layer.Terrain], gadget.Pos, C.CustDrawMode.OnlyAtMask);
            }

            var owwGadgetList = level.GadgetList.FindAll(gad => gad.ObjType == C.OBJ.ONE_WAY_WALL);
            foreach (GadgetPiece gadget in owwGadgetList)
            {
                Bitmap gadgetImage;
                if (gadget.Style != "default")
                {
                    gadgetImage = gadget.Image;
                }
                else
                {
                    gadgetImage = gadget.Image.ApplyThemeColor(level.GetThemeColor(C.StyleColor.ONE_WAY_WALL));
                }
                layerImages[C.Layer.ObjTop].DrawOn(gadgetImage, layerImages[C.Layer.Terrain], gadget.Pos, C.CustDrawMode.OnlyAtOWW);
            }

            var normalGadgetList = level.GadgetList.FindAll(gad => 
                    !gad.IsNoOverwrite && !gad.IsOnlyOnTerrain && gad.ObjType != C.OBJ.ONE_WAY_WALL);
            foreach (GadgetPiece gadget in normalGadgetList)
            {
                layerImages[C.Layer.ObjTop].DrawOn(gadget.Image, gadget.Pos);
            }   
        }

        /// <summary>
        /// Renders all trigger areas.
        /// </summary>
        private void CreateTriggerLayer()
        {
            layerImages[C.Layer.Trigger].Clear();

            var triggerRectangles = level.GadgetList.ConvertAll(obj => obj.TriggerRect);
            layerImages[C.Layer.Trigger].DrawOnFilledRectangles(triggerRectangles, C.NLColors[C.NLColor.Trigger]);
        }

        /// <summary>
        /// Combines and crops stored layers and returns the image to display on the screen.
        /// </summary>
        /// <returns></returns>
        public Bitmap CombineLayers(string dragNewPieceKey = null)
        {
            UpdateScreenPos();
            Point oldScreenPos = new Point(ScreenPosX, ScreenPosY);
            Point negativeScreenPos = new Point(-ScreenPosX, -ScreenPosY);

            Size levelBmpSize = GetLevelBmpSize();
            Bitmap levelBmp;
            if (IsBackgroundLayer)
            {
                levelBmp = (Bitmap)layerImages[C.Layer.Background].Clone();
            }
            else
            {
                // Still use background color
                levelBmp = new Bitmap(levelBmpSize.Width, levelBmpSize.Height);
                levelBmp.Clear(level.MainStyle?.GetColor(C.StyleColor.BACKGROUND) ?? C.NLColors[C.NLColor.BackDefault]);
            }
            

            if (IsObjectLayer) 
            {
                levelBmp.DrawOn(layerImages[C.Layer.ObjBack], negativeScreenPos);
            }

            if (IsTerrainLayer)
            {
                levelBmp.DrawOn(layerImages[C.Layer.Terrain], negativeScreenPos);
            }

            if (dragNewPieceKey != null && MouseDragAction == C.DragActions.DragNewPiece 
                                        && IsPointInLevelArea(MouseCurPos))
            {
                AddDragNewPiece(levelBmp, dragNewPieceKey, negativeScreenPos);
            }

            if (IsTerrainLayer && IsObjectLayer)
            {
                levelBmp.DrawOn(layerImages[C.Layer.ObjTop], negativeScreenPos);
            }

            if (IsTriggerLayer)
            {
                levelBmp.DrawOnWithAlpha(layerImages[C.Layer.Trigger], negativeScreenPos);
            }

            // Zoom the LevelBmp correctly
            levelBmp = levelBmp.Zoom(ZoomFactor);
            if (picBoxWidth < levelBmp.Width || picBoxHeight < levelBmp.Height)
            {
                levelBmp = levelBmp.Crop(picBoxRect);
            }

            // Add rectangles around selected pieces
            if (IsScreenStart) levelBmp = AddScreenStartRectangle(levelBmp);
            levelBmp = AddSelectedRectangles(levelBmp);
            if (ZoomFactor >= 0 && IsObjectLayer) levelBmp = AddHatchOrder(levelBmp);
            levelBmp = AddMouseSelectionArea(levelBmp);

            // Revert changes to the screen position, until calling it properly
            ScreenPosX = oldScreenPos.X;
            ScreenPosY = oldScreenPos.Y;

            return levelBmp;
        }


        /// <summary>
        /// Gets the size of the displayable area in level coordinates.
        /// </summary>
        /// <returns></returns>
        private Size GetLevelBmpSize()
        {
            return GetLevelBmpSize(picBoxRect.Size);
        }

        /// <summary>
        /// Gets the size of the specified displayable area in level coordinates.
        /// </summary>
        /// <returns></returns>
        private Size GetLevelBmpSize(Size picBoxSize)
        {
            int levelBmpWidth = ApplyUnZoom(picBoxSize.Width);
            int levelBmpHeight = ApplyUnZoom(picBoxSize.Height);

            // Ensure that the LevelBmpSize is at most the size of the level
            levelBmpWidth = Math.Min(levelBmpWidth, level.Width);
            levelBmpHeight = Math.Min(levelBmpHeight, level.Height);

            return new Size(levelBmpWidth, levelBmpHeight);
        }

        /// <summary>
        /// Returns the rectangle of the displayed level area in level coordinates.
        /// </summary>
        /// <returns></returns>
        public Rectangle GetLevelBmpRect(Size picBoxSize)
        {
            return new Rectangle(ScreenPos, GetLevelBmpSize(picBoxSize));
        }

        /// <summary>
        /// Gets the screen start rectangle in level coordinates.
        /// </summary>
        /// <returns></returns>
        public Rectangle ScreenStartRectangle()
        {
            Size levelScreenSize = level.ScreenSize;
            int levelScreenPosX = level.StartPosX - levelScreenSize.Width / 2;
            int levelScreenPosY = level.StartPosY - levelScreenSize.Height / 2;
            levelScreenPosX = Math.Max(Math.Min(levelScreenPosX, level.Width - levelScreenSize.Width), 0);
            levelScreenPosY = Math.Max(Math.Min(levelScreenPosY, level.Height - levelScreenSize.Height), 0);
            return new Rectangle(levelScreenPosX, levelScreenPosY, levelScreenSize.Width, levelScreenSize.Height);
        }

        /// <summary>
        /// Adds the screen start rectangle to the zoomed and cropped image.
        /// </summary>
        /// <param name="levelBmp"></param>
        /// <param name="NegScreenPos"></param>
        /// <returns></returns>
        private Bitmap AddScreenStartRectangle(Bitmap levelBmp)
        {
            Rectangle screenStartRect = GetPicRectFromLevelRect(ScreenStartRectangle());

            Point screenCenterPos = GetPicPointFromLevelPoint(level.StartPos);
            Rectangle screenCenterRect1 = new Rectangle(screenCenterPos.X - 1, screenCenterPos.Y - 1, 3, 3);
            Rectangle screenCenterRect2 = new Rectangle(screenCenterPos.X - 3, screenCenterPos.Y - 3, 7, 7);

            levelBmp.DrawOnRectangles(new List<Rectangle>() { screenStartRect, screenCenterRect1, screenCenterRect2 }, 
                                      C.NLColors[C.NLColor.ScreenStart]);
            return levelBmp;
        }

        /// <summary>
        /// Adds indizes above hatches
        /// </summary>
        /// <param name="levelBmp"></param>
        /// <returns></returns>
        private Bitmap AddHatchOrder(Bitmap levelBmp)
        {
            var hatches = level.GadgetList.FindAll(obj => obj.ObjType == C.OBJ.HATCH);

            for (int hatchIndex = 0; hatchIndex < hatches.Count; hatchIndex++)
            {
                GadgetPiece hatch = hatches[hatchIndex];
                string text = (hatchIndex + 1).ToString() + "/" + hatches.Count.ToString();
                int fontSize = 8 + 2 * ZoomFactor;

                Point levelTextCenterPos = new Point(hatch.PosX + hatch.Width / 2, hatch.PosY);
                Point screenTextCenterPos = GetPicPointFromLevelPoint(levelTextCenterPos);
                screenTextCenterPos.Y -= fontSize;

                levelBmp.WriteText(text, screenTextCenterPos, C.NLColors[C.NLColor.Text], fontSize);
            }

            return levelBmp;
        }

        /// <summary>
        /// Draws rectangles around selected pieces on already zoomed and cropped image.
        /// </summary>
        /// <param name="levelBmp"></param>
        /// <returns></returns>
        private Bitmap AddSelectedRectangles(Bitmap levelBmp)
        {
            // First get a list of all Rectangled to draw (in image coordinates)
            var gadgetRectangles = level.GadgetList.FindAll(gad => gad.IsSelected)
                                                   .ConvertAll(gad => GetPicRectFromLevelRect(gad.ImageRectangle));
            levelBmp.DrawOnRectangles(gadgetRectangles, C.NLColors[C.NLColor.SelRectGadget]);
            
            var TerrRectangles = level.TerrainList.FindAll(ter => ter.IsSelected)
                                                  .ConvertAll(ter => GetPicRectFromLevelRect(ter.ImageRectangle));
            levelBmp.DrawOnRectangles(TerrRectangles, C.NLColors[C.NLColor.SelRectTerrain]);

            return levelBmp;
        }

        /// <summary>
        /// Translates a rectangle in level coordinates into screen coordinates (relative to pic_Level)
        /// </summary>
        /// <param name="origRect"></param>
        /// <returns></returns>
        private Rectangle GetPicRectFromLevelRect(Rectangle origRect)
        {
            int posX = ApplyZoom(origRect.X - ScreenPosX);
            int posY = ApplyZoom(origRect.Y - ScreenPosY);

            int width = ApplyZoom(origRect.Width - 1);
            int height = ApplyZoom(origRect.Height - 1);

            if (ZoomFactor > 0)
            {
                width += ZoomFactor;
                height += ZoomFactor;
            }

            return new Rectangle(posX, posY, width, height);
        }

        /// <summary>
        /// Translates a point in level coordinates into screen coordinates (relative to pic_Level)
        /// </summary>
        /// <param name="origPoint"></param>
        /// <returns></returns>
        private Point GetPicPointFromLevelPoint(Point origPoint)
        {
            int posX = ApplyZoom(origPoint.X - ScreenPosX);
            int posY = ApplyZoom(origPoint.Y - ScreenPosY);
            return new Point(posX, posY);
        }


        /// <summary>
        /// Draws the rectangle around the area currently selected with the mouse.
        /// </summary>
        /// <param name="levelBmp"></param>
        /// <returns></returns>
        private Bitmap AddMouseSelectionArea(Bitmap levelBmp)
        {
            if (MouseDragAction != C.DragActions.SelectArea) return levelBmp;
            if (MouseStartPos == null || MouseCurPos == null) return levelBmp;

            Rectangle mouseRect = Utility.RectangleFrom((Point)MouseStartPos, (Point)MouseCurPos);
            
            // Adapt to borders
            mouseRect.X -= BorderWidth();
            mouseRect.Y -= BorderHeight();

            levelBmp.DrawOnDottedRectangle(mouseRect);
            
            return levelBmp;
        }

        /// <summary>
        /// Moves the screen position in a given direction in approx delta screen pixels.
        /// </summary>
        /// <param name="direction"></param>
        /// <param name="delta"></param>
        public void MoveScreenPos(C.DIR direction, int delta)
        {
            // Adapt delta to zoom level
            int levelDelta = ApplyUnZoom(delta);

            // Move screen position
            switch (direction)
            {
                case C.DIR.E: ScreenPosX += levelDelta; break;
                case C.DIR.W: ScreenPosX -= levelDelta; break;
                case C.DIR.N: ScreenPosY -= levelDelta; break;
                case C.DIR.S: ScreenPosY += levelDelta; break;
            }
            EnsureScreenPosInLevel();
        }

        /// <summary>
        /// Modifies the zoom level and zooms onto the mouse position.
        /// </summary>
        /// <param name="change"></param>
        /// <param name="mouseScreenPos"></param>
        public void ChangeZoom(int change, Point mouseScreenPos)
        {
            Point mouseLevelPos = GetMousePosInLevel(mouseScreenPos);

            int oldZoom = ZoomFactor;
            ZoomFactor = Math.Max(Math.Min(oldZoom + change, C.ZOOM_MAX), C.ZOOM_MIN);

            if (ZoomFactor != oldZoom)
            {
                ScreenPosX = mouseLevelPos.X - ApplyUnZoom(mouseScreenPos.X);
                ScreenPosY = mouseLevelPos.Y - ApplyUnZoom(mouseScreenPos.Y);
                EnsureScreenPosInLevel();
            }
        }

        
        /// <summary>
        /// Modifies the zoom level and adapts the screen position.
        /// </summary>
        /// <param name="change"></param>
        public void ChangeZoom(int change)
        {
            int oldBorderWidth = ApplyUnZoom(BorderWidth());
            int oldBorderHeight = ApplyUnZoom(BorderHeight());
            int oldZoom = ZoomFactor;
            ZoomFactor = Math.Max(Math.Min(oldZoom + change, C.ZOOM_MAX), C.ZOOM_MIN);

            // Change screen position
            float changeFactor;
            if (ZoomFactor + oldZoom > 0) // both at least equal to 0
            {
                changeFactor = ((float)(ZoomFactor - oldZoom)) / ((oldZoom + 1) * (ZoomFactor + 1) * 2);
            }
            else // both at most equal to 0
            {
                changeFactor = ((float)(ZoomFactor - oldZoom)) / 2;
            }
            
            ScreenPosX += (int)(picBoxWidth * changeFactor) - oldBorderWidth;
            ScreenPosY += (int)(picBoxHeight * changeFactor) - oldBorderHeight;
            EnsureScreenPosInLevel();
        }

        /// <summary>
        /// Ensures that the screen position is chosen such that no unnecessary boundaries appear 
        /// </summary>
        public void EnsureScreenPosInLevel()
        {
            ScreenPosX = EnsureScreenPosInLevel(false, ScreenPosX);
            ScreenPosY = EnsureScreenPosInLevel(true, ScreenPosY);
        }

        /// <summary>
        /// Ensures that the screen top resp left position is chosen such that no unnecessary boundaries appear 
        /// </summary>
        /// <param name="isVert"></param>
        private int EnsureScreenPosInLevel(bool isVert, int curPos)
        {
            int levelLength = isVert ? level.Height : level.Width;
            int displayAreaLength = isVert ? GetLevelBmpSize().Height : GetLevelBmpSize().Width;
            int maxCoord = levelLength - displayAreaLength;
            // do not interchange Max and Min because of possibly negative MaxCoord
            return Math.Max(Math.Min(curPos, maxCoord), 0);    
        }

        /// <summary>
        /// Modified the screen position while ensuring that no unnecessary boundaries appear.
        /// </summary>
        public void UpdateScreenPos()
        {
            if (MouseDragAction != C.DragActions.MoveEditorPos) return;
            if (MouseStartPos == null || MouseCurPos == null) return;

            Point newPos = GetNewPosFromDragging();
            ScreenPosX = newPos.X;
            ScreenPosY = newPos.Y;

            EnsureScreenPosInLevel();
        }

        /// <summary>
        /// Returns the difference between the original and the current mouse position in level coordinates.
        /// </summary>
        /// <returns></returns>
        public Point GetNewPosFromDragging()
        {
            System.Diagnostics.Debug.Assert(LevelStartPos != null, "Position for dragging called while reference position is null.");

            int DeltaX = ApplyUnZoom(((Point)MouseCurPos).X - ((Point)MouseStartPos).X);
            int DeltaY = ApplyUnZoom(((Point)MouseCurPos).Y - ((Point)MouseStartPos).Y);

            // Screen positions moves away from dragging direction, everything else in the draggin direction.
            if (MouseDragAction == C.DragActions.MoveEditorPos)
            {
                DeltaX *= -1;
                DeltaY *= -1;
            }

            return new Point(((Point)LevelStartPos).X + DeltaX, ((Point)LevelStartPos).Y + DeltaY);
        }

    }
}
