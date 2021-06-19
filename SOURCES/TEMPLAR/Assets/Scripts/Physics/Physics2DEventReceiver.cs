namespace Templar.Physics
{
    using RSLib.Extensions;
    using UnityEngine;

    public class Physics2DEventReceiver : MonoBehaviour
    {
        [SerializeField] private LayerMask _mask = 0;

        [SerializeField] private RSLib.Framework.Events.Collider2DEvent _onTriggerEnter = null;
        [SerializeField] private RSLib.Framework.Events.Collider2DEvent _onTriggerExit = null;
        [SerializeField] private RSLib.Framework.Events.Collision2DEvent _onCollisionEnter = null;
        [SerializeField] private RSLib.Framework.Events.Collision2DEvent _onCollisionExit = null;

        public delegate void Collider2DEventHandler(Collider2D collider);
        public delegate void Collision2DEventHandler(Collision2D collider);

        public event Collider2DEventHandler OnTriggerEnter;
        public event Collider2DEventHandler OnTriggerExit;
        public event Collision2DEventHandler OnCollisionEnter;
        public event Collision2DEventHandler OnCollisionExit;

        protected virtual void OnTriggerEnter2D(Collider2D collider)
        {
            if (!_mask.HasLayer(collider.gameObject.layer))
                return;

            OnTriggerEnter?.Invoke(collider);
            _onTriggerEnter?.Invoke(collider);
        }

        protected virtual void OnTriggerExit2D(Collider2D collider)
        {
            if (!_mask.HasLayer(collider.gameObject.layer))
                return;

            OnTriggerExit?.Invoke(collider);
            _onTriggerExit?.Invoke(collider);
        }

        protected virtual void OnCollisionEnter2D(Collision2D collision)
        {
            if (!_mask.HasLayer(collision.gameObject.layer))
                return;

            OnCollisionEnter?.Invoke(collision);
            _onCollisionEnter?.Invoke(collision);
        }

        protected virtual void OnCollisionExit2D(Collision2D collision)
        {
            if (!_mask.HasLayer(collision.gameObject.layer))
                return;

            OnCollisionExit?.Invoke(collision);
            _onCollisionExit?.Invoke(collision);
        }
    }
}