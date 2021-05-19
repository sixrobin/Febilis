﻿namespace Templar.UI.Options
{
    using RSLib.Extensions;
    using System.Linq;
    using UnityEngine;

    public class SettingsPanel : OptionPanelBase
    {
        [Header("OPTIONS BUTTONS")]
        [SerializeField] private UnityEngine.UI.Button _controlsBtn = null;
        [SerializeField] private UnityEngine.UI.Button _videoBtn = null;
        [SerializeField] private UnityEngine.UI.Button _audioBtn = null;
        [SerializeField] private UnityEngine.UI.Button _languageBtn = null;

        private UnityEngine.UI.Button[] _settingsBtns;

        public override GameObject FirstSelected => _controlsBtn.gameObject;

        public UnityEngine.UI.Button ControlsBtn => _controlsBtn;
        public UnityEngine.UI.Button VideoBtn => _videoBtn;
        public UnityEngine.UI.Button AudioBtn => _audioBtn;
        public UnityEngine.UI.Button LanguageBtn => _languageBtn;

        public override void OnBackButtonPressed()
        {
            Close();
        }

        private void InitSettingsButtonsNavigation()
        {
            _settingsBtns = new UnityEngine.UI.Button[]
            {
                _controlsBtn,
                _videoBtn,
                _audioBtn,
                _languageBtn
            }.OrderBy(o => o.transform.GetSiblingIndex()).ToArray();

            for (int i = 0; i < _settingsBtns.Length; ++i)
            {
                _settingsBtns[i].SetMode(UnityEngine.UI.Navigation.Mode.Explicit);
                _settingsBtns[i].SetSelectOnUp(i == 0 ? QuitBtn : _settingsBtns[i - 1]);
                _settingsBtns[i].SetSelectOnDown(i == _settingsBtns.Length - 1 ? QuitBtn : _settingsBtns[i + 1]);

                _settingsBtns[i].SetSelectOnRight(QuitBtn);
            }

            QuitBtn.SetSelectOnLeft(_settingsBtns[0]);
            QuitBtn.SetSelectOnDown(_settingsBtns[0]);
            QuitBtn.SetSelectOnUp(_settingsBtns[_settingsBtns.Length - 1]);
        }

        protected override void Start()
        {
            base.Start();
            InitSettingsButtonsNavigation();
        }
    }
}