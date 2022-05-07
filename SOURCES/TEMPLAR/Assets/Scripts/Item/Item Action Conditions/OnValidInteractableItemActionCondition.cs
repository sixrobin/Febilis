namespace Templar.Item.Conditions
{
    public class OnValidInteractableItemActionCondition : ItemActionCondition<Datas.Item.OnValidInteractableItemActionConditionDatas>
    {
        public OnValidInteractableItemActionCondition(Item item, Datas.Item.OnValidInteractableItemActionConditionDatas conditionDatas)
            : base(item, conditionDatas)
        {
        }

        public override bool Check()
        {
            Interaction.IInteractable currentInteractable = Manager.GameManager.PlayerCtrl.Interacter.CurrentInteractable;

            // Waiting interaction is null or has no valid item to use on.
            if (currentInteractable?.ValidItems == null)
                return false;

            bool validItem = false;
            for (int i = currentInteractable.ValidItems.Length - 1; i >= 0; --i)
            {
                if (currentInteractable.ValidItems[i] == Item.Datas.Id)
                {
                    validItem = true;
                    break;
                }
            }
            
            return ApplyNegation(validItem);
        }
    }
}