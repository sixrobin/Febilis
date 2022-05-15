namespace Templar
{
    using UnityEngine;

    [DisallowMultipleComponent]
    public class SplashScreen : MonoBehaviour
    {
        private const string SHOW = "Show";
        private const string HIDE = "Hide";

        [SerializeField] private RSLib.Framework.SceneField _sceneToLoad = null;
        [SerializeField] private Animator _logoAnimator = null;
        [SerializeField] private float _logoAppearDelay = 1f;
        [SerializeField] private float _logoShowDuration = 2f;
        [SerializeField] private float _postLogoHideDelay = 2f;
        [SerializeField] private RSLib.Framework.DisabledFloat _fullSplashScreenDuration = default;
        
        private System.Collections.IEnumerator SplashScreenCoroutine()
        {
            yield return RSLib.Yield.SharedYields.WaitForSeconds(_logoAppearDelay);
            _logoAnimator.SetTrigger(SHOW);
            yield return RSLib.Yield.SharedYields.WaitForSeconds(_logoShowDuration);
            _logoAnimator.SetTrigger(HIDE);
            yield return RSLib.Yield.SharedYields.WaitForSeconds(_postLogoHideDelay);

            UnityEngine.SceneManagement.SceneManager.LoadScene(_sceneToLoad);
        }
        
        private void Start()
        {
            StartCoroutine(SplashScreenCoroutine());
        }

        private void OnValidate()
        {
            _fullSplashScreenDuration = new RSLib.Framework.DisabledFloat(_logoAppearDelay + _logoShowDuration + _postLogoHideDelay);
        }
    }
}
