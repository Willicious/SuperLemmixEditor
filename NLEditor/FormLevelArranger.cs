using System;
using System.Drawing;
using System.Windows.Forms;

namespace NLEditor
{
    public partial class FormLevelArranger : Form
    {
        private NLEditForm mainForm;
        readonly PictureBox picLevel;
        readonly Renderer curRenderer;

        HScrollBar scrollHoriz;
        VScrollBar scrollVert;

        Rectangle originalLevelSize;

        public event Action PicLevelReturned;

        internal FormLevelArranger(PictureBox picLevelFromMain,
                                   NLEditForm parentForm,
                                   Renderer parentRenderer)
        {
            InitializeComponent();
            
            mainForm = parentForm; 
            picLevel = picLevelFromMain;
            curRenderer = parentRenderer;

            originalLevelSize = mainForm.CurLevel.Size;

            // Ensure interactivity with the main form whilst keeping the window on top
            this.Owner = mainForm;

            // Subscribe to MouseWheel event handler
            this.MouseWheel += FormLevelArranger_MouseWheel;

            AddControlsToWindow();
        }

        private void ReturnPicLevelToMainForm()
        {
            // Notify main form to handle re-parenting
            PicLevelReturned?.Invoke();
        }

        internal void AddControlsToWindow()
        {
            // Add picLevel from the main form
            this.Controls.Add(picLevel);
            picLevel.Dock = DockStyle.Fill;

            // Match background color to picLevel's background (prevents flickering)
            this.BackColor = picLevel.BackColor;

            // Add scrollbars
            scrollHoriz = new HScrollBar();
            scrollVert = new VScrollBar();

            this.Controls.Add(scrollHoriz);
            this.Controls.Add(scrollVert);

            scrollHoriz.Dock = DockStyle.Bottom;
            scrollVert.Dock = DockStyle.Right;

            scrollHoriz.Scroll += ScrollHoriz_Scroll;
            scrollVert.Scroll += ScrollVert_Scroll;

            UpdateScrollbars();
        }

        private void ResetPicLevel()
        {
            picLevel.SetImage(curRenderer.GetScreenImage());
        }

        /// <summary>
        /// Much of this is duplicated from UpdateNLEditorForm's CheckEnableLevelScrollbars,
        /// but it's necessary to make the scrollbars work properly!
        /// </summary>
        private void UpdateScrollbars()
        {
            // Exit early if nothing has been initialized yet
            if (picLevel == null || curRenderer == null || mainForm == null)
                return;

            if (scrollHoriz == null || scrollVert == null)
                return;

            // Get the size of the displayed part of the level
            Rectangle displayedLevelRect = curRenderer.GetLevelBmpRect();

            bool displayScrollHoriz = false;
            bool displayScrollVert = false;

            // Display scrollbars if the level has been zoomed in
            displayScrollHoriz = (displayedLevelRect.Width + 1 < originalLevelSize.Width);
            displayScrollVert = (displayedLevelRect.Height + 1 < originalLevelSize.Height);

            // Check whether zooming out made either scrollbar necessary, too
            if (displayScrollHoriz ^ displayScrollVert)
            {
                displayedLevelRect = curRenderer.GetLevelBmpRect();
                if (!displayScrollHoriz && displayedLevelRect.Width + 1 < originalLevelSize.Width)
                {
                    displayScrollHoriz = true;
                }
                if (!displayScrollVert && displayedLevelRect.Height + 1 < originalLevelSize.Height)
                {
                    displayScrollVert = true;
                }
            }

            // Update displayed level area
            displayedLevelRect = curRenderer.GetLevelBmpRect();

            // Set horizontal scrollbar
            if (displayScrollHoriz)
            {
                int maxValue = originalLevelSize.Width + (Renderer.AllowedGrayBorder + 18) - displayedLevelRect.Width + 1;
                scrollHoriz.Minimum = -Renderer.AllowedGrayBorder;
                scrollHoriz.Maximum = maxValue;
                scrollHoriz.SmallChange = 8;
                scrollHoriz.LargeChange = 16;
                scrollHoriz.Height = 24;
                scrollHoriz.Value = Math.Max(Math.Min(displayedLevelRect.Left, maxValue - 1), -Renderer.AllowedGrayBorder);
            }
            scrollHoriz.Enabled = displayScrollHoriz;
            scrollHoriz.Visible = displayScrollHoriz;

            // Set vertical scrollbar
            if (displayScrollVert)
            {
                int maxValue = originalLevelSize.Height + (Renderer.AllowedGrayBorder + 8) - displayedLevelRect.Height + 1;
                scrollVert.Minimum = -Renderer.AllowedGrayBorder;
                scrollVert.Maximum = maxValue;
                scrollVert.SmallChange = 4;
                scrollVert.LargeChange = 8;
                scrollVert.Width = 24;
                scrollVert.Value = Math.Max(Math.Min(displayedLevelRect.Top, maxValue - 1), -Renderer.AllowedGrayBorder);
            }
            scrollVert.Enabled = displayScrollVert;
            scrollVert.Visible = displayScrollVert;
        }

        private void FormLevelArranger_FormClosing(object sender, FormClosingEventArgs e)
        {
            // Update settings
            Properties.Settings.Default.LevelArrangerIsOpen = e.CloseReason != CloseReason.UserClosing;
            Properties.Settings.Default.LevelArrangerSize = this.Size;
            Properties.Settings.Default.LevelArrangerLocation = this.Location;
            Properties.Settings.Default.LevelArrangerIsMaximized = this.WindowState == FormWindowState.Maximized;
            Properties.Settings.Default.Save();

            ReturnPicLevelToMainForm();
            mainForm = null;
        }

        private void FormLevelArranger_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                Close();
            }
            else if (mainForm != null)
            {
                mainForm.NLEditForm_KeyDown(this, e);
            }
        }

        private void FormLevelArranger_KeyUp(object sender, KeyEventArgs e)
        {
            if (mainForm != null)
            {
                mainForm.NLEditForm_KeyUp(this, e);
            }
        }

        private void FormLevelArranger_MouseWheel(object sender, MouseEventArgs e)
        {
            if (mainForm != null)
            {
                mainForm.NLEditForm_MouseWheel(this, e);
            }

            UpdateScrollbars();
        }

        private void FormLevelArranger_Resize(object sender, EventArgs e)
        {
            UpdateScrollbars();
        }

        private void ScrollHoriz_Scroll(object sender, ScrollEventArgs e)
        {
            curRenderer.ScreenPosX = e.NewValue;
            ResetPicLevel();
        }

        private void ScrollVert_Scroll(object sender, ScrollEventArgs e)
        {
            curRenderer.ScreenPosY = e.NewValue;
            ResetPicLevel();
        }

        private void FormLevelArranger_Shown(object sender, EventArgs e)
        {
            ResetPicLevel();
        }

        private void FormLevelArranger_Load(object sender, EventArgs e)
        {
            // Size and position the form according to settings
            this.Size = Properties.Settings.Default.LevelArrangerSize;
            this.Location = Properties.Settings.Default.LevelArrangerLocation;

            // If the window was maximized, apply maximize to ensure correct sizing
            if (Properties.Settings.Default.LevelArrangerIsMaximized)
            {
                this.WindowState = FormWindowState.Maximized;
            }
        }
    }
}
