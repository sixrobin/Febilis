namespace Templar.UI.Inventory.ContextMenu
{
    using UnityEngine;

    public class ItemContextMenuActionEquip : ItemContextMenuAction
    {
        protected override bool IsActionAllowed()
        {
            throw new System.NotImplementedException($"{GetType().Name} NOT IMPLEMENTED");

            return Slot.Item.Datas.Type == Item.ItemType.EQUIPABLE
                && Slot.Item.EquipConditionsChecker.CheckConditions();
        }

        protected override void TriggerAction()
        {
            Debug.Log($"Equiping {Slot.Item.Datas.Id}.");
        }
    }
}