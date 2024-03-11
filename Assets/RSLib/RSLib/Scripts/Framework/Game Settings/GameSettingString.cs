namespace RSLib.Framework.GameSettings
{
    using System.Linq;

    public abstract class GameSettingString : GameSetting
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

        public GameSettingString() : base() { }
        public GameSettingString(System.Xml.Linq.XElement saveElement) : base(saveElement) { }

        public delegate void ValueChangedEventHandler(StringOption previous, StringOption current);
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

        protected override void Init()
        {
            UnityEngine.Assertions.Assert.IsFalse(!Options.Any(o => o.IsDefaultOne), "No default option has been set.");
            UnityEngine.Assertions.Assert.IsFalse(Options.Count(o => o.IsDefaultOne) > 1, "More than one default option has been set.");

            Value = Options.FirstOrDefault(o => o.IsDefaultOne);
        }
        
        public override void Load(System.Xml.Linq.XElement saveElement)
        {
            if (saveElement == null)
                return;
            
            StringOption[] fittingOptions = Options.Where(o => o.StringValue == saveElement.Value).ToArray();
            if (fittingOptions.Length > 0)
            {
                Value = fittingOptions[0];
            }
            else
            {
                UnityEngine.Debug.LogWarning($"No option with string value {saveElement.Value} has been found, using default option.");
                Init();
            }
        }

        public override System.Xml.Linq.XElement Save()
        {
            return new System.Xml.Linq.XElement(SerializationName, Value.StringValue);
        }

        public StringOption GetNextOption()
        {
            for (int i = 0; i < Options.Length; ++i)
                if (Options[i].StringValue == Value.StringValue)
                    return Options[(i + 1) % Options.Length];

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