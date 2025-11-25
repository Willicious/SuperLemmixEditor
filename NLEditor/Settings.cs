using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Windows.Forms;

namespace NLEditor
{
    class Settings
    {
        public Settings(NLEditForm editorForm)
        {
            this.editorForm = editorForm;
            SetDefault();
        }

        NLEditForm editorForm;
        Form settingsForm;
        Button btnSaveAndClose;
        Button btnCancel;

        private bool doSaveSettings = false;
        private bool settingChanged = false;

        // Global color dictionary
        private static readonly Dictionary<string, Color> ColorOptions = new Dictionary<string, Color>
        {
            { "Midnight", Color.MidnightBlue },
            { "Red", Color.Red },
            { "Orange", Color.DarkOrange },
            { "Yellow", Color.Khaki },
            { "Green", Color.ForestGreen },
            { "Blue", Color.RoyalBlue },
            { "Purple", Color.DarkViolet },
            { "Black", Color.Black },
            { "Gray", Color.Gray },
            { "White", Color.Silver },
            { "(Invisible)", Color.Empty }
        };

        public enum TriggerAreaColor
        {
            Pink,
            Yellow,
            Green,
            Blue,
            Purple
        }

        public enum EditorMode
        {
            SuperLemmix,
            NeoLemmix,
            Auto
        }

        public enum PieceBrowserMode
        {
            ShowPiecesOnly,
            ShowDescriptions,
            ShowData,
        }

        public bool PreferObjectName { get; private set; }
        public EditorMode CurrentEditorMode { get; private set; }
        public PieceBrowserMode CurrentPieceBrowserMode { get; private set; }
        public TriggerAreaColor CurrentTriggerAreaColor { get; private set; }
        public bool InfiniteScrolling { get; private set; }
        public bool UseGridForPieces { get; private set; }
        public bool UseSpawnInterval { get; private set; }
        public bool ValidateWhenSaving { get; private set; }
        public bool Autosave { get; private set; }
        public bool RemoveOldAutosaves { get; private set; }
        public int NumTooltipBottonDisplay { get; set; }
        public bool UseTooltipBotton => (NumTooltipBottonDisplay > 0);

        private int gridSize;
        public int GridSize { get { return UseGridForPieces ? gridSize : 1; } }
        public int GridMoveAmount { get { return gridSize; } }
        public Color GridColor { get; private set; }

        public int customMove;
        public int CustomMove { get { return customMove; } }
        private int autosaveFrequency;
        public int AutosaveFrequency { get { return Autosave ? autosaveFrequency : 0; } }
        private int keepAutosaveCount;
        public int KeepAutosaveCount { get { return RemoveOldAutosaves ? keepAutosaveCount : 0; } }
        public bool IsFormMaximized { get; private set; }
        public System.Drawing.Size FormSize { get; private set; }

        /// <summary>
        /// Resets the editor options to the default values.
        /// </summary>
        public void SetDefault()
        {
            CurrentEditorMode = EditorMode.Auto;
            CurrentPieceBrowserMode = PieceBrowserMode.ShowData;
            CurrentTriggerAreaColor = TriggerAreaColor.Pink;
            PreferObjectName = false;
            InfiniteScrolling = false;
            UseGridForPieces = false;
            UseSpawnInterval = false;
            gridSize = 8;
            GridColor = Color.MidnightBlue;
            customMove = 64;
            ValidateWhenSaving = true;
            Autosave = true;
            autosaveFrequency = 5;
            RemoveOldAutosaves = true;
            keepAutosaveCount = 10;
            NumTooltipBottonDisplay = 3;
            IsFormMaximized = false;
            FormSize = editorForm.MinimumSize;

            DisplaySettings.SetDisplayed(C.DisplayType.Terrain, true);
            DisplaySettings.SetDisplayed(C.DisplayType.Objects, true);
            DisplaySettings.SetDisplayed(C.DisplayType.Background, true);
            DisplaySettings.SetDisplayed(C.DisplayType.ScreenStart, false);
            DisplaySettings.SetDisplayed(C.DisplayType.Trigger, false);
            DisplaySettings.SetDisplayed(C.DisplayType.ClearPhysics, false);
            DisplaySettings.SetDisplayed(C.DisplayType.Deprecated, false);

            settingChanged = false;
        }

