namespace Templar.Physics.Destroyables
{
    public enum DestroyableSourceType
    {
        ATTACK = 0,
        ROLL = 1,
        FALL = 2,
        ALL = ATTACK | ROLL | FALL
    }
}