namespace Templar.UI.Inventory.ContextMenu
{
    using UnityEngine;

    [DisallowMultipleComponent]
    public class ItemContextMenuActionMove : ItemContextMenuAction
    {
        protected override bool IsActionAllowed()
        {
            return true;
        }

        protected override void TriggerAction()
        {
            Debug.Log($"Moving {Slot.Item.Id}.");
        }
    }
}