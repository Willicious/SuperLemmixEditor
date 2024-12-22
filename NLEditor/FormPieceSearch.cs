using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NLEditor
{
    public partial class FormPieceSearch : Form
    {
        private readonly string rootPath;
        private readonly string curStylePath;
        public string curStyleName;

        public FormPieceSearch(string rootPath, string curStyleName, string curStylePath)
        {
            InitializeComponent();

            this.rootPath = rootPath;
            this.curStyleName = curStyleName;
            this.curStylePath = curStylePath;

            lblCurrentStyle.Text = $"{curStyleName}";
            check_CurrentStyleOnly.Checked = false;
        }

        private void textBoxSearch_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                BeginSearch();
            }
        }

        private void BeginSearch()
        {
            string searchQuery = textBoxSearch.Text.Trim();
            if (string.IsNullOrEmpty(searchQuery))
            {
                MessageBox.Show("Please enter a search term.", "Invalid Input", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            List<string> searchResults = SearchForPieces(rootPath, searchQuery);

            listBoxSearchResults.Items.Clear();
            foreach (var result in searchResults)
            {
                listBoxSearchResults.Items.Add(result);
            }
        }


        /// <summary>
        /// Recursive search for pieces in the styles directory
        /// </summary>
        /// <param name="rootPath"></param>
        /// <param name="searchQuery"></param>
        /// <param name="searchCurrentStyleOnly"></param>
        private List<string> SearchForPieces(string rootPath, string searchQuery)
        {
            List<string> results = new List<string>();

            // Determine the base path to search
            string searchPath = check_CurrentStyleOnly.Checked
                                ? Path.Combine(rootPath, "styles", curStylePath) // Search only the current style's folder
                                : Path.Combine(rootPath, "styles"); // Search all styles

            try
            {
                // Perform search within the determined path
                foreach (string file in Directory.GetFiles(searchPath, "*.png", SearchOption.AllDirectories))
                {
                    string fileName = Path.GetFileNameWithoutExtension(file);

                    // Only process files in "terrain", "objects", or "backgrounds" subfolders
                    string relativePath = GetRelativePath(Path.Combine(rootPath, "styles"), file);
                    if (!IsInRelevantSubfolder(relativePath))
                        continue;

                    if (fileName.IndexOf(searchQuery, StringComparison.OrdinalIgnoreCase) >= 0)
                    {
                        // Format result as "style_name\subfolder_name\piece_name"
                        results.Add(relativePath.Replace(Path.DirectorySeparatorChar, '\\'));
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred while searching: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            return results;
        }

        private bool IsInRelevantSubfolder(string relativePath)
        {
            string lowerPath = relativePath.ToLower();
            return lowerPath.Contains("\\terrain\\") || lowerPath.Contains("\\objects\\") || lowerPath.Contains("\\backgrounds\\");
        }

        private string GetRelativePath(string basePath, string filePath)
        {
            Uri baseUri = new Uri(basePath.EndsWith("\\") ? basePath : basePath + "\\");
            Uri fileUri = new Uri(filePath);
            return Uri.UnescapeDataString(baseUri.MakeRelativeUri(fileUri).ToString().Replace('/', Path.DirectorySeparatorChar));
        }

        private void listBoxSearchResults_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listBoxSearchResults.SelectedItem == null)
            {
                pictureBoxPreview.Image = null;
                lblMetaData.Text = "No piece selected.";
                return;
            }

            string selectedResult = listBoxSearchResults.SelectedItem.ToString();
            string[] parts = selectedResult.Split('\\'); // Assuming the result format is: "style\\subfolder\\piece"
            if (parts.Length < 3)
            {
                pictureBoxPreview.Image = null;
                lblMetaData.Text = "Invalid result format.";
                return;
            }

            string style = parts[0];
            string subfolder = parts[1];
            string pieceKey = parts[2];

            // Update metadata
            lblMetaData.Text = $"Style: {style}\nSubfolder: {subfolder}\nPiece: {pieceKey}";

            // Load preview image
            string piecePath = Path.Combine(rootPath, "styles", style, subfolder, pieceKey);

            if (File.Exists(piecePath))
            {
                Bitmap originalImage = new Bitmap(piecePath);
                ZoomImageWithNearestNeighbor(originalImage);
            }
            else
            {
                pictureBoxPreview.Image = null;
                lblMetaData.Text += $"\n(Preview not available)";
            }
        }

        private void ZoomImageWithNearestNeighbor(Bitmap originalImage)
        {
            // Get the current size of the PictureBox
            int maxWidth = pictureBoxPreview.Width;
            int maxHeight = pictureBoxPreview.Height;

            // Start with the original image dimensions
            int currentWidth = originalImage.Width;
            int currentHeight = originalImage.Height;

            // Keep doubling the image size until it exceeds the PictureBox size
            while (currentWidth * 2 <= maxWidth && currentHeight * 2 <= maxHeight)
            {
                currentWidth *= 2;
                currentHeight *= 2;
            }

            // Create a new Bitmap to hold the scaled image
            Bitmap zoomedImage = new Bitmap(currentWidth, currentHeight);

            // Set up the graphics object with NearestNeighbor interpolation
            using (Graphics g = Graphics.FromImage(zoomedImage))
            {
                g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.NearestNeighbor;
                g.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
                g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.None;

                // Draw the original image scaled to the new size
                g.DrawImage(originalImage, 0, 0, currentWidth, currentHeight);
            }

            // Set the PictureBox (or your desired container) to display the zoomed image
            pictureBoxPreview.Image = zoomedImage;
        }

        private void check_CurrentStyleOnly_CheckedChanged(object sender, EventArgs e)
        {
            string searchQuery = textBoxSearch.Text.Trim();
            if (!string.IsNullOrEmpty(searchQuery))
            {
                // Re-run the search when the checkbox state changes
                List<string> searchResults = SearchForPieces(rootPath, searchQuery);
                listBoxSearchResults.Items.Clear();
                foreach (var result in searchResults)
                {
                    listBoxSearchResults.Items.Add(result);
                }
            }
        }

        
        private void btnLoadStyle_Click(object sender, EventArgs e)
        {
            AddPieceAndOrLoadStyle(true, false);
        }
        
        private void btnAddPieceAndLoadStyle_Click(object sender, EventArgs e)
        {
            AddPieceAndOrLoadStyle(true, true);
        }

        private void btnAddPiece_Click(object sender, EventArgs e)
        {
            AddPieceAndOrLoadStyle(false, true);
        }

        public event Action<string> StyleSelected;
        public event Action<string> PieceSelected;
        private void AddPieceAndOrLoadStyle(bool loadStyle, bool addPiece)
        {
            if (listBoxSearchResults.SelectedItem == null)
            {
                MessageBox.Show("Please select a piece first.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            string selectedResult = listBoxSearchResults.SelectedItem.ToString();
            string[] parts = selectedResult.Split('\\'); // Assuming the result format is: "style\\subfolder\\piece"
            if (parts.Length < 3)
            {
                MessageBox.Show("Invalid result format.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            string newStylePath = parts[0]; // Extract style from path
            string newPiece = selectedResult.Replace(".png", ""); // Use full path (minus extension) for pieces

            // Trigger the event to notify NLEditForm
            if (loadStyle) StyleSelected?.Invoke(newStylePath);
            if (addPiece)  PieceSelected?.Invoke(newPiece);

            // Update label
            lblCurrentStyle.Text = curStyleName;

            // Close the search form if adding a piece
            if (addPiece) Close();
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void FormPieceSearch_Load(object sender, EventArgs e)
        {
            Rectangle screenRect = Screen.PrimaryScreen.WorkingArea;
            int topPos = screenRect.Bottom - this.Height - 40;
            int leftPos = screenRect.Left + (screenRect.Width - this.Width) / 2;

            this.Location = new Point(leftPos, topPos);
        }
    }
}
