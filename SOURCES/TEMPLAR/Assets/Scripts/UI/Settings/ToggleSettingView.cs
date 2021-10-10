namespace Templar.UI.Settings.Game
{
    using UnityEngine;

    public abstract class ToggleSettingView : SettingView
    {
        [SerializeField] private RSLib.Framework.GUI.EnhancedToggle _toggle = null;

        public override UnityEngine.UI.Selectable Selectable => _toggle;

        public abstract Templar.Settings.BoolSetting BoolSetting { get; }

        public override void Init()
        {
            _toggle.onValueChanged.AddListener(OnValueChanged);
            _toggle.isOn = BoolSetting.Value;
        }

        protected virtual void OnValueChanged(bool value)
        {
            BoolSetting.Value = value;
        }

        protected override void Reset()
        {
            base.Reset();
            _toggle = _toggle ?? GetComponentInChildren<RSLib.Framework.GUI.EnhancedToggle>();
        }
    }
}