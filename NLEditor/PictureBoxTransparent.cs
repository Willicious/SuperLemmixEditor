using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Drawing;

namespace NLEditor
{
    /// <summary>
    /// A picturebox whose background is transparent.
    /// </summary>
    class PictureBoxTransparent : PictureBox
    {
        public PictureBoxTransparent() : base()
        {
            this.SetStyle(ControlStyles.AllPaintingInWmPaint
                        | ControlStyles.OptimizedDoubleBuffer
                        | ControlStyles.ResizeRedraw
                        | ControlStyles.DoubleBuffer
                        | ControlStyles.UserPaint, true);

            this.BackColor = Color.Black;
            //this.LocationChanged += new EventHandler((object sender, EventArgs e) => RedrawBackground());
        }
        
        /// <summary>
        /// Draws the control to the Graphics object.
        /// </summary>
        /// <param name="control"></param>
        private void DrawControlToGraphics(Graphics graphics, Control control)
        {
            // Check it's visible and overlaps this control
            if (control.Bounds.IntersectsWith(Bounds) && control.Visible)
            {
                // Load appearance of underlying control and redraw it on this background
                using (Bitmap bmp = new Bitmap(control.Width, control.Height, graphics))
                {
                    control.DrawToBitmap(bmp, control.ClientRectangle);
                    graphics.TranslateTransform(control.Left - Left, control.Top - Top);
                    graphics.DrawImageUnscaled(bmp, Point.Empty);
                    graphics.TranslateTransform(Left - control.Left, Top - control.Top);
                }
            }
        }

        private void RedrawBackground()
        {
            if (Parent == null) return;

            this.BackColor = Parent.BackColor;

            Bitmap bgBmp = new Bitmap(Width, Height);

            using (Graphics background = Graphics.FromImage(bgBmp))
            {
                // Take each control in turn
                int picBoxIndex = Parent.Controls.GetChildIndex(this);
                for (int i = Parent.Controls.Count - 1; i > picBoxIndex; i--)
                {
                    DrawControlToGraphics(background, Parent.Controls[i]);
                }
            }
            this.BackgroundImage = bgBmp;
            this.Refresh();
        }
    }
}
