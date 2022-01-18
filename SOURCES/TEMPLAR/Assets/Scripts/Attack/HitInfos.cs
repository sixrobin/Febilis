﻿namespace Templar.Attack
{
    using UnityEngine;

    public class HitInfos
    {
        public HitInfos(Datas.Attack.AttackDatas attackDatas, float attackDir, Transform source)
        {
            AttackDatas = attackDatas;
            AttackDir = attackDir;
            Source = source;
        }

        public HitInfos(Datas.Attack.AttackDatas attackDatas, float attackDir, Transform source, bool chargeCollision)
            : this(attackDatas, attackDir, source)
        {
            ChargeCollision = chargeCollision;
        }

        public Datas.Attack.AttackDatas AttackDatas { get; private set; }
        public float AttackDir { get; private set; }
        public Transform Source { get; private set; }
        public bool ChargeCollision { get; private set; }

        public float ComputeHitDir(Transform attackTarget)
        {
            UnityEngine.Assertions.Assert.IsTrue(
                AttackDatas.HitDirComputationType != HitDirComputationType.NONE,
                "Can't compute hit dir with no computation method set.");

            switch (AttackDatas.HitDirComputationType)
            {
                case HitDirComputationType.ATTACK_DIR:
                    return AttackDir;

                case HitDirComputationType.X_OFFSET:
                    return Mathf.Sign(attackTarget.position.x - Source.position.x);

                default:
                    CProLogger.Log(this, "Returning the default value of the switch, even though it was not meant to happen.");
                    return AttackDir;
            }
        }
    }
}