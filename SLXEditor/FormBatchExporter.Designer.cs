namespace SLXEditor
{
    partial class FormBatchExporter
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormBatchExporter));
            this.listViewLevels = new System.Windows.Forms.ListView();
            this.columnLevel = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnOutputStyle = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.btnAddLevels = new System.Windows.Forms.Button();
            this.lblExportTo = new System.Windows.Forms.Label();
            this.rbINI = new System.Windows.Forms.RadioButton();
            this.rbNXLV = new System.Windows.Forms.RadioButton();
            this.rbSXLV = new System.Windows.Forms.RadioButton();
            this.rbRLV = new System.Windows.Forms.RadioButton();
            this.btnExport = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.comboStyles = new System.Windows.Forms.ComboBox();
            this.lblSelectOutputStyle = new System.Windows.Forms.Label();
            this.panelSizer = new System.Windows.Forms.Panel();
            this.btnRemoveLevels = new System.Windows.Forms.Button();
            this.btnClearList = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // listViewLevels
            // 
            this.listViewLevels.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnLevel,
            this.columnOutputStyle});
            this.listViewLevels.HideSelection = false;
            this.listViewLevels.Location = new System.Drawing.Point(12, 12);
            this.listViewLevels.Name = "listViewLevels";
            this.listViewLevels.Size = new System.Drawing.Size(656, 786);
            this.listViewLevels.TabIndex = 0;
            this.listViewLevels.UseCompatibleStateImageBehavior = false;
            this.listViewLevels.View = System.Windows.Forms.View.Details;
            this.listViewLevels.SelectedIndexChanged += new System.EventHandler(this.listViewLevels_SelectedIndexChanged);
            // 
            // columnLevel
            // 
            this.columnLevel.Text = "Level";
            this.columnLevel.Width = 286;
            // 
            // columnOutputStyle
            // 
            this.columnOutputStyle.Text = "Output Style";
            this.columnOutputStyle.Width = 134;
            // 
            // btnAddLevels
            // 
            this.btnAddLevels.Location = new System.Drawing.Point(695, 12);
            this.btnAddLevels.Name = "btnAddLevels";
            this.btnAddLevels.Size = new System.Drawing.Size(416, 65);
            this.btnAddLevels.TabIndex = 1;
            this.btnAddLevels.Text = "Add Levels";
            this.btnAddLevels.UseVisualStyleBackColor = true;
            this.btnAddLevels.Click += new System.EventHandler(this.btnAddLevels_Click);
            // 
            // lblExportTo
            // 
            this.lblExportTo.AutoSize = true;
            this.lblExportTo.Location = new System.Drawing.Point(691, 194);
            this.lblExportTo.Name = "lblExportTo";
            this.lblExportTo.Size = new System.Drawing.Size(77, 20);
            this.lblExportTo.TabIndex = 2;
            this.lblExportTo.Text = "Export To";
            // 
            // rbINI
            // 
            this.rbINI.AutoSize = true;
            this.rbINI.Location = new System.Drawing.Point(801, 222);
            this.rbINI.Name = "rbINI";
            this.rbINI.Size = new System.Drawing.Size(126, 24);
            this.rbINI.TabIndex = 3;
            this.rbINI.TabStop = true;
            this.rbINI.Text = "Lemmini (.ini)";
            this.rbINI.UseVisualStyleBackColor = true;
            this.rbINI.CheckedChanged += new System.EventHandler(this.radioFormatCheckedChanged);
            // 
            // rbNXLV
            // 
            this.rbNXLV.AutoSize = true;
            this.rbNXLV.Location = new System.Drawing.Point(801, 282);
            this.rbNXLV.Name = "rbNXLV";
            this.rbNXLV.Size = new System.Drawing.Size(161, 24);
            this.rbNXLV.TabIndex = 4;
            this.rbNXLV.TabStop = true;
            this.rbNXLV.Text = "NeoLemmix (.nxlv)";
            this.rbNXLV.UseVisualStyleBackColor = true;
            this.rbNXLV.CheckedChanged += new System.EventHandler(this.radioFormatCheckedChanged);
            // 
            // rbSXLV
            // 
            this.rbSXLV.AutoSize = true;
            this.rbSXLV.Location = new System.Drawing.Point(801, 252);
            this.rbSXLV.Name = "rbSXLV";
            this.rbSXLV.Size = new System.Drawing.Size(174, 24);
            this.rbSXLV.TabIndex = 5;
            this.rbSXLV.TabStop = true;
            this.rbSXLV.Text = "SuperLemmix (.sxlv)";
            this.rbSXLV.UseVisualStyleBackColor = true;
            this.rbSXLV.CheckedChanged += new System.EventHandler(this.radioFormatCheckedChanged);
            // 
            // rbRLV
            // 
            this.rbRLV.AutoSize = true;
            this.rbRLV.Location = new System.Drawing.Point(801, 192);
            this.rbRLV.Name = "rbRLV";
            this.rbRLV.Size = new System.Drawing.Size(166, 24);
            this.rbRLV.TabIndex = 6;
            this.rbRLV.TabStop = true;
            this.rbRLV.Text = "RetroLemmini (.rlv)";
            this.rbRLV.UseVisualStyleBackColor = true;
            this.rbRLV.CheckedChanged += new System.EventHandler(this.radioFormatCheckedChanged);
            // 
            // btnExport
            // 
            this.btnExport.Location = new System.Drawing.Point(695, 652);
            this.btnExport.Name = "btnExport";
            this.btnExport.Size = new System.Drawing.Size(416, 65);
            this.btnExport.TabIndex = 8;
            this.btnExport.Text = "Export Levels";
            this.btnExport.UseVisualStyleBackColor = true;
            this.btnExport.Click += new System.EventHandler(this.btnExport_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(695, 733);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(416, 65);
            this.btnCancel.TabIndex = 9;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // comboStyles
            // 
            this.comboStyles.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboStyles.FormattingEnabled = true;
            this.comboStyles.ItemHeight = 20;
            this.comboStyles.Items.AddRange(new object[] {
            "(Select a Style)"});
            this.comboStyles.Location = new System.Drawing.Point(856, 350);
            this.comboStyles.Name = "comboStyles";
            this.comboStyles.Size = new System.Drawing.Size(255, 28);
            this.comboStyles.TabIndex = 11;
            this.comboStyles.SelectedIndexChanged += new System.EventHandler(this.comboStyles_SelectedIndexChanged);
            // 
            // lblSelectOutputStyle
            // 
            this.lblSelectOutputStyle.AutoSize = true;
            this.lblSelectOutputStyle.Location = new System.Drawing.Point(691, 353);
            this.lblSelectOutputStyle.Name = "lblSelectOutputStyle";
            this.lblSelectOutputStyle.Size = new System.Drawing.Size(146, 20);
            this.lblSelectOutputStyle.TabIndex = 10;
            this.lblSelectOutputStyle.Text = "Select Output Style";
            // 
            // panelSizer
            // 
            this.panelSizer.Location = new System.Drawing.Point(12, 787);
            this.panelSizer.Name = "panelSizer";
            this.panelSizer.Size = new System.Drawing.Size(1099, 30);
            this.panelSizer.TabIndex = 12;
            // 
            // btnRemoveLevels
            // 
            this.btnRemoveLevels.Location = new System.Drawing.Point(695, 93);
            this.btnRemoveLevels.Name = "btnRemoveLevels";
            this.btnRemoveLevels.Size = new System.Drawing.Size(199, 65);
            this.btnRemoveLevels.TabIndex = 13;
            this.btnRemoveLevels.Text = "Remove Levels";
            this.btnRemoveLevels.UseVisualStyleBackColor = true;
            this.btnRemoveLevels.Click += new System.EventHandler(this.btnRemoveLevels_Click);
            // 
            // btnClearList
            // 
            this.btnClearList.Location = new System.Drawing.Point(912, 93);
            this.btnClearList.Name = "btnClearList";
            this.btnClearList.Size = new System.Drawing.Size(199, 65);
            this.btnClearList.TabIndex = 14;
            this.btnClearList.Text = "Clear List";
            this.btnClearList.UseVisualStyleBackColor = true;
            this.btnClearList.Click += new System.EventHandler(this.btnClearList_Click);
            // 
            // FormBatchExporter
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.ClientSize = new System.Drawing.Size(1123, 810);
            this.Controls.Add(this.btnClearList);
            this.Controls.Add(this.btnRemoveLevels);
            this.Controls.Add(this.comboStyles);
            this.Controls.Add(this.lblSelectOutputStyle);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnExport);
            this.Controls.Add(this.rbRLV);
            this.Controls.Add(this.rbSXLV);
            this.Controls.Add(this.rbNXLV);
            this.Controls.Add(this.rbINI);
            this.Controls.Add(this.lblExportTo);
            this.Controls.Add(this.btnAddLevels);
            this.Controls.Add(this.listViewLevels);
            this.Controls.Add(this.panelSizer);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "FormBatchExporter";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Batch Exporter";
            this.Load += new System.EventHandler(this.FormBatchExporter_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ListView listViewLevels;
        private System.Windows.Forms.Button btnAddLevels;
        private System.Windows.Forms.Label lblExportTo;
        private System.Windows.Forms.RadioButton rbINI;
        private System.Windows.Forms.RadioButton rbNXLV;
        private System.Windows.Forms.RadioButton rbSXLV;
        private System.Windows.Forms.RadioButton rbRLV;
        private System.Windows.Forms.Button btnExport;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.ColumnHeader columnLevel;
        private System.Windows.Forms.ColumnHeader columnOutputStyle;
        private System.Windows.Forms.ComboBox comboStyles;
        private System.Windows.Forms.Label lblSelectOutputStyle;
        private System.Windows.Forms.Panel panelSizer;
        private System.Windows.Forms.Button btnRemoveLevels;
        private System.Windows.Forms.Button btnClearList;
    }
}