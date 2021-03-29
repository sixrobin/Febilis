namespace Templar.Unit.Enemy.Conditions
{
    public abstract class EnemyCondition<T> : IEnemyCondition where T : Datas.Unit.Enemy.EnemyConditionDatas
    {
        private Datas.Unit.Enemy.EnemyConditionDatas _conditionDatas;

        public EnemyCondition(EnemyController enemyCtrl, T conditionDatas)
        {
            EnemyCtrl = enemyCtrl;
            _conditionDatas = conditionDatas;
        }

        public EnemyController EnemyCtrl { get; private set; }

        public T ConditionsDatas => _conditionDatas as T;

        public abstract bool Check();

        protected bool ApplyNegation(bool check)
        {
            return _conditionDatas.Negate ? !check : check;
        }
    }
}