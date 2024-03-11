namespace RSLib.Jumble.DungeonGenerator
{
    [System.Flags]
    public enum RoomType : byte
    {
        NA = 0,
        L = 1,
        R = 2,
        T = 4,
        B = 8,
        LR = L | R,
        TB = T | B,
        RB = R | B,
        RT = R | T,
        LB = L | B,
        LT = L | T,
        LRT = L | R | T,
        LRB = L | R | B,
        LTB = L | T | B,
        RTB = R | T | B,
        LRTB = L | R | T | B
    }
}