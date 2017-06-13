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
        }
        
        
        
        /// <summary>
        /// Paint background with underlying graphics from other controls
        /// </summary>
        /// <param name="e"></param>
        protected override void OnPaintBackground(PaintEventArgs e)
        {
            /*if (Parent != null)
            {
                using (Graphics tempGraphics = e.Graphics)
                {
                    // Take each control in turn
                    int picBoxIndex = Parent.Controls.GetChildIndex(this);
                    for (int i = Parent.Controls.Count - 1; i > picBoxIndex; i--)
                    {
                        DrawControlToGraphics(tempGraphics, Parent.Controls[i]);
                    }
                }
            }
            else
            {
                base.OnPaintBackground(e);
            }*/
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

    }
}
