namespace RSLib.Data
{
    using UnityEngine;

    [CreateAssetMenu(fileName = "New Data Easing Curve", menuName = "RSLib/Data/Easing Curve", order = -50)]
    public class EasingCurve : ScriptableObject
    {
        public const RSLib.Maths.Curve DEFAULT = RSLib.Maths.Curve.Linear;

        [SerializeField] private RSLib.Maths.Curve _curve = DEFAULT;

        public RSLib.Maths.Curve Curve => _curve;
        
#region CONVERSION OPERATORS
        
        public static implicit operator RSLib.Maths.Curve(EasingCurve dataEasingCurve)
        {
            return dataEasingCurve.Curve;
        }

#endregion // CONVERSION OPERATORS

        public override string ToString()
        {
            return Curve.ToString();
        }
    }
}