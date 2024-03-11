namespace RSLib.Jumble.Flock
{
    using System.Collections.Generic;
    using UnityEngine;

    public abstract class FlockBehaviour : ScriptableObject
    {
        public abstract Vector2 ComputeVelocity(FlockAgent agent, List<Transform> context, Flock flock);
    }
}