namespace Templar
{
    using RSLib.Extensions;
    using UnityEngine;

    public class WorldSpaceHealthBar : MonoBehaviour
    {
        [SerializeField] private GameObject _mainContainer = null;
        [SerializeField] private Transform _fillScaler = null;
        [SerializeField] private bool _onlyShowOnHealthMissing = true;

        public Unit.UnitHealthController HealthCtrl { get; set; }

        private bool _disabled;
        public bool Disabled
        {
            get => _disabled;
            set
            {
                _disabled = value;
                if (_disabled)
                    Display(false);
            }
        }

        public void Display(bool show)
        {
            _mainContainer.SetActive(show);
        }

        public void UpdateHealth()
        {
            if (Disabled)
                return;

            _fillScaler.SetScaleX(HealthCtrl.HealthSystem.HealthPercentage);

            if (HealthCtrl.HealthSystem.HealthPercentage < 1f)
                Display(true);
            else if (_onlyShowOnHealthMissing)
                Display(false);
        }

        private void Awake()
        {
            Display(!_onlyShowOnHealthMissing);
        }
    }
}