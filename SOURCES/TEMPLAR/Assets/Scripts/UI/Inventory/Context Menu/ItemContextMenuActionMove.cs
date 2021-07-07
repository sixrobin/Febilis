namespace Templar.UI.Inventory.ContextMenu
{
    using UnityEngine;

    public class ItemContextMenuActionMove : ItemContextMenuAction
    {
        protected override bool IsActionAllowed()
        {
            CProLogger.LogWarning(this, "NOT IMPLEMENTED");
            return false;

            return Slot.Item.MoveConditionsChecker.CheckConditions();
        }

        protected override void TriggerAction()
        {
            Debug.Log($"Moving {Slot.Item.Id}.");
        }
    }
}