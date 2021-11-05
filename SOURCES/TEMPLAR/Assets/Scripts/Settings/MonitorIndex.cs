namespace Templar.Settings
{
    using System.Xml.Linq;

    public class MonitorIndex : IntSetting
    {
        public const string SAVE_ELEMENT_NAME = "MonitorIndex";
        private const string MONITOR_PLAYER_PREFS_KEY = "UnitySelectMonitor";

        public MonitorIndex() : base()
        {
        }

        public MonitorIndex(XElement element) : base(element)
        {
        }

        public override string SaveElementName => SAVE_ELEMENT_NAME;

        public override int Value
        {
            get => base.Value;
            set
            {
                base.Value = value;
                UnityEngine.PlayerPrefs.SetInt(MONITOR_PLAYER_PREFS_KEY, Value);
            }
        }

        public override (int Min, int Max) Range => (0, UnityEngine.Display.displays.Length - 1);

        public override void Init()
        {
            Value = 0;
        }

        public override bool CanBeDisplayedToUser()
        {
            return base.CanBeDisplayedToUser() && UnityEngine.Display.displays.Length > 1;
        }
    }
}