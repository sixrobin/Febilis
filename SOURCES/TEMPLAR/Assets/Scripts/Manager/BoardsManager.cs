namespace Templar.Manager
{
    using UnityEngine;

    public class BoardsManager : RSLib.Framework.ConsoleProSingleton<BoardsManager>
    {
        [System.Serializable]
        private class BoardsLinkPair
        {
            [SerializeField] private BoardsLink _first = null;
            [SerializeField] private BoardsLink _second = null;

            public BoardsLink First => _first;
            public BoardsLink Second => _second;
        }

        [SerializeField] private BoardsLinkPair[] _links = null;
        // [TODO] SceneLoaders[]

        [Header("TRANSITION VIEW")]
        [SerializeField] private Datas.RampFadeDatas _fadeInDatas = null;
        [SerializeField] private Datas.RampFadeDatas _fadeOutDatas = null;
        [SerializeField] private float _fadedInDur = 0.5f;

        [Header("DEBUG")]
        [SerializeField] private RSLib.DataColor _debugColor = null;

        private static System.Collections.Generic.Dictionary<BoardsLink, BoardsLink> s_linksPairs = new System.Collections.Generic.Dictionary<BoardsLink, BoardsLink>();

        private static System.Collections.IEnumerator s_boardTransitionCoroutine;
        private static System.Collections.IEnumerator s_playerMovementCoroutine;

        public static bool IsInBoardTransition => s_boardTransitionCoroutine != null;

        public static void TriggerLink(BoardsLink link)
        {
            Instance.Log($"{link.transform.name} triggered.", link.gameObject);
            UnityEngine.Assertions.Assert.IsTrue(
                s_linksPairs.ContainsKey(link),
                $"Unknown {typeof(BoardsLink).Name} instance by {Instance.GetType().Name} on {link.transform.name}.");

            Instance.StartCoroutine(s_boardTransitionCoroutine = BoardTransitionCoroutine(link, s_linksPairs[link]));
        }

        private static System.Collections.IEnumerator BoardTransitionCoroutine(BoardsLink source, BoardsLink target)
        {
            source.gameObject.SetActive(false);
            target.gameObject.SetActive(false);

            GameManager.PlayerCtrl.AllowInputs(false);

            switch (source.ExitDir)
            {
                case CardinalDirection.EAST:
                case CardinalDirection.WEST:
                    Vector2 outDir = source.ExitDir.ConvertToVector2();
                    Instance.StartCoroutine(s_playerMovementCoroutine = GameManager.PlayerCtrl.MoveToDirection(outDir.x, 0.4f));
                    break;

                case CardinalDirection.NORTH:
                    // [TODO] Jump in.
                    break;

                case CardinalDirection.SOUTH:
                    // Just let player fall.
                    break;
            }

            RampFadeManager.Fade(GameManager.PlayerCtrl.CameraCtrl.GrayscaleRamp, Instance._fadeInDatas, (0f, 0f));
            yield return RSLib.Yield.SharedYields.WaitForEndOfFrame;
            yield return new WaitUntil(() => !RampFadeManager.IsFading);

            if (s_playerMovementCoroutine != null)
                Instance.StopCoroutine(s_playerMovementCoroutine);

            // This is fucking ugly, but for now we need to wait a frame after we reset the velocity and then
            // set the position again before groundind the character. Without waiting, we set the position, but the potential
            // fall velocity will make the character fall under the ground, and the Groud method won't work properly.
            GameManager.PlayerCtrl.ResetVelocity();
            GameManager.PlayerCtrl.transform.position = target.transform.position;
            yield return null;
            GameManager.PlayerCtrl.transform.position = target.transform.position;
            GameManager.PlayerCtrl.CollisionsCtrl.Ground(GameManager.PlayerCtrl.transform, true);
            
            yield return new WaitForSeconds(Instance._fadedInDur);

            RampFadeManager.Fade(GameManager.PlayerCtrl.CameraCtrl.GrayscaleRamp, Instance._fadeOutDatas, (0f, 0f));
            GameManager.PlayerCtrl.PlayerView.PlayIdleAnimation();

            switch (target.EnterDir)
            {
                case CardinalDirection.EAST:
                case CardinalDirection.WEST:
                    Vector2 inDir = target.EnterDir.ConvertToVector2();
                    Instance.StartCoroutine(s_playerMovementCoroutine = GameManager.PlayerCtrl.MoveToDirection(inDir.x, 0.4f));
                    break;

                case CardinalDirection.NORTH:
                    // [TODO] Jump in.
                    break;

                case CardinalDirection.SOUTH:
                    // Just let player fall.
                    break;
            }

            yield return RSLib.Yield.SharedYields.WaitForEndOfFrame;
            yield return new WaitUntil(() => !RampFadeManager.IsFading);

            GameManager.PlayerCtrl.AllowInputs(true);

            source.gameObject.SetActive(true);
            target.gameObject.SetActive(true);

            s_boardTransitionCoroutine = null;
            s_playerMovementCoroutine = null;
        }

        protected override void Awake()
        {
            for (int i = _links.Length - 1; i >= 0; --i)
            {
                s_linksPairs.Add(_links[i].First, _links[i].Second);
                s_linksPairs.Add(_links[i].Second, _links[i].First);
            }
        }

        private void OnDrawGizmos()
        {
            if (_links == null || _links.Length == 0)
                return;

            Gizmos.color = _debugColor?.Color ?? Color.yellow;

            for (int i = _links.Length - 1; i >= 0; --i)
                if (_links[i].First != null && _links[i].Second != null)
                    Gizmos.DrawLine(_links[i].First.transform.position, _links[i].Second.transform.position);
        }
    }
}