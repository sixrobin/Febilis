namespace Templar.UI.Options
{
    using UnityEngine;

    /// <summary>
    /// Base class that every option panel class should derive from.
    /// Manages the basic canvas display state, contains the main navigation buttons and main navigation methods. 
    /// </summary>
    [DisallowMultipleComponent]
    public abstract class OptionPanelBase : MonoBehaviour, Navigation.IUIPanel
    {
        [Header("PANEL BASE")]
        [SerializeField] private Canvas _canvas = null;
        [SerializeField] private UnityEngine.UI.Button _backBtn = null;
        [SerializeField] private UnityEngine.UI.Button _quitBtn = null;

        public abstract GameObject FirstSelected { get; }

        public bool Displayed { get; private set; }

        public Canvas Canvas => _canvas;
        public UnityEngine.UI.Button BackBtn => _backBtn;
        public UnityEngine.UI.Button QuitBtn => _quitBtn;

        public virtual void Display(bool show)
        {
            Displayed = show;
            _canvas.enabled = Displayed;
        }

        public virtual void Open()
        {
            Display(true);
        }

        public virtual void Close()
        {
            Display(false);
            Manager.OptionsManager.Instance.Close();
        }

        public virtual void OnBackButtonPressed()
        {
            Display(false);
        }

        protected virtual void Start()
        {
            QuitBtn.onClick.AddListener(Close);
            BackBtn?.onClick.AddListener(OnBackButtonPressed);
        }
    }
}