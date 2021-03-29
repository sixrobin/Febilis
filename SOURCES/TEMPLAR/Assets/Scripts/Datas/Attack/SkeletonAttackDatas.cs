﻿namespace Templar.Datas.Attack
{
    using UnityEngine;

    [CreateAssetMenu(fileName = "New Skeleton Attack Datas", menuName = "Datas/Skeleton/Attack")]
    public class SkeletonAttackDatas : AttackDatas
    {
        [Tooltip("Duration from the attack animation beginning to the actual attack application.")]
        [SerializeField] private float _attackAnticipationDur = 0.5f;

        [Tooltip("Duration from the attack application to the comeback to idle.")]
        [SerializeField] private float _attackDur = 0.5f;

        public float AttackAnticipationDur => _attackAnticipationDur;
        public float AttackDur => _attackDur;
    }
}