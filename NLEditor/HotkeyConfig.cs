using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NLEditor
{
    public static class HotkeyConfig
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
        public static Keys HotkeyShowMissingPieces;
        public static Keys HotkeyToggleSnapToGrid;
        public static Keys HotkeyOpenSettings;
        public static Keys HotkeyOpenConfigHotkeys;
        public static Keys HotkeyOpenAboutSLX;
        public static Keys HotkeySelectPieces;
        public static Keys HotkeyDragToScroll;
        public static Keys HotkeyRemovePiecesAtCursor;
        public static Keys HotkeyAddRemoveSinglePiece;
        public static Keys HotkeySelectPiecesBelow;
        public static Keys HotkeyZoomIn;
        public static Keys HotkeyZoomOut;
        public static Keys HotkeyScrollHorizontally;
        public static Keys HotkeyScrollVertically;
        public static Keys HotkeyMoveScreenStart;
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
        public static Keys HotkeyDragHorizontally;
        public static Keys HotkeyDragVertically;
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
        public static Keys HotkeyCloseWindow;
        public static Keys HotkeyCloseEditor;
        public static string FormatHotkeyString(Keys hotkey)
        {
            List<string> hotkeyParts = new List<string>();

            if (hotkey.HasFlag(Keys.Control)) hotkeyParts.Add("Ctrl");
            if (hotkey.HasFlag(Keys.Shift)) hotkeyParts.Add("Shift");
            if (hotkey.HasFlag(Keys.Alt)) hotkeyParts.Add("Alt");

            // Get the base key from the flags
            Keys baseKey = hotkey & ~(Keys.Control | Keys.Shift | Keys.Alt);
            hotkeyParts.Add(baseKey.ToString());

            return string.Join("+", hotkeyParts);
        }

        public static Keys ParseHotkeyString(string hotkeyString)
        {
            Keys result = Keys.None;
            string[] parts = hotkeyString.Split('+');

            foreach (string part in parts)
            {
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
                // Extract the value after the '=' sign
                var parts = line.Split('=');
                if (parts.Length < 2) continue;

                var key = parts[1].Trim();

                if (key != "None")
                {       // Check for invalid keys           // Check for duplicates
                    if (!Enum.TryParse<Keys>(key, out _) || seenKeys.Contains(key))
                    {
                        // Truncate the key if it's longer than 100 characters
                        if (key.Length > 100) { key = key.Substring(0, 100) + "..."; }
                        
                        invalidKey = key;
                        return true; // Invalid or duplicate key found
                    }

                    seenKeys.Add(key);
                }
            }

            return false;
        }

        public static void LoadHotkeysFromIniFile()
        {
            var lines = System.IO.File.ReadAllLines("SLXEditorHotkeys.ini");

            // Check for duplicates directly
            if (ValidateHotkeyIniFile(lines, out string invalidKey))
            {
                MessageBox.Show($"Invalid or duplicate key detected in SLXEditorHotkeys.ini\n\n" +
                                $"{invalidKey}\n\n" +
                                $"The default hotkeys will be loaded instead",
                                "Hotkey Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                GetDefaultHotkeys();
                return;
            }

            foreach (var line in lines)
            {
                if (line.StartsWith("HotkeyCreateNewLevel="))
                    HotkeyCreateNewLevel = (Keys)Enum.Parse(typeof(Keys), line.Split('=')[1]);
                if (line.StartsWith("HotkeyLoadLevel="))
                    HotkeyLoadLevel = (Keys)Enum.Parse(typeof(Keys), line.Split('=')[1]);
                if (line.StartsWith("HotkeySaveLevel="))
                    HotkeySaveLevel = (Keys)Enum.Parse(typeof(Keys), line.Split('=')[1]);
                if (line.StartsWith("HotkeySaveLevelAs="))
                    HotkeySaveLevelAs = (Keys)Enum.Parse(typeof(Keys), line.Split('=')[1]);
                if (line.StartsWith("HotkeySaveLevelAsImage="))
                    HotkeySaveLevelAsImage = (Keys)Enum.Parse(typeof(Keys), line.Split('=')[1]);
                if (line.StartsWith("HotkeyPlaytestLevel="))
                    HotkeyPlaytestLevel = (Keys)Enum.Parse(typeof(Keys), line.Split('=')[1]);
                if (line.StartsWith("HotkeyValidateLevel="))
                    HotkeyValidateLevel = (Keys)Enum.Parse(typeof(Keys), line.Split('=')[1]);
                if (line.StartsWith("HotkeyCleanseLevels="))
                    HotkeyCleanseLevels = (Keys)Enum.Parse(typeof(Keys), line.Split('=')[1]);
                if (line.StartsWith("HotkeyToggleClearPhysics="))
                    HotkeyToggleClearPhysics = (Keys)Enum.Parse(typeof(Keys), line.Split('=')[1]);
                if (line.StartsWith("HotkeyToggleTerrain="))
                    HotkeyToggleTerrain = (Keys)Enum.Parse(typeof(Keys), line.Split('=')[1]);
                if (line.StartsWith("HotkeyToggleObjects="))
                    HotkeyToggleObjects = (Keys)Enum.Parse(typeof(Keys), line.Split('=')[1]);
                if (line.StartsWith("HotkeyToggleTriggerAreas="))
                    HotkeyToggleTriggerAreas = (Keys)Enum.Parse(typeof(Keys), line.Split('=')[1]);
                if (line.StartsWith("HotkeyToggleScreenStart="))
                    HotkeyToggleScreenStart = (Keys)Enum.Parse(typeof(Keys), line.Split('=')[1]);
                if (line.StartsWith("HotkeyToggleBackground="))
                    HotkeyToggleBackground = (Keys)Enum.Parse(typeof(Keys), line.Split('=')[1]);
                if (line.StartsWith("HotkeyToggleDeprecatedPieces="))
                    HotkeyToggleDeprecatedPieces = (Keys)Enum.Parse(typeof(Keys), line.Split('=')[1]);
                if (line.StartsWith("HotkeyShowMissingPieces="))
                    HotkeyShowMissingPieces = (Keys)Enum.Parse(typeof(Keys), line.Split('=')[1]);
                if (line.StartsWith("HotkeyToggleSnapToGrid="))
                    HotkeyToggleSnapToGrid = (Keys)Enum.Parse(typeof(Keys), line.Split('=')[1]);
                if (line.StartsWith("HotkeyOpenSettings="))
                    HotkeyOpenSettings = (Keys)Enum.Parse(typeof(Keys), line.Split('=')[1]);
                if (line.StartsWith("HotkeyOpenConfigHotkeys="))
                    HotkeyOpenConfigHotkeys = (Keys)Enum.Parse(typeof(Keys), line.Split('=')[1]);
                if (line.StartsWith("HotkeyOpenAboutSLX="))
                    HotkeyOpenAboutSLX = (Keys)Enum.Parse(typeof(Keys), line.Split('=')[1]);
                if (line.StartsWith("HotkeySelectPieces="))
                    HotkeySelectPieces = (Keys)Enum.Parse(typeof(Keys), line.Split('=')[1]);
                if (line.StartsWith("HotkeyDragToScroll="))
                    HotkeyDragToScroll = (Keys)Enum.Parse(typeof(Keys), line.Split('=')[1]);
                if (line.StartsWith("HotkeyRemovePiecesAtCursor="))
                    HotkeyRemovePiecesAtCursor = (Keys)Enum.Parse(typeof(Keys), line.Split('=')[1]);
                if (line.StartsWith("HotkeyAddRemoveSinglePiece="))
                    HotkeyAddRemoveSinglePiece = (Keys)Enum.Parse(typeof(Keys), line.Split('=')[1]);
                if (line.StartsWith("HotkeySelectPiecesBelow="))
                    HotkeySelectPiecesBelow = (Keys)Enum.Parse(typeof(Keys), line.Split('=')[1]);
                if (line.StartsWith("HotkeyZoomIn="))
                    HotkeyZoomIn = (Keys)Enum.Parse(typeof(Keys), line.Split('=')[1]);
                if (line.StartsWith("HotkeyZoomOut="))
                    HotkeyZoomOut = (Keys)Enum.Parse(typeof(Keys), line.Split('=')[1]);
                if (line.StartsWith("HotkeyScrollHorizontally="))
                    HotkeyScrollHorizontally = (Keys)Enum.Parse(typeof(Keys), line.Split('=')[1]);
                if (line.StartsWith("HotkeyScrollVertically="))
                    HotkeyScrollVertically = (Keys)Enum.Parse(typeof(Keys), line.Split('=')[1]);
                if (line.StartsWith("HotkeyMoveScreenStart="))
                    HotkeyMoveScreenStart = (Keys)Enum.Parse(typeof(Keys), line.Split('=')[1]);
                if (line.StartsWith("HotkeyShowPreviousPiece="))
                    HotkeyShowPreviousPiece = (Keys)Enum.Parse(typeof(Keys), line.Split('=')[1]);
                if (line.StartsWith("HotkeyShowNextPiece="))
                    HotkeyShowNextPiece = (Keys)Enum.Parse(typeof(Keys), line.Split('=')[1]);
                if (line.StartsWith("HotkeyShowPreviousGroup="))
                    HotkeyShowPreviousGroup = (Keys)Enum.Parse(typeof(Keys), line.Split('=')[1]);
                if (line.StartsWith("HotkeyShowNextGroup="))
                    HotkeyShowNextGroup = (Keys)Enum.Parse(typeof(Keys), line.Split('=')[1]);
                if (line.StartsWith("HotkeyShowPreviousStyle="))
                    HotkeyShowPreviousStyle = (Keys)Enum.Parse(typeof(Keys), line.Split('=')[1]);
                if (line.StartsWith("HotkeyShowNextStyle="))
                    HotkeyShowNextStyle = (Keys)Enum.Parse(typeof(Keys), line.Split('=')[1]);
                if (line.StartsWith("HotkeyCycleBrowser="))
                    HotkeyCycleBrowser = (Keys)Enum.Parse(typeof(Keys), line.Split('=')[1]);
                if (line.StartsWith("HotkeyAddPiece1="))
                    HotkeyAddPiece1 = (Keys)Enum.Parse(typeof(Keys), line.Split('=')[1]);
                if (line.StartsWith("HotkeyAddPiece2="))
                    HotkeyAddPiece2 = (Keys)Enum.Parse(typeof(Keys), line.Split('=')[1]);
                if (line.StartsWith("HotkeyAddPiece3="))
                    HotkeyAddPiece3 = (Keys)Enum.Parse(typeof(Keys), line.Split('=')[1]);
                if (line.StartsWith("HotkeyAddPiece4="))
                    HotkeyAddPiece4 = (Keys)Enum.Parse(typeof(Keys), line.Split('=')[1]);
                if (line.StartsWith("HotkeyAddPiece5="))
                    HotkeyAddPiece5 = (Keys)Enum.Parse(typeof(Keys), line.Split('=')[1]);
                if (line.StartsWith("HotkeyAddPiece6="))
                    HotkeyAddPiece6 = (Keys)Enum.Parse(typeof(Keys), line.Split('=')[1]);
                if (line.StartsWith("HotkeyAddPiece7="))
                    HotkeyAddPiece7 = (Keys)Enum.Parse(typeof(Keys), line.Split('=')[1]);
                if (line.StartsWith("HotkeyAddPiece8="))
                    HotkeyAddPiece8 = (Keys)Enum.Parse(typeof(Keys), line.Split('=')[1]);
                if (line.StartsWith("HotkeyAddPiece9="))
                    HotkeyAddPiece9 = (Keys)Enum.Parse(typeof(Keys), line.Split('=')[1]);
                if (line.StartsWith("HotkeyAddPiece10="))
                    HotkeyAddPiece10 = (Keys)Enum.Parse(typeof(Keys), line.Split('=')[1]);
                if (line.StartsWith("HotkeyAddPiece11="))
                    HotkeyAddPiece11 = (Keys)Enum.Parse(typeof(Keys), line.Split('=')[1]);
                if (line.StartsWith("HotkeyAddPiece12="))
                    HotkeyAddPiece12 = (Keys)Enum.Parse(typeof(Keys), line.Split('=')[1]);
                if (line.StartsWith("HotkeyAddPiece13="))
                    HotkeyAddPiece13 = (Keys)Enum.Parse(typeof(Keys), line.Split('=')[1]);
                if (line.StartsWith("HotkeyUndo="))
                    HotkeyUndo = (Keys)Enum.Parse(typeof(Keys), line.Split('=')[1]);
                if (line.StartsWith("HotkeyRedo="))
                    HotkeyRedo = (Keys)Enum.Parse(typeof(Keys), line.Split('=')[1]);
                if (line.StartsWith("HotkeyCut="))
                    HotkeyCut = (Keys)Enum.Parse(typeof(Keys), line.Split('=')[1]);
                if (line.StartsWith("HotkeyCopy="))
                    HotkeyCopy = (Keys)Enum.Parse(typeof(Keys), line.Split('=')[1]);
                if (line.StartsWith("HotkeyPaste="))
                    HotkeyPaste = (Keys)Enum.Parse(typeof(Keys), line.Split('=')[1]);
                if (line.StartsWith("HotkeyPasteInPlace"))
                    HotkeyPasteInPlace = (Keys)Enum.Parse(typeof(Keys), line.Split('=')[1]);
                if (line.StartsWith("HotkeyDuplicate="))
                    HotkeyDuplicate = (Keys)Enum.Parse(typeof(Keys), line.Split('=')[1]);
                if (line.StartsWith("HotkeyDelete="))
                    HotkeyDelete = (Keys)Enum.Parse(typeof(Keys), line.Split('=')[1]);
                if (line.StartsWith("HotkeyMoveUp="))
                    HotkeyMoveUp = (Keys)Enum.Parse(typeof(Keys), line.Split('=')[1]);
                if (line.StartsWith("HotkeyMoveDown="))
                    HotkeyMoveDown = (Keys)Enum.Parse(typeof(Keys), line.Split('=')[1]);
                if (line.StartsWith("HotkeyMoveLeft="))
                    HotkeyMoveLeft = (Keys)Enum.Parse(typeof(Keys), line.Split('=')[1]);
                if (line.StartsWith("HotkeyMoveRight="))
                    HotkeyMoveRight = (Keys)Enum.Parse(typeof(Keys), line.Split('=')[1]);
                if (line.StartsWith("HotkeyMove8Up="))
                    HotkeyMove8Up = (Keys)Enum.Parse(typeof(Keys), line.Split('=')[1]);
                if (line.StartsWith("HotkeyMove8Down="))
                    HotkeyMove8Down = (Keys)Enum.Parse(typeof(Keys), line.Split('=')[1]);
                if (line.StartsWith("HotkeyMove8Left="))
                    HotkeyMove8Left = (Keys)Enum.Parse(typeof(Keys), line.Split('=')[1]);
                if (line.StartsWith("HotkeyMove8Right="))
                    HotkeyMove8Right = (Keys)Enum.Parse(typeof(Keys), line.Split('=')[1]);
                if (line.StartsWith("HotkeyCustomMoveUp="))
                    HotkeyCustomMoveUp = (Keys)Enum.Parse(typeof(Keys), line.Split('=')[1]);
                if (line.StartsWith("HotkeyCustomMoveDown="))
                    HotkeyCustomMoveDown = (Keys)Enum.Parse(typeof(Keys), line.Split('=')[1]);
                if (line.StartsWith("HotkeyCustomMoveLeft="))
                    HotkeyCustomMoveLeft = (Keys)Enum.Parse(typeof(Keys), line.Split('=')[1]);
                if (line.StartsWith("HotkeyCustomMoveRight="))
                    HotkeyCustomMoveRight = (Keys)Enum.Parse(typeof(Keys), line.Split('=')[1]);
                if (line.StartsWith("HotkeyDragHorizontally="))
                    HotkeyDragHorizontally = (Keys)Enum.Parse(typeof(Keys), line.Split('=')[1]);
                if (line.StartsWith("HotkeyDragVertically="))
                    HotkeyDragVertically = (Keys)Enum.Parse(typeof(Keys), line.Split('=')[1]);
                if (line.StartsWith("HotkeyRotate="))
                    HotkeyRotate = (Keys)Enum.Parse(typeof(Keys), line.Split('=')[1]);
                if (line.StartsWith("HotkeyFlip="))
                    HotkeyFlip = (Keys)Enum.Parse(typeof(Keys), line.Split('=')[1]);
                if (line.StartsWith("HotkeyInvert="))
                    HotkeyInvert = (Keys)Enum.Parse(typeof(Keys), line.Split('=')[1]);
                if (line.StartsWith("HotkeyGroup="))
                    HotkeyGroup = (Keys)Enum.Parse(typeof(Keys), line.Split('=')[1]);
                if (line.StartsWith("HotkeyUngroup="))
                    HotkeyUngroup = (Keys)Enum.Parse(typeof(Keys), line.Split('=')[1]);
                if (line.StartsWith("HotkeyErase="))
                    HotkeyErase = (Keys)Enum.Parse(typeof(Keys), line.Split('=')[1]);
                if (line.StartsWith("HotkeyNoOverwrite="))
                    HotkeyNoOverwrite = (Keys)Enum.Parse(typeof(Keys), line.Split('=')[1]);
                if (line.StartsWith("HotkeyOnlyOnTerrain="))
                    HotkeyOnlyOnTerrain = (Keys)Enum.Parse(typeof(Keys), line.Split('=')[1]);
                if (line.StartsWith("HotkeyAllowOneWay="))
                    HotkeyAllowOneWay = (Keys)Enum.Parse(typeof(Keys), line.Split('=')[1]);
                if (line.StartsWith("HotkeyDrawLast="))
                    HotkeyDrawLast = (Keys)Enum.Parse(typeof(Keys), line.Split('=')[1]);
                if (line.StartsWith("HotkeyDrawSooner="))
                    HotkeyDrawSooner = (Keys)Enum.Parse(typeof(Keys), line.Split('=')[1]);
                if (line.StartsWith("HotkeyDrawLater="))
                    HotkeyDrawLater = (Keys)Enum.Parse(typeof(Keys), line.Split('=')[1]);
                if (line.StartsWith("HotkeyDrawFirst="))
                    HotkeyDrawFirst = (Keys)Enum.Parse(typeof(Keys), line.Split('=')[1]);
                if (line.StartsWith("HotkeyCloseWindow="))
                    HotkeyCloseWindow = (Keys)Enum.Parse(typeof(Keys), line.Split('=')[1]);
                if (line.StartsWith("HotkeyCloseEditor="))
                    HotkeyCloseEditor = (Keys)Enum.Parse(typeof(Keys), line.Split('=')[1]);
            }
        }

        public static void SaveHotkeysToIniFile()
        {
            var lines = new List<string>
            {
                "[Hotkeys]",
                $"HotkeyCreateNewLevel={HotkeyCreateNewLevel}",
                $"HotkeyLoadLevel={HotkeyLoadLevel}",
                $"HotkeySaveLevel={HotkeySaveLevel}",
                $"HotkeySaveLevelAs={HotkeySaveLevelAs}",
                $"HotkeySaveLevelAsImage={HotkeySaveLevelAsImage}",
                $"HotkeyPlaytestLevel={HotkeyPlaytestLevel}",
                $"HotkeyValidateLevel={HotkeyValidateLevel}",
                $"HotkeyCleanseLevels={HotkeyCleanseLevels}",
                $"HotkeyToggleClearPhysics={HotkeyToggleClearPhysics}",
                $"HotkeyToggleTerrain={HotkeyToggleTerrain}",
                $"HotkeyToggleObjects={HotkeyToggleObjects}",
                $"HotkeyToggleTriggerAreas={HotkeyToggleTriggerAreas}",
                $"HotkeyToggleScreenStart={HotkeyToggleScreenStart}",
                $"HotkeyToggleBackground={HotkeyToggleBackground}",
                $"HotkeyToggleDeprecatedPieces={HotkeyToggleDeprecatedPieces}",
                $"HotkeyShowMissingPieces={HotkeyShowMissingPieces}",
                $"HotkeyToggleSnapToGrid={HotkeyToggleSnapToGrid}",
                $"HotkeyOpenSettings={HotkeyOpenSettings}",
                $"HotkeyOpenConfigHotkeys={HotkeyOpenConfigHotkeys}",
                $"HotkeyOpenAboutSLX={HotkeyOpenAboutSLX}",
                $"HotkeySelectPieces={HotkeySelectPieces}",
                $"HotkeyDragToScroll={HotkeyDragToScroll}",
                $"HotkeyRemovePiecesAtCursor={HotkeyRemovePiecesAtCursor}",
                $"HotkeyAddRemoveSinglePiece={HotkeyAddRemoveSinglePiece}",
                $"HotkeySelectPiecesBelow={HotkeySelectPiecesBelow}",
                $"HotkeyZoomIn={HotkeyZoomIn}",
                $"HotkeyZoomOut={HotkeyZoomOut}",
                $"HotkeyScrollHorizontally={HotkeyScrollHorizontally}",
                $"HotkeyScrollVertically={HotkeyScrollVertically}",
                $"HotkeyMoveScreenStart={HotkeyMoveScreenStart}",
                $"HotkeyShowPreviousPiece={HotkeyShowPreviousPiece}",
                $"HotkeyShowNextPiece={HotkeyShowNextPiece}",
                $"HotkeyShowPreviousGroup={HotkeyShowPreviousGroup}",
                $"HotkeyShowNextGroup={HotkeyShowNextGroup}",
                $"HotkeyShowPreviousStyle={HotkeyShowPreviousStyle}",
                $"HotkeyShowNextStyle={HotkeyShowNextStyle}",
                $"HotkeyCycleBrowser={HotkeyCycleBrowser}",
                $"HotkeyAddPiece1={HotkeyAddPiece1}",
                $"HotkeyAddPiece2={HotkeyAddPiece2}",
                $"HotkeyAddPiece3={HotkeyAddPiece3}",
                $"HotkeyAddPiece4={HotkeyAddPiece4}",
                $"HotkeyAddPiece5={HotkeyAddPiece5}",
                $"HotkeyAddPiece6={HotkeyAddPiece6}",
                $"HotkeyAddPiece7={HotkeyAddPiece7}",
                $"HotkeyAddPiece8={HotkeyAddPiece8}",
                $"HotkeyAddPiece9={HotkeyAddPiece9}",
                $"HotkeyAddPiece10={HotkeyAddPiece10}",
                $"HotkeyAddPiece11={HotkeyAddPiece11}",
                $"HotkeyAddPiece12={HotkeyAddPiece12}",
                $"HotkeyAddPiece13={HotkeyAddPiece13}",
                $"HotkeyUndo={HotkeyUndo}",
                $"HotkeyRedo={HotkeyRedo}",
                $"HotkeyCut={HotkeyCut}",
                $"HotkeyCopy={HotkeyCopy}",
                $"HotkeyPaste={HotkeyPaste}",
                $"HotkeyPasteInPlace={HotkeyPasteInPlace}",
                $"HotkeyDuplicate={HotkeyDuplicate}",
                $"HotkeyDelete={HotkeyDelete}",
                $"HotkeyMoveUp={HotkeyMoveUp}",
                $"HotkeyMoveDown={HotkeyMoveDown}",
                $"HotkeyMoveLeft={HotkeyMoveLeft}",
                $"HotkeyMoveRight={HotkeyMoveRight}",
                $"HotkeyMove8Up={HotkeyMove8Up}",
                $"HotkeyMove8Down={HotkeyMove8Down}",
                $"HotkeyMove8Left={HotkeyMove8Left}",
                $"HotkeyMove8Right={HotkeyMove8Right}",
                $"HotkeyCustomMoveUp={HotkeyCustomMoveUp}",
                $"HotkeyCustomMoveDown={HotkeyCustomMoveDown}",
                $"HotkeyCustomMoveLeft={HotkeyCustomMoveLeft}",
                $"HotkeyCustomMoveRight={HotkeyCustomMoveRight}",
                $"HotkeyDragHorizontally={HotkeyDragHorizontally}",
                $"HotkeyDragVertically={HotkeyDragVertically}",
                $"HotkeyRotate={HotkeyRotate}",
                $"HotkeyFlip={HotkeyFlip}",
                $"HotkeyInvert={HotkeyInvert}",
                $"HotkeyGroup={HotkeyGroup}",
                $"HotkeyUngroup={HotkeyUngroup}",
                $"HotkeyErase={HotkeyErase}",
                $"HotkeyNoOverwrite={HotkeyNoOverwrite}",
                $"HotkeyOnlyOnTerrain={HotkeyOnlyOnTerrain}",
                $"HotkeyAllowOneWay={HotkeyAllowOneWay}",
                $"HotkeyDrawLast={HotkeyDrawLast}",
                $"HotkeyDrawSooner={HotkeyDrawSooner}",
                $"HotkeyDrawLater={HotkeyDrawLater}",
                $"HotkeyDrawFirst={HotkeyDrawFirst}",
                $"HotkeyCloseWindow={HotkeyCloseWindow}",
                $"HotkeyCloseEditor={HotkeyCloseEditor}"
            };

            System.IO.File.WriteAllLines("SLXEditorHotkeys.ini", lines);
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
            HotkeyShowMissingPieces = Keys.F8;
            HotkeyToggleSnapToGrid = Keys.F9;
            HotkeyOpenSettings = Keys.F10;
            HotkeyOpenConfigHotkeys = Keys.F11;
            HotkeyOpenAboutSLX = Keys.Control | Keys.F10;
            HotkeySelectPieces = Keys.LButton;
            HotkeyDragToScroll = Keys.RButton;
            HotkeyRemovePiecesAtCursor = Keys.MButton;
            HotkeyAddRemoveSinglePiece = Keys.Control | Keys.LButton;
            HotkeySelectPiecesBelow = Keys.Alt | Keys.LButton;
            HotkeyZoomIn = Keys.Oemplus;
            HotkeyZoomOut = Keys.OemMinus;
            HotkeyScrollHorizontally = Keys.Control | Keys.None; // Come back to this later
            HotkeyScrollVertically = Keys.Alt | Keys.None; // Come back to this later
            HotkeyMoveScreenStart = Keys.P;
            HotkeyShowPreviousPiece = Keys.Shift | Keys.Left;
            HotkeyShowNextPiece = Keys.Shift | Keys.Right;
            HotkeyShowPreviousGroup = Keys.Shift | Keys.Alt | Keys.Left;
            HotkeyShowNextGroup = Keys.Shift | Keys.Alt | Keys.Right;
            HotkeyShowPreviousStyle = Keys.Shift | Keys.Up;
            HotkeyShowNextStyle = Keys.Shift | Keys.Down;
            HotkeyCycleBrowser = Keys.Space;
            HotkeyAddPiece1 = Keys.NumPad1;
            HotkeyAddPiece2 = Keys.NumPad2;
            HotkeyAddPiece3 = Keys.NumPad3;
            HotkeyAddPiece4 = Keys.NumPad4;
            HotkeyAddPiece5 = Keys.NumPad5;
            HotkeyAddPiece6 = Keys.NumPad6;
            HotkeyAddPiece7 = Keys.NumPad7;
            HotkeyAddPiece8 = Keys.NumPad8;
            HotkeyAddPiece9 = Keys.NumPad9;
            HotkeyAddPiece10 = Keys.NumPad0;
            HotkeyAddPiece11 = Keys.None; // Unassigned by default
            HotkeyAddPiece12 = Keys.None; // Unassigned by default
            HotkeyAddPiece13 = Keys.None; // Unassigned by default
            HotkeyUndo = Keys.Control | Keys.Z;
            HotkeyRedo = Keys.Control | Keys.Y;
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
            HotkeyDragHorizontally = Keys.Control | Keys.Shift;
            HotkeyDragVertically = Keys.Control | Keys.Alt;
            HotkeyRotate = Keys.R;
            HotkeyFlip = Keys.E;
            HotkeyInvert = Keys.W;
            HotkeyGroup = Keys.G;
            HotkeyUngroup = Keys.H;
            HotkeyErase = Keys.A;
            HotkeyNoOverwrite = Keys.S;
            HotkeyOnlyOnTerrain = Keys.D;
            HotkeyAllowOneWay = Keys.F;
            HotkeyDrawLast = Keys.Home;
            HotkeyDrawSooner = Keys.PageUp;
            HotkeyDrawLater = Keys.PageDown;
            HotkeyDrawFirst = Keys.End;
            HotkeyCloseWindow = Keys.Escape;
            HotkeyCloseEditor = Keys.Alt | Keys.F4;
        }
    }
}
