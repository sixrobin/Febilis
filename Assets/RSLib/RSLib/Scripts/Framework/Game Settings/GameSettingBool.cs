namespace RSLib.Framework.GameSettings
{
    using Extensions;

    public abstract class GameSettingBool : GameSetting
    {
        public GameSettingBool() : base() { }
        public GameSettingBool(System.Xml.Linq.XElement saveElement) : base(saveElement) { }

        public delegate void ValueChangedEventHandler(bool value);
        public event ValueChangedEventHandler ValueChanged;
        
        private bool _value;
        public virtual bool Value
        {
            get => _value;
            set
            {
                _value = value;
                ValueChanged?.Invoke(_value);
            }
        }
        
        public virtual bool Default => true;
        
        protected override void Init()
        {
            Value = Default;
        }

        public override void Load(System.Xml.Linq.XElement saveElement)
        {
            Value = saveElement.ValueToBool();
        }

        public override System.Xml.Linq.XElement Save()
        {
            return new System.Xml.Linq.XElement(SerializationName, Value);
        }
    }
}