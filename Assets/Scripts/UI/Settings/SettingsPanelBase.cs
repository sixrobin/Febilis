namespace Templar.UI.Settings
{
    using UnityEngine;

    /// <summary>
    /// Base class that every option panel class should derive from.
    /// Manages the basic canvas display state, contains the main navigation buttons and main navigation methods. 
    /// </summary>
    [DisallowMultipleComponent]
    public abstract class SettingsPanelBase : UIPanel
    {
        [SerializeField] protected UnityEngine.UI.Button _backBtn = null;
        [SerializeField] protected UnityEngine.UI.Button _quitBtn = null;

        public UnityEngine.UI.Button BackBtn => _backBtn;
        public UnityEngine.UI.Button QuitBtn => _quitBtn;

        public override void Open()
        {
            base.Open();
            RSLib.Audio.UI.UIAudioManager.PlayGenericNavigationClip();
        }

        public override void Close()
        {
            base.Close();
            Manager.OptionsManager.Instance.Close();
        }

        protected virtual void Start()
        {
            QuitBtn.onClick.AddListener(Close);
            BackBtn?.onClick.AddListener(OnBackButtonPressed);
        }
    }
}