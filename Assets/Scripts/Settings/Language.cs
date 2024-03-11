namespace Templar.Settings
{
    using System.Xml.Linq;
    
    public class Language : StringRangeSetting
    {
        public const string SAVE_ELEMENT_NAME = "Language";
        
        private StringOption[] _options;
        
        public Language() : base()
        {
        }

        public Language(XElement element) : base(element)
        {
        }
        
        public override string SaveElementName => SAVE_ELEMENT_NAME;
        
        public override StringOption Value
        {
            get => base.Value;
            set
            {
                base.Value = value;
                RSLib.Localization.Localizer.SetCurrentLanguage(base.Value.StringValue);
            }
        }

        public override StringOption[] Options
        {
            get
            {
                if (_options == null)
                {
                    _options = new StringOption[RSLib.Localization.Localizer.Instance.Languages.Length];
                    for (int i = 0; i < RSLib.Localization.Localizer.Instance.Languages.Length; ++i)
                        _options[i] = new StringOption(RSLib.Localization.Localizer.Instance.Languages[i], i == 0);
                }

                return _options;
            }
        }
    }
}