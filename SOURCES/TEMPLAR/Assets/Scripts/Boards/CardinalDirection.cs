namespace Templar
{
    public enum CardinalDirection
    {
        NONE,
        NORTH,
        SOUTH,
        EAST,
        WEST
    }

    public static class CardinalDirectionExtensions
    {
        public static CardinalDirection Opposite(this CardinalDirection dir)
        {
            switch (dir)
            {
                case CardinalDirection.NORTH:
                    return CardinalDirection.SOUTH;

                case CardinalDirection.SOUTH:
                    return CardinalDirection.NORTH;

                case CardinalDirection.EAST:
                    return CardinalDirection.WEST;

                case CardinalDirection.WEST:
                    return CardinalDirection.EAST;
            }

            return CardinalDirection.NONE;
        }

        public static UnityEngine.Vector2 ConvertToVector2(this CardinalDirection dir)
        {
            switch (dir)
            {
                case CardinalDirection.NORTH:
                    return new UnityEngine.Vector2(0f, 1f);

                case CardinalDirection.SOUTH:
                    return new UnityEngine.Vector2(0f, -1f);

                case CardinalDirection.EAST:
                    return new UnityEngine.Vector2(1f, 0f);

                case CardinalDirection.WEST:
                    return new UnityEngine.Vector2(-1f, 0f);
            }

            return UnityEngine.Vector2.zero;
        }
    }
}