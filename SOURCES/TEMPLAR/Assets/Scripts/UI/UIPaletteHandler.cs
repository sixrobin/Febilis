namespace Templar.UI
{
    using UnityEngine;

    /// <summary>
    /// This class only modifies the material of a given graphic, making the change for all graphics using this material.
    /// There should be only one instance of this in the scene but the singleton pattern here makes the instance private since we have no need to access it.
    /// </summary>
    [DisallowMultipleComponent]
    public class UIPaletteHandler : MonoBehaviour
    {
        private const string RAMP_TEX_SHADER_PARAM = "_RampTex";

        [SerializeField] private UnityEngine.UI.Graphic _matUser = null;

        private UIPaletteHandler _instance;

        private Texture2D _initRampTex;
        private Material _mat;

        private void ChangePalette(Texture2D rampTex)
        {
            _mat.SetTexture(RAMP_TEX_SHADER_PARAM, rampTex);
        }

        private void Awake()
        {
            if (_instance != null)
            {
                DestroyImmediate(gameObject);
                return;
            }

            _instance = this;

            FindObjectOfType<Templar.PaletteSelector>().PaletteChanged += ChangePalette; // [TMP] Find.
            _mat = _matUser.materialForRendering;
            _initRampTex = _mat.GetTexture(RAMP_TEX_SHADER_PARAM) as Texture2D;
        }

        private void OnDestroy()
        {
            // Reset material when leaving play mode.
            // Still need to check out how it's behaving with scenes loading.
            if (_mat != null)
                ChangePalette(_initRampTex);

            _instance = null;
        }
    }
}