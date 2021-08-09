namespace Templar.Interaction
{
    using UnityEngine;

    public class Lock : Dialogue.Speaker
    {
        [SerializeField] private UnityEngine.Events.UnityEvent _onOpen = null;

        private bool _locked = true;

        public void SetLockedState(bool state)
        {
            _locked = state;
        }

        public override void Interact()
        {
            if (_locked)
            {
                base.Interact();
                return;
            }

            _onOpen?.Invoke();
            gameObject.SetActive(false);
        }
    }
}