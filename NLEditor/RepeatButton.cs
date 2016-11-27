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

            fButtonIntervals = new Dictionary<MouseButtons, int>();
            fButtonIntervals.Add(MouseButtons.Left, 100);
            fButtonIntervals.Add(MouseButtons.Right, 100);
            fButtonIntervals.Add(MouseButtons.Middle, 100);
        }

        Timer fButtonTimer;
        MouseEventArgs fLastMouseEventArgs;
        bool fIsRepeatedAction;
        Dictionary<MouseButtons, int> fButtonIntervals;

        /// <summary>
        /// Gets the interval between two repeated actions.
        /// </summary>
        public int Interval 
        { 
            get { return fButtonTimer.Interval; }  
            //set { fButtonTimer.Interval = Math.Max(value, 1); }
        }

        /// <summary>
        /// Sets the interval for a specified button.
        /// </summary>
        /// <param name="Interval"></param>
        /// <param name="Button"></param>
        public void SetInterval(int Interval, MouseButtons Button = MouseButtons.Left)
        {
            if (fButtonIntervals.Keys.Contains(Button))
            {
                fButtonIntervals[Button] = Interval;
            }
            else
            {
                fButtonIntervals.Add(Button, Interval);
            }
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
            if (!fButtonIntervals.Keys.Contains(e.Button)) return;
            
            fLastMouseEventArgs = e;

            fButtonTimer.Interval = fButtonIntervals[e.Button];
            fButtonTimer.Enabled = true;

            if (fButtonTimer.Interval < 200)
            {
                fIsRepeatedAction = false; // just to be sure
            }
            else
            {
                fIsRepeatedAction = false;
                OnClick(fLastMouseEventArgs);
                fIsRepeatedAction = true; // just to be sure
            }
            
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
