namespace Templar.Tmp
{
    using UnityEngine;
#if UNITY_EDITOR
    using UnityEditor;
#endif

    [CreateAssetMenu(fileName = "New SubAsset", menuName = "Templar/Scene Passage", order = 0)]
    public class ScenePassage : ScriptableObject
    {
        public ScenePassage targetPassage;
        public DisabledString targetSceneName;

        public void Init(ScenePassages container, int subAssetIndex)
        {
            name = $"Passage {subAssetIndex}";
            _container = container;
        }

        private ScenePassages _container;

        public void Delete()
        {
            _container.DeleteSubAsset(this);
        }

        private void OnValidate()
        {
            targetSceneName = new DisabledString(targetPassage?._container.name ?? string.Empty);
        }
    }

#if UNITY_EDITOR
    [CustomEditor(typeof(ScenePassage))]
    public class ScenePassageEditor : RSLib.EditorUtilities.ButtonProviderEditor<ScenePassage>
    {
        protected override void DrawButtons()
        {
            if (GUILayout.Button("Delete"))
                Obj.Delete();
        }
    }



    [System.Serializable]
    public struct DisabledString
    {
        [SerializeField] private string _value;

        public DisabledString(string initValue)
        {
            _value = initValue;
        }

        public string Value => _value;
    }

    [CustomPropertyDrawer(typeof(DisabledString))]
    public class DisabledStringProperty : PropertyDrawer
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