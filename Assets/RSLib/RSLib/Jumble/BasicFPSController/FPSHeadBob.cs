namespace RSLib.Jumble.FPSController
{
    using UnityEngine;

    /// <summary>
    /// Extra movement to apply to the FPS camera to imitate a head bobbing motion.
    /// </summary>
    public class FPSHeadBob : FPSCameraExtraMovement
    {
        [Header("REFERENCES")]
        [SerializeField] private FPSController _fpsController = null;

        [Header("BOBBING SETTINGS")]
        [SerializeField] private float _idleAmplitude = 0.3f;
        [SerializeField] private float _idleSpeed = 0.35f;
        [SerializeField] private float _walkingAmplitude = 0.3f;
        [SerializeField] private float _walkingSpeed = 0.3f;
        [SerializeField] private float _crouchedAmplitude = 0.25f;
        [SerializeField] private float _crouchedSpeed = 0.2f;
        [SerializeField] private float _sprintingAmplitude = 0.55f;
        [SerializeField] private float _sprintingSpeed = 0.7f;
        [SerializeField] private float _bobDamping = 0.3f;
        [SerializeField, Range(0f, 1f)] private float _crouchWalkPercentage = 0.5f;

        private float _sineTimer;
        private float _currentAmplitude;
        private float _currentSpeed;
        private float _refAmplitude;
        private float _refSpeed;

        public override void ApplyMovement()
        {
            Bob();
        }

        /// <summary>
        /// Only calculates bobbing values without applying them.
        /// </summary>
        private void EvaluateBobbingValues()
        {
            float targetAmplitude;
            float targetSpeed;
            bool moving = _fpsController.CheckMovementMagnitude();

            if (_fpsController.Crouched)
            {
                targetAmplitude = moving ? Mathf.Lerp(_crouchedAmplitude, _walkingAmplitude, _crouchWalkPercentage) : _crouchedAmplitude;
                targetSpeed = moving ? Mathf.Lerp(_crouchedSpeed, _walkingSpeed, _crouchWalkPercentage) : _crouchedSpeed;
            }
            else if (_fpsController.Sprinting)
            {
                targetAmplitude = _sprintingAmplitude;
                targetSpeed = _sprintingSpeed;
            }
            else
            {
                targetAmplitude = moving ? _walkingAmplitude : _idleAmplitude;
                targetSpeed = moving ? _walkingSpeed : _idleSpeed;
            }

            _currentAmplitude = Mathf.SmoothDamp(_currentAmplitude, targetAmplitude, ref _refAmplitude, _bobDamping);
            _currentSpeed = Mathf.SmoothDamp(_currentSpeed, targetSpeed, ref _refSpeed, _bobDamping);
        }

        /// <summary>
        /// Moves this object transform up and down to imitate a head bobbing motion, using a basic sine wave.
        /// </summary>
        private void Bob()
        {
            EvaluateBobbingValues();
            _sineTimer += Time.deltaTime * _currentSpeed;
            transform.position += new Vector3(0f, Mathf.Sin(_sineTimer) * _currentAmplitude);
        }
    }
}