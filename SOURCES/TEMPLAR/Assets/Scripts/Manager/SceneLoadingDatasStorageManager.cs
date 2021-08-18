namespace Templar.Manager
{
    using SceneLoadingDatasStorage;

    public class SceneLoadingDatasStorageManager : RSLib.Framework.ConsoleProSingleton<SceneLoadingDatasStorageManager>
    {
        private SceneLoadDatasContainer? _sceneLoadDatasContainer = null;

        public static void Cleanup()
        {
            Instance._sceneLoadDatasContainer = null;
        }

        public static void StoreDatas()
        {
            Instance._sceneLoadDatasContainer = new SceneLoadDatasContainer()
            {
                Player = GameManager.PlayerCtrl.SaveDatasBeforeSceneLoading(),
                Inventory = GameManager.InventoryCtrl.SaveDatasBeforeSceneLoading()
            };
        }

        public static void TryLoadDatas()
        {
            if (Instance._sceneLoadDatasContainer == null)
                return;

            GameManager.PlayerCtrl.LoadDatasAfterSceneLoading(Instance._sceneLoadDatasContainer.Value.Player);
            GameManager.InventoryCtrl.LoadDatasAfterSceneLoading(Instance._sceneLoadDatasContainer.Value.Inventory);

            Cleanup();
        }
    }
}