namespace Templar.Physics.Triggerables
{
    public enum TriggerableSourceType
    {
        NONE = 0,
        ATTACK = 1,
        ROLL = 2,
        FALL = 4,
        ALL = ATTACK | ROLL | FALL
    }
}