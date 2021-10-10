namespace Templar.Settings
{
    using System.Xml.Linq;

    public class ConstrainCursor : BoolSetting
    {
        public const string SAVE_ELEMENT_NAME = "ConstrainCursor";

        public ConstrainCursor() : base()
        {
        }

        public ConstrainCursor(XElement element) : base(element)
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
                UnityEngine.Cursor.lockState = value ? UnityEngine.CursorLockMode.Confined : UnityEngine.CursorLockMode.None;
            }
        }

        public override void Init()
        {
            Value = false;
        }
    }
}