using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using static System.Windows.Forms.LinkLabel;

namespace NLEditor
{
    partial class FormPrePostText : Form
    {
        public FormPrePostText(Level level, bool ispreText)
        {
            KeyPreview = true;

            InitializeComponent();

            curLevel = level;
            isPreText = ispreText;
            doSaveOnClosing = true;

            SetControlData();
        }

        Level curLevel;
        bool isPreText;
        bool doSaveOnClosing;

        private const int MaxLines = 21;

        private void SetControlData()
        {
            SetLabel();

            txtPrePostText.Text = isPreText ? string.Join(C.NewLine, curLevel.PreviewText)
                                            : string.Join(C.NewLine, curLevel.PostviewText);
        }
        private void SetLabel()
        {
            lblPTextTitle.ForeColor = System.Drawing.Color.Black;

            this.Text = "Edit " + (isPreText ? "preview" : "postview") + " text";
            lblPTextTitle.Text = "Edit " + (isPreText ? "preview" : "postview")
                               + " text for level " + curLevel.Title;
        }

        /// <summary>
        /// Detect auto-wrapped lines
        /// </summary>
        private List<int> GetWrappingPoints(TextBox textBox)
        {
            var wrappingPoints = new List<int>();
            int lineIndex = 0;

            while (true)
            {
                // Get the start of the next visual line
                int startIndex = textBox.GetFirstCharIndexFromLine(lineIndex);

                if (startIndex == -1)
                    break;

                // Get the end of the line (next line's start - 1)
                int endIndex = textBox.GetFirstCharIndexFromLine(lineIndex + 1) - 1;

                if (endIndex == -2) // No next line
                {
                    endIndex = textBox.Text.Length - 1;
                }

                // Add the end index of the current visual line
                wrappingPoints.Add(endIndex);

                lineIndex++;
            }

            return wrappingPoints;
        }

        /// <summary>
        /// Insert '\n' at wrapping points to prevent lines overshooting the edge of the screen
        /// </summary>
        private string PrepareWrappedText(TextBox textBox)
        {
            if (string.IsNullOrWhiteSpace(textBox.Text))
            {
                return string.Empty;
            }

            var wrappingPoints = GetWrappingPoints(textBox);
            var text = textBox.Text;
            var builder = new StringBuilder(text);

            int offset = 0; // Track insertion offset to adjust for already-inserted characters
            foreach (var point in wrappingPoints)
            {
                if (point + offset < builder.Length && builder[point + offset] != '\n')
                {
                    builder.Insert(point + offset + 1, '\n'); // Insert '\n' after the wrapping point
                    offset++;
                }
            }

            return builder.ToString();
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

        private void FormPrePostTest_Leave(object sender, EventArgs e)
        {
            doSaveOnClosing = true;
            Close();
        }

        private void FormPrePostText_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                doSaveOnClosing = false;
                Close();
            }
        }

        private void FormPrePostTest_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (doSaveOnClosing)
            {
                var wrappedText = PrepareWrappedText(txtPrePostText);

                if (isPreText)
                    curLevel.PreviewText = wrappedText.SplitAtNewLine();
                else
                    curLevel.PostviewText = wrappedText.SplitAtNewLine();
            }
        }

        private void txtPrePostText_TextChanged(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtPrePostText.Text))
            {
                return;
            }

            var lines = txtPrePostText.Text.Split(new[] { "\r\n", "\n" }, StringSplitOptions.None);

            if (lines.Length > MaxLines)
            {
                // Truncate to the first 21 lines
                txtPrePostText.Text = string.Join(Environment.NewLine, lines.Take(MaxLines));
                txtPrePostText.SelectionStart = txtPrePostText.Text.Length; // Move caret to the end
            }
        }

        private void butPPreview_Click(object sender, EventArgs e)
        {
            var previewText = PrepareWrappedText(txtPrePostText);
            
            // Add blank lines at the top to centralize the text
            var lines = previewText.Split(new[] { "\r\n", "\n" }, StringSplitOptions.None).ToList();

            int blankLinesToAdd = (MaxLines - lines.Count) / 2;
            
            for (int i = 0; i < blankLinesToAdd; i++)
            {
                lines.Insert(0, string.Empty);
            }

            previewText = string.Join(Environment.NewLine, lines);

            // Create a form with a non-editable TextBox to display the preview text
            Form previewForm = new Form
            {
                Text = "Preview",
                Size = new Size(408, 348),
                StartPosition = FormStartPosition.CenterParent,
                FormBorderStyle = FormBorderStyle.FixedDialog,
                MaximizeBox = false,
                MinimizeBox = false
            };
            
            TextBox previewTextBox = new TextBox
            {
                Multiline = true,
                ReadOnly = true,
                Enabled = false,
                TextAlign = HorizontalAlignment.Center,
                Dock = DockStyle.Fill,
                Text = previewText,
                Font = txtPrePostText.Font
            };

            previewForm.Controls.Add(previewTextBox);
            previewForm.ShowDialog(this);
        }

        private void butPClearText_Click(object sender, EventArgs e)
        {
            txtPrePostText.Clear();
            txtPrePostText.Focus();
        }
    }
}
