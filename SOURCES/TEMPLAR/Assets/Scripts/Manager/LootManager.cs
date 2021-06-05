namespace Templar.Manager
{
    using RSLib.Extensions;
    using UnityEngine;

    /// <summary>
    /// Manages the loot spawn but has nothing to do with the actual currency count and management.
    /// Can be used to spawn loot, split loot between different prefabs (like coins, gems, etc.).
    /// </summary>
    public class LootManager : RSLib.Framework.ConsoleProSingleton<LootManager>
    {
        [SerializeField] private GameObject _coinPrefab = null;

        [Header("DEBUG")]
        [SerializeField] private bool _forceLootChance = false;
        [SerializeField] private bool _spawnCoinsOnClickMode = false;

        private static System.Collections.Generic.List<GameObject> s_waitingCoins = new System.Collections.Generic.List<GameObject>();

        public static void SpawnLoot(Datas.LootDatas lootDatas, Vector3 pos)
        {
            float value = Random.value;
            Instance.Log($"Trying to spawn loot ({(Instance._forceLootChance ? "forced from debug" : $"chances: {lootDatas.Chance * 100}% / drop: {value * 100f}")}).");

            if (!Instance._forceLootChance && value > lootDatas.Chance)
                return;

            SpawnCoins(lootDatas.Value, pos);
        }

        public static void SpawnCoins(int count, Vector3 pos)
        {
            for (int i = 0; i < count; ++i)
            {
                GameObject coinInstance = RSLib.Framework.Pooling.Pool.Get(Instance._coinPrefab);
                coinInstance.transform.position = pos;
                s_waitingCoins.Add(coinInstance);
            }
        }

        public static void DisableWaitingCoins()
        {
            if (s_waitingCoins.Count == 0)
                return;

            Instance.Log($"Disabling {s_waitingCoins.Count} waiting coin(s).");

            for (int i = s_waitingCoins.Count - 1; i >= 0; --i)
                s_waitingCoins[i].SetActive(false);

            s_waitingCoins.Clear();
        }

        private void OnCoinDisabled(CoinController coin)
        {
            UnityEngine.Assertions.Assert.IsTrue(
                s_waitingCoins.Contains(coin.gameObject),
                $"Coin {coin.transform.name} has been picked up but {GetType().Name} has not recorded it when spawning from coins pool.");

            s_waitingCoins.Remove(coin.gameObject);
        }

        protected override void Awake()
        {
            base.Awake();

            CoinController.CoinDisabled += OnCoinDisabled;

            RSLib.Debug.Console.DebugConsole.OverrideCommand(new RSLib.Debug.Console.Command<bool>("LootForceChance", "Forces every random loot to happen.", (state) => _forceLootChance = state));
            RSLib.Debug.Console.DebugConsole.OverrideCommand(new RSLib.Debug.Console.Command("ToggleCoinsOnClick", "Spawns coins on click position.", () => _spawnCoinsOnClickMode = !_spawnCoinsOnClickMode));
        }

        private void Update()
        {
            if (_spawnCoinsOnClickMode && Input.GetMouseButtonDown(0))
                SpawnCoins(Random.Range(3, 5), Camera.main.ScreenToWorldPoint(Input.mousePosition).WithZ(0f));
        }

        private void OnDestroy()
        {
            CoinController.CoinDisabled -= OnCoinDisabled;
            DisableWaitingCoins();
        }
    }
}