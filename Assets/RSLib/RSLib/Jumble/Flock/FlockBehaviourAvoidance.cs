namespace RSLib.Jumble.Flock
{
    using System.Collections.Generic;
    using UnityEngine;

    /// <summary>
    /// Avoidance behaviour makes the flock agents move away from transforms that are too close.
    /// This behaviour can be filtered so that agents only move away from some specific transforms.
    /// Radius can be overridden, else the flock values will be used.
    /// </summary>
    [CreateAssetMenu(fileName = "New Avoidance Flock Behaviour", menuName = "RSLib/Flock/Behaviour/Avoidance")]
    public class FlockBehaviourAvoidance : FilteredFlockBehaviour
    {
        [SerializeField] private RSLib.Framework.OptionalFloat _smoothTime = new RSLib.Framework.OptionalFloat(0f, false);
        [SerializeField] private RSLib.Framework.OptionalFloat _customRadius = new RSLib.Framework.OptionalFloat(0f, false);

        private Vector2 _refVelocity;

        public override Vector2 ComputeVelocity(FlockAgent agent, List<Transform> context, Flock flock)
        {
            if (context.Count == 0)
                return agent.transform.up;

            List<Transform> contextFiltered = FilterContext(agent, context);
            if (contextFiltered.Count == 0)
                return Vector2.zero;

            Vector2 avoidanceVelocity = Vector2.zero;
            int avoidedCount = 0;
            float radius = _customRadius.Enabled ? _customRadius.Value : flock.AvoidanceRadius * flock.AvoidanceRadius;

            for (int i = contextFiltered.Count - 1; i >= 0; --i)
            {
                if (Vector2.SqrMagnitude(contextFiltered[i].position - agent.transform.position) > radius)
                    continue;

                avoidanceVelocity += (Vector2)(agent.transform.position - contextFiltered[i].position);
                avoidedCount++;
            }

            if (avoidedCount > 0)
                avoidanceVelocity /= avoidedCount;

            if (_smoothTime.Enabled && _smoothTime.Value > 0f)
                avoidanceVelocity = Vector2.SmoothDamp(agent.transform.up, avoidanceVelocity, ref _refVelocity, _smoothTime.Value);

            return avoidanceVelocity;
        }
    }
}