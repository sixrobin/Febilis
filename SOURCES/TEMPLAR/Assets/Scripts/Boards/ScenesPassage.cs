namespace Templar.Boards
{
    using UnityEngine;
#if UNITY_EDITOR
    using UnityEditor;
#endif

    public class ScenesPassage : ScriptableObject
    {
        [SerializeField] private ScenesPassage _targetPassage;
        [SerializeField] private DisabledString _targetSceneName;

        private ScenesPassagesHandler _passagesHandlerContainer;

        public void Init(ScenesPassagesHandler container, int subAssetIndex)
        {
            name = $"Passage {subAssetIndex}";
            SetContainer(container);
        }

        public ScenesPassage TargetPassage => _targetPassage;

        public void SetContainer(ScenesPassagesHandler container)
        {
            _passagesHandlerContainer = container;
        }

        public void Delete()
        {
            _passagesHandlerContainer.DeleteSubAsset(this);
        }

        private void UpdateTargetSceneName()
        {
            _targetSceneName = new DisabledString(TargetPassage?._passagesHandlerContainer?.name ?? string.Empty);
        }

        private void OnValidate()
        {
            UpdateTargetSceneName();
        }
    }

#if UNITY_EDITOR
    [CustomEditor(typeof(ScenesPassage))]
    public class ScenePassageEditor : RSLib.EditorUtilities.ButtonProviderEditor<ScenesPassage>
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