namespace Templar
{
    public enum ScreenDirection
    {
        NONE,
        UP,
        DOWN,
        RIGHT,
        LEFT
    }

    public static class CardinalDirectionExtensions
    {
        public static ScreenDirection Opposite(this ScreenDirection dir)
        {
            switch (dir)
            {
                case ScreenDirection.UP:
                    return ScreenDirection.DOWN;
                case ScreenDirection.DOWN:
                    return ScreenDirection.UP;
                case ScreenDirection.RIGHT:
                    return ScreenDirection.LEFT;
                case ScreenDirection.LEFT:
                    return ScreenDirection.RIGHT;

                default:
                    return ScreenDirection.NONE;
            }
        }

        public static UnityEngine.Vector2 ConvertToVector2(this ScreenDirection dir)
        {
            switch (dir)
            {
                case ScreenDirection.UP:
                    return new UnityEngine.Vector2(0f, 1f);
                case ScreenDirection.DOWN:
                    return new UnityEngine.Vector2(0f, -1f);
                case ScreenDirection.RIGHT:
                    return new UnityEngine.Vector2(1f, 0f);
                case ScreenDirection.LEFT:
                    return new UnityEngine.Vector2(-1f, 0f);

                default:
                    return UnityEngine.Vector2.zero;
            }
        }
    }
}