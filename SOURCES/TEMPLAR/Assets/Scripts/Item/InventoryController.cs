namespace Templar.Item
{
    using System.Linq;
    using UnityEngine;
#if UNITY_EDITOR
    using UnityEditor;
#endif

    [DisallowMultipleComponent]
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
        public delegate void InventoryClearedEventHandler();
        
        public event InventoryContentChangedEventHandler InventoryContentChanged;
        public event InventoryClearedEventHandler InventoryCleared;

        public System.Collections.Generic.Dictionary<string, int> Items = new System.Collections.Generic.Dictionary<string, int>();

        public void Load()
        {
            // [TODO] Handle new game and loading cases.

            // New game : generate native inventory.
            foreach (System.Collections.Generic.KeyValuePair<string, int> nativeItem in Database.ItemDatabase.NativeInventoryItems)
            {
                AddItem(nativeItem.Key, nativeItem.Value);
            }
        }

        public void AddItem(string id)
        {
            AddItem(id, 1);
        }

        public void AddItem(string id, int quantity)
        {
            if (!Items.TryGetValue(id, out int previousQuantity))
                Items.Add(id, 0);

            Items[id] += quantity;

            CProLogger.Log(this, $"Added {quantity} {id}(s) to inventory, now has {Items[id]} copy(ies) of it.");
            InventoryContentChanged?.Invoke(new InventoryContentChangedEventArgs(id, previousQuantity, Items[id]));
        }

        public void RemoveItem(string id)
        {
            RemoveItem(id, 1);
        }

        public void RemoveItem(string id, int quantity)
        {
            if (!Items.TryGetValue(id, out int previousQuantity))
            {
                CProLogger.LogError(this, $"Trying to remove item {id} from the inventory though no instance is owned.", gameObject);
                return;
            }

            UnityEngine.Assertions.Assert.IsTrue(previousQuantity >= quantity, $"Trying to remove more {id}s that the quantity owned.");

            Items[id] -= quantity;
            if (Items[id] == 0)
                Items.Remove(id);

            CProLogger.Log(this, $"Removed {quantity} {id}(s) to inventory, now has {(Items.ContainsKey(id) ? Items[id] : 0)} copy(ies) of it.");
            InventoryContentChanged?.Invoke(new InventoryContentChangedEventArgs(id, previousQuantity, Items.ContainsKey(id) ? Items[id] : 0));
        }

        public void Clear()
        {
            Items.Clear();
            InventoryCleared?.Invoke();
        }

        public void LogInventoryState()
        {
            if (Items.Count == 0)
            {
                CProLogger.Log(this, "Inventory is empty.", gameObject);
                return;
            }

            string log = "Inventory Content:";
            Items.ToList().ForEach(o => log += $"\n- {o.Value} {o.Key}(s)");
            CProLogger.Log(this, log, gameObject);
        }

        private void Awake()
        {
            RSLib.Debug.Console.DebugConsole.OverrideCommand(new RSLib.Debug.Console.Command<string>("AddItem", "Adds an item copy to the inventory.", AddItem));
            RSLib.Debug.Console.DebugConsole.OverrideCommand(new RSLib.Debug.Console.Command<string, int>("AddItem", "Adds item(s) copy(ies) to the inventory.", AddItem));
            RSLib.Debug.Console.DebugConsole.OverrideCommand(new RSLib.Debug.Console.Command<string>("RemoveItem", "Remove an item copy from the inventory.", RemoveItem));
            RSLib.Debug.Console.DebugConsole.OverrideCommand(new RSLib.Debug.Console.Command<string, int>("RemoveItem", "Remove item(s) copy(ies) from the inventory.", RemoveItem));
            RSLib.Debug.Console.DebugConsole.OverrideCommand(new RSLib.Debug.Console.Command("ClearInventory", "Clears the inventory.", Clear));
            RSLib.Debug.Console.DebugConsole.OverrideCommand(new RSLib.Debug.Console.Command("LogInventoryState", "Logs the inventory state to the Unity console.", LogInventoryState));
        }
    }

#if UNITY_EDITOR
    [CustomEditor(typeof(InventoryController))]
    public class InventoryControllerEditor : RSLib.EditorUtilities.ButtonProviderEditor<InventoryController>
    {
        protected override void DrawButtons()
        {
            DrawButton("Log Inventory State", Obj.LogInventoryState);

            if (UnityEditor.EditorApplication.isPlaying)
                DrawButton("Clear Inventory Content", Obj.Clear);
        }
    }
#endif
}