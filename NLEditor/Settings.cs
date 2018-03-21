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

        public bool UseLvlPropertiesTabs { get; private set; }
        public bool UsePieceSelectionNames { get; private set; }
        public bool UseGridForPieces { get; private set; }
        public int NumTooltipBottonDisplay { get; set; }
        public bool UseTooltipBotton => (NumTooltipBottonDisplay > 0);
        private int gridSize;
        public int GridSize { get { return UseGridForPieces ? gridSize : 1; } }
        public bool IsFormMaximized { get; private set; }
        public System.Drawing.Size FormSize { get; private set; }

        /// <summary>
        /// Resets the editor options to the default values.
        /// </summary>
        public void SetDefault()
        {
            UseLvlPropertiesTabs = true;
            UsePieceSelectionNames = true;
            UseGridForPieces = false;
            gridSize = 8;
            NumTooltipBottonDisplay = 3;
            IsFormMaximized = false;
            FormSize = editorForm.MinimumSize;

            DisplaySettings.SetDisplayed(C.DisplayType.Terrain, true);
            DisplaySettings.SetDisplayed(C.DisplayType.Objects, true);
            DisplaySettings.SetDisplayed(C.DisplayType.Background, true);
            DisplaySettings.SetDisplayed(C.DisplayType.ScreenStart, false);
            DisplaySettings.SetDisplayed(C.DisplayType.Trigger, false);
            DisplaySettings.SetDisplayed(C.DisplayType.ClearPhysics, false);
        }

        /// <summary>
        /// Displays the settings form with the settings options.
        /// </summary>
        public void OpenSettingsWindow()
        {
            int leftPos = 30;

            settingsForm = new Form();
            settingsForm.ClientSize = new System.Drawing.Size(310, 160);
            settingsForm.MaximizeBox = false;
            settingsForm.ShowInTaskbar = false;
            settingsForm.FormBorderStyle = FormBorderStyle.FixedToolWindow;
            settingsForm.Text = "NLEditor - Settings";
            settingsForm.FormClosing += new FormClosingEventHandler(settingsForm_FormClosing);

            CheckBox checkUseTabs = new CheckBox();
            checkUseTabs.Name = "check_UseTabs";
            checkUseTabs.AutoSize = true;
            checkUseTabs.CheckAlign = System.Drawing.ContentAlignment.MiddleLeft;
            checkUseTabs.Checked = UseLvlPropertiesTabs;
            checkUseTabs.Text = "Use tabs to display level properties";
            checkUseTabs.Top = 8;
            checkUseTabs.Left = leftPos;
            checkUseTabs.CheckedChanged += new EventHandler(checkUseTabs_CheckedChanged);

            CheckBox checkPieceNames = new CheckBox();
            checkPieceNames.Name = "check_PieceNames";
            checkPieceNames.AutoSize = true;
            checkPieceNames.CheckAlign = System.Drawing.ContentAlignment.MiddleLeft;
            checkPieceNames.Checked = UsePieceSelectionNames;
            checkPieceNames.Text = "Display piece names";
            checkPieceNames.Top = 38;
            checkPieceNames.Left = leftPos;
            checkPieceNames.CheckedChanged += new EventHandler(checkPieceNames_CheckedChanged);

            CheckBox checkUseGrid = new CheckBox();
            checkUseGrid.Name = "check_UseGrid";
            checkUseGrid.AutoSize = true;
            checkUseGrid.CheckAlign = System.Drawing.ContentAlignment.MiddleLeft;
            checkUseGrid.Checked = UseGridForPieces;
            checkUseGrid.Text = "Use grid for pieces of size:";
            checkUseGrid.Top = 68;
            checkUseGrid.Left = leftPos;
            checkUseGrid.CheckedChanged += new EventHandler(checkUseGrid_CheckedChanged);

            NumericUpDown numGridSize = new NumericUpDown();
            numGridSize.Name = "num_GridSize";
            numGridSize.AutoSize = true;
            numGridSize.TextAlign = HorizontalAlignment.Center;
            numGridSize.Value = gridSize;
            numGridSize.Minimum = 1;
            numGridSize.Maximum = 32;
            numGridSize.Top = checkUseGrid.Top - 2;
            numGridSize.Left = checkUseGrid.Right + 50;
            numGridSize.Width = 47;
            numGridSize.Enabled = UseGridForPieces;
            numGridSize.ValueChanged += new EventHandler(numGridSize_ValueChanged);

            settingsForm.Controls.Add(checkUseTabs);
            settingsForm.Controls.Add(checkPieceNames);
            settingsForm.Controls.Add(checkUseGrid);
            settingsForm.Controls.Add(numGridSize);

            settingsForm.Show();
        }

        private void settingsForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            WriteSettingsToFile();
        }

        private void checkUseTabs_CheckedChanged(object sender, EventArgs e)
        {
            UseLvlPropertiesTabs = ((sender as CheckBox).CheckState == CheckState.Checked);
            editorForm.ApplyOptionLvlPropertiesTabs();
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
        /// Reads the users editor settings from NLEditorSettings.ini.
        /// </summary>
        public void ReadSettingsFromFile()
        {
            SetDefault();

            if (!File.Exists(C.AppPathSettings)) return;

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
                        case "LVLPROPERTIESTABS":
                            {
                                UseLvlPropertiesTabs = (line.Text.Trim().ToUpper() == "TRUE");
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
                                if (UseGridForPieces) gridSize = line.Value;
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
        /// Saves the user's current editor settings to NLEditorSettings.ini. 
        /// </summary>
        public void WriteSettingsToFile()
        {
            try
            {
                File.Create(C.AppPathSettings).Close();

                TextWriter settingsFile = new StreamWriter(C.AppPathSettings, true);

                settingsFile.WriteLine("# NLEditor settings ");
                settingsFile.WriteLine(" LvlPropertiesTabs   " + (UseLvlPropertiesTabs ? "True" : "False"));
                settingsFile.WriteLine(" PieceSelectionNames " + (UsePieceSelectionNames ? "True" : "False"));
                settingsFile.WriteLine(" GridSize            " + GridSize.ToString());
                settingsFile.WriteLine(" Button_Tooltip      " + NumTooltipBottonDisplay.ToString());
                settingsFile.WriteLine("");
                settingsFile.WriteLine(" Form_Maximized      " + (IsFormMaximized ? "True" : "False"));
                settingsFile.WriteLine(" Form_Width          " + FormSize.Width.ToString());
                settingsFile.WriteLine(" Form_Height         " + FormSize.Height.ToString());
                settingsFile.WriteLine("");

                var displayTypes = new List<C.DisplayType>()
                {
                    C.DisplayType.Trigger, C.DisplayType.ScreenStart, C.DisplayType.Background
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
