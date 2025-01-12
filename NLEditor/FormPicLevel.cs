using System;
using System.Windows.Forms;

namespace NLEditor
{
    public partial class FormPicLevel : Form
    {
        private NLEditForm mainForm;

        readonly PictureBox picLevel;
        readonly ScrollBar scrollBarHoriz;
        readonly ScrollBar scrollBarVert;

        public event Action PicLevelReturned;

        internal FormPicLevel(PictureBox picLevelFromMain,
                              ScrollBar scrollHorizFromMain,
                              ScrollBar scrollVertFromMain,
                              NLEditForm parentForm)
        {
            InitializeComponent();
            
            // Store the reference to the main form
            mainForm = parentForm; 
            
            // Set picLevel and scrollbars to those passed from the main form
            picLevel = picLevelFromMain; 
            scrollBarHoriz = scrollHorizFromMain;
            scrollBarVert = scrollVertFromMain;

            // Ensure interactivity with the main form whilst keeping the window on top
            this.Owner = mainForm;
        }

        private void ReturnPicLevelToMainForm()
        {
            //// Remove the PictureBox from the pop-out window
            //this.Controls.Remove(picLevel);
            //this.Controls.Remove(scrollBarVert);
            //this.Controls.Remove(scrollBarHoriz);

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

            this.MouseWheel += FormPicLevel_MouseWheel;

            //// Add the scrollbars
            //this.Controls.Add(scrollBarHoriz);
            //this.Controls.Add(scrollBarVert);
        }

        private void FormPicLevel_FormClosing(object sender, FormClosingEventArgs e)
        {
            ReturnPicLevelToMainForm();
            mainForm = null;
        }

        private void FormPicLevel_KeyDown(object sender, KeyEventArgs e)
        {
            if (mainForm != null)
            {
                mainForm.NLEditForm_KeyDown(this, e);
            }
        }

        private void FormPicLevel_KeyUp(object sender, KeyEventArgs e)
        {
            if (mainForm != null)
            {
                mainForm.NLEditForm_KeyUp(this, e);
            }
        }

        private void FormPicLevel_MouseWheel(object sender, MouseEventArgs e)
        {
            if (mainForm != null)
            {
                mainForm.NLEditForm_MouseWheel(this, e);
            }
        }
    }
}
