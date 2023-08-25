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
            int columnRight = 228;

            settingsForm = new EscExitForm();
            settingsForm.StartPosition = FormStartPosition.CenterScreen;
            settingsForm.ClientSize = new System.Drawing.Size(310, 160);
            settingsForm.MaximizeBox = false;
            settingsForm.ShowInTaskbar = false;
            settingsForm.FormBorderStyle = FormBorderStyle.FixedToolWindow;
            settingsForm.Text = "SLXEditor - Settings";
            settingsForm.FormClosing += new FormClosingEventHandler(settingsForm_FormClosing);

            Label lblCustomMove = new Label();
            lblCustomMove.Text = "Custom move amount (Alt + Arrows):";
            lblCustomMove.AutoSize = true;
            lblCustomMove.Top = 8;
            lblCustomMove.Left = columnLeft;

            NumericUpDown numCustomMove = new NumericUpDown();
            numCustomMove.Name = "num_CustomMove";
            numCustomMove.AutoSize = true;
            numCustomMove.TextAlign = HorizontalAlignment.Center;
            numCustomMove.Minimum = 2;
            numCustomMove.Maximum = 3200;
            numCustomMove.Value = customMove;
            numCustomMove.Top = lblCustomMove.Top - 2;
            numCustomMove.Left = columnRight;
            numCustomMove.Width = 47;
            numCustomMove.ValueChanged += new EventHandler(numCustomMove_ValueChanged);

            CheckBox checkUseGrid = new CheckBox();
            checkUseGrid.Name = "check_UseGrid";
            checkUseGrid.AutoSize = true;
            checkUseGrid.CheckAlign = System.Drawing.ContentAlignment.MiddleLeft;
            checkUseGrid.Checked = UseGridForPieces;
            checkUseGrid.Text = "Snap-to-Grid amount:";
            checkUseGrid.Top = 38;
            checkUseGrid.Left = columnLeft;
            checkUseGrid.CheckedChanged += new EventHandler(checkUseGrid_CheckedChanged);

            NumericUpDown numGridSize = new NumericUpDown();
            numGridSize.Name = "num_GridSize";
            numGridSize.AutoSize = true;
            numGridSize.TextAlign = HorizontalAlignment.Center;
            numGridSize.Minimum = 1;
            numGridSize.Maximum = 128;
            numGridSize.Value = gridSize;
            numGridSize.Top = checkUseGrid.Top - 2;
            numGridSize.Left = columnRight;
            numGridSize.Width = 47;
            numGridSize.Enabled = UseGridForPieces;
            numGridSize.ValueChanged += new EventHandler(numGridSize_ValueChanged);

            CheckBox checkAutosave = new CheckBox();
            checkAutosave.Name = "check_Autosave";
            checkAutosave.AutoSize = true;
            checkAutosave.CheckAlign = System.Drawing.ContentAlignment.MiddleLeft;
            checkAutosave.Checked = Autosave;
            checkAutosave.Text = "Autosave (minutes):";
            checkAutosave.Top = 68;
            checkAutosave.Left = columnLeft;
            checkAutosave.CheckedChanged += new EventHandler(checkAutosave_CheckedChanged);

            NumericUpDown numAutosaveFrequency = new NumericUpDown();
            numAutosaveFrequency.Name = "num_AutosaveFrequency";
            numAutosaveFrequency.AutoSize = true;
            numAutosaveFrequency.TextAlign = HorizontalAlignment.Center;
            numAutosaveFrequency.Value = autosaveFrequency;
            numAutosaveFrequency.Minimum = 1;
            numAutosaveFrequency.Maximum = 60;
            numAutosaveFrequency.Top = checkAutosave.Top - 2;
            numAutosaveFrequency.Left = columnRight;
            numAutosaveFrequency.Width = 47;
            numAutosaveFrequency.Enabled = Autosave;
            numAutosaveFrequency.ValueChanged += new EventHandler(numAutosaveFrequency_ValueChanged);

            CheckBox checkDeleteAutosaves = new CheckBox();
            checkDeleteAutosaves.Name = "check_DeleteAutosaves";
            checkDeleteAutosaves.AutoSize = true;
            checkDeleteAutosaves.CheckAlign = System.Drawing.ContentAlignment.MiddleLeft;
            checkDeleteAutosaves.Checked = RemoveOldAutosaves;
            checkDeleteAutosaves.Enabled = Autosave;
            checkDeleteAutosaves.Text = "Limit autosaves kept to:";
            checkDeleteAutosaves.Top = 98;
            checkDeleteAutosaves.Left = columnLeft;
            checkDeleteAutosaves.CheckedChanged += new EventHandler(checkDeleteAutosaves_CheckedChanged);

            NumericUpDown numAutosavesToKeep = new NumericUpDown();
            numAutosavesToKeep.Name = "num_AutosavesToKeep";
            numAutosavesToKeep.AutoSize = true;
            numAutosavesToKeep.TextAlign = HorizontalAlignment.Center;
            numAutosavesToKeep.Value = keepAutosaveCount;
            numAutosavesToKeep.Minimum = 1;
            numAutosavesToKeep.Maximum = 999;
            numAutosavesToKeep.Top = checkDeleteAutosaves.Top - 2;
            numAutosavesToKeep.Left = columnRight;
            numAutosavesToKeep.Width = 47;
            numAutosavesToKeep.Enabled = Autosave && RemoveOldAutosaves;
            numAutosavesToKeep.ValueChanged += new EventHandler(numAutosavesToKeep_ValueChanged);

            CheckBox checkPieceNames = new CheckBox();
            checkPieceNames.Name = "check_PieceNames";
            checkPieceNames.AutoSize = true;
            checkPieceNames.CheckAlign = System.Drawing.ContentAlignment.MiddleLeft;
            checkPieceNames.Checked = UsePieceSelectionNames;
            checkPieceNames.Text = "Display piece names";
            checkPieceNames.Top = 128;
            checkPieceNames.Left = columnLeft;
            checkPieceNames.CheckedChanged += new EventHandler(checkPieceNames_CheckedChanged);

            settingsForm.Controls.Add(checkPieceNames);
            settingsForm.Controls.Add(checkUseGrid);
            settingsForm.Controls.Add(numGridSize);
            settingsForm.Controls.Add(lblCustomMove);
            settingsForm.Controls.Add(numCustomMove);
            settingsForm.Controls.Add(checkAutosave);
            settingsForm.Controls.Add(numAutosaveFrequency);
            settingsForm.Controls.Add(checkDeleteAutosaves);
            settingsForm.Controls.Add(numAutosavesToKeep);

            settingsForm.Show();
        }

        private void settingsForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            WriteSettingsToFile();
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
