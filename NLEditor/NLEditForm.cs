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

            // Set list of all piectures for single pieces
            PictureBox[] PicBoxArr = { this.picPiece1, this.picPiece2, this.picPiece3, this.picPiece4,
                                       this.picPiece5, this.picPiece6, this.picPiece7, this.picPiece8 };
            fpicPieceList = new List<PictureBox>(PicBoxArr);

            // Create the list of all styles
            CreateStyleList();
            if (StyleList.Count > 0)
            {
                this.combo_MainStyle.Items.AddRange(StyleList.ConvertAll(sty => sty.Name).ToArray());
                this.combo_MainStyle.SelectedIndex = 0;

                this.combo_PieceStyle.Items.AddRange(StyleList.ConvertAll(sty => sty.Name).ToArray());
                this.combo_PieceStyle.SelectedIndex = 0;
            }

            // Load pieces into the picPieces
            fPieceStartIndex = 0;
            fPieceDoDisplayObject = false;
            UpdateForm.LoadPiecesIntoPictureBox(this, this.combo_PieceStyle.SelectedItem.ToString());
        }

        List<PictureBox> fpicPieceList;

        List<Style> fStyleList;
        int fPieceStartIndex;
        bool fPieceDoDisplayObject;

        public List<PictureBox> picPieceList { get { return fpicPieceList; } }
        public List<Style> StyleList { get { return fStyleList; } }
        public int PieceStartIndex { get { return fPieceStartIndex; } }
        public bool PieceDoDisplayObject { get { return fPieceDoDisplayObject; } }


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
