using UnityEngine;

namespace Templar.Attack
{
    public class EnemyAttackController : AttackController
    {
        private Unit.Enemy.EnemyController _enemyCtrl;
        private Datas.Attack.EnemyAttackDatas _currAttackDatas;
        
        public EnemyAttackController(Unit.Enemy.EnemyController enemyCtrl)
            : base(enemyCtrl, enemyCtrl.AttackHitboxesContainer, enemyCtrl.transform, enemyCtrl)
        {
            _enemyCtrl = enemyCtrl;
        }

        protected override Renderer AttackerRenderer => _enemyCtrl.EnemyView.Renderer;

        public void Attack(Unit.Enemy.Actions.AttackEnemyAction attackAction, AttackOverEventHandler attackOverCallback = null)
        {
            _attackCoroutineRunner.StartCoroutine(_attackCoroutine = AttackCoroutine(attackAction, attackOverCallback));
        }

        protected override void OnAttackHit(AttackHitbox.HitEventArgs hitArgs)
        {
            UnityEngine.Assertions.Assert.IsNotNull(_currAttackDatas, "An attack hit has been triggered but enemy attack datas are null.");
            Manager.GameManager.CameraCtrl.ApplyShakeFromDatas(_currAttackDatas.HitTraumaDatas, AttackerRenderer);
            Manager.FreezeFrameManager.FreezeFrame(0, _currAttackDatas.HitFreezeFrameDur);
        }

        protected override void ComputeAttackDirection()
        {
            AttackDir = _enemyCtrl.CurrDir;
        }

        private System.Collections.IEnumerator AttackCoroutine(Unit.Enemy.Actions.AttackEnemyAction attackAction, AttackOverEventHandler attackOverCallback = null)
        {
            if (!Database.AttackDatabase.EnemyAttacksDatas.TryGetValue(attackAction.ActionDatas.Id, out _currAttackDatas))
            {
                // Should never happen.
                _currAttackDatas = Datas.Attack.EnemyAttackDatas.Default;
                CProLogger.LogError(this, $"Enemy Attack Datas with Id {attackAction.ActionDatas.Id} could not be found using default datas instead.");
            }

            ComputeAttackDirection();

            _enemyCtrl.EnemyView.SetupAttackAnimationsDatas(attackAction, _currAttackDatas);
            _enemyCtrl.EnemyView.PlayAttackAnticipationAnimation();

            yield return RSLib.Yield.SharedYields.WaitForSeconds(_currAttackDatas.AnticipationDur);

            _enemyCtrl.EnemyView.PlayAttackAnimation();
            TriggerHit(_currAttackDatas, attackAction.ActionDatas.Id);

            yield return RSLib.Yield.SharedYields.WaitForSeconds(_currAttackDatas.AttackDur);

            attackOverCallback?.Invoke(new AttackOverEventArgs(_currAttackDatas, AttackDir));
            _attackCoroutine = null;

            _enemyCtrl.EnemyView.PlayIdleAnimation();
        }
    }
}