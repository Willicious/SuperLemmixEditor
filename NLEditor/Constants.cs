using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace NLEditor
{
    public static class C // for Constants
    {
        public static string Version
        {
            get
            {
                var version = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version;

                if (version.Build > 0)
                {
                    return version.Major.ToString() + "." + version.Minor.ToString() + "." + version.Build.ToString();
                }
                else
                {
                    return version.Major.ToString() + "." + version.Minor.ToString();
                }
            }
        }

        public static string AppPath => System.Windows.Forms.Application.StartupPath + DirSep;
        public static string AppPathAutosave => AppPath + "autosave" + DirSep;
        public static string AppPathPieces => AppPath + "styles" + DirSep;
        public static string AppPathMusic => AppPath + "music" + DirSep;
        public static string AppPathLevels => AppPath + "levels" + DirSep;
        public static string AppPathTempLevel => AppPath + "TempTestLevel.nxlv";
        public static string AppPathThemeInfo(string styleName) => AppPathPieces + styleName + C.DirSep + "theme.nxtm";
        public static string AppPathSettings => AppPath + "settings" + DirSep + "SLXEditorSettings.ini";
        public static string AppPathHotkeys => AppPath + "settings" + DirSep + "SLXEditorHotkeys.ini";
        public static string AppPathPlayerSettings => AppPath + "settings" + DirSep + "settings.ini";
        public static string AppPathPlayerSettingsOld => AppPath + "SuperLemmix147Settings.ini";
        public static string AppPathSuperLemmix => AppPath + "SuperLemmix.exe";
        public static string AppPathNeoLemmix => AppPath + "NeoLemmix.exe";
        public static string AppPathNeoLemmixCE => AppPath + "NeoLemmixCE.exe";

        public static char DirSep => System.IO.Path.DirectorySeparatorChar;
        public static string NewLine => Environment.NewLine;

        public static Size PicPieceSize => new Size(84, 84);

        public static ScreenSize ScreenSize;

        public enum SelectPieceType
        {
            Terrain, Steel, Objects, Backgrounds, Sketches
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
        /// Warning: The values of the object types here do NOT correspond to the numbers used in SuperLemmix! 
        /// </summary>
        public enum OBJ
        {
            TERRAIN = -1, STEEL = -2,
            LEMMING = 0, HATCH = 1, EXIT = 2, EXIT_LOCKED = 3,
            TRAP = 4, TRAPONCE = 5, FIRE = 6,
            WATER = 7, BLASTICINE = 8, VINEWATER = 9, POISON = 10, LAVA = 11,
            ONE_WAY_WALL = 12, FORCE_FIELD = 13,
            PICKUP = 14, BUTTON = 15, COLLECTIBLE = 16,
            TELEPORTER = 17, RECEIVER = 18,
            UPDRAFT = 19, SPLAT = 20, SPLITTER = 21,
            RADIATION = 22, SLOWFREEZE = 23,
            DECORATION = 24, PAINT = 25,
            PORTAL = 26,
            NEUTRALIZER = 27, DENEUTRALIZER = 28,
            SKILL_ADD = 29, SKILL_REMOVE = 30,
            NONE = 100, NULL
        }

        public static OBJ[] HideTriggerObjects = new OBJ[] { OBJ.TERRAIN, OBJ.STEEL, OBJ.NONE, OBJ.DECORATION, OBJ.NULL, OBJ.PAINT };
        public static OBJ[] TriggerPointObjects = new OBJ[] { OBJ.HATCH, OBJ.RECEIVER };

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
          {OBJ.TERRAIN, "Terrain"}, {OBJ.STEEL, "Steel"}, {OBJ.NONE, "No Effect"},
          {OBJ.EXIT, "Exit"}, {OBJ.FORCE_FIELD, "Force-Field"}, {OBJ.ONE_WAY_WALL, "One-Way-Wall"}, {OBJ.PAINT, "Paint"},
          {OBJ.TRAP, "Triggered Trap"}, {OBJ.WATER, "Water"}, {OBJ.FIRE, "Fire"},
          {OBJ.BLASTICINE, "Blasticine"}, {OBJ.VINEWATER, "Vinewater"}, {OBJ.POISON, "Poison"}, {OBJ.LAVA, "Lava"},
          {OBJ.RADIATION, "Radiation"}, {OBJ.SLOWFREEZE, "Slowfreeze"},
          {OBJ.TELEPORTER, "Teleporter"}, {OBJ.RECEIVER, "Receiver"}, {OBJ.LEMMING, "Preplaced Lemming"},
          {OBJ.PICKUP, "Pickup Skill"}, {OBJ.EXIT_LOCKED, "Locked Exit"}, {OBJ.BUTTON, "Button"},
          {OBJ.COLLECTIBLE, "Collectible"}, {OBJ.UPDRAFT, "Updraft"},
          {OBJ.SPLITTER, "Splitter"}, {OBJ.HATCH, "Hatch"},
          {OBJ.SPLAT, "Splat Pad"}, {OBJ.DECORATION, "Decoration"}, {OBJ.TRAPONCE, "Single-Use Trap"},
          {OBJ.PORTAL, "Portal" }, {OBJ.NEUTRALIZER, "Neutralizer" }, {OBJ.SKILL_ADD, "PermaSkill Assigner" },
          {OBJ.DENEUTRALIZER, "Deneutralizer" }, {OBJ.SKILL_REMOVE, "PermaSkills Remover" }
        };

        public enum DragActions
        {
            Null, SelectArea, MaybeDragPieces,
            DragPieces, HorizontalDrag, VerticalDrag,
            DragNewPiece, MoveEditorPos, MoveStartPos
        }

        public enum Resize { None, Vert, Horiz, Both }

        public static readonly byte ALPHA_OWW = 255;
        public static readonly byte ALPHA_NOOWW = 254;

        public enum Layer { Background, ObjBack, Terrain, ObjTop, Trigger }
        public static readonly List<Layer> LayerList = new List<Layer>()
    {
      Layer.Background, Layer.ObjBack, Layer.Terrain, Layer.ObjTop, Layer.Trigger
    };

        // The integer values here are only used to pick the correct frame of pickupanim.png
        public enum Skill
        {
            // Non-pickup-skills need to be <0 as they aren't used
            Neutral = -3, Zombie = -2, Rival = -1,
            
            // Use frame 0 for Skill.None
            None = 0,

            // All pickup skills are 2 frames apart
            Walker = 1, Jumper = 3, Shimmier = 5, Ballooner = 7,
            Slider = 9, Climber = 11, Swimmer = 13,
            Floater = 15, Glider = 17, Disarmer = 19,
            Timebomber = 21, Bomber = 23, Freezer = 25, Stoner = 27,
            Blocker = 29,
            Ladderer = 31, Platformer = 33, Builder = 35, Stacker = 37,
            Spearer = 39, Grenader = 41, Laserer = 43,
            Basher = 45, Fencer = 47, Miner = 49, Digger = 51,
            Cloner = 53
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

        public enum TalismanType { Bronze, Silver, Gold }
        public enum TalismanReq
        {
            SaveReq, TimeLimit, KillZombies, ClassicMode, NoPause,
            SkillTotal, SkillTypes, SkillEachLimit,
            SkillWalker, SkillJumper, SkillShimmier, SkillBallooner,
            SkillSlider, SkillClimber, SkillSwimmer, SkillFloater, SkillGlider,
            SkillDisarmer, SkillTimebomber, SkillBomber, SkillFreezer, SkillStoner,
            SkillBlocker,
            SkillLadderer, SkillPlatformer, SkillBuilder, SkillStacker,
            SkillSpearer, SkillGrenader, SkillLaserer,
            SkillBasher, SkillFencer, SkillMiner, SkillDigger, SkillCloner,
            UseOnlySkill
        }
        public static Array TalismanReqArray => Enum.GetValues(typeof(C.TalismanReq));

        public static readonly List<string> TalismanSkills = new List<string>()
        {
            "Walker", "Jumper", "Shimmier", "Ballooner",
            "Slider", "Climber", "Swimmer", "Floater", "Glider", 
            "Disarmer", "Timebomber", "Bomber", "Freezer", "Stoner",
            "Blocker",
            "Ladderer", "Platformer", "Builder", "Stacker",
            "Spearer", "Grenader", "Laserer",
            "Basher", "Fencer", "Miner", "Digger", "Cloner"
        };

        public static readonly Dictionary<TalismanReq, string> TalismanReqText = new Dictionary<TalismanReq, string>()
        {
            { TalismanReq.SaveReq, "Save Requirement" },
            { TalismanReq.TimeLimit, "Time Limit" },
            { TalismanReq.SkillTotal, "Limit Total Skills" },
            { TalismanReq.SkillTypes, "Limit Skill Types" },
            { TalismanReq.SkillWalker, "Limit Walkers" },
            { TalismanReq.SkillJumper, "Limit Jumpers" },
            { TalismanReq.SkillShimmier, "Limit Shimmiers" },
            { TalismanReq.SkillBallooner, "Limit Ballooners" },
            { TalismanReq.SkillSlider, "Limit Sliders" },
            { TalismanReq.SkillClimber, "Limit Climbers" },
            { TalismanReq.SkillSwimmer, "Limit Swimmers" },
            { TalismanReq.SkillFloater, "Limit Floaters" },
            { TalismanReq.SkillGlider, "Limit Gliders" },
            { TalismanReq.SkillDisarmer, "Limit Disarmers" },
            { TalismanReq.SkillTimebomber, "Limit Timebombers" },
            { TalismanReq.SkillBomber, "Limit Bombers" },
            { TalismanReq.SkillFreezer, "Limit Freezers" },
            { TalismanReq.SkillStoner, "Limit Stoners" },
            { TalismanReq.SkillBlocker, "Limit Blockers" },
            { TalismanReq.SkillLadderer, "Limit Ladderers" },
            { TalismanReq.SkillPlatformer, "Limit Platformers" },
            { TalismanReq.SkillBuilder, "Limit Builders" },
            { TalismanReq.SkillStacker, "Limit Stackers" },
            { TalismanReq.SkillSpearer, "Limit Spearers" },
            { TalismanReq.SkillGrenader, "Limit Grenaders" },
            { TalismanReq.SkillLaserer, "Limit Laserers" },
            { TalismanReq.SkillBasher, "Limit Bashers" },
            { TalismanReq.SkillFencer, "Limit Fencers" },
            { TalismanReq.SkillMiner, "Limit Miners" },
            { TalismanReq.SkillDigger, "Limit Diggers" },
            { TalismanReq.SkillCloner, "Limit Cloners" },
            { TalismanReq.SkillEachLimit, "Limit All Skills" },
            { TalismanReq.UseOnlySkill, "Using only the Skill" },
            { TalismanReq.KillZombies, "Kill All Zombies" },
            { TalismanReq.ClassicMode, "Play in Classic Mode" },
            { TalismanReq.NoPause, "Play Without Pressing Pause" }
            //{ TalismanReq.OnlyOneWorker, "One Worker Lem" }, //bookmark
            //{ TalismanReq.RRMin, "RR Minimum" }, //bookmark
            //{ TalismanReq.RRMax, "RR Maximum" }  //bookmark
        };

        public static readonly Dictionary<TalismanReq, string> TalismanKeys = new Dictionary<TalismanReq, string>()
        {
          { TalismanReq.SaveReq, "SAVE_REQUIREMENT" }, { TalismanReq.TimeLimit, "TIME_LIMIT" },
          { TalismanReq.SkillTotal, "SKILL_LIMIT" }, { TalismanReq.SkillTypes, "SKILL_TYPE_LIMIT" },
          { TalismanReq.SkillWalker, "WALKER_LIMIT" },
          { TalismanReq.SkillJumper, "JUMPER_LIMIT" }, { TalismanReq.SkillShimmier, "SHIMMIER_LIMIT" },
          { TalismanReq.SkillBallooner, "BALLOONER_LIMIT" },
          { TalismanReq.SkillSlider, "SLIDER_LIMIT" }, { TalismanReq.SkillClimber, "CLIMBER_LIMIT"},
          { TalismanReq.SkillSwimmer, "SWIMMER_LIMIT"}, { TalismanReq.SkillFloater, "FLOATER_LIMIT" },
          { TalismanReq.SkillGlider, "GLIDER_LIMIT" }, { TalismanReq.SkillDisarmer, "DISARMER_LIMIT" },
          { TalismanReq.SkillTimebomber, "TIMEBOMBER_LIMIT" }, { TalismanReq.SkillBomber, "BOMBER_LIMIT" },
          { TalismanReq.SkillFreezer, "FREEZER_LIMIT"}, { TalismanReq.SkillStoner, "STONER_LIMIT"},
          { TalismanReq.SkillBlocker, "BLOCKER_LIMIT"},
          { TalismanReq.SkillLadderer, "LADDERER_LIMIT" }, { TalismanReq.SkillPlatformer, "PLATFORMER_LIMIT" },
          { TalismanReq.SkillBuilder, "BUILDER_LIMIT" }, { TalismanReq.SkillStacker, "STACKER_LIMIT" },
          { TalismanReq.SkillSpearer, "SPEARER_LIMIT" }, { TalismanReq.SkillGrenader, "GRENADER_LIMIT" },
          { TalismanReq.SkillLaserer, "LASERER_LIMIT" },
          { TalismanReq.SkillBasher, "BASHER_LIMIT" }, { TalismanReq.SkillMiner, "MINER_LIMIT" },
          { TalismanReq.SkillDigger, "DIGGER_LIMIT" }, { TalismanReq.SkillFencer, "FENCER_LIMIT" },
          { TalismanReq.SkillCloner, "CLONER_LIMIT" }, { TalismanReq.SkillEachLimit, "SKILL_EACH_LIMIT" },
          { TalismanReq.UseOnlySkill, "USE_ONLY_SKILL" }, { TalismanReq.KillZombies, "KILL_ZOMBIES" },
          { TalismanReq.ClassicMode, "CLASSIC_MODE" }, { TalismanReq.NoPause, "NO_PAUSE" }
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

        public static readonly Dictionary<int, string> FileConverterErrorMsg = new Dictionary<int, string>()
        {
          { 2, "Warning: Could not convert some object properties to the nxlv. format due to missing .nxmo files." },
          { 90, "Error: Level converter got passed invalid file paths." },
          { 92, "Error: Level converter could not find the translation table .nxtt for the graphic style used in the level." },
          { 99, "Error: Level converter encountered an unknown error." }
        };
    }
}
