namespace Templar.UI.Settings.Audio
{
    using System.Linq;
    using RSLib.Extensions;
    using UnityEngine;
#if UNITY_EDITOR
    using UnityEditor;
#endif
    
    public class AudioSettingsPanel : SettingsPanelBase
    {
        [Header("AUDIO SETTINGS")]
        [SerializeField] private VolumeSlider[] _sliders = null;
        [Space(10f)]
        [SerializeField] private UnityEngine.UI.Button _resetSettingsBtn = null;
        [SerializeField] private UnityEngine.UI.Button _saveSettingsBtn = null;

        private bool _initialized;

        public override GameObject FirstSelected => _sliders.FirstOrDefault(o => o.gameObject.activeSelf)?.gameObject;

        public override void OnBackButtonPressed()
        {
            base.OnBackButtonPressed();
            Manager.OptionsManager.Instance.OpenSettings(this);
        }
        
        public override void Display(bool show)
        {
            base.Display(show);

            if (show)
                Init();
        }

        private void Init()
        {
            VolumeSlider[] enabledSliders = _sliders.Where(o => o.gameObject.activeSelf).ToArray();

            if (!_initialized)
            {
                InitNavigation();
                for (int i = 0; i < enabledSliders.Length; ++i)
                    enabledSliders[i].Init();
            }
            else
            {
                for (int i = 0; i < enabledSliders.Length; ++i)
                    enabledSliders[i].InitValueFromAudioManager();
            }
            
            _initialized = true;
        }
        
        private void InitNavigation()
        {
            VolumeSlider[] enabledSliders = _sliders.Where(o => o.gameObject.activeSelf).ToArray();

            for (int i = 0; i < enabledSliders.Length; ++i)
            {
                bool first = i == 0;
                bool last = i == enabledSliders.Length - 1;

                enabledSliders[i].Selectable.SetMode(UnityEngine.UI.Navigation.Mode.Explicit);

                if (first)
                {
                    enabledSliders[i].Selectable.SetSelectOnUp(BackBtn);
                    enabledSliders[i].Selectable.SetSelectOnUp(QuitBtn);

                    BackBtn.SetSelectOnDown(enabledSliders[i].Selectable);
                    QuitBtn.SetSelectOnDown(enabledSliders[i].Selectable);
                }
                else
                {
                    enabledSliders[i].Selectable.SetSelectOnUp(enabledSliders[i - 1].Selectable);
                }

                if (!last)
                {
                    enabledSliders[i].Selectable.SetSelectOnDown(enabledSliders[i + 1].Selectable);
                    continue;
                }

                _resetSettingsBtn.SetMode(UnityEngine.UI.Navigation.Mode.Explicit);
                _saveSettingsBtn.SetMode(UnityEngine.UI.Navigation.Mode.Explicit);

                _resetSettingsBtn.SetSelectOnUp(enabledSliders[i].Selectable);
                _saveSettingsBtn.SetSelectOnUp(enabledSliders[i].Selectable);

                enabledSliders[i].Selectable.SetSelectOnDown(_saveSettingsBtn);

                break;
            }
            
            _resetSettingsBtn.SetSelectOnRight(_saveSettingsBtn);
            _saveSettingsBtn.SetSelectOnLeft(_resetSettingsBtn);
            
            _resetSettingsBtn.SetSelectOnDown(_backBtn);
            _backBtn.SetSelectOnUp(_resetSettingsBtn);
            _saveSettingsBtn.SetSelectOnDown(_quitBtn);
            _quitBtn.SetSelectOnUp(_saveSettingsBtn);
        }
        
        private void ResetSettings()
        {
            for (int i = _sliders.Length - 1; i >= 0; --i)
                _sliders[i].ResetValue();
        }
        
        protected override void Start()
        {
            base.Start();

            _resetSettingsBtn.onClick.AddListener(ResetSettings);
            _saveSettingsBtn.onClick.AddListener(Manager.SettingsManager.Save); // TODO: Save Volume only?
            _saveSettingsBtn.onClick.AddListener(OnBackButtonPressed);
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();

            _resetSettingsBtn.onClick.RemoveListener(ResetSettings);
            _saveSettingsBtn.onClick.RemoveListener(Manager.SettingsManager.Save); // TODO: Save Volume only?
            _saveSettingsBtn.onClick.RemoveListener(OnBackButtonPressed);
        }
        
        public void LocateVolumeSliders()
        {
            _sliders = transform.parent.GetComponentsInChildren<VolumeSlider>();
            RSLib.EditorUtilities.PrefabEditorUtilities.SetCurrentPrefabStageDirty();
            RSLib.EditorUtilities.SceneManagerUtilities.SetCurrentSceneDirty();
        }
        
#if UNITY_EDITOR
        [CustomEditor(typeof(AudioSettingsPanel))]
        public class GameSettingsPanelEditor : RSLib.EditorUtilities.ButtonProviderEditor<AudioSettingsPanel>
        {
            protected override void DrawButtons()
            {
                DrawButton("Locate Settings", Obj.LocateVolumeSliders);
            }
        }
#endif
    }
}