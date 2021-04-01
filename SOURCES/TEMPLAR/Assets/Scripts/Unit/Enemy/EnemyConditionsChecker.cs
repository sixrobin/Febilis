namespace Templar.Unit.Enemy
{
    using Conditions;

    public class EnemyConditionsChecker
    {
        public EnemyConditionsChecker(EnemyController enemyCtrl, Datas.Unit.Enemy.EnemyConditionsCheckerDatas conditionsCheckerDatas)
        {
            EnemyCtrl = enemyCtrl;
            ConditionsCheckerDatas = conditionsCheckerDatas;

            CreateConditions();
        }

        public Datas.Unit.Enemy.EnemyConditionsCheckerDatas ConditionsCheckerDatas { get; private set; }
        public EnemyController EnemyCtrl { get; private set; }

        public IEnemyCondition[] Conditions { get; private set; } // Can be null if there is no condition.

        public void CreateConditions()
        {
            // No condition.
            if (ConditionsCheckerDatas.Conditions == null)
                return;

            Conditions = new IEnemyCondition[ConditionsCheckerDatas.Conditions.Count];

            for (int i = Conditions.Length - 1; i >= 0; --i)
            {
                if (ConditionsCheckerDatas.Conditions[i] is Datas.Unit.Enemy.HealthMaxEnemyConditionDatas healthMaxCondition)
                    Conditions[i] = new HealthMaxEnemyCondition(EnemyCtrl, healthMaxCondition);
                else if (ConditionsCheckerDatas.Conditions[i] is Datas.Unit.Enemy.HealthMinEnemyConditionDatas healthMinCondition)
                    Conditions[i] = new HealthMinEnemyCondition(EnemyCtrl, healthMinCondition);
                else if (ConditionsCheckerDatas.Conditions[i] is Datas.Unit.Enemy.FullHealthEnemyConditionDatas fullHealthCondition)
                    Conditions[i] = new FullHealthEnemyCondition(EnemyCtrl, fullHealthCondition);
                else if (ConditionsCheckerDatas.Conditions[i] is Datas.Unit.Enemy.MaxHeightOffsetConditionDatas maxHealthOffsetCondition)
                    Conditions[i] = new MaxHeightOffsetEnemyCondition(EnemyCtrl, maxHealthOffsetCondition);
                else if (ConditionsCheckerDatas.Conditions[i] is Datas.Unit.Enemy.PlayerAboveEnemyConditionDatas playerAboveCondition)
                    Conditions[i] = new PlayerAboveEnemyCondition(EnemyCtrl, playerAboveCondition);
                else if (ConditionsCheckerDatas.Conditions[i] is Datas.Unit.Enemy.PlayerAliveEnemyConditionDatas playerAliveCondition)
                    Conditions[i] = new PlayerAliveEnemyCondition(EnemyCtrl, playerAliveCondition);
                else if (ConditionsCheckerDatas.Conditions[i] is Datas.Unit.Enemy.PlayerDetectedEnemyConditionDatas playerDetectedCondition)
                    Conditions[i] = new PlayerDetectedEnemyCondition(EnemyCtrl, playerDetectedCondition);
                else if (ConditionsCheckerDatas.Conditions[i] is Datas.Unit.Enemy.PlayerInRangeEnemyConditionDatas playerInRangeCondition)
                    Conditions[i] = new PlayerInRangeEnemyCondition(EnemyCtrl, playerInRangeCondition);
                else if (ConditionsCheckerDatas.Conditions[i] is Datas.Unit.Enemy.RandomChanceConditionDatas rndChanceCondition)
                    Conditions[i] = new RandomChanceEnemyCondition(EnemyCtrl, rndChanceCondition);
                else
                    CProLogger.LogError(this, $"Unknown condition type {Conditions[i].GetType().FullName}");
            }
        }

        public bool CheckConditions()
        {
            if (Conditions == null)
                return true; // No condition.

            for (int i = Conditions.Length - 1; i >= 0; --i)
                if (!Conditions[i].Check())
                    return false;

            return true;
        }
    }
}