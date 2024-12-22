namespace NLEditor
{
    partial class FormPieceSearch
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormPieceSearch));
            this.listBoxSearchResults = new System.Windows.Forms.ListBox();
            this.textBoxSearch = new System.Windows.Forms.TextBox();
            this.lblSearchFor = new System.Windows.Forms.Label();
            this.pictureBoxPreview = new System.Windows.Forms.PictureBox();
            this.lblMetaData = new System.Windows.Forms.Label();
            this.check_CurrentStyleOnly = new System.Windows.Forms.CheckBox();
            this.lblCurrentStyle = new System.Windows.Forms.Label();
            this.btnLoadStyle = new System.Windows.Forms.Button();
            this.btnAddPieceAndLoadStyle = new System.Windows.Forms.Button();
            this.btnClose = new System.Windows.Forms.Button();
            this.btnAddPiece = new System.Windows.Forms.Button();
            this.lblFilterResults = new System.Windows.Forms.Label();
            this.btnSearch = new System.Windows.Forms.Button();
            this.formPadding = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxPreview)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.formPadding)).BeginInit();
            this.SuspendLayout();
            // 
            // listBoxSearchResults
            // 
            this.listBoxSearchResults.FormattingEnabled = true;
            this.listBoxSearchResults.ItemHeight = 20;
            this.listBoxSearchResults.Location = new System.Drawing.Point(359, 24);
            this.listBoxSearchResults.Name = "listBoxSearchResults";
            this.listBoxSearchResults.Size = new System.Drawing.Size(432, 384);
            this.listBoxSearchResults.TabIndex = 0;
            this.listBoxSearchResults.SelectedIndexChanged += new System.EventHandler(this.listBoxSearchResults_SelectedIndexChanged);
            // 
            // textBoxSearch
            // 
            this.textBoxSearch.Location = new System.Drawing.Point(94, 24);
            this.textBoxSearch.Name = "textBoxSearch";
            this.textBoxSearch.Size = new System.Drawing.Size(243, 26);
            this.textBoxSearch.TabIndex = 1;
            this.textBoxSearch.KeyDown += new System.Windows.Forms.KeyEventHandler(this.textBoxSearch_KeyDown);
            // 
            // lblSearchFor
            // 
            this.lblSearchFor.AutoSize = true;
            this.lblSearchFor.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblSearchFor.Location = new System.Drawing.Point(17, 27);
            this.lblSearchFor.Name = "lblSearchFor";
            this.lblSearchFor.Size = new System.Drawing.Size(71, 20);
            this.lblSearchFor.TabIndex = 2;
            this.lblSearchFor.Text = "Search:";
            // 
            // pictureBoxPreview
            // 
            this.pictureBoxPreview.Location = new System.Drawing.Point(815, 24);
            this.pictureBoxPreview.Name = "pictureBoxPreview";
            this.pictureBoxPreview.Size = new System.Drawing.Size(240, 240);
            this.pictureBoxPreview.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBoxPreview.TabIndex = 3;
            this.pictureBoxPreview.TabStop = false;
            // 
            // lblMetaData
            // 
            this.lblMetaData.AutoSize = true;
            this.lblMetaData.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblMetaData.Location = new System.Drawing.Point(811, 270);
            this.lblMetaData.Name = "lblMetaData";
            this.lblMetaData.Size = new System.Drawing.Size(70, 20);
            this.lblMetaData.TabIndex = 4;
            this.lblMetaData.Text = "Preview";
            // 
            // check_CurrentStyleOnly
            // 
            this.check_CurrentStyleOnly.AutoSize = true;
            this.check_CurrentStyleOnly.Location = new System.Drawing.Point(21, 107);
            this.check_CurrentStyleOnly.Name = "check_CurrentStyleOnly";
            this.check_CurrentStyleOnly.Size = new System.Drawing.Size(128, 24);
            this.check_CurrentStyleOnly.TabIndex = 5;
            this.check_CurrentStyleOnly.Text = "Current style:";
            this.check_CurrentStyleOnly.UseVisualStyleBackColor = true;
            this.check_CurrentStyleOnly.CheckedChanged += new System.EventHandler(this.check_CurrentStyleOnly_CheckedChanged);
            // 
            // lblCurrentStyle
            // 
            this.lblCurrentStyle.AutoSize = true;
            this.lblCurrentStyle.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblCurrentStyle.Location = new System.Drawing.Point(144, 108);
            this.lblCurrentStyle.Name = "lblCurrentStyle";
            this.lblCurrentStyle.Size = new System.Drawing.Size(126, 20);
            this.lblCurrentStyle.TabIndex = 6;
            this.lblCurrentStyle.Text = "(Current Style)";
            // 
            // btnLoadStyle
            // 
            this.btnLoadStyle.Location = new System.Drawing.Point(1075, 24);
            this.btnLoadStyle.Name = "btnLoadStyle";
            this.btnLoadStyle.Size = new System.Drawing.Size(230, 40);
            this.btnLoadStyle.TabIndex = 7;
            this.btnLoadStyle.Text = "Load Style";
            this.btnLoadStyle.UseVisualStyleBackColor = true;
            this.btnLoadStyle.Click += new System.EventHandler(this.btnLoadStyle_Click);
            // 
            // btnAddPieceAndLoadStyle
            // 
            this.btnAddPieceAndLoadStyle.Location = new System.Drawing.Point(1075, 71);
            this.btnAddPieceAndLoadStyle.Name = "btnAddPieceAndLoadStyle";
            this.btnAddPieceAndLoadStyle.Size = new System.Drawing.Size(230, 40);
            this.btnAddPieceAndLoadStyle.TabIndex = 8;
            this.btnAddPieceAndLoadStyle.Text = "Add Piece and Load Style";
            this.btnAddPieceAndLoadStyle.UseVisualStyleBackColor = true;
            this.btnAddPieceAndLoadStyle.Click += new System.EventHandler(this.btnAddPieceAndLoadStyle_Click);
            // 
            // btnClose
            // 
            this.btnClose.Location = new System.Drawing.Point(815, 367);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(490, 40);
            this.btnClose.TabIndex = 9;
            this.btnClose.Text = "Close";
            this.btnClose.UseVisualStyleBackColor = true;
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // btnAddPiece
            // 
            this.btnAddPiece.Location = new System.Drawing.Point(1075, 118);
            this.btnAddPiece.Name = "btnAddPiece";
            this.btnAddPiece.Size = new System.Drawing.Size(230, 40);
            this.btnAddPiece.TabIndex = 10;
            this.btnAddPiece.Text = "Add Piece";
            this.btnAddPiece.UseVisualStyleBackColor = true;
            this.btnAddPiece.Click += new System.EventHandler(this.btnAddPiece_Click);
            // 
            // lblFilterResults
            // 
            this.lblFilterResults.AutoSize = true;
            this.lblFilterResults.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblFilterResults.Location = new System.Drawing.Point(17, 71);
            this.lblFilterResults.Name = "lblFilterResults";
            this.lblFilterResults.Size = new System.Drawing.Size(114, 20);
            this.lblFilterResults.TabIndex = 11;
            this.lblFilterResults.Text = "Filter results:";
            // 
            // btnSearch
            // 
            this.btnSearch.Location = new System.Drawing.Point(21, 367);
            this.btnSearch.Name = "btnSearch";
            this.btnSearch.Size = new System.Drawing.Size(316, 40);
            this.btnSearch.TabIndex = 12;
            this.btnSearch.Text = "Search";
            this.btnSearch.UseVisualStyleBackColor = true;
            // 
            // formPadding
            // 
            this.formPadding.Location = new System.Drawing.Point(1061, 1);
            this.formPadding.Name = "formPadding";
            this.formPadding.Size = new System.Drawing.Size(270, 406);
            this.formPadding.TabIndex = 13;
            this.formPadding.TabStop = false;
            // 
            // FormPieceSearch
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.ClientSize = new System.Drawing.Size(1330, 423);
            this.Controls.Add(this.btnSearch);
            this.Controls.Add(this.lblFilterResults);
            this.Controls.Add(this.btnAddPiece);
            this.Controls.Add(this.btnClose);
            this.Controls.Add(this.btnAddPieceAndLoadStyle);
            this.Controls.Add(this.btnLoadStyle);
            this.Controls.Add(this.lblCurrentStyle);
            this.Controls.Add(this.check_CurrentStyleOnly);
            this.Controls.Add(this.lblMetaData);
            this.Controls.Add(this.pictureBoxPreview);
            this.Controls.Add(this.lblSearchFor);
            this.Controls.Add(this.textBoxSearch);
            this.Controls.Add(this.listBoxSearchResults);
            this.Controls.Add(this.formPadding);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FormPieceSearch";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Search Pieces";
            this.Load += new System.EventHandler(this.FormPieceSearch_Load);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxPreview)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.formPadding)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ListBox listBoxSearchResults;
        private System.Windows.Forms.TextBox textBoxSearch;
        private System.Windows.Forms.Label lblSearchFor;
        private System.Windows.Forms.PictureBox pictureBoxPreview;
        private System.Windows.Forms.Label lblMetaData;
        private System.Windows.Forms.CheckBox check_CurrentStyleOnly;
        private System.Windows.Forms.Label lblCurrentStyle;
        private System.Windows.Forms.Button btnLoadStyle;
        private System.Windows.Forms.Button btnAddPieceAndLoadStyle;
        private System.Windows.Forms.Button btnClose;
        private System.Windows.Forms.Button btnAddPiece;
        private System.Windows.Forms.Label lblFilterResults;
        private System.Windows.Forms.Button btnSearch;
        private System.Windows.Forms.PictureBox formPadding;
    }
}