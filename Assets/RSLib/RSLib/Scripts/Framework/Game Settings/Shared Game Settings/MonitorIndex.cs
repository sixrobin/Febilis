namespace RSLib.Framework.GameSettings
{
    public class MonitorIndex : GameSettingInt
    {
        public const string SERIALIZATION_NAME = "MonitorIndex";
        private const string MONITOR_PLAYER_PREFS_KEY = "UnitySelectMonitor";
        
        public MonitorIndex() : base() { }
        public MonitorIndex(System.Xml.Linq.XElement saveElement) : base(saveElement) { }

        public override string SerializationName => SERIALIZATION_NAME;

        public override bool UserAssignable => base.UserAssignable && UnityEngine.Display.displays.Length > 1;

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
        public override int Default => 0;
    }
}