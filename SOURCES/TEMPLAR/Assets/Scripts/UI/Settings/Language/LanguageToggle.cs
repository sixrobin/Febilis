namespace Templar.UI.Settings.Language
{
    using UnityEngine;
    
    public class LanguageToggle : UnityEngine.UI.Selectable, UnityEngine.EventSystems.ISelectHandler
    {
        [SerializeField] private TMPro.TextMeshProUGUI _title = null;
        [SerializeField] private RSLib.Framework.GUI.EnhancedToggle _toggle = null;

        public event System.Action<LanguageToggle, bool> ValueChanged;
        
        public string LanguageName { get; private set; }

        public UnityEngine.UI.Toggle Toggle => _toggle;
        
        public override void OnSelect(UnityEngine.EventSystems.BaseEventData eventData)
        {
            base.OnSelect(eventData);
            Navigation.UINavigationManager.Select(Toggle.gameObject);
            RSLib.Audio.UI.UIAudioManager.PlayHoverClip();
        }
        
        public override void OnPointerEnter(UnityEngine.EventSystems.PointerEventData eventData)
        {
            base.OnPointerEnter(eventData);
            RSLib.Audio.UI.UIAudioManager.PlayHoverClip();
        }
        
        public void Init(string languageName)
        {
            LanguageName = languageName;
            _title.text = LanguageName;
            Toggle.isOn = LanguageName == Localizer.Instance.Language;
            
            Toggle.onValueChanged.AddListener(OnValueChanged);
        }
        
        protected virtual void OnValueChanged(bool value)
        {
            ValueChanged?.Invoke(this, value);
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            Toggle.onValueChanged.RemoveListener(OnValueChanged);
        }

#if UNITY_EDITOR
        protected override void Reset()
        {
            base.Reset();
            _title = _title ?? GetComponent<TMPro.TextMeshProUGUI>() ?? GetComponentInChildren<TMPro.TextMeshProUGUI>();
        }
#endif
    }
}