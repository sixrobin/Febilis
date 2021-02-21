using UnityEngine;

[CreateAssetMenu(fileName = "New Skeleton Controller Datas", menuName = "Datas/Skeleton/Controller")]
public class SkeletonControllerDatas : ScriptableObject
{
    [Header("ATTACK")]
    [SerializeField] private SkeletonAttackDatas _baseAttack = null;

    public SkeletonAttackDatas BaseAttack => _baseAttack;
}