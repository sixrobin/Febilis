namespace RSLib.Data
{
    using UnityEngine;

    [CreateAssetMenu(fileName = "New Data Color", menuName = "RSLib/Data/Color")]
    public class Color : ScriptableObject
    {
        [SerializeField] private UnityEngine.Color _color = UnityEngine.Color.white;

        public static UnityEngine.Color Default => UnityEngine.Color.magenta;
        
        public string HexCode => ColorUtility.ToHtmlStringRGBA(_color);
        
        public static implicit operator UnityEngine.Color(Color color)
        {
            return color._color;
        }
        
        public override string ToString()
        {
            return _color.ToString();
        }
    }
    
    [System.Serializable]
    public class ColorField
    {
        [SerializeField] private Color _dataColor = null;
        [SerializeField] private UnityEngine.Color _valueColor = UnityEngine.Color.white;
        [SerializeField] private bool _useDataValue = true;

        public UnityEngine.Color Value => _useDataValue ? _dataColor : _valueColor;

        #region CONVERSION OPERATORS
        
        public static implicit operator UnityEngine.Color(ColorField colorField)
        {
            return colorField.Value;
        }
        
        #endregion // CONVERSION OPERATORS
    }
}