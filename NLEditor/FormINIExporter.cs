using Microsoft.SqlServer.Server;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using static NLEditor.C;

namespace NLEditor
{
    public partial class FormINIExporter : Form
    {
        private Level curLevel;

        internal FormINIExporter(Level level)
        {
            InitializeComponent();
            this.curLevel = level;
        }

        // ───────────────────────────────────────────────
        // LOCAL CLASS: Only FormINIExporter uses this
        // ───────────────────────────────────────────────
        private class IniLevel
        {
            public string Name { get; set; }
            public string Author { get; set; }
            public int ReleaseRate { get; set; }
            public int NumLemmings { get; set; }
            public int NumToRescue { get; set; }
            public int TimeLimitSeconds { get; set; }
            public int NumClimbers { get; set; }
            public int NumFloaters { get; set; }
            public int NumBombers { get; set; }
            public int NumBlockers { get; set; }
            public int NumBuilders { get; set; }
            public int NumBashers { get; set; }
            public int NumMiners { get; set; }
            public int NumDiggers { get; set; }
            public int XPosCenter { get; set; }
            public int YPosCenter { get; set; }
            public string Style { get; set; }
            public int MaxFallDistance { get; set; }
            public int AutosteelMode { get; set; }
            public int Width { get; set; }
            public int Height { get; set; }
            public int TopBoundary { get; set; }
            public int BottomBoundary { get; set; }
            public int LeftBoundary { get; set; }
            public int RightBoundary { get; set; }
        }

        // ───────────────────────────────────────────────
        // CONVERTER: Creates an IniLevel from the .nxlv data
        // ───────────────────────────────────────────────
        private IniLevel ConvertToIni(Level level, string selectedStyle)
        {
            var ini = new IniLevel();

            // Title & Author
            ini.Name = level.Title;
            ini.Author = level.Author;

            // Lem count, time limit and release rate
            ini.NumLemmings = level.NumLems;
            ini.NumToRescue = level.SaveReq;
            ini.TimeLimitSeconds = level.TimeLimit;
            ini.ReleaseRate = level.ReleaseRate;

            // Skills
            ini.NumClimbers = GetSkill(level, Skill.Climber);
            ini.NumFloaters = GetSkill(level, Skill.Floater);
            ini.NumBombers = GetSkill(level, Skill.Timebomber)
                           + GetSkill(level, Skill.Bomber);
            ini.NumBlockers = GetSkill(level, Skill.Blocker);
            ini.NumBuilders = GetSkill(level, Skill.Builder);
            ini.NumBashers = GetSkill(level, Skill.Basher);
            ini.NumMiners = GetSkill(level, Skill.Miner);
            ini.NumDiggers = GetSkill(level, Skill.Digger);

            // Dimensions and screen pos are multiplied by 2
            ini.Width = level.Width * 2;
            ini.Height = level.Height * 2;
            ini.XPosCenter = level.StartPosX * 2;
            ini.YPosCenter = level.StartPosY * 2;

            // Style - Default to Crystal if none is selected
            ini.Style = string.IsNullOrWhiteSpace(selectedStyle) ? "Crystal" : selectedStyle;

            // Default values for these
            ini.MaxFallDistance = 126;
            ini.AutosteelMode = 2;

            // Boundary - Default values for these
            ini.TopBoundary = 8;
            ini.BottomBoundary = 20;
            ini.LeftBoundary = 0;
            ini.RightBoundary = -16;

            return ini;
        }

        private int GetSkill(Level level, Skill skill)
        {
            if (level.SkillSet != null && level.SkillSet.ContainsKey(skill))
                return level.SkillSet[skill];

            return 0;
        }

