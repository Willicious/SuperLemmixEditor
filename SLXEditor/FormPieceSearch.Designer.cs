namespace SLXEditor
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
            this.pictureBoxPreview = new System.Windows.Forms.PictureBox();
            this.lblMetaData = new System.Windows.Forms.Label();
            this.progressBar = new System.Windows.Forms.ProgressBar();
            this.panelSearchFilters = new System.Windows.Forms.Panel();
            this.btnClearFilters = new System.Windows.Forms.Button();
            this.textBoxStyleName = new System.Windows.Forms.TextBox();
            this.lblStyleName = new System.Windows.Forms.Label();
            this.check_Steel = new System.Windows.Forms.CheckBox();
            this.cbTriggerEffect = new System.Windows.Forms.ComboBox();
            this.lblTriggerEffect = new System.Windows.Forms.Label();
            this.check_CanNineSlice = new System.Windows.Forms.CheckBox();
            this.check_CanResize = new System.Windows.Forms.CheckBox();
            this.lblCurrentStyle = new System.Windows.Forms.Label();
            this.check_CurrentStyleOnly = new System.Windows.Forms.CheckBox();
            this.lblPieceName = new System.Windows.Forms.Label();
            this.textBoxPieceName = new System.Windows.Forms.TextBox();
            this.lblFilterResults = new System.Windows.Forms.Label();
            this.panelButtons = new System.Windows.Forms.Panel();
            this.lblButtonHint = new System.Windows.Forms.Label();
            this.btnClose = new System.Windows.Forms.Button();
            this.btnAddPiece = new System.Windows.Forms.Button();
            this.btnLoadPiece = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxPreview)).BeginInit();
            this.panelSearchFilters.SuspendLayout();
            this.panelButtons.SuspendLayout();
            this.SuspendLayout();
            // 
            // listBoxSearchResults
            // 
            this.listBoxSearchResults.FormattingEnabled = true;
            this.listBoxSearchResults.ItemHeight = 20;
            this.listBoxSearchResults.Location = new System.Drawing.Point(359, 40);
            this.listBoxSearchResults.Name = "listBoxSearchResults";
            this.listBoxSearchResults.Size = new System.Drawing.Size(432, 384);
            this.listBoxSearchResults.TabIndex = 0;
            this.listBoxSearchResults.SelectedIndexChanged += new System.EventHandler(this.listBoxSearchResults_SelectedIndexChanged);
            // 
            // pictureBoxPreview
            // 
            this.pictureBoxPreview.Location = new System.Drawing.Point(815, 40);
            this.pictureBoxPreview.Name = "pictureBoxPreview";
            this.pictureBoxPreview.Size = new System.Drawing.Size(296, 268);
            this.pictureBoxPreview.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBoxPreview.TabIndex = 3;
            this.pictureBoxPreview.TabStop = false;
            // 
            // lblMetaData
            // 
            this.lblMetaData.AutoSize = true;
            this.lblMetaData.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblMetaData.Location = new System.Drawing.Point(811, 311);
            this.lblMetaData.Name = "lblMetaData";
            this.lblMetaData.Size = new System.Drawing.Size(70, 20);
            this.lblMetaData.TabIndex = 4;
            this.lblMetaData.Text = "Preview";
            // 
            // progressBar
            // 
            this.progressBar.Location = new System.Drawing.Point(359, 393);
            this.progressBar.Name = "progressBar";
            this.progressBar.Size = new System.Drawing.Size(432, 30);
            this.progressBar.Style = System.Windows.Forms.ProgressBarStyle.Continuous;
            this.progressBar.TabIndex = 21;
            this.progressBar.Visible = false;
            // 
            // panelSearchFilters
            // 
            this.panelSearchFilters.Controls.Add(this.btnClearFilters);
            this.panelSearchFilters.Controls.Add(this.textBoxStyleName);
            this.panelSearchFilters.Controls.Add(this.lblStyleName);
            this.panelSearchFilters.Controls.Add(this.check_Steel);
            this.panelSearchFilters.Controls.Add(this.cbTriggerEffect);
            this.panelSearchFilters.Controls.Add(this.lblTriggerEffect);
            this.panelSearchFilters.Controls.Add(this.check_CanNineSlice);
            this.panelSearchFilters.Controls.Add(this.check_CanResize);
            this.panelSearchFilters.Controls.Add(this.lblCurrentStyle);
            this.panelSearchFilters.Controls.Add(this.check_CurrentStyleOnly);
            this.panelSearchFilters.Controls.Add(this.lblPieceName);
            this.panelSearchFilters.Controls.Add(this.textBoxPieceName);
            this.panelSearchFilters.Location = new System.Drawing.Point(12, 63);
            this.panelSearchFilters.Name = "panelSearchFilters";
            this.panelSearchFilters.Size = new System.Drawing.Size(341, 390);
            this.panelSearchFilters.TabIndex = 27;
            // 
            // btnClearFilters
            // 
            this.btnClearFilters.Location = new System.Drawing.Point(13, 323);
            this.btnClearFilters.Name = "btnClearFilters";
            this.btnClearFilters.Size = new System.Drawing.Size(316, 40);
            this.btnClearFilters.TabIndex = 31;
            this.btnClearFilters.Text = "Clear All Filters";
            this.btnClearFilters.UseVisualStyleBackColor = true;
            this.btnClearFilters.Click += new System.EventHandler(this.btnClearFilters_Click);
            // 
            // textBoxStyleName
            // 
            this.textBoxStyleName.Location = new System.Drawing.Point(111, 128);
            this.textBoxStyleName.Name = "textBoxStyleName";
            this.textBoxStyleName.Size = new System.Drawing.Size(218, 26);
            this.textBoxStyleName.TabIndex = 30;
            this.textBoxStyleName.Text = "(Any)";
            this.textBoxStyleName.TextChanged += new System.EventHandler(this.textBox_TextChanged);
            this.textBoxStyleName.KeyDown += new System.Windows.Forms.KeyEventHandler(this.textBox_KeyDown);
            this.textBoxStyleName.MouseDown += new System.Windows.Forms.MouseEventHandler(this.textBox_MouseDown);
            // 
            // lblStyleName
            // 
            this.lblStyleName.AutoSize = true;
            this.lblStyleName.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblStyleName.Location = new System.Drawing.Point(9, 131);
            this.lblStyleName.Name = "lblStyleName";
            this.lblStyleName.Size = new System.Drawing.Size(92, 20);
            this.lblStyleName.TabIndex = 29;
            this.lblStyleName.Text = "Style name:";
            // 
            // check_Steel
            // 
            this.check_Steel.AutoSize = true;
            this.check_Steel.Location = new System.Drawing.Point(13, 254);
            this.check_Steel.Name = "check_Steel";
            this.check_Steel.Size = new System.Drawing.Size(72, 24);
            this.check_Steel.TabIndex = 28;
            this.check_Steel.Text = "Steel";
            this.check_Steel.UseVisualStyleBackColor = true;
            this.check_Steel.CheckedChanged += new System.EventHandler(this.check_Steel_CheckedChanged);
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
            "Portal",
            "Splitter",
            "Radiation",
            "Slowfreeze",
            "Button",
            "Collectible",
            "SplatPad",
            "AntiSplatPad",
            "ForceField",
            "OneWay",
            "Neutralizer",
            "DeNeutralizer",
            "Normalizer",
            "SkillAssigner",
            "PermaSkillAssigner",
            "PermaSkillRemover",
            "Animation",
            "Decoration",
            "Paint"});
            this.cbTriggerEffect.Location = new System.Drawing.Point(123, 176);
            this.cbTriggerEffect.Name = "cbTriggerEffect";
            this.cbTriggerEffect.Size = new System.Drawing.Size(206, 28);
            this.cbTriggerEffect.TabIndex = 27;
            this.cbTriggerEffect.Text = "<Any>";
            this.cbTriggerEffect.SelectedIndexChanged += new System.EventHandler(this.cbTriggerEffect_SelectedIndexChanged);
            // 
            // lblTriggerEffect
            // 
            this.lblTriggerEffect.AutoSize = true;
            this.lblTriggerEffect.Location = new System.Drawing.Point(9, 179);
            this.lblTriggerEffect.Name = "lblTriggerEffect";
            this.lblTriggerEffect.Size = new System.Drawing.Size(107, 20);
            this.lblTriggerEffect.TabIndex = 26;
            this.lblTriggerEffect.Text = "Trigger effect:";
            // 
            // check_CanNineSlice
            // 
            this.check_CanNineSlice.AutoSize = true;
            this.check_CanNineSlice.Location = new System.Drawing.Point(140, 224);
            this.check_CanNineSlice.Name = "check_CanNineSlice";
            this.check_CanNineSlice.Size = new System.Drawing.Size(132, 24);
            this.check_CanNineSlice.TabIndex = 25;
            this.check_CanNineSlice.Text = "Is Nine-Sliced";
            this.check_CanNineSlice.UseVisualStyleBackColor = true;
            this.check_CanNineSlice.CheckStateChanged += new System.EventHandler(this.check_CanNineSlice_CheckedChanged);
            // 
            // check_CanResize
            // 
            this.check_CanResize.AutoSize = true;
            this.check_CanResize.Location = new System.Drawing.Point(13, 224);
            this.check_CanResize.Name = "check_CanResize";
            this.check_CanResize.Size = new System.Drawing.Size(110, 24);
            this.check_CanResize.TabIndex = 24;
            this.check_CanResize.Text = "Can resize";
            this.check_CanResize.UseVisualStyleBackColor = true;
            this.check_CanResize.CheckedChanged += new System.EventHandler(this.check_CanResize_CheckedChanged);
            // 
            // lblCurrentStyle
            // 
            this.lblCurrentStyle.AutoSize = true;
            this.lblCurrentStyle.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblCurrentStyle.Location = new System.Drawing.Point(143, 84);
            this.lblCurrentStyle.Name = "lblCurrentStyle";
            this.lblCurrentStyle.Size = new System.Drawing.Size(126, 20);
            this.lblCurrentStyle.TabIndex = 16;
            this.lblCurrentStyle.Text = "(Current Style)";
            // 
            // check_CurrentStyleOnly
            // 
            this.check_CurrentStyleOnly.AutoSize = true;
            this.check_CurrentStyleOnly.Location = new System.Drawing.Point(13, 83);
            this.check_CurrentStyleOnly.Name = "check_CurrentStyleOnly";
            this.check_CurrentStyleOnly.Size = new System.Drawing.Size(128, 24);
            this.check_CurrentStyleOnly.TabIndex = 15;
            this.check_CurrentStyleOnly.Text = "Current style:";
            this.check_CurrentStyleOnly.UseVisualStyleBackColor = true;
            this.check_CurrentStyleOnly.CheckedChanged += new System.EventHandler(this.check_CurrentStyleOnly_CheckedChanged);
            // 
            // lblPieceName
            // 
            this.lblPieceName.AutoSize = true;
            this.lblPieceName.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblPieceName.Location = new System.Drawing.Point(9, 38);
            this.lblPieceName.Name = "lblPieceName";
            this.lblPieceName.Size = new System.Drawing.Size(96, 20);
            this.lblPieceName.TabIndex = 14;
            this.lblPieceName.Text = "Piece name:";
            // 
            // textBoxPieceName
            // 
            this.textBoxPieceName.Location = new System.Drawing.Point(111, 35);
            this.textBoxPieceName.Name = "textBoxPieceName";
            this.textBoxPieceName.Size = new System.Drawing.Size(218, 26);
            this.textBoxPieceName.TabIndex = 13;
            this.textBoxPieceName.Text = "(Any)";
            this.textBoxPieceName.TextChanged += new System.EventHandler(this.textBox_TextChanged);
            this.textBoxPieceName.KeyDown += new System.Windows.Forms.KeyEventHandler(this.textBox_KeyDown);
            this.textBoxPieceName.MouseDown += new System.Windows.Forms.MouseEventHandler(this.textBox_MouseDown);
            // 
            // lblFilterResults
            // 
            this.lblFilterResults.AutoSize = true;
            this.lblFilterResults.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblFilterResults.Location = new System.Drawing.Point(21, 40);
            this.lblFilterResults.Name = "lblFilterResults";
            this.lblFilterResults.Size = new System.Drawing.Size(114, 20);
            this.lblFilterResults.TabIndex = 28;
            this.lblFilterResults.Text = "Filter results:";
            // 
            // panelButtons
            // 
            this.panelButtons.Controls.Add(this.lblButtonHint);
            this.panelButtons.Controls.Add(this.btnClose);
            this.panelButtons.Controls.Add(this.btnAddPiece);
            this.panelButtons.Controls.Add(this.btnLoadPiece);
            this.panelButtons.Location = new System.Drawing.Point(359, 0);
            this.panelButtons.Name = "panelButtons";
            this.panelButtons.Size = new System.Drawing.Size(941, 453);
            this.panelButtons.TabIndex = 30;
            // 
            // lblButtonHint
            // 
            this.lblButtonHint.AutoSize = true;
            this.lblButtonHint.ForeColor = System.Drawing.Color.RoyalBlue;
            this.lblButtonHint.Location = new System.Drawing.Point(833, 9);
            this.lblButtonHint.Name = "lblButtonHint";
            this.lblButtonHint.Size = new System.Drawing.Size(87, 20);
            this.lblButtonHint.TabIndex = 31;
            this.lblButtonHint.Text = "Button hint";
            // 
            // btnClose
            // 
            this.btnClose.Location = new System.Drawing.Point(770, 386);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(150, 40);
            this.btnClose.TabIndex = 28;
            this.btnClose.Text = "Close";
            this.btnClose.UseVisualStyleBackColor = true;
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // btnAddPiece
            // 
            this.btnAddPiece.Location = new System.Drawing.Point(770, 88);
            this.btnAddPiece.Name = "btnAddPiece";
            this.btnAddPiece.Size = new System.Drawing.Size(150, 40);
            this.btnAddPiece.TabIndex = 27;
            this.btnAddPiece.Text = "Add Piece";
            this.btnAddPiece.UseVisualStyleBackColor = true;
            this.btnAddPiece.Click += new System.EventHandler(this.btnAddPiece_Click);
            this.btnAddPiece.MouseEnter += new System.EventHandler(this.ButtonMouseEnter);
            this.btnAddPiece.MouseLeave += new System.EventHandler(this.ButtonMouseLeave);
            // 
            // btnLoadPiece
            // 
            this.btnLoadPiece.Location = new System.Drawing.Point(770, 42);
            this.btnLoadPiece.Name = "btnLoadPiece";
            this.btnLoadPiece.Size = new System.Drawing.Size(150, 40);
            this.btnLoadPiece.TabIndex = 26;
            this.btnLoadPiece.Text = "Load Piece";
            this.btnLoadPiece.UseVisualStyleBackColor = true;
            this.btnLoadPiece.Click += new System.EventHandler(this.btnLoadPiece_Click);
            this.btnLoadPiece.MouseEnter += new System.EventHandler(this.ButtonMouseEnter);
            this.btnLoadPiece.MouseLeave += new System.EventHandler(this.ButtonMouseLeave);
            // 
            // FormPieceSearch
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.ClientSize = new System.Drawing.Size(1299, 453);
            this.Controls.Add(this.lblFilterResults);
            this.Controls.Add(this.panelSearchFilters);
            this.Controls.Add(this.progressBar);
            this.Controls.Add(this.lblMetaData);
            this.Controls.Add(this.pictureBoxPreview);
            this.Controls.Add(this.listBoxSearchResults);
            this.Controls.Add(this.panelButtons);
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
            this.Shown += new System.EventHandler(this.FormPieceSearch_Shown);
            this.Click += new System.EventHandler(this.FormPieceSearch_Click);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxPreview)).EndInit();
            this.panelSearchFilters.ResumeLayout(false);
            this.panelSearchFilters.PerformLayout();
            this.panelButtons.ResumeLayout(false);
            this.panelButtons.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ListBox listBoxSearchResults;
        private System.Windows.Forms.PictureBox pictureBoxPreview;
        private System.Windows.Forms.Label lblMetaData;
        private System.Windows.Forms.ProgressBar progressBar;
        private System.Windows.Forms.Panel panelSearchFilters;
        private System.Windows.Forms.Button btnClearFilters;
        private System.Windows.Forms.TextBox textBoxStyleName;
        private System.Windows.Forms.Label lblStyleName;
        private System.Windows.Forms.CheckBox check_Steel;
        private System.Windows.Forms.ComboBox cbTriggerEffect;
        private System.Windows.Forms.Label lblTriggerEffect;
        private System.Windows.Forms.CheckBox check_CanNineSlice;
        private System.Windows.Forms.CheckBox check_CanResize;
        private System.Windows.Forms.Label lblCurrentStyle;
        private System.Windows.Forms.CheckBox check_CurrentStyleOnly;
        private System.Windows.Forms.Label lblPieceName;
        private System.Windows.Forms.TextBox textBoxPieceName;
        private System.Windows.Forms.Label lblFilterResults;
        private System.Windows.Forms.Panel panelButtons;
        private System.Windows.Forms.Button btnClose;
        private System.Windows.Forms.Button btnAddPiece;
        private System.Windows.Forms.Button btnLoadPiece;
        private System.Windows.Forms.Label lblButtonHint;
    }
}