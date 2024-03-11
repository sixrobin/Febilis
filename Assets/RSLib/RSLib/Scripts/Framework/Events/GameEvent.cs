namespace RSLib.Framework.Events
{
    using UnityEngine;

    [CreateAssetMenu(fileName = "New Game Event", menuName = "RSLib/Event/Game Event")]
    public class GameEvent : ScriptableObject
    {
        private readonly System.Collections.Generic.List<GameEventListener> _listeners = new System.Collections.Generic.List<GameEventListener>();

        public void Raise()
        {
            for (int i = _listeners.Count - 1; i >= 0; --i)
                _listeners[i].OnEventRaised();
        }
        
        public void Register(GameEventListener listener)
        {
            _listeners.Add(listener);
        }
        public void Unregister(GameEventListener listener)
        {
            _listeners.Remove(listener);
        }
    }
}