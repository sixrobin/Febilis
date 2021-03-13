using RSLib.Extensions;
using UnityEngine;

public class AttackDatas : ScriptableObject
{
    [Tooltip("Identifier used to retrieve the attack hitbox.")]
    [SerializeField] private string _id = string.Empty;

    [Tooltip("Base damage applied to any hit target.")]
    [SerializeField, Min(0)] private int _dmg = 10;

    [Tooltip("Full attack motion duration.")]
    [SerializeField] private float _dur = 0.8f;

    [Tooltip("Duration during which the attack hitbox is enabled")]
    [SerializeField, Min(0f)] private float _hitDur = 0.1f;

    [Tooltip("Targets than can be hit by the attack.")]
    [SerializeField] private HitLayer _hitLayer = HitLayer.None;

    [Tooltip("Multiplier applied to attack animation speed.")]
    [SerializeField] private float _animMult = 1f;

    [Tooltip("Shake trauma applied if the attack hits at least one target.")]
    [SerializeField] private Vector2 _traumaOnHit = Vector2.zero;

    [Tooltip("Freeze frame duration if the attack hits at least one target.")]
    [SerializeField, Min(0f)] private float _freezeFrameDurOnHit = 0.1f;

    /// <summary>
    /// Instantiates a default datas container that can be used for testing purpose, without having
    /// to create a new asset and reference it in scripts.
    /// </summary>
    public static AttackDatas Default
    {
        get
        {
            AttackDatas defaultDatas = CreateInstance<AttackDatas>();
            defaultDatas._id = "Default";
            defaultDatas._dmg = 10;
            defaultDatas._dur = 0f;
            defaultDatas._hitLayer = HitLayer.All;
            defaultDatas._traumaOnHit = Vector2.zero;
            defaultDatas._freezeFrameDurOnHit = 0f;

            return defaultDatas;
        }
    }

    public string Id => _id;
    public int Dmg => _dmg;
    public float Dur => _dur;
    public float HitDur => _hitDur;
    public HitLayer HitLayer => _hitLayer;
    public float AnimMult => _animMult;
    public Vector2 TraumaOnHit => _traumaOnHit;
    public float FreezeFrameDurOnHit => _freezeFrameDurOnHit;

    protected virtual void OnValidate()
    {
        _traumaOnHit = _traumaOnHit.ClampAll01();
    }
}