        /// <summary>
        /// Displays the settings form with the settings options.
        /// </summary>
        public void OpenSettingsWindow()
        {
            int columnLeft = 30;
            int columnRight = 340;
            int groupBoxTop = 20;
            int groupBoxColumnLeft = 16;
            int groupBoxColumnRight = 208;
            int buttonsTop = 350;

            settingsForm = new EscExitForm();
            settingsForm.StartPosition = FormStartPosition.CenterScreen;
            settingsForm.ClientSize = new System.Drawing.Size(650, 400);
            settingsForm.FormBorderStyle = FormBorderStyle.FixedDialog;
            settingsForm.MinimizeBox = false;
            settingsForm.MaximizeBox = false;
            settingsForm.ShowInTaskbar = false;
            settingsForm.Text = "SLXEditor - Settings";
            settingsForm.MouseDown += new MouseEventHandler(settingsForm_MouseDown);
            settingsForm.FormClosing += new FormClosingEventHandler(settingsForm_FormClosing);

            // ========================== Editor Mode GroupBox =========================== //

            GroupBox groupEditorMode = new GroupBox();
            groupEditorMode.Text = "Editor Mode";
            groupEditorMode.Top = 20;
            groupEditorMode.Left = columnLeft;
            groupEditorMode.Width = 280;
            groupEditorMode.Height = 50;

            RadioButton radSuperLemmixMode = new RadioButton();
            radSuperLemmixMode.Name = "radSuperLemmixMode";
            radSuperLemmixMode.AutoSize = true;
            radSuperLemmixMode.CheckAlign = System.Drawing.ContentAlignment.MiddleLeft;
            radSuperLemmixMode.Checked = CurrentEditorMode == EditorMode.SuperLemmix;
            radSuperLemmixMode.Text = "SuperLemmix";
            radSuperLemmixMode.Top = groupBoxTop;
            radSuperLemmixMode.Left = groupBoxColumnLeft;
            radSuperLemmixMode.CheckedChanged += new EventHandler(EditorMode_CheckedChanged);

            RadioButton radNeoLemmixMode = new RadioButton();
            radNeoLemmixMode.Name = "radNeoLemmixMode";
            radNeoLemmixMode.AutoSize = true;
            radNeoLemmixMode.CheckAlign = System.Drawing.ContentAlignment.MiddleLeft;
            radNeoLemmixMode.Checked = CurrentEditorMode == EditorMode.NeoLemmix;
            radNeoLemmixMode.Text = "NeoLemmix";
            radNeoLemmixMode.Top = groupBoxTop;
            radNeoLemmixMode.Left = groupBoxColumnLeft + radSuperLemmixMode.Width;
            radNeoLemmixMode.CheckedChanged += new EventHandler(EditorMode_CheckedChanged);

            RadioButton radAutoMode = new RadioButton();
            radAutoMode.Name = "radAutoMode";
            radAutoMode.AutoSize = true;
            radAutoMode.CheckAlign = System.Drawing.ContentAlignment.MiddleLeft;
            radAutoMode.Checked = CurrentEditorMode == EditorMode.Auto;
            radAutoMode.Text = "Auto";
            radAutoMode.Top = groupBoxTop;
            radAutoMode.Left = groupBoxColumnLeft + radSuperLemmixMode.Width + radNeoLemmixMode.Width - 10;
            radAutoMode.CheckedChanged += new EventHandler(EditorMode_CheckedChanged);

            groupEditorMode.Controls.Add(radSuperLemmixMode);
            groupEditorMode.Controls.Add(radNeoLemmixMode);
            groupEditorMode.Controls.Add(radAutoMode);

            // ======================= Piece Browser Mode GroupBox ======================== //

            GroupBox groupPieceBrowserMode = new GroupBox();
            groupPieceBrowserMode.Text = "Piece Browser Mode";
            groupPieceBrowserMode.Top = 90;
            groupPieceBrowserMode.Left = columnLeft;
            groupPieceBrowserMode.Width = 280;
            groupPieceBrowserMode.Height = 110;

            RadioButton radShowPieceData = new RadioButton();
            radShowPieceData.Name = "radShowPieceData";
            radShowPieceData.AutoSize = true;
            radShowPieceData.Width = 80;
            radShowPieceData.CheckAlign = System.Drawing.ContentAlignment.MiddleLeft;
            radShowPieceData.Checked = CurrentPieceBrowserMode == PieceBrowserMode.ShowData;
            radShowPieceData.Text = "Data";
            radShowPieceData.Top = groupBoxTop;
            radShowPieceData.Left = groupBoxColumnLeft;
            radShowPieceData.CheckedChanged += new EventHandler(PieceBrowserMode_CheckedChanged);

            RadioButton radShowPieceDescriptions = new RadioButton();
            radShowPieceDescriptions.Name = "radShowPieceDescriptions";
            radShowPieceDescriptions.AutoSize = true;
            radShowPieceDescriptions.Width = 80;
            radShowPieceDescriptions.CheckAlign = System.Drawing.ContentAlignment.MiddleLeft;
            radShowPieceDescriptions.Checked = CurrentPieceBrowserMode == PieceBrowserMode.ShowDescriptions;
            radShowPieceDescriptions.Text = "Descriptions";
            radShowPieceDescriptions.Top = groupBoxTop;
            radShowPieceDescriptions.Left = groupBoxColumnLeft + radShowPieceData.Width - 16;
            radShowPieceDescriptions.CheckedChanged += new EventHandler(PieceBrowserMode_CheckedChanged);

            RadioButton radShowPiecesOnly = new RadioButton();
            radShowPiecesOnly.Name = "radShowPiecesOnly";
            radShowPiecesOnly.AutoSize = true;
            radShowPiecesOnly.CheckAlign = System.Drawing.ContentAlignment.MiddleLeft;
            radShowPiecesOnly.Checked = CurrentPieceBrowserMode == PieceBrowserMode.ShowPiecesOnly;
            radShowPiecesOnly.Text = "Pieces Only";
            radShowPiecesOnly.Top = groupBoxTop;
            radShowPiecesOnly.Left = groupBoxColumnLeft + radShowPieceData.Width + radShowPieceDescriptions.Width;
            radShowPiecesOnly.CheckedChanged += new EventHandler(PieceBrowserMode_CheckedChanged);

            CheckBox checkPreferObjectName = new CheckBox();
            checkPreferObjectName.Name = "checkPreferObjectName";
            checkPreferObjectName.AutoSize = true;
            checkPreferObjectName.CheckAlign = System.Drawing.ContentAlignment.MiddleLeft;
            checkPreferObjectName.Checked = PreferObjectName;
            checkPreferObjectName.Text = "Prefer piece name to object type";
            checkPreferObjectName.Top = groupBoxTop + 30;
            checkPreferObjectName.Left = groupBoxColumnLeft;
            checkPreferObjectName.CheckedChanged += new EventHandler(checkPreferObjectName_CheckedChanged);

            CheckBox checkInfiniteScrolling = new CheckBox();
            checkInfiniteScrolling.Name = "checkInfiniteScrolling";
            checkInfiniteScrolling.AutoSize = true;
            checkInfiniteScrolling.CheckAlign = System.Drawing.ContentAlignment.MiddleLeft;
            checkInfiniteScrolling.Checked = InfiniteScrolling;
            checkInfiniteScrolling.Text = "Infinite Scrolling";
            checkInfiniteScrolling.Top = groupBoxTop + 60;
            checkInfiniteScrolling.Left = groupBoxColumnLeft;
            checkInfiniteScrolling.CheckedChanged += new EventHandler(checkInfiniteScrolling_CheckedChanged);

            groupPieceBrowserMode.Controls.Add(radShowPiecesOnly);
            groupPieceBrowserMode.Controls.Add(radShowPieceDescriptions);
            groupPieceBrowserMode.Controls.Add(radShowPieceData);
            groupPieceBrowserMode.Controls.Add(checkPreferObjectName);
            groupPieceBrowserMode.Controls.Add(checkInfiniteScrolling);

            // =========================== Saving Options GroupBox =========================== //

            GroupBox groupSavingOptions = new GroupBox();
            groupSavingOptions.Text = "Level Saving Options";
            groupSavingOptions.Top = 220;
            groupSavingOptions.Left = columnLeft;
            groupSavingOptions.Width = 280;
            groupSavingOptions.Height = 110;

            CheckBox checkValidateWhenSaving = new CheckBox();
            checkValidateWhenSaving.Name = "checkValidateWhenSaving";
            checkValidateWhenSaving.AutoSize = true;
            checkValidateWhenSaving.CheckAlign = System.Drawing.ContentAlignment.MiddleLeft;
            checkValidateWhenSaving.Checked = ValidateWhenSaving;
            checkValidateWhenSaving.Text = "Validate level when saving";
            checkValidateWhenSaving.Top = groupBoxTop;
            checkValidateWhenSaving.Left = groupBoxColumnLeft;
            checkValidateWhenSaving.CheckedChanged += new EventHandler(checkValidateWhenSaving_CheckedChanged);

            CheckBox checkAutosave = new CheckBox();
            checkAutosave.Name = "checkAutosave";
            checkAutosave.AutoSize = true;
            checkAutosave.CheckAlign = System.Drawing.ContentAlignment.MiddleLeft;
            checkAutosave.Checked = Autosave;
            checkAutosave.Text = "Autosave level every";
            checkAutosave.Top = groupBoxTop + 30;
            checkAutosave.Left = groupBoxColumnLeft;
            checkAutosave.CheckedChanged += new EventHandler(checkAutosave_CheckedChanged);

            NumericUpDown numAutosaveFrequency = new NumericUpDown();
            numAutosaveFrequency.Name = "numAutosaveFrequency";
            numAutosaveFrequency.AutoSize = true;
            numAutosaveFrequency.TextAlign = HorizontalAlignment.Center;
            numAutosaveFrequency.Value = autosaveFrequency;
            numAutosaveFrequency.Minimum = 1;
            numAutosaveFrequency.Maximum = 60;
            numAutosaveFrequency.Top = checkAutosave.Top - 2;
            numAutosaveFrequency.Left = checkAutosave.Right + 24;
            numAutosaveFrequency.Width = 48;
            numAutosaveFrequency.Enabled = Autosave;
            numAutosaveFrequency.ValueChanged += new EventHandler(numAutosaveFrequency_ValueChanged);
            numAutosaveFrequency.KeyDown += new KeyEventHandler(numUpDown_KeyDown);

            Label lblMinutes = new Label();
            lblMinutes.Text = "minutes";
            lblMinutes.AutoSize = true;
            lblMinutes.Top = groupBoxTop + 30;
            lblMinutes.Left = numAutosaveFrequency.Right + 8;

            CheckBox checkDeleteAutosaves = new CheckBox();
            checkDeleteAutosaves.Name = "checkDeleteAutosaves";
            checkDeleteAutosaves.AutoSize = true;
            checkDeleteAutosaves.CheckAlign = System.Drawing.ContentAlignment.MiddleLeft;
            checkDeleteAutosaves.Checked = RemoveOldAutosaves;
            checkDeleteAutosaves.Enabled = Autosave;
            checkDeleteAutosaves.Text = "Limit autosaves kept to";
            checkDeleteAutosaves.Top = groupBoxTop + 60;
            checkDeleteAutosaves.Left = groupBoxColumnLeft;
            checkDeleteAutosaves.CheckedChanged += new EventHandler(checkDeleteAutosaves_CheckedChanged);

            NumericUpDown numAutosavesToKeep = new NumericUpDown();
            numAutosavesToKeep.Name = "numAutosavesToKeep";
            numAutosavesToKeep.AutoSize = true;
            numAutosavesToKeep.TextAlign = HorizontalAlignment.Center;
            numAutosavesToKeep.Value = keepAutosaveCount;
            numAutosavesToKeep.Minimum = 1;
            numAutosavesToKeep.Maximum = 999;
            numAutosavesToKeep.Top = checkDeleteAutosaves.Top - 2;
            numAutosavesToKeep.Left = checkDeleteAutosaves.Right + 34;
            numAutosavesToKeep.Width = 48;
            numAutosavesToKeep.Enabled = Autosave && RemoveOldAutosaves;
            numAutosavesToKeep.ValueChanged += new EventHandler(numAutosavesToKeep_ValueChanged);
            numAutosavesToKeep.KeyDown += new KeyEventHandler(numUpDown_KeyDown);

            groupSavingOptions.Controls.Add(checkValidateWhenSaving);
            groupSavingOptions.Controls.Add(checkAutosave);
            groupSavingOptions.Controls.Add(numAutosaveFrequency);
            groupSavingOptions.Controls.Add(lblMinutes);
            groupSavingOptions.Controls.Add(checkDeleteAutosaves);
            groupSavingOptions.Controls.Add(numAutosavesToKeep);

            // ========================== Snap-to-Grid GroupBox ========================== //

            GroupBox groupSnapToGrid = new GroupBox();
            groupSnapToGrid.Text = "Snap Pieces to Grid";
            groupSnapToGrid.Top = 20;
            groupSnapToGrid.Left = columnRight;
            groupSnapToGrid.Width = 280;
            groupSnapToGrid.Height = 80;

            CheckBox checkUseGrid = new CheckBox();
            checkUseGrid.Name = "checkUseGrid";
            checkUseGrid.AutoSize = true;
            checkUseGrid.CheckAlign = System.Drawing.ContentAlignment.MiddleLeft;
            checkUseGrid.Checked = UseGridForPieces;
            checkUseGrid.Text = "Snap-to-Grid amount in pixels:";
            checkUseGrid.Top = groupBoxTop;
            checkUseGrid.Left = groupBoxColumnLeft;
            checkUseGrid.CheckedChanged += new EventHandler(checkUseGrid_CheckedChanged);

            NumericUpDown numGridSize = new NumericUpDown();
            numGridSize.Name = "numGridSize";
            numGridSize.AutoSize = true;
            numGridSize.TextAlign = HorizontalAlignment.Center;
            numGridSize.Minimum = 2;
            numGridSize.Maximum = 128;
            numGridSize.Value = gridSize;
            numGridSize.Top = checkUseGrid.Top - 2;
            numGridSize.Left = groupBoxColumnRight;
            numGridSize.Width = 48;
            numGridSize.Enabled = UseGridForPieces;
            numGridSize.ValueChanged += new EventHandler(numGridSize_ValueChanged);
            numGridSize.KeyDown += new KeyEventHandler(numUpDown_KeyDown);

            Label lblGridColor = new Label();
            lblGridColor.Name = "lblGridColor";
            lblGridColor.Text = "Choose grid color:";
            lblGridColor.Top = groupBoxTop + 30;
            lblGridColor.Left = groupBoxColumnLeft;
            lblGridColor.AutoSize = true;
            lblGridColor.Enabled = UseGridForPieces;

            ComboBox comboGridColor = new ComboBox();
            comboGridColor.Name = "comboGridColor";
            comboGridColor.DropDownStyle = ComboBoxStyle.DropDownList;
            comboGridColor.Top = lblGridColor.Top - 4;
            comboGridColor.Left = lblGridColor.Right + 20;
            comboGridColor.Width = 120;
            comboGridColor.Enabled = UseGridForPieces;
            comboGridColor.Items.AddRange(ColorOptions.Keys.ToArray());
            comboGridColor.SelectedItem = ColorOptions.FirstOrDefault(x => x.Value == GridColor).Key ?? "Midnight";
            comboGridColor.SelectedIndexChanged += new EventHandler(comboGridColor_IndexChanged);

            groupSnapToGrid.Controls.Add(checkUseGrid);
            groupSnapToGrid.Controls.Add(numGridSize);
            groupSnapToGrid.Controls.Add(lblGridColor);
            groupSnapToGrid.Controls.Add(comboGridColor);

            // ========================== Custom Move GroupBox =========================== //

            GroupBox groupCustomMove = new GroupBox();
            groupCustomMove.Text = "Custom move selected pieces";
            groupCustomMove.Top = 120;
            groupCustomMove.Left = columnRight;
            groupCustomMove.Width = 280;
            groupCustomMove.Height = 50;

            Label lblCustomMove = new Label();
            lblCustomMove.Text = "Custom move amount in pixels:";
            lblCustomMove.AutoSize = true;
            lblCustomMove.Top = groupBoxTop;
            lblCustomMove.Left = groupBoxColumnLeft;

            NumericUpDown numCustomMove = new NumericUpDown();
            numCustomMove.Name = "numCustomMove";
            numCustomMove.AutoSize = true;
            numCustomMove.TextAlign = HorizontalAlignment.Center;
            numCustomMove.Minimum = 2;
            numCustomMove.Maximum = 3200;
            numCustomMove.Value = customMove;
            numCustomMove.Top = lblCustomMove.Top - 2;
            numCustomMove.Left = groupBoxColumnRight;
            numCustomMove.Width = 48;
            numCustomMove.ValueChanged += new EventHandler(numCustomMove_ValueChanged);
            numCustomMove.KeyDown += new KeyEventHandler(numUpDown_KeyDown);

            groupCustomMove.Controls.Add(lblCustomMove);
            groupCustomMove.Controls.Add(numCustomMove);

            // ========================== Trigger Area Color GroupBox ========================== //

            GroupBox groupTriggerAreaColor = new GroupBox();
            groupTriggerAreaColor.Text = "Trigger Area Color";
            groupTriggerAreaColor.Top = 190;
            groupTriggerAreaColor.Left = columnRight;
            groupTriggerAreaColor.Width = 280;
            groupTriggerAreaColor.Height = 50;

            Label lblTriggerAreaColor = new Label();
            lblTriggerAreaColor.Name = "lblTriggerAreaColor";
            lblTriggerAreaColor.Text = "Choose trigger area color:";
            lblTriggerAreaColor.Top = groupBoxTop;
            lblTriggerAreaColor.Left = groupBoxColumnLeft;
            lblTriggerAreaColor.AutoSize = true;
            lblTriggerAreaColor.Enabled = true;

            ComboBox comboTriggerAreaColor = new ComboBox();
            comboTriggerAreaColor.Name = "comboTriggerAreaColor";
            comboTriggerAreaColor.DropDownStyle = ComboBoxStyle.DropDownList;
            comboTriggerAreaColor.Top = lblTriggerAreaColor.Top - 4;
            comboTriggerAreaColor.Left = lblTriggerAreaColor.Right + 40;
            comboTriggerAreaColor.Width = 100;
            comboTriggerAreaColor.Enabled = true;
            comboTriggerAreaColor.Items.AddRange(Enum.GetNames(typeof(TriggerAreaColor)));
            comboTriggerAreaColor.SelectedItem = CurrentTriggerAreaColor.ToString();
            comboTriggerAreaColor.SelectedIndexChanged += new EventHandler(comboTriggerAreaColor_IndexChanged);

            groupTriggerAreaColor.Controls.Add(lblTriggerAreaColor);
            groupTriggerAreaColor.Controls.Add(comboTriggerAreaColor);

            // ========================== Spawn Interval GroupBox ========================== //

            GroupBox groupSpawnInterval = new GroupBox();
            groupSpawnInterval.Text = "Spawn Interval / Release Rate";
            groupSpawnInterval.Top = 260;
            groupSpawnInterval.Left = columnRight;
            groupSpawnInterval.Width = 280;
            groupSpawnInterval.Height = 50;

            RadioButton radUseSpawnInterval = new RadioButton();
            radUseSpawnInterval.Name = "radUseSpawnInterval";
            radUseSpawnInterval.AutoSize = true;
            radUseSpawnInterval.Width = 130;
            radUseSpawnInterval.CheckAlign = System.Drawing.ContentAlignment.MiddleLeft;
            radUseSpawnInterval.Checked = UseSpawnInterval;
            radUseSpawnInterval.Text = "Spawn Interval";
            radUseSpawnInterval.Top = groupBoxTop;
            radUseSpawnInterval.Left = groupBoxColumnLeft;
            radUseSpawnInterval.CheckedChanged += new EventHandler(UseSpawnInterval_CheckedChanged);

            RadioButton radUseReleaseRate = new RadioButton();
            radUseReleaseRate.Name = "radUseReleaseRate";
            radUseReleaseRate.AutoSize = true;
            radUseReleaseRate.Width = 130;
            radUseReleaseRate.CheckAlign = System.Drawing.ContentAlignment.MiddleLeft;
            radUseReleaseRate.Checked = !UseSpawnInterval;
            radUseReleaseRate.Text = "Release Rate";
            radUseReleaseRate.Top = groupBoxTop;
            radUseReleaseRate.Left = groupBoxColumnLeft + radUseSpawnInterval.Width - 16;
            radUseReleaseRate.CheckedChanged += new EventHandler(UseSpawnInterval_CheckedChanged);

            groupSpawnInterval.Controls.Add(radUseSpawnInterval);
            groupSpawnInterval.Controls.Add(radUseReleaseRate);

            // ========================== Save And Close Button ========================== //

            btnSaveAndClose = new Button();
            btnSaveAndClose.Height = 30;
            btnSaveAndClose.Width = 110;
            btnSaveAndClose.Top = buttonsTop;
            btnSaveAndClose.Text = "Save And Close";
            btnSaveAndClose.Click += new EventHandler(BtnSaveAndClose_Click);

            btnCancel = new Button();
            btnCancel.Height = 30;
            btnCancel.Width = 70;
            btnCancel.Top = buttonsTop;
            btnCancel.Text = "Cancel";
            btnCancel.Click += new EventHandler(BtnCancel_Click);
            
            // Position the buttons
            int totalButtonsWidth = btnSaveAndClose.Width + 10 + btnCancel.Width;
            int startX = (settingsForm.Width - totalButtonsWidth) / 2;
            btnSaveAndClose.Left = startX;
            btnCancel.Left = startX + btnSaveAndClose.Width + 10;


            // ========================== Add Controls to Form =========================== //

            settingsForm.Controls.Add(groupEditorMode);
            settingsForm.Controls.Add(groupPieceBrowserMode);
            settingsForm.Controls.Add(groupCustomMove);
            settingsForm.Controls.Add(groupSnapToGrid);
            settingsForm.Controls.Add(groupTriggerAreaColor);
            settingsForm.Controls.Add(groupSpawnInterval);
            settingsForm.Controls.Add(groupSavingOptions);

            settingsForm.Controls.Add(btnSaveAndClose);
            settingsForm.Controls.Add(btnCancel);

            settingsForm.ShowDialog();
        }

