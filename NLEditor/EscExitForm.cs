using System.Windows.Forms;

namespace NLEditor
{
  class EscExitForm : Form
  {
    public EscExitForm()
        : base()
    {
      KeyDown += new KeyEventHandler(EscExitForm_KeyDown);
      KeyPreview = true;
    }

    private void EscExitForm_KeyDown(object sender, KeyEventArgs e)
    {
      if (e.KeyCode == Keys.Escape)
      {
        Close();
        e.Handled = true;
      }
      else
      {
        e.Handled = false;
      }
    }
  }
}
