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
        private int gridSize;
        public int GridSize { get { return UseGridForPieces ? gridSize : 1; } }

        /// <summary>
        /// Resets the editor options to the default values.
        /// </summary>
        public void SetDefault()
        {
            UseLvlPropertiesTabs = true;
            UsePieceSelectionNames = false;
            UseGridForPieces = false;
            gridSize = 8;
        }

        /// <summary>
        /// Displays the settings form with the settings options.
        /// </summary>
        public void OpenSettingsWindow()
        {
            int leftPos = 30;

            settingsForm = new Form();
            settingsForm.Width = 310;
            settingsForm.Height = 170;
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
        /// Reads the users editor settings from NLEditorSettings.ini.
        /// </summary>
        public HashSet<string> ReadSettingsFromFile()
        {
            SetDefault();
            var displaySettings = new HashSet<string>();

            if (!File.Exists(C.AppPathSettings)) return displaySettings;

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
                                if (line.Text.Trim().ToUpper() == "TRUE") UseLvlPropertiesTabs = true;
                                else if (line.Text.Trim().ToUpper() == "FALSE") UseLvlPropertiesTabs = false;
                                break;
                            }
                        case "PIECESELECTIONNAMES":
                            {
                                if (line.Text.Trim().ToUpper() == "TRUE") UsePieceSelectionNames = true;
                                else if (line.Text.Trim().ToUpper() == "FALSE") UsePieceSelectionNames = false;
                                break;
                            }
                        case "GRIDSIZE":
                            {
                                UseGridForPieces = (line.Value != 1);
                                if (UseGridForPieces) gridSize = line.Value;
                                break;
                            }
                        case "DISPLAY":
                            {
                                displaySettings.Add(line.Text.Trim());
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

            return displaySettings;
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
                settingsFile.WriteLine("");
                foreach (string displaySetting in editorForm.GetDisplaySettings())
                {
                    settingsFile.WriteLine(" Display             " + displaySetting);
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
