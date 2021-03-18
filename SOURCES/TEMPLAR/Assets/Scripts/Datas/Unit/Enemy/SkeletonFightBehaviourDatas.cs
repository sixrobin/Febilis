namespace Templar.Datas.Unit.Enemy
{
    using UnityEngine;

    [CreateAssetMenu(fileName = "New Skeleton Fight Behaviour Datas", menuName = "Datas/Skeleton/Fight Behaviour")]
    public class SkeletonFightBehaviourDatas : ScriptableObject
    {
        [Header("BEHAVIOUR")]
        [Tooltip("Distance from which the skeleton \"sees\" the player and can behave accordingly.")]
        [SerializeField, Min(0f)] private float _targetDetectionRange = 3.5f;

        [Tooltip("Distance from which the skeleton can attack the player.")]
        [SerializeField, Min(0f)] private float _targetAttackRange = 2.5f;

        [Tooltip("Time the skeleton will wait once in attack range being triggering the actual attack.")]
        [SerializeField, Min(0f)] private float _beforeAttackDur = 0.5f;

        [Header("ATTACK")]
        [SerializeField] private Attack.Datas.SkeletonAttackDatas _baseAttack = null;
        [SerializeField] private Attack.Datas.SkeletonAttackDatas _aboveAttack = null;

        public float TargetDetectionRange => _targetDetectionRange;
        public float TargetDetectionRangeSqr => TargetDetectionRange * TargetDetectionRange;
        public float TargetAttackRange => _targetAttackRange;
        public float TargetAttackRangeSqr => TargetAttackRange * TargetAttackRange;
        public float BeforeAttackDur => _beforeAttackDur;

        public Attack.Datas.SkeletonAttackDatas BaseAttack => _baseAttack;
        public Attack.Datas.SkeletonAttackDatas AboveAttack => _aboveAttack;
    }
}