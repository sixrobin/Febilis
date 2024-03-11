namespace RSLib
{
    /// <summary>
    /// Empty sealed class only used not to destroy the gameObject it is attached to when leaving a scene.
    /// </summary>
    [UnityEngine.DisallowMultipleComponent]
    public sealed class DontDestroyOnLoad : UnityEngine.MonoBehaviour
    {
        private void Awake()
        {
            DontDestroyOnLoad(gameObject);
        }
    }

#if UNITY_EDITOR
    [UnityEditor.CustomEditor(typeof(DontDestroyOnLoad))]
    public class DontDestroyOnLoadEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            UnityEditor.EditorGUILayout.HelpBox(
                "Empty class only used not to destroy the gameObject it is attached to when leaving a scene. Cannot be inherited.",
                UnityEditor.MessageType.Info);
        }
    }
#endif
}