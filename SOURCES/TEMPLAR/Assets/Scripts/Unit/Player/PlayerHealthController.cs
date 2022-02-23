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

        public PlayerController PlayerCtrl { get; private set; }

        public override Attack.HitLayer HitLayer => Attack.HitLayer.PLAYER;

        public bool DebugMode => _debugMode;
        public bool GodMode { get; private set; }

        public override bool CanBeHit(Attack.HitInfos hitInfos)
        {
            if (GodMode || Manager.BoardsTransitionManager.IsInBoardTransition)
                return false;

            return base.CanBeHit(hitInfos)
                && !PlayerCtrl.IsBeingHurt
                && (!PlayerCtrl.RollCtrl.IsRolling || hitInfos.AttackDatas.ForceHurt);
        }

        public override void OnHit(Attack.HitInfos hitInfos)
        {
            UnityEngine.Assertions.Assert.IsNotNull(PlayerCtrl, "PlayerController must be referenced to handle PlayerHealthController.");
            if (GodMode || (PlayerCtrl.RollCtrl.IsRolling && !hitInfos.AttackDatas.ForceHurt) || PlayerCtrl.IsBeingHurt)
                return;

            base.OnHit(hitInfos);
        }

        public bool CanHeal()
        {
            return !PlayerCtrl.RollCtrl.IsRolling
                && !PlayerCtrl.AttackCtrl.IsAttacking
                && !PlayerCtrl.JumpCtrl.IsInLandImpact
                && !PlayerCtrl.IsBeingHurt
                && !PlayerCtrl.IsHealing
                && (!PlayerCtrl.HealthCtrl.HealthSystem.IsFull && HealsLeft > 0 || DebugMode)
                && PlayerCtrl.InputCtrl.CheckInput(PlayerInputController.ButtonCategory.HEAL)
                && PlayerCtrl.CollisionsCtrl.Below
                && !Manager.BoardsTransitionManager.IsInBoardTransition;
        }

        public override void Kill()
        {
            if (GodMode)
                return;

            base.Kill();
        }

        public void Init(PlayerController playerCtrl, int maxHealth, UnitHealthChangedEventHandler onUnitHealthChanged, UnitKilledEventHandler onUnitKilled, int initHealth = -1)
        {
            if (_init)
                return;

            UnitHealthChanged += onUnitHealthChanged;
            UnitKilled += onUnitKilled;

            PlayerCtrl = playerCtrl;
            Init(PlayerCtrl, maxHealth, initHealth);

            RSLib.Debug.Console.DebugConsole.OverrideCommand(new RSLib.Debug.Console.Command("tgm", "Toggles god mode.", () => GodMode = !GodMode));
            RSLib.Debug.Console.DebugConsole.OverrideCommand(new RSLib.Debug.Console.Command<int>("heal", "Heals of a given amount.", (amount) => HealthSystem.Heal(Mathf.Max(0, amount))));
            RSLib.Debug.Console.DebugConsole.OverrideCommand(new RSLib.Debug.Console.Command<int>("health", "Sets health.", (value) => HealthSystem.CurrentHealth = value));
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