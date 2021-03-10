using UnityEngine;

[CreateAssetMenu(fileName = "New Templar Controller Datas", menuName = "Datas/Templar/Controller")]
public class TemplarControllerDatas : ScriptableObject
{
    [Header("INPUT")]
    [SerializeField] private TemplarInputDatas _templarInputDatas = null;

    [Header("JUMP")]
    [SerializeField] private TemplarJumpDatas _templarJumpDatas = null;

    [Header("ROLL")]
    [SerializeField] private TemplarRollDatas _templarRollDatas = null;

    [Header("ATTACK")]
    [SerializeField] private TemplarAttackDatas[] _baseCombo = null;
    [SerializeField] private TemplarAttackDatas _airborneAttack = null;

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
    [SerializeField] private Recoil.RecoilSettings _hurtRecoilSettings = null;

    public TemplarInputDatas Input => _templarInputDatas;
    public TemplarJumpDatas Jump => _templarJumpDatas;
    public TemplarRollDatas Roll => _templarRollDatas;
    public TemplarAttackDatas[] BaseCombo => _baseCombo;
    public TemplarAttackDatas AirborneAttack => _airborneAttack;

    public float RunSpeed => _runSpeed;
    public float GroundedDamping => _groundedDamping;
    public bool GroundOnAwake => _groundOnAwake;
    public float HurtDur => _hurtDur;
    public float MaxFallVelocity => _maxFallVelocity;
    public Recoil.RecoilSettings HurtRecoilSettings => _hurtRecoilSettings;
}