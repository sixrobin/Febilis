namespace RSLib.Framework.GUI
{
    using UnityEngine;
    using UnityEngine.EventSystems;
    
    /// <summary>
    /// Component that can be attached to Graphics to handle pointer events without having to implement them.
    /// This class can also be derived and its methods overriden from if extra code logic is needed.
    /// </summary>
    public class PointerEventsHandler : MonoBehaviour, IPointerClickHandler, IPointerDownHandler, IPointerEnterHandler, IPointerExitHandler, IPointerUpHandler
    {
        [SerializeField] private UnityEngine.Events.UnityEvent _onPointerClick = null;
        [SerializeField] private UnityEngine.Events.UnityEvent _onPointerDown = null;
        [SerializeField] private UnityEngine.Events.UnityEvent _onPointerEnter = null;
        [SerializeField] private UnityEngine.Events.UnityEvent _onPointerExit = null;
        [SerializeField] private UnityEngine.Events.UnityEvent _onPointerUp = null;

        public delegate void PointerEventHandler(PointerEventsHandler pointerEventHandler);

        public event PointerEventHandler PointerClick;
        public event PointerEventHandler PointerDown;
        public event PointerEventHandler PointerEnter;
        public event PointerEventHandler PointerExit;
        public event PointerEventHandler PointerUp;

        public RectTransform RectTransform { get; private set; }

        public virtual void OnPointerClick(PointerEventData eventData)
        {
            _onPointerClick?.Invoke();
            PointerClick?.Invoke(this);
        }

        public virtual void OnPointerDown(PointerEventData eventData)
        {
            _onPointerDown?.Invoke();
            PointerDown?.Invoke(this);
        }

        public virtual void OnPointerEnter(PointerEventData eventData)
        {
            _onPointerEnter?.Invoke();
            PointerEnter?.Invoke(this);
        }

        public virtual void OnPointerExit(PointerEventData eventData)
        {
            _onPointerExit?.Invoke();
            PointerExit?.Invoke(this);
        }

        public virtual void OnPointerUp(PointerEventData eventData)
        {
            _onPointerUp?.Invoke();
            PointerUp?.Invoke(this);
        }

        private void Awake()
        {
            RectTransform = GetComponent<RectTransform>();
        }
    }
}