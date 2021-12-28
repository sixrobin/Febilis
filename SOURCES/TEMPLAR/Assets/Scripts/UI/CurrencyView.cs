namespace Templar.UI
{
    using Templar.Item;
    using UnityEngine;

    public class CurrencyView : HUDElement
    {
        [Header("REFS")]
        [SerializeField] private TMPro.TextMeshProUGUI _currencyText = null;
        [SerializeField] private TMPro.TextMeshProUGUI _diffText = null;

        [Header("DIFF UPDATE")]
        [SerializeField, Min(0f)] private float _diffUpdateStartDelay = 1.5f;
        [SerializeField, Min(1)] private int _diffUpdateStepInit = 4;
        [SerializeField, Min(0)] private int _diffUpdateStepIncrement = 1;
        [SerializeField, Min(0f)] private float _stepInterval = 0.05f;

        private System.Collections.IEnumerator _diffUpdateCoroutine;

        private int _displayedCurrency = 0;
        private int _diff = 0;

        protected override void OnInventoryContentChanged(InventoryController.InventoryContentChangedEventArgs args)
        {
            base.OnInventoryContentChanged(args);

            if (args.Item.Datas.Id != InventoryController.ITEM_ID_COIN)
                return;

            _diff += args.NewQuantity - args.PrevQuantity;

            KillUpdateDifferenceCoroutine();
            StartCoroutine(_diffUpdateCoroutine = UpdateDifferenceCoroutine());
        }

        protected override void Display(bool state)
        {
            base.Display(state);

            if (state && !CanBeDisplayed())
                return;

            _displayedCurrency = Manager.GameManager.InventoryCtrl?.GetItemQuantity(InventoryController.ITEM_ID_COIN) ?? 999;
            _diff = 0;

            KillUpdateDifferenceCoroutine();
            UpdateTexts();
        }

        private void UpdateTexts()
        {
            _currencyText.text = _displayedCurrency.ToString();

            _diffText.enabled = _diff != 0;
            if (_diffText.enabled)
                _diffText.text = $"{(System.Math.Sign(_diff) > 0 ? "+" : "-")}{System.Math.Abs(_diff)}";
        }

        private void KillUpdateDifferenceCoroutine()
        {
            if (_diffUpdateCoroutine != null)
                StopCoroutine(_diffUpdateCoroutine);
        }

        private System.Collections.IEnumerator UpdateDifferenceCoroutine()
        {
            UpdateTexts();

            yield return RSLib.Yield.SharedYields.WaitForSeconds(_diffUpdateStartDelay);

            int stepIncr = 0;

            while (_diff != 0)
            {
                int step = _diff > 0
                    ? System.Math.Min(_diffUpdateStepInit + stepIncr, _diff)
                    : System.Math.Max(-_diffUpdateStepInit - stepIncr, _diff);

                stepIncr += _diffUpdateStepIncrement;

                _diff -= step;
                _displayedCurrency += step;

                UpdateTexts();

                yield return RSLib.Yield.SharedYields.WaitForSeconds(_stepInterval);
            }
        }

        protected override void Awake()
        {
            base.Awake();
            Display(false);
        }
    }
}