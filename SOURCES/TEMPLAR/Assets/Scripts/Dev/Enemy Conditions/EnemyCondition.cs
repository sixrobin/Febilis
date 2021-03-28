namespace Templar.Dev
{
    public interface IEnemyCondition
    {
        bool Check();
    }

    public abstract class EnemyCondition<T> : IEnemyCondition where T : EnemyConditionDatas
    {
        private EnemyConditionDatas _conditionDatas;

        public EnemyCondition(GenericEnemyController enemyCtrl, T conditionDatas)
        {
            EnemyCtrl = enemyCtrl;
            _conditionDatas = conditionDatas;
        }

        public GenericEnemyController EnemyCtrl { get; private set; }

        public T ConditionsDatas => _conditionDatas as T;

        public abstract bool Check();

        protected bool ApplyNegation(bool check)
        {
            return _conditionDatas.Negate ? !check : check;
        }
    }
}