using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace NLEditor
{
    public partial class NLEditForm : Form
    {
        public NLEditForm()
        {
            InitializeComponent();

            // Create the list of all styles
            CreateStyleList();
        }

        List<Style> fStyleList;


        public List<Style> StyleList { get { return fStyleList; } }

        private void CreateStyleList()
        { 
            // get list of all existing style names
            List<string> StyleNameList = null;
            
            try 
            {
                StyleNameList = Directory.GetDirectories(C.AppPathPieces)
                                         .Select(dir => Path.GetFileName(dir))
                                         .ToList();
            }
            catch (Exception Ex)
            {
                String ErrorPath = C.AppPath + "ErrorLog.txt";
                TextWriter TextFile = new StreamWriter(ErrorPath, true);
                TextFile.WriteLine(Ex.ToString());
                TextFile.Close();

                MessageBox.Show(Ex.Message);
                Environment.Exit(-1);
            }
            
            StyleNameList.RemoveAll(sty => sty == "default");
            StyleNameList = LoadFromFile.OrderStyleNames(StyleNameList);

            // Add the actual styles
            fStyleList = new List<Style>();
            StyleNameList.ForEach(sty => fStyleList.Add(new Style(sty)));
        }






        private void newToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Create new level
            // TODO
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Exit editor
            Application.Exit();
        }
    }
}
