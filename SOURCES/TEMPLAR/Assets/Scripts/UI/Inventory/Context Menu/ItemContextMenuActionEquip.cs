namespace Templar.UI.Inventory.ContextMenu
{
    using UnityEngine;

    [DisallowMultipleComponent]
    public class ItemContextMenuActionEquip : ItemContextMenuAction
    {
        protected override bool IsActionAllowed()
        {
            CProLogger.LogWarning(this, "NOT IMPLEMENTED");
            return false;

            return Slot.Item.Datas.Type == Item.ItemType.EQUIPABLE
                && Slot.Item.EquipConditionsChecker.CheckConditions();
        }

        protected override void TriggerAction()
        {
            Debug.Log($"Equiping {Slot.Item.Id}.");
        }
    }
}