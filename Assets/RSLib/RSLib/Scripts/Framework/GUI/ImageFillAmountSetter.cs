namespace RSLib.Framework.GUI
{
    using UnityEngine;

    [RequireComponent(typeof(UnityEngine.UI.Image))]
    public class ImageFillAmountSetter : MonoBehaviour
    {
        [SerializeField] private Data.Float _dataFloat = null;

        private UnityEngine.UI.Image _image;
        
        private void OnValueChanged(RSLib.Data.Float.ValueChangedEventArgs args)
        {
            _image.fillAmount = args.New;
        }
        
        private void Awake()
        {
            _image = GetComponent<UnityEngine.UI.Image>();
            if (_image == null)
            {
                Debug.LogWarning($"No image found on {transform.name} to use {nameof(ImageFillAmountSetter)}!", gameObject);
                return;
            }
            
            if (_image.type != UnityEngine.UI.Image.Type.Filled)
                Debug.LogWarning($"A {nameof(ImageFillAmountSetter)} instance is attached on an image that is NOT set to {nameof(UnityEngine.UI.Image.Type.Filled)}!", gameObject);

            if (_dataFloat == null)
            {
                Debug.LogWarning($"Missing {nameof(_dataFloat)} on {transform.name} to use {nameof(ImageFillAmountSetter)}!", gameObject);
                return;
            }

            _dataFloat.ValueChanged += OnValueChanged;
        }

        private void OnDestroy()
        {
            if (_dataFloat != null)
                _dataFloat.ValueChanged -= OnValueChanged;
        }
    }
}
