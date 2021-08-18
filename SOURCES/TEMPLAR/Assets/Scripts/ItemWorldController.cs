namespace Templar
{
    using UnityEngine;

    public class ItemWorldController : LootItemPhysicsController
    {
        [SerializeField] private Interaction.ItemCollectableController _itemCollectableCtrl = null;

        public delegate void ItemPickedUpEventHandler(ItemWorldController item);
        public static ItemPickedUpEventHandler ItemPickedUp;

        protected override void OnLoot()
        {
            ItemPickedUp?.Invoke(this);
        }

        public override void OnGetFromPool(params object[] args)
        {
            base.OnGetFromPool(args);

            _itemCollectableCtrl.SetItemId(args[0].ToString());
        }
    }
}