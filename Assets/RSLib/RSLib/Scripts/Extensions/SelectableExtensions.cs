namespace RSLib.Extensions
{
    using UnityEngine.UI;

    public static class SelectableExtensions
    {
        /// <summary>
        /// Sets the Selectable navigation mode.
        /// </summary>
        public static void SetMode(this Selectable selectable, Navigation.Mode mode)
        {
            selectable.navigation = new Navigation()
            {
                mode = mode,
                selectOnUp = selectable.navigation.selectOnUp,
                selectOnDown = selectable.navigation.selectOnDown,
                selectOnLeft = selectable.navigation.selectOnLeft,
                selectOnRight = selectable.navigation.selectOnRight
            };
        }

        /// <summary>
        /// Sets the Selectable selected on up.
        /// </summary>
        public static void SetSelectOnUp(this Selectable selectable, Selectable selectOnUp)
        {
            selectable.navigation = new Navigation()
            {
                mode = selectable.navigation.mode,
                selectOnUp = selectOnUp,
                selectOnDown = selectable.navigation.selectOnDown,
                selectOnLeft = selectable.navigation.selectOnLeft,
                selectOnRight = selectable.navigation.selectOnRight
            };
        }

        /// <summary>
        /// Sets the Selectable selected on down.
        /// </summary>
        public static void SetSelectOnDown(this Selectable selectable, Selectable selectOnDown)
        {
            selectable.navigation = new Navigation()
            {
                mode = selectable.navigation.mode,
                selectOnUp = selectable.navigation.selectOnUp,
                selectOnDown = selectOnDown,
                selectOnLeft = selectable.navigation.selectOnLeft,
                selectOnRight = selectable.navigation.selectOnRight
            };
        }

        /// <summary>
        /// Sets the Selectable selected on left.
        /// </summary>
        public static void SetSelectOnLeft(this Selectable selectable, Selectable selectOnLeft)
        {
            selectable.navigation = new Navigation()
            {
                mode = selectable.navigation.mode,
                selectOnUp = selectable.navigation.selectOnUp,
                selectOnDown = selectable.navigation.selectOnDown,
                selectOnLeft = selectOnLeft,
                selectOnRight = selectable.navigation.selectOnRight
            };
        }

        /// <summary>
        /// Sets the Selectable selected on right.
        /// </summary>
        public static void SetSelectOnRight(this Selectable selectable, Selectable selectOnRight)
        {
            selectable.navigation = new Navigation()
            {
                mode = selectable.navigation.mode,
                selectOnUp = selectable.navigation.selectOnUp,
                selectOnDown = selectable.navigation.selectOnDown,
                selectOnLeft = selectable.navigation.selectOnLeft,
                selectOnRight = selectOnRight
            };
        }
    }
}