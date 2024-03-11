namespace RSLib.Framework
{
    using UnityEngine;
    #if ODIN_INSPECTOR
    using Sirenix.OdinInspector;
    #endif
    
    /// <summary>
    /// Child class of Singleton to handle Console Pro plugin prefix system.
    /// </summary>
    [DisallowMultipleComponent]
    public class SingletonConsolePro<T> : Singleton<T> where T : MonoBehaviour
    {
        #if ODIN_INSPECTOR
        [FoldoutGroup("Singleton")]
        #endif
        [SerializeField] private OptionalString _overrideLogPrefix = new OptionalString(string.Empty, false);

        private string Prefix => !_overrideLogPrefix.Enabled
            ? GetType().Name
            : string.IsNullOrEmpty(_overrideLogPrefix.Value) ? GetType().Name : _overrideLogPrefix.Value;

        public override void Log(string msg, bool forceVerbose = false)
        {
            if (Verbose || forceVerbose)
                Debug.Log($"#{Prefix}#{msg}", gameObject);
        }

        public override void Log(string msg, Object context, bool forceVerbose = false)
        {
            if (Verbose || forceVerbose)
                Debug.Log($"#{Prefix}#{msg}", context ? context : gameObject);
        }

        public override void LogWarning(string msg)
        {
            Debug.LogWarning($"#{Prefix}#{msg}", gameObject);
        }

        public override void LogWarning(string msg, Object context)
        {
            Debug.LogWarning($"#{Prefix}#{msg}", context ? context : gameObject);
        }

        public override void LogError(string msg)
        {
            Debug.LogError($"#{Prefix}#{msg}", gameObject);
        }

        public override void LogError(string msg, Object context)
        {
            Debug.LogError($"#{Prefix}#{msg}", context ? context : gameObject);
        }
    }
}