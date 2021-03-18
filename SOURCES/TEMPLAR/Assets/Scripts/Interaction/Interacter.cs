namespace Templar.Interaction
{
    using UnityEngine;

    [DisallowMultipleComponent]
    public class Interacter : MonoBehaviour
    {
        private System.Collections.Generic.Dictionary<Collider2D, Interactable> _knownInteractables = new System.Collections.Generic.Dictionary<Collider2D, Interactable>();
        private Interactable _currInteractable;

        public void TryInteract()
        {
            _currInteractable?.Interact();
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

            if (!_knownInteractables.TryGetValue(collision, out Interactable interactable))
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
                collision.GetComponent<Interactable>() != null && !_knownInteractables.ContainsKey(collision),
                "Exiting a trigger that has an Interactable component that was not recorded in the known interactables.");

            if (_knownInteractables.TryGetValue(collision, out Interactable interactable))
            {
                interactable.Unfocus();
                if (interactable == _currInteractable)
                    _currInteractable = null;
            }
        }
    }
}