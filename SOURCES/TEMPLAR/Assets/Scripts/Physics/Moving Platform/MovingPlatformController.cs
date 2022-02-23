namespace Templar.Physics.MovingPlatform
{
    using UnityEngine;
#if UNITY_EDITOR
    using UnityEditor;
#endif

    [DisallowMultipleComponent]
    public class MovingPlatformController : MonoBehaviour
    {
        [System.Serializable]
        public class PlatformResetDatas
        {
            [SerializeField] private MovingPlatformController _platform = null;
            [SerializeField, Min(0)] private int _waypointIndex = 0;
            [SerializeField, Range(0f, 1f)] private float _percentage = 0f;
            [SerializeField] private bool _resetCycleDirection = false;

            public MovingPlatformController Platform => _platform;
            public int WaypointIndex => _waypointIndex;
            public float Percentage => _percentage;
            public bool ResetCycleDirection => _resetCycleDirection;

            public void ResetPlatform()
            {
                if (Platform.MovementTriggered)
                    Platform.ResetPlatform(this);
            }
        }

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

        [Header("TRIGGER")]
        [SerializeField] private Templar.Tools.OptionalTriggerableObject _movementTrigger = new Templar.Tools.OptionalTriggerableObject(null, false);
        [SerializeField] private RSLib.Framework.OptionalFloat _movementTriggerDelay = new RSLib.Framework.OptionalFloat(0f, false);

        [Header("DEBUG")]
        [SerializeField] private RSLib.DataColor _dbgColor = null;
        [SerializeField] private RSLib.DataColor _raycastsColor = null;
        [SerializeField] private bool _verbose = false;
        [SerializeField] private RSLib.DataColor _dbgResetDatasColor = null;
        [SerializeField] public PlatformResetDatas _dbgResetDatas = new PlatformResetDatas();

        private RaycastsController _raycastsCtrl;

        public bool MovementTriggered { get; private set; }

        private int _fromWaypointIndex;
        private int _toWaypointIndex;
        private float _currWaypointsPercentage;
        private bool _onWaypointPause;
        private bool _cycleReversed;

        private static System.Collections.Generic.Dictionary<Collider2D, IMovingPlatformPassenger> s_alreadyKnownPassengers = new System.Collections.Generic.Dictionary<Collider2D, IMovingPlatformPassenger>();

        private System.Collections.Generic.HashSet<IMovingPlatformPassenger> _movedPassengers = new System.Collections.Generic.HashSet<IMovingPlatformPassenger>();
        private System.Collections.Generic.List<PassengerVelocity> _passengersVelocities = new System.Collections.Generic.List<PassengerVelocity>();

        public void ResetPlatform()
        {
            _currWaypointsPercentage = _initPercentage.Enabled ? _initPercentage.Value : 0f;
            _fromWaypointIndex = _initFromIndex.Enabled ? _initFromIndex.Value : 0;
            _toWaypointIndex = 0;
        }

        public void ResetPlatform(PlatformResetDatas resetDatas)
        {
            _currWaypointsPercentage = resetDatas.Percentage;

            if (_cycleReversed && resetDatas.ResetCycleDirection)
                ReverseWaypoints();

            _fromWaypointIndex = resetDatas.WaypointIndex;
            _toWaypointIndex = (_fromWaypointIndex + 1) % _waypoints.PathLength;
        }

        private void OnMovementTriggerTriggered(Triggerables.TriggerableObject.TriggerEventArgs args)
        {
            if (_movementTriggerDelay.Enabled && _movementTriggerDelay.Value > 0f)
                StartCoroutine(TriggerMovementCoroutine());
            else
                MovementTriggered = true;
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
                _waypoints.GetGlobalWaypointAt(_fromWaypointIndex),
                _waypoints.GetGlobalWaypointAt(_toWaypointIndex),
                _waypoints.GetEasedPercentage(_currWaypointsPercentage));

            if (_currWaypointsPercentage >= 1f)
            {
                _currWaypointsPercentage = 0;
                _fromWaypointIndex++;

                if (!_waypoints.Cyclic && _fromWaypointIndex == _waypoints.PathLength - 1)
                {
                    _fromWaypointIndex = 0;
                    ReverseWaypoints();
                }

                if (_waypoints.PauseDur.Enabled)
                    StartCoroutine(WaypointPauseCoroutine());
            }

            return nextPos - transform.position;
        }

        private void ReverseWaypoints()
        {
            _waypoints.Reverse();
            _cycleReversed = !_cycleReversed;
        }

        private void ComputePassengersVelocity(Vector3 vel)
        {
            _movedPassengers.Clear();
            _passengersVelocities.Clear();

            float signX = Mathf.Sign(vel.x);
            float signY = Mathf.Sign(vel.y);

            // NOTE: RaycastAll doesn't seem to care about max distance so manuel checks on RaycastHit2D.distance are added.
            // KnP bug entry: https://app.hacknplan.com/p/148469/kanban?categoryId=8&boardId=392835&taskId=68&tabId=basicinfo.

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
                        if (hit && hit.distance <= length)
                        {
                            if (!s_alreadyKnownPassengers.TryGetValue(hit.collider, out IMovingPlatformPassenger passenger))
                                if (hit.collider.TryGetComponent(out passenger))
                                    s_alreadyKnownPassengers.Add(hit.collider, passenger);

                            if (passenger == null || _movedPassengers.Contains(passenger))
                                continue;

                            LogAddedPassenger(hit);

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
                        if (hit && hit.distance <= length)
                        {
                            if (!s_alreadyKnownPassengers.TryGetValue(hit.collider, out IMovingPlatformPassenger passenger))
                                if (hit.collider.TryGetComponent(out passenger))
                                    s_alreadyKnownPassengers.Add(hit.collider, passenger);

                            if (passenger == null || _movedPassengers.Contains(passenger))
                                continue;

                            LogAddedPassenger(hit);

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
                        if (hit && hit.distance <= length)
                        {
                            if (!s_alreadyKnownPassengers.TryGetValue(hit.collider, out IMovingPlatformPassenger passenger))
                                if (hit.collider.TryGetComponent(out passenger))
                                    s_alreadyKnownPassengers.Add(hit.collider, passenger);

                            if (passenger == null || _movedPassengers.Contains(passenger))
                                continue;

                            LogAddedPassenger(hit);

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
            if (_verbose)
                CProLogger.Log(this, $"{transform.name} pause.", gameObject);

            _onWaypointPause = true;
            yield return RSLib.Yield.SharedYields.WaitForSeconds(_waypoints.PauseDur.Value);
            _onWaypointPause = false;
        }

        private System.Collections.IEnumerator TriggerMovementCoroutine()
        {
            yield return RSLib.Yield.SharedYields.WaitForSeconds(_movementTriggerDelay.Value);
            MovementTriggered = true;
        }

        private void Awake()
        {
            ResetPlatform();

            _raycastsCtrl = new RaycastsController(_boxCollider2D);
            MovementTriggered = !_movementTrigger.Enabled || _movementTrigger.Value == null; // [TODO] Or if loading a platform already triggered.
        
            if (_movementTrigger.Enabled && _movementTrigger.Value != null)
                _movementTrigger.Value.Triggered += OnMovementTriggerTriggered;
        }

        private void Update()
        {
            if (!MovementTriggered)
                return;

            _raycastsCtrl.ComputeRaycastOrigins();

            Vector3 vel = ComputePlatformVelocity();
            ComputePassengersVelocity(vel / Time.deltaTime);

            ApplyPassengersVelocity(true);
            transform.Translate(vel);
            ApplyPassengersVelocity(false);
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = _dbgColor?.Color ?? RSLib.DataColor.Default;

            if (_movementTrigger.Enabled && _movementTrigger.Value != null)
                Gizmos.DrawLine(transform.position, _movementTrigger.Value.transform.position);

            if (_initFromIndex.Enabled || _initPercentage.Enabled)
            {
                DrawWaypointsStartGizmos(_initFromIndex.Enabled ? _initFromIndex.Value : 0,
                    _initPercentage.Enabled ? _initPercentage.Value : 0f,
                    _dbgColor?.Color ?? RSLib.DataColor.Default);
            }

            DrawWaypointsStartGizmos(
                _dbgResetDatas.WaypointIndex,
                _dbgResetDatas.Percentage,
                _dbgResetDatasColor?.Color ?? _dbgColor?.Color ?? RSLib.DataColor.Default);
        }

        public void DrawWaypointsStartGizmos(int fromIndex, float percentage, Color? color = null)
        {
            if (Application.isPlaying)
                return;

            Gizmos.color = color ?? RSLib.DataColor.Default;

            Vector3 initPointPos = _waypoints.GetLocalWaypointAt(fromIndex);
            if (percentage != 0f)
            {
                Vector3 initNextPointPos = _waypoints.GetLocalWaypointAt((fromIndex + 1) % _waypoints.PathLength);
                initPointPos += (initNextPointPos - initPointPos) * percentage;
            }

            Gizmos.DrawWireSphere(transform.position + initPointPos, 0.2f);
        }

        private void LogAddedPassenger(RaycastHit2D hit)
        {
            if (_verbose)
                CProLogger.Log(this, $"{transform.name} adding passenger {hit.collider.transform.name} (hit distance: {hit.distance}).", gameObject);
        }

        private void OnValidate()
        {
            _initPercentage = new RSLib.Framework.OptionalFloat(Mathf.Clamp01(_initPercentage.Value), _initPercentage.Enabled);
            _initFromIndex = new RSLib.Framework.OptionalInt(Mathf.Max(0, _initFromIndex.Value), _initFromIndex.Enabled);
        }
    }

#if UNITY_EDITOR
    [CustomEditor(typeof(MovingPlatformController))]
    public class MovingPlatformControllerEditor : RSLib.EditorUtilities.ButtonProviderEditor<MovingPlatformController>
    {
        protected override void DrawButtons()
        {
            DrawButton("Reset Platform", Obj.ResetPlatform);
            DrawButton("Reset Platform With Debug Datas", () => Obj.ResetPlatform(Obj._dbgResetDatas));
        }
    }
#endif
}