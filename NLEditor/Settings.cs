using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

        public bool UseLvlPropertiesTabs { get; private set; }
        public bool UsePieceSelectionNames { get; private set; }

        /// <summary>
        /// Resets the editor options to the default values.
        /// </summary>
        public void SetDefault()
        {
            UseLvlPropertiesTabs = true;
            UsePieceSelectionNames = false;
        }

        /// <summary>
        /// Displays the settings form with the settings options.
        /// </summary>
        public void OpenSettingsWindow()
        {
            Form settingsForm = new Form();
            settingsForm.Width = 310;
            settingsForm.Height = 170;
            settingsForm.MaximizeBox = false;
            settingsForm.FormBorderStyle = FormBorderStyle.FixedToolWindow;
            settingsForm.Text = "NLEditor - Settings";
            settingsForm.FormClosing += new FormClosingEventHandler(settingsForm_FormClosing);

            settingsForm.Show();

            CheckBox checkUseTabs = new CheckBox();
            checkUseTabs.AutoSize = true;
            checkUseTabs.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
            checkUseTabs.Checked = UseLvlPropertiesTabs;
            checkUseTabs.Text = "Use tabs to display level proerties: ";
            checkUseTabs.Top = 4;
            checkUseTabs.Left = 160 - checkUseTabs.Width;
            checkUseTabs.CheckedChanged += new EventHandler(checkUseTabs_CheckedChanged);

            settingsForm.Controls.Add(checkUseTabs);
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



        /// <summary>
        /// Reads the users editor settings from NLEditorSettings.ini.
        /// </summary>
        public void ReadSettingsFromFile()
        {
            SetDefault();

            if (!File.Exists(C.AppPathSettings)) return;

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
                    }
                }

                parser.DisposeStreamReader();
            }
            catch (Exception Ex)
            {
                MessageBox.Show("Warning: Could not read editor options from "
                            + Path.GetFileName(C.AppPathSettings) + ". Editor uses the default settings.");
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

                settingsFile.Close();
            }
            catch (Exception Ex)
            {
                Utility.LogException(Ex);
                MessageBox.Show("Error: Could not save settings to " + Path.GetFileName(C.AppPathSettings) + ".");
                return;
            }
        }





    }
}
