namespace Templar.Interaction
{
    using System.Linq;
    using UnityEngine;

    public class Lock : Dialogue.Speaker, Flags.IIdentifiable
    {
        [Header("IDENTIFIER")]
        [SerializeField] private Flags.LockIdentifier _lockIdentifier = null;

        [Header("REFS")]
        [SerializeField] private Animator _animator = null;
        [SerializeField] private UnityEngine.Events.UnityEvent _onOpen = null;

        private bool _locked = true;
        private bool _debugForceUnlock;

        public Flags.IIdentifier Identifier => _lockIdentifier;

        public override void Interact()
        {
            if (_locked && !_debugForceUnlock)
            {
                base.Interact();
                return;
            }

            Manager.FlagsManager.Register(this);
            Unlock(false);
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

        private void Unlock(bool instantly)
        {
            _onOpen?.Invoke();
            _highlight.SetActive(false);

            if (instantly)
                OnUnlockAnimationOver();
            else
                _animator.SetTrigger("Unlock");
        }

        private void Start()
        {
            if (Manager.FlagsManager.Check(this))
                Unlock(true);

            RSLib.Debug.Console.DebugConsole.OverrideCommand(new RSLib.Debug.Console.Command("LocksForceUnlock", "Forces every locks to open on interaction.", () =>
            {
                FindObjectsOfType<Lock>().ToList().ForEach(o => o._debugForceUnlock = true);
            }));
        }
    }
}