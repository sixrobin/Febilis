namespace Templar.Interaction.Checkpoint
{
    using UnityEngine;
#if UNITY_EDITOR
    using UnityEditor;
#endif

    public class CheckpointController : Interactable
    {
        [Header("REFS")]
        [SerializeField] private CheckpointView _checkpointView = null;
        [SerializeField] private GameObject _highlight = null;

        [Header("CHECKPOINT DATAS")]
        [SerializeField] private string _id = string.Empty;
        [SerializeField] private Vector2 _respawnOffset = Vector2.zero;

        [Header("DEBUG COLOR")]
        [SerializeField] private RSLib.DataColor _debugColor = null;

        private delegate void BeforeCheckpointChangeEventHandler(string currId, string nextId);
        private static BeforeCheckpointChangeEventHandler BeforeCheckpointChange;

        public static string CurrCheckpointId { get; private set; }
        public static CheckpointController CurrCheckpoint { get; private set; }

        public Vector3 RespawnPos => transform.position + (Vector3)_respawnOffset;

        public string Id => _id;

        // [TMP] We might want to keep this for some uses, but for now it's only for debug.
        public static void ForceRemoveCurrentCheckpoint()
        {
            CurrCheckpoint = null;
            CurrCheckpointId = null;
        }

        /// <summary>
        /// Used to set the current checkpoint Id from save file.
        /// Should only be called by a save manager of some sort.
        /// </summary>
        public static void LoadCurrentCheckpointId(string id)
        {
            // [TODO] Check if id is found in the scene to log a warning if not.
            CurrCheckpointId = id;
        }

        public override void Focus()
        {
            base.Focus();
            _highlight.SetActive(true);
        }

        public override void Unfocus()
        {
            base.Unfocus();
            _highlight.SetActive(false);
        }

        public override void Interact()
        {
            base.Interact();

            if (CurrCheckpointId != Id)
            {
                BeforeCheckpointChange(CurrCheckpointId, Id);
                CurrCheckpointId = Id;
                CurrCheckpoint = this;
            }

            Manager.GameManager.PlayerCtrl.AllowInputs(false);
            _checkpointView.PlayInteractedAnimation(() => Manager.GameManager.OnCheckpointInteracted(this));
        }

        private void OnBeforeCheckpointChange(string currId, string nextId)
        {
            UnityEngine.Assertions.Assert.IsFalse(currId == nextId,
                "Checkpoint change event has been called but current Id and next Id are the same.");

            // Turn off last checkpoint if it's in the scene.
            if (currId == Id)
                _checkpointView.PlayOffAnimation();
        }

        private void Start()
        {
            BeforeCheckpointChange += OnBeforeCheckpointChange;

            if (CurrCheckpointId == Id)
            {
                CurrCheckpoint = this;
                _checkpointView.PlayOnAnimation();
            }
        }

        private void OnDestroy()
        {
            BeforeCheckpointChange -= OnBeforeCheckpointChange;
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = _debugColor.Color;
            Gizmos.DrawWireSphere(RespawnPos, 0.2f);
        }
    }

    [System.Serializable]
    public struct OptionalCheckpointController
    {
        [SerializeField] private CheckpointController _value;
        [SerializeField] private bool _enabled;

        public OptionalCheckpointController(CheckpointController initValue)
        {
            _value = initValue;
            _enabled = true;
        }

        public CheckpointController Value => _value;
        public bool Enabled => _enabled;
    }

#if UNITY_EDITOR
    [CustomPropertyDrawer(typeof(OptionalCheckpointController))]
    public class OptionalCheckpointControllerPropertyDrawer : PropertyDrawer
    {
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            SerializedProperty valueProperty = property.FindPropertyRelative("_value");
            return EditorGUI.GetPropertyHeight(valueProperty);
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            SerializedProperty valueProperty = property.FindPropertyRelative("_value");
            SerializedProperty enabledProperty = property.FindPropertyRelative("_enabled");

            position.width -= 24;

            EditorGUI.BeginDisabledGroup(!enabledProperty.boolValue);
            EditorGUI.PropertyField(position, valueProperty, label, true);
            EditorGUI.EndDisabledGroup();

            position.x += position.width + 24;
            position.width = EditorGUI.GetPropertyHeight(enabledProperty);
            position.height = position.width;
            position.x -= position.width;

            EditorGUI.PropertyField(position, enabledProperty, GUIContent.none);
        }
    }
#endif
}