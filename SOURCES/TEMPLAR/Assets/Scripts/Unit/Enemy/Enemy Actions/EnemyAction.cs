namespace Templar.Unit.Enemy.Actions
{
    public abstract class EnemyAction<T> : EnemyConditionsChecker, IEnemyAction where T : Datas.Unit.Enemy.EnemyActionDatas
    {
        public EnemyAction(EnemyController enemyCtrl, T actionDatas)
            : base(enemyCtrl, actionDatas)
        {
            ActionDatas = actionDatas as T;
            Init();
        }

        public T ActionDatas { get; private set; }

        public abstract bool CanExit();
        public abstract void Execute();

        public virtual void Init()
        {
        }

        public virtual void Reset()
        {
        }
    }
}