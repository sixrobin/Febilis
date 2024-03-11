namespace RSLib.Framework.GameSettings
{
    using Extensions;

    public abstract class GameSettingInt : GameSetting
    {
        public GameSettingInt() : base() { }
        public GameSettingInt(System.Xml.Linq.XElement saveElement) : base(saveElement) { }

        public delegate void ValueChangedEventHandler(int previous, int current);
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
        public virtual int Default => Range.Max;
        
        protected override void Init()
        {
            Value = Default;
        }

        public override void Load(System.Xml.Linq.XElement saveElement)
        {
            Value = saveElement.ValueToInt();
        }

        public override System.Xml.Linq.XElement Save()
        {
            return new System.Xml.Linq.XElement(SerializationName, Value);
        }
    }
}