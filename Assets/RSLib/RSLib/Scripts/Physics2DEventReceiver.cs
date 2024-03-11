namespace RSLib
{
    using RSLib.Extensions;
    using UnityEngine;
    #if ODIN_INSPECTOR
    using Sirenix.OdinInspector;
    #endif
    
    [DisallowMultipleComponent]
    public class Physics2DEventReceiver : MonoBehaviour
    {
        #if !ODIN_INSPECTOR
        [Header("LAYER MASK")]
        #endif

        #if ODIN_INSPECTOR
        [BoxGroup("Layer Mask")]
        #endif
        [SerializeField] private LayerMask _mask = 0;

        #if !ODIN_INSPECTOR
        [Header("EVENTS")]
        #endif
        
        #if ODIN_INSPECTOR
        [FoldoutGroup("Events")]
        #endif
        [SerializeField] private Framework.Events.Collider2DEvent _onTriggerEnter = null;
        
        #if ODIN_INSPECTOR
        [FoldoutGroup("Events")]
        #endif
        [SerializeField] private Framework.Events.Collider2DEvent _onTriggerExit = null;
        
        #if ODIN_INSPECTOR
        [FoldoutGroup("Events")]
        #endif
        [SerializeField] private Framework.Events.Collision2DEvent _onCollisionEnter = null;
        
        #if ODIN_INSPECTOR
        [FoldoutGroup("Events")]
        #endif
        [SerializeField] private Framework.Events.Collision2DEvent _onCollisionExit = null;

        public delegate void Collider2DEventHandler(Collider2D collider);
        public delegate void Collision2DEventHandler(Collision2D collider);

        public event Collider2DEventHandler TriggerEntered;
        public event Collider2DEventHandler TriggerExit;
        public event Collision2DEventHandler CollisionEntered;
        public event Collision2DEventHandler CollisionExit;

        protected virtual void OnTriggerEnter2D(Collider2D collider)
        {
            if (!_mask.HasLayer(collider.gameObject.layer))
                return;

            TriggerEntered?.Invoke(collider);
            _onTriggerEnter?.Invoke(collider);
        }

        protected virtual void OnTriggerExit2D(Collider2D collider)
        {
            if (!_mask.HasLayer(collider.gameObject.layer))
                return;

            TriggerExit?.Invoke(collider);
            _onTriggerExit?.Invoke(collider);
        }

        protected virtual void OnCollisionEnter2D(Collision2D collision)
        {
            if (!_mask.HasLayer(collision.gameObject.layer))
                return;

            CollisionEntered?.Invoke(collision);
            _onCollisionEnter?.Invoke(collision);
        }

        protected virtual void OnCollisionExit2D(Collision2D collision)
        {
            if (!_mask.HasLayer(collision.gameObject.layer))
                return;

            CollisionExit?.Invoke(collision);
            _onCollisionExit?.Invoke(collision);
        }

        private void Awake()
        {
            if (!GetComponent<Collider2D>())
                UnityEngine.Debug.LogError($"{GetType().Name} instance on {transform.name} does not have a Collider2D and can then not be triggered.", gameObject);
        }
    }
}