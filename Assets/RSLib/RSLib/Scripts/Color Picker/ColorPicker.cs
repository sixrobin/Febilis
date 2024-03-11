namespace TheLastStand.View.LevelEditor
{
    using UnityEngine;
    using UnityEngine.UI;

    [System.Serializable]
    public class ColorEvent : UnityEngine.Events.UnityEvent<Color> { }

    [DisallowMultipleComponent]
    public class ColorPicker : MonoBehaviour
    {
        [SerializeField] private RectTransform _colorPickerRect = null;
        [SerializeField] private Image _colorPickerImage = null;
        [SerializeField] private Image _previewImage = null;
        [SerializeField] private TMPro.TextMeshProUGUI _colorHexText = null;
        [SerializeField] private Button _hexToClipboardButton = null;

        [Tooltip("Defines how the color should be updated by user input.")]
        [SerializeField] private PickType _pickType = PickType.DRAG;
        [Tooltip("Does not take fully transparent pixels into account.")]
        [SerializeField] private bool _ignoreTransparent = true;
        [Tooltip("Only pick the tint and always set the color as opaque.")]
        [SerializeField] private bool _ignoreAlpha = true;

        [SerializeField] private ColorEvent _onColorHovered = new ColorEvent();
        [SerializeField] private ColorEvent _onColorPicked = new ColorEvent();

#pragma warning disable IDE0052
#pragma warning disable CS0414
        [SerializeField] private Color _hoveredColorPreview = Color.white;
        [SerializeField] private Color _clickedColorPreview = Color.white;
#pragma warning restore IDE0052
#pragma warning restore CS0414

        private Texture2D _colorPickerTexture;

        private Color _lastPickedColor;
        private bool _previousMousePressed;
        private bool _mousePressed;

        public delegate void ColorEventHandler(Color color);
        public ColorEventHandler ColorHovered;
        public ColorEventHandler ColorPicked;

        public enum PickType
        {
            [InspectorName("Hover")] HOVER,
            [InspectorName("Click")] CLICK,
            [InspectorName("Drag")] DRAG
        }

        public ColorEvent OnColorHovered => _onColorHovered;
        public ColorEvent OnColorPicked => _onColorPicked;

        public string LastPickedColorToHtmlString => _ignoreAlpha
                                                    ? ColorUtility.ToHtmlStringRGB(_lastPickedColor)
                                                    : ColorUtility.ToHtmlStringRGBA(_lastPickedColor);

        private void PickColor(Color color)
        {
            OnColorPicked?.Invoke(color);
            ColorPicked?.Invoke(color);

            _clickedColorPreview = color;
            _lastPickedColor = color;

            if (_previewImage != null)
                _previewImage.color = color;

            if (_colorHexText != null)
                _colorHexText.text = LastPickedColorToHtmlString;
        }

        private void UpdateColor()
        {
            if (!RectTransformUtility.RectangleContainsScreenPoint(_colorPickerRect, Input.mousePosition))
                return;

            Rect colorPickerRect = _colorPickerRect.rect;
            float width = colorPickerRect.width;
            float height = colorPickerRect.height;

            RectTransformUtility.ScreenPointToLocalPointInRectangle(_colorPickerRect, Input.mousePosition, null, out Vector2 delta);
            delta += _colorPickerRect.rect.size * 0.5f;

            float x = delta.x / width;
            float y = delta.y / height;

            Color color = _colorPickerTexture.GetPixel(Mathf.RoundToInt(x * _colorPickerTexture.width), Mathf.RoundToInt(y * _colorPickerTexture.height));
            
            if (_ignoreTransparent && color.a == 0f)
                return;

            if (_ignoreAlpha && color.a < 1f)
                color = new Color(color.r, color.g, color.b, 1f);

            OnColorHovered?.Invoke(color);
            ColorHovered?.Invoke(color);

            _hoveredColorPreview = color;

            bool colorHasChanged = _lastPickedColor != color;
            if (colorHasChanged)
            {
                switch (_pickType)
                {
                    case PickType.CLICK:
                        if (_mousePressed && !_previousMousePressed)
                            PickColor(color);
                        break;

                    case PickType.HOVER:
                        PickColor(color);
                        break;

                    case PickType.DRAG:
                        if (_mousePressed)
                            PickColor(color);
                        break;

                    default:
                        Debug.LogError($"Unhandled color pick type {_pickType}.");
                        break;
                }
            }
        }

        private void CopyPickedColorHexToClipboard()
        {
            GUIUtility.systemCopyBuffer = LastPickedColorToHtmlString;
        }

        private void Start()
        {
            _colorPickerTexture = _colorPickerImage.sprite.texture;

            if (_hexToClipboardButton != null)
                _hexToClipboardButton.onClick.AddListener(CopyPickedColorHexToClipboard);
        }

        private void Update()
        {
            _previousMousePressed = _mousePressed;
            _mousePressed = Input.GetMouseButton(0);
         
            UpdateColor();
            // TODO: Add some icon to show last picked color position. Might need to changed it as a nullable color.
        }

        private void OnDestroy()
        {
            if (_hexToClipboardButton != null)
                _hexToClipboardButton.onClick.RemoveListener(CopyPickedColorHexToClipboard);
        }
    }
}