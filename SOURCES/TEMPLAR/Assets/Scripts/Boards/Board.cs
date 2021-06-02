namespace Templar
{
    using UnityEngine;

    [DisallowMultipleComponent]
    public class Board : MonoBehaviour
    {
        [SerializeField] private BoardsLink[] _boardsLinks = null;
        // [TODO] Reference board camera bounds to update it.

        [ContextMenu("Locate Boards Links in Children")]
        private void LocateBoardsLinksInChildren()
        {
            _boardsLinks = GetComponentsInChildren<BoardsLink>();
            RSLib.EditorUtilities.PrefabEditorUtilities.SetCurrentPrefabStageDirty();
            RSLib.EditorUtilities.SceneManagerUtilities.SetCurrentSceneDirty();
        }
    }
}