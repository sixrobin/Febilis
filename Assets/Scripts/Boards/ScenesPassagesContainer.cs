namespace Templar.Boards
{
    using UnityEngine;
#if UNITY_EDITOR
    using UnityEditor;
#endif

    [CreateAssetMenu(fileName = "New Scenes Passages Handler", menuName = "Boards Transitions/Scene Passages")]
    public class ScenesPassagesContainer : ScriptableObject
    {
        [SerializeField] private RSLib.Framework.SceneField _scene = null;
        [SerializeField] private DisabledScenesPassageList _passages = new DisabledScenesPassageList(new System.Collections.Generic.List<ScenesPassage>());

        public RSLib.Framework.SceneField Scene => _scene;

#if UNITY_EDITOR
        public void CreatePassageSubAsset()
        {
            ScenesPassage subAsset = CreateInstance<ScenesPassage>();
            subAsset.Init(this);
            _passages.Value.Add(subAsset);

            AssetDatabase.AddObjectToAsset(subAsset, AssetDatabase.GetAssetPath(this));
            AssetDatabase.SaveAssets();
        }

        public void ClearPassages()
        {
            for (int i = _passages.Value.Count - 1; i >= 0; --i)
                DestroyImmediate(_passages.Value[i], true);

            _passages.Value.Clear();

            AssetDatabase.SaveAssets();
        }

        public void DeleteSubAsset(ScenesPassage subAsset)
        {
            _passages.Value.Remove(subAsset);
            DestroyImmediate(subAsset, true);

            AssetDatabase.SaveAssets();
        }

        private void Awake()
        {
            // Workaround to make asset duplication work.
            string assetPath = AssetDatabase.GetAssetPath(this);
            name = System.IO.Path.GetFileNameWithoutExtension(assetPath);

            for (int i = _passages.Value.Count - 1; i >= 0; --i)
                _passages.Value[i].SetContainer(this);
        }
#endif
    }

#if UNITY_EDITOR
    [CustomEditor(typeof(ScenesPassagesContainer))]
    public class ScenesPassagesHandlerEditor : RSLib.EditorUtilities.ButtonProviderEditor<ScenesPassagesContainer>
    {
        public override void OnInspectorGUI()
        {
            EditorGUILayout.HelpBox("To rename sub assets, go to Assets > Sub Assets Editor. Datas might refresh strangely but it provides a \"Rename\" option.\n" +
                "To delete a sub asset, select it and click on the \"Delete\" option.", MessageType.Info);
            EditorGUILayout.HelpBox("List is disabled because manipulating it directly would lead to issues with sub assets management.", MessageType.Info);

            EditorGUILayout.Space(10f);

            base.OnInspectorGUI();
        }

        protected override void DrawButtons()
        {
            DrawButton("Create Passage (create sub asset)", Obj.CreatePassageSubAsset);
            DrawButton("Clear Passages (destroy sub assets)", Obj.ClearPassages);
        }
    }
#endif

    [System.Serializable]
    public struct DisabledScenesPassageList
    {
        [SerializeField] private System.Collections.Generic.List<ScenesPassage> _value;

        public DisabledScenesPassageList(System.Collections.Generic.List<ScenesPassage> initValue)
        {
            _value = initValue;
        }

        public System.Collections.Generic.List<ScenesPassage> Value => _value;
    }

#if UNITY_EDITOR
    [CustomPropertyDrawer(typeof(DisabledScenesPassageList))]
    public class DisabledScenesPassageListPropertyDrawer : PropertyDrawer
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