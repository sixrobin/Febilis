namespace Templar.Tools
{
    using UnityEngine;
#if UNITY_EDITOR
    using UnityEditor;
    using Templar.Physics.Triggerables;
#endif

    [System.Serializable]
    public struct OptionalTriggerableObject
    {
        [SerializeField] private TriggerableObject _value;
        [SerializeField] private bool _enabled;

        public OptionalTriggerableObject(TriggerableObject initValue)
        {
            _value = initValue;
            _enabled = true;
        }

        public OptionalTriggerableObject(TriggerableObject initValue, bool initEnabled)
        {
            _value = initValue;
            _enabled = initEnabled;
        }

        public TriggerableObject Value => _value;
        public bool Enabled => _enabled;

        public void SetEnabled(bool state)
        {
            _enabled = state;
        }
    }

#if UNITY_EDITOR
    [CustomPropertyDrawer(typeof(OptionalTriggerableObject))]
    public class OptionalTriggerableObjectPropertyDrawer : PropertyDrawer
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