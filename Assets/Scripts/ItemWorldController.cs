namespace Templar
{
    using UnityEngine;

    public class ItemWorldController : LootItemPhysicsController
    {
        [SerializeField] private Interaction.ItemCollectableController _itemCollectableCtrl = null;

        public delegate void ItemPickedUpEventHandler(ItemWorldController item);
        public static ItemPickedUpEventHandler ItemPickedUp;

        public Interaction.ItemCollectableController ItemCollectableController => _itemCollectableCtrl;
        
        protected override void OnLoot()
        {
            ItemPickedUp?.Invoke(this);
        }

        public override void OnGetFromPool(params object[] args)
        {
            base.OnGetFromPool(args);
            ItemCollectableController.SetItemId(args[0].ToString());
        }

        private void OnItemInteracted(Interaction.Interactable.InteractionEventArgs args)
        {
            OnLoot();
        }
        
        private void OnEnable()
        {
            ItemCollectableController.Interacted += OnItemInteracted;
        }

        private void OnDisable()
        {
            ItemCollectableController.Interacted -= OnItemInteracted;
        }
    }
}