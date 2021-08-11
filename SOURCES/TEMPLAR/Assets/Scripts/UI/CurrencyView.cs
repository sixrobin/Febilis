namespace Templar.UI
{
    using Templar.Item;
    using UnityEngine;

    public class CurrencyView : MonoBehaviour
    {
        [SerializeField] private Canvas _currencyCanvas = null;
        [SerializeField] private TMPro.TextMeshProUGUI _currencyText = null;
        [SerializeField] private TMPro.TextMeshProUGUI _diffText = null;

        [Header("DIFF UPDATE")]
        [SerializeField] private float _startUpdateDiffDelay = 1.5f;
        [SerializeField] private int _stepMax = 3;
        [SerializeField] private float _stepInterval = 0.05f;

        private System.Collections.IEnumerator _diffUpdateCoroutine;

        private int _displayedCurrency = 0;
        private int _diff = 0;

        private void OnInventoryContentChanged(InventoryController.InventoryContentChangedEventArgs args)
        {
            if (args.Item.Id != InventoryController.ITEM_ID_COIN)
                return;

            _diff += args.NewQuantity - args.PrevQuantity;

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

        private void OnSleepAnimationBegan()
        {
            Display(false);
        }

        private void OnSleepAnimationOver()
        {
            Display(true);
        }

        private void OnInventoryDisplayChanged(bool displayed)
        {
            Display(!displayed);
        }

        private void UpdateTexts()
        {
            _currencyText.text = _displayedCurrency.ToString();

            _diffText.enabled = _diff != 0;
            if (_diffText.enabled)
                _diffText.text = $"{(System.Math.Sign(_diff) > 0 ? "+" : "-")}{System.Math.Abs(_diff)}";
        }

        private void Display(bool state)
        {
            _currencyCanvas.enabled = state;
            _displayedCurrency = Manager.GameManager.InventoryCtrl?.GetItemQuantity(InventoryController.ITEM_ID_COIN) ?? 999;
            _diff = 0;

            KillUpdateDifferenceCoroutine();

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

            int step = 0;

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
            Manager.RampFadeManager.Instance.FadeBegan += OnFadeBegan;
            Manager.RampFadeManager.Instance.FadeOver += OnFadeOver;

            Manager.OptionsManager.Instance.OptionsOpened += () => Display(false);
            Manager.OptionsManager.Instance.OptionsClosed += () => Display(true);

            Manager.GameManager.PlayerCtrl.PlayerView.SleepAnimationBegan += OnSleepAnimationBegan;
            Manager.GameManager.PlayerCtrl.PlayerView.SleepAnimationOver += OnSleepAnimationOver;

            if (Manager.GameManager.InventoryCtrl != null)
                Manager.GameManager.InventoryCtrl.InventoryContentChanged += OnInventoryContentChanged;
            
            if (Manager.GameManager.InventoryView != null)
                Manager.GameManager.InventoryView.DisplayChanged += OnInventoryDisplayChanged;

            UI.Dialogue.DialogueManager.Instance.DialogueStarted += (dialogueDatas) => Display(false);
            UI.Dialogue.DialogueManager.Instance.DialogueOver += (dialogueDatas) => Display(true);
        }

        private void OnDestroy()
        {
            if (Manager.GameManager.Exists())
                Manager.GameManager.InventoryCtrl.InventoryContentChanged -= OnInventoryContentChanged;

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

            if (Manager.GameManager.Exists())
            {
            if (Manager.GameManager.InventoryCtrl != null)
                Manager.GameManager.InventoryCtrl.InventoryContentChanged -= OnInventoryContentChanged;
            
                if (Manager.GameManager.InventoryView != null)
                Manager.GameManager.InventoryView.DisplayChanged -= OnInventoryDisplayChanged;

                if (Manager.GameManager.PlayerCtrl != null)
                {
                    Manager.GameManager.PlayerCtrl.PlayerView.SleepAnimationBegan -= OnSleepAnimationBegan;
                    Manager.GameManager.PlayerCtrl.PlayerView.SleepAnimationOver -= OnSleepAnimationOver;
                }
            }

            if (UI.Dialogue.DialogueManager.Exists())
            {
                UI.Dialogue.DialogueManager.Instance.DialogueStarted -= (dialogueId) => Display(false);
                UI.Dialogue.DialogueManager.Instance.DialogueOver -= (dialogueId) => Display(true);
            }
        }
    }
}