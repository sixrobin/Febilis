namespace RSLib.Framework.GameSettings
{
    using Extensions;

    public abstract class GameSettingEnum<T> : GameSetting where T : System.Enum
    {
        public GameSettingEnum() : base() { }
        public GameSettingEnum(System.Xml.Linq.XElement saveElement) : base(saveElement) { }

        public delegate void ValueChangedEventHandler(T previous, T current);
        public event ValueChangedEventHandler ValueChanged;
        
        private T _value;
        public virtual T Value
        {
            get => _value;
            set
            {
                T previousValue = _value;
                _value = value;
                ValueChanged?.Invoke(previousValue, _value);
            }
        }
        
        public abstract T Default { get; }
        
        protected override void Init()
        {
            Value = Default;
        }

        public override void Load(System.Xml.Linq.XElement saveElement)
        {
            Value = saveElement.ValueToEnum<T>();
        }

        public override System.Xml.Linq.XElement Save()
        {
            return new System.Xml.Linq.XElement(SerializationName, Value);
        }
    }
}