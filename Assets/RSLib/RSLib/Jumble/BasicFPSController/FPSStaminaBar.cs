namespace RSLib.Jumble.FPSController
{
    using UnityEngine;
    using UnityEngine.UI;

    /// <summary>
    /// Visual handler of the FPS stamina manager.
    /// </summary>
    public class FPSStaminaBar : MonoBehaviour
    {
        [Header("REFERENCES")]
        [SerializeField] private FPSController _fpsController = null;
        [SerializeField] private GameObject _barHolder = null;
        [SerializeField] private Image _staminaFill = null;

        [Header("STATS")]
        [SerializeField] private float _hideDelay = 2f;

        private float _hideTimer;
        private bool _hidden;

        /// <summary>
        /// Hides the bar (when the stamina is full). This method is called on start.
        /// </summary>
        private void Hide()
        {
            _barHolder.SetActive(false);
            _hidden = true;
        }

        /// <summary>
        /// Shows the bar (called when the stamina is not full and the bar is hidden).
        /// </summary>
        private void Show()
        {
            _barHolder.SetActive(true);
            _hidden = false;
        }

        /// <summary>
        /// Updates the bar visual depending on the stamina manager current charge (between 0 and 1).
        /// </summary>
        private void UpdateBarVisual()
        {
            _staminaFill.fillAmount = _fpsController.StaminaManager.CurrentCharge;

            if (_staminaFill.fillAmount == 1f)
            {
                if (_hidden)
                    return;

                _hideTimer += Time.deltaTime;
                if (_hideTimer > _hideDelay)
                    Hide();
            }
            else
            {
                _hideTimer = 0;
                if (_hidden)
                    Show();
            }
        }

        private void Start()
        {
            Hide();
        }

        private void Update()
        {
            UpdateBarVisual();
        }
    }
}