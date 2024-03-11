namespace RSLib.Data
{
    using UnityEngine;
    
    [CreateAssetMenu(fileName = "New Data Int", menuName = "RSLib/Data/Int", order = -100)]
    public class Int : ScriptableObject
    {
        public struct ValueChangedEventArgs
        {
            public int Previous;
            public int New;
        }

        [SerializeField] private int _value = 0;
        [SerializeField] private Vector2Int _range = new Vector2Int(int.MinValue, int.MaxValue);

        public delegate void ValueChangedEventHandler(ValueChangedEventArgs args);
        public event ValueChangedEventHandler ValueChanged;

        public int Value
        {
            get => _value;
            set
            {
                int newValue = Mathf.Clamp(value, _range.x, _range.y);

                ValueChangedEventArgs valueChangedEventArgs = new ValueChangedEventArgs
                {
                    Previous = _value,
                    New = newValue
                };

                _value = newValue;
                ValueChanged?.Invoke(valueChangedEventArgs);
            }
        }

        public Vector2Int Range => _range;
        public int Min => Range.x;
        public int Max => Range.y;
        
        private void OnValidate()
        {
            Value = Mathf.Clamp(_value, _range.x, _range.y);
        }

        #region ARITHMETIC OPERATORS
        
        public static Int operator +(Int a, Int b)
        {
            a.Value += b.Value;
            return a;
        }
        public static Int operator +(Int a, int b)
        {
            a.Value += b;
            return a;
        }
        public static int operator +(int a, Int b)
        {
            a += b.Value;
            return a;
        }

        public static Int operator -(Int a, Int b)
        {
            a.Value -= b.Value;
            return a;
        }
        public static Int operator -(Int a, int b)
        {
            a.Value -= b;
            return a;
        }
        public static int operator -(int a, Int b)
        {
            a -= b.Value;
            return a;
        }

        public static Int operator *(Int a, Int b)
        {
            a.Value *= b.Value;
            return a;
        }
        public static Int operator *(Int a, int b)
        {
            a.Value *= b;
            return a;
        }
        public static int operator *(int a, Int b)
        {
            a *= b.Value;
            return a;
        }

        public static Int operator /(Int a, Int b)
        {
            a.Value /= b.Value;
            return a;
        }
        public static Int operator /(Int a, int b)
        {
            a.Value /= b;
            return a;
        }
        public static int operator /(int a, Int b)
        {
            a /= b.Value;
            return a;
        }

        public static Int operator %(Int a, Int b)
        {
            a.Value %= b.Value;
            return a;
        }
        public static Int operator %(Int a, int b)
        {
            a.Value %= b;
            return a;
        }
        public static int operator %(int a, Int b)
        {
            a %= b.Value;
            return a;
        }

        public static bool operator >(Int a, Int b)
        {
            return a.Value > b.Value;
        }
        public static bool operator >(Int a, int b)
        {
            return a.Value > b;
        }
        public static bool operator >(int a, Int b)
        {
            return a > b.Value;
        }
        
        public static bool operator <(Int a, Int b)
        {
            return a.Value < b.Value;
        }
        public static bool operator <(Int a, int b)
        {
            return a.Value < b;
        }
        public static bool operator <(int a, Int b)
        {
            return a < b.Value;
        }
        
        public static bool operator >=(Int a, Int b)
        {
            return a.Value >= b.Value;
        }
        public static bool operator >=(Int a, int b)
        {
            return a.Value >= b;
        }
        public static bool operator >=(int a, Int b)
        {
            return a >= b.Value;
        }
        
        public static bool operator <=(Int a, Int b)
        {
            return a.Value <= b.Value;
        }
        public static bool operator <=(Int a, int b)
        {
            return a.Value <= b;
        }
        public static bool operator <=(int a, Int b)
        {
            return a <= b.Value;
        }
        
        #endregion // ARITHMETIC OPERATORS
        
        #region CONVERSION OPERATORS
        
        public static implicit operator int(Int dataInt)
        {
            return dataInt.Value;
        }
        
        #endregion // CONVERSION OPERATORS

        protected bool Equals(Int other)
        {
            return base.Equals(other) && _value == other._value && _range.Equals(other._range);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
                return false;
            
            if (ReferenceEquals(this, obj))
                return true;
            
            if (obj.GetType() != GetType())
                return false;
            
            return Equals((Int) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hashCode = base.GetHashCode();
                hashCode = (hashCode * 397) ^ _value;
                hashCode = (hashCode * 397) ^ _range.GetHashCode();
                return hashCode;
            }
        }
        
        public override string ToString()
        {
            return Value.ToString();
        }
    }
    
    [System.Serializable]
    public class IntField
    {
        [SerializeField] private Int _dataInt = null;
        [SerializeField] private int _valueInt = 0;
        [SerializeField] private bool _useDataValue = true;

        public int Value => _useDataValue ? _dataInt : _valueInt;
        
        public void Set(int value)
        {
            if (_useDataValue) _dataInt.Value = value;
            else _valueInt = value;
        }
        public void Set(Int dataInt)
        {
            if (_useDataValue) _dataInt.Value = dataInt;
            else _valueInt = dataInt;
        }
        public void Set(IntField intField)
        {
            if (_useDataValue) _dataInt.Value = intField;
            else _valueInt = intField;
        }
        
        #region ARITHMETIC OPERATORS
        
        public static IntField operator +(IntField a, IntField b)
        {
            if (a._useDataValue) a._dataInt += b.Value;
            else a._valueInt += b.Value;
            return a;
        }
        public static IntField operator +(IntField a, int b)
        {
            if (a._useDataValue) a._dataInt += b;
            else a._valueInt += b;
            return a;
        }
        public static int operator +(int a, IntField b)
        {
            a += b.Value;
            return a;
        }

        public static IntField operator -(IntField a, IntField b)
        {
            if (a._useDataValue) a._dataInt -= b.Value;
            else a._valueInt -= b.Value;
            return a;
        }
        public static IntField operator -(IntField a, int b)
        {
            if (a._useDataValue) a._dataInt -= b;
            else a._valueInt -= b;
            return a;
        }
        public static int operator -(int a, IntField b)
        {
            a -= b.Value;
            return a;
        }

        public static IntField operator *(IntField a, IntField b)
        {
            if (a._useDataValue) a._dataInt *= b.Value;
            else a._valueInt *= b.Value;
            return a;
        }
        public static IntField operator *(IntField a, int b)
        {
            if (a._useDataValue) a._dataInt *= b;
            else a._valueInt *= b;
            return a;
        }
        public static int operator *(int a, IntField b)
        {
            a *= b.Value;
            return a;
        }

        public static IntField operator /(IntField a, IntField b)
        {
            if (a._useDataValue) a._dataInt /= b.Value;
            else a._valueInt /= b.Value;
            return a;
        }
        public static IntField operator /(IntField a, int b)
        {
            if (a._useDataValue) a._dataInt /= b;
            else a._valueInt /= b;
            return a;
        }
        public static int operator /(int a, IntField b)
        {
            a /= b.Value;
            return a;
        }

        public static IntField operator %(IntField a, IntField b)
        {
            if (a._useDataValue) a._dataInt %= b.Value;
            else a._valueInt %= b.Value;
            return a;
        }
        public static IntField operator %(IntField a, int b)
        {
            if (a._useDataValue) a._dataInt %= b;
            else a._valueInt %= b;
            return a;
        }
        public static int operator %(int a, IntField b)
        {
            a %= b.Value;
            return a;
        }

        public static bool operator >(IntField a, IntField b)
        {
            return a.Value > b.Value;
        }
        public static bool operator >(IntField a, int b)
        {
            return a.Value > b;
        }
        public static bool operator >(int a, IntField b)
        {
            return a > b.Value;
        }
        
        public static bool operator <(IntField a, IntField b)
        {
            return a.Value < b.Value;
        }
        public static bool operator <(IntField a, int b)
        {
            return a.Value < b;
        }
        public static bool operator <(int a, IntField b)
        {
            return a < b.Value;
        }
        
        public static bool operator >=(IntField a, IntField b)
        {
            return a.Value >= b.Value;
        }
        public static bool operator >=(IntField a, int b)
        {
            return a.Value >= b;
        }
        public static bool operator >=(int a, IntField b)
        {
            return a >= b.Value;
        }
        
        public static bool operator <=(IntField a, IntField b)
        {
            return a.Value <= b.Value;
        }
        public static bool operator <=(IntField a, int b)
        {
            return a.Value <= b;
        }
        public static bool operator <=(int a, IntField b)
        {
            return a <= b.Value;
        }
        
        #endregion // ARITHMETIC OPERATORS
        
        #region CONVERSION OPERATORS
        
        public static implicit operator int(IntField intField)
        {
            return intField.Value;
        }
        
        #endregion // CONVERSION OPERATORS
    }
}