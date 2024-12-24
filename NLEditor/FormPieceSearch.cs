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
        public Style curStyle;
        private readonly string rootPath;

        public FormPieceSearch(string rootPath, Style curStyle)
        {
            InitializeComponent();

            this.rootPath = rootPath;
            this.curStyle = curStyle;

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
                                ? Path.Combine(rootPath, "styles", curStyle?.NameInDirectory) // Search only the current style's folder
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
                        continue; // Skip files not matching the chosen style
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
                            // Check for .nxmo file if the piece is in the "objects" folder
                            string nxmoFilePath = Path.Combine(Path.GetDirectoryName(file), fileName + ".nxmo");
                            
                            if (relativePath.IndexOf("\\objects\\", StringComparison.OrdinalIgnoreCase) >= 0)
                            {
                                if (File.Exists(nxmoFilePath)) // Only add files with an associated .nxmo
                                {
                                    addToResults = ProcessNxmoFile(nxmoFilePath);
                                }
                            }
                            else
                            {
                                // Always add to results if no filters are active
                                if ((string.IsNullOrEmpty(cbTriggerEffect.Text) || cbTriggerEffect.Text == "<Any>")
                                    && !check_CanNineSlice.Checked && !check_CanResize.Checked)
                                {
                                    addToResults = true;
                                }
                            }
                        }

                        if (addToResults)
                        {
                            results.Add(relativePath.Replace(Path.DirectorySeparatorChar, '\\')
                                                    .Replace(Path.GetExtension(relativePath), ""));
                        }
                    }

                    // Update progress bar
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

        private void listBoxSearchResults_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listBoxSearchResults.SelectedItem == null)
            {
                pictureBoxPreview.Image = null;
                lblMetaData.Text = "No piece selected.";
                return;
            }

            string selectedResult = listBoxSearchResults.SelectedItem.ToString();

            UpdateMetaDataLabel(selectedResult);
            PreviewPiece(selectedResult, pictureBoxPreview);
        }

        private void UpdateMetaDataLabel(string selectedResult)
        {
            // Assumed format of selectedResult is ("style\\subfolder\\piece")
            string[] parts = selectedResult.Split('\\');

            if (parts.Length < 3)
            {
                pictureBoxPreview.Image = null;
                lblMetaData.Text = "Invalid result format.";
                return;
            }

            string style = parts[0];
            string subfolder = parts[1];
            string piece = parts[2];

            lblMetaData.Text = $"Style: {style}\nSubfolder: {subfolder}\nPiece: {piece}";
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

            string[] lines = File.ReadAllLines(filePath);

            foreach (string line in lines)
            {
                string trimmed = line.Trim();

                if (trimmed.StartsWith("EFFECT ", StringComparison.OrdinalIgnoreCase))
                {
                    triggerEffect = trimmed.Substring(7).Trim();
                    switch (triggerEffect)
                    {
                        case "LOCKEDEXIT":
                            triggerEffect = "EXIT";
                            break;
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

                if (trimmed.StartsWith("NINE_SLICE", StringComparison.OrdinalIgnoreCase))
                {
                    canNineSlice = true;
                }
            }

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

        public void PreviewPiece(string pieceKey, PictureBox previewPictureBox)
        {
            if (string.IsNullOrEmpty(pieceKey))
            {
                previewPictureBox.Image = null;
                lblMetaData.Text = "No piece selected";
                return;
            }

            int frameIndex = (ImageLibrary.GetObjType(pieceKey).In(C.OBJ.PICKUP, C.OBJ.EXIT_LOCKED, C.OBJ.BUTTON, C.OBJ.COLLECTIBLE, C.OBJ.TRAPONCE)) ? 1 : 0;
            Bitmap pieceImage = ImageLibrary.GetImage(pieceKey, RotateFlipType.RotateNoneFlipNone, frameIndex);

            if (pieceKey.StartsWith("default") && ImageLibrary.GetObjType(pieceKey) == C.OBJ.ONE_WAY_WALL)
            {
                Color blendColor = curStyle?.GetColor(C.StyleColor.ONE_WAY_WALL) ?? C.NLColors[C.NLColor.OWWDefault];
                pieceImage = pieceImage.ApplyThemeColor(blendColor);
            }

            ZoomImageWithNearestNeighbor(pieceImage);
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

        private void ZoomImageWithNearestNeighbor(Bitmap originalImage)
        {
            // Add padding to the image before rendering to prevent cropping
            int padding = 1;
            Bitmap paddedImage = new Bitmap(originalImage.Width + padding, originalImage.Height + padding);

            using (Graphics g = Graphics.FromImage(paddedImage))
            {
                g.Clear(Color.Transparent);
                g.DrawImage(originalImage, padding, padding);
            }

            // Get the current size of the PictureBox
            int maxWidth = pictureBoxPreview.Width;
            int maxHeight = pictureBoxPreview.Height;

            // Start with the original image dimensions
            int currentWidth = paddedImage.Width;
            int currentHeight = paddedImage.Height;

            // Keep doubling the image size until it exceeds the PictureBox size
            while (currentWidth * 2 <= maxWidth && currentHeight * 2 <= maxHeight)
            {
                currentWidth *= 2;
                currentHeight *= 2;
            }

            // Create a new Bitmap to hold the scaled image
            Bitmap zoomedImage = new Bitmap(currentWidth, currentHeight);

            // Draw the scaled image with NearestNeighbor interpolation for accuracy
            using (Graphics g = Graphics.FromImage(zoomedImage))
            {
                g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.NearestNeighbor;
                g.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
                g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.None;
                g.DrawImage(paddedImage, 0, 0, currentWidth, currentHeight);
            }

            // Set the PictureBox to display the zoomed image
            pictureBoxPreview.Image = zoomedImage;
        }

        private void btnLoadStyle_Click(object sender, EventArgs e)
        {
            LoadStyleOrAddPiece(true, false);
        }

        private void btnAddPiece_Click(object sender, EventArgs e)
        {
            LoadStyleOrAddPiece(false, true);
        }

        public event Action<string> StyleSelected;
        public event Action<string> PieceSelected;
        private void LoadStyleOrAddPiece(bool loadStyle, bool addPiece)
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
            lblCurrentStyle.Text = curStyle?.NameInEditor;
        }

        private void FormPieceSearch_Load(object sender, EventArgs e)
        {
            Rectangle screenRect = Screen.PrimaryScreen.WorkingArea;
            int topPos = screenRect.Bottom - this.Height - 40;
            int leftPos = screenRect.Left + (screenRect.Width - this.Width) / 2;

            this.Location = new Point(leftPos, topPos);

            ResetUI();
            PerformSearch();
        }

        private void ResetUI()
        {
            lblCurrentStyle.Text = $"{curStyle?.NameInEditor}";
            pictureBoxPreview.Image = null;
            lblMetaData.Text = "";
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

        private void textBox_TextChanged(object sender, EventArgs e)
        {
            PerformSearch();
        }

        private void FormPieceSearch_Click(object sender, EventArgs e)
        {
            listBoxSearchResults.Focus();
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
