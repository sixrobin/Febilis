namespace Templar.Physics.MovingPlatform
{
    using UnityEngine;

    public class MovingPlatformSimplePassenger : MonoBehaviour, IMovingPlatformPassenger
    {
        void IMovingPlatformPassenger.OnPlatformMoved(Vector3 vel, bool standingOnPlatform)
        {
            transform.Translate(vel * Time.deltaTime);
        }
    }
}