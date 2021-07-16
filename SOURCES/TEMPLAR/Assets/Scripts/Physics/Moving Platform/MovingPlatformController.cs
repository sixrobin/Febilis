namespace Templar.Physics.MovingPlatform
{
    using UnityEngine;
#if UNITY_EDITOR
    using UnityEditor;
#endif

    [DisallowMultipleComponent]
    public class MovingPlatformController : MonoBehaviour
    {
        private struct PassengerVelocity
        {
            public PassengerVelocity(IMovingPlatformPassenger passenger, Vector3 vel, bool standingOnPlatform, bool moveBeforePlatform)
            {
                Passenger = passenger;
                Velocity = vel;
                StandingOnPlatform = standingOnPlatform;
                MoveBeforePlatform = moveBeforePlatform;
            }
            
            public IMovingPlatformPassenger Passenger { get; private set; }
            public Vector3 Velocity { get; private set; }
            public bool StandingOnPlatform { get; private set; }
            public bool MoveBeforePlatform { get; private set; }
        }

        [SerializeField] private BoxCollider2D _boxCollider2D = null;
        [SerializeField] private LayerMask _passengersMask = 0;
        [SerializeField] private MovingPlatformWaypoints _waypoints = null;
        [SerializeField] private float _speed = 5f;
        [SerializeField, Tooltip("Should be checked if objects are placed on the platform.")] private bool _raycastAll = false;

        [Header("INIT VALUES")]
        [SerializeField] private RSLib.Framework.OptionalFloat _initPercentage = new RSLib.Framework.OptionalFloat(0f, false);
        [SerializeField] private RSLib.Framework.OptionalInt _initFromIndex = new RSLib.Framework.OptionalInt(0, false);

        [Header("DEBUG")]
        [SerializeField] private RSLib.DataColor _raycastsColor = null;

        private RaycastsController _raycastsCtrl;

        private int _fromWaypointIndex;
        private int _toWaypointIndex;
        private float _currWaypointsPercentage;
        private bool _onWaypointPause;

        private static System.Collections.Generic.Dictionary<Collider2D, IMovingPlatformPassenger> s_alreadyKnownPassengers = new System.Collections.Generic.Dictionary<Collider2D, IMovingPlatformPassenger>();

        private System.Collections.Generic.HashSet<IMovingPlatformPassenger> _movedPassengers = new System.Collections.Generic.HashSet<IMovingPlatformPassenger>();
        private System.Collections.Generic.List<PassengerVelocity> _passengersVelocities = new System.Collections.Generic.List<PassengerVelocity>();

        public void ResetPlatform()
        {
            _currWaypointsPercentage = _initPercentage.Enabled ? _initPercentage.Value : 0f;
            _fromWaypointIndex = _initFromIndex.Enabled ? _initFromIndex.Value : 0;
            _toWaypointIndex = 0;
        }

        private Vector3 ComputePlatformVelocity()
        {
            if (_onWaypointPause)
                return Vector3.zero;

            _fromWaypointIndex %= _waypoints.PathLength;
            _toWaypointIndex = (_fromWaypointIndex + 1) % _waypoints.PathLength;

            float waypointsDist = _waypoints.DistanceBetweenWaypoints(_fromWaypointIndex, _toWaypointIndex);
            _currWaypointsPercentage += Mathf.Clamp01(_speed * Time.deltaTime / waypointsDist);

            Vector3 nextPos = Vector3.Lerp(
                _waypoints.GetPointAt(_fromWaypointIndex),
                _waypoints.GetPointAt(_toWaypointIndex),
                _waypoints.GetEasedPercentage(_currWaypointsPercentage));

            if (_currWaypointsPercentage >= 1f)
            {
                _currWaypointsPercentage = 0;
                _fromWaypointIndex++;

                if (!_waypoints.Cyclic && _fromWaypointIndex == _waypoints.PathLength - 1)
                {
                    _fromWaypointIndex = 0;
                    _waypoints.Reverse();
                }

                if (_waypoints.PauseDur.Enabled)
                    StartCoroutine(WaypointPauseCoroutine());
            }

            return nextPos - transform.position;
        }

        private void ComputePassengersVelocity(Vector3 vel)
        {
            _movedPassengers.Clear();
            _passengersVelocities.Clear();

            float signX = Mathf.Sign(vel.x);
            float signY = Mathf.Sign(vel.y);

            if (vel.y != 0f)
            {
                float length = Mathf.Abs(vel.y) * Time.deltaTime + RaycastsController.SKIN_WIDTH;

                for (int i = 0; i < _raycastsCtrl.VerticalRaycastsCount; ++i)
                {
                    Vector2 rayOrigin = (signY == 1f ? _raycastsCtrl.RaycastsOrigins.TopLeft : _raycastsCtrl.RaycastsOrigins.BottomLeft) + Vector2.right * i * _raycastsCtrl.VerticalRaycastsSpacing;
                    RaycastHit2D[] hits = _raycastAll
                        ? Physics2D.RaycastAll(rayOrigin, Vector2.up * signY, length, _passengersMask)
                        : (new RaycastHit2D[1] { Physics2D.Raycast(rayOrigin, Vector2.up * signY, length, _passengersMask) });

                    for (int j = hits.Length - 1; j >= 0; --j)
                    {
                        RaycastHit2D hit = hits[j];
                        if (hit)
                        {
                            if (!s_alreadyKnownPassengers.TryGetValue(hit.collider, out IMovingPlatformPassenger passenger))
                                if (hit.collider.TryGetComponent(out passenger))
                                    s_alreadyKnownPassengers.Add(hit.collider, passenger);

                            if (passenger == null || _movedPassengers.Contains(passenger))
                                continue;

                            _movedPassengers.Add(passenger);

                            float pushX = signY == 1f ? vel.x : 0f;
                            float pushY = vel.y - (hit.distance - RaycastsController.SKIN_WIDTH) * signY;
                            _passengersVelocities.Add(new PassengerVelocity(passenger, new Vector3(pushX, pushY), signY == 1f, true));
                        }
                    }
                }
            }

            if (vel.x != 0f)
            {
                float length = Mathf.Abs(vel.x) * Time.deltaTime + RaycastsController.SKIN_WIDTH;

                for (int i = 0; i < _raycastsCtrl.HorizontalRaycastsCount; ++i)
                {
                    Vector2 rayOrigin = (signX == 1f ? _raycastsCtrl.RaycastsOrigins.BottomRight : _raycastsCtrl.RaycastsOrigins.BottomLeft) + Vector2.up * i * _raycastsCtrl.HorizontalRaycastsSpacing;
                    RaycastHit2D[] hits = _raycastAll
                        ? Physics2D.RaycastAll(rayOrigin, Vector2.right * signX, length, _passengersMask)
                        : (new RaycastHit2D[1] { Physics2D.Raycast(rayOrigin, Vector2.right * signX, length, _passengersMask) });

                    Debug.DrawLine(rayOrigin, rayOrigin + length * signX * Vector2.right, _raycastsColor?.Color ?? RSLib.DataColor.Default);

                    for (int j = hits.Length - 1; j >= 0; --j)
                    {
                        RaycastHit2D hit = hits[j];
                        if (hit)
                        {
                            if (!s_alreadyKnownPassengers.TryGetValue(hit.collider, out IMovingPlatformPassenger passenger))
                                if (hit.collider.TryGetComponent(out passenger))
                                    s_alreadyKnownPassengers.Add(hit.collider, passenger);

                            if (passenger == null || _movedPassengers.Contains(passenger))
                                continue;

                            _movedPassengers.Add(passenger);

                            float pushX = vel.x - (hit.distance - RaycastsController.SKIN_WIDTH) * signX;
                            _passengersVelocities.Add(new PassengerVelocity(passenger, new Vector3(pushX, 0f), false, true));
                        }
                    }
                }
            }

            // Passenger on horizontally or downward moving platform.
            if (signY == -1 || vel.y == 0f && vel.x != 0f)
            {
                float length = RaycastsController.SKIN_WIDTH * 2;

                for (int i = 0; i < _raycastsCtrl.VerticalRaycastsCount; ++i)
                {
                    Vector2 rayOrigin = _raycastsCtrl.RaycastsOrigins.TopLeft + _raycastsCtrl.VerticalRaycastsSpacing * i * Vector2.right;
                    RaycastHit2D[] hits = _raycastAll
                        ? Physics2D.RaycastAll(rayOrigin, Vector2.up, length, _passengersMask)
                        : (new RaycastHit2D[1] { Physics2D.Raycast(rayOrigin, Vector2.up, length, _passengersMask) });

                    for (int j = hits.Length - 1; j >= 0; --j)
                    {
                        RaycastHit2D hit = hits[j];
                        if (hit)
                        {
                            if (!s_alreadyKnownPassengers.TryGetValue(hit.collider, out IMovingPlatformPassenger passenger))
                                if (hit.collider.TryGetComponent(out passenger))
                                    s_alreadyKnownPassengers.Add(hit.collider, passenger);

                            if (passenger == null || _movedPassengers.Contains(passenger))
                                continue;

                            _movedPassengers.Add(passenger);
                            _passengersVelocities.Add(new PassengerVelocity(passenger, vel, true, false));
                        }
                    }
                }
            }
        }

        private void ApplyPassengersVelocity(bool moveBeforePlatform)
        {
            foreach (PassengerVelocity passengerVel in _passengersVelocities)
                if (passengerVel.MoveBeforePlatform == moveBeforePlatform)
                    passengerVel.Passenger.OnPlatformMoved(passengerVel.Velocity, passengerVel.StandingOnPlatform);
        }

        private System.Collections.IEnumerator WaypointPauseCoroutine()
        {
            _onWaypointPause = true;
            yield return RSLib.Yield.SharedYields.WaitForSeconds(_waypoints.PauseDur.Value);
            _onWaypointPause = false;
        }

        private void Awake()
        {
            ResetPlatform();
            _raycastsCtrl = new RaycastsController(_boxCollider2D);
        }

        private void Update()
        {
            _raycastsCtrl.ComputeRaycastOrigins();

            Vector3 vel = ComputePlatformVelocity();
            ComputePassengersVelocity(vel / Time.deltaTime);

            ApplyPassengersVelocity(true);
            transform.Translate(vel);
            ApplyPassengersVelocity(false);
        }
    }

#if UNITY_EDITOR
    [CustomEditor(typeof(MovingPlatformController))]
    public class MovingPlatformControllerEditor : RSLib.EditorUtilities.ButtonProviderEditor<MovingPlatformController>
    {
        protected override void DrawButtons()
        {
            DrawButton("Reset Platform", Obj.ResetPlatform);
        }
    }
#endif
}