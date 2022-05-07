namespace Templar.Settings
{
    using System.Xml.Linq;

    public class FullscreenMode : StringRangeSetting
    {
        public const string SAVE_ELEMENT_NAME = "FullscreenMode";
        
        private StringOption[] _options;

        public FullscreenMode() : base()
        {
        }

        public FullscreenMode(XElement element) : base(element)
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
                UnityEngine.Screen.fullScreenMode = ParseValueToFullScreenMode();
            }
        }

        public override StringOption[] Options
        {
            get
            {
                if (_options == null)
                {
                    _options = new[]
                    {
                        //new StringOption(UnityEngine.FullScreenMode.ExclusiveFullScreen.ToString(), false),
                        new StringOption(UnityEngine.FullScreenMode.FullScreenWindow.ToString(), false),
                        new StringOption(UnityEngine.FullScreenMode.MaximizedWindow.ToString(), false),
                        new StringOption(UnityEngine.FullScreenMode.Windowed.ToString(), true)
                    };
                }

                return _options;
            }
        }

        private UnityEngine.FullScreenMode ParseValueToFullScreenMode()
        {
            UnityEngine.Assertions.Assert.IsTrue(
                System.Enum.TryParse<UnityEngine.FullScreenMode>(Value.StringValue, out _),
                $"Could not parse {Value.StringValue} to a valid UnityEngine.FullScreenMode value.");

            return (UnityEngine.FullScreenMode)System.Enum.Parse(typeof(UnityEngine.FullScreenMode), Value.StringValue);
        }
    }
}