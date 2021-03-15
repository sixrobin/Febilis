using RSLib.Extensions;
using UnityEngine;

public class WorldSpaceHealthBar : MonoBehaviour
{
    [SerializeField] private GameObject _mainContainer = null;
    [SerializeField] private Transform _fillScaler = null;
    [SerializeField] private bool _onlyShowOnHealthMissing = true;

    public UnitHealthController HealthCtrl { get; set; }

    public void Display(bool show)
    {
        _mainContainer.SetActive(show);
    }

    public void UpdateHealth()
    {
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