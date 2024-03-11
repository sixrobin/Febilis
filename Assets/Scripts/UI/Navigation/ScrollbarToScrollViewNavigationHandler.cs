namespace Templar.UI
{
    public class ScrollbarToScrollViewNavigationHandler : UnityEngine.UI.Selectable
    {
        IScrollViewClosestItemGetter _closestItemGetter;

        public void SetClosestItemGetter(IScrollViewClosestItemGetter closestItemGetter)
        {
            _closestItemGetter = closestItemGetter;
        }

        public override void OnSelect(UnityEngine.EventSystems.BaseEventData eventData)
        {
            base.OnSelect(eventData);
            Navigation.UINavigationManager.Select(_closestItemGetter.GetClosestItemToScrollbar());
        }

        public override void OnDeselect(UnityEngine.EventSystems.BaseEventData eventData)
        {
            base.OnDeselect(eventData);
        }
    }
}