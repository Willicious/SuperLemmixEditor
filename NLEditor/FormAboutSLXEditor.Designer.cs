using System.Windows.Forms;

namespace NLEditor
{
    partial class FormAboutSLXEditor
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormAboutSLXEditor));
            this.lblWhatsNew = new System.Windows.Forms.Label();
            this.pictureFloater = new System.Windows.Forms.PictureBox();
            this.pictureClimber = new System.Windows.Forms.PictureBox();
            this.check_ShowThisWindow = new System.Windows.Forms.CheckBox();
            this.lblPreviousUpdates = new System.Windows.Forms.Label();
            this.richTextBox_PreviousUpdates = new System.Windows.Forms.RichTextBox();
            this.richTextBox_WhatsNew = new System.Windows.Forms.RichTextBox();
            this.lblSuperLemmixEditor = new System.Windows.Forms.Label();
            this.lblAuthor = new System.Windows.Forms.Label();
            this.lblThanksTo = new System.Windows.Forms.Label();
            this.lblDMA = new System.Windows.Forms.Label();
            this.lblLFCommunity = new System.Windows.Forms.Label();
            this.linkLF = new System.Windows.Forms.LinkLabel();
            this.lblBasedOn = new System.Windows.Forms.Label();
            this.picturePadding = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.pictureFloater)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureClimber)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.picturePadding)).BeginInit();
            this.SuspendLayout();
            // 
            // lblWhatsNew
            // 
            this.lblWhatsNew.AutoSize = true;
            this.lblWhatsNew.Font = new System.Drawing.Font("Hobo Std", 18F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblWhatsNew.Location = new System.Drawing.Point(555, 16);
            this.lblWhatsNew.Name = "lblWhatsNew";
            this.lblWhatsNew.Size = new System.Drawing.Size(222, 51);
            this.lblWhatsNew.TabIndex = 0;
            this.lblWhatsNew.Text = "What\'s New";
            // 
            // pictureFloater
            // 
            this.pictureFloater.Image = global::NLEditor.Properties.Resources.IntroFloater;
            this.pictureFloater.Location = new System.Drawing.Point(31, 16);
            this.pictureFloater.Name = "pictureFloater";
            this.pictureFloater.Size = new System.Drawing.Size(156, 220);
            this.pictureFloater.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureFloater.TabIndex = 2;
            this.pictureFloater.TabStop = false;
            // 
            // pictureClimber
            // 
            this.pictureClimber.BackColor = System.Drawing.SystemColors.ControlDark;
            this.pictureClimber.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.pictureClimber.Image = global::NLEditor.Properties.Resources.IntroClimber;
            this.pictureClimber.InitialImage = null;
            this.pictureClimber.Location = new System.Drawing.Point(1170, 82);
            this.pictureClimber.Name = "pictureClimber";
            this.pictureClimber.Size = new System.Drawing.Size(96, 179);
            this.pictureClimber.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureClimber.TabIndex = 3;
            this.pictureClimber.TabStop = false;
            // 
            // check_ShowThisWindow
            // 
            this.check_ShowThisWindow.AutoSize = true;
            this.check_ShowThisWindow.Checked = true;
            this.check_ShowThisWindow.CheckState = System.Windows.Forms.CheckState.Checked;
            this.check_ShowThisWindow.ForeColor = System.Drawing.SystemColors.WindowText;
            this.check_ShowThisWindow.Location = new System.Drawing.Point(548, 774);
            this.check_ShowThisWindow.Name = "check_ShowThisWindow";
            this.check_ShowThisWindow.Size = new System.Drawing.Size(232, 24);
            this.check_ShowThisWindow.TabIndex = 4;
            this.check_ShowThisWindow.Text = "Show this window at startup";
            this.check_ShowThisWindow.UseVisualStyleBackColor = true;
            this.check_ShowThisWindow.CheckedChanged += new System.EventHandler(this.Check_ShowThisWindow_CheckedChanged);
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
            this.richTextBox_PreviousUpdates.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F);
            this.richTextBox_PreviousUpdates.Location = new System.Drawing.Point(164, 316);
            this.richTextBox_PreviousUpdates.Name = "richTextBox_PreviousUpdates";
            this.richTextBox_PreviousUpdates.Size = new System.Drawing.Size(1008, 265);
            this.richTextBox_PreviousUpdates.TabIndex = 8;
            this.richTextBox_PreviousUpdates.Text = "Previous updates...";
            // 
            // richTextBox_WhatsNew
            // 
            this.richTextBox_WhatsNew.BackColor = System.Drawing.SystemColors.ControlDarkDark;
            this.richTextBox_WhatsNew.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.richTextBox_WhatsNew.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.richTextBox_WhatsNew.Location = new System.Drawing.Point(164, 82);
            this.richTextBox_WhatsNew.Name = "richTextBox_WhatsNew";
            this.richTextBox_WhatsNew.Size = new System.Drawing.Size(1008, 499);
            this.richTextBox_WhatsNew.TabIndex = 9;
            this.richTextBox_WhatsNew.Text = "What\'s new...";
            // 
            // lblSuperLemmixEditor
            // 
            this.lblSuperLemmixEditor.AutoSize = true;
            this.lblSuperLemmixEditor.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblSuperLemmixEditor.Location = new System.Drawing.Point(521, 585);
            this.lblSuperLemmixEditor.Name = "lblSuperLemmixEditor";
            this.lblSuperLemmixEditor.Size = new System.Drawing.Size(302, 25);
            this.lblSuperLemmixEditor.TabIndex = 10;
            this.lblSuperLemmixEditor.Text = "SuperLemmix Editor (Version)";
            // 
            // lblAuthor
            // 
            this.lblAuthor.AutoSize = true;
            this.lblAuthor.Location = new System.Drawing.Point(569, 612);
            this.lblAuthor.Name = "lblAuthor";
            this.lblAuthor.Size = new System.Drawing.Size(200, 20);
            this.lblAuthor.TabIndex = 11;
            this.lblAuthor.Text = "By William James (WillLem)";
            // 
            // lblThanksTo
            // 
            this.lblThanksTo.AutoSize = true;
            this.lblThanksTo.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblThanksTo.Location = new System.Drawing.Point(620, 668);
            this.lblThanksTo.Name = "lblThanksTo";
            this.lblThanksTo.Size = new System.Drawing.Size(97, 20);
            this.lblThanksTo.TabIndex = 13;
            this.lblThanksTo.Text = "Thanks To:";
            // 
            // lblDMA
            // 
            this.lblDMA.AutoSize = true;
            this.lblDMA.Location = new System.Drawing.Point(491, 688);
            this.lblDMA.Name = "lblDMA";
            this.lblDMA.Size = new System.Drawing.Size(332, 20);
            this.lblDMA.TabIndex = 14;
            this.lblDMA.Text = "DMA Design for the original Lemmings games";
            // 
            // lblLFCommunity
            // 
            this.lblLFCommunity.AutoSize = true;
            this.lblLFCommunity.Location = new System.Drawing.Point(517, 708);
            this.lblLFCommunity.Name = "lblLFCommunity";
            this.lblLFCommunity.Size = new System.Drawing.Size(269, 20);
            this.lblLFCommunity.TabIndex = 15;
            this.lblLFCommunity.Text = "The Lemmings Forums community at";
            // 
            // linkLF
            // 
            this.linkLF.AutoSize = true;
            this.linkLF.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.linkLF.Location = new System.Drawing.Point(543, 728);
            this.linkLF.Name = "linkLF";
            this.linkLF.Size = new System.Drawing.Size(233, 25);
            this.linkLF.TabIndex = 16;
            this.linkLF.TabStop = true;
            this.linkLF.Text = "www.lemmingsforums.net";
            this.linkLF.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLF_LinkClicked);
            // 
            // lblBasedOn
            // 
            this.lblBasedOn.AutoSize = true;
            this.lblBasedOn.Location = new System.Drawing.Point(349, 634);
            this.lblBasedOn.Name = "lblBasedOn";
            this.lblBasedOn.Size = new System.Drawing.Size(614, 20);
            this.lblBasedOn.TabIndex = 18;
            this.lblBasedOn.Text = "Based on the NeoLemmix Editor by Stephan Neupert (Nepster) and Namida Verasche";
            // 
            // picturePadding
            // 
            this.picturePadding.BackColor = System.Drawing.SystemColors.ControlDark;
            this.picturePadding.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.picturePadding.InitialImage = null;
            this.picturePadding.Location = new System.Drawing.Point(1170, 82);
            this.picturePadding.Name = "picturePadding";
            this.picturePadding.Size = new System.Drawing.Size(126, 179);
            this.picturePadding.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.picturePadding.TabIndex = 17;
            this.picturePadding.TabStop = false;
            // 
            // FormAboutSLXEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.BackColor = System.Drawing.SystemColors.ControlDark;
            this.ClientSize = new System.Drawing.Size(1300, 811);
            this.Controls.Add(this.lblBasedOn);
            this.Controls.Add(this.pictureClimber);
            this.Controls.Add(this.linkLF);
            this.Controls.Add(this.lblLFCommunity);
            this.Controls.Add(this.lblThanksTo);
            this.Controls.Add(this.lblAuthor);
            this.Controls.Add(this.lblSuperLemmixEditor);
            this.Controls.Add(this.richTextBox_WhatsNew);
            this.Controls.Add(this.richTextBox_PreviousUpdates);
            this.Controls.Add(this.lblPreviousUpdates);
            this.Controls.Add(this.check_ShowThisWindow);
            this.Controls.Add(this.pictureFloater);
            this.Controls.Add(this.lblWhatsNew);
            this.Controls.Add(this.lblDMA);
            this.Controls.Add(this.picturePadding);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FormAboutSLXEditor";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "About SuperLemmix Editor";
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.FormAboutSLXEditor_KeyDown);
            ((System.ComponentModel.ISupportInitialize)(this.pictureFloater)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureClimber)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.picturePadding)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblWhatsNew;
        private PictureBox pictureFloater;
        private PictureBox pictureClimber;
        private CheckBox check_ShowThisWindow;
        private Label lblPreviousUpdates;
        private RichTextBox richTextBox_PreviousUpdates;
        private RichTextBox richTextBox_WhatsNew;
        private Label lblSuperLemmixEditor;
        private Label lblAuthor;
        private Label lblThanksTo;
        private Label lblDMA;
        private Label lblLFCommunity;
        private LinkLabel linkLF;
        private Label lblBasedOn;
        private PictureBox picturePadding;
    }
}