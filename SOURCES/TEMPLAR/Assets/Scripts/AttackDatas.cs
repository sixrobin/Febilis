using UnityEngine;

public class AttackDatas : ScriptableObject
{
    [Tooltip("Identifier used to retrieve the attack hitbox.")]
    [SerializeField] private string _id = string.Empty;

    [Tooltip("Full attack motion duration.")]
    [SerializeField] private float _dur = 0.8f;

    [Tooltip("Duration during which the attack hitbox is enabled")]
    [SerializeField, Min(0f)] private float _hitDur = 0.1f;

    [Tooltip("Targets than can be hit by the attack.")]
    [SerializeField] private HitLayer _hitLayer = HitLayer.None;

    [Tooltip("Multiplier applied to attack animation speed.")]
    [SerializeField] private float _animMult = 1f;

    public string Id => _id;
    public float Dur => _dur;
    public float HitDur => _hitDur;
    public HitLayer HitLayer => _hitLayer;
    public float AnimMult => _animMult;

    protected virtual void OnValidate()
    {
    }
}