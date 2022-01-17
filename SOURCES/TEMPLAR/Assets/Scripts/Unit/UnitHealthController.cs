namespace Templar.Unit
{
    using UnityEngine;
#if UNITY_EDITOR
    using UnityEditor;
#endif

    [DisallowMultipleComponent]
    public abstract class UnitHealthController : MonoBehaviour, Attack.IHittable
    {
        public class UnitHealthChangedEventArgs : RSLib.HealthSystem.HealthChangedEventArgs
        {
            public UnitHealthChangedEventArgs(int previous, int current, int max, Attack.HitInfos hitDatas)
                : base(previous, current, max)
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
            public UnitKilledEventArgs(UnitController sourceUnit, Attack.HitInfos hitDatas)
            {
                SourceUnit = sourceUnit;
                HitDatas = hitDatas;
            }

            public UnitController SourceUnit { get; }
            public Attack.HitInfos HitDatas { get; private set; }  // If null, death comes from another source than an attack.
        }

        [SerializeField] private Collider2D _collider = null;

        protected bool _init;

        // [TMP] This is probably not ideal : in case we receive damage not coming with AttackDatas,
        // this variable will still be used. It is reset to fix it, but it's more a workaround than a good solution.
        private Attack.HitInfos _lastHitDatas;

        public delegate void UnitHealthChangedEventHandler(UnitHealthChangedEventArgs args);
        public delegate void UnitKilledEventHandler(UnitKilledEventArgs args);

        public event UnitHealthChangedEventHandler UnitHealthChanged;
        public event UnitKilledEventHandler UnitKilled;

        public UnitController Unit { get; private set; }
        public RSLib.HealthSystem HealthSystem { get; private set; }

        public abstract Attack.HitLayer HitLayer { get; }

        public virtual bool CanBeHit(Attack.HitInfos hitInfos)
        {
            return !HealthSystem.IsDead;
        }

        public virtual void OnHit(Attack.HitInfos hitInfos)
        {
            UnityEngine.Assertions.Assert.IsFalse(HealthSystem.IsDead, "Hitting an unit that is already dead.");

            _lastHitDatas = hitInfos;
            HealthSystem.Damage(hitInfos.AttackDatas.Dmg);
        }

        public virtual void Init(UnitController unit, int maxHealth, int initHealth = -1)
        {
            if (_init)
                return;

            Unit = unit;

            HealthSystem = initHealth == -1 ? new RSLib.HealthSystem(maxHealth) : new RSLib.HealthSystem(maxHealth, initHealth);

            HealthSystem.HealthChanged += OnHealthChanged;
            HealthSystem.Killed += OnKilled;

            _init = true;
        }

        public virtual void ResetController()
        {
            HealthSystem.HealFull(false);
            _collider.enabled = true;
        }

        public virtual void Kill()
        {
            HealthSystem.Kill();
        }

        public void HealFull()
        {
            HealthSystem.HealFull();
        }

        public void SetHealth(int value, bool triggerEvents = true)
        {
            HealthSystem.SetHealth(value, triggerEvents);
        }

        protected virtual void OnHealthChanged(RSLib.HealthSystem.HealthChangedEventArgs args)
        {
            UnitHealthChanged?.Invoke(new UnitHealthChangedEventArgs(args, _lastHitDatas));
            _lastHitDatas = null;
        }

        protected virtual void OnKilled()
        {
            UnitKilled?.Invoke(new UnitKilledEventArgs(Unit, _lastHitDatas));
            _lastHitDatas = null;
            _collider.enabled = false;
        }

        protected virtual void OnDestroy()
        {
            HealthSystem.HealthChanged -= OnHealthChanged;
            HealthSystem.Killed -= OnKilled;
        }

        public void DebugDamageDefault()
        {
            OnHit(new Attack.HitInfos(Datas.Attack.AttackDatas.Default, 1f, transform));
        }

        public void DebugDamageInfinite()
        {
            OnHit(new Attack.HitInfos(Datas.Attack.AttackDatas.InfiniteDamage, 1f, transform));
        }
    }

#if UNITY_EDITOR
    public abstract class UnitHealthControllerEditor<T> : RSLib.EditorUtilities.ButtonProviderEditor<T> where T : UnitHealthController
    {
        protected override void DrawButtons()
        {
            DrawButton("Damage with infinite damage", Obj.DebugDamageInfinite);
            DrawButton("Damage with default AttackDatas", Obj.DebugDamageDefault);
        }
    }
#endif
}