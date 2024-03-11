namespace Templar.Manager
{
    using UnityEngine;

    public class DebugManager : RSLib.Framework.SingletonConsolePro<DebugManager>
    {
        [SerializeField] private bool _assertsRaiseExceptions = false;

        protected override void Awake()
        {
            base.Awake();
            if (!IsValid)
                return;

#pragma warning disable 0618
            UnityEngine.Assertions.Assert.raiseExceptions = _assertsRaiseExceptions;
#pragma warning restore 0618
        }
    }
}