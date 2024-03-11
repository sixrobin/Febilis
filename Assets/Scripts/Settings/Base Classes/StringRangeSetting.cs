namespace Templar.Settings
{
    using System.Linq;
    using System.Xml.Linq;

    public abstract class StringRangeSetting : Setting
    {
        public struct StringOption
        {
            public StringOption(string stringValue, bool isDefaultOne, string customDisplay = null)
            {
                StringValue = stringValue;
                CustomDisplay = customDisplay;
                IsDefaultOne = isDefaultOne;
            }

            public string StringValue;
            public string CustomDisplay;
            public bool IsDefaultOne;

            public bool HasCustomDisplay => !string.IsNullOrEmpty(CustomDisplay);
        }

        public StringRangeSetting() : base()
        {
        }

        public StringRangeSetting(XElement element) : base(element)
        {
        }

        public delegate void ValueChangedEventHandler(StringOption previousValue, StringOption currentValue);
        public event ValueChangedEventHandler ValueChanged;

        private StringOption _value;
        public virtual StringOption Value
        {
            get => _value;
            set
            {
                StringOption previousValue = _value;
                _value = value;
                ValueChanged?.Invoke(previousValue, _value);
            }
        }

        public abstract StringOption[] Options { get; }

        public override void Load(XElement element)
        {
            System.Collections.Generic.IEnumerable<StringOption> fittingOptions = Options.Where(o => o.StringValue == element.Value);
            if (fittingOptions.Any())
            {
                Value = fittingOptions.First();
                return;
            }

            UnityEngine.Debug.LogWarning($"No option with string value {element.Value} has been found, using default option.");
            Init();
        }

        public override XElement Save()
        {
            return new XElement(SaveElementName, Value.StringValue);
        }

        public override void Init()
        {
            UnityEngine.Assertions.Assert.IsFalse(!Options.Any(o => o.IsDefaultOne), "No default option has been set.");
            UnityEngine.Assertions.Assert.IsFalse(Options.Count(o => o.IsDefaultOne) > 1, "More than one default option has been set.");

            Value = Options.FirstOrDefault(o => o.IsDefaultOne);
        }

        public StringOption GetNextOption()
        {
            for (int i = 0; i < Options.Length; ++i)
                if (Options[i].StringValue == Value.StringValue)
                    return Options[RSLib.Helpers.Mod(i + 1, Options.Length)];

            UnityEngine.Debug.LogError($"Current value {Value} was not found in options to get next one.");
            return default;
        }

        public StringOption GetPreviousOption()
        {
            for (int i = 0; i < Options.Length; ++i)
                if (Options[i].StringValue == Value.StringValue)
                    return Options[RSLib.Helpers.Mod(i - 1, Options.Length)];

            UnityEngine.Debug.LogError($"Current value {Value} was not found in options to get previous one.");
            return default;
        }
    }
}