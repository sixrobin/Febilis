namespace Templar.Physics.MovingPlatform
{
    using UnityEngine;

    public class MovingPlatformRigidbodyPassenger : MonoBehaviour, IMovingPlatformPassenger
    {
        [SerializeField] private Rigidbody2D _rb = null;

        void IMovingPlatformPassenger.OnPlatformMoved(Vector3 vel, bool standingOnPlatform)
        {
            _rb.velocity = (Vector2)vel;
        }
    }
}