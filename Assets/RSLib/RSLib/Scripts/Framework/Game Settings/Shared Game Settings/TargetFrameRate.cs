namespace RSLib.Framework.GameSettings
{
    public class TargetFrameRate : GameSettingInt
    {
        public const string SERIALIZATION_NAME = "TargetFrameRate";

        public TargetFrameRate() : base() { }
        public TargetFrameRate(System.Xml.Linq.XElement saveElement) : base(saveElement) { }

        public override string SerializationName => SERIALIZATION_NAME;

        public override int Value
        {
            get => base.Value;
            set
            {
                base.Value = value;
                UnityEngine.Application.targetFrameRate = base.Value;
            }
        }

        public override (int Min, int Max) Range => (-1, 300);

        public override int Default => -1; // Maximum unlimited frame rate.
    }
}