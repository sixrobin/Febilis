﻿namespace Templar.Attack
{
    using UnityEngine;

    public class HitInfos
    {
        public HitInfos(Datas.Attack.AttackDatas attackDatas, float attackDir, Transform source, AttackController attackController)
        {
            AttackDatas = attackDatas;
            AttackDir = attackDir;
            Source = source;
            AttackController = attackController;
        }

        public HitInfos(Datas.Attack.AttackDatas attackDatas, float attackDir, Transform source, AttackController attackController, Datas.Unit.Enemy.ChargeActionCollisionDatas chargeCollisionDatas)
            : this(attackDatas, attackDir, source, attackController)
        {
            ChargeCollisionDatas = chargeCollisionDatas;
        }

        public Datas.Attack.AttackDatas AttackDatas { get; private set; }
        public float AttackDir { get; private set; }
        public Transform Source { get; private set; }
        public AttackController AttackController { get; private set; }
        public Datas.Unit.Enemy.ChargeActionCollisionDatas ChargeCollisionDatas { get; private set; }

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

                case HitDirComputationType.NONE:
                default:
                    CProLogger.Log(this, "Returning the default value of the switch, even though it was not meant to happen.");
                    return AttackDir;
            }
        }
    }
}