namespace Templar.Boards
{
    using UnityEngine;
#if UNITY_EDITOR
    using UnityEditor;
#endif

    public class InteractableBoardsLink : BoardsLink, Interaction.IInteractable
    {
        [Header("INTERACTABLE REFS")]
        [SerializeField] private Collider2D _collider = null;
        [SerializeField] private SpriteRenderer _spriteRenderer = null;
        [SerializeField] private GameObject _highlight = null;
        [SerializeField] private Sprite _interactedSprite = null;

        [Header("AUDIO")]
        [SerializeField] private RSLib.Audio.ClipProvider _interactedClipProvider = null;
        
        private Sprite _baseSprite;

        public string[] ValidItems => null;
        
        public override void OnBoardsTransitionBegan()
        {
            _collider.enabled = false;
            _spriteRenderer.sprite = _interactedSprite;
            Unfocus();
            
            RSLib.Audio.AudioManager.PlaySound(_interactedClipProvider);
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

#if UNITY_EDITOR
    [CustomEditor(typeof(InteractableBoardsLink))]
    public class InteractableBoardsLinkEditor : BoardsLinkEditor
    {
    }
#endif
}