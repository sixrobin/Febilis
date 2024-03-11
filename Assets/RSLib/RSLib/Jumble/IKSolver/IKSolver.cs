namespace RSLib.Jumble.IKSolver
{
    using UnityEngine;

    /// <summary>
    /// Attached to a gameObject to make it an IK system target.
    /// Solves the IK using specified pivots and pole.
    /// </summary>
    public class IKSolver : MonoBehaviour
    {
        [System.Serializable]
        public class IKBone
        {
            public IKBone(Transform pivot)
            {
                Pivot = pivot;
            }

            public float Length { get; set; }
            public Transform Pivot { get; set; }

            /// <summary>
            /// Sets the bone pivot position.
            /// </summary>
            /// <param name="target">The pivot new position.</param>
            public void AttachTo(Vector3 target)
            {
                Pivot.position = target;
            }

            /// <summary>
            /// Aligns the bone's up vector to a target.
            /// </summary>
            /// <param name="target">The target to make the bone look.</param>
            public void LookAt(Vector3 target)
            {
                Pivot.up = Pivot.position - target;
            }
        }

        [Header("FROM LEAF TO ROOT")]
        [SerializeField] private Transform[] _pivots = null;
        [Space(8)]
        [SerializeField] private Transform _iKEnd = null;
        [SerializeField] private Transform _iKPole = null;
        [SerializeField] private int _iterations = 5;

        [Header("DEBUG")]
        [SerializeField] private bool _showDebug = true;

        private IKBone[] _bones;

        /// <summary>
        /// Replaces every bone pivot with instantiated objects to have the right up vector.
        /// Calculates bones lengths using vectors magnitudes.
        /// </summary>
        private void Init()
        {
            _bones = new IKBone[_pivots.Length];
            for (int i = 0; i < _bones.Length; ++i)
                _bones[i] = new IKBone(_pivots[i]);

            for (int i = 0; i < _bones.Length; ++i)
            {
                _bones[i].Length = ((i == 0 ? _iKEnd.position : _bones[i - 1].Pivot.position) - _bones[i].Pivot.position).magnitude;

                Transform g = new GameObject { name = _bones[i].Pivot.name + "_IKRoot" }.transform;
                g.position = _bones[i].Pivot.position;
                g.up = -((i == 0 ? _iKEnd.position : _bones[i - 1].Pivot.position) - _bones[i].Pivot.position);
                g.parent = _bones[i].Pivot.parent;
                _bones[i].Pivot.parent = g;
                _bones[i].Pivot = g;
            }
        }

        /// <summary>
        /// Solves the IK system, calculating each bone position and up vector direction.
        /// First aligns the complete IK to the pole, then attaches every bone to its destination (leaf's destination
        /// is the IK target) to get the right up vectors. Then re-attaches to the root.
        /// </summary>
        public void Solve()
        {
            Vector3 rootPoint = _bones[_bones.Length - 1].Pivot.position;

            _bones[_bones.Length - 1].LookAt(_iKPole.position);
            for (int i = _bones.Length - 2; i >= 0; --i)
            {
                _bones[i].AttachTo(_bones[i + 1].Pivot.position - (_bones[i + 1].Pivot.up * _bones[i + 1].Length));
                _bones[i].LookAt(_iKPole.position);
            }

            for (int i = 0; i < _iterations; ++i)
            {
                _bones[0].LookAt(transform.position);
                _bones[0].AttachTo(transform.position + _bones[0].Pivot.up * _bones[0].Length);

                for (int j = 1; j < _bones.Length; ++j)
                {
                    _bones[j].LookAt(_bones[j - 1].Pivot.position);
                    _bones[j].AttachTo(_bones[j - 1].Pivot.position + _bones[j].Pivot.up * _bones[j].Length);
                }

                _bones[_bones.Length - 1].AttachTo(rootPoint);
                for (int j = _bones.Length - 2; j >= 0; --j)
                    _bones[j].AttachTo(_bones[j + 1].Pivot.position - _bones[j + 1].Pivot.up * _bones[j + 1].Length);
            }
        }

        private void Awake()
        {
            Init();
        }

        private void Update()
        {
            Solve();
        }

        private void OnDrawGizmos()
        {
            if (!_showDebug)
                return;

            Gizmos.color = Color.blue;
            Gizmos.DrawLine(_pivots[_pivots.Length - 1].position, _iKEnd.position);

            Gizmos.color = Color.cyan;
            for (int i = 0; i < _pivots.Length; ++i)
                Gizmos.DrawLine(_pivots[i].position, i == 0 ? _iKEnd.position : _pivots[i - 1].position);
        }
    }
}