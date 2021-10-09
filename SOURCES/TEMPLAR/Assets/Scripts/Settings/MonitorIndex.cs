namespace Templar.Settings
{
    using System.Xml.Linq;

    public class MonitorIndex : IntSetting
    {
        public const string SAVE_ELEMENT_NAME = "MonitorIndex";

        public MonitorIndex() : base()
        {
        }

        public MonitorIndex(XElement element) : base(element)
        {
        }

        public override string SaveElementName => SAVE_ELEMENT_NAME;

        public override (int Min, int Max) Range => (0, UnityEngine.Display.displays.Length);

        public override void Init()
        {
            Value = 0;
        }
    }
}