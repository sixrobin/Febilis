namespace Templar
{
    public enum BoardDirection
    {
        NONE,
        NORTH,
        SOUTH,
        EAST,
        WEST
    }

    public static class CardinalDirectionExtensions
    {
        public static BoardDirection Opposite(this BoardDirection dir)
        {
            switch (dir)
            {
                case BoardDirection.NORTH:
                    return BoardDirection.SOUTH;

                case BoardDirection.SOUTH:
                    return BoardDirection.NORTH;

                case BoardDirection.EAST:
                    return BoardDirection.WEST;

                case BoardDirection.WEST:
                    return BoardDirection.EAST;
            }

            return BoardDirection.NONE;
        }

        public static UnityEngine.Vector2 ConvertToVector2(this BoardDirection dir)
        {
            switch (dir)
            {
                case BoardDirection.NORTH:
                    return new UnityEngine.Vector2(0f, 1f);

                case BoardDirection.SOUTH:
                    return new UnityEngine.Vector2(0f, -1f);

                case BoardDirection.EAST:
                    return new UnityEngine.Vector2(1f, 0f);

                case BoardDirection.WEST:
                    return new UnityEngine.Vector2(-1f, 0f);
            }

            return UnityEngine.Vector2.zero;
        }
    }
}