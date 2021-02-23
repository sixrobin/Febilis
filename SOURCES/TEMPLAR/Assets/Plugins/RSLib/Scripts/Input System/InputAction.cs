namespace RSLib.Framework.InputSystem
{
    /// <summary>
    /// Represents every action of the game that can be triggered by a button (axis are not handled).
    /// This enum is the only file that will necessarily modified when used on a new project.
    /// Keep the "NONE" case for debugging/default values purpose.
    /// </summary>
    public enum InputAction : int
    {
        NONE = 0,
        JUMP = 1,
        ROLL = 2,
        ATTACK = 3
    }
}