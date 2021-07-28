namespace Templar.Tools
{
    /// <summary>
    /// Managers that have references locally in scenes that cannot be applied into their prefab can implement this interface.
    /// It allows the ManagersReferencesFinder class to locate them and call the two methods.
    /// </summary>
    public interface IManagerReferencesHandler
    {
        UnityEngine.GameObject PrefabInstanceRoot { get; }

        void DebugFindAllReferences();
        void DebugFindMissingReferences();
    }
}