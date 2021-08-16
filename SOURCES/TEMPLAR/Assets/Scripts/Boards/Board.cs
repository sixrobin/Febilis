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

        private void Awake()
        {
#if UNITY_EDITOR
            int childrenLinksCount = GetComponentsInChildren<BoardsLink>().Length;
            if (childrenLinksCount != _boardsLinks.Length)
                CProLogger.LogWarning(this, $"{_boardsLinks.Length} links are referenced while {childrenLinksCount} have been found in children for board {transform.name}.", gameObject);
#endif
        }

        public void LocateBoardsLinksInChildren()
        {
            _boardsLinks = GetComponentsInChildren<BoardsLink>();
            RSLib.EditorUtilities.PrefabEditorUtilities.SetCurrentPrefabStageDirty();
            RSLib.EditorUtilities.SceneManagerUtilities.SetCurrentSceneDirty();
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