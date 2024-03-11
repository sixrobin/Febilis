namespace RSLib.Jumble.FPSController
{
    using UnityEngine;

    /// <summary>
    /// Main component of the FPS camera. Controls the camera position and rotation.
    /// Can be enabled/disabled if needed.
    /// </summary>
    public class FPSCamera : FPSControllableComponent
    {
        [Header("REFERENCES")]
        [SerializeField] private FPSController _fpsController = null;

        [Header("CAMERA SETTINGS")]
        [SerializeField] private float _pitchSpeed = 120f;
        [SerializeField] private float _yawSpeed = 145f;
        [SerializeField] private float _height = 1.8f;
        [SerializeField] private float _crouchedHeight = 0.7f;
        [SerializeField] private float _addForward = 0.15f;
        [SerializeField] private bool _yAxisReversed = false;
        [SerializeField, Range(0f, 90f)] private float _minPitch = 60f;
        [SerializeField, Range(0f, 90f)] private float _maxPitch = 60f;

        [Header("EXTRAS")]
        [SerializeField] private FPSCameraExtraMovement[] _extraMovements = null;

        private Transform _fpsControllerTransform;

        private Vector3 _rawCameraInput;
        private Vector3 _cameraDestination;
        private Vector3 _currentCameraPosition;
        private Vector3 _currentCameraEulerAngles;

        protected override void OnControlDisallowed()
        {
            base.OnControlDisallowed();
            _rawCameraInput.x = 0f;
            _rawCameraInput.y = 0f;
        }

        /// <summary>
        /// Stores the camera inputs to use them later.
        /// </summary>
        private void GetInputs()
        {
            _rawCameraInput.x = Input.GetAxisRaw("Mouse Y");
            _rawCameraInput.y = Input.GetAxisRaw("Mouse X");
        }

        /// <summary>
        /// Calculates the camera position and rotation, and clamps its rotation between -360 and 360.
        /// </summary>
        private void EvaluateCameraDestination()
        {
            _cameraDestination = _fpsControllerTransform.position;
            _cameraDestination += _fpsControllerTransform.up * (_fpsController.Crouched ? _crouchedHeight : _height);
            _cameraDestination += transform.forward * _addForward;
            _currentCameraPosition.x = _cameraDestination.x;
            _currentCameraPosition.z = _cameraDestination.z;
            _currentCameraPosition.y = Mathf.Lerp(_currentCameraPosition.y, _cameraDestination.y, Time.deltaTime * 10);

            _rawCameraInput.x *= _pitchSpeed;
            _rawCameraInput.y *= _yawSpeed;
            _rawCameraInput *= Time.deltaTime;

            _currentCameraEulerAngles.x += _rawCameraInput.x * (_yAxisReversed ? 1 : -1);
            _currentCameraEulerAngles.x = Mathf.Clamp(_currentCameraEulerAngles.x, -_minPitch, _maxPitch);
            _currentCameraEulerAngles.y += _rawCameraInput.y;

            while (_currentCameraEulerAngles.x > 360) _currentCameraEulerAngles.x -= 360;
            while (_currentCameraEulerAngles.y > 360) _currentCameraEulerAngles.y -= 360;
            while (_currentCameraEulerAngles.x < -360) _currentCameraEulerAngles.x += 360;
            while (_currentCameraEulerAngles.y < -360) _currentCameraEulerAngles.y += 360;
        }

        /// <summary>
        /// Moves camera to its calculated position and rotates it the right way.
        /// </summary>
        private void MoveCamera()
        {
            transform.position = _currentCameraPosition;
            transform.localEulerAngles = _currentCameraEulerAngles;
        }

        private void ApplyExtraMovements()
        {
            foreach (FPSCameraExtraMovement extra in _extraMovements)
                extra.ApplyMovement();
        }

        public void ReverseYAxis()
        {
            _yAxisReversed = !_yAxisReversed;
        }

        private void Awake()
        {
            _fpsControllerTransform = _fpsController.transform;
            _currentCameraEulerAngles = transform.localEulerAngles;
        }

        private void Update()
        {
            if (Controllable)
                GetInputs();
        }

        private void LateUpdate()
        {
            EvaluateCameraDestination();
            MoveCamera();
            ApplyExtraMovements();
        }
    }
}