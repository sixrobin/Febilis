namespace RSLib.Framework
{
    using UnityEngine;
#if UNITY_EDITOR
    using UnityEditor;
#endif

    [System.Serializable]
    public struct OptionalFloat
    {
        [SerializeField] private float _value;
        [SerializeField] private bool _enabled;

        public OptionalFloat(float initValue)
        {
            _value = initValue;
            _enabled = true;
        }

        public OptionalFloat(float initValue, bool initEnabled)
        {
            _value = initValue;
            _enabled = initEnabled;
        }

        public float Value => _value;
        public bool Enabled => _enabled;
    }

#if UNITY_EDITOR
    [CustomPropertyDrawer(typeof(OptionalFloat))]
    public class OptionalFloatPropertyDrawer : PropertyDrawer
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