using System;
using System.Collections.Generic;
using System.IO;
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

        public enum EditorMode
        {
            SuperLemmix,
            NeoLemmix,
            Auto
        }

        public EditorMode CurrentEditorMode { get; private set; }
        public bool UsePieceSelectionNames { get; private set; }
        public bool UseGridForPieces { get; private set; }
        public bool Autosave { get; private set; }
        public bool RemoveOldAutosaves { get; private set; }
        public int NumTooltipBottonDisplay { get; set; }
        public bool UseTooltipBotton => (NumTooltipBottonDisplay > 0);
        private int gridSize;
        public int GridSize { get { return UseGridForPieces ? gridSize : 1; } }
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
            CurrentEditorMode = EditorMode.SuperLemmix;
            UsePieceSelectionNames = true;
            UseGridForPieces = false;
            gridSize = 8;
            customMove = 64;
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
        }

        /// <summary>
        /// Displays the settings form with the settings options.
        /// </summary>
        public void OpenSettingsWindow()
        {
            int columnLeft = 30;
            int groupBoxTop = 20;
            int groupBoxColumnLeft = 16;
            int groupBoxColumnRight = 208;

            settingsForm = new EscExitForm();
            settingsForm.StartPosition = FormStartPosition.CenterScreen;
            settingsForm.ClientSize = new System.Drawing.Size(340, 380);
            settingsForm.MaximizeBox = false;
            settingsForm.ShowInTaskbar = false;
            settingsForm.FormBorderStyle = FormBorderStyle.FixedToolWindow;
            settingsForm.Text = "SLXEditor - Settings";
            settingsForm.FormClosing += new FormClosingEventHandler(settingsForm_FormClosing);

            // =========================== Use Piece Names =========================== //

            CheckBox checkPieceNames = new CheckBox();
            checkPieceNames.Name = "check_PieceNames";
            checkPieceNames.AutoSize = true;
            checkPieceNames.CheckAlign = System.Drawing.ContentAlignment.MiddleLeft;
            checkPieceNames.Checked = UsePieceSelectionNames;
            checkPieceNames.Text = "Display piece names in piece selection browser";
            checkPieceNames.Top = 20;
            checkPieceNames.Left = columnLeft;
            checkPieceNames.CheckedChanged += new EventHandler(checkPieceNames_CheckedChanged);

            // ========================== Custom Move GroupBox =========================== //

            GroupBox groupCustomMove = new GroupBox();
            groupCustomMove.Text = "Custom move selected pieces (Alt + Arrows)";
            groupCustomMove.Top = 60;
            groupCustomMove.Left = columnLeft;
            groupCustomMove.Width = 280;
            groupCustomMove.Height = 50;

            Label lblCustomMove = new Label();
            lblCustomMove.Text = "Custom move amount in pixels:";
            lblCustomMove.AutoSize = true;
            lblCustomMove.Top = groupBoxTop;
            lblCustomMove.Left = groupBoxColumnLeft;

            NumericUpDown numCustomMove = new NumericUpDown();
            numCustomMove.Name = "num_CustomMove";
            numCustomMove.AutoSize = true;
            numCustomMove.TextAlign = HorizontalAlignment.Center;
            numCustomMove.Minimum = 2;
            numCustomMove.Maximum = 3200;
            numCustomMove.Value = customMove;
            numCustomMove.Top = lblCustomMove.Top - 2;
            numCustomMove.Left = groupBoxColumnRight;
            numCustomMove.Width = 48;
            numCustomMove.ValueChanged += new EventHandler(numCustomMove_ValueChanged);

            groupCustomMove.Controls.Add(lblCustomMove);
            groupCustomMove.Controls.Add(numCustomMove);

            // ========================== Snap-to-Grid GroupBox ========================== //

            GroupBox groupSnapToGrid = new GroupBox();
            groupSnapToGrid.Text = "Snap Pieces to Grid";
            groupSnapToGrid.Top = 130;
            groupSnapToGrid.Left = columnLeft;
            groupSnapToGrid.Width = 280;
            groupSnapToGrid.Height = 50;

            CheckBox checkUseGrid = new CheckBox();
            checkUseGrid.Name = "check_UseGrid";
            checkUseGrid.AutoSize = true;
            checkUseGrid.CheckAlign = System.Drawing.ContentAlignment.MiddleLeft;
            checkUseGrid.Checked = UseGridForPieces;
            checkUseGrid.Text = "Snap-to-Grid amount in pixels:";
            checkUseGrid.Top = groupBoxTop;
            checkUseGrid.Left = groupBoxColumnLeft;
            checkUseGrid.CheckedChanged += new EventHandler(checkUseGrid_CheckedChanged);

            NumericUpDown numGridSize = new NumericUpDown();
            numGridSize.Name = "num_GridSize";
            numGridSize.AutoSize = true;
            numGridSize.TextAlign = HorizontalAlignment.Center;
            numGridSize.Minimum = 1;
            numGridSize.Maximum = 128;
            numGridSize.Value = gridSize;
            numGridSize.Top = checkUseGrid.Top - 2;
            numGridSize.Left = groupBoxColumnRight;
            numGridSize.Width = 48;
            numGridSize.Enabled = UseGridForPieces;
            numGridSize.ValueChanged += new EventHandler(numGridSize_ValueChanged);

            groupSnapToGrid.Controls.Add(checkUseGrid);
            groupSnapToGrid.Controls.Add(numGridSize);

            // =========================== Autosave GroupBox =========================== //

            GroupBox groupAutosave = new GroupBox();
            groupAutosave.Text = "Autosave";
            groupAutosave.Top = 200;
            groupAutosave.Left = columnLeft;
            groupAutosave.Width = 280;
            groupAutosave.Height = 80;

            CheckBox checkAutosave = new CheckBox();
            checkAutosave.Name = "check_Autosave";
            checkAutosave.AutoSize = true;
            checkAutosave.CheckAlign = System.Drawing.ContentAlignment.MiddleLeft;
            checkAutosave.Checked = Autosave;
            checkAutosave.Text = "Autosave level every";
            checkAutosave.Top = groupBoxTop;
            checkAutosave.Left = groupBoxColumnLeft;
            checkAutosave.CheckedChanged += new EventHandler(checkAutosave_CheckedChanged);

            NumericUpDown numAutosaveFrequency = new NumericUpDown();
            numAutosaveFrequency.Name = "num_AutosaveFrequency";
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

            Label lblMinutes = new Label();
            lblMinutes.Text = "minutes";
            lblMinutes.AutoSize = true;
            lblMinutes.Top = groupBoxTop;
            lblMinutes.Left = numAutosaveFrequency.Right + 8;

            CheckBox checkDeleteAutosaves = new CheckBox();
            checkDeleteAutosaves.Name = "check_DeleteAutosaves";
            checkDeleteAutosaves.AutoSize = true;
            checkDeleteAutosaves.CheckAlign = System.Drawing.ContentAlignment.MiddleLeft;
            checkDeleteAutosaves.Checked = RemoveOldAutosaves;
            checkDeleteAutosaves.Enabled = Autosave;
            checkDeleteAutosaves.Text = "Limit autosaves kept to";
            checkDeleteAutosaves.Top = groupBoxTop + 30;
            checkDeleteAutosaves.Left = groupBoxColumnLeft;
            checkDeleteAutosaves.CheckedChanged += new EventHandler(checkDeleteAutosaves_CheckedChanged);

            NumericUpDown numAutosavesToKeep = new NumericUpDown();
            numAutosavesToKeep.Name = "num_AutosavesToKeep";
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

            groupAutosave.Controls.Add(checkAutosave);
            groupAutosave.Controls.Add(numAutosaveFrequency);
            groupAutosave.Controls.Add(lblMinutes);
            groupAutosave.Controls.Add(checkDeleteAutosaves);
            groupAutosave.Controls.Add(numAutosavesToKeep);

            // ========================== Editor Mode GroupBox =========================== //

            GroupBox groupEditorMode = new GroupBox();
            groupEditorMode.Text = "Editor Mode";
            groupEditorMode.Top = 300;
            groupEditorMode.Left = columnLeft;
            groupEditorMode.Width = 280;
            groupEditorMode.Height = 50;

            RadioButton radSuperLemmixMode = new RadioButton();
            radSuperLemmixMode.Name = "rad_SuperLemmixMode";
            radSuperLemmixMode.AutoSize = true;
            radSuperLemmixMode.CheckAlign = System.Drawing.ContentAlignment.MiddleLeft;
            radSuperLemmixMode.Checked = CurrentEditorMode == EditorMode.SuperLemmix;
            radSuperLemmixMode.Text = "SuperLemmix";
            radSuperLemmixMode.Top = groupBoxTop;
            radSuperLemmixMode.Left = groupBoxColumnLeft;
            radSuperLemmixMode.CheckedChanged += new EventHandler(RadioMode_CheckedChanged);

            RadioButton radNeoLemmixMode = new RadioButton();
            radNeoLemmixMode.Name = "rad_NeoLemmixMode";
            radNeoLemmixMode.AutoSize = true;
            radNeoLemmixMode.CheckAlign = System.Drawing.ContentAlignment.MiddleLeft;
            radNeoLemmixMode.Checked = CurrentEditorMode == EditorMode.NeoLemmix;
            radNeoLemmixMode.Text = "NeoLemmix";
            radNeoLemmixMode.Top = groupBoxTop;
            radNeoLemmixMode.Left = groupBoxColumnLeft + radSuperLemmixMode.Width;
            radNeoLemmixMode.CheckedChanged += new EventHandler(RadioMode_CheckedChanged);

            RadioButton radAutoMode = new RadioButton();
            radAutoMode.Name = "rad_AutoMode";
            radAutoMode.AutoSize = true;
            radAutoMode.CheckAlign = System.Drawing.ContentAlignment.MiddleLeft;
            radAutoMode.Checked = CurrentEditorMode == EditorMode.Auto;
            radAutoMode.Text = "Auto";
            radAutoMode.Top = groupBoxTop;
            radAutoMode.Left = groupBoxColumnLeft + radSuperLemmixMode.Width + radNeoLemmixMode.Width;
            radAutoMode.CheckedChanged += new EventHandler(RadioMode_CheckedChanged);

            groupEditorMode.Controls.Add(radSuperLemmixMode);
            groupEditorMode.Controls.Add(radNeoLemmixMode);
            groupEditorMode.Controls.Add(radAutoMode);

            // ========================== Add Controls to Form =========================== //

            settingsForm.Controls.Add(checkPieceNames);
            settingsForm.Controls.Add(groupCustomMove);
            settingsForm.Controls.Add(groupSnapToGrid);
            settingsForm.Controls.Add(groupAutosave);
            settingsForm.Controls.Add(groupEditorMode);

            settingsForm.Show();
        }

        private void settingsForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            WriteSettingsToFile();
        }

        private void RadioMode_CheckedChanged(object sender, EventArgs e)
        {
            if (sender is RadioButton rb && rb.Checked)
            {
                switch (rb.Name)
                {
                    case "rad_NeoLemmixMode":
                        CurrentEditorMode = EditorMode.NeoLemmix;
                        break;
                    case "rad_SuperLemmixMode":
                        CurrentEditorMode = EditorMode.SuperLemmix;
                        break;
                    case "rad_AutoMode":
                        CurrentEditorMode = EditorMode.Auto;
                        break;
                }
                editorForm.ShowMustRestartMessage();
            }
        }


        private void checkPieceNames_CheckedChanged(object sender, EventArgs e)
        {
            UsePieceSelectionNames = ((sender as CheckBox).CheckState == CheckState.Checked);
            editorForm.LoadPiecesIntoPictureBox();
        }

        private void checkUseGrid_CheckedChanged(object sender, EventArgs e)
        {
            UseGridForPieces = ((sender as CheckBox).CheckState == CheckState.Checked);
            settingsForm.Controls.Find("num_GridSize", false)[0].Enabled = UseGridForPieces;
        }

        private void numGridSize_ValueChanged(object sender, EventArgs e)
        {
            gridSize = (int)(sender as NumericUpDown).Value;
        }

        private void numCustomMove_ValueChanged(object sender, EventArgs e)
        {
            customMove = (int)(sender as NumericUpDown).Value;
        }

        private void checkAutosave_CheckedChanged(object sender, EventArgs e)
        {
            Autosave = ((sender as CheckBox).CheckState == CheckState.Checked);
            settingsForm.Controls.Find("num_AutoSaveFrequency", false)[0].Enabled = Autosave;
            settingsForm.Controls.Find("check_DeleteAutosaves", false)[0].Enabled = Autosave;
            settingsForm.Controls.Find("num_AutosavesToKeep", false)[0].Enabled = Autosave && RemoveOldAutosaves;
        }

        private void numAutosaveFrequency_ValueChanged(object sender, EventArgs e)
        {
            autosaveFrequency = (int)(sender as NumericUpDown).Value;
        }

        private void checkDeleteAutosaves_CheckedChanged(object sender, EventArgs e)
        {
            RemoveOldAutosaves = ((sender as CheckBox).CheckState == CheckState.Checked);
            settingsForm.Controls.Find("num_AutosavesToKeep", false)[0].Enabled = Autosave && RemoveOldAutosaves;
        }

        private void numAutosavesToKeep_ValueChanged(object sender, EventArgs e)
        {
            keepAutosaveCount = (int)(sender as NumericUpDown).Value;
        }

        private void radSuperLemmixMode_CheckedChanged(object sender, EventArgs e)
        {
            if ((sender as RadioButton)?.Checked == true)
            {
                CurrentEditorMode = EditorMode.SuperLemmix;
                editorForm.ShowMustRestartMessage();
            }
        }


        private void radNeoLemmixMode_CheckedChanged(object sender, EventArgs e)
        {
            if ((sender as RadioButton)?.Checked == true)
            {
                CurrentEditorMode = EditorMode.NeoLemmix;
                editorForm.ShowMustRestartMessage();
            }
        }

        private void radAutoMode_CheckedChanged(object sender, EventArgs e)
        {
            if ((sender as RadioButton)?.Checked == true)
            {
                CurrentEditorMode = EditorMode.Auto;
                editorForm.ShowMustRestartMessage();
            }
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
                                if (modeText == "NEOLEMMIX")
                                    CurrentEditorMode = EditorMode.NeoLemmix;
                                else if (modeText == "AUTO")
                                    CurrentEditorMode = EditorMode.Auto;
                                else // Default to SuperLemmix Mode
                                    CurrentEditorMode = EditorMode.SuperLemmix;
                                break;
                            }
                        case "PIECESELECTIONNAMES":
                            {
                                UsePieceSelectionNames = (line.Text.Trim().ToUpper() == "TRUE");
                                break;
                            }
                        case "GRIDSIZE":
                            {
                                UseGridForPieces = (line.Value != 1);
                                if (UseGridForPieces)
                                    gridSize = line.Value;
                                break;
                            }
                        case "CUSTOMMOVE":
                            {
                                customMove = line.Value;
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
        }

        /// <summary>
        /// Saves the user's current editor settings to SLXEditorSettings.ini. 
        /// </summary>
        public void WriteSettingsToFile()
        {
            try
            {
                File.Create(C.AppPathSettings).Close();

                TextWriter settingsFile = new StreamWriter(C.AppPathSettings, true);

                settingsFile.WriteLine("# SLXEditor settings ");
                settingsFile.WriteLine(" Autosave            " + AutosaveFrequency.ToString());
                settingsFile.WriteLine(" AutosaveLimit       " + KeepAutosaveCount.ToString());
                settingsFile.WriteLine(" EditorMode          " + CurrentEditorMode.ToString());
                settingsFile.WriteLine(" PieceSelectionNames " + (UsePieceSelectionNames ? "True" : "False"));
                settingsFile.WriteLine(" GridSize            " + GridSize.ToString());
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
