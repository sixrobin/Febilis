namespace Templar.UI.Settings
{
    using RSLib.Extensions;
    using System.Linq;
    using UnityEngine;

    public class SettingsHubPanel : SettingsPanelBase
    {
        [Header("OPTIONS BUTTONS")]
        [SerializeField] private RSLib.Framework.GUI.EnhancedButton _controlsBtn = null;
        [SerializeField] private RSLib.Framework.GUI.EnhancedButton _gameBtn = null;
        [SerializeField] private RSLib.Framework.GUI.EnhancedButton _audioBtn = null;
        [SerializeField] private RSLib.Framework.GUI.EnhancedButton _languageBtn = null;
        [SerializeField] private RSLib.Framework.GUI.EnhancedButton _quitGameBtn = null;

        private UnityEngine.UI.Button[] _settingsBtns;

        public override GameObject FirstSelected => ControlsBtn.gameObject;

        public RSLib.Framework.GUI.EnhancedButton ControlsBtn => _controlsBtn;
        public RSLib.Framework.GUI.EnhancedButton GameBtn => _gameBtn;
        public RSLib.Framework.GUI.EnhancedButton AudioBtn => _audioBtn;
        public RSLib.Framework.GUI.EnhancedButton LanguageBtn => _languageBtn;
        public RSLib.Framework.GUI.EnhancedButton QuitGameBtn => _quitGameBtn;

        public override void Open()
        {
            base.Open();
            Localize();
        }

        public override void OnBackButtonPressed()
        {
            Close();
        }

        public override void Close()
        {
            base.Close();
            RSLib.Audio.UI.UIAudioManager.PlayGenericNavigationClip();
        }

        private void InitSettingsButtonsNavigation()
        {
            _settingsBtns = new UnityEngine.UI.Button[]
            {
                ControlsBtn,
                GameBtn,
                AudioBtn,
                LanguageBtn,
                QuitGameBtn
            }
            .Where(o => o.gameObject.activeSelf)
            .ToArray();

            for (int i = 0; i < _settingsBtns.Length; ++i)
            {
                _settingsBtns[i].SetMode(UnityEngine.UI.Navigation.Mode.Explicit);
                _settingsBtns[i].SetSelectOnUp(i == 0 ? QuitBtn : _settingsBtns[i - 1]);
                _settingsBtns[i].SetSelectOnDown(i == _settingsBtns.Length - 1 ? QuitBtn : _settingsBtns[i + 1]);

                _settingsBtns[i].SetSelectOnRight(QuitBtn);
            }

            QuitBtn.SetMode(UnityEngine.UI.Navigation.Mode.Explicit);
            QuitBtn.SetSelectOnLeft(_settingsBtns[0]);
            QuitBtn.SetSelectOnDown(_settingsBtns[0]);
            QuitBtn.SetSelectOnUp(_settingsBtns[_settingsBtns.Length - 1]);
        }

        private void Localize()
        {
            ControlsBtn.SetText(Localizer.Get(Localization.Settings.CONTROLS));
            GameBtn.SetText(Localizer.Get(Localization.Settings.GAME));
            AudioBtn.SetText(Localizer.Get(Localization.Settings.AUDIO));
            LanguageBtn.SetText(Localizer.Get(Localization.Settings.LANGUAGE));
            QuitGameBtn.SetText(Localizer.Get(Localization.Game.QUIT));
        }
        
        protected override void Start()
        {
            base.Start();
            InitSettingsButtonsNavigation();
        }
    }
}