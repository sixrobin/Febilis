namespace Templar.Unit.Player
{
    using UnityEngine;
#if UNITY_EDITOR
    using UnityEditor;
#endif

    public class PlayerHealthController : UnitHealthController
    {
        [SerializeField] private int _healAmount = 50;

        [Header("DEBUG")]
        [SerializeField] private bool _debugMode = false;

        public event System.Action PotionsCountChanged;
        
        public int MaxPotionsCount
        {
            get => Manager.GameManager.InventoryCtrl != null
                   ? Manager.GameManager.InventoryCtrl.GetItemQuantity(Item.InventoryController.ITEM_ID_POTION)
                   : 2;
        }

        private int _availablePotionsCount;
        public int AvailablePotionsCount
        {
            get => _availablePotionsCount;
            set
            {
                _availablePotionsCount = Mathf.Clamp(value, 0, MaxPotionsCount);
                PotionsCountChanged?.Invoke();
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
                && (!PlayerCtrl.HealthCtrl.HealthSystem.IsFull && AvailablePotionsCount > 0 || DebugMode)
                && PlayerCtrl.CollisionsCtrl.Below
                && !Manager.BoardsTransitionManager.IsInBoardTransition;
        }

        public void RestorePotions()
        {
            AvailablePotionsCount = MaxPotionsCount;
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

            Manager.GameManager.InventoryCtrl.InventoryContentChanged += OnInventoryContentChanged;
            
            UnitHealthChanged += onUnitHealthChanged;
            UnitKilled += onUnitKilled;

            PlayerCtrl = playerCtrl;
            Init(PlayerCtrl, maxHealth, initHealth);
            
            RestorePotions();
            
            RSLib.Debug.Console.DebugConsole.OverrideCommand("tgm", "Toggles god mode.", () => GodMode = !GodMode);
            RSLib.Debug.Console.DebugConsole.OverrideCommand<int>("heal", "Heals of a given amount.", amount => HealthSystem.Heal(Mathf.Max(0, amount)));
            RSLib.Debug.Console.DebugConsole.OverrideCommand<int>("health", "Sets health.", value => HealthSystem.CurrentHealth = value);
            RSLib.Debug.Console.DebugConsole.OverrideCommand("kill", "Kills player.", () => HealthSystem.Damage(int.MaxValue));
        }

        private void OnInventoryContentChanged(Templar.Item.InventoryController.InventoryContentChangedEventArgs args)
        {
            if (args.Item.Datas.Id != Item.InventoryController.ITEM_ID_POTION)
                return;
            
            if (args.NewQuantity > args.PrevQuantity)
                AvailablePotionsCount += args.NewQuantity - args.PrevQuantity;
            else
                AvailablePotionsCount = Mathf.Min(AvailablePotionsCount, MaxPotionsCount);
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            
            if (Manager.GameManager.Exists())
                Manager.GameManager.InventoryCtrl.InventoryContentChanged -= OnInventoryContentChanged;
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