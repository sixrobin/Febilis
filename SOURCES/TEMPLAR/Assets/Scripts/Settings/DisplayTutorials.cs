namespace Templar.Settings
{
    using System.Xml.Linq;

    public class DisplayTutorials : BoolSetting
    {
        public const string SAVE_ELEMENT_NAME = "DisplayTutorials";

        public DisplayTutorials() : base()
        {
        }

        public DisplayTutorials(XElement element) : base(element)
        {
        }

        public override string SaveElementName => SAVE_ELEMENT_NAME;

        public override void Init()
        {
            Value = true;
        }
    }
}