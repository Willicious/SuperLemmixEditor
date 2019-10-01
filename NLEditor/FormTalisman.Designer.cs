﻿namespace NLEditor
{
    partial class FormTalisman
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormTalisman));
            this.lblTalismanTitle = new System.Windows.Forms.Label();
            this.cmbRequirementTypes = new System.Windows.Forms.ComboBox();
            this.numReqValue1 = new System.Windows.Forms.NumericUpDown();
            this.numReqValue2 = new System.Windows.Forms.NumericUpDown();
            this.butRequirementDelete = new System.Windows.Forms.Button();
            this.butRequirementAdd = new System.Windows.Forms.Button();
            this.lblTalTitle = new System.Windows.Forms.Label();
            this.txtTalismanTitle = new System.Windows.Forms.TextBox();
            this.radBronze = new System.Windows.Forms.RadioButton();
            this.radSilver = new System.Windows.Forms.RadioButton();
            this.radGold = new System.Windows.Forms.RadioButton();
            this.butTalismanSave = new System.Windows.Forms.Button();
            this.butTalismanCancel = new System.Windows.Forms.Button();
            this.listRequirements = new System.Windows.Forms.ListBox();
            this.cmbRequirementSkill = new System.Windows.Forms.ComboBox();
            ((System.ComponentModel.ISupportInitialize)(this.numReqValue1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numReqValue2)).BeginInit();
            this.SuspendLayout();
            // 
            // lblTalismanTitle
            // 
            this.lblTalismanTitle.Location = new System.Drawing.Point(12, 9);
            this.lblTalismanTitle.Name = "lblTalismanTitle";
            this.lblTalismanTitle.Size = new System.Drawing.Size(268, 21);
            this.lblTalismanTitle.TabIndex = 0;
            this.lblTalismanTitle.Text = "Talisman for ...";
            this.lblTalismanTitle.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // cmbRequirementTypes
            // 
            this.cmbRequirementTypes.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbRequirementTypes.FormattingEnabled = true;
            this.cmbRequirementTypes.Location = new System.Drawing.Point(14, 228);
            this.cmbRequirementTypes.Name = "cmbRequirementTypes";
            this.cmbRequirementTypes.Size = new System.Drawing.Size(148, 21);
            this.cmbRequirementTypes.TabIndex = 14;
            this.cmbRequirementTypes.SelectedIndexChanged += new System.EventHandler(this.cmbRequirementTypes_SelectedIndexChanged);
            // 
            // numReqValue1
            // 
            this.numReqValue1.Location = new System.Drawing.Point(168, 228);
            this.numReqValue1.Name = "numReqValue1";
            this.numReqValue1.Size = new System.Drawing.Size(52, 20);
            this.numReqValue1.TabIndex = 15;
            // 
            // numReqValue2
            // 
            this.numReqValue2.Location = new System.Drawing.Point(228, 228);
            this.numReqValue2.Name = "numReqValue2";
            this.numReqValue2.Size = new System.Drawing.Size(52, 20);
            this.numReqValue2.TabIndex = 16;
            this.numReqValue2.Visible = false;
            // 
            // butRequirementDelete
            // 
            this.butRequirementDelete.Location = new System.Drawing.Point(150, 185);
            this.butRequirementDelete.Name = "butRequirementDelete";
            this.butRequirementDelete.Size = new System.Drawing.Size(128, 26);
            this.butRequirementDelete.TabIndex = 18;
            this.butRequirementDelete.Text = "Delete Requirement";
            this.butRequirementDelete.UseVisualStyleBackColor = true;
            this.butRequirementDelete.Click += new System.EventHandler(this.butRequirementDelete_Click);
            // 
            // butRequirementAdd
            // 
            this.butRequirementAdd.Location = new System.Drawing.Point(150, 254);
            this.butRequirementAdd.Name = "butRequirementAdd";
            this.butRequirementAdd.Size = new System.Drawing.Size(128, 26);
            this.butRequirementAdd.TabIndex = 19;
            this.butRequirementAdd.Text = "Add New Requirement";
            this.butRequirementAdd.UseVisualStyleBackColor = true;
            this.butRequirementAdd.Click += new System.EventHandler(this.butRequirementAdd_Click);
            // 
            // lblTalTitle
            // 
            this.lblTalTitle.AutoSize = true;
            this.lblTalTitle.Location = new System.Drawing.Point(14, 33);
            this.lblTalTitle.Name = "lblTalTitle";
            this.lblTalTitle.Size = new System.Drawing.Size(27, 13);
            this.lblTalTitle.TabIndex = 7;
            this.lblTalTitle.Text = "Title";
            // 
            // txtTalismanTitle
            // 
            this.txtTalismanTitle.Location = new System.Drawing.Point(47, 30);
            this.txtTalismanTitle.MaxLength = 38;
            this.txtTalismanTitle.Name = "txtTalismanTitle";
            this.txtTalismanTitle.Size = new System.Drawing.Size(233, 20);
            this.txtTalismanTitle.TabIndex = 8;
            // 
            // radBronze
            // 
            this.radBronze.AutoSize = true;
            this.radBronze.Location = new System.Drawing.Point(17, 56);
            this.radBronze.Name = "radBronze";
            this.radBronze.Size = new System.Drawing.Size(58, 17);
            this.radBronze.TabIndex = 9;
            this.radBronze.TabStop = true;
            this.radBronze.Text = "Bronze";
            this.radBronze.UseVisualStyleBackColor = true;
            // 
            // radSilver
            // 
            this.radSilver.AutoSize = true;
            this.radSilver.Location = new System.Drawing.Point(95, 56);
            this.radSilver.Name = "radSilver";
            this.radSilver.Size = new System.Drawing.Size(51, 17);
            this.radSilver.TabIndex = 10;
            this.radSilver.TabStop = true;
            this.radSilver.Text = "Silver";
            this.radSilver.UseVisualStyleBackColor = true;
            // 
            // radGold
            // 
            this.radGold.AutoSize = true;
            this.radGold.Location = new System.Drawing.Point(168, 56);
            this.radGold.Name = "radGold";
            this.radGold.Size = new System.Drawing.Size(47, 17);
            this.radGold.TabIndex = 11;
            this.radGold.TabStop = true;
            this.radGold.Text = "Gold";
            this.radGold.UseVisualStyleBackColor = true;
            // 
            // butTalismanSave
            // 
            this.butTalismanSave.Location = new System.Drawing.Point(12, 286);
            this.butTalismanSave.Name = "butTalismanSave";
            this.butTalismanSave.Size = new System.Drawing.Size(134, 24);
            this.butTalismanSave.TabIndex = 20;
            this.butTalismanSave.Text = "Save Talisman";
            this.butTalismanSave.UseVisualStyleBackColor = true;
            this.butTalismanSave.Click += new System.EventHandler(this.butTalismanSave_Click);
            // 
            // butTalismanCancel
            // 
            this.butTalismanCancel.Location = new System.Drawing.Point(150, 286);
            this.butTalismanCancel.Name = "butTalismanCancel";
            this.butTalismanCancel.Size = new System.Drawing.Size(128, 24);
            this.butTalismanCancel.TabIndex = 21;
            this.butTalismanCancel.Text = "Cancel";
            this.butTalismanCancel.UseVisualStyleBackColor = true;
            this.butTalismanCancel.Click += new System.EventHandler(this.butTalismanCancel_Click);
            // 
            // listRequirements
            // 
            this.listRequirements.FormattingEnabled = true;
            this.listRequirements.Items.AddRange(new object[] {
            "No requirement..."});
            this.listRequirements.Location = new System.Drawing.Point(15, 84);
            this.listRequirements.Name = "listRequirements";
            this.listRequirements.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
            this.listRequirements.Size = new System.Drawing.Size(263, 95);
            this.listRequirements.TabIndex = 12;
            // 
            // cmbRequirementSkill
            // 
            this.cmbRequirementSkill.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbRequirementSkill.FormattingEnabled = true;
            this.cmbRequirementSkill.Location = new System.Drawing.Point(168, 227);
            this.cmbRequirementSkill.Name = "cmbRequirementSkill";
            this.cmbRequirementSkill.Size = new System.Drawing.Size(110, 21);
            this.cmbRequirementSkill.TabIndex = 22;
            this.cmbRequirementSkill.Visible = false;
            // 
            // FormTalisman
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(292, 322);
            this.Controls.Add(this.cmbRequirementSkill);
            this.Controls.Add(this.listRequirements);
            this.Controls.Add(this.butTalismanCancel);
            this.Controls.Add(this.butTalismanSave);
            this.Controls.Add(this.radGold);
            this.Controls.Add(this.radSilver);
            this.Controls.Add(this.radBronze);
            this.Controls.Add(this.txtTalismanTitle);
            this.Controls.Add(this.lblTalTitle);
            this.Controls.Add(this.butRequirementAdd);
            this.Controls.Add(this.butRequirementDelete);
            this.Controls.Add(this.numReqValue2);
            this.Controls.Add(this.numReqValue1);
            this.Controls.Add(this.cmbRequirementTypes);
            this.Controls.Add(this.lblTalismanTitle);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FormTalisman";
            this.ShowInTaskbar = false;
            this.Text = "Talisman Creation";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FormTalisman_FormClosing);
            this.Leave += new System.EventHandler(this.FormTalisman_Leave);
            ((System.ComponentModel.ISupportInitialize)(this.numReqValue1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numReqValue2)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblTalismanTitle;
        private System.Windows.Forms.ComboBox cmbRequirementTypes;
        private System.Windows.Forms.NumericUpDown numReqValue1;
        private System.Windows.Forms.NumericUpDown numReqValue2;
        private System.Windows.Forms.Button butRequirementDelete;
        private System.Windows.Forms.Button butRequirementAdd;
        private System.Windows.Forms.Label lblTalTitle;
        private System.Windows.Forms.TextBox txtTalismanTitle;
        private System.Windows.Forms.RadioButton radBronze;
        private System.Windows.Forms.RadioButton radSilver;
        private System.Windows.Forms.RadioButton radGold;
        private System.Windows.Forms.Button butTalismanSave;
        private System.Windows.Forms.Button butTalismanCancel;
        private System.Windows.Forms.ListBox listRequirements;
        private System.Windows.Forms.ComboBox cmbRequirementSkill;
    }
}