namespace RSLib.Framework
{
    using UnityEngine;
#if UNITY_EDITOR
    using UnityEditor;
#endif

    [System.Serializable]
    public struct DisabledBool
    {
        [SerializeField] private bool _value;

        public DisabledBool(bool initValue)
        {
            _value = initValue;
        }

        public bool Value => _value;
    }

#if UNITY_EDITOR
    [CustomPropertyDrawer(typeof(DisabledBool))]
    public class DisabledBoolPropertyDrawer : PropertyDrawer
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