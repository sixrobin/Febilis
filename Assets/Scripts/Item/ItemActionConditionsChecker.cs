namespace Templar.Item
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

        public Datas.Item.ItemActionConditionsCheckerDatas ConditionsCheckerDatas { get; }
        public Item Item { get; }

        public IItemActionCondition[] Conditions { get; private set; } // Can be null if there is no condition.

        public void CreateConditions()
        {
            // No condition.
            if (ConditionsCheckerDatas?.Conditions == null)
                return;

            Conditions = new IItemActionCondition[ConditionsCheckerDatas.Conditions.Count];

            for (int i = Conditions.Length - 1; i >= 0; --i)
            {
                if (ConditionsCheckerDatas.Conditions[i] is Datas.Item.FullHealthItemActionConditionDatas fullHealthCondition)
                    Conditions[i] = new FullHealthItemActionCondition(Item, fullHealthCondition);
                else if (ConditionsCheckerDatas.Conditions[i] is Datas.Item.OnValidInteractableItemActionConditionDatas onValidInteractableCondition)
                    Conditions[i] = new OnValidInteractableItemActionCondition(Item, onValidInteractableCondition);
                else
                    CProLogger.LogError(this, $"Unknown Item Action Condition type {Conditions[i].GetType().FullName} in item {Item.Datas.Id}");
            }
        }

        public bool CheckConditions()
        {
            if (ConditionsCheckerDatas == null)
                return false; // Action is not defined, then not allowed.

            if (Conditions == null)
                return true; // Action is defined, but without needed condition.

            for (int i = Conditions.Length - 1; i >= 0; --i)
                if (!Conditions[i].Check())
                    return false;

            return true;
        }
    }
}