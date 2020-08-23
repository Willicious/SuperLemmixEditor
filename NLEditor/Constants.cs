using System;
using System.Collections.Generic;
using System.Drawing;

namespace NLEditor
{
    using THotkeyTexts = Dictionary<C.HotkeyTabs, List<string>>;

    static class C // for Constants
    {
        public static string Version
        {
            get
            {
                var version = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version;
                return version.Major.ToString() + "." + version.Minor.ToString();
            }
        }

        public static string AppPath => System.Windows.Forms.Application.StartupPath + DirSep;
        public static string AppPathAutosave => AppPath + "autosave" + DirSep;
        public static string AppPathPieces => AppPath + "styles" + DirSep;
        public static string AppPathMusic => AppPath + "music" + DirSep;
        public static string AppPathLevels => AppPath + "levels" + DirSep;
        public static string AppPathTempLevel => AppPath + "TempTestLevel.nxlv";
        public static string AppPathThemeInfo(string styleName) => AppPathPieces + styleName + C.DirSep + "theme.nxtm";
        public static string AppPathSettings => AppPath + "NLEditorSettings.ini";
        public static string AppPathPlayerSettings => AppPath + "settings" + DirSep + "settings.ini";
        public static string AppPathPlayerSettingsOld => AppPath + "NeoLemmix147Settings.ini";
        public static string AppPathNeoLemmix => AppPath + "NeoLemmix.exe";

        public static char DirSep => System.IO.Path.DirectorySeparatorChar;
        public static string NewLine => Environment.NewLine;

        public static Size PicPieceSize => new Size(84, 84);

        public static ScreenSize ScreenSize;

        public enum SelectPieceType
        {
            Terrain, Objects, Backgrounds, Sketches
        }

        public enum DisplayType
        {
            Terrain, Objects, Trigger, ScreenStart, Background, ClearPhysics, Deprecated
        }

        public enum CustDrawMode
        {
            Default, DefaultOWW, Erase, OnlyAtMask, OnlyAtOWW,
            NoOverwrite, NoOverwriteOWW,
            ClearPhysics, ClearPhysicsOWW, ClearPhysicsSteel,
            ClearPhysicsNoOverwrite, ClearPhysicsNoOverwriteOWW, ClearPhysicsSteelNoOverwrite,
            Custom
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
            UPDRAFT = 22, SPLAT = 24, FORCE_FIELD = 25, SPLITTER = 27,
            NONE = 50, BACKGROUND = 52,
            NULL
        }

        public enum StyleColor
        {
            BACKGROUND, ONE_WAY_WALL, MASK, PICKUP_BORDER, PICKUP_INSIDE
        }
        public static NLColor ToNLColor(this StyleColor styleColor)
        {
            switch (styleColor)
            {
                case StyleColor.BACKGROUND:
                    return NLColor.BackDefault;
                case StyleColor.ONE_WAY_WALL:
                    return NLColor.OWWDefault;
                default:
                    return NLColor.BackDefault;
            }
        }

        public static readonly Dictionary<OBJ, string> TooltipList = new Dictionary<OBJ, string>
    {
      {OBJ.TERRAIN, "Terrain"}, {OBJ.STEEL, "Steel"}, {OBJ.NONE, "No effect"},
      {OBJ.EXIT, "Exit"}, {OBJ.FORCE_FIELD, "Turning field"}, {OBJ.ONE_WAY_WALL, "One-way-wall"},
      {OBJ.TRAP, "Triggered trap"}, {OBJ.WATER, "Water"}, {OBJ.FIRE, "Fire"},
      {OBJ.TELEPORTER, "Teleporter"}, {OBJ.RECEIVER, "Receiver"}, {OBJ.LEMMING, "Preplaced lemming"},
      {OBJ.PICKUP, "Pick-up skill"}, {OBJ.EXIT_LOCKED, "Locked exit"}, {OBJ.BUTTON, "Button"},
      {OBJ.UPDRAFT, "Updraft"}, {OBJ.SPLITTER, "Splitter"}, {OBJ.HATCH, "Hatch"},
      {OBJ.SPLAT, "Splat wire"}, {OBJ.BACKGROUND, "Decoration"}, {OBJ.TRAPONCE, "Single-use trap"}
    };

        public enum DragActions
        {
            Null, SelectArea, DragPieces, DragNewPiece, MoveEditorPos, MoveStartPos
        }

        public enum Resize { None, Vert, Horiz, Both }

