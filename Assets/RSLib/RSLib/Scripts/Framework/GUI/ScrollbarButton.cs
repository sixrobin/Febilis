namespace RSLib.Framework.GUI
{
    using UnityEngine;

    [RequireComponent(typeof(UnityEngine.UI.Button))]
    public class ScrollbarButton : MonoBehaviour
    {
        [SerializeField] private UnityEngine.UI.Scrollbar _scrollbar = null;
        [SerializeField, Min(0f)] private float _sensitivity = 1f;
        [SerializeField] private bool _positive = true;
        [SerializeField] private bool _clampValue = true;

        private UnityEngine.UI.Button _button;

        private void OnButtonClick()
        {
            float newValue = _scrollbar.value + (_positive ? _sensitivity : -_sensitivity);
            if (_clampValue)
                newValue = Mathf.Clamp01(newValue);

            _scrollbar.value = newValue;
        }
        
        private void Awake()
        {
            if (_scrollbar == null)
            {
                Debug.LogError($"{nameof(_scrollbar)} reference is missing on {transform.name}. Disabling component.", gameObject);
                enabled = false;
            }
            
            _button = GetComponent<UnityEngine.UI.Button>();
            _button.onClick.AddListener(OnButtonClick);
        }

        private void OnDestroy()
        {
            _button.onClick.RemoveListener(OnButtonClick);
        }
    }
}