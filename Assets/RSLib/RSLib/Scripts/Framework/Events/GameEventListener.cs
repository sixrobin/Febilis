namespace RSLib.Framework.Events
{
    using UnityEngine;

    public class GameEventListener : MonoBehaviour
    {
        [SerializeField] private GameEvent _event = null;
        [SerializeField] private UnityEngine.Events.UnityEvent _onEventRaised = null;
        
        public void OnEventRaised()
        {
            _onEventRaised?.Invoke();
        }

        private void OnEnable()
        {
           _event.Register(this);
        }
        private void OnDisable()
        {
            _event.Unregister(this);
        }
    }
}