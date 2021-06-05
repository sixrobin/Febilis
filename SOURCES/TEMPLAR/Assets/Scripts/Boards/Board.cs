namespace Templar.Boards
{
    using UnityEngine;

    [DisallowMultipleComponent]
    public class Board : MonoBehaviour
    {
        [SerializeField] private BoardsLink[] _boardsLinks = null;
        [SerializeField] private BoxCollider2D _cameraBounds = null;

        public BoxCollider2D CameraBounds => _cameraBounds;

        private void Awake()
        {
#if UNITY_EDITOR
            int childrenLinksCount = GetComponentsInChildren<BoardsLink>().Length;
            if (childrenLinksCount != _boardsLinks.Length)
                CProLogger.LogWarning(this, $"{_boardsLinks.Length} links are referenced while {childrenLinksCount} have been found in children for board {transform.name}.", gameObject);
#endif

            for (int i = _boardsLinks.Length - 1; i >= 0; --i)
                _boardsLinks[i].OwnerBoard = this;
        }

        [ContextMenu("Locate Boards Links in Children")]
        private void LocateBoardsLinksInChildren()
        {
            _boardsLinks = GetComponentsInChildren<BoardsLink>();
            RSLib.EditorUtilities.PrefabEditorUtilities.SetCurrentPrefabStageDirty();
            RSLib.EditorUtilities.SceneManagerUtilities.SetCurrentSceneDirty();
        }
    }
}