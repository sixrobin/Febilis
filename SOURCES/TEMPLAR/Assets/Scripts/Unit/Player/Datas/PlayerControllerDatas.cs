namespace Templar.Unit.Player.Datas
{
    using UnityEngine;

    [CreateAssetMenu(fileName = "New Player Controller Datas", menuName = "Datas/Player/Controller")]
    public class PlayerControllerDatas : ScriptableObject
    {
        [Header("INPUT")]
        [SerializeField] private PlayerInputDatas _playerInputDatas = null;

        [Header("JUMP")]
        [SerializeField] private Unit.Datas.UnitJumpDatas _playerJumpDatas = null;

        [Header("ROLL")]
        [SerializeField] private Unit.Datas.UnitRollDatas _playerRollDatas = null;

        [Header("ATTACK")]
        [SerializeField] private Attack.Datas.PlayerAttackDatas[] _baseCombo = null;
        [SerializeField] private Attack.Datas.PlayerAttackDatas _airborneAttack = null;

        [Header("BASE MOVEMENT")]
        [Tooltip("Base run speed of the controller.")]
        [SerializeField, Min(0f)] private float _runSpeed = 1f;

        [Tooltip("Duration in seconds to reach target speed when controller is on the ground. Use 0 for no damping.")]
        [SerializeField, Range(0f, 1f)] private float _groundedDamping = 0f;

        [Tooltip("Instantly grounds the controller on awake without triggering any event, just for visual purpose.")]
        [SerializeField] private bool _groundOnAwake = true;

        [Tooltip("Full hurt motion duration.")]
        [SerializeField, Min(0f)] private float _hurtDur = 1f;

        [Tooltip("Maximum fall velocity.")]
        [SerializeField, Min(0f)] private float _maxFallVelocity = 1000f;

        [Tooltip("Recoil applied to controller when getting hurt.")]
        [SerializeField] private Templar.Physics.Recoil.RecoilSettings _hurtRecoilSettings = null;

        public PlayerInputDatas Input => _playerInputDatas;
        public Unit.Datas.UnitJumpDatas Jump => _playerJumpDatas;
        public Unit.Datas.UnitRollDatas Roll => _playerRollDatas;
        public Attack.Datas.PlayerAttackDatas[] BaseCombo => _baseCombo;
        public Attack.Datas.PlayerAttackDatas AirborneAttack => _airborneAttack;

        public float RunSpeed => _runSpeed;
        public float GroundedDamping => _groundedDamping;
        public bool GroundOnAwake => _groundOnAwake;
        public float HurtDur => _hurtDur;
        public float MaxFallVelocity => _maxFallVelocity;
        public Templar.Physics.Recoil.RecoilSettings HurtRecoilSettings => _hurtRecoilSettings;
    }
}