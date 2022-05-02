namespace Templar.Unit.Enemy
{
    using UnityEngine;

    public class EnemyHealthController : UnitHealthController
    {
        [SerializeField] private WorldSpaceHealthBar _healthBar = null;

        public override Attack.HitLayer HitLayer => Attack.HitLayer.ENEMY;

        public override bool SpawnVFXOnHit
        {
            get
            {
                return true;
                // Avoid spawning VFX when killing a boss unit.
                return (Unit as EnemyController)?.IsBossUnit == false || !Unit.IsDead;
            }
        }

        public WorldSpaceHealthBar WorldSpaceHealthBar => _healthBar;

        public override void Init(UnitController unit, int maxHealth, int initHealth)
        {
            base.Init(unit, maxHealth, initHealth);
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