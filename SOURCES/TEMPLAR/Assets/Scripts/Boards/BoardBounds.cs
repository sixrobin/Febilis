namespace Templar.Boards
{
    using UnityEngine;
#if UNITY_EDITOR
    using UnityEditor;
#endif

    [DisallowMultipleComponent]
    [RequireComponent(typeof(BoxCollider2D))]
    public class BoardBounds : MonoBehaviour
    {
        public BoxCollider2D Bounds { get; private set; }

        private BoundsSwitch[] _switches;
        public BoundsSwitch[] Switches
        {
            get
            {
                if (_switches == null)
                    _switches = GetComponentsInChildren<BoundsSwitch>();

                return _switches;
            }
        }

        private void Awake()
        {
            Bounds = GetComponent<BoxCollider2D>();
            if (Bounds == null)
                CProLogger.LogError(this, $"{GetType().Name} needs an attached BoxCollider2D.", gameObject);

            for (int i = Switches.Length - 1; i >= 0; --i)
                Switches[i].Triggered += OnSwitchTriggered;
        }

        private void OnSwitchTriggered()
        {
            Manager.GameManager.CameraCtrl.SetBoardBounds(this);
        }
    }

#if UNITY_EDITOR
    [CustomEditor(typeof(BoardBounds))]
    public class BoardBoundsEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            EditorGUILayout.HelpBox("Must be attached to the gameObject also having the BoxCollider2D attached that acts as camera bound. " +
                "Triggers that enable this as camera bound must be this component's instance gameObject children.", MessageType.Info);

            EditorGUILayout.Space(10f);

            base.OnInspectorGUI();
        }
    }
#endif


    [System.Serializable]
    public struct OptionalBoardBounds
    {
        [SerializeField] private BoardBounds _value;
        [SerializeField] private bool _enabled;

        public OptionalBoardBounds(BoardBounds initValue)
        {
            _value = initValue;
            _enabled = true;
        }

        public OptionalBoardBounds(BoardBounds initValue, bool initEnabled)
        {
            _value = initValue;
            _enabled = initEnabled;
        }

        public BoardBounds Value => _value;
        public bool Enabled => _enabled;

        public void SetEnabled(bool state)
        {
            _enabled = state;
        }
    }

    [System.Serializable]
    public struct DisabledBoardBounds
    {
        [SerializeField] private BoardBounds _value;

        public DisabledBoardBounds(BoardBounds initValue)
        {
            _value = initValue;
        }

        public BoardBounds Value => _value;
    }

#if UNITY_EDITOR
    [CustomPropertyDrawer(typeof(OptionalBoardBounds))]
    public class OptionalBoardBoundsDrawer : PropertyDrawer
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

    [CustomPropertyDrawer(typeof(DisabledBoardBounds))]
    public class DisabledBoardBoundsPropertyDrawer : PropertyDrawer
    {
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            SerializedProperty valueProperty = property.FindPropertyRelative("_value");
            return EditorGUI.GetPropertyHeight(valueProperty);
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            SerializedProperty valueProperty = property.FindPropertyRelative("_value");

            EditorGUI.BeginDisabledGroup(true);
            EditorGUI.PropertyField(position, valueProperty, label, true);
            EditorGUI.EndDisabledGroup();
        }
    }
#endif
}