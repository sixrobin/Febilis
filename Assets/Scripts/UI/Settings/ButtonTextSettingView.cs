namespace Templar.UI.Settings.Game
{
    using UnityEngine;

    public abstract class ButtonTextSettingView : SettingView
    {
        [SerializeField] protected RSLib.Framework.GUI.EnhancedButton _btn = null;

        public override Templar.Settings.Setting Setting => StringRangeSetting;

        public override UnityEngine.UI.Selectable Selectable => _btn;

        public abstract Templar.Settings.StringRangeSetting StringRangeSetting { get; }

        public override void InitSelectable()
        {
            _btn.onClick.RemoveAllListeners();
            _btn.onClick.AddListener(OnButtonClicked);
            RefreshText();
        }

        public override void Localize()
        {
            base.Localize();
            RefreshText();
        }

        protected virtual void OnButtonClicked()
        {
            StringRangeSetting.Value = StringRangeSetting.GetNextOption();
            RefreshText();
            
            RSLib.Audio.UI.UIAudioManager.PlayButtonClickClip();
        }

        private void RefreshText()
        {
            string text;

            if (StringRangeSetting.Value.HasCustomDisplay)
                text = StringRangeSetting.Value.CustomDisplay;
            else if (RSLib.Localization.Localizer.TryGet($"{Localization.Settings.SCREEN_MODE_PREFIX}{StringRangeSetting.Value.StringValue}", out string localizedText))
                text = localizedText;
            else
                text = StringRangeSetting.Value.StringValue;
            
            _btn.SetText(text);
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