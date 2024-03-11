namespace RSLib.Data
{
    using UnityEngine;

    [CreateAssetMenu(fileName = "New Data Animation Curve", menuName = "RSLib/Data/Animation Curve", order = -50)]
    public class AnimationCurve : ScriptableObject
    {
        [SerializeField] private UnityEngine.AnimationCurve _curve = UnityEngine.AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);

        public static UnityEngine.AnimationCurve Default => UnityEngine.AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);

        public UnityEngine.AnimationCurve Curve => _curve;

        public float Evaluate(float time)
        {
            return Curve?.Evaluate(time) ?? Default.Evaluate(time);
        }
    }
}