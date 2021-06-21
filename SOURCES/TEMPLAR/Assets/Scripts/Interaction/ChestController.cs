namespace Templar.Interaction.Checkpoint
{
    using UnityEngine;

    public class ChestController : Interactable
    {
        [SerializeField] private GameObject _highlight = null;
        [SerializeField] private Templar.Physics.Triggerables.TriggerableObject _triggerableChest = null;

        private void OnChestTriggered(Templar.Physics.Triggerables.TriggerableObject.TriggerEventArgs args)
        {
            InteractionDisabled = true;
            _highlight.SetActive(false);
        }

        private void OnChestReset(Templar.Physics.Triggerables.TriggerableObject.TriggerEventArgs args)
        {
            InteractionDisabled = false;
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
            InteractionDisabled = true;

            _highlight.SetActive(false);
        }

        private void Awake()
        {
            _triggerableChest.Triggered += OnChestTriggered;
            _triggerableChest.Reset += OnChestReset;
        }

        private void OnDestroy()
        {
            _triggerableChest.Triggered -= OnChestTriggered;
            _triggerableChest.Reset -= OnChestReset;
        }
    }
}