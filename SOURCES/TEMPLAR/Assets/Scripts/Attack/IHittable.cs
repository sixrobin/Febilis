namespace Templar.Attack
{
    public interface IHittable
    {
        HitLayer HitLayer { get; }

        bool CanBeHit();
        void OnHit(HitInfos hitDatas);
    }
}