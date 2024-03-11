namespace RSLib.Framework.GameSettings.Example
{
    public class CustomGameSettingsManagerExample : GameSettingsManager
    {
        public CustomFloatSettingExample FloatExample { get; private set; }

        protected override string SavePath => System.IO.Path.Combine(UnityEngine.Application.persistentDataPath, "Save", "TmpSettings.xml");

        protected override void SaveCustomSettings(System.Xml.Linq.XContainer container)
        {
            base.SaveCustomSettings(container);
            container.Add(FloatExample.Save());
        }

        protected override void LoadCustomSettings(System.Xml.Linq.XElement gameSettingsElement)
        {
            base.LoadCustomSettings(gameSettingsElement);
            FloatExample = new CustomFloatSettingExample(gameSettingsElement.Element(CustomFloatSettingExample.SERIALIZATION_NAME));
        }

        public override void Init()
        {
            base.Init();
            FloatExample = new CustomFloatSettingExample();
        }

        protected override string DebugGetSettingsLog()
        {
            const string format = "\n\r{0}: {1}";
            string log = base.DebugGetSettingsLog();

            log += string.Format(format, FloatExample.SerializationName, FloatExample.Value);
            
            return log;
        }
    }
}