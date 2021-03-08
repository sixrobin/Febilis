namespace RSLib.Yield
{
    using System.Collections.Generic;
    using UnityEngine;

    public static class SharedYields
    {
        private static Dictionary<int, WaitForFrames> s_waitForFramesCollection = new Dictionary<int, WaitForFrames>(100, new Framework.Comparers.IntComparer());
        private static Dictionary<float, WaitForSecondsRealtime> s_waitForFramesRealtimeCollection = new Dictionary<float, WaitForSecondsRealtime>(100, new Framework.Comparers.FloatComparer());
        private static Dictionary<float, WaitForSeconds> s_waitsForSeconds = new Dictionary<float, WaitForSeconds>(100, new Framework.Comparers.FloatComparer());

        public static WaitForEndOfFrame WaitForEndOfFrame { get; } = new WaitForEndOfFrame();
        public static WaitForFixedUpdate WaitForFixedUpdate { get; } = new WaitForFixedUpdate();

        /// <summary>Returns an existing WaitForFrames if one with the given duration has already been pooled, else a new one and pools it.</summary>
        /// <param name="duration">Frames to wait.</param>
        /// <returns>WaitForFrames instance.</returns>
        public static WaitForFrames WaitForFrames(int framesCount)
        {
            if (!s_waitForFramesCollection.TryGetValue(framesCount, out WaitForFrames wait))
                s_waitForFramesCollection.Add(framesCount, wait = new WaitForFrames(framesCount));

            return wait;
        }

        /// <summary>Returns an existing WaitForSeconds if one with the given duration has already been pooled, else a new one and pools it.</summary>
        /// <param name="duration">Seconds to wait.</param>
        /// <returns>WaitForSeconds instance.</returns>
        public static WaitForSeconds WaitForSeconds(float duration)
        {
            if (!s_waitsForSeconds.TryGetValue(duration, out WaitForSeconds wait))
                s_waitsForSeconds.Add(duration, wait = new WaitForSeconds(duration));

            return wait;
        }

        /// <summary>Returns an existing WaitForSecondsRealtime if one with the given duration has already been pooled, else a new one and pools it.</summary>
        /// <param name="duration">Seconds to wait.</param>
        /// <returns>WaitForSecondsRealtime instance.</returns>
        public static WaitForSecondsRealtime WaitForSecondsRealtime(float duration)
        {
            if (!s_waitForFramesRealtimeCollection.TryGetValue(duration, out WaitForSecondsRealtime wait))
                s_waitForFramesRealtimeCollection.Add(duration, wait = new WaitForSecondsRealtime(duration));

            return wait;
        }
    }

    public class WaitForFrames : CustomYieldInstruction
    {
        private int _framesCount = 0;

        public WaitForFrames(int framesCount = 1) : base()
        {
            _framesCount = framesCount;
        }

        public override bool keepWaiting => _framesCount-- > 0;
    }

    public class WaitWhile : CustomYieldInstruction
    {
        private System.Func<bool> _predicate;

        public WaitWhile(System.Func<bool> predicate)
        {
            _predicate = predicate;
        }

        public override bool keepWaiting => _predicate();
    }
}