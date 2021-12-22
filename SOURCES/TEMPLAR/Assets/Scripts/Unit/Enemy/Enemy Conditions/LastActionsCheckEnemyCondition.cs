namespace Templar.Unit.Enemy.Conditions
{
    using System.Linq;

    public class LastActionsCheckEnemyCondition : EnemyCondition<Datas.Unit.Enemy.LastActionsCheckConditionDatas>
    {
        public LastActionsCheckEnemyCondition(EnemyController enemyCtrl, Datas.Unit.Enemy.LastActionsCheckConditionDatas conditionDatas)
            : base(enemyCtrl, conditionDatas)
        {
        }

        public override bool Check()
        {
            Actions.IEnemyAction[] lastActions = EnemyCtrl.LastActions.ToArray();
            int checkCount = UnityEngine.Mathf.Min(lastActions.Length, ConditionsDatas.ActionsCount);
            UnityEngine.Debug.LogError($"Checking last {checkCount} actions...");

            int includedTypesCount = 0;

            for (int i = 0; i < checkCount; ++i)
            {
                System.Type actionType = lastActions[lastActions.Length - 1 - i].GetType();

                // Check excluded.
                foreach (System.Type excludedType in ConditionsDatas.Exclude)
                {
                    UnityEngine.Debug.LogError($"Checking excluded type: {excludedType.Name}...");

                    if (actionType == excludedType)
                    {
                        UnityEngine.Debug.LogError($"Action type {excludedType.Name} must be excluded but has been found in {checkCount} last actions.");
                        return ApplyNegation(false);
                    }
                }

                // Check included.
                foreach (System.Type includedType in ConditionsDatas.Include)
                {
                    UnityEngine.Debug.LogError($"Checking included type: {includedType.Name}...");
                    if (actionType == includedType)
                        includedTypesCount++;
                }

                if (includedTypesCount < ConditionsDatas.Include.Count)
                {
                    UnityEngine.Debug.LogError($"Any action is not included in {checkCount} last actions.");
                    return ApplyNegation(false);
                }
            }

            return ApplyNegation(true);
        }
    }
}