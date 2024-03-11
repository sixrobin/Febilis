namespace RSLib.Jumble.Flock
{
    using System.Collections.Generic;
    using UnityEngine;

    /// <summary>
    /// Alignment behaviour makes the flock agents look toward the average direction of their neighbour.
    /// This behaviour can be filtered so that agents only look using the direction of some specific transforms.
    /// </summary>
    [CreateAssetMenu(fileName = "New Alignment Flock Behaviour", menuName = "RSLib/Flock/Behaviour/Alignment")]
    public class FlockBehaviourAlignment : FilteredFlockBehaviour
    {
        [SerializeField] private RSLib.Framework.OptionalFloat _smoothTime = new RSLib.Framework.OptionalFloat(0f, false);

        private Vector2 _refVelocity;

        public override Vector2 ComputeVelocity(FlockAgent agent, List<Transform> context, Flock flock)
        {
            if (context.Count == 0)
                return agent.transform.up;

            List<Transform> contextFiltered = FilterContext(agent, context);
            if (contextFiltered.Count == 0)
                return Vector2.zero;

            Vector2 alignmentVelocity = Vector2.zero;
            for (int i = contextFiltered.Count - 1; i >= 0; --i)
                alignmentVelocity += (Vector2)contextFiltered[i].up;

            alignmentVelocity /= contextFiltered.Count;
            alignmentVelocity.Normalize();

            if (_smoothTime.Enabled && _smoothTime.Value > 0f)
                alignmentVelocity = Vector2.SmoothDamp(agent.transform.up, alignmentVelocity, ref _refVelocity, _smoothTime.Value);

            return alignmentVelocity;
        }
    }
}