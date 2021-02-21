public interface IHittable
{
    HitLayer HitLayer { get; }

    void OnHit(AttackDatas attackDatas, float dir);
}