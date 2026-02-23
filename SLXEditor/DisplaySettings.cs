using System.Collections.Generic;
using System.Windows.Forms;

namespace SLXEditor
{
    static class DisplaySettings
    {
        static Dictionary<C.DisplayType, ToolStripMenuItem> displayMenuItems = new Dictionary<C.DisplayType, ToolStripMenuItem>();
        static HashSet<C.DisplayType> displaySet = new HashSet<C.DisplayType>();

        /// <summary>
        /// Sets the dictionary which DisplayType is modified by which MenuItem.
        /// </summary>
        static public void SetMenuTabItems(Dictionary<C.DisplayType, ToolStripMenuItem> newDisplayMenuItems)
        {
            displayMenuItems = newDisplayMenuItems;
        }

        /// <summary>
        /// Gets whether a DisplayType is currently drawn on the level image.
        /// </summary>
        static public bool IsDisplayed(C.DisplayType displayType)
        {
            return displaySet.Contains(displayType);
        }

        /// <summary>
        /// Sets whether a DisplayType is drawn on the level image or not.
        /// </summary>
        static public void SetDisplayed(C.DisplayType displayType, bool doDisplay)
        {
            if (doDisplay)
            {
                displaySet.Add(displayType);
            }
            else
            {
                displaySet.Remove(displayType);
            }
            SetCheckmarkOnMenuItem(displayType);
        }

        /// <summary>
        /// Changes whether a DisplayType is drawn on the level image.
        /// </summary>
        static public void ChangeDisplayed(C.DisplayType displayType)
        {
            SetDisplayed(displayType, !IsDisplayed(displayType));
        }

        /// <summary>
        /// Sets the checkmark on the corresponding MenuItem correctly.
        /// </summary>
        static void SetCheckmarkOnMenuItem(C.DisplayType displayType)
        {
            if (displayMenuItems.ContainsKey(displayType))
            {
                displayMenuItems[displayType].Checked = IsDisplayed(displayType);
            }
        }
    }
}