        public static readonly byte ALPHA_OWW = 255;
        public static readonly byte ALPHA_NOOWW = 254;

        public enum Layer { Background, ObjBack, Terrain, ObjTop, Trigger }
        public static readonly List<Layer> LayerList = new List<Layer>()
    {
      Layer.Background, Layer.ObjBack, Layer.Terrain, Layer.ObjTop, Layer.Trigger
    };

        // The integer values here are only used to pick the correct frame of pickup-skills
        public enum Skill
        {
            Neutral = -1, Zombie = 0,
            Walker = 1, Jumper = 3, Shimmier = 5,
            Climber = 7, Swimmer = 9, Floater = 11, Glider = 13, Disarmer = 15,
            Bomber = 17, Stoner = 19, Blocker = 21,
            Platformer = 23, Builder = 25, Stacker = 27,
            Basher = 29, Fencer = 31, Miner = 33, Digger = 35,
            Cloner = 37
        };
        public static Array SkillArray => Enum.GetValues(typeof(C.Skill));

        public static readonly int ZOOM_MIN = -2;
        public static readonly int ZOOM_MAX = 7;

        public static readonly int LEM_OFFSET_X = 2;
        public static readonly int LEM_OFFSET_Y = 9;

        // Other colors are specified directly in BmpModify to speed up rendering.
        public enum NLColor
        {
            Text, OWWDefault, BackDefault,
            Trigger, ScreenStart, SelRectGadget, SelRectTerrain
        }
        public static readonly Dictionary<NLColor, Color> NLColors = new Dictionary<NLColor, Color>()
    {
      { NLColor.Text, Utility.HexToColor("FEF5F5F5") }, // Color.WhiteSmoke with slightly reduced alpha
      { NLColor.OWWDefault, Color.Linen },
      { NLColor.BackDefault, Color.Black },
      { NLColor.Trigger, Utility.HexToColor("80EE82EE") }, // Color.Violet with reduced alpha
      { NLColor.ScreenStart, Color.AliceBlue },
      { NLColor.SelRectGadget, Color.Chartreuse },
      { NLColor.SelRectTerrain, Color.Gold }
    };

        public enum HotkeyTabs { General, Level, Pieces }

        public enum TalismanType { Bronze, Silver, Gold }
        public enum TalismanReq
        {
            SaveReq, TimeLimit, SkillTotal,
            SkillWalker, SkillJumper, SkillShimmier, SkillClimber, SkillSwimmer, SkillFloater, SkillGlider,
            SkillDisarmer, SkillBomber, SkillStoner, SkillBlocker,
            SkillBuilder, SkillPlatformer, SkillStacker,
            SkillBasher, SkillFencer, SkillMiner, SkillDigger, SkillCloner,
            SkillEachLimit, UseOnlySkill
        }
        public static Array TalismanReqArray => Enum.GetValues(typeof(C.TalismanReq));



        public static readonly List<string> TalismanSkills = new List<string>()
        {
            "Walker", "Jumper", "Shimmier", "Climber", "Swimmer", "Floater", "Glider",
            "Disarmer", "Bomber", "Stoner", "Blocker",
            "Builder", "Platformer", "Stacker",
            "Basher", "Fencer", "Miner", "Digger", "Cloner"
        };

        public static readonly Dictionary<TalismanReq, string> TalismanReqText = new Dictionary<TalismanReq, string>()
        {
            { TalismanReq.SaveReq, "Save Requirement" },
            { TalismanReq.TimeLimit, "Time Limit" },
            { TalismanReq.SkillTotal, "Limit Total Skills" },
            { TalismanReq.SkillWalker, "Limit Walkers" },
            { TalismanReq.SkillJumper, "Limit Jumpers" },
            { TalismanReq.SkillShimmier, "Limit Shimmiers" },
            { TalismanReq.SkillClimber, "Limit Climbers" },
            { TalismanReq.SkillSwimmer, "Limit Swimmers" },
            { TalismanReq.SkillFloater, "Limit Floaters" },
            { TalismanReq.SkillGlider, "Limit Gliders" },
            { TalismanReq.SkillDisarmer, "Limit Disarmers" },
            { TalismanReq.SkillBomber, "Limit Bombers" },
            { TalismanReq.SkillStoner, "Limit Stoners" },
            { TalismanReq.SkillBlocker, "Limit Blockers" },
            { TalismanReq.SkillBuilder, "Limit Builders" },
            { TalismanReq.SkillPlatformer, "Limit Platformers" },
            { TalismanReq.SkillStacker, "Limit Stackers" },
            { TalismanReq.SkillBasher, "Limit Bashers" },
            { TalismanReq.SkillFencer, "Limit Fencers" },
            { TalismanReq.SkillMiner, "Limit Miners" },
            { TalismanReq.SkillDigger, "Limit Diggers" },
            { TalismanReq.SkillCloner, "Limit Cloners" },
            { TalismanReq.SkillEachLimit, "Limit All Skills" },
            { TalismanReq.UseOnlySkill, "Using only the Skill" }
            //{ TalismanReq.OnlyOneWorker, "One Worker Lem" },
            //{ TalismanReq.RRMin, "RR Minimum" },
            //{ TalismanReq.RRMax, "RR Maximum" }
        };

