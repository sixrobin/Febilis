public class FreezeFrameController : RSLib.Framework.Singleton<FreezeFrameController>
{
    private System.Collections.IEnumerator _freezeFrameCoroutine;

    public static bool IsFroze => Exists() && Instance._freezeFrameCoroutine != null;

    public static void FreezeFrame(int framesDelay, float dur, float targetTimeScale = 0f)
    {
        if (!Exists())
            return;

        Instance._freezeFrameCoroutine = FreezeFrameCoroutine(framesDelay, dur, targetTimeScale);
        Instance.StartCoroutine(Instance._freezeFrameCoroutine);
    }

    private static System.Collections.IEnumerator FreezeFrameCoroutine(int framesDelay, float dur, float targetTimeScale = 0f)
    {
        for (int i = 0; i < framesDelay; ++i)
            yield return null;

        UnityEngine.Time.timeScale = targetTimeScale;
        yield return RSLib.Yield.SharedYields.WaitForSecondsRealtime(dur);
        UnityEngine.Time.timeScale = 1f;

        Instance._freezeFrameCoroutine = null;
    }
}