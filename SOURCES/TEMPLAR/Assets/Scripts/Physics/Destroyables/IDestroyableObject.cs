namespace Templar.Physics.Destroyables
{
    public interface IDestroyableObject
    {
        DestroyableSourceType ValidSourcesTypes { get; }

        void Destroy(DestroyableSourceType sourceType);
    }
}