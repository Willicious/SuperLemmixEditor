namespace NLEditor
{
    partial class FormLevelArranger
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormLevelArranger));
            this.SuspendLayout();
            // 
            // FormLevelArranger
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.ClientSize = new System.Drawing.Size(578, 244);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "FormLevelArranger";
            this.Text = "SuperLemmix Editor - Level Arranger";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FormLevelArranger_FormClosing);
            this.Load += new System.EventHandler(this.FormLevelArranger_Load);
            this.Shown += new System.EventHandler(this.FormLevelArranger_Shown);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.FormLevelArranger_KeyDown);
            this.KeyUp += new System.Windows.Forms.KeyEventHandler(this.FormLevelArranger_KeyUp);
            this.Resize += new System.EventHandler(this.FormLevelArranger_Resize);
            this.ResumeLayout(false);

        }

        #endregion
    }
}