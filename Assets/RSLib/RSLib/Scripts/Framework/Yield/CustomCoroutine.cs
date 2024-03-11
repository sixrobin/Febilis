namespace RSLib.Yield
{
    using System.Collections;
    using UnityEngine;

    /// <summary>
    /// Coroutine wrapper adding a property to know if the coroutine is done and an event called on completion.
    /// </summary>
    public class CustomCoroutine : IEnumerator
    {
        public CustomCoroutine(MonoBehaviour runner, IEnumerator coroutine)
        {
            Current = runner.StartCoroutine(Wrap(coroutine));
        }

        public CustomCoroutine(MonoBehaviour runner, IEnumerator coroutine, System.Action<CustomCoroutine> callback) : this(runner, coroutine)
        {
            Completed += callback;
        }
        
        public event System.Action<CustomCoroutine> Completed;
        
        public bool IsDone { get; private set; }
        
        public object Current { get; }

        public bool MoveNext()
        {
            return !IsDone;
        }

        public void Reset()
        {
            Debug.LogError("Reset!");
        }

        private IEnumerator Wrap(IEnumerator coroutine)
        {
            yield return coroutine;
            IsDone = true;
            Completed?.Invoke(this);
        }
    }

    /// <summary>
    /// Utilities class adding extensions method to run CustomCoroutines with an easily readable code.
    /// </summary>
    public static class CustomCoroutineUtilities
    {
        public static Yield.CustomCoroutine RunCustomCoroutine(this MonoBehaviour runner, System.Collections.IEnumerator coroutine)
        {
            return new RSLib.Yield.CustomCoroutine(runner, coroutine);
        }
        
        public static Yield.CustomCoroutine RunCustomCoroutine(this MonoBehaviour runner, System.Collections.IEnumerator coroutine, System.Action<RSLib.Yield.CustomCoroutine> callback)
        {
            return new RSLib.Yield.CustomCoroutine(runner, coroutine, callback);
        }
    }
}
