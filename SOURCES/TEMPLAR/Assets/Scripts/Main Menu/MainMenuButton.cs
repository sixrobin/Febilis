namespace Templar.MainMenu
{
    using UnityEngine;

    public class MainMenuButton : UnityEngine.UI.Selectable, UnityEngine.EventSystems.ISelectHandler
    {
        [SerializeField] private RSLib.Framework.GUI.PointerEventsHandler _pointerEventsHandler = null;
        [SerializeField] private TMPro.TextMeshProUGUI _btnText = null;
        [SerializeField] private GameObject _highlight = null;
        [SerializeField] private Color _highlightTextColor = Color.white;
        [SerializeField] private Color _baseTextColor = Color.white;

        public override void OnSelect(UnityEngine.EventSystems.BaseEventData eventData)
        {
            base.OnSelect(eventData);

            _highlight.SetActive(true);
            _btnText.color = _highlightTextColor;
        }

        public override void OnDeselect(UnityEngine.EventSystems.BaseEventData eventData)
        {
            base.OnDeselect(eventData);

            _highlight.SetActive(false);
            _btnText.color = _baseTextColor;
        }

        private void OnPointerEnter(RSLib.Framework.GUI.PointerEventsHandler pointerEventHandler)
        {
            OnSelect(null);
        }

        private void OnPointerExit(RSLib.Framework.GUI.PointerEventsHandler pointerEventHandler)
        {
            OnDeselect(null);
        }

        protected override void Awake()
        {
            base.Awake();

            _pointerEventsHandler.PointerEnter += OnPointerEnter;
            _pointerEventsHandler.PointerExit += OnPointerExit;

            _btnText.color = _baseTextColor;
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();

            _pointerEventsHandler.PointerEnter -= OnPointerEnter;
            _pointerEventsHandler.PointerExit -= OnPointerExit;
        }
    }
}