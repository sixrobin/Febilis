namespace Templar.Attack
{
    public enum HitLayer
    {
        NONE = 0,
        PLAYER = 1,
        ENEMY = 2,
        ALL = PLAYER | ENEMY
    }
}