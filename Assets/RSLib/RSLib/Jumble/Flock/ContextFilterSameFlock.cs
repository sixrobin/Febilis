namespace RSLib.Jumble.Flock
{
    using System.Collections.Generic;
    using UnityEngine;

    [CreateAssetMenu(fileName = "New Same Flock Context Filter", menuName = "RSLib/Flock/Context Filter/Same Flock")]
    public class ContextFilterSameFlock : ContextFilter
    {
        public override List<Transform> Filter(FlockAgent agent, List<Transform> source)
        {
            List<Transform> filtered = new List<Transform>();

            for (int i = source.Count - 1; i >= 0; --i)
                if (source[i].TryGetComponent(out FlockAgent otherAgent) && otherAgent.Flock == agent.Flock)
                    filtered.Add(source[i]);

            return filtered;
        }

        public override void FilterNonAlloc(FlockAgent agent, List<Transform> context)
        {
            for (int i = context.Count - 1; i >= 0; --i)
                if (context[i].TryGetComponent(out FlockAgent otherAgent) && otherAgent.Flock != agent.Flock)
                    context.RemoveAt(i);
        }
    }
}