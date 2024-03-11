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

        [Header("AUDIO")]
        [SerializeField] private bool _muteMusic = false;
        [SerializeField] private float _muteMusicDuration = 0.5f;
        [SerializeField] private RSLib.Maths.Curve _muteMusicCurve = RSLib.Maths.Curve.InOutSine;
        
        void IBoardTransitionHandler.OnBoardsTransitionBegan()
        {
            if (_muteMusic)
                Templar.Manager.MusicManager.StopMusic(_muteMusicDuration, _muteMusicCurve);
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