namespace Templar.Physics.Destroyables
{
    public enum DestroyableSourceType
    {
        NONE = 0,
        ATTACK = 1,
        ROLL = 2,
        FALL = 4,
        ALL = ATTACK | ROLL | FALL
    }
}