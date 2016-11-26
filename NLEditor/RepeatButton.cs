using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NLEditor
{
    /// <summary>
    /// Button that acts periodically when mouse button is pressed
    /// </summary>
    public class RepeatButton : Button
    {
        public RepeatButton() : base()
        {
            fButtonTimer = new Timer();
            fButtonTimer.Enabled = false;
            fButtonTimer.Interval = 100;
            fButtonTimer.Tick += new EventHandler(delegate(Object obj, EventArgs e) { ButtonTimer_Tick(); });

            this.MouseDown += new MouseEventHandler(delegate(Object obj, MouseEventArgs e) { RepeatButton_MouseDown(e); });
            this.MouseUp += new MouseEventHandler(delegate(Object obj, MouseEventArgs e) { RepeatButton_MouseUp(e); });

            fIsRepeatedAction = false;
        }

        Timer fButtonTimer;
        MouseEventArgs fLastMouseEventArgs;
        bool fIsRepeatedAction;

        /// <summary>
        /// Gets and sets the interval between two repeated actions.
        /// </summary>
        public int Interval 
        { 
            get { return fButtonTimer.Interval; }  
            set { fButtonTimer.Interval = Math.Max(value, 1); }
        }


        /// <summary>
        /// Returns whether a prepeated action was already triggered.
        /// </summary>
        public bool IsRepeatedAction
        {
            get { return fIsRepeatedAction; }
        }

        private void RepeatButton_MouseDown(MouseEventArgs e)
        {
            fLastMouseEventArgs = e;
            fButtonTimer.Enabled = true;
            fIsRepeatedAction = false; // just to be sure
        }

        private void RepeatButton_MouseUp(MouseEventArgs e)
        {
            fButtonTimer.Enabled = false;
            fIsRepeatedAction = false;
        }

        private void ButtonTimer_Tick()
        {
            OnClick(fLastMouseEventArgs);
            fIsRepeatedAction = true;
        }
    }
}
