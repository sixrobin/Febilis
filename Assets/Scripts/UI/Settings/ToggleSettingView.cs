﻿namespace Templar.UI.Settings.Game
{
    using UnityEngine;

    public abstract class ToggleSettingView : SettingView
    {
        [SerializeField] private RSLib.Framework.GUI.EnhancedToggle _toggle = null;

        public override Templar.Settings.Setting Setting => BoolSetting;

        public override UnityEngine.UI.Selectable Selectable => _toggle;

        public abstract Templar.Settings.BoolSetting BoolSetting { get; }

        public override void InitSelectable()
        {
            _toggle.onValueChanged.AddListener(OnValueChanged);
            _toggle.isOn = BoolSetting.Value;
        }

        protected virtual void OnValueChanged(bool value)
        {
            BoolSetting.Value = value;
        }

#if UNITY_EDITOR
        protected override void Reset()
        {
            base.Reset();
            _toggle = _toggle ?? GetComponentInChildren<RSLib.Framework.GUI.EnhancedToggle>();
        }
#endif
    }
}