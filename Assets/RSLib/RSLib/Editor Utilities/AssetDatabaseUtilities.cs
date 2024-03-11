#if UNITY_EDITOR
namespace RSLib.EditorUtilities
{
    using UnityEngine;

    public static class AssetDatabaseUtilities
    {
        public static T[] GetAllAssetsAtFolderPath<T>(string path) where T : Object
        {
            System.Collections.Generic.List<T> assetsList = new System.Collections.Generic.List<T>();
            string[] fileEntries = System.IO.Directory.GetFiles($"{Application.dataPath}/{path.Remove(0, 7)}");

            for (int i = fileEntries.Length - 1; i >= 0; --i)
            {
                string slashReplacedPath = fileEntries[i].Replace("\\", "/");
                int lastSlashIndex = slashReplacedPath.LastIndexOf("/");
                string localPath = $"Assets/{path.Remove(0, 7)}";

                if (lastSlashIndex > 0)
                    localPath += slashReplacedPath.Substring(lastSlashIndex);

                T asset = UnityEditor.AssetDatabase.LoadAssetAtPath(localPath, typeof(T)) as T;
                if (asset != null)
                    assetsList.Add(asset);
            }

            return assetsList.ToArray();
        }

        public static string[] GetSubFoldersRecursively(string rootPath, bool includeRoot = false)
        {
            System.Collections.Generic.List<string> paths = new System.Collections.Generic.List<string>();

            if (includeRoot)
                paths.Add(rootPath);
            
            foreach (string path in UnityEditor.AssetDatabase.GetSubFolders(rootPath))
            {
                paths.Add(path);
                paths.AddRange(GetSubFoldersRecursively(path, false));
            }

            return paths.ToArray();
        }
    }
}
#endif