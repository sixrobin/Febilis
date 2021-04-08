namespace Templar.Camera
{
    using RSLib.Extensions;
    using UnityEngine;

    public class CameraController : MonoBehaviour
    {
        [SerializeField] private Datas.CameraDatas _cameraDatas = null;
        [SerializeField] private Camera _camera = null;
        [SerializeField] private Unit.Player.PlayerController _playerCtrl = null;
        [SerializeField] private CameraShake.Settings _shakeSettings = CameraShake.Settings.Default;
        [SerializeField] private RSLib.ImageEffects.CameraGrayscaleRamp _grayscaleRamp = null;

        [Header("PIXEL PERFECT FIX")]
        [SerializeField] private bool _toggleManualFix = true;
        [SerializeField] private Vector2Int _referenceResolution = new Vector2Int(160, 144);
        [SerializeField, Min(1)] private int _assetsPixelsPerUnit = 100;

        [Header("DEBUG")]
        [SerializeField] private bool _debugOnSelectedOnly = true;
        [SerializeField] private RSLib.DataColor _debugColor = null;

        private RSLib.FocusArea _focusArea;

        private float _currLookAhead;
        private float _targetLookAhead;
        private float _lookAheadDir;
        private bool _isLookingAhead;

        private float _currLookVerticalDir;
        private float _nonNullLookVerticalTimer;
        private float _currLookVertical;
        private float _targetLookVertical;

        private float _refX;
        private float _refY;
        private float _refLookAheadVertical;

        public CameraShake Shake { get; private set; }

        public RSLib.ImageEffects.CameraGrayscaleRamp GrayscaleRamp => _grayscaleRamp;

        private Vector3 ComputeBaseTargetPosition()
        {
            return _focusArea.Center + Vector2.up * _cameraDatas.HeightOffset;
        }

        private void ComputeLookAheadPosition(ref Vector3 pos)
        {
            if (Mathf.Abs(_focusArea.Velocity.x) < Mathf.Epsilon)
            {
                if (_isLookingAhead)
                {
                    _isLookingAhead = false;
                    _targetLookAhead = _currLookAhead + (_lookAheadDir * _cameraDatas.HorizontalLookAheadDist - _currLookAhead) * 0.25f;
                }
            }
            else
            {
                _lookAheadDir = Mathf.Sign(_focusArea.Velocity.x);
                if (_playerCtrl.InputCtrl.CurrentHorizontalDir == _lookAheadDir
                    && _playerCtrl.InputCtrl.Horizontal != 0f
                    || _playerCtrl.RollCtrl.IsRolling)
                {
                    _isLookingAhead = true;
                    _targetLookAhead = _lookAheadDir * _cameraDatas.HorizontalLookAheadDist;
                }
                else if (_isLookingAhead)
                {
                    _isLookingAhead = false;
                    _targetLookAhead = _currLookAhead + (_lookAheadDir * _cameraDatas.HorizontalLookAheadDist - _currLookAhead) * 0.25f;
                }
            }

            _currLookAhead = Mathf.SmoothDamp(_currLookAhead, _targetLookAhead, ref _refX, _cameraDatas.HorizontalLookAheadDamping);
            pos += Vector3.right * _currLookAhead;
        }

        private void ComputeLookVerticalPosition(ref Vector3 pos)
        {
            if (!_playerCtrl.Initialized)
                return;

            _currLookVerticalDir = _playerCtrl.InputCtrl.Vertical;

            if (_currLookVerticalDir != 0f && _playerCtrl.InputCtrl.Horizontal == 0f)
            {
                _nonNullLookVerticalTimer += Time.deltaTime;
                if (_nonNullLookVerticalTimer > _cameraDatas.VerticalLookAheadDelay)
                    _targetLookVertical = _playerCtrl.InputCtrl.CurrentVerticalDir * _cameraDatas.VerticalLookAheadDist;
            }
            else
            {
                _targetLookVertical = 0f;
                _nonNullLookVerticalTimer = 0f;
            }

            _currLookVertical = Mathf.SmoothDamp(_currLookVertical, _targetLookVertical, ref _refLookAheadVertical, _cameraDatas.VerticalLookAheadDamping);
            pos.y += _currLookVertical;
        }

        private void ComputeDampedPosition(ref Vector3 pos)
        {
            // This condition seems to avoid SmoothDamp method to sometimes divide by 0, throwing an error.
            if (Mathf.Abs(pos.y - transform.position.y) > Mathf.Epsilon)
                pos.y = Mathf.SmoothDamp(transform.position.y, pos.y, ref _refY, _cameraDatas.VerticalDamping);
        }

        private void ComputeShakePosition(ref Vector3 pos)
        {
            pos += Shake.GetShake();
        }

        private void UpdatePixelPerfectCameraSize()
        {
            // Test method to fix pixel perfect jitter.

            if (!_toggleManualFix)
                return;

            int w = _camera.targetTexture?.width ?? Screen.width;
            int h = _camera.targetTexture?.height ?? Screen.height;

            int verticalZoom = h / _referenceResolution.y;
            int horizontalZoom = w / _referenceResolution.x;
            int zoom = Mathf.Max(1, Mathf.Min(verticalZoom, horizontalZoom));

            _camera.orthographicSize = h * 0.5f / (zoom * _assetsPixelsPerUnit);
        }

        private void Awake()
        {
            _focusArea = new RSLib.FocusArea(_playerCtrl.BoxCollider2D, _cameraDatas.FocusAreaSize);
            Shake = new CameraShake(_shakeSettings);

            // Instantly position camera on first frame.
            transform.position = ComputeBaseTargetPosition().WithZ(transform.position.z);
        }

        private void Update()
        {
            // [TMP] Should be in an option panel.
            if (Input.GetKeyDown(KeyCode.F3))
                GetComponent<UnityEngine.U2D.PixelPerfectCamera>().enabled = !GetComponent<UnityEngine.U2D.PixelPerfectCamera>().enabled;
        }

        private void LateUpdate()
        {
#if UNITY_EDITOR
            if (_focusArea.Size != _cameraDatas.FocusAreaSize)
                _focusArea = new RSLib.FocusArea(_playerCtrl.BoxCollider2D, _cameraDatas.FocusAreaSize);
#endif
            _focusArea.Update();

            Vector3 targetPosition = ComputeBaseTargetPosition();

            ComputeLookAheadPosition(ref targetPosition);
            ComputeLookVerticalPosition(ref targetPosition);
            ComputeShakePosition(ref targetPosition);
            ComputeDampedPosition(ref targetPosition);

            transform.position = targetPosition.WithZ(transform.position.z);

            //UpdatePixelPerfectCameraSize();
        }

        private void OnDrawGizmos()
        {
            if (_debugOnSelectedOnly)
                return;

            _focusArea?.DrawArea(_debugColor.Color);
        }

        private void OnDrawGizmosSelected()
        {
            if (!_debugOnSelectedOnly)
                return;

            _focusArea?.DrawArea(_debugColor.Color);
        }

        private void OnValidate()
        {
            Shake?.SetSettings(_shakeSettings);
        }
    }
}