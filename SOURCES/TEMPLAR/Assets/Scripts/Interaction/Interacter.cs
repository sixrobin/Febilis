namespace Templar.Interaction
{
    using UnityEngine;

    [DisallowMultipleComponent]
    public class Interacter : MonoBehaviour
    {
        private System.Collections.Generic.Dictionary<Collider2D, IInteractable> _knownInteractables = new System.Collections.Generic.Dictionary<Collider2D, IInteractable>();
        private IInteractable _currInteractable;

        public delegate void InteractedEventHandler(IInteractable interactable);
        public InteractedEventHandler Interacted;
        
        public void TryInteract()
        {
            if (_currInteractable == null)
                return;
            
            Interacted?.Invoke(_currInteractable);
            _currInteractable.Interact();
        }

        // Collision layering should be managed by the project physics settings.
        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (_currInteractable != null)
            {
                // Focus only one at a time.
                _currInteractable.Unfocus();
                _currInteractable = null;
            }

            if (!_knownInteractables.TryGetValue(collision, out IInteractable interactable))
                if (collision.TryGetComponent(out interactable))
                    _knownInteractables.Add(collision, interactable);

            if (interactable == null)
                return;

            _currInteractable = interactable;
            _currInteractable.Focus();
        }

        private void OnTriggerExit2D(Collider2D collision)
        {
            UnityEngine.Assertions.Assert.IsFalse(
                collision.GetComponent<IInteractable>() != null && !_knownInteractables.ContainsKey(collision),
                "Exiting a trigger that has an Interactable component that was not recorded in the known interactables.");

            if (_knownInteractables.TryGetValue(collision, out IInteractable interactable))
            {
                interactable.Unfocus();
                if (interactable == _currInteractable)
                    _currInteractable = null;
            }
        }
    }
}