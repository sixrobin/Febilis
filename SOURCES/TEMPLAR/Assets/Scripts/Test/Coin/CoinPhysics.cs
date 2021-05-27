namespace Templar
{
    using RSLib.Extensions;
    using UnityEngine;

    public class CoinPhysics : MonoBehaviour
    {
        [SerializeField] private Rigidbody2D _rb = null;
        [SerializeField] private Vector2 _forceRange = new Vector2(4f, 9f);
        [SerializeField] private float _initDirRandomAngle = 70f;

        private void Bump()
        {
            Vector3 dir = Quaternion.Euler(0f, 0f, Random.Range(-_initDirRandomAngle * 0.5f, _initDirRandomAngle * 0.5f)) * Vector2.up;
            _rb.AddForce(dir * _forceRange.Random(), ForceMode2D.Impulse);
        }

        private void Start()
        {
            // Should be done in OnEnable OR a method that is called when called from a pool ?
            Bump();
        }
    }
}