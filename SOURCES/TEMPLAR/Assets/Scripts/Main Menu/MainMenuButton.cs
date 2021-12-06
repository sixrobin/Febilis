namespace Templar.MainMenu
{
    using UnityEngine;

    public class MainMenuButton : MonoBehaviour,
        UnityEngine.EventSystems.IPointerEnterHandler,
        UnityEngine.EventSystems.IPointerExitHandler,
        UnityEngine.EventSystems.ISelectHandler,
        UnityEngine.EventSystems.IDeselectHandler
    {
        [SerializeField] private RSLib.Framework.GUI.EnhancedButton _btn = null;
        [SerializeField] private TMPro.TextMeshProUGUI _btnText = null;
        [SerializeField] private GameObject _highlight = null;
        [SerializeField] private Color _highlightTextColor = Color.white;
        [SerializeField] private Color _baseTextColor = Color.white;

        public delegate void MainMenuButtonEventHandler(MainMenuButton mainMenuButton);

        public static MainMenuButtonEventHandler Selected;
        public static MainMenuButtonEventHandler Deselected;

        public UnityEngine.UI.Button Button => _btn;

        public void OnSelect(UnityEngine.EventSystems.BaseEventData eventData)
        {
            Selected?.Invoke(this);
        }

        public void OnDeselect(UnityEngine.EventSystems.BaseEventData eventData)
        {
            Deselected?.Invoke(this);
        }

        public void OnPointerEnter(UnityEngine.EventSystems.PointerEventData eventData)
        {
            OnSelect(null);
        }

        public void OnPointerExit(UnityEngine.EventSystems.PointerEventData eventData)
        {
            OnDeselect(null);
        }

        public void Highlight(bool state)
        {
            _highlight.SetActive(state);
            _btnText.color = state ? _highlightTextColor : _baseTextColor;
        }

        private void Awake()
        {
            _btnText.color = _baseTextColor;
        }
    }
}