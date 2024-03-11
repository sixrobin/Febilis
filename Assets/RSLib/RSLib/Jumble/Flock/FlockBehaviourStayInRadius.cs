namespace RSLib.Jumble.Flock
{
    using UnityEngine;

    /// <summary>
    /// Stay In Radius behaviour makes the flock agents stay in a range around the flock center.
    /// </summary>
    [CreateAssetMenu(fileName = "New Stay In Radius Flock Behaviour", menuName = "RSLib/Flock/Behaviour/Stay In Radius")]
    public class FlockBehaviourStayInRadius : FlockBehaviour
    {
        [SerializeField, Min(0f)] private float _radius = 15f;
        [SerializeField, Range(0f, 0.9f)] private float _minCenterDistancePercentage = 0.9f;

        public override Vector2 ComputeVelocity(FlockAgent agent, System.Collections.Generic.List<Transform> context, Flock flock)
        {
            Vector2 centerOffset = flock.Center.position - agent.transform.position;
            float percentage = centerOffset.magnitude / _radius;

            if (percentage < _minCenterDistancePercentage)
                return Vector2.zero;

            return centerOffset * percentage * percentage;
        }
    }
}