namespace Templar.Datas.Unit.Player
{
    using UnityEngine;

#if UNITY_EDITOR
    [UnityEditor.CanEditMultipleObjects]
#endif
    [CreateAssetMenu(fileName = "New Player Controller Datas", menuName = "Datas/Player/Controller")]
    public class PlayerControllerDatas : ValuesValidatedEventScriptableObject
    {
        [Header("INPUT")]
        [SerializeField] private PlayerInputDatas _playerInputDatas = null;

        [Header("JUMP")]
        [SerializeField] private UnitJumpDatas _playerJumpDatas = null;

        [Header("ROLL")]
        [SerializeField] private UnitRollDatas _playerRollDatas = null;

        [Header("ATTACK")]
        [SerializeField] private string[] _baseComboIds = null;
        [SerializeField] private string _airborneAttackId = null;

        [Header("BASE MOVEMENT")]
        [Tooltip("Base run speed of the controller.")]
        [SerializeField, Min(0f)] private float _runSpeed = 1f;

        [Tooltip("Minimum run speed, despite axis dead zone being possibly very low.")]
        [SerializeField, Min(0f)] private float _minRunSpeed = 0.5f;

        [Tooltip("Duration in seconds to reach target speed when controller is on the ground. Use 0 for no damping.")]
        [SerializeField, Range(0f, 1f)] private float _groundedDamping = 0f;

        [Tooltip("Instantly grounds the controller on awake without triggering any event, just for visual purpose.")]
        [SerializeField] private bool _groundOnAwake = true;

        [Tooltip("Full hurt motion duration.")]
        [SerializeField, Min(0f)] private float _hurtDur = 1f;

        [Tooltip("Maximum fall velocity.")]
        [SerializeField, Min(0f)] private float _maxFallVelocity = 1000f;

        [Tooltip("Time between heal input and actual healing.")]
        [SerializeField, Min(0f)] private float _preHealDelay = 1f;

        [Tooltip("Time between healing and back to idle.")]
        [SerializeField, Min(0f)] private float _postHealDelay = 1f;

        public PlayerInputDatas Input => _playerInputDatas;
        public UnitJumpDatas Jump => _playerJumpDatas;
        public UnitRollDatas Roll => _playerRollDatas;
        public string[] BaseComboIds => _baseComboIds;
        public string AirborneAttackId => _airborneAttackId;
        
        public float RunSpeed => _runSpeed;
        public float MinRunSpeed => _minRunSpeed;
        public float GroundedDamping => _groundedDamping;
        public bool GroundOnAwake => _groundOnAwake;
        public float HurtDur => _hurtDur;
        public float MaxFallVelocity => _maxFallVelocity;
        public float PreHealDelay => _preHealDelay;
        public float PostHealDelay => _postHealDelay;

        protected override void OnValidate()
        {
            base.OnValidate();

            _minRunSpeed = Mathf.Min(MinRunSpeed, RunSpeed);
        }
    }
}