namespace SLXEditor
{
    partial class FormPrePostText
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormPrePostText));
            this.txtPrePostText = new System.Windows.Forms.TextBox();
            this.butPTextOK = new System.Windows.Forms.Button();
            this.butPTextCancel = new System.Windows.Forms.Button();
            this.lblPTextTitle = new System.Windows.Forms.Label();
            this.butPPreview = new System.Windows.Forms.Button();
            this.butPClearText = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // txtPrePostText
            // 
            this.txtPrePostText.Font = new System.Drawing.Font("Courier New", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtPrePostText.Location = new System.Drawing.Point(18, 51);
            this.txtPrePostText.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.txtPrePostText.MaxLength = 1134;
            this.txtPrePostText.Multiline = true;
            this.txtPrePostText.Name = "txtPrePostText";
            this.txtPrePostText.Size = new System.Drawing.Size(580, 460);
            this.txtPrePostText.TabIndex = 0;
            this.txtPrePostText.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.txtPrePostText.TextChanged += new System.EventHandler(this.txtPrePostText_TextChanged);
            // 
            // butPTextOK
            // 
            this.butPTextOK.Location = new System.Drawing.Point(326, 526);
            this.butPTextOK.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.butPTextOK.Name = "butPTextOK";
            this.butPTextOK.Size = new System.Drawing.Size(146, 46);
            this.butPTextOK.TabIndex = 1;
            this.butPTextOK.Text = "Save && Close";
            this.butPTextOK.UseVisualStyleBackColor = true;
            this.butPTextOK.Click += new System.EventHandler(this.butPTextOK_Click);
            // 
            // butPTextCancel
            // 
            this.butPTextCancel.Location = new System.Drawing.Point(478, 526);
            this.butPTextCancel.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.butPTextCancel.Name = "butPTextCancel";
            this.butPTextCancel.Size = new System.Drawing.Size(120, 46);
            this.butPTextCancel.TabIndex = 2;
            this.butPTextCancel.Text = "Cancel";
            this.butPTextCancel.UseVisualStyleBackColor = true;
            this.butPTextCancel.Click += new System.EventHandler(this.butPTextCancel_Click);
            // 
            // lblPTextTitle
            // 
            this.lblPTextTitle.Location = new System.Drawing.Point(14, 14);
            this.lblPTextTitle.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblPTextTitle.Name = "lblPTextTitle";
            this.lblPTextTitle.Size = new System.Drawing.Size(584, 32);
            this.lblPTextTitle.TabIndex = 3;
            this.lblPTextTitle.Text = "Edit";
            this.lblPTextTitle.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // butPPreview
            // 
            this.butPPreview.Location = new System.Drawing.Point(144, 526);
            this.butPPreview.Name = "butPPreview";
            this.butPPreview.Size = new System.Drawing.Size(146, 46);
            this.butPPreview.TabIndex = 4;
            this.butPPreview.Text = "Preview";
            this.butPPreview.UseVisualStyleBackColor = true;
            this.butPPreview.Click += new System.EventHandler(this.butPPreview_Click);
            // 
            // butPClearText
            // 
            this.butPClearText.Location = new System.Drawing.Point(18, 526);
            this.butPClearText.Name = "butPClearText";
            this.butPClearText.Size = new System.Drawing.Size(120, 46);
            this.butPClearText.TabIndex = 5;
            this.butPClearText.Text = "Clear Text";
            this.butPClearText.UseVisualStyleBackColor = true;
            this.butPClearText.Click += new System.EventHandler(this.butPClearText_Click);
            // 
            // FormPrePostText
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.ClientSize = new System.Drawing.Size(615, 586);
            this.Controls.Add(this.butPClearText);
            this.Controls.Add(this.butPPreview);
            this.Controls.Add(this.lblPTextTitle);
            this.Controls.Add(this.butPTextCancel);
            this.Controls.Add(this.butPTextOK);
            this.Controls.Add(this.txtPrePostText);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FormPrePostText";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "FormPrePostText";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FormPrePostTest_FormClosing);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.FormPrePostText_KeyDown);
            this.Leave += new System.EventHandler(this.FormPrePostTest_Leave);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox txtPrePostText;
        private System.Windows.Forms.Button butPTextOK;
        private System.Windows.Forms.Button butPTextCancel;
        private System.Windows.Forms.Label lblPTextTitle;
        private System.Windows.Forms.Button butPPreview;
        private System.Windows.Forms.Button butPClearText;
    }
}