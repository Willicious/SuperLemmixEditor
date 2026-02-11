using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace SLXEditor
{
    public partial class FormINIExporter : Form
    {
        private INILevelExporter Exporter;

        private Level curLevel;

        private SLXEditForm MainForm;

        private Dictionary<string, INIStyleInfo> LoadedStyles = new Dictionary<string, INIStyleInfo>();

        internal FormINIExporter(Level level, SLXEditForm parentForm)
        {
            InitializeComponent();
            curLevel = level.Clone();
            comboStyles.DisplayMember = "Name";
            MainForm = parentForm;
        }

        private class INIStyleInfo
        {
            public string Name { get; set; }
            public string FolderPath { get; set; }
            public override string ToString() => Name;
        }

        /// <summary>
        /// Returns the number of fully transparent pixels on each side of the image.
        /// </summary>
        public static (int left, int right, int top, int bottom) GetBlankEdges(Bitmap img)
        {
            int width = img.Width;
            int height = img.Height;

            int left = 0;
            int right = 0;
            int top = 0;
            int bottom = 0;

            // Left
            for (int x = 0; x < width; x++)
            {
                bool isBlank = true;
                for (int y = 0; y < height; y++)
                {
                    if (img.GetPixel(x, y).A != 0)
                    {
                        isBlank = false;
                        break;
                    }
                }
                if (!isBlank) break;
                left++;
            }

            // Right
            for (int x = width - 1; x >= 0; x--)
            {
                bool isBlank = true;
                for (int y = 0; y < height; y++)
                {
                    if (img.GetPixel(x, y).A != 0)
                    {
                        isBlank = false;
                        break;
                    }
                }
                if (!isBlank) break;
                right++;
            }

            // Top
            for (int y = 0; y < height; y++)
            {
                bool isBlank = true;
                for (int x = 0; x < width; x++)
                {
                    if (img.GetPixel(x, y).A != 0)
                    {
                        isBlank = false;
                        break;
                    }
                }
                if (!isBlank) break;
                top++;
            }

            // Bottom
            for (int y = height - 1; y >= 0; y--)
            {
                bool isBlank = true;
                for (int x = 0; x < width; x++)
                {
                    if (img.GetPixel(x, y).A != 0)
                    {
                        isBlank = false;
                        break;
                    }
                }
                if (!isBlank) break;
                bottom++;
            }

            return (left, right, top, bottom);
        }

        /// <summary>
        /// Computes transparency offsets to align pieces between .*xlv and .ini
        /// </summary>
        public static (int xo, int yo, int xio, int yio) ComputeOffsets(
            (int left, int right, int top, int bottom) xlvEdges,
            (int left, int right, int top, int bottom) iniEdges,
            int scale = 2)
        {
            int xo = Math.Abs(xlvEdges.left * scale - iniEdges.left);
            int yo = Math.Abs(xlvEdges.top * scale - iniEdges.top);
            int xio = Math.Abs(xlvEdges.right * scale - iniEdges.right);
            int yio = Math.Abs(xlvEdges.bottom * scale - iniEdges.bottom);

            return (xo, yo, xio, yio);
        }

        private void ShowWarnings()
        {
            if (Exporter.LevelContainsGroups)
                MessageBox.Show(
                    "This level contains grouped pieces.\n" +
                    "The grouping may not be fully preserved in the exported level.",
                    "Grouped Pieces Warning",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning
                );

            bool levelContainsResizedPieces =
                curLevel.TerrainList.OfType<TerrainPiece>()
                    .Any(p => p.DefaultWidth > 0 && p.DefaultHeight > 0 &&
                              (p.Width != p.DefaultWidth || p.Height != p.DefaultHeight))
                ||
                curLevel.GadgetList.OfType<GadgetPiece>()
                    .Any(p => p.DefaultWidth > 0 && p.DefaultHeight > 0 &&
                              (p.Width != p.DefaultWidth || p.Height != p.DefaultHeight));


            if (levelContainsResizedPieces)
            {
                MessageBox.Show(
                    "This level contains resized pieces.\n" +
                    "These pieces will be at their original size in the exported level.",
                    "Resized Pieces Warning",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning
                );
            }
        }

        private void ExportLevel(bool toRLV)
        {
            ShowWarnings();

            if (comboStyles.SelectedIndex <= 0)
            {
                MessageBox.Show("Please select a style before exporting.",
                                "No Style Selected",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Warning);
                return;
            }

            string selectedStyle = comboStyles.SelectedItem.ToString();

            string filterText = toRLV ? "RLV files (*.rlv)|*.rlv" : "INI files (*.ini)|*.ini";
            string ext = toRLV ? ".rlv" : ".ini";

            using (var saveDialog = new SaveFileDialog())
            {
                saveDialog.InitialDirectory = C.AppPathLevels;
                saveDialog.Filter = filterText;
                saveDialog.Title = "Export Level";
                saveDialog.FileName = curLevel.Title + ext;

                if (saveDialog.ShowDialog() != DialogResult.OK)
                    return;

                try
                {
                    var result = Exporter.ExportLevelToIni(curLevel, selectedStyle, saveDialog.FileName);

                    if (result.UnlinkedPieces.Count > 0)
                    {
                        MessageBox.Show(
                            $"Warning: Some pieces are not linked:\n\n" +
                            string.Join(Environment.NewLine, result.UnlinkedPieces),
                            "Unlinked Pieces",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Warning);
                    }

                    MessageBox.Show($"Level exported successfully!\n\n{saveDialog.FileName}",
                                    "Export Complete",
                                    MessageBoxButtons.OK,
                                    MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error exporting level: {ex.Message}",
                                    "Export Failed",
                                    MessageBoxButtons.OK,
                                    MessageBoxIcon.Error);
                }
            }
        }

        private void AddTranslationTableStyle()
        {
            string newStyle = Microsoft.VisualBasic.Interaction.InputBox(
                "To add a style to the dropdown list, please type the style name here."
                + Environment.NewLine + Environment.NewLine +
                "Note that the style should be RetroLemmini-compatible.",
                "Add New Style",
                "");

            if (string.IsNullOrWhiteSpace(newStyle))
                return;

            using (FolderBrowserDialog fbd = new FolderBrowserDialog())
            {
                fbd.Description = $"Select folder containing .ini style pieces for {newStyle.Trim()}";
                if (fbd.ShowDialog() != DialogResult.OK)
                    return;

                var styleInfo = new INIStyleInfo { Name = newStyle.Trim(), FolderPath = fbd.SelectedPath };

                // Add to combo and select
                comboStyles.Items.Add(styleInfo);
                comboStyles.SelectedItem = styleInfo;

                // Also add to translation table if necessary
                AddStyleToTranslationTable(styleInfo.Name);
            }
        }

        private void AddStyleToTranslationTable(string styleName)
        {
            // Ensure the file exists
            List<string> lines = File.Exists(C.AppPathTranslationTables)
                ? File.ReadAllLines(C.AppPathTranslationTables).ToList()
                : new List<string> { "[Styles]" };

            // Find [Styles] section
            int stylesIndex = lines.FindIndex(l => l.Trim().Equals("[Styles]", StringComparison.OrdinalIgnoreCase));

            if (stylesIndex == -1)
            {
                // Add [Styles] header if missing
                lines.Insert(0, "[Styles]");
                stylesIndex = 0;
            }

            // Check if the style already exists
            bool exists = lines.Skip(stylesIndex + 1)
                               .Any(l => l.Trim().Equals(styleName, StringComparison.OrdinalIgnoreCase));

            if (!exists)
            {
                // Insert style after the [Styles] header
                lines.Insert(stylesIndex + 1, styleName);
                File.WriteAllLines(C.AppPathTranslationTables, lines);
            }
        }

        private void PopulatePieceListView(string selectedStyle)
        {
            listViewPieceLinks.Items.Clear();

            var links = Exporter.LoadAllPieceLinks();
            var allPieces = curLevel.TerrainList.Cast<object>()
                            .Concat(curLevel.GadgetList.Cast<object>());

            var seenKeys = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

            foreach (dynamic piece in allPieces) // dynamic to handle Terrain/Gadget
            {
                string pieceKey = piece.Key;

                if (seenKeys.Contains(pieceKey))
                    continue;
                seenKeys.Add(pieceKey);

                string iniIdText = string.Empty;
                if (links.TryGetValue(pieceKey, out var link))
                {
                    iniIdText = link.iniId.ToString();

                    // Show style in brackets when it differs from main style
                    if (!link.style.Equals(selectedStyle, StringComparison.OrdinalIgnoreCase))
                        iniIdText += $" ({link.style})";
                }
                else
                    iniIdText = "Not yet linked";

                var item = new ListViewItem(new[] { pieceKey, iniIdText });
                item.Tag = pieceKey;
                listViewPieceLinks.Items.Add(item);
            }
        }

        private void SelectNextUnlinkedPiece(int startIndex)
        {
            if (listViewPieceLinks.Items.Count == 0)
                return;

            listViewPieceLinks.SelectedItems.Clear();

            // Look forward from the next item
            for (int i = startIndex + 1; i < listViewPieceLinks.Items.Count; i++)
            {
                var idText = listViewPieceLinks.Items[i].SubItems[1].Text;

                if (string.IsNullOrWhiteSpace(idText) || idText == "Not yet linked")
                {
                    listViewPieceLinks.Items[i].Selected = true;
                    listViewPieceLinks.Items[i].Focused = true;
                    listViewPieceLinks.EnsureVisible(i);
                    listViewPieceLinks.Focus();
                    return;
                }
            }

            // If not found ahead, wrap around to the beginning
            for (int i = 0; i <= startIndex; i++)
            {
                var idText = listViewPieceLinks.Items[i].SubItems[1].Text;

                if (string.IsNullOrWhiteSpace(idText) || idText == "Not yet linked")
                {
                    listViewPieceLinks.Items[i].Selected = true;
                    listViewPieceLinks.Items[i].Focused = true;
                    listViewPieceLinks.EnsureVisible(i);
                    listViewPieceLinks.Focus();
                    return;
                }
            }
        }

        private void LinkPiece(string pieceKey, string styleName, int iniId, string pieceFolder = null, string iniPiecePath = null)
        {
            if (string.IsNullOrEmpty(pieceKey) || string.IsNullOrEmpty(styleName))
                return;

            if (!LoadedStyles.TryGetValue(styleName, out var styleInfo) || string.IsNullOrEmpty(styleInfo.FolderPath))
            {
                if (!string.IsNullOrEmpty(pieceFolder))
                {
                    styleInfo = new INIStyleInfo
                    {
                        Name = styleName,
                        FolderPath = pieceFolder
                    };
                    LoadedStyles[styleName] = styleInfo;
                }
                else
                {
                    using (FolderBrowserDialog fbd = new FolderBrowserDialog())
                    {
                        fbd.Description = $"Select folder containing .ini pieces for style '{styleName}'";
                        if (fbd.ShowDialog() != DialogResult.OK)
                            return;

                        styleInfo = new INIStyleInfo
                        {
                            Name = styleName,
                            FolderPath = fbd.SelectedPath
                        };
                        LoadedStyles[styleName] = styleInfo;
                    }
                }
            }

            UpdatePieceLink(styleName, pieceKey, iniId, styleInfo.FolderPath);

            if (listViewPieceLinks.SelectedItems.Count > 0)
            {
                var selectedItem = listViewPieceLinks.SelectedItems[0];
                selectedItem.SubItems[1].Text = iniId.ToString();

                // Show style in brackets when it differs from main style
                if (!styleName.Equals(comboStyles.Text, StringComparison.OrdinalIgnoreCase) && !string.IsNullOrEmpty(styleName))
                    selectedItem.SubItems[1].Text += $" ({char.ToUpper(styleName[0]) + styleName.Substring(1)})";

                SelectNextUnlinkedPiece(selectedItem.Index);
            }

            UpdateUnlinkedPieceLabel();
        }

        private string ResolveIniPiecePath(string styleName, string iniFolder, int iniId, bool isGadget)
        {
            string baseName = styleName.ToLowerInvariant();
            var candidates = isGadget
                ? new[] { Path.Combine(iniFolder, $"{baseName}o_{iniId}.png") }
                : new[] { Path.Combine(iniFolder, $"{baseName}_{iniId}.png") };

            return candidates.FirstOrDefault(File.Exists);
        }

        private void UpdatePieceLink(string styleName, string pieceKey, int iniId, string iniFolder)
        {
            if (string.IsNullOrEmpty(iniFolder))
                throw new ArgumentException("INI folder path must be specified.", nameof(iniFolder));

            List<string> lines = File.Exists(C.AppPathTranslationTables)
                ? File.ReadAllLines(C.AppPathTranslationTables).ToList()
                : new List<string> { "[Styles]" };

            // Locate or create style section
            int sectionIndex = lines.FindIndex(l => l.Trim().Equals($"[{styleName}]", StringComparison.OrdinalIgnoreCase));
            if (sectionIndex == -1)
            {
                lines.Add("");
                lines.Add($"[{char.ToUpper(styleName[0]) + styleName.Substring(1)}]");
                sectionIndex = lines.Count - 1;
            }

            int xo = 0, yo = 0, xio = 0, yio = 0;

            try
            {
                bool isGadget = pieceKey.IndexOf($"{styleName}\\objects\\", StringComparison.OrdinalIgnoreCase) >= 0;
                string iniPiecePath = ResolveIniPiecePath(styleName, iniFolder, iniId, isGadget);
                string xlvPath = Path.Combine(C.AppPathPieces, pieceKey + ".png");

                if (!string.IsNullOrEmpty(iniPiecePath) && File.Exists(xlvPath) && File.Exists(iniPiecePath))
                {
                    using (var xlvImg = new Bitmap(xlvPath))
                    using (var iniImg = new Bitmap(iniPiecePath))
                    {
                        var xlvEdges = GetBlankEdges(xlvImg);
                        var iniEdges = GetBlankEdges(iniImg);
                        var offsets = ComputeOffsets(xlvEdges, iniEdges);
                        xo = offsets.xo;
                        yo = offsets.yo;
                        xio = offsets.xio;
                        yio = offsets.yio;
                    }
                }
            }
            catch
            {
                // Ignore errors in offset calculation
            }

            // Format for translation table: pieceKey:iniId(xo#,yo#,xio#,yio#)
            string newLine = $"{pieceKey}:{iniId}(xo{xo},yo{yo},xio{xio},yio{yio})";

            // Replace existing or insert
            int pieceIndex = -1;
            for (int i = sectionIndex + 1; i < lines.Count; i++)
            {
                string line = lines[i].Trim();
                if (line.StartsWith("[") && line.EndsWith("]")) break;
                if (line.StartsWith(pieceKey + ",", StringComparison.OrdinalIgnoreCase) ||
                    line.StartsWith(pieceKey + ":", StringComparison.OrdinalIgnoreCase))
                {
                    pieceIndex = i;
                    break;
                }
            }

            if (pieceIndex >= 0)
                lines[pieceIndex] = newLine;
            else
                lines.Insert(sectionIndex + 1, newLine);

            File.WriteAllLines(C.AppPathTranslationTables, lines);
        }

        private void AddPieceLink()
        {
            if (listViewPieceLinks.SelectedItems.Count == 0) return;

            var selectedItem = listViewPieceLinks.SelectedItems[0];
            string pieceKey = selectedItem.Tag as string;
            if (string.IsNullOrEmpty(pieceKey)) return;

            string styleName = Path.GetFileName(Path.GetDirectoryName(Path.Combine(C.AppPathPieces, pieceKey)));

            string filename = Path.GetFileNameWithoutExtension(pieceKey);
            var idMatch = Regex.Match(filename, @"(\d+)$");
            if (!idMatch.Success)
            {
                MessageBox.Show("Cannot determine ID from piece key.");
                return;
            }

            int iniId = int.Parse(idMatch.Groups[1].Value);

            LinkPiece(pieceKey, styleName, iniId);
        }

        private void BrowseForPieceLink()
        {
            if (listViewPieceLinks.SelectedItems.Count == 0) return;

            var selectedItem = listViewPieceLinks.SelectedItems[0];
            string pieceKey = selectedItem.Tag as string;
            if (string.IsNullOrEmpty(pieceKey)) return;

            using (OpenFileDialog ofd = new OpenFileDialog())
            {
                ofd.Title = "Select corresponding .ini piece file";
                ofd.Filter = "Image files|*.png;*.jpg;*.bmp|All files|*.*";

                if (ofd.ShowDialog() != DialogResult.OK) return;

                string pieceFolder = Path.GetDirectoryName(ofd.FileName);
                string filename = Path.GetFileNameWithoutExtension(ofd.FileName);

                string styleName = Path.GetFileName(pieceFolder);

                var idMatch = Regex.Match(filename, @"(\d+)$");
                if (!idMatch.Success)
                {
                    MessageBox.Show("Cannot determine ID from selected file.", "Error",
                                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                int iniId = int.Parse(idMatch.Groups[1].Value);
                string iniPiecePath = ofd.FileName;

                LinkPiece(pieceKey, styleName, iniId, pieceFolder, iniPiecePath);
            }
        }

        private void UpdateTransparencyOffsets()
        {
            string stylesRootFolder;

            using (FolderBrowserDialog fbd = new FolderBrowserDialog())
            {
                fbd.Description = "Select folder containing INI style folders";
                if (fbd.ShowDialog() != DialogResult.OK)
                    return;

                stylesRootFolder = fbd.SelectedPath;
            }

            var pieceLinks = Exporter.LoadAllPieceLinks();

            var allPieces = curLevel.TerrainList.Cast<object>()
                .Concat(curLevel.GadgetList.Cast<object>());

            foreach (dynamic piece in allPieces)
            {
                string key = piece.Key;

                if (!pieceLinks.TryGetValue(key, out var link))
                {
                    MessageBox.Show(
                        "Could not update transparency offsets.\n" +
                        $"Unlinked piece: {key}"
                    );
                    continue;
                }

                string iniStyle = link.style;
                int iniId = link.iniId;

                // Cache per-style folder lookup
                if (!LoadedStyles.TryGetValue(iniStyle, out var styleInfo))
                {
                    string candidateFolder = Path.Combine(stylesRootFolder, iniStyle);

                    if (!Directory.Exists(candidateFolder))
                    {
                        MessageBox.Show(
                            $"Could not find folder for style '{iniStyle}'.\n" +
                            $"Expected: {candidateFolder}"
                        );
                        continue;
                    }

                    styleInfo = new INIStyleInfo
                    {
                        Name = iniStyle,
                        FolderPath = candidateFolder
                    };

                    LoadedStyles[iniStyle] = styleInfo;
                }

                UpdatePieceLink(iniStyle, key, iniId, styleInfo.FolderPath);
            }
        }

        private void UpdateControls()
        {
            if (comboStyles.SelectedItem == null)
                return;

            if (comboStyles.SelectedIndex != 0)
            {
                lblChosenOutputStyle.Text = comboStyles.Text;
                lblChosenOutputStyle.ForeColor = Color.ForestGreen;
                btnExportToINI.Enabled = true;
                btnExportToRLV.Enabled = true;
                labelFormatHint.Visible = true;
                PopulatePieceListView(comboStyles.SelectedItem.ToString());
            }
            else
            {
                lblChosenOutputStyle.Text = "Please choose a style";
                lblChosenOutputStyle.ForeColor = Color.DarkRed;
                btnExportToINI.Enabled = false;
                btnExportToRLV.Enabled = false;
                labelFormatHint.Visible = false;
                listViewPieceLinks.Items.Clear();
            }

            UpdatePieceListControls();
            UpdateUnlinkedPieceLabel();
        }

        private void UpdatePieceListControls()
        {
            numLinkedPieceID.Enabled = false;
            btnAddLinkedPieceID.Enabled = false;
            btnBrowseForPieceLink.Enabled = false;
            picPiecePreview.Image = null;

            if (listViewPieceLinks.SelectedItems.Count == 1)
            {
                numLinkedPieceID.Enabled = true;
                btnAddLinkedPieceID.Enabled = true;
                btnBrowseForPieceLink.Enabled = true;

                string pieceKey = listViewPieceLinks.SelectedItems[0].Tag as string;
                if (!string.IsNullOrEmpty(pieceKey))
                    PreviewPiece(pieceKey, picPiecePreview);
            }
        }

        private void UpdateUnlinkedPieceLabel()
        {
            if (listViewPieceLinks.Items.Count == 0)
            {
                lblUnlinkedPieces.Visible = false;
                return;
            }

            int unlinkedPieces = 0;

            foreach (ListViewItem item in listViewPieceLinks.Items)
            {
                string idText = item.SubItems[1].Text;
                if (string.IsNullOrWhiteSpace(idText) || idText == "Not yet linked")
                {
                    unlinkedPieces++;
                }
            }

            if (unlinkedPieces > 0)
            {
                string text = unlinkedPieces == 1 ? "1 piece is" : $"{unlinkedPieces} pieces are";
                lblUnlinkedPieces.Text = text + " unlinked. For best results, please link all pieces before exporting";
                lblUnlinkedPieces.ForeColor = Color.DarkRed;
                lblUnlinkedPieces.Visible = true;
                lblTransparencyOffsetHint.Visible = false;
            }
            else
            {
                lblUnlinkedPieces.Text = "All pieces are linked! The level can now be fully exported";
                lblUnlinkedPieces.ForeColor = Color.ForestGreen;
                lblUnlinkedPieces.Visible = true;
                lblTransparencyOffsetHint.Text = 
                    $"HINT: Press F1 if you need to update transparent edge offsets " +
                    $"(recommended if {comboStyles.Text} has been recently updated)";
                lblTransparencyOffsetHint.Visible = true;
            }
        }

        public void PreviewPiece(string pieceKey, PictureBox piecePreview)
        {
            picPiecePreview.Image?.Dispose();

            if (string.IsNullOrEmpty(pieceKey))
            {
                piecePreview.Image = null;
                return;
            }

            int frameIndex = (ImageLibrary.GetObjType(pieceKey).In(C.OBJ.PICKUP, C.OBJ.EXIT_LOCKED, C.OBJ.BUTTON, C.OBJ.COLLECTIBLE, C.OBJ.TRAPONCE)) ? 1 : 0;
            Bitmap pieceImage = ImageLibrary.GetImage(pieceKey, RotateFlipType.RotateNoneFlipNone, frameIndex);

            if (pieceImage == null)
            {
                piecePreview.Image = null;
                return;
            }

            if (pieceKey.StartsWith("default") && ImageLibrary.GetObjType(pieceKey) == C.OBJ.ONE_WAY_WALL)
            {
                Color blendColor = curLevel.MainStyle?.GetColor(C.StyleColor.ONE_WAY_WALL) ?? C.SLXColors[C.SLXColor.OWWDefault];
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
            int maxWidth = picPiecePreview.Width;
            int maxHeight = picPiecePreview.Height;

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
            picPiecePreview.Image = zoomedImage;
        }

        private void FormINIExporter_Load(object sender, EventArgs e)
        {
            Exporter = new INILevelExporter();

            var styles = Exporter.LoadTranslationStyles();
            comboStyles.Items.Clear();
            comboStyles.Items.Add("<Please choose a style>");
            comboStyles.Items.AddRange(styles.ToArray());

            if (comboStyles.Items.Count > 0)
                comboStyles.SelectedIndex = 0;

            UpdateControls();
            Exporter.UngroupAllPiecesForExport(curLevel);
        }

        private void btnAddStyle_Click(object sender, EventArgs e)
        {
            AddTranslationTableStyle();
        }

        private void btnExportToINI_Click(object sender, EventArgs e)
        {
            ExportLevel(false);
        }

        private void btnExportToRLV_Click(object sender, EventArgs e)
        {
            ExportLevel(true);
        }

        private void comboStyles_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateControls();
        }

        private void listViewPieceLinks_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdatePieceListControls();
        }

        private void btnAddLinkedPieceID_Click(object sender, EventArgs e)
        {
            AddPieceLink();
        }

        private void btnBrowseForPieceLink_Click(object sender, EventArgs e)
        {
            BrowseForPieceLink();
        }

        private void FormINIExporter_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.F1) UpdateTransparencyOffsets();
        }

        private void btnOpenBatchExporter_Click(object sender, EventArgs e)
        {
            MainForm.OpenBatchExporter();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}