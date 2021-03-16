namespace Templar.Attack
{
    /// <summary>
     /// Represents the different methods that can be used to compute the direction to pass in as argument
     /// to any target that got hit by the attack, so that then can be recoiled, flip their sprite, etc.
     /// NONE = default value, should never be used.
     /// ATTACK_DIR = use the base attack direction.
     /// X_OFFSET = use the offset between the attack source's position x and the target's position x.
     /// </summary>
    public enum HitDirComputationType
    {
        NONE,
        ATTACK_DIR,
        X_OFFSET
    }
}