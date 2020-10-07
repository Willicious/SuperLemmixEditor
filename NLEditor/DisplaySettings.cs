using System.Collections.Generic;
using System.Windows.Forms;

namespace NLEditor
{
    static class DisplaySettings
    {
        static Dictionary<C.DisplayType, ToolStripMenuItem> displayMenuItems = new Dictionary<C.DisplayType, ToolStripMenuItem>();
        static HashSet<C.DisplayType> displaySet = new HashSet<C.DisplayType>();

        /// <summary>
        /// Sets the dictionary which DisplayType is modified by which MenuItem.
        /// </summary>
        /// <param name="newDisplayMenuItems"></param>
        static public void SetMenuTabItems(Dictionary<C.DisplayType, ToolStripMenuItem> newDisplayMenuItems)
        {
            displayMenuItems = newDisplayMenuItems;
        }

        /// <summary>
        /// Gets whether a DisplayType is currently drawn on the level image.
        /// </summary>
        /// <param name="displayType"></param>
        /// <returns></returns>
        static public bool IsDisplayed(C.DisplayType displayType)
        {
            return displaySet.Contains(displayType);
        }

        /// <summary>
        /// Sets whether a DisplayType is drawn on the level image or not.
        /// </summary>
        /// <param name="displayType"></param>
        /// <param name="doDisplay"></param>
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
        /// <param name="displayType"></param>
        static public void ChangeDisplayed(C.DisplayType displayType)
        {
            SetDisplayed(displayType, !IsDisplayed(displayType));
        }

        /// <summary>
        /// Sets the checkmark on the corresponding MenuItem correctly.
        /// </summary>
        /// <param name="displayType"></param>
        static void SetCheckmarkOnMenuItem(C.DisplayType displayType)
        {
            if (displayMenuItems.ContainsKey(displayType))
            {
                displayMenuItems[displayType].Checked = IsDisplayed(displayType);
            }
        }
    }
}
