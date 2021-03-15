using UnityEngine;

[CreateAssetMenu(fileName = "New Templar Attack Datas", menuName = "Datas/Templar/Attack")]
public class TemplarAttackDatas : AttackDatas
{
    [Tooltip("Duration from which the next attack, if it exists, can be triggered. Must then be less than full duration.")]
    [SerializeField] private float _chainAllowedTime = 0.4f;

    [Tooltip("Determines if controller velocity is driven by attack datas or not.")]
    [SerializeField] private bool _controlVelocity = true;

    [Tooltip("Speed that will be multiplied by the attack curve evaluation.")]
    [SerializeField] private float _moveSpeed = 0.3f;

    [Tooltip("Gravity that will be applied if controller is airborne, while attack motion is running.")]
    [SerializeField] private float _gravity = 0.5f;

    [Tooltip("Curve that will be applied to attack speed over the attack duration. Values should be between 0 and 1.")]
    [SerializeField] private AnimationCurve _moveSpeedCurve = null;

    public float ChainAllowedTime => _chainAllowedTime;
    public bool ControlVelocity => _controlVelocity;
    public float MoveSpeed => _moveSpeed;
    public float Gravity => _gravity;
    public AnimationCurve MoveSpeedCurve => _moveSpeedCurve;

    protected override void OnValidate()
    {
        _chainAllowedTime = Mathf.Min(_chainAllowedTime, Dur);
    }
}