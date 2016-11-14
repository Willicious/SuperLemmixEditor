using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NLEditor
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            try
            {
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                Application.Run(new NLEditForm());
            }
            catch (Exception Ex)
            {
                Utility.LogException(Ex);
                MessageBox.Show("Fatal Error:" + Ex.Message);

                Application.Exit();
            }
        }
    }
}
