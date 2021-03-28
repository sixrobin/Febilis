namespace Templar.Dev
{
    public interface IEnemyAction
    {
        bool CheckConditions();
        void Execute();
        void Reset();
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

        public virtual void Reset()
        {
        }
    }
}