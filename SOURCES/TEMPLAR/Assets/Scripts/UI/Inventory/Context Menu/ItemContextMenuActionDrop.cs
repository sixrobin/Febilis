namespace Templar.UI.Inventory.ContextMenu
{
    using UnityEngine;

    [DisallowMultipleComponent]
    public class ItemContextMenuActionDrop : ItemContextMenuAction
    {
        protected override bool IsActionAllowed()
        {
            CProLogger.LogWarning(this, "NOT IMPLEMENTED");
            return false;

            return !Slot.Item.Datas.AlwaysInInventory
                && Slot.Item.DropConditionsChecker.CheckConditions();
                // && !"IsQuestItem"
        }

        protected override void TriggerAction()
        {
            Debug.Log($"Dropping {Slot.Item.Id}.");
        }
    }
}