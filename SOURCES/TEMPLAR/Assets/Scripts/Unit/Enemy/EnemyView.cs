namespace Templar.Unit.Enemy
{
    using UnityEngine;
    
    public class EnemyView : UnitView
    {
        private const string ATTACK_ANM_OVERRIDE_ID = "Attack";
        private const string ATTACK_ANTICIPATION_ANM_OVERRIDE_ID = "Attack Anticipation";
        private const string ATTACK_CLIP_NAME_FORMAT = "Anm_{0}_Attack_{1}";
        private const string ATTACK_ANTICIPATION_CLIP_NAME_FORMAT = "Anm_{0}_Attack_{1}_Anticipation";

        private const string CHARGE = "Charge";
        private const string CHARGE_ANTICIPATION = "Charge_Anticipation";
        private const string CHARGE_COLLISION = "Charge_Collision";
        private const string IS_WALKING = "IsWalking";
        private const string ATTACK_ANTICIPATION = "Attack_Anticipation";

        [Header("DEAD FADE")]
        [SerializeField] private float _deadFadeDelay = 1f;
        
        public string EnemyId { get; private set; }

        public override float DeadFadeDelay => _deadFadeDelay;

        public void Init(string enemyId)
        {
            EnemyId = enemyId;
            InitAnimatorOverrideController();
        }

        public void PlayWalkAnimation(bool state)
        {
            _animator.SetBool(IS_WALKING, state);
        }

        public void PlayChargeAnimation()
        {
            _animator.SetTrigger(CHARGE);
        }

        public void PlayChargeAnticipationAnimation()
        {
            _animator.SetTrigger(CHARGE_ANTICIPATION);
        }

        public void PlayChargeCollisionAnimation()
        {
            _animator.SetTrigger(CHARGE_COLLISION);
        }

        public void ResetChargeTriggers()
        {
            _animator.ResetTrigger(CHARGE);
            _animator.ResetTrigger(CHARGE_ANTICIPATION);
            _animator.ResetTrigger(CHARGE_COLLISION);
        }

        public void SetupAttackAnimationsDatas(Actions.AttackEnemyAction attackAction, Datas.Attack.AttackDatas attackDatas)
        {
            string enemyId = attackAction.ActionDatas.AnimatorEnemyIdOverride ?? EnemyId;

            string attackClipName = string.Format(ATTACK_CLIP_NAME_FORMAT, enemyId, attackAction.ActionDatas.AnimatorId);
            string attackAnticipationClipName = string.Format(ATTACK_ANTICIPATION_CLIP_NAME_FORMAT, enemyId, attackAction.ActionDatas.AnimatorId);

            UnityEngine.Assertions.Assert.IsTrue(Database.EnemyDatabase.AnimationClips.ContainsKey(attackClipName), $"Animation clip {attackClipName} was not found in {Database.EnemyDatabase.Instance.GetType().Name}.");
            UnityEngine.Assertions.Assert.IsTrue(Database.EnemyDatabase.AnimationClips.ContainsKey(attackAnticipationClipName), $"Animation clip {attackAnticipationClipName} was not found in {Database.EnemyDatabase.Instance.GetType().Name}.");

            OverrideClip(ATTACK_ANM_OVERRIDE_ID, Database.EnemyDatabase.AnimationClips[attackClipName]);
            OverrideClip(ATTACK_ANTICIPATION_ANM_OVERRIDE_ID, Database.EnemyDatabase.AnimationClips[attackAnticipationClipName]);

            _animator.SetFloat(MULT_ATTACK, attackDatas.AnimSpeedMult);
        }

        public void PlayAttackAnticipationAnimation()
        {
            _animator.SetTrigger(ATTACK_ANTICIPATION);
        }

        public void PlayAttackAnimation()
        {
            _animator.SetTrigger(ATTACK);
            RSLib.Audio.AudioManager.PlaySound(_attackClipProvider);
        }
    }
}