        public static readonly Dictionary<TalismanReq, string> TalismanKeys = new Dictionary<TalismanReq, string>()
    {
      { TalismanReq.SaveReq, "SAVE_REQUIREMENT" }, { TalismanReq.TimeLimit, "TIME_LIMIT" },
      { TalismanReq.SkillTotal, "SKILL_LIMIT" }, { TalismanReq.SkillWalker, "WALKER_LIMIT" },
      { TalismanReq.SkillJumper, "JUMPER_LIMIT" }, { TalismanReq.SkillShimmier, "SHIMMIER_LIMIT" },
      { TalismanReq.SkillClimber, "CLIMBER_LIMIT"}, { TalismanReq.SkillSwimmer, "SWIMMER_LIMIT"},
      { TalismanReq.SkillFloater, "FLOATER_LIMIT" }, { TalismanReq.SkillGlider, "GLIDER_LIMIT" },
      { TalismanReq.SkillDisarmer, "DISARMER_LIMIT" }, { TalismanReq.SkillBomber, "BOMBER_LIMIT" },
      { TalismanReq.SkillStoner, "STONER_LIMIT" }, { TalismanReq.SkillBlocker, "BLOCKER_LIMIT"},
      { TalismanReq.SkillBuilder, "BUILDER_LIMIT" }, { TalismanReq.SkillPlatformer, "PLATFORMER_LIMIT" },
      { TalismanReq.SkillStacker, "STACKER_LIMIT" }, { TalismanReq.SkillBasher, "BASHER_LIMIT" },
      { TalismanReq.SkillMiner, "MINER_LIMIT" }, { TalismanReq.SkillDigger, "DIGGER_LIMIT" },
      { TalismanReq.SkillFencer, "FENCER_LIMIT" }, { TalismanReq.SkillCloner, "CLONER_LIMIT" },
      { TalismanReq.SkillEachLimit, "SKILL_EACH_LIMIT" }, { TalismanReq.UseOnlySkill, "USE_ONLY_SKILL" }
    };

        public static readonly string[] MusicExtensions = new List<string>()
    {
      ".ogg", ".it", ".mp3", ".mo3", ".wav", ".aiff", ".aif",
      ".mod", ".xm", ".s3m", ".mtm", ".umx"
    }.ToArray();

        public static readonly List<string> MusicNames = new List<string>()
    {
      "orig_01", "orig_02", "orig_03", "orig_04", "orig_05", "orig_06", "orig_07", "orig_08", "orig_09", "orig_10",
      "orig_11", "orig_12", "orig_13", "orig_14", "orig_15", "orig_16", "orig_17",
      "ohno_01", "ohno_02", "ohno_03", "ohno_04", "ohno_05", "ohno_06",
      "xmas_01", "xmas_02", "xmas_03"
    };


