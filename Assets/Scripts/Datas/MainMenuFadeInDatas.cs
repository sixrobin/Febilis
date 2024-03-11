namespace Templar.Datas.MainMenu
{
    using UnityEngine;

    [CreateAssetMenu(fileName = "New Main Menu Fade In Datas", menuName = "Datas/Main Menu Fade In")]
    public class MainMenuFadeInDatas : ScriptableObject
    {
        [Header("VIGNETTE")]
        [SerializeField, Min(0f)] private float _vignetteTargetScale = 10f;
        [SerializeField, Min(0f)] private float _vignetteDur = 2f;
        [SerializeField] private RSLib.Maths.Curve _vignetteCurve = RSLib.Maths.Curve.InOutSine;

        public float VignetteTargetScale => _vignetteTargetScale;
        public float VignetteDur => _vignetteDur;
        public RSLib.Maths.Curve VignetteCurve => _vignetteCurve;
    }
}