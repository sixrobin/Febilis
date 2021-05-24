namespace Templar
{
    using UnityEngine;

    public class PaletteSelector : MonoBehaviour
    {
        private const string RAMP_TEX_SHADER_PARAM = "_RampTex";

        [SerializeField] private Texture2D[] _ramps = null;

        public delegate void PaletteChangeEventHandler(Texture2D rampTex);
        public event PaletteChangeEventHandler PaletteChanged;
        
        private int _currRampIndex = -1;
        public int CurrRampIndex
        {
            get => _currRampIndex;
            private set
            {
                _currRampIndex = RSLib.Helpers.Mod(value, _ramps.Length);

                Manager.GameManager.PlayerCtrl.CameraCtrl.GrayscaleRamp.OverrideRamp(_ramps[_currRampIndex]);
                PaletteChanged?.Invoke(_ramps[_currRampIndex]);
            }
        }

        /// <summary>
        /// Used to set the current checkpoint Id from save file.
        /// Should only be called by a save manager of some sort.
        /// </summary>
        public void LoadPalette(int rampIndex)
        {
            CurrRampIndex = rampIndex;
        }

        public Texture2D GetCurrentRamp()
        {
            return _ramps[CurrRampIndex];
        }

        private void Awake()
        {
            if (CurrRampIndex == -1)
                CurrRampIndex = System.Array.IndexOf(_ramps, Manager.GameManager.PlayerCtrl.CameraCtrl.GrayscaleRamp.TextureRamp);
        }

        private void Update()
        {
            // [TOOD] Remove this and do a debug panel.
            if (Input.GetKeyDown(KeyCode.F1))
                CurrRampIndex++;
        }
    }
}