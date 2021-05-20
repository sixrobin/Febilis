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

        public abstract GameObject FirstSelected { get; }

        public bool Displayed { get; private set; }

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
    }
}