using System;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace SLXEditor
{
    public class CropTool
    {
        public Rectangle LevelCropRect { get; private set; }
        public bool Active { get; set; }

        private readonly Func<Rectangle, Rectangle> levelToPicRect;
        private readonly Func<Point, Point> picToLevelPoint;
        private readonly Func<Rectangle> getLevelBounds;
        private readonly Func<Point> getLevelDrawOffset;

        private enum DragMode
        {
            None,
            Move,
            ResizeLeft,
            ResizeRight,
            ResizeTop,
            ResizeBottom,
            ResizeTopLeft,
            ResizeTopRight,
            ResizeBottomLeft,
            ResizeBottomRight
        }

        private DragMode dragMode = DragMode.None;
        private Point dragStartLevel;
        private Rectangle originalRect;

        private const int HANDLE_SIZE = 6;
        private const int MIN_WIDTH = 80;
        private const int MIN_HEIGHT = 80;

        public CropTool(
            Func<Rectangle, Rectangle> levelRect,
            Func<Point, Point> levelPoint,
            Func<Rectangle> levelBounds,
            Func<Point> drawOffset)
        {
            this.levelToPicRect = levelRect;
            this.picToLevelPoint = levelPoint;
            this.getLevelBounds = levelBounds;
            this.getLevelDrawOffset = drawOffset;
        }

        public void Start()
        {
            Active = true;
            LevelCropRect = getLevelBounds();
        }

        public void Stop()
        {
            Active = false;
            dragMode = DragMode.None;
        }

        public void Draw(Graphics g)
        {
            if (!Active)
                return;

            Point offset = getLevelDrawOffset();

            Rectangle picRect = levelToPicRect(LevelCropRect);
            picRect.Offset(offset);

            using (Brush darken = new SolidBrush(Color.FromArgb(120, 0, 0, 0)))
            {
                Rectangle full = levelToPicRect(getLevelBounds());
                full.Offset(offset);

                Region outside = new Region(full);
                outside.Exclude(picRect);
                g.FillRegion(darken, outside);
            }

            using (Pen pen = new Pen(Color.Lime, 2))
            {
                pen.DashStyle = DashStyle.Dash;
                g.DrawRectangle(pen, picRect);
            }

            DrawHandles(g, picRect);
        }

        private void DrawHandles(Graphics g, Rectangle rect)
        {
            foreach (Rectangle handle in GetHandleRects(rect))
            {
                g.FillRectangle(Brushes.White, handle);
                g.DrawRectangle(Pens.Black, handle);
            }
        }

        private Rectangle[] GetHandleRects(Rectangle rect)
        {
            int ch = HANDLE_SIZE;     // corner handles
            int eh = HANDLE_SIZE * 8; // edge handles

            return new Rectangle[]
            {
                // Corner handles (squares)
                new Rectangle(rect.Left - ch, rect.Top - ch, ch * 2, ch * 2),      // top-left
                new Rectangle(rect.Right - ch, rect.Top - ch, ch * 2, ch * 2),     // top-right
                new Rectangle(rect.Left - ch, rect.Bottom - ch, ch * 2, ch * 2),   // bottom-left
                new Rectangle(rect.Right - ch, rect.Bottom - ch, ch * 2, ch * 2),  // bottom-right

                // Edge handles (rectangles)
                new Rectangle(rect.Left + rect.Width/2 - eh/2, rect.Top - ch, eh, ch * 2),     // top
                new Rectangle(rect.Left + rect.Width/2 - eh/2, rect.Bottom - ch, eh, ch * 2),  // bottom
                new Rectangle(rect.Left - ch, rect.Top + rect.Height/2 - eh/2, ch * 2, eh),    // left
                new Rectangle(rect.Right - ch, rect.Top + rect.Height/2 - eh/2, ch * 2, eh),   // right
            };
        }

        public void MouseDown(Point picPoint)
        {
            if (!Active)
                return;

            Point offset = getLevelDrawOffset();

            Point adjusted = new Point(picPoint.X - offset.X, picPoint.Y - offset.Y);
            Point levelPoint = picToLevelPoint(adjusted);

            Rectangle picRect = levelToPicRect(LevelCropRect);
            picRect.Offset(offset);

            dragMode = HitTest(picPoint, picRect);

            if (dragMode == DragMode.None)
                return;

            dragStartLevel = levelPoint;
            originalRect = LevelCropRect;
        }

        public void MouseMove(Point picPoint)
        {
            if (!Active || dragMode == DragMode.None)
                return;

            Point offset = getLevelDrawOffset();
            Point adjusted = new Point(
                picPoint.X - offset.X,
                picPoint.Y - offset.Y);

            Point levelPoint = picToLevelPoint(adjusted);
            int dx = levelPoint.X - dragStartLevel.X;
            int dy = levelPoint.Y - dragStartLevel.Y;

            Rectangle r = originalRect;

            switch (dragMode)
            {
                case DragMode.Move:
                    r.Offset(dx, dy);
                    break;

                case DragMode.ResizeLeft:
                    r.X += dx;
                    r.Width -= dx;
                    break;

                case DragMode.ResizeRight:
                    r.Width += dx;
                    break;

                case DragMode.ResizeTop:
                    r.Y += dy;
                    r.Height -= dy;
                    break;

                case DragMode.ResizeBottom:
                    r.Height += dy;
                    break;

                case DragMode.ResizeTopLeft:
                    r.X += dx;
                    r.Width -= dx;
                    r.Y += dy;
                    r.Height -= dy;
                    break;

                case DragMode.ResizeTopRight:
                    r.Width += dx;
                    r.Y += dy;
                    r.Height -= dy;
                    break;

                case DragMode.ResizeBottomLeft:
                    r.X += dx;
                    r.Width -= dx;
                    r.Height += dy;
                    break;

                case DragMode.ResizeBottomRight:
                    r.Width += dx;
                    r.Height += dy;
                    break;
            }

            if (r.Width < MIN_WIDTH)
            {
                if (dragMode == DragMode.ResizeLeft || dragMode == DragMode.ResizeTopLeft || dragMode == DragMode.ResizeBottomLeft)
                    r.X -= MIN_WIDTH - r.Width;
                r.Width = MIN_WIDTH;
            }

            if (r.Height < MIN_HEIGHT)
            {
                if (dragMode == DragMode.ResizeTop || dragMode == DragMode.ResizeTopLeft || dragMode == DragMode.ResizeTopRight)
                    r.Y -= MIN_HEIGHT - r.Height;
                r.Height = MIN_HEIGHT;
            }

            Rectangle bounds = getLevelBounds();
            r = Rectangle.Intersect(r, bounds);

            LevelCropRect = r;
        }

        public void MouseUp()
        {
            dragMode = DragMode.None;
        }

        private DragMode HitTest(Point picPoint, Rectangle picRect)
        {
            Rectangle[] handles = GetHandleRects(picRect);

            // Corner handles
            if (handles[0].Contains(picPoint)) return DragMode.ResizeTopLeft;
            if (handles[1].Contains(picPoint)) return DragMode.ResizeTopRight;
            if (handles[2].Contains(picPoint)) return DragMode.ResizeBottomLeft;
            if (handles[3].Contains(picPoint)) return DragMode.ResizeBottomRight;

            // Edge handles
            if (handles[4].Contains(picPoint)) return DragMode.ResizeTop;
            if (handles[5].Contains(picPoint)) return DragMode.ResizeBottom;
            if (handles[6].Contains(picPoint)) return DragMode.ResizeLeft;
            if (handles[7].Contains(picPoint)) return DragMode.ResizeRight;

            // Inside rectangle
            if (picRect.Contains(picPoint)) return DragMode.Move;

            return DragMode.None;
        }
    }
}
