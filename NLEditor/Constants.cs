using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Windows.Forms;

namespace NLEditor
{
    class C // for Constants
    {
        static C() { }

        public static string AppPath
        {
            get { return "C:\\Stephan\\Programme\\NLEditor\\"; }
            // get { return Application.StartupPath + DirSep; }
        }

        public static string AppPathPieces
        {
            get { return AppPath + "styles" + DirSep + "pieces" + DirSep; }
        }

        public static char DirSep
        { 
            get { return Path.DirectorySeparatorChar; }
        }

        public static string NewLine
        {
            get { return Environment.NewLine; }
        }

        public const int DIR_N = 0;
        public const int DIR_E = 1;
        public const int DIR_S = 2;
        public const int DIR_W = 3;


        public const int OBJ_NULL = -1; // usual terrain piece!
        public const int OBJ_STEEL = -2; // steel terrain pieces!
        public const int OBJ_NONE = 0;
        public const int OBJ_EXIT = 1;
        public const int OBJ_FORCE_LEFT = 2;
        public const int OBJ_FORCE_RIGHT = 3;
        public const int OBJ_TRAP = 4;
        public const int OBJ_WATER = 5;
        public const int OBJ_FIRE = 6;
        public const int OBJ_OWW_LEFT = 7;
        public const int OBJ_OWW_RIGHT = 8;
        public const int OBJ_TELEPORTER = 11;
        public const int OBJ_RECEIVER = 12;
        public const int OBJ_LEMMING = 13;
        public const int OBJ_PICKUP = 14;
        public const int OBJ_EXIT_LOCKED = 15;
        public const int OBJ_BUTTON = 17;
        public const int OBJ_RADIATION = 18;
        public const int OBJ_OWW_DOWN = 19;
        public const int OBJ_UPDRAFT = 20;
        public const int OBJ_FLIPPER = 21;
        public const int OBJ_SLOWFREEZE = 22;
        public const int OBJ_HATCH = 23;
        public const int OBJ_ANIMATION = 24;
        public const int OBJ_NOSPLAT = 26;
        public const int OBJ_SPLAT = 27;
        public const int OBJ_BACKGROUND = 30;
        public const int OBJ_TRAPONCE = 31;

        public const int SKI_COUNT = 16;
        public const int SKI_CLIMBER = 0;
        public const int SKI_FLOATER = 1;
        public const int SKI_BLOCKER = 2;
        public const int SKI_EXPLODER = 3;
        public const int SKI_BUILDER = 4;
        public const int SKI_BASHER = 5;
        public const int SKI_MINER = 6;
        public const int SKI_DIGGER = 7;
        public const int SKI_WALKER = 8;
        public const int SKI_SWIMMER = 9;
        public const int SKI_GLIDER = 10;
        public const int SKI_DISARMER = 11;
        public const int SKI_STONER = 12;
        public const int SKI_PLATFORMER = 13;
        public const int SKI_STACKER = 14;
        public const int SKI_CLONER = 15;

    }

    static class Utility
    {
        public static bool In<T>(this T obj, params T[] args)
        {
            return args.Contains(obj);
        }
    }
}
