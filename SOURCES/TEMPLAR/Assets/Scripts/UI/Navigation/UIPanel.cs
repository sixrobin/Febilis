namespace Templar.UI
{
    using UnityEngine;

    /// <summary>
    /// Base class that every UI panel class should derive from.
    /// Manages the basic canvas display state and references the first gameObject to select when the panel is open.
    /// </summary>
    [DisallowMultipleComponent]
    public abstract class UIPanel : MonoBehaviour
    {
        [Header("PANEL BASE")]
        [SerializeField] private Canvas _canvas = null;

        public delegate void DisplayChangedEventHandler(bool displayed);
        public event DisplayChangedEventHandler DisplayChanged;

        public abstract GameObject FirstSelected { get; }

        private RectTransform _rectTransform;
        public RectTransform RectTransform
        {
            get
            {
                if (_rectTransform == null)
                    _rectTransform = GetComponent<RectTransform>();

                return _rectTransform;
            }

            protected set => _rectTransform = value;
        }

        private bool _displayed;
        public bool Displayed
        {
            get => _displayed;
            private set
            {
                _displayed = value;
                DisplayChanged?.Invoke(_displayed);
            }
        }

        public Canvas Canvas => _canvas;

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
        }

        public virtual void OnBackButtonPressed()
        {
            Display(false);
        }

        public virtual void OnBoardsTransitionTriggered()
        {
            Close();
        }

        protected virtual void Awake()
        {
            Manager.BoardsTransitionManager.Instance.BoardsTransitionTriggered += OnBoardsTransitionTriggered;
        }
    }
}