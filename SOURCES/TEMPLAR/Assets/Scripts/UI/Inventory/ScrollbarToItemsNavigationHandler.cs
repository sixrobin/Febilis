namespace Templar.UI.Inventory
{
    using UnityEngine;

    public class ScrollbarToItemsNavigationHandler : UnityEngine.UI.Selectable
    {
        [SerializeField] private InventoryView _inventoryView = null;

        public override void OnSelect(UnityEngine.EventSystems.BaseEventData eventData)
        {
            base.OnSelect(eventData);

            // When going left to the scrollbar, this gameObject is selected and acts as a bridge between scrollbar
            // selection and slot selection, just so that we can select the most accurate slot by selecting it from code.
            Navigation.UINavigationManager.Select(_inventoryView.GetClosestSlotToScrollHandle());
        }

        public override void OnDeselect(UnityEngine.EventSystems.BaseEventData eventData)
        {
            base.OnDeselect(eventData);
        }
    }
}