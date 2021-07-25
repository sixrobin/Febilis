namespace Templar.Tmp
{
    using System.Collections.Generic;
    using UnityEngine;
#if UNITY_EDITOR
    using UnityEditor;
    using System.Linq;
#endif

    [CreateAssetMenu(fileName = "New Test", menuName = "Templar/Scene Passages", order = 0)]
    public class ScenePassages : ScriptableObject
    {
        // No need to show it, or for debugging?
        public List<ScenePassage> subAssets = new List<ScenePassage>();

        public void CreateSubAsset()
        {
            // Create a simple material asset

            string scriptableObjectPath = AssetDatabase.GetAssetPath(this);

            ScenePassage subAsset = CreateInstance<ScenePassage>();
            subAssets.Add(subAsset);
            subAsset.Init(this, subAssets.Count);

            AssetDatabase.AddObjectToAsset(subAsset, scriptableObjectPath);

            AssetDatabase.SaveAssets();
        }

        public void ClearSubAssets()
        {
            for (int i = 0; i < subAssets.Count; ++i)
                DestroyImmediate(subAssets[i], true);

            subAssets.Clear();
            AssetDatabase.SaveAssets();
        }

        public void DeleteSubAsset(ScenePassage subAsset)
        {
            subAssets.Remove(subAsset);
            DestroyImmediate(subAsset, true);
            AssetDatabase.SaveAssets();
        }

#if UNITY_EDITOR
        private void Awake()
        {
            var assetPath = AssetDatabase.GetAssetPath(this);
            var fileName = System.IO.Path.GetFileNameWithoutExtension(assetPath);
            name = fileName;
        }
#endif
    }

#if UNITY_EDITOR
    [CustomEditor(typeof(ScenePassages))]
    public class ScriptableSubAssetEditor : RSLib.EditorUtilities.ButtonProviderEditor<ScenePassages>
    {
        protected override void DrawButtons()
        {
            if (GUILayout.Button("Create SubAsset"))
                Obj.CreateSubAsset();

            if (GUILayout.Button("Clear SubAssets"))
                Obj.ClearSubAssets();
        }
    }
#endif
}