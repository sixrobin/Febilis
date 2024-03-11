namespace Templar.Settings
{
    using System.Xml.Linq;

    public class RunInBackground : BoolSetting
    {
        public const string SAVE_ELEMENT_NAME = "RunInBackground";

        public RunInBackground() : base()
        {
        }

        public RunInBackground(XElement element) : base(element)
        {
        }

        public override string SaveElementName => SAVE_ELEMENT_NAME;

        public override bool Value
        {
            get => base.Value;
            set
            {
                base.Value = value;

                // This setting should not have any listener so we can set it directly here.
                UnityEngine.Application.runInBackground = value;
            }
        }

        public override void Init()
        {
            Value = false;
        }
    }
}