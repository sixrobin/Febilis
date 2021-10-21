namespace Templar.Interaction.Checkpoint
{
    using UnityEngine;

    public class ChestController : Interactable, Flags.IIdentifiable
    {
        [Header("IDENTIFIER")]
        [SerializeField] private Flags.ChestIdentifier _chestIdentifier = null;

        [Header("REFS")]
        [SerializeField] private Collider2D _interactionCollider = null;
        [SerializeField] private GameObject _highlight = null;
        [SerializeField] private Templar.Physics.Triggerables.TriggerableObject _triggerableChest = null;

        public Flags.IIdentifier Identifier => _chestIdentifier;

        private void OnChestTriggered(Templar.Physics.Triggerables.TriggerableObject.TriggerEventArgs args)
        {
            if (Identifier != null && args.SourceType != Templar.Physics.Triggerables.TriggerableSourceType.LOAD)
            {
                Manager.FlagsManager.Register(this);
                _triggerableChest.NotResetableAnymore = true;
            }

            DisableChestInteraction();
        }

        private void DisableChestInteraction()
        {
            InteractionDisabled = true;
            _interactionCollider.enabled = false;
            _highlight.SetActive(false);
        }

        private void OnChestReset(Templar.Physics.Triggerables.TriggerableObject.TriggerEventArgs args)
        {
            InteractionDisabled = false;
            _interactionCollider.enabled = true;
        }

        public override void Focus()
        {
            if (InteractionDisabled)
                return;

            base.Focus();
            _highlight.SetActive(true);
        }

        public override void Unfocus()
        {
            base.Unfocus();
            _highlight.SetActive(false);
        }

        public override void Interact()
        {
            if (InteractionDisabled)
                return;

            base.Interact();
            DisableChestInteraction();
        }

        private void Start()
        {
            _triggerableChest.Triggered += OnChestTriggered;
            _triggerableChest.Reset += OnChestReset;

            if (Identifier != null && Manager.FlagsManager.Check(this))
            {
                DisableChestInteraction();
                _triggerableChest.TryTrigger(Templar.Physics.Triggerables.TriggerableSourceType.LOAD, true);
                _triggerableChest.NotResetableAnymore = true;
            }
        }

        private void OnDestroy()
        {
            _triggerableChest.Triggered -= OnChestTriggered;
            _triggerableChest.Reset -= OnChestReset;
        }
    }
}