        private void settingsForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = true;
            PrepareFormForClosing();
            e.Cancel = false;
        }

        private void settingsForm_MouseDown(object sender, MouseEventArgs e)
        {
            btnSaveAndClose.Focus();
        }

        private void BtnSaveAndClose_Click(object sender, EventArgs e)
        {
            doSaveSettings = true;
            settingsForm.Close();
        }

        private void BtnCancel_Click(object sender, EventArgs e)
        {
            doSaveSettings = false;
            settingChanged = false;
            settingsForm.Close();
        }

        private void EditorMode_CheckedChanged(object sender, EventArgs e)
        {           
            if (sender is RadioButton rb && rb.Checked)
            {
                switch (rb.Name)
                {
                    case "radNeoLemmixMode":
                        CurrentEditorMode = EditorMode.NeoLemmix;
                        break;
                    case "radSuperLemmixMode":
                        CurrentEditorMode = EditorMode.SuperLemmix;
                        break;
                    case "radAutoMode":
                        CurrentEditorMode = EditorMode.Auto;
                        break;
                }

                editorForm.DetectLemmixVersions();
                editorForm.UpdateLemmixVersionFeatures();
            }

            settingChanged = true;
        }

        private void PieceBrowserMode_CheckedChanged(object sender, EventArgs e)
        {
            if (sender is RadioButton rb && rb.Checked)
            {
                switch (rb.Name)
                {
                    case "radShowPiecesOnly":
                        CurrentPieceBrowserMode = PieceBrowserMode.ShowPiecesOnly;
                        break;
                    case "radShowPieceDescriptions":
                        CurrentPieceBrowserMode = PieceBrowserMode.ShowDescriptions;
                        break;
                    case "radShowPieceData":
                        CurrentPieceBrowserMode = PieceBrowserMode.ShowData;
                        break;
                }

                editorForm.LoadPiecesIntoPictureBox();
            }

            if (settingsForm.Controls.Find("checkPreferObjectName", true).FirstOrDefault() is CheckBox checkPreferObjectName)
            {
                if (CurrentPieceBrowserMode == PieceBrowserMode.ShowPiecesOnly)
                    checkPreferObjectName.Enabled = false;
                else
                    checkPreferObjectName.Enabled = true;
            }

            settingChanged = true;
        }

