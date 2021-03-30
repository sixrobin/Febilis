namespace Templar.Unit
{
    using UnityEngine;

    public abstract class UnitHealthController : MonoBehaviour, Attack.IHittable
    {
        public class UnitHealthChangedEventArgs : RSLib.HealthSystem.HealthChangedEventArgs
        {
            public UnitHealthChangedEventArgs(int previous, int current, Attack.HitInfos hitDatas)
                : base(previous, current)
            {
                HitDatas = hitDatas;
            }

            public UnitHealthChangedEventArgs(RSLib.HealthSystem.HealthChangedEventArgs baseArgs, Attack.HitInfos hitDatas)
                : base(baseArgs)
            {
                HitDatas = hitDatas;
            }

            public Attack.HitInfos HitDatas { get; private set; }
        }

        public class UnitKilledEventArgs : System.EventArgs
        {
            public UnitKilledEventArgs(Attack.HitInfos hitDatas)
            {
                HitDatas = hitDatas;
            }

            // If this is null, death comes from another source than an attack.
            public Attack.HitInfos HitDatas { get; private set; }
        }

        [SerializeField] private Collider2D _collider = null;
        [SerializeField] private int _baseHealth = 100;

        private bool _init;

        // [TMP] This is probably not ideal : in case we receive damage not coming with AttackDatas,
        // this variable will still be used. It is reset to fix it, but it's more a workaround than a good solution.
        private Attack.HitInfos _lastHitDatas;

        public delegate void UnitHealthChangedEventHandler(UnitHealthChangedEventArgs args);
        public delegate void UnitKilledEventHandler(UnitKilledEventArgs args);

        public event UnitHealthChangedEventHandler UnitHealthChanged;
        public event UnitKilledEventHandler UnitKilled;

        public RSLib.HealthSystem HealthSystem { get; private set; }

        public bool CanBeHit => !HealthSystem.IsDead;

        public abstract Attack.HitLayer HitLayer { get; }

        public virtual void OnHit(Attack.HitInfos hitDatas)
        {
            if (HealthSystem.IsDead)
                return;

            _lastHitDatas = hitDatas;
            HealthSystem.Damage(hitDatas.AttackDatas.Dmg);
        }

        public virtual void Init()
        {
            HealthSystem = new RSLib.HealthSystem(_baseHealth);
            HealthSystem.HealthChanged += OnHealthChanged;
            HealthSystem.Killed += OnKilled;

            _init = true;
        }

        protected virtual void OnHealthChanged(RSLib.HealthSystem.HealthChangedEventArgs args)
        {
            UnitHealthChanged?.Invoke(new UnitHealthChangedEventArgs(args, _lastHitDatas));
            _lastHitDatas = null;
        }

        protected virtual void OnKilled()
        {
            UnitKilled?.Invoke(new UnitKilledEventArgs(_lastHitDatas));
            _lastHitDatas = null;
            _collider.enabled = false;
        }

        protected virtual void Awake()
        {
            if (!_init)
                Init();
        }

        protected virtual void OnDestroy()
        {
            HealthSystem.HealthChanged -= OnHealthChanged;
            HealthSystem.Killed -= OnKilled;
        }
    }
}