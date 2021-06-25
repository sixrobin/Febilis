namespace Templar.Item
{
    using RSLib.Extensions;
    using System.Linq;
    using System.Xml.Linq;
    using UnityEngine;
#if UNITY_EDITOR
    using UnityEditor;
#endif

    [DisallowMultipleComponent]
    public partial class InventoryController : MonoBehaviour
    {
        public class InventoryContentChangedEventArgs : System.EventArgs
        {
            public InventoryContentChangedEventArgs(Item item, int prevQuantity, int newQuantity)
            {
                Item = item;
                PrevQuantity = prevQuantity;
                NewQuantity = newQuantity;
            }

            public Item Item { get; private set; }
            public int PrevQuantity { get; private set; }
            public int NewQuantity { get; private set; }
        }

        public delegate void InventoryContentChangedEventHandler(InventoryContentChangedEventArgs args);
        public delegate void InventoryClearedEventHandler();
        
        public event InventoryContentChangedEventHandler InventoryContentChanged;
        public event InventoryClearedEventHandler InventoryCleared;

        // List of a custom class instead of <Item, Quantity> to store more infos, like the slot index ?
        // InventorySlot, and change current class of this name to InventorySlotView ?
        public System.Collections.Generic.Dictionary<Item, int> Items = new System.Collections.Generic.Dictionary<Item, int>();

        public int GetItemQuantity(string id)
        {
            if (TryGetOwnedItemKey(id, out Item item))
                return Items[item];

            return 0;
        }

        public void AddItem(string id)
        {
            AddItem(id, 1);
        }

        public void AddItem(string id, int quantity)
        {
            if (!TryGetOwnedItemKey(id, out Item item))
                Items.Add(item = new Item(id), 0);

            int previousQuantity = Items[item];
            Items[item] += quantity;

            CProLogger.Log(this, $"Added {quantity} {id}(s) to inventory, now has {Items[item]} copy(ies) of it.");
            InventoryContentChanged?.Invoke(new InventoryContentChangedEventArgs(item, previousQuantity, Items[item]));
        }

        public void RemoveItem(string id)
        {
            RemoveItem(id, 1);
        }

        public void RemoveItem(string id, int quantity)
        {
            if (!TryGetOwnedItemKey(id, out Item item))
            {
                CProLogger.LogError(this, $"Trying to remove item {id} from the inventory though no copy is owned.", gameObject);
                return;
            }

            int previousQuantity = Items[item];
            UnityEngine.Assertions.Assert.IsTrue(previousQuantity >= quantity, $"Trying to remove more {id}s that the quantity owned.");

            Items[item] -= quantity;
            if (Items[item] == 0 && !item.Datas.AlwaysInInventory)
                Items.Remove(item);

            CProLogger.Log(this, $"Removed {quantity} {id}(s) to inventory, now has {(Items.ContainsKey(item) ? Items[item] : 0)} copy(ies) of it.");
            InventoryContentChanged?.Invoke(new InventoryContentChangedEventArgs(item, previousQuantity, Items.ContainsKey(item) ? Items[item] : 0));
        }

        public void Clear()
        {
            System.Collections.Generic.List<Item> itemsKeys = Items.Keys.ToList();
            foreach (Item item in itemsKeys)
                RemoveItem(item.Id, Items[item]);
        }

        public void ForceClear()
        {
            Items.Clear();
            InventoryCleared.Invoke();
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

        private bool TryGetOwnedItemKey(string id, out Item item)
        {
            return (item = Items.Where(o => o.Key.Id == id).FirstOrDefault().Key) != null;
        }

        private void Awake()
        {
            RSLib.Debug.Console.DebugConsole.OverrideCommand(new RSLib.Debug.Console.Command<string>("AddItem", "Adds an item copy to the inventory.", AddItem));
            RSLib.Debug.Console.DebugConsole.OverrideCommand(new RSLib.Debug.Console.Command<string, int>("AddItem", "Adds item(s) copy(ies) to the inventory.", AddItem));
            RSLib.Debug.Console.DebugConsole.OverrideCommand(new RSLib.Debug.Console.Command<string>("RemoveItem", "Remove an item copy from the inventory.", RemoveItem));
            RSLib.Debug.Console.DebugConsole.OverrideCommand(new RSLib.Debug.Console.Command<string, int>("RemoveItem", "Remove item(s) copy(ies) from the inventory.", RemoveItem));
            RSLib.Debug.Console.DebugConsole.OverrideCommand(new RSLib.Debug.Console.Command("ClearInventory", "Clears the inventory.", Clear));
            RSLib.Debug.Console.DebugConsole.OverrideCommand(new RSLib.Debug.Console.Command("ClearInventoryForced", "Deletes all inventory items.", ForceClear));
            RSLib.Debug.Console.DebugConsole.OverrideCommand(new RSLib.Debug.Console.Command("LogInventoryState", "Logs the inventory state to the Unity console.", LogInventoryState));
            RSLib.Debug.Console.DebugConsole.OverrideCommand(new RSLib.Debug.Console.Command("AddAllItems", $"Adds all items of {Database.ItemDatabase.Instance.GetType().Name} to the inventory.", DebugAddAllItems));
        }

        private void DebugAddAllItems()
        {
            foreach (System.Collections.Generic.KeyValuePair<string, Datas.Item.ItemDatas> item in Database.ItemDatabase.ItemsDatas)
                AddItem(item.Key);
        }
    }

    public partial class InventoryController : MonoBehaviour
    {
        public void Load(XElement inventoryElement = null)
        {
            if (inventoryElement == null)
            {
                foreach (System.Collections.Generic.KeyValuePair<string, int> nativeItem in Database.ItemDatabase.NativeInventoryItems)
                    AddItem(nativeItem.Key, nativeItem.Value);

                return;
            }

            foreach (XElement itemElement in inventoryElement.Elements())
                AddItem(itemElement.Name.LocalName, itemElement.ValueToInt());
        }

        public XElement Save()
        {
            XElement inventoryElement = new XElement("Inventory");

            foreach (System.Collections.Generic.KeyValuePair<Item, int> item in Items)
                inventoryElement.Add(new XElement(item.Key.Id, item.Value));

            return inventoryElement;
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
            {
                DrawButton("Clear Inventory", Obj.Clear);
                DrawButton("Delete All Inventory Items", Obj.ForceClear);
            }
        }
    }
#endif
}