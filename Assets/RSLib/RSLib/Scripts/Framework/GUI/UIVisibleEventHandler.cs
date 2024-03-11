namespace RSLib.Framework.GUI
{
    using System.Collections.Generic;
    using UnityEngine;

    /// <summary>
    /// UI GameObjects that needs an event when they became visible or not can implement this
    /// and register themselves in the UIVisibleEventHandler instance.
    /// </summary>
    public interface IUIVisibleEventListener
    {
        GameObject GameObject { get; }
        void OnUIVisibleChanged(bool visible);
    }
    
    /// <summary>
    /// Calls an event on registered UI gameObjects when they became visible/invisible, based on their parent canvases.
    /// Listeners gameObjects must implement the IUIVisibleEventListener interface and register/unregister themselves.
    /// </summary>
    public class UIVisibleEventHandler : RSLib.Framework.Singleton<UIVisibleEventHandler>
    {
#if UNITY_EDITOR
        [SerializeField] private List<GameObject> _listenersGameObjects = null;
#endif
        
        private HashSet<IUIVisibleEventListener> _listeners = new HashSet<IUIVisibleEventListener>();
        private Dictionary<IUIVisibleEventListener, bool> _listenersRegisteredVisible = new Dictionary<IUIVisibleEventListener, bool>();
        private Dictionary<IUIVisibleEventListener, Canvas[]> _listenersCanvases = new Dictionary<IUIVisibleEventListener, Canvas[]>();

        /// <summary>
        /// Registers an IUIVisibleEventListener instance to handle its visible state event.
        /// </summary>
        /// <param name="listener">IUIVisibleEventListener instance.</param>
        public static void Register(IUIVisibleEventListener listener)
        {
            if (!Exists())
                return;
            
            if (!Instance._listeners.Add(listener))
                return;
            
            Instance._listenersRegisteredVisible.Add(listener, listener.GameObject.activeInHierarchy);

            Canvas[] canvases = listener.GameObject.GetComponentsInParent<Canvas>();
            Instance._listenersCanvases.Add(listener, canvases);

#if UNITY_EDITOR
            Instance._listenersGameObjects.Add(listener.GameObject);
#endif
            
            if (canvases == null || canvases.Length == 0)
            {
                Instance.LogWarning($"Tried to register {listener.GameObject.name} but it has no parent canvas, unregistering it.", listener.GameObject);
                Unregister(listener);
            }
        }

        /// <summary>
        /// Unregisters an IUIVisibleEventListener instance to stop handling its visible state event.
        /// </summary>
        /// <param name="listener">IUIVisibleEventListener instance.</param>
        public static void Unregister(IUIVisibleEventListener listener)
        {
            if (!Exists())
                return;
            
            if (!Instance._listeners.Remove(listener))
                return;
            
            Instance._listenersRegisteredVisible.Remove(listener);
            Instance._listenersCanvases.Remove(listener);
            
#if UNITY_EDITOR
            Instance._listenersGameObjects.Remove(listener.GameObject);
#endif
        }

        /// <summary>
        /// Behaviour used to check if an IUIVisibleEventListener is visible or not.
        /// Loop through all parent canvases and checks if at least one is disabled to mark the listener as invisible.
        /// Can be overriden if needed.
        /// </summary>
        /// <param name="listener">IUIVisibleEventListener instance.</param>
        /// <returns>True if the IUIVisibleEventListener instance is visible, else false.</returns>
        protected virtual bool IsVisible(IUIVisibleEventListener listener)
        {
            Canvas[] canvases = _listenersCanvases[listener];
            for (int i = 0; i < canvases.Length; ++i)
                if (!canvases[i].enabled)
                    return false;

            return true;
        }
        
        private void Update()
        {
            foreach (IUIVisibleEventListener listener in _listeners)
            {
                bool registeredVisible = _listenersRegisteredVisible[listener];
                bool currentVisible = IsVisible(listener);
                
                if (currentVisible == registeredVisible)
                    continue;
                
                listener.OnUIVisibleChanged(currentVisible);
                _listenersRegisteredVisible[listener] = currentVisible;
            }
        }
    }
}
