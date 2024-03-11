namespace Templar.Attack
{
    public interface IHittable
    {
        bool SpawnVFXOnHit { get; }
        HitLayer HitLayer { get; }

        bool CanBeHit(HitInfos hitInfos);
        void OnHit(HitInfos hitInfos);
    }
}