namespace RSLib.Data
{
    using UnityEngine;

    [CreateAssetMenu(fileName = "New Data Float", menuName = "RSLib/Data/Float", order = -100)]
    public class Float : ScriptableObject
    {
        public struct ValueChangedEventArgs
        {
            public float Previous;
            public float New;
        }

        [SerializeField] private float _value = 0;
        [SerializeField] private Vector2 _range = new Vector2(Mathf.NegativeInfinity, Mathf.Infinity);

        public delegate void ValueChangedEventHandler(ValueChangedEventArgs args);
        public event ValueChangedEventHandler ValueChanged;

        public float Value
        {
            get => _value;
            set
            {
                float newValue = Mathf.Clamp(value, _range.x, _range.y);

                ValueChangedEventArgs valueChangedEventArgs = new ValueChangedEventArgs
                {
                    Previous = _value,
                    New = newValue
                };

                _value = newValue;
                ValueChanged?.Invoke(valueChangedEventArgs);
            }
        }

        public Vector2 Range => _range;
        public float Min => Range.x;
        public float Max => Range.y;
        
        private void OnValidate()
        {
            Value = Mathf.Clamp(_value, _range.x, _range.y);
        }

        #region ARITHMETIC OPERATORS
        
        public static Float operator +(Float a, Float b)
        {
            a.Value += b.Value;
            return a;
        }
        public static Float operator +(Float a, float b)
        {
            a.Value += b;
            return a;
        }
        public static float operator +(float a, Float b)
        {
            a += b.Value;
            return a;
        }

        public static Float operator -(Float a, Float b)
        {
            a.Value -= b.Value;
            return a;
        }
        public static Float operator -(Float a, float b)
        {
            a.Value -= b;
            return a;
        }
        public static float operator -(float a, Float b)
        {
            a -= b.Value;
            return a;
        }

        public static Float operator *(Float a, Float b)
        {
            a.Value *= b.Value;
            return a;
        }
        public static Float operator *(Float a, float b)
        {
            a.Value *= b;
            return a;
        }
        public static float operator *(float a, Float b)
        {
            a *= b.Value;
            return a;
        }

        public static Float operator /(Float a, Float b)
        {
            a.Value /= b.Value;
            return a;
        }
        public static Float operator /(Float a, float b)
        {
            a.Value /= b;
            return a;
        }
        public static float operator /(float a, Float b)
        {
            a /= b.Value;
            return a;
        }

        public static Float operator %(Float a, Float b)
        {
            a.Value %= b.Value;
            return a;
        }
        public static Float operator %(Float a, float b)
        {
            a.Value %= b;
            return a;
        }
        public static float operator %(float a, Float b)
        {
            a %= b.Value;
            return a;
        }

        public static bool operator >(Float a, Float b)
        {
            return a.Value > b.Value;
        }
        public static bool operator >(Float a, float b)
        {
            return a.Value > b;
        }
        public static bool operator >(float a, Float b)
        {
            return a > b.Value;
        }
        
        public static bool operator <(Float a, Float b)
        {
            return a.Value < b.Value;
        }
        public static bool operator <(Float a, float b)
        {
            return a.Value < b;
        }
        public static bool operator <(float a, Float b)
        {
            return a < b.Value;
        }
        
        public static bool operator >=(Float a, Float b)
        {
            return a.Value >= b.Value;
        }
        public static bool operator >=(Float a, float b)
        {
            return a.Value >= b;
        }
        public static bool operator >=(float a, Float b)
        {
            return a >= b.Value;
        }
        
        public static bool operator <=(Float a, Float b)
        {
            return a.Value <= b.Value;
        }
        public static bool operator <=(Float a, float b)
        {
            return a.Value <= b;
        }
        public static bool operator <=(float a, Float b)
        {
            return a <= b.Value;
        }
        
        #endregion // ARITHMETIC OPERATORS

        #region CONVERSION OPERATORS
        
        public static implicit operator float(Float dataFloat)
        {
            return dataFloat.Value;
        }

        #endregion // CONVERSION OPERATORS
        
