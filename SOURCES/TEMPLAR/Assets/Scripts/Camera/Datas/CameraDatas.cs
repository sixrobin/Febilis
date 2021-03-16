namespace Templar.Camera.Datas
{
    using RSLib.Extensions;
    using UnityEngine;

    [CreateAssetMenu(fileName = "New Camera Datas", menuName = "Datas/Camera")]
    public class CameraDatas : ScriptableObject
    {
        [Header("CAMERA")]
        [Tooltip("Dimensions of the rect the player stays inside and determines when the camera needs to move (when he \"pushes\" the rect around).")]
        [SerializeField] private Vector2 _focusAreaSize = Vector2.zero;

        [Tooltip("Damping applied to camera when looking ahead.")]
        [SerializeField, Min(0f)] private float _horizontalLookAheadDamping = 0.1f;

        [Tooltip("Distance added to camera X when looking ahead.")]
        [SerializeField, Min(0f)] private float _horizontalLookAheadDist = 1f;

        [Tooltip("Damping applied to camera Y.")]
        [SerializeField, Min(0f)] private float _verticalDamping = 0.08f;

        [Tooltip("Height added to camera at every frame.")]
        [SerializeField] private float _heightOffset = 1f;

        public Vector2 FocusAreaSize => _focusAreaSize;
        public float HorizontalLookAheadDamping => _horizontalLookAheadDamping;
        public float HorizontalLookAheadDist => _horizontalLookAheadDist;
        public float VerticalDamping => _verticalDamping;
        public float HeightOffset => _heightOffset;

        private void OnValidate()
        {
            _horizontalLookAheadDamping = Mathf.Max(_horizontalLookAheadDamping, 0f);
            _focusAreaSize = _focusAreaSize.ClampAll(0f, float.MaxValue);
        }
    }
}