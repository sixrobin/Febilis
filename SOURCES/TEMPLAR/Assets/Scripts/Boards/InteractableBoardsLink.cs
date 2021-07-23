namespace Templar.Boards
{
    using UnityEngine;

    public class InteractableBoardsLink : BoardsLink, Interaction.IInteractable
    {
        [Header("INTERACTABLE REFS")]
        [SerializeField] private Collider2D _collider = null;
        [SerializeField] private SpriteRenderer _spriteRenderer = null;
        [SerializeField] private GameObject _highlight = null;
        [SerializeField] private Sprite _interactedSprite = null;

        private Sprite _baseSprite;

        public override void OnBoardsTransitionBegan()
        {
            _collider.enabled = false;
            _spriteRenderer.sprite = _interactedSprite;
            Unfocus();
        }

        public override void OnBoardsTransitionOver()
        {
            _collider.enabled = true;
            _spriteRenderer.sprite = _baseSprite;
        }

        public void Focus()
        {
            _highlight.SetActive(true);
        }

        public void Unfocus()
        {
            _highlight.SetActive(false);
        }

        public void Interact()
        {
            Trigger();
        }

        private void Awake()
        {
            _baseSprite = _spriteRenderer.sprite;
        }
    }
}