using System;
using System.Drawing;
using System.Windows.Forms;

namespace SLXEditor
{
    public partial class FormPieceBrowser : Form
    {
        private SLXEditForm mainForm;
        readonly Panel panelPieceBrowser;
        private Settings curSettings;

        private int fixedHeight;
        private int minWidth;

        public event Action PieceBrowserReturned;

        internal FormPieceBrowser(Panel panelPieceBrowserFromMain,
                                  SLXEditForm parentForm,
                                  Settings settings)
        {
            InitializeComponent();

            mainForm = parentForm;
            panelPieceBrowser = panelPieceBrowserFromMain;
            this.curSettings = settings;

            // Ensure interactivity with the main form whilst keeping the window on top
            this.Owner = mainForm;

            // Subscribe to MouseWheel event handler
            this.MouseWheel += FormPieceBrowser_MouseWheel;

            ResetWindowSettings();
            AddControlsToWindow();
        }
        private void ResetWindowSettings()
        {
            fixedHeight = panelPieceBrowser.Height;
            minWidth = mainForm.editorMinWidth + 15;

            this.MinimumSize = new Size(minWidth, fixedHeight);
            this.MaximumSize = new Size(99999, fixedHeight);

            this.Size = new Size(minWidth, fixedHeight);
            this.Location = new Point(40, 500);
        }

        private void ReturnPieceBrowserToMainForm()
        {
            // Notify main form to handle re-parenting
            PieceBrowserReturned?.Invoke();
        }

        internal void AddControlsToWindow()
        {
            // Add panelPieceBrowser from the main form
            this.Controls.Add(panelPieceBrowser);
            mainForm.RepositionPieceBrowser(true, this.Width);
            mainForm.RepositionPicPieces(true, this.Width);
        }

        private bool ValidateScreenSettings(Point location)
        {
            // Check for connected displays
            foreach (var screen in Screen.AllScreens)
            {
                if (screen.WorkingArea.Contains(location))
                {
                    return true;
                }
            }
            return false;
        }

        private void FormPieceBrowser_FormClosing(object sender, FormClosingEventArgs e)
        {
            // Update settings
            curSettings.PieceBrowser.IsOpen = e.CloseReason != CloseReason.UserClosing;
            curSettings.PieceBrowser.IsMaximized = this.WindowState == FormWindowState.Maximized;
            curSettings.PieceBrowser.Location = this.Location;
            curSettings.PieceBrowser.Size = this.Size;
            curSettings.WriteSettingsToFile();

            ReturnPieceBrowserToMainForm();
            mainForm = null;
        }

        private void FormPieceBrowser_KeyDown(object sender, KeyEventArgs e)
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

        private void FormPieceBrowser_KeyUp(object sender, KeyEventArgs e)
        {
            if (mainForm != null)
            {
                mainForm.NLEditForm_KeyUp(this, e);
            }
        }

        private void FormPieceBrowser_MouseWheel(object sender, MouseEventArgs e)
        {
            int movement = e.Delta / SystemInformation.MouseWheelScrollDelta;

            if (mainForm != null)
            {
                mainForm.MoveTerrPieceSelection(movement > 0 ? 1 : -1);
            }
        }

        private void FormPieceBrowser_Resize(object sender, EventArgs e)
        {
            // Don't do anything on minimizing the form!
            if (WindowState == FormWindowState.Minimized)
                return;

            if (mainForm != null)
            {
                mainForm.RepositionPieceBrowser(true, this.Width);
                mainForm.RepositionPicPieces(true, this.Width);
            }
        }

        private void FormPieceBrowser_Shown(object sender, EventArgs e)
        {
            curSettings.PieceBrowser.IsOpen = true;
        }

        private void FormPieceBrowser_Load(object sender, EventArgs e)
        {
            // Size and position the form according to settings
            this.Size = curSettings.PieceBrowser.Size;
            this.Location = curSettings.PieceBrowser.Location;

            // Reset window to default size and position if setting is invalid
            if (!ValidateScreenSettings(this.Location))
            {
                ResetWindowSettings();
            }

            // If the window was maximized, apply maximize to ensure correct sizing
            if (curSettings.PieceBrowser.IsMaximized)
            {
                this.WindowState = FormWindowState.Maximized;
            }
        }
    }
}
