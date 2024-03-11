namespace RSLib.Framework.GameSettings
{
    using UnityEngine;

    public class ShakeAmount : GameSettingFloat
    {
        public const string SERIALIZATION_NAME = "ShakeAmount";

        public ShakeAmount() : base() { }
        public ShakeAmount(System.Xml.Linq.XElement saveElement) : base(saveElement) { }

        public override string SerializationName => SERIALIZATION_NAME;

        public override (float Min, float Max) Range => (0f, 2f);
        public override float Default => 1f;
    }
}