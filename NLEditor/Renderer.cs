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
      this.ScreenPosX = 0;
      this.ScreenPosY = 0;
      this.ZoomFactor = 0;

      this.levelPicBox = pic_Level;

      SetLevel(level);
      ClearLayers();
    }

    public const int AllowedGrayBorder = 10;

    Dictionary<C.Layer, Bitmap> layerImages;
    Bitmap baseLevelImage;
    Level level;
    bool IsClearPhysics => DisplaySettings.IsDisplayed(C.DisplayType.ClearPhysics);
    bool IsTerrainLayer => DisplaySettings.IsDisplayed(C.DisplayType.Terrain);
    bool IsObjectLayer => DisplaySettings.IsDisplayed(C.DisplayType.Objects);
    bool IsTriggerLayer => DisplaySettings.IsDisplayed(C.DisplayType.Trigger);
    bool IsScreenStart => DisplaySettings.IsDisplayed(C.DisplayType.ScreenStart);
    bool IsBackgroundLayer => DisplaySettings.IsDisplayed(C.DisplayType.Background);

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

    Point zoomMouseScreenPos;
    Point zoomMouseLevelPos;

    public void Dispose()
    {
      layerImages?.Values.ToList().ForEach(bmp => bmp.Dispose());
      baseLevelImage?.Dispose();
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
    /// Combines and crops stored layers and returns the image to display on the screen.
    /// </summary>
    /// <returns></returns>
    public Bitmap CombineLayers(string dragNewPieceKey = null)
    {
      // Create the base level image
      CreateLevelImageFromLayers(dragNewPieceKey);

      // Crop the whole level and add the editor helpers
      return GetScreenImage();
    }

    /// <summary>
    /// Create the screen image from the zoomed level
    /// </summary>
    /// <returns></returns>
    public Bitmap GetScreenImage()
    {
      return CreateScreenImage();
    }

    /// <summary>
    /// Combine the layers to the (correctly zoomed) level image
    /// </summary>
    private void CreateLevelImageFromLayers(string dragNewPieceKey = null)
    {
      // Dispose existing baseLevelImage
      baseLevelImage?.Dispose();

      // Create new baseLevelImage
      if (IsClearPhysics)
      {
        // Always use a black background here
        baseLevelImage = new Bitmap(level.Width, level.Height);
      }
      else if (IsBackgroundLayer)
      {
        baseLevelImage = (Bitmap)layerImages[C.Layer.Background].Clone();
      }
      else
      {
        // Still use background color
        baseLevelImage = new Bitmap(level.Width, level.Height);
        baseLevelImage.Clear(level.MainStyle?.GetColor(C.StyleColor.BACKGROUND) ?? C.NLColors[C.NLColor.BackDefault]);
      }

      // Draw all the layers
      if (IsObjectLayer)
      {
        baseLevelImage.DrawOn(layerImages[C.Layer.ObjBack]);
      }

      if (IsTerrainLayer)
      {
        baseLevelImage.DrawOn(layerImages[C.Layer.Terrain]);
      }

      if (dragNewPieceKey != null && MouseDragAction == C.DragActions.DragNewPiece
                                  && IsPointInLevelArea(MouseCurPos))
      {
        AddDragNewPiece(baseLevelImage, dragNewPieceKey, new Point(0, 0));
      }

      if (IsObjectLayer)
      {
        baseLevelImage.DrawOn(layerImages[C.Layer.ObjTop]);
      }

      if (IsTriggerLayer)
      {
        baseLevelImage.DrawOnWithAlpha(layerImages[C.Layer.Trigger]);
      }
    }

    /// <summary>
    /// Creates the screen image from the zoomed level image
    /// </summary>
    /// <returns></returns>
    private Bitmap CreateScreenImage()
    {
      UpdateScreenPos();

      // Create the rectangle where to crop the zoomedLevelImage
      Size levelBmpSize = GetLevelBmpSize();
      Rectangle screenBmpRect = new Rectangle(ScreenPosX, ScreenPosY, levelBmpSize.Width, levelBmpSize.Height);
      Bitmap croppedBmp = baseLevelImage.Crop(screenBmpRect);

      // Zoom this image
      Bitmap screenBmp = croppedBmp.Zoom(ZoomFactor);

      // Add rectangles around selected pieces
      if (IsScreenStart) AddScreenStartRectangle(ref screenBmp);
      AddSelectedRectangles(ref screenBmp);
      if (ZoomFactor >= 0 && IsObjectLayer) AddHatchOrder(ref screenBmp);
      AddMouseSelectionArea(ref screenBmp);

      // Embed the screen image in a bitmap of the size of the whole picture box.
      Bitmap fullBmp = new Bitmap(picBoxWidth, picBoxHeight);
      fullBmp.Clear(Color.FromArgb(0, 0, 0, 0));
      int levelPosX = DoesFitLevelHorizontally()
                      ? (picBoxWidth - screenBmp.Width) / 2
                      : Math.Max(-ApplyZoom(ScreenPosX), 0);
      int levelPosY = DoesFitLevelVertically()
                      ? (picBoxHeight - screenBmp.Height) / 2
                      : Math.Max(-ApplyZoom(ScreenPosY), 0);
      Point levelPos = new Point(levelPosX, levelPosY);
      fullBmp.DrawOn(screenBmp, levelPos);

      // Add selection coordinates it applicable
      if (level.SelectionList()?.Count > 0) AddSelectionCoordinates(ref fullBmp);

      // Dispose the single screen bitmap
      croppedBmp.Dispose();
      screenBmp.Dispose();

      return fullBmp;
    }

    private void ClearLayers()
    {
      Dispose();
      layerImages = C.LayerList.ToDictionary(layer => layer, layer => new Bitmap(level.Width, level.Height));
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
    /// Returns whether the whole level fits into the picturebox horizontally
    /// </summary>
    /// <returns></returns>
    private bool DoesFitLevelHorizontally()
    {
      return picBoxWidth > ApplyZoom(baseLevelImage?.Width ?? 0);
    }

    /// <summary>
    /// Returns whether the whole level fits into the picturebox vertically
    /// </summary>
    /// <returns></returns>
    private bool DoesFitLevelVertically()
    {
      return picBoxHeight > ApplyZoom(baseLevelImage?.Height ?? 0);
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
    public Point GetMousePosInLevel(Point mouseScreenPos, bool doCropToLevelArea = true)
    {
      // Adapt to images that do not fill the whole pic_Level and to Mouse positions outside the level
      int mouseScreenPosX;
      int mouseScreenPosY;
      if (doCropToLevelArea)
      {
        mouseScreenPosX = Math.Min(Math.Max(mouseScreenPos.X, BorderWidth()), picBoxWidth - BorderWidth())
                            - BorderWidth();
        mouseScreenPosY = Math.Min(Math.Max(mouseScreenPos.Y, BorderHeight()), picBoxHeight - BorderHeight())
                            - BorderHeight();
      }
      else
      {
        mouseScreenPosX = mouseScreenPos.X - BorderWidth();
        mouseScreenPosY = mouseScreenPos.Y - BorderHeight();
      }

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

      var backgroundGadgets = level.GadgetList.FindAll(obj => obj.ObjType == C.OBJ.BACKGROUND && !obj.IsOnlyOnTerrain);
      foreach (GadgetPiece gadget in backgroundGadgets)
      {
        layerImages[C.Layer.ObjBack].DrawOn(gadget.Image, gadget.Pos);
      }

      var backGadgets = level.GadgetList.FindAll(obj => obj.IsNoOverwrite && obj.ObjType != C.OBJ.BACKGROUND);
      backGadgets.Reverse();
      foreach (GadgetPiece gadget in backGadgets)
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
              !gad.IsNoOverwrite && !gad.IsOnlyOnTerrain && !gad.ObjType.In(C.OBJ.ONE_WAY_WALL, C.OBJ.BACKGROUND));
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
    public Rectangle GetLevelBmpRect()
    {
      return new Rectangle(ScreenPos, GetLevelBmpSize(new Size(picBoxWidth, picBoxHeight)));
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
    private void AddScreenStartRectangle(ref Bitmap levelBmp)
    {
      Rectangle screenStartRect = GetPicRectFromLevelRect(ScreenStartRectangle());

      Point screenCenterPos = GetPicPointFromLevelPoint(level.StartPos);
      Rectangle screenCenterRect1 = new Rectangle(screenCenterPos.X - 1, screenCenterPos.Y - 1, 3, 3);
      Rectangle screenCenterRect2 = new Rectangle(screenCenterPos.X - 3, screenCenterPos.Y - 3, 7, 7);

      levelBmp.DrawOnRectangles(new List<Rectangle>() { screenStartRect, screenCenterRect1, screenCenterRect2 },
                                C.NLColors[C.NLColor.ScreenStart]);
    }

    /// <summary>
    /// Adds indizes above hatches
    /// </summary>
    /// <param name="levelBmp"></param>
    /// <returns></returns>
    private void AddHatchOrder(ref Bitmap levelBmp)
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
    }

    /// <summary>
    /// Adds the selection coordinates at the bottom right of the level image.
    /// </summary>
    /// <param name="levelBmp"></param>
    /// <returns></returns>
    private void AddSelectionCoordinates(ref Bitmap fullBmp)
    {
      Rectangle selectRect = level.SelectionRectangle();
      string text = selectRect.X.ToString() + "/" + selectRect.Y.ToString();
      Point textPos = new Point(picBoxWidth + 7, picBoxHeight + 3);

      fullBmp.WriteText(text, textPos, C.NLColors[C.NLColor.Text], 10, ContentAlignment.BottomRight);
    }

    /// <summary>
    /// Draws rectangles around selected pieces on already zoomed and cropped image.
    /// </summary>
    /// <param name="levelBmp"></param>
    /// <returns></returns>
    private void AddSelectedRectangles(ref Bitmap levelBmp)
    {
      // First get a list of all Rectangled to draw (in image coordinates)
      var gadgetRectangles = level.GadgetList.FindAll(gad => gad.IsSelected)
                                             .ConvertAll(gad => GetPicRectFromLevelRect(gad.ImageRectangle));
      levelBmp.DrawOnRectangles(gadgetRectangles, C.NLColors[C.NLColor.SelRectGadget]);

      var terrRectangles = level.TerrainList.FindAll(ter => ter.IsSelected)
                                            .ConvertAll(ter => GetPicRectFromLevelRect(ter.ImageRectangle));
      levelBmp.DrawOnRectangles(terrRectangles, C.NLColors[C.NLColor.SelRectTerrain]);
    }

    /// <summary>
    /// Translates a rectangle in level coordinates into screen coordinates (relative to pic_Level)
    /// </summary>
    /// <param name="origRect"></param>
    /// <returns></returns>
    private Rectangle GetPicRectFromLevelRect(Rectangle origRect)
    {
      int posX = ApplyZoom(origRect.X - Math.Max(ScreenPosX, 0));
      int posY = ApplyZoom(origRect.Y - Math.Max(ScreenPosY, 0));

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
      int posX = ApplyZoom(origPoint.X - Math.Max(ScreenPosX, 0));
      int posY = ApplyZoom(origPoint.Y - Math.Max(ScreenPosY, 0));
      return new Point(posX, posY);
    }


    /// <summary>
    /// Draws the rectangle around the area currently selected with the mouse.
    /// </summary>
    /// <param name="levelBmp"></param>
    /// <returns></returns>
    private void AddMouseSelectionArea(ref Bitmap levelBmp)
    {
      if (MouseDragAction != C.DragActions.SelectArea) return;
      if (MouseStartPos == null || MouseCurPos == null) return;

      Rectangle mouseRect = Utility.RectangleFrom((Point)MouseStartPos, (Point)MouseCurPos);

      // Adapt to borders
      mouseRect.X -= BorderWidth();
      mouseRect.Y -= BorderHeight();

      levelBmp.DrawOnDottedRectangle(mouseRect);
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
    /// Sets the point to zoom into
    /// </summary>
    /// <param name="mouseScreenPos"></param>
    public void SetZoomMousePos(Point mouseScreenPos)
    {
      if (level.Size.Contains(GetMousePosInLevel(mouseScreenPos)))
      {
        zoomMouseScreenPos = mouseScreenPos;
        zoomMouseLevelPos = GetMousePosInLevel(mouseScreenPos);
      }
      else
      {
        zoomMouseScreenPos = new Point(-1, -1);
        zoomMouseLevelPos = new Point(-1, -1);
      }
    }

    /// <summary>
    /// Modifies the zoom level and zooms onto the mouse position.
    /// </summary>
    /// <param name="change"></param>
    /// <param name="mouseScreenPos"></param>
    void ChangeZoomAtMousePos(int change)
    {
      int oldZoom = ZoomFactor;
      ZoomFactor = Math.Max(Math.Min(oldZoom + change, C.ZOOM_MAX), C.ZOOM_MIN);

      if (ZoomFactor != oldZoom)
      {
        ScreenPosX = zoomMouseLevelPos.X - ApplyUnZoom(zoomMouseScreenPos.X);
        ScreenPosY = zoomMouseLevelPos.Y - ApplyUnZoom(zoomMouseScreenPos.Y);
        EnsureScreenPosInLevel();
      }
    }

    /// <summary>
    /// Modifies the zoom level, zooming in at the center and adapts the screen position.
    /// </summary>
    /// <param name="change"></param>
    void ChangeZoomAtCenter(int change)
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
    /// Modifies the zoom level
    /// </summary>
    /// <param name="change"></param>
    public void ChangeZoom(int change, bool mayCenterAtMouse = false)
    {
      if (mayCenterAtMouse && picBoxRect.Contains(zoomMouseScreenPos))
      {
        ChangeZoomAtMousePos(change);
      }
      else
      {
        ChangeZoomAtCenter(change);
      }
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
      bool doAllowBorder = isVert ? !DoesFitLevelVertically() : !DoesFitLevelHorizontally();
      int allowedBorder = doAllowBorder ? AllowedGrayBorder : 0;
      return Math.Max(Math.Min(curPos, maxCoord + allowedBorder), -allowedBorder);
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
