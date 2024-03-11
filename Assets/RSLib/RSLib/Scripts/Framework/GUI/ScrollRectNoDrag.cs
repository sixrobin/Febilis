namespace RSLib.Framework.GUI
{
    public class ScrollRectNoDrag : UnityEngine.UI.ScrollRect
    {
        public override void OnBeginDrag(UnityEngine.EventSystems.PointerEventData eventData) { }
        public override void OnDrag(UnityEngine.EventSystems.PointerEventData eventData) { }
        public override void OnEndDrag(UnityEngine.EventSystems.PointerEventData eventData) { }
    }
}