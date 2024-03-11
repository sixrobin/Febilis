namespace RSLib.EditorUtilities
{
    public static class SceneManagerUtilities
    {
        public static void SetCurrentSceneDirty()
        {
#if UNITY_EDITOR
            UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(UnityEngine.SceneManagement.SceneManager.GetActiveScene());
#endif
        }
    }
}