using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace SLXEditor
{
    public class HotkeyConfig
    {
        public enum HotkeyName
        {
            HotkeyCreateNewLevel,
            HotkeyLoadLevel,
            HotkeySaveLevel,
            HotkeySaveLevelAs,
            HotkeySaveLevelAsImage,
            HotkeyExportLevelAsINI,
            HotkeyOpenTemplate,
            HotkeySaveLevelAsTemplate,
            HotkeyPlaytestLevel,
            HotkeyValidateLevel,
            HotkeyCleanseLevels,
            HotkeyHighlightGroupedPieces,
            HotkeyHighlightEraserPieces,
            HotkeyToggleClearPhysics,
            HotkeyToggleTerrain,
            HotkeyToggleObjects,
            HotkeyToggleTriggerAreas,
            HotkeyToggleRulers,
            HotkeyToggleScreenStart,
            HotkeyToggleBackground,
            HotkeyToggleDeprecatedPieces,
            HotkeyPieceSearch,
            HotkeyShowMissingPieces,
            HotkeyRefreshStyles,
            HotkeyOpenStyleManager,
            HotkeyToggleSnapToGrid,
            HotkeyToggleCrop,
            HotkeyOpenLevelArrangerWindow,
            HotkeyOpenPieceBrowserWindow,
            HotkeyToggleAllTabs,
            HotkeyOpenSettings,
            HotkeyOpenConfigHotkeys,
            HotkeyOpenAboutSLX,
            HotkeySelectPieces,
            HotkeyDragToScroll,
            HotkeyDragHorizontally,
            HotkeyDragVertically,
            HotkeyMoveScreenStart,
            HotkeySetScreenStartToCursor,
            HotkeyRemovePiecesAtCursor,
            HotkeyAddRemoveSinglePiece,
            HotkeySelectPiecesBelow,
            HotkeyZoomIn,
            HotkeyZoomOut,
            HotkeyScrollHorizontally,
            HotkeyScrollVertically,
            HotkeyShowPreviousPiece,
            HotkeyShowNextPiece,
            HotkeyShowPreviousGroup,
            HotkeyShowNextGroup,
            HotkeyShowPreviousStyle,
            HotkeyShowNextStyle,
            HotkeyCycleBrowser,
            HotkeyAddPiece1,
            HotkeyAddPiece2,
            HotkeyAddPiece3,
            HotkeyAddPiece4,
            HotkeyAddPiece5,
            HotkeyAddPiece6,
            HotkeyAddPiece7,
            HotkeyAddPiece8,
            HotkeyAddPiece9,
            HotkeyAddPiece10,
            HotkeyAddPiece11,
            HotkeyAddPiece12,
            HotkeyAddPiece13,
            HotkeyUndo,
            HotkeyRedo,
            HotkeySelectAll,
            HotkeyCut,
            HotkeyCopy,
            HotkeyPaste,
            HotkeyPasteInPlace,
            HotkeyDuplicate,
            HotkeyDuplicateUp,
            HotkeyDuplicateDown,
            HotkeyDuplicateLeft,
            HotkeyDuplicateRight,
            HotkeyDelete,
            HotkeyMoveUp,
            HotkeyMoveDown,
            HotkeyMoveLeft,
            HotkeyMoveRight,
            HotkeyGridMoveUp,
            HotkeyGridMoveDown,
            HotkeyGridMoveLeft,
            HotkeyGridMoveRight,
            HotkeyCustomMoveUp,
            HotkeyCustomMoveDown,
            HotkeyCustomMoveLeft,
            HotkeyCustomMoveRight,
            HotkeyRotate,
            HotkeyFlip,
            HotkeyInvert,
            HotkeyGroup,
            HotkeyUngroup,
            HotkeyErase,
            HotkeyNoOverwrite,
            HotkeyOnlyOnTerrain,
            HotkeyAllowOneWay,
            HotkeyDrawLast,
            HotkeyDrawSooner,
            HotkeyDrawLater,
            HotkeyDrawFirst,
            HotkeyCloseEditor
        }

        public static readonly List<HotkeyData> AllHotkeys = new List<HotkeyData>
        { 
            new HotkeyData(HotkeyName.HotkeyCreateNewLevel,
                "Create New Level",
                Keys.Control | Keys.N),
            new HotkeyData(HotkeyName.HotkeyLoadLevel,
                "Load Level",
                Keys.Control | Keys.O),
            new HotkeyData(HotkeyName.HotkeySaveLevel,
                "Save Level",
                Keys.Control | Keys.S),
            new HotkeyData(HotkeyName.HotkeySaveLevelAs,
                "Save Level As",
                Keys.Control | Keys.Shift | Keys.S),
            new HotkeyData(HotkeyName.HotkeySaveLevelAsImage,
                "Save Level As Image",
                Keys.Control | Keys.Alt | Keys.S),
            new HotkeyData(HotkeyName.HotkeyExportLevelAsINI,
                "Export Level To INI",
                Keys.Control | Keys.I),
            new HotkeyData(HotkeyName.HotkeyOpenTemplate,
                "Open Templates Loader",
                Keys.Control | Keys.T),
            new HotkeyData(HotkeyName.HotkeySaveLevelAsTemplate,
                "Save Level As Template",
                Keys.Control | Keys.Alt | Keys.T),
            new HotkeyData(HotkeyName.HotkeyPlaytestLevel,
                "Playtest Level",
                Keys.F12),
            new HotkeyData(HotkeyName.HotkeyValidateLevel,
                "Validate Level",
                Keys.Control | Keys.F12),
            new HotkeyData(HotkeyName.HotkeyCleanseLevels,
                "Cleanse Levels",
                Keys.Control | Keys.Shift | Keys.F12),
            new HotkeyData(HotkeyName.HotkeyHighlightGroupedPieces,
                "Highlight Grouped Pieces",
                Keys.Control | Keys.G),
            new HotkeyData(HotkeyName.HotkeyHighlightEraserPieces,
                "Highlight Eraser Pieces",
                Keys.Control | Keys.E),
            new HotkeyData(HotkeyName.HotkeyToggleClearPhysics,
                "Toggle Clear Physics",
                Keys.F1),
            new HotkeyData(HotkeyName.HotkeyToggleTerrain,
                "Toggle Terrain",
                Keys.F2),
            new HotkeyData(HotkeyName.HotkeyToggleObjects,
                "Toggle Objects",
                Keys.F3),
            new HotkeyData(HotkeyName.HotkeyToggleTriggerAreas,
                "Toggle Trigger Areas",
                Keys.F4),
            new HotkeyData(HotkeyName.HotkeyToggleRulers,
                "Toggle Rulers",
                Keys.Control | Keys.F4),
            new HotkeyData(HotkeyName.HotkeyToggleScreenStart,
                "Toggle Screen Start",
                Keys.F5),
            new HotkeyData(HotkeyName.HotkeyToggleBackground,
                "Toggle Background",
                Keys.F6),
            new HotkeyData(HotkeyName.HotkeyToggleDeprecatedPieces,
                "Toggle Deprecated Pieces",
                Keys.F7),
            new HotkeyData(HotkeyName.HotkeyPieceSearch,
                "Open Piece Search",
                Keys.F8),
            new HotkeyData(HotkeyName.HotkeyShowMissingPieces,
                "Show Missing Pieces",
                Keys.Control | Keys.F8),
            new HotkeyData(HotkeyName.HotkeyRefreshStyles,
                "Refresh Styles",
                Keys.Control | Keys.Shift | Keys.F8),
            new HotkeyData(HotkeyName.HotkeyOpenStyleManager,
                "Open Style Manager",
                Keys.Control | Keys.Alt | Keys.F8),
            new HotkeyData(HotkeyName.HotkeyToggleSnapToGrid,
                "Toggle Snap To Grid",
                Keys.F9),
            new HotkeyData(HotkeyName.HotkeyToggleCrop,
                "Toggle Crop",
                Keys.X),
            new HotkeyData(HotkeyName.HotkeyOpenLevelArrangerWindow,
                "Open Level Arranger Window",
                Keys.Control | Keys.F9),
            new HotkeyData(HotkeyName.HotkeyOpenPieceBrowserWindow,
                "Open Piece Browser Window",
                Keys.Control | Keys.Shift | Keys.F9),
            new HotkeyData(HotkeyName.HotkeyToggleAllTabs,
                "Toggle Expand All Tabs",
                Keys.Control | Keys.F10),
            new HotkeyData(HotkeyName.HotkeyOpenSettings,
                "Open Settings",
                Keys.F10),
            new HotkeyData(HotkeyName.HotkeyOpenConfigHotkeys,
                "Open Hotkey Config",
                Keys.F11),
            new HotkeyData(HotkeyName.HotkeyOpenAboutSLX,
                "Open About",
                Keys.Control | Keys.F11),
            new HotkeyData(HotkeyName.HotkeySelectPieces,
                "Select/Drag Pieces",
                Keys.LButton,
                true),
            new HotkeyData(HotkeyName.HotkeyDragToScroll,
                "Drag To Scroll",
                Keys.RButton,
                true),
            new HotkeyData(HotkeyName.HotkeyDragHorizontally,
                "Drag Horizontally",
                Keys.Control | Keys.Alt | Keys.LButton,
                true),
            new HotkeyData(HotkeyName.HotkeyDragVertically,
                "Drag Vertically",
                Keys.Control | Keys.Shift | Keys.LButton,
                true),
            new HotkeyData(HotkeyName.HotkeyMoveScreenStart,
                "Move Screen Start",
                Keys.P),
            new HotkeyData(HotkeyName.HotkeySetScreenStartToCursor,
                "Set Screen Start To Cursor",
                Keys.L),
            new HotkeyData(HotkeyName.HotkeyRemovePiecesAtCursor,
                "Remove Pieces At Cursor",
                Keys.MButton,
                true),
            new HotkeyData(HotkeyName.HotkeyAddRemoveSinglePiece,
                "Add/Remove Single Piece To/From Selection",
                Keys.Control | Keys.LButton,
                true),
            new HotkeyData(HotkeyName.HotkeySelectPiecesBelow,
                "Select Pieces Below Current Piece Under Cursor",
                Keys.Alt | Keys.LButton,
                true),
            new HotkeyData(HotkeyName.HotkeyZoomIn,
                "Zoom In",
                Keys.Oemplus),
            new HotkeyData(HotkeyName.HotkeyZoomOut,
                "Zoom Out",
                Keys.OemMinus),
            new HotkeyData(HotkeyName.HotkeyScrollHorizontally,
                "Scroll Horizontally",
                Keys.H),
            new HotkeyData(HotkeyName.HotkeyScrollVertically,
                "Scroll Vertically",
                Keys.V),
            new HotkeyData(HotkeyName.HotkeyShowPreviousPiece,
                "Show Previous Piece in Piece Browser",
                Keys.Shift | Keys.Left),
            new HotkeyData(HotkeyName.HotkeyShowNextPiece,
                "Show Next Piece in Piece Browser",
                Keys.Shift | Keys.Right),
            new HotkeyData(HotkeyName.HotkeyShowPreviousGroup,
                "Show Previous Group in Piece Browser",
                Keys.Shift | Keys.Alt | Keys.Left),
            new HotkeyData(HotkeyName.HotkeyShowNextGroup,
                "Show Next Group in Piece Browser",
                Keys.Shift | Keys.Alt | Keys.Right),
            new HotkeyData(HotkeyName.HotkeyShowPreviousStyle,
                "Show Previous Style in Piece Browser",
                Keys.Shift | Keys.Up),
            new HotkeyData(HotkeyName.HotkeyShowNextStyle,
                "Show Next Style in Piece Browser",
                Keys.Shift | Keys.Down),
            new HotkeyData(HotkeyName.HotkeyCycleBrowser,
                "Cycle Through Piece Browser Items (Terrain, Objects, etc)",
                Keys.Space),
            new HotkeyData(HotkeyName.HotkeyAddPiece1,
                "Add Piece At Position 1 in Piece Browser",
                Keys.D1),
            new HotkeyData(HotkeyName.HotkeyAddPiece2,
                "Add Piece At Position 2 in Piece Browser",
                Keys.D2),
            new HotkeyData(HotkeyName.HotkeyAddPiece3,
                "Add Piece At Position 3 in Piece Browser",
                Keys.D3),
            new HotkeyData(HotkeyName.HotkeyAddPiece4,
                "Add Piece At Position 4 in Piece Browser",
                Keys.D4),
            new HotkeyData(HotkeyName.HotkeyAddPiece5,
                "Add Piece At Position 5 in Piece Browser",
                Keys.D5),
            new HotkeyData(HotkeyName.HotkeyAddPiece6,
                "Add Piece At Position 6 in Piece Browser",
                Keys.D6),
            new HotkeyData(HotkeyName.HotkeyAddPiece7,
                "Add Piece At Position 7 in Piece Browser",
                Keys.D7),
            new HotkeyData(HotkeyName.HotkeyAddPiece8,
                "Add Piece At Position 8 in Piece Browser",
                Keys.D8),
            new HotkeyData(HotkeyName.HotkeyAddPiece9,
                "Add Piece At Position 9 in Piece Browser",
                Keys.D9),
            new HotkeyData(HotkeyName.HotkeyAddPiece10,
                "Add Piece At Position 10 in Piece Browser",
                Keys.D0),
            new HotkeyData(HotkeyName.HotkeyAddPiece11,
                "Add Piece At Position 11 in Piece Browser",
                Keys.NumPad1),
            new HotkeyData(HotkeyName.HotkeyAddPiece12,
                "Add Piece At Position 12 in Piece Browser",
                Keys.NumPad2),
            new HotkeyData(HotkeyName.HotkeyAddPiece13,
                "Add Piece At Position 13 in Piece Browser",
                Keys.NumPad3),
            new HotkeyData(HotkeyName.HotkeyUndo,
                "Undo",
                Keys.Control | Keys.Z),
            new HotkeyData(HotkeyName.HotkeyRedo,
                "Redo",
                Keys.Control | Keys.Y),
            new HotkeyData(HotkeyName.HotkeySelectAll,
                "Select All",
                Keys.Control | Keys.A),
            new HotkeyData(HotkeyName.HotkeyCut,
                "Cut",
                Keys.Control | Keys.X),
            new HotkeyData(HotkeyName.HotkeyCopy,
                "Copy",
                Keys.Control | Keys.C),
            new HotkeyData(HotkeyName.HotkeyPaste,
                "Paste",
                Keys.Control | Keys.V),
            new HotkeyData(HotkeyName.HotkeyPasteInPlace,
                "Paste-In-Place",
                Keys.Control | Keys.Shift | Keys.V),
            new HotkeyData(HotkeyName.HotkeyDuplicate,
                "Duplicate-In-Place",
                Keys.D),
            new HotkeyData(HotkeyName.HotkeyDuplicateUp,
                "Duplicate-Upwards",
                Keys.Control | Keys.Alt | Keys.Up),
            new HotkeyData(HotkeyName.HotkeyDuplicateDown,
                "Duplicate-Downwards",
                Keys.Control | Keys.Alt | Keys.Down),
            new HotkeyData(HotkeyName.HotkeyDuplicateLeft,
                "Duplicate-To-Left",
                Keys.Control | Keys.Alt | Keys.Left),
            new HotkeyData(HotkeyName.HotkeyDuplicateRight,
                "Duplicate-To-Right",
                Keys.Control | Keys.Alt | Keys.Right),
            new HotkeyData(HotkeyName.HotkeyDelete,
                "Delete",
                Keys.Delete),
            new HotkeyData(HotkeyName.HotkeyMoveUp,
                "Move Up 1px",
                Keys.Up),
            new HotkeyData(HotkeyName.HotkeyMoveDown,
                "Move Down 1px",
                Keys.Down),
            new HotkeyData(HotkeyName.HotkeyMoveLeft,
                "Move Left 1px",
                Keys.Left),
            new HotkeyData(HotkeyName.HotkeyMoveRight,
                "Move Right 1px",
                Keys.Right),
            new HotkeyData(HotkeyName.HotkeyGridMoveUp,
                "Move Up by Grid Amount",
                Keys.Control | Keys.Up),
            new HotkeyData(HotkeyName.HotkeyGridMoveDown,
                "Move Down by Grid Amount",
                Keys.Control | Keys.Down),
            new HotkeyData(HotkeyName.HotkeyGridMoveLeft,
                "Move Left by Grid Amount",
                Keys.Control | Keys.Left),
            new HotkeyData(HotkeyName.HotkeyGridMoveRight,
                "Move Right by Grid Amount",
                Keys.Control | Keys.Right),
            new HotkeyData(HotkeyName.HotkeyCustomMoveUp,
                "Move Up by Custom Amount",
                Keys.Alt | Keys.Up),
            new HotkeyData(HotkeyName.HotkeyCustomMoveDown,
                "Move Down by Custom Amount",
                Keys.Alt | Keys.Down),
            new HotkeyData(HotkeyName.HotkeyCustomMoveLeft,
                "Move Left by Custom Amount",
                Keys.Alt | Keys.Left),
            new HotkeyData(HotkeyName.HotkeyCustomMoveRight,
                "Move Right by Custom Amount",
                Keys.Alt | Keys.Right),
            new HotkeyData(HotkeyName.HotkeyRotate,
                "Rotate Piece(s)",
                Keys.R),
            new HotkeyData(HotkeyName.HotkeyFlip,
                "Flip Piece(s)",
                Keys.F),
            new HotkeyData(HotkeyName.HotkeyInvert,
                "Invert Piece(s)",
                Keys.I),
            new HotkeyData(HotkeyName.HotkeyGroup,
                "Group Selected Pieces",
                Keys.G),
            new HotkeyData(HotkeyName.HotkeyUngroup,
                "Ungroup Selected Pieces",
                Keys.U),
            new HotkeyData(HotkeyName.HotkeyErase,
                "Set Piece(s) As Erase",
                Keys.E),
            new HotkeyData(HotkeyName.HotkeyNoOverwrite,
                "Set Piece(s) As No-Overwrite",
                Keys.N),
            new HotkeyData(HotkeyName.HotkeyOnlyOnTerrain,
                "Set Piece(s) As Only-On-Terrain",
                Keys.T),
            new HotkeyData(HotkeyName.HotkeyAllowOneWay,
                "Set Piece(s) As Allow-One-Way",
                Keys.O),
            new HotkeyData(HotkeyName.HotkeyDrawLast,
                "Bring Piece(s) To Front",
                Keys.Home),
            new HotkeyData(HotkeyName.HotkeyDrawSooner,
                "Bring Piece(s) Up One Layer",
                Keys.PageUp),
            new HotkeyData(HotkeyName.HotkeyDrawLater,
                "Send Piece(s) Down One Layer",
                Keys.PageDown),
            new HotkeyData(HotkeyName.HotkeyDrawFirst,
                "Send Piece(s) To Back",
                Keys.End),
            new HotkeyData(HotkeyName.HotkeyCloseEditor,
                "Close Editor",
                Keys.Alt | Keys.F4)
        };

        public static readonly List<Keys> MouseKeys = new List<Keys>
        {
            Keys.LButton,
            Keys.RButton,
            Keys.MButton,
            Keys.XButton1,
            Keys.XButton2
        };

        public static readonly List<ListViewItem> mouseMandatoryItems = new List<ListViewItem>();
        
        public static bool DefaultHotkeysLoaded = false;
        public static bool PlayerHotkeysLoaded = false;

        public static string FormatHotkeyString(HotkeyData hotkeyData)
        {
            if (hotkeyData == null) return "None";
            return FormatHotkeyString(hotkeyData.CurrentKeys);
        }

        public static string FormatHotkeyString(HotkeyName name)
        {
            var hotkeyData = GetHotkey(name);
            return FormatHotkeyString(hotkeyData);
        }
        public static string FormatHotkeyString(Keys hotkey)
        {
            List<string> hotkeyParts = new List<string>();

            if (hotkey.HasFlag(Keys.Control)) hotkeyParts.Add("Ctrl");
            if (hotkey.HasFlag(Keys.Shift)) hotkeyParts.Add("Shift");
            if (hotkey.HasFlag(Keys.Alt)) hotkeyParts.Add("Alt");

            // Get the base key from the flags
            Keys baseKey = hotkey & ~(Keys.Control | Keys.Shift | Keys.Alt);

            // Convert special cases to more user-friendly names
            string baseKeyString;
            switch (baseKey)
            {
                case Keys.Back:
                    baseKeyString = "Backspace";
                    break;
                case Keys.OemMinus:
                    baseKeyString = "Minus";
                    break;
                case Keys.Oemplus:
                    baseKeyString = "Plus";
                    break;
                case Keys.ShiftKey:
                    baseKeyString = "Shift";
                    break;
                case Keys.ControlKey:
                    baseKeyString = "Ctrl";
                    break;
                case Keys.Menu:
                    baseKeyString = "Alt";
                    break;
                case Keys.PageDown:
                    baseKeyString = "PageDown";
                    break;
                case Keys.PageUp:
                    baseKeyString = "PageUp";
                    break;
                case Keys.Oemcomma:
                    baseKeyString = "Comma";
                    break;
                case Keys.OemPeriod:
                    baseKeyString = "Period";
                    break;
                case Keys.OemQuestion:
                    baseKeyString = "Question";
                    break;
                default:
                    baseKeyString = baseKey.ToString();
                    break;
            }

            hotkeyParts.Add(baseKeyString);

            return string.Join("+", hotkeyParts);
        }

        public static Keys ParseHotkeyString(string hotkeyString)
        {
            Keys result = Keys.None;
            string[] parts = hotkeyString.Split('+');

            foreach (string part in parts)
            {
                // Convert user-friendly names back to relevant Keys
                switch (part.Trim())
                {
                    case "Ctrl":
                        result |= Keys.Control;
                        break;
                    case "Shift":
                        result |= Keys.Shift;
                        break;
                    case "Alt":
                        result |= Keys.Alt;
                        break;
                    case "Backspace":
                        result |= Keys.Back;
                        break;
                    case "Minus":
                        result |= Keys.OemMinus;
                        break;
                    case "Plus":
                        result |= Keys.Oemplus;
                        break;
                    case "PageDown":
                        result |= Keys.PageDown;
                        break;
                    case "PageUp":
                        result |= Keys.PageUp;
                        break;
                    case "Comma":
                        result |= Keys.Oemcomma;
                        break;
                    case "Period":
                        result |= Keys.OemPeriod;
                        break;
                    case "Question":
                        result |= Keys.OemQuestion;
                        break;
                    default:
                        if (Enum.TryParse(part, out Keys key))
                            result |= key;
                        break;
                }
            }

            return result;
        }

        public static bool ValidateHotkeyIniFile(string[] lines, out string invalidKey)
        {
            invalidKey = string.Empty;

            // Track duplicates
            var seenKeys = new HashSet<Keys>();

            // Track required mouse keys existence
            bool foundSelectPieces = false;

            foreach (var line in lines)
            {
                if (string.IsNullOrWhiteSpace(line) || line.StartsWith("["))
                    continue;

                var parts = line.Split('=');
                if (parts.Length != 2)
                    continue;

                string name = parts[0].Trim();
                string key = parts[1].Trim();

                // Skip "None"
                if (string.Equals(key, "None", StringComparison.OrdinalIgnoreCase))
                    continue;

                // Parse the key
                Keys parsedKey = ParseHotkeyString(key);

                // Lookup the hotkey
                HotkeyData hotkey = AllHotkeys.FirstOrDefault(h => h.Name.ToString() == name);
                if (hotkey == null)
                {
                    invalidKey = $"Unknown hotkey '{name}'";
                    return true;
                }

                // Special case for HotkeySelectPieces if needed
                if (hotkey.Name == HotkeyName.HotkeySelectPieces)
                {
                    foundSelectPieces = true;
                    Keys baseKey = parsedKey & ~Keys.Modifiers;
                    if (baseKey != Keys.LButton)
                    {
                        invalidKey = $"HotkeySelectPieces must be LButton, but found '{key}'";
                        return true;
                    }
                }

                // Disallow invalid or modifier-only keys
                if (parsedKey == Keys.None ||
                    parsedKey == Keys.Control || parsedKey == Keys.Shift || parsedKey == Keys.Alt ||
                    (parsedKey & (Keys.Control | Keys.Shift | Keys.Alt)) == parsedKey)
                {
                    invalidKey = $"Invalid key '{key}'";
                    return true;
                }

                // Check for duplicate keys
                if (!seenKeys.Add(parsedKey))
                {
                    invalidKey = $"Duplicate key '{key}'";
                    return true;
                }

                // Check for mandatory mouse buttons
                if (hotkey.RequiresMouseButton)
                {
                    Keys baseKey = parsedKey & ~Keys.Modifiers;
                    if (!MouseKeys.Contains(baseKey))
                    {
                        invalidKey = $"{hotkey.Name} requires a mouse button. Current key is {key}";
                        return true;
                    }
                }
            }

            if (!foundSelectPieces)
            {
                invalidKey = "HotkeySelectPieces is missing from SLXEditorHotkeys.ini";
                return true;
            }

            return false;
        }

        public static void LoadHotkeysFromIniFile()
        {
            if (!File.Exists(C.AppPathHotkeys))
            {
                GetDefaultHotkeys();
                SaveHotkeysToIniFile();
                PlayerHotkeysLoaded = true;
                return;
            }

            var lines = File.ReadAllLines(C.AppPathHotkeys);

            if (ValidateHotkeyIniFile(lines, out string invalidKey))
            {
                MessageBox.Show(
                    $"Invalid or duplicate key detected in SLXEditorHotkeys.ini\n\n" +
                    $"{invalidKey}\n\n" +
                    $"The default hotkeys will be loaded instead",
                    "Hotkey Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);

                GetDefaultHotkeys();
                PlayerHotkeysLoaded = true;
                return;
            }

            // Always start from defaults
            GetDefaultHotkeys();

            foreach (var line in lines)
            {
                if (string.IsNullOrWhiteSpace(line) || line.StartsWith("["))
                    continue;

                var parts = line.Split('=');
                if (parts.Length != 2)
                    continue;

                string name = parts[0].Trim();
                string key = parts[1].Trim();

                HandleHotkeyBackwardsCompatibility(name, key);

                HotkeyName parsedName;
                if (Enum.TryParse(name, out parsedName))
                {
                    var hotkey = AllHotkeys.FirstOrDefault(h => h.Name == parsedName);
                    if (hotkey != null)
                        hotkey.CurrentKeys = ParseHotkeyString(key);
                }
            }

            PlayerHotkeysLoaded = true;
        }

        private static readonly Dictionary<string, HotkeyName> LegacyHotkeyMap = new Dictionary<string, HotkeyName>
        {
            { "HotkeyMove8Up", HotkeyName.HotkeyGridMoveUp },
            { "HotkeyMove8Down", HotkeyName.HotkeyGridMoveDown },
            { "HotkeyMove8Left", HotkeyName.HotkeyGridMoveLeft },
            { "HotkeyMove8Right", HotkeyName.HotkeyGridMoveRight },
            { "HotkeyOpenLevelWindow", HotkeyName.HotkeyOpenLevelArrangerWindow }
        };

        private static void HandleHotkeyBackwardsCompatibility(string oldName, string key)
        {
            if (!LegacyHotkeyMap.TryGetValue(oldName, out HotkeyName newName))
                return;

            HotkeyData hotkey = AllHotkeys.FirstOrDefault(h => h.Name == newName);
            if (hotkey == null)
                return;

            if (hotkey.CurrentKeys == Keys.None)
                hotkey.CurrentKeys = ParseHotkeyString(key);
        }

        public static void SaveHotkeysToIniFile()
        {
            var lines = new List<string>
            {
                "[Hotkeys]"
            };

            foreach (var hotkey in AllHotkeys)
            {
                lines.Add($"{hotkey.Name}={FormatHotkeyString(hotkey.CurrentKeys)}");
            }

            string settingsDirectory = Path.GetDirectoryName(C.AppPathHotkeys);
            if (!Directory.Exists(settingsDirectory))
                Directory.CreateDirectory(settingsDirectory);

            File.WriteAllLines(C.AppPathHotkeys, lines);
        }

        public static void GetDefaultHotkeys()
        {
            foreach (var hotkey in AllHotkeys)
            {
                hotkey.CurrentKeys = hotkey.DefaultKeys;
            }
        }

        public static HotkeyData GetHotkey(HotkeyName name)
        {
            return AllHotkeys.FirstOrDefault(h => h.Name == name);
        }
    }
}
