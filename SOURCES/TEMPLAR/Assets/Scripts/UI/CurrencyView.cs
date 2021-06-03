namespace Templar.UI
{
    using UnityEngine;

    public class CurrencyView : MonoBehaviour
    {
        [SerializeField] private Canvas _currencyCanvas = null;
        [SerializeField] private TMPro.TextMeshProUGUI _currencyText = null;
        [SerializeField] private TMPro.TextMeshProUGUI _diffText = null;

        [Header("DIFF UPDATE")]
        [SerializeField] private float _startUpdateDiffDelay = 1.5f;
        [SerializeField] private long _stepMax = 3;
        [SerializeField] private float _stepInterval = 0.05f;

        private System.Collections.IEnumerator _diffUpdateCoroutine;

        private long _displayedCurrency = 0;
        private long _diff = 0;

        private void OnCurrencyChanged(long previous, long current)
        {
            _diff += current - previous;

            KillUpdateDifferenceCoroutine();
            StartCoroutine(_diffUpdateCoroutine = UpdateDifferenceCoroutine());
        }

        private void OnFadeBegan(bool fadeIn)
        {
            Display(false);
        }

        private void OnFadeOver(bool fadeIn)
        {
            if (fadeIn)
                return;

            Display(true);
        }

        private void UpdateTexts()
        {
            _currencyText.text = _displayedCurrency.ToString();

            _diffText.enabled = _diff != 0;
            if (_diffText.enabled)
                _diffText.text = $"{(System.Math.Sign(_diff) > 0 ? "+" : "-")}{System.Math.Abs(_diff).ToString()}";
        }

        private void Display(bool state)
        {
            _currencyCanvas.enabled = state;
            _displayedCurrency = Manager.CurrencyManager.Currency;

            UpdateTexts();
        }

        private void KillUpdateDifferenceCoroutine()
        {
            if (_diffUpdateCoroutine != null)
                StopCoroutine(_diffUpdateCoroutine);
        }

        private System.Collections.IEnumerator UpdateDifferenceCoroutine()
        {
            UpdateTexts();
            yield return RSLib.Yield.SharedYields.WaitForSeconds(_startUpdateDiffDelay);

            long step = 0;

            while (_diff != 0)
            {
                step = _diff > 0 ? System.Math.Min(_stepMax, _diff) : System.Math.Max(-_stepMax, _diff);
                _diff -= step;
                _displayedCurrency += step;

                UpdateTexts();

                // We may want this to accelerate over time in case there's a huge difference to interpolate.
                yield return RSLib.Yield.SharedYields.WaitForSeconds(_stepInterval);
            }
        }

        private void Awake()
        {
            Manager.CurrencyManager.Instance.CurrencyChanged += OnCurrencyChanged;

            Manager.RampFadeManager.Instance.FadeBegan += OnFadeBegan;
            Manager.RampFadeManager.Instance.FadeOver += OnFadeOver;

            Manager.OptionsManager.Instance.OptionsOpened += () => Display(false);
            Manager.OptionsManager.Instance.OptionsClosed += () => Display(true);

            UI.Dialogue.DialogueManager.Instance.DialogueStarted += (dialogueId) => Display(false);
            UI.Dialogue.DialogueManager.Instance.DialogueOver += (dialogueId) => Display(true);
        }

        private void OnDestroy()
        {
            if (Manager.CurrencyManager.Exists())
                Manager.CurrencyManager.Instance.CurrencyChanged -= OnCurrencyChanged;

            if (Manager.RampFadeManager.Exists())
            {
                Manager.RampFadeManager.Instance.FadeBegan -= OnFadeBegan;
                Manager.RampFadeManager.Instance.FadeOver -= OnFadeOver;
            }

            if (Manager.OptionsManager.Exists())
            {
                Manager.OptionsManager.Instance.OptionsOpened -= () => Display(false);
                Manager.OptionsManager.Instance.OptionsClosed -= () => Display(true);
            }

            if (UI.Dialogue.DialogueManager.Exists())
            {
                UI.Dialogue.DialogueManager.Instance.DialogueStarted -= (dialogueId) => Display(false);
                UI.Dialogue.DialogueManager.Instance.DialogueOver -= (dialogueId) => Display(true);
            }
        }
    }
}