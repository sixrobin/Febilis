namespace Templar.UI
{
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
        [SerializeField] private HealView _healViewPrefab = null;

        private System.Collections.Generic.List<HealView> _healViews = new System.Collections.Generic.List<HealView>();

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

                _healViews[i].Display(i < _playerHealthCtrl.MaxPotionsCount);
                _healViews[i].MarkAsFilled(i < _playerHealthCtrl.AvailablePotionsCount);
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
            
            // Health change may come from a potion healing -> refresh potions view.
            if (!args.IsLoss)
                UpdateHealViews();
        }

        private void OnKilled()
        {
            _healthView.OnKilled();
        }

        private void OnPotionsCountChanged()
        {
            UpdateHealViews();
        }
        
        private System.Collections.IEnumerator InitHealth()
        {
            yield return new WaitUntil(() => _playerHealthCtrl.HealthSystem != null);

            _playerHealthCtrl.HealthSystem.HealthChanged += OnHealthChanged;
            _playerHealthCtrl.HealthSystem.Killed += OnKilled;
            _playerHealthCtrl.PotionsCountChanged += OnPotionsCountChanged;
            
            // Instantly refresh the health, in case the players comes from another scene with already missing health.
            _healthView.SetHealthInstantly(_playerHealthCtrl.HealthSystem.CurrentHealth, _playerHealthCtrl.HealthSystem.MaxHealth);

            InitHealViews();
        }

        protected override void Awake()
        {
            base.Awake();

            if (_playerHealthCtrl == null)
            {
                CProLogger.LogError(this, $"{nameof(Unit.Player.PlayerHealthController)} reference is missing on {GetType().Name}, getting it through FindObjectOfType!");
                _playerHealthCtrl = FindObjectOfType<Unit.Player.PlayerHealthController>();
            }

            Display(false);
            StartCoroutine(InitHealth());
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            
            _playerHealthCtrl.HealthSystem.HealthChanged -= OnHealthChanged;
            _playerHealthCtrl.HealthSystem.Killed -= OnKilled;
            _playerHealthCtrl.PotionsCountChanged -= OnPotionsCountChanged;
        }
    }
}