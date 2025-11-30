namespace NLEditor
{
    partial class FormINIExporter
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormINIExporter));
            this.btnExport = new System.Windows.Forms.Button();
            this.comboStyles = new System.Windows.Forms.ComboBox();
            this.btnAddStyle = new System.Windows.Forms.Button();
            this.lblChooseStyle = new System.Windows.Forms.Label();
            this.lblChosenStyle = new System.Windows.Forms.Label();
            this.lblChosenOutputStyle = new System.Windows.Forms.Label();
            this.listViewPieceLinks = new System.Windows.Forms.ListView();
            this.NXLVLevelPieces = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.INILinkedPieceID = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.picPiecePreview = new System.Windows.Forms.PictureBox();
            this.btnAddPieceLink = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.picPiecePreview)).BeginInit();
            this.SuspendLayout();
            // 
            // btnExport
            // 
            this.btnExport.Location = new System.Drawing.Point(28, 568);
            this.btnExport.Name = "btnExport";
            this.btnExport.Size = new System.Drawing.Size(735, 54);
            this.btnExport.TabIndex = 0;
            this.btnExport.Text = "Export";
            this.btnExport.UseVisualStyleBackColor = true;
            this.btnExport.Click += new System.EventHandler(this.btnExport_Click);
            // 
            // comboStyles
            // 
            this.comboStyles.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboStyles.FormattingEnabled = true;
            this.comboStyles.Location = new System.Drawing.Point(319, 22);
            this.comboStyles.Name = "comboStyles";
            this.comboStyles.Size = new System.Drawing.Size(291, 28);
            this.comboStyles.TabIndex = 1;
            this.comboStyles.SelectedIndexChanged += new System.EventHandler(this.comboStyles_SelectedIndexChanged);
            // 
            // btnAddStyle
            // 
            this.btnAddStyle.Location = new System.Drawing.Point(633, 17);
            this.btnAddStyle.Name = "btnAddStyle";
            this.btnAddStyle.Size = new System.Drawing.Size(130, 36);
            this.btnAddStyle.TabIndex = 2;
            this.btnAddStyle.Text = "Add Style";
            this.btnAddStyle.UseVisualStyleBackColor = true;
            this.btnAddStyle.Click += new System.EventHandler(this.btnAddStyle_Click);
            // 
            // lblChooseStyle
            // 
            this.lblChooseStyle.AutoSize = true;
            this.lblChooseStyle.Location = new System.Drawing.Point(24, 25);
            this.lblChooseStyle.Name = "lblChooseStyle";
            this.lblChooseStyle.Size = new System.Drawing.Size(289, 20);
            this.lblChooseStyle.TabIndex = 3;
            this.lblChooseStyle.Text = "To begin, please choose an output style";
            // 
            // lblChosenStyle
            // 
            this.lblChosenStyle.AutoSize = true;
            this.lblChosenStyle.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblChosenStyle.Location = new System.Drawing.Point(24, 73);
            this.lblChosenStyle.Name = "lblChosenStyle";
            this.lblChosenStyle.Size = new System.Drawing.Size(120, 20);
            this.lblChosenStyle.TabIndex = 4;
            this.lblChosenStyle.Text = "Chosen Style:";
            // 
            // lblChosenOutputStyle
            // 
            this.lblChosenOutputStyle.AutoSize = true;
            this.lblChosenOutputStyle.ForeColor = System.Drawing.Color.DarkRed;
            this.lblChosenOutputStyle.Location = new System.Drawing.Point(150, 73);
            this.lblChosenOutputStyle.Name = "lblChosenOutputStyle";
            this.lblChosenOutputStyle.Size = new System.Drawing.Size(307, 20);
            this.lblChosenOutputStyle.TabIndex = 5;
            this.lblChosenOutputStyle.Text = "Please choose a Lemmini-compatible style";
            // 
            // listViewPieceLinks
            // 
            this.listViewPieceLinks.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.NXLVLevelPieces,
            this.INILinkedPieceID});
            this.listViewPieceLinks.HideSelection = false;
            this.listViewPieceLinks.Location = new System.Drawing.Point(28, 117);
            this.listViewPieceLinks.Name = "listViewPieceLinks";
            this.listViewPieceLinks.Size = new System.Drawing.Size(498, 426);
            this.listViewPieceLinks.TabIndex = 6;
            this.listViewPieceLinks.UseCompatibleStateImageBehavior = false;
            this.listViewPieceLinks.View = System.Windows.Forms.View.Details;
            // 
            // NXLVLevelPieces
            // 
            this.NXLVLevelPieces.Text = "NXLV Level Pieces";
            this.NXLVLevelPieces.Width = 200;
            // 
            // INILinkedPieceID
            // 
            this.INILinkedPieceID.Text = "INI Linked Piece ID";
            this.INILinkedPieceID.Width = 120;
            // 
            // picPiecePreview
            // 
            this.picPiecePreview.Location = new System.Drawing.Point(555, 187);
            this.picPiecePreview.Name = "picPiecePreview";
            this.picPiecePreview.Size = new System.Drawing.Size(208, 202);
            this.picPiecePreview.TabIndex = 7;
            this.picPiecePreview.TabStop = false;
            // 
            // btnAddPieceLink
            // 
            this.btnAddPieceLink.Location = new System.Drawing.Point(555, 117);
            this.btnAddPieceLink.Name = "btnAddPieceLink";
            this.btnAddPieceLink.Size = new System.Drawing.Size(208, 53);
            this.btnAddPieceLink.TabIndex = 8;
            this.btnAddPieceLink.Text = "Add Piece Link";
            this.btnAddPieceLink.UseVisualStyleBackColor = true;
            this.btnAddPieceLink.Click += new System.EventHandler(this.btnAddPieceLink_Click);
            // 
            // FormINIExporter
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(788, 634);
            this.Controls.Add(this.btnAddPieceLink);
            this.Controls.Add(this.picPiecePreview);
            this.Controls.Add(this.listViewPieceLinks);
            this.Controls.Add(this.lblChosenOutputStyle);
            this.Controls.Add(this.lblChosenStyle);
            this.Controls.Add(this.lblChooseStyle);
            this.Controls.Add(this.btnAddStyle);
            this.Controls.Add(this.comboStyles);
            this.Controls.Add(this.btnExport);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "FormINIExporter";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "FormINIConverter";
            this.Load += new System.EventHandler(this.FormINIExporter_Load);
            ((System.ComponentModel.ISupportInitialize)(this.picPiecePreview)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnExport;
        private System.Windows.Forms.ComboBox comboStyles;
        private System.Windows.Forms.Button btnAddStyle;
        private System.Windows.Forms.Label lblChooseStyle;
        private System.Windows.Forms.Label lblChosenStyle;
        private System.Windows.Forms.Label lblChosenOutputStyle;
        private System.Windows.Forms.ListView listViewPieceLinks;
        private System.Windows.Forms.ColumnHeader NXLVLevelPieces;
        private System.Windows.Forms.ColumnHeader INILinkedPieceID;
        private System.Windows.Forms.PictureBox picPiecePreview;
        private System.Windows.Forms.Button btnAddPieceLink;
    }
}