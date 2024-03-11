namespace RSLib.Yield
{
    using UnityEngine;

    /// <summary>
    /// Custom yield instruction used to wait for seconds, but allow to break the waiting if a condition is filled.
    /// Can be used to wait before doing something but allow the player to skip with some input, for example.
    /// </summary>
    public class WaitForSecondsOrBreakIf : CustomYieldInstruction
    {
        private System.Func<bool> _breakCondition;
        private System.Action _breakCallback;
        private float _seconds = 0f;

        public WaitForSecondsOrBreakIf(float seconds, System.Func<bool> breakCondition, System.Action breakCallback = null)
        {
            _breakCondition = breakCondition;
            _breakCallback = breakCallback;
            _seconds = seconds;
        }

        public override bool keepWaiting
        {
            get
            {
                _seconds -= Time.deltaTime;
                if (_seconds <= 0f)
                    return false;

                if (_breakCondition())
                {
                    _breakCallback?.Invoke();
                    return false;
                }

                return true;
            }
        }
    }
}