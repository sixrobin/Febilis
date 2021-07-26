namespace Templar.Boards
{
    using UnityEngine;
#if UNITY_EDITOR
    using UnityEditor;
#endif

    [CreateAssetMenu(fileName = "New Scenes Passages Handler", menuName = "Scene Passages")]
    public class ScenesPassagesHandler : ScriptableObject
    {
        [SerializeField] private RSLib.Framework.SceneField _scene = null;
        [SerializeField] private System.Collections.Generic.List<ScenesPassage> _passages = new System.Collections.Generic.List<ScenesPassage>();

        public RSLib.Framework.SceneField Scene => _scene;

        public void CreatePassageSubAsset()
        {
            ScenesPassage subAsset = CreateInstance<ScenesPassage>();
            subAsset.Init(this, _passages.Count);
            _passages.Add(subAsset);

            AssetDatabase.AddObjectToAsset(subAsset, AssetDatabase.GetAssetPath(this));
            AssetDatabase.SaveAssets();
        }

        public void ClearPassages()
        {
            for (int i = _passages.Count - 1; i >= 0; --i)
                DestroyImmediate(_passages[i], true);

            _passages.Clear();

            AssetDatabase.SaveAssets();
        }

        public void DeleteSubAsset(ScenesPassage subAsset)
        {
            _passages.Remove(subAsset);
            DestroyImmediate(subAsset, true);

            AssetDatabase.SaveAssets();
        }

#if UNITY_EDITOR
        private void Awake()
        {
            // Workaround to make asset duplication work.
            string assetPath = AssetDatabase.GetAssetPath(this);
            name = System.IO.Path.GetFileNameWithoutExtension(assetPath);

            for (int i = _passages.Count - 1; i >= 0; --i)
                _passages[i].SetContainer(this);
        }
#endif
    }

#if UNITY_EDITOR
    [CustomEditor(typeof(ScenesPassagesHandler))]
    public class ScriptableSubAssetEditor : RSLib.EditorUtilities.ButtonProviderEditor<ScenesPassagesHandler>
    {
        protected override void DrawButtons()
        {
            if (GUILayout.Button("Create Passage Sub Asset"))
                Obj.CreatePassageSubAsset();

            if (GUILayout.Button("Clear Passages"))
                Obj.ClearPassages();
        }
    }
#endif
}