namespace Templar.Manager
{
    public class FreezeFrameManager : RSLib.Framework.ConsoleProSingleton<FreezeFrameManager>
    {
        private System.Collections.IEnumerator _freezeFrameCoroutine;

        public static bool IsFroze => Exists() && Instance._freezeFrameCoroutine != null;

        public static void FreezeFrame(int framesDelay, float dur, float targetTimeScale = 0f, bool overrideCurrFreeze = false)
        {
            if (!Exists() || dur == 0f)
                return;

            if (Instance._freezeFrameCoroutine != null)
            {
                if (!overrideCurrFreeze)
                    return;

                Instance.StopCoroutine(Instance._freezeFrameCoroutine);
            }

            Instance.StartCoroutine(Instance._freezeFrameCoroutine = FreezeFrameCoroutine(framesDelay, dur, targetTimeScale));
        }

        private static System.Collections.IEnumerator FreezeFrameCoroutine(int framesDelay, float dur, float targetTimeScale = 0f)
        {
            for (int i = 0; i < framesDelay; ++i)
                yield return RSLib.Yield.SharedYields.WaitForEndOfFrame;

            UnityEngine.Time.timeScale = targetTimeScale;
            yield return RSLib.Yield.SharedYields.WaitForSecondsRealtime(dur);
            UnityEngine.Time.timeScale = 1f;

            Instance._freezeFrameCoroutine = null;
        }
    }
}