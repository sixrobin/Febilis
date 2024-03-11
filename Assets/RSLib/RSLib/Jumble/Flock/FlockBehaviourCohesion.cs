namespace RSLib.Jumble.Flock
{
    using System.Collections.Generic;
    using UnityEngine;

    /// <summary>
    /// Cohesion behaviour makes the flock agents move towards the average position of their neighbours.
    /// This behaviour can be filtered so that agents only move towards some specific transforms.
    /// </summary>
    [CreateAssetMenu(fileName = "New Cohesion Flock Behaviour", menuName = "RSLib/Flock/Behaviour/Cohesion")]
    public class FlockBehaviourCohesion : FilteredFlockBehaviour
    {
        [SerializeField] private RSLib.Framework.OptionalFloat _smoothTime = new RSLib.Framework.OptionalFloat(0f, false);

        private Vector2 _refVelocity;
        
        public override Vector2 ComputeVelocity(FlockAgent agent, List<Transform> context, Flock flock)
        {
            if (context.Count == 0)
                return Vector2.zero;

            List<Transform> contextFiltered = FilterContext(agent, context);
            if (contextFiltered.Count == 0)
                return Vector2.zero;
            
            Vector2 cohesionVelocity = Vector2.zero;
            for (int i = contextFiltered.Count - 1; i >= 0; --i)
                cohesionVelocity += (Vector2)contextFiltered[i].position;

            cohesionVelocity /= contextFiltered.Count;
            cohesionVelocity -= (Vector2)agent.transform.position; // Transform to local offset.

            if (_smoothTime.Enabled && _smoothTime.Value > 0f)
                cohesionVelocity = Vector2.SmoothDamp(agent.transform.up, cohesionVelocity, ref _refVelocity, _smoothTime.Value);
            
            return cohesionVelocity;
        }
    }
}
