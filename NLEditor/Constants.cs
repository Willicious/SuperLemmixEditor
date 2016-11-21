using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Windows.Forms;
using System.Drawing;

namespace NLEditor
{
    public class C // for Constants
    {
        static C() { }

        public static string Version
        {
            get { return "V0.1"; }
        }

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

        public enum DIR { N, E, S, W }

        public enum OBJ 
        { 
            NULL = -1, STEEL = -2, NONE = 0, EXIT = 1, FORCE_LEFT = 2,
            FORCE_RIGHT = 3, TRAP = 4, WATER = 5, FIRE = 6, OWW_LEFT = 7,
            OWW_RIGHT = 8, TELEPORTER = 11, RECEIVER = 12, LEMMING = 13, PICKUP = 14,
            EXIT_LOCKED = 15, BUTTON = 17, RADIATION = 18, OWW_DOWN = 19, UPDRAFT = 20,
            SPLITTER = 21, SLOWFREEZE = 22, HATCH = 23, ANIMATION = 24, NOSPLAT = 26,
            SPLAT = 27, BACKGROUND = 30, TRAPONCE = 31
        }


        public static readonly int LAY_COUNT = 6;
        public static readonly int LAY_OBJBACK = 0;
        public static readonly int LAY_TERRAIN = 1;
        public static readonly int LAY_OBJTOP = 3;
        public static readonly int LAY_TRIGGER = 4;
        public static readonly int LAY_OWWTERRAIN = 5;


        public static readonly int SKI_COUNT = 16; // count without zombie!
        public static readonly int SKI_CLIMBER = 0;
        public static readonly int SKI_FLOATER = 1;
        public static readonly int SKI_BLOCKER = 2;
        public static readonly int SKI_EXPLODER = 3;
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

    }

    static class Utility
    {
        public static bool In<T>(this T obj, params T[] args)
        {
            return args.Contains(obj);
        }

        public static Rectangle RectangleFrom(Point Pos1, Point Pos2)
        {
            int Left = Math.Min(Pos1.X, Pos2.X);
            int Top = Math.Min(Pos1.Y, Pos2.Y);
            int Width = Math.Abs(Pos1.X - Pos2.X);
            int Height = Math.Abs(Pos1.Y - Pos2.Y);

            return new Rectangle(Left, Top, Width, Height);  
        }

        public static void LogException(Exception Ex)
        {
            String ErrorPath = C.AppPath + "ErrorLog.txt";
            TextWriter TextFile = new StreamWriter(ErrorPath, true);
            TextFile.WriteLine(Ex.ToString());
            TextFile.Close();
        }

        public static void LogProfiling(long Time)
        {
            String LogPath = C.AppPath + "ProfilingLog.txt";
            TextWriter TextFile = new StreamWriter(LogPath, true);
            TextFile.WriteLine(Time.ToString());
            TextFile.Close();
        }

    }
}
