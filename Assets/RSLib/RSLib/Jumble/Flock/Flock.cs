namespace RSLib.Jumble.Flock
{
    using System.Collections.Generic;
    using UnityEngine;

    public class Flock : MonoBehaviour
    {
        private const float AGENT_DENSITY = 0.08f;

        [Header("REFS")] 
        [SerializeField] private FlockAgent _flockAgentPrefab = null;
        [SerializeField] private FlockBehaviour _flockBehaviour = null;
        [SerializeField] private Transform _center = null;
        [SerializeField] private FlockAgent[] _startingAgents = null;
        
        [Header("AGENTS BEHAVIOUR")]
        [SerializeField, Range(0, 500)] private int _startingCount = 100;
        [SerializeField, Range(1f, 100f)] private float _driveFactor = 10f;
        [SerializeField, Range(1f, 100f)] private float _maxSpeed = 5f;
        [SerializeField, Range(1f, 10f)] private float _neighbourRadius = 1.5f;
        [SerializeField, Range(0f, 1f)] private float _avoidanceRadiusMultiplier = 0.5f;

        private List<FlockAgent> _agents = new List<FlockAgent>();
        private List<Transform> _context;

        public float AvoidanceRadius => _neighbourRadius * _avoidanceRadiusMultiplier;

        public Transform Center => _center;

        private List<Transform> GetContext(FlockAgent agent)
        {
            List<Transform> context = new List<Transform>();
            Collider2D[] nearbyColliders = Physics2D.OverlapCircleAll(agent.transform.position, _neighbourRadius);

            for (int i = nearbyColliders.Length - 1; i >= 0; --i)
                if (nearbyColliders[i] != agent.Collider2D)
                    context.Add(nearbyColliders[i].transform);

            return context;
        }

        private int GetContextNonAlloc(FlockAgent agent, List<Transform> context)
        {
            Collider2D[] nearbyColliders = Physics2D.OverlapCircleAll(agent.transform.position, _neighbourRadius);

            for (int i = nearbyColliders.Length - 1; i >= 0; --i)
                if (nearbyColliders[i] != agent.Collider2D)
                    context.Add(nearbyColliders[i].transform);

            return context.Count;
        }

        private void Start()
        {
            _context = new List<Transform>(_startingCount + _startingAgents.Length);

            for (int i = 0; i < _startingCount; ++i)
            {
                Vector2 position = Random.insideUnitCircle * _startingCount * AGENT_DENSITY;
                Quaternion rotation = Quaternion.Euler(Vector3.forward * Random.Range(0f, 360f));
                FlockAgent agentInstance = Instantiate(_flockAgentPrefab, position, rotation, transform);

                agentInstance.Init(this);
                agentInstance.name = $"FlockAgent_{i}";
                _agents.Add(agentInstance);
            }
            
            for (int i = 0; i < _startingAgents.Length; ++i)
            {
                _startingAgents[i].Init(this);
                _agents.Add(_startingAgents[i]);
            }
        }

        private void Update()
        {
            for (int i = _agents.Count - 1; i >= 0; --i)
            {
                _context.Clear();

                FlockAgent agent = _agents[i];
                GetContextNonAlloc(agent, _context);

                // agent.GetComponentInChildren<SpriteRenderer>().color =  Color.Lerp(Color.white, Color.magenta, _context.Count / 10f); // Debug visualizer.

                Vector2 velocity = _flockBehaviour.ComputeVelocity(agent, _context, this);
                velocity *= _driveFactor;
                if (velocity.sqrMagnitude > _maxSpeed * _maxSpeed)
                    velocity = velocity.normalized * _maxSpeed;

                agent.OnVelocityComputed(velocity);
            }
        }
    }
}