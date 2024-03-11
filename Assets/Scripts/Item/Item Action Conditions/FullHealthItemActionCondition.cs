namespace Templar.Item.Conditions
{
    public class FullHealthItemActionCondition : ItemActionCondition<Datas.Item.FullHealthItemActionConditionDatas>
    {
        public FullHealthItemActionCondition(Item item, Datas.Item.FullHealthItemActionConditionDatas conditionDatas)
            : base(item, conditionDatas)
        {
        }

        public override bool Check()
        {
            return ApplyNegation(Manager.GameManager.PlayerCtrl.HealthCtrl.HealthSystem.HealthPercentage == 1f);
        }
    }
}