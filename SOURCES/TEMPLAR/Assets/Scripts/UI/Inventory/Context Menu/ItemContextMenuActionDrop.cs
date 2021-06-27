namespace Templar.UI.Inventory.ContextMenu
{
    using UnityEngine;

    [DisallowMultipleComponent]
    public class ItemContextMenuActionDrop : ItemContextMenuAction
    {
        protected override bool IsActionAllowed()
        {
            return !Slot.Item.Datas.AlwaysInInventory; // && !"IsQuestItem"
        }

        protected override void TriggerAction()
        {
            Debug.Log($"Dropping {Slot.Item.Id}.");
        }
    }
}