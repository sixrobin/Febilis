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
            for (int i = _boardsLinks.Length - 1; i >= 0; --i)
                _boardsLinks[i].SetOwnerBoard(this);
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