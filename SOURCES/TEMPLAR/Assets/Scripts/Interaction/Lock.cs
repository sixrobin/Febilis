namespace Templar.Interaction
{
    using System.Linq;
    using UnityEngine;

    public class Lock : Dialogue.Speaker
    {
        [SerializeField] private UnityEngine.Events.UnityEvent _onOpen = null;
        [SerializeField] private Animator _animator = null;

        private bool _locked = true;
        private bool _debugForceUnlock;

        public override void Interact()
        {
            if (_locked && !_debugForceUnlock)
            {
                base.Interact();
                return;
            }

            _onOpen?.Invoke();

            _highlight.SetActive(false);
            _animator.SetTrigger("Unlock");
        }

        public void OnUnlockAnimationOver()
        {
            gameObject.SetActive(false);
        }

        // Referenced in UnityEvent.
        public void SetLockedState(bool state)
        {
            _locked = state;
        }

        private void Awake()
        {
            RSLib.Debug.Console.DebugConsole.OverrideCommand(new RSLib.Debug.Console.Command("LocksForceUnlock", "Forces every locks to open on interaction.", () =>
            {
                FindObjectsOfType<Lock>().ToList().ForEach(o => o._debugForceUnlock = true);
            }));
        }
    }
}