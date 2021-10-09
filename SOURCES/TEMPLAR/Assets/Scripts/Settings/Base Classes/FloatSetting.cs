namespace Templar.Settings
{
    using RSLib.Extensions;
    using System.Xml.Linq;

    public abstract class FloatSetting : Setting
    {
        public FloatSetting() : base()
        {
        }

        public FloatSetting(XElement element) : base(element)
        {
        }

        public delegate void ValueChangedEventHandler(float previousValue, float currentValue);
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

        public override void Init()
        {
            Value = Range.Max;
        }

        public override void Load(XElement element)
        {
            Value = element.ValueToFloat();
        }

        public override XElement Save()
        {
            return new XElement(SaveElementName, Value);
        }
    }
}