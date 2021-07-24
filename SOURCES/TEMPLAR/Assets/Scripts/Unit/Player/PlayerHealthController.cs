namespace Templar.Unit.Player
{
    using UnityEngine;
#if UNITY_EDITOR
    using UnityEditor;
#endif

    public class PlayerHealthController : UnitHealthController
    {
        //[SerializeField] private int _baseHealsLeft = 2;
        [SerializeField] private int _healAmount = 50;

        [Header("DEBUG")]
        [SerializeField] private bool _debugMode = false;

        public override bool CanBeHit => base.CanBeHit && !PlayerCtrl.RollCtrl.IsRolling && !GodMode;

        public int HealsLeft
        {
            get => Manager.GameManager.InventoryCtrl?.GetItemQuantity(Item.InventoryController.ITEM_ID_POTION) ?? 10;
            set
            {
                if (!DebugMode)
                    Manager.GameManager.InventoryCtrl.RemoveItem(Item.InventoryController.ITEM_ID_POTION);
            }
        }

        public int HealAmount => _healAmount;

        public PlayerController PlayerCtrl { get; set; }

        public override Attack.HitLayer HitLayer => Attack.HitLayer.PLAYER;

        public bool DebugMode => _debugMode;
        public bool GodMode { get; private set; }
        
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

        public override void Kill()
        {
            if (GodMode)
                return;

            base.Kill();
        }

        public void Init(int health, UnitHealthChangedEventHandler onUnitHealthChanged, UnitKilledEventHandler onUnitKilled)
        {
            UnitHealthChanged += onUnitHealthChanged;
            UnitKilled += onUnitKilled;
            Init(health);

            RSLib.Debug.Console.DebugConsole.OverrideCommand(new RSLib.Debug.Console.Command("tgm", "Toggles god mode.", () => GodMode = !GodMode));
            RSLib.Debug.Console.DebugConsole.OverrideCommand(new RSLib.Debug.Console.Command<int>("heal", "Heals of a given amount.", (amount) => HealthSystem.Heal(Mathf.Max(0, amount))));
            RSLib.Debug.Console.DebugConsole.OverrideCommand(new RSLib.Debug.Console.Command<int>("health", "Sets health.", (value) => HealthSystem.CurrentHealth = value));
        }

        public override void OnHit(Attack.HitInfos hitDatas)
        {
            UnityEngine.Assertions.Assert.IsNotNull(PlayerCtrl, "PlayerController must be referenced to handle PlayerHealthController.");
            if (GodMode || PlayerCtrl.RollCtrl.IsRolling || PlayerCtrl.IsBeingHurt)
                return;

            base.OnHit(hitDatas);
        }
    }

#if UNITY_EDITOR
    [CustomEditor(typeof(PlayerHealthController))]
    public class PlayerHealthControllerEditor : UnitHealthControllerEditor<PlayerHealthController>
    {
        protected override void DrawButtons()
        {
            base.DrawButtons();
            DrawButton("Add Potion", () => Manager.GameManager.InventoryCtrl.AddItem(Item.InventoryController.ITEM_ID_POTION));
        }
    }
#endif
}