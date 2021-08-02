namespace Templar.Manager
{
    using System.Linq;
    using UnityEngine;
#if UNITY_EDITOR
    using UnityEditor;
#endif

    public class BoardsLinksManager : RSLib.Framework.ConsoleProSingleton<BoardsLinksManager>
    {
        [SerializeField] private Boards.BoardsLink[] _boardsLinks = null;

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

        [ContextMenu("Find All References")]
        private void DebugFindAllReferences()
        {
            _boardsLinks = FindObjectsOfType<Boards.BoardsLink>();
        }

        [ContextMenu("Find Missing References")]
        private void DebugFindMissingReferences()
        {
            if (_boardsLinks == null || _boardsLinks.Length == 0 || _boardsLinks.Where(o => o != null).Count() == 0)
                _boardsLinks = FindObjectsOfType<Boards.BoardsLink>();
        }

        // [TODO] OnDrawGizmos to visualize links.
    }
}