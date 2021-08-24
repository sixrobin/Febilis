namespace Templar.SceneLoadingDatasStorage
{
    public struct SceneLoadDatasContainer
    {
        public SceneLoadDatasPlayer Player;
        public SceneLoadDatasInventory Inventory;
        public System.Collections.Generic.Dictionary<string, SceneLoadDatasDialogueStructure> DialogueStructuresBySpeaker;
    }
}