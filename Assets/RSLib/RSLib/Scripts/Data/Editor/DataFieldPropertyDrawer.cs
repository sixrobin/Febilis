namespace RSLib.Data.Editor
{
    using UnityEngine;
    using UnityEditor;
    
    public abstract class DataFieldPropertyDrawer : PropertyDrawer
    {
        protected abstract string DataFieldName { get; }
        protected abstract string ValueFieldName { get; }
        
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            SerializedProperty valueProperty = property.FindPropertyRelative(DataFieldName);
            return EditorGUI.GetPropertyHeight(valueProperty);
        }
        
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            SerializedProperty useDataFloatProperty = property.FindPropertyRelative("_useDataValue");
            SerializedProperty valueFloatProperty = property.FindPropertyRelative(ValueFieldName);
            SerializedProperty dataFloatProperty = property.FindPropertyRelative(DataFieldName);

            position.width -= 24;

            EditorGUI.PropertyField(position, useDataFloatProperty.boolValue ? dataFloatProperty : valueFloatProperty, label, true);
            
            position.x += position.width + 24;
            position.width = EditorGUI.GetPropertyHeight(useDataFloatProperty);
            position.height = position.width;
            position.x -= position.width;
            
            EditorGUI.PropertyField(position, useDataFloatProperty, GUIContent.none);
        }
    }
}
