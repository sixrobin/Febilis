namespace RSLib.Framework.GUI
{
    using UnityEngine;
#if UNITY_EDITOR
    using UnityEditor;
#endif
    
    /// <summary>
    /// Enhancements for native Unity Slider component, mostly useful to add events for pointer enter exit and click event.
    /// </summary>
    public class EnhancedSlider : UnityEngine.UI.Slider
    {
        [SerializeField] private UnityEngine.Events.UnityEvent _onPointerEnter = null;
        [SerializeField] private UnityEngine.Events.UnityEvent _onPointerExit = null;
        
        public delegate void PointerEventHandler(EnhancedSlider source);

        public event PointerEventHandler PointerEnter;
        public event PointerEventHandler PointerExit;
        
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
    [CustomEditor(typeof(EnhancedSlider)), CanEditMultipleObjects]
    public class EnhancedSliderEditor : UnityEditor.UI.SliderEditor
    {
        private SerializedProperty _onPointerEnterProperty;
        private SerializedProperty _onPointerExitProperty;

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            serializedObject.ApplyModifiedProperties();

            base.OnInspectorGUI();
            EditorGUILayout.Separator();

            serializedObject.Update();

            EditorGUILayout.Space();

            EditorGUILayout.PropertyField(_onPointerEnterProperty);
            EditorGUILayout.PropertyField(_onPointerExitProperty);

            serializedObject.ApplyModifiedProperties();
        }

        protected override void OnEnable()
        {
            base.OnEnable();

            _onPointerEnterProperty = serializedObject.FindProperty("_onPointerEnter");
            _onPointerExitProperty = serializedObject.FindProperty("_onPointerExit");
        }
    }
#endif
}