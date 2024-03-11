namespace RSLib.Framework
{
    using UnityEngine;
#if UNITY_EDITOR
    using UnityEditor;
#endif

    /// <summary>
    /// Requires Unity 2020.1+ because generic types serialization is not possible in older versions.
    /// </summary>
    [System.Serializable]
    public struct Disabled<T>
    {
        [SerializeField] private T _value;

        public Disabled(T initValue)
        {
            _value = initValue;
        }

        public T Value => _value;
    }

#if UNITY_EDITOR
    [CustomPropertyDrawer(typeof(Disabled<>))]
    public class DisabledPropertyDrawer : PropertyDrawer
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