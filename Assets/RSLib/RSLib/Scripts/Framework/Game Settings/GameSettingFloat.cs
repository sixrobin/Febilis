namespace RSLib.Framework.GameSettings
{
    using Extensions;

    public abstract class GameSettingFloat : GameSetting
    {
        public GameSettingFloat() : base() { }
        public GameSettingFloat(System.Xml.Linq.XElement saveElement) : base(saveElement) { }

        public delegate void ValueChangedEventHandler(float previous, float current);
        public event ValueChangedEventHandler ValueChanged;
        
        private float _value;
        public virtual float Value
        {
            get => _value;
            set
            {
                float previousValue = _value;
                _value = UnityEngine.Mathf.Clamp(value, Range.Min, Range.Max);
                ValueChanged?.Invoke(previousValue, _value);
            }
        }
        
        public abstract (float Min, float Max) Range { get; }
        public virtual float Default => Range.Max;
        
        protected override void Init()
        {
            Value = Default;
        }

        public override void Load(System.Xml.Linq.XElement saveElement)
        {
            Value = saveElement.ValueToFloat();
        }

        public override System.Xml.Linq.XElement Save()
        {
            return new System.Xml.Linq.XElement(SerializationName, Value);
        }
    }
}