namespace Templar.Interaction.Checkpoint
{
    using RSLib.Extensions;
    using System.Linq;
    using UnityEngine;

    public class LeverController : Interactable, Flags.IIdentifiable
    {
        [Header("IDENTIFIER")]
        [SerializeField] private Flags.LeverIdentifier _leverIdentifier = null;

        [Header("REFS")]
        [SerializeField] private Collider2D _interactionCollider = null;
        [SerializeField] private SpriteRenderer _highlight = null;
        [SerializeField] private Sprite[] _highlightSprites = null;
        [SerializeField] private Templar.Physics.Triggerables.TriggerableObject _triggerableLever = null;

        [Header("LEVER DATAS")]
        [SerializeField] private bool _disableAfterTrigger = true;

        private int _highlightSpriteIndex = -1;

        public Flags.IIdentifier Identifier => _leverIdentifier;

        private void OnLeverTriggered(Templar.Physics.Triggerables.TriggerableObject.TriggerEventArgs args)
        {
            if (Identifier != null && args.SourceType != Templar.Physics.Triggerables.TriggerableSourceType.LOAD)
            {
                Manager.FlagsManager.Register(this);

                if (_disableAfterTrigger)
                    _triggerableLever.NotResetableAnymore = true;
            }

            DisableLeverInteraction();
            FlipHighlight();
        }

        private void DisableLeverInteraction()
        {
            if (!_disableAfterTrigger)
                return;

            InteractionDisabled = true;
            _interactionCollider.enabled = false;
            _highlight.gameObject.SetActive(false);
        }

        private void OnLeverReset(Templar.Physics.Triggerables.TriggerableObject.TriggerEventArgs args)
        {
            InteractionDisabled = false;
            _interactionCollider.enabled = true;
        }

        public override void Focus()
        {
            if (InteractionDisabled)
                return;

            base.Focus();
            _highlight.gameObject.SetActive(true);
        }

        public override void Unfocus()
        {
            base.Unfocus();
            _highlight.gameObject.SetActive(false);
        }

        public override void Interact()
        {
            if (InteractionDisabled)
                return;

            base.Interact();
            DisableLeverInteraction();
        }

        public void FlipHighlight()
        {
            if (_highlightSpriteIndex == -1)
                _highlightSpriteIndex = _highlightSprites.ToList().IndexOf(_highlight.sprite);

            _highlightSpriteIndex = ++_highlightSpriteIndex % _highlightSprites.Length;
            _highlight.sprite = _highlightSprites[_highlightSpriteIndex];
        }

        private void Start()
        {
            _triggerableLever.Triggered += OnLeverTriggered;
            _triggerableLever.Reset += OnLeverReset;

            if (Identifier != null && Manager.FlagsManager.Check(this))
            {
                DisableLeverInteraction();
                _triggerableLever.TryTrigger(Templar.Physics.Triggerables.TriggerableSourceType.LOAD, true);

                if (_disableAfterTrigger)
                    _triggerableLever.NotResetableAnymore = true;
            }
        }

        private void OnDestroy()
        {
            _triggerableLever.Triggered -= OnLeverTriggered;
            _triggerableLever.Reset -= OnLeverReset;
        }
    }
}