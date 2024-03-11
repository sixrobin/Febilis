namespace RSLib.Jumble.FPSController
{
    using UnityEngine;
    using RSLib.Extensions;

    /// <summary>
    /// Main class for the FPS movement. Determines the speed, the sprint settings, the ability to crouch.
    /// Can be enabled/disabled if needed.
    /// </summary>
    [RequireComponent(typeof(Rigidbody), typeof(CapsuleCollider))]
    public class FPSController : FPSControllableComponent
    {
        [Header("CONTROLLER OPTIONS")]
        [SerializeField] private bool _canSprint = true;
        [SerializeField] private bool _canCrouch = true;

        [Header("REFERENCES")]
        [SerializeField] private Transform _cameraTransform = null;

        [Header("MOVEMENT")]
        [SerializeField] private float _baseSpeed = 3.2f;
        [SerializeField] private float _crouchedSpeed = 1.5f;
        [SerializeField] private float _sprintSpeed = 7.8f;
        [SerializeField] private float _speedDampingTime = 0.2f;
        [SerializeField] private float _capsuleCrouchedHeight = 0.8f;
        [SerializeField] private float _inputDamping = 0.1f;
        [SerializeField] private float _uncrouchCheckAccuracy = 0.3f;

        [Header("STAMINA")]
        [SerializeField] private float _fullSprintDuration = 5f;
        [SerializeField] private float _recoverDuration = 2f;
        [SerializeField] private float _fullReloadDuration = 15f;

        [Header("MISC")]
        [SerializeField] private LayerMask _groundMask = 0;

        [Header("DEBUG")]
        [SerializeField] private bool _showDebug = true;

        private Rigidbody _rigidbody;
        private CapsuleCollider _capsule;

        private float _baseCapsuleHeight;
        private float _baseCapsuleYCenter;

        private Vector3 _rawMovementInput;
        private Vector3 _currentMovementInput;
        private Vector3 _refMovementInput;

        public FPSStaminaManager StaminaManager { get; private set; }

        private bool _sprinting;
        public bool Sprinting
        {
            get => _sprinting;
            private set => _sprinting = value && CheckMovementMagnitude();
        }

        private bool _crouched;
        public bool Crouched
        {
            get => _crouched;
            private set
            {
                if (value && !_crouched)
                {
                    _capsule.center = _capsule.center.WithY(_baseCapsuleYCenter / (_baseCapsuleHeight / _capsuleCrouchedHeight));
                    _capsule.height = _capsuleCrouchedHeight;
                    _crouched = true;
                }
                else if (!value && _crouched)
                {
                    if (!CheckUncrouchAbility())
                        return;

                    _capsule.center = _capsule.center.WithY(_baseCapsuleYCenter);
                    _capsule.height = _baseCapsuleHeight;
                    _crouched = false;
                }
            }
        }

        private Vector3 _currentVelocity;
        private float _currentSpeed;
        private float _refSpeed;

        protected override void OnControlDisallowed()
        {
            base.OnControlDisallowed();
            _rawMovementInput = Vector3.zero;
            _currentMovementInput = Vector3.zero;
            Sprinting = false;
        }

        /// <summary>
        /// Checks if the controller's capsule is on the ground.
        /// </summary>
        /// <returns>True if grounded, else false.</returns>
        public bool CheckGround()
        {
            return Physics.Raycast(transform.position + Vector3.up * 0.05f, Vector3.down, 0.1f, _groundMask);
        }

        /// <summary>
        /// Checks if the controller can uncrouch, by raycasting upward to check for a possible roof.
        /// </summary>
        /// <returns>True if uncrouch is allowed, else false.</returns>
        public bool CheckUncrouchAbility()
        {
            for (int i = 0; i < 4; i++)
            {
                float radAngle = 90f * i * Mathf.Deg2Rad;
                Vector3 raycastStart = transform.position + new Vector3(Mathf.Cos(radAngle), 0, Mathf.Sin(radAngle)) * _uncrouchCheckAccuracy;
                if (Physics.Raycast(raycastStart, Vector3.up, _baseCapsuleHeight + 0.1f, _groundMask))
                    return false;
            }

            return true;
        }
       
        /// <summary>
        /// Checks if the controller is moving.
        /// </summary>
        /// <returns>True if the controller is moving, else false.</returns>
        public bool CheckMovementMagnitude()
        {
            return _rawMovementInput.sqrMagnitude > 0f;
        }

        /// <summary>
        /// Stores the controller inputs to use them later.
        /// </summary>
        private void GetInputs()
        {
            _rawMovementInput.x = Input.GetAxisRaw("Horizontal");
            _rawMovementInput.z = Input.GetAxisRaw("Vertical");
            _currentMovementInput = Vector3.SmoothDamp(_currentMovementInput, _rawMovementInput.normalized, ref _refMovementInput, _inputDamping);

            Sprinting = _canSprint && !Crouched && !StaminaManager.IsEmpty && Input.GetButton("Sprint");
            Crouched = _canCrouch && Input.GetButton("Crouch");
        }

        /// <summary>
        /// Calculates the target speed to use depending on the controller state.
        /// </summary>
        private void EvaluateSpeed()
        {
            float targetSpeed = Sprinting ? _sprintSpeed : (_crouched ? _crouchedSpeed : _baseSpeed);
            _currentSpeed = Mathf.SmoothDamp(_currentSpeed, targetSpeed, ref _refSpeed, _speedDampingTime);
        }

        /// <summary>
        /// Applies the calculated values to the controller's attached rigidbody.
        /// </summary>
        private void MoveBody()
        {
            EvaluateSpeed();

            Quaternion cameraForward = Quaternion.Euler(0f, _cameraTransform.localEulerAngles.y, 0f);
            _currentVelocity = cameraForward * _currentMovementInput;
            _currentVelocity *= _currentSpeed;
            _currentVelocity.y = _rigidbody.velocity.y;

            _rigidbody.velocity = _currentVelocity;

            if (_showDebug)
                Debug.DrawLine(transform.position, transform.position + _rigidbody.velocity, Color.yellow);
        }

        private void Awake()
        {
            _rigidbody = GetComponent<Rigidbody>();
            _capsule = GetComponent<CapsuleCollider>();

            StaminaManager = new FPSStaminaManager(_fullSprintDuration, _recoverDuration, _fullReloadDuration);
            _baseCapsuleHeight = _capsule.height;
            _baseCapsuleYCenter = _capsule.center.y;
        }

        private void Update()
        {
            if (Crouched && _showDebug)
            {
                for (int i = 0; i < 4; ++i)
                {
                    float radAngle = 90f * i * Mathf.Deg2Rad;
                    Vector3 raycastStart = transform.position + new Vector3(Mathf.Cos(radAngle), 0, Mathf.Sin(radAngle)) * _uncrouchCheckAccuracy;
                    Debug.DrawLine(raycastStart, raycastStart + Vector3.up * (_baseCapsuleHeight + 0.1f), Color.cyan);
                }
            }

            if (_canSprint)
                StaminaManager.Update(Sprinting);

            if (Controllable)
                GetInputs();
        }

        private void FixedUpdate()
        {
            MoveBody();
        }
    }
}