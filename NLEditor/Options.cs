using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Windows.Forms;

namespace NLEditor
{
    class Options
    {
        public Options(NLEditForm editorForm)
        {
            this.editorForm = editorForm;
            SetDefault();
        }

        NLEditForm editorForm;

        public bool UseLvlPropertiesTabs { get; private set; }
        public bool UsePieceSelectionNames { get; private set; }

        public void SetDefault()
        {
            UseLvlPropertiesTabs = true;
            UsePieceSelectionNames = false;
        }

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
