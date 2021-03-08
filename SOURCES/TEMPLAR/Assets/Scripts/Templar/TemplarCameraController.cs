using UnityEngine;
using RSLib.Extensions;

public class TemplarCameraController : MonoBehaviour
{
    [SerializeField] private BoxCollider2D _targetCollider = null;
    [SerializeField] private Vector2 _focusAreaSize = Vector2.zero;
    [SerializeField] private float _yOffset = 1f;
    [SerializeField] private CameraShake.Settings _shakeSettings = CameraShake.Settings.Default;
    [SerializeField] private Color _debugColor = Color.red;

    private RSLib.FocusArea _focusArea;

    public CameraShake Shake { get; private set; }

    private void Awake()
    {
        _focusArea = new RSLib.FocusArea(_targetCollider, _focusAreaSize);
        Shake = new CameraShake(_shakeSettings);
    }

    private void LateUpdate()
    {
        _focusArea.Update();

        Vector3 targetPosition = _focusArea.Center + Vector2.up * _yOffset;
        transform.position = targetPosition.WithZ(transform.position.z);

        Shake.Apply(transform);
    }

    private void OnDrawGizmos()
    {
        _focusArea?.DrawArea(_debugColor);
    }

    private void OnValidate()
    {
        _focusArea?.SetSize(_focusAreaSize);
        Shake?.SetSettings(_shakeSettings);
    }
}