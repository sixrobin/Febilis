namespace RSLib.Framework.GUI
{
    using UnityEngine;
#if UNITY_EDITOR
    using UnityEditor;
#endif

    /// <summary>
    /// Enhancements for native Unity Button component, mostly useful to add events for pointer enter
    /// and exit event. Also adds an event when interactable value changes and some tools to
    /// control the button's text if it exists (requires TMPro).
    /// </summary>
    public class EnhancedButton : UnityEngine.UI.Button
    {
        [System.Serializable]
        public class BoolEvent : UnityEngine.Events.UnityEvent<bool>
        {
        }

        [SerializeField] private UnityEngine.Events.UnityEvent _onPointerEnter = null;
        [SerializeField] private UnityEngine.Events.UnityEvent _onPointerExit = null;
        [SerializeField] private BoolEvent _onInteractableChanged = null;

        [SerializeField] private TMPro.TextMeshProUGUI _buttonText = null;
        [SerializeField] private Color _nonInteractableTextColor = Color.grey;

        private Color _initTextColor;

        public delegate void PointerEventHandler();
        public delegate void InteractableChangedEventHandler(bool interactable);

        public event PointerEventHandler PointerEnter;
        public event PointerEventHandler PointerExit;
        public event InteractableChangedEventHandler InteractableChanged;

        public bool Interactable
        {
            get => interactable;
            set
            {
                if (interactable != value)
                {
                    _onInteractableChanged?.Invoke(value);
                    InteractableChanged?.Invoke(value);
                }

                interactable = value;
                if (_buttonText != null)
                    _buttonText.color = interactable ? _initTextColor : _nonInteractableTextColor;
            }
        }

        public override void OnPointerEnter(UnityEngine.EventSystems.PointerEventData eventData)
        {
            base.OnPointerEnter(eventData);

            if (interactable)
            {
                _onPointerEnter?.Invoke();
                PointerEnter?.Invoke();
            }
        }

        public override void OnPointerExit(UnityEngine.EventSystems.PointerEventData eventData)
        {
            base.OnPointerExit(eventData);

            if (interactable)
            {
                _onPointerExit?.Invoke();
                PointerExit?.Invoke();
            }
        }

        public void SetText(string text)
        {
            if (_buttonText == null)
                return;

            _buttonText.text = text;
        }

        public void Deselect()
        {
            DoStateTransition(SelectionState.Normal, true);
        }

        protected override void Awake()
        {
            base.Awake();

            if (_buttonText != null)
            {
                _initTextColor = _buttonText.color;
                if (!interactable)
                    _buttonText.color = _nonInteractableTextColor;
            }
        }
    }

#if UNITY_EDITOR
    [CustomEditor(typeof(EnhancedButton)), CanEditMultipleObjects]
    public class BetterButtonEditor : UnityEditor.UI.ButtonEditor
    {
        private SerializedProperty _onPointerEnterProperty;
        private SerializedProperty _onPointerExitProperty;
        private SerializedProperty _onInteractableChangedProperty;
        private SerializedProperty _buttonTextProperty;
        private SerializedProperty _nonInteractableTextColorProperty;

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.PropertyField(_buttonTextProperty);
            if (_buttonTextProperty.objectReferenceValue != null)
                EditorGUILayout.PropertyField(_nonInteractableTextColorProperty);

            serializedObject.ApplyModifiedProperties();

            EditorGUILayout.Space();
            base.OnInspectorGUI();

            serializedObject.Update();

            EditorGUILayout.PropertyField(_onPointerEnterProperty);
            EditorGUILayout.PropertyField(_onPointerExitProperty);
            EditorGUILayout.PropertyField(_onInteractableChangedProperty);

            serializedObject.ApplyModifiedProperties();
        }

        protected override void OnEnable()
        {
            base.OnEnable();

            _onPointerEnterProperty = serializedObject.FindProperty("_onPointerEnter");
            _onPointerExitProperty = serializedObject.FindProperty("_onPointerExit");
            _onInteractableChangedProperty = serializedObject.FindProperty("_onInteractableChanged");
            _buttonTextProperty = serializedObject.FindProperty("_buttonText");
            _nonInteractableTextColorProperty = serializedObject.FindProperty("_nonInteractableTextColor");
        }
    }
#endif
}