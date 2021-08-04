namespace Templar.Manager
{
    using System.Linq;
    using UnityEngine;
#if UNITY_EDITOR
    using UnityEditor;
#endif

    public class BoardsManager : RSLib.Framework.ConsoleProSingleton<BoardsManager>
    {
        [Header("ALL SCENE BOARDS")]
        [SerializeField] private Boards.BoardsLink[] _boardsLinks = null;
        
        [Header("DEBUG")]
        [SerializeField] private RSLib.DataColor _debugColor = null;

        public static RSLib.DataColor DebugColor => Instance._debugColor;

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

        public static void DebugForceRefreshBoard()
        {
            Boards.Board[] boards = FindObjectsOfType<Boards.Board>();
            Unit.Player.PlayerController playerCtrl = FindObjectOfType<Unit.Player.PlayerController>();

            for (int i = boards.Length - 1; i >= 0; --i)
                if (boards[i].CameraBounds.bounds.Contains(playerCtrl.transform.position))
                    Manager.GameManager.CameraCtrl.SetBoardBounds(boards[i]);
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
    public class BoardsLinksManagerEditor : RSLib.EditorUtilities.ButtonProviderEditor<BoardsManager>
    {
        protected override void DrawButtons()
        {
            DrawButton("Refresh Current Board", BoardsManager.DebugForceRefreshBoard);
        }
    }
#endif
}