namespace NLEditor
{
    partial class FormPrePostTest
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormPrePostTest));
            this.txtPrePostText = new System.Windows.Forms.TextBox();
            this.butPTextOK = new System.Windows.Forms.Button();
            this.butPTextCancel = new System.Windows.Forms.Button();
            this.lblPTextTitle = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // txtPrePostText
            // 
            this.txtPrePostText.Font = new System.Drawing.Font("Courier New", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtPrePostText.Location = new System.Drawing.Point(6, 33);
            this.txtPrePostText.Multiline = true;
            this.txtPrePostText.Name = "txtPrePostText";
            this.txtPrePostText.Size = new System.Drawing.Size(289, 144);
            this.txtPrePostText.TabIndex = 0;
            // 
            // butPTextOK
            // 
            this.butPTextOK.Location = new System.Drawing.Point(6, 192);
            this.butPTextOK.Name = "butPTextOK";
            this.butPTextOK.Size = new System.Drawing.Size(139, 30);
            this.butPTextOK.TabIndex = 1;
            this.butPTextOK.Text = "OK";
            this.butPTextOK.UseVisualStyleBackColor = true;
            this.butPTextOK.Click += new System.EventHandler(this.butPTextOK_Click);
            // 
            // butPTextCancel
            // 
            this.butPTextCancel.Location = new System.Drawing.Point(157, 192);
            this.butPTextCancel.Name = "butPTextCancel";
            this.butPTextCancel.Size = new System.Drawing.Size(138, 30);
            this.butPTextCancel.TabIndex = 2;
            this.butPTextCancel.Text = "Cancel";
            this.butPTextCancel.UseVisualStyleBackColor = true;
            this.butPTextCancel.Click += new System.EventHandler(this.butPTextCancel_Click);
            // 
            // lblPTextTitle
            // 
            this.lblPTextTitle.Location = new System.Drawing.Point(3, 9);
            this.lblPTextTitle.Name = "lblPTextTitle";
            this.lblPTextTitle.Size = new System.Drawing.Size(292, 15);
            this.lblPTextTitle.TabIndex = 3;
            this.lblPTextTitle.Text = "Edit";
            this.lblPTextTitle.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // FormPrePostTest
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(299, 234);
            this.Controls.Add(this.lblPTextTitle);
            this.Controls.Add(this.butPTextCancel);
            this.Controls.Add(this.butPTextOK);
            this.Controls.Add(this.txtPrePostText);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FormPrePostTest";
            this.ShowInTaskbar = false;
            this.Text = "FormPrePostTest";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FormPrePostTest_FormClosing);
            this.Leave += new System.EventHandler(this.FormPrePostTest_Leave);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox txtPrePostText;
        private System.Windows.Forms.Button butPTextOK;
        private System.Windows.Forms.Button butPTextCancel;
        private System.Windows.Forms.Label lblPTextTitle;
    }
}