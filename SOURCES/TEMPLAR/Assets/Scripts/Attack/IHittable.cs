namespace Templar.Attack
{
    public interface IHittable
    {
        HitLayer HitLayer { get; }

        bool CanBeHit(HitInfos hitInfos);
        void OnHit(HitInfos hitInfos);
    }
}