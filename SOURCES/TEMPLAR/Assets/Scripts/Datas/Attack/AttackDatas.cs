namespace Templar.Datas.Attack
{
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
        [SerializeField] private Templar.Attack.HitLayer _hitLayer = Templar.Attack.HitLayer.NONE;

        [Tooltip("Method used to compute direction to apply to hit target. Look at enum definition for more infos.")]
        [SerializeField] private Templar.Attack.HitDirComputationType _hitDirComputationType = Templar.Attack.HitDirComputationType.ATTACK_DIR;

        [Tooltip("Multiplier applied to attack animation speed.")]
        [SerializeField] private float _animMult = 1f;

        [Tooltip("Shake trauma applied on the attack frame, no matter the attack hits or not.")]
        [SerializeField] private Vector2 _traumaOnAttackFrame = Vector2.zero;

        [Tooltip("Shake trauma applied if the attack hits at least one target.")]
        [SerializeField] private Vector2 _traumaOnHit = Vector2.zero;

        [Tooltip("Freeze frame duration if the attack hits at least one target.")]
        [SerializeField, Min(0f)] private float _freezeFrameDurOnHit = 0.1f;

        // If a "parriable" boolean exists someday, it should be there.

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
                defaultDatas._dmg = 25;
                defaultDatas._dur = 0f;
                defaultDatas._hitLayer = Templar.Attack.HitLayer.ALL;
                defaultDatas._traumaOnHit = Vector2.zero;
                defaultDatas._freezeFrameDurOnHit = 0f;

                return defaultDatas;
            }
        }

        public string Id => _id;
        public int Dmg => _dmg;
        public float Dur => _dur;
        public float HitDur => _hitDur;
        public Templar.Attack.HitLayer HitLayer => _hitLayer;
        public Templar.Attack.HitDirComputationType HitDirComputationType => _hitDirComputationType;
        public float AnimMult => _animMult;
        public Vector2 TraumaOnAttackFrame => _traumaOnAttackFrame;
        public Vector2 TraumaOnHit => _traumaOnHit;
        public float FreezeFrameDurOnHit => _freezeFrameDurOnHit;
        
        protected virtual void OnValidate()
        {
            _traumaOnHit = _traumaOnHit.ClampAll01();
        }
    }
}