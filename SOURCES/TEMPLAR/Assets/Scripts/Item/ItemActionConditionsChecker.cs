﻿namespace Templar.Item
{
    using Conditions;

    public class ItemActionConditionsChecker
    {
        public ItemActionConditionsChecker(Item item, Datas.Item.ItemActionConditionsCheckerDatas conditionsCheckerDatas)
        {
            Item = item;
            ConditionsCheckerDatas = conditionsCheckerDatas;

            CreateConditions();
        }

        public Datas.Item.ItemActionConditionsCheckerDatas ConditionsCheckerDatas { get; private set; }
        public Item Item { get; private set; }

        public IItemActionCondition[] Conditions { get; private set; } // Can be null if there is no condition.

        public void CreateConditions()
        {
            // No condition.
            if (ConditionsCheckerDatas == null || ConditionsCheckerDatas.Conditions == null)
                return;

            Conditions = new IItemActionCondition[ConditionsCheckerDatas.Conditions.Count];

            for (int i = Conditions.Length - 1; i >= 0; --i)
            {
                if (ConditionsCheckerDatas.Conditions[i] is Datas.Item.FullHealthItemActionConditionDatas fullHealthCondition)
                    Conditions[i] = new FullHealthItemActionCondition(Item, fullHealthCondition);
                else
                    CProLogger.LogError(this, $"Unknown Item Action Condition type {Conditions[i].GetType().FullName}");
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