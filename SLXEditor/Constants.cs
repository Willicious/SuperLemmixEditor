using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace SLXEditor
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
        public static string AppPathTemplates => AppPath + "templates" + DirSep;
        public static string AppPathAutosave => AppPath + "autosave" + DirSep;
        public static string AppPathStyles => AppPath + "styles" + DirSep;
        public static string AppPathRulers => AppPath + "rulers" + DirSep;
        public static string AppPathMusic => AppPath + "music" + DirSep;
        public static string AppPathLevels => AppPath + "levels" + DirSep;
        public static string AppPathTempLevel => AppPath + "TempTestLevel";
        public static string AppPathThemeInfo(string styleName) => AppPathStyles + styleName + C.DirSep + "theme.nxtm";
        public static string AppPathSettings => AppPath + "settings" + DirSep;
        public static string AppPathEditorSettings => AppPathSettings + "SLXEditorSettings.ini";
        public static string AppPathHotkeys => AppPathSettings + "SLXEditorHotkeys.ini";
        public static string AppPathCustomSkillsets => AppPathSettings + "SLXEditorCustomSkillsets.ini";
        public static string AppPathTranslationTables => AppPathSettings + "SLXEditorINITranslationTables.ini";
        public static string AppPathPlayerSettings => AppPathSettings + "settings.ini";
        public static string AppPathSuperLemmix => AppPath + "SuperLemmix.exe";
        public static string AppPathNeoLemmix => AppPath + "NeoLemmix.exe";
        public static string AppPathNeoLemmixCE => AppPath + "NeoLemmixCE.exe";

        public static char DirSep => System.IO.Path.DirectorySeparatorChar;
        public static string NewLine => Environment.NewLine;

        public static Size PicPieceSize => new Size(84, 84);

        public static ScreenSize ScreenSize;

        public enum SelectPieceType
        {
            Terrain, Steel, Objects, Backgrounds, Rulers
        }

        public enum DisplayType
        {
            Terrain, Objects, Triggers, Rulers, ScreenStart,
            Background, ClearPhysics, Deprecated
        }

        public enum CustDrawMode
        {
            Default, DefaultOWW, Erase, OnlyAtMask, OnlyAtOWW,
            NoOverwrite, NoOverwriteOWW,
            ClearPhysics, ClearPhysicsOWW, ClearPhysicsSteel,
            ClearPhysicsNoOverwrite, ClearPhysicsNoOverwriteOWW, ClearPhysicsSteelNoOverwrite,
            HighlightGroups,
            Custom
        }

        public enum DIR { N, E, S, W }

        /// <summary>
        /// Warning: The values of the object types here do NOT correspond to the numbers used in SuperLemmix! 
        /// </summary>
        public enum OBJ
        {
            TERRAIN = -1, STEEL = -2, RULER = -3,
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
            PERMASKILL_ADD = 29, PERMASKILL_REMOVE = 30,
            SKILL_ASSIGNER = 31,
            NONE = 100, NULL
        }

        public static OBJ[] SuperLemmixObjects = new OBJ[] { OBJ.BLASTICINE, OBJ.VINEWATER, OBJ.LAVA, OBJ.POISON, OBJ.RADIATION, OBJ.SLOWFREEZE, OBJ.COLLECTIBLE };
        public static OBJ[] HideTriggerObjects = new OBJ[] { OBJ.TERRAIN, OBJ.STEEL, OBJ.RULER, OBJ.NONE, OBJ.DECORATION, OBJ.NULL, OBJ.PAINT };
        public static OBJ[] TriggerPointObjects = new OBJ[] { OBJ.HATCH, OBJ.RECEIVER };

        public enum StyleColor
        {
            BACKGROUND, ONE_WAY_WALL, MASK, PICKUP_BORDER, PICKUP_INSIDE
        }
        public static SLXColor ToSLXColor(this StyleColor styleColor)
        {
            switch (styleColor)
            {
                case StyleColor.BACKGROUND:
                    return SLXColor.BackDefault;
                case StyleColor.ONE_WAY_WALL:
                    return SLXColor.OWWDefault;
                default:
                    return SLXColor.BackDefault;
            }
        }

        public static readonly Dictionary<OBJ, string> ObjectDescriptions = new Dictionary<OBJ, string>
        {
          {OBJ.TERRAIN, "Terrain"}, {OBJ.STEEL, "Steel"}, {OBJ.RULER, "Ruler"}, {OBJ.NONE, "No Effect"},
          {OBJ.EXIT, "Exit"}, {OBJ.FORCE_FIELD, "Force-Field"}, {OBJ.ONE_WAY_WALL, "One-Way"}, {OBJ.PAINT, "Paint"},
          {OBJ.TRAP, "Trap"}, {OBJ.WATER, "Water"}, {OBJ.FIRE, "Fire"},
          {OBJ.BLASTICINE, "Blasticine"}, {OBJ.VINEWATER, "Vinewater"}, {OBJ.POISON, "Poison"}, {OBJ.LAVA, "Lava"},
          {OBJ.RADIATION, "Radiation"}, {OBJ.SLOWFREEZE, "Slowfreeze"},
          {OBJ.TELEPORTER, "Teleporter"}, {OBJ.RECEIVER, "Receiver"}, {OBJ.LEMMING, "Lemming"},
          {OBJ.PICKUP, "Pickup Skill"}, {OBJ.EXIT_LOCKED, "Locked Exit"}, {OBJ.BUTTON, "Button"},
          {OBJ.COLLECTIBLE, "Collectible"}, {OBJ.UPDRAFT, "Updraft"},
          {OBJ.SPLITTER, "Splitter"}, {OBJ.HATCH, "Hatch"},
          {OBJ.SPLAT, "Splat Pad"}, {OBJ.DECORATION, "Decoration"}, {OBJ.TRAPONCE, "Single Trap"},
          {OBJ.PORTAL, "Portal" }, {OBJ.NEUTRALIZER, "Neutralizer" }, {OBJ.PERMASKILL_ADD, "PermaSkill Assigner" },
          {OBJ.DENEUTRALIZER, "Deneutralizer" }, {OBJ.PERMASKILL_REMOVE, "PermaSkill Remover" }, {OBJ.SKILL_ASSIGNER, "Skill Assigner"}
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

        public enum Layer { Background, Grid, ObjBack, Terrain, ObjTop, Triggers, Rulers }
        public static readonly List<Layer> LayerList = new List<Layer>()
    {
      Layer.Background, Layer.Grid, Layer.ObjBack, Layer.Terrain, Layer.ObjTop, Layer.Triggers, Layer.Rulers
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

        public static readonly HashSet<Skill> PermaSkills = new HashSet<Skill>
        {
            Skill.Slider,
            Skill.Climber,
            Skill.Swimmer,
            Skill.Floater,
            Skill.Glider,
            Skill.Disarmer
        };

        public static readonly HashSet<Skill> SuperLemmixSkills = new HashSet<Skill>
        {
            Skill.Ballooner,
            Skill.Timebomber,
            Skill.Freezer,
            Skill.Grenader,
            Skill.Spearer,
            Skill.Ladderer
        };

        public static readonly int ZOOM_MIN = -2;
        public static readonly int ZOOM_MAX = 10;

        public static readonly int LEM_OFFSET_X = 2;
        public static readonly int LEM_OFFSET_Y = 9;

        // Other colors are specified directly in BmpModify to speed up rendering.
        public enum SLXColor
        {
            Text, OWWDefault, BackDefault, ScreenStart,
            SelRectGadget, SelRectTerrain, SelRectSteel, SelRectRulers,
            TriggerPink, TriggerYellow, TriggerGreen, TriggerBlue, TriggerPurple
        }
        public static readonly Dictionary<SLXColor, Color> TriggerColors = new Dictionary<SLXColor, Color>()
        {
          { SLXColor.TriggerPink, Utility.HexToColor("55EE88EE") }, // Pink with reduced alpha
          { SLXColor.TriggerYellow, Utility.HexToColor("44FFDD00") }, // Banana with reduced alpha
          { SLXColor.TriggerGreen, Utility.HexToColor("4411FFAA") }, // Mint with reduced alpha
          { SLXColor.TriggerBlue, Utility.HexToColor("4400FFFF") }, // Cyan with reduced alpha
          { SLXColor.TriggerPurple, Utility.HexToColor("44AA00FF") }, // Indigo with reduced alpha
        };
        public static readonly Dictionary<SLXColor, Color> SLXColors = new Dictionary<SLXColor, Color>()
        {
          { SLXColor.Text, Utility.HexToColor("FEF5F5F5") }, // Color.WhiteSmoke with slightly reduced alpha
          { SLXColor.OWWDefault, Color.Linen },
          { SLXColor.BackDefault, Color.Black },
          { SLXColor.ScreenStart, Color.AliceBlue },
          { SLXColor.SelRectGadget, Color.Chartreuse },
          { SLXColor.SelRectTerrain, Color.Gold },
          { SLXColor.SelRectSteel, Color.LightSteelBlue },
          { SLXColor.SelRectRulers, Color.Violet }
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
          "orig_01_cancan_amiga", "orig_02_lemming1_amiga", "orig_03_tim2_amiga",
          "orig_04_lemming2_amiga", "orig_05_tim8_amiga", "orig_06_tim3_amiga",
          "orig_07_tim5_amiga", "orig_08_doggie_amiga", "orig_09_tim6_amiga",
          "orig_10_lemming3_amiga", "orig_11_tim7_amiga", "orig_12_tim9_amiga",
          "orig_13_tim1_amiga", "orig_14_tim10_amiga", "orig_15_tim4_amiga",
          "orig_16_tenlemms_amiga", "orig_17_mountain_amiga",
          "awesome_amiga", "beasti_amiga", "beastii_amiga", "menace_amiga",
          "ohno_tune1_amiga", "ohno_tune2_amiga", "ohno_tune3_amiga",
          "ohno_tune4_amiga", "ohno_tune5_amiga", "ohno_tune6_amiga",
          "WillLem_Xmas_Music/WL_Ding_Dong", "WillLem_Xmas_Music/WL_Hark_Angels",
          "WillLem_Xmas_Music/WL_Jingle_Bells", "WillLem_Xmas_Music/WL_O_Holy_Night",
          "WillLem_Xmas_Music/WL_Rockin_Around", "WillLem_Xmas_Music/WL_Rudolph",
          "WillLem_Xmas_Music/WL_Twelve_Days", "WillLem_Xmas_Music/WL_Winter_Wonderland",
          "xmas_01", "xmas_02", "xmas_03",
        };

        public static readonly Dictionary<int, string> FileConverterErrorMsg = new Dictionary<int, string>()
        {
          { 2, "Warning: Could not convert some object properties to the output format due to missing .nxmo files." },
          { 90, "Error: Level converter got passed invalid file paths." },
          { 92, "Error: Level converter could not find the translation table .nxtt for the graphic style used in the level." },
          { 99, "Error: Level converter encountered an unknown error." }
        };
    }
}
