namespace RSLib.Framework.Events
{
    using UnityEngine;

    public abstract class ValuesValidatedEventScriptableObject : ScriptableObject
    {
        public delegate void ValuesValidatedEventHandler();
        public event ValuesValidatedEventHandler ValuesValidated;

        protected virtual void OnValidate()
        {
            ValuesValidated?.Invoke();
        }
    }
}