        // ───────────────────────────────────────────────
        // WRITER: Outputs a RetroLemmini-compatible .ini file
        // ───────────────────────────────────────────────
        private void WriteIniFile(IniLevel ini, string filePath, List<string> terrainLines, List<string> objectLines)
        {
            var sb = new StringBuilder();

            // Add level stats
            sb.AppendLine("# ---------------------------------------");
            sb.AppendLine("# RetroLemmini Level");
            sb.AppendLine($"# Exported from SuperLemmix Editor Version {C.Version}");
            sb.AppendLine("# ---------------------------------------");
            sb.AppendLine();
            sb.AppendLine("# Level stats");
            sb.AppendLine($"name = {Escape(ini.Name)}");
            sb.AppendLine($"author = {Escape(ini.Author)}");
            sb.AppendLine($"releaseRate = {ini.ReleaseRate}");
            sb.AppendLine($"numLemmings = {ini.NumLemmings}");
            sb.AppendLine($"numToRescue = {ini.NumToRescue}");
            sb.AppendLine($"timeLimitSeconds = {ini.TimeLimitSeconds}");
            sb.AppendLine($"numClimbers = {ini.NumClimbers}");
            sb.AppendLine($"numFloaters = {ini.NumFloaters}");
            sb.AppendLine($"numBombers = {ini.NumBombers}");
            sb.AppendLine($"numBlockers = {ini.NumBlockers}");
            sb.AppendLine($"numBuilders = {ini.NumBuilders}");
            sb.AppendLine($"numBashers = {ini.NumBashers}");
            sb.AppendLine($"numMiners = {ini.NumMiners}");
            sb.AppendLine($"numDiggers = {ini.NumDiggers}");
            sb.AppendLine($"xPosCenter = {ini.XPosCenter}");
            sb.AppendLine($"yPosCenter = {ini.YPosCenter}");
            sb.AppendLine($"style = {ini.Style}");
            sb.AppendLine($"maxFallDistance = {ini.MaxFallDistance}");
            sb.AppendLine($"autosteelMode = {ini.AutosteelMode}");
            sb.AppendLine($"width = {ini.Width}");
            sb.AppendLine($"height = {ini.Height}");
            sb.AppendLine($"topBoundary = {ini.TopBoundary}");
            sb.AppendLine($"bottomBoundary = {ini.BottomBoundary}");
            sb.AppendLine($"leftBoundary = {ini.LeftBoundary}");
            sb.AppendLine($"rightBoundary = {ini.RightBoundary}");
            sb.AppendLine();

            // Add objects
            sb.AppendLine("# Objects");
            sb.AppendLine("# ID, X position, Y position, paint mode, flags, object-specific modifier (optional)");
            sb.AppendLine("# Paint modes: 0 = full, 2 = invisible, 4 = don't overwrite, 8 = visible only on terrain (only one value possible)");
            sb.AppendLine("# Flags: 1 = upside down, 2 = fake, 4 = upside-down mask, 8 = horizontally flipped (combining allowed)");
            foreach (var line in objectLines)
                sb.AppendLine(line);

            sb.AppendLine();

            // Add terrain
            sb.AppendLine("# Terrain");
            sb.AppendLine("# ID, X position, Y position, modifier");
            sb.AppendLine("# Modifier: 1 = invisible, 2 = remove, 4 = upside down, 8 = don't overwrite, 16 = fake, 32 = horizontally flipped, 64 = no one-way arrows (combining allowed)");
            foreach (var line in terrainLines)
                sb.AppendLine(line);

            sb.AppendLine();

            // Write all to .ini
            File.WriteAllText(filePath, sb.ToString(), Encoding.UTF8);
        }

        private string Escape(string text)
        {
            if (string.IsNullOrEmpty(text))
                return "";

            // INI safe: wrap strings containing special chars in quotes
            if (text.Contains('=') || text.Contains(';'))
                return "\"" + text.Replace("\"", "\\\"") + "\"";

            return text;
        }

        private (List<string> terrainLines,
                 List<string> objectLines,
                 List<string> unlinkedPieces) GetPieceLinks(Level level, string selectedStyle)
        {
            var pieceLinks = LoadStylePieceLinks(selectedStyle);

            var terrainLines = new List<string>();
            var objectLines = new List<string>();
            var unlinkedPieces = new List<string>();

            int terrainIndex = 0;
            int objectIndex = 0;

            // Terrain
            foreach (var terrain in level.TerrainList)
            {
                string key = terrain.Key;

                // Build terrain line for INI
                if (pieceLinks.TryGetValue(key, out int iniId))
                {
                    // Format: terrain_0 = ID, X, Y, modifier
                    int modifier = 0; // TODO: set based on flags like ONE_WAY, INVISIBLE, etc.
                    string line = 
                        $"terrain_{terrainIndex} = {iniId}," +
                        $"{terrain.PosX * 2}," +
                        $"{terrain.PosY * 2}," +
                        $"{modifier}";
                    terrainLines.Add(line);
                    terrainIndex++;
                }
                else
                {
                    unlinkedPieces.Add(key);
                }
            }

            // Objects
            foreach (var gadget in level.GadgetList)
            {
                string key = gadget.Key;

                // Build object line for INI
                if (pieceLinks.TryGetValue(key, out int iniId))
                {
                    // Format: object_0 = ID, X, Y, paintMode, flags, optionalModifier
                    int paintMode = 4; // TODO: set based on gadget properties - default = don't overwrite
                    int flags = 0;     // TODO: set based on gadget properties - flags
                    int optional = 0;  // TODO: set based on gadget properties - optional modifier
                    string line =
                        $"object_{objectIndex} = {iniId}," +
                        $"{gadget.PosX * 2}, {gadget.PosY * 2}," +
                        $"{paintMode}," +
                        $"{flags}," +
                        $"{optional}";
                    objectLines.Add(line);
                    objectIndex++;
                }
                else
                {
                    unlinkedPieces.Add(key);
                }
            }

            return (terrainLines, objectLines, unlinkedPieces);
        }

