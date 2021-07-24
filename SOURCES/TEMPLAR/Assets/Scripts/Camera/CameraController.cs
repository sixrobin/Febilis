namespace Templar.Camera
{
    using RSLib.Extensions;
    using UnityEngine;

    public class CameraController : MonoBehaviour
    {
        [System.Serializable]
        public class ShakeComponent
        {
            [SerializeField] private string _id = string.Empty;
            [SerializeField] private CameraShake.Settings _shakeSettings = CameraShake.Settings.Default;

            public string Id => _id;

            public CameraShake GetShakeFromSettings()
            {
                return new CameraShake(_shakeSettings);
            }
        }

        [SerializeField] private Datas.CameraDatas _cameraDatas = null;
        [SerializeField] private Camera _camera = null;
        [SerializeField] private Unit.Player.PlayerController _playerCtrl = null;
        [SerializeField] private RSLib.ImageEffects.CameraGrayscaleRamp _grayscaleRamp = null;

        [Header("SHAKES")]
        [SerializeField] private ShakeComponent[] _shakes = null;

        [Header("PIXEL PERFECT FIX")]
        [SerializeField] private bool _toggleManualFix = true;
        [SerializeField] private Vector2Int _referenceResolution = new Vector2Int(160, 144);
        [SerializeField, Min(1)] private int _assetsPixelsPerUnit = 100;

        [Header("DEBUG")]
        [SerializeField] private bool _debugOnSelectedOnly = true;
        [SerializeField] private RSLib.DataColor _debugColor = null;
#if UNITY_EDITOR
        [SerializeField] private Vector2 _currTraumaVisualizer = Vector2.zero;
#endif

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

        private System.Collections.Generic.Dictionary<string, CameraShake> _shakesDictionary;

        public RSLib.ImageEffects.CameraGrayscaleRamp GrayscaleRamp => _grayscaleRamp;

        public BoxCollider2D CurrBoardBounds { get; private set; }

        public bool Frozen { get; private set; }

        public void ApplyShakeFromDatas(Templar.Datas.ShakeTraumaDatas shakeDatas)
        {
            if (shakeDatas == null)
                return;

            GetShake(shakeDatas.ShakeId).AddTraumaFromDatas(shakeDatas);
        }

        public CameraShake GetShake(string id)
        {
            UnityEngine.Assertions.Assert.IsTrue(
                _shakesDictionary.ContainsKey(id),
                $"Shake with Id {id} does not exist. Existing shakes are {string.Join(",", _shakesDictionary.Keys)}.");

            return _shakesDictionary[id];
        }

        public void PositionInstantly()
        {
            transform.position = ComputeBaseTargetPosition().WithZ(transform.position.z);
        }

        public void ToggleFreeze(bool state)
        {
            Frozen = state;
        }

        public void SetBoardBounds(Boards.Board board)
        {
            CurrBoardBounds = board?.CameraBounds;
        }

        public void SetBounds(BoxCollider2D box)
        {
            CurrBoardBounds = box;
        }

        private void GenerateShakesDictionary()
        {
            _shakesDictionary = new System.Collections.Generic.Dictionary<string, CameraShake>();
            for (int i = _shakes.Length - 1; i >= 0; --i)
                _shakesDictionary.Add(_shakes[i].Id, _shakes[i].GetShakeFromSettings());
        }

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
                {
                    _targetLookVertical = _playerCtrl.InputCtrl.CurrentVerticalDir * _cameraDatas.VerticalLookAheadDist;

                    if (Mathf.Sign(_targetLookVertical) > 0f)
                        _playerCtrl.PlayerView.PlayLookUpAnimation();
                    else
                        _playerCtrl.PlayerView.PlayLookDownAnimation();
                }
            }
            else
            {
                _targetLookVertical = 0f;
                _nonNullLookVerticalTimer = 0f;
                _playerCtrl.PlayerView.StopLookUpOrDownAnimation();
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

        private void ComputeBoundedPosition(ref Vector3 pos)
        {
            if (CurrBoardBounds == null)
                return;

            float halfHeight = _camera.orthographicSize;
            float halfWidth = halfHeight * Screen.width / Screen.height;

            float xMin = CurrBoardBounds.bounds.min.x + halfWidth;
            float xMax = CurrBoardBounds.bounds.max.x - halfWidth;
            float yMin = CurrBoardBounds.bounds.min.y + halfHeight;
            float yMax = CurrBoardBounds.bounds.max.y - halfHeight;

            pos.x = Mathf.Clamp(pos.x, xMin, xMax);
            pos.y = Mathf.Clamp(pos.y, yMin, yMax);
        }

        private void ComputeShakePosition(ref Vector3 pos)
        {
            foreach (System.Collections.Generic.KeyValuePair<string, CameraShake> shake in _shakesDictionary)
                pos += shake.Value.GetShake();
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
            GenerateShakesDictionary();
            PositionInstantly();

            RSLib.Debug.Console.DebugConsole.OverrideCommand(new RSLib.Debug.Console.Command(
                "PixelPerfectToggle", "Toggles pixel perfect camera.", () => GetComponent<UnityEngine.U2D.PixelPerfectCamera>().enabled = !GetComponent<UnityEngine.U2D.PixelPerfectCamera>().enabled));
        }

        private void LateUpdate()
        {
#if UNITY_EDITOR
            if (_focusArea.Size != _cameraDatas.FocusAreaSize)
                _focusArea = new RSLib.FocusArea(_playerCtrl.BoxCollider2D, _cameraDatas.FocusAreaSize);

            // [TODO] Visualize all shakes traumas.
            //_currTraumaVisualizer = Shake.Trauma;
#endif

            if (Frozen)
                return;

            _focusArea.Update();

            Vector3 targetPosition = ComputeBaseTargetPosition();

            ComputeLookAheadPosition(ref targetPosition);
            ComputeLookVerticalPosition(ref targetPosition);
            ComputeShakePosition(ref targetPosition);
            ComputeDampedPosition(ref targetPosition);
            ComputeBoundedPosition(ref targetPosition);

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

#if UNITY_EDITOR
        private void OnValidate()
        {
            if (!UnityEditor.EditorApplication.isPlaying)
                return;

            if (_shakes != null)
                GenerateShakesDictionary();
        }
#endif
    }
}