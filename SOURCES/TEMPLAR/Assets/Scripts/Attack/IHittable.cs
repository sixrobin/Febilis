namespace Templar.Attack
{
    public interface IHittable
    {
        bool CanBeHit { get; }
        HitLayer HitLayer { get; }

        void OnHit(HitInfos hitDatas);
    }
}