        private void UseSpawnInterval_CheckedChanged(object sender, EventArgs e)
        {
            if (sender is RadioButton rb && rb.Checked)
            {
                switch (rb.Name)
                {
                    case "radUseSpawnInterval":
                        UseSpawnInterval = true;
                        break;
                    case "radUseReleaseRate":
                        UseSpawnInterval = false;
                        break;
                }

                editorForm.UpdateRRSIControls();
            }

            settingChanged = true;
        }

        private void checkPreferObjectName_CheckedChanged(object sender, EventArgs e)
        {
            PreferObjectName = ((sender as CheckBox).CheckState == CheckState.Checked);
            editorForm.LoadPiecesIntoPictureBox();
            settingChanged = true;
        }

        private void checkInfiniteScrolling_CheckedChanged(object sender, EventArgs e)
        {
            InfiniteScrolling = ((sender as CheckBox).CheckState == CheckState.Checked);
            settingChanged = true;
        }

        private void checkUseGrid_CheckedChanged(object sender, EventArgs e)
        {
            UseGridForPieces = ((sender as CheckBox).CheckState == CheckState.Checked);

            if (settingsForm.Controls.Find("numGridSize", true).FirstOrDefault() is NumericUpDown numGridSize)
                numGridSize.Enabled = UseGridForPieces;

            if (settingsForm.Controls.Find("lblGridColor", true).FirstOrDefault() is Label lblGridColor)
                lblGridColor.Enabled = UseGridForPieces;

            if (settingsForm.Controls.Find("comboGridColor", true).FirstOrDefault() is ComboBox comboGridColor)
                comboGridColor.Enabled = UseGridForPieces;

            editorForm.ToggleSnapToGrid();

            settingChanged = true;
        }

