using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NLEditor
{
    public partial class FormPieceBrowser : Form
    {
        private NLEditForm mainForm;
        readonly Panel panelPieceBrowser;

        public event Action PieceBrowserReturned;

        internal FormPieceBrowser(Panel panelPieceBrowserFromMain, NLEditForm parentForm)
        {
            InitializeComponent();

            mainForm = parentForm;
            panelPieceBrowser = panelPieceBrowserFromMain;

            // Ensure interactivity with the main form whilst keeping the window on top
            this.Owner = mainForm;

            // Subscribe to MouseWheel event handler
            this.MouseWheel += FormPieceBrowser_MouseWheel;

            ResetWindowSettings();
            AddControlsToWindow();
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
            mainForm.RepositionPieceBrowser(true);
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

        private void ResetWindowSettings()
        {
            this.Size = new Size(500, 200);
            this.Location = new Point(280, 40);
        }

        private void FormPieceBrowser_FormClosing(object sender, FormClosingEventArgs e)
        {
            // Update settings
            //Properties.Settings.Default.LevelArrangerIsOpen = e.CloseReason != CloseReason.UserClosing;
            //Properties.Settings.Default.LevelArrangerSize = this.Size;
            //Properties.Settings.Default.LevelArrangerLocation = this.Location;
            //Properties.Settings.Default.LevelArrangerIsMaximized = this.WindowState == FormWindowState.Maximized;
            //Properties.Settings.Default.Save();

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
            // Add more pic pieces? Reposition controls?
        }

        private void FormPieceBrowser_Shown(object sender, EventArgs e)
        {
            //Properties.Settings.Default.LevelArrangerIsOpen = true;
        }

        private void FormPieceBrowser_Load(object sender, EventArgs e)
        {
            // Size and position the form according to settings
            //this.Size = Properties.Settings.Default.LevelArrangerSize;
            //this.Location = Properties.Settings.Default.LevelArrangerLocation;

            // Reset window to default size and position if setting is invalid
            if (!ValidateScreenSettings(this.Location))
            {
                ResetWindowSettings();
            }

            //// If the window was maximized, apply maximize to ensure correct sizing
            //if (Properties.Settings.Default.LevelArrangerIsMaximized)
            //{
            //    this.WindowState = FormWindowState.Maximized;
            //}
        }
    }
}
