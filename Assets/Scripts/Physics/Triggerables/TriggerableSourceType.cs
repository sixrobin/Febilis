namespace Templar.Physics.Triggerables
{
    public enum TriggerableSourceType
    {
        NONE = 0,
        ATTACK = 1,
        ROLL = 2,
        FALL = 4,
        PHYSICS = ATTACK | ROLL | FALL,
        LOAD = 8
    }
}