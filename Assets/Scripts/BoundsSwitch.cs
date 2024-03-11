namespace Templar.Boards
{
    using UnityEngine;
#if UNITY_EDITOR
    using UnityEditor;
#endif

    [DisallowMultipleComponent]
    [RequireComponent(typeof(Rigidbody2D))]
    public class BoundsSwitch : MonoBehaviour
    {
        public delegate void TriggerEventHandler();
        public event TriggerEventHandler Triggered;

        public void Enable(bool state)
        {
            gameObject.SetActive(state);
        }

        private void Awake()
        {
            if (!GetComponent<Collider2D>())
                CProLogger.LogError(this, $"{GetType().Name} does not have a Collider2D and can then not be triggered.");
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.gameObject.GetComponent<Unit.Player.PlayerController>())
                Triggered?.Invoke();
        }
    }

#if UNITY_EDITOR
    [CustomEditor(typeof(BoundsSwitch))]
    public class BoundsSwitchEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            EditorGUILayout.HelpBox("Must be attached on a collider that triggers a camera bound change. " +
                $"Also must be a child gameObject of a gameObject with a {typeof(BoardBounds).Name} component instance.", MessageType.Info);
            EditorGUILayout.HelpBox("Make sure the layer fits the player detection and that the collider is set as trigger.", MessageType.Info);

            EditorGUILayout.Space(10f);

            base.OnInspectorGUI();
        }
    }
#endif
}