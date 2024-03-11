namespace RSLib.Jumble.Flock
{
    using System.Collections.Generic;
    using UnityEngine;

    [CreateAssetMenu(fileName = "New Same Layer Context Filter", menuName = "RSLib/Flock/Context Filter/Same Layer")]
    public class ContextFilterSameLayer : ContextFilter
    {
        public override List<Transform> Filter(FlockAgent agent, List<Transform> source)
        {
            List<Transform> filtered = new List<Transform>();

            for (int i = source.Count - 1; i >= 0; --i)
                if (source[i].gameObject.layer == agent.gameObject.layer)
                    filtered.Add(source[i]);

            return filtered;
        }

        public override void FilterNonAlloc(FlockAgent agent, List<Transform> context)
        {
            for (int i = context.Count - 1; i >= 0; --i)
                if (context[i].gameObject.layer != agent.gameObject.layer)
                    context.RemoveAt(i);
        }
    }
}