        public static readonly THotkeyTexts HotkeyDict = new THotkeyTexts
    {
      { HotkeyTabs.General, new List<string>
        {
          "Alt + F4",
          "Esc",
          "Ctrl + N",
          "Ctrl + O",
          "Ctrl + S",
          "Ctrl + Alt + S",
          "",
          "F1",
          "F2",
          "F3",
          "F4",
          "F5",
          "F6",
          "F9",
          "F10",
          "F11",
          "F12"
        }
      },
      { HotkeyTabs.Level, new List<string>
        {
          "Left mouse",
          "",
          "",
          "Right mouse",
          "Middle mouse",
          "Ctrl + mouse",
          "Alt + mouse",
          "P + Left mouse",
          "Mouse wheel",
          "Numpad plus/minus",
          "",
          "Shift + Right/Left",
          "Shift + Alt + Right/Left",
          "Shift + Up/Down",
          "Space",
          "1, 2, 3, ..., 9, 0",
          "",
          "P + Up/Down/Right/Left"
        }
      },
      { HotkeyTabs.Pieces, new List<string>
        {
          "Ctrl + Z",
          "Ctrl + Shift + Z",
          "Ctrl + Y",
          "Ctrl + X",
          "Ctrl + V",
          "Ctrl + C",
          "C",
          "Delete",
          "",
          "Up/Down/Right/Left",
          "Ctrl + Up/Down/Right/Left",
          "R",
          "E",
          "W",
          "G",
          "H",
          "",
          "A",
          "S",
          "D",
          "F",
          "",
          "Home",
          "Page Up",
          "Page Down",
          "End"
        }
      }
    };

        public static readonly THotkeyTexts DescriptionDict = new THotkeyTexts
    {
      { HotkeyTabs.General, new List<string>
        {
            "Exit the editor.",
            "Close secondary windows.",
            "Create a new empty level.",
            "Load a new level.",
            "Save the current level.",
            "Save the current level in a new file.",
            "",
            "Switch to and from Clear Physics mode.",
            "Switch to and from displaying terrain.",
            "Switch to and from displaying interactive objects.",
            "Switch to and from displaying trigger areas.",
            "Switch to and from displaying the screen start.",
            "Switch to and from displaying background images.",
            "Toggle grid usage.",
            "Display a window to change global options.",
            "Display this hotkey help window.",
            "Playtest the level in the NeoLemmix player."
        }
      },
      { HotkeyTabs.Level, new List<string>
        {
          "Drag selected pieces or...",
          "Drag new piece from the bottom row or...",
          "Select a single piece or all pieces in an area.",
          "Drag current editor position.",
          "Removes all pieces at cursor location from selection .",
          "Add/Remove a single pieces to current selection.",
          "Invert priority of piece selection.",
          "Drag the start position.",
          "Change zoom.",
          "Change zoom.",
          "",
          "Display previous/next item in piece selection.",
          "Display previous/next group of items in piece selection.",
          "Change style for the piece selection to previous/next one.",
          "Toggle between new terrain and new objects.",
          "Add the respective piece to the level.",
          "",
          "Move start position by 8 pixels."
        }
      },
      { HotkeyTabs.Pieces, new List<string>
        {
          "Undo last change.",
          "Revert last Undo and redo that action.",
          "Revert last Undo and redo that action.",
          "Cut selected pieces.",
          "Insert previously deleted or copied pieces.",
          "Copy selected pieces.",
          "Duplicate selected pieces.",
          "Delete selected pieces.",
          "",
          "Move selected pieces.",
          "Move selected pieces 8 pixels.",
          "Rotate selected pieces.",
          "Flip selected pieces.",
          "Invert selected pieces.",
          "Group selected pieces.",
          "Ungroup all selected groups.",
          "",
          "Toggle drawing selected pieces as erasing.",
          "Toggle drawing selected pieces below existing terrain.",
          "Toggle drawing selected pieces only on existing terrain.",
          "Toggle allowing one-way-walls on selected pieces.",
          "",
          "Take selected pieces to front.",
          "Take selected pieces one step to front.",
          "Take selected pieces one step to back.",
          "Take selected pieces to back."
        }
      }
    };

        public static readonly List<string> VersionList = new List<string>
    {
      "Version " + Version,
      "   by Stephan Neupert (Nepster), updates by Namida Verasche (namida)",
      "",
      "Thanks to...",
      "  DMA for creating the original Lemmings games.",
      "  Namida Verasche for the NeoLemmix player.",
      "  The LemmingsForums at http://www.lemmingsforums.net.",
      "",
      "This application and all its source code is licensed under",
      "   CC BY-NC 4.0."
    };


        public static readonly Dictionary<int, string> FileConverterErrorMsg = new Dictionary<int, string>()
    {
      { 2, "Warning: Could not convert some object properties to the nxlv. format due to missing .nxmo files." },
      { 90, "Error: Level converter got passed invalid file paths." },
      { 91, "Error: Level to convert to .nxlv format is not a .lvl file." },
      { 92, "Error: Level converter could not find the translation table .nxtt for the graphic style used in the level." },
      { 99, "Error: Level converter encountered an unknown error." }
    };
    }
}
