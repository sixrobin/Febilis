﻿namespace Templar.Interaction
{
    using RSLib.Extensions;
    using UnityEngine;

    public class ItemCollectableController : Interactable, Flags.IIdentifiable
    {
        [Header("FOR ITEMS SET IN SCENE")]
        [SerializeField] private RSLib.Framework.OptionalString _itemId = new RSLib.Framework.OptionalString(string.Empty, false);

        [Header("IDENTIFIER")]
        [SerializeField] private Flags.ItemIdentifier _itemIdentifier = null;

        [Header("REFS")]
        [SerializeField] private GameObject _container = null;
        [SerializeField] private SpriteRenderer _itemSprite = null;
        [SerializeField] private SpriteRenderer _smoke = null;
        [SerializeField] private GameObject _highlight = null;

        [Header("ITEM ADD")]
        [SerializeField] private bool _addAfterFadeOut = false;

        [Header("SMOKE FADE OUT")]
        [SerializeField, Min(0f)] private float _fadeOutDur = 1f;
        [SerializeField, Range(0f, 1f)] private float _fadeOutPercentageStop = 0.8f;
        [SerializeField] private RSLib.Maths.Curve _fadeOutCurve = RSLib.Maths.Curve.InOutSine;
        [SerializeField] private GameObject[] _fadeOverParticles = null;

        [Header("AUDIO")]
        [SerializeField] private RSLib.Audio.ClipProvider _itemCollectedClipProvider = null;

        public string ItemId { get; private set; }
        public Flags.IIdentifier Identifier => _itemIdentifier;

        public void SetItemId(string id)
        {
            ItemId = id;

            UnityEngine.Assertions.Assert.IsTrue(
                Database.ItemDatabase.ItemsDatas.ContainsKey(ItemId),
                $"Setting Item Collectable Id to {ItemId} but {Database.ItemDatabase.Instance.GetType().Name} doesn't know it.");
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

            if (!_addAfterFadeOut)
                Pickup();

            _highlight.SetActive(false);
            _itemSprite.enabled = false;
            StartCoroutine(FadeOutSmokeCoroutine());
            
            RSLib.Audio.AudioManager.PlaySound(_itemCollectedClipProvider);
        }

        private void Pickup()
        {
            UnityEngine.Assertions.Assert.IsFalse(string.IsNullOrEmpty(ItemId), "Picking up an item with unspecified Id.");
            Manager.GameManager.InventoryCtrl.AddItem(ItemId);

            // Items can be collected from chests, and not have any registered Id.
            // [TODO] However, they can be left behind and must then be saved and reloaded at the same place.
            if (Identifier != null)
                Manager.FlagsManager.Register(this);
        }

        private System.Collections.IEnumerator FadeOutSmokeCoroutine()
        {
            Vector3 smokeInitScale = _smoke.transform.localScale;

            for (float t = 0f; t < 1f; t += Time.deltaTime / _fadeOutDur)
            {
                if (t >= _fadeOutPercentageStop)
                    break;

                _smoke.transform.localScale = smokeInitScale * RSLib.Maths.Easing.Ease(1f - t, _fadeOutCurve);
                _smoke.color = _smoke.color.WithA(RSLib.Maths.Easing.Ease(1f - t, _fadeOutCurve));
                yield return null;
            }

            for (int i = _fadeOverParticles.Length - 1; i >= 0; --i)
                RSLib.Framework.Pooling.Pool.Get(_fadeOverParticles[i]).transform.position = transform.position;

            if (_addAfterFadeOut)
                Pickup();

            _container.SetActive(false);
        }

        private void Start()
        {
            if (Identifier != null && Manager.FlagsManager.Check(this))
                _container.SetActive(false);

            if (_itemId.Enabled && !string.IsNullOrEmpty(_itemId.Value))
                SetItemId(_itemId.Value);
        }
    }
}