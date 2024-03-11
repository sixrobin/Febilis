namespace RSLib.Jumble.Flock
{
    using System.Collections.Generic;
    using UnityEngine;

    public abstract class ContextFilter : ScriptableObject
    {
        public abstract List<Transform> Filter(FlockAgent agent, List<Transform> source);
        public abstract void FilterNonAlloc(FlockAgent agent, List<Transform> context);
    }
}