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
        public static Keys HotkeyCreateNewLevel             = Keys.Control | Keys.N;
        public static Keys HotkeyLoadLevel                  = Keys.Control | Keys.O;
        public static Keys HotkeySaveLevel                  = Keys.Control | Keys.S;
        public static Keys HotkeySaveLevelAs                = Keys.Control | Keys.Alt | Keys.S;
        public static Keys HotkeyPlaytestLevel              = Keys.F12;
        public static Keys HotkeyValidateLevel              = Keys.Control | Keys.F12;
        public static Keys HotkeyToggleClearPhysics         = Keys.F1;
        public static Keys HotkeyToggleTerrain              = Keys.F2;
        public static Keys HotkeyToggleObjects              = Keys.F3;
        public static Keys HotkeyToggleTriggerAreas         = Keys.F4;
        public static Keys HotkeyToggleScreenStart          = Keys.F5;
        public static Keys HotkeyToggleBackgroundImage      = Keys.F6;
        public static Keys HotkeyToggleSnapToGrid           = Keys.F9;
        public static Keys HotkeyOpenSettings               = Keys.F10;
        public static Keys HotkeyOpenHotkeyConfig           = Keys.F11;
        public static Keys HotkeyOpenAboutSLX               = Keys.Control | Keys.Alt | Keys.F11;
        public static Keys HotkeySelectPieces               = Keys.LButton;
        public static Keys HotkeyDragToScroll               = Keys.RButton;
        public static Keys HotkeyRemovePiecesAtCursor       = Keys.MButton;
        public static Keys HotkeyAddRemoveSinglePiece       = Keys.Control | Keys.LButton;
        public static Keys HotkeySelectLowerPieces          = Keys.Alt | Keys.LButton;
        public static Keys HotkeyZoomIn                     = Keys.Oemplus;
        public static Keys HotkeyZoomOut                    = Keys.OemMinus;
        public static Keys HotkeyScrollHorizontally         = Keys.Control | Keys.None; // Come back to this later
        public static Keys HotkeyScrollVertically           = Keys.Alt | Keys.None; // Come back to this later
        public static Keys HotkeyMoveScreenStart            = Keys.P;
        public static Keys HotkeyShowPreviousPiece          = Keys.Shift | Keys.Left;
        public static Keys HotkeyShowNextPiece              = Keys.Shift | Keys.Right;
        public static Keys HotkeyShowPreviousGroup          = Keys.Shift | Keys.Alt | Keys.Left;
        public static Keys HotkeyShowNextGroup              = Keys.Shift | Keys.Alt | Keys.Right;
        public static Keys HotkeyShowPreviousStyle          = Keys.Shift | Keys.Up;
        public static Keys HotkeyShowNextStyle              = Keys.Shift | Keys.Down;
        public static Keys HotkeyToggleBrowseTerrainObjects = Keys.Space;
        public static Keys HotkeyAddPiece1                  = Keys.NumPad1;
        public static Keys HotkeyAddPiece2                  = Keys.NumPad2;
        public static Keys HotkeyAddPiece3                  = Keys.NumPad3;
        public static Keys HotkeyAddPiece4                  = Keys.NumPad4;
        public static Keys HotkeyAddPiece5                  = Keys.NumPad5;
        public static Keys HotkeyAddPiece6                  = Keys.NumPad6;
        public static Keys HotkeyAddPiece7                  = Keys.NumPad7;
        public static Keys HotkeyAddPiece8                  = Keys.NumPad8;
        public static Keys HotkeyAddPiece9                  = Keys.NumPad9;
        public static Keys HotkeyAddPiece10                 = Keys.NumPad0;
        public static Keys HotkeyAddPiece11                 = Keys.None; // Unassigned by default
        public static Keys HotkeyAddPiece12                 = Keys.None; // Unassigned by default
        public static Keys HotkeyAddPiece13                 = Keys.None; // Unassigned by default
        public static Keys HotkeyUndo                       = Keys.Control | Keys.Z;
        public static Keys HotkeyRedo                       = Keys.Control | Keys.Y;
        public static Keys HotkeyCut                        = Keys.Control | Keys.X;
        public static Keys HotkeyCopy                       = Keys.Control | Keys.C;
        public static Keys HotkeyPaste                      = Keys.Control | Keys.V;
        public static Keys HotkeyDuplicate                  = Keys.C;
        public static Keys HotkeyDelete                     = Keys.Delete;
        public static Keys HotkeyMoveOnePixelUp             = Keys.Up;
        public static Keys HotkeyMoveOnePixelDown           = Keys.Down;
        public static Keys HotkeyMoveOnePixelLeft           = Keys.Left;
        public static Keys HotkeyMoveOnePixelRight          = Keys.Right;
        public static Keys HotkeyMoveEightPixelsUp          = Keys.Control | Keys.Up;
        public static Keys HotkeyMoveEightPixelsDown        = Keys.Control | Keys.Down;
        public static Keys HotkeyMoveEightPixelsLeft        = Keys.Control | Keys.Left;
        public static Keys HotkeyMoveEightPixelsRight       = Keys.Control | Keys.Right;
        public static Keys HotkeyCustomMove                 = Keys.Alt | Keys.Up;
        public static Keys HotkeyDragHorizontally           = Keys.Control | Keys.Shift;
        public static Keys HotkeyDragVertically             = Keys.Control | Keys.Alt;
        public static Keys HotkeyRotate                     = Keys.R;
        public static Keys HotkeyFlip                       = Keys.E;
        public static Keys HotkeyInvert                     = Keys.W;
        public static Keys HotkeyGroup                      = Keys.G;
        public static Keys HotkeyUngroup                    = Keys.H;
        public static Keys HotkeyErase                      = Keys.A;
        public static Keys HotkeyNoOverwrite                = Keys.S;
        public static Keys HotkeyOnlyOnTerrain              = Keys.D;
        public static Keys HotkeyAllowOneWay                = Keys.F;
        public static Keys HotkeyDrawLast                   = Keys.Home;
        public static Keys HotkeyDrawSooner                 = Keys.PageUp;
        public static Keys HotkeyDrawLater                  = Keys.PageDown;
        public static Keys HotkeyDrawFirst                  = Keys.End;
        public static Keys HotkeyCloseWindow                = Keys.Escape;
        public static Keys HotkeyCloseEditor                = Keys.Alt | Keys.F4;
    }
}
