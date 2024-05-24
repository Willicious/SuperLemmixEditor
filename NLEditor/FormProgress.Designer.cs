using System;
using System.Windows.Forms;

namespace NLEditor
{
    public partial class FormProgress : Form
    {
        public ProgressBar ProgressBar => progressBar;
        public void UpdateProgress(int progress, string status)
        {
            if (progressBar.InvokeRequired || labelStatus.InvokeRequired)
            {
                progressBar.Invoke((Action)(() =>
                {
                    // Ensure progress value is within the valid range
                    progress = Math.Max(progressBar.Minimum, Math.Min(progressBar.Maximum, progress));

                    progressBar.Value = progress;
                    labelStatus.Text = status;
                }));
            }
            else
            {
                // Ensure progress value is within the valid range
                progress = Math.Max(progressBar.Minimum, Math.Min(progressBar.Maximum, progress));

                progressBar.Value = progress;
                labelStatus.Text = status;
            }
        }


        private void InitializeComponent()
        {
            this.labelStatus = new System.Windows.Forms.Label();
            this.progressBar = new System.Windows.Forms.ProgressBar();
            this.SuspendLayout();
            // 
            // labelStatus
            // 
            this.labelStatus.AutoSize = true;
            this.labelStatus.Location = new System.Drawing.Point(22, 27);
            this.labelStatus.Name = "labelStatus";
            this.labelStatus.Size = new System.Drawing.Size(56, 20);
            this.labelStatus.TabIndex = 0;
            this.labelStatus.Text = "Status";
            // 
            // progressBar
            // 
            this.progressBar.Location = new System.Drawing.Point(26, 70);
            this.progressBar.Name = "progressBar";
            this.progressBar.Size = new System.Drawing.Size(641, 32);
            this.progressBar.TabIndex = 2;
            // 
            // FormProgress
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(694, 134);
            this.Controls.Add(this.progressBar);
            this.Controls.Add(this.labelStatus);
            this.Name = "FormProgress";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "FormProgress";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        private System.Windows.Forms.Label labelStatus;
        private System.Windows.Forms.ProgressBar progressBar;
    }
}
