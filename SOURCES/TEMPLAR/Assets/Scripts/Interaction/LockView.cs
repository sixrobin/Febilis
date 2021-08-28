namespace Templar.Interaction
{
    using UnityEngine;

    public class LockView : MonoBehaviour
    {
        [SerializeField] private Lock _lock = null;

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