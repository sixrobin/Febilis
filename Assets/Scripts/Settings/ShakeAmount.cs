namespace Templar.Settings
{
    using System.Xml.Linq;

    public class ShakeAmount : FloatSetting
    {
        public const string SAVE_ELEMENT_NAME = "ShakeAmount";

        public ShakeAmount() : base()
        {
        }

        public ShakeAmount(XElement element) : base(element)
        {
        }

        public override string SaveElementName => SAVE_ELEMENT_NAME;

        public override (float Min, float Max) Range => (0f, 2f);
        public override float Default => 1f;
    }
}