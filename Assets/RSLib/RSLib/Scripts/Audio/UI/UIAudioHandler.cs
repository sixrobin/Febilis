namespace RSLib.Audio.UI
{
    using UnityEngine;
    using UnityEngine.UI;

    [DisallowMultipleComponent]
    public class UIAudioHandler : MonoBehaviour
    {
        private Button _button;
        private Toggle _toggle;
        private Dropdown _dropdown; 
        private Slider _slider; 
        private RSLib.Framework.GUI.EnhancedButton _enhancedButton;
        private RSLib.Framework.GUI.EnhancedToggle _enhancedToggle;
        private RSLib.Framework.GUI.EnhancedSlider _enhancedSlider;
        // TODO: EnhancedDropdown.
        
        protected virtual void OnButtonClick()
        {
            UIAudioManager.PlayButtonClickClip();
        }

        protected virtual void OnToggleValueChanged(bool value)
        {
            UIAudioManager.PlayToggleValueChangedClip();
        }
        
        protected virtual void OnDropdownValueChanged(int value)
        {
            UIAudioManager.PlayDropdownValueChangedClip();
        }
        
        protected virtual void OnSliderValueChanged(float value)
        {
            UIAudioManager.PlaySliderValueChangedClip();
        }
        
        protected virtual void OnButtonPointerEnter(RSLib.Framework.GUI.EnhancedButton source)
        {
            UIAudioManager.PlayHoverClip();
        }
        
        protected virtual void OnTogglePointerEnter(RSLib.Framework.GUI.EnhancedToggle source)
        {
            UIAudioManager.PlayHoverClip();
        }
        
        protected virtual void OnSliderPointerEnter(RSLib.Framework.GUI.EnhancedSlider source)
        {
            UIAudioManager.PlayHoverClip();
        }

        private void Awake()
        {
            _button = GetComponent<Button>();
            if (_button != null)
                _button.onClick.AddListener(OnButtonClick);
            
            _enhancedButton = GetComponent<RSLib.Framework.GUI.EnhancedButton>();
            if (_enhancedButton != null)
                _enhancedButton.PointerEnter += OnButtonPointerEnter;

            _toggle = GetComponent<Toggle>();
            if (_toggle != null)
                _toggle.onValueChanged.AddListener(OnToggleValueChanged);

            _enhancedToggle = GetComponent<RSLib.Framework.GUI.EnhancedToggle>();
            if (_enhancedToggle != null)
                _enhancedToggle.PointerEnter += OnTogglePointerEnter;

            _dropdown = GetComponent<Dropdown>();
            if (_dropdown != null)
                _dropdown.onValueChanged.AddListener(OnDropdownValueChanged);

            // TODO: Dropdown OnPointerEnter listener.
            
            _slider = GetComponent<Slider>();
            if (_slider != null)
                _slider.onValueChanged.AddListener(OnSliderValueChanged);
            
            _enhancedSlider = GetComponent<RSLib.Framework.GUI.EnhancedSlider>();
            if (_enhancedSlider != null)
                _enhancedSlider.PointerEnter += OnSliderPointerEnter;
        }

        private void OnDestroy()
        {
            if (_button != null)
                _button.onClick.RemoveListener(OnButtonClick);
            
            if (_enhancedButton != null)
                _enhancedButton.PointerEnter -= OnButtonPointerEnter;
            
            if (_toggle != null)
                _toggle.onValueChanged.RemoveListener(OnToggleValueChanged);

            if (_enhancedToggle != null)
                _enhancedToggle.PointerEnter -= OnTogglePointerEnter;

            if (_dropdown != null)
                _dropdown.onValueChanged.RemoveListener(OnDropdownValueChanged);
            
            // TODO: Dropdown OnPointerEnter listener.
            
            if (_slider != null)
                _slider.onValueChanged.RemoveListener(OnSliderValueChanged);
            
            if (_enhancedSlider != null)
                _enhancedSlider.PointerEnter -= OnSliderPointerEnter;
        }
    }
}