        private void numGridSize_ValueChanged(object sender, EventArgs e)
        {
            gridSize = (int)(sender as NumericUpDown).Value;
            editorForm.ToggleSnapToGrid();

            settingChanged = true;
        }

        private void comboGridColor_IndexChanged(object sender, EventArgs e)
        {
            if (sender is ComboBox comboGridColor &&
                comboGridColor.SelectedItem is string selectedColor &&
                ColorOptions.ContainsKey(selectedColor))
            {
                GridColor = ColorOptions[selectedColor];
            }
            editorForm.ToggleSnapToGrid();

            settingChanged = true;
        }

        private void comboTriggerAreaColor_IndexChanged(object sender, EventArgs e)
        {
            if (sender is ComboBox comboTriggerAreaColor &&
                comboTriggerAreaColor.SelectedItem != null)
            {
                Enum.TryParse<TriggerAreaColor>(
                    comboTriggerAreaColor.SelectedItem.ToString(),
                    out var selectedColor);

                CurrentTriggerAreaColor = selectedColor;
            }
            editorForm.RefreshLevel();

            settingChanged = true;
        }

        private void numCustomMove_ValueChanged(object sender, EventArgs e)
        {
            customMove = (int)(sender as NumericUpDown).Value;
            settingChanged = true;
        }

