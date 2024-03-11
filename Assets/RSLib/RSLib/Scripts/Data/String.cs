namespace RSLib.Dynamics
{
    using UnityEngine;

    [CreateAssetMenu(fileName = "New Data String", menuName = "RSLib/Data/String")]
    public class String : ScriptableObject
    {
        public struct ValueChangedEventArgs
        {
            public string Previous;
            public string New;
        }

        [SerializeField] private string _value = string.Empty;

        public delegate void ValueChangedEventHandler(ValueChangedEventArgs args);
        public event ValueChangedEventHandler ValueChanged;

        public string Value
        {
            get => _value;
            set
            {
                ValueChangedEventArgs valueChangedEventArgs = new ValueChangedEventArgs()
                {
                    Previous = _value,
                    New = value
                };

                _value = value;
                ValueChanged?.Invoke(valueChangedEventArgs);
            }
        }
        
#region CONVERSION OPERATORS
        
        public static implicit operator string(String dataString)
        {
            return dataString.Value;
        }

#endregion // CONVERSION OPERATORS

        public override string ToString()
        {
            return Value;
        }
    }
}