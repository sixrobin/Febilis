namespace Templar.UI
{
    using UnityEngine;

    public class CurrencyView : MonoBehaviour
    {
        [SerializeField] private Canvas _currencyCanvas = null;
        [SerializeField] private TMPro.TextMeshProUGUI _currencyText = null;

        private void OnCurrencyChanged(ulong previous, ulong current)
        {
            UpdateText(current);
        }

        private void UpdateText(ulong currency)
        {
            _currencyText.text = currency.ToString();
        }

        private void Display(bool state)
        {
            _currencyCanvas.enabled = state;
            if (state)
                UpdateText(Manager.CurrencyManager.Currency);
        }

        private void Awake()
        {
            Manager.CurrencyManager.Instance.CurrencyChanged += OnCurrencyChanged;

            Manager.RampFadeManager.Instance.FadeBegan += () => Display(false);
            Manager.RampFadeManager.Instance.FadeOver += () => Display(true);

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
                Manager.RampFadeManager.Instance.FadeBegan -= () => Display(false);
                Manager.RampFadeManager.Instance.FadeOver -= () => Display(true);
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