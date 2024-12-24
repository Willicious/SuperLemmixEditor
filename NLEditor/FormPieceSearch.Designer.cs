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
            this.textBoxPieceName = new System.Windows.Forms.TextBox();
            this.lblPieceName = new System.Windows.Forms.Label();
            this.pictureBoxPreview = new System.Windows.Forms.PictureBox();
            this.lblMetaData = new System.Windows.Forms.Label();
            this.check_CurrentStyleOnly = new System.Windows.Forms.CheckBox();
            this.lblCurrentStyle = new System.Windows.Forms.Label();
            this.btnLoadStyle = new System.Windows.Forms.Button();
            this.btnAddPiece = new System.Windows.Forms.Button();
            this.lblFilterResults = new System.Windows.Forms.Label();
            this.formPadding = new System.Windows.Forms.PictureBox();
            this.check_CanResize = new System.Windows.Forms.CheckBox();
            this.check_CanNineSlice = new System.Windows.Forms.CheckBox();
            this.lblTriggerEffect = new System.Windows.Forms.Label();
            this.cbTriggerEffect = new System.Windows.Forms.ComboBox();
            this.check_Steel = new System.Windows.Forms.CheckBox();
            this.progressBar = new System.Windows.Forms.ProgressBar();
            this.lblStyleName = new System.Windows.Forms.Label();
            this.textBoxStyleName = new System.Windows.Forms.TextBox();
            this.btnClearFilters = new System.Windows.Forms.Button();
            this.btnClose = new System.Windows.Forms.Button();
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
            // textBoxPieceName
            // 
            this.textBoxPieceName.Location = new System.Drawing.Point(119, 54);
            this.textBoxPieceName.Name = "textBoxPieceName";
            this.textBoxPieceName.Size = new System.Drawing.Size(218, 26);
            this.textBoxPieceName.TabIndex = 1;
            this.textBoxPieceName.Text = "(Any)";
            this.textBoxPieceName.TextChanged += new System.EventHandler(this.textBox_TextChanged);
            this.textBoxPieceName.KeyDown += new System.Windows.Forms.KeyEventHandler(this.textBox_KeyDown);
            this.textBoxPieceName.MouseDown += new System.Windows.Forms.MouseEventHandler(this.textBox_MouseDown);
            // 
            // lblPieceName
            // 
            this.lblPieceName.AutoSize = true;
            this.lblPieceName.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblPieceName.Location = new System.Drawing.Point(17, 57);
            this.lblPieceName.Name = "lblPieceName";
            this.lblPieceName.Size = new System.Drawing.Size(96, 20);
            this.lblPieceName.TabIndex = 2;
            this.lblPieceName.Text = "Piece name:";
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
            this.check_CurrentStyleOnly.Location = new System.Drawing.Point(21, 102);
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
            this.lblCurrentStyle.Location = new System.Drawing.Point(144, 103);
            this.lblCurrentStyle.Name = "lblCurrentStyle";
            this.lblCurrentStyle.Size = new System.Drawing.Size(126, 20);
            this.lblCurrentStyle.TabIndex = 6;
            this.lblCurrentStyle.Text = "(Current Style)";
            // 
            // btnLoadStyle
            // 
            this.btnLoadStyle.Location = new System.Drawing.Point(815, 367);
            this.btnLoadStyle.Name = "btnLoadStyle";
            this.btnLoadStyle.Size = new System.Drawing.Size(150, 40);
            this.btnLoadStyle.TabIndex = 7;
            this.btnLoadStyle.Text = "Load Style";
            this.btnLoadStyle.UseVisualStyleBackColor = true;
            this.btnLoadStyle.Click += new System.EventHandler(this.btnLoadStyle_Click);
            // 
            // btnAddPiece
            // 
            this.btnAddPiece.Location = new System.Drawing.Point(971, 367);
            this.btnAddPiece.Name = "btnAddPiece";
            this.btnAddPiece.Size = new System.Drawing.Size(150, 40);
            this.btnAddPiece.TabIndex = 10;
            this.btnAddPiece.Text = "Add Piece";
            this.btnAddPiece.UseVisualStyleBackColor = true;
            this.btnAddPiece.Click += new System.EventHandler(this.btnAddPiece_Click);
            // 
            // lblFilterResults
            // 
            this.lblFilterResults.AutoSize = true;
            this.lblFilterResults.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblFilterResults.Location = new System.Drawing.Point(17, 24);
            this.lblFilterResults.Name = "lblFilterResults";
            this.lblFilterResults.Size = new System.Drawing.Size(114, 20);
            this.lblFilterResults.TabIndex = 11;
            this.lblFilterResults.Text = "Filter results:";
            // 
            // formPadding
            // 
            this.formPadding.Location = new System.Drawing.Point(1061, 1);
            this.formPadding.Name = "formPadding";
            this.formPadding.Size = new System.Drawing.Size(242, 406);
            this.formPadding.TabIndex = 13;
            this.formPadding.TabStop = false;
            // 
            // check_CanResize
            // 
            this.check_CanResize.AutoSize = true;
            this.check_CanResize.Location = new System.Drawing.Point(21, 228);
            this.check_CanResize.Name = "check_CanResize";
            this.check_CanResize.Size = new System.Drawing.Size(110, 24);
            this.check_CanResize.TabIndex = 14;
            this.check_CanResize.Text = "Can resize";
            this.check_CanResize.UseVisualStyleBackColor = true;
            this.check_CanResize.CheckedChanged += new System.EventHandler(this.check_CanResize_CheckedChanged);
            // 
            // check_CanNineSlice
            // 
            this.check_CanNineSlice.AutoSize = true;
            this.check_CanNineSlice.Location = new System.Drawing.Point(148, 228);
            this.check_CanNineSlice.Name = "check_CanNineSlice";
            this.check_CanNineSlice.Size = new System.Drawing.Size(132, 24);
            this.check_CanNineSlice.TabIndex = 15;
            this.check_CanNineSlice.Text = "Is Nine-Sliced";
            this.check_CanNineSlice.UseVisualStyleBackColor = true;
            this.check_CanNineSlice.CheckedChanged += new System.EventHandler(this.check_CanNineSlice_CheckedChanged);
            // 
            // lblTriggerEffect
            // 
            this.lblTriggerEffect.AutoSize = true;
            this.lblTriggerEffect.Location = new System.Drawing.Point(17, 183);
            this.lblTriggerEffect.Name = "lblTriggerEffect";
            this.lblTriggerEffect.Size = new System.Drawing.Size(107, 20);
            this.lblTriggerEffect.TabIndex = 16;
            this.lblTriggerEffect.Text = "Trigger effect:";
            // 
            // cbTriggerEffect
            // 
            this.cbTriggerEffect.FormattingEnabled = true;
            this.cbTriggerEffect.Items.AddRange(new object[] {
            "<Any>",
            "Entrance",
            "Exit",
            "Fire",
            "Water",
            "Blasticine",
            "Vinewater",
            "Poison",
            "Lava",
            "Trap",
            "Updraft",
            "Teleporter",
            "Receiver",
            "Splitter",
            "Radiation",
            "Slowfreeze",
            "Button",
            "Collectible",
            "Splat",
            "NoSplat",
            "Force",
            "OneWay"});
            this.cbTriggerEffect.Location = new System.Drawing.Point(131, 180);
            this.cbTriggerEffect.Name = "cbTriggerEffect";
            this.cbTriggerEffect.Size = new System.Drawing.Size(206, 28);
            this.cbTriggerEffect.TabIndex = 17;
            this.cbTriggerEffect.Text = "<Any>";
            this.cbTriggerEffect.SelectedIndexChanged += new System.EventHandler(this.cbTriggerEffect_SelectedIndexChanged);
            // 
            // check_Steel
            // 
            this.check_Steel.AutoSize = true;
            this.check_Steel.Location = new System.Drawing.Point(21, 258);
            this.check_Steel.Name = "check_Steel";
            this.check_Steel.Size = new System.Drawing.Size(72, 24);
            this.check_Steel.TabIndex = 18;
            this.check_Steel.Text = "Steel";
            this.check_Steel.UseVisualStyleBackColor = true;
            this.check_Steel.CheckedChanged += new System.EventHandler(this.check_Steel_CheckedChanged);
            // 
            // progressBar
            // 
            this.progressBar.Location = new System.Drawing.Point(359, 377);
            this.progressBar.Name = "progressBar";
            this.progressBar.Size = new System.Drawing.Size(432, 30);
            this.progressBar.Style = System.Windows.Forms.ProgressBarStyle.Continuous;
            this.progressBar.TabIndex = 21;
            this.progressBar.Visible = false;
            // 
            // lblStyleName
            // 
            this.lblStyleName.AutoSize = true;
            this.lblStyleName.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblStyleName.Location = new System.Drawing.Point(17, 135);
            this.lblStyleName.Name = "lblStyleName";
            this.lblStyleName.Size = new System.Drawing.Size(92, 20);
            this.lblStyleName.TabIndex = 22;
            this.lblStyleName.Text = "Style name:";
            // 
            // textBoxStyleName
            // 
            this.textBoxStyleName.Location = new System.Drawing.Point(119, 132);
            this.textBoxStyleName.Name = "textBoxStyleName";
            this.textBoxStyleName.Size = new System.Drawing.Size(218, 26);
            this.textBoxStyleName.TabIndex = 23;
            this.textBoxStyleName.Text = "(Any)";
            this.textBoxStyleName.TextChanged += new System.EventHandler(this.textBox_TextChanged);
            this.textBoxStyleName.KeyDown += new System.Windows.Forms.KeyEventHandler(this.textBox_KeyDown);
            this.textBoxStyleName.MouseDown += new System.Windows.Forms.MouseEventHandler(this.textBox_MouseDown);
            // 
            // btnClearFilters
            // 
            this.btnClearFilters.Location = new System.Drawing.Point(21, 367);
            this.btnClearFilters.Name = "btnClearFilters";
            this.btnClearFilters.Size = new System.Drawing.Size(316, 40);
            this.btnClearFilters.TabIndex = 24;
            this.btnClearFilters.Text = "Clear All Filters";
            this.btnClearFilters.UseVisualStyleBackColor = true;
            this.btnClearFilters.Click += new System.EventHandler(this.btnClearFilters_Click);
            // 
            // btnClose
            // 
            this.btnClose.Location = new System.Drawing.Point(1127, 367);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(150, 40);
            this.btnClose.TabIndex = 25;
            this.btnClose.Text = "Close";
            this.btnClose.UseVisualStyleBackColor = true;
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // FormPieceSearch
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.ClientSize = new System.Drawing.Size(1299, 423);
            this.Controls.Add(this.btnClose);
            this.Controls.Add(this.btnClearFilters);
            this.Controls.Add(this.textBoxStyleName);
            this.Controls.Add(this.lblStyleName);
            this.Controls.Add(this.progressBar);
            this.Controls.Add(this.check_Steel);
            this.Controls.Add(this.cbTriggerEffect);
            this.Controls.Add(this.lblTriggerEffect);
            this.Controls.Add(this.check_CanNineSlice);
            this.Controls.Add(this.check_CanResize);
            this.Controls.Add(this.lblFilterResults);
            this.Controls.Add(this.btnAddPiece);
            this.Controls.Add(this.btnLoadStyle);
            this.Controls.Add(this.lblCurrentStyle);
            this.Controls.Add(this.check_CurrentStyleOnly);
            this.Controls.Add(this.lblMetaData);
            this.Controls.Add(this.pictureBoxPreview);
            this.Controls.Add(this.lblPieceName);
            this.Controls.Add(this.textBoxPieceName);
            this.Controls.Add(this.listBoxSearchResults);
            this.Controls.Add(this.formPadding);
            this.DoubleBuffered = true;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FormPieceSearch";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Search Pieces";
            this.Load += new System.EventHandler(this.FormPieceSearch_Load);
            this.Click += new System.EventHandler(this.FormPieceSearch_Click);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxPreview)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.formPadding)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ListBox listBoxSearchResults;
        private System.Windows.Forms.TextBox textBoxPieceName;
        private System.Windows.Forms.Label lblPieceName;
        private System.Windows.Forms.PictureBox pictureBoxPreview;
        private System.Windows.Forms.Label lblMetaData;
        private System.Windows.Forms.CheckBox check_CurrentStyleOnly;
        private System.Windows.Forms.Label lblCurrentStyle;
        private System.Windows.Forms.Button btnLoadStyle;
        private System.Windows.Forms.Button btnAddPiece;
        private System.Windows.Forms.Label lblFilterResults;
        private System.Windows.Forms.PictureBox formPadding;
        private System.Windows.Forms.CheckBox check_CanResize;
        private System.Windows.Forms.CheckBox check_CanNineSlice;
        private System.Windows.Forms.Label lblTriggerEffect;
        private System.Windows.Forms.ComboBox cbTriggerEffect;
        private System.Windows.Forms.CheckBox check_Steel;
        private System.Windows.Forms.ProgressBar progressBar;
        private System.Windows.Forms.Label lblStyleName;
        private System.Windows.Forms.TextBox textBoxStyleName;
        private System.Windows.Forms.Button btnClearFilters;
        private System.Windows.Forms.Button btnClose;
    }
}