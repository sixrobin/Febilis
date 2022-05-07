namespace Templar.Manager
{
    using RSLib;
    using RSLib.Extensions;
    using UnityEngine;

    /// <summary>
    /// Manages the loot spawn but has nothing to do with the actual currency count and management.
    /// Can be used to spawn loot, split loot between different prefabs (like coins, gems, etc.).
    /// </summary>
    public class LootManager : RSLib.Framework.ConsoleProSingleton<LootManager>
    {
        [SerializeField] private GameObject _coinPrefab = null;
        [SerializeField] private GameObject _itemPrefab = null;

        [Header("DEBUG")]
        [SerializeField] private bool _forceLootChance = false;
        [SerializeField] private bool _spawnCoinsOnClickMode = false;
        [SerializeField] private string _spawnItemOnClickMode = string.Empty;

        private static System.Collections.Generic.List<GameObject> s_waitingObjects = new System.Collections.Generic.List<GameObject>();

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
                s_waitingObjects.Add(coinInstance);
            }
        }
        
        public static void SpawnItem(string itemId, Vector3 pos)
        {
            GameObject itemInstance = RSLib.Framework.Pooling.Pool.Get(Instance._itemPrefab, itemId);
            itemInstance.transform.position = pos;
            s_waitingObjects.Add(itemInstance);
        }

        public static void DisableWaitingObjects()
        {
            if (s_waitingObjects.Count == 0)
                return;

            Instance.Log($"Disabling {s_waitingObjects.Count} waiting coin(s).");

            for (int i = s_waitingObjects.Count - 1; i >= 0; --i)
                if (s_waitingObjects[i] != null)
                    s_waitingObjects[i].SetActive(false);

            s_waitingObjects.Clear();
        }

        private void OnCoinDisabled(CoinController coin)
        {
            UnityEngine.Assertions.Assert.IsTrue(
                s_waitingObjects.Contains(coin.gameObject),
                $"Coin {coin.transform.name} has been picked up but {GetType().Name} has not recorded it when spawning from pool.");

            s_waitingObjects.Remove(coin.gameObject);
        }

        private void OnItemPickedUp(ItemWorldController item)
        {
            UnityEngine.Assertions.Assert.IsTrue(
                s_waitingObjects.Contains(item.gameObject),
                $"Item {item.transform.name} has been picked up but {GetType().Name} has not recorded it when spawning from pool.");

            s_waitingObjects.Remove(item.gameObject);
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
}