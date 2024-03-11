namespace RSLib.Editor
{
    using UnityEditor;

    public static class OpenPersistentDataPathMenu
    {
        [MenuItem("RSLib/Open Persistent Data Path")]
        private static void OpenPersistentDataPath()
        {
            System.Diagnostics.Process.Start($@"{UnityEngine.Application.persistentDataPath}");
        }
    }
}