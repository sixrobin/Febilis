namespace Templar
{
    using UnityEngine;

    public class PaletteSelector : RSLib.Framework.ConsoleProSingleton<PaletteSelector>
    {
        private const string RAMP_TEX_SHADER_PARAM = "_RampTex";

        [SerializeField] private Texture2D[] _ramps = null;

        public delegate void PaletteChangeEventHandler(Texture2D rampTex);
        public event PaletteChangeEventHandler PaletteChanged;
        
        private static int s_currRampIndex = -1;
        public static int CurrRampIndex
        {
            get => s_currRampIndex;
            private set
            {
                s_currRampIndex = RSLib.Helpers.Mod(value, Instance._ramps.Length);

                Manager.GameManager.PlayerCtrl.CameraCtrl.GrayscaleRamp.OverrideRamp(Instance._ramps[s_currRampIndex]);
                Instance.PaletteChanged?.Invoke(Instance._ramps[s_currRampIndex]);
            }
        }

        public static void SetPalette(int rampIndex)
        {
            CurrRampIndex = rampIndex;
        }

        public static Texture2D GetCurrentRamp()
        {
            return Instance._ramps[CurrRampIndex];
        }

        protected override void Awake()
        {
            base.Awake();

            if (CurrRampIndex == -1)
                CurrRampIndex = System.Array.IndexOf(_ramps, Manager.GameManager.PlayerCtrl.CameraCtrl.GrayscaleRamp.TextureRamp);

            RSLib.Debug.Console.DebugConsole.OverrideCommand(new RSLib.Debug.Console.DebugCommand<int>("RampIndex", "Sets the color ramp index.", (index) => SetPalette(index)));
        }
    }
}