namespace SLXEditor
{
    partial class FormPiecesList
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormPiecesList));
            this.listViewTerrain = new System.Windows.Forms.ListView();
            this.columnIndex = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnPiece = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnType = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnDrawMode = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.btnClose = new System.Windows.Forms.Button();
            this.btnDrawFirst = new System.Windows.Forms.Button();
            this.btnDrawSooner = new System.Windows.Forms.Button();
            this.btnDrawLater = new System.Windows.Forms.Button();
            this.btnDrawLast = new System.Windows.Forms.Button();
            this.btnDeleteTerr = new System.Windows.Forms.Button();
            this.listViewObjects = new System.Windows.Forms.ListView();
            this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader2 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader3 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader4 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.panelPadding = new System.Windows.Forms.Panel();
            this.picLemming = new System.Windows.Forms.PictureBox();
            this.picPiecePreview = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.picLemming)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.picPiecePreview)).BeginInit();
            this.SuspendLayout();
            // 
            // listViewTerrain
            // 
            this.listViewTerrain.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnIndex,
            this.columnPiece,
            this.columnType,
            this.columnDrawMode});
            this.listViewTerrain.FullRowSelect = true;
            this.listViewTerrain.HideSelection = false;
            this.listViewTerrain.Location = new System.Drawing.Point(12, 12);
            this.listViewTerrain.Name = "listViewTerrain";
            this.listViewTerrain.Size = new System.Drawing.Size(498, 455);
            this.listViewTerrain.TabIndex = 7;
            this.listViewTerrain.UseCompatibleStateImageBehavior = false;
            this.listViewTerrain.View = System.Windows.Forms.View.Details;
            this.listViewTerrain.SelectedIndexChanged += new System.EventHandler(this.listView_SelectedIndexChanged);
            // 
            // columnIndex
            // 
            this.columnIndex.Text = "Terrain";
            this.columnIndex.Width = 45;
            // 
            // columnPiece
            // 
            this.columnPiece.Text = "Piece";
            this.columnPiece.Width = 65;
            // 
            // columnType
            // 
            this.columnType.Text = "Type";
            this.columnType.Width = 65;
            // 
            // columnDrawMode
            // 
            this.columnDrawMode.Text = "Draw Mode";
            this.columnDrawMode.Width = 140;
            // 
            // btnClose
            // 
            this.btnClose.Location = new System.Drawing.Point(529, 768);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(267, 46);
            this.btnClose.TabIndex = 9;
            this.btnClose.Text = "Close";
            this.btnClose.UseVisualStyleBackColor = true;
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // btnDrawFirst
            // 
            this.btnDrawFirst.Location = new System.Drawing.Point(529, 12);
            this.btnDrawFirst.Name = "btnDrawFirst";
            this.btnDrawFirst.Size = new System.Drawing.Size(267, 46);
            this.btnDrawFirst.TabIndex = 10;
            this.btnDrawFirst.Text = "Draw First ↑↑";
            this.btnDrawFirst.UseVisualStyleBackColor = true;
            this.btnDrawFirst.Click += new System.EventHandler(this.btnDrawFirst_Click);
            // 
            // btnDrawSooner
            // 
            this.btnDrawSooner.Location = new System.Drawing.Point(529, 64);
            this.btnDrawSooner.Name = "btnDrawSooner";
            this.btnDrawSooner.Size = new System.Drawing.Size(267, 46);
            this.btnDrawSooner.TabIndex = 11;
            this.btnDrawSooner.Text = "Draw Sooner ↑";
            this.btnDrawSooner.UseVisualStyleBackColor = true;
            this.btnDrawSooner.Click += new System.EventHandler(this.btnDrawSooner_Click);
            // 
            // btnDrawLater
            // 
            this.btnDrawLater.Location = new System.Drawing.Point(529, 116);
            this.btnDrawLater.Name = "btnDrawLater";
            this.btnDrawLater.Size = new System.Drawing.Size(267, 46);
            this.btnDrawLater.TabIndex = 12;
            this.btnDrawLater.Text = "Draw Later ↓";
            this.btnDrawLater.UseVisualStyleBackColor = true;
            this.btnDrawLater.Click += new System.EventHandler(this.btnDrawLater_Click);
            // 
            // btnDrawLast
            // 
            this.btnDrawLast.Location = new System.Drawing.Point(529, 168);
            this.btnDrawLast.Name = "btnDrawLast";
            this.btnDrawLast.Size = new System.Drawing.Size(267, 46);
            this.btnDrawLast.TabIndex = 13;
            this.btnDrawLast.Text = "Draw Last ↓↓";
            this.btnDrawLast.UseVisualStyleBackColor = true;
            this.btnDrawLast.Click += new System.EventHandler(this.btnDrawLast_Click);
            // 
            // btnDeleteTerr
            // 
            this.btnDeleteTerr.Location = new System.Drawing.Point(529, 269);
            this.btnDeleteTerr.Name = "btnDeleteTerr";
            this.btnDeleteTerr.Size = new System.Drawing.Size(267, 46);
            this.btnDeleteTerr.TabIndex = 14;
            this.btnDeleteTerr.Text = "Delete";
            this.btnDeleteTerr.UseVisualStyleBackColor = true;
            this.btnDeleteTerr.Click += new System.EventHandler(this.btnDelete_Click);
            // 
            // listViewObjects
            // 
            this.listViewObjects.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1,
            this.columnHeader2,
            this.columnHeader3,
            this.columnHeader4});
            this.listViewObjects.FullRowSelect = true;
            this.listViewObjects.HideSelection = false;
            this.listViewObjects.Location = new System.Drawing.Point(12, 488);
            this.listViewObjects.Name = "listViewObjects";
            this.listViewObjects.Size = new System.Drawing.Size(498, 326);
            this.listViewObjects.TabIndex = 15;
            this.listViewObjects.UseCompatibleStateImageBehavior = false;
            this.listViewObjects.View = System.Windows.Forms.View.Details;
            this.listViewObjects.SelectedIndexChanged += new System.EventHandler(this.listView_SelectedIndexChanged);
            // 
            // columnHeader1
            // 
            this.columnHeader1.Text = "Object";
            this.columnHeader1.Width = 45;
            // 
            // columnHeader2
            // 
            this.columnHeader2.Text = "Piece";
            this.columnHeader2.Width = 65;
            // 
            // columnHeader3
            // 
            this.columnHeader3.Text = "Type";
            this.columnHeader3.Width = 65;
            // 
            // columnHeader4
            // 
            this.columnHeader4.Text = "Draw Mode";
            this.columnHeader4.Width = 140;
            // 
            // panelPadding
            // 
            this.panelPadding.Location = new System.Drawing.Point(-1, 798);
            this.panelPadding.Name = "panelPadding";
            this.panelPadding.Size = new System.Drawing.Size(809, 27);
            this.panelPadding.TabIndex = 16;
            // 
            // picLemming
            // 
            this.picLemming.Image = global::SLXEditor.Properties.Resources.LemmingPreview;
            this.picLemming.Location = new System.Drawing.Point(529, 377);
            this.picLemming.Name = "picLemming";
            this.picLemming.Size = new System.Drawing.Size(60, 90);
            this.picLemming.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.picLemming.TabIndex = 17;
            this.picLemming.TabStop = false;
            this.picLemming.Visible = false;
            // 
            // picPiecePreview
            // 
            this.picPiecePreview.Location = new System.Drawing.Point(529, 377);
            this.picPiecePreview.Name = "picPiecePreview";
            this.picPiecePreview.Size = new System.Drawing.Size(267, 324);
            this.picPiecePreview.TabIndex = 8;
            this.picPiecePreview.TabStop = false;
            // 
            // FormPiecesList
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.ClientSize = new System.Drawing.Size(808, 826);
            this.Controls.Add(this.picLemming);
            this.Controls.Add(this.listViewObjects);
            this.Controls.Add(this.btnDeleteTerr);
            this.Controls.Add(this.btnDrawLast);
            this.Controls.Add(this.btnDrawLater);
            this.Controls.Add(this.btnDrawSooner);
            this.Controls.Add(this.btnDrawFirst);
            this.Controls.Add(this.btnClose);
            this.Controls.Add(this.picPiecePreview);
            this.Controls.Add(this.listViewTerrain);
            this.Controls.Add(this.panelPadding);
            this.DoubleBuffered = true;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "FormPiecesList";
            this.Text = "Pieces List";
            this.Activated += new System.EventHandler(this.FormPiecesList_Activated);
            this.Deactivate += new System.EventHandler(this.FormPiecesList_Deactivate);
            this.Load += new System.EventHandler(this.FormPiecesList_Load);
            ((System.ComponentModel.ISupportInitialize)(this.picLemming)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.picPiecePreview)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ListView listViewTerrain;
        private System.Windows.Forms.ColumnHeader columnIndex;
        private System.Windows.Forms.ColumnHeader columnPiece;
        private System.Windows.Forms.ColumnHeader columnType;
        private System.Windows.Forms.ColumnHeader columnDrawMode;
        private System.Windows.Forms.PictureBox picPiecePreview;
        private System.Windows.Forms.Button btnClose;
        private System.Windows.Forms.Button btnDrawFirst;
        private System.Windows.Forms.Button btnDrawSooner;
        private System.Windows.Forms.Button btnDrawLater;
        private System.Windows.Forms.Button btnDrawLast;
        private System.Windows.Forms.Button btnDeleteTerr;
        private System.Windows.Forms.ListView listViewObjects;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.ColumnHeader columnHeader2;
        private System.Windows.Forms.ColumnHeader columnHeader3;
        private System.Windows.Forms.ColumnHeader columnHeader4;
        private System.Windows.Forms.Panel panelPadding;
        private System.Windows.Forms.PictureBox picLemming;
    }
}