namespace Templar.UI
{
    using System.Linq;
    using UnityEngine;

    /// <summary>
    /// This class only modifies the material of given graphics, making the change for all graphics using the same materials as the template ones.
    /// There should be only one instance of this in the scene but the singleton pattern here makes the instance private since we have no need to access it.
    /// </summary>
    [DisallowMultipleComponent]
    public class UIPaletteHandler : MonoBehaviour
    {
        private const string RAMP_TEX_SHADER_PARAM = "_RampTex";

        [SerializeField] private UnityEngine.UI.Graphic[] _templateGraphics = null;

        private static UIPaletteHandler s_instance;

        private Texture2D _initRampTex;
        private Material[] _mats;

        private void ChangePalette(Texture2D rampTex)
        {
            for (int i = _mats.Length - 1; i >= 0; --i)
                if (_mats[i] != null)
                    _mats[i].SetTexture(RAMP_TEX_SHADER_PARAM, rampTex);
        }

        private void Awake()
        {
            if (s_instance != null)
            {
                DestroyImmediate(gameObject);
                return;
            }

            s_instance = this;

            PaletteManager.Instance.PaletteChanged += ChangePalette;
            _mats = _templateGraphics.Select(o => o.materialForRendering).ToArray();
            _initRampTex = _mats[0].GetTexture(RAMP_TEX_SHADER_PARAM) as Texture2D;
        }

        private void OnDestroy()
        {
            ChangePalette(_initRampTex); // Reset material for editor purpose. [TODO] Check out how it's going with scene loading.
            s_instance = null;
        }
    }
}