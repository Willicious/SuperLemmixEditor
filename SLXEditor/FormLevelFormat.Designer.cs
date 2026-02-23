namespace SLXEditor
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
            this.labelTargetFolder = new System.Windows.Forms.Label();
            this.labelCleansingLevels = new System.Windows.Forms.Label();
            this.checkApplyFormatToLevelsNXMI = new System.Windows.Forms.CheckBox();
            this.labelNXLVWarning = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // lblChooseOutputFormat
            // 
            this.lblChooseOutputFormat.AutoSize = true;
            this.lblChooseOutputFormat.Location = new System.Drawing.Point(27, 81);
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
            this.comboBoxFormats.Location = new System.Drawing.Point(31, 117);
            this.comboBoxFormats.Name = "comboBoxFormats";
            this.comboBoxFormats.Size = new System.Drawing.Size(523, 28);
            this.comboBoxFormats.TabIndex = 1;
            this.comboBoxFormats.SelectedIndexChanged += new System.EventHandler(this.comboBoxFormats_SelectedIndexChanged);
            // 
            // btnCleanseLevels
            // 
            this.btnCleanseLevels.Location = new System.Drawing.Point(31, 248);
            this.btnCleanseLevels.Name = "btnCleanseLevels";
            this.btnCleanseLevels.Size = new System.Drawing.Size(272, 50);
            this.btnCleanseLevels.TabIndex = 2;
            this.btnCleanseLevels.Text = "Cleanse Levels";
            this.btnCleanseLevels.UseVisualStyleBackColor = true;
            this.btnCleanseLevels.Click += new System.EventHandler(this.btnCleanseLevels_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(309, 248);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(245, 50);
            this.btnCancel.TabIndex = 3;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // labelTargetFolder
            // 
            this.labelTargetFolder.AutoSize = true;
            this.labelTargetFolder.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelTargetFolder.Location = new System.Drawing.Point(159, 30);
            this.labelTargetFolder.Name = "labelTargetFolder";
            this.labelTargetFolder.Size = new System.Drawing.Size(120, 20);
            this.labelTargetFolder.TabIndex = 7;
            this.labelTargetFolder.Text = "(target folder)";
            // 
            // labelCleansingLevels
            // 
            this.labelCleansingLevels.AutoSize = true;
            this.labelCleansingLevels.Location = new System.Drawing.Point(27, 30);
            this.labelCleansingLevels.Name = "labelCleansingLevels";
            this.labelCleansingLevels.Size = new System.Drawing.Size(126, 20);
            this.labelCleansingLevels.TabIndex = 6;
            this.labelCleansingLevels.Text = "Cleansing levels:";
            // 
            // checkApplyFormatToLevelsNXMI
            // 
            this.checkApplyFormatToLevelsNXMI.AutoSize = true;
            this.checkApplyFormatToLevelsNXMI.Location = new System.Drawing.Point(31, 160);
            this.checkApplyFormatToLevelsNXMI.Name = "checkApplyFormatToLevelsNXMI";
            this.checkApplyFormatToLevelsNXMI.Size = new System.Drawing.Size(277, 24);
            this.checkApplyFormatToLevelsNXMI.TabIndex = 8;
            this.checkApplyFormatToLevelsNXMI.Text = "Apply chosen format to levels.nxmi";
            this.checkApplyFormatToLevelsNXMI.UseVisualStyleBackColor = true;
            // 
            // labelNXLVWarning
            // 
            this.labelNXLVWarning.AutoSize = true;
            this.labelNXLVWarning.ForeColor = System.Drawing.Color.SeaGreen;
            this.labelNXLVWarning.Location = new System.Drawing.Point(27, 206);
            this.labelNXLVWarning.Name = "labelNXLVWarning";
            this.labelNXLVWarning.Size = new System.Drawing.Size(502, 20);
            this.labelNXLVWarning.TabIndex = 9;
            this.labelNXLVWarning.Text = "N.B. Levels that are incompatible with NeoLemmix will be saved to .sxlv";
            this.labelNXLVWarning.Visible = false;
            // 
            // FormLevelFormat
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(584, 324);
            this.Controls.Add(this.labelNXLVWarning);
            this.Controls.Add(this.checkApplyFormatToLevelsNXMI);
            this.Controls.Add(this.labelTargetFolder);
            this.Controls.Add(this.labelCleansingLevels);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnCleanseLevels);
            this.Controls.Add(this.comboBoxFormats);
            this.Controls.Add(this.lblChooseOutputFormat);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "FormLevelFormat";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Cleanse Levels";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblChooseOutputFormat;
        private System.Windows.Forms.ComboBox comboBoxFormats;
        private System.Windows.Forms.Button btnCleanseLevels;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Label labelTargetFolder;
        private System.Windows.Forms.Label labelCleansingLevels;
        private System.Windows.Forms.CheckBox checkApplyFormatToLevelsNXMI;
        private System.Windows.Forms.Label labelNXLVWarning;
    }
}