using System;
using System.Collections.Generic;

namespace NLEditor
{
    class Background
    {
        public Background(Style newStyle, string newName)
        {
            this.Style = newStyle;
            this.Name = newName;
        }

        public Style Style { get; private set; }
        public string Name { get; private set; }
        public string Key => Style.NameInDirectory + C.DirSep + "backgrounds" + C.DirSep + Name;
    }

    class BackgroundList
    {
        public BackgroundList()
        {
            backgroundList = new List<Background>();
        }

        List<Background> backgroundList;

        /// <summary>
        /// Adds a new background.
        /// </summary>
        /// <param name="newBackground"></param>
        public void Add(Background newBackground)
        {
            backgroundList.Add(newBackground);
        }

        /// <summary>
        /// Adds a new background by specifying the correcposnding style and the background name.
        /// </summary>
        /// <param name="newStyle"></param>
        /// <param name="newName"></param>
        public void Add(Style newStyle, string newName)
        {
            backgroundList.Add(new Background(newStyle, newName));
        }

        /// <summary>
        /// Sorts the backgrounds according to style and background name.
        /// </summary>
        public void SortBackgrounds()
        {
            backgroundList.Sort(delegate (Background bg1, Background bg2)
            {
                if (bg1?.Style == null && bg2?.Style == null)
                    return 0;
                else if (bg1?.Style == null)
                    return -1;
                else if (bg2?.Style == null)
                    return 1;
                else if (!bg1.Style.Equals(bg2.Style))
                    return bg1.Style.NameInEditor.CompareTo(bg2.Style.NameInEditor);
                else
                    return bg1.Name.CompareTo(bg2.Name);
            });
        }

        /// <summary>
        /// Gets a list of background names, starting with the one in the mainStyle.
        /// </summary>
        /// <param name="mainStyle"></param>
        /// <returns></returns>
        public List<string> GetDisplayNames(Style mainStyle)
        {
            List<string> displayNames = new List<string>() { "--none--" };
            displayNames.AddRange(backgroundList.FindAll(bg => bg.Style.Equals(mainStyle)).ConvertAll(bg => bg.Name));
            displayNames.AddRange(backgroundList.FindAll(bg => !bg.Style.Equals(mainStyle)).ConvertAll(bg => bg.Name));
            return displayNames;
        }

        /// <summary>
        /// Finds a background by its name.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public Background Find(string name)
        {
            name = name.Trim();
            return backgroundList.Find(bg => bg.Name.Equals(name));
        }

        /// <summary>
        /// Finds a background by its name and corresponding style
        /// </summary>
        /// <param name="name"></param>
        /// <param name="style"></param>
        /// <returns></returns>
        public Background Find(string name, Style style)
        {
            name = name.Trim();
            return backgroundList.Find(bg => bg.Name.Equals(name) && bg.Style.Equals(style));
        }
    }
}
