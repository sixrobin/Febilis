namespace Templar.Interaction
{
    using UnityEngine;

    [DisallowMultipleComponent]
    public class Interacter : MonoBehaviour
    {
        private System.Collections.Generic.Dictionary<Collider2D, IInteractable> _knownInteractables = new System.Collections.Generic.Dictionary<Collider2D, IInteractable>();

        public delegate void InteractedEventHandler(IInteractable interactable);
        public InteractedEventHandler Interacted;

        public IInteractable CurrentInteractable { get; private set; }

        public void TryInteract()
        {
            if (CurrentInteractable == null)
                return;
            
            Interacted?.Invoke(CurrentInteractable);
            CurrentInteractable.Interact();
        }

        // Collision layering should be managed by the project physics settings.
        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (CurrentInteractable != null)
            {
                // Focus only one at a time.
                CurrentInteractable.Unfocus();
                CurrentInteractable = null;
            }

            if (!_knownInteractables.TryGetValue(collision, out IInteractable interactable))
                if (collision.TryGetComponent(out interactable))
                    _knownInteractables.Add(collision, interactable);

            if (interactable == null)
                return;

            CurrentInteractable = interactable;
            CurrentInteractable.Focus();
        }

        private void OnTriggerExit2D(Collider2D collision)
        {
            UnityEngine.Assertions.Assert.IsFalse(
                collision.GetComponent<IInteractable>() != null && !_knownInteractables.ContainsKey(collision),
                "Exiting a trigger that has an Interactable component that was not recorded in the known interactables.");

            if (_knownInteractables.TryGetValue(collision, out IInteractable interactable))
            {
                interactable.Unfocus();
                if (interactable == CurrentInteractable)
                    CurrentInteractable = null;
            }
        }
    }
}