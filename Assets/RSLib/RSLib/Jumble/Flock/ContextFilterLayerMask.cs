namespace RSLib.Jumble.Flock
{
    using System.Collections.Generic;
    using UnityEngine;

    [CreateAssetMenu(fileName = "New Layer Mask Context Filter", menuName = "RSLib/Flock/Context Filter/Layer Mask")]
    public class ContextFilterLayerMask : ContextFilter
    {
        [SerializeField] private LayerMask _layerMask = 0;

        public override List<Transform> Filter(FlockAgent agent, List<Transform> source)
        {
            List<Transform> filtered = new List<Transform>();

            for (int i = source.Count - 1; i >= 0; --i)
                if (_layerMask == (_layerMask | (1 << source[i].gameObject.layer)))
                    filtered.Add(source[i]);

            return filtered;
        }

        public override void FilterNonAlloc(FlockAgent agent, List<Transform> context)
        {
            for (int i = context.Count - 1; i >= 0; --i)
                if (_layerMask != (_layerMask | (1 << context[i].gameObject.layer)))
                    context.RemoveAt(i);
        }
    }
}