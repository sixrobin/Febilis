namespace Templar.Physics.MovingPlatform
{
    public interface IMovingPlatformPassenger
    {
        void OnPlatformMoved(UnityEngine.Vector3 vel, bool standingOnPlatform);
    }
}