namespace RSLib.Jumble.Flock
{
    using System.Collections.Generic;
    using UnityEngine;

    [CreateAssetMenu(fileName = "New Same Tag Context Filter", menuName = "RSLib/Flock/Context Filter/Same Tag")]
    public class ContextFilterSameTag : ContextFilter
    {
        public override List<Transform> Filter(FlockAgent agent, List<Transform> source)
        {
            List<Transform> filtered = new List<Transform>();

            for (int i = source.Count - 1; i >= 0; --i)
                if (source[i].gameObject.CompareTag(agent.gameObject.tag))
                    filtered.Add(source[i]);

            return filtered;
        }

        public override void FilterNonAlloc(FlockAgent agent, List<Transform> context)
        {
            for (int i = context.Count - 1; i >= 0; --i)
                if (!context[i].gameObject.CompareTag(agent.gameObject.tag))
                    context.RemoveAt(i);
        }
    }
}