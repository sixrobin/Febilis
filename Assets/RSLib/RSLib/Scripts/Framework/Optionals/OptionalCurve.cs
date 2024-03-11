namespace RSLib.Framework
{
    using UnityEngine;
#if UNITY_EDITOR
    using UnityEditor;
#endif

    [System.Serializable]
    public struct OptionalCurve
    {
        [SerializeField] private Maths.Curve _value;
        [SerializeField] private bool _enabled;

        public OptionalCurve(Maths.Curve initValue)
        {
            _value = initValue;
            _enabled = true;
        }

        public OptionalCurve(Maths.Curve initValue, bool initEnabled)
        {
            _value = initValue;
            _enabled = initEnabled;
        }

        public Maths.Curve Value => _value;
        public bool Enabled => _enabled;
    }

#if UNITY_EDITOR
    [CustomPropertyDrawer(typeof(OptionalCurve))]
    public class OptionalCurvePropertyDrawer : PropertyDrawer
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