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

        [SerializeField] private Datas.RampFadeDatas _fadeInDatas = null;
        [SerializeField] private Datas.RampFadeDatas _fadeOutDatas = null;

        [Header("DEBUG")]
        [SerializeField] private RSLib.DataColor _debugColor = null;

        private static System.Collections.Generic.Dictionary<BoardsLink, BoardsLink> s_linksPairs = new System.Collections.Generic.Dictionary<BoardsLink, BoardsLink>();
        private static System.Collections.IEnumerator s_boardTransitionCoroutine;

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
            // Make player runs/jump/fall to a given point (let him just fall if link is set to SOUTH, not to have strange y vel change ?).

            RampFadeManager.Fade(GameManager.PlayerCtrl.CameraCtrl.GrayscaleRamp, Instance._fadeInDatas, (0f, 0f));
            yield return RSLib.Yield.SharedYields.WaitForEndOfFrame;
            yield return new WaitUntil(() => !RampFadeManager.IsFading);

            GameManager.PlayerCtrl.transform.position = target.transform.position;
            GameManager.PlayerCtrl.CollisionsCtrl.Ground(GameManager.PlayerCtrl.transform); // Doesn't seem to work ?

            yield return new WaitForSeconds(0.5f); // [TODO] Expose.

            RampFadeManager.Fade(GameManager.PlayerCtrl.CameraCtrl.GrayscaleRamp, Instance._fadeOutDatas, (0f, 0f));

            Vector2 inDir = target.EnterDir.ConvertToVector2();

            // [TODO] Not if falling.
            // [TODO] If going up, a teleport for now, but a scripted jump later ?

            // This looks terrible, but we should use a coroutine in PlayerController to handle all of this a better way.
            // So for now we let it as it is.
            for (float t = 0f; t < 1f; t += Time.deltaTime / 0.5f)
            {
                GameManager.PlayerCtrl.Translate(inDir * 5f, true, true, false);
                yield return null;
            }

            yield return RSLib.Yield.SharedYields.WaitForEndOfFrame;
            yield return new WaitUntil(() => !RampFadeManager.IsFading);

            GameManager.PlayerCtrl.AllowInputs(true);

            source.gameObject.SetActive(true);
            target.gameObject.SetActive(true);

            s_boardTransitionCoroutine = null;
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