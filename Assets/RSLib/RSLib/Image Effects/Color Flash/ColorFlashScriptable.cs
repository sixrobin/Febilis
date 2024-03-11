namespace RSLib.ImageEffects
{
    using RSLib.Maths;
    using UnityEngine;

    [CreateAssetMenu(fileName = "New Color Flash Data", menuName = "RSLib/Image Effects/Color Flash")]
    public class ColorFlashScriptable : ScriptableObject
    {
        [SerializeField] private Color _color = Color.white;
        [SerializeField] private RSLib.Framework.OptionalFloat _overrideAlpha = new RSLib.Framework.OptionalFloat(1f, false);
        [SerializeField] private float _inDuration = 0f;
        [SerializeField] private float _duration = 0.2f;
        [SerializeField] private float _outDuration = 0f;
        [SerializeField] private Curve _inCurve = Curve.Linear;
        [SerializeField] private Curve _outCurve = Curve.Linear;

        public Color Color => _color;
        public float? Alpha => _overrideAlpha.Enabled ? _overrideAlpha.Value : Color.a;
        public float InDuration => _inDuration;
        public float Duration => _duration;
        public float OutDuration => _outDuration;
        public Curve InCurve => _inCurve;
        public Curve OutCurve => _outCurve;

        public float TotalDuration => InDuration + Duration + OutDuration;
    }
}