        private void numUpDown_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                btnSaveAndClose.Focus();
            }
        }

        private void checkValidateWhenSaving_CheckedChanged(object sender, EventArgs e)
        {
            ValidateWhenSaving = ((sender as CheckBox).CheckState == CheckState.Checked);
            settingChanged = true;
        }

        private void checkAutosave_CheckedChanged(object sender, EventArgs e)
        {
            Autosave = ((sender as CheckBox).CheckState == CheckState.Checked);

            if (settingsForm.Controls.Find("numAutoSaveFrequency", true).FirstOrDefault() is NumericUpDown numAutoSaveFrequency)
                numAutoSaveFrequency.Enabled = Autosave;

            if (settingsForm.Controls.Find("checkDeleteAutosaves", true).FirstOrDefault() is CheckBox checkDeleteAutosaves)
                checkDeleteAutosaves.Enabled = Autosave;

            if (settingsForm.Controls.Find("numAutosavesToKeep", true).FirstOrDefault() is NumericUpDown numAutosavesToKeep)
                numAutosavesToKeep.Enabled = Autosave && RemoveOldAutosaves;

            settingChanged = true;
        }

        private void numAutosaveFrequency_ValueChanged(object sender, EventArgs e)
        {
            autosaveFrequency = (int)(sender as NumericUpDown).Value;
            settingChanged = true;
        }

        private void checkDeleteAutosaves_CheckedChanged(object sender, EventArgs e)
        {
            RemoveOldAutosaves = ((sender as CheckBox).CheckState == CheckState.Checked);

            if (settingsForm.Controls.Find("numAutosavesToKeep", true).FirstOrDefault() is NumericUpDown numAutosavesToKeep)
                numAutosavesToKeep.Enabled = Autosave && RemoveOldAutosaves;

            settingChanged = true;
        }

        private void numAutosavesToKeep_ValueChanged(object sender, EventArgs e)
        {
            keepAutosaveCount = (int)(sender as NumericUpDown).Value;
            settingChanged = true;
        }

        /// <summary>
        /// Sets the settings options regarding the form size according to current form usage.
        /// </summary>
        public void SetFormSize()
        {
            if (editorForm.WindowState == FormWindowState.Maximized)
            {
                IsFormMaximized = true;
            }
            else
            {
                IsFormMaximized = false;
                FormSize = editorForm.ClientSize;
            }
        }

        /// <summary>
        /// Switches between using the Grid and not doing so
        /// </summary>
        public void SwitchGridUsage()
        {
            UseGridForPieces = !UseGridForPieces;
        }

        private void ValidateGridColor(FileLine line)
        {
            string colorString = line.Text.Trim();
            if (colorString == "(Invisible)")
            {
                GridColor = Color.Empty;  // Set invisible color if the string is "(Invisible)"
            }
            else
            {
                try
                {
                    GridColor = ColorTranslator.FromHtml(colorString);
                }
                catch (Exception) // Default to MidnightBlue if the color string is invalid
                {
                    GridColor = Color.MidnightBlue;
                }
            }
        }

        private void ValidateTriggerAreaColor(FileLine line)
        {
            string colorString = line.Text.Trim();

            if (!Enum.TryParse<TriggerAreaColor>(colorString, ignoreCase: true, out var parsedColor))
            {                
                parsedColor = TriggerAreaColor.Pink; // Default to Pink if parsing fails
            }

            CurrentTriggerAreaColor = parsedColor;
        }

        private void PrepareFormForClosing()
        {
            btnCancel.Focus(); // Prevents saving values if caret is still in numUpDown

            if (!doSaveSettings)
            {
                if (settingChanged)
                {
                    // Show dialog asking if the user wants to save
                    DialogResult result = MessageBox.Show(
                        "Do you want to save your changes?",
                        "Unsaved Changes",
                        MessageBoxButtons.YesNoCancel,
                        MessageBoxIcon.Question
                    );

                    if (result == DialogResult.Yes)
                    {
                        WriteSettingsToFile();
                    }
                    else if (result == DialogResult.No)
                    {
                        ReloadSettings();
                    }
                    else if (result == DialogResult.Cancel)
                    {
                        return;
                    }
                }
                else
                {
                    ReloadSettings();
                }
            }
            else
            {
                WriteSettingsToFile();
            }
        }

        public void ReloadSettings()
        {
            ReadSettingsFromFile();

            editorForm.ToggleSnapToGrid();
            editorForm.LoadPiecesIntoPictureBox();

            WriteSettingsToFile();
        }

        /// <summary>
        /// Reads the users editor settings from SLXEditorSettings.ini.
        /// </summary>
        public void ReadSettingsFromFile()
        {
            SetDefault();

            if (!File.Exists(C.AppPathSettings))
                return;

            // Reset background display to false
            DisplaySettings.SetDisplayed(C.DisplayType.Background, false);

            try
            {
                FileParser parser = new FileParser(C.AppPathSettings);

                List<FileLine> fileLines;
                while ((fileLines = parser.GetNextLines()) != null)
                {
                    FileLine line = fileLines?[0];
                    switch (line?.Key)
                    {
                        case "EDITORMODE":
                            {
                                var modeText = line.Text.Trim().ToUpper();
                                if (modeText == "SUPERLEMMIX")
                                    CurrentEditorMode = EditorMode.SuperLemmix;
                                else if (modeText == "NEOLEMMIX")
                                    CurrentEditorMode = EditorMode.NeoLemmix;
                                else // Default to Auto Mode
                                    CurrentEditorMode = EditorMode.Auto;
                                break;
                            }
                        case "PIECEBROWSERMODE":
                            {
                                var modeText = line.Text.Trim().ToUpper();
                                if (modeText == "SHOWPIECESONLY")
                                    CurrentPieceBrowserMode = PieceBrowserMode.ShowPiecesOnly;
                                else if (modeText == "SHOWDESCRIPTIONS")
                                    CurrentPieceBrowserMode = PieceBrowserMode.ShowDescriptions;
                                else // Default to Show Data
                                    CurrentPieceBrowserMode = PieceBrowserMode.ShowData;
                                break;
                            }
                        case "PREFEROBJECTNAME":
                            {
                                PreferObjectName = (line.Text.Trim().ToUpper() == "TRUE");
                                break;
                            }
                        case "INFINITESCROLLING":
                            {
                                InfiniteScrolling = (line.Text.Trim().ToUpper() == "TRUE");
                                break;
                            }
                        case "GRIDSIZE":
                            {
                                UseGridForPieces = (line.Value != 1);
                                if (UseGridForPieces)
                                    gridSize = line.Value;
                                break;
                            }
                        case "GRIDCOLOR":
                            {
                                ValidateGridColor(line);
                                break;
                            }
                        case "TRIGGERAREACOLOR":
                            {
                                ValidateTriggerAreaColor(line);
                                break;
                            }
                        case "CUSTOMMOVE":
                            {
                                customMove = line.Value;
                                break;
                            }
                        case "VALIDATEWHENSAVING":
                            {
                                ValidateWhenSaving = (line.Text.Trim().ToUpper() == "TRUE");
                                break;
                            }
                        case "AUTOSAVE":
                            {
                                Autosave = (line.Value != 0);
                                if (Autosave)
                                    autosaveFrequency = line.Value;
                                break;
                            }
                        case "AUTOSAVELIMIT":
                            {
                                RemoveOldAutosaves = (line.Value != 0);
                                if (RemoveOldAutosaves)
                                    keepAutosaveCount = line.Value;
                                break;
                            }
                        case "BUTTON_TOOLTIP":
                            {
                                NumTooltipBottonDisplay = line.Value;
                                break;
                            }
                        case "DISPLAY":
                            {
                                if (Utility.ExistsInEnum<C.DisplayType>(line.Text.Trim()))
                                {
                                    C.DisplayType displayType = Utility.ParseEnum<C.DisplayType>(line.Text.Trim());
                                    DisplaySettings.SetDisplayed(displayType, true);
                                }
                                break;
                            }
                        case "FORM_MAXIMIZED":
                            {
                                IsFormMaximized = (line.Text.Trim().ToUpper() == "TRUE");
                                break;
                            }
                        case "FORM_WIDTH":
                            {
                                FormSize = new System.Drawing.Size(line.Value, FormSize.Height);
                                break;
                            }
                        case "FORM_HEIGHT":
                            {
                                FormSize = new System.Drawing.Size(FormSize.Width, line.Value);
                                break;
                            }
                        case "AUTOSTART":
                            {
                                bool autoStartEnabled;
                                if (bool.TryParse(line.Text.Trim(), out autoStartEnabled))
                                {
                                    editorForm.chk_Lvl_AutoStart.Checked = autoStartEnabled;
                                }
                                break;
                            }
                    }
                }

                parser.DisposeStreamReader();
            }
            catch (Exception Ex)
            {
                MessageBox.Show("Warning: Could not read editor options from "
                            + Path.GetFileName(C.AppPathSettings) + ". Editor uses the default settings.", "File not found");
                Utility.LogException(Ex);
            }

            editorForm.previousEditorMode = CurrentEditorMode;
            settingChanged = false;
        }

        /// <summary>
        /// Saves the user's current editor settings to SLXEditorSettings.ini. 
        /// </summary>
        public void WriteSettingsToFile()
        {
            try
            {
                string settingsDirectory = Path.GetDirectoryName(C.AppPathSettings);
                if (!Directory.Exists(settingsDirectory))
                {
                    Directory.CreateDirectory(settingsDirectory);
                }

                File.Create(C.AppPathSettings).Close();

                TextWriter settingsFile = new StreamWriter(C.AppPathSettings, true);

                settingsFile.WriteLine("# SLXEditor settings ");
                settingsFile.WriteLine(" ValidateWhenSaving  " + (ValidateWhenSaving ? "True" : "False"));
                settingsFile.WriteLine(" Autosave            " + AutosaveFrequency.ToString());
                settingsFile.WriteLine(" AutosaveLimit       " + KeepAutosaveCount.ToString());
                settingsFile.WriteLine(" EditorMode          " + CurrentEditorMode.ToString());
                settingsFile.WriteLine(" PieceBrowserMode    " + CurrentPieceBrowserMode.ToString());
                settingsFile.WriteLine(" PreferObjectName    " + (PreferObjectName ? "True" : "False"));
                settingsFile.WriteLine(" InfiniteScrolling   " + (InfiniteScrolling ? "True" : "False"));
                settingsFile.WriteLine(" GridSize            " + GridSize.ToString());
                settingsFile.WriteLine(" GridColor           " + (GridColor == Color.Empty ? "(Invisible)" : ColorTranslator.ToHtml(GridColor)));
                settingsFile.WriteLine(" TriggerAreaColor    " + CurrentTriggerAreaColor.ToString());
                settingsFile.WriteLine(" CustomMove          " + CustomMove.ToString());
                settingsFile.WriteLine(" Button_Tooltip      " + NumTooltipBottonDisplay.ToString());
                settingsFile.WriteLine("");
                settingsFile.WriteLine(" Form_Maximized      " + (IsFormMaximized ? "True" : "False"));
                settingsFile.WriteLine(" Form_Width          " + FormSize.Width.ToString());
                settingsFile.WriteLine(" Form_Height         " + FormSize.Height.ToString());
                settingsFile.WriteLine(" Autostart           " + editorForm.chk_Lvl_AutoStart.Checked.ToString());
                settingsFile.WriteLine("");

                var displayTypes = new List<C.DisplayType>()
                {
                    C.DisplayType.Trigger, C.DisplayType.ScreenStart, C.DisplayType.Background, C.DisplayType.Deprecated
                };
                foreach (var displayType in displayTypes)
                {
                    if (DisplaySettings.IsDisplayed(displayType))
                    {
                        settingsFile.WriteLine(" Display             " + displayType.ToString());
                    }
                }

                settingsFile.Close();
            }
            catch (Exception Ex)
            {
                Utility.LogException(Ex);
                MessageBox.Show("Error: Could not save settings to " + Path.GetFileName(C.AppPathSettings) + ".", "Could not save");
                return;
            }
        }
    }
}
