namespace SLXEditor
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
            this.XLVLevelPieces = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.INILinkedPieceID = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.picPiecePreview = new System.Windows.Forms.PictureBox();
            this.btnBrowseForPieceLink = new System.Windows.Forms.Button();
            this.btnAddLinkedPieceID = new System.Windows.Forms.Button();
            this.lblOr = new System.Windows.Forms.Label();
            this.lblUnlinkedPieces = new System.Windows.Forms.Label();
            this.lblTransparencyOffsetHint = new System.Windows.Forms.Label();
            this.numLinkedPieceID = new SLXEditor.NumUpDownOverwrite();
            this.btnOpenBatchExporter = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.picPiecePreview)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numLinkedPieceID)).BeginInit();
            this.SuspendLayout();
            // 
            // btnExport
            // 
            this.btnExport.Location = new System.Drawing.Point(250, 655);
            this.btnExport.Name = "btnExport";
            this.btnExport.Size = new System.Drawing.Size(446, 54);
            this.btnExport.TabIndex = 0;
            this.btnExport.Text = "Export This Level To INI";
            this.btnExport.UseVisualStyleBackColor = true;
            this.btnExport.Click += new System.EventHandler(this.btnExport_Click);
            // 
            // comboStyles
            // 
            this.comboStyles.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboStyles.FormattingEnabled = true;
            this.comboStyles.Location = new System.Drawing.Point(319, 22);
            this.comboStyles.Name = "comboStyles";
            this.comboStyles.Size = new System.Drawing.Size(360, 28);
            this.comboStyles.TabIndex = 1;
            this.comboStyles.SelectedIndexChanged += new System.EventHandler(this.comboStyles_SelectedIndexChanged);
            // 
            // btnAddStyle
            // 
            this.btnAddStyle.Location = new System.Drawing.Point(714, 15);
            this.btnAddStyle.Name = "btnAddStyle";
            this.btnAddStyle.Size = new System.Drawing.Size(206, 40);
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
            this.XLVLevelPieces,
            this.INILinkedPieceID});
            this.listViewPieceLinks.HideSelection = false;
            this.listViewPieceLinks.Location = new System.Drawing.Point(28, 117);
            this.listViewPieceLinks.Name = "listViewPieceLinks";
            this.listViewPieceLinks.Size = new System.Drawing.Size(498, 457);
            this.listViewPieceLinks.TabIndex = 6;
            this.listViewPieceLinks.UseCompatibleStateImageBehavior = false;
            this.listViewPieceLinks.View = System.Windows.Forms.View.Details;
            this.listViewPieceLinks.SelectedIndexChanged += new System.EventHandler(this.listViewPieceLinks_SelectedIndexChanged);
            // 
            // XLVLevelPieces
            // 
            this.XLVLevelPieces.Text = "Level Pieces";
            this.XLVLevelPieces.Width = 200;
            // 
            // INILinkedPieceID
            // 
            this.INILinkedPieceID.Text = "INI Linked Piece ID";
            this.INILinkedPieceID.Width = 120;
            // 
            // picPiecePreview
            // 
            this.picPiecePreview.Location = new System.Drawing.Point(547, 171);
            this.picPiecePreview.Name = "picPiecePreview";
            this.picPiecePreview.Size = new System.Drawing.Size(373, 353);
            this.picPiecePreview.TabIndex = 7;
            this.picPiecePreview.TabStop = false;
            // 
            // btnBrowseForPieceLink
            // 
            this.btnBrowseForPieceLink.Location = new System.Drawing.Point(547, 530);
            this.btnBrowseForPieceLink.Name = "btnBrowseForPieceLink";
            this.btnBrowseForPieceLink.Size = new System.Drawing.Size(373, 44);
            this.btnBrowseForPieceLink.TabIndex = 8;
            this.btnBrowseForPieceLink.Text = "Browse For Linked Piece";
            this.btnBrowseForPieceLink.UseVisualStyleBackColor = true;
            this.btnBrowseForPieceLink.Click += new System.EventHandler(this.btnBrowseForPieceLink_Click);
            // 
            // btnAddLinkedPieceID
            // 
            this.btnAddLinkedPieceID.Location = new System.Drawing.Point(547, 117);
            this.btnAddLinkedPieceID.Name = "btnAddLinkedPieceID";
            this.btnAddLinkedPieceID.Size = new System.Drawing.Size(264, 44);
            this.btnAddLinkedPieceID.TabIndex = 9;
            this.btnAddLinkedPieceID.Text = "Add Linked Piece ID:";
            this.btnAddLinkedPieceID.UseVisualStyleBackColor = true;
            this.btnAddLinkedPieceID.Click += new System.EventHandler(this.btnAddLinkedPieceID_Click);
            // 
            // lblOr
            // 
            this.lblOr.AutoSize = true;
            this.lblOr.Location = new System.Drawing.Point(685, 25);
            this.lblOr.Name = "lblOr";
            this.lblOr.Size = new System.Drawing.Size(23, 20);
            this.lblOr.TabIndex = 11;
            this.lblOr.Text = "or";
            // 
            // lblUnlinkedPieces
            // 
            this.lblUnlinkedPieces.AutoSize = true;
            this.lblUnlinkedPieces.ForeColor = System.Drawing.Color.DarkRed;
            this.lblUnlinkedPieces.Location = new System.Drawing.Point(24, 592);
            this.lblUnlinkedPieces.Name = "lblUnlinkedPieces";
            this.lblUnlinkedPieces.Size = new System.Drawing.Size(579, 20);
            this.lblUnlinkedPieces.TabIndex = 12;
            this.lblUnlinkedPieces.Text = "Some pieces are unlinked. For best results, please link all pieces before exporti" +
    "ng";
            this.lblUnlinkedPieces.Visible = false;
            // 
            // lblTransparencyOffsetHint
            // 
            this.lblTransparencyOffsetHint.AutoSize = true;
            this.lblTransparencyOffsetHint.ForeColor = System.Drawing.SystemColors.ControlDarkDark;
            this.lblTransparencyOffsetHint.Location = new System.Drawing.Point(24, 618);
            this.lblTransparencyOffsetHint.Name = "lblTransparencyOffsetHint";
            this.lblTransparencyOffsetHint.Size = new System.Drawing.Size(805, 20);
            this.lblTransparencyOffsetHint.TabIndex = 13;
            this.lblTransparencyOffsetHint.Text = "HINT: Press F1 if you need to update transparent edge offsets (recommended if sty" +
    "le has been recently updated)";
            this.lblTransparencyOffsetHint.Visible = false;
            // 
            // numLinkedPieceID
            // 
            this.numLinkedPieceID.Location = new System.Drawing.Point(817, 127);
            this.numLinkedPieceID.Name = "numLinkedPieceID";
            this.numLinkedPieceID.Size = new System.Drawing.Size(103, 26);
            this.numLinkedPieceID.TabIndex = 10;
            // 
            // btnOpenBatchExporter
            // 
            this.btnOpenBatchExporter.Location = new System.Drawing.Point(28, 655);
            this.btnOpenBatchExporter.Name = "btnOpenBatchExporter";
            this.btnOpenBatchExporter.Size = new System.Drawing.Size(204, 54);
            this.btnOpenBatchExporter.TabIndex = 14;
            this.btnOpenBatchExporter.Text = "Open Batch Exporter";
            this.btnOpenBatchExporter.UseVisualStyleBackColor = true;
            this.btnOpenBatchExporter.Click += new System.EventHandler(this.btnOpenBatchExporter_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(716, 655);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(204, 54);
            this.btnCancel.TabIndex = 15;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // FormINIExporter
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(944, 732);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOpenBatchExporter);
            this.Controls.Add(this.lblTransparencyOffsetHint);
            this.Controls.Add(this.lblUnlinkedPieces);
            this.Controls.Add(this.lblOr);
            this.Controls.Add(this.numLinkedPieceID);
            this.Controls.Add(this.btnAddLinkedPieceID);
            this.Controls.Add(this.btnBrowseForPieceLink);
            this.Controls.Add(this.picPiecePreview);
            this.Controls.Add(this.listViewPieceLinks);
            this.Controls.Add(this.lblChosenOutputStyle);
            this.Controls.Add(this.lblChosenStyle);
            this.Controls.Add(this.lblChooseStyle);
            this.Controls.Add(this.btnAddStyle);
            this.Controls.Add(this.comboStyles);
            this.Controls.Add(this.btnExport);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.KeyPreview = true;
            this.Name = "FormINIExporter";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Export as INI";
            this.Load += new System.EventHandler(this.FormINIExporter_Load);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.FormINIExporter_KeyDown);
            ((System.ComponentModel.ISupportInitialize)(this.picPiecePreview)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numLinkedPieceID)).EndInit();
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
        private System.Windows.Forms.ColumnHeader XLVLevelPieces;
        private System.Windows.Forms.ColumnHeader INILinkedPieceID;
        private System.Windows.Forms.PictureBox picPiecePreview;
        private System.Windows.Forms.Button btnBrowseForPieceLink;
        private System.Windows.Forms.Button btnAddLinkedPieceID;
        private NumUpDownOverwrite numLinkedPieceID;
        private System.Windows.Forms.Label lblOr;
        private System.Windows.Forms.Label lblUnlinkedPieces;
        private System.Windows.Forms.Label lblTransparencyOffsetHint;
        private System.Windows.Forms.Button btnOpenBatchExporter;
        private System.Windows.Forms.Button btnCancel;
    }
}