        protected bool Equals(Float other)
        {
            return base.Equals(other) && _value.Equals(other._value) && _range.Equals(other._range);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
                return false;
            
            if (ReferenceEquals(this, obj))
                return true;
            
            if (obj.GetType() != GetType())
                return false;
            
            return Equals((Float) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hashCode = base.GetHashCode();
                hashCode = (hashCode * 397) ^ _value.GetHashCode();
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
    public class FloatField
    {
        [SerializeField] private Float _dataFloat = null;
        [SerializeField] private float _valueFloat = 0f;
        [SerializeField] private bool _useDataValue = true;

        public float Value => _useDataValue ? _dataFloat : _valueFloat;

        public void Set(float value)
        {
            if (_useDataValue) _dataFloat.Value = value;
            else _valueFloat = value;
        }
        public void Set(Float dataFloat)
        {
            if (_useDataValue) _dataFloat.Value = dataFloat;
            else _valueFloat = dataFloat;
        }
        public void Set(FloatField floatField)
        {
            if (_useDataValue) _dataFloat.Value = floatField;
            else _valueFloat = floatField;
        }
        
        #region ARITHMETIC OPERATORS
        
        public static FloatField operator +(FloatField a, FloatField b)
        {
            if (a._useDataValue) a._dataFloat += b.Value;
            else a._valueFloat += b.Value;
            return a;
        }
        public static FloatField operator +(FloatField a, float b)
        {
            if (a._useDataValue) a._dataFloat += b;
            else a._valueFloat += b;
            return a;
        }
        public static float operator +(float a, FloatField b)
        {
            a += b.Value;
            return a;
        }

        public static FloatField operator -(FloatField a, FloatField b)
        {
            if (a._useDataValue) a._dataFloat -= b.Value;
            else a._valueFloat -= b.Value;
            return a;
        }
        public static FloatField operator -(FloatField a, float b)
        {
            if (a._useDataValue) a._dataFloat -= b;
            else a._valueFloat -= b;
            return a;
        }
        public static float operator -(float a, FloatField b)
        {
            a -= b.Value;
            return a;
        }

        public static FloatField operator *(FloatField a, FloatField b)
        {
            if (a._useDataValue) a._dataFloat *= b.Value;
            else a._valueFloat *= b.Value;
            return a;
        }
        public static FloatField operator *(FloatField a, float b)
        {
            if (a._useDataValue) a._dataFloat *= b;
            else a._valueFloat *= b;
            return a;
        }
        public static float operator *(float a, FloatField b)
        {
            a *= b.Value;
            return a;
        }

        public static FloatField operator /(FloatField a, FloatField b)
        {
            if (a._useDataValue) a._dataFloat /= b.Value;
            else a._valueFloat /= b.Value;
            return a;
        }
        public static FloatField operator /(FloatField a, float b)
        {
            if (a._useDataValue) a._dataFloat /= b;
            else a._valueFloat /= b;
            return a;
        }
        public static float operator /(float a, FloatField b)
        {
            a /= b.Value;
            return a;
        }

        public static FloatField operator %(FloatField a, FloatField b)
        {
            if (a._useDataValue) a._dataFloat %= b.Value;
            else a._valueFloat %= b.Value;
            return a;
        }
        public static FloatField operator %(FloatField a, float b)
        {
            if (a._useDataValue) a._dataFloat %= b;
            else a._valueFloat %= b;
            return a;
        }
        public static float operator %(float a, FloatField b)
        {
            a %= b.Value;
            return a;
        }

        public static bool operator >(FloatField a, FloatField b)
        {
            return a.Value > b.Value;
        }
        public static bool operator >(FloatField a, float b)
        {
            return a.Value > b;
        }
        public static bool operator >(float a, FloatField b)
        {
            return a > b.Value;
        }
        
        public static bool operator <(FloatField a, FloatField b)
        {
            return a.Value < b.Value;
        }
        public static bool operator <(FloatField a, float b)
        {
            return a.Value < b;
        }
        public static bool operator <(float a, FloatField b)
        {
            return a < b.Value;
        }
        
        public static bool operator >=(FloatField a, FloatField b)
        {
            return a.Value >= b.Value;
        }
        public static bool operator >=(FloatField a, float b)
        {
            return a.Value >= b;
        }
        public static bool operator >=(float a, FloatField b)
        {
            return a >= b.Value;
        }
        
        public static bool operator <=(FloatField a, FloatField b)
        {
            return a.Value <= b.Value;
        }
        public static bool operator <=(FloatField a, float b)
        {
            return a.Value <= b;
        }
        public static bool operator <=(float a, FloatField b)
        {
            return a <= b.Value;
        }
        
        #endregion // ARITHMETIC OPERATORS
        
        #region CONVERSION OPERATORS
        
        public static implicit operator float(FloatField floatField)
        {
            return floatField.Value;
        }
        
        #endregion // CONVERSION OPERATORS
    }
}