        private void ExportLevel()
        {
            try
            {
                if (comboStyles.SelectedItem == null || comboStyles.SelectedIndex == 0)
                {
                    MessageBox.Show("Please select a style before exporting.", "No Style Selected",
                                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                string selectedStyle = comboStyles.SelectedItem.ToString();

                IniLevel ini = ConvertToIni(curLevel, selectedStyle);
                GetPieceLinks(curLevel, selectedStyle);

                using (SaveFileDialog saveDialog = new SaveFileDialog())
                {
                    saveDialog.Filter = "INI files (*.ini)|*.ini";
                    saveDialog.Title = "Export Level to .INI";
                    saveDialog.FileName = $"{curLevel.Title}.ini";

                    if (saveDialog.ShowDialog() == DialogResult.OK)
                    {
                        var (terrainLines, objectLines, unlinkedPieces) = GetPieceLinks(curLevel, selectedStyle);

                        // Warn user if any pieces are missing
                        if (unlinkedPieces.Count > 0)
                        {
                            MessageBox.Show(
                                $"Warning: Some pieces are not linked to a corresponding piece in '{selectedStyle}':\n\n" +
                                string.Join(Environment.NewLine, unlinkedPieces) + "\n\n" +
                                "Please ensure these pieces are linked in SLXEditorINITranslationTables.ini",
                                "Unlinked Pieces",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Warning);
                        }

                        // Pass these lists to WriteIniFile
                        WriteIniFile(ini, saveDialog.FileName, terrainLines, objectLines);

                        MessageBox.Show($"Level exported successfully!\n\n{saveDialog.FileName}",
                            "Export Complete",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Information);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error exporting level: {ex.Message}",
                    "Export Failed",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        private void AddTranslationTableStyle()
        {
            string newStyle = Microsoft.VisualBasic.Interaction.InputBox(
                "To add a style to the dropdown list, please type the style name here."
                + Environment.NewLine + Environment.NewLine +
                "Note that the style should be Lemmini-compatible, and the name should" +
                "be written exactly as it would appear in a .ini level file.",
                "Add New Style",
                "");

            if (string.IsNullOrWhiteSpace(newStyle))
                return;

            newStyle = newStyle.Trim();

            var lines = File.ReadAllLines(AppPathTranslationTables).ToList();

            // Find index of [Styles] section
            int stylesIndex = lines.FindIndex(l => l.Trim().Equals("[Styles]", StringComparison.OrdinalIgnoreCase));

            if (stylesIndex == -1)
            {
                // If header missing, add it at the top
                lines.Insert(0, "[Styles]");
                stylesIndex = 0;
            }

            // Check if style already exists
            bool exists = lines.Skip(stylesIndex + 1)
                               .Any(l => l.Trim().Equals(newStyle, StringComparison.OrdinalIgnoreCase));

            if (exists)
            {
                MessageBox.Show("Style already exists in the translation table.", "Duplicate Style",
                                MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            // Insert new style into the list alphabetically
            var existingStyles = lines.Skip(stylesIndex + 1)
                                      .Where(l => !string.IsNullOrWhiteSpace(l))
                                      .Select(l => l.Trim())
                                      .ToList();
            existingStyles.Add(newStyle);
            existingStyles.Sort(StringComparer.OrdinalIgnoreCase);
            var newLines = lines.Take(stylesIndex + 1).ToList();
            newLines.AddRange(existingStyles);

            // Update the translation table .ini
            File.WriteAllLines(AppPathTranslationTables, newLines);

            // Update the combo
            comboStyles.Items.Add(newStyle);
            comboStyles.SelectedItem = newStyle;
        }

        private List<string> LoadTranslationStyles()
        {
            var styles = new List<string>();

            string filePath = AppPathTranslationTables;

            // Ensure the file exists
            if (!File.Exists(AppPathTranslationTables))
            {
                // If not, create file with [Styles] header and default styles
                var defaultStyles = new List<string>
                {
                    "Brick",
                    "Bubble",
                    "Crystal",
                    "Dirt",
                    "Fire",
                    "Marble",
                    "Pillar",
                    "Rock",
                    "Snow"
                };

                var linesToWrite = new List<string> { "[Styles]" };
                linesToWrite.AddRange(defaultStyles);

                File.WriteAllLines(AppPathTranslationTables, linesToWrite);
            }

            var lines = File.ReadAllLines(filePath);

            bool inStylesSection = false;

            foreach (var rawLine in lines)
            {
                string line = rawLine.Trim();

                if (string.IsNullOrEmpty(line))
                    continue;

                if (line.StartsWith("[") && line.EndsWith("]"))
                {
                    inStylesSection = line.Equals("[Styles]", StringComparison.OrdinalIgnoreCase);
                    continue;
                }

                if (inStylesSection)
                {
                    styles.Add(line);
                }
            }

            return styles;
        }

        private Dictionary<string, int> LoadStylePieceLinks(string styleName)
        {
            var links = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);

            if (!File.Exists(AppPathTranslationTables))
                return links;

            var lines = File.ReadAllLines(AppPathTranslationTables);

            bool inSection = false;

            foreach (var rawLine in lines)
            {
                string line = rawLine.Trim();

                if (string.IsNullOrEmpty(line))
                    continue;

                if (line.StartsWith("[") && line.EndsWith("]"))
                {
                    inSection = line.Substring(1, line.Length - 2)
                                   .Equals(styleName, StringComparison.OrdinalIgnoreCase);
                    continue;
                }

                if (inSection)
                {
                    var parts = line.Split(':');
                    if (parts.Length == 2 && int.TryParse(parts[1], out int iniId))
                    {
                        links[parts[0].Trim()] = iniId;
                    }
                }
            }

            return links;
        }

        private void PopulatePieceListView(string selectedStyle)
        {
            listViewPieceLinks.Items.Clear();

            var links = LoadStylePieceLinks(selectedStyle);
            var allPieces = curLevel.TerrainList.Cast<object>()
                            .Concat(curLevel.GadgetList.Cast<object>());

            var seenKeys = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

            foreach (dynamic piece in allPieces) // dynamic to handle Terrain/Gadget
            {
                string pieceKey = piece.Key;

                if (seenKeys.Contains(pieceKey))
                    continue;
                seenKeys.Add(pieceKey);

                string iniIdText = links.TryGetValue(pieceKey, out int iniId) ? iniId.ToString() : "Not yet linked";

                var item = new ListViewItem(new[] { pieceKey, iniIdText });
                item.Tag = pieceKey;
                listViewPieceLinks.Items.Add(item);
            }
        }

        private void AddPieceLink()
        {
            if (listViewPieceLinks.SelectedItems.Count == 0)
                return;

            var selectedItem = listViewPieceLinks.SelectedItems[0];
            string pieceKey = selectedItem.Tag as string;

            if (string.IsNullOrEmpty(pieceKey))
                return;

            using (OpenFileDialog ofd = new OpenFileDialog())
            {
                ofd.Title = "Select corresponding .ini piece file";
                ofd.Filter = "Image files|*.png;*.jpg;*.bmp|All files|*.*";

                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    string filename = Path.GetFileNameWithoutExtension(ofd.FileName);

                    int id = -1;
                    var match = System.Text.RegularExpressions.Regex.Match(filename, @"\d+");
                    if (match.Success)
                        id = int.Parse(match.Value);

                    if (id == -1)
                    {
                        MessageBox.Show(
                            "Could not determine ID from filename. Please ensure the filename contains the ID.",
                            "Invalid File", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }

                    if (comboStyles.SelectedItem == null)
                    {
                        MessageBox.Show(
                            "Please select a style before adding a piece link.",
                            "No Style Selected", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }

                    string selectedStyle = comboStyles.SelectedItem.ToString();

                    // Save to translation table using the internal piece key
                    UpdatePieceLink(selectedStyle, pieceKey, id);

                    selectedItem.SubItems[1].Text = id.ToString();
                }
            }
        }

        private void UpdatePieceLink(string styleName, string pieceKey, int iniId)
        {
            List<string> lines = File.Exists(AppPathTranslationTables)
                ? File.ReadAllLines(AppPathTranslationTables).ToList()
                : new List<string> { "[Styles]" };

            int sectionIndex = lines.FindIndex(l => l.Trim().Equals($"[{styleName}]", StringComparison.OrdinalIgnoreCase));

            if (sectionIndex == -1)
            {
                // Add new style section if it doesn't exist
                lines.Add("");
                lines.Add($"[{styleName}]");
                sectionIndex = lines.Count - 1;
            }

            // Check if the piece is already defined
            int pieceIndex = -1;
            for (int i = sectionIndex + 1; i < lines.Count; i++)
            {
                string line = lines[i].Trim();
                if (line.StartsWith("[") && line.EndsWith("]"))
                    break;

                if (line.StartsWith(pieceKey + ":", StringComparison.OrdinalIgnoreCase))
                {
                    pieceIndex = i;
                    break;
                }
            }

            string newLine = $"{pieceKey}:{iniId}";

            if (pieceIndex >= 0)
                lines[pieceIndex] = newLine;
            else
                lines.Insert(sectionIndex + 1, newLine);

            File.WriteAllLines(AppPathTranslationTables, lines);
        }

        private void UpdateControls()
        {
            if (comboStyles.SelectedItem == null)
                return;

            if (comboStyles.SelectedIndex != 0)
            {
                lblChosenOutputStyle.Text = comboStyles.Text;
                lblChosenOutputStyle.ForeColor = Color.ForestGreen;
                btnExport.Enabled = true;
                PopulatePieceListView(comboStyles.SelectedItem.ToString());
                UpdatePieceListControls();
            }
            else
            {
                lblChosenOutputStyle.Text = "Please choose a style";
                lblChosenOutputStyle.ForeColor = Color.DarkRed;
                btnExport.Enabled = false;
                listViewPieceLinks.Items.Clear();
                UpdatePieceListControls();
            }
        }

        private void UpdatePieceListControls()
        {
            btnAddPieceLink.Enabled = false;
            picPiecePreview.Image = null;

            if (listViewPieceLinks.SelectedItems.Count == 1)
            {
                btnAddPieceLink.Enabled = true;

                string pieceKey = listViewPieceLinks.SelectedItems[0].Tag as string;
                if (!string.IsNullOrEmpty(pieceKey))
                    PreviewPiece(pieceKey, picPiecePreview);
            }
        }

        public void PreviewPiece(string pieceKey, PictureBox piecePreview)
        {
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
                Color blendColor = curLevel.MainStyle?.GetColor(C.StyleColor.ONE_WAY_WALL) ?? C.NLColors[C.NLColor.OWWDefault];
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
            var styles = LoadTranslationStyles();
            comboStyles.Items.Clear();
            comboStyles.Items.Add("<Please choose a style>");
            comboStyles.Items.AddRange(styles.ToArray());

            if (comboStyles.Items.Count > 0)
                comboStyles.SelectedIndex = 0;

            UpdateControls();
        }

        private void btnAddStyle_Click(object sender, EventArgs e)
        {
            AddTranslationTableStyle();
        }

        private void btnExport_Click(object sender, EventArgs e)
        {
            ExportLevel();
        }

        private void comboStyles_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateControls();
        }

        private void btnAddPieceLink_Click(object sender, EventArgs e)
        {
            AddPieceLink();
        }

        private void listViewPieceLinks_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdatePieceListControls();
        }
    }
}
        // TODO:
        // ID 000 > # NXLV ID 000 (reference only)
        // VERSION 000 > # NXLV VERSION 000 (reference only)
        // SUPERLEMMING > superlemming = true
        // forceNormalTimerSpeed = true

        // TODO:
        // Print:
        // # Steel
        // # X position, Y position, width, height, flags (optional)
        // # Flags: 1 = remove existing steel
        // Here, we need the translation table to mark steel terrain pieces as steel
        // Then, for each steel piece, we print the following:
        // > steel_n++ = 2n (X position of terrain piece), 2n (Y position of terrain piece),
        // 2n (width of terrain piece without empty pixels), 2n (height, same), 0 (no flags)
