
/*----------------------------------------------------------------
 *                  NeoLemmix Editor
 *   All parts of the source code are licensed under
 *                    CC BY-NC 4.0
 * see: https://creativecommons.org/licenses/by-nc/4.0/legalcode                 
 *----------------------------------------------------------------*/
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
                // Handle exceptions of the UI
                Application.ThreadException += 
                    new System.Threading.ThreadExceptionEventHandler(NLEditForm.ExceptionHandler);
                // Handle all other exceptions
                Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException);
                AppDomain.CurrentDomain.UnhandledException +=
                    new UnhandledExceptionEventHandler(UnhandledExceptionHandler);

                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                Application.Run(new NLEditForm());
            }
            catch (Exception Ex)
            {
                try
                {
                    Utility.LogException(Ex);
                    MessageBox.Show("Klopt niet: " + Ex.Message + C.NewLine + "Editor will quit now.", "Fatal error");
                }
                finally
                {
                    Application.Exit();
                }
            }
        }

        /// <summary>
        /// This handles exceptions not coming from the UI.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void UnhandledExceptionHandler(object sender, UnhandledExceptionEventArgs e)
        {
            try
            {
                Exception Ex = (Exception)e.ExceptionObject;
                Utility.LogException(Ex);
                MessageBox.Show("Klopt niet: " + Ex.Message + C.NewLine + "Editor will quit now.", "Fatal error");
            }
            finally
            {
                Application.Exit();
            }
        }
    }
}
