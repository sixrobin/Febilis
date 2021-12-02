namespace Templar.UI.Settings.Game
{
    using UnityEngine;

    public abstract class ButtonTextSettingView : SettingView
    {
        [SerializeField] private RSLib.Framework.GUI.EnhancedButton _btn = null;

        public override Templar.Settings.Setting Setting => StringRangeSetting;

        public override UnityEngine.UI.Selectable Selectable => _btn;

        public abstract Templar.Settings.StringRangeSetting StringRangeSetting { get; }

        public override void Init()
        {
            _btn.onClick.RemoveAllListeners();
            _btn.onClick.AddListener(OnButtonClicked);
            RefreshText();
        }

        protected virtual void OnButtonClicked()
        {
            StringRangeSetting.Value = StringRangeSetting.GetNextOption();
            RefreshText();
        }

        private void RefreshText()
        {
            _btn.SetText(StringRangeSetting.Value.HasCustomDisplay ? StringRangeSetting.Value.CustomDisplay : StringRangeSetting.Value.StringValue);
        }

#if UNITY_EDITOR
        protected override void Reset()
        {
            base.Reset();
            _btn = _btn ?? GetComponentInChildren<RSLib.Framework.GUI.EnhancedButton>();
        }
#endif
    }
}