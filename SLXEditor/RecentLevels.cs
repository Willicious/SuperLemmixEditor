using System;
using System.Collections.Generic;
using System.IO;

namespace SLXEditor
{
    public class RecentLevels
    {
        private const int MaxLevels = 20;

        private readonly List<string> levels = new List<string>();

        public IReadOnlyList<string> Levels => levels;

        public void Add(string filename)
        {
            filename = Path.GetFullPath(filename);

            levels.RemoveAll(f =>
                string.Equals(f, filename, StringComparison.OrdinalIgnoreCase));

            levels.Insert(0, filename);

            if (levels.Count > MaxLevels)
                levels.RemoveRange(MaxLevels, levels.Count - MaxLevels);
        }

        public void Remove(string filename)
        {
            levels.RemoveAll(f =>
                string.Equals(f, filename, StringComparison.OrdinalIgnoreCase));
        }

        public void Clear()
        {
            levels.Clear();
        }
    }
}
