namespace Templar.UI.Settings.Audio
{
    using UnityEngine;

    public class VolumeSlider : UnityEngine.UI.Selectable, UnityEngine.EventSystems.ISelectHandler
    {
        [Header("REFS")]
        [SerializeField] private TMPro.TextMeshProUGUI _sliderText = null;
        [SerializeField] private RSLib.Framework.GUI.PointerEventsHandler _pointerEventsHandler = null;
        [SerializeField] private UnityEngine.UI.Slider _slider = null;

        [Header("VOLUME PARAMETER")]
        [SerializeField] private string _mixerParameterName = null;

        public RSLib.Framework.GUI.PointerEventsHandler PointerEventsHandler => _pointerEventsHandler;

        public UnityEngine.UI.Selectable Selectable => _slider;
        
        public override void OnSelect(UnityEngine.EventSystems.BaseEventData eventData)
        {
            base.OnSelect(eventData);
            PointerEventsHandler.OnPointerEnter(null);

            Navigation.UINavigationManager.Select(_slider.gameObject);
            
            RSLib.Audio.UI.UIAudioManager.PlayHoverClip();
        }
        
        public override void OnPointerEnter(UnityEngine.EventSystems.PointerEventData eventData)
        {
            base.OnPointerEnter(eventData);
            PointerEventsHandler.OnPointerEnter(eventData);
            
            RSLib.Audio.UI.UIAudioManager.PlayHoverClip();
        }
        
        public void Init()
        {
            UnityEngine.Assertions.Assert.IsFalse(string.IsNullOrEmpty(_mixerParameterName), $"No mixerParameterName is set on {transform.name}!");

            InitValueFromAudioManager();
            _slider.onValueChanged.AddListener(OnValueChanged);
        }

        public void InitValueFromAudioManager()
        {
            if (RSLib.Audio.AudioManager.TryGetMixerFloatParameterValue(_mixerParameterName, out float volume))
                SetValue(RSLib.Audio.AudioManager.DecibelsToLinear(volume));
        }

        public void SetValue(float value)
        {
            _slider.value = value;
        }
        
        public void ResetValue()
        {
            SetValue(1f);
        }

        public void Localize()
        {
            _sliderText.text = Localizer.Get($"{Localization.Settings.VOLUME_PREFIX}{_mixerParameterName}");
        }
        
        private void OnValueChanged(float value)
        {
            RSLib.Audio.AudioManager.SetMixerVolumePercentage(_mixerParameterName, value);
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            _slider.onValueChanged.RemoveListener(OnValueChanged);
        }
    }
}