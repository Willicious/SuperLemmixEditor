
/*----------------------------------------------------------------
 *                  NeoLemmix Editor
 *   All parts of the source code are licensed under
 *                    CC BY-NC 4.0
 * see: https://creativecommons.org/licenses/by-nc/4.0/legalcode                 
 *----------------------------------------------------------------*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
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
                Application.ThreadException += new System.Threading.ThreadExceptionEventHandler(
                    (object sender, ThreadExceptionEventArgs t) => Utility.HandleGlobalException(t.Exception));
                // Handle all other exceptions
                Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException);
                AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(
                    (object sender, UnhandledExceptionEventArgs e) => Utility.HandleGlobalException((Exception)e.ExceptionObject));

                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                Application.Run(new NLEditForm());
            }
            catch (Exception Ex)
            {
                Utility.HandleGlobalException(Ex);
            }
        }
    }
}
