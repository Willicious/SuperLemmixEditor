using System;
using System.Windows.Forms;

namespace NLEditor
{
    partial class FormPrePostTest : Form
    {
        public FormPrePostTest(Level level, bool ispreText)
        {
            InitializeComponent();

            curLevel = level;
            isPreText = ispreText;
            doSaveOnClosing = true;

            SetControlData();
        }

        Level curLevel;
        bool isPreText;
        bool doSaveOnClosing;

        private void SetControlData()
        {
            this.Text = "Edit " + (isPreText ? "preview" : "postview") + " text";
            lblPTextTitle.Text = "Edit " + (isPreText ? "preview" : "postview") 
                               + " text for level " + curLevel.Title;
            txtPrePostText.Text = isPreText ? string.Join(C.NewLine, curLevel.PreviewText) 
                                            : string.Join(C.NewLine, curLevel.PostviewText); 
        }

        private void butPTextOK_Click(object sender, EventArgs e)
        {
            doSaveOnClosing = true;
            this.Close();
        }

        private void butPTextCancel_Click(object sender, EventArgs e)
        {
            doSaveOnClosing = false;
            Close();
        }

        private void FormPrePostTest_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (doSaveOnClosing)
            {
                if (isPreText) curLevel.PreviewText = txtPrePostText.Text.SplitAtNewLine();
                else curLevel.PostviewText = txtPrePostText.Text.SplitAtNewLine();
            }
        }

        private void FormPrePostTest_Leave(object sender, EventArgs e)
        {
            doSaveOnClosing = true;
            Close();
        }
    }
}
