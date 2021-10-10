namespace Templar.Camera
{
    using RSLib.Extensions;
    using UnityEngine;

    public class CameraController : MonoBehaviour
    {
        [SerializeField] private Datas.CameraDatas _cameraDatas = null;
        [SerializeField] private Camera _camera = null;
        [SerializeField] private Unit.Player.PlayerController _playerCtrl = null;
        [SerializeField] private RSLib.ImageEffects.CameraGrayscaleRamp _grayscaleRamp = null;
        [SerializeField] private Templar.Datas.ShakeSettingsLibrary _shakesLibrary = null;
        [SerializeField] private UnityEngine.U2D.PixelPerfectCamera _pixelPerfectCamera = null;

        //[Header("PIXEL PERFECT FIX")]
        //[SerializeField] private bool _toggleManualFix = true;
        //[SerializeField] private Vector2Int _referenceResolution = new Vector2Int(160, 144);
        //[SerializeField, Min(1)] private int _assetsPixelsPerUnit = 100;

        [Header("DEBUG")]
        [SerializeField] private bool _debugOnSelectedOnly = true;
        [SerializeField] private RSLib.DataColor _debugColor = null;
        [SerializeField] private Boards.OptionalBoardBounds _initBounds = new Boards.OptionalBoardBounds(null, false);
#if UNITY_EDITOR
        [SerializeField] private Boards.DisabledBoardBounds _currBoardBounds = new Boards.DisabledBoardBounds();
        [SerializeField] private RSLib.Framework.DisabledVector2 _currTraumaVisualizer = new RSLib.Framework.DisabledVector2();
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

        public Boards.OptionalBoardBounds InitBoardBounds => _initBounds;

        public Boards.BoardBounds CurrBoardBounds { get; private set; }

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

        public void SetBoardBounds(Boards.BoardBounds boardBounds)
        {
            if (CurrBoardBounds != null)
                for (int i = CurrBoardBounds.Switches.Length - 1; i >= 0; --i)
                    CurrBoardBounds.Switches[i].Enable(true);

            CurrBoardBounds = boardBounds;

            if (CurrBoardBounds != null)
            {
                for (int i = CurrBoardBounds.Switches.Length - 1; i >= 0; --i)
                    CurrBoardBounds.Switches[i].Enable(false);
            }
            else
            {
                CProLogger.Log(this, "CameraController bounds being set to null.");
            }
        }

        public void SetColor(Color color)
        {
            _camera.backgroundColor = color;
        }

        private void GenerateShakesDictionary()
        {
            _shakesDictionary = new System.Collections.Generic.Dictionary<string, CameraShake>();
            foreach (System.Collections.Generic.KeyValuePair<string, Templar.Datas.ShakeSettingsDatas> shakeSettings in _shakesLibrary.GetShakes())
                _shakesDictionary.Add(shakeSettings.Key, new CameraShake(shakeSettings.Value));
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

            float xMin = CurrBoardBounds.Bounds.bounds.min.x + halfWidth;
            float xMax = CurrBoardBounds.Bounds.bounds.max.x - halfWidth;
            float yMin = CurrBoardBounds.Bounds.bounds.min.y + halfHeight;
            float yMax = CurrBoardBounds.Bounds.bounds.max.y - halfHeight;

            pos.x = Mathf.Clamp(pos.x, xMin, xMax);
            pos.y = Mathf.Clamp(pos.y, yMin, yMax);
        }

        private void ComputeShakePosition(ref Vector3 pos)
        {
            foreach (System.Collections.Generic.KeyValuePair<string, CameraShake> shake in _shakesDictionary)
                pos += shake.Value.GetShakeWithSettings();
        }

        //private void UpdatePixelPerfectCameraSize()
        //{
        //    // Test method to fix pixel perfect jitter.

        //    if (!_toggleManualFix)
        //        return;

        //    int w = _camera.targetTexture?.width ?? Screen.width;
        //    int h = _camera.targetTexture?.height ?? Screen.height;

        //    int verticalZoom = h / _referenceResolution.y;
        //    int horizontalZoom = w / _referenceResolution.x;
        //    int zoom = Mathf.Max(1, Mathf.Min(verticalZoom, horizontalZoom));

        //    _camera.orthographicSize = h * 0.5f / (zoom * _assetsPixelsPerUnit);
        //}

        private void OnPixelPerfectValueChanged(bool value)
        {
            _pixelPerfectCamera.enabled = value;
        }

        private void Start()
        {
            _focusArea = new RSLib.FocusArea(_playerCtrl.BoxCollider2D, _cameraDatas.FocusAreaSize);
            GenerateShakesDictionary();
            PositionInstantly();

            _pixelPerfectCamera.enabled = Manager.SettingsManager.PixelPerfect.Value;

            Manager.SettingsManager.PixelPerfect.ValueChanged += OnPixelPerfectValueChanged;

            RSLib.Debug.Console.DebugConsole.OverrideCommand(new RSLib.Debug.Console.Command(
                "PixelPerfectToggle", "Toggles pixel perfect camera.", () => GetComponent<UnityEngine.U2D.PixelPerfectCamera>().enabled = !GetComponent<UnityEngine.U2D.PixelPerfectCamera>().enabled));
        }

        private void LateUpdate()
        {
#if UNITY_EDITOR
            if (_focusArea.Size != _cameraDatas.FocusAreaSize)
                _focusArea = new RSLib.FocusArea(_playerCtrl.BoxCollider2D, _cameraDatas.FocusAreaSize);

            _currBoardBounds = new Boards.DisabledBoardBounds(CurrBoardBounds);

            // [TODO] Visualize all shakes traumas.
            //_currTraumaVisualizer = new RSLib.Framework.DisabledVector2(Shake.Trauma);
#endif

            if (Frozen)
                return;

            _focusArea.Update();

            Vector3 targetPosition = ComputeBaseTargetPosition();

            ComputeLookAheadPosition(ref targetPosition);
            ComputeLookVerticalPosition(ref targetPosition);
            ComputeDampedPosition(ref targetPosition);
            ComputeBoundedPosition(ref targetPosition);
            ComputeShakePosition(ref targetPosition);

            transform.position = targetPosition.WithZ(transform.position.z);

            //UpdatePixelPerfectCameraSize();
        }

        private void OnDestroy()
        {
            Manager.SettingsManager.PixelPerfect.ValueChanged -= OnPixelPerfectValueChanged;
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

            if (_shakesDictionary != null)
                GenerateShakesDictionary();
        }
#endif
    }
}