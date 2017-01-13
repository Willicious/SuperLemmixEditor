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
            buttonTimer = new Timer();
            buttonTimer.Enabled = false;
            buttonTimer.Tick += new EventHandler(delegate(Object obj, EventArgs e) { ButtonTimer_Tick(); });

            this.MouseDown += new MouseEventHandler(delegate(Object obj, MouseEventArgs e) { RepeatButton_MouseDown(e); });
            this.MouseUp += new MouseEventHandler(delegate(Object obj, MouseEventArgs e) { RepeatButton_MouseUp(e); });

            IsRepeatedAction = false;

            buttonIntervals = new Dictionary<MouseButtons, int>();
            buttonIntervals.Add(MouseButtons.Left, 50);
            buttonIntervals.Add(MouseButtons.Right, 50);
            buttonIntervals.Add(MouseButtons.Middle, 50);
        }

        Timer buttonTimer;
        MouseEventArgs lastMouseEventArgs;
        Dictionary<MouseButtons, int> buttonIntervals;

        public bool IsRepeatedAction { get; private set; }

        /// <summary>
        /// Gets the interval between two repeated actions.
        /// </summary>
        public int Interval(MouseButtons button = MouseButtons.Left)
        {
            return buttonIntervals?[button] ?? 100; 
        }
        

        /// <summary>
        /// Sets the interval for a specified button.
        /// </summary>
        /// <param name="interval"></param>
        /// <param name="button"></param>
        public void SetInterval(int interval, MouseButtons button = MouseButtons.Left)
        {
            if (buttonIntervals.Keys.Contains(button))
            {
                buttonIntervals[button] = interval;
            }
            else
            {
                buttonIntervals.Add(button, interval);
            }
        }
          

        private void RepeatButton_MouseDown(MouseEventArgs e)
        {
            if (!buttonIntervals.Keys.Contains(e.Button)) return;
            
            lastMouseEventArgs = e;

            buttonTimer.Interval = buttonIntervals[e.Button];
            buttonTimer.Enabled = true;

            if (buttonTimer.Interval < 200)
            {
                IsRepeatedAction = false; // just to be sure
            }
            else
            {
                IsRepeatedAction = false;
                OnClick(lastMouseEventArgs);
                IsRepeatedAction = true; // just to be sure
            }
            
        }

        private void RepeatButton_MouseUp(MouseEventArgs e)
        {
            buttonTimer.Enabled = false;
            IsRepeatedAction = false;
        }

        private void ButtonTimer_Tick()
        {
            OnClick(lastMouseEventArgs);
            IsRepeatedAction = true;
        }
    }
}
