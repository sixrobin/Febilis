namespace RSLib.Framework
{
    public static class ResourcesPooler<T> where T : UnityEngine.Object
    {
        private static System.Collections.Generic.Dictionary<string, T> s_resources = new System.Collections.Generic.Dictionary<string, T>();
        private static System.Collections.Generic.Dictionary<string, T[]> s_resourcesAll = new System.Collections.Generic.Dictionary<string, T[]>();

        /// <summary>Caches asset at a given path starting in Resources folder.</summary>
        /// <param name="path">Asset path.</param>
        public static void Cache(string path)
        {
            if (!s_resources.ContainsKey(path))
                s_resources.Add(path, UnityEngine.Resources.Load<T>(path));
        }

        /// <summary>Caches all assets in a folder at a given path starting in Resources folder.</summary>
        /// <param name="path">Asset path.</param>
        public static void CacheAll(string path)
        {
            if (!s_resourcesAll.ContainsKey(path))
                s_resourcesAll.Add(path, UnityEngine.Resources.LoadAll<T>(path));
        }

        /// <summary>
        /// Clears loaded assets in pool, without unloading them.
        /// </summary>
        public static void Clear()
        {
            s_resources.Clear();
            s_resourcesAll.Clear();
        }

        /// <summary>Loads asset at a given path starting in Resources folder and returns it.</summary>
        /// <param name="path">Asset path.</param>
        /// <returns>Loaded asset if it has been found.</returns>
        public static T Load(string path)
        {
            if (s_resources.TryGetValue(path, out T resource))
                return resource;

            s_resources.Add(path, UnityEngine.Resources.Load<T>(path));
            return s_resources[path];
        }

        /// <summary>Loads all assets in a folder at a given path starting in Resources folder and returns them.</summary>
        /// <param name="path">Assets folder path.</param>
        /// <returns>Loaded assets if folder has been found.</returns>
        public static T[] LoadAll(string path)
        {
            if (s_resourcesAll.TryGetValue(path, out T[] resources))
                return resources;

            s_resourcesAll.Add(path, UnityEngine.Resources.LoadAll<T>(path));
            return s_resourcesAll[path];
        }

        /// <summary>Removes already loaded path from pool, reloads it at path starting in Resources folder, and returns it.</summary>
        /// <param name="path">Asset path.</param>
        /// <returns>Loaded asset if it has been found.</returns>
        public static T Reload(string path)
        {
            if (s_resources.ContainsKey(path))
                s_resources.Remove(path);

            return Load(path);
        }

        /// <summary>Removes already loaded assets path from pool, reloads them at path starting in Resources folder, and returns them.</summary>
        /// <param name="path">Assets folder path.</param>
        /// <returns>Loaded assets if folder has been found.</returns>
        public static T[] ReloadAll(string path)
        {
            if (s_resourcesAll.ContainsKey(path))
                s_resourcesAll.Remove(path);

            return LoadAll(path);
        }
    }

    public static class ResourcesPooler
    {
        /// <summary>Caches asset at a given path starting in Resources folder.</summary>
        /// <param name="path">Asset path.</param>
        public static void Cache<T>(string resourcePath) where T : UnityEngine.Object
        {
            ResourcesPooler<T>.Cache(resourcePath);
        }

        /// <summary>Caches all assets in a folder at a given path starting in Resources folder.</summary>
        /// <param name="path">Asset path.</param>
        public static void CacheAll<T>(string resourcePath) where T : UnityEngine.Object
        {
            ResourcesPooler<T>.CacheAll(resourcePath);
        }

        /// <summary>
        /// Clears loaded assets in pool, without unloading them.
        /// </summary>
        public static void Clear<T>() where T : UnityEngine.Object
        {
            ResourcesPooler<T>.Clear();
        }

        /// <summary>Loads asset at a given path starting in Resources folder and returns it.</summary>
        /// <param name="path">Asset path.</param>
        /// <returns>Loaded asset if it has been found.</returns>
        public static T Load<T>(string path) where T : UnityEngine.Object
        {
            return ResourcesPooler<T>.Load(path);
        }

        /// <summary>Loads all assets in a folder at a given path starting in Resources folder and returns them.</summary>
        /// <param name="path">Assets folder path.</param>
        /// <returns>Loaded assets if folder has been found.</returns>
        public static T[] LoadAll<T>(string path) where T : UnityEngine.Object
        {
            return ResourcesPooler<T>.LoadAll(path);
        }

        /// <summary>Removes already loaded path from pool, reloads it at path starting in Resources folder, and returns it.</summary>
        /// <param name="path">Asset path.</param>
        /// <returns>Loaded asset if it has been found.</returns>
        public static T Reload<T>(string path) where T : UnityEngine.Object
        {
            return ResourcesPooler<T>.Reload(path);
        }

        /// <summary>Removes already loaded assets path from pool, reloads them at path starting in Resources folder, and returns them.</summary>
        /// <param name="path">Assets folder path.</param>
        /// <returns>Loaded assets if folder has been found.</returns>
        public static T[] ReloadAll<T>(string path) where T : UnityEngine.Object
        {
            return ResourcesPooler<T>.ReloadAll(path);
        }
    }
}