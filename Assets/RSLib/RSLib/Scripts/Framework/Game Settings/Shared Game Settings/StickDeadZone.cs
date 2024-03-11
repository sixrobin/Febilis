namespace RSLib.Framework.GameSettings
{
    using UnityEngine;

    public class StickDeadZone : GameSettingFloat
    {
        public const string SERIALIZATION_NAME = "StickDeadZone";

        public StickDeadZone() : base() { }
        public StickDeadZone(System.Xml.Linq.XElement saveElement) : base(saveElement) { }

        public override string SerializationName => SERIALIZATION_NAME;

        public override (float Min, float Max) Range => (0f, 0.9f);
        public override float Default => 0.4f;
    }
}