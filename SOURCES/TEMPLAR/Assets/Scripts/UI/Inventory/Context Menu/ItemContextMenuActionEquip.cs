namespace Templar.UI.Inventory.ContextMenu
{
    using UnityEngine;

    [DisallowMultipleComponent]
    public class ItemContextMenuActionEquip : ItemContextMenuAction
    {
        protected override bool IsActionAllowed()
        {
            return false;
        }

        protected override void TriggerAction()
        {
            Debug.Log($"Equiping {Slot.Item.Id}.");
        }
    }
}