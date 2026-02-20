using System.Windows.Forms;
using static SLXEditor.HotkeyConfig;

namespace SLXEditor
{
    public class HotkeyData
    {
        public HotkeyName Name { get; }
        public string Description { get; }
        public Keys DefaultKeys { get; }
        public Keys CurrentKeys { get; set; }
        public bool RequiresMouseButton { get; }

        public HotkeyData(
            HotkeyName name,
            string description,
            Keys defaultKeys,
            bool requiresMouseButton = false)
        {
            Name = name;
            Description = description;
            DefaultKeys = defaultKeys;
            CurrentKeys = defaultKeys;
            RequiresMouseButton = requiresMouseButton;
        }
    }
}
