namespace RSLib.Framework.GameSettings
{
    public class PixelPerfect : GameSettingBool
    {
        public const string SERIALIZATION_NAME = "PixelPerfect";

        public PixelPerfect() : base() { }
        public PixelPerfect(System.Xml.Linq.XElement saveElement) : base(saveElement) { }

        public override string SerializationName => SERIALIZATION_NAME;
    }
}