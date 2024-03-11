namespace Templar.Datas.MainMenu
{
    using UnityEngine;

    [CreateAssetMenu(fileName = "New Main Menu Fade Out Datas", menuName = "Datas/Main Menu Fade Out")]
    public class MainMenuFadeOutDatas : ScriptableObject
    {
        [Header("VIGNETTE")]
        [SerializeField, Min(0f)] private float _vignetteDelay = 1f;
        [SerializeField, Min(0f)] private float _vignetteTargetScale = 10f;
        [SerializeField, Min(0f)] private float _vignetteDur = 2f;
        [SerializeField] private RSLib.Maths.Curve _vignetteCurve = RSLib.Maths.Curve.InOutSine;

        [Header("GRAYSCALE RAMP")]
        [SerializeField] private RampFadeDatas _rampDatas = null;

        [Header("TITLE")]
        [SerializeField, Min(0f)] private float _titleColorDur = 1.6f;
        [SerializeField, Min(0f)] private float _titleColorDelay = 1.1f;
        [SerializeField, Min(0f)] private float _titleAlphaDur = 1.6f;
        [SerializeField, Min(0f)] private float _titleAlphaDelay = 1.1f;
        [SerializeField] private RSLib.Maths.Curve _titleCurve = RSLib.Maths.Curve.InQuint;

        public float VignetteDelay => _vignetteDelay;

        public float VignetteTargetScale => _vignetteTargetScale;
        public float VignetteDur => _vignetteDur;
        public RSLib.Maths.Curve VignetteCurve => _vignetteCurve;

        public RampFadeDatas RampDatas => _rampDatas;

        public float TitleColorDur => _titleColorDur;
        public float TitleColorDelay => _titleColorDelay;
        public float TitleAlphaDur => _titleAlphaDur;
        public float TitleAlphaDelay => _titleAlphaDelay;
        public RSLib.Maths.Curve TitleCurve => _titleCurve;
    }
}