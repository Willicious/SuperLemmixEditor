/* This class is heavily inspired by 
 * https://stackoverflow.com/questions/571074/how-to-select-all-text-in-winforms-numericupdown-upon-tab-in */

using System;
using System.Windows.Forms;

namespace NLEditor
{
    /// <summary>
    /// A NumericUpDown that selects the whole number every time the user enters the control.
    /// </summary>
    class NumUpDownOverwrite : NumericUpDown
    {
        public NumUpDownOverwrite() : base()
        {
            this.Enter += NumUpDownOverwrite_Enter;
            this.MouseDown += NumUpDownOverwrite_MouseDown;
        }

        bool selectByMouse = false;

        private void NumUpDownOverwrite_Enter(object sender, EventArgs e)
        {
            Select();
            Select(0, Text.Length);
            if (MouseButtons == MouseButtons.Left)
            {
                selectByMouse = true;
            }
        }

        private void NumUpDownOverwrite_MouseDown(object sender, MouseEventArgs e)
        {
            if (selectByMouse)
            {
                Select(0, Text.Length);
                selectByMouse = false;
            }
        }
    }
}
