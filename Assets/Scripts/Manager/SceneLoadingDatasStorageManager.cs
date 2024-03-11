namespace Templar.Manager
{
    using SceneLoadingDatasStorage;
    using System.Linq;

    public class SceneLoadingDatasStorageManager : RSLib.Framework.SingletonConsolePro<SceneLoadingDatasStorageManager>
    {
        private SceneLoadDatasContainer? _sceneLoadDatasContainer = null;

        public static void Cleanup()
        {
            Instance._sceneLoadDatasContainer = null;
        }

        public static void StoreDatas()
        {
            Instance.Log($"Storing datas before scene loading...");

            Instance._sceneLoadDatasContainer = new SceneLoadDatasContainer()
            {
                Player = GameManager.PlayerCtrl.SaveDatasBeforeSceneLoading(),
                Inventory = GameManager.InventoryCtrl.SaveDatasBeforeSceneLoading(),
            };
        }

        public static void TryLoadDatas()
        {
            if (Instance._sceneLoadDatasContainer == null)
                return;

            Instance.Log($"Loading datas after scene loading...");

            GameManager.PlayerCtrl.LoadDatasAfterSceneLoading(Instance._sceneLoadDatasContainer.Value.Player);
            GameManager.InventoryCtrl.LoadDatasAfterSceneLoading(Instance._sceneLoadDatasContainer.Value.Inventory);

            Cleanup();
        }
    }
}