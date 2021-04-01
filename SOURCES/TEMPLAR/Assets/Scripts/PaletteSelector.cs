namespace Templar
{
    using UnityEngine;

    public class PaletteSelector : MonoBehaviour
    {
        [SerializeField] private Texture2D[] _ramps = null;

        private int _currRampIndex = 0;

        private void Awake()
        {
            _currRampIndex = System.Array.IndexOf(_ramps, Manager.GameManager.PlayerCtrl.CameraCtrl.GrayscaleRamp.TextureRamp);
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.F1))
            {
                _currRampIndex = ++_currRampIndex % _ramps.Length;
                Manager.GameManager.PlayerCtrl.CameraCtrl.GrayscaleRamp.OverrideRamp(_ramps[_currRampIndex]);
            }
        }
    }
}