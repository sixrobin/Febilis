namespace Templar.ContextualConditions
{
    public abstract class ContextualConditionChecker<T> : IContextualConditionChecker
        where T : Datas.ContextualConditions.ContextualConditionDatas
    {
        protected T _conditionDatas;

        public ContextualConditionChecker(Datas.ContextualConditions.ContextualConditionDatas conditionDatas)
        {
            _conditionDatas = (T)conditionDatas;
        }

        public abstract bool Check();
    }


    public class HasItemContextualConditionChecker : ContextualConditionChecker<Datas.ContextualConditions.HasItemContextualConditionDatas>
    {
        public HasItemContextualConditionChecker(Datas.ContextualConditions.ContextualConditionDatas conditionDatas) : base(conditionDatas)
        {
        }

        public override bool Check()
        {
            return Manager.GameManager.InventoryCtrl.GetItemQuantity(_conditionDatas.ItemId) >= _conditionDatas.MinQuantity;
        }
    }


    public class DoesntHaveItemContextualConditionChecker : ContextualConditionChecker<Datas.ContextualConditions.DoesntHaveItemContextualConditionDatas>
    {
        public DoesntHaveItemContextualConditionChecker(Datas.ContextualConditions.ContextualConditionDatas conditionDatas) : base(conditionDatas)
        {
        }

        public override bool Check()
        {
            return Manager.GameManager.InventoryCtrl.GetItemQuantity(_conditionDatas.ItemId) == 0;
        }
    }
}