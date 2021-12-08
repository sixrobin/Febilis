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
        [SerializeField] private float _startUpdateDiffDelay = 1.5f;
        [SerializeField] private int _stepMax = 3;
        [SerializeField] private float _stepInterval = 0.05f;

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

        protected override void Awake()
        {
            base.Awake();
            Display(false);
        }
    }
}