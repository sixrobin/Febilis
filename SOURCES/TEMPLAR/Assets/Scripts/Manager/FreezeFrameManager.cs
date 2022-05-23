namespace Templar.Manager
{
    public class FreezeFrameManager : RSLib.Framework.SingletonConsolePro<FreezeFrameManager>
    {
        private System.Collections.IEnumerator _freezeFrameCoroutine;

        public static bool IsFroze => Exists() && Instance._freezeFrameCoroutine != null;

        public static void FreezeFrame(float delay, float dur, float targetTimeScale = 0f, bool overrideCurrFreeze = false, System.Action callback = null)
        {
            if (!Exists() || dur == 0f)
                return;

            if (Instance._freezeFrameCoroutine != null)
            {
                if (!overrideCurrFreeze)
                    return;

                Instance.StopCoroutine(Instance._freezeFrameCoroutine);
            }

            Instance.StartCoroutine(Instance._freezeFrameCoroutine = FreezeFrameCoroutine(delay, dur, targetTimeScale, callback));
        }

        private static System.Collections.IEnumerator FreezeFrameCoroutine(float delay, float dur, float targetTimeScale = 0f, System.Action callback = null)
        {
            yield return RSLib.Yield.SharedYields.WaitForSecondsRealtime(delay);
            UnityEngine.Time.timeScale = targetTimeScale;
            yield return RSLib.Yield.SharedYields.WaitForSecondsRealtime(dur);
            UnityEngine.Time.timeScale = 1f;

            Instance._freezeFrameCoroutine = null;
            callback?.Invoke();
        }
    }
}