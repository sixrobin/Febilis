namespace RSLib.Framework.GameSettings
{
    public class ShowTutorials : GameSettingBool
    {
        public const string SERIALIZATION_NAME = "ShowTutorials";

        public ShowTutorials() : base() { }
        public ShowTutorials(System.Xml.Linq.XElement saveElement) : base(saveElement) { }

        public override string SerializationName => SERIALIZATION_NAME;
    }
}