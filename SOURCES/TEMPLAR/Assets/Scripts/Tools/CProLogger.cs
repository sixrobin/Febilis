public static class CProLogger
{
    public static void Log(object caller, string log, UnityEngine.GameObject context = null)
    {
        UnityEngine.Debug.Log($"#{caller.GetType().Name}#{log}", context);
    }

    public static void LogWarning(object caller, string log, UnityEngine.GameObject context = null)
    {
        UnityEngine.Debug.LogWarning($"#{caller.GetType().Name}#{log}", context);
    }

    public static void LogError(object caller, string log, UnityEngine.GameObject context = null)
    {
        UnityEngine.Debug.LogError($"#{caller.GetType().Name}#{log}", context);
    }
}