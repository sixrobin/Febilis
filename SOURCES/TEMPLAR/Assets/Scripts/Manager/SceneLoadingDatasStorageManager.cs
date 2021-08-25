namespace Templar.Manager
{
    using SceneLoadingDatasStorage;
    using System.Linq;

    public class SceneLoadingDatasStorageManager : RSLib.Framework.ConsoleProSingleton<SceneLoadingDatasStorageManager>
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
                DialogueStructuresBySpeaker = FindObjectsOfType<Interaction.Dialogue.NPCController>().ToDictionary(o => o.SpeakerId, o => o.SaveDatasBeforeSceneLoading())
            };
        }

        public static void TryLoadDatas()
        {
            if (Instance._sceneLoadDatasContainer == null)
                return;

            Instance.Log($"Loading datas after scene loading...");

            GameManager.PlayerCtrl.LoadDatasAfterSceneLoading(Instance._sceneLoadDatasContainer.Value.Player);
            GameManager.InventoryCtrl.LoadDatasAfterSceneLoading(Instance._sceneLoadDatasContainer.Value.Inventory);

            foreach (Interaction.Dialogue.NPCController npc in FindObjectsOfType<Interaction.Dialogue.NPCController>())
                if (Instance._sceneLoadDatasContainer.Value.DialogueStructuresBySpeaker.TryGetValue(npc.SpeakerId, out SceneLoadDatasDialogueStructure dialogueStructure))
                    npc.LoadDatasAfterSceneLoading(dialogueStructure);

            // [TODO] Cleaning up will lose dialogue structures of NPC that are NOT in the current scene.
            // We need to save them somewhere, in DialogueManager probably.

            Cleanup();
        }
    }
}