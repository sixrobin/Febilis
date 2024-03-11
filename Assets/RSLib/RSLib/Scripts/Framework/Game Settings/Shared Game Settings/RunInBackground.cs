namespace RSLib.Framework.GameSettings
{
    public class RunInBackground : GameSettingBool
    {
        public const string SERIALIZATION_NAME = "RunInBackground";

        public RunInBackground() : base() { }
        public RunInBackground(System.Xml.Linq.XElement saveElement) : base(saveElement) { }

        public override string SerializationName => SERIALIZATION_NAME;
    }
}