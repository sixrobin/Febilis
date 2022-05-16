namespace Templar.Boards
{
    using UnityEngine;
#if UNITY_EDITOR
    using UnityEditor;
#endif

    [CreateAssetMenu(fileName = "New Scene Loader", menuName = "Boards Transitions/Scene Loader")]
    public class SceneLoader : ScriptableObject, IBoardTransitionHandler
    {
        [SerializeField] private RSLib.Framework.SceneField _scene = null;

        void IBoardTransitionHandler.OnBoardsTransitionBegan()
        {
            // No callback is needed, this class only implements the interface so that it can be converted.
        }

        void IBoardTransitionHandler.OnBoardsTransitionOver()
        {
            // No callback is needed, this class only implements the interface so that it can be converted.
        }

        public RSLib.Framework.SceneField GetTargetScene()
        {
            return _scene;
        }
    }
}