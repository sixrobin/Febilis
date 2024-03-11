namespace RSLib.Framework.GameSettings.Example
{
    using UnityEngine;

    public class CustomFloatSettingExample : GameSettingFloat
    {
        public const string SERIALIZATION_NAME = "CustomFloatExample";

        public CustomFloatSettingExample() : base() { }
        public CustomFloatSettingExample(System.Xml.Linq.XElement saveElement) : base(saveElement) { }
        
        public override string SerializationName => SERIALIZATION_NAME;
        public override (float Min, float Max) Range => (-6f, 6f);
    }
}