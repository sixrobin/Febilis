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
            Slot.BeginMoveSlot();

            _contextMenu.OnBackButtonPressed();
        }
    }
}