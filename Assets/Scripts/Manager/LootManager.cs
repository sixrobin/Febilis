namespace Templar.Manager
{
    using System.Linq;
    using RSLib;
    using RSLib.Extensions;
    using System.Xml.Linq;
    using UnityEngine;

    /// <summary>
    /// Manages the loot spawn but has nothing to do with the actual currency count and management.
    /// Can be used to spawn loot, split loot between different prefabs (like coins, gems, etc.).
    /// </summary>
    public partial class LootManager : RSLib.Framework.SingletonConsolePro<LootManager>
    {
        public struct WorldLootItem
        {
            public string ItemId;
            public Transform ItemPickup;
        }
        
        [SerializeField] private GameObject _coinPrefab = null;
        [SerializeField] private GameObject _itemPrefab = null;

        [Header("DEBUG")]
        [SerializeField] private bool _forceLootChance = false;
        [SerializeField] private bool _spawnCoinsOnClickMode = false;
        [SerializeField] private string _spawnItemOnClickMode = string.Empty;

        private System.Collections.Generic.List<GameObject> _waitingObjects = new System.Collections.Generic.List<GameObject>();
        private System.Collections.Generic.List<WorldLootItem> _waitingItems = new System.Collections.Generic.List<WorldLootItem>();
        
        public static void SpawnLoot(Datas.LootDatas lootData, Vector3 position, float delay = 0f)
        {
            for (int i = lootData.Loots.Length - 1; i >= 0; --i)
            {
                float value = Random.value;

                if (lootData.Loots[i] is Datas.LootDatas.CoinsLoot coinsLoot)
                {
                    Instance.Log($"Trying to spawn {coinsLoot.Value} coin(s) ({(Instance._forceLootChance ? "forced from debug" : $"chances: {coinsLoot.Chance * 100}% / drop: {value * 100f}")}).");
                    if (!Instance._forceLootChance && value > coinsLoot.Chance)
                        return;

                    void SpawnAction() => SpawnCoins(coinsLoot.Value, position);

                    if (delay > 0f)
                        Instance.DoAfter(delay, SpawnAction);
                    else
                        SpawnAction();
                }
                else if (lootData.Loots[i] is Datas.LootDatas.ItemLoot itemLoot)
                {
                    Instance.Log($"Trying to spawn item {itemLoot.ItemId} ({(Instance._forceLootChance ? "forced from debug" : $"chances: {itemLoot.Chance * 100}% / drop: {value * 100f}")}).");
                    if (!Instance._forceLootChance && value > itemLoot.Chance)
                        return;

                    void SpawnAction() => SpawnItem(itemLoot.ItemId, position);

                    if (delay > 0f)
                        Instance.DoAfter(delay, SpawnAction);
                    else
                        SpawnAction();
                }
            }
        }

        public static void SpawnCoins(int count, Vector3 pos)
        {
            for (int i = 0; i < count; ++i)
            {
                GameObject coinInstance = RSLib.Framework.Pooling.Pool.Get(Instance._coinPrefab);
                coinInstance.transform.position = pos;
                Instance._waitingObjects.Add(coinInstance);
            }
        }
        
        public static void SpawnItem(string itemId, Vector3 pos)
        {
            GameObject itemInstance = RSLib.Framework.Pooling.Pool.Get(Instance._itemPrefab, itemId);
            itemInstance.transform.position = pos;
            
            Instance._waitingObjects.Add(itemInstance);
            
            Instance._waitingItems.Add(new WorldLootItem
            {
                ItemId = itemId,
                ItemPickup = itemInstance.transform
            });
        }

        public static void DisableWaitingObjects()
        {
            if (Instance._waitingObjects.Count == 0)
                return;

            Instance.Log($"Disabling {Instance._waitingObjects.Count} waiting coin(s).");

            for (int i = Instance._waitingObjects.Count - 1; i >= 0; --i)
                if (Instance._waitingObjects[i] != null)
                    Instance._waitingObjects[i].SetActive(false);

            Instance._waitingObjects.Clear();
            Instance._waitingItems.Clear();
        }

        private void OnCoinDisabled(CoinController coin)
        {
            UnityEngine.Assertions.Assert.IsTrue(
                _waitingObjects.Contains(coin.gameObject),
                $"Coin {coin.transform.name} has been picked up but {GetType().Name} has not recorded it when spawning from pool.");

            _waitingObjects.Remove(coin.gameObject);
        }

        private void OnItemPickedUp(ItemWorldController item)
        {
            if (_waitingObjects.Contains(item.gameObject))
                _waitingObjects.Remove(item.gameObject);
    
            _waitingItems.RemoveAll(o => o.ItemPickup == item.transform);
        }
        
        protected override void Awake()
        {
            base.Awake();

            CoinController.CoinDisabled += OnCoinDisabled;
            ItemWorldController.ItemPickedUp += OnItemPickedUp;

            RSLib.Debug.Console.DebugConsole.OverrideCommand<bool>("LootForceChance", "Forces every random loot to happen.", state => _forceLootChance = state);
            RSLib.Debug.Console.DebugConsole.OverrideCommand("ToggleCoinsOnClick", "Spawns coins on click position.", () => _spawnCoinsOnClickMode = !_spawnCoinsOnClickMode);
            RSLib.Debug.Console.DebugConsole.OverrideCommand<string>("SetItemOnClick", "Spawns item on click position.", id => _spawnItemOnClickMode = id);
            RSLib.Debug.Console.DebugConsole.OverrideCommand("SetItemOnClickNull", "Cancel item spawn on click position.", () => _spawnItemOnClickMode = string.Empty);
        }

        private void Update()
        {
            if (Input.GetMouseButtonDown(0))
            {
                if (_spawnCoinsOnClickMode)
                    SpawnCoins(Random.Range(3, 5), Camera.main.ScreenToWorldPoint(Input.mousePosition).WithZ(0f));

                if (!string.IsNullOrEmpty(_spawnItemOnClickMode))
                    SpawnItem(_spawnItemOnClickMode, Camera.main.ScreenToWorldPoint(Input.mousePosition).WithZ(0f));
            }
        }

        private void OnDestroy()
        {
            CoinController.CoinDisabled -= OnCoinDisabled;
            ItemWorldController.ItemPickedUp -= OnItemPickedUp;

            DisableWaitingObjects();
        }
    }

    public partial class LootManager : RSLib.Framework.SingletonConsolePro<LootManager>
    {
        public static XElement Save()
        {
            XElement itemsLootElement = new XElement("ItemsLoot");
            
            for (int i = Instance._waitingItems.Count - 1; i >= 0; --i)
            {
                WorldLootItem itemLoot = Instance._waitingItems[i];
                XElement itemLootElement = new XElement("ItemLoot", new XAttribute("ItemId", itemLoot.ItemId));

                Vector3 itemPosition = itemLoot.ItemPickup.position;
                itemLootElement.Add(new XElement("Position",
                                                 new XAttribute("X", itemPosition.x),
                                                 new XAttribute("Y", itemPosition.y)));
                
                // TODO: If there are multiple scenes in the game, need to save the scene each item belongs to.
                
                itemsLootElement.Add(itemLootElement);
            }
            
            return itemsLootElement;
        }

        public static void Load(XElement itemsLootElement = null)
        {
            if (itemsLootElement == null)
                return;

            foreach (XElement itemLootElement in itemsLootElement.Elements("ItemLoot"))
            {
                XAttribute itemIdAttribute = itemLootElement.Attribute("ItemId");
                if (itemIdAttribute == null)
                    continue;

                XElement positionElement = itemLootElement.Element("Position");
                if (positionElement == null)
                    continue;
                
                Vector3 position = new Vector3(positionElement.Attribute("X").ValueToFloat(),
                                               positionElement.Attribute("Y").ValueToFloat());
                
                // TODO: If there are multiple scenes in the game, need to load the scene each item belongs to.
                
                SpawnItem(itemIdAttribute.Value, position);
            }
        }
    }
}