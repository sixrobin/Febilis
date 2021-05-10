﻿namespace Templar.Unit.Enemy
{
    using System.Collections.Generic;
    using UnityEngine;

    public class EnemyView : UnitView
    {
        private const float DEAD_FADE_DELAY = 0.8f;

        private const string ATTACK_ANM_OVERRIDE_ID = "Attack";
        private const string ATTACK_ANTICIPATION_ANM_OVERRIDE_ID = "Attack Anticipation";
        private const string ATTACK_CLIP_NAME_FORMAT = "Anm_{0}_Attack_{1}";
        private const string ATTACK_ANTICIPATION_CLIP_NAME_FORMAT = "Anm_{0}_Attack_{1}_Anticipation";

        private const string IS_WALKING = "IsWalking";
        private const string ATTACK_ANTICIPATION = "Attack_Anticipation";

        public string EnemyId { get; private set; }

        public override float DeadFadeDelay => DEAD_FADE_DELAY;

        public void Init(string enemyId)
        {
            EnemyId = enemyId;
            InitAnimatorOverrideController();
        }

        public void PlayWalkAnimation(bool state)
        {
            _animator.SetBool(IS_WALKING, state);
        }

        public void SetupAttackAnimationsDatas(string id, Datas.Attack.AttackDatas attackDatas)
        {
            string attackClipName = string.Format(ATTACK_CLIP_NAME_FORMAT, EnemyId, id);
            string attackAnticipationClipName = string.Format(ATTACK_ANTICIPATION_CLIP_NAME_FORMAT, EnemyId, id);

            UnityEngine.Assertions.Assert.IsTrue(Datas.Unit.Enemy.EnemyDatabase.AnimationClips.ContainsKey(attackClipName), $"Animation clip {attackClipName} was not found in {Datas.Unit.Enemy.EnemyDatabase.Instance.GetType().Name}.");
            UnityEngine.Assertions.Assert.IsTrue(Datas.Unit.Enemy.EnemyDatabase.AnimationClips.ContainsKey(attackAnticipationClipName), $"Animation clip {attackAnticipationClipName} was not found in {Datas.Unit.Enemy.EnemyDatabase.Instance.GetType().Name}.");

            OverrideClip(ATTACK_ANM_OVERRIDE_ID, Datas.Unit.Enemy.EnemyDatabase.AnimationClips[attackClipName]);
            OverrideClip(ATTACK_ANTICIPATION_ANM_OVERRIDE_ID, Datas.Unit.Enemy.EnemyDatabase.AnimationClips[attackAnticipationClipName]);

            _animator.SetFloat(MULT_ATTACK, attackDatas.AnimSpeedMult);
        }

        public void PlayAttackAnticipationAnimation()
        {
            _animator.SetTrigger(ATTACK_ANTICIPATION);
        }

        public void PlayAttackAnimation()
        {
            _animator.SetTrigger(ATTACK);
        }
    }
}