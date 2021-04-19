namespace Templar.Interaction
{
    using UnityEngine;

    public class CheckpointView : MonoBehaviour
    {
        private const string INTERACTED = "Interacted";
        private const string ON = "On";
        private const string OFF = "Off";

        [SerializeField] private Animator _animator = null;
        [SerializeField] private GameObject[] _enabledOnLightBurst = null;

        private LightBurstCallbackHandler _lightBurstCallback;

        public delegate void LightBurstCallbackHandler();

        // Animation event.
        public void OnLightBurst()
        {
            _lightBurstCallback?.Invoke();

            FindObjectOfType<Templar.Camera.CameraController>().GetShake("Small").AddTrauma(0.3f, 0.8f); // [TMP] Find and hardcoded values.
            for (int i = _enabledOnLightBurst.Length - 1; i >= 0; --i)
                _enabledOnLightBurst[i].SetActive(true);
        }

        public void PlayOnAnimation()
        {
            _animator.SetTrigger(ON);

            for (int i = _enabledOnLightBurst.Length - 1; i >= 0; --i)
                _enabledOnLightBurst[i].SetActive(true);
        }

        public void PlayInteractedAnimation(LightBurstCallbackHandler lightBurstCallback)
        {
            _lightBurstCallback = lightBurstCallback;
            _animator.SetTrigger(INTERACTED);
        }
        
        public void PlayOffAnimation()
        {
            _animator.SetTrigger(OFF);

            for (int i = _enabledOnLightBurst.Length - 1; i >= 0; --i)
                _enabledOnLightBurst[i].SetActive(false);
        }
    }
}