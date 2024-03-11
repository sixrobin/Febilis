namespace RSLib.Framework.GameSettings
{
    public class ConstrainCursor : GameSettingBool
    {
        public const string SERIALIZATION_NAME = "ConstrainCursor";

        public ConstrainCursor() : base() { }
        public ConstrainCursor(System.Xml.Linq.XElement saveElement) : base(saveElement) { }

        public override string SerializationName => SERIALIZATION_NAME;
    }
}