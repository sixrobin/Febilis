namespace RSLib.Framework
{
    using UnityEngine;
#if UNITY_EDITOR
    using UnityEditor;
#endif

    [System.Serializable]
    public struct DisabledGameObject
    {
        [SerializeField] private GameObject _value;

        public DisabledGameObject(GameObject initValue)
        {
            _value = initValue;
        }

        public GameObject Value => _value;
    }

#if UNITY_EDITOR
    [CustomPropertyDrawer(typeof(DisabledGameObject))]
    public class DisabledGameObjectPropertyDrawer : PropertyDrawer
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