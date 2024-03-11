namespace RSLib.Framework.GUI
{
    using UnityEngine;
#if UNITY_EDITOR
    using UnityEditor;
#endif

    /// <summary>
    /// Enhancements for native Unity Toggle component, mostly useful to add events for pointer enter exit and click event.
    /// Also adds an option to deselect toggle when clicked.
    /// </summary>
    public class EnhancedToggle : UnityEngine.UI.Toggle
    {
        [SerializeField] private bool _autoDeselection = true;

        [SerializeField] private UnityEngine.Events.UnityEvent _onPointerClick = null;
        [SerializeField] private UnityEngine.Events.UnityEvent _onPointerEnter = null;
        [SerializeField] private UnityEngine.Events.UnityEvent _onPointerExit = null;

        public delegate void PointerEventHandler(EnhancedToggle source);

        public event PointerEventHandler PointerClick;
        public event PointerEventHandler PointerEnter;
        public event PointerEventHandler PointerExit;

        public override void OnPointerClick(UnityEngine.EventSystems.PointerEventData eventData)
        {
            base.OnPointerClick(eventData);

            if (interactable)
            {
                _onPointerClick?.Invoke();
                PointerClick?.Invoke(this);

                if (_autoDeselection && UnityEngine.EventSystems.EventSystem.current != null)
                    UnityEngine.EventSystems.EventSystem.current.SetSelectedGameObject(null);
            }
        }

        public override void OnPointerEnter(UnityEngine.EventSystems.PointerEventData eventData)
        {
            base.OnPointerEnter(eventData);

            if (interactable)
            {
                _onPointerEnter?.Invoke();
                PointerEnter?.Invoke(this);
            }
        }

        public override void OnPointerExit(UnityEngine.EventSystems.PointerEventData eventData)
        {
            base.OnPointerExit(eventData);

            if (interactable)
            {
                _onPointerExit?.Invoke();
                PointerExit?.Invoke(this);
            }
        }
    }

#if UNITY_EDITOR
    [CustomEditor(typeof(EnhancedToggle)), CanEditMultipleObjects]
    public class EnhancedToggleEditor : UnityEditor.UI.ToggleEditor
    {
        private SerializedProperty _autoDeselectionProperty;
        private SerializedProperty _onPointerClickProperty;
        private SerializedProperty _onPointerEnterProperty;
        private SerializedProperty _onPointerExitProperty;

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.PropertyField(_autoDeselectionProperty);

            serializedObject.ApplyModifiedProperties();

            base.OnInspectorGUI();
            EditorGUILayout.Separator();

            serializedObject.Update();

            EditorGUILayout.Space();

            EditorGUILayout.PropertyField(_onPointerClickProperty);
            EditorGUILayout.PropertyField(_onPointerEnterProperty);
            EditorGUILayout.PropertyField(_onPointerExitProperty);

            serializedObject.ApplyModifiedProperties();
        }

        protected override void OnEnable()
        {
            base.OnEnable();

            _autoDeselectionProperty = serializedObject.FindProperty("_autoDeselection");
            _onPointerClickProperty = serializedObject.FindProperty("_onPointerClick");
            _onPointerEnterProperty = serializedObject.FindProperty("_onPointerEnter");
            _onPointerExitProperty = serializedObject.FindProperty("_onPointerExit");
        }
    }
#endif
}