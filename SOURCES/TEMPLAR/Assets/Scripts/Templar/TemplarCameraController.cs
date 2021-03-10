using UnityEngine;
using RSLib.Extensions;

public class TemplarCameraController : MonoBehaviour
{
    [SerializeField] private TemplarCameraDatas _cameraDatas = null;
    [SerializeField] private TemplarController _templarController = null;
    [SerializeField] private CameraShake.Settings _shakeSettings = CameraShake.Settings.Default;

    [Header("DEBUG")]
    [SerializeField] private bool _debugOnSelectedOnly = true;
    [SerializeField] private RSLib.DataColor _debugColor = null;

    private RSLib.FocusArea _focusArea;

    private float _currLookAheadX;
    private float _targetLookAheadX;
    private float _lookAheadDir;
    private float _refX;
    private float _refY;
    private bool _isLookingAhead;

    public CameraShake Shake { get; private set; }

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
                _targetLookAheadX = _currLookAheadX + (_lookAheadDir * _cameraDatas.HorizontalLookAheadDist - _currLookAheadX) * 0.25f;
            }
        }
        else
        {
            _lookAheadDir = Mathf.Sign(_focusArea.Velocity.x);
            if (_templarController.InputCtrl.CurrentInputDir == _lookAheadDir
                && _templarController.InputCtrl.Horizontal != 0f
                || _templarController.RollCtrl.IsRolling)
            {
                _isLookingAhead = true;
                _targetLookAheadX = _lookAheadDir * _cameraDatas.HorizontalLookAheadDist;
            }
            else if (_isLookingAhead)
            {
                _isLookingAhead = false;
                _targetLookAheadX = _currLookAheadX + (_lookAheadDir * _cameraDatas.HorizontalLookAheadDist - _currLookAheadX) * 0.25f;
            }
        }

        _currLookAheadX = Mathf.SmoothDamp(_currLookAheadX, _targetLookAheadX, ref _refX, _cameraDatas.HorizontalLookAheadDamping);
        pos += Vector3.right * _currLookAheadX;
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

    private void Awake()
    {
        _focusArea = new RSLib.FocusArea(_templarController.BoxCollider2D, _cameraDatas.FocusAreaSize);
        Shake = new CameraShake(_shakeSettings);
    }

    private void LateUpdate()
    {
#if UNITY_EDITOR
        if (_focusArea.Size != _cameraDatas.FocusAreaSize)
            _focusArea = new RSLib.FocusArea(_templarController.BoxCollider2D, _cameraDatas.FocusAreaSize);
#endif

        _focusArea.Update();

        Vector3 targetPosition = ComputeBaseTargetPosition();

        ComputeLookAheadPosition(ref targetPosition);
        ComputeShakePosition(ref targetPosition);
        ComputeDampedPosition(ref targetPosition);

        transform.position = targetPosition.WithZ(transform.position.z);
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