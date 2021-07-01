namespace Templar.Item.Conditions
{
    public abstract class ItemActionCondition<T> : IItemActionCondition where T : Datas.Item.ItemActionConditionDatas
    {
        private Datas.Item.ItemActionConditionDatas _conditionDatas;

        public ItemActionCondition(Item item, T conditionDatas)
        {
            Item = item;
            _conditionDatas = conditionDatas;
        }

        public Item Item { get; private set; }

        public T ConditionsDatas => _conditionDatas as T;

        public abstract bool Check();

        protected bool ApplyNegation(bool check)
        {
            return _conditionDatas.Negate ? !check : check;
        }
    }
}