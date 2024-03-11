namespace RSLib.Data
{
    using UnityEngine;

    [CreateAssetMenu(fileName = "New Data Tween", menuName = "RSLib/Data/Tween", order = -50)]
    public class Tween : ScriptableObject
    {
        [SerializeField] private float _duration = 0f;
        [SerializeField] private RSLib.Maths.Curve _curve = RSLib.Maths.Curve.InOutSine;

        public float Duration => Mathf.Max(0f, _duration);
        public RSLib.Maths.Curve Curve => _curve;

        private void OnValidate()
        {
            _duration = Mathf.Max(0f, _duration);
        }
    }
}