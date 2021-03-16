namespace Templar.Attack.Datas
{
    using UnityEngine;

    [CreateAssetMenu(fileName = "New Skeleton Attack Datas", menuName = "Datas/Skeleton/Attack")]
    public class SkeletonAttackDatas : AttackDatas
    {
        [Tooltip("Duration from the attack animation beginning to the actual attack application.")]
        [SerializeField] private float _attackAnticipationDur = 0.5f;

        [Tooltip("Duration from the attack application to the comeback to idle.")]
        [SerializeField] private float _attackDur = 0.5f;

        [Tooltip("Suffix added to base animator parameters to play specific clips.")]
        [SerializeField] private string _animatorParamsSuffix = string.Empty;

        // [TODO] bool _parriable

        public float AttackAnticipationDur => _attackAnticipationDur;
        public float AttackDur => _attackDur;
        public string AnimatorParamsSuffix => _animatorParamsSuffix;
    }
}