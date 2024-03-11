namespace RSLib.Jumble.FPSController
{
    using UnityEngine;

    /// <summary>
    /// Main class for the FPS interaction controller. Raycasts to the camera forward to handle interactions.
    /// Can be enabled/disabled if needed.
    /// </summary>
    public class FPSInteracter : FPSControllableComponent
    {
        [Header("REFERENCES")]
        [SerializeField] private Transform _cameraTransform = null;
        [SerializeField] private GameObject _scopeVisual = null;

        [Header("SETTINGS")]
        [SerializeField] private float _maxDistance = 1.5f;
        [SerializeField] private LayerMask _interactionMask = 0;

        [Header("DEBUG")]
        [SerializeField] private bool _showDebug = true;

        private FPSInteraction _lastInteracted;

        private FPSInteraction _currentInteraction;
        private FPSInteraction CurrentInteraction
        {
            get => _currentInteraction;
            set
            {
                _currentInteraction = value;
                _currentInteraction?.Focus();
            }
        }

        protected override void OnControlAllowed()
        {
            base.OnControlAllowed();
            _scopeVisual.SetActive(true);
        }

        protected override void OnControlDisallowed()
        {
            base.OnControlDisallowed();
            ResetCurrentInteractions();
            _scopeVisual.SetActive(false);
        }

        /// <summary>
        /// Resets all interactions variables.
        /// </summary>
        private void ResetCurrentInteractions()
        {
            CurrentInteraction?.Unfocus();
            CurrentInteraction = null;
            _lastInteracted = null;
        }

        /// <summary>
        /// Checks if an interactable gameObject is in front of the camera, and sets it as the current interaction
        /// after some more checks to make sure the interaction is allowed.
        /// </summary>
        private void CheckForInteraction()
        {
            if (Physics.Raycast(_cameraTransform.position, _cameraTransform.forward, out RaycastHit hit, _maxDistance, _interactionMask))
            {
                if (_showDebug)
                    Debug.DrawLine(_cameraTransform.position, hit.point, Color.red);

                if (CurrentInteraction == null
                    && hit.collider.TryGetComponent(out FPSInteraction interactable)
                    && interactable.InteractionAllowed
                    && interactable != _lastInteracted)
                        CurrentInteraction = interactable;
            }
            else
            {
                if (_showDebug)
                    Debug.DrawLine(_cameraTransform.position, _cameraTransform.position + _cameraTransform.forward * _maxDistance, Color.red);

                ResetCurrentInteractions();
            }
        }

        /// <summary>
        /// Triggers the current interaction if it exists and the players inputs the interaction button.
        /// </summary>
        private void TryInteract()
        {
            if (CurrentInteraction == null)
                return;

            if (Input.GetButtonDown("Interact"))
            {
                CurrentInteraction.Interact();
                if (CurrentInteraction && CurrentInteraction.UnfocusedOnInteracted)
                {
                    _lastInteracted = CurrentInteraction;
                    CurrentInteraction = null;
                }
            }
        }

        private void Update()
        {
            if (!Controllable)
                return;

            CheckForInteraction();
            TryInteract();
        }
    }
}