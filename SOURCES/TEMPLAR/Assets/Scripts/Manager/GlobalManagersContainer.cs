namespace Templar.Manager
{
    /// <summary>
    /// Empty sealed class only used not to destroy the gameObject it is attached to when leaving a scene.
    /// </summary>
    [UnityEngine.DisallowMultipleComponent]
    public sealed class GlobalManagersContainer : UnityEngine.MonoBehaviour
    {
        // Instance does not need to be public as it's only meant to detect other instances, not being called by other types.
        private static GlobalManagersContainer s_instance;

        private void Awake()
        {
            if (s_instance == null)
                s_instance = this;

            if (s_instance != this)
            {
                if (s_instance.gameObject == gameObject)
                    DestroyImmediate(this);
                else
                    DestroyImmediate(gameObject);
            }
            else
            {
                transform.SetParent(null);
                DontDestroyOnLoad(gameObject);
            }
        }
    }

#if UNITY_EDITOR
    [UnityEditor.CustomEditor(typeof(GlobalManagersContainer))]
    public class GlobalManagersContainerEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            UnityEditor.EditorGUILayout.HelpBox(
                "Only used not to destroy the global managers of the game when going through the scenes ; " +
                "this object won't be destroyed on load, so managers that should also not be destroyed can be a child of this container." +
                "\nClass should stay empty and cannot be inherited.",
                UnityEditor.MessageType.Info);
        }
    }
#endif
}