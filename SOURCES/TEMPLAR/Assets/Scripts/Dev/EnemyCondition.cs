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

    public class HealthMaxEnemyCondition : EnemyCondition<HealthMaxEnemyConditionDatas>
    {
        public HealthMaxEnemyCondition(GenericEnemyController enemyCtrl, HealthMaxEnemyConditionDatas conditionDatas)
            : base(enemyCtrl, conditionDatas)
        {
        }

        public override bool Check()
        {
            return ApplyNegation(EnemyCtrl.HealthCtrl.HealthSystem.CurrentHealth <= ConditionsDatas.Threshold);
        }
    }

    public class HealthMinEnemyCondition : EnemyCondition<HealthMinEnemyConditionDatas>
    {
        public HealthMinEnemyCondition(GenericEnemyController enemyCtrl, HealthMinEnemyConditionDatas conditionDatas)
            : base(enemyCtrl, conditionDatas)
        {
        }

        public override bool Check()
        {
            return ApplyNegation(EnemyCtrl.HealthCtrl.HealthSystem.CurrentHealth >= ConditionsDatas.Threshold);
        }
    }

    public class FullHealthEnemyCondition : EnemyCondition<FullHealthEnemyConditionDatas>
    {
        public FullHealthEnemyCondition(GenericEnemyController enemyCtrl, FullHealthEnemyConditionDatas conditionDatas)
            : base(enemyCtrl, conditionDatas)
        {
        }

        public override bool Check()
        {
            return ApplyNegation(EnemyCtrl.HealthCtrl.HealthSystem.HealthPercentage == 1f);
        }
    }

    public class PlayerDetectedEnemyCondition : EnemyCondition<PlayerDetectedEnemyConditionDatas>
    {
        public PlayerDetectedEnemyCondition(GenericEnemyController enemyCtrl, PlayerDetectedEnemyConditionDatas conditionDatas)
            : base(enemyCtrl, conditionDatas)
        {
        }

        public override bool Check()
        {
            return ApplyNegation((EnemyCtrl.Player.position - EnemyCtrl.transform.position).sqrMagnitude
                <= EnemyCtrl.EnemyDatas.PlayerDetectionDistSqr);
        }
    }

    public class RandomChanceEnemyCondition : EnemyCondition<RandomChanceConditionDatas>
    {
        public RandomChanceEnemyCondition(GenericEnemyController enemyCtrl, RandomChanceConditionDatas conditionDatas)
            : base(enemyCtrl, conditionDatas)
        {
        }

        public override bool Check()
        {
            return ApplyNegation(UnityEngine.Random.value < ConditionsDatas.Chance);
        }
    }
}