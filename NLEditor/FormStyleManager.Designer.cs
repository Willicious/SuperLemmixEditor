namespace NLEditor
{
    partial class FormStyleManager
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormStyleManager));
            this.btnMoveUp1 = new System.Windows.Forms.Button();
            this.btnMoveDown1 = new System.Windows.Forms.Button();
            this.btnSave = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.listStyles = new System.Windows.Forms.ListView();
            this.colFolderName = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.colDisplayName = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.txtDisplayName = new System.Windows.Forms.TextBox();
            this.btnRename = new System.Windows.Forms.Button();
            this.btnAddNew = new System.Windows.Forms.Button();
            this.btnMoveUp10 = new System.Windows.Forms.Button();
            this.btnMoveDown10 = new System.Windows.Forms.Button();
            this.btnSortAlphabetically = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // btnMoveUp1
            // 
            this.btnMoveUp1.Location = new System.Drawing.Point(527, 128);
            this.btnMoveUp1.Name = "btnMoveUp1";
            this.btnMoveUp1.Size = new System.Drawing.Size(137, 55);
            this.btnMoveUp1.TabIndex = 1;
            this.btnMoveUp1.Text = "Move Up 1";
            this.btnMoveUp1.UseVisualStyleBackColor = true;
            this.btnMoveUp1.Click += new System.EventHandler(this.BtnMoveUp_Click);
            // 
            // btnMoveDown1
            // 
            this.btnMoveDown1.Location = new System.Drawing.Point(527, 189);
            this.btnMoveDown1.Name = "btnMoveDown1";
            this.btnMoveDown1.Size = new System.Drawing.Size(137, 55);
            this.btnMoveDown1.TabIndex = 2;
            this.btnMoveDown1.Text = "Move Down 1";
            this.btnMoveDown1.UseVisualStyleBackColor = true;
            this.btnMoveDown1.Click += new System.EventHandler(this.BtnMoveDown_Click);
            // 
            // btnSave
            // 
            this.btnSave.Location = new System.Drawing.Point(527, 572);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(277, 55);
            this.btnSave.TabIndex = 3;
            this.btnSave.Text = "Save";
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Click += new System.EventHandler(this.BtnSave_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(527, 633);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(277, 55);
            this.btnCancel.TabIndex = 4;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.BtnCancel_Click);
            // 
            // listStyles
            // 
            this.listStyles.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.colFolderName,
            this.colDisplayName});
            this.listStyles.FullRowSelect = true;
            this.listStyles.GridLines = true;
            this.listStyles.HideSelection = false;
            this.listStyles.Location = new System.Drawing.Point(13, 22);
            this.listStyles.Name = "listStyles";
            this.listStyles.Size = new System.Drawing.Size(499, 666);
            this.listStyles.TabIndex = 5;
            this.listStyles.UseCompatibleStateImageBehavior = false;
            this.listStyles.View = System.Windows.Forms.View.Details;
            this.listStyles.SelectedIndexChanged += new System.EventHandler(this.ListStyles_SelectedIndexChanged);
            // 
            // colFolderName
            // 
            this.colFolderName.Text = "Folder Name";
            this.colFolderName.Width = 130;
            // 
            // colDisplayName
            // 
            this.colDisplayName.Text = "Style Name";
            this.colDisplayName.Width = 130;
            // 
            // txtDisplayName
            // 
            this.txtDisplayName.Location = new System.Drawing.Point(527, 301);
            this.txtDisplayName.Name = "txtDisplayName";
            this.txtDisplayName.Size = new System.Drawing.Size(277, 26);
            this.txtDisplayName.TabIndex = 6;
            this.txtDisplayName.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtDisplayName_KeyDown);
            // 
            // btnRename
            // 
            this.btnRename.Location = new System.Drawing.Point(527, 333);
            this.btnRename.Name = "btnRename";
            this.btnRename.Size = new System.Drawing.Size(277, 55);
            this.btnRename.TabIndex = 7;
            this.btnRename.Text = "Rename";
            this.btnRename.UseVisualStyleBackColor = true;
            this.btnRename.Click += new System.EventHandler(this.BtnRename_Click);
            // 
            // btnAddNew
            // 
            this.btnAddNew.Location = new System.Drawing.Point(527, 22);
            this.btnAddNew.Name = "btnAddNew";
            this.btnAddNew.Size = new System.Drawing.Size(277, 55);
            this.btnAddNew.TabIndex = 8;
            this.btnAddNew.Text = "Add New Style";
            this.btnAddNew.UseVisualStyleBackColor = true;
            this.btnAddNew.Click += new System.EventHandler(this.BtnAddNew_Click);
            // 
            // btnMoveUp10
            // 
            this.btnMoveUp10.Location = new System.Drawing.Point(670, 128);
            this.btnMoveUp10.Name = "btnMoveUp10";
            this.btnMoveUp10.Size = new System.Drawing.Size(134, 55);
            this.btnMoveUp10.TabIndex = 9;
            this.btnMoveUp10.Text = "Move Up 10";
            this.btnMoveUp10.UseVisualStyleBackColor = true;
            this.btnMoveUp10.Click += new System.EventHandler(this.BtnMoveUp_Click);
            // 
            // btnMoveDown10
            // 
            this.btnMoveDown10.Location = new System.Drawing.Point(670, 189);
            this.btnMoveDown10.Name = "btnMoveDown10";
            this.btnMoveDown10.Size = new System.Drawing.Size(134, 55);
            this.btnMoveDown10.TabIndex = 10;
            this.btnMoveDown10.Text = "Move Down 10";
            this.btnMoveDown10.UseVisualStyleBackColor = true;
            this.btnMoveDown10.Click += new System.EventHandler(this.BtnMoveDown_Click);
            // 
            // btnSortAlphabetically
            // 
            this.btnSortAlphabetically.Location = new System.Drawing.Point(527, 449);
            this.btnSortAlphabetically.Name = "btnSortAlphabetically";
            this.btnSortAlphabetically.Size = new System.Drawing.Size(277, 55);
            this.btnSortAlphabetically.TabIndex = 11;
            this.btnSortAlphabetically.Text = "Sort Alphabetically";
            this.btnSortAlphabetically.UseVisualStyleBackColor = true;
            this.btnSortAlphabetically.Click += new System.EventHandler(this.btnSortAlphabetically_Click);
            // 
            // FormStyleManager
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(815, 702);
            this.Controls.Add(this.btnSortAlphabetically);
            this.Controls.Add(this.btnMoveDown10);
            this.Controls.Add(this.btnMoveUp10);
            this.Controls.Add(this.btnAddNew);
            this.Controls.Add(this.btnRename);
            this.Controls.Add(this.txtDisplayName);
            this.Controls.Add(this.listStyles);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnSave);
            this.Controls.Add(this.btnMoveDown1);
            this.Controls.Add(this.btnMoveUp1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "FormStyleManager";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Style Manager";
            this.Load += new System.EventHandler(this.FormStyleManager_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Button btnMoveUp1;
        private System.Windows.Forms.Button btnMoveDown1;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.ListView listStyles;
        private System.Windows.Forms.ColumnHeader colFolderName;
        private System.Windows.Forms.ColumnHeader colDisplayName;
        private System.Windows.Forms.TextBox txtDisplayName;
        private System.Windows.Forms.Button btnRename;
        private System.Windows.Forms.Button btnAddNew;
        private System.Windows.Forms.Button btnMoveUp10;
        private System.Windows.Forms.Button btnMoveDown10;
        private System.Windows.Forms.Button btnSortAlphabetically;
    }
}