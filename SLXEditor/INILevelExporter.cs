using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SLXEditor
{
    internal class INILevelExporter
    {
        public bool LevelContainsGroups { get; private set; }

        internal sealed class IniExportResult
        {
            public List<string> UnlinkedPieces { get; }

            public IniExportResult(List<string> unlinkedPieces)
            {
                UnlinkedPieces = unlinkedPieces ?? new List<string>();
            }
        }
        public class IniLevel
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

        /// <summary>
        /// Ungroups all grouped pieces in the export copy.
        /// </summary>
        public void UngroupAllPiecesForExport(Level level)
        {
            var groupsToUngroup = level.TerrainList
                .OfType<GroupPiece>()
                .ToList();

            if (groupsToUngroup.Count != 0)
                LevelContainsGroups = true;
            else
                return;

            foreach (var group in groupsToUngroup)
            {
                level.UnselectAll();
                group.IsSelected = true;

                if (level.MayUngroupSelection())
                    level.UngroupSelection();
            }

            level.UnselectAll();
        }

        /// <summary>
        /// Loads all known translation tables
        /// </summary>
        public List<string> LoadTranslationStyles()
        {
            var styles = new List<string>();

            string filePath = C.AppPathTranslationTables;

            // Ensure the file exists - if not, create it with default styles added
            if (!File.Exists(filePath))
            {
                File.WriteAllText(C.AppPathTranslationTables,
                INITranslationTables.DefaultContent,
                Encoding.UTF8
                );
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

        /// <summary>
        /// Loads piece links for the selected style
        /// </summary>
        public Dictionary<string, int> LoadStylePieceLinks(string styleName)
        {
            var links = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);

            if (!File.Exists(C.AppPathTranslationTables))
                return links;

            var lines = File.ReadAllLines(C.AppPathTranslationTables);

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

        private int GetSkill(Level level, C.Skill skill)
        {
            if (level.SkillSet != null && level.SkillSet.ContainsKey(skill))
                return level.SkillSet[skill];

            return 0;
        }

        private IniLevel ConvertToIni(Level level, string selectedStyle)
        {
            var ini = new IniLevel();

            // Level ID and Version (for reference)
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
            ini.NumClimbers = GetSkill(level, C.Skill.Climber);
            ini.NumFloaters = GetSkill(level, C.Skill.Floater);
            ini.NumBombers = GetSkill(level, C.Skill.Timebomber)
                           + GetSkill(level, C.Skill.Bomber);
            ini.NumBlockers = GetSkill(level, C.Skill.Blocker);
            ini.NumBuilders = GetSkill(level, C.Skill.Builder);
            ini.NumBashers = GetSkill(level, C.Skill.Basher);
            ini.NumMiners = GetSkill(level, C.Skill.Miner);
            ini.NumDiggers = GetSkill(level, C.Skill.Digger);

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

        private string GetSafeString(string text)
        {
            if (string.IsNullOrEmpty(text))
                return "";

            // INI safe: wrap strings containing special chars in quotes
            if (text.Contains('=') || text.Contains(';'))
                return "\"" + text.Replace("\"", "\\\"") + "\"";

            return text;
        }

        private void WriteIniFile(IniLevel ini, string filePath, List<string> objectLines, List<string> terrainLines)
        {
            var sb = new StringBuilder();

            // Add level stats
            sb.AppendLine($"# LVL {Path.GetFileName(filePath)}");
            sb.AppendLine($"# Exported from SuperLemmix Editor Version {C.Version}");
            sb.AppendLine($"# Original level ID {ini.ID} Version {ini.Version}");
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

            // Write all to .ini
            File.WriteAllText(filePath, sb.ToString(), Encoding.UTF8);
        }

        private (int xo, int yo, int xio, int yio) GetPieceOffsets(string styleName, string pieceKey)
        {
            if (!File.Exists(C.AppPathTranslationTables))
                return (0, 0, 0, 0);

            var lines = File.ReadAllLines(C.AppPathTranslationTables);
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
        /// Gets piece links between .*xlv style and corresponding .ini style,
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
                    var o = GetPieceOffsets(selectedStyle, key);

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
                    if (terrain.IsErase) flags |= 2;
                    if (terrain.IsInvertedInPlayer) flags |= 4;
                    if (terrain.IsNoOverwrite) flags |= 8;
                    if (terrain.IsFlippedInPlayer) flags |= 32;
                    if (!terrain.IsOneWay) flags |= 64;
                    if (terrain.IsRotatedInPlayer) flags |= 128;

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
                    if (gadget.IsFlippedInPlayer) flags |= 8;
                    if (gadget.IsRotatedInPlayer) flags |= 16;

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

        public IniExportResult ExportLevelToIni(Level level, string style, string outputPath)
        {
            if (string.IsNullOrWhiteSpace(style))
                throw new ArgumentException("Style must be provided.", nameof(style));

            // Convert level to INI structure
            IniLevel ini = ConvertToIni(level, style);

            // Get terrain/object lines with offsets applied
            var (terrainLines, objectLines, unlinkedPieces) = GetPieceLinks(level, style);

            // Write output file
            WriteIniFile(ini, outputPath, objectLines, terrainLines);

            return new IniExportResult(unlinkedPieces);
        }
    }
}
