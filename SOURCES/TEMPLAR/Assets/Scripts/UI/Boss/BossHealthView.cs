namespace Templar.UI.Boss
{
    using UnityEngine;

    public class BossHealthView : MonoBehaviour
    {
        [Header("REFS")]
        [SerializeField] private UnitHealthView _healthView = null;
        [SerializeField] private UnityEngine.UI.Image _healthPanel = null;
        [SerializeField] private Sprite _panelKilledSprite = null;

        private Sprite _panelBaseSprite;
        private Unit.Enemy.EnemyController _bossEnemyCtrl;

        private void OnBossEnemyKilled(Unit.UnitHealthController.UnitKilledEventArgs args)
        {
            _bossEnemyCtrl.HealthCtrl.UnitHealthChanged -= OnBossEnemyHealthChanged;
            _bossEnemyCtrl.HealthCtrl.UnitKilled -= OnBossEnemyKilled;

            _healthPanel.sprite = _panelKilledSprite;

            _healthView.SetHealthInstantly(0, _bossEnemyCtrl.HealthCtrl.HealthSystem.MaxHealth);
            _healthView.DisplayBackground(false);
        }

        private void OnBossEnemyHealthChanged(Unit.UnitHealthController.UnitHealthChangedEventArgs args)
        {
            _healthView.OnHealthChanged(args);
        }

        public void Display(bool show)
        {
            gameObject.SetActive(show);
        }

        public void Init(Unit.Enemy.EnemyController bossEnemyCtrl)
        {
            _bossEnemyCtrl = bossEnemyCtrl;
            _bossEnemyCtrl.HealthCtrl.UnitHealthChanged += OnBossEnemyHealthChanged;
            _bossEnemyCtrl.HealthCtrl.UnitKilled += OnBossEnemyKilled;

            _healthPanel.sprite = _panelBaseSprite;
            _healthView.DisplayBackground(true);
        }

        private void Awake()
        {
            _panelBaseSprite = _healthPanel.sprite;
        }
    }
}