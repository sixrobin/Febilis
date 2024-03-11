namespace Templar.Settings
{
    using System.Xml.Linq;

    public class TargetFrameRate : StringRangeSetting
    {
        public const string SAVE_ELEMENT_NAME = "TargetFrameRate";

        private StringOption[] _options;

        public TargetFrameRate() : base()
        {
        }

        public TargetFrameRate(XElement element) : base(element)
        {
        }

        public override string SaveElementName => SAVE_ELEMENT_NAME;

        public override StringOption Value
        {
            get => base.Value;
            set
            {
                base.Value = value;

                // This setting should not have any listener so we can set it directly here.
                UnityEngine.Assertions.Assert.IsTrue(int.TryParse(Value.StringValue, out _), $"Could not parse {Value.StringValue} to a valid int value.");
                UnityEngine.Application.targetFrameRate = int.Parse(value.StringValue);
            }
        }

        public override StringOption[] Options
        {
            get
            {
                if (_options == null)
                {
                    _options = new StringOption[]
                    {
                        new StringOption("-1", false, "MAX"),
                        new StringOption("24", false),
                        new StringOption("30", false),
                        new StringOption("60", true),
                        new StringOption("120", false),
                    };
                }

                return _options;
            }
        }
    }
}