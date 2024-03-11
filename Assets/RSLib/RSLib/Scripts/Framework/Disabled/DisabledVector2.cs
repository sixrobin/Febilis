namespace RSLib.Framework
{
    using UnityEngine;
#if UNITY_EDITOR
    using UnityEditor;
#endif

    [System.Serializable]
    public struct DisabledVector2
    {
        [SerializeField] private Vector2 _value;

        public DisabledVector2(Vector2 initValue)
        {
            _value = initValue;
        }

        public Vector2 Value => _value;
    }

#if UNITY_EDITOR
    [CustomPropertyDrawer(typeof(DisabledVector2))]
    public class DisabledVector2PropertyDrawer : PropertyDrawer
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