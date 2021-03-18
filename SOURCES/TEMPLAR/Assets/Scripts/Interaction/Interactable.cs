namespace Templar.Interaction
{
    using UnityEngine;

    [DisallowMultipleComponent]
    public abstract class Interactable : MonoBehaviour
    {
        public class InteractionEventArgs : System.EventArgs
        {
            public InteractionEventArgs(Interactable source)
            {
                Source = source;
            }

            public Interactable Source { get; private set; }
        }

        [SerializeField] private UnityEngine.Events.UnityEvent _onFocused = null;
        [SerializeField] private UnityEngine.Events.UnityEvent _onUnfocused = null;
        [SerializeField] private UnityEngine.Events.UnityEvent _onInteracted = null;

        public delegate void InteractionEventHandler(InteractionEventArgs args);

        public event InteractionEventHandler Focused;
        public event InteractionEventHandler Unfocused;
        public event InteractionEventHandler Interacted;

        public virtual void Focus()
        {
            CProLogger.Log(this, $"Focusing {transform.name}.", gameObject);
            Focused?.Invoke(new InteractionEventArgs(this));
            _onFocused?.Invoke();
        }

        public virtual void Unfocus()
        {
            CProLogger.Log(this, $"Unfocusing {transform.name}.", gameObject);
            Unfocused?.Invoke(new InteractionEventArgs(this));
            _onUnfocused?.Invoke();
        }

        public virtual void Interact()
        {
            CProLogger.Log(this, $"Interacting with {transform.name}.", gameObject);
            Interacted?.Invoke(new InteractionEventArgs(this));
            _onInteracted?.Invoke();
        }
    }
}