namespace Templar.UI.Settings.Language
{
    using System.Linq;
    using RSLib.Extensions;
    using UnityEngine;

    public class LanguageSettingsPanel : SettingsPanelBase
    {
        [Header("LANGUAGE SETTINGS")]
        [SerializeField] private TMPro.TextMeshProUGUI _title = null;
        [SerializeField] private LanguageToggle _languageTogglePrefab = null;
        [SerializeField] private RectTransform _languageTogglesContainer = null;
        [SerializeField] private RSLib.Framework.GUI.EnhancedButton _saveSettingsBtn = null;
        [SerializeField] private UnityEngine.UI.ToggleGroup _toggleGroup = null;
        
        private bool _initialized;

        private LanguageToggle[] _languageToggles;

        public override GameObject FirstSelected => _languageToggles[0].gameObject;
        
        public override void OnBackButtonPressed()
        {
            base.OnBackButtonPressed();
            Manager.OptionsManager.Instance.OpenSettings(this);
        }
        
        public override void Display(bool show)
        {
            base.Display(show);

            if (show)
            {
                Init();
                Localize();
            }
        }
        
        private void Init()
        {
            if (_initialized)
                return;

            _languageToggles = new LanguageToggle[Localizer.Instance.Languages.Length];
            
            for (int i = 0; i < _languageToggles.Length; ++i)
            {
                LanguageToggle languageToggle = Instantiate(_languageTogglePrefab, _languageTogglesContainer);
                languageToggle.Init(Localizer.Instance.Languages[i]);
                _languageToggles[i] = languageToggle;

                languageToggle.ValueChanged += OnLanguageToggleValueChanged;
                languageToggle.Toggle.group = _toggleGroup;
                _toggleGroup.RegisterToggle(languageToggle.Toggle);
            }

            InitNavigation();

            _initialized = true;
        }
        
        private void InitNavigation()
        {
            for (int i = 0; i < _languageToggles.Length; ++i)
            {
                bool first = i == 0;
                bool last = i == _languageToggles.Length - 1;

                _languageToggles[i].Toggle.SetMode(UnityEngine.UI.Navigation.Mode.Explicit);

                if (first)
                {
                    _languageToggles[i].Toggle.SetSelectOnUp(BackBtn);
                    BackBtn.SetSelectOnDown(_languageToggles[i].Toggle);
                    QuitBtn.SetSelectOnDown(_languageToggles[i].Toggle);
                }
                else
                {
                    _languageToggles[i].Toggle.SetSelectOnUp(_languageToggles[i - 1].Toggle);
                }

                if (!last)
                {
                    _languageToggles[i].Toggle.SetSelectOnDown(_languageToggles[i + 1].Toggle);
                    continue;
                }

                break;
            }

            _saveSettingsBtn.SetMode(UnityEngine.UI.Navigation.Mode.Explicit);
            QuitBtn.SetMode(UnityEngine.UI.Navigation.Mode.Explicit);
            BackBtn.SetMode(UnityEngine.UI.Navigation.Mode.Explicit);
            
            _saveSettingsBtn.SetSelectOnUp(_languageToggles[_languageToggles.Length - 1].Toggle);
            _languageToggles[_languageToggles.Length - 1].Toggle.SetSelectOnDown(_saveSettingsBtn);
            
            _saveSettingsBtn.SetSelectOnDown(BackBtn);
            BackBtn.SetSelectOnUp(_saveSettingsBtn);
            QuitBtn.SetSelectOnUp(_saveSettingsBtn);
        }

        private void OnLanguageToggleValueChanged(LanguageToggle languageToggle, bool value)
        {
            Manager.SettingsManager.Language.Value = Manager.SettingsManager.Language.Options.First(o => o.StringValue == languageToggle.LanguageName);
            Localize();
        }
        
        private void Localize()
        {
            _title.text = Localizer.Get(Localization.Settings.LANGUAGE);
            _saveSettingsBtn.SetText(Localizer.Get(Localization.Settings.LANGUAGE_SAVE));
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();

            if (_languageToggles != null)
                for (int i = 0; i < _languageToggles.Length; ++i)
                    _languageToggles[i].ValueChanged -= OnLanguageToggleValueChanged;
        }
    }
}
