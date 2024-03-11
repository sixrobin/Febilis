namespace Templar.Datas
{
    using UnityEngine;

    [CreateAssetMenu(fileName = "New Boss Intro Datas", menuName = "Datas/Boss Intro")]
    public class BossIntroDatas : ScriptableObject
    {
        [SerializeField, Min(0f)] private float _bossNameAppearanceDelay = 0f;
        [SerializeField, Min(0f)] private float _bossNameDuration = 1.5f;
        [SerializeField] private bool _disallowInputs = true;

        [Header("CAMERA FOCUS")]
        [SerializeField] private bool _cameraFocusBoss = true;
        [SerializeField, Min(0f)] private Vector3 _cameraFocusPositionOffset = Vector3.zero;
        [SerializeField, Min(0f)] private float _cameraFocusDelay = 0.5f;
        [SerializeField, Min(0f)] private float _cameraInDuration = 0.5f;
        [SerializeField, Min(0f)] private float _cameraOutDuration = 0.5f;
        [SerializeField, Min(0f)] private float _cameraFocusedDuration = 1f;
        [SerializeField] private RSLib.Maths.Curve _cameraInCurve = RSLib.Maths.Curve.Linear;
        [SerializeField] private RSLib.Maths.Curve _cameraOutCurve = RSLib.Maths.Curve.Linear;

        public float TotalDuration => CameraFocusDelay + CameraInDuration + CameraFocusedDuration + CameraOutDuration;
        public float BossNameAppearanceDelay => _bossNameAppearanceDelay;
        public float BossNameDuration => _bossNameDuration;
        public bool DisallowInputs => _disallowInputs;

        public bool CameraFocusBoss => _cameraFocusBoss;
        public Vector3 CameraFocusPositionOffset => _cameraFocusPositionOffset;
        public float CameraFocusDelay => _cameraFocusDelay;
        public float CameraInDuration => _cameraInDuration;
        public float CameraOutDuration => _cameraOutDuration;
        public float CameraFocusedDuration => _cameraFocusedDuration;
        public RSLib.Maths.Curve CameraInCurve => _cameraInCurve;
        public RSLib.Maths.Curve CameraOutCurve => _cameraOutCurve;
    }
}