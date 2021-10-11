namespace Templar.UI.Settings
{
    using UnityEngine;

    public abstract class SettingView : UnityEngine.UI.Selectable, UnityEngine.EventSystems.ISelectHandler
    {
        [Header("SETTING VIEW BASE")]
        [SerializeField] private RSLib.Framework.GUI.PointerEventsHandler _pointerEventsHandler = null;

        public RSLib.Framework.GUI.PointerEventsHandler PointerEventsHandler => _pointerEventsHandler;

        public abstract UnityEngine.UI.Selectable Selectable { get; }

        public override void OnSelect(UnityEngine.EventSystems.BaseEventData eventData)
        {
            base.OnSelect(eventData);
            PointerEventsHandler.OnPointerEnter(null);

            Navigation.UINavigationManager.Select(Selectable.gameObject);
        }

        public abstract void Init();

        public override void OnPointerEnter(UnityEngine.EventSystems.PointerEventData eventData)
        {
            base.OnPointerEnter(eventData);
            PointerEventsHandler.OnPointerEnter(eventData);
        }

        public override void OnPointerExit(UnityEngine.EventSystems.PointerEventData eventData)
        {
            base.OnPointerExit(eventData);
            PointerEventsHandler.OnPointerExit(eventData);
        }

        public override void OnPointerDown(UnityEngine.EventSystems.PointerEventData eventData)
        {
            base.OnPointerDown(eventData);
            PointerEventsHandler.OnPointerDown(eventData);
        }

        public override void OnPointerUp(UnityEngine.EventSystems.PointerEventData eventData)
        {
            base.OnPointerDown(eventData);
            PointerEventsHandler.OnPointerUp(eventData);
        }

#if UNITY_EDITOR
        protected override void Reset()
        {
            base.Reset();
            transition = Transition.None;
            _pointerEventsHandler = GetComponentInChildren<RSLib.Framework.GUI.PointerEventsHandler>();
        }
#endif
    }
}