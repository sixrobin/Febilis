using UnityEngine;

public class UnitHealthController : MonoBehaviour, IHittable
{
    public class UnitHealthChangedEventArgs : RSLib.HealthSystem.HealthChangedEventArgs
    {
        public UnitHealthChangedEventArgs(int previous, int current, AttackDatas attackDatas, float dir)
            : base(previous, current)
        {
            AttackDatas = attackDatas;
            Dir = dir;
        }

        public UnitHealthChangedEventArgs(RSLib.HealthSystem.HealthChangedEventArgs baseArgs, AttackDatas attackDatas, float dir)
            : base(baseArgs)
        {
            AttackDatas = attackDatas;
            Dir = dir;
        }

        public AttackDatas AttackDatas { get; private set; }
        public float Dir { get; private set; }
    }

    [SerializeField] private int _baseHealth = 100;

    private bool _init;

    private AttackDatas _lastHitAttackDatas;
    private float _lastHitDir;

    public delegate void UnitHealthChangedEventHandler(UnitHealthChangedEventArgs args);
    public event UnitHealthChangedEventHandler UnitHealthChanged;

    public RSLib.HealthSystem HealthSystem { get; private set; }

    public HitLayer HitLayer => HitLayer.Player;

    public virtual void OnHit(AttackDatas attackDatas, float dir)
    {
        _lastHitAttackDatas = attackDatas;
        _lastHitDir = dir;

        HealthSystem.Damage(attackDatas.Dmg);
    }

    public void Init()
    {
        HealthSystem = new RSLib.HealthSystem(_baseHealth);
        HealthSystem.HealthChanged += OnHealthChanged;
        HealthSystem.Killed += OnKilled;

        _init = true;
    }

    protected virtual void OnHealthChanged(RSLib.HealthSystem.HealthChangedEventArgs args)
    {
        UnitHealthChanged?.Invoke(new UnitHealthChangedEventArgs(args, _lastHitAttackDatas, _lastHitDir));
    }

    protected virtual void OnKilled()
    {
    }

    protected virtual void Awake()
    {
        if (!_init)
            Init();
    }

    protected virtual void OnDestroy()
    {
        HealthSystem.HealthChanged -= OnHealthChanged;
    }
}