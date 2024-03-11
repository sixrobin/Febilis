namespace RSLib
{
    using RSLib.Extensions;
    using UnityEngine;
    #if ODIN_INSPECTOR
    using Sirenix.OdinInspector;
    #endif
    
    [DisallowMultipleComponent]
    public class PhysicsEventReceiver : MonoBehaviour
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
        [SerializeField] private Framework.Events.ColliderEvent _onTriggerEnter = null;
        
        #if ODIN_INSPECTOR
        [FoldoutGroup("Events")]
        #endif
        [SerializeField] private Framework.Events.ColliderEvent _onTriggerExit = null;
        
        #if ODIN_INSPECTOR
        [FoldoutGroup("Events")]
        #endif
        [SerializeField] private Framework.Events.CollisionEvent _onCollisionEnter = null;
        
        #if ODIN_INSPECTOR
        [FoldoutGroup("Events")]
        #endif
        [SerializeField] private Framework.Events.CollisionEvent _onCollisionExit = null;

        public delegate void ColliderEventHandler(Collider collider);
        public delegate void CollisionEventHandler(Collision collider);

        public event ColliderEventHandler TriggerEntered;
        public event ColliderEventHandler TriggerExit;
        public event CollisionEventHandler CollisionEntered;
        public event CollisionEventHandler CollisionExit;

        protected virtual void OnTriggerEnter(Collider collider)
        {
            if (!_mask.HasLayer(collider.gameObject.layer))
                return;

            TriggerEntered?.Invoke(collider);
            _onTriggerEnter?.Invoke(collider);
        }

        protected virtual void OnTriggerExit(Collider collider)
        {
            if (!_mask.HasLayer(collider.gameObject.layer))
                return;

            TriggerExit?.Invoke(collider);
            _onTriggerExit?.Invoke(collider);
        }

        protected virtual void OnCollisionEnter(Collision collision)
        {
            if (!_mask.HasLayer(collision.gameObject.layer))
                return;

            CollisionEntered?.Invoke(collision);
            _onCollisionEnter?.Invoke(collision);
        }

        protected virtual void OnCollisionExit(Collision collision)
        {
            if (!_mask.HasLayer(collision.gameObject.layer))
                return;

            CollisionExit?.Invoke(collision);
            _onCollisionExit?.Invoke(collision);
        }
    }
}