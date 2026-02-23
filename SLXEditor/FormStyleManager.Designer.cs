namespace SLXEditor
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
            this.colPinnedStatus = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.colRandomizerStatus = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.txtDisplayName = new System.Windows.Forms.TextBox();
            this.btnRename = new System.Windows.Forms.Button();
            this.btnAddNew = new System.Windows.Forms.Button();
            this.btnMoveUp10 = new System.Windows.Forms.Button();
            this.btnMoveDown10 = new System.Windows.Forms.Button();
            this.btnSortAlphabetically = new System.Windows.Forms.Button();
            this.btnPinToTop = new System.Windows.Forms.Button();
            this.btnPinToBottom = new System.Windows.Forms.Button();
            this.lblSearch = new System.Windows.Forms.Label();
            this.txtSearch = new System.Windows.Forms.TextBox();
            this.btnShowSelectedItemsInList = new System.Windows.Forms.Button();
            this.btnClearSearch = new System.Windows.Forms.Button();
            this.btnUnpin = new System.Windows.Forms.Button();
            this.picPadding = new System.Windows.Forms.PictureBox();
            this.checkAutoPinOGStyles = new System.Windows.Forms.CheckBox();
            this.btnRandomizer = new System.Windows.Forms.Button();
            this.checkShowRandomButton = new System.Windows.Forms.CheckBox();
            ((System.ComponentModel.ISupportInitialize)(this.picPadding)).BeginInit();
            this.SuspendLayout();
            // 
            // btnMoveUp1
            // 
            this.btnMoveUp1.Location = new System.Drawing.Point(736, 104);
            this.btnMoveUp1.Name = "btnMoveUp1";
            this.btnMoveUp1.Size = new System.Drawing.Size(137, 44);
            this.btnMoveUp1.TabIndex = 1;
            this.btnMoveUp1.Text = "Move Up 1";
            this.btnMoveUp1.UseVisualStyleBackColor = true;
            this.btnMoveUp1.Click += new System.EventHandler(this.BtnMoveStyles_Click);
            // 
            // btnMoveDown1
            // 
            this.btnMoveDown1.Location = new System.Drawing.Point(879, 104);
            this.btnMoveDown1.Name = "btnMoveDown1";
            this.btnMoveDown1.Size = new System.Drawing.Size(134, 44);
            this.btnMoveDown1.TabIndex = 2;
            this.btnMoveDown1.Text = "Move Down 1";
            this.btnMoveDown1.UseVisualStyleBackColor = true;
            this.btnMoveDown1.Click += new System.EventHandler(this.BtnMoveStyles_Click);
            // 
            // btnSave
            // 
            this.btnSave.Location = new System.Drawing.Point(736, 709);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(277, 44);
            this.btnSave.TabIndex = 3;
            this.btnSave.Text = "Save";
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Click += new System.EventHandler(this.BtnSave_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(736, 759);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(277, 44);
            this.btnCancel.TabIndex = 4;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.BtnCancel_Click);
            // 
            // listStyles
            // 
            this.listStyles.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.colFolderName,
            this.colDisplayName,
            this.colPinnedStatus,
            this.colRandomizerStatus});
            this.listStyles.FullRowSelect = true;
            this.listStyles.GridLines = true;
            this.listStyles.HideSelection = false;
            this.listStyles.Location = new System.Drawing.Point(17, 59);
            this.listStyles.Name = "listStyles";
            this.listStyles.Size = new System.Drawing.Size(698, 744);
            this.listStyles.TabIndex = 5;
            this.listStyles.UseCompatibleStateImageBehavior = false;
            this.listStyles.View = System.Windows.Forms.View.Details;
            this.listStyles.SelectedIndexChanged += new System.EventHandler(this.ListStyles_SelectedIndexChanged);
            // 
            // colFolderName
            // 
            this.colFolderName.Text = "Folder Name";
            this.colFolderName.Width = 140;
            // 
            // colDisplayName
            // 
            this.colDisplayName.Text = "Display Name";
            this.colDisplayName.Width = 140;
            // 
            // colPinnedStatus
            // 
            this.colPinnedStatus.Text = "Pinned";
            this.colPinnedStatus.Width = 80;
            // 
            // colRandomizerStatus
            // 
            this.colRandomizerStatus.Text = "Randomize";
            this.colRandomizerStatus.Width = 80;
            // 
            // txtDisplayName
            // 
            this.txtDisplayName.Location = new System.Drawing.Point(736, 497);
            this.txtDisplayName.Name = "txtDisplayName";
            this.txtDisplayName.Size = new System.Drawing.Size(277, 26);
            this.txtDisplayName.TabIndex = 6;
            this.txtDisplayName.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtDisplayName_KeyDown);
            // 
            // btnRename
            // 
            this.btnRename.Location = new System.Drawing.Point(736, 529);
            this.btnRename.Name = "btnRename";
            this.btnRename.Size = new System.Drawing.Size(277, 44);
            this.btnRename.TabIndex = 7;
            this.btnRename.Text = "Rename";
            this.btnRename.UseVisualStyleBackColor = true;
            this.btnRename.Click += new System.EventHandler(this.BtnRename_Click);
            // 
            // btnAddNew
            // 
            this.btnAddNew.Location = new System.Drawing.Point(736, 16);
            this.btnAddNew.Name = "btnAddNew";
            this.btnAddNew.Size = new System.Drawing.Size(277, 44);
            this.btnAddNew.TabIndex = 8;
            this.btnAddNew.Text = "Add New Style";
            this.btnAddNew.UseVisualStyleBackColor = true;
            this.btnAddNew.Click += new System.EventHandler(this.BtnAddNew_Click);
            // 
            // btnMoveUp10
            // 
            this.btnMoveUp10.Location = new System.Drawing.Point(736, 154);
            this.btnMoveUp10.Name = "btnMoveUp10";
            this.btnMoveUp10.Size = new System.Drawing.Size(137, 44);
            this.btnMoveUp10.TabIndex = 9;
            this.btnMoveUp10.Text = "Move Up 10";
            this.btnMoveUp10.UseVisualStyleBackColor = true;
            this.btnMoveUp10.Click += new System.EventHandler(this.BtnMoveStyles_Click);
            // 
            // btnMoveDown10
            // 
            this.btnMoveDown10.Location = new System.Drawing.Point(879, 154);
            this.btnMoveDown10.Name = "btnMoveDown10";
            this.btnMoveDown10.Size = new System.Drawing.Size(134, 44);
            this.btnMoveDown10.TabIndex = 10;
            this.btnMoveDown10.Text = "Move Down 10";
            this.btnMoveDown10.UseVisualStyleBackColor = true;
            this.btnMoveDown10.Click += new System.EventHandler(this.BtnMoveStyles_Click);
            // 
            // btnSortAlphabetically
            // 
            this.btnSortAlphabetically.Location = new System.Drawing.Point(736, 621);
            this.btnSortAlphabetically.Name = "btnSortAlphabetically";
            this.btnSortAlphabetically.Size = new System.Drawing.Size(277, 44);
            this.btnSortAlphabetically.TabIndex = 11;
            this.btnSortAlphabetically.Text = "Sort Alphabetically";
            this.btnSortAlphabetically.UseVisualStyleBackColor = true;
            this.btnSortAlphabetically.Click += new System.EventHandler(this.btnSortAlphabetically_Click);
            // 
            // btnPinToTop
            // 
            this.btnPinToTop.Location = new System.Drawing.Point(736, 204);
            this.btnPinToTop.Name = "btnPinToTop";
            this.btnPinToTop.Size = new System.Drawing.Size(137, 44);
            this.btnPinToTop.TabIndex = 12;
            this.btnPinToTop.Text = "Pin To Top";
            this.btnPinToTop.UseVisualStyleBackColor = true;
            this.btnPinToTop.Click += new System.EventHandler(this.btnPinToTop_Click);
            // 
            // btnPinToBottom
            // 
            this.btnPinToBottom.Location = new System.Drawing.Point(879, 204);
            this.btnPinToBottom.Name = "btnPinToBottom";
            this.btnPinToBottom.Size = new System.Drawing.Size(134, 44);
            this.btnPinToBottom.TabIndex = 13;
            this.btnPinToBottom.Text = "Pin To Bottom";
            this.btnPinToBottom.UseVisualStyleBackColor = true;
            this.btnPinToBottom.Click += new System.EventHandler(this.btnPinToBottom_Click);
            // 
            // lblSearch
            // 
            this.lblSearch.AutoSize = true;
            this.lblSearch.Location = new System.Drawing.Point(13, 22);
            this.lblSearch.Name = "lblSearch";
            this.lblSearch.Size = new System.Drawing.Size(60, 20);
            this.lblSearch.TabIndex = 14;
            this.lblSearch.Text = "Search";
            // 
            // txtSearch
            // 
            this.txtSearch.Location = new System.Drawing.Point(85, 19);
            this.txtSearch.Name = "txtSearch";
            this.txtSearch.Size = new System.Drawing.Size(549, 26);
            this.txtSearch.TabIndex = 15;
            this.txtSearch.TextChanged += new System.EventHandler(this.txtSearch_TextChanged);
            // 
            // btnShowSelectedItemsInList
            // 
            this.btnShowSelectedItemsInList.Enabled = false;
            this.btnShowSelectedItemsInList.Location = new System.Drawing.Point(736, 59);
            this.btnShowSelectedItemsInList.Name = "btnShowSelectedItemsInList";
            this.btnShowSelectedItemsInList.Size = new System.Drawing.Size(277, 55);
            this.btnShowSelectedItemsInList.TabIndex = 16;
            this.btnShowSelectedItemsInList.Text = "Show Selected Items in List";
            this.btnShowSelectedItemsInList.UseVisualStyleBackColor = true;
            this.btnShowSelectedItemsInList.Visible = false;
            this.btnShowSelectedItemsInList.Click += new System.EventHandler(this.btnShowSelectedItemsInList_Click);
            // 
            // btnClearSearch
            // 
            this.btnClearSearch.Enabled = false;
            this.btnClearSearch.Location = new System.Drawing.Point(640, 19);
            this.btnClearSearch.Name = "btnClearSearch";
            this.btnClearSearch.Size = new System.Drawing.Size(75, 32);
            this.btnClearSearch.TabIndex = 17;
            this.btnClearSearch.Text = "Clear";
            this.btnClearSearch.UseVisualStyleBackColor = true;
            this.btnClearSearch.Click += new System.EventHandler(this.btnClearSearch_Click);
            // 
            // btnUnpin
            // 
            this.btnUnpin.Location = new System.Drawing.Point(736, 254);
            this.btnUnpin.Name = "btnUnpin";
            this.btnUnpin.Size = new System.Drawing.Size(277, 44);
            this.btnUnpin.TabIndex = 18;
            this.btnUnpin.Text = "Unpin";
            this.btnUnpin.UseVisualStyleBackColor = true;
            this.btnUnpin.Click += new System.EventHandler(this.btnUnpin_Click);
            // 
            // picPadding
            // 
            this.picPadding.BackColor = System.Drawing.Color.Transparent;
            this.picPadding.Location = new System.Drawing.Point(-1, 809);
            this.picPadding.Name = "picPadding";
            this.picPadding.Size = new System.Drawing.Size(1028, 10);
            this.picPadding.TabIndex = 19;
            this.picPadding.TabStop = false;
            // 
            // checkAutoPinOGStyles
            // 
            this.checkAutoPinOGStyles.AutoSize = true;
            this.checkAutoPinOGStyles.Checked = true;
            this.checkAutoPinOGStyles.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkAutoPinOGStyles.Location = new System.Drawing.Point(736, 304);
            this.checkAutoPinOGStyles.Name = "checkAutoPinOGStyles";
            this.checkAutoPinOGStyles.Size = new System.Drawing.Size(167, 24);
            this.checkAutoPinOGStyles.TabIndex = 20;
            this.checkAutoPinOGStyles.Text = "AutoPin OG Styles";
            this.checkAutoPinOGStyles.UseVisualStyleBackColor = true;
            this.checkAutoPinOGStyles.CheckedChanged += new System.EventHandler(this.checkAutoPinOGStyles_CheckedChanged);
            // 
            // btnRandomizer
            // 
            this.btnRandomizer.Location = new System.Drawing.Point(736, 374);
            this.btnRandomizer.Name = "btnRandomizer";
            this.btnRandomizer.Size = new System.Drawing.Size(277, 44);
            this.btnRandomizer.TabIndex = 21;
            this.btnRandomizer.Text = "Add to Randomizer";
            this.btnRandomizer.UseVisualStyleBackColor = true;
            this.btnRandomizer.Click += new System.EventHandler(this.btnRandomizer_Click);
            // 
            // checkShowRandomButton
            // 
            this.checkShowRandomButton.AutoSize = true;
            this.checkShowRandomButton.Location = new System.Drawing.Point(736, 429);
            this.checkShowRandomButton.Name = "checkShowRandomButton";
            this.checkShowRandomButton.Size = new System.Drawing.Size(270, 24);
            this.checkShowRandomButton.TabIndex = 22;
            this.checkShowRandomButton.Text = "Show Random Button in Browser";
            this.checkShowRandomButton.UseVisualStyleBackColor = true;
            this.checkShowRandomButton.CheckedChanged += new System.EventHandler(this.checkShowRandomButton_CheckedChanged);
            // 
            // FormStyleManager
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.ClientSize = new System.Drawing.Size(1030, 820);
            this.Controls.Add(this.checkShowRandomButton);
            this.Controls.Add(this.btnRandomizer);
            this.Controls.Add(this.checkAutoPinOGStyles);
            this.Controls.Add(this.picPadding);
            this.Controls.Add(this.btnUnpin);
            this.Controls.Add(this.btnShowSelectedItemsInList);
            this.Controls.Add(this.btnClearSearch);
            this.Controls.Add(this.txtSearch);
            this.Controls.Add(this.lblSearch);
            this.Controls.Add(this.btnPinToBottom);
            this.Controls.Add(this.btnPinToTop);
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
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "FormStyleManager";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Style Manager";
            this.Load += new System.EventHandler(this.FormStyleManager_Load);
            this.Shown += new System.EventHandler(this.FormStyleManager_Shown);
            this.Click += new System.EventHandler(this.FormStyleManager_Click);
            ((System.ComponentModel.ISupportInitialize)(this.picPadding)).EndInit();
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
        private System.Windows.Forms.ColumnHeader colPinnedStatus;
        private System.Windows.Forms.ColumnHeader colRandomizerStatus;
        private System.Windows.Forms.TextBox txtDisplayName;
        private System.Windows.Forms.Button btnRename;
        private System.Windows.Forms.Button btnAddNew;
        private System.Windows.Forms.Button btnMoveUp10;
        private System.Windows.Forms.Button btnMoveDown10;
        private System.Windows.Forms.Button btnSortAlphabetically;
        private System.Windows.Forms.Button btnPinToTop;
        private System.Windows.Forms.Button btnPinToBottom;
        private System.Windows.Forms.Label lblSearch;
        private System.Windows.Forms.TextBox txtSearch;
        private System.Windows.Forms.Button btnShowSelectedItemsInList;
        private System.Windows.Forms.Button btnClearSearch;
        private System.Windows.Forms.Button btnUnpin;
        private System.Windows.Forms.PictureBox picPadding;
        private System.Windows.Forms.CheckBox checkAutoPinOGStyles;
        private System.Windows.Forms.Button btnRandomizer;
        private System.Windows.Forms.CheckBox checkShowRandomButton;
    }
}