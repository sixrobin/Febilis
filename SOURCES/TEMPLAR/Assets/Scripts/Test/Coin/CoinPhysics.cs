namespace Templar
{
    using UnityEngine;

    public class CoinPhysics : MonoBehaviour
    {
        [SerializeField] private Rigidbody2D _rb = null;
        [SerializeField] private float _force = 10f;

        private Vector2 _lastForce;

        private void Bump()
        {
            Vector3 dir = Quaternion.Euler(0f, 0f, Random.Range(-45f, 45f)) * Vector2.up;
            _rb.AddForce(dir * _force, ForceMode2D.Impulse);
            _lastForce = dir;
        }

        private void Start()
        {
            Bump();
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.W))
                Bump();
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            Debug.Log($"{transform.name} collide with {collision.transform.name}");
        }

        private void OnCollisionEnter2D(Collision2D collision)
        {
            Debug.Log($"{transform.name} collide with {collision.transform.name}");
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(Vector2.zero, _lastForce.normalized);
        }
    }
}