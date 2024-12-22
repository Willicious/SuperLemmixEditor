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
        private int frameCount;
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

        private void PerformSearch()
        {
            ResetUI();

            string pieceQuery = textBoxPieceName.Text.Trim();
            string styleQuery = textBoxStyleName.Text.Trim();

            if (string.IsNullOrEmpty(pieceQuery) || pieceQuery == "(Any)")
            {
                pieceQuery = ""; // Treat as a wildcard to search for all pieces
            }

            if (string.IsNullOrEmpty(styleQuery) || styleQuery == "(Any)")
            {
                styleQuery = ""; // Treat as a wildcard to search for all pieces
            }

            progressBar.Visible = true;
            List<string> searchResults = SearchForPieces(rootPath, pieceQuery, styleQuery, progressBar);
            progressBar.Visible = false;

            // Populate the list box with the filtered results
            listBoxSearchResults.Items.Clear();
            foreach (var result in searchResults)
            {
                listBoxSearchResults.Items.Add(result);
            }

            if (listBoxSearchResults.Items.Count <= 0)
            {
                if (!string.IsNullOrEmpty(pieceQuery))
                    listBoxSearchResults.Items.Add($"No results found for piece: '{pieceQuery}'");
                else if (!string.IsNullOrEmpty(styleQuery))
                    listBoxSearchResults.Items.Add($"No results found for style: '{styleQuery}'");
                else
                    listBoxSearchResults.Items.Add($"No results found");
            }
        }


        /// <summary>
        /// Recursive search for pieces in the styles directory
        /// </summary>
        /// <param name="rootPath"></param>
        /// <param name="searchQuery"></param>
        /// <param name="searchCurrentStyleOnly"></param>
        private List<string> SearchForPieces(string rootPath, string pieceQuery, string styleQuery, ProgressBar progressBar)
        {
            List<string> results = new List<string>();

            // Determine the base path to search
            string searchPath = check_CurrentStyleOnly.Checked
                                ? Path.Combine(rootPath, "styles", curStylePath) // Search only the current style's folder
                                : Path.Combine(rootPath, "styles"); // Search all styles

            try
            {
                // Get all PNG files first to calculate progress
                string[] files = Directory.GetFiles(searchPath, "*.png", SearchOption.AllDirectories);
                int totalFiles = files.Length;
                int processedFiles = 0;

                // Set the progress bar's maximum value
                progressBar.Maximum = totalFiles;
                progressBar.Value = 0;

                foreach (string file in files)
                {
                    string fileName = Path.GetFileNameWithoutExtension(file);

                    // Get the relative path for adding to results
                    string relativePath = GetRelativePath(Path.Combine(rootPath, "styles"), file);

                    // Extract style name from the relative path
                    string[] pathParts = relativePath.Split(Path.DirectorySeparatorChar);
                    string styleName = pathParts.Length > 0 ? pathParts[0] : "";

                    // Ensure the file is in a relevant subfolder
                    if (!IsInRelevantSubfolder(relativePath, new[] { "objects", "terrain" }))
                    {
                        continue; // Skip files not in relevant subfolders
                    }

                    // Ensure the style matches the search query (or no filtering if empty)
                    if (!string.IsNullOrEmpty(styleQuery) && styleName.IndexOf(styleQuery, StringComparison.OrdinalIgnoreCase) < 0)

                    {
                        continue; // Skip files not matching the style
                    }

                    // Ensure the filename matches the search query (or no filtering if empty)
                    if (string.IsNullOrEmpty(pieceQuery) || fileName.IndexOf(pieceQuery, StringComparison.OrdinalIgnoreCase) >= 0)
                    {
                        bool addToResults = false;

                        // Check for .nxmt file if Steel is checked
                        string nxmtFilePath = Path.Combine(Path.GetDirectoryName(file), fileName + ".nxmt");
                        if (check_Steel.Checked)
                        {
                            // Only add to results if an .nxmt file exists and passes the ProcessNxmtFile check
                            if (File.Exists(nxmtFilePath))
                            {
                                addToResults = ProcessNxmtFile(nxmtFilePath);
                            }
                        }
                        else
                        {
                            // Check for .nxmo file
                            string nxmoFilePath = Path.Combine(Path.GetDirectoryName(file), fileName + ".nxmo");
                            if (File.Exists(nxmoFilePath))
                            {
                                addToResults = ProcessNxmoFile(nxmoFilePath);
                            }

                            // Always add to results if no filters are active
                            if ((string.IsNullOrEmpty(cbTriggerEffect.Text) || cbTriggerEffect.Text == "<Any>")
                                && !check_CanNineSlice.Checked && !check_CanResize.Checked)
                            {
                                addToResults = true;
                            }
                        }

                        // Add the result to the list if applicable
                        if (addToResults)
                        {
                            results.Add(relativePath.Replace(Path.DirectorySeparatorChar, '\\'));
                        }
                    }

                    // Update progress bar (ensure this happens on the UI thread)
                    processedFiles++;
                    progressBar.Invoke((Action)(() => progressBar.Value = processedFiles));
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred while searching: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            return results;
        }

        private bool IsInRelevantSubfolder(string relativePath, string[] relevantSubfolders)
        {
            foreach (var folder in relevantSubfolders)
            {
                if (relativePath.IndexOf($"\\{folder}\\", StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    return true;
                }
            }
            return false;
        }


        /// <summary>
        /// Process an .nxmo file and filter based on checkbox states.
        /// </summary>
        /// <param name="filePath">Path to the .nxmo file</param>
        /// <param name="imageHeight">Height of the associated image</param>
        private bool ProcessNxmoFile(string filePath)
        {
            string triggerEffect = string.Empty;
            bool canResize = false;
            bool canNineSlice = false;
            int frameCount = 0;

            string[] lines = File.ReadAllLines(filePath);
            bool inPrimaryAnimation = false;

            foreach (string line in lines)
            {
                string trimmed = line.Trim();

                if (trimmed.StartsWith("EFFECT ", StringComparison.OrdinalIgnoreCase))
                {
                    triggerEffect = trimmed.Substring(7).Trim();

                    switch (triggerEffect)
                    {
                        case "UNLOCKBUTTON":
                            triggerEffect = "BUTTON";
                            break;
                        case "ONEWAYDOWN":
                        case "ONEWAYUP":
                        case "ONEWAYLEFT":
                        case "ONEWAYRIGHT":
                            triggerEffect = "ONEWAY";
                            break;
                        case "FORCELEFT":
                        case "FORCERIGHT":
                            triggerEffect = "FORCE";
                            break;
                    }
                }

                if (trimmed.StartsWith("RESIZE_", StringComparison.OrdinalIgnoreCase))
                {
                    canResize = true;
                }

                if (trimmed.StartsWith("$PRIMARY_ANIMATION", StringComparison.OrdinalIgnoreCase))
                {
                    inPrimaryAnimation = true;
                }

                if (trimmed.StartsWith("$END", StringComparison.OrdinalIgnoreCase))
                {
                    inPrimaryAnimation = false;
                }

                if (inPrimaryAnimation && trimmed.StartsWith("NINE_SLICE", StringComparison.OrdinalIgnoreCase))
                {
                    canNineSlice = true;
                }

                if (inPrimaryAnimation && trimmed.StartsWith("FRAMES", StringComparison.OrdinalIgnoreCase))
                {
                    // Extract frame count, e.g., "FRAMES 5"
                    if (int.TryParse(trimmed.Substring(7).Trim(), out frameCount))
                    {
                        // Successfully parsed frame count
                    }
                }
            }

            // Store the frame count and triggerEffect for later use
            this.frameCount = frameCount;

            string selectedEffect = cbTriggerEffect.Text.Trim();
            
            bool passesTriggerEffectFilter =
                string.IsNullOrEmpty(cbTriggerEffect.Text) ||
                selectedEffect == "<Any>" ||
                selectedEffect.Equals(triggerEffect, StringComparison.OrdinalIgnoreCase);

            bool passesResizeFilter = !check_CanResize.Checked || canResize;
            bool passesNineSliceFilter = !check_CanNineSlice.Checked || canNineSlice;

            // Combine all filters
            return passesTriggerEffectFilter && passesResizeFilter && passesNineSliceFilter;
        }


        /// <summary>
        /// Process an .nxmt file and filter based on checkbox states.
        /// </summary>
        /// <param name="filePath">Path to the .nxmt file</param>
        /// <returns>True if the file matches the filter criteria</returns>
        private bool ProcessNxmtFile(string filePath)
        {
            string[] lines = File.ReadAllLines(filePath);

            foreach (string line in lines)
            {
                if (line.Trim() == "STEEL")
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Gets the relative path from the base path to the file path.
        /// </summary>
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

                // Call ProcessNxmoFile to extract frame count and image height
                string nxmoFilePath = Path.Combine(Path.GetDirectoryName(piecePath), Path.GetFileNameWithoutExtension(piecePath) + ".nxmo");
                int imageHeight = originalImage.Height;
                int frameCount = 0;

                if (File.Exists(nxmoFilePath))
                {
                    ProcessNxmoFile(nxmoFilePath);
                    frameCount = this.frameCount;
                }

                // If it's an animated image, extract the first frame
                if (frameCount > 1)
                {
                    Bitmap firstFrame = GetFirstFrame(originalImage, frameCount);
                    ZoomImageWithNearestNeighbor(firstFrame);
                }
                else
                {
                    ZoomImageWithNearestNeighbor(originalImage);
                }
            }
            else
            {
                pictureBoxPreview.Image = null;
                lblMetaData.Text += $"\n(Preview not available)";
            }
        }

        private Bitmap GetFirstFrame(Bitmap originalImage, int frameCount)
        {
            // Calculate the height of each individual frame
            int frameHeight = originalImage.Height / frameCount;
            Rectangle firstFrameRect = new Rectangle(0, 0, originalImage.Width, frameHeight);

            // Crop the image to get the first frame
            Bitmap firstFrame = originalImage.Clone(firstFrameRect, originalImage.PixelFormat);
            return firstFrame;
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

        private void ResetUI()
        {
            pictureBoxPreview.Image = null;
            lblMetaData.Text = "Preview";
        }

        private void check_CurrentStyleOnly_CheckedChanged(object sender, EventArgs e)
        {
            if (check_CurrentStyleOnly.Checked)
            {
                lblStyleName.Enabled = false;
                textBoxStyleName.Enabled = false;
            }
            else
            {
                lblStyleName.Enabled = true;
                textBoxStyleName.Enabled = true;
            }

            PerformSearch();
        }

        private void check_CanResize_CheckedChanged(object sender, EventArgs e)
        {
            PerformSearch();
        }

        private void check_CanNineSlice_CheckedChanged(object sender, EventArgs e)
        {
            PerformSearch();
        }

        private void check_Steel_CheckedChanged(object sender, EventArgs e)
        {
            PerformSearch();
        }

        private void cbTriggerEffect_SelectedIndexChanged(object sender, EventArgs e)
        {
            PerformSearch();
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            PerformSearch();
        }

        private void textBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                PerformSearch();
            }
        }

        private void btnClearFilters_Click(object sender, EventArgs e)
        {
            textBoxPieceName.Text = "(Any)";
            textBoxStyleName.Text = "(Any)";

            cbTriggerEffect.Text = "<Any>";

            check_CurrentStyleOnly.Checked = false;
            check_CanResize.Checked = false;
            check_CanNineSlice.Checked = false;
            check_Steel.Checked = false;

            PerformSearch();
        }

        private void textBox_MouseDown(object sender, MouseEventArgs e)
        {
            TextBox textBox = sender as TextBox;
            if (textBox != null)
            {
                textBox.SelectAll();
            }
        }

        private void FormPieceSearch_Click(object sender, EventArgs e)
        {
            btnSearch.Focus();
        }
    }
}
