namespace RSLib.Framework.Events
{
    using UnityEngine;

    [DisallowMultipleComponent]
    public sealed class EventFunctionsListener : MonoBehaviour
    {
        [SerializeField] private UnityEngine.Events.UnityEvent _onAwake = null;
        [SerializeField] private UnityEngine.Events.UnityEvent _onEnable = null;
        [SerializeField] private UnityEngine.Events.UnityEvent _onStart = null;
        [SerializeField] private UnityEngine.Events.UnityEvent _onDisable = null;

        private void Awake()
        {
            _onAwake?.Invoke();
        }

        private void OnEnable()
        {
            _onEnable?.Invoke();
        }

        private void Start()
        {
            _onStart?.Invoke();
        }
        
        private void OnDisable()
        {
            _onDisable?.Invoke();
        }
    }
}
