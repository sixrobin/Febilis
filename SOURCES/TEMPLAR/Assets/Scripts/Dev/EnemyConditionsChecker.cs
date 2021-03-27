namespace Templar.Dev
{
    public class EnemyConditionsChecker
    {
        public EnemyConditionsChecker(GenericEnemyController enemyCtrl, EnemyConditionsCheckerDatas conditionsCheckerDatas)
        {
            EnemyCtrl = enemyCtrl;
            ConditionsCheckerDatas = conditionsCheckerDatas;

            CreateConditions();
        }

        public EnemyConditionsCheckerDatas ConditionsCheckerDatas { get; private set; }
        public GenericEnemyController EnemyCtrl { get; private set; }

        public IEnemyCondition[] Conditions { get; private set; } // Can be null if there is no condition.

        public void CreateConditions()
        {
            // No condition.
            if (ConditionsCheckerDatas.Conditions == null)
                return;

            Conditions = new IEnemyCondition[ConditionsCheckerDatas.Conditions.Count];

            for (int i = Conditions.Length - 1; i >= 0; --i)
            {
                if (ConditionsCheckerDatas.Conditions[i] is HealthMaxEnemyConditionDatas healthMaxCondition)
                    Conditions[i] = new HealthMaxEnemyCondition(EnemyCtrl, healthMaxCondition);
                else if (ConditionsCheckerDatas.Conditions[i] is HealthMinEnemyConditionDatas healthMinCondition)
                    Conditions[i] = new HealthMinEnemyCondition(EnemyCtrl, healthMinCondition);
                else if (ConditionsCheckerDatas.Conditions[i] is FullHealthEnemyConditionDatas fullHealthCondition)
                    Conditions[i] = new FullHealthEnemyCondition(EnemyCtrl, fullHealthCondition);
                else if (ConditionsCheckerDatas.Conditions[i] is PlayerDetectedEnemyConditionDatas playerUndetectedCondition)
                    Conditions[i] = new PlayerDetectedEnemyCondition(EnemyCtrl, playerUndetectedCondition);
                else if (ConditionsCheckerDatas.Conditions[i] is RandomChanceConditionDatas rndChanceCondition)
                    Conditions[i] = new RandomChanceEnemyCondition(EnemyCtrl, rndChanceCondition);
                else
                    CProLogger.LogError(this, $"Unknown condition type {Conditions[i].GetType().FullName}");
            }
        }

        public bool CheckConditions()
        {
            // No condition.
            if (Conditions == null)
                return true;

            for (int i = Conditions.Length - 1; i >= 0; --i)
                if (!Conditions[i].Check())
                    return false;

            return true;
        }
    }
}