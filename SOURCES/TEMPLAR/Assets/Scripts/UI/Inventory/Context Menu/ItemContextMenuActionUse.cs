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
            else if (Slot.Item.Datas.Type == Item.ItemType.KEY)
            {
                UnityEngine.Assertions.Assert.IsTrue(Manager.GameManager.PlayerCtrl.Interacter.CurrentInteractable != null, "Allowing to use a key on a null interactable.");
                _contextMenu.CloseAtEndOfFrame(); // Closes all inventory panel.
                Manager.GameManager.PlayerCtrl.Interacter.TryInteract();
            }
        }
        
        public override void Localize()
        {
            Button.SetText(RSLib.Localization.Localizer.Get(Localization.Item.ACTION_USE));
        }
    }
}