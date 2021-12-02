namespace Templar.UI
{
    using System;
    using RSLib.Extensions;
    using Templar.Item;
    using UnityEngine;

    public class PlayerHealthView : HUDElement
    {
        [Header("REFS")]
        [SerializeField] private Unit.Player.PlayerHealthController _playerHealthCtrl = null;

        [Header("HEALTH BAR UPDATE")]
        [SerializeField] private Canvas _healthBarCanvas = null;
        [SerializeField] private UnityEngine.UI.Image _healthFill = null;
        [SerializeField] private UnityEngine.UI.Image _healthBlink = null;
        [SerializeField] private float _healthBarBlinkPauseDur = 0.15f;
        [SerializeField] private float _healthBarBlinkUpdateSpeed = 4f;

        [Header("HEALS")]
        [SerializeField] private RectTransform _healsContainer = null;
        [SerializeField] private GameObject _healViewPrefab = null;

        private System.Collections.Generic.List<GameObject> _healViews = new System.Collections.Generic.List<GameObject>();

        private System.Collections.IEnumerator _healthBarUpdateCoroutine;

        private void InitHealViews()
        {
            _healsContainer.DestroyChildren();
            _healViews.Clear();

            for (int i = 0; i < 10; ++i)
                _healViews.Add(Instantiate(_healViewPrefab, _healsContainer));

            UpdateHealViews();

            UnityEngine.UI.LayoutRebuilder.ForceRebuildLayoutImmediate(_healsContainer);
        }

        private void UpdateHealViews()
        {
            for (int i = 0; i < _healViews.Count; ++i)
            {
                if (i == _healViews.Count)
                    AddHealViews(1);

                _healViews[i].SetActive(i < _playerHealthCtrl.HealsLeft);
            }
        }

        private void AddHealViews(int count)
        {
            for (int i = 0; i < count; ++i)
                _healViews.Add(Instantiate(_healViewPrefab, _healsContainer));

            UnityEngine.UI.LayoutRebuilder.ForceRebuildLayoutImmediate(_healsContainer);
        }

        protected override void OnInventoryContentChanged(InventoryController.InventoryContentChangedEventArgs args)
        {
            base.OnInventoryContentChanged(args);

            if (args.Item.Datas.Id != InventoryController.ITEM_ID_POTION)
                return;

            UpdateHealViews();
        }

        private void OnHealthChanged(RSLib.HealthSystem.HealthChangedEventArgs args)
        {
            if (_healthBarUpdateCoroutine != null)
                SkipHealthBarUpdateCoroutine();

            if (args.IsLoss)
                _healthFill.fillAmount = _playerHealthCtrl.HealthSystem.HealthPercentage;
            else
                _healthBlink.fillAmount = _playerHealthCtrl.HealthSystem.HealthPercentage;

            StartCoroutine(_healthBarUpdateCoroutine = BlinkHealthBarCoroutine(args.IsLoss));
        }

        private void OnKilled()
        {
            // [TMP] We may want to do something special on the HUD on death.
            _healthFill.fillAmount = 0;
            StartCoroutine(BlinkHealthBarCoroutine(true));
        }

        private void SkipHealthBarUpdateCoroutine()
        {
            UnityEngine.Assertions.Assert.IsNotNull(_healthBarUpdateCoroutine, "Trying to stop a coroutine that is not running.");
            StopCoroutine(_healthBarUpdateCoroutine);
            _healthBlink.fillAmount = _healthFill.fillAmount;
        }

        private System.Collections.IEnumerator BlinkHealthBarCoroutine(bool isLoss)
        {
            yield return RSLib.Yield.SharedYields.WaitForSeconds(_healthBarBlinkPauseDur);

            UnityEngine.UI.Image barToFill = isLoss ? _healthBlink : _healthFill;

            float targetValue = _playerHealthCtrl.HealthSystem.HealthPercentage;
            float sign = Mathf.Sign(targetValue - (isLoss ? _healthBlink.fillAmount : _healthFill.fillAmount));

            while (sign > 0f ? barToFill.fillAmount < targetValue : barToFill.fillAmount > targetValue)
            {
                barToFill.fillAmount += _healthBarBlinkUpdateSpeed * Time.deltaTime * sign;
                yield return null;
            }

            barToFill.fillAmount = targetValue;
            _healthBarUpdateCoroutine = null;
        }

        private System.Collections.IEnumerator InitHealth()
        {
            yield return new WaitUntil(() => _playerHealthCtrl.HealthSystem != null);

            _playerHealthCtrl.HealthSystem.HealthChanged += OnHealthChanged;
            _playerHealthCtrl.HealthSystem.Killed += OnKilled;

            // Instantly refresh the health, in case the players comes from another scene with already missing health.
            _healthFill.fillAmount = _playerHealthCtrl.HealthSystem.HealthPercentage;
            _healthBlink.fillAmount = _playerHealthCtrl.HealthSystem.HealthPercentage;

            InitHealViews();
        }

        protected override void Awake()
        {
            base.Awake();

            Display(false);
            StartCoroutine(InitHealth());
        }
    }
}