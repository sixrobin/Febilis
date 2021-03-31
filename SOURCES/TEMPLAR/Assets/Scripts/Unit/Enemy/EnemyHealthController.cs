namespace Templar.Unit.Enemy
{
    using UnityEngine;

    public class EnemyHealthController : UnitHealthController
    {
        [SerializeField] private WorldSpaceHealthBar _healthBar = null;

        public override Attack.HitLayer HitLayer => Attack.HitLayer.ENEMY;

        public override void Init(int health)
        {
            base.Init(health);
            _healthBar.HealthCtrl = this;
        }

        protected override void OnHealthChanged(RSLib.HealthSystem.HealthChangedEventArgs args)
        {
            base.OnHealthChanged(args);
            _healthBar?.UpdateHealth();
        }

        protected override void OnKilled()
        {
            base.OnKilled();
            _healthBar?.Display(false);
        }
    }
}