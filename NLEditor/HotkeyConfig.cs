using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NLEditor
{
    public class HotkeyConfig
    {
        public static Keys HotkeyCreateNewLevel;
        public static Keys HotkeyLoadLevel;
        public static Keys HotkeySaveLevel;
        public static Keys HotkeySaveLevelAs;
        public static Keys HotkeySaveLevelAsImage;
        public static Keys HotkeyPlaytestLevel;
        public static Keys HotkeyValidateLevel;
        public static Keys HotkeyCleanseLevels;
        public static Keys HotkeyToggleClearPhysics;
        public static Keys HotkeyToggleTerrain;
        public static Keys HotkeyToggleObjects;
        public static Keys HotkeyToggleTriggerAreas;
        public static Keys HotkeyToggleScreenStart;
        public static Keys HotkeyToggleBackground;
        public static Keys HotkeyToggleDeprecatedPieces;
        public static Keys HotkeyPieceSearch;
        public static Keys HotkeyShowMissingPieces;
        public static Keys HotkeyToggleSnapToGrid;
        public static Keys HotkeyOpenSettings;
        public static Keys HotkeyOpenConfigHotkeys;
        public static Keys HotkeyOpenAboutSLX;
        public static Keys HotkeySelectPieces;
        public static Keys HotkeyDragToScroll;
        public static Keys HotkeyDragHorizontally;
        public static Keys HotkeyDragVertically;
        public static Keys HotkeyMoveScreenStart;
        public static Keys HotkeyRemovePiecesAtCursor;
        public static Keys HotkeyAddRemoveSinglePiece;
        public static Keys HotkeySelectPiecesBelow;
        public static Keys HotkeyZoomIn;
        public static Keys HotkeyZoomOut;
        public static Keys HotkeyScrollHorizontally;
        public static Keys HotkeyScrollVertically;
        public static Keys HotkeyShowPreviousPiece;
        public static Keys HotkeyShowNextPiece;
        public static Keys HotkeyShowPreviousGroup;
        public static Keys HotkeyShowNextGroup;
        public static Keys HotkeyShowPreviousStyle;
        public static Keys HotkeyShowNextStyle;
        public static Keys HotkeyCycleBrowser;
        public static Keys HotkeyAddPiece1;
        public static Keys HotkeyAddPiece2;
        public static Keys HotkeyAddPiece3;
        public static Keys HotkeyAddPiece4;
        public static Keys HotkeyAddPiece5;
        public static Keys HotkeyAddPiece6;
        public static Keys HotkeyAddPiece7;
        public static Keys HotkeyAddPiece8;
        public static Keys HotkeyAddPiece9;
        public static Keys HotkeyAddPiece10;
        public static Keys HotkeyAddPiece11;
        public static Keys HotkeyAddPiece12;
        public static Keys HotkeyAddPiece13;
        public static Keys HotkeyUndo;
        public static Keys HotkeyRedo;
        public static Keys HotkeySelectAll;
        public static Keys HotkeyCut;
        public static Keys HotkeyCopy;
        public static Keys HotkeyPaste;
        public static Keys HotkeyPasteInPlace;
        public static Keys HotkeyDuplicate;
        public static Keys HotkeyDelete;
        public static Keys HotkeyMoveUp;
        public static Keys HotkeyMoveDown;
        public static Keys HotkeyMoveLeft;
        public static Keys HotkeyMoveRight;
        public static Keys HotkeyMove8Up;
        public static Keys HotkeyMove8Down;
        public static Keys HotkeyMove8Left;
        public static Keys HotkeyMove8Right;
        public static Keys HotkeyCustomMoveUp;
        public static Keys HotkeyCustomMoveDown;
        public static Keys HotkeyCustomMoveLeft;
        public static Keys HotkeyCustomMoveRight;
        public static Keys HotkeyRotate;
        public static Keys HotkeyFlip;
        public static Keys HotkeyInvert;
        public static Keys HotkeyGroup;
        public static Keys HotkeyUngroup;
        public static Keys HotkeyErase;
        public static Keys HotkeyNoOverwrite;
        public static Keys HotkeyOnlyOnTerrain;
        public static Keys HotkeyAllowOneWay;
        public static Keys HotkeyDrawLast;
        public static Keys HotkeyDrawSooner;
        public static Keys HotkeyDrawLater;
        public static Keys HotkeyDrawFirst;
        public static Keys HotkeyCloseEditor;

        // For mandatory mouse hotkeys
        public static readonly List<string> mandatoryMouseHotkeyText = new List<string>
        {
            "HotkeySelectPieces",
            "HotkeyDragToScroll",
            "HotkeyDragHorizontally",
            "HotkeyDragVertically",
            "HotkeyAddRemoveSinglePiece",
            "HotkeyRemovePiecesAtCursor",
            "HotkeySelectPiecesBelow"
        };

        public static readonly List<Keys> mandatoryMouseKeys = new List<Keys>
        {
            Keys.LButton,
            Keys.RButton,
            Keys.MButton,
            Keys.XButton1,
            Keys.XButton2
        };

        public static readonly List<ListViewItem> mouseMandatoryItems = new List<ListViewItem>();

        public static bool defaultHotkeysLoaded = false;
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
            var seenKeys = new HashSet<string>();
            invalidKey = string.Empty;

            foreach (var line in lines)
            {
                // Extract the key-value pair
                var parts = line.Split('=');
                if (parts.Length < 2) continue;

                var settingName = parts[0].Trim();
                var key = parts[1].Trim();

                // Skip empty or "None" values
                if (key == "None") continue;

                var parsedKey = ParseHotkeyString(key);

                // Check for invalid or disallowed keys
                if (parsedKey == Keys.None || key.Contains("None") ||
                    parsedKey == Keys.Control || parsedKey == Keys.Shift || parsedKey == Keys.Alt ||
                    (parsedKey & (Keys.Control | Keys.Shift | Keys.Alt)) == parsedKey)
                {
                    invalidKey = key.Length > 100 ? key.Substring(0, 100) + "..." : key;
                    return true;
                }

                // Check for duplicate keys
                if (!seenKeys.Add(key))
                {
                    invalidKey = key.Length > 100 ? key.Substring(0, 100) + "..." : key;
                    return true;
                }

                // Check for mandatory mouse keys
                Keys baseKey = parsedKey & ~Keys.Modifiers;

                if (mandatoryMouseHotkeyText.Contains(settingName) && !mandatoryMouseKeys.Contains(baseKey))
                {
                    invalidKey = $"{settingName} requires a mouse button key. Current key is {parsedKey}";
                    return true;
                }
            }

            return false;
        }


        public static void LoadHotkeysFromIniFile()
        {
            var lines = System.IO.File.ReadAllLines(C.AppPathHotkeys);

            if (ValidateHotkeyIniFile(lines, out string invalidKey))
            {
                MessageBox.Show($"Invalid or duplicate key detected in SLXEditorHotkeys.ini\n\n" +
                                $"{invalidKey}\n\n" +
                                $"The default hotkeys will be loaded instead",
                                "Hotkey Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                
                GetDefaultHotkeys();
                defaultHotkeysLoaded = true;
                
                return;
            }

            foreach (var line in lines)
            {
                if (line.StartsWith("HotkeyCreateNewLevel="))
                    HotkeyCreateNewLevel = ParseHotkeyString(line.Substring("HotkeyCreateNewLevel=".Length));
                if (line.StartsWith("HotkeyLoadLevel="))
                    HotkeyLoadLevel = ParseHotkeyString(line.Substring("HotkeyLoadLevel=".Length));
                if (line.StartsWith("HotkeySaveLevel="))
                    HotkeySaveLevel = ParseHotkeyString(line.Substring("HotkeySaveLevel=".Length));
                if (line.StartsWith("HotkeySaveLevelAs="))
                    HotkeySaveLevelAs = ParseHotkeyString(line.Substring("HotkeySaveLevelAs=".Length));
                if (line.StartsWith("HotkeySaveLevelAsImage="))
                    HotkeySaveLevelAsImage = ParseHotkeyString(line.Substring("HotkeySaveLevelAsImage=".Length));
                if (line.StartsWith("HotkeyPlaytestLevel="))
                    HotkeyPlaytestLevel = ParseHotkeyString(line.Substring("HotkeyPlaytestLevel=".Length));
                if (line.StartsWith("HotkeyValidateLevel="))
                    HotkeyValidateLevel = ParseHotkeyString(line.Substring("HotkeyValidateLevel=".Length));
                if (line.StartsWith("HotkeyCleanseLevels="))
                    HotkeyCleanseLevels = ParseHotkeyString(line.Substring("HotkeyCleanseLevels=".Length));
                if (line.StartsWith("HotkeyToggleClearPhysics="))
                    HotkeyToggleClearPhysics = ParseHotkeyString(line.Substring("HotkeyToggleClearPhysics=".Length));
                if (line.StartsWith("HotkeyToggleTerrain="))
                    HotkeyToggleTerrain = ParseHotkeyString(line.Substring("HotkeyToggleTerrain=".Length));
                if (line.StartsWith("HotkeyToggleObjects="))
                    HotkeyToggleObjects = ParseHotkeyString(line.Substring("HotkeyToggleObjects=".Length));
                if (line.StartsWith("HotkeyToggleTriggerAreas="))
                    HotkeyToggleTriggerAreas = ParseHotkeyString(line.Substring("HotkeyToggleTriggerAreas=".Length));
                if (line.StartsWith("HotkeyToggleScreenStart="))
                    HotkeyToggleScreenStart = ParseHotkeyString(line.Substring("HotkeyToggleScreenStart=".Length));
                if (line.StartsWith("HotkeyToggleBackground="))
                    HotkeyToggleBackground = ParseHotkeyString(line.Substring("HotkeyToggleBackground=".Length));
                if (line.StartsWith("HotkeyToggleDeprecatedPieces="))
                    HotkeyToggleDeprecatedPieces = ParseHotkeyString(line.Substring("HotkeyToggleDeprecatedPieces=".Length));
                if (line.StartsWith("HotkeyPieceSearch="))
                    HotkeyPieceSearch = ParseHotkeyString(line.Substring("HotkeyPieceSearch=".Length));
                if (line.StartsWith("HotkeyShowMissingPieces="))
                    HotkeyShowMissingPieces = ParseHotkeyString(line.Substring("HotkeyShowMissingPieces=".Length));
                if (line.StartsWith("HotkeyToggleSnapToGrid="))
                    HotkeyToggleSnapToGrid = ParseHotkeyString(line.Substring("HotkeyToggleSnapToGrid=".Length));
                if (line.StartsWith("HotkeyOpenSettings="))
                    HotkeyOpenSettings = ParseHotkeyString(line.Substring("HotkeyOpenSettings=".Length));
                if (line.StartsWith("HotkeyOpenConfigHotkeys="))
                    HotkeyOpenConfigHotkeys = ParseHotkeyString(line.Substring("HotkeyOpenConfigHotkeys=".Length));
                if (line.StartsWith("HotkeyOpenAboutSLX="))
                    HotkeyOpenAboutSLX = ParseHotkeyString(line.Substring("HotkeyOpenAboutSLX=".Length));
                if (line.StartsWith("HotkeySelectPieces="))
                    HotkeySelectPieces = ParseHotkeyString(line.Substring("HotkeySelectPieces=".Length));
                if (line.StartsWith("HotkeyDragToScroll="))
                    HotkeyDragToScroll = ParseHotkeyString(line.Substring("HotkeyDragToScroll=".Length));
                if (line.StartsWith("HotkeyDragHorizontally="))
                    HotkeyDragHorizontally = ParseHotkeyString(line.Substring("HotkeyDragHorizontally=".Length));
                if (line.StartsWith("HotkeyDragVertically="))
                    HotkeyDragVertically = ParseHotkeyString(line.Substring("HotkeyDragVertically=".Length));
                if (line.StartsWith("HotkeyMoveScreenStart="))
                    HotkeyMoveScreenStart = ParseHotkeyString(line.Substring("HotkeyMoveScreenStart=".Length));
                if (line.StartsWith("HotkeyRemovePiecesAtCursor="))
                    HotkeyRemovePiecesAtCursor = ParseHotkeyString(line.Substring("HotkeyRemovePiecesAtCursor=".Length));
                if (line.StartsWith("HotkeyAddRemoveSinglePiece="))
                    HotkeyAddRemoveSinglePiece = ParseHotkeyString(line.Substring("HotkeyAddRemoveSinglePiece=".Length));
                if (line.StartsWith("HotkeySelectPiecesBelow="))
                    HotkeySelectPiecesBelow = ParseHotkeyString(line.Substring("HotkeySelectPiecesBelow=".Length));
                if (line.StartsWith("HotkeyZoomIn="))
                    HotkeyZoomIn = ParseHotkeyString(line.Substring("HotkeyZoomIn=".Length));
                if (line.StartsWith("HotkeyZoomOut="))
                    HotkeyZoomOut = ParseHotkeyString(line.Substring("HotkeyZoomOut=".Length));
                if (line.StartsWith("HotkeyScrollHorizontally="))
                    HotkeyScrollHorizontally = ParseHotkeyString(line.Substring("HotkeyScrollHorizontally=".Length));
                if (line.StartsWith("HotkeyScrollVertically="))
                    HotkeyScrollVertically = ParseHotkeyString(line.Substring("HotkeyScrollVertically=".Length));
                if (line.StartsWith("HotkeyShowPreviousPiece="))
                    HotkeyShowPreviousPiece = ParseHotkeyString(line.Substring("HotkeyShowPreviousPiece=".Length));
                if (line.StartsWith("HotkeyShowNextPiece="))
                    HotkeyShowNextPiece = ParseHotkeyString(line.Substring("HotkeyShowNextPiece=".Length));
                if (line.StartsWith("HotkeyShowPreviousGroup="))
                    HotkeyShowPreviousGroup = ParseHotkeyString(line.Substring("HotkeyShowPreviousGroup=".Length));
                if (line.StartsWith("HotkeyShowNextGroup="))
                    HotkeyShowNextGroup = ParseHotkeyString(line.Substring("HotkeyShowNextGroup=".Length));
                if (line.StartsWith("HotkeyShowPreviousStyle="))
                    HotkeyShowPreviousStyle = ParseHotkeyString(line.Substring("HotkeyShowPreviousStyle=".Length));
                if (line.StartsWith("HotkeyShowNextStyle="))
                    HotkeyShowNextStyle = ParseHotkeyString(line.Substring("HotkeyShowNextStyle=".Length));
                if (line.StartsWith("HotkeyCycleBrowser="))
                    HotkeyCycleBrowser = ParseHotkeyString(line.Substring("HotkeyCycleBrowser=".Length));
                if (line.StartsWith("HotkeyAddPiece1="))
                    HotkeyAddPiece1 = ParseHotkeyString(line.Substring("HotkeyAddPiece1=".Length));
                if (line.StartsWith("HotkeyAddPiece2="))
                    HotkeyAddPiece2 = ParseHotkeyString(line.Substring("HotkeyAddPiece2=".Length));
                if (line.StartsWith("HotkeyAddPiece3="))
                    HotkeyAddPiece3 = ParseHotkeyString(line.Substring("HotkeyAddPiece3=".Length));
                if (line.StartsWith("HotkeyAddPiece4="))
                    HotkeyAddPiece4 = ParseHotkeyString(line.Substring("HotkeyAddPiece4=".Length));
                if (line.StartsWith("HotkeyAddPiece5="))
                    HotkeyAddPiece5 = ParseHotkeyString(line.Substring("HotkeyAddPiece5=".Length));
                if (line.StartsWith("HotkeyAddPiece6="))
                    HotkeyAddPiece6 = ParseHotkeyString(line.Substring("HotkeyAddPiece6=".Length));
                if (line.StartsWith("HotkeyAddPiece7="))
                    HotkeyAddPiece7 = ParseHotkeyString(line.Substring("HotkeyAddPiece7=".Length));
                if (line.StartsWith("HotkeyAddPiece8="))
                    HotkeyAddPiece8 = ParseHotkeyString(line.Substring("HotkeyAddPiece8=".Length));
                if (line.StartsWith("HotkeyAddPiece9="))
                    HotkeyAddPiece9 = ParseHotkeyString(line.Substring("HotkeyAddPiece9=".Length));
                if (line.StartsWith("HotkeyAddPiece10="))
                    HotkeyAddPiece10 = ParseHotkeyString(line.Substring("HotkeyAddPiece10=".Length));
                if (line.StartsWith("HotkeyAddPiece11="))
                    HotkeyAddPiece11 = ParseHotkeyString(line.Substring("HotkeyAddPiece11=".Length));
                if (line.StartsWith("HotkeyAddPiece12="))
                    HotkeyAddPiece12 = ParseHotkeyString(line.Substring("HotkeyAddPiece12=".Length));
                if (line.StartsWith("HotkeyAddPiece13="))
                    HotkeyAddPiece13 = ParseHotkeyString(line.Substring("HotkeyAddPiece13=".Length));
                if (line.StartsWith("HotkeyUndo="))
                    HotkeyUndo = ParseHotkeyString(line.Substring("HotkeyUndo=".Length));
                if (line.StartsWith("HotkeyRedo="))
                    HotkeyRedo = ParseHotkeyString(line.Substring("HotkeyRedo=".Length));
                if (line.StartsWith("HotkeySelectAll="))
                    HotkeySelectAll = ParseHotkeyString(line.Substring("HotkeySelectAll=".Length));
                if (line.StartsWith("HotkeyCut="))
                    HotkeyCut = ParseHotkeyString(line.Substring("HotkeyCut=".Length));
                if (line.StartsWith("HotkeyCopy="))
                    HotkeyCopy = ParseHotkeyString(line.Substring("HotkeyCopy=".Length));
                if (line.StartsWith("HotkeyPaste="))
                    HotkeyPaste = ParseHotkeyString(line.Substring("HotkeyPaste=".Length));
                if (line.StartsWith("HotkeyPasteInPlace"))
                    HotkeyPasteInPlace = ParseHotkeyString(line.Substring("HotkeyPasteInPlace=".Length));
                if (line.StartsWith("HotkeyDuplicate="))
                    HotkeyDuplicate = ParseHotkeyString(line.Substring("HotkeyDuplicate=".Length));
                if (line.StartsWith("HotkeyDelete="))
                    HotkeyDelete = ParseHotkeyString(line.Substring("HotkeyDelete=".Length));
                if (line.StartsWith("HotkeyMoveUp="))
                    HotkeyMoveUp = ParseHotkeyString(line.Substring("HotkeyMoveUp=".Length));
                if (line.StartsWith("HotkeyMoveDown="))
                    HotkeyMoveDown = ParseHotkeyString(line.Substring("HotkeyMoveDown=".Length));
                if (line.StartsWith("HotkeyMoveLeft="))
                    HotkeyMoveLeft = ParseHotkeyString(line.Substring("HotkeyMoveLeft=".Length));
                if (line.StartsWith("HotkeyMoveRight="))
                    HotkeyMoveRight = ParseHotkeyString(line.Substring("HotkeyMoveRight=".Length));
                if (line.StartsWith("HotkeyMove8Up="))
                    HotkeyMove8Up = ParseHotkeyString(line.Substring("HotkeyMove8Up=".Length));
                if (line.StartsWith("HotkeyMove8Down="))
                    HotkeyMove8Down = ParseHotkeyString(line.Substring("HotkeyMove8Down=".Length));
                if (line.StartsWith("HotkeyMove8Left="))
                    HotkeyMove8Left = ParseHotkeyString(line.Substring("HotkeyMove8Left=".Length));
                if (line.StartsWith("HotkeyMove8Right="))
                    HotkeyMove8Right = ParseHotkeyString(line.Substring("HotkeyMove8Right=".Length));
                if (line.StartsWith("HotkeyCustomMoveUp="))
                    HotkeyCustomMoveUp = ParseHotkeyString(line.Substring("HotkeyCustomMoveUp=".Length));
                if (line.StartsWith("HotkeyCustomMoveDown="))
                    HotkeyCustomMoveDown = ParseHotkeyString(line.Substring("HotkeyCustomMoveDown=".Length));
                if (line.StartsWith("HotkeyCustomMoveLeft="))
                    HotkeyCustomMoveLeft = ParseHotkeyString(line.Substring("HotkeyCustomMoveLeft=".Length));
                if (line.StartsWith("HotkeyCustomMoveRight="))
                    HotkeyCustomMoveRight = ParseHotkeyString(line.Substring("HotkeyCustomMoveRight=".Length));
                if (line.StartsWith("HotkeyRotate="))
                    HotkeyRotate = ParseHotkeyString(line.Substring("HotkeyRotate=".Length));
                if (line.StartsWith("HotkeyFlip="))
                    HotkeyFlip = ParseHotkeyString(line.Substring("HotkeyFlip=".Length));
                if (line.StartsWith("HotkeyInvert="))
                    HotkeyInvert = ParseHotkeyString(line.Substring("HotkeyInvert=".Length));
                if (line.StartsWith("HotkeyGroup="))
                    HotkeyGroup = ParseHotkeyString(line.Substring("HotkeyGroup=".Length));
                if (line.StartsWith("HotkeyUngroup="))
                    HotkeyUngroup = ParseHotkeyString(line.Substring("HotkeyUngroup=".Length));
                if (line.StartsWith("HotkeyErase="))
                    HotkeyErase = ParseHotkeyString(line.Substring("HotkeyErase=".Length));
                if (line.StartsWith("HotkeyNoOverwrite="))
                    HotkeyNoOverwrite = ParseHotkeyString(line.Substring("HotkeyNoOverwrite=".Length));
                if (line.StartsWith("HotkeyOnlyOnTerrain="))
                    HotkeyOnlyOnTerrain = ParseHotkeyString(line.Substring("HotkeyOnlyOnTerrain=".Length));
                if (line.StartsWith("HotkeyAllowOneWay="))
                    HotkeyAllowOneWay = ParseHotkeyString(line.Substring("HotkeyAllowOneWay=".Length));
                if (line.StartsWith("HotkeyDrawLast="))
                    HotkeyDrawLast = ParseHotkeyString(line.Substring("HotkeyDrawLast=".Length));
                if (line.StartsWith("HotkeyDrawSooner="))
                    HotkeyDrawSooner = ParseHotkeyString(line.Substring("HotkeyDrawSooner=".Length));
                if (line.StartsWith("HotkeyDrawLater="))
                    HotkeyDrawLater = ParseHotkeyString(line.Substring("HotkeyDrawLater=".Length));
                if (line.StartsWith("HotkeyDrawFirst="))
                    HotkeyDrawFirst = ParseHotkeyString(line.Substring("HotkeyDrawFirst=".Length));
                if (line.StartsWith("HotkeyCloseEditor="))
                    HotkeyCloseEditor = ParseHotkeyString(line.Substring("HotkeyCloseEditor=".Length));
            }
        }

        public static void SaveHotkeysToIniFile()
        {
            var lines = new List<string>
            {
                "[Hotkeys]",
                $"HotkeyCreateNewLevel={FormatHotkeyString(HotkeyCreateNewLevel)}",
                $"HotkeyLoadLevel={FormatHotkeyString(HotkeyLoadLevel)}",
                $"HotkeySaveLevel={FormatHotkeyString(HotkeySaveLevel)}",
                $"HotkeySaveLevelAs={FormatHotkeyString(HotkeySaveLevelAs)}",
                $"HotkeySaveLevelAsImage={FormatHotkeyString(HotkeySaveLevelAsImage)}",
                $"HotkeyPlaytestLevel={FormatHotkeyString(HotkeyPlaytestLevel)}",
                $"HotkeyValidateLevel={FormatHotkeyString(HotkeyValidateLevel)}",
                $"HotkeyCleanseLevels={FormatHotkeyString(HotkeyCleanseLevels)}",
                $"HotkeyToggleClearPhysics={FormatHotkeyString(HotkeyToggleClearPhysics)}",
                $"HotkeyToggleTerrain={FormatHotkeyString(HotkeyToggleTerrain)}",
                $"HotkeyToggleObjects={FormatHotkeyString(HotkeyToggleObjects)}",
                $"HotkeyToggleTriggerAreas={FormatHotkeyString(HotkeyToggleTriggerAreas)}",
                $"HotkeyToggleScreenStart={FormatHotkeyString(HotkeyToggleScreenStart)}",
                $"HotkeyToggleBackground={FormatHotkeyString(HotkeyToggleBackground)}",
                $"HotkeyToggleDeprecatedPieces={FormatHotkeyString(HotkeyToggleDeprecatedPieces)}",
                $"HotkeyPieceSearch={FormatHotkeyString(HotkeyPieceSearch)}",
                $"HotkeyShowMissingPieces={FormatHotkeyString(HotkeyShowMissingPieces)}",
                $"HotkeyToggleSnapToGrid={FormatHotkeyString(HotkeyToggleSnapToGrid)}",
                $"HotkeyOpenSettings={FormatHotkeyString(HotkeyOpenSettings)}",
                $"HotkeyOpenConfigHotkeys={FormatHotkeyString(HotkeyOpenConfigHotkeys)}",
                $"HotkeyOpenAboutSLX={FormatHotkeyString(HotkeyOpenAboutSLX)}",
                $"HotkeySelectPieces={FormatHotkeyString(HotkeySelectPieces)}",
                $"HotkeyDragToScroll={FormatHotkeyString(HotkeyDragToScroll)}",
                $"HotkeyDragHorizontally={FormatHotkeyString(HotkeyDragHorizontally)}",
                $"HotkeyDragVertically={FormatHotkeyString(HotkeyDragVertically)}",
                $"HotkeyMoveScreenStart={FormatHotkeyString(HotkeyMoveScreenStart)}",
                $"HotkeyRemovePiecesAtCursor={FormatHotkeyString(HotkeyRemovePiecesAtCursor)}",
                $"HotkeyAddRemoveSinglePiece={FormatHotkeyString(HotkeyAddRemoveSinglePiece)}",
                $"HotkeySelectPiecesBelow={FormatHotkeyString(HotkeySelectPiecesBelow)}",
                $"HotkeyZoomIn={FormatHotkeyString(HotkeyZoomIn)}",
                $"HotkeyZoomOut={FormatHotkeyString(HotkeyZoomOut)}",
                $"HotkeyScrollHorizontally={FormatHotkeyString(HotkeyScrollHorizontally)}",
                $"HotkeyScrollVertically={FormatHotkeyString(HotkeyScrollVertically)}",
                $"HotkeyShowPreviousPiece={FormatHotkeyString(HotkeyShowPreviousPiece)}",
                $"HotkeyShowNextPiece={FormatHotkeyString(HotkeyShowNextPiece)}",
                $"HotkeyShowPreviousGroup={FormatHotkeyString(HotkeyShowPreviousGroup)}",
                $"HotkeyShowNextGroup={FormatHotkeyString(HotkeyShowNextGroup)}",
                $"HotkeyShowPreviousStyle={FormatHotkeyString(HotkeyShowPreviousStyle)}",
                $"HotkeyShowNextStyle={FormatHotkeyString(HotkeyShowNextStyle)}",
                $"HotkeyCycleBrowser={FormatHotkeyString(HotkeyCycleBrowser)}",
                $"HotkeyAddPiece1={FormatHotkeyString(HotkeyAddPiece1)}",
                $"HotkeyAddPiece2={FormatHotkeyString(HotkeyAddPiece2)}",
                $"HotkeyAddPiece3={FormatHotkeyString(HotkeyAddPiece3)}",
                $"HotkeyAddPiece4={FormatHotkeyString(HotkeyAddPiece4)}",
                $"HotkeyAddPiece5={FormatHotkeyString(HotkeyAddPiece5)}",
                $"HotkeyAddPiece6={FormatHotkeyString(HotkeyAddPiece6)}",
                $"HotkeyAddPiece7={FormatHotkeyString(HotkeyAddPiece7)}",
                $"HotkeyAddPiece8={FormatHotkeyString(HotkeyAddPiece8)}",
                $"HotkeyAddPiece9={FormatHotkeyString(HotkeyAddPiece9)}",
                $"HotkeyAddPiece10={FormatHotkeyString(HotkeyAddPiece10)}",
                $"HotkeyAddPiece11={FormatHotkeyString(HotkeyAddPiece11)}",
                $"HotkeyAddPiece12={FormatHotkeyString(HotkeyAddPiece12)}",
                $"HotkeyAddPiece13={FormatHotkeyString(HotkeyAddPiece13)}",
                $"HotkeyUndo={FormatHotkeyString(HotkeyUndo)}",
                $"HotkeyRedo={FormatHotkeyString(HotkeyRedo)}",
                $"HotkeySelectAll={FormatHotkeyString(HotkeySelectAll)}",
                $"HotkeyCut={FormatHotkeyString(HotkeyCut)}",
                $"HotkeyCopy={FormatHotkeyString(HotkeyCopy)}",
                $"HotkeyPaste={FormatHotkeyString(HotkeyPaste)}",
                $"HotkeyPasteInPlace={FormatHotkeyString(HotkeyPasteInPlace)}",
                $"HotkeyDuplicate={FormatHotkeyString(HotkeyDuplicate)}",
                $"HotkeyDelete={FormatHotkeyString(HotkeyDelete)}",
                $"HotkeyMoveUp={FormatHotkeyString(HotkeyMoveUp)}",
                $"HotkeyMoveDown={FormatHotkeyString(HotkeyMoveDown)}",
                $"HotkeyMoveLeft={FormatHotkeyString(HotkeyMoveLeft)}",
                $"HotkeyMoveRight={FormatHotkeyString(HotkeyMoveRight)}",
                $"HotkeyMove8Up={FormatHotkeyString(HotkeyMove8Up)}",
                $"HotkeyMove8Down={FormatHotkeyString(HotkeyMove8Down)}",
                $"HotkeyMove8Left={FormatHotkeyString(HotkeyMove8Left)}",
                $"HotkeyMove8Right={FormatHotkeyString(HotkeyMove8Right)}",
                $"HotkeyCustomMoveUp={FormatHotkeyString(HotkeyCustomMoveUp)}",
                $"HotkeyCustomMoveDown={FormatHotkeyString(HotkeyCustomMoveDown)}",
                $"HotkeyCustomMoveLeft={FormatHotkeyString(HotkeyCustomMoveLeft)}",
                $"HotkeyCustomMoveRight={FormatHotkeyString(HotkeyCustomMoveRight)}",
                $"HotkeyRotate={FormatHotkeyString(HotkeyRotate)}",
                $"HotkeyFlip={FormatHotkeyString(HotkeyFlip)}",
                $"HotkeyInvert={FormatHotkeyString(HotkeyInvert)}",
                $"HotkeyGroup={FormatHotkeyString(HotkeyGroup)}",
                $"HotkeyUngroup={FormatHotkeyString(HotkeyUngroup)}",
                $"HotkeyErase={FormatHotkeyString(HotkeyErase)}",
                $"HotkeyNoOverwrite={FormatHotkeyString(HotkeyNoOverwrite)}",
                $"HotkeyOnlyOnTerrain={FormatHotkeyString(HotkeyOnlyOnTerrain)}",
                $"HotkeyAllowOneWay={FormatHotkeyString(HotkeyAllowOneWay)}",
                $"HotkeyDrawLast={FormatHotkeyString(HotkeyDrawLast)}",
                $"HotkeyDrawSooner={FormatHotkeyString(HotkeyDrawSooner)}",
                $"HotkeyDrawLater={FormatHotkeyString(HotkeyDrawLater)}",
                $"HotkeyDrawFirst={FormatHotkeyString(HotkeyDrawFirst)}",
                $"HotkeyCloseEditor={FormatHotkeyString(HotkeyCloseEditor)}"
            };

            string settingsDirectory = Path.GetDirectoryName(C.AppPathHotkeys);
            if (!Directory.Exists(settingsDirectory))
            {
                Directory.CreateDirectory(settingsDirectory);
            }

            System.IO.File.WriteAllLines(C.AppPathHotkeys, lines);
        }

        public static void GetDefaultHotkeys()
        {
            HotkeyCreateNewLevel = Keys.Control | Keys.N;
            HotkeyLoadLevel = Keys.Control | Keys.O;
            HotkeySaveLevel = Keys.Control | Keys.S;
            HotkeySaveLevelAs = Keys.Control | Keys.Shift | Keys.S;
            HotkeySaveLevelAsImage = Keys.Control | Keys.Alt | Keys.S;
            HotkeyPlaytestLevel = Keys.F12;
            HotkeyValidateLevel = Keys.Control | Keys.F12;
            HotkeyCleanseLevels = Keys.Control | Keys.Shift | Keys.F12;
            HotkeyToggleClearPhysics = Keys.F1;
            HotkeyToggleTerrain = Keys.F2;
            HotkeyToggleObjects = Keys.F3;
            HotkeyToggleTriggerAreas = Keys.F4;
            HotkeyToggleScreenStart = Keys.F5;
            HotkeyToggleBackground = Keys.F6;
            HotkeyToggleDeprecatedPieces = Keys.F7;
            HotkeyPieceSearch = Keys.F8;
            HotkeyShowMissingPieces = Keys.Control | Keys.F8;
            HotkeyToggleSnapToGrid = Keys.F9;
            HotkeyOpenSettings = Keys.F10;
            HotkeyOpenConfigHotkeys = Keys.F11;
            HotkeyOpenAboutSLX = Keys.Control | Keys.F10;
            HotkeySelectPieces = Keys.LButton;
            HotkeyDragToScroll = Keys.RButton;
            HotkeyDragHorizontally = Keys.Control | Keys.Alt | Keys.LButton;
            HotkeyDragVertically = Keys.Control | Keys.Shift | Keys.LButton;
            HotkeyMoveScreenStart = Keys.P;
            HotkeyRemovePiecesAtCursor = Keys.MButton;
            HotkeyAddRemoveSinglePiece = Keys.Control | Keys.LButton;
            HotkeySelectPiecesBelow = Keys.Alt | Keys.LButton;
            HotkeyZoomIn = Keys.Oemplus;
            HotkeyZoomOut = Keys.OemMinus;
            HotkeyScrollHorizontally = Keys.H;
            HotkeyScrollVertically = Keys.V;
            HotkeyShowPreviousPiece = Keys.Shift | Keys.Left;
            HotkeyShowNextPiece = Keys.Shift | Keys.Right;
            HotkeyShowPreviousGroup = Keys.Shift | Keys.Alt | Keys.Left;
            HotkeyShowNextGroup = Keys.Shift | Keys.Alt | Keys.Right;
            HotkeyShowPreviousStyle = Keys.Shift | Keys.Up;
            HotkeyShowNextStyle = Keys.Shift | Keys.Down;
            HotkeyCycleBrowser = Keys.Space;
            HotkeyAddPiece1 = Keys.D1;
            HotkeyAddPiece2 = Keys.D2;
            HotkeyAddPiece3 = Keys.D3;
            HotkeyAddPiece4 = Keys.D4;
            HotkeyAddPiece5 = Keys.D5;
            HotkeyAddPiece6 = Keys.D6;
            HotkeyAddPiece7 = Keys.D7;
            HotkeyAddPiece8 = Keys.D8;
            HotkeyAddPiece9 = Keys.D9;
            HotkeyAddPiece10 = Keys.D0;
            HotkeyAddPiece11 = Keys.NumPad1;
            HotkeyAddPiece12 = Keys.NumPad2;
            HotkeyAddPiece13 = Keys.NumPad3; // Unassigned by default
            HotkeyUndo = Keys.Control | Keys.Z;
            HotkeyRedo = Keys.Control | Keys.Y;
            HotkeySelectAll = Keys.Control | Keys.A;
            HotkeyCut = Keys.Control | Keys.X;
            HotkeyCopy = Keys.Control | Keys.C;
            HotkeyPaste = Keys.Control | Keys.V;
            HotkeyPasteInPlace = Keys.Control | Keys.Shift | Keys.V;
            HotkeyDuplicate = Keys.D;
            HotkeyDelete = Keys.Delete;
            HotkeyMoveUp = Keys.Up;
            HotkeyMoveDown = Keys.Down;
            HotkeyMoveLeft = Keys.Left;
            HotkeyMoveRight = Keys.Right;
            HotkeyMove8Up = Keys.Control | Keys.Up;
            HotkeyMove8Down = Keys.Control | Keys.Down;
            HotkeyMove8Left = Keys.Control | Keys.Left;
            HotkeyMove8Right = Keys.Control | Keys.Right;
            HotkeyCustomMoveUp = Keys.Alt | Keys.Up;
            HotkeyCustomMoveDown = Keys.Alt | Keys.Down;
            HotkeyCustomMoveLeft = Keys.Alt | Keys.Left;
            HotkeyCustomMoveRight = Keys.Alt | Keys.Right;
            HotkeyRotate = Keys.R;
            HotkeyFlip = Keys.F;
            HotkeyInvert = Keys.I;
            HotkeyGroup = Keys.G;
            HotkeyUngroup = Keys.U;
            HotkeyErase = Keys.E;
            HotkeyNoOverwrite = Keys.N;
            HotkeyOnlyOnTerrain = Keys.T;
            HotkeyAllowOneWay = Keys.O;
            HotkeyDrawLast = Keys.Home;
            HotkeyDrawSooner = Keys.PageUp;
            HotkeyDrawLater = Keys.PageDown;
            HotkeyDrawFirst = Keys.End;
            HotkeyCloseEditor = Keys.Alt | Keys.F4;
        }

        public static void GetClassicHotkeys()
        {
            HotkeyCreateNewLevel = Keys.Control | Keys.N;
            HotkeyLoadLevel = Keys.Control | Keys.O;
            HotkeySaveLevel = Keys.Control | Keys.S;
            HotkeySaveLevelAs = Keys.Control | Keys.Shift | Keys.S;
            HotkeySaveLevelAsImage = Keys.Control | Keys.Alt | Keys.S;
            HotkeyPlaytestLevel = Keys.F12;
            HotkeyValidateLevel = Keys.Control | Keys.F12;
            HotkeyCleanseLevels = Keys.Control | Keys.Shift | Keys.F12;
            HotkeyToggleClearPhysics = Keys.F1;
            HotkeyToggleTerrain = Keys.F2;
            HotkeyToggleObjects = Keys.F3;
            HotkeyToggleTriggerAreas = Keys.F4;
            HotkeyToggleScreenStart = Keys.F5;
            HotkeyToggleBackground = Keys.F6;
            HotkeyToggleDeprecatedPieces = Keys.F7;
            HotkeyPieceSearch = Keys.F8;
            HotkeyShowMissingPieces = Keys.Control | Keys.F8;
            HotkeyToggleSnapToGrid = Keys.F9;
            HotkeyOpenSettings = Keys.F10;
            HotkeyOpenConfigHotkeys = Keys.F11;
            HotkeyOpenAboutSLX = Keys.Control | Keys.F10;
            HotkeySelectPieces = Keys.LButton;
            HotkeyDragToScroll = Keys.RButton;
            HotkeyDragHorizontally = Keys.Control | Keys.Alt | Keys.LButton;
            HotkeyDragVertically = Keys.Control | Keys.Shift | Keys.LButton;
            HotkeyMoveScreenStart = Keys.P;
            HotkeyRemovePiecesAtCursor = Keys.MButton;
            HotkeyAddRemoveSinglePiece = Keys.Control | Keys.LButton;
            HotkeySelectPiecesBelow = Keys.Alt | Keys.LButton;
            HotkeyZoomIn = Keys.Oemplus;
            HotkeyZoomOut = Keys.OemMinus;
            HotkeyScrollHorizontally = Keys.H;
            HotkeyScrollVertically = Keys.V;
            HotkeyShowPreviousPiece = Keys.Shift | Keys.Left;
            HotkeyShowNextPiece = Keys.Shift | Keys.Right;
            HotkeyShowPreviousGroup = Keys.Shift | Keys.Alt | Keys.Left;
            HotkeyShowNextGroup = Keys.Shift | Keys.Alt | Keys.Right;
            HotkeyShowPreviousStyle = Keys.Shift | Keys.Up;
            HotkeyShowNextStyle = Keys.Shift | Keys.Down;
            HotkeyCycleBrowser = Keys.Space;
            HotkeyAddPiece1 = Keys.D1;
            HotkeyAddPiece2 = Keys.D2;
            HotkeyAddPiece3 = Keys.D3;
            HotkeyAddPiece4 = Keys.D4;
            HotkeyAddPiece5 = Keys.D5;
            HotkeyAddPiece6 = Keys.D6;
            HotkeyAddPiece7 = Keys.D7;
            HotkeyAddPiece8 = Keys.D8;
            HotkeyAddPiece9 = Keys.D9;
            HotkeyAddPiece10 = Keys.D0;
            HotkeyAddPiece11 = Keys.None; // Unassigned in Classic layout
            HotkeyAddPiece12 = Keys.None; // Unassigned in Classic layout
            HotkeyAddPiece13 = Keys.None; // Unassigned in Classic layout
            HotkeyUndo = Keys.Control | Keys.Z;
            HotkeyRedo = Keys.Control | Keys.Y;
            HotkeySelectAll = Keys.Control | Keys.A;
            HotkeyCut = Keys.Control | Keys.X;
            HotkeyCopy = Keys.Control | Keys.C;
            HotkeyPaste = Keys.Control | Keys.V;
            HotkeyPasteInPlace = Keys.Control | Keys.Shift | Keys.V;
            HotkeyDuplicate = Keys.C;
            HotkeyDelete = Keys.Delete;
            HotkeyMoveUp = Keys.Up;
            HotkeyMoveDown = Keys.Down;
            HotkeyMoveLeft = Keys.Left;
            HotkeyMoveRight = Keys.Right;
            HotkeyMove8Up = Keys.Control | Keys.Up;
            HotkeyMove8Down = Keys.Control | Keys.Down;
            HotkeyMove8Left = Keys.Control | Keys.Left;
            HotkeyMove8Right = Keys.Control | Keys.Right;
            HotkeyCustomMoveUp = Keys.Alt | Keys.Up;
            HotkeyCustomMoveDown = Keys.Alt | Keys.Down;
            HotkeyCustomMoveLeft = Keys.Alt | Keys.Left;
            HotkeyCustomMoveRight = Keys.Alt | Keys.Right;
            HotkeyRotate = Keys.R;
            HotkeyFlip = Keys.E;
            HotkeyInvert = Keys.W;
            HotkeyGroup = Keys.G;
            HotkeyUngroup = Keys.U;
            HotkeyErase = Keys.A;
            HotkeyNoOverwrite = Keys.S;
            HotkeyOnlyOnTerrain = Keys.D;
            HotkeyAllowOneWay = Keys.F;
            HotkeyDrawLast = Keys.Home;
            HotkeyDrawSooner = Keys.PageUp;
            HotkeyDrawLater = Keys.PageDown;
            HotkeyDrawFirst = Keys.End;
            HotkeyCloseEditor = Keys.Alt | Keys.F4;
        }
    }
}
