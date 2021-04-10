namespace Templar
{
    using UnityEngine;

    public class PaletteSelector : MonoBehaviour
    {
        [SerializeField] private Texture2D[] _ramps = null;

        private int _currRampIndex = -1;

        public int CurrRampIndex
        {
            get => _currRampIndex;
            private set
            {
                _currRampIndex = RSLib.Helpers.Mod(value, _ramps.Length);
                Manager.GameManager.PlayerCtrl.CameraCtrl.GrayscaleRamp.OverrideRamp(_ramps[_currRampIndex]);
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

        private void Awake()
        {
            if (CurrRampIndex == -1)
                CurrRampIndex = System.Array.IndexOf(_ramps, Manager.GameManager.PlayerCtrl.CameraCtrl.GrayscaleRamp.TextureRamp);
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.F1))
                CurrRampIndex++;
        }
    }
}