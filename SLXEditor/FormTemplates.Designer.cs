namespace SLXEditor
{
    partial class FormTemplates
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormTemplates));
            this.panelTemplate = new System.Windows.Forms.Panel();
            this.rtSkillSetData = new System.Windows.Forms.RichTextBox();
            this.rtLevelData = new System.Windows.Forms.RichTextBox();
            this.btnDelete = new System.Windows.Forms.Button();
            this.btnLoadTemplate = new System.Windows.Forms.Button();
            this.labelTitle = new System.Windows.Forms.Label();
            this.picPreview = new System.Windows.Forms.PictureBox();
            this.listTemplates = new System.Windows.Forms.ListBox();
            this.btnSetAsDefault = new System.Windows.Forms.Button();
            this.panelTemplate.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picPreview)).BeginInit();
            this.SuspendLayout();
            // 
            // panelTemplate
            // 
            this.panelTemplate.Controls.Add(this.btnSetAsDefault);
            this.panelTemplate.Controls.Add(this.rtSkillSetData);
            this.panelTemplate.Controls.Add(this.rtLevelData);
            this.panelTemplate.Controls.Add(this.btnDelete);
            this.panelTemplate.Controls.Add(this.btnLoadTemplate);
            this.panelTemplate.Controls.Add(this.labelTitle);
            this.panelTemplate.Controls.Add(this.picPreview);
            this.panelTemplate.Location = new System.Drawing.Point(364, 26);
            this.panelTemplate.Name = "panelTemplate";
            this.panelTemplate.Size = new System.Drawing.Size(939, 381);
            this.panelTemplate.TabIndex = 0;
            // 
            // rtSkillSetData
            // 
            this.rtSkillSetData.BackColor = System.Drawing.SystemColors.Control;
            this.rtSkillSetData.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.rtSkillSetData.Location = new System.Drawing.Point(739, 34);
            this.rtSkillSetData.Name = "rtSkillSetData";
            this.rtSkillSetData.ReadOnly = true;
            this.rtSkillSetData.Size = new System.Drawing.Size(200, 327);
            this.rtSkillSetData.TabIndex = 6;
            this.rtSkillSetData.TabStop = false;
            this.rtSkillSetData.Text = "Skillset data";
            // 
            // rtLevelData
            // 
            this.rtLevelData.BackColor = System.Drawing.SystemColors.Control;
            this.rtLevelData.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.rtLevelData.Location = new System.Drawing.Point(514, 34);
            this.rtLevelData.Name = "rtLevelData";
            this.rtLevelData.ReadOnly = true;
            this.rtLevelData.Size = new System.Drawing.Size(219, 327);
            this.rtLevelData.TabIndex = 5;
            this.rtLevelData.TabStop = false;
            this.rtLevelData.Text = "Level data";
            // 
            // btnDelete
            // 
            this.btnDelete.Location = new System.Drawing.Point(396, 320);
            this.btnDelete.Name = "btnDelete";
            this.btnDelete.Size = new System.Drawing.Size(100, 44);
            this.btnDelete.TabIndex = 4;
            this.btnDelete.Text = "Delete";
            this.btnDelete.UseVisualStyleBackColor = true;
            this.btnDelete.Click += new System.EventHandler(this.btnDelete_Click);
            // 
            // btnLoadTemplate
            // 
            this.btnLoadTemplate.Location = new System.Drawing.Point(0, 320);
            this.btnLoadTemplate.Name = "btnLoadTemplate";
            this.btnLoadTemplate.Size = new System.Drawing.Size(240, 44);
            this.btnLoadTemplate.TabIndex = 2;
            this.btnLoadTemplate.Text = "Load Template";
            this.btnLoadTemplate.UseVisualStyleBackColor = true;
            this.btnLoadTemplate.Click += new System.EventHandler(this.btnLoadTemplate_Click);
            // 
            // labelTitle
            // 
            this.labelTitle.AutoSize = true;
            this.labelTitle.Font = new System.Drawing.Font("Arial Rounded MT Bold", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelTitle.Location = new System.Drawing.Point(3, 3);
            this.labelTitle.Name = "labelTitle";
            this.labelTitle.Size = new System.Drawing.Size(63, 28);
            this.labelTitle.TabIndex = 1;
            this.labelTitle.Text = "Title";
            this.labelTitle.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // picPreview
            // 
            this.picPreview.Location = new System.Drawing.Point(0, 34);
            this.picPreview.Name = "picPreview";
            this.picPreview.Size = new System.Drawing.Size(496, 277);
            this.picPreview.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.picPreview.TabIndex = 0;
            this.picPreview.TabStop = false;
            // 
            // listTemplates
            // 
            this.listTemplates.FormattingEnabled = true;
            this.listTemplates.ItemHeight = 20;
            this.listTemplates.Location = new System.Drawing.Point(36, 26);
            this.listTemplates.Name = "listTemplates";
            this.listTemplates.Size = new System.Drawing.Size(311, 364);
            this.listTemplates.TabIndex = 1;
            this.listTemplates.SelectedIndexChanged += new System.EventHandler(this.listTemplates_SelectedIndexChanged);
            // 
            // btnSetAsDefault
            // 
            this.btnSetAsDefault.Location = new System.Drawing.Point(247, 320);
            this.btnSetAsDefault.Name = "btnSetAsDefault";
            this.btnSetAsDefault.Size = new System.Drawing.Size(144, 44);
            this.btnSetAsDefault.TabIndex = 7;
            this.btnSetAsDefault.Text = "Set As Default";
            this.btnSetAsDefault.UseVisualStyleBackColor = true;
            this.btnSetAsDefault.Click += new System.EventHandler(this.btnSetAsDefault_Click);
            // 
            // FormTemplates
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1315, 406);
            this.Controls.Add(this.listTemplates);
            this.Controls.Add(this.panelTemplate);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "FormTemplates";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Templates";
            this.Load += new System.EventHandler(this.FormTemplates_Load);
            this.panelTemplate.ResumeLayout(false);
            this.panelTemplate.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picPreview)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panelTemplate;
        private System.Windows.Forms.PictureBox picPreview;
        private System.Windows.Forms.Button btnDelete;
        private System.Windows.Forms.Button btnLoadTemplate;
        private System.Windows.Forms.Label labelTitle;
        private System.Windows.Forms.RichTextBox rtLevelData;
        private System.Windows.Forms.RichTextBox rtSkillSetData;
        private System.Windows.Forms.ListBox listTemplates;
        private System.Windows.Forms.Button btnSetAsDefault;
    }
}