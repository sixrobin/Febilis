namespace RSLib.Jumble.FPSController
{
    using UnityEngine;
    using UnityEngine.Events;

    /// <summary>
    /// Abstract class for every gameObject in the game that the FPS controller can interact with.
    /// </summary>
    public abstract class FPSInteraction : MonoBehaviour
    {
        [SerializeField] private bool _unfocusedOnInteracted = false;
        [SerializeField] private bool _uniqueInteraction = false;
        [SerializeField] private UnityEvent _onInteracted = null;

        public delegate void InteractedEventHandler();
        public event InteractedEventHandler Interacted;

        public bool InteractionAllowed { get; set; } = true;
        public bool UnfocusedOnInteracted => _unfocusedOnInteracted;

        /// <summary>
        /// Called when the player is looking to the gameObject and is ready to interact with it.
        /// </summary>
        public virtual void Focus()
        {
        }

        /// <summary>
        /// Called with the player leaves the interactable gameObject.
        /// </summary>
        public virtual void Unfocus()
        {
        }

        /// <summary>
        /// Called when the player interacts with the gameObject.
        /// Calls related events after some more checks.
        /// </summary>
        public virtual void Interact()
        {
            if (!InteractionAllowed)
                return;

            if (_uniqueInteraction)
                InteractionAllowed = false;

            if (_unfocusedOnInteracted)
                Unfocus();

            Interacted?.Invoke();
            _onInteracted.Invoke();
        }

        protected virtual void Awake()
        {
            if (!GetComponent<Collider>())
                Debug.LogWarning($"{nameof(FPSInteraction)}: gameObject {transform.name} doesn't have a collider and can't be interacted.", gameObject);
        }
    }
}