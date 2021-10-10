namespace Templar.UI.Settings
{
    using UnityEngine;

    public abstract class SliderSettingView : SettingView
    {
        [SerializeField] private UnityEngine.UI.Slider _slider = null;

        protected bool _initializing;

        public override UnityEngine.UI.Selectable Selectable => _slider;

        public abstract Templar.Settings.FloatSetting FloatSetting { get; }

        public override void Init()
        {
            _initializing = true;

            _slider.onValueChanged.AddListener(OnValueChanged);

            _slider.minValue = FloatSetting.Range.Min;
            _slider.maxValue = FloatSetting.Range.Max;
            _slider.value = FloatSetting.Value;

            _initializing = false;
        }

        protected virtual void OnValueChanged(float value)
        {
            // Setting min and max value may change the slider value, triggering unwanted OnValueChanged at the moment.
            // To avoid that, we keep track of the initialization state using a simple flag.
            if (_initializing)
                return;

            FloatSetting.Value = value;
        }

#if UNITY_EDITOR
        protected override void Reset()
        {
            base.Reset();
            _slider = _slider ?? GetComponentInChildren<UnityEngine.UI.Slider>();
        }
#endif
    }
}