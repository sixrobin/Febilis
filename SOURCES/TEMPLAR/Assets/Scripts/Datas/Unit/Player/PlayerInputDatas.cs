namespace Templar.Datas.Unit.Player
{
    using UnityEngine;

    [CreateAssetMenu(fileName = "New Player Input Datas", menuName = "Datas/Player/Input")]
    public class PlayerInputDatas : ValuesValidatedEventScriptableObject
    {
        [Header("INPUT")]
        [Tooltip("Maximum value below which left stick inclinaison is ignored.")]
        [SerializeField, Range(0f, 1f)] private float _leftJoystickDeadZone = 0.4f;

        [Tooltip("Duration during which the jump input is stored and can be applied.")]
        [SerializeField, Min(0f)] private float _jumpInputDelay = 0.15f;

        [Tooltip("Duration during which the roll input is stored and can be applied.")]
        [SerializeField, Min(0f)] private float _rollInputDelay = 0.15f;

        [Tooltip("Duration during which the attack input is stored and can be applied.")]
        [SerializeField, Min(0f)] private float _attackInputDelay = 0.15f;

        public float LeftJoystickDeadZoneSqr => _leftJoystickDeadZone * _leftJoystickDeadZone;
        public float JumpInputDelay => _jumpInputDelay;
        public float RollInputDelay => _rollInputDelay;
        public float AttackInputDelay => _attackInputDelay;
    }
}