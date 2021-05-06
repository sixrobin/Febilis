namespace Templar.Unit.Player
{
    using UnityEngine;

    public class PlayerHealthController : UnitHealthController
    {
        [SerializeField] private int _baseHealsLeft = 2;
        [SerializeField] private int _healAmount = 50;

        [Header("DEBUG")]
        [SerializeField] private bool _debugMode = false;

        public delegate void HealsLeftChangedEventHandler(int healsLeft);
        public event HealsLeftChangedEventHandler HealsLeftChanged;

        public override bool CanBeHit => base.CanBeHit && !PlayerCtrl.RollCtrl.IsRolling;

        private int _healsLeft;
        public int HealsLeft
        {
            get => _healsLeft;
            set
            {
                if (DebugMode)
                    return;

                _healsLeft = Mathf.Max(0, value);
                HealsLeftChanged?.Invoke(_healsLeft);
            }
        }

        public int HealAmount => _healAmount;

        public PlayerController PlayerCtrl { get; set; }

        public override Attack.HitLayer HitLayer => Attack.HitLayer.PLAYER;

        public bool DebugMode => _debugMode;

        public bool CanHeal()
        {
            return !PlayerCtrl.RollCtrl.IsRolling
                && !PlayerCtrl.AttackCtrl.IsAttacking
                && !PlayerCtrl.JumpCtrl.IsInLandImpact
                && !PlayerCtrl.IsBeingHurt
                && !PlayerCtrl.IsHealing
                && (!PlayerCtrl.HealthCtrl.HealthSystem.IsFull && HealsLeft > 0 || DebugMode)
                && PlayerCtrl.InputCtrl.CheckInput(PlayerInputController.ButtonCategory.HEAL);
        }

        public override void Init(int health)
        {
            base.Init(health);
            RestoreCells();
        }

        public void Init(int health, UnitHealthChangedEventHandler onUnitHealthChanged, UnitKilledEventHandler onUnitKilled)
        {
            UnitHealthChanged += onUnitHealthChanged;
            UnitKilled += onUnitKilled;
            Init(health);
        }
        
        public override void OnHit(Attack.HitInfos hitDatas)
        {
            UnityEngine.Assertions.Assert.IsNotNull(PlayerCtrl, "PlayerController must be referenced to handle PlayerHealthController.");
            if (PlayerCtrl.RollCtrl.IsRolling || PlayerCtrl.IsBeingHurt)
                return;

            base.OnHit(hitDatas);
        }

        [ContextMenu("Restore cells")]
        public void RestoreCells()
        {
            // [TMP] Need to load, or get it from datas, etc.
            HealsLeft = _baseHealsLeft;
        }
    }
}