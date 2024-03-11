namespace RSLib.Jumble.Flock
{
    using UnityEngine;

    [RequireComponent(typeof(Collider2D))]
    public class FlockAgent : MonoBehaviour
    {
        private Transform _transform;

        public Collider2D Collider2D { get; private set; }

        public Flock Flock { get; private set; }

        public virtual void Init(Flock flock)
        {
            Flock = flock;
        }

        public virtual void OnVelocityComputed(Vector2 velocity)
        {
            _transform.up = velocity;
            _transform.position += (Vector3) velocity * Time.deltaTime;
        }

        private void Awake()
        {
            _transform = transform;
            Collider2D = GetComponent<Collider2D>();
        }
    }
}