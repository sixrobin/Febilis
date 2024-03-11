namespace Templar.Tools
{
    using UnityEngine;
    using Templar.Boards;
#if UNITY_EDITOR
    using UnityEditor;
#endif

    [System.Serializable]
    public struct OptionalScenesPassage
    {
        [SerializeField] private ScenesPassage _value;
        [SerializeField] private bool _enabled;

        public OptionalScenesPassage(ScenesPassage initValue)
        {
            _value = initValue;
            _enabled = true;
        }

        public OptionalScenesPassage(ScenesPassage initValue, bool initEnabled)
        {
            _value = initValue;
            _enabled = initEnabled;
        }

        public ScenesPassage Value => _value;
        public bool Enabled => _enabled;

        public void SetEnabled(bool state)
        {
            _enabled = state;
        }
    }

#if UNITY_EDITOR
    [CustomPropertyDrawer(typeof(OptionalScenesPassage))]
    public class OptionalScenesPassagePropertyDrawer : PropertyDrawer
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