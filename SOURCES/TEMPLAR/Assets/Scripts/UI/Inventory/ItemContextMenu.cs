namespace Templar.UI.Inventory
{
    using UnityEngine;

    public class ItemContextMenu : MonoBehaviour
    {
        [SerializeField] private Canvas _canvas;

        public void Open(InventorySlot slot)
        {
            CProLogger.Log(this, $"Opening Item context menu for item {slot.Item.Id}.");
        }
    }
}