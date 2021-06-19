namespace Templar.Boards
{
    using UnityEngine;
#if UNITY_EDITOR
    using UnityEditor;
#endif

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

        public void LocateBoardsLinksInChildren()
        {
            _boardsLinks = GetComponentsInChildren<BoardsLink>();
#if UNITY_EDITOR
            RSLib.EditorUtilities.PrefabEditorUtilities.SetCurrentPrefabStageDirty();
            RSLib.EditorUtilities.SceneManagerUtilities.SetCurrentSceneDirty();
#endif
        }
    }

#if UNITY_EDITOR
    [CustomEditor(typeof(Board))]
    public class BoardEditor : RSLib.EditorUtilities.ButtonProviderEditor<Board>
    {
        protected override void DrawButtons()
        {
            DrawButton("Locate Boards Links in Children", Obj.LocateBoardsLinksInChildren);
        }
    }
#endif
}