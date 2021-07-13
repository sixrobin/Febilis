namespace Templar.Physics
{
    public interface IPlatformPassenger
    {
        void OnPlatformMoved(UnityEngine.Vector3 vel, bool standingOnPlatform);
    }
}