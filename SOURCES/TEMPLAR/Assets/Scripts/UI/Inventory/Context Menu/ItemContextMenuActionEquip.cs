namespace Templar.UI.Inventory.ContextMenu
{
    using UnityEngine;

    public class ItemContextMenuActionEquip : ItemContextMenuAction
    {
        protected override bool IsActionAllowed()
        {
            Debug.LogWarning($"{GetType().Name} NOT IMPLEMENTED");
            return false;
            // return Slot.Item.Datas.Type == Item.ItemType.EQUIPABLE && Slot.Item.EquipConditionsChecker.CheckConditions();
        }

        protected override void TriggerAction()
        {
            Debug.Log($"Equipping {Slot.Item.Datas.Id}.");
        }
        
        public override void Localize()
        {
            Button.SetText(Localizer.Get(Localization.Item.ACTION_EQUIP));
        }
    }
}