namespace SLXEditor
{
    partial class FormPieceBrowser
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormPieceBrowser));
            this.SuspendLayout();
            // 
            // FormPieceBrowser
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1121, 154);
            this.Cursor = System.Windows.Forms.Cursors.Default;
            this.DoubleBuffered = true;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.KeyPreview = true;
            this.Name = "FormPieceBrowser";
            this.Text = "SuperLemmix Editor - Piece Browser";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FormPieceBrowser_FormClosing);
            this.Load += new System.EventHandler(this.FormPieceBrowser_Load);
            this.Shown += new System.EventHandler(this.FormPieceBrowser_Shown);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.FormPieceBrowser_KeyDown);
            this.KeyUp += new System.Windows.Forms.KeyEventHandler(this.FormPieceBrowser_KeyUp);
            this.Resize += new System.EventHandler(this.FormPieceBrowser_Resize);
            this.ResumeLayout(false);

        }

        #endregion
    }
}