namespace Templar.UI.Inventory.ContextMenu
{
    public class ItemContextMenuActionMove : ItemContextMenuAction
    {
        protected override bool IsActionAllowed()
        {
            return Slot.Item.MoveConditionsChecker.CheckConditions();
        }

        protected override void TriggerAction()
        {
            Slot.InventoryView.BeginMoveSlot(Slot);

            _contextMenu.OnBackButtonPressed();
        }
    }
}