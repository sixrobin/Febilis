namespace Templar.UI.Inventory.ContextMenu
{
    public class ItemContextMenuActionUse : ItemContextMenuAction
    {
        protected override bool IsActionAllowed()
        {
            return Slot.Item.UseConditionsChecker.CheckConditions();
        }

        protected override void TriggerAction()
        {
            CProLogger.Log(this, $"Using {Slot.Item.Datas.Id}.");

            // TODO: This is hardcoded, we might want xml tags like <Heal/> or something, probably.
            // Or a Use() method in items, or a <UseCallback> tag.            

            if (Slot.Item.Datas.Id == Item.InventoryController.ITEM_ID_POTION)
            {
                _contextMenu.CloseAtEndOfFrame(); // Closes all inventory panel.
                Manager.GameManager.PlayerCtrl.TriggerHeal();
            }
        }
        
        public override void Localize()
        {
            Button.SetText(Localizer.Get(Localization.Item.ACTION_USE));
        }
    }
}