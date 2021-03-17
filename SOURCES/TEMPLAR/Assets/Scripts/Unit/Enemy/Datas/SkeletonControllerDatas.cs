namespace Templar.Unit.Enemy.Datas
{
    using UnityEngine;

    [CreateAssetMenu(fileName = "New Skeleton Controller Datas", menuName = "Datas/Skeleton/Controller")]
    public class SkeletonControllerDatas : ScriptableObject
    {
        [Header("FIGHT BEHAVIOUR")]
        [SerializeField] private SkeletonFightBehaviourDatas _fightBehaviourDatas = null;

        [Header("BASE MOVEMENT")]
        [Tooltip("Base walk speed of the skeleton.")]
        [SerializeField, Min(0f)] private float _walkSpeed = 1.3f;

        [Tooltip("Range in which the skeleton goes back and forth, his initial position being the middle point.")]
        [SerializeField] private float _backAndForthRange = 5f;

        [Tooltip("Time waited before changing duration during the back and forth motion.")]
        [SerializeField] private float _backAndForthPause = 1f;

        [Tooltip("Full hurt motion duration.")]
        [SerializeField, Min(0f)] private float _hurtDur = 0.25f;

        public SkeletonFightBehaviourDatas FightBehaviourDatas => _fightBehaviourDatas;

        public float WalkSpeed => _walkSpeed;
        public float BackAndForthRange => _backAndForthRange;
        public float HalfBackAndForthRange => BackAndForthRange * 0.5f;
        public float BackAndForthPause => _backAndForthPause;
        public float HurtDur => _hurtDur;
    }
}