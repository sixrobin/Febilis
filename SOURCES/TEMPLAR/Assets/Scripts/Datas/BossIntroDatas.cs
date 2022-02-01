namespace Templar.Datas
{
    using UnityEngine;

    [CreateAssetMenu(fileName = "New Boss Intro Datas", menuName = "Datas/Boss Intro")]
    public class BossIntroDatas : ScriptableObject
    {
        [SerializeField, Min(0f)] private float _totalDuration = 0f;
        [SerializeField, Min(0f)] private float _bossNameAppearanceDelay = 0f;
        [SerializeField, Min(0f)] private float _bossNameDuration = 1.5f;
        [SerializeField] private bool _cameraFocusBoss = true;
        [SerializeField] private bool _disallowInputs = true;

        public float TotalDuration => _totalDuration;
        public float BossNameAppearanceDelay => _bossNameAppearanceDelay;
        public float BossNameDuration => _bossNameDuration;
        public bool CameraFocusBoss => _cameraFocusBoss;
        public bool DisallowInputs => _disallowInputs;
    }
}