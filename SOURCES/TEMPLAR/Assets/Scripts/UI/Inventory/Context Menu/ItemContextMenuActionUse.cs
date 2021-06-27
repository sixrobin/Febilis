namespace Templar.UI.Inventory.ContextMenu
{
    using UnityEngine;

    [DisallowMultipleComponent]
    public class ItemContextMenuActionUse : ItemContextMenuAction
    {
        protected override bool IsActionAllowed()
        {
            return true;
        }

        protected override void TriggerAction()
        {
            Debug.Log($"Using {Slot.Item.Id}.");
        }
    }
}