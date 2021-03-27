namespace Templar.Dev
{
    public interface IEnemyAction
    {
        void Execute();
    }

    public abstract class EnemyAction<T> : EnemyConditionsChecker, IEnemyAction where T : EnemyActionDatas
    {
        private EnemyActionDatas _actionDatas;

        public EnemyAction(GenericEnemyController enemyCtrl, T actionDatas)
            : base(enemyCtrl, actionDatas)
        {
            _actionDatas = actionDatas;
        }

        public T ActionDatas => _actionDatas as T;

        public abstract void Execute();
    }

    public class AttackAction : EnemyAction<AttackEnemyActionDatas>
    {
        public AttackAction(GenericEnemyController enemyCtrl, AttackEnemyActionDatas actionDatas)
            : base(enemyCtrl, actionDatas)
        {
        }

        public override void Execute()
        {
            CProLogger.Log(this, "Attacking.");
        }
    }

    public class BackAndForthEnemyAction : EnemyAction<BackAndForthEnemyActionDatas>
    {
        public BackAndForthEnemyAction(GenericEnemyController enemyCtrl, BackAndForthEnemyActionDatas actionDatas)
            : base(enemyCtrl, actionDatas)
        {
        }

        public override void Execute()
        {
            CProLogger.Log(this, "Going back and forth.");
        }
    }

    public class FleeEnemyAction : EnemyAction<FleeEnemyActionDatas>
    {
        public FleeEnemyAction(GenericEnemyController enemyCtrl, FleeEnemyActionDatas actionDatas)
            : base(enemyCtrl, actionDatas)
        {
        }

        public override void Execute()
        {
            CProLogger.Log(this, "Fleeing.");
        }
    }
}