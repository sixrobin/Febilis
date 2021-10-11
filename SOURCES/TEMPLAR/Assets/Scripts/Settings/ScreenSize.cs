namespace Templar.Settings
{
    using System.Xml.Linq;

    public class ScreenSize : StringRangeSetting
    {
        public const string SAVE_ELEMENT_NAME = "ScreenSize";

        private StringOption[] _options;

        public ScreenSize() : base()
        {
        }

        public ScreenSize(XElement element) : base(element)
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
                (int w, int h) = ParseValueToSize();
                UnityEngine.Screen.SetResolution(w, h, UnityEngine.Screen.fullScreen);
            }
        }

        protected override StringOption[] Options
        {
            get
            {
                if (_options == null)
                {
                    _options = new StringOption[]
                    {
                        new StringOption("160x144", false),
                        new StringOption("320x288", false),
                        new StringOption("480x432", false),
                        new StringOption("640x576", true),
                        new StringOption("800x720", false),
                        new StringOption("960x864", false)
                    };
                }

                return _options;
            }
        }

        private (int w, int h) ParseValueToSize()
        {
            string[] values = Value.StringValue.Split('x');

            UnityEngine.Assertions.Assert.IsTrue(int.TryParse(values[0], out _), $"Could not parse {values[0]} to a valid int value.");
            UnityEngine.Assertions.Assert.IsTrue(int.TryParse(values[1], out _), $"Could not parse {values[1]} to a valid int value.");

            return (int.Parse(values[0]), int.Parse(values[1]));
        }
    }
}