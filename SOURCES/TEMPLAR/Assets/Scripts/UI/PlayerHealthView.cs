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
        [SerializeField] private UnitHealthView _healthView = null;

        [Header("HEALS")]
        [SerializeField] private RectTransform _healsContainer = null;
        [SerializeField] private GameObject _healViewPrefab = null;

        private System.Collections.Generic.List<GameObject> _healViews = new System.Collections.Generic.List<GameObject>();

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

            if (args.Item.Datas.Id == InventoryController.ITEM_ID_POTION)
                UpdateHealViews();
        }

        private void OnHealthChanged(RSLib.HealthSystem.HealthChangedEventArgs args)
        {
            _healthView.OnHealthChanged(args);
        }

        private void OnKilled()
        {
            _healthView.OnKilled();
        }

        private System.Collections.IEnumerator InitHealth()
        {
            yield return new WaitUntil(() => _playerHealthCtrl.HealthSystem != null);

            _playerHealthCtrl.HealthSystem.HealthChanged += OnHealthChanged;
            _playerHealthCtrl.HealthSystem.Killed += OnKilled;

            // Instantly refresh the health, in case the players comes from another scene with already missing health.
            _healthView.SetHealthInstantly(_playerHealthCtrl.HealthSystem.CurrentHealth, _playerHealthCtrl.HealthSystem.MaxHealth);

            InitHealViews();
        }

        protected override void Awake()
        {
            base.Awake();

            if (_playerHealthCtrl == null)
            {
                CProLogger.LogError(this, $"{typeof(Unit.Player.PlayerHealthController).Name} reference is missing on {GetType().Name}, getting it through FindObjectOfType!");
                _playerHealthCtrl = FindObjectOfType<Unit.Player.PlayerHealthController>();
            }

            Display(false);
            StartCoroutine(InitHealth());
        }
    }
}