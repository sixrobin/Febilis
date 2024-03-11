namespace Templar.Settings
{
    using RSLib.Extensions;
    using System.Xml.Linq;

    public abstract class IntSetting : Setting
    {
        public IntSetting() : base()
        {
        }

        public IntSetting(XElement element) : base(element)
        {
        }

        public delegate void ValueChangedEventHandler(int previousValue, int currentValue);
        public event ValueChangedEventHandler ValueChanged;

        private int _value;
        public virtual int Value
        {
            get => _value;
            set
            {
                int previousValue = _value;
                _value = UnityEngine.Mathf.Clamp(value, Range.Min, Range.Max);
                ValueChanged?.Invoke(previousValue, _value);
            }
        }

        public abstract (int Min, int Max) Range { get; }

        public override void Init()
        {
            Value = Range.Max;
        }

        public override void Load(XElement element)
        {
            Value = element.ValueToInt();
        }

        public override XElement Save()
        {
            return new XElement(SaveElementName, Value);
        }
    }
}