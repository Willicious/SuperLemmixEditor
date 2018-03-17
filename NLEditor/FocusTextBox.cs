
using System.Windows.Forms;

namespace NLEditor
{
  class FocusTextBox : TextBox
  {
    protected override void WndProc(ref Message m)
    {
      // Trap WM_CUT, WM_COPY and WM_PASTE:
      if (m.Msg.In(0x0300, 0x0301, 0x0302))
      {
        return;
      }
      base.WndProc(ref m);
    }
  }
}
