namespace RSLib.EditorUtilities
{
#if UNITY_EDITOR
    using UnityEditor.Experimental.SceneManagement;
    using UnityEditor.SceneManagement;
#endif

    public static class PrefabEditorUtilities
    {
        public static void SetCurrentPrefabStageDirty()
        {
#if UNITY_EDITOR
            PrefabStage prefabStage = PrefabStageUtility.GetCurrentPrefabStage();
            if (prefabStage != null)
                EditorSceneManager.MarkSceneDirty(prefabStage.scene);
#endif
        }
    }
}