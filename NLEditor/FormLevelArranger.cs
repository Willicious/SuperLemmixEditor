using System;
using System.Windows.Forms;

namespace NLEditor
{
    public partial class FormLevelArranger : Form
    {
        private NLEditForm mainForm;
        readonly PictureBox picLevel;

        public event Action PicLevelReturned;

        internal FormLevelArranger(PictureBox picLevelFromMain, NLEditForm parentForm)
        {
            InitializeComponent();
            
            mainForm = parentForm; 
            picLevel = picLevelFromMain;

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
        }

        private void FormLevelArranger_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing)
            {
                Properties.Settings.Default.LevelArrangerIsOpen = false;
                Properties.Settings.Default.Save();
            }

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
        }
    }
}
