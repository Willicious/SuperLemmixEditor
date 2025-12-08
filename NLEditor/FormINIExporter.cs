using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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
            comboStyles.DisplayMember = "Name";

        }

        private Dictionary<string, INIStyleInfo> LoadedStyles = new Dictionary<string, INIStyleInfo>();

        private class INIStyleInfo
        {
            public string Name { get; set; }
            public string FolderPath { get; set; }

            public override string ToString() => Name;
        }

        // ───────────────────────────────────────────────
        // LOCAL CLASS: Only FormINIExporter uses this
        // ───────────────────────────────────────────────
        private class IniLevel
        {
            public string ID { get; set; }
            public string Version { get; set; }
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
            public string Superlemming { get; set; }
            public string ForceNormalTimerSpeed { get; set; }
        }

        // ───────────────────────────────────────────────
        // CONVERTER: Creates an IniLevel from the .nxlv data
        // ───────────────────────────────────────────────
        private IniLevel ConvertToIni(Level level, string selectedStyle)
        {
            var ini = new IniLevel();

            // NXLV ID and Version (for reference)
            ini.ID = level.LevelID.ToString("X16");
            ini.Version = level.LevelVersion.ToString();

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

            // Speed
            ini.Superlemming = level.IsSuperlemming ? "true" : "false";
            ini.ForceNormalTimerSpeed = "true";

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
        private void WriteIniFile(IniLevel ini, string filePath, List<string> objectLines, List<string> terrainLines, List<string> steelLines)
        {
            var sb = new StringBuilder();

            // Add level stats
            sb.AppendLine($"# LVL {Path.GetFileName(filePath)}");
            sb.AppendLine($"# Exported from SuperLemmix Editor Version {C.Version}");
            sb.AppendLine($"# Original .nxlv ID {ini.ID} Version {ini.Version}");
            sb.AppendLine("# RetroLemmini Level");
            sb.AppendLine();
            sb.AppendLine("# Level stats");
            sb.AppendLine($"name = {GetSafeString(ini.Name)}");
            sb.AppendLine($"author = {GetSafeString(ini.Author)}");
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
            sb.AppendLine($"superlemming = {ini.Superlemming}");
            sb.AppendLine($"forceNormalTimerSpeed = {ini.ForceNormalTimerSpeed}");
            sb.AppendLine();

            // Add objects
            sb.AppendLine("# Objects");
            sb.AppendLine("# ID, X pos, Y pos, paint mode, flags, (optional) object-specific modifier");
            sb.AppendLine("# Paint modes (one value only): 0 = full, 2 = invisible, 4 = no overwrite, 8 = only on terrain");
            sb.AppendLine("# Flags (combining allowed): 1 = inverted, 2 = fake, 4 = inverted trigger mask, 8 = flipped, 16 = rotated");
            foreach (var line in objectLines)
                sb.AppendLine(line);

            sb.AppendLine();

            // Add terrain
            sb.AppendLine("# Terrain");
            sb.AppendLine("# ID, X pos, Y pos, flags");
            sb.AppendLine("# Flags (combining allowed): 1 = invisible, 2 = eraser, 4 = inverted, 8 = no overwrite, 16 = fake, 32 = flipped, 64 = no one-way");
            foreach (var line in terrainLines)
                sb.AppendLine(line);

            sb.AppendLine();

            // Add steel
            sb.AppendLine("# Steel");
            sb.AppendLine("# X pos, Y pos, width, height, (optional) flags");
            sb.AppendLine("# Flags (optional): 1 = remove existing steel");
            foreach (var line in steelLines)
                sb.AppendLine(line);

            // Write all to .ini
            File.WriteAllText(filePath, sb.ToString(), Encoding.UTF8);
        }

        /// <summary>
        /// A helper method to generate INI-safe strings for level title and author
        /// </summary>
        private string GetSafeString(string text)
        {
            if (string.IsNullOrEmpty(text))
                return "";

            // INI safe: wrap strings containing special chars in quotes
            if (text.Contains('=') || text.Contains(';'))
                return "\"" + text.Replace("\"", "\\\"") + "\"";

            return text;
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
        /// Computes transparency offsets to align pieces between .nxlv and .ini
        /// </summary>
        public static (int xo, int yo, int xio, int yio) ComputeOffsets(
            (int left, int right, int top, int bottom) nxlvEdges,
            (int left, int right, int top, int bottom) iniEdges,
            int scale = 2)
        {
            int xo = Math.Abs(nxlvEdges.left * scale - iniEdges.left);
            int yo = Math.Abs(nxlvEdges.top * scale - iniEdges.top);
            int xio = Math.Abs(nxlvEdges.right * scale - iniEdges.right);
            int yio = Math.Abs(nxlvEdges.bottom * scale - iniEdges.bottom);

            return (xo, yo, xio, yio);
        }

        public static void AnalyzePieceOffsets(string styleName,
                                               string nxlvPiecePath,
                                               int iniId,
                                               string iniFolder,
                                               string translationTablePath)
        {
            if (!File.Exists(nxlvPiecePath))
                throw new FileNotFoundException("NXLV piece not found", nxlvPiecePath);

            string iniPiecePath = Path.Combine(iniFolder, $"{iniId}.png");
            if (!File.Exists(iniPiecePath))
                throw new FileNotFoundException("INI piece not found", iniPiecePath);

            using (var nxlvImg = new Bitmap(nxlvPiecePath))
            using (var iniImg = new Bitmap(iniPiecePath))
            {
                var nxlvEdges = GetBlankEdges(nxlvImg);
                var iniEdges = GetBlankEdges(iniImg);

                var offsets = ComputeOffsets(nxlvEdges, iniEdges);

                string pieceKey = Path.GetFileNameWithoutExtension(nxlvPiecePath);

                // Format line
                string line = string.Format("{0}\\{1}:{2},xo{3},yo{4},xio{5},yio{6}",
                    styleName, pieceKey, iniId, offsets.xo, offsets.yo, offsets.xio, offsets.yio);

                // Append to translation table file
                File.AppendAllLines(translationTablePath, new[] { line });
            }
        }

        private (int xo, int yo, int xio, int yio) GetPieceOffsets(string styleName, string pieceKey)
        {
            if (!File.Exists(AppPathTranslationTables))
                return (0, 0, 0, 0);

            var lines = File.ReadAllLines(AppPathTranslationTables);
            bool inSection = false;

            foreach (var rawLine in lines)
            {
                string line = rawLine.Trim();
                if (string.IsNullOrEmpty(line)) continue;

                if (line.StartsWith("[") && line.EndsWith("]"))
                {
                    inSection = line.Substring(1, line.Length - 2)
                                  .Equals(styleName, StringComparison.OrdinalIgnoreCase);
                    continue;
                }

                if (inSection && (line.StartsWith(pieceKey + ",", StringComparison.OrdinalIgnoreCase) ||
                                  line.StartsWith(pieceKey + ":", StringComparison.OrdinalIgnoreCase)))
                {
                    var match = Regex.Match(line, @"\((?:xo(-?\d+),yo(-?\d+),xio(-?\d+),yio(-?\d+))\)");
                    if (match.Success)
                    {
                        return (
                            int.Parse(match.Groups[1].Value),
                            int.Parse(match.Groups[2].Value),
                            int.Parse(match.Groups[3].Value),
                            int.Parse(match.Groups[4].Value)
                        );
                    }
                }
            }

            return (0, 0, 0, 0);
        }

        private static (int left, int top) GetExportOffsets(
            int xo, int yo, int xio, int yio,
            bool flip, bool invert, bool rotate, int rotation)
        {
            if (!flip && !invert && !rotate)
                return (xo, yo); // left = xo / top = yo

            if (!flip && !invert && rotation == 1)
                return (yio, xo); // left = yio / top = xo

            if (flip && invert && !rotate)
                return (xio, yio); // left = xio / top = yio

            if (flip && invert && rotation == 3)
                return (yo, xio); // left = yo / Top = xio

            if (flip & !invert && !rotate)
                return (xio, yo); // left = xio / top = yo

            if (!flip && invert && rotation == 1)
                return (yio, xio); // left = yio / top = xio

            if (!flip && invert && !rotate)
                return (xo, yio); // left = xo / top = yio

            if (flip & !invert && rotation == 3)
                return (yo, xo); // left = yo / top = xo

            else
                return (0, 0); // default fallback
        }


        /// <summary>
        /// Gets piece links between .nxlv style and corresponding .ini style,
        /// applying precomputed offsets from the translation table.
        private (List<string> terrainLines, List<string> objectLines, List<string> unlinkedPieces)
                GetPieceLinks(Level level, string selectedStyle)
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

                if (pieceLinks.TryGetValue(key, out int iniId))
                {
                    var o= GetPieceOffsets(selectedStyle, key);

                    (bool flip, bool invert, bool rotate, int rotation) =
                        (terrain.IsFlippedInPlayer,
                         terrain.IsInvertedInPlayer,
                         terrain.IsRotatedInPlayer,
                         terrain.GetRotation());

                    (int ox, int oy) = GetExportOffsets(
                        o.xo, o.yo, o.xio, o.yio,
                        flip, invert, rotate, rotation
                    );

                    int x = terrain.PosX * 2 - ox;
                    int y = terrain.PosY * 2 - oy;

                    int flags = 0;
                    if (terrain.IsErase)            flags |= 2;
                    if (terrain.IsInvertedInPlayer) flags |= 4;
                    if (terrain.IsNoOverwrite)      flags |= 8;
                    if (terrain.IsFlippedInPlayer)  flags |= 32;
                    if (!terrain.IsOneWay)          flags |= 64;
                    if (terrain.IsRotatedInPlayer)  flags |= 128;

                    terrainLines.Add($"terrain_{terrainIndex} = {iniId},{x},{y},{flags}");
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

                if (pieceLinks.TryGetValue(key, out int iniId))
                {
                    var o = GetPieceOffsets(selectedStyle, key);

                    (bool flip, bool invert, bool rotate, int rotation) =
                        (gadget.IsFlippedInPlayer,
                         gadget.IsInvertedInPlayer,
                         gadget.IsRotatedInPlayer,
                         gadget.GetRotation());

                    (int ox, int oy) = GetExportOffsets(
                        o.xo, o.yo, o.xio, o.yio,
                        flip, invert, rotate, rotation
                    );

                    int x = gadget.PosX * 2 - ox;
                    int y = gadget.PosY * 2 - oy;

                    int paintMode = gadget.IsNoOverwrite ? 4 : (gadget.IsOnlyOnTerrain ? 8 : 0);

                    int flags = 0;
                    if (gadget.IsInvertedInPlayer) { flags |= 1; flags |= 4; } // Also invert the mask
                    if (gadget.IsFlippedInPlayer)    flags |= 8;
                    if (gadget.IsRotatedInPlayer)    flags |= 16;

                    objectLines.Add($"object_{objectIndex} = {iniId},{x},{y},{paintMode},{flags}");
                    objectIndex++;
                }
                else
                {
                    unlinkedPieces.Add(key);
                }
            }

            return (terrainLines, objectLines, unlinkedPieces);
        }

        private List<string> GetSteelLines(Level level, string selectedStyle)
        {
            var steelLines = new List<string>();
            int steelIndex = 0;

            foreach (var terrain in level.TerrainList)
            {
                if (terrain.IsSteel)
                {
                    var o = GetPieceOffsets(selectedStyle, terrain.Key);

                    (bool flip, bool invert, bool rotate, int rotation) =
                        (terrain.IsFlippedInPlayer,
                         terrain.IsInvertedInPlayer,
                         terrain.IsRotatedInPlayer,
                         terrain.GetRotation());

                    (int ox, int oy) = GetExportOffsets(
                        o.xo, o.yo, o.xio, o.yio,
                        flip, invert, rotate, rotation
                    );

                    int x = terrain.PosX * 2 - ox;
                    int y = terrain.PosY * 2 - oy;

                    int width = terrain.Width * 2;
                    int height = terrain.Height * 2;

                    steelLines.Add($"steel_{steelIndex} = {x},{y},{width},{height}");
                    steelIndex++;
                }
            }

            return steelLines;
        }


        private void ExportLevel()
        {
            try
            {
                // Ensure a style is selected
                if (comboStyles.SelectedItem == null || comboStyles.SelectedIndex == 0)
                {
                    MessageBox.Show("Please select a style before exporting.", "No Style Selected",
                                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                string selectedStyle = comboStyles.SelectedItem.ToString();

                // Convert level to INI structure
                IniLevel ini = ConvertToIni(curLevel, selectedStyle);

                // Get terrain/object lines with offsets applied
                var (terrainLines, objectLines, unlinkedPieces) = GetPieceLinks(curLevel, selectedStyle);

                // Get steel lines
                var steelLines = GetSteelLines(curLevel, selectedStyle);

                // Warn if some pieces are unlinked
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

                // Prompt for export location
                using (SaveFileDialog saveDialog = new SaveFileDialog())
                {
                    saveDialog.InitialDirectory = AppPathLevels;
                    saveDialog.Filter = "INI files (*.ini)|*.ini";
                    saveDialog.Title = "Export Level to .INI";
                    saveDialog.FileName = $"{curLevel.Title}.ini";

                    if (saveDialog.ShowDialog() == DialogResult.OK)
                    {
                        WriteIniFile(ini, saveDialog.FileName, objectLines, terrainLines, steelLines);

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
            List<string> lines = File.Exists(AppPathTranslationTables)
                ? File.ReadAllLines(AppPathTranslationTables).ToList()
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
                File.WriteAllLines(AppPathTranslationTables, lines);
            }
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
                    if (parts.Length == 2)
                    {
                        var iniPart = parts[1].Trim();
                        var iniIdString = iniPart.Split('(')[0];
                        if (int.TryParse(iniIdString, out int id))
                        {
                            links[parts[0].Trim()] = id;
                        }
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
            UpdateUnlinkedPieceLabel();

            if (listViewPieceLinks.SelectedItems.Count > 0)
            {
                var selectedItem = listViewPieceLinks.SelectedItems[0];
                selectedItem.SubItems[1].Text = iniId.ToString();

                SelectNextUnlinkedPiece(selectedItem.Index);
            }
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

            List<string> lines = File.Exists(AppPathTranslationTables)
                ? File.ReadAllLines(AppPathTranslationTables).ToList()
                : new List<string> { "[Styles]" };

            // Locate or create style section
            int sectionIndex = lines.FindIndex(l => l.Trim().Equals($"[{styleName}]", StringComparison.OrdinalIgnoreCase));
            if (sectionIndex == -1)
            {
                lines.Add("");
                lines.Add($"[{styleName}]");
                sectionIndex = lines.Count - 1;
            }

            int xo = 0, yo = 0, xio = 0, yio = 0;

            try
            {
                bool isGadget = pieceKey.IndexOf($"{styleName}\\objects\\", StringComparison.OrdinalIgnoreCase) >= 0;
                string iniPiecePath = ResolveIniPiecePath(styleName, iniFolder, iniId, isGadget);
                string nxlvPath = Path.Combine(AppPathPieces, pieceKey + ".png");

                if (!string.IsNullOrEmpty(iniPiecePath) && File.Exists(nxlvPath) && File.Exists(iniPiecePath))
                {
                    using (var nxlvImg = new Bitmap(nxlvPath))
                    using (var iniImg = new Bitmap(iniPiecePath))
                    {
                        var nxlvEdges = GetBlankEdges(nxlvImg);
                        var iniEdges = GetBlankEdges(iniImg);
                        var offsets = ComputeOffsets(nxlvEdges, iniEdges);
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

            File.WriteAllLines(AppPathTranslationTables, lines);
        }

        private void AddPieceLink()
        {
            if (listViewPieceLinks.SelectedItems.Count == 0) return;

            var selectedItem = listViewPieceLinks.SelectedItems[0];
            string pieceKey = selectedItem.Tag as string;
            if (string.IsNullOrEmpty(pieceKey)) return;

            if (comboStyles.SelectedItem == null) return;
            string styleName = comboStyles.SelectedItem.ToString();
            int iniId = Decimal.ToInt32(numLinkedPieceID.Value);

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

                string filename = Path.GetFileNameWithoutExtension(ofd.FileName);
                var match = System.Text.RegularExpressions.Regex.Match(filename, @"\d+");
                if (!match.Success)
                {
                    MessageBox.Show("Could not determine ID from filename.", "Error",
                                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                int iniId = int.Parse(match.Value);

                if (comboStyles.SelectedItem == null) return;
                string styleName = comboStyles.SelectedItem.ToString();

                string pieceFolder = Path.GetDirectoryName(ofd.FileName);
                string iniPiecePath = ofd.FileName;
                LinkPiece(pieceKey, styleName, iniId, pieceFolder, iniPiecePath);
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
                btnExport.Enabled = true;
                PopulatePieceListView(comboStyles.SelectedItem.ToString());
            }
            else
            {
                lblChosenOutputStyle.Text = "Please choose a style";
                lblChosenOutputStyle.ForeColor = Color.DarkRed;
                btnExport.Enabled = false;
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
            }
            else
            {
                lblUnlinkedPieces.Text = "All pieces are linked! The level can now be fully exported";
                lblUnlinkedPieces.ForeColor = Color.ForestGreen;
                lblUnlinkedPieces.Visible = true;
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
    }
}

// TODO
// Double-check transparency offset values every session (in case images have changed)
