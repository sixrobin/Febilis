namespace Templar.Boards
{
    using UnityEngine;
#if UNITY_EDITOR
    using UnityEditor;
#endif
    
    [DisallowMultipleComponent]
#if UNITY_EDITOR
    [CanEditMultipleObjects]
#endif
    public class Board : MonoBehaviour, Flags.IIdentifiable
    {
        [Header("IDENTIFIER")]
        [SerializeField] private Flags.BoardIdentifier _boardIdentifier = null;
        
        [Header("REFS")]
        [SerializeField] private RSLib.DataColor _backgroundColor = null;
        [SerializeField] private BoardsLink[] _boardsLinks = null;
        [SerializeField] private GameObject[] _disableWhenNotCurrentBoard = null;

        public delegate void BoardEventHandler(Board board);
        public static event BoardEventHandler BoardEntered;

        public Color BackgroundColor => _backgroundColor?.Color ?? Color.grey;

        public Flags.IIdentifier Identifier => _boardIdentifier;

        public void OnBoardEntered()
        {
            if (_boardIdentifier == null)
            {
                CProLogger.LogWarning(this, $"Identifier is missing on Board {transform.name}, cannot flag it.", gameObject);
                return;
            }

            ToggleBoardObjects(true);

            BoardEntered?.Invoke(this);
            Manager.FlagsManager.Register(this);
        }
        
        public void OnBoardExit()
        {
            ToggleBoardObjects(false);
        }

        private void ToggleBoardObjects(bool state)
        {
            for (int i = _disableWhenNotCurrentBoard.Length - 1; i >= 0; --i)
                _disableWhenNotCurrentBoard[i].SetActive(state);
        }
        
        private void Awake()
        {
#if UNITY_EDITOR
            int childrenLinksCount = GetComponentsInChildren<BoardsLink>().Length;
            if (childrenLinksCount != _boardsLinks.Length)
                CProLogger.LogWarning(this, $"{_boardsLinks.Length} links are referenced while {childrenLinksCount} have been found in children for board {transform.name}.", gameObject);
#endif

            for (int i = _boardsLinks.Length - 1; i >= 0; --i)
                _boardsLinks[i].SetBoard(this);
            
            ToggleBoardObjects(false);
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