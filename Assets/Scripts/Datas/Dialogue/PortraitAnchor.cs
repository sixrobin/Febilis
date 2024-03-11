namespace Templar.Datas.Dialogue
{
    public enum PortraitAnchor
    {
        NONE,
        TOP_LEFT,
        TOP_RIGHT
    }

    public static class PortraitAnchorExtensions
    {
        public static PortraitAnchor GetOpposite(this PortraitAnchor anchor)
        {
            switch (anchor)
            {
                case PortraitAnchor.TOP_LEFT:
                    return PortraitAnchor.TOP_RIGHT;

                case PortraitAnchor.TOP_RIGHT:
                    return PortraitAnchor.TOP_LEFT;

                default:
                    return PortraitAnchor.NONE;
            }
        }
    }
}