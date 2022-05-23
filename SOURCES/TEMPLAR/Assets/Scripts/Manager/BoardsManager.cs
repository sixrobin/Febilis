namespace Templar.Manager
{
    using System.Linq;
    using UnityEngine;
#if UNITY_EDITOR
    using UnityEditor;
#endif

    public class BoardsManager : RSLib.Framework.SingletonConsolePro<BoardsManager>
    {
        [Header("ALL SCENE BOARDS")]
        [SerializeField] private Boards.BoardsLink[] _boardsLinks = null;

        [Header("DEBUG")]
        [SerializeField] private RSLib.Data.Color _debugColor = null;

        private static bool s_init;

        public static Boards.Board CurrentBoard { get; private set; }

        public static RSLib.Data.Color DebugColor => Instance._debugColor;

        /// <summary>
        /// Tries to get the BoardsLinks the given ScenesPassage leads to.
        /// This method will assert exactly one link exists, since having none or more than one would be a design error.
        /// </summary>
        /// <param name="passage">Source ScenesPassage.</param>
        /// <returns>The target link if it has been found.</returns>
        public static Boards.BoardsLink GetLinkRelatedToScenesPassage(Boards.ScenesPassage passage)
        {
            System.Collections.Generic.IEnumerable<Boards.BoardsLink> boardsLinks = Instance._boardsLinks.Where(o => o.CompareTargetPassage(passage));

            UnityEngine.Assertions.Assert.IsNotNull(
                boardsLinks.FirstOrDefault(),
                $"No BoardsLink related to ScenesPassage {passage.name} has been found.");

            UnityEngine.Assertions.Assert.IsFalse(
                boardsLinks.Count() > 1,
                $"More than one BoardsLink related to ScenesPassage {passage.name} have been found. Names are {string.Join(",", boardsLinks.Select(o => o.transform.name))}.");

            return boardsLinks.First();
        }

        public static void Init()
        {
            if (s_init)
                return;

            Boards.Board.BoardEntered += OnBoardEntered;

            for (int i = Instance._boardsLinks.Length - 1; i >= 0; --i)
                if (Instance._boardsLinks[i].BoardBounds != null)
                    for (int j = Instance._boardsLinks[i].BoardBounds.Switches.Length - 1; j >= 0; --j)
                        Instance._boardsLinks[i].BoardBounds.Switches[j].Enable(true);

            s_init = true;
        }

        public static Boards.Board DebugForceRefreshInitBoard()
        {
            Init();

            Boards.BoardBounds[] boardsBounds = FindObjectsOfType<Boards.BoardBounds>();
            Unit.Player.PlayerController playerCtrl = FindObjectOfType<Unit.Player.PlayerController>();

            for (int i = boardsBounds.Length - 1; i >= 0; --i)
            {
                if (boardsBounds[i].Bounds.bounds.Contains(playerCtrl.transform.position))
                {
                    GameManager.CameraCtrl.SetBoardBounds(boardsBounds[i]);
                    if (boardsBounds[i].Board != null)
                        GameManager.CameraCtrl.SetBackgroundColor(boardsBounds[i].Board.BackgroundColor);

                    CurrentBoard = boardsBounds[i].Board;

                    return boardsBounds[i].Board;
                }
            }

            return null;
        }

        private static void OnBoardEntered(Boards.Board board)
        {
            CurrentBoard = board;
            PaletteManager.UpdatePaletteForCurrentZone();
        }

        protected override void Awake()
        {
            base.Awake();
            Init();
        }

        private void OnDestroy()
        {
            s_init = false;
            Boards.Board.BoardEntered -= OnBoardEntered;
        }

        [ContextMenu("Find All References")]
        private void DebugFindAllReferences()
        {
            _boardsLinks = FindObjectsOfType<Boards.BoardsLink>();
            RSLib.EditorUtilities.SceneManagerUtilities.SetCurrentSceneDirty();
        }

        [ContextMenu("Find Missing References")]
        private void DebugFindMissingReferences()
        {
            if (_boardsLinks == null || _boardsLinks.Length == 0 || _boardsLinks.Where(o => o != null).Count() == 0)
                _boardsLinks = FindObjectsOfType<Boards.BoardsLink>();

            RSLib.EditorUtilities.SceneManagerUtilities.SetCurrentSceneDirty();
        }

        private void OnDrawGizmos()
        {
            for (int i = _boardsLinks.Length - 1; i >= 0; --i)
                _boardsLinks[i].DebugDrawLineToTarget();
        }
    }

#if UNITY_EDITOR
    [CustomEditor(typeof(BoardsManager))]
    public class BoardsManagerEditor : RSLib.EditorUtilities.ButtonProviderEditor<BoardsManager>
    {
        protected override void DrawButtons()
        {
            DrawButton("Refresh Current Board", () => BoardsManager.DebugForceRefreshInitBoard());
        }
    }
#endif
}