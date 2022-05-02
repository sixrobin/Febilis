namespace Templar.Manager
{
    using System.Linq;
    using UnityEngine;

    public class PaletteManager : RSLib.Framework.ConsoleProSingleton<PaletteManager>
    {
        [System.Serializable]
        private class RampByZone
        {
            [SerializeField] private Flags.ZoneIdentifier _zoneIdentifier = null;
            [SerializeField] private Texture2D _ramp = null;

            public Flags.ZoneIdentifier ZoneIdentifier => _zoneIdentifier;
            public Texture2D Ramp => _ramp;
        }

        [System.Serializable]
        public struct RampsGroup
        {
            public UnityEngine.Texture2D Base;
            public UnityEngine.Texture2D NoWhite;

            public bool Match(UnityEngine.Texture2D ramp)
            {
                return Base == ramp || NoWhite == ramp;
            }
        }
        
        [Header("ZONES PALETTES")]
        [SerializeField] private RampByZone[] _rampByZones = null;

        [Header("ALL RAMPS")]
        [SerializeField] private Texture2D[] _ramps = null;

        [Header("RAMPS GROUPS")]
        [UnityEngine.SerializeField] private RampsGroup[] _rampsGroups = null;
        
        public delegate void PaletteChangeEventHandler(Texture2D rampTex);
        public event PaletteChangeEventHandler PaletteChanged;

        private static Texture2D s_currentRamp = null;
        public static Texture2D CurrentRamp
        {
            get => s_currentRamp;
            private set
            {
                s_currentRamp = value;
                GameManager.CameraCtrl.GrayscaleRamp.OverrideRamp(s_currentRamp);
                
                Instance.PaletteChanged?.Invoke(s_currentRamp);
            }
        }

        public static void UpdatePaletteForCurrentZone()
        {
            Texture2D ramp = Instance._rampByZones
                            .FirstOrDefault(o => o.ZoneIdentifier == (BoardsManager.CurrentBoard.Identifier as Flags.BoardIdentifier).ContainingZoneIdentifier)
                            .Ramp;

            SetPalette(ramp);
        }

        public static void SetPalette(Texture2D ramp)
        {
            CurrentRamp = ramp;
        }

        public static void SetPalette(int rampIndex)
        {
            CurrentRamp = Instance._ramps[rampIndex];
        }

        public static RampsGroup GetGroupFromRamp(UnityEngine.Texture2D ramp)
        {
            return Instance._rampsGroups.FirstOrDefault(o => o.Match(ramp));
        }
        
        protected override void Awake()
        {
            base.Awake();
            if (!IsValid)
                return;

            RSLib.Debug.Console.DebugConsole.OverrideCommand(new RSLib.Debug.Console.Command<int>("RampIndex", "Sets the color ramp index.", (index) => SetPalette(index)));
        }
    }
}