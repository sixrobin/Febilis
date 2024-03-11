namespace Templar.Interaction
{
    using UnityEngine;

    public class LockView : MonoBehaviour
    {
        [Header("REFS")]
        [SerializeField] private Lock _lock = null;
        [SerializeField] private Animator _animator = null;
        
        [Header("AUDIO")]
        [SerializeField] private RSLib.Audio.ClipProvider _unlockClipProvider = null;

        public void PlayUnlockAnimation()
        {
            _animator.SetTrigger("Unlock");
            RSLib.Audio.AudioManager.PlaySound(_unlockClipProvider);
        }
        
        public void OnUnlockAnimationOver()
        {
            _lock.OnUnlockAnimationOver();
        }

        private void Reset()
        {
            _lock = _lock ?? GetComponent<Lock>() ?? GetComponentInParent<Lock>();
        }
    }
}