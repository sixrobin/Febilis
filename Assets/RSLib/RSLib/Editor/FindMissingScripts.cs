namespace RSLib.Editor
{
    using UnityEditor;
    using UnityEngine;

    public class FindMissingScripts : EditorWindow
    {
        private static int s_goCount = 0;
        private static int s_componentsCount = 0;
        private static int s_missingCount = 0;

        [MenuItem("RSLib/Find Missing Scripts")]
        public static void ShowWindow()
        {
            GetWindow<FindMissingScripts>("Find Missing Scripts").Show();
        }

        private static Object[] LoadAllAssetsAtPath(string assetPath)
        {
            return typeof(SceneAsset) == AssetDatabase.GetMainAssetTypeAtPath(assetPath)
                ? new[] { AssetDatabase.LoadMainAssetAtPath(assetPath) }
                : AssetDatabase.LoadAllAssetsAtPath(assetPath);
        }

        private static void FindAll()
        {
            s_componentsCount = 0;
            s_goCount = 0;
            s_missingCount = 0;

            string[] assetsPaths = AssetDatabase.GetAllAssetPaths();
            foreach (string assetPath in assetsPaths)
            {
                Object[] asset = LoadAllAssetsAtPath(assetPath);
                for (int i = 0; i < asset.Length; ++i)
                    if (asset[i] is GameObject gameObject)
                        FindInGameObject(gameObject);
            }

            Debug.Log($"Searched {s_goCount} GameObject(s), {s_componentsCount} component(s), found {s_missingCount} missing.");
        }

        private static void FindInSelected()
        {
            s_goCount = 0;
            s_componentsCount = 0;
            s_missingCount = 0;

            GameObject[] selection = Selection.gameObjects;
            foreach (GameObject gameObject in selection)
                FindInGameObject(gameObject);

            Debug.Log($"Searched {s_goCount} GameObject(s), {s_componentsCount} component(s), found {s_missingCount} missing.");
        }

        private static void FindInGameObject(GameObject gameObject)
        {
            s_goCount++;

            Component[] components = gameObject.GetComponents<Component>();
            for (int i = 0; i < components.Length; ++i)
            {
                s_componentsCount++;
                if (components[i] != null)
                    continue;

                s_missingCount++;

                string name = gameObject.name;
                Transform transform = gameObject.transform;
                while (transform.parent != null)
                {
                    Transform parent = transform.parent;
                    name = $"{parent.name}/{name}";
                    transform = parent;
                }

                Debug.LogWarning($"{name} has a missing script attached (script position: {i}).", gameObject);
            }

            // Recursive scan.
            foreach (Transform child in gameObject.transform)
                FindInGameObject(child.gameObject);
        }
        
        public void OnGUI()
        {
            if (GUILayout.Button("Find Missing Scripts in selected GameObjects"))
                FindInSelected();

            if (GUILayout.Button("Find all Missing Scripts")
				&& EditorUtility.DisplayDialog("Command warning", "This may take some time depending on Asset Database size.", "Continue", "Cancel"))
                FindAll();
            
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Component Scanned:");
            EditorGUILayout.LabelField("" + (s_componentsCount == -1 ? "---" : s_componentsCount.ToString()));
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Object Scanned:");
            EditorGUILayout.LabelField("" + (s_goCount == -1 ? "---" : s_goCount.ToString()));
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Possible Missing Scripts:");
            EditorGUILayout.LabelField("" + (s_missingCount == -1 ? "---" : s_missingCount.ToString()));
            EditorGUILayout.EndHorizontal();
        }
    }
}