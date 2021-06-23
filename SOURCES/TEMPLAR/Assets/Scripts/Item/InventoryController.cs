namespace Templar.Item
{
    using UnityEngine;

    public class InventoryController : MonoBehaviour
    {
        public class InventoryContentChangedEventArgs : System.EventArgs
        {
            public InventoryContentChangedEventArgs(string changedItemId, int previousQuantity, int newQuantity)
            {
                ChangedItemId = changedItemId;
                NewQuantity = newQuantity;
            }

            public string ChangedItemId { get; private set; }
            public int PreviousQuantity { get; private set; }
            public int NewQuantity { get; private set; }
        }

        public delegate void InventoryContentChangedEventHandler(InventoryContentChangedEventArgs args);
        public event InventoryContentChangedEventHandler InventoryContentChanged;

        public System.Collections.Generic.Dictionary<string, int> Items = new System.Collections.Generic.Dictionary<string, int>();

        public void AddItem(string id)
        {
            if (!Items.TryGetValue(id, out int previousQuantity))
                Items.Add(id, 0);

            Items[id]++;

            CProLogger.Log(this, $"Added 1 {id} to inventory, now has {Items[id]} instance(s) of it.");
            InventoryContentChanged?.Invoke(new InventoryContentChangedEventArgs(id, previousQuantity, Items[id]));
        }

        public void RemoveItem(string id)
        {
            if (!Items.TryGetValue(id, out int previousQuantity))
            {
                CProLogger.LogError(this, $"Trying to remove item {id} from the inventory though no instance is owned.", gameObject);
                return;
            }

            Items[id]--;
            if (Items[id] == 0)
                Items.Remove(id);

            CProLogger.Log(this, $"Removed 1 {id} to inventory, now has {(Items.ContainsKey(id) ? Items[id] : 0)} instance(s) of it.");
            InventoryContentChanged?.Invoke(new InventoryContentChangedEventArgs(id, previousQuantity, Items.ContainsKey(id) ? Items[id] : 0));
        }

        private void Awake()
        {
            RSLib.Debug.Console.DebugConsole.OverrideCommand(new RSLib.Debug.Console.Command<string>("AddItem", "Adds an item to the inventory.", AddItem));
            RSLib.Debug.Console.DebugConsole.OverrideCommand(new RSLib.Debug.Console.Command<string>("RemoveItem", "Adds an item to the inventory.", RemoveItem));
        }
    }
}