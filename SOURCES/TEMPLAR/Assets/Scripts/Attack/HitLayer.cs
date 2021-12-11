namespace Templar.Attack
{
    public enum HitLayer
    {
        NONE = 0,
        PLAYER = 1,
        ENEMY = 2,
        PICKUP = 4,
        UNITS = PLAYER | ENEMY,
        ALL = UNITS | PICKUP
    }
}