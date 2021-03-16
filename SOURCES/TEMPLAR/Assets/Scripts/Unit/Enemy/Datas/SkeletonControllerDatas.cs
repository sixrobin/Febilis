namespace Templar.Unit.Enemy.Datas
{
    using UnityEngine;

    [CreateAssetMenu(fileName = "New Skeleton Controller Datas", menuName = "Datas/Skeleton/Controller")]
    public class SkeletonControllerDatas : ScriptableObject
    {
        [Header("ATTACK")]
        [SerializeField] private Attack.Datas.SkeletonAttackDatas _baseAttack = null;
        [SerializeField] private Attack.Datas.SkeletonAttackDatas _aboveAttack = null;

        public Attack.Datas.SkeletonAttackDatas BaseAttack => _baseAttack;
        public Attack.Datas.SkeletonAttackDatas AboveAttack => _aboveAttack;
    }
}