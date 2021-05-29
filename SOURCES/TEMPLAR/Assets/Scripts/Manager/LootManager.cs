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
                Transform coinInstance = RSLib.Framework.Pooling.Pool.Get(Instance._coinPrefab).transform;
                coinInstance.position = pos;
            }
        }

        protected override void Awake()
        {
            base.Awake();

            RSLib.Debug.Console.DebugConsole.OverrideCommand(new RSLib.Debug.Console.Command<bool>("LootForceChance", "Forces every random loot to happen.", (state) => _forceLootChance = state));
            RSLib.Debug.Console.DebugConsole.OverrideCommand(new RSLib.Debug.Console.Command("ToggleCoinsOnClick", "Spawns coins on click position.", () => _spawnCoinsOnClickMode = !_spawnCoinsOnClickMode));
        }

        private void Update()
        {
            if (_spawnCoinsOnClickMode && Input.GetMouseButtonDown(0))
                SpawnCoins(Random.Range(3, 5), Camera.main.ScreenToWorldPoint(Input.mousePosition).WithZ(0f));
        }
    }
}