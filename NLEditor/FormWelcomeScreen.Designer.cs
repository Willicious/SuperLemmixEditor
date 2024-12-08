using System.Windows.Forms;

namespace NLEditor
{
    partial class FormWelcomeScreen
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
            this.lblWhatsNew = new System.Windows.Forms.Label();
            this.pictureFloater = new System.Windows.Forms.PictureBox();
            this.pictureClimber = new System.Windows.Forms.PictureBox();
            this.check_ShowWelcomeScreen = new System.Windows.Forms.CheckBox();
            this.lblPreviousUpdates = new System.Windows.Forms.Label();
            this.richTextBox_PreviousUpdates = new System.Windows.Forms.RichTextBox();
            this.richTextBox_WhatsNew = new System.Windows.Forms.RichTextBox();
            ((System.ComponentModel.ISupportInitialize)(this.pictureFloater)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureClimber)).BeginInit();
            this.SuspendLayout();
            // 
            // lblWhatsNew
            // 
            this.lblWhatsNew.AutoSize = true;
            this.lblWhatsNew.Font = new System.Drawing.Font("Hobo Std", 18F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblWhatsNew.Location = new System.Drawing.Point(539, 28);
            this.lblWhatsNew.Name = "lblWhatsNew";
            this.lblWhatsNew.Size = new System.Drawing.Size(222, 51);
            this.lblWhatsNew.TabIndex = 0;
            this.lblWhatsNew.Text = "What\'s New";
            // 
            // pictureFloater
            // 
            this.pictureFloater.Image = global::NLEditor.Properties.Resources.IntroFloater;
            this.pictureFloater.Location = new System.Drawing.Point(22, 28);
            this.pictureFloater.Name = "pictureFloater";
            this.pictureFloater.Size = new System.Drawing.Size(156, 220);
            this.pictureFloater.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureFloater.TabIndex = 2;
            this.pictureFloater.TabStop = false;
            // 
            // pictureClimber
            // 
            this.pictureClimber.Image = global::NLEditor.Properties.Resources.IntroClimber;
            this.pictureClimber.Location = new System.Drawing.Point(1119, 69);
            this.pictureClimber.Name = "pictureClimber";
            this.pictureClimber.Size = new System.Drawing.Size(89, 179);
            this.pictureClimber.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureClimber.TabIndex = 3;
            this.pictureClimber.TabStop = false;
            // 
            // check_ShowWelcomeScreen
            // 
            this.check_ShowWelcomeScreen.AutoSize = true;
            this.check_ShowWelcomeScreen.Checked = true;
            this.check_ShowWelcomeScreen.CheckState = System.Windows.Forms.CheckState.Checked;
            this.check_ShowWelcomeScreen.Location = new System.Drawing.Point(509, 579);
            this.check_ShowWelcomeScreen.Name = "check_ShowWelcomeScreen";
            this.check_ShowWelcomeScreen.Size = new System.Drawing.Size(272, 24);
            this.check_ShowWelcomeScreen.TabIndex = 4;
            this.check_ShowWelcomeScreen.Text = "Show Welcome Screen at startup";
            this.check_ShowWelcomeScreen.UseVisualStyleBackColor = true;
            this.check_ShowWelcomeScreen.CheckedChanged += new System.EventHandler(this.Check_ShowWelcomeScreen_CheckedChanged);
            // 
            // lblPreviousUpdates
            // 
            this.lblPreviousUpdates.AutoSize = true;
            this.lblPreviousUpdates.Font = new System.Drawing.Font("Hobo Std", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblPreviousUpdates.Location = new System.Drawing.Point(530, 274);
            this.lblPreviousUpdates.Name = "lblPreviousUpdates";
            this.lblPreviousUpdates.Size = new System.Drawing.Size(216, 34);
            this.lblPreviousUpdates.TabIndex = 7;
            this.lblPreviousUpdates.Text = "Previous Updates";
            // 
            // richTextBox_PreviousUpdates
            // 
            this.richTextBox_PreviousUpdates.Location = new System.Drawing.Point(195, 316);
            this.richTextBox_PreviousUpdates.Name = "richTextBox_PreviousUpdates";
            this.richTextBox_PreviousUpdates.Size = new System.Drawing.Size(909, 241);
            this.richTextBox_PreviousUpdates.TabIndex = 8;
            this.richTextBox_PreviousUpdates.Text = "Previous updates...";
            // 
            // richTextBox_WhatsNew
            // 
            this.richTextBox_WhatsNew.Location = new System.Drawing.Point(195, 85);
            this.richTextBox_WhatsNew.Name = "richTextBox_WhatsNew";
            this.richTextBox_WhatsNew.Size = new System.Drawing.Size(909, 163);
            this.richTextBox_WhatsNew.TabIndex = 9;
            this.richTextBox_WhatsNew.Text = "What\'s new...";
            // 
            // FormWelcomeScreen
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1230, 615);
            this.Controls.Add(this.richTextBox_WhatsNew);
            this.Controls.Add(this.richTextBox_PreviousUpdates);
            this.Controls.Add(this.lblPreviousUpdates);
            this.Controls.Add(this.check_ShowWelcomeScreen);
            this.Controls.Add(this.pictureClimber);
            this.Controls.Add(this.pictureFloater);
            this.Controls.Add(this.lblWhatsNew);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Name = "FormWelcomeScreen";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Welcome To SuperLemmix Editor";
            this.TopMost = true;
            ((System.ComponentModel.ISupportInitialize)(this.pictureFloater)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureClimber)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblWhatsNew;
        private PictureBox pictureFloater;
        private PictureBox pictureClimber;
        private CheckBox check_ShowWelcomeScreen;
        private Label lblPreviousUpdates;
        private RichTextBox richTextBox_PreviousUpdates;
        private RichTextBox richTextBox_WhatsNew;
    }
}