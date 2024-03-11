namespace RSLib.Framework
{
    using UnityEngine;
#if UNITY_EDITOR
    using UnityEditor;
#endif

    [System.Serializable]
    public struct OptionalBoxCollider2D
    {
        [SerializeField] private BoxCollider2D _value;
        [SerializeField] private bool _enabled;

        public OptionalBoxCollider2D(BoxCollider2D initValue)
        {
            _value = initValue;
            _enabled = true;
        }

        public OptionalBoxCollider2D(BoxCollider2D initValue, bool initEnabled)
        {
            _value = initValue;
            _enabled = initEnabled;
        }

        public BoxCollider2D Value => _value;
        public bool Enabled => _enabled;
    }

#if UNITY_EDITOR
    [CustomPropertyDrawer(typeof(OptionalBoxCollider2D))]
    public class OptionalBoxCollider2DPropertyDrawer : PropertyDrawer
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