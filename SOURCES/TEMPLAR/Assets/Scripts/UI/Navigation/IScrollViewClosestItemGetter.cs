namespace Templar.UI
{
    public interface IScrollViewClosestItemGetter
    {
        ScrollbarToScrollViewNavigationHandler ScrollbarToScrollViewNavigationHandler { get; }
        UnityEngine.GameObject GetClosestItemToScrollbar();
    }
}