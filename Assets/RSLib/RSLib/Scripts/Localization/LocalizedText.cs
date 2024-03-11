namespace RSLib.Localization
{
    using UnityEngine;

    [RequireComponent(typeof(TMPro.TextMeshProUGUI))]
    public class LocalizedText : MonoBehaviour
    {
        [SerializeField] private string _localizationKey = string.Empty;
        
        private TMPro.TextMeshProUGUI _text;
        
        private void Localize()
        {
            _text.text = Localizer.Get(_localizationKey);
        }
        
        private void OnEnable()
        {
            if (_text == null)
            {
                _text = GetComponent<TMPro.TextMeshProUGUI>();
                if (_text == null)
                {
                    Debug.LogWarning($"No {nameof(TMPro.TextMeshProUGUI)} found on {transform.name} to use {nameof(LocalizedText)}!", gameObject);
                    return;
                }
            }

            Localize();
            Localizer.LanguageChanged += Localize;
        }

        private void OnDisable()
        {
            Localizer.LanguageChanged -= Localize;
        }
    }
}