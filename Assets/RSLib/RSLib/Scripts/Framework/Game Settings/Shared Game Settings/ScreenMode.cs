namespace RSLib.Framework.GameSettings
{
    using UnityEngine;

    public class ScreenMode : GameSettingEnum<FullScreenMode>
    {
        public const string SERIALIZATION_NAME = "ScreenMode";

        public ScreenMode() : base() { }
        public ScreenMode(System.Xml.Linq.XElement saveElement) : base(saveElement) { }

        public override string SerializationName => SERIALIZATION_NAME;

        public override FullScreenMode Value
        {
            get => base.Value;
            set
            {
                base.Value = value;
                UnityEngine.Screen.fullScreenMode = base.Value;
            }
        }
        
        public override FullScreenMode Default => FullScreenMode.FullScreenWindow;
    }
}