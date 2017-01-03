﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NLEditor
{
    public class C // for Constants
    {
        public static string Version
        {
            get { return "0.2"; }
        }

        public static string AppPath
        {
            get 
            {
                #if DEBUG
                    return "C:\\Stephan\\Programme\\NLEditor\\";
                #endif

                return System.Windows.Forms.Application.StartupPath + DirSep; 
            }
        }

        public static string AppPathPieces
        {
            get { return AppPath + "styles" + DirSep; }
        }

        public static string AppPathThemeInfo(string StyleName)
        {
            return AppPathPieces + StyleName + C.DirSep + "theme.nxtm";
        }

        public static char DirSep
        {
            get { return System.IO.Path.DirectorySeparatorChar; }
        }

        public static string NewLine
        {
            get { return Environment.NewLine; }
        }

        public enum CustDrawMode 
        { 
            Default, DefaultOWW, Erase, OnlyAtMask, OnlyAtOWW,
            NoOverwrite, NoOverwriteOWW,
            ClearPhysics, ClearPhysicsOWW, ClearPhysicsSteel,
            ClearPhysicsNoOverwrite, ClearPhysicsNoOverwriteOWW, ClearPhysicsSteelNoOverwrite
        }

        public enum DIR { N, E, S, W }

        /// <summary>
        /// Warning: The values of the object types here do NOT correspond to the numbers used in NeoLemmix! 
        /// </summary>
        public enum OBJ 
        { 
            TERRAIN = -1, STEEL = -2,
            HATCH = 0, EXIT = 1, TRAP = 4, TRAPONCE = 5, WATER = 6, FIRE = 7,
            ONE_WAY_WALL = 10,
            LEMMING = 15, PICKUP = 16, TELEPORTER = 17, RECEIVER = 18,
            EXIT_LOCKED = 20, BUTTON = 21,
            UPDRAFT = 22, NOSPLAT = 23, SPLAT = 24,
            FORCE_FIELD = 25,
            SPLITTER = 27, RADIATION = 28, SLOWFREEZE = 29,
            NONE = 50, ANIMATION = 51, BACKGROUND = 52, 
            NULL
        }

        public static readonly Dictionary<OBJ, string> TooltipList = new Dictionary<OBJ, string>
            {
                {OBJ.TERRAIN, "Terrain"}, {OBJ.STEEL, "Steel"}, {OBJ.NONE, "No effect"},
                {OBJ.EXIT, "Exit"}, {OBJ.FORCE_FIELD, "Force-field"}, {OBJ.ONE_WAY_WALL, "One-way-wall"},
                {OBJ.TRAP, "Triggered trap"}, {OBJ.WATER, "Water"}, {OBJ.FIRE, "Fire"},
                {OBJ.TELEPORTER, "Teleporter"}, {OBJ.RECEIVER, "Receiver"}, {OBJ.LEMMING, "Preplaced lemming"},
                {OBJ.PICKUP, "Pick-up skill"}, {OBJ.EXIT_LOCKED, "Locked exit"}, {OBJ.BUTTON, "Button"},
                {OBJ.RADIATION, "Radiation"}, {OBJ.SLOWFREEZE, "Slowfreeze"}, {OBJ.UPDRAFT, "Updraft"},
                {OBJ.SPLITTER, "Splitter"}, {OBJ.HATCH, "Hatch"}, {OBJ.ANIMATION, "Do NOT use!"},
                {OBJ.NOSPLAT, "Anti-splat wire"}, {OBJ.SPLAT, "Splat wire"}, {OBJ.BACKGROUND, "Do NOT use!"},
                {OBJ.TRAPONCE, "Single-use trap"}
            };

        public enum DragActions { Null, SelectArea, DragPieces, MoveEditorPos }

        public enum Resize { None, Vert, Horiz, Both }

        public static readonly byte ALPHA_OWW = 255;
        public static readonly byte ALPHA_NOOWW = 254;


        public static readonly int LAY_COUNT = 5;
        public static readonly int LAY_BACKGROUND = 0;
        public static readonly int LAY_OBJBACK = 1;
        public static readonly int LAY_TERRAIN = 2;
        public static readonly int LAY_OBJTOP = 3;
        public static readonly int LAY_TRIGGER = 4;


        public static readonly int SKI_COUNT = 16; // count without zombie!
        public static readonly int SKI_CLIMBER = 0;
        public static readonly int SKI_FLOATER = 1;
        public static readonly int SKI_EXPLODER = 2;
        public static readonly int SKI_BLOCKER = 3;
        public static readonly int SKI_BUILDER = 4;
        public static readonly int SKI_BASHER = 5;
        public static readonly int SKI_MINER = 6;
        public static readonly int SKI_DIGGER = 7;
        public static readonly int SKI_WALKER = 8;
        public static readonly int SKI_SWIMMER = 9;
        public static readonly int SKI_GLIDER = 10;
        public static readonly int SKI_DISARMER = 11;
        public static readonly int SKI_STONER = 12;
        public static readonly int SKI_PLATFORMER = 13;
        public static readonly int SKI_STACKER = 14;
        public static readonly int SKI_CLONER = 15;
        public static readonly int SKI_ZOMBIE = 16;

        public static readonly int ZOOM_MIN = -2;
        public static readonly int ZOOM_MAX = 7;


        public static readonly List<string> HotkeyList = new List<string>
            {
                "Left mouse", 
                "", 
                "Middle mouse", 
                "Ctrl + mouse", 
                "Alt + mouse", 
                "Right mouse",
                "",
                "Esc", 
                "Alt + F4", 
                "Ctrl + N", 
                "Ctrl + O", 
                "Ctrl + S", 
                "Ctrl + Alt + S",
                "",
                "Ctrl + Z",
                "Ctrl + Y",
                "Ctrl + X",
                "Ctrl + V",
                "Ctrl + C",
                "",
                "Up/Down/Right/Left",
                "Ctrl + Up/Down/Right/Left",

                "Shift + Right/Left", 
                "Shift + Up/Down",
                "Shift + Space",
                "Shift + Number",
                "",
                "F1", 
                "F2", 
                "F3", 
                "F4", 
                "F5", 
                "F6",
                "F11",
                "F12"
            };

        public static readonly List<string> DescriptionList = new List<string>
            {
                "Drag selected pieces or...",
                "Select one piece or all pieces in an area.",
                "Remove one piece or all pieces in an area from selection.",
                "Add/Remove pieces to current selection.",
                "Invert priority of piece selection.",
                "Drag current editor position.",
                "",
                "Exit the editor.",
                "Exit the editor.",
                "Create a new empty level.",
                "Load a new level.",
                "Save the current level.",
                "Save the current level in a new file.",
                "",
                "Undo last change.",
                "Revert last Undo and redo that action.",
                "Delete selected pieces.",
                "Insert previously deleted or copied pieces.",
                "Copy selected pieces.",
                "",
                "Move selected pieces.",
                "Move selected pieces 8 pixels.",

                "Display previous/next item in piece selection.",
                "Change style for the piece selection to previous/next one.",
                "Toggle between new terrain and new objects.",
                "Add the respective piece to the level.",
                "",
                "Switch to and from Clear Physics mode.",
                "Switch to and from displaying terrain.",
                "Switch to and from displaying interactive objects.",
                "Switch to and from displaying trigger areas.",
                "Switch to and from displaying the screen start.",
                "Switch to and from displaying background images.",
                "Display this hotkey help window.",
                "Playtest the level in the NeoLemmix player."
            };

        public static readonly List<string> VersionList = new List<string>
        {
            "Version " + Version,
            "   by Stephan Neupert",
            "",
            "Thanks to...",
            "  DMA for creating the original Lemmings games.",
            "  Namida Verasche for the NeoLemmix player.",
            "  The LemmingsForums at http://www.lemmingsforums.net.",
            "",
            "This application and all its source code is licensed under",
            "   CC BY-NC 4.0."
        };



    }
}
