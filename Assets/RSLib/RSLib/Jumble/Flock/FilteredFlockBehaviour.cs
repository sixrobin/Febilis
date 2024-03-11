namespace RSLib.Jumble.Flock
{
    using System.Collections.Generic;
    using UnityEngine;

    public abstract class FilteredFlockBehaviour : FlockBehaviour
    {
        [SerializeField] private ContextFilter[] _contextFilters = null;

        public List<Transform> FilterContext(FlockAgent agent, List<Transform> context)
        {
            if (_contextFilters == null || _contextFilters.Length == 0)
                return context;

            List<Transform> filtered = new List<Transform>(context);
            for (int i = _contextFilters.Length - 1; i >= 0; --i)
            {
                _contextFilters[i].FilterNonAlloc(agent, filtered);
                if (filtered.Count == 0)
                    return filtered;
            }

            return filtered;
        }
    }
}