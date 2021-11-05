namespace Templar.UI.Settings.Game
{
    using UnityEngine;

    public abstract class IntButtonSettingView : SettingView
    {
        [SerializeField] private RSLib.Framework.GUI.EnhancedButton _btn = null;
        [SerializeField] private int _displayIncrement = 0;

        public override Templar.Settings.Setting Setting => IntSetting;

        public override UnityEngine.UI.Selectable Selectable => _btn;

        public abstract Templar.Settings.IntSetting IntSetting { get; }

        public override void Init()
        {
            _btn.onClick.RemoveAllListeners();
            _btn.onClick.AddListener(OnButtonClicked);
            RefreshText();
        }

        protected virtual void OnButtonClicked()
        {
            if (IntSetting.Value == IntSetting.Range.Max)
                IntSetting.Value = IntSetting.Range.Min;
            else
                IntSetting.Value++;

            RefreshText();
        }

        private void RefreshText()
        {
            _btn.SetText((IntSetting.Value + _displayIncrement).ToString());
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