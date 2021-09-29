﻿namespace Templar.Boards
{
    using UnityEngine;
#if UNITY_EDITOR
    using UnityEditor;
#endif
    
    [DisallowMultipleComponent]
#if UNITY_EDITOR
    [CanEditMultipleObjects]
#endif
    public class Board : MonoBehaviour
    {
        [SerializeField] private BoardsLink[] _boardsLinks = null;
        [SerializeField] private RSLib.DataColor _backgroundColor = null;

        public Color BackgroundColor => _backgroundColor?.Color ?? Color.grey;

        private void Awake()
        {
#if UNITY_EDITOR
            int childrenLinksCount = GetComponentsInChildren<BoardsLink>().Length;
            if (childrenLinksCount != _boardsLinks.Length)
                CProLogger.LogWarning(this, $"{_boardsLinks.Length} links are referenced while {childrenLinksCount} have been found in children for board {transform.name}.", gameObject);
#endif

            for (int i = _boardsLinks.Length - 1; i >= 0; --i)
                _boardsLinks[i].SetBoard(this);
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