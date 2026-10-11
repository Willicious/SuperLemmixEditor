namespace SLXEditor
{
    partial class FormHotkeys
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormHotkeys));
            this.comboBoxChooseKey = new System.Windows.Forms.ComboBox();
            this.btnSave = new System.Windows.Forms.Button();
            this.lblAction = new System.Windows.Forms.Label();
            this.lblChooseKey = new System.Windows.Forms.Label();
            this.btnListen = new System.Windows.Forms.Button();
            this.lblAssignedKey = new System.Windows.Forms.Label();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnClearAllKeys = new System.Windows.Forms.Button();
            this.btnLoadDefault = new System.Windows.Forms.Button();
            this.listViewHotkeys = new System.Windows.Forms.ListView();
            this.EditorFunction = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.AssignedKey = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.lblActionToBeAssigned = new System.Windows.Forms.Label();
            this.btnAssignChosenKey = new System.Windows.Forms.Button();
            this.lblCurrentKey = new System.Windows.Forms.Label();
            this.lblChosenKey = new System.Windows.Forms.Label();
            this.checkModCtrl = new System.Windows.Forms.CheckBox();
            this.lblAddModifier = new System.Windows.Forms.Label();
            this.lblCurrentHotkey = new System.Windows.Forms.Label();
            this.lblChosenHotkey = new System.Windows.Forms.Label();
            this.checkModShift = new System.Windows.Forms.CheckBox();
            this.checkModAlt = new System.Windows.Forms.CheckBox();
            this.btnClose = new System.Windows.Forms.Button();
            this.lblDuplicateAction = new System.Windows.Forms.Label();
            this.lblDuplicateDetected = new System.Windows.Forms.Label();
            this.lblEditedSaved = new System.Windows.Forms.Label();
            this.lblListening = new System.Windows.Forms.Label();
            this.focusText = new System.Windows.Forms.TextBox();
            this.panelSizing = new System.Windows.Forms.Panel();
            this.btnClearKey = new System.Windows.Forms.Button();
            this.panelSizing.SuspendLayout();
            this.SuspendLayout();
            // 
            // comboBoxChooseKey
            // 
            this.comboBoxChooseKey.FormattingEnabled = true;
            this.comboBoxChooseKey.Location = new System.Drawing.Point(865, 206);
            this.comboBoxChooseKey.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.comboBoxChooseKey.Name = "comboBoxChooseKey";
            this.comboBoxChooseKey.Size = new System.Drawing.Size(235, 28);
            this.comboBoxChooseKey.TabIndex = 0;
            this.comboBoxChooseKey.SelectedIndexChanged += new System.EventHandler(this.comboBoxChooseKey_SelectedIndexChanged);
            this.comboBoxChooseKey.KeyDown += new System.Windows.Forms.KeyEventHandler(this.comboBoxChooseKey_KeyDown);
            // 
            // btnSave
            // 
            this.btnSave.Location = new System.Drawing.Point(25, 8);
            this.btnSave.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(380, 52);
            this.btnSave.TabIndex = 1;
            this.btnSave.Text = "Save Current Configuration";
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // lblAction
            // 
            this.lblAction.AutoSize = true;
            this.lblAction.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblAction.Location = new System.Drawing.Point(30, 18);
            this.lblAction.Name = "lblAction";
            this.lblAction.Size = new System.Drawing.Size(73, 25);
            this.lblAction.TabIndex = 2;
            this.lblAction.Text = "Action";
            // 
            // lblChooseKey
            // 
            this.lblChooseKey.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblChooseKey.Location = new System.Drawing.Point(718, 206);
            this.lblChooseKey.Name = "lblChooseKey";
            this.lblChooseKey.Size = new System.Drawing.Size(176, 28);
            this.lblChooseKey.TabIndex = 3;
            this.lblChooseKey.Text = "Choose Key:";
            // 
            // btnListen
            // 
            this.btnListen.Location = new System.Drawing.Point(720, 246);
            this.btnListen.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.btnListen.Name = "btnListen";
            this.btnListen.Size = new System.Drawing.Size(380, 52);
            this.btnListen.TabIndex = 4;
            this.btnListen.Text = "Listen For Input";
            this.btnListen.UseVisualStyleBackColor = true;
            this.btnListen.Click += new System.EventHandler(this.btnListen_Click);
            // 
            // lblAssignedKey
            // 
            this.lblAssignedKey.AutoSize = true;
            this.lblAssignedKey.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblAssignedKey.Location = new System.Drawing.Point(454, 18);
            this.lblAssignedKey.Name = "lblAssignedKey";
            this.lblAssignedKey.Size = new System.Drawing.Size(146, 25);
            this.lblAssignedKey.TabIndex = 5;
            this.lblAssignedKey.Text = "Assigned Key";
            // 
            // btnCancel
            // 
            this.btnCancel.Enabled = false;
            this.btnCancel.Location = new System.Drawing.Point(917, 430);
            this.btnCancel.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(183, 52);
            this.btnCancel.TabIndex = 6;
            this.btnCancel.Text = "Cancel Editing Key";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // btnClearAllKeys
            // 
            this.btnClearAllKeys.Location = new System.Drawing.Point(720, 590);
            this.btnClearAllKeys.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.btnClearAllKeys.Name = "btnClearAllKeys";
            this.btnClearAllKeys.Size = new System.Drawing.Size(183, 52);
            this.btnClearAllKeys.TabIndex = 7;
            this.btnClearAllKeys.Text = "Clear All";
            this.btnClearAllKeys.UseVisualStyleBackColor = true;
            this.btnClearAllKeys.Click += new System.EventHandler(this.btnClearAllKeys_Click);
            // 
            // btnLoadDefault
            // 
            this.btnLoadDefault.Location = new System.Drawing.Point(917, 590);
            this.btnLoadDefault.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.btnLoadDefault.Name = "btnLoadDefault";
            this.btnLoadDefault.Size = new System.Drawing.Size(183, 52);
            this.btnLoadDefault.TabIndex = 8;
            this.btnLoadDefault.Text = "Load Default";
            this.btnLoadDefault.UseVisualStyleBackColor = true;
            this.btnLoadDefault.Click += new System.EventHandler(this.btnLoadDefault_Click);
            // 
            // listViewHotkeys
            // 
            this.listViewHotkeys.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.EditorFunction,
            this.AssignedKey});
            this.listViewHotkeys.FullRowSelect = true;
            this.listViewHotkeys.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.None;
            this.listViewHotkeys.HideSelection = false;
            this.listViewHotkeys.Location = new System.Drawing.Point(25, 49);
            this.listViewHotkeys.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.listViewHotkeys.MultiSelect = false;
            this.listViewHotkeys.Name = "listViewHotkeys";
            this.listViewHotkeys.Size = new System.Drawing.Size(664, 760);
            this.listViewHotkeys.TabIndex = 10;
            this.listViewHotkeys.UseCompatibleStateImageBehavior = false;
            this.listViewHotkeys.View = System.Windows.Forms.View.Details;
            this.listViewHotkeys.SelectedIndexChanged += new System.EventHandler(this.listViewHotkeys_SelectedIndexChanged);
            this.listViewHotkeys.Click += new System.EventHandler(this.listViewHotkeys_Click);
            // 
            // EditorFunction
            // 
            this.EditorFunction.Text = "Editor Function";
            this.EditorFunction.Width = 300;
            // 
            // AssignedKey
            // 
            this.AssignedKey.Text = "Assigned Key";
            this.AssignedKey.Width = 100;
            // 
            // lblActionToBeAssigned
            // 
            this.lblActionToBeAssigned.AutoSize = true;
            this.lblActionToBeAssigned.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblActionToBeAssigned.ForeColor = System.Drawing.Color.DarkBlue;
            this.lblActionToBeAssigned.Location = new System.Drawing.Point(719, 49);
            this.lblActionToBeAssigned.Name = "lblActionToBeAssigned";
            this.lblActionToBeAssigned.Size = new System.Drawing.Size(186, 25);
            this.lblActionToBeAssigned.TabIndex = 12;
            this.lblActionToBeAssigned.Text = "(No action selected)";
            // 
            // btnAssignChosenKey
            // 
            this.btnAssignChosenKey.Enabled = false;
            this.btnAssignChosenKey.Location = new System.Drawing.Point(720, 430);
            this.btnAssignChosenKey.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.btnAssignChosenKey.Name = "btnAssignChosenKey";
            this.btnAssignChosenKey.Size = new System.Drawing.Size(183, 52);
            this.btnAssignChosenKey.TabIndex = 13;
            this.btnAssignChosenKey.Text = "Assign Chosen Key";
            this.btnAssignChosenKey.UseVisualStyleBackColor = true;
            this.btnAssignChosenKey.Click += new System.EventHandler(this.btnAssignChosenKey_Click);
            // 
            // lblCurrentKey
            // 
            this.lblCurrentKey.AutoSize = true;
            this.lblCurrentKey.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblCurrentKey.Location = new System.Drawing.Point(718, 86);
            this.lblCurrentKey.Name = "lblCurrentKey";
            this.lblCurrentKey.Size = new System.Drawing.Size(135, 25);
            this.lblCurrentKey.TabIndex = 14;
            this.lblCurrentKey.Text = "Current Key:";
            // 
            // lblChosenKey
            // 
            this.lblChosenKey.AutoSize = true;
            this.lblChosenKey.Enabled = false;
            this.lblChosenKey.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblChosenKey.Location = new System.Drawing.Point(719, 395);
            this.lblChosenKey.Name = "lblChosenKey";
            this.lblChosenKey.Size = new System.Drawing.Size(138, 25);
            this.lblChosenKey.TabIndex = 15;
            this.lblChosenKey.Text = "Chosen Key:";
            // 
            // checkModCtrl
            // 
            this.checkModCtrl.AutoSize = true;
            this.checkModCtrl.Location = new System.Drawing.Point(801, 332);
            this.checkModCtrl.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.checkModCtrl.Name = "checkModCtrl";
            this.checkModCtrl.Size = new System.Drawing.Size(59, 24);
            this.checkModCtrl.TabIndex = 16;
            this.checkModCtrl.Text = "Ctrl";
            this.checkModCtrl.UseVisualStyleBackColor = true;
            this.checkModCtrl.Click += new System.EventHandler(this.checkModifiers_Click);
            // 
            // lblAddModifier
            // 
            this.lblAddModifier.AutoSize = true;
            this.lblAddModifier.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblAddModifier.Location = new System.Drawing.Point(803, 310);
            this.lblAddModifier.Name = "lblAddModifier";
            this.lblAddModifier.Size = new System.Drawing.Size(196, 20);
            this.lblAddModifier.TabIndex = 17;
            this.lblAddModifier.Text = "Add modifier (optional):";
            // 
            // lblCurrentHotkey
            // 
            this.lblCurrentHotkey.AutoSize = true;
            this.lblCurrentHotkey.Font = new System.Drawing.Font("Microsoft Sans Serif", 14F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblCurrentHotkey.ForeColor = System.Drawing.Color.DarkBlue;
            this.lblCurrentHotkey.Location = new System.Drawing.Point(860, 80);
            this.lblCurrentHotkey.Name = "lblCurrentHotkey";
            this.lblCurrentHotkey.Size = new System.Drawing.Size(74, 32);
            this.lblCurrentHotkey.TabIndex = 18;
            this.lblCurrentHotkey.Text = "KEY";
            // 
            // lblChosenHotkey
            // 
            this.lblChosenHotkey.AutoSize = true;
            this.lblChosenHotkey.Enabled = false;
            this.lblChosenHotkey.Font = new System.Drawing.Font("Microsoft Sans Serif", 14F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblChosenHotkey.ForeColor = System.Drawing.Color.MediumSeaGreen;
            this.lblChosenHotkey.Location = new System.Drawing.Point(860, 389);
            this.lblChosenHotkey.Name = "lblChosenHotkey";
            this.lblChosenHotkey.Size = new System.Drawing.Size(74, 32);
            this.lblChosenHotkey.TabIndex = 19;
            this.lblChosenHotkey.Text = "KEY";
            // 
            // checkModShift
            // 
            this.checkModShift.AutoSize = true;
            this.checkModShift.Location = new System.Drawing.Point(875, 332);
            this.checkModShift.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.checkModShift.Name = "checkModShift";
            this.checkModShift.Size = new System.Drawing.Size(68, 24);
            this.checkModShift.TabIndex = 20;
            this.checkModShift.Text = "Shift";
            this.checkModShift.UseVisualStyleBackColor = true;
            this.checkModShift.Click += new System.EventHandler(this.checkModifiers_Click);
            // 
            // checkModAlt
            // 
            this.checkModAlt.AutoSize = true;
            this.checkModAlt.Location = new System.Drawing.Point(952, 332);
            this.checkModAlt.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.checkModAlt.Name = "checkModAlt";
            this.checkModAlt.Size = new System.Drawing.Size(54, 24);
            this.checkModAlt.TabIndex = 21;
            this.checkModAlt.Text = "Alt";
            this.checkModAlt.UseVisualStyleBackColor = true;
            this.checkModAlt.Click += new System.EventHandler(this.checkModifiers_Click);
            // 
            // btnClose
            // 
            this.btnClose.Location = new System.Drawing.Point(25, 69);
            this.btnClose.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(380, 52);
            this.btnClose.TabIndex = 22;
            this.btnClose.Text = "Close";
            this.btnClose.UseVisualStyleBackColor = true;
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // lblDuplicateAction
            // 
            this.lblDuplicateAction.AutoSize = true;
            this.lblDuplicateAction.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblDuplicateAction.ForeColor = System.Drawing.Color.Black;
            this.lblDuplicateAction.Location = new System.Drawing.Point(719, 518);
            this.lblDuplicateAction.Name = "lblDuplicateAction";
            this.lblDuplicateAction.Size = new System.Drawing.Size(154, 20);
            this.lblDuplicateAction.TabIndex = 24;
            this.lblDuplicateAction.Text = "(No duplicate action)";
            this.lblDuplicateAction.Visible = false;
            // 
            // lblDuplicateDetected
            // 
            this.lblDuplicateDetected.AutoSize = true;
            this.lblDuplicateDetected.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblDuplicateDetected.ForeColor = System.Drawing.Color.Red;
            this.lblDuplicateDetected.Location = new System.Drawing.Point(719, 495);
            this.lblDuplicateDetected.Name = "lblDuplicateDetected";
            this.lblDuplicateDetected.Size = new System.Drawing.Size(203, 20);
            this.lblDuplicateDetected.TabIndex = 23;
            this.lblDuplicateDetected.Text = "Duplicate Key Detected!";
            this.lblDuplicateDetected.Visible = false;
            // 
            // lblEditedSaved
            // 
            this.lblEditedSaved.AutoSize = true;
            this.lblEditedSaved.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblEditedSaved.ForeColor = System.Drawing.Color.DarkViolet;
            this.lblEditedSaved.Location = new System.Drawing.Point(720, 664);
            this.lblEditedSaved.Name = "lblEditedSaved";
            this.lblEditedSaved.Size = new System.Drawing.Size(250, 20);
            this.lblEditedSaved.TabIndex = 25;
            this.lblEditedSaved.Text = "Hotkey Configuration Edited...";
            this.lblEditedSaved.Visible = false;
            // 
            // lblListening
            // 
            this.lblListening.AutoEllipsis = true;
            this.lblListening.AutoSize = true;
            this.lblListening.BackColor = System.Drawing.SystemColors.ControlLight;
            this.lblListening.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblListening.ForeColor = System.Drawing.Color.DarkViolet;
            this.lblListening.Location = new System.Drawing.Point(720, 310);
            this.lblListening.Name = "lblListening";
            this.lblListening.Size = new System.Drawing.Size(326, 20);
            this.lblListening.TabIndex = 26;
            this.lblListening.Text = "Listening for keyboard or mouse input...";
            this.lblListening.Visible = false;
            // 
            // focusText
            // 
            this.focusText.Location = new System.Drawing.Point(988, 259);
            this.focusText.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.focusText.Name = "focusText";
            this.focusText.Size = new System.Drawing.Size(100, 26);
            this.focusText.TabIndex = 27;
            // 
            // panelSizing
            // 
            this.panelSizing.Controls.Add(this.btnClose);
            this.panelSizing.Controls.Add(this.btnSave);
            this.panelSizing.Location = new System.Drawing.Point(695, 688);
            this.panelSizing.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.panelSizing.Name = "panelSizing";
            this.panelSizing.Size = new System.Drawing.Size(434, 138);
            this.panelSizing.TabIndex = 29;
            // 
            // btnClearKey
            // 
            this.btnClearKey.Enabled = false;
            this.btnClearKey.Location = new System.Drawing.Point(720, 118);
            this.btnClearKey.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.btnClearKey.Name = "btnClearKey";
            this.btnClearKey.Size = new System.Drawing.Size(380, 52);
            this.btnClearKey.TabIndex = 30;
            this.btnClearKey.Text = "Clear Current Key";
            this.btnClearKey.UseVisualStyleBackColor = true;
            this.btnClearKey.Click += new System.EventHandler(this.btnClearKey_Click);
            // 
            // FormHotkeys
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.BackColor = System.Drawing.SystemColors.ControlLight;
            this.ClientSize = new System.Drawing.Size(1128, 821);
            this.Controls.Add(this.btnClearKey);
            this.Controls.Add(this.lblEditedSaved);
            this.Controls.Add(this.lblDuplicateDetected);
            this.Controls.Add(this.lblDuplicateAction);
            this.Controls.Add(this.comboBoxChooseKey);
            this.Controls.Add(this.checkModAlt);
            this.Controls.Add(this.checkModShift);
            this.Controls.Add(this.lblChosenHotkey);
            this.Controls.Add(this.lblCurrentHotkey);
            this.Controls.Add(this.lblAddModifier);
            this.Controls.Add(this.checkModCtrl);
            this.Controls.Add(this.lblChosenKey);
            this.Controls.Add(this.lblCurrentKey);
            this.Controls.Add(this.btnAssignChosenKey);
            this.Controls.Add(this.lblActionToBeAssigned);
            this.Controls.Add(this.listViewHotkeys);
            this.Controls.Add(this.btnLoadDefault);
            this.Controls.Add(this.btnClearAllKeys);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.lblAssignedKey);
            this.Controls.Add(this.btnListen);
            this.Controls.Add(this.lblChooseKey);
            this.Controls.Add(this.lblAction);
            this.Controls.Add(this.lblListening);
            this.Controls.Add(this.focusText);
            this.Controls.Add(this.panelSizing);
            this.DoubleBuffered = true;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FormHotkeys";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "SLX Editor - Hotkey Configuration";
            this.Load += new System.EventHandler(this.FormHotkeys_Load);
            this.Shown += new System.EventHandler(this.FormHotkeys_Shown);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.FormHotkeys_KeyDown);
            this.panelSizing.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ComboBox comboBoxChooseKey;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.Label lblAction;
        private System.Windows.Forms.Label lblChooseKey;
        private System.Windows.Forms.Button btnListen;
        private System.Windows.Forms.Label lblAssignedKey;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnClearAllKeys;
        private System.Windows.Forms.Button btnLoadDefault;
        private System.Windows.Forms.ListView listViewHotkeys;
        private System.Windows.Forms.ColumnHeader EditorFunction;
        private System.Windows.Forms.ColumnHeader AssignedKey;
        private System.Windows.Forms.Label lblActionToBeAssigned;
        private System.Windows.Forms.Button btnAssignChosenKey;
        private System.Windows.Forms.Label lblCurrentKey;
        private System.Windows.Forms.Label lblChosenKey;
        private System.Windows.Forms.CheckBox checkModCtrl;
        private System.Windows.Forms.Label lblAddModifier;
        private System.Windows.Forms.Label lblCurrentHotkey;
        private System.Windows.Forms.Label lblChosenHotkey;
        private System.Windows.Forms.CheckBox checkModShift;
        private System.Windows.Forms.CheckBox checkModAlt;
        private System.Windows.Forms.Button btnClose;
        private System.Windows.Forms.Label lblDuplicateAction;
        private System.Windows.Forms.Label lblDuplicateDetected;
        private System.Windows.Forms.Label lblEditedSaved;
        private System.Windows.Forms.Label lblListening;
        private System.Windows.Forms.TextBox focusText;
        private System.Windows.Forms.Panel panelSizing;
        private System.Windows.Forms.Button btnClearKey;
    }
}