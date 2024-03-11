namespace RSLib.Framework
{
    using UnityEngine;
    #if ODIN_INSPECTOR
    using Sirenix.OdinInspector;
    #endif
    
    [DisallowMultipleComponent]
    public abstract class Singleton<T> : MonoBehaviour where T : MonoBehaviour
    {
        #if ODIN_INSPECTOR
        [FoldoutGroup("Singleton")]
        #endif
        [SerializeField] private bool _dontDestroy = false;
        
        #if ODIN_INSPECTOR
        [FoldoutGroup("Singleton")]
        #endif
        [SerializeField] private bool _verbose = false;

        private static T s_instance;
        public static T Instance
        {
            get
            {
                if (s_instance == null)
                {
                    s_instance = FindObjectOfType<T>();
                    if (s_instance == null)
                        Debug.LogError($"No {typeof(T).Name} instance found in the scene to make a singleton.");
                }

                return s_instance;
            }
        }

        /// <summary>
        /// Checks if this instance of the class is the initialized singleton.
        /// </summary>
        protected bool IsValid => s_instance == this;

        /// <summary>
        /// Determines if the info logs should be logged. Warning and errors are always logged.
        /// </summary>
        public bool Verbose => _verbose;

        /// <summary>
        /// Checks if the singleton instance is referenced. This method should not be called in any Awake method since
        /// singletons initializations are done during the Awake call, so they can actually exist in the scene but not
        /// having been initialized yet.
        /// </summary>
        /// <returns>If the singleton instance is referenced or not.</returns>
        public static bool Exists()
        {
            // Instance property is not used to avoid errors when this method is called in an OnDestroy method.
            return s_instance != null;
        }

        /// <summary>
        /// Destroys the singleton instance if it exists.
        /// </summary>
        public static void Kill()
        {
            if (!Exists())
                return;

            Destroy(s_instance.gameObject);
            s_instance = null;
        }

        #region STATIC LOG
        
        public static void LogStatic(string msg, bool forceVerbose = false)
        {
            if (s_instance is Singleton<T> instance)
                instance.Log(msg, forceVerbose);
            else
                Debug.Log($"{typeof(T).Name}: {msg}");
        }

        public static void LogStatic(string msg, Object context, bool forceVerbose = false)
        {
            if (s_instance is Singleton<T> instance)
                instance.Log(msg, context, forceVerbose);
            else
                Debug.Log($"{typeof(T).Name}: {msg}");
        }
        
        public static void LogWarningStatic(string msg)
        {
            if (s_instance is Singleton<T> instance)
                instance.LogWarning(msg);
            else
                Debug.LogWarning($"{typeof(T).Name}: {msg}");
        }
        
        public static void LogWarningStatic(string msg, Object context)
        {
            if (s_instance is Singleton<T> instance)
                instance.LogWarning(msg, context);
            else
                Debug.LogWarning($"{typeof(T).Name}: {msg}");
        }
        
        public static void LogErrorStatic(string msg)
        {
            if (s_instance is Singleton<T> instance)
                instance.LogError(msg);
            else
                Debug.LogError($"{typeof(T).Name}: {msg}");
        }
        
        public static void LogErrorStatic(string msg, Object context)
        {
            if (s_instance is Singleton<T> instance)
                instance.LogError(msg, context);
            else
                Debug.LogError($"{typeof(T).Name}: {msg}");
        }
        
        #endregion // STATIC LOG
        
        #region LOG

        public virtual void Log(string msg, bool forceVerbose = false)
        {
            if (Verbose || forceVerbose)
                Debug.Log($"{typeof(T).Name}: {msg}", gameObject);
        }

        public virtual void Log(string msg, Object context, bool forceVerbose = false)
        {
            if (Verbose || forceVerbose)
                Debug.Log($"{typeof(T).Name}: {msg}", context ? context : gameObject);
        }

        public virtual void LogError(string msg)
        {
            Debug.LogError($"{typeof(T).Name}: {msg}", gameObject);
        }

        public virtual void LogError(string msg, Object context)
        {
            Debug.LogError($"{typeof(T).Name}: {msg}", context ? context : gameObject);
        }

        public virtual void LogWarning(string msg)
        {
            Debug.LogWarning($"{typeof(T).Name}: {msg}", gameObject);
        }

        public virtual void LogWarning(string msg, Object context)
        {
            Debug.LogWarning($"{typeof(T).Name}: {msg}", context ? context : gameObject);
        }

        #endregion // LOG

        protected virtual void Awake()
        {
            if (s_instance == null)
                s_instance = this as T;

            if (s_instance != this)
            {
                if (s_instance.gameObject == gameObject)
                    DestroyImmediate(this);
                else
                    DestroyImmediate(gameObject);
            }
            else
            {
                if (_dontDestroy)
                {
                    transform.SetParent(null);
                    DontDestroyOnLoad(gameObject);
                }
            }
        }
    }
}