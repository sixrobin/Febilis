namespace Templar.Physics.MovingPlatform
{
    using UnityEngine;

    public class MovingPlatformWaypoints : MonoBehaviour
    {
        [SerializeField] private Vector2[] _wayPositions = null;
        [SerializeField] private RSLib.Framework.OptionalCurve _curve = new RSLib.Framework.OptionalCurve(RSLib.Maths.Curve.Linear, false);
        [SerializeField] private RSLib.Framework.OptionalFloat _pauseDur = new RSLib.Framework.OptionalFloat(0.3f);
        [SerializeField] private bool _cyclic = false;

        [Header("DEBUG")]
        [SerializeField] private RSLib.Data.Color _dbgColor = null;
        [SerializeField] private float _waypointsGizmosRadius = 0.3f;

        private Vector3[] _globalWayPositions;

        public bool Cyclic => _cyclic;
        public RSLib.Framework.OptionalCurve Curve => _curve;
        public RSLib.Framework.OptionalFloat PauseDur => _pauseDur;
        public int PathLength => Application.isPlaying ? _globalWayPositions.Length : _wayPositions.Length;

        public Vector3 GetLocalWaypointAt(int index)
        {
            return _wayPositions[index];
        }

        public Vector3 GetGlobalWaypointAt(int index)
        {
            return _globalWayPositions[index];
        }

        public float GetEasedPercentage(float value)
        {
            return !Curve.Enabled ? value : RSLib.Maths.Easing.Ease(value, Curve.Value);
        }

        public float DistanceBetweenWaypoints(int firstIndex, int secondIndex)
        {
            return (_globalWayPositions[firstIndex] - _globalWayPositions[secondIndex]).magnitude;
        }

        public void Reverse()
        {
            System.Array.Reverse(_globalWayPositions);
        }

        private void InitGlobalWaypoints()
        {
            _globalWayPositions = new Vector3[_wayPositions.Length];
            for (int i = 0; i < _globalWayPositions.Length; ++i)
                _globalWayPositions[i] = (Vector3)_wayPositions[i] + transform.position;
        }

        private void Awake()
        {
            InitGlobalWaypoints();
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = _dbgColor ?? RSLib.Data.Color.Default;

            if (_wayPositions != null)
            {
                for (int i = _wayPositions.Length - 1; i >= 0; --i)
                {
                    Vector3 globalPos = Application.isPlaying ? _globalWayPositions[i] : (Vector3)_wayPositions[i] + transform.position;
                    Gizmos.DrawWireSphere(globalPos, _waypointsGizmosRadius);
                }

                if (Application.isPlaying)
                    RSLib.Debug.GizmosUtilities.DrawVectorsPath(_globalWayPositions, Cyclic);
                else
                    RSLib.Debug.GizmosUtilities.DrawVectorsPath(_wayPositions, transform.position, Cyclic);
            }
        }
    }
}