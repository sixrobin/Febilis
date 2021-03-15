using UnityEngine;

[CreateAssetMenu(fileName = "New Skeleton Controller Datas", menuName = "Datas/Skeleton/Controller")]
public class SkeletonControllerDatas : ScriptableObject
{
    [Header("ATTACK")]
    [SerializeField] private SkeletonAttackDatas _baseAttack = null;
    [SerializeField] private SkeletonAttackDatas _aboveAttack = null;

    public SkeletonAttackDatas BaseAttack => _baseAttack;
    public SkeletonAttackDatas AboveAttack => _aboveAttack;
}