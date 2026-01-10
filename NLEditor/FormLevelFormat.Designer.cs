namespace NLEditor
{
    partial class FormLevelFormat
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormLevelFormat));
            this.lblChooseOutputFormat = new System.Windows.Forms.Label();
            this.comboBoxFormats = new System.Windows.Forms.ComboBox();
            this.btnCleanseLevels = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // lblChooseOutputFormat
            // 
            this.lblChooseOutputFormat.AutoSize = true;
            this.lblChooseOutputFormat.Location = new System.Drawing.Point(28, 34);
            this.lblChooseOutputFormat.Name = "lblChooseOutputFormat";
            this.lblChooseOutputFormat.Size = new System.Drawing.Size(351, 20);
            this.lblChooseOutputFormat.TabIndex = 0;
            this.lblChooseOutputFormat.Text = "Choose an output format for the cleansed levels:";
            // 
            // comboBoxFormats
            // 
            this.comboBoxFormats.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxFormats.FormattingEnabled = true;
            this.comboBoxFormats.Items.AddRange(new object[] {
            "Keep original format",
            "SuperLemmix (.sxlv)",
            "NeoLemmix(.nxlv)"});
            this.comboBoxFormats.Location = new System.Drawing.Point(32, 75);
            this.comboBoxFormats.Name = "comboBoxFormats";
            this.comboBoxFormats.Size = new System.Drawing.Size(347, 28);
            this.comboBoxFormats.TabIndex = 1;
            // 
            // btnCleanseLevels
            // 
            this.btnCleanseLevels.Location = new System.Drawing.Point(32, 122);
            this.btnCleanseLevels.Name = "btnCleanseLevels";
            this.btnCleanseLevels.Size = new System.Drawing.Size(180, 50);
            this.btnCleanseLevels.TabIndex = 2;
            this.btnCleanseLevels.Text = "Cleanse Levels";
            this.btnCleanseLevels.UseVisualStyleBackColor = true;
            this.btnCleanseLevels.Click += new System.EventHandler(this.btnCleanseLevels_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(227, 122);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(150, 50);
            this.btnCancel.TabIndex = 3;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // FormLevelFormat
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(406, 204);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnCleanseLevels);
            this.Controls.Add(this.comboBoxFormats);
            this.Controls.Add(this.lblChooseOutputFormat);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "FormLevelFormat";
            this.Text = "Cleanse Levels";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblChooseOutputFormat;
        private System.Windows.Forms.ComboBox comboBoxFormats;
        private System.Windows.Forms.Button btnCleanseLevels;
        private System.Windows.Forms.Button btnCancel;
    }
}