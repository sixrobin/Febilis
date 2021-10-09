namespace Templar.Settings
{
    using System.Xml.Linq;

    public class AxisDeadZone : FloatSetting
    {
        public const string SAVE_ELEMENT_NAME = "AxisDeadZone";

        public AxisDeadZone() : base()
        {
        }

        public AxisDeadZone(XElement element) : base(element)
        {
        }

        public override string SaveElementName => SAVE_ELEMENT_NAME;

        public override (float Min, float Max) Range => (0f, 1f);
    }
}