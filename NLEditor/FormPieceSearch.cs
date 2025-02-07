﻿using System;
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
        private Timer searchDelayTimer;
        private readonly string rootPath;

        private List<string> cachedSearchResults = null;

        public FormPieceSearch(string rootPath, Style curStyle)
        {
            InitializeComponent();
            InitializeTimer();

            this.rootPath = rootPath;
            this.curStyle = curStyle;

            check_CurrentStyleOnly.Checked = false;
        }

        private void FilterSearchResults()
        {
            searchDelayTimer.Stop();
            PerformSearch();
        }

        private void SetControlsActive(bool allowUserInput)
        {
            if (allowUserInput)
            {   
                //progressBar.Visible = false;
                lblSearchingStyles.Visible = false;

                this.Cursor = Cursors.Default;
            }
            else
            {
                //progressBar.Visible = true;
                lblSearchingStyles.Visible = true;

                this.Cursor = Cursors.WaitCursor;
            }

            Application.DoEvents();
        }

        private async void PerformSearch()
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

            SetControlsActive(false);

            // If it's the first search, build the cache on a background thread
            if (cachedSearchResults == null)
            {
                cachedSearchResults = await Task.Run(() => SearchForPieces(rootPath));
            }

            string selectedEffectFilter = cbTriggerEffect.Text.Trim();
            bool noTriggerFilter = string.IsNullOrEmpty(cbTriggerEffect.Text) || cbTriggerEffect.Text == "<Any>";

            // Perform filtering on a background thread
            var filteredResults = await Task.Run(() =>
            {
                return cachedSearchResults
                    .Where(result =>
                    {
                        string fileName = Path.GetFileName(result);
                        // Assuming the first folder in the path is the style name
                        string styleName = result.Split('\\')[0];

                        // Text filters (Name & Style)
                        bool textFilterPassed =
                            (string.IsNullOrEmpty(pieceQuery) || fileName.IndexOf(pieceQuery, StringComparison.OrdinalIgnoreCase) >= 0) &&
                            (string.IsNullOrEmpty(styleQuery) || styleName.IndexOf(styleQuery, StringComparison.OrdinalIgnoreCase) >= 0);

                        if (!textFilterPassed)
                            return false;

                        // Additional filters
                        return PiecePassesFilters(result, fileName, selectedEffectFilter, noTriggerFilter);
                    })
                    .ToList();
            });

            // Update the ListBox on the UI thread
            listBoxSearchResults.BeginUpdate();
            listBoxSearchResults.Items.Clear();

            foreach (var result in filteredResults)
            {
                listBoxSearchResults.Items.Add(result);
            }
            listBoxSearchResults.EndUpdate();

            if (listBoxSearchResults.Items.Count <= 0)
            {
                listBoxSearchResults.Items.Add("No results found");
                listBoxSearchResults.Enabled = false;
            }
            else
            {
                listBoxSearchResults.Enabled = true;
            }

            SetControlsActive(true);
        }

        private List<string> SearchForPieces(string rootPath)
        {
            List<string> results = new List<string>();

            // Search all styles
            string searchPath = Path.Combine(rootPath, "styles");

            try
            {
                // Get all PNG files first
                string[] files = Directory.GetFiles(searchPath, "*.png", SearchOption.AllDirectories);

                foreach (string file in files)
                {
                    string fileName = Path.GetFileNameWithoutExtension(file);
                    string relativePath = GetRelativePath(Path.Combine(rootPath, "styles"), file);

                    // Extract style name from the relative path
                    string[] pathParts = relativePath.Split(Path.DirectorySeparatorChar);
                    string styleName = pathParts.Length > 0 ? pathParts[0] : "";

                    results.Add(relativePath.Replace(Path.DirectorySeparatorChar, '\\')
                                            .Replace(Path.GetExtension(relativePath), ""));
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred while searching: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            return results;
        }

        private bool PiecePassesFilters(string relativePath, string fileName, string selectedEffect, bool noTriggerFilter)
        {
            // Skip files not in a relevant subfolder
            if (!IsInRelevantSubfolder(relativePath, new[] { "objects", "terrain" }))
            {
                return false;
            }

            // Handle "Current Style Only" checkbox
            if (check_CurrentStyleOnly.Checked && curStyle != null)
            {
                string styleName = relativePath.Split('\\')[0]; // Extract the style name
                if (!string.Equals(styleName, curStyle.NameInDirectory, StringComparison.OrdinalIgnoreCase))
                {
                    return false;
                }
            }

            string nxmoFilePath = Path.Combine("styles", Path.GetDirectoryName(relativePath), fileName + ".nxmo");
            string nxmtFilePath = Path.Combine("styles", Path.GetDirectoryName(relativePath), fileName + ".nxmt");

            // "Steel" checkbox filter
            if (check_Steel.Checked)
            {
                if (relativePath.IndexOf("\\terrain\\", StringComparison.OrdinalIgnoreCase) < 0 || !File.Exists(nxmtFilePath))
                {
                    return false;
                }

                if (!ProcessNxmtFile(nxmtFilePath))
                {
                    return false;
                }
            }

            // Filters for .nxmo file in "objects" folder
            if (relativePath.IndexOf("\\objects\\", StringComparison.OrdinalIgnoreCase) >= 0)
            {
                if (!File.Exists(nxmoFilePath))
                    return false;
                else
                    return ProcessNxmoFile(nxmoFilePath, selectedEffect);
            }

            // If no filters are active, pass by default
            return noTriggerFilter && !check_CanNineSlice.Checked && !check_CanResize.Checked;
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
        private bool ProcessNxmoFile(string filePath, string selectedEffect)
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
                        case "LOCKEDEXIT": triggerEffect = "EXIT"; break;
                        case "UNLOCKBUTTON": triggerEffect = "BUTTON"; break;
                        case "ONEWAYDOWN":
                        case "ONEWAYUP":
                        case "ONEWAYLEFT":
                        case "ONEWAYRIGHT": triggerEffect = "ONEWAY"; break;
                        case "FORCELEFT":
                        case "FORCERIGHT": triggerEffect = "FORCE"; break;
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

            bool passesTriggerEffectFilter =
                string.IsNullOrEmpty(selectedEffect) ||
                selectedEffect == "<Any>" ||
                selectedEffect.Equals(triggerEffect, StringComparison.OrdinalIgnoreCase);

            bool passesResizeFilter = !check_CanResize.Checked || canResize;
            bool passesNineSliceFilter = !check_CanNineSlice.Checked || canNineSlice;

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
            string[] parts = selectedResult.Split('\\'); // Assuming format: "style\\subfolder\\piece"
            if (parts.Length < 3)
            {
                MessageBox.Show("Invalid result format.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            try
            {
                string newStylePath = parts[0].ToLower(); // Normalize to lowercase
                string newPiece = selectedResult.Replace(".png", "").ToLower(); // Normalize to lowercase

                // Validate newStylePath
                if (string.IsNullOrWhiteSpace(newStylePath))
                {
                    MessageBox.Show("Invalid style path.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                // Validate newPiece
                if (string.IsNullOrWhiteSpace(newPiece))
                {
                    MessageBox.Show("Invalid piece key.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                // Trigger the event to notify NLEditForm
                if (loadStyle) StyleSelected?.Invoke(newStylePath);
                if (addPiece) PieceSelected?.Invoke(newPiece);

                // Update label
                lblCurrentStyle.Text = curStyle?.NameInEditor;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred while processing the selection.\n\n" +
                                $"Selected Result: {selectedResult}\n" +
                                $"Style Path: {parts[0]}\n" +
                                $"Piece Key: {selectedResult.Replace(".png", "")}\n\n" +
                                $"Exception: {ex.Message}\n{ex.StackTrace}",
                                "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void FormPieceSearch_Load(object sender, EventArgs e)
        {
            Rectangle screenRect = Screen.PrimaryScreen.WorkingArea;
            int topPos = screenRect.Bottom - this.Height - 40;
            int leftPos = screenRect.Left + (screenRect.Width - this.Width) / 2;

            this.Location = new Point(leftPos, topPos);

            lblSearchingStyles.Left = btnClearFilters.Left +
                                     (btnClearFilters.Width - lblSearchingStyles.Width) / 2;
        }

        private void ResetUI()
        {
            lblCurrentStyle.Text = $"{curStyle?.NameInEditor}";
            pictureBoxPreview.Image = null;
            lblMetaData.Text = "";

            Application.DoEvents();
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

            FilterSearchResults();
        }

        private void check_CanResize_CheckedChanged(object sender, EventArgs e)
        {
            FilterSearchResults();
        }

        private void check_CanNineSlice_CheckedChanged(object sender, EventArgs e)
        {
            FilterSearchResults();
        }

        private void check_Steel_CheckedChanged(object sender, EventArgs e)
        {
            FilterSearchResults();
        }

        private void cbTriggerEffect_SelectedIndexChanged(object sender, EventArgs e)
        {
            FilterSearchResults();
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            FilterSearchResults();
        }

        private void textBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                FilterSearchResults();
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

            FilterSearchResults();
        }

        private void textBox_MouseDown(object sender, MouseEventArgs e)
        {
            TextBox textBox = sender as TextBox;
            if (textBox != null)
            {
                textBox.SelectAll();
            }
        }

        /// <summary>
        /// Typing into text searches triggers a timer which performs a search after 500 ticks
        /// </summary>
        private void textBox_TextChanged(object sender, EventArgs e)
        {
            // Reset the timer on each keystroke
            searchDelayTimer.Stop();
            searchDelayTimer.Start();
        }

        private void InitializeTimer()
        {
            // Initialize the timer
            searchDelayTimer = new Timer();
            searchDelayTimer.Interval = 500; // Short delay
            searchDelayTimer.Tick += SearchDelayTimer_Tick;
        }

        private void SearchDelayTimer_Tick(object sender, EventArgs e)
        {
            FilterSearchResults();
        }

        private void FormPieceSearch_Click(object sender, EventArgs e)
        {
            listBoxSearchResults.Focus();
        }

        private void FormPieceSearch_Shown(object sender, EventArgs e)
        {
            